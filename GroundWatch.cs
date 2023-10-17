using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public class GroundWatch : BaseMonoBehaviour, IServerComponent
{
	public Vector3 groundPosition = Vector3.zero;

	public LayerMask layers = LayerMask.op_Implicit(161546240);

	public float radius = 0.1f;

	[Header("Whitelist")]
	public BaseEntity[] whitelist;

	private int fails = 0;

	private void OnDrawGizmosSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(groundPosition, radius);
	}

	public static void PhysicsChanged(GameObject obj)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)obj == (Object)null)
		{
			return;
		}
		Collider component = obj.GetComponent<Collider>();
		if (!Object.op_Implicit((Object)(object)component))
		{
			return;
		}
		Bounds bounds = component.bounds;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vector3 center = ((Bounds)(ref bounds)).center;
		Vector3 extents = ((Bounds)(ref bounds)).extents;
		Vis.Entities(center, ((Vector3)(ref extents)).magnitude + 1f, list, 2263296, (QueryTriggerInteraction)2);
		foreach (BaseEntity item in list)
		{
			if (!item.IsDestroyed && !item.isClient && !(item is BuildingBlock))
			{
				Profiler.BeginSample("BroadcastMessage OnPhysicsNeighbourChanged");
				((Component)item).BroadcastMessage("OnPhysicsNeighbourChanged", (SendMessageOptions)1);
				Profiler.EndSample();
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	public static void PhysicsChanged(Vector3 origin, float radius, int layerMask)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(origin, radius, list, layerMask, (QueryTriggerInteraction)2);
		foreach (BaseEntity item in list)
		{
			if (!item.IsDestroyed && !item.isClient && !(item is BuildingBlock))
			{
				Profiler.BeginSample("BroadcastMessage OnPhysicsNeighbourChanged - Deployables");
				((Component)item).BroadcastMessage("OnPhysicsNeighbourChanged", (SendMessageOptions)1);
				Profiler.EndSample();
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	private void OnPhysicsNeighbourChanged()
	{
		if (!OnGround())
		{
			fails++;
			if (fails >= Physics.groundwatchfails)
			{
				BaseEntity baseEntity = ((Component)this).gameObject.ToBaseEntity();
				if (Object.op_Implicit((Object)(object)baseEntity))
				{
					((Component)((Component)baseEntity).transform).BroadcastMessage("OnGroundMissing", (SendMessageOptions)1);
				}
			}
			else
			{
				if (Physics.groundwatchdebug)
				{
					Debug.Log((object)("GroundWatch retry: " + fails));
				}
				((FacepunchBehaviour)this).Invoke((Action)OnPhysicsNeighbourChanged, Physics.groundwatchdelay);
			}
		}
		else
		{
			fails = 0;
		}
	}

	private bool OnGround()
	{
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity component = ((Component)this).GetComponent<BaseEntity>();
		if (Object.op_Implicit((Object)(object)component))
		{
			if (component.HasParent())
			{
				return true;
			}
			Construction construction = PrefabAttribute.server.Find<Construction>(component.prefabID);
			if ((bool)construction)
			{
				Socket_Base[] allSockets = construction.allSockets;
				foreach (Socket_Base socket_Base in allSockets)
				{
					SocketMod[] socketMods = socket_Base.socketMods;
					foreach (SocketMod socketMod in socketMods)
					{
						SocketMod_AreaCheck socketMod_AreaCheck = socketMod as SocketMod_AreaCheck;
						if ((bool)socketMod_AreaCheck && socketMod_AreaCheck.wantsInside && !socketMod_AreaCheck.DoCheck(((Component)component).transform.position, ((Component)component).transform.rotation, component))
						{
							if (Physics.groundwatchdebug)
							{
								Debug.Log((object)("GroundWatch failed: " + socketMod_AreaCheck.hierachyName));
							}
							return false;
						}
					}
				}
			}
		}
		List<Collider> list = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(((Component)this).transform.TransformPoint(groundPosition), radius, list, LayerMask.op_Implicit(layers), (QueryTriggerInteraction)2);
		foreach (Collider item in list)
		{
			BaseEntity baseEntity = ((Component)item).gameObject.ToBaseEntity();
			if (Object.op_Implicit((Object)(object)baseEntity) && ((Object)(object)baseEntity == (Object)(object)component || baseEntity.IsDestroyed || baseEntity.isClient))
			{
				continue;
			}
			if (whitelist != null && whitelist.Length != 0)
			{
				bool flag = false;
				BaseEntity[] array = whitelist;
				foreach (BaseEntity baseEntity2 in array)
				{
					if (baseEntity.prefabID == baseEntity2.prefabID)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
			}
			DecayEntity decayEntity = component as DecayEntity;
			DecayEntity decayEntity2 = baseEntity as DecayEntity;
			if (Object.op_Implicit((Object)(object)decayEntity) && decayEntity.buildingID != 0 && Object.op_Implicit((Object)(object)decayEntity2) && decayEntity2.buildingID != 0 && decayEntity.buildingID != decayEntity2.buildingID)
			{
				continue;
			}
			Pool.FreeList<Collider>(ref list);
			return true;
		}
		if (Physics.groundwatchdebug)
		{
			Debug.Log((object)"GroundWatch failed: Legacy radius check");
		}
		Pool.FreeList<Collider>(ref list);
		return false;
	}
}
