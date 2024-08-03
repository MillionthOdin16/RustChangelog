using UnityEngine;

public class ConstructionSocket_Elevator : ConstructionSocket
{
	public int MaxFloor = 5;

	protected override bool CanConnectToEntity(Construction.Target target)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (target.entity is Elevator elevator && elevator.Floor >= MaxFloor)
		{
			return false;
		}
		Vector3 val = target.GetWorldPosition();
		Quaternion val2 = target.GetWorldRotation(female: true);
		OBB obb = default(OBB);
		((OBB)(ref obb))._002Ector(val, new Vector3(2f, 0.5f, 2f), val2);
		if (GamePhysics.CheckOBB(obb, 2097152, (QueryTriggerInteraction)0))
		{
			return false;
		}
		return base.CanConnectToEntity(target);
	}

	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		Matrix4x4 val = Matrix4x4.TRS(position, rotation, Vector3.one);
		Vector3 val2 = ((Matrix4x4)(ref val)).MultiplyPoint3x4(worldPosition);
		OBB obb = default(OBB);
		((OBB)(ref obb))._002Ector(val2, new Vector3(2f, 0.5f, 2f), rotation);
		return !GamePhysics.CheckOBB(obb, 2097152, (QueryTriggerInteraction)0);
	}
}
