using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

public class DecayEntity : BaseCombatEntity
{
	[Serializable]
	public struct DebrisPosition
	{
		public Vector3 Position;

		public Vector3 Rotation;

		public bool dropToTerrain;
	}

	public GameObjectRef debrisPrefab;

	public Vector3 debrisRotationOffset = Vector3.zero;

	public DebrisPosition[] DebrisPositions;

	[NonSerialized]
	public uint buildingID;

	private float decayTimer;

	private float upkeepTimer;

	private Upkeep upkeep;

	private Decay decay;

	private DecayPoint[] decayPoints;

	private float lastDecayTick;

	private float decayVariance = 1f;

	public Upkeep Upkeep => upkeep;

	public virtual bool BypassInsideDecayMultiplier => false;

	public virtual bool AllowOnCargoShip => false;

	public override void ResetState()
	{
		base.ResetState();
		buildingID = 0u;
		if (base.isServer)
		{
			decayTimer = 0f;
		}
	}

	public void AttachToBuilding(uint id)
	{
		if (base.isServer)
		{
			BuildingManager.server.Remove(this);
			buildingID = id;
			BuildingManager.server.Add(this);
			SendNetworkUpdate();
		}
	}

	public BuildingManager.Building GetBuilding()
	{
		if (base.isServer)
		{
			return BuildingManager.server.GetBuilding(buildingID);
		}
		return null;
	}

