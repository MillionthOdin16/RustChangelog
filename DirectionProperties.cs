using System;
using UnityEngine;

public class DirectionProperties : PrefabAttribute
{
	private const float radius = 200f;

	public Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	public ProtectionProperties extraProtection;

	protected override Type GetIndexedType()
	{
		return typeof(DirectionProperties);
	}

	public bool IsWeakspot(Transform tx, HitInfo info)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		if (((Bounds)(ref bounds)).size == Vector3.zero)
		{
			return false;
		}
		BasePlayer initiatorPlayer = info.InitiatorPlayer;
		if ((Object)(object)initiatorPlayer == (Object)null)
		{
			return false;
		}
		BaseEntity hitEntity = info.HitEntity;
		if ((Object)(object)hitEntity == (Object)null)
		{
			return false;
		}
		Matrix4x4 worldToLocalMatrix = tx.worldToLocalMatrix;
		Vector3 val = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint3x4(info.PointStart) - worldPosition;
		float num = Vector3Ex.DotDegrees(worldForward, val);
		Vector3 val2 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint3x4(info.HitPositionWorld);
		OBB val3 = default(OBB);
		((OBB)(ref val3))._002Ector(worldPosition, worldRotation, bounds);
		Vector3 position = initiatorPlayer.eyes.position;
		WeakpointProperties[] array = PrefabAttribute.server.FindAll<WeakpointProperties>(hitEntity.prefabID);
		if (array != null && array.Length != 0)
		{
			bool flag = false;
			WeakpointProperties[] array2 = array;
			foreach (WeakpointProperties weakpointProperties in array2)
			{
				if ((!weakpointProperties.BlockWhenRoofAttached || CheckWeakpointRoof(hitEntity)) && IsWeakspotVisible(hitEntity, position, tx.TransformPoint(weakpointProperties.worldPosition)))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		else if (!IsWeakspotVisible(hitEntity, position, tx.TransformPoint(val3.position)))
		{
			return false;
		}
		return num > 100f && ((OBB)(ref val3)).Contains(val2);
	}

	private bool CheckWeakpointRoof(BaseEntity hitEntity)
	{
		foreach (EntityLink entityLink in hitEntity.GetEntityLinks())
		{
			if (!(entityLink.socket is NeighbourSocket))
			{
				continue;
			}
			foreach (EntityLink connection in entityLink.connections)
			{
				if (connection.owner is BuildingBlock buildingBlock && (buildingBlock.ShortPrefabName == "roof" || buildingBlock.ShortPrefabName == "roof.triangle"))
				{
					return false;
				}
			}
		}
		return true;
	}

	private bool IsWeakspotVisible(BaseEntity hitEntity, Vector3 playerEyes, Vector3 weakspotPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		if (!hitEntity.IsVisible(playerEyes, weakspotPos))
		{
			return false;
		}
		return true;
	}
}
