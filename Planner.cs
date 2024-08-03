using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class Planner : HeldEntity
{
	public struct CanBuildResult
	{
		public bool Result;

		public Phrase Phrase;

		public string[] Arguments;
	}

	public BaseEntity[] buildableList;

	public bool isTypeDeployable => (Object)(object)GetModDeployable() != (Object)null;

	public Vector3 serverStartDurationPlacementPosition { get; private set; }

	public TimeSince serverStartDurationPlacementTime { get; private set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Planner.OnRpcMessage", 0);
		try
		{
			if (rpc == 1872774636 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - DoPlace "));
				}
				TimeWarning val2 = TimeWarning.New("DoPlace", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1872774636u, "DoPlace", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							DoPlace(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoPlace");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3892284151u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - StartDurationPlace "));
				}
				TimeWarning val2 = TimeWarning.New("StartDurationPlace", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3892284151u, "StartDurationPlace", this, player, 10uL))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(3892284151u, "StartDurationPlace", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							StartDurationPlace(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in StartDurationPlace");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public ItemModDeployable GetModDeployable()
	{
		ItemDefinition ownerItemDefinition = GetOwnerItemDefinition();
		if ((Object)(object)ownerItemDefinition == (Object)null)
		{
			return null;
		}
		return ((Component)ownerItemDefinition).GetComponent<ItemModDeployable>();
	}

	public Deployable GetDeployable()
	{
		ItemModDeployable modDeployable = GetModDeployable();
		if ((Object)(object)modDeployable == (Object)null)
		{
			return null;
		}
		return modDeployable.GetDeployable(this);
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void DoPlace(RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		CreateBuilding val = CreateBuilding.Deserialize((Stream)(object)msg.read);
		try
		{
			DoBuild(val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.CallsPerSecond(10uL)]
	private void StartDurationPlace(RPCMessage msg)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (Object.op_Implicit((Object)(object)ownerPlayer))
		{
			serverStartDurationPlacementPosition = ((Component)ownerPlayer).transform.position;
			serverStartDurationPlacementTime = TimeSince.op_Implicit(0f);
		}
	}

	public Socket_Base FindSocket(string name, uint prefabIDToFind)
	{
		return PrefabAttribute.server.FindAll<Socket_Base>(prefabIDToFind).FirstOrDefault((Socket_Base s) => s.socketName == name);
	}

	public void DoBuild(CreateBuilding msg)
	{
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return;
		}
		if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack())
		{
			ownerPlayer.ChatMessage("AntiHack!");
			return;
		}
		Construction construction = PrefabAttribute.server.Find<Construction>(msg.blockID);
		if (construction == null)
		{
			ownerPlayer.ChatMessage("Couldn't find Construction " + msg.blockID);
			return;
		}
		if (!CanAffordToPlace(construction))
		{
			ownerPlayer.ChatMessage("Can't afford to place!");
			ItemAmountList val = Pool.Get<ItemAmountList>();
			try
			{
				val.amount = Pool.GetList<float>();
				val.itemID = Pool.GetList<int>();
				GetConstructionCost(val, construction);
				ownerPlayer.ClientRPC<ItemAmountList>(RpcTarget.Player("Client_OnRepairFailedResources", ownerPlayer), val);
				return;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (!ownerPlayer.CanBuild() && !construction.canBypassBuildingPermission)
		{
			ownerPlayer.ChatMessage("Building is blocked!");
			return;
		}
		Deployable deployable = GetDeployable();
		if (construction.deployable != deployable)
		{
			ownerPlayer.ChatMessage("Deployable mismatch!");
			AntiHack.NoteAdminHack(ownerPlayer);
			return;
		}
		Construction.Target target = default(Construction.Target);
		if (((NetworkableId)(ref msg.entity)).IsValid)
		{
			target.entity = BaseNetworkable.serverEntities.Find(msg.entity) as BaseEntity;
			if ((Object)(object)target.entity == (Object)null)
			{
				NetworkableId entity = msg.entity;
				ownerPlayer.ChatMessage("Couldn't find entity " + ((object)(NetworkableId)(ref entity)).ToString());
				return;
			}
			msg.ray = new Ray(((Component)target.entity).transform.TransformPoint(((Ray)(ref msg.ray)).origin), ((Component)target.entity).transform.TransformDirection(((Ray)(ref msg.ray)).direction));
			msg.position = ((Component)target.entity).transform.TransformPoint(msg.position);
			msg.normal = ((Component)target.entity).transform.TransformDirection(msg.normal);
			msg.rotation = ((Component)target.entity).transform.rotation * msg.rotation;
			if (msg.socket != 0)
			{
				string text = StringPool.Get(msg.socket);
				if (text != "")
				{
					target.socket = FindSocket(text, target.entity.prefabID);
				}
				if (target.socket == null)
				{
					ownerPlayer.ChatMessage("Couldn't find socket " + msg.socket);
					return;
				}
			}
			else if (target.entity is Door)
			{
				ownerPlayer.ChatMessage("Can't deploy on door");
				return;
			}
		}
		target.ray = msg.ray;
		target.onTerrain = msg.onterrain;
		target.position = msg.position;
		target.normal = msg.normal;
		target.rotation = msg.rotation;
		target.player = ownerPlayer;
		target.isHoldingShift = msg.isHoldingShift;
		target.valid = true;
		if (ShouldParent(target.entity, deployable))
		{
			Vector3 position = ((target.socket != null) ? target.GetWorldPosition() : target.position);
			float num = target.entity.Distance(position);
			if (num > 1f)
			{
				ownerPlayer.ChatMessage("Parent too far away: " + num);
				return;
			}
		}
		BaseEntity baseEntity = DoBuild(target, construction);
		if ((Object)(object)baseEntity != (Object)null && ownerPlayer.IsInCreativeMode && Creative.freeBuild && baseEntity is BuildingBlock buildingBlock)
		{
			ConstructionGrade constructionGrade = construction.grades[msg.setToGrade];
			if (buildingBlock.currentGrade != constructionGrade)
			{
				buildingBlock.ChangeGradeAndSkin(constructionGrade.gradeBase.type, constructionGrade.gradeBase.skin);
			}
		}
	}

	public BaseEntity DoBuild(Construction.Target target, Construction component)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return null;
		}
		if (target.ray.IsNaNOrInfinity())
		{
			return null;
		}
		if (Vector3Ex.IsNaNOrInfinity(target.position))
		{
			return null;
		}
		if (Vector3Ex.IsNaNOrInfinity(target.normal))
		{
			return null;
		}
		if (target.socket != null)
		{
			if (!target.socket.female)
			{
				ownerPlayer.ChatMessage("Target socket is not female. (" + target.socket.socketName + ")");
				return null;
			}
			if ((Object)(object)target.entity != (Object)null && target.entity.IsOccupied(target.socket))
			{
				ownerPlayer.ChatMessage("Target socket is occupied. (" + target.socket.socketName + ")");
				return null;
			}
			if (target.onTerrain)
			{
				ownerPlayer.ChatMessage("Target on terrain is not allowed when attaching to socket. (" + target.socket.socketName + ")");
				return null;
			}
		}
		Vector3 val = (((Object)(object)target.entity != (Object)null && target.socket != null) ? target.GetWorldPosition() : target.position);
		if (AntiHack.TestIsBuildingInsideSomething(target, val))
		{
			ownerPlayer.ChatMessage("Can't deploy inside objects");
			return null;
		}
		if (ConVar.AntiHack.eye_protection >= 2)
		{
			Vector3 center = ownerPlayer.eyes.center;
			Vector3 position = ownerPlayer.eyes.position;
			Vector3 origin = ((Ray)(ref target.ray)).origin;
			Vector3 val2 = val;
			int num = 2097152;
			int num2 = 2162688;
			if (ConVar.AntiHack.build_terraincheck)
			{
				num2 |= 0x800000;
			}
			if (ConVar.AntiHack.build_vehiclecheck)
			{
				num2 |= 0x8000000;
			}
			float num3 = ConVar.AntiHack.build_losradius;
			float padding = ConVar.AntiHack.build_losradius + 0.01f;
			int layerMask = num2;
			if (target.socket != null)
			{
				num3 = 0f;
				padding = 0.5f;
				layerMask = num;
			}
			if (component.isSleepingBag)
			{
				num3 = ConVar.AntiHack.build_losradius_sleepingbag;
				padding = ConVar.AntiHack.build_losradius_sleepingbag + 0.01f;
				layerMask = num2;
			}
			if (num3 > 0f)
			{
				val2 += ((Vector3)(ref target.normal)).normalized * num3;
			}
			if ((Object)(object)target.entity != (Object)null)
			{
				DeployShell deployShell = PrefabAttribute.server.Find<DeployShell>(target.entity.prefabID);
				if (deployShell != null)
				{
					val2 += ((Vector3)(ref target.normal)).normalized * deployShell.LineOfSightPadding();
				}
			}
			if (!GamePhysics.LineOfSightRadius(center, position, layerMask, num3) || !GamePhysics.LineOfSightRadius(position, origin, layerMask, num3) || !GamePhysics.LineOfSightRadius(origin, val2, layerMask, num3, 0f, padding))
			{
				ownerPlayer.ChatMessage("Line of sight blocked.");
				return null;
			}
		}
		Construction.lastPlacementError = "No Error";
		if (Server.max_sleeping_bags > 0)
		{
			CanBuildResult? result = SleepingBag.CanBuildBed(ownerPlayer, component);
			if (HandleCanBuild(result, ownerPlayer))
			{
				return null;
			}
		}
		if (Server.max_shelters > 0)
		{
			CanBuildResult? result2 = LegacyShelter.CanBuildShelter(ownerPlayer, component);
			if (HandleCanBuild(result2, ownerPlayer))
			{
				return null;
			}
		}
		GameObject val3 = DoPlacement(target, component);
		if ((Object)(object)val3 == (Object)null)
		{
			ownerPlayer.ChatMessage("Can't place: " + Construction.lastPlacementError);
		}
		if ((Object)(object)val3 != (Object)null)
		{
			Deployable deployable = GetDeployable();
			BaseEntity baseEntity = val3.ToBaseEntity();
			if ((Object)(object)baseEntity != (Object)null && deployable != null)
			{
				if (ShouldParent(target.entity, deployable))
				{
					if (target.socket is Socket_Specific_Female socket_Specific_Female)
					{
						if (socket_Specific_Female.parentToBone)
						{
							baseEntity.SetParent(target.entity, socket_Specific_Female.boneName, worldPositionStays: true);
						}
						else
						{
							baseEntity.SetParent(target.entity, worldPositionStays: true);
						}
					}
					else
					{
						baseEntity.SetParent(target.entity, worldPositionStays: true);
					}
				}
				if (deployable.wantsInstanceData && GetOwnerItem().instanceData != null)
				{
					(baseEntity as IInstanceDataReceiver).ReceiveInstanceData(GetOwnerItem().instanceData);
				}
				if (deployable.copyInventoryFromItem)
				{
					StorageContainer component2 = ((Component)baseEntity).GetComponent<StorageContainer>();
					if (Object.op_Implicit((Object)(object)component2))
					{
						component2.ReceiveInventoryFromItem(GetOwnerItem());
					}
				}
				ItemModDeployable modDeployable = GetModDeployable();
				if ((Object)(object)modDeployable != (Object)null)
				{
					modDeployable.OnDeployed(baseEntity, ownerPlayer);
				}
				baseEntity.OnDeployed(baseEntity.GetParentEntity(), ownerPlayer, GetOwnerItem());
				if (deployable.placeEffect.isValid)
				{
					if (Object.op_Implicit((Object)(object)target.entity) && target.socket != null)
					{
						Effect.server.Run(deployable.placeEffect.resourcePath, ((Component)target.entity).transform.TransformPoint(target.socket.worldPosition), ((Component)target.entity).transform.up);
					}
					else
					{
						Effect.server.Run(deployable.placeEffect.resourcePath, target.position, target.normal);
					}
				}
			}
			if ((Object)(object)baseEntity != (Object)null)
			{
				Analytics.Azure.OnEntityBuilt(baseEntity, ownerPlayer);
				if ((Object)(object)GetOwnerItemDefinition() != (Object)null)
				{
					ownerPlayer.ProcessMissionEvent(BaseMission.MissionEventType.DEPLOY, new BaseMission.MissionEventPayload
					{
						WorldPosition = ((Component)baseEntity).transform.position,
						UintIdentifier = baseEntity.prefabID,
						IntIdentifier = GetOwnerItemDefinition().itemid
					}, 1f);
				}
			}
			PayForPlacement(ownerPlayer, component);
			return baseEntity;
		}
		return null;
	}

	public GameObject DoPlacement(Construction.Target placement, Construction component)
	{
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return null;
		}
		BaseEntity baseEntity = component.CreateConstruction(placement, bNeedsValidPlacement: true);
		if (!Object.op_Implicit((Object)(object)baseEntity))
		{
			return null;
		}
		float num = 1f;
		float num2 = 0f;
		Item ownerItem = GetOwnerItem();
		if (ownerItem != null)
		{
			baseEntity.skinID = ownerItem.skin;
			if (ownerItem.hasCondition)
			{
				num = ownerItem.conditionNormalized;
			}
		}
		((Component)baseEntity).gameObject.AwakeFromInstantiate();
		BuildingBlock buildingBlock = baseEntity as BuildingBlock;
		if (Object.op_Implicit((Object)(object)buildingBlock))
		{
			buildingBlock.blockDefinition = PrefabAttribute.server.Find<Construction>(buildingBlock.prefabID);
			if (!buildingBlock.blockDefinition)
			{
				Debug.LogError((object)"Placing a building block that has no block definition!");
				return null;
			}
			buildingBlock.SetGrade(buildingBlock.blockDefinition.defaultGrade.gradeBase.type);
			num2 = buildingBlock.currentGrade.maxHealth;
		}
		BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
		if (Object.op_Implicit((Object)(object)baseCombatEntity))
		{
			num2 = (((Object)(object)buildingBlock != (Object)null) ? buildingBlock.currentGrade.maxHealth : baseCombatEntity.startHealth);
			baseCombatEntity.ResetLifeStateOnSpawn = false;
			baseCombatEntity.InitializeHealth(num2 * num, num2);
		}
		baseEntity.OnPlaced(ownerPlayer);
		baseEntity.OwnerID = ownerPlayer.userID;
		baseEntity.Spawn();
		if (Object.op_Implicit((Object)(object)buildingBlock))
		{
			Effect.server.Run("assets/bundled/prefabs/fx/build/frame_place.prefab", baseEntity, 0u, Vector3.zero, Vector3.zero);
		}
		StabilityEntity stabilityEntity = baseEntity as StabilityEntity;
		if (Object.op_Implicit((Object)(object)stabilityEntity))
		{
			stabilityEntity.UpdateSurroundingEntities();
		}
		return ((Component)baseEntity).gameObject;
	}

	public void PayForPlacement(BasePlayer player, Construction component)
	{
		if (player.IsInCreativeMode && Creative.freeBuild)
		{
			return;
		}
		if (player.IsInTutorial)
		{
			TutorialIsland currentTutorialIsland = player.GetCurrentTutorialIsland();
			if ((Object)(object)currentTutorialIsland != (Object)null)
			{
				currentTutorialIsland.OnPlayerBuiltConstruction(player);
			}
		}
		if (isTypeDeployable)
		{
			GetItem().UseItem();
			return;
		}
		List<Item> list = new List<Item>();
		foreach (ItemAmount item in component.defaultGrade.CostToBuild())
		{
			player.inventory.Take(list, item.itemDef.itemid, (int)item.amount);
			player.Command("note.inv", item.itemDef.itemid, item.amount * -1f);
		}
		foreach (Item item2 in list)
		{
			item2.Remove();
		}
	}

	public bool CanAffordToPlace(Construction component)
	{
		if (isTypeDeployable)
		{
			return true;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return false;
		}
		if (ownerPlayer.IsInCreativeMode && Creative.freeBuild)
		{
			return true;
		}
		foreach (ItemAmount item in component.defaultGrade.CostToBuild())
		{
			if ((float)ownerPlayer.inventory.GetAmount(item.itemDef.itemid) < item.amount)
			{
				return false;
			}
		}
		return true;
	}

	private void GetConstructionCost(ItemAmountList list, Construction component)
	{
		list.amount.Clear();
		list.itemID.Clear();
		foreach (ItemAmount item in component.defaultGrade.CostToBuild())
		{
			list.itemID.Add(item.itemDef.itemid);
			list.amount.Add((int)item.amount);
		}
	}

	private bool ShouldParent(BaseEntity targetEntity, Deployable deployable)
	{
		if ((Object)(object)targetEntity != (Object)null && targetEntity.SupportsChildDeployables() && (targetEntity.ForceDeployableSetParent() || (deployable != null && deployable.setSocketParent)))
		{
			return true;
		}
		return false;
	}

	private bool HandleCanBuild(CanBuildResult? result, BasePlayer player)
	{
		if (result.HasValue)
		{
			if (result.Value.Phrase != null && !player.IsInTutorial)
			{
				player.ShowToast((!result.Value.Result) ? GameTip.Styles.Red_Normal : GameTip.Styles.Blue_Long, result.Value.Phrase, result.Value.Arguments);
			}
			if (!result.Value.Result)
			{
				return true;
			}
		}
		return false;
	}
}