	public override BuildingPrivlidge GetBuildingPrivilege()
	{
		BuildingManager.Building building = GetBuilding();
		if (building != null)
		{
			return building.GetDominatingBuildingPrivilege();
		}
		return base.GetBuildingPrivilege();
	}

	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts, float multiplier)
	{
		if (upkeep == null)
		{
			return;
		}
		float num = upkeep.upkeepMultiplier * multiplier;
		if (num == 0f)
		{
			return;
		}
		List<ItemAmount> list = BuildCost();
		if (list == null)
		{
			return;
		}
		foreach (ItemAmount item in list)
		{
			if (item.itemDef.category != ItemCategory.Resources)
			{
				continue;
			}
			float num2 = item.amount * num;
			bool flag = false;
			foreach (ItemAmount itemAmount in itemAmounts)
			{
				if ((Object)(object)itemAmount.itemDef == (Object)(object)item.itemDef)
				{
					itemAmount.amount += num2;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				itemAmounts.Add(new ItemAmount(item.itemDef, num2));
			}
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		decayVariance = Random.Range(0.95f, 1f);
		decay = PrefabAttribute.server.Find<Decay>(prefabID);
		decayPoints = PrefabAttribute.server.FindAll<DecayPoint>(prefabID);
		upkeep = PrefabAttribute.server.Find<Upkeep>(prefabID);
		BuildingManager.server.Add(this);
		if (!Application.isLoadingSave)
		{
			BuildingManager.server.CheckMerge(this);
		}
		lastDecayTick = Time.time;
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		BuildingManager.server.Remove(this);
		BuildingManager.server.CheckSplit(this);
	}

	public virtual void AttachToBuilding(DecayEntity other)
	{
		if ((Object)(object)other != (Object)null)
		{
			AttachToBuilding(other.buildingID);
			BuildingManager.server.CheckMerge(this);
			return;
		}
		BuildingBlock nearbyBuildingBlock = GetNearbyBuildingBlock();
		if (Object.op_Implicit((Object)(object)nearbyBuildingBlock))
		{
			AttachToBuilding(nearbyBuildingBlock.buildingID);
		}
	}

	public BuildingBlock GetNearbyBuildingBlock()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		BuildingBlock result = null;
		Vector3 position = PivotPoint();
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities(position, 1.5f, list, 2097152, (QueryTriggerInteraction)2);
		for (int i = 0; i < list.Count; i++)
		{
			BuildingBlock buildingBlock = list[i];
			if (buildingBlock.isServer == base.isServer)
			{
				float num2 = buildingBlock.SqrDistance(position);
				if (!buildingBlock.grounded)
				{
					num2 += 1f;
				}
				if (num2 < num)
				{
					num = num2;
					result = buildingBlock;
				}
			}
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return result;
	}

	public void ResetUpkeepTime()
	{
		upkeepTimer = 0f;
	}

	public void DecayTouch()
	{
		decayTimer = 0f;
	}

	public void AddUpkeepTime(float time)
	{
		upkeepTimer -= time;
	}

	public float GetProtectedSeconds()
	{
		return Mathf.Max(0f, 0f - upkeepTimer);
	}

	public virtual float GetEntityDecayDuration()
	{
		return decay.GetDecayDuration(this);
	}

	public virtual float GetEntityHealScale()
	{
		return decay.GetHealScale(this);
	}

	public virtual float GetEntityDecayDelay()
	{
		return decay.GetDecayDelay(this);
	}

	public virtual void DecayTick()
	{
		if (decay == null)
		{
			return;
		}
		float num = decay.GetDecayTickOverride();
		if (num == 0f)
		{
			num = ConVar.Decay.tick;
		}
		float num2 = Time.time - lastDecayTick;
		if (num2 < num)
		{
			return;
		}
		lastDecayTick = Time.time;
		if (HasParent() || !decay.ShouldDecay(this))
		{
			return;
		}
		float num3 = num2 * ConVar.Decay.scale;
		if (ConVar.Decay.upkeep)
		{
			upkeepTimer += num3;
			if (upkeepTimer > 0f)
			{
				BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
				if ((Object)(object)buildingPrivilege != (Object)null)
				{
					upkeepTimer -= buildingPrivilege.PurchaseUpkeepTime(this, Mathf.Max(upkeepTimer, 600f));
				}
			}
			if (upkeepTimer < 1f)
			{
				if (base.healthFraction < 1f && GetEntityHealScale() > 0f && base.SecondsSinceAttacked > 600f)
				{
					float num4 = num2 / GetEntityDecayDuration() * GetEntityHealScale();
					Heal(MaxHealth() * num4);
				}
				return;
			}
			upkeepTimer = 1f;
		}
		decayTimer += num3;
		if (decayTimer < GetEntityDecayDelay())
		{
			return;
		}
		TimeWarning val = TimeWarning.New("DecayTick", 0);
		try
		{
			float num5 = 1f;
			if (ConVar.Decay.upkeep)
			{
				if (!BypassInsideDecayMultiplier && !IsOutside())
				{
					num5 *= ConVar.Decay.upkeep_inside_decay_scale;
				}
			}
			else
			{
				for (int i = 0; i < decayPoints.Length; i++)
				{
					DecayPoint decayPoint = decayPoints[i];
					if (decayPoint.IsOccupied(this))
					{
						num5 -= decayPoint.protection;
					}
				}
			}
			if (num5 > 0f)
			{
				float num6 = num3 / GetEntityDecayDuration() * MaxHealth();
				Hurt(num6 * num5 * decayVariance, DamageType.Decay);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void OnRepairFinished()
	{
		base.OnRepairFinished();
		DecayTouch();
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (debrisPrefab.isValid)
		{
			if (DebrisPositions != null && DebrisPositions.Length != 0)
			{
				DebrisPosition[] debrisPositions = DebrisPositions;
				for (int i = 0; i < debrisPositions.Length; i++)
				{
					DebrisPosition debrisPosition = debrisPositions[i];
					SpawnDebris(debrisPosition.Position, Quaternion.Euler(debrisPosition.Rotation), debrisPosition.dropToTerrain);
				}
			}
			else
			{
				SpawnDebris(Vector3.zero, Quaternion.Euler(debrisRotationOffset), dropToTerrain: false);
			}
		}
		base.OnKilled(info);
	}

	private void SpawnDebris(Vector3 localPos, Quaternion rot, bool dropToTerrain)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.TransformPoint(localPos);
		RaycastHit val2 = default(RaycastHit);
		if (dropToTerrain && Physics.Raycast(val, Vector3.down, ref val2, 6f, 8388608))
		{
			float num = val.y - ((RaycastHit)(ref val2)).point.y;
			val.y = ((RaycastHit)(ref val2)).point.y;
			localPos.y -= num;
		}
		List<DebrisEntity> list = Pool.GetList<DebrisEntity>();
		Vis.Entities(val, 0.1f, list, 256, (QueryTriggerInteraction)2);
		if (list.Count <= 0)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(debrisPrefab.resourcePath, ((Component)this).transform.TransformPoint(localPos), ((Component)this).transform.rotation * rot);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.SetParent(parentEntity.Get(serverside: true), worldPositionStays: true);
				baseEntity.Spawn();
			}
		}
	}

	public override bool SupportsChildDeployables()
	{
		BaseEntity baseEntity = GetParentEntity();
		if (!((Object)(object)baseEntity != (Object)null))
		{
			return false;
		}
		return baseEntity.ForceDeployableSetParent();
	}

	public override bool ForceDeployableSetParent()
	{
		BaseEntity baseEntity = GetParentEntity();
		if (!((Object)(object)baseEntity != (Object)null))
		{
			return false;
		}
		return baseEntity.ForceDeployableSetParent();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.decayEntity = Pool.Get<DecayEntity>();
		info.msg.decayEntity.buildingID = buildingID;
		if (info.forDisk)
		{
			info.msg.decayEntity.decayTimer = decayTimer;
			info.msg.decayEntity.upkeepTimer = upkeepTimer;
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.decayEntity == null)
		{
			return;
		}
		decayTimer = info.msg.decayEntity.decayTimer;
		upkeepTimer = info.msg.decayEntity.upkeepTimer;
		if (buildingID != info.msg.decayEntity.buildingID)
		{
			AttachToBuilding(info.msg.decayEntity.buildingID);
			if (info.fromDisk)
			{
				BuildingManager.server.LoadBuildingID(buildingID);
			}
		}
	}
}
