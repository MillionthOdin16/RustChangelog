using System;
using System.Collections;
using ConVar;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public class NPCPlayer : BasePlayer
{
	public AIInformationZone VirtualInfoZone;

	public Vector3 finalDestination;

	[NonSerialized]
	private float randomOffset;

	[NonSerialized]
	private Vector3 spawnPos;

	public PlayerInventoryProperties[] loadouts;

	public LayerMask movementMask = LayerMask.op_Implicit(429990145);

	public bool LegacyNavigation = true;

	public NavMeshAgent NavAgent;

	public float damageScale = 1f;

	public float shortRange = 10f;

	public float attackLengthMaxShortRangeScale = 1f;

	private bool _isDormant;

	protected float lastGunShotTime = 0f;

	private float triggerEndTime = 0f;

	protected float nextTriggerTime = 0f;

	private float lastThinkTime = 0f;

	private float lastPositionUpdateTime = 0f;

	private float lastMovementTickTime = 0f;

	private Vector3 lastPos;

	private float lastThrowTime = 0f;

	public override bool IsNpc => true;

	public virtual bool IsDormant
	{
		get
		{
			return _isDormant;
		}
		set
		{
			_isDormant = value;
			if (!_isDormant)
			{
			}
		}
	}

	protected override float PositionTickRate => 0.1f;

	public virtual bool IsOnNavMeshLink
	{
		get
		{
			if (IsNavRunning())
			{
				return NavAgent.isOnOffMeshLink;
			}
			return false;
		}
	}

	public virtual bool HasPath
	{
		get
		{
			if (IsNavRunning())
			{
				return NavAgent.hasPath;
			}
			return false;
		}
	}

	public virtual bool IsLoadBalanced()
	{
		return false;
	}

	public override void ServerInit()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		spawnPos = GetPosition();
		randomOffset = Random.Range(0f, 1f);
		base.ServerInit();
		UpdateNetworkGroup();
		EquipLoadout(loadouts);
		if (!IsLoadBalanced())
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)ServerThink_Internal, 0f, 0.1f);
			lastThinkTime = Time.time;
		}
		((FacepunchBehaviour)this).Invoke((Action)EquipTest, 0.25f);
		finalDestination = ((Component)this).transform.position;
		if ((Object)(object)NavAgent == (Object)null)
		{
			NavAgent = ((Component)this).GetComponent<NavMeshAgent>();
		}
		if (Object.op_Implicit((Object)(object)NavAgent))
		{
			NavAgent.updateRotation = false;
			NavAgent.updatePosition = false;
			if (!LegacyNavigation)
			{
				((Component)((Component)this).transform).gameObject.GetComponent<BaseNavigator>().Init(this, NavAgent);
			}
		}
		((FacepunchBehaviour)this).InvokeRandomized((Action)TickMovement, 1f, PositionTickRate, PositionTickRate * 0.1f);
	}

	public void EquipLoadout(PlayerInventoryProperties[] loads)
	{
		if (loads != null && loads.Length != 0)
		{
			loads[Random.Range(0, loads.Length)].GiveToPlayer(this);
		}
	}

	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		ServerPosition = BaseNpc.GetNewNavPosWithVelocity(this, velocity);
	}

	public void RandomMove()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		float num = 8f;
		Vector2 val = Random.insideUnitCircle * num;
		SetDestination(spawnPos + new Vector3(val.x, 0f, val.y));
	}

	public virtual void SetDestination(Vector3 newDestination)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		finalDestination = newDestination;
	}

	public AttackEntity GetAttackEntity()
	{
		HeldEntity heldEntity = GetHeldEntity();
		return heldEntity as AttackEntity;
	}

	public BaseProjectile GetGun()
	{
		HeldEntity heldEntity = GetHeldEntity();
		AttackEntity attackEntity = heldEntity as AttackEntity;
		if ((Object)(object)attackEntity == (Object)null)
		{
			return null;
		}
		BaseProjectile baseProjectile = attackEntity as BaseProjectile;
		if (Object.op_Implicit((Object)(object)baseProjectile))
		{
			return baseProjectile;
		}
		return null;
	}

	public virtual float AmmoFractionRemaining()
	{
		AttackEntity attackEntity = GetAttackEntity();
		if (Object.op_Implicit((Object)(object)attackEntity))
		{
			return attackEntity.AmmoFraction();
		}
		return 0f;
	}

	public virtual bool IsReloading()
	{
		AttackEntity attackEntity = GetAttackEntity();
		if (!Object.op_Implicit((Object)(object)attackEntity))
		{
			return false;
		}
		return attackEntity.ServerIsReloading();
	}

	public virtual void AttemptReload()
	{
		AttackEntity attackEntity = GetAttackEntity();
		if (!((Object)(object)attackEntity == (Object)null) && attackEntity.CanReload())
		{
			attackEntity.ServerReload();
		}
	}

	public virtual bool ShotTest(float targetDist)
	{
		HeldEntity heldEntity = GetHeldEntity();
		AttackEntity attackEntity = heldEntity as AttackEntity;
		if ((Object)(object)attackEntity == (Object)null)
		{
			return false;
		}
		BaseProjectile baseProjectile = attackEntity as BaseProjectile;
		if (Object.op_Implicit((Object)(object)baseProjectile))
		{
			if (baseProjectile.primaryMagazine.contents <= 0)
			{
				baseProjectile.ServerReload();
				return false;
			}
			if (baseProjectile.NextAttackTime > Time.time)
			{
				return false;
			}
		}
		if (!Mathf.Approximately(attackEntity.attackLengthMin, -1f))
		{
			if (((FacepunchBehaviour)this).IsInvoking((Action)TriggerDown))
			{
				return true;
			}
			if (Time.time < nextTriggerTime)
			{
				return true;
			}
			((FacepunchBehaviour)this).InvokeRepeating((Action)TriggerDown, 0f, 0.01f);
			if (targetDist <= shortRange)
			{
				triggerEndTime = Time.time + Random.Range(attackEntity.attackLengthMin, attackEntity.attackLengthMax * attackLengthMaxShortRangeScale);
			}
			else
			{
				triggerEndTime = Time.time + Random.Range(attackEntity.attackLengthMin, attackEntity.attackLengthMax);
			}
			TriggerDown();
			return true;
		}
		attackEntity.ServerUse(damageScale);
		lastGunShotTime = Time.time;
		return true;
	}

	public virtual float GetAimConeScale()
	{
		return 1f;
	}

	public void CancelBurst(float delay = 0.2f)
	{
		if (triggerEndTime > Time.time + delay)
		{
			triggerEndTime = Time.time + delay;
		}
	}

	public bool MeleeAttack()
	{
		HeldEntity heldEntity = GetHeldEntity();
		AttackEntity attackEntity = heldEntity as AttackEntity;
		if ((Object)(object)attackEntity == (Object)null)
		{
			return false;
		}
		BaseMelee baseMelee = attackEntity as BaseMelee;
		if ((Object)(object)baseMelee == (Object)null)
		{
			return false;
		}
		baseMelee.ServerUse(damageScale);
		return true;
	}

	public virtual void TriggerDown()
	{
		HeldEntity heldEntity = GetHeldEntity();
		AttackEntity attackEntity = heldEntity as AttackEntity;
		if ((Object)(object)attackEntity != (Object)null)
		{
			attackEntity.ServerUse(damageScale);
		}
		lastGunShotTime = Time.time;
		if (Time.time > triggerEndTime)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)TriggerDown);
			nextTriggerTime = Time.time + (((Object)(object)attackEntity != (Object)null) ? attackEntity.attackSpacing : 1f);
		}
	}

	public virtual void EquipWeapon(bool skipDeployDelay = false)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)inventory == (Object)null || inventory.containerBelt == null)
		{
			return;
		}
		Item slot = inventory.containerBelt.GetSlot(0);
		if (slot != null)
		{
			UpdateActiveItem(inventory.containerBelt.GetSlot(0).uid);
			BaseEntity heldEntity = slot.GetHeldEntity();
			if ((Object)(object)heldEntity != (Object)null)
			{
				AttackEntity component = ((Component)heldEntity).GetComponent<AttackEntity>();
				if ((Object)(object)component != (Object)null)
				{
					if (skipDeployDelay)
					{
						component.ResetAttackCooldown();
					}
					component.TopUpAmmo();
				}
			}
		}
		if (!skipDeployDelay)
		{
		}
	}

	public void EquipTest()
	{
		EquipWeapon(skipDeployDelay: true);
	}

	internal void ServerThink_Internal()
	{
		float delta = Time.time - lastThinkTime;
		ServerThink(delta);
		lastThinkTime = Time.time;
	}

	public virtual void ServerThink(float delta)
	{
		TickAi(delta);
	}

	public virtual void Resume()
	{
	}

	public virtual bool IsNavRunning()
	{
		return false;
	}

	public virtual void TickAi(float delta)
	{
	}

	public void TickMovement()
	{
		float delta = Time.realtimeSinceStartup - lastMovementTickTime;
		lastMovementTickTime = Time.realtimeSinceStartup;
		MovementUpdate(delta);
	}

	public override float GetNetworkTime()
	{
		float num = Time.realtimeSinceStartup - lastPositionUpdateTime;
		if (num > PositionTickRate * 2f)
		{
			return Time.time;
		}
		return lastPositionUpdateTime;
	}

	public virtual void MovementUpdate(float delta)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		if (!LegacyNavigation || base.isClient || !IsAlive() || IsWounded() || (!base.isMounted && !IsNavRunning()))
		{
			return;
		}
		if (IsDormant || !syncPosition)
		{
			Profiler.BeginSample("NPCPlayer.MovementUpdate.AssignDestination");
			if (IsNavRunning())
			{
				NavAgent.destination = ServerPosition;
			}
			Profiler.EndSample();
			return;
		}
		Vector3 moveToPosition = ((Component)this).transform.position;
		if (HasPath)
		{
			moveToPosition = NavAgent.nextPosition;
		}
		if (ValidateNextPosition(ref moveToPosition))
		{
			UpdateSpeed(delta);
			UpdatePositionAndRotation(moveToPosition);
		}
	}

	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("ValidBounds.Test");
		bool flag = ValidBounds.Test(moveToPosition);
		Profiler.EndSample();
		if (!flag && (Object)(object)((Component)this).transform != (Object)null && !base.IsDestroyed)
		{
			Debug.Log((object)string.Concat("Invalid NavAgent Position: ", this, " ", ((object)(Vector3)(ref moveToPosition)).ToString(), " (destroying)"));
			Kill();
			return false;
		}
		return true;
	}

	private void UpdateSpeed(float delta)
	{
		float num = DesiredMoveSpeed();
		NavAgent.speed = Mathf.Lerp(NavAgent.speed, num, delta * 8f);
	}

	protected virtual void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		lastPositionUpdateTime = Time.time;
		ServerPosition = moveToPosition;
		SetAimDirection(GetAimDirection());
	}

	public Vector3 GetPosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position;
	}

	public virtual float DesiredMoveSpeed()
	{
		float running = Mathf.Sin(Time.time + randomOffset);
		return GetSpeed(running, 0f, 0f);
	}

	public override bool EligibleForWounding(HitInfo info)
	{
		return false;
	}

	public virtual Vector3 GetAimDirection()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3Ex.Distance2D(finalDestination, GetPosition());
		if (num >= 1f)
		{
			Vector3 val = finalDestination - GetPosition();
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			return new Vector3(normalized.x, 0f, normalized.z);
		}
		return eyes.BodyForward();
	}

	public virtual void SetAimDirection(Vector3 newAim)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!(newAim == Vector3.zero))
		{
			AttackEntity attackEntity = GetAttackEntity();
			if (Object.op_Implicit((Object)(object)attackEntity))
			{
				newAim = attackEntity.ModifyAIAim(newAim);
			}
			eyes.rotation = Quaternion.LookRotation(newAim, Vector3.up);
			Quaternion rotation = eyes.rotation;
			viewAngles = ((Quaternion)(ref rotation)).eulerAngles;
			ServerRotation = eyes.rotation;
			lastPositionUpdateTime = Time.time;
		}
	}

	public bool TryUseThrownWeapon(BaseEntity target, float attackRate)
	{
		if (HasThrownItemCooldown())
		{
			return false;
		}
		Item item = FindThrownWeapon();
		if (item == null)
		{
			lastThrowTime = Time.time;
			return false;
		}
		return TryUseThrownWeapon(item, target, attackRate);
	}

	public bool TryUseThrownWeapon(Item item, BaseEntity target, float attackRate)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (HasThrownItemCooldown())
		{
			return false;
		}
		float num = Vector3.Distance(((Component)target).transform.position, ((Component)this).transform.position);
		if (num <= 2f || num >= 20f)
		{
			return false;
		}
		Vector3 position = ((Component)target).transform.position;
		if (!IsVisible(CenterPoint(), position))
		{
			return false;
		}
		if (UseThrownWeapon(item, target))
		{
			if (this is ScarecrowNPC)
			{
				ScarecrowNPC.NextBeanCanAllowedTime = Time.time + Halloween.scarecrow_throw_beancan_global_delay;
			}
			lastThrowTime = Time.time;
			return true;
		}
		return false;
	}

	public bool HasThrownItemCooldown()
	{
		return Time.time - lastThrowTime < 10f;
	}

	protected bool UseThrownWeapon(Item item, BaseEntity target)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		UpdateActiveItem(item.uid);
		ThrownWeapon thrownWeapon = GetActiveItem().GetHeldEntity() as ThrownWeapon;
		if ((Object)(object)thrownWeapon == (Object)null)
		{
			return false;
		}
		((MonoBehaviour)this).StartCoroutine(DoThrow(thrownWeapon, target));
		return true;
	}

	private IEnumerator DoThrow(ThrownWeapon thrownWeapon, BaseEntity target)
	{
		modelState.aiming = true;
		yield return (object)new WaitForSeconds(1.5f);
		SetAimDirection(Vector3Ex.Direction(((Component)target).transform.position, ((Component)this).transform.position));
		thrownWeapon.ResetAttackCooldown();
		thrownWeapon.ServerThrow(((Component)target).transform.position);
		modelState.aiming = false;
		((FacepunchBehaviour)this).Invoke((Action)EquipTest, 0.5f);
	}

	public Item FindThrownWeapon()
	{
		if ((Object)(object)inventory == (Object)null || inventory.containerBelt == null)
		{
			return null;
		}
		for (int i = 0; i < inventory.containerBelt.capacity; i++)
		{
			Item slot = inventory.containerBelt.GetSlot(i);
			if (slot != null)
			{
				ThrownWeapon thrownWeapon = slot.GetHeldEntity() as ThrownWeapon;
				if ((Object)(object)thrownWeapon != (Object)null)
				{
					return slot;
				}
			}
		}
		return null;
	}
}
