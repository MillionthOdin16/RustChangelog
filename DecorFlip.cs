using UnityEngine;

public class DecorFlip : DecorComponent
{
	public enum AxisType
	{
		X,
		Y,
		Z
	}

	public AxisType FlipAxis = AxisType.Y;

	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		uint num = SeedEx.Seed(pos, World.Seed) + 4;
		if (!(SeedRandom.Value(ref num) > 0.5f))
		{
			switch (FlipAxis)
			{
			case AxisType.X:
			case AxisType.Z:
				rot = Quaternion.AngleAxis(180f, rot * Vector3.up) * rot;
				break;
			case AxisType.Y:
				rot = Quaternion.AngleAxis(180f, rot * Vector3.forward) * rot;
				break;
			}
		}
	}
}
