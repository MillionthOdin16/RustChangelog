using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Profiling;

public class HumanNPC : NPCPlayer, IAISenses, IAIAttack, IThinker
{
	[Header("LOS")]
	public int AdditionalLosBlockingLayer = 0;

	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	[Header("Damage")]
	public float aimConeScale = 2f;

	public float lastDismountTime = 0f;

	[NonSerialized]
	protected bool lightsOn = false;

	private float nextZoneSearchTime = 0f;

	private AIInformationZone cachedInfoZone = null;

	private float targetAimedDuration = 0f;

	private float lastAimSetTime = 0f;

	private Vector3 aimOverridePosition = Vector3.zero;

	public ScientistBrain Brain { get; private set; }

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

	public override bool IsLoadBalanced()
	{
		return true;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		Brain = ((Component)this).GetComponent<ScientistBrain>();
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

	public void LightCheck()
	{
		if ((TOD_Sky.Instance.IsNight && !lightsOn) || (TOD_Sky.Instance.IsDay && lightsOn))
		{
			LightToggle();
			lightsOn = !lightsOn;
		}
	}

	public override float GetAimConeScale()
	{
		return aimConeScale;
	}

	public override void EquipWeapon(bool skipDeployDelay = false)
	{
		base.EquipWeapon(skipDeployDelay);
	}

	public override void DismountObject()
	{
		base.DismountObject();
		lastDismountTime = Time.time;
	}

	public bool RecentlyDismounted()
	{
		return Time.time < lastDismountTime + 10f;
	}

	public virtual float GetIdealDistanceFromTarget()
	{
		return Mathf.Max(5f, EngagementRange() * 0.75f);
	}

	public AIInformationZone GetInformationZone(Vector3 pos)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)VirtualInfoZone != (Object)null)
		{
			return VirtualInfoZone;
		}
		if ((Object)(object)cachedInfoZone == (Object)null || Time.time > nextZoneSearchTime)
		{
			cachedInfoZone = AIInformationZone.GetForPoint(pos);
			nextZoneSearchTime = Time.time + 5f;
		}
		return cachedInfoZone;
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

	public void SetDucked(bool flag)
	{
		modelState.ducked = flag;
		SendNetworkUpdate();
	}

	public virtual void TryThink()
	{
		Profiler.BeginSample("HumanNPCNew.TryThink");
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

	public void TickAttack(float delta, BaseCombatEntity target, bool targetIsLOS)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)target == (Object)null)
		{
			return;
		}
		Vector3 val = eyes.BodyForward();
		Vector3 val2 = target.CenterPoint() - eyes.position;
		float num = Vector3.Dot(val, ((Vector3)(ref val2)).normalized);
		if (targetIsLOS)
		{
			if (num > 0.2f)
			{
				targetAimedDuration += delta;
			}
		}
		else
		{
			if (num < 0.5f)
			{
				targetAimedDuration = 0f;
			}
			CancelBurst();
		}
		if (targetAimedDuration >= 0.2f && targetIsLOS)
		{
			bool flag = false;
			float dist = 0f;
			if (this != null)
			{
				flag = ((IAIAttack)this).IsTargetInRange((BaseEntity)target, out dist);
			}
			else
			{
				AttackEntity attackEntity = GetAttackEntity();
				if (Object.op_Implicit((Object)(object)attackEntity))
				{
					dist = (((Object)(object)target != (Object)null) ? Vector3.Distance(((Component)this).transform.position, ((Component)target).transform.position) : (-1f));
					flag = dist < attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f);
				}
			}
			if (flag)
			{
				ShotTest(dist);
			}
		}
		else
		{
			CancelBurst();
		}
	}

	public override void Hurt(HitInfo info)
	{
		if (base.isMounted)
		{
			info.damageTypes.ScaleAll(0.1f);
		}
		base.Hurt(info);
		BaseEntity initiator = info.Initiator;
		if ((Object)(object)initiator != (Object)null && !initiator.EqualNetID((BaseNetworkable)this))
		{
			Brain.Senses.Memory.SetKnown(initiator, this, null);
		}
	}

	public float GetAimSwayScalar()
	{
		return 1f - Mathf.InverseLerp(1f, 3f, Time.time - lastGunShotTime);
	}

	public override Vector3 GetAimDirection()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Brain != (Object)null && (Object)(object)Brain.Navigator != (Object)null && Brain.Navigator.IsOverridingFacingDirection)
		{
			return Brain.Navigator.FacingDirectionOverride;
		}
		return base.GetAimDirection();
	}

	public override void SetAimDirection(Vector3 newAim)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		if (newAim == Vector3.zero)
		{
			return;
		}
		float num = Time.time - lastAimSetTime;
		lastAimSetTime = Time.time;
		AttackEntity attackEntity = GetAttackEntity();
		if (Object.op_Implicit((Object)(object)attackEntity))
		{
			newAim = attackEntity.ModifyAIAim(newAim, GetAimSwayScalar());
		}
		Quaternion val;
		if (base.isMounted)
		{
			BaseMountable baseMountable = GetMounted();
			Vector3 eulerAngles = ((Component)baseMountable).transform.eulerAngles;
			val = Quaternion.LookRotation(newAim, ((Component)baseMountable).transform.up);
			Quaternion val2 = Quaternion.Euler(((Quaternion)(ref val)).eulerAngles);
			Vector3 val3 = ((Component)this).transform.InverseTransformDirection(val2 * Vector3.forward);
			Quaternion val4 = Quaternion.LookRotation(val3, ((Component)this).transform.up);
			Vector3 eulerAngles2 = ((Quaternion)(ref val4)).eulerAngles;
			eulerAngles2 = BaseMountable.ConvertVector(eulerAngles2);
			Quaternion val5 = Quaternion.Euler(Mathf.Clamp(eulerAngles2.x, baseMountable.pitchClamp.x, baseMountable.pitchClamp.y), Mathf.Clamp(eulerAngles2.y, baseMountable.yawClamp.x, baseMountable.yawClamp.y), eulerAngles.z);
			Vector3 val6 = ((Component)this).transform.TransformDirection(val5 * Vector3.forward);
			Quaternion val7 = Quaternion.LookRotation(val6, ((Component)this).transform.up);
			Vector3 eulerAngles3 = ((Quaternion)(ref val7)).eulerAngles;
			newAim = BaseMountable.ConvertVector(eulerAngles3);
		}
		else
		{
			BaseEntity baseEntity = GetParentEntity();
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				Vector3 val8 = ((Component)baseEntity).transform.InverseTransformDirection(newAim);
				Vector3 val9 = default(Vector3);
				((Vector3)(ref val9))._002Ector(newAim.x, val8.y, newAim.z);
				eyes.rotation = Quaternion.Lerp(eyes.rotation, Quaternion.LookRotation(val9, ((Component)baseEntity).transform.up), num * 25f);
				val = eyes.bodyRotation;
				viewAngles = ((Quaternion)(ref val)).eulerAngles;
				ServerRotation = eyes.bodyRotation;
				return;
			}
		}
		eyes.rotation = (base.isMounted ? Quaternion.Slerp(eyes.rotation, Quaternion.Euler(newAim), num * 70f) : Quaternion.Lerp(eyes.rotation, Quaternion.LookRotation(newAim, ((Component)this).transform.up), num * 25f));
		val = eyes.rotation;
		viewAngles = ((Quaternion)(ref val)).eulerAngles;
		ServerRotation = eyes.rotation;
	}

	public void SetStationaryAimPoint(Vector3 aimAt)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		aimOverridePosition = aimAt;
	}

	public void ClearStationaryAimPoint()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		aimOverridePosition = Vector3.zero;
	}

	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	public override BaseCorpse CreateCorpse()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Create corpse", 0);
		try
		{
			NPCPlayerCorpse nPCPlayerCorpse = DropCorpse("assets/prefabs/npc/scientist/scientist_corpse.prefab") as NPCPlayerCorpse;
			if (Object.op_Implicit((Object)(object)nPCPlayerCorpse))
			{
				((Component)nPCPlayerCorpse).transform.position = ((Component)nPCPlayerCorpse).transform.position + Vector3.down * NavAgent.baseOffset;
				nPCPlayerCorpse.SetLootableIn(2f);
				nPCPlayerCorpse.SetFlag(Flags.Reserved5, HasPlayerFlag(PlayerFlags.DisplaySash));
				nPCPlayerCorpse.SetFlag(Flags.Reserved2, b: true);
				nPCPlayerCorpse.TakeFrom(inventory.containerMain, inventory.containerWear, inventory.containerBelt);
				nPCPlayerCorpse.playerName = OverrideCorpseName();
				nPCPlayerCorpse.playerSteamID = userID;
				nPCPlayerCorpse.Spawn();
				nPCPlayerCorpse.TakeChildren(this);
				for (int i = 0; i < nPCPlayerCorpse.containers.Length; i++)
				{
					ItemContainer itemContainer = nPCPlayerCorpse.containers[i];
					if (i != 1)
					{
						itemContainer.Clear();
					}
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

	protected virtual string OverrideCorpseName()
	{
		return base.displayName;
	}

	public override void AttackerInfo(DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}

	public bool IsThreat(BaseEntity entity)
	{
		return IsTarget(entity);
	}

	public bool IsTarget(BaseEntity entity)
	{
		if (entity is BasePlayer && !entity.IsNpc)
		{
			return true;
		}
		if (entity is BasePet)
		{
			return true;
		}
		if (entity is ScarecrowNPC)
		{
			return true;
		}
		return false;
	}

	public bool IsFriendly(BaseEntity entity)
	{
		if ((Object)(object)entity == (Object)null)
		{
			return false;
		}
		return entity.prefabID == prefabID;
	}

	public bool CanAttack(BaseEntity entity)
	{
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
		BasePlayer basePlayer = entity as BasePlayer;
		if ((Object)(object)basePlayer == (Object)null)
		{
			return true;
		}
		if (AdditionalLosBlockingLayer == 0)
		{
			return IsPlayerVisibleToUs(basePlayer, 1218519041);
		}
		return IsPlayerVisibleToUs(basePlayer, 0x48A12001 | (1 << AdditionalLosBlockingLayer));
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
		return 5f;
	}

	public bool IsOnCooldown()
	{
		return false;
	}

	public bool StartAttacking(BaseEntity entity)
	{
		return true;
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
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("HumanNPC.GetBestTarget");
		BaseEntity result = null;
		float num = -1f;
		List<BaseEntity> players = Brain.Senses.Players;
		foreach (BaseEntity item in players)
		{
			if (!((Object)(object)item == (Object)null) && !(item.Health() <= 0f))
			{
				float num2 = Vector3.Distance(((Component)item).transform.position, ((Component)this).transform.position);
				float num3 = 1f - Mathf.InverseLerp(1f, Brain.SenseRange, num2);
				Vector3 val = ((Component)item).transform.position - eyes.position;
				float num4 = Vector3.Dot(((Vector3)(ref val)).normalized, eyes.BodyForward());
				num3 += Mathf.InverseLerp(Brain.VisionCone, 1f, num4) / 2f;
				num3 += (Brain.Senses.Memory.IsLOS(item) ? 2f : 0f);
				if (num3 > num)
				{
					result = item;
					num = num3;
				}
			}
		}
		Profiler.EndSample();
		return result;
	}

	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
		BaseCombatEntity target2 = target as BaseCombatEntity;
		TickAttack(delta, target2, targetIsLOS);
	}

	public void UseHealingItem(Item item)
	{
		((MonoBehaviour)this).StartCoroutine(Heal(item));
	}

	private IEnumerator Heal(Item item)
	{
		UpdateActiveItem(item.uid);
		Item activeItem = GetActiveItem();
		MedicalTool heldItem = activeItem.GetHeldEntity() as MedicalTool;
		if (!((Object)(object)heldItem == (Object)null))
		{
			yield return (object)new WaitForSeconds(1f);
			heldItem.ServerUse();
			Heal(MaxHealth());
			yield return (object)new WaitForSeconds(2f);
			EquipWeapon();
		}
	}

	public Item FindHealingItem()
	{
		if ((Object)(object)Brain == (Object)null)
		{
			return null;
		}
		if (!Brain.CanUseHealingItems)
		{
			return null;
		}
		if ((Object)(object)inventory == (Object)null || inventory.containerBelt == null)
		{
			return null;
		}
		for (int i = 0; i < inventory.containerBelt.capacity; i++)
		{
			Item slot = inventory.containerBelt.GetSlot(i);
			if (slot != null && slot.amount > 1)
			{
				MedicalTool medicalTool = slot.GetHeldEntity() as MedicalTool;
				if ((Object)(object)medicalTool != (Object)null)
				{
					return slot;
				}
			}
		}
		return null;
	}

	public override bool IsOnGround()
	{
		return true;
	}
}
