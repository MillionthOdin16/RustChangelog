using System.Collections.Generic;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

public class BasePet : NPCPlayer, IThinker
{
	public static Dictionary<ulong, BasePet> ActivePetByOwnerID = new Dictionary<ulong, BasePet>();

	[ServerVar]
	public static bool queuedMovementsAllowed = true;

	[ServerVar]
	public static bool onlyQueueBaseNavMovements = true;

	[ServerVar]
	[Help("How many miliseconds to budget for processing pet movements per frame")]
	public static float movementupdatebudgetms = 1f;

	public float BaseAttackRate = 2f;

	public float BaseAttackDamge = 20f;

	public DamageType AttackDamageType = DamageType.Slash;

	public GameObjectRef mapMarkerPrefab;

	private BaseEntity _mapMarkerInstance;

	[HideInInspector]
	public bool inQueue = false;

	public static Queue<BasePet> _movementProcessQueue = new Queue<BasePet>();

	public PetBrain Brain { get; protected set; }

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
		return _maxHealth;
	}

	public static void ProcessMovementQueue()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = movementupdatebudgetms / 1000f;
		while (_movementProcessQueue.Count > 0 && Time.realtimeSinceStartup < realtimeSinceStartup + num)
		{
			BasePet basePet = _movementProcessQueue.Dequeue();
			if ((Object)(object)basePet != (Object)null)
			{
				basePet.DoBudgetedMoveUpdate();
				basePet.inQueue = false;
			}
		}
	}

	public void DoBudgetedMoveUpdate()
	{
		if ((Object)(object)Brain != (Object)null)
		{
			Brain.DoMovementTick();
		}
	}

	public override bool IsLoadBalanced()
	{
		return true;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		Brain = ((Component)this).GetComponent<PetBrain>();
		if (!base.isClient)
		{
			AIThinkManager.AddPet(this);
		}
	}

	public void CreateMapMarker()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_mapMarkerInstance != (Object)null)
		{
			_mapMarkerInstance.Kill();
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(mapMarkerPrefab?.resourcePath, Vector3.zero, Quaternion.identity);
		baseEntity.OwnerID = base.OwnerID;
		baseEntity.Spawn();
		baseEntity.SetParent(this);
		_mapMarkerInstance = baseEntity;
	}

	internal override void DoServerDestroy()
	{
		if ((Object)(object)Brain.OwningPlayer != (Object)null)
		{
			Brain.OwningPlayer.ClearClientPetLink();
		}
		AIThinkManager.RemovePet(this);
		base.DoServerDestroy();
	}

	public virtual void TryThink()
	{
		Profiler.BeginSample("BasePet.TryThink");
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

	public void ApplyPetStatModifiers()
	{
		if ((Object)(object)inventory == (Object)null)
		{
			return;
		}
		for (int i = 0; i < inventory.containerWear.capacity; i++)
		{
			Item slot = inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				ItemModPetStats component = ((Component)slot.info).GetComponent<ItemModPetStats>();
				if ((Object)(object)component != (Object)null)
				{
					component.Apply(this);
				}
			}
		}
		Heal(MaxHealth());
	}

	private void OnPhysicsNeighbourChanged()
	{
		if ((Object)(object)Brain != (Object)null && (Object)(object)Brain.Navigator != (Object)null)
		{
			Brain.Navigator.ForceToGround();
		}
	}
}
