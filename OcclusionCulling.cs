using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RustNative;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class OcclusionCulling : MonoBehaviour
{
	public class BufferSet
	{
		public ComputeBuffer inputBuffer = null;

		public ComputeBuffer resultBuffer = null;

		public int width = 0;

		public int height = 0;

		public int capacity = 0;

		public int count = 0;

		public Texture2D inputTexture = null;

		public RenderTexture resultTexture = null;

		public Texture2D resultReadTexture = null;

		public Color[] inputData = (Color[])(object)new Color[0];

		public Color32[] resultData = (Color32[])(object)new Color32[0];

		private OcclusionCulling culling;

		private const int MaxAsyncGPUReadbackRequests = 10;

		private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();

		public IntPtr readbackInst = IntPtr.Zero;

		public bool Ready => resultData.Length != 0;

		public void Attach(OcclusionCulling culling)
		{
			this.culling = culling;
		}

		public void Dispose(bool data = true)
		{
			if (inputBuffer != null)
			{
				inputBuffer.Dispose();
				inputBuffer = null;
			}
			if (resultBuffer != null)
			{
				resultBuffer.Dispose();
				resultBuffer = null;
			}
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
			if ((Object)(object)resultReadTexture != (Object)null)
			{
				Object.DestroyImmediate((Object)(object)resultReadTexture);
				resultReadTexture = null;
			}
			if (readbackInst != IntPtr.Zero)
			{
				BufferReadback.Destroy(readbackInst);
				readbackInst = IntPtr.Zero;
			}
			if (data)
			{
				inputData = (Color[])(object)new Color[0];
				resultData = (Color32[])(object)new Color32[0];
				capacity = 0;
				count = 0;
			}
		}

		public bool CheckResize(int count, int granularity)
		{
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Expected O, but got Unknown
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Expected O, but got Unknown
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Expected O, but got Unknown
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Expected O, but got Unknown
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Expected O, but got Unknown
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Expected I4, but got Unknown
			//IL_027f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0280: Unknown result type (might be due to invalid IL or missing references)
			if (count > capacity || (culling.usePixelShaderFallback && (Object)(object)resultTexture != (Object)null && !resultTexture.IsCreated()))
			{
				Dispose(data: false);
				int num = capacity;
				int num2 = count / granularity * granularity + granularity;
				if (culling.usePixelShaderFallback)
				{
					width = Mathf.CeilToInt(Mathf.Sqrt((float)num2));
					height = Mathf.CeilToInt((float)num2 / (float)width);
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
					resultReadTexture = new Texture2D(width, height, (TextureFormat)5, false, true);
					((Object)resultReadTexture).name = "_ResultRead";
					((Texture)resultReadTexture).filterMode = (FilterMode)0;
					((Texture)resultReadTexture).wrapMode = (TextureWrapMode)1;
					if (!culling.useAsyncReadAPI)
					{
						readbackInst = BufferReadback.CreateForTexture(((Texture)resultTexture).GetNativeTexturePtr(), (uint)width, (uint)height, (uint)(int)resultTexture.format);
					}
					capacity = width * height;
				}
				else
				{
					inputBuffer = new ComputeBuffer(num2, 16);
					resultBuffer = new ComputeBuffer(num2, 4);
					if (!culling.useAsyncReadAPI)
					{
						uint num3 = (uint)(capacity * 4);
						readbackInst = BufferReadback.CreateForBuffer(resultBuffer.GetNativeBufferPtr(), num3);
					}
					capacity = num2;
				}
				Array.Resize(ref inputData, capacity);
				Array.Resize(ref resultData, capacity);
				Color32 val = default(Color32);
				((Color32)(ref val))._002Ector(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				for (int i = num; i < capacity; i++)
				{
					resultData[i] = val;
				}
				this.count = count;
				return true;
			}
			return false;
		}

		public void UploadData()
		{
			if (culling.usePixelShaderFallback)
			{
				inputTexture.SetPixels(inputData);
				inputTexture.Apply();
			}
			else
			{
				inputBuffer.SetData((Array)inputData);
			}
		}

		private int AlignDispatchSize(int dispatchSize)
		{
			return (dispatchSize + 63) / 64;
		}

		public void Dispatch(int count)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			if (culling.usePixelShaderFallback)
			{
				RenderBuffer activeColorBuffer = Graphics.activeColorBuffer;
				RenderBuffer activeDepthBuffer = Graphics.activeDepthBuffer;
				culling.fallbackMat.SetTexture("_Input", (Texture)(object)inputTexture);
				Graphics.Blit((Texture)(object)inputTexture, resultTexture, culling.fallbackMat, 0);
				Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
			}
			else if (inputBuffer != null)
			{
				culling.computeShader.SetBuffer(0, "_Input", inputBuffer);
				culling.computeShader.SetBuffer(0, "_Result", resultBuffer);
				culling.computeShader.Dispatch(0, AlignDispatchSize(count), 1, 1);
			}
		}

		public void IssueRead()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			if (SafeMode)
			{
				return;
			}
			if (culling.useAsyncReadAPI)
			{
				if (asyncRequests.Count < 10)
				{
					AsyncGPUReadbackRequest item = ((!culling.usePixelShaderFallback) ? AsyncGPUReadback.Request(resultBuffer, (Action<AsyncGPUReadbackRequest>)null) : AsyncGPUReadback.Request((Texture)(object)resultTexture, 0, (Action<AsyncGPUReadbackRequest>)null));
					asyncRequests.Enqueue(item);
				}
			}
			else if (readbackInst != IntPtr.Zero)
			{
				BufferReadback.IssueRead(readbackInst);
			}
		}

		public void GetResults()
		{
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			if (resultData == null || resultData.Length == 0)
			{
				return;
			}
			if (!SafeMode)
			{
				if (culling.useAsyncReadAPI)
				{
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
				else if (readbackInst != IntPtr.Zero)
				{
					BufferReadback.GetData(readbackInst, ref resultData[0]);
				}
			}
			else if (culling.usePixelShaderFallback)
			{
				RenderTexture.active = resultTexture;
				resultReadTexture.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
				resultReadTexture.Apply();
				Color32[] pixels = resultReadTexture.GetPixels32();
				Array.Copy(pixels, resultData, resultData.Length);
			}
			else
			{
				resultBuffer.GetData((Array)resultData);
			}
		}
	}

	public enum DebugFilter
	{
		Off,
		Dynamic,
		Static,
		Grid,
		All
	}

	[Flags]
	public enum DebugMask
	{
		Off = 0,
		Dynamic = 1,
		Static = 2,
		Grid = 4,
		All = 7
	}

	[Serializable]
	public class DebugSettings
	{
		public bool log = false;

		public bool showAllVisible = false;

		public bool showMipChain = false;

		public bool showMain = false;

		public int showMainLod = 0;

		public bool showFallback = false;

		public bool showStats = false;

		public bool showScreenBounds = false;

		public DebugMask showMask;

		public LayerMask layerFilter = LayerMask.op_Implicit(-1);
	}

	public class HashedPoolValue
	{
		public ulong hashedPoolKey = ulong.MaxValue;

		public int hashedPoolIndex = -1;
	}

	public class HashedPool<ValueType> where ValueType : HashedPoolValue, new()
	{
		private int granularity;

		private Dictionary<ulong, ValueType> dict;

		private List<ValueType> pool;

		private List<ValueType> list;

		private Queue<ValueType> recycled;

		public int Size => list.Count;

		public int Count => dict.Count;

		public ValueType this[int i]
		{
			get
			{
				return list[i];
			}
			set
			{
				list[i] = value;
			}
		}

		public HashedPool(int capacity, int granularity)
		{
			this.granularity = granularity;
			dict = new Dictionary<ulong, ValueType>(capacity);
			pool = new List<ValueType>(capacity);
			list = new List<ValueType>(capacity);
			recycled = new Queue<ValueType>();
		}

		public void Clear()
		{
			dict.Clear();
			pool.Clear();
			list.Clear();
			recycled.Clear();
		}

		public ValueType Add(ulong key, int capacityGranularity = 16)
		{
			ValueType val;
			if (recycled.Count > 0)
			{
				val = recycled.Dequeue();
				list[val.hashedPoolIndex] = val;
			}
			else
			{
				int count = pool.Count;
				if (count == pool.Capacity)
				{
					pool.Capacity += granularity;
				}
				val = new ValueType
				{
					hashedPoolIndex = count
				};
				pool.Add(val);
				list.Add(val);
			}
			val.hashedPoolKey = key;
			dict.Add(key, val);
			return val;
		}

		public void Remove(ValueType value)
		{
			dict.Remove(value.hashedPoolKey);
			list[value.hashedPoolIndex] = null;
			recycled.Enqueue(value);
			value.hashedPoolKey = ulong.MaxValue;
		}

		public bool TryGetValue(ulong key, out ValueType value)
		{
			return dict.TryGetValue(key, out value);
		}

		public bool ContainsKey(ulong key)
		{
			return dict.ContainsKey(key);
		}
	}

	public class SimpleList<T>
	{
		private const int defaultCapacity = 16;

		private static readonly T[] emptyArray = new T[0];

		public T[] array;

		public int count;

		public int Count => count;

		public int Capacity
		{
			get
			{
				return array.Length;
			}
			set
			{
				if (value == array.Length)
				{
					return;
				}
				if (value > 0)
				{
					T[] destinationArray = new T[value];
					if (count > 0)
					{
						Array.Copy(array, 0, destinationArray, 0, count);
					}
					array = destinationArray;
				}
				else
				{
					array = emptyArray;
				}
			}
		}

		public T this[int index]
		{
			get
			{
				return array[index];
			}
			set
			{
				array[index] = value;
			}
		}

		public SimpleList()
		{
			array = emptyArray;
		}

		public SimpleList(int capacity)
		{
			array = ((capacity == 0) ? emptyArray : new T[capacity]);
		}

		public void Add(T item)
		{
			if (count == array.Length)
			{
				EnsureCapacity(count + 1);
			}
			array[count++] = item;
		}

		public void Clear()
		{
			if (count > 0)
			{
				Array.Clear(array, 0, count);
				count = 0;
			}
		}

		public bool Contains(T item)
		{
			for (int i = 0; i < count; i++)
			{
				if (array[i].Equals(item))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(T[] array)
		{
			Array.Copy(this.array, 0, array, 0, count);
		}

		public void EnsureCapacity(int min)
		{
			if (array.Length < min)
			{
				int num = ((array.Length == 0) ? 16 : (array.Length * 2));
				num = ((num < min) ? min : num);
				Capacity = num;
			}
		}
	}

	public class SmartListValue
	{
		public int hashedListIndex = -1;
	}

	public class SmartList
	{
		private const int defaultCapacity = 16;

		private static readonly OccludeeState[] emptyList = new OccludeeState[0];

		private static readonly int[] emptySlots = new int[0];

		private OccludeeState[] list;

		private int[] slots;

		private Queue<int> recycled;

		private int count;

		public OccludeeState[] List => list;

		public int[] Slots => slots;

		public int Size => count;

		public int Count => count - recycled.Count;

		public OccludeeState this[int i]
		{
			get
			{
				return list[i];
			}
			set
			{
				list[i] = value;
			}
		}

		public int Capacity
		{
			get
			{
				return list.Length;
			}
			set
			{
				if (value == list.Length)
				{
					return;
				}
				if (value > 0)
				{
					OccludeeState[] destinationArray = new OccludeeState[value];
					int[] destinationArray2 = new int[value];
					if (count > 0)
					{
						Array.Copy(list, destinationArray, count);
						Array.Copy(slots, destinationArray2, count);
					}
					list = destinationArray;
					slots = destinationArray2;
				}
				else
				{
					list = emptyList;
					slots = emptySlots;
				}
			}
		}

		public SmartList(int capacity)
		{
			list = new OccludeeState[capacity];
			slots = new int[capacity];
			recycled = new Queue<int>();
			count = 0;
		}

		public void Add(OccludeeState value, int capacityGranularity = 16)
		{
			int num;
			if (recycled.Count > 0)
			{
				num = recycled.Dequeue();
				list[num] = value;
				slots[num] = value.slot;
			}
			else
			{
				num = count;
				if (num == list.Length)
				{
					EnsureCapacity(count + 1);
				}
				list[num] = value;
				slots[num] = value.slot;
				count++;
			}
			value.hashedListIndex = num;
		}

		public void Remove(OccludeeState value)
		{
			int hashedListIndex = value.hashedListIndex;
			list[hashedListIndex] = null;
			slots[hashedListIndex] = -1;
			recycled.Enqueue(hashedListIndex);
			value.hashedListIndex = -1;
		}

		public bool Contains(OccludeeState value)
		{
			int hashedListIndex = value.hashedListIndex;
			return hashedListIndex >= 0 && list[hashedListIndex] != null;
		}

		public void EnsureCapacity(int min)
		{
			if (list.Length < min)
			{
				int num = ((list.Length == 0) ? 16 : (list.Length * 2));
				num = ((num < min) ? min : num);
				Capacity = num;
			}
		}
	}

	[Serializable]
	public class Cell : HashedPoolValue
	{
		public int x;

		public int y;

		public int z;

		public Bounds bounds;

		public Vector4 sphereBounds;

		public bool isVisible;

		public SmartList staticBucket;

		public SmartList dynamicBucket;

		public void Reset()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			x = (y = (z = 0));
			bounds = default(Bounds);
			sphereBounds = Vector4.zero;
			isVisible = true;
			staticBucket = null;
			dynamicBucket = null;
		}

		public Cell Initialize(int x, int y, int z, Bounds bounds)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			this.x = x;
			this.y = y;
			this.z = z;
			this.bounds = bounds;
			float num = ((Bounds)(ref bounds)).center.x;
			float num2 = ((Bounds)(ref bounds)).center.y;
			float num3 = ((Bounds)(ref bounds)).center.z;
			Vector3 extents = ((Bounds)(ref bounds)).extents;
			sphereBounds = new Vector4(num, num2, num3, ((Vector3)(ref extents)).magnitude);
			isVisible = true;
			staticBucket = new SmartList(32);
			dynamicBucket = new SmartList(32);
			return this;
		}
	}

	public struct Sphere
	{
		public Vector3 position;

		public float radius;

		public bool IsValid()
		{
			return radius > 0f;
		}

		public Sphere(Vector3 position, float radius)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			this.position = position;
			this.radius = radius;
		}
	}

	public delegate void OnVisibilityChanged(bool visible);

	public DebugSettings debugSettings = new DebugSettings();

	private Material debugMipMat;

	private const float debugDrawDuration = 0.0334f;

	private Material downscaleMat = null;

	private Material blitCopyMat = null;

	private int hiZLevelCount = 0;

	private int hiZWidth = 0;

	private int hiZHeight = 0;

	private RenderTexture depthTexture = null;

	private RenderTexture hiZTexture = null;

	private RenderTexture[] hiZLevels = null;

	private const int GridCellsPerAxis = 2097152;

	private const int GridHalfCellsPerAxis = 1048576;

	private const int GridMinHalfCellsPerAxis = -1048575;

	private const int GridMaxHalfCellsPerAxis = 1048575;

	private const float GridCellSize = 100f;

	private const float GridHalfCellSize = 50f;

	private const float GridRcpCellSize = 0.01f;

	private const int GridPoolCapacity = 16384;

	private const int GridPoolGranularity = 4096;

	private static HashedPool<Cell> grid = new HashedPool<Cell>(16384, 4096);

	private static Queue<Cell> gridChanged = new Queue<Cell>();

	public ComputeShader computeShader;

	public bool usePixelShaderFallback = true;

	public bool useAsyncReadAPI = false;

	private Camera camera;

	private const int ComputeThreadsPerGroup = 64;

	private const int InputBufferStride = 16;

	private const int ResultBufferStride = 4;

	private const int OccludeeMaxSlotsPerPool = 1048576;

	private const int OccludeePoolGranularity = 2048;

	private const int StateBufferGranularity = 2048;

	private const int GridBufferGranularity = 256;

	private static Queue<OccludeeState> statePool = new Queue<OccludeeState>();

	private static SimpleList<OccludeeState> staticOccludees = new SimpleList<OccludeeState>(2048);

	private static SimpleList<OccludeeState.State> staticStates = new SimpleList<OccludeeState.State>(2048);

	private static SimpleList<int> staticVisibilityChanged = new SimpleList<int>(1024);

	private static SimpleList<OccludeeState> dynamicOccludees = new SimpleList<OccludeeState>(2048);

	private static SimpleList<OccludeeState.State> dynamicStates = new SimpleList<OccludeeState.State>(2048);

	private static SimpleList<int> dynamicVisibilityChanged = new SimpleList<int>(1024);

	private static List<int> staticChanged = new List<int>(256);

	private static Queue<int> staticRecycled = new Queue<int>();

	private static List<int> dynamicChanged = new List<int>(1024);

	private static Queue<int> dynamicRecycled = new Queue<int>();

	private static BufferSet staticSet = new BufferSet();

	private static BufferSet dynamicSet = new BufferSet();

	private static BufferSet gridSet = new BufferSet();

	private Vector4[] frustumPlanes = (Vector4[])(object)new Vector4[6];

	private string[] frustumPropNames = new string[6];

	private float[] matrixToFloatTemp = new float[16];

	private Material fallbackMat = null;

	private Material depthCopyMat = null;

	private Matrix4x4 viewMatrix;

	private Matrix4x4 projMatrix;

	private Matrix4x4 viewProjMatrix;

	private Matrix4x4 prevViewProjMatrix;

	private Matrix4x4 invViewProjMatrix;

	private bool useNativePath = true;

	private static OcclusionCulling instance;

	private static GraphicsDeviceType[] supportedDeviceTypes = (GraphicsDeviceType[])(object)new GraphicsDeviceType[1] { (GraphicsDeviceType)2 };

	private static bool _enabled = false;

	private static bool _safeMode = false;

	private static DebugFilter _debugShow = DebugFilter.Off;

	public bool HiZReady => (Object)(object)hiZTexture != (Object)null && hiZWidth > 0 && hiZHeight > 0;

	public static OcclusionCulling Instance => instance;

	public static bool Supported => supportedDeviceTypes.Contains(SystemInfo.graphicsDeviceType);

	public static bool Enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			_enabled = value;
			if ((Object)(object)instance != (Object)null)
			{
				((Behaviour)instance).enabled = value;
			}
		}
	}

	public static bool SafeMode
	{
		get
		{
			return _safeMode;
		}
		set
		{
			_safeMode = value;
		}
	}

	public static DebugFilter DebugShow
	{
		get
		{
			return _debugShow;
		}
		set
		{
			_debugShow = value;
		}
	}

	public static bool DebugFilterIsDynamic(int filter)
	{
		return filter == 1 || filter == 4;
	}

	public static bool DebugFilterIsStatic(int filter)
	{
		return filter == 2 || filter == 4;
	}

	public static bool DebugFilterIsGrid(int filter)
	{
		return filter == 3 || filter == 4;
	}

	private void DebugInitialize()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		debugMipMat = new Material(Shader.Find("Hidden/OcclusionCulling/DebugMip"))
		{
			hideFlags = (HideFlags)61
		};
	}

	private void DebugShutdown()
	{
		if ((Object)(object)debugMipMat != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)debugMipMat);
			debugMipMat = null;
		}
	}

	private void DebugUpdate()
	{
		if (HiZReady)
		{
			debugSettings.showMainLod = Mathf.Clamp(debugSettings.showMainLod, 0, hiZLevels.Length - 1);
		}
	}

	private void DebugDraw()
	{
	}

	public static void NormalizePlane(ref Vector4 plane)
	{
		float num = Mathf.Sqrt(plane.x * plane.x + plane.y * plane.y + plane.z * plane.z);
		plane.x /= num;
		plane.y /= num;
		plane.z /= num;
		plane.w /= num;
	}

	public static void ExtractFrustum(Matrix4x4 viewProjMatrix, ref Vector4[] planes)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		planes[0].x = viewProjMatrix.m30 + viewProjMatrix.m00;
		planes[0].y = viewProjMatrix.m31 + viewProjMatrix.m01;
		planes[0].z = viewProjMatrix.m32 + viewProjMatrix.m02;
		planes[0].w = viewProjMatrix.m33 + viewProjMatrix.m03;
		NormalizePlane(ref planes[0]);
		planes[1].x = viewProjMatrix.m30 - viewProjMatrix.m00;
		planes[1].y = viewProjMatrix.m31 - viewProjMatrix.m01;
		planes[1].z = viewProjMatrix.m32 - viewProjMatrix.m02;
		planes[1].w = viewProjMatrix.m33 - viewProjMatrix.m03;
		NormalizePlane(ref planes[1]);
		planes[2].x = viewProjMatrix.m30 - viewProjMatrix.m10;
		planes[2].y = viewProjMatrix.m31 - viewProjMatrix.m11;
		planes[2].z = viewProjMatrix.m32 - viewProjMatrix.m12;
		planes[2].w = viewProjMatrix.m33 - viewProjMatrix.m13;
		NormalizePlane(ref planes[2]);
		planes[3].x = viewProjMatrix.m30 + viewProjMatrix.m10;
		planes[3].y = viewProjMatrix.m31 + viewProjMatrix.m11;
		planes[3].z = viewProjMatrix.m32 + viewProjMatrix.m12;
		planes[3].w = viewProjMatrix.m33 + viewProjMatrix.m13;
		NormalizePlane(ref planes[3]);
		planes[4].x = viewProjMatrix.m20;
		planes[4].y = viewProjMatrix.m21;
		planes[4].z = viewProjMatrix.m22;
		planes[4].w = viewProjMatrix.m23;
		NormalizePlane(ref planes[4]);
		planes[5].x = viewProjMatrix.m30 - viewProjMatrix.m20;
		planes[5].y = viewProjMatrix.m31 - viewProjMatrix.m21;
		planes[5].z = viewProjMatrix.m32 - viewProjMatrix.m22;
		planes[5].w = viewProjMatrix.m33 - viewProjMatrix.m23;
		NormalizePlane(ref planes[5]);
	}

	public void CheckResizeHiZMap()
	{
		int pixelWidth = camera.pixelWidth;
		int pixelHeight = camera.pixelHeight;
		if (pixelWidth <= 0 || pixelHeight <= 0)
		{
			return;
		}
		int num = pixelWidth / 4;
		int num2 = pixelHeight / 4;
		if (hiZLevels == null || hiZWidth != num || hiZHeight != num2)
		{
			InitializeHiZMap(num, num2);
			hiZWidth = num;
			hiZHeight = num2;
			if (debugSettings.log)
			{
				Debug.Log((object)("[OcclusionCulling] Resized HiZ Map to " + hiZWidth + " x " + hiZHeight));
			}
		}
	}

	private void InitializeHiZMap()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		Shader val = Shader.Find("Hidden/OcclusionCulling/DepthDownscale");
		Shader val2 = Shader.Find("Hidden/OcclusionCulling/BlitCopy");
		downscaleMat = new Material(val)
		{
			hideFlags = (HideFlags)61
		};
		blitCopyMat = new Material(val2)
		{
			hideFlags = (HideFlags)61
		};
		CheckResizeHiZMap();
	}

	private void FinalizeHiZMap()
	{
		DestroyHiZMap();
		if ((Object)(object)downscaleMat != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)downscaleMat);
			downscaleMat = null;
		}
		if ((Object)(object)blitCopyMat != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)blitCopyMat);
			blitCopyMat = null;
		}
	}

	private void InitializeHiZMap(int width, int height)
	{
		DestroyHiZMap();
		width = Mathf.Clamp(width, 1, 65536);
		height = Mathf.Clamp(height, 1, 65536);
		int num = Mathf.Min(width, height);
		hiZLevelCount = (int)(Mathf.Log((float)num, 2f) + 1f);
		hiZLevels = (RenderTexture[])(object)new RenderTexture[hiZLevelCount];
		depthTexture = CreateDepthTexture("DepthTex", width, height);
		hiZTexture = CreateDepthTexture("HiZMapTex", width, height, mips: true);
		for (int i = 0; i < hiZLevelCount; i++)
		{
			hiZLevels[i] = CreateDepthTextureMip("HiZMap" + i, width, height, i);
		}
	}

	private void DestroyHiZMap()
	{
		if ((Object)(object)depthTexture != (Object)null)
		{
			RenderTexture.active = null;
			Object.DestroyImmediate((Object)(object)depthTexture);
			depthTexture = null;
		}
		if ((Object)(object)hiZTexture != (Object)null)
		{
			RenderTexture.active = null;
			Object.DestroyImmediate((Object)(object)hiZTexture);
			hiZTexture = null;
		}
		if (hiZLevels != null)
		{
			for (int i = 0; i < hiZLevels.Length; i++)
			{
				Object.DestroyImmediate((Object)(object)hiZLevels[i]);
			}
			hiZLevels = null;
		}
	}

	private RenderTexture CreateDepthTexture(string name, int width, int height, bool mips = false)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		RenderTexture val = new RenderTexture(width, height, 0, (RenderTextureFormat)14, (RenderTextureReadWrite)1);
		((Object)val).name = name;
		val.useMipMap = mips;
		val.autoGenerateMips = false;
		((Texture)val).wrapMode = (TextureWrapMode)1;
		((Texture)val).filterMode = (FilterMode)0;
		val.Create();
		return val;
	}

	private RenderTexture CreateDepthTextureMip(string name, int width, int height, int mip)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		int num = width >> mip;
		int num2 = height >> mip;
		RenderTexture val = new RenderTexture(num, num2, 0, (RenderTextureFormat)14, (RenderTextureReadWrite)1);
		((Object)val).name = name;
		val.useMipMap = false;
		((Texture)val).wrapMode = (TextureWrapMode)1;
		((Texture)val).filterMode = (FilterMode)0;
		val.Create();
		return val;
	}

	public void GrabDepthTexture()
	{
		if ((Object)(object)depthTexture != (Object)null)
		{
			Graphics.Blit((Texture)null, depthTexture, depthCopyMat, 0);
		}
	}

	public void GenerateHiZMipChain()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (HiZReady)
		{
			bool flag = true;
			depthCopyMat.SetMatrix("_CameraReprojection", prevViewProjMatrix * invViewProjMatrix);
			depthCopyMat.SetFloat("_FrustumNoDataDepth", flag ? 1f : 0f);
			Graphics.Blit((Texture)(object)depthTexture, hiZLevels[0], depthCopyMat, 1);
			for (int i = 1; i < hiZLevels.Length; i++)
			{
				RenderTexture val = hiZLevels[i - 1];
				RenderTexture val2 = hiZLevels[i];
				int num = ((((uint)((Texture)val).width & (true ? 1u : 0u)) != 0 || ((uint)((Texture)val).height & (true ? 1u : 0u)) != 0) ? 1 : 0);
				downscaleMat.SetTexture("_MainTex", (Texture)(object)val);
				Graphics.Blit((Texture)(object)val, val2, downscaleMat, num);
			}
			for (int j = 0; j < hiZLevels.Length; j++)
			{
				Graphics.SetRenderTarget(hiZTexture, j);
				Graphics.Blit((Texture)(object)hiZLevels[j], blitCopyMat);
			}
		}
	}

	private void DebugDrawGizmos()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		Camera component = ((Component)this).GetComponent<Camera>();
		Gizmos.color = new Color(0.75f, 0.75f, 0f, 0.5f);
		Gizmos.matrix = Matrix4x4.TRS(((Component)this).transform.position, ((Component)this).transform.rotation, Vector3.one);
		Gizmos.DrawFrustum(Vector3.zero, component.fieldOfView, component.farClipPlane, component.nearClipPlane, component.aspect);
		Gizmos.color = Color.red;
		Gizmos.matrix = Matrix4x4.identity;
		Matrix4x4 worldToCameraMatrix = component.worldToCameraMatrix;
		Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(component.projectionMatrix, false);
		Matrix4x4 val = gPUProjectionMatrix * worldToCameraMatrix;
		Vector4[] planes = (Vector4[])(object)new Vector4[6];
		ExtractFrustum(val, ref planes);
		Vector3 val2 = default(Vector3);
		for (int i = 0; i < planes.Length; i++)
		{
			((Vector3)(ref val2))._002Ector(planes[i].x, planes[i].y, planes[i].z);
			float w = planes[i].w;
			Vector3 val3 = -val2 * w;
			Gizmos.DrawLine(val3, val3 * 2f);
		}
	}

	private static int floor(float x)
	{
		int num = (int)x;
		return (x < (float)num) ? (num - 1) : num;
	}

	public static Cell RegisterToGrid(OccludeeState occludee)
	{
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("BucketHash");
		int num = floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num2 = floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num3 = floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		int num4 = Mathf.Clamp(num, -1048575, 1048575);
		int num5 = Mathf.Clamp(num2, -1048575, 1048575);
		int num6 = Mathf.Clamp(num3, -1048575, 1048575);
		ulong num7 = (ulong)((num4 >= 0) ? num4 : (num4 + 1048575));
		ulong num8 = (ulong)((num5 >= 0) ? num5 : (num5 + 1048575));
		ulong num9 = (ulong)((num6 >= 0) ? num6 : (num6 + 1048575));
		ulong key = (num7 << 42) | (num8 << 21) | num9;
		Profiler.EndSample();
		Profiler.BeginSample("BucketFind");
		Cell value;
		bool flag = grid.TryGetValue(key, out value);
		Profiler.EndSample();
		Profiler.BeginSample("BucketAdd");
		if (!flag)
		{
			Vector3 val = default(Vector3);
			val.x = (float)num * 100f + 50f;
			val.y = (float)num2 * 100f + 50f;
			val.z = (float)num3 * 100f + 50f;
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(100f, 100f, 100f);
			value = grid.Add(key).Initialize(num, num2, num3, new Bounds(val, val2));
		}
		SmartList smartList = (occludee.isStatic ? value.staticBucket : value.dynamicBucket);
		if (!flag || !smartList.Contains(occludee))
		{
			occludee.cell = value;
			smartList.Add(occludee);
			gridChanged.Enqueue(value);
		}
		Profiler.EndSample();
		return value;
	}

	public static void UpdateInGrid(OccludeeState occludee)
	{
		int num = floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num2 = floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num3 = floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		if (num != occludee.cell.x || num2 != occludee.cell.y || num3 != occludee.cell.z)
		{
			UnregisterFromGrid(occludee);
			RegisterToGrid(occludee);
		}
	}

	public static void UnregisterFromGrid(OccludeeState occludee)
	{
		Cell cell = occludee.cell;
		SmartList smartList = (occludee.isStatic ? cell.staticBucket : cell.dynamicBucket);
		gridChanged.Enqueue(cell);
		smartList.Remove(occludee);
		if (cell.staticBucket.Count == 0 && cell.dynamicBucket.Count == 0)
		{
			grid.Remove(cell);
			cell.Reset();
		}
		occludee.cell = null;
	}

	public void UpdateGridBuffers()
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (gridSet.CheckResize(grid.Size, 256))
		{
			if (debugSettings.log)
			{
				Debug.Log((object)("[OcclusionCulling] Resized grid to " + grid.Size));
			}
			for (int i = 0; i < grid.Size; i++)
			{
				if (grid[i] != null)
				{
					gridChanged.Enqueue(grid[i]);
				}
			}
		}
		bool flag = gridChanged.Count > 0;
		Profiler.BeginSample("UpdateInputData");
		while (gridChanged.Count > 0)
		{
			Cell cell = gridChanged.Dequeue();
			gridSet.inputData[cell.hashedPoolIndex] = Color.op_Implicit(cell.sphereBounds);
		}
		Profiler.EndSample();
		if (flag)
		{
			Profiler.BeginSample("UpdateInputBuffer");
			gridSet.UploadData();
			Profiler.EndSample();
		}
	}

	private static void GrowStatePool()
	{
		for (int i = 0; i < 2048; i++)
		{
			statePool.Enqueue(new OccludeeState());
		}
	}

	private static OccludeeState Allocate()
	{
		if (statePool.Count == 0)
		{
			GrowStatePool();
		}
		return statePool.Dequeue();
	}

	private static void Release(OccludeeState state)
	{
		statePool.Enqueue(state);
	}

	private void Awake()
	{
		instance = this;
		camera = ((Component)this).GetComponent<Camera>();
		for (int i = 0; i < 6; i++)
		{
			frustumPropNames[i] = "_FrustumPlane" + i;
		}
	}

	private void OnEnable()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Invalid comparison between Unknown and I4
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		if (!Enabled)
		{
			Enabled = false;
			return;
		}
		if (!Supported)
		{
			Debug.LogWarning((object)string.Concat("[OcclusionCulling] Disabled due to graphics device type ", SystemInfo.graphicsDeviceType, " not supported."));
			Enabled = false;
			return;
		}
		usePixelShaderFallback = usePixelShaderFallback || !SystemInfo.supportsComputeShaders || (Object)(object)computeShader == (Object)null || !computeShader.HasKernel("compute_cull");
		useNativePath = (int)SystemInfo.graphicsDeviceType == 2 && SupportsNativePath();
		useAsyncReadAPI = !useNativePath && SystemInfo.supportsAsyncGPUReadback;
		if (!useNativePath && !useAsyncReadAPI)
		{
			Debug.LogWarning((object)("[OcclusionCulling] Disabled due to unsupported Async GPU Reads on device " + SystemInfo.graphicsDeviceType));
			Enabled = false;
			return;
		}
		for (int i = 0; i < staticOccludees.Count; i++)
		{
			staticChanged.Add(i);
		}
		for (int j = 0; j < dynamicOccludees.Count; j++)
		{
			dynamicChanged.Add(j);
		}
		if (usePixelShaderFallback)
		{
			fallbackMat = new Material(Shader.Find("Hidden/OcclusionCulling/Culling"))
			{
				hideFlags = (HideFlags)61
			};
		}
		staticSet.Attach(this);
		dynamicSet.Attach(this);
		gridSet.Attach(this);
		depthCopyMat = new Material(Shader.Find("Hidden/OcclusionCulling/DepthCopy"))
		{
			hideFlags = (HideFlags)61
		};
		InitializeHiZMap();
		UpdateCameraMatrices(starting: true);
	}

	private bool SupportsNativePath()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		bool result = true;
		try
		{
			OccludeeState.State states = default(OccludeeState.State);
			Color32 results = default(Color32);
			((Color32)(ref results))._002Ector((byte)0, (byte)0, (byte)0, (byte)0);
			Vector4 zero = Vector4.zero;
			int bucket = 0;
			int changed = 0;
			int changedCount = 0;
			ProcessOccludees_Native(ref states, ref bucket, 0, ref results, 0, ref changed, ref changedCount, ref zero, 0f, 0u);
		}
		catch (EntryPointNotFoundException)
		{
			Debug.Log((object)"[OcclusionCulling] Fast native path not available. Reverting to managed fallback.");
			result = false;
		}
		return result;
	}

	private void OnDisable()
	{
		if ((Object)(object)fallbackMat != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)fallbackMat);
			fallbackMat = null;
		}
		if ((Object)(object)depthCopyMat != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)depthCopyMat);
			depthCopyMat = null;
		}
		staticSet.Dispose();
		dynamicSet.Dispose();
		gridSet.Dispose();
		FinalizeHiZMap();
	}

	public static void MakeAllVisible()
	{
		for (int i = 0; i < staticOccludees.Count; i++)
		{
			if (staticOccludees[i] != null)
			{
				staticOccludees[i].MakeVisible();
			}
		}
		for (int j = 0; j < dynamicOccludees.Count; j++)
		{
			if (dynamicOccludees[j] != null)
			{
				dynamicOccludees[j].MakeVisible();
			}
		}
	}

	private void Update()
	{
		if (!Enabled)
		{
			((Behaviour)this).enabled = false;
			return;
		}
		CheckResizeHiZMap();
		DebugUpdate();
		DebugDraw();
	}

	public static void RecursiveAddOccludees<T>(Transform transform, float minTimeVisible = 0.1f, bool isStatic = true, bool stickyGizmos = false) where T : Occludee
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		Renderer component = ((Component)transform).GetComponent<Renderer>();
		Collider component2 = ((Component)transform).GetComponent<Collider>();
		if ((Object)(object)component != (Object)null && (Object)(object)component2 != (Object)null)
		{
			T component3 = ((Component)component).gameObject.GetComponent<T>();
			component3 = (((Object)(object)component3 == (Object)null) ? ((Component)component).gameObject.AddComponent<T>() : component3);
			component3.minTimeVisible = minTimeVisible;
			component3.isStatic = isStatic;
			component3.stickyGizmos = stickyGizmos;
			component3.Register();
		}
		foreach (Transform item in transform)
		{
			Transform transform2 = item;
			RecursiveAddOccludees<T>(transform2, minTimeVisible, isStatic, stickyGizmos);
		}
	}

	private static int FindFreeSlot(SimpleList<OccludeeState> occludees, SimpleList<OccludeeState.State> states, Queue<int> recycled)
	{
		int result;
		if (recycled.Count > 0)
		{
			result = recycled.Dequeue();
		}
		else
		{
			if (occludees.Count == occludees.Capacity)
			{
				int num = Mathf.Min(occludees.Capacity + 2048, 1048576);
				if (num > 0)
				{
					occludees.Capacity = num;
					states.Capacity = num;
				}
			}
			if (occludees.Count < occludees.Capacity)
			{
				result = occludees.Count;
				occludees.Add(null);
				states.Add(default(OccludeeState.State));
			}
			else
			{
				result = -1;
			}
		}
		return result;
	}

	public static OccludeeState GetStateById(int id)
	{
		if (id >= 0 && id < 2097152)
		{
			bool flag = id < 1048576;
			int index = (flag ? id : (id - 1048576));
			if (flag)
			{
				return staticOccludees[index];
			}
			return dynamicOccludees[index];
		}
		return null;
	}

	public static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OnVisibilityChanged onVisibilityChanged = null)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		num = ((!isStatic) ? RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, dynamicOccludees, dynamicStates, dynamicRecycled, dynamicChanged, dynamicSet, dynamicVisibilityChanged) : RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, staticOccludees, staticStates, staticRecycled, staticChanged, staticSet, staticVisibilityChanged));
		return (num < 0 || isStatic) ? num : (num + 1048576);
	}

	private static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OnVisibilityChanged onVisibilityChanged, SimpleList<OccludeeState> occludees, SimpleList<OccludeeState.State> states, Queue<int> recycled, List<int> changed, BufferSet set, SimpleList<int> visibilityChanged)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		int num = FindFreeSlot(occludees, states, recycled);
		if (num >= 0)
		{
			Vector4 sphereBounds = default(Vector4);
			((Vector4)(ref sphereBounds))._002Ector(center.x, center.y, center.z, radius);
			OccludeeState occludeeState = Allocate().Initialize(states, set, num, sphereBounds, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged);
			occludeeState.cell = RegisterToGrid(occludeeState);
			occludees[num] = occludeeState;
			changed.Add(num);
			if (states.array[num].isVisible != 0 != occludeeState.cell.isVisible)
			{
				visibilityChanged.Add(num);
			}
		}
		return num;
	}

	public static void UnregisterOccludee(int id)
	{
		if (id >= 0 && id < 2097152)
		{
			bool flag = id < 1048576;
			int slot = (flag ? id : (id - 1048576));
			if (flag)
			{
				UnregisterOccludee(slot, staticOccludees, staticRecycled, staticChanged);
			}
			else
			{
				UnregisterOccludee(slot, dynamicOccludees, dynamicRecycled, dynamicChanged);
			}
		}
	}

	private static void UnregisterOccludee(int slot, SimpleList<OccludeeState> occludees, Queue<int> recycled, List<int> changed)
	{
		OccludeeState occludeeState = occludees[slot];
		UnregisterFromGrid(occludeeState);
		recycled.Enqueue(slot);
		changed.Add(slot);
		Release(occludeeState);
		occludees[slot] = null;
		occludeeState.Invalidate();
	}

	public static void UpdateDynamicOccludee(int id, Vector3 center, float radius)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		int num = id - 1048576;
		if (num >= 0 && num < 1048576)
		{
			dynamicStates.array[num].sphereBounds = new Vector4(center.x, center.y, center.z, radius);
			dynamicChanged.Add(num);
		}
	}

	private void UpdateBuffers(SimpleList<OccludeeState> occludees, SimpleList<OccludeeState.State> states, BufferSet set, List<int> changed, bool isStatic)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		int count = occludees.Count;
		bool flag = changed.Count > 0;
		Profiler.BeginSample("CheckResize");
		set.CheckResize(count, 2048);
		Profiler.EndSample();
		Profiler.BeginSample("UpdateInputData");
		for (int i = 0; i < changed.Count; i++)
		{
			int num = changed[i];
			OccludeeState occludeeState = occludees[num];
			if (occludeeState != null)
			{
				if (!isStatic)
				{
					UpdateInGrid(occludeeState);
				}
				set.inputData[num] = Color.op_Implicit(states[num].sphereBounds);
			}
			else
			{
				set.inputData[num] = Color.op_Implicit(Vector4.zero);
			}
		}
		changed.Clear();
		Profiler.EndSample();
		if (flag)
		{
			Profiler.BeginSample("UpdateInputBuffer");
			set.UploadData();
			Profiler.EndSample();
		}
	}

	private void UpdateCameraMatrices(bool starting = false)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (!starting)
		{
			prevViewProjMatrix = viewProjMatrix;
		}
		Matrix4x4 val = Matrix4x4.Perspective(camera.fieldOfView, camera.aspect, camera.nearClipPlane, camera.farClipPlane);
		viewMatrix = camera.worldToCameraMatrix;
		projMatrix = GL.GetGPUProjectionMatrix(val, false);
		viewProjMatrix = projMatrix * viewMatrix;
		invViewProjMatrix = Matrix4x4.Inverse(viewProjMatrix);
		if (starting)
		{
			prevViewProjMatrix = viewProjMatrix;
		}
	}

	private void OnPreCull()
	{
		UpdateCameraMatrices();
		GenerateHiZMipChain();
		PrepareAndDispatch();
		IssueRead();
		if (grid.Size <= gridSet.resultData.Length)
		{
			RetrieveAndApplyVisibility();
			return;
		}
		Debug.LogWarning((object)("[OcclusionCulling] Grid size and result capacity are out of sync: " + grid.Size + ", " + gridSet.resultData.Length));
	}

	private void OnPostRender()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		bool sRGBWrite = GL.sRGBWrite;
		RenderBuffer activeColorBuffer = Graphics.activeColorBuffer;
		RenderBuffer activeDepthBuffer = Graphics.activeDepthBuffer;
		GrabDepthTexture();
		Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
		GL.sRGBWrite = sRGBWrite;
	}

	private float[] MatrixToFloatArray(Matrix4x4 m)
	{
		int i = 0;
		int num = 0;
		for (; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				matrixToFloatTemp[num++] = ((Matrix4x4)(ref m))[j, i];
			}
		}
		return matrixToFloatTemp;
	}

	private void PrepareAndDispatch()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("PrepareAndDispatch");
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)hiZWidth, (float)hiZHeight);
		ExtractFrustum(viewProjMatrix, ref frustumPlanes);
		bool flag = true;
		if (usePixelShaderFallback)
		{
			fallbackMat.SetTexture("_HiZMap", (Texture)(object)hiZTexture);
			fallbackMat.SetFloat("_HiZMaxLod", (float)(hiZLevelCount - 1));
			fallbackMat.SetMatrix("_ViewMatrix", viewMatrix);
			fallbackMat.SetMatrix("_ProjMatrix", projMatrix);
			fallbackMat.SetMatrix("_ViewProjMatrix", viewProjMatrix);
			fallbackMat.SetVector("_CameraWorldPos", Vector4.op_Implicit(((Component)this).transform.position));
			fallbackMat.SetVector("_ViewportSize", Vector4.op_Implicit(val));
			fallbackMat.SetFloat("_FrustumCull", flag ? 0f : 1f);
			for (int i = 0; i < 6; i++)
			{
				fallbackMat.SetVector(frustumPropNames[i], frustumPlanes[i]);
			}
		}
		else
		{
			computeShader.SetTexture(0, "_HiZMap", (Texture)(object)hiZTexture);
			computeShader.SetFloat("_HiZMaxLod", (float)(hiZLevelCount - 1));
			computeShader.SetFloats("_ViewMatrix", MatrixToFloatArray(viewMatrix));
			computeShader.SetFloats("_ProjMatrix", MatrixToFloatArray(projMatrix));
			computeShader.SetFloats("_ViewProjMatrix", MatrixToFloatArray(viewProjMatrix));
			computeShader.SetVector("_CameraWorldPos", Vector4.op_Implicit(((Component)this).transform.position));
			computeShader.SetVector("_ViewportSize", Vector4.op_Implicit(val));
			computeShader.SetFloat("_FrustumCull", flag ? 0f : 1f);
			for (int j = 0; j < 6; j++)
			{
				computeShader.SetVector(frustumPropNames[j], frustumPlanes[j]);
			}
		}
		if (staticOccludees.Count > 0)
		{
			Profiler.BeginSample("UpdateStaticBuffers");
			UpdateBuffers(staticOccludees, staticStates, staticSet, staticChanged, isStatic: true);
			Profiler.EndSample();
			Profiler.BeginSample("DispatchStatic");
			staticSet.Dispatch(staticOccludees.Count);
			Profiler.EndSample();
		}
		if (dynamicOccludees.Count > 0)
		{
			Profiler.BeginSample("UpdateDynamicBuffers");
			UpdateBuffers(dynamicOccludees, dynamicStates, dynamicSet, dynamicChanged, isStatic: false);
			Profiler.EndSample();
			Profiler.BeginSample("DispatchDynamic");
			dynamicSet.Dispatch(dynamicOccludees.Count);
			Profiler.EndSample();
		}
		Profiler.BeginSample("UpdateGridBuffers");
		UpdateGridBuffers();
		Profiler.EndSample();
		Profiler.BeginSample("DispatchGrid");
		gridSet.Dispatch(grid.Size);
		Profiler.EndSample();
		Profiler.EndSample();
	}

	private void IssueRead()
	{
		Profiler.BeginSample("IssueRead");
		if (staticOccludees.Count > 0)
		{
			staticSet.IssueRead();
		}
		if (dynamicOccludees.Count > 0)
		{
			dynamicSet.IssueRead();
		}
		if (grid.Count > 0)
		{
			gridSet.IssueRead();
		}
		GL.IssuePluginEvent(Graphics.GetRenderEventFunc(), 2);
		Profiler.EndSample();
	}

	public void ResetTiming(SmartList bucket)
	{
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState occludeeState = bucket[i];
			if (occludeeState != null)
			{
				occludeeState.states.array[occludeeState.slot].waitTime = 0f;
			}
		}
	}

	public void ResetTiming()
	{
		for (int i = 0; i < grid.Size; i++)
		{
			Cell cell = grid[i];
			if (cell != null)
			{
				ResetTiming(cell.staticBucket);
				ResetTiming(cell.dynamicBucket);
			}
		}
	}

	private static bool FrustumCull(Vector4[] planes, Vector4 testSphere)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 6; i++)
		{
			float num = planes[i].x * testSphere.x + planes[i].y * testSphere.y + planes[i].z * testSphere.z + planes[i].w;
			if (num < 0f - testSphere.w)
			{
				return false;
			}
		}
		return true;
	}

	private static int ProcessOccludees_Safe(SimpleList<OccludeeState.State> states, SmartList bucket, Color32[] results, SimpleList<int> changed, Vector4[] frustumPlanes, float time, uint frame)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState occludeeState = bucket[i];
			if (occludeeState == null || occludeeState.slot >= results.Length)
			{
				continue;
			}
			int slot = occludeeState.slot;
			OccludeeState.State value = states[slot];
			bool flag = FrustumCull(frustumPlanes, value.sphereBounds);
			bool flag2 = results[slot].r > 0 && flag;
			if (flag2 || frame < value.waitFrame)
			{
				value.waitTime = time + value.minTimeVisible;
			}
			if (!flag2)
			{
				flag2 = time < value.waitTime;
			}
			if (flag2 != (value.isVisible != 0))
			{
				if (value.callback != 0)
				{
					changed.Add(slot);
				}
				else
				{
					value.isVisible = (byte)(flag2 ? 1 : 0);
				}
			}
			states[slot] = value;
			num += value.isVisible;
		}
		return num;
	}

	private static int ProcessOccludees_Fast(OccludeeState.State[] states, int[] bucket, int bucketCount, Color32[] results, int resultCount, int[] changed, ref int changedCount, Vector4[] frustumPlanes, float time, uint frame)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < bucketCount; i++)
		{
			int num2 = bucket[i];
			if (num2 < 0 || num2 >= resultCount || states[num2].active == 0)
			{
				continue;
			}
			OccludeeState.State state = states[num2];
			bool flag = FrustumCull(frustumPlanes, state.sphereBounds);
			bool flag2 = results[num2].r > 0 && flag;
			if (flag2 || frame < state.waitFrame)
			{
				state.waitTime = time + state.minTimeVisible;
			}
			if (!flag2)
			{
				flag2 = time < state.waitTime;
			}
			if (flag2 != (state.isVisible != 0))
			{
				if (state.callback != 0)
				{
					changed[changedCount++] = num2;
				}
				else
				{
					state.isVisible = (byte)(flag2 ? 1 : 0);
				}
			}
			states[num2] = state;
			num += ((!flag2) ? 1 : 0);
		}
		return num;
	}

	[DllImport("Renderer", EntryPoint = "CULL_ProcessOccludees")]
	private static extern int ProcessOccludees_Native(ref OccludeeState.State states, ref int bucket, int bucketCount, ref Color32 results, int resultCount, ref int changed, ref int changedCount, ref Vector4 frustumPlanes, float time, uint frame);

	private void ApplyVisibility_Safe(float time, uint frame)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		bool ready = staticSet.Ready;
		bool ready2 = dynamicSet.Ready;
		for (int i = 0; i < grid.Size; i++)
		{
			Cell cell = grid[i];
			if (cell == null || gridSet.resultData.Length == 0)
			{
				continue;
			}
			bool flag = FrustumCull(frustumPlanes, cell.sphereBounds);
			bool flag2 = gridSet.resultData[i].r > 0 && flag;
			if (cell.isVisible || flag2)
			{
				int num = 0;
				int num2 = 0;
				if (ready && cell.staticBucket.Count > 0)
				{
					num = ProcessOccludees_Safe(staticStates, cell.staticBucket, staticSet.resultData, staticVisibilityChanged, frustumPlanes, time, frame);
				}
				if (ready2 && cell.dynamicBucket.Count > 0)
				{
					num2 = ProcessOccludees_Safe(dynamicStates, cell.dynamicBucket, dynamicSet.resultData, dynamicVisibilityChanged, frustumPlanes, time, frame);
				}
				cell.isVisible = flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count;
			}
		}
	}

	private void ApplyVisibility_Fast(float time, uint frame)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		bool ready = staticSet.Ready;
		bool ready2 = dynamicSet.Ready;
		for (int i = 0; i < grid.Size; i++)
		{
			Cell cell = grid[i];
			if (cell == null || gridSet.resultData.Length == 0)
			{
				continue;
			}
			bool flag = FrustumCull(frustumPlanes, cell.sphereBounds);
			bool flag2 = gridSet.resultData[i].r > 0 && flag;
			if (cell.isVisible || flag2)
			{
				int num = 0;
				int num2 = 0;
				if (ready && cell.staticBucket.Count > 0)
				{
					num = ProcessOccludees_Fast(staticStates.array, cell.staticBucket.Slots, cell.staticBucket.Size, staticSet.resultData, staticSet.resultData.Length, staticVisibilityChanged.array, ref staticVisibilityChanged.count, frustumPlanes, time, frame);
				}
				if (ready2 && cell.dynamicBucket.Count > 0)
				{
					num2 = ProcessOccludees_Fast(dynamicStates.array, cell.dynamicBucket.Slots, cell.dynamicBucket.Size, dynamicSet.resultData, dynamicSet.resultData.Length, dynamicVisibilityChanged.array, ref dynamicVisibilityChanged.count, frustumPlanes, time, frame);
				}
				cell.isVisible = flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count;
			}
		}
	}

	private void ApplyVisibility_Native(float time, uint frame)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		bool ready = staticSet.Ready;
		bool ready2 = dynamicSet.Ready;
		for (int i = 0; i < grid.Size; i++)
		{
			Cell cell = grid[i];
			if (cell == null || gridSet.resultData.Length == 0)
			{
				continue;
			}
			bool flag = FrustumCull(frustumPlanes, cell.sphereBounds);
			bool flag2 = gridSet.resultData[i].r > 0 && flag;
			if (cell.isVisible || flag2)
			{
				int num = 0;
				int num2 = 0;
				if (ready && cell.staticBucket.Count > 0)
				{
					num = ProcessOccludees_Native(ref staticStates.array[0], ref cell.staticBucket.Slots[0], cell.staticBucket.Size, ref staticSet.resultData[0], staticSet.resultData.Length, ref staticVisibilityChanged.array[0], ref staticVisibilityChanged.count, ref frustumPlanes[0], time, frame);
				}
				if (ready2 && cell.dynamicBucket.Count > 0)
				{
					num2 = ProcessOccludees_Native(ref dynamicStates.array[0], ref cell.dynamicBucket.Slots[0], cell.dynamicBucket.Size, ref dynamicSet.resultData[0], dynamicSet.resultData.Length, ref dynamicVisibilityChanged.array[0], ref dynamicVisibilityChanged.count, ref frustumPlanes[0], time, frame);
				}
				cell.isVisible = flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count;
			}
		}
	}

	private void ProcessCallbacks(SimpleList<OccludeeState> occludees, SimpleList<OccludeeState.State> states, SimpleList<int> changed)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		for (int i = 0; i < changed.Count; i++)
		{
			int num = changed[i];
			OccludeeState occludeeState = occludees[num];
			if (occludeeState != null)
			{
				bool flag = states.array[num].isVisible == 0;
				OnVisibilityChanged onVisibilityChanged = occludeeState.onVisibilityChanged;
				if (onVisibilityChanged != null && (Object)onVisibilityChanged.Target != (Object)null)
				{
					onVisibilityChanged(flag);
				}
				if (occludeeState.slot >= 0)
				{
					states.array[occludeeState.slot].isVisible = (byte)(flag ? 1 : 0);
				}
			}
		}
		changed.Clear();
	}

	public void RetrieveAndApplyVisibility()
	{
		if (staticOccludees.Count > 0)
		{
			Profiler.BeginSample("GetStaticResults");
			staticSet.GetResults();
			Profiler.EndSample();
		}
		if (dynamicOccludees.Count > 0)
		{
			Profiler.BeginSample("GetDynamicResults");
			dynamicSet.GetResults();
			Profiler.EndSample();
		}
		if (grid.Count > 0)
		{
			Profiler.BeginSample("GetGridResults");
			gridSet.GetResults();
			Profiler.EndSample();
		}
		if (debugSettings.showAllVisible)
		{
			for (int i = 0; i < staticSet.resultData.Length; i++)
			{
				staticSet.resultData[i].r = 1;
			}
			for (int j = 0; j < dynamicSet.resultData.Length; j++)
			{
				dynamicSet.resultData[j].r = 1;
			}
			for (int k = 0; k < gridSet.resultData.Length; k++)
			{
				gridSet.resultData[k].r = 1;
			}
		}
		staticVisibilityChanged.EnsureCapacity(staticOccludees.Count);
		dynamicVisibilityChanged.EnsureCapacity(dynamicOccludees.Count);
		float time = Time.time;
		uint frameCount = (uint)Time.frameCount;
		Profiler.BeginSample("ApplyVisibility");
		if (useNativePath)
		{
			Profiler.BeginSample("NativePath");
			ApplyVisibility_Native(time, frameCount);
			Profiler.EndSample();
		}
		else
		{
			Profiler.BeginSample("CSharpPath");
			ApplyVisibility_Fast(time, frameCount);
			Profiler.EndSample();
		}
		Profiler.EndSample();
		Profiler.BeginSample("OnVisibilityChanged");
		ProcessCallbacks(staticOccludees, staticStates, staticVisibilityChanged);
		ProcessCallbacks(dynamicOccludees, dynamicStates, dynamicVisibilityChanged);
		Profiler.EndSample();
	}
}
