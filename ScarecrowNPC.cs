using System;
using ConVar;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Profiling;

public class ScarecrowNPC : NPCPlayer, IAISenses, IAIAttack, IThinker
{
	public float BaseAttackRate = 2f;

	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	public static float NextBeanCanAllowedTime;

	public bool BlockClothingOnCorpse;

	public bool RoamAroundHomePoint = false;

	public ScarecrowBrain Brain { get; protected set; }

	public override BaseNpc.AiStatistics.FamilyEnum Family => BaseNpc.AiStatistics.FamilyEnum.Murderer;

	public override float StartHealth()
	{
		return startHealth;
	}

	public override float StartMaxHealth()
	{
		return startHealth;
	}

	public override float MaxHealth()
	{
		return startHealth;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		Brain = ((Component)this).GetComponent<ScarecrowBrain>();
		if (!base.isClient)
		{
			AIThinkManager.Add(this);
		}
	}

	internal override void DoServerDestroy()
	{
		AIThinkManager.Remove(this);
		base.DoServerDestroy();
	}

	public virtual void TryThink()
	{
		Profiler.BeginSample("ScarecrowNPC.TryThink");
		ServerThink_Internal();
		Profiler.EndSample();
	}

	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		if (Brain.ShouldServerThink())
		{
			Brain.DoThink();
		}
	}

	public override string Categorize()
	{
		return "Scarecrow";
	}

	public override void EquipWeapon(bool skipDeployDelay = false)
	{
		base.EquipWeapon(skipDeployDelay);
		HeldEntity heldEntity = GetHeldEntity();
		if ((Object)(object)heldEntity != (Object)null && heldEntity is Chainsaw chainsaw)
		{
			chainsaw.ServerNPCStart();
		}
	}

	public float EngagementRange()
	{
		AttackEntity attackEntity = GetAttackEntity();
		if (Object.op_Implicit((Object)(object)attackEntity))
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * Brain.AttackRangeMultiplier;
		}
		return Brain.SenseRange;
	}

	public bool IsThreat(BaseEntity entity)
	{
		return IsTarget(entity);
	}

	public bool IsTarget(BaseEntity entity)
	{
		return entity is BasePlayer && !entity.IsNpc;
	}

	public bool IsFriendly(BaseEntity entity)
	{
		return false;
	}

	public bool CanAttack(BaseEntity entity)
	{
		if ((Object)(object)entity == (Object)null)
		{
			return false;
		}
		if (NeedsToReload())
		{
			return false;
		}
		if (IsOnCooldown())
		{
			return false;
		}
		if (!IsTargetInRange(entity, out var _))
		{
			return false;
		}
		if (InSafeZone() || (entity is BasePlayer basePlayer && basePlayer.InSafeZone()))
		{
			return false;
		}
		if (!CanSeeTarget(entity))
		{
			return false;
		}
		return true;
	}

	public bool IsTargetInRange(BaseEntity entity, out float dist)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		dist = Vector3.Distance(((Component)entity).transform.position, ((Component)this).transform.position);
		return dist <= EngagementRange();
	}

	public bool CanSeeTarget(BaseEntity entity)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)entity == (Object)null)
		{
			return false;
		}
		Profiler.BeginSample("ScarecrowNPC.CanSeeTarget");
		bool result = entity.IsVisible(GetEntity().CenterPoint(), entity.CenterPoint());
		Profiler.EndSample();
		return result;
	}

	public bool NeedsToReload()
	{
		return false;
	}

	public bool Reload()
	{
		return true;
	}

	public float CooldownDuration()
	{
		return BaseAttackRate;
	}

	public bool IsOnCooldown()
	{
		AttackEntity attackEntity = GetAttackEntity();
		if (Object.op_Implicit((Object)(object)attackEntity))
		{
			return attackEntity.HasAttackCooldown();
		}
		return true;
	}

	public bool StartAttacking(BaseEntity target)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		if ((Object)(object)baseCombatEntity == (Object)null)
		{
			return false;
		}
		Attack(baseCombatEntity);
		return true;
	}

	private void Attack(BaseCombatEntity target)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)target == (Object)null))
		{
			Vector3 val = target.ServerPosition - ServerPosition;
			float magnitude = ((Vector3)(ref val)).magnitude;
			if (magnitude > 0.001f)
			{
				ServerRotation = Quaternion.LookRotation(((Vector3)(ref val)).normalized);
			}
			AttackEntity attackEntity = GetAttackEntity();
			if (Object.op_Implicit((Object)(object)attackEntity))
			{
				attackEntity.ServerUse();
			}
		}
	}

	public void StopAttacking()
	{
	}

	public float GetAmmoFraction()
	{
		return AmmoFractionRemaining();
	}

	public BaseEntity GetBestTarget()
	{
		Profiler.BeginSample("ScarecrowNPC.GetBestTarget");
		Profiler.EndSample();
		return null;
	}

	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
	}

	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	public override BaseCorpse CreateCorpse()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Create corpse", 0);
		try
		{
			string strCorpsePrefab = "assets/prefabs/npc/murderer/murderer_corpse.prefab";
			NPCPlayerCorpse nPCPlayerCorpse = DropCorpse(strCorpsePrefab) as NPCPlayerCorpse;
			if (Object.op_Implicit((Object)(object)nPCPlayerCorpse))
			{
				((Component)nPCPlayerCorpse).transform.position = ((Component)nPCPlayerCorpse).transform.position + Vector3.down * NavAgent.baseOffset;
				nPCPlayerCorpse.SetLootableIn(2f);
				nPCPlayerCorpse.SetFlag(Flags.Reserved5, HasPlayerFlag(PlayerFlags.DisplaySash));
				nPCPlayerCorpse.SetFlag(Flags.Reserved2, b: true);
				nPCPlayerCorpse.TakeFrom(inventory.containerMain, inventory.containerWear, inventory.containerBelt);
				nPCPlayerCorpse.playerName = "Scarecrow";
				nPCPlayerCorpse.playerSteamID = userID;
				nPCPlayerCorpse.Spawn();
				ItemContainer[] containers = nPCPlayerCorpse.containers;
				foreach (ItemContainer itemContainer in containers)
				{
					itemContainer.Clear();
				}
				if (LootSpawnSlots.Length != 0)
				{
					LootContainer.LootSpawnSlot[] lootSpawnSlots = LootSpawnSlots;
					for (int j = 0; j < lootSpawnSlots.Length; j++)
					{
						LootContainer.LootSpawnSlot lootSpawnSlot = lootSpawnSlots[j];
						for (int k = 0; k < lootSpawnSlot.numberToSpawn; k++)
						{
							float num = Random.Range(0f, 1f);
							if (num <= lootSpawnSlot.probability)
							{
								lootSpawnSlot.definition.SpawnIntoContainer(nPCPlayerCorpse.containers[0]);
							}
						}
					}
				}
			}
			return nPCPlayerCorpse;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void Hurt(HitInfo info)
	{
		if (!info.isHeadshot)
		{
			if (((Object)(object)info.InitiatorPlayer != (Object)null && !info.InitiatorPlayer.IsNpc) || ((Object)(object)info.InitiatorPlayer == (Object)null && (Object)(object)info.Initiator != (Object)null && info.Initiator.IsNpc))
			{
				info.damageTypes.ScaleAll(Halloween.scarecrow_body_dmg_modifier);
			}
			else
			{
				info.damageTypes.ScaleAll(2f);
			}
		}
		base.Hurt(info);
	}

	public override void AttackerInfo(DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}
}
