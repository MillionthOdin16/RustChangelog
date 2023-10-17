using UnityEngine;

public class DeployVolumeEntityBounds : DeployVolume
{
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	protected override bool Check(Vector3 position, Quaternion rotation, int mask = -1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		position += rotation * ((Bounds)(ref bounds)).center;
		OBB obb = default(OBB);
		((OBB)(ref obb))._002Ector(position, ((Bounds)(ref bounds)).size, rotation);
		if (DeployVolume.CheckOBB(obb, LayerMask.op_Implicit(layers) & mask, this))
		{
			return true;
		}
		return false;
	}

	protected override bool Check(Vector3 position, Quaternion rotation, OBB obb, int mask = -1)
	{
		return false;
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		bounds = rootObj.GetComponent<BaseEntity>().bounds;
	}
}
