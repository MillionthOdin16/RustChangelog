using System;
using System.Collections;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

public class FrankensteinPet : BasePet, IAISenses, IAIAttack
{
	[Header("Frankenstein")]
	[ServerVar(Help = "How long before a Frankenstein Pet dies un controlled and not asleep on table")]
	public static float decayminutes = 180f;

	[Header("Audio")]
	public SoundDefinition AttackVocalSFX = null;

	private float nextAttackTime = 0f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("FrankensteinPet.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!base.isClient)
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)TickDecay, Random.Range(30f, 60f), 60f, 6f);
		}
	}

	public IEnumerator DelayEquipWeapon(ItemDefinition item, float delay)
	{
		yield return (object)new WaitForSeconds(delay);
		if (!((Object)(object)inventory == (Object)null) && inventory.containerBelt != null && !((Object)(object)item == (Object)null))
		{
			inventory.GiveItem(ItemManager.Create(item, 1, 0uL), inventory.containerBelt);
			EquipWeapon();
		}
	}

	private void TickDecay()
	{
		BasePlayer basePlayer = BasePlayer.FindByID(base.OwnerID);
		if ((!((Object)(object)basePlayer != (Object)null) || basePlayer.IsSleeping()) && !(base.healthFraction <= 0f) && !base.IsDestroyed)
		{
			float num = 1f / decayminutes;
			float amount = MaxHealth() * num;
			Hurt(amount, DamageType.Decay, this, useProtection: false);
		}
	}

	public float EngagementRange()
	{
		AttackEntity attackEntity = GetAttackEntity();
		if (Object.op_Implicit((Object)(object)attackEntity))
		{
			return attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f) * base.Brain.AttackRangeMultiplier;
		}
		return base.Brain.SenseRange;
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
		if (((Component)entity).gameObject.layer == 21 || ((Component)entity).gameObject.layer == 8)
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
		Profiler.BeginSample("FrankensteinPet.CanSeeTarget");
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
		return Time.realtimeSinceStartup < nextAttackTime;
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
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)target == (Object)null))
		{
			Vector3 val = target.ServerPosition - ServerPosition;
			float magnitude = ((Vector3)(ref val)).magnitude;
			if (magnitude > 0.001f)
			{
				ServerRotation = Quaternion.LookRotation(((Vector3)(ref val)).normalized);
			}
			target.Hurt(BaseAttackDamge, AttackDamageType, this);
			SignalBroadcast(Signal.Attack);
			ClientRPC(null, "OnAttack");
			nextAttackTime = Time.realtimeSinceStartup + CooldownDuration();
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
		Profiler.BeginSample("FrankensteinPet.GetBestTarget");
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
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Create corpse", 0);
		try
		{
			NPCPlayerCorpse nPCPlayerCorpse = DropCorpse("assets/rust.ai/agents/NPCPlayer/pet/frankensteinpet_corpse.prefab") as NPCPlayerCorpse;
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
				ItemContainer[] containers = nPCPlayerCorpse.containers;
				foreach (ItemContainer itemContainer in containers)
				{
					itemContainer.Clear();
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
		return "Frankenstein";
	}
}
