using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

public class PatrolHelicopter : BaseCombatEntity, SeekerTarget.ISeekerTargetOwner
{
	[Serializable]
	public class weakspot
	{
		[NonSerialized]
		public PatrolHelicopter body;

		public string[] bonenames;

		public float maxHealth;

		public float health;

		public float healthFractionOnDestroyed = 0.5f;

		public GameObjectRef destroyedParticles;

		public GameObjectRef damagedParticles;

		public GameObject damagedEffect;

		public GameObject destroyedEffect;

		public List<BasePlayer> attackers;

		private bool isDestroyed;

		public float HealthFraction()
		{
			return health / maxHealth;
		}

		public void Hurt(float amount, HitInfo info)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			if (!isDestroyed)
			{
				health -= amount;
				Effect.server.Run(damagedParticles.resourcePath, body, StringPool.Get(bonenames[Random.Range(0, bonenames.Length)]), Vector3.zero, Vector3.up, null, broadcast: true);
				if (health <= 0f)
				{
					health = 0f;
					WeakspotDestroyed();
				}
			}
		}

		public void Heal(float amount)
		{
			health += amount;
		}

		public void WeakspotDestroyed()
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			isDestroyed = true;
			Effect.server.Run(destroyedParticles.resourcePath, body, StringPool.Get(bonenames[Random.Range(0, bonenames.Length)]), Vector3.zero, Vector3.up, null, broadcast: true);
			body.Hurt(body.MaxHealth() * healthFractionOnDestroyed, DamageType.Generic, null, useProtection: false);
		}
	}

	public GameObject rotorPivot;

	public GameObject mainRotor;

	public GameObject mainRotor_blades;

	public GameObject mainRotor_blur;

	public GameObject tailRotor;

	public GameObject tailRotor_blades;

	public GameObject tailRotor_blur;

	public GameObject rocket_tube_left;

	public GameObject rocket_tube_right;

	public GameObject left_gun_yaw;

	public GameObject left_gun_pitch;

	public GameObject left_gun_muzzle;

	public GameObject right_gun_yaw;

	public GameObject right_gun_pitch;

	public GameObject right_gun_muzzle;

	public GameObject spotlight_rotation;

	public GameObjectRef rocket_fire_effect;

	public GameObjectRef gun_fire_effect;

	public GameObjectRef bulletEffect;

	public GameObjectRef explosionEffect;

	public GameObjectRef fireBall;

	public GameObjectRef crateToDrop;

	public int maxCratesToSpawn = 4;

	public float bulletSpeed = 250f;

	public float bulletDamage = 20f;

	public GameObjectRef servergibs;

	public GameObjectRef debrisFieldMarker;

	public float flareDuration = 5f;

	public SoundDefinition rotorWashSoundDef;

	private Sound _rotorWashSound;

	public SoundDefinition flightEngineSoundDef;

	public SoundDefinition flightThwopsSoundDef;

	private Sound flightEngineSound;

	private Sound flightThwopsSound;

	private SoundModulation.Modulator flightEngineGainMod;

	private SoundModulation.Modulator flightThwopsGainMod;

	public float rotorGainModSmoothing = 0.25f;

	public float engineGainMin = 0.5f;

	public float engineGainMax = 1f;

	public float thwopGainMin = 0.5f;

	public float thwopGainMax = 1f;

	public float spotlightJitterAmount = 5f;

	public float spotlightJitterSpeed = 5f;

	public GameObject[] nightLights;

	public Vector3 spotlightTarget;

	public float engineSpeed = 1f;

	public float targetEngineSpeed = 1f;

	public float blur_rotationScale = 0.05f;

	public ParticleSystem[] _rotorWashParticles;

	private PatrolHelicopterAI myAI;

	public GameObjectRef mapMarkerEntityPrefab;

	private float lastNetworkUpdate = float.NegativeInfinity;

	private const float networkUpdateRate = 0.25f;

	private BaseEntity mapMarkerInstance;

	public weakspot[] weakspots;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PatrolHelicopter.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override float MaxVelocity()
	{
		return 100f;
	}

	public override void InitShared()
	{
		base.InitShared();
		InitalizeWeakspots();
	}

	public override void Load(LoadInfo info)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.helicopter != null)
		{
			spotlightTarget = info.msg.helicopter.spotlightVec;
		}
	}

	public void RadarLock(SeekingServerProjectile incoming)
	{
		if (!((FacepunchBehaviour)this).IsInvoking((Action)CancelRadar))
		{
			SetFlag(Flags.Reserved8, b: true);
			((FacepunchBehaviour)this).Invoke((Action)CancelRadar, 5f);
		}
	}

	public void CancelRadar()
	{
		SetFlag(Flags.Reserved8, b: false);
	}

	public override void Save(SaveInfo info)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.helicopter = Pool.Get<Helicopter>();
		Helicopter helicopter = info.msg.helicopter;
		Quaternion localRotation = rotorPivot.transform.localRotation;
		helicopter.tiltRot = ((Quaternion)(ref localRotation)).eulerAngles;
		info.msg.helicopter.spotlightVec = spotlightTarget;
		info.msg.helicopter.weakspothealths = Pool.Get<List<float>>();
		for (int i = 0; i < weakspots.Length; i++)
		{
			info.msg.helicopter.weakspothealths.Add(weakspots[i].health);
		}
	}

	public override void ServerInit()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		myAI = ((Component)this).GetComponent<PatrolHelicopterAI>();
		if (!myAI.hasInterestZone)
		{
			myAI.SetInitialDestination(Vector3.zero, 1.25f);
			myAI.targetThrottleSpeed = 1f;
			myAI.ExitCurrentState();
			myAI.State_Patrol_Enter();
		}
		CreateMapMarker();
		SeekerTarget.SetSeekerTarget(this, SeekerTarget.SeekerStrength.MEDIUM);
	}

	public override void DestroyShared()
	{
		if (base.isServer)
		{
			SeekerTarget.SetSeekerTarget(this, SeekerTarget.SeekerStrength.OFF);
		}
		base.DestroyShared();
	}

	public void CreateMapMarker()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)mapMarkerInstance))
		{
			mapMarkerInstance.Kill();
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity);
		baseEntity.SetParent(this);
		baseEntity.Spawn();
		mapMarkerInstance = baseEntity;
	}

	public override void OnPositionalNetworkUpdate()
	{
		SendNetworkUpdate();
		base.OnPositionalNetworkUpdate();
	}

	public void CreateExplosionMarker(float durationMinutes)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GameManager.server.CreateEntity(debrisFieldMarker.resourcePath, ((Component)this).transform.position, Quaternion.identity);
		baseEntity.Spawn();
		((Component)baseEntity).SendMessage("SetDuration", (object)durationMinutes, (SendMessageOptions)1);
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		CreateExplosionMarker(10f);
		Effect.server.Run(explosionEffect.resourcePath, ((Component)this).transform.position, Vector3.up, null, broadcast: true);
		Vector3 val = myAI.GetLastMoveDir() * myAI.GetMoveSpeed() * 0.75f;
		GameObject gibSource = servergibs.Get().GetComponent<ServerGib>()._gibSource;
		List<ServerGib> list = ServerGib.CreateGibs(servergibs.resourcePath, ((Component)this).gameObject, gibSource, val, 3f);
		if (info.damageTypes.GetMajorityDamageType() != DamageType.Decay)
		{
			for (int i = 0; i < 12 - maxCratesToSpawn; i++)
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireBall.resourcePath, ((Component)this).transform.position, ((Component)this).transform.rotation);
				if (!Object.op_Implicit((Object)(object)baseEntity))
				{
					continue;
				}
				float num = 3f;
				float num2 = 10f;
				Vector3 onUnitSphere = Random.onUnitSphere;
				((Component)baseEntity).transform.position = ((Component)this).transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere * Random.Range(-4f, 4f);
				Collider component = ((Component)baseEntity).GetComponent<Collider>();
				baseEntity.Spawn();
				baseEntity.SetVelocity(val + onUnitSphere * Random.Range(num, num2));
				foreach (ServerGib item in list)
				{
					Physics.IgnoreCollision(component, (Collider)(object)item.GetCollider(), true);
				}
			}
		}
		for (int j = 0; j < maxCratesToSpawn; j++)
		{
			Vector3 onUnitSphere2 = Random.onUnitSphere;
			Vector3 pos = ((Component)this).transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere2 * Random.Range(2f, 3f);
			BaseEntity baseEntity2 = GameManager.server.CreateEntity(crateToDrop.resourcePath, pos, Quaternion.LookRotation(onUnitSphere2));
			baseEntity2.Spawn();
			LootContainer lootContainer = baseEntity2 as LootContainer;
			if (Object.op_Implicit((Object)(object)lootContainer))
			{
				((FacepunchBehaviour)lootContainer).Invoke((Action)lootContainer.RemoveMe, 1800f);
			}
			Collider component2 = ((Component)baseEntity2).GetComponent<Collider>();
			Rigidbody val2 = ((Component)baseEntity2).gameObject.AddComponent<Rigidbody>();
			val2.useGravity = true;
			val2.collisionDetectionMode = (CollisionDetectionMode)2;
			val2.mass = 2f;
			val2.interpolation = (RigidbodyInterpolation)1;
			val2.velocity = val + onUnitSphere2 * Random.Range(1f, 3f);
			val2.angularVelocity = Vector3Ex.Range(-1.75f, 1.75f);
			val2.drag = 0.5f * (val2.mass / 5f);
			val2.angularDrag = 0.2f * (val2.mass / 5f);
			FireBall fireBall = GameManager.server.CreateEntity(this.fireBall.resourcePath) as FireBall;
			if (Object.op_Implicit((Object)(object)fireBall))
			{
				fireBall.SetParent(baseEntity2);
				fireBall.Spawn();
				((Component)fireBall).GetComponent<Rigidbody>().isKinematic = true;
				((Component)fireBall).GetComponent<Collider>().enabled = false;
			}
			((Component)baseEntity2).SendMessage("SetLockingEnt", (object)((Component)fireBall).gameObject, (SendMessageOptions)1);
			foreach (ServerGib item2 in list)
			{
				Physics.IgnoreCollision(component2, (Collider)(object)item2.GetCollider(), true);
			}
		}
		base.OnKilled(info);
	}

	public bool IsValidHomingTarget()
	{
		return true;
	}

	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		base.OnEntityMessage(from, msg);
		if (msg == "RadarLock" && !((FacepunchBehaviour)this).IsInvoking((Action)DoFlare))
		{
			((FacepunchBehaviour)this).Invoke((Action)DoFlare, Random.Range(0.5f, 1f));
		}
	}

	public void DoFlare()
	{
		SetFlag(Flags.OnFire, b: true);
		SeekerTarget.SetSeekerTarget(this, SeekerTarget.SeekerStrength.OFF);
		((FacepunchBehaviour)this).Invoke((Action)ClearFlares, flareDuration);
	}

	public void ClearFlares()
	{
		SetFlag(Flags.OnFire, b: false);
		SeekerTarget.SetSeekerTarget(this, SeekerTarget.SeekerStrength.MEDIUM);
	}

	public void Update()
	{
		if (base.isServer && Time.realtimeSinceStartup - lastNetworkUpdate >= 0.25f)
		{
			SendNetworkUpdate();
			lastNetworkUpdate = Time.realtimeSinceStartup;
		}
	}

	public void InitalizeWeakspots()
	{
		weakspot[] array = weakspots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].body = this;
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (base.isServer)
		{
			myAI.WasAttacked(info);
		}
	}

	public override void Hurt(HitInfo info)
	{
		bool flag = false;
		if (info.damageTypes.Total() >= base.health)
		{
			base.health = 1000000f;
			myAI.CriticalDamage();
			flag = true;
		}
		base.Hurt(info);
		if (flag)
		{
			return;
		}
		weakspot[] array = weakspots;
		foreach (weakspot weakspot in array)
		{
			string[] bonenames = weakspot.bonenames;
			foreach (string str in bonenames)
			{
				if (info.HitBone == StringPool.Get(str))
				{
					weakspot.Hurt(info.damageTypes.Total(), info);
					myAI.WeakspotDamaged(weakspot, info);
				}
			}
		}
	}
}
