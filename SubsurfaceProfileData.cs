using System;
using UnityEngine;

[Serializable]
public struct SubsurfaceProfileData
{
	[Range(0.1f, 100f)]
	public float ScatterRadius;

	[ColorUsage(false, false)]
	public Color SubsurfaceColor;

	[ColorUsage(false, false)]
	public Color FalloffColor;

	[ColorUsage(false, true)]
	public Color TransmissionTint;

	public static SubsurfaceProfileData Default
	{
		get
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			SubsurfaceProfileData result = default(SubsurfaceProfileData);
			result.ScatterRadius = 1.2f;
			result.SubsurfaceColor = new Color(0.48f, 0.41f, 0.28f);
			result.FalloffColor = new Color(1f, 0.37f, 0.3f);
			result.TransmissionTint = new Color(0.48f, 0.41f, 0.28f);
			return result;
		}
	}

	public static SubsurfaceProfileData Invalid
	{
		get
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			SubsurfaceProfileData result = default(SubsurfaceProfileData);
			result.ScatterRadius = 0f;
			result.SubsurfaceColor = Color.clear;
			result.FalloffColor = Color.clear;
			result.TransmissionTint = Color.clear;
			return result;
		}
	}
}
