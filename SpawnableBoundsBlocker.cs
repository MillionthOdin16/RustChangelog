using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SpawnableBoundsBlocker : MonoBehaviour
{
	public BoundsCheck.BlockType BlockType;

	public BoxCollider BoxCollider = null;

	[Button("Clear Trees")]
	public void ClearTrees()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		List<TreeEntity> list = Pool.GetList<TreeEntity>();
		if ((Object)(object)BoxCollider != (Object)null)
		{
			GamePhysics.OverlapOBB<TreeEntity>(new OBB(((Component)this).transform.TransformPoint(BoxCollider.center), BoxCollider.size + Vector3.one, ((Component)this).transform.rotation), list, 1073741824, (QueryTriggerInteraction)2);
		}
		foreach (TreeEntity item in list)
		{
			BoundsCheck boundsCheck = PrefabAttribute.server.Find<BoundsCheck>(item.prefabID);
			if (boundsCheck != null && boundsCheck.IsType == BlockType)
			{
				item.Kill();
			}
		}
		Pool.FreeList<TreeEntity>(ref list);
	}
}
