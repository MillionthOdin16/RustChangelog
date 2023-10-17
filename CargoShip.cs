using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

public class CargoShip : BaseEntity
{
	public int targetNodeIndex = -1;

	public GameObject wakeParent;

	public GameObjectRef scientistTurretPrefab;

	public Transform[] scientistSpawnPoints;

	public List<Transform> crateSpawns;

	public GameObjectRef lockedCratePrefab;

	public GameObjectRef militaryCratePrefab;

	public GameObjectRef eliteCratePrefab;

	public GameObjectRef junkCratePrefab;

	public Transform waterLine;

	public Transform rudder;

	public Transform propeller;

	public GameObjectRef escapeBoatPrefab;

	public Transform escapeBoatPoint;

	public GameObjectRef microphonePrefab;

	public Transform microphonePoint;

	public GameObjectRef speakerPrefab;

	public Transform[] speakerPoints;

	public GameObject radiation;

	public GameObjectRef mapMarkerEntityPrefab;

	public GameObject hornOrigin;

	public SoundDefinition hornDef;

	public CargoShipSounds cargoShipSounds;

	public GameObject[] layouts;

	public GameObjectRef playerTest;

	private uint layoutChoice;

	[ServerVar]
	public static bool event_enabled = true;

	[ServerVar]
	public static float event_duration_minutes = 50f;

	[ServerVar]
	public static float egress_duration_minutes = 10f;

	[ServerVar]
	public static int loot_rounds = 3;

	[ServerVar]
	public static float loot_round_spacing_minutes = 10f;

	private BaseEntity mapMarkerInstance;

	private Vector3 currentVelocity = Vector3.zero;

	private float currentThrottle = 0f;

	private float currentTurnSpeed = 0f;

	private float turnScale = 0f;

	private int lootRoundsPassed = 0;

	private int hornCount = 0;

	private float currentRadiation = 0f;

