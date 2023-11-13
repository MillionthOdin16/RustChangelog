using UnityEngine;

public class DeployVolumeOBB : DeployVolume
{
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		position += rotation * (worldRotation * ((Bounds)(ref bounds)).center + worldPosition);
		OBB obb = default(OBB);
		((OBB)(ref obb))._002Ector(position, ((Bounds)(ref bounds)).size, rotation * worldRotation);
		if (DeployVolume.CheckOBB(obb, LayerMask.op_Implicit(layers) & mask, this))
		{
			return true;
		}
		return false;
	}

	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		position += rotation * (worldRotation * ((Bounds)(ref bounds)).center + worldPosition);
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(position, ((Bounds)(ref bounds)).size, rotation * worldRotation);
		if ((LayerMask.op_Implicit(layers) & mask) != 0 && ((OBB)(ref val)).Intersects(test))
		{
			return true;
		}
		return false;
	}
}
