using System.Collections.Generic;
using UnityEngine;

public class SubsurfaceProfileTexture
{
	private struct SubsurfaceProfileEntry
	{
		public SubsurfaceProfileData data;

		public SubsurfaceProfile profile;

		public SubsurfaceProfileEntry(SubsurfaceProfileData data, SubsurfaceProfile profile)
		{
			this.data = data;
			this.profile = profile;
		}
	}

	public const int SUBSURFACE_PROFILE_COUNT = 16;

	public const int MAX_SUBSURFACE_PROFILES = 15;

	public const int SUBSURFACE_RADIUS_SCALE = 1024;

	public const int SUBSURFACE_KERNEL_SIZE = 3;

	private HashSet<SubsurfaceProfile> entries = new HashSet<SubsurfaceProfile>();

	private Texture2D texture;

	private Vector4[] transmissionTints = (Vector4[])(object)new Vector4[16];

	private const int KernelSize0 = 24;

	private const int KernelSize1 = 16;

	private const int KernelSize2 = 8;

	private const int KernelTotalSize = 49;

	private const int Width = 49;

	public Texture2D Texture
	{
		get
		{
			if ((Object)(object)texture == (Object)null)
			{
				CreateResources();
			}
			return texture;
		}
	}

	public Vector4[] TransmissionTints
	{
		get
		{
			if ((Object)(object)texture == (Object)null)
			{
				CreateResources();
			}
			return transmissionTints;
		}
	}

	public void AddProfile(SubsurfaceProfile profile)
	{
		entries.Add(profile);
		if (entries.Count > 15)
		{
			Debug.LogWarning((object)$"[SubsurfaceScattering] Maximum number of supported Subsurface Profiles has been reached ({entries.Count}/{15}). Please remove some.");
		}
		ReleaseResources();
	}

	public static Color Clamp(Color color, float min = 0f, float max = 1f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Color result = default(Color);
		result.r = Mathf.Clamp(color.r, min, max);
		result.g = Mathf.Clamp(color.g, min, max);
		result.b = Mathf.Clamp(color.b, min, max);
		result.a = Mathf.Clamp(color.a, min, max);
		return result;
	}

	private void WriteKernel(ref Color[] pixels, ref Color[] kernel, int id, int y, in SubsurfaceProfileData data)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		Color val = Clamp(data.SubsurfaceColor);
		Color falloffColor = Clamp(data.FalloffColor, 0.009f);
		transmissionTints[id] = Color.op_Implicit(data.TransmissionTint);
		kernel[0] = val;
		kernel[0].a = data.ScatterRadius;
		SeparableSSS.CalculateKernel(kernel, 1, 24, val, falloffColor);
		SeparableSSS.CalculateKernel(kernel, 25, 16, val, falloffColor);
		SeparableSSS.CalculateKernel(kernel, 41, 8, val, falloffColor);
		int num = 49 * y;
		for (int i = 0; i < 49; i++)
		{
			Color val2 = kernel[i];
			val2.a *= ((i > 0) ? (data.ScatterRadius / 1024f) : 1f);
			pixels[num + i] = val2;
		}
	}

	private void CreateResources()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		if (entries.Count <= 0)
		{
			return;
		}
		int num = Mathf.Min(entries.Count, 15) + 1;
		ReleaseResources();
		texture = new Texture2D(49, num, (TextureFormat)17, false, true);
		((Object)texture).name = "SubsurfaceProfiles";
		((Texture)texture).wrapMode = (TextureWrapMode)1;
		((Texture)texture).filterMode = (FilterMode)1;
		Color[] pixels = texture.GetPixels(0);
		Color[] kernel = (Color[])(object)new Color[49];
		int num2 = num - 1;
		int id = 0;
		int id2 = id++;
		int y = num2--;
		SubsurfaceProfileData data = SubsurfaceProfileData.Default;
		WriteKernel(ref pixels, ref kernel, id2, y, in data);
		foreach (SubsurfaceProfile entry in entries)
		{
			entry.Id = id;
			WriteKernel(ref pixels, ref kernel, id++, num2--, in entry.Data);
			if (num2 < 0)
			{
				break;
			}
		}
		texture.SetPixels(pixels, 0);
		texture.Apply(false, false);
	}

	public void ReleaseResources()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)texture != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)texture);
			texture = null;
		}
		if (transmissionTints != null)
		{
			for (int i = 0; i < transmissionTints.Length; i++)
			{
				Vector4[] array = transmissionTints;
				int num = i;
				SubsurfaceProfileData @default = SubsurfaceProfileData.Default;
				array[num] = Color.op_Implicit(((Color)(ref @default.TransmissionTint)).linear);
			}
		}
	}
}
