using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class DeployVolumeEntityBoundsReverse : DeployVolume
{
	private Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	private int layer = 0;

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
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		position += rotation * ((Bounds)(ref bounds)).center;
		OBB test = default(OBB);
		((OBB)(ref test))._002Ector(position, ((Bounds)(ref bounds)).size, rotation);
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(position, ((Vector3)(ref test.extents)).magnitude, list, LayerMask.op_Implicit(layers) & mask, (QueryTriggerInteraction)2);
		foreach (BaseEntity item in list)
		{
			DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(item.prefabID);
			if (DeployVolume.Check(((Component)item).transform.position, ((Component)item).transform.rotation, volumes, test, 1 << layer))
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return false;
	}

	protected override bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1)
	{
		return false;
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		bounds = rootObj.GetComponent<BaseEntity>().bounds;
		layer = rootObj.layer;
	}
}
