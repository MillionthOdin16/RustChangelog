using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class CoverageQueries : MonoBehaviour
{
	public class BufferSet
	{
		public int width = 0;

		public int height = 0;

		public Texture2D inputTexture = null;

		public RenderTexture resultTexture = null;

		public Color[] inputData = (Color[])(object)new Color[0];

		public Color32[] resultData = (Color32[])(object)new Color32[0];

		private Material coverageMat = null;

		private const int MaxAsyncGPUReadbackRequests = 10;

		private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();

		public void Attach(Material coverageMat)
		{
			this.coverageMat = coverageMat;
		}

		public void Dispose(bool data = true)
		{
			if ((Object)(object)inputTexture != (Object)null)
			{
				Object.DestroyImmediate((Object)(object)inputTexture);
				inputTexture = null;
			}
			if ((Object)(object)resultTexture != (Object)null)
			{
				RenderTexture.active = null;
				resultTexture.Release();
				Object.DestroyImmediate((Object)(object)resultTexture);
				resultTexture = null;
			}
			if (data)
			{
				inputData = (Color[])(object)new Color[0];
				resultData = (Color32[])(object)new Color32[0];
			}
		}

		public bool CheckResize(int count)
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Expected O, but got Unknown
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Expected O, but got Unknown
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0158: Unknown result type (might be due to invalid IL or missing references)
			if (count > inputData.Length || ((Object)(object)resultTexture != (Object)null && !resultTexture.IsCreated()))
			{
				Dispose(data: false);
				width = Mathf.CeilToInt(Mathf.Sqrt((float)count));
				height = Mathf.CeilToInt((float)count / (float)width);
				inputTexture = new Texture2D(width, height, (TextureFormat)20, false, true);
				((Object)inputTexture).name = "_Input";
				((Texture)inputTexture).filterMode = (FilterMode)0;
				((Texture)inputTexture).wrapMode = (TextureWrapMode)1;
				resultTexture = new RenderTexture(width, height, 0, (RenderTextureFormat)0, (RenderTextureReadWrite)1);
				((Object)resultTexture).name = "_Result";
				((Texture)resultTexture).filterMode = (FilterMode)0;
				((Texture)resultTexture).wrapMode = (TextureWrapMode)1;
				resultTexture.useMipMap = false;
				resultTexture.Create();
				int num = resultData.Length;
				int num2 = width * height;
				Array.Resize(ref inputData, num2);
				Array.Resize(ref resultData, num2);
				Color32 val = default(Color32);
				((Color32)(ref val))._002Ector(byte.MaxValue, (byte)0, (byte)0, (byte)0);
				for (int i = num; i < num2; i++)
				{
					resultData[i] = val;
				}
				return true;
			}
			return false;
		}

		public void UploadData()
		{
			if (inputData.Length != 0)
			{
				inputTexture.SetPixels(inputData);
				inputTexture.Apply();
			}
		}

		public void Dispatch(int count)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			if (inputData.Length != 0)
			{
				RenderBuffer activeColorBuffer = Graphics.activeColorBuffer;
				RenderBuffer activeDepthBuffer = Graphics.activeDepthBuffer;
				coverageMat.SetTexture("_Input", (Texture)(object)inputTexture);
				Graphics.Blit((Texture)(object)inputTexture, resultTexture, coverageMat, 0);
				Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
			}
		}

		public void IssueRead()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (asyncRequests.Count < 10)
			{
				asyncRequests.Enqueue(AsyncGPUReadback.Request((Texture)(object)resultTexture, 0, (Action<AsyncGPUReadbackRequest>)null));
			}
		}

		public void GetResults()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			if (resultData.Length == 0)
			{
				return;
			}
			while (asyncRequests.Count > 0)
			{
				AsyncGPUReadbackRequest val = asyncRequests.Peek();
				if (((AsyncGPUReadbackRequest)(ref val)).hasError)
				{
					asyncRequests.Dequeue();
					continue;
				}
				if (((AsyncGPUReadbackRequest)(ref val)).done)
				{
					NativeArray<Color32> data = ((AsyncGPUReadbackRequest)(ref val)).GetData<Color32>(0);
					for (int i = 0; i < data.Length; i++)
					{
						resultData[i] = data[i];
					}
					asyncRequests.Dequeue();
					continue;
				}
				break;
			}
		}
	}

	public enum RadiusSpace
	{
		ScreenNormalized,
		World
	}

	public class Query
	{
		public struct Input
		{
			public Vector3 position;

			public RadiusSpace radiusSpace;

			public float radius;

			public int sampleCount;

			public float smoothingSpeed;
		}

		public struct Internal
		{
			public int id;

			public void Reset()
			{
				id = -1;
			}
		}

		public struct Result
		{
			public int passed;

			public float coverage;

			public float smoothCoverage;

			public float weightedCoverage;

			public float weightedSmoothCoverage;

			public bool originOccluded;

			public int frame;

			public float originVisibility;

			public float originSmoothVisibility;

			public void Reset()
			{
				passed = 0;
				coverage = 0f;
				smoothCoverage = 0f;
				weightedCoverage = 0f;
				weightedSmoothCoverage = 0f;
				originOccluded = true;
				frame = -1;
				originVisibility = 0f;
				originSmoothVisibility = 0f;
			}
		}

		public Input input = default(Input);

		public Internal intern = default(Internal);

		public Result result = default(Result);

		public bool IsRegistered => intern.id >= 0;
	}

	public bool debug = false;

	public float depthBias = -0.1f;
}