	private bool egressing = false;

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.simpleUint != null)
		{
			layoutChoice = info.msg.simpleUint.value;
		}
	}

	public void RefreshActiveLayout()
	{
		for (int i = 0; i < layouts.Length; i++)
		{
			layouts[i].SetActive(layoutChoice == i);
		}
	}

	public void TriggeredEventSpawn()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = TerrainMeta.RandomPointOffshore();
		val.y = TerrainMeta.WaterMap.GetHeight(val);
		((Component)this).transform.position = val;
		if (!event_enabled || event_duration_minutes == 0f)
		{
			((FacepunchBehaviour)this).Invoke((Action)DelayedDestroy, 1f);
		}
	}

	public void CreateMapMarker()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)mapMarkerInstance))
		{
			mapMarkerInstance.Kill();
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity);
		baseEntity.Spawn();
		baseEntity.SetParent(this);
		mapMarkerInstance = baseEntity;
	}

	public void DisableCollisionTest()
	{
	}

	public void SpawnCrate(string resourcePath)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		int index = Random.Range(0, crateSpawns.Count);
		Vector3 position = crateSpawns[index].position;
		Quaternion rotation = crateSpawns[index].rotation;
		crateSpawns.Remove(crateSpawns[index]);
		BaseEntity baseEntity = GameManager.server.CreateEntity(resourcePath, position, rotation);
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			baseEntity.enableSaving = false;
			((Component)baseEntity).SendMessage("SetWasDropped", (SendMessageOptions)1);
			baseEntity.Spawn();
			baseEntity.SetParent(this, worldPositionStays: true);
			Rigidbody component = ((Component)baseEntity).GetComponent<Rigidbody>();
			if ((Object)(object)component != (Object)null)
			{
				component.isKinematic = true;
			}
		}
	}

	public void RespawnLoot()
	{
		((FacepunchBehaviour)this).InvokeRepeating((Action)PlayHorn, 0f, 8f);
		SpawnCrate(lockedCratePrefab.resourcePath);
		SpawnCrate(eliteCratePrefab.resourcePath);
		for (int i = 0; i < 4; i++)
		{
			SpawnCrate(militaryCratePrefab.resourcePath);
		}
		for (int j = 0; j < 4; j++)
		{
			SpawnCrate(junkCratePrefab.resourcePath);
		}
		lootRoundsPassed++;
		if (lootRoundsPassed >= loot_rounds)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)RespawnLoot);
		}
	}

	public void SpawnSubEntities()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isLoadingSave)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(escapeBoatPrefab.resourcePath, escapeBoatPoint.position, escapeBoatPoint.rotation);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.SetParent(this, worldPositionStays: true);
				baseEntity.Spawn();
				RHIB component = ((Component)baseEntity).GetComponent<RHIB>();
				component.SetToKinematic();
				if (Object.op_Implicit((Object)(object)component))
				{
					component.AddFuel(50);
				}
			}
		}
		MicrophoneStand microphoneStand = GameManager.server.CreateEntity(microphonePrefab.resourcePath, microphonePoint.position, microphonePoint.rotation) as MicrophoneStand;
		if (Object.op_Implicit((Object)(object)microphoneStand))
		{
			microphoneStand.enableSaving = false;
			microphoneStand.SetParent(this, worldPositionStays: true);
			microphoneStand.Spawn();
			microphoneStand.SpawnChildEntity();
			IOEntity iOEntity = microphoneStand.ioEntity.Get(serverside: true);
			Transform[] array = speakerPoints;
			foreach (Transform val in array)
			{
				IOEntity iOEntity2 = GameManager.server.CreateEntity(speakerPrefab.resourcePath, val.position, val.rotation) as IOEntity;
				iOEntity2.enableSaving = false;
				iOEntity2.SetParent(this, worldPositionStays: true);
				iOEntity2.Spawn();
				iOEntity.outputs[0].connectedTo.Set(iOEntity2);
				iOEntity2.inputs[0].connectedTo.Set(iOEntity);
				iOEntity = iOEntity2;
			}
			microphoneStand.ioEntity.Get(serverside: true).MarkDirtyForceUpdateOutputs();
		}
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		base.OnChildAdded(child);
		if (base.isServer && Application.isLoadingSave && child is RHIB rHIB)
		{
			Vector3 localPosition = ((Component)rHIB).transform.localPosition;
			Vector3 val = ((Component)this).transform.InverseTransformPoint(((Component)escapeBoatPoint).transform.position);
			if (Vector3.Distance(localPosition, val) < 1f)
			{
				rHIB.SetToKinematic();
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUint = Pool.Get<SimpleUInt>();
		info.msg.simpleUint.value = layoutChoice;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		RefreshActiveLayout();
	}

	public void PlayHorn()
	{
		ClientRPC(null, "DoHornSound");
		hornCount++;
		if (hornCount >= 3)
		{
			hornCount = 0;
			((FacepunchBehaviour)this).CancelInvoke((Action)PlayHorn);
		}
	}

	public override void Spawn()
	{
		if (!Application.isLoadingSave)
		{
			layoutChoice = (uint)Random.Range(0, layouts.Length);
			SendNetworkUpdate();
			RefreshActiveLayout();
		}
		base.Spawn();
	}

	public override void ServerInit()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)FindInitialNode, 2f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)BuildingCheck, 1f, 5f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)RespawnLoot, 10f, 60f * loot_round_spacing_minutes);
		((FacepunchBehaviour)this).Invoke((Action)DisableCollisionTest, 10f);
		float height = TerrainMeta.WaterMap.GetHeight(((Component)this).transform.position);
		Vector3 val = ((Component)this).transform.InverseTransformPoint(((Component)waterLine).transform.position);
		((Component)this).transform.position = new Vector3(((Component)this).transform.position.x, height - val.y, ((Component)this).transform.position.z);
		SpawnSubEntities();
		((FacepunchBehaviour)this).Invoke((Action)StartEgress, 60f * event_duration_minutes);
		CreateMapMarker();
	}

	public void UpdateRadiation()
	{
		currentRadiation += 1f;
		TriggerRadiation[] componentsInChildren = radiation.GetComponentsInChildren<TriggerRadiation>();
		foreach (TriggerRadiation triggerRadiation in componentsInChildren)
		{
			triggerRadiation.RadiationAmountOverride = currentRadiation;
		}
	}

	public void StartEgress()
	{
		if (!egressing)
		{
			egressing = true;
			((FacepunchBehaviour)this).CancelInvoke((Action)PlayHorn);
			radiation.SetActive(true);
			SetFlag(Flags.Reserved8, b: true);
			((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateRadiation, 10f, 1f);
			((FacepunchBehaviour)this).Invoke((Action)DelayedDestroy, 60f * egress_duration_minutes);
		}
	}

	public void DelayedDestroy()
	{
		Kill();
	}

	public void FindInitialNode()
	{
		targetNodeIndex = GetClosestNodeToUs();
	}

	public void BuildingCheck()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		List<DecayEntity> list = Pool.GetList<DecayEntity>();
		Vis.Entities(WorldSpaceBounds(), list, 2097152, (QueryTriggerInteraction)2);
		foreach (DecayEntity item in list)
		{
			if (item.isServer && item.IsAlive())
			{
				item.Kill(DestroyMode.Gib);
			}
		}
		Pool.FreeList<DecayEntity>(ref list);
	}

	public void FixedUpdate()
	{
		if (!base.isClient)
		{
			UpdateMovement();
		}
	}

	public void UpdateMovement()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0 || targetNodeIndex == -1)
		{
			return;
		}
		Vector3 val = TerrainMeta.Path.OceanPatrolFar[targetNodeIndex];
		Vector3 val2 = val;
		Vector3 val3;
		if (egressing)
		{
			Vector3 position = ((Component)this).transform.position;
			val3 = ((Component)this).transform.position - Vector3.zero;
			val2 = position + ((Vector3)(ref val3)).normalized * 10000f;
		}
		float num = 0f;
		val3 = val2 - ((Component)this).transform.position;
		Vector3 normalized = ((Vector3)(ref val3)).normalized;
		float num2 = Vector3.Dot(((Component)this).transform.forward, normalized);
		float num3 = Mathf.InverseLerp(0f, 1f, num2);
		num = num3;
		float num4 = Vector3.Dot(((Component)this).transform.right, normalized);
		float num5 = 2.5f;
		float num6 = Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(num4));
		turnScale = Mathf.Lerp(turnScale, num6, Time.deltaTime * 0.2f);
		float num7 = ((!(num4 < 0f)) ? 1 : (-1));
		currentTurnSpeed = num5 * turnScale * num7;
		((Component)this).transform.Rotate(Vector3.up, Time.deltaTime * currentTurnSpeed, (Space)0);
		currentThrottle = Mathf.Lerp(currentThrottle, num, Time.deltaTime * 0.2f);
		currentVelocity = ((Component)this).transform.forward * (8f * currentThrottle);
		Transform transform = ((Component)this).transform;
		transform.position += currentVelocity * Time.deltaTime;
		if (Vector3.Distance(((Component)this).transform.position, val2) < 80f)
		{
			targetNodeIndex++;
			if (targetNodeIndex >= TerrainMeta.Path.OceanPatrolFar.Count)
			{
				targetNodeIndex = 0;
			}
		}
	}

	public int GetClosestNodeToUs()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		int result = 0;
		float num = float.PositiveInfinity;
		for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
		{
			Vector3 val = TerrainMeta.Path.OceanPatrolFar[i];
			float num2 = Vector3.Distance(((Component)this).transform.position, val);
			if (num2 < num)
			{
				result = i;
				num = num2;
			}
		}
		return result;
	}

	public override Vector3 GetLocalVelocityServer()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return currentVelocity;
	}

	public override Quaternion GetAngularVelocityServer()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.Euler(0f, currentTurnSpeed, 0f);
	}

	public override float InheritedVelocityScale()
	{
		return 1f;
	}

	public override bool BlocksWaterFor(BasePlayer player)
	{
		return true;
	}

	public override float MaxVelocity()
	{
		return 8f;
	}

	public override bool SupportsChildDeployables()
	{
		return true;
	}

	public override bool ForceDeployableSetParent()
	{
		return true;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CargoShip.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}
}
