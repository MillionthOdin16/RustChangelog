using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class BuildingBlock : StabilityEntity
{
	public static class BlockFlags
	{
		public const Flags CanRotate = Flags.Reserved1;
	}

	public class UpdateSkinWorkQueue : ObjectWorkQueue<BuildingBlock>
	{
		protected override void RunJob(BuildingBlock entity)
		{
			if (((ObjectWorkQueue<BuildingBlock>)this).ShouldAdd(entity))
			{
				entity.UpdateSkin(force: true);
			}
		}

		protected override bool ShouldAdd(BuildingBlock entity)
		{
			return entity.IsValid();
		}
	}

	[NonSerialized]
	public Construction blockDefinition;

	private static Vector3[] outsideLookupOffsets;

	private bool forceSkinRefresh;

	private ulong lastSkinID;

	private int lastModelState;

	private uint lastCustomColour;

	private uint playerCustomColourToApply;

	public BuildingGrade.Enum grade;

	private BuildingGrade.Enum lastGrade = BuildingGrade.Enum.None;

	private ConstructionSkin currentSkin;

	private DeferredAction skinChange;

	private MeshRenderer placeholderRenderer;

	private MeshCollider placeholderCollider;

	public static UpdateSkinWorkQueue updateSkinQueueServer;

	public static readonly Phrase RotateTitle;

	public static readonly Phrase RotateDesc;

	private bool globalNetworkCooldown;

	public bool CullBushes;

	public bool CheckForPipesOnModelChange;

	public OBBComponent AlternativePipeBounds;

	private bool hasWallpaper;

	public override bool CanBeDemolished => true;

	public int modelState { get; private set; }

	public uint customColour { get; private set; }

	public ConstructionGrade currentGrade
	{
		get
		{
			if (blockDefinition == null)
			{
				Debug.LogWarning((object)$"blockDefinition is null for {base.ShortPrefabName} {grade} {skinID}");
				return null;
			}
			ConstructionGrade constructionGrade = blockDefinition.GetGrade(grade, skinID);
			if (constructionGrade == null)
			{
				Debug.LogWarning((object)$"currentGrade is null for {base.ShortPrefabName} {grade} {skinID}");
				return null;
			}
			return constructionGrade;
		}
	}

	public ulong wallpaperID { get; private set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BuildingBlock.OnRpcMessage", 0);
		try
		{
			if (rpc == 1956645865 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - DoRotation "));
				}
				TimeWarning val2 = TimeWarning.New("DoRotation", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1956645865u, "DoRotation", this, player, 3f))
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
							DoRotation(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoRotation");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3746288057u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - DoUpgradeToGrade "));
				}
				TimeWarning val2 = TimeWarning.New("DoUpgradeToGrade", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3746288057u, "DoUpgradeToGrade", this, player, 3f))
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
							DoUpgradeToGrade(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in DoUpgradeToGrade");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4081052216u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - DoUpgradeToGrade_Delayed "));
				}
				TimeWarning val2 = TimeWarning.New("DoUpgradeToGrade_Delayed", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4081052216u, "DoUpgradeToGrade_Delayed", this, player, 3f))
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
							RPCMessage msg4 = rPCMessage;
							DoUpgradeToGrade_Delayed(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in DoUpgradeToGrade_Delayed");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 526349102 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_PickupWallpaperStart "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_PickupWallpaperStart", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(526349102u, "RPC_PickupWallpaperStart", this, player, 3f))
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
							RPCMessage rpc2 = rPCMessage;
							RPC_PickupWallpaperStart(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_PickupWallpaperStart");
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

	public override void ResetState()
	{
		base.ResetState();
		blockDefinition = null;
		forceSkinRefresh = false;
		modelState = 0;
		lastModelState = 0;
		hasWallpaper = false;
		wallpaperID = 0uL;
		grade = BuildingGrade.Enum.Twigs;
		lastGrade = BuildingGrade.Enum.None;
		DestroySkin();
		UpdatePlaceholder(state: true);
	}

	public override void InitShared()
	{
		base.InitShared();
		placeholderRenderer = ((Component)this).GetComponent<MeshRenderer>();
		placeholderCollider = ((Component)this).GetComponent<MeshCollider>();
	}

	public override void PostInitShared()
	{
		baseProtection = currentGrade.gradeBase.damageProtecton;
		grade = currentGrade.gradeBase.type;
		base.PostInitShared();
	}

	public override void DestroyShared()
	{
		if (base.isServer)
		{
			RefreshNeighbours(linkToNeighbours: false);
		}
		base.DestroyShared();
	}

	public override string Categorize()
	{
		return "building";
	}

	public override float BoundsPadding()
	{
		return 1f;
	}

	public override bool IsOutside()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		float outside_test_range = ConVar.Decay.outside_test_range;
		Vector3 val = PivotPoint();
		for (int i = 0; i < outsideLookupOffsets.Length; i++)
		{
			Vector3 val2 = outsideLookupOffsets[i];
			Vector3 val3 = val + val2 * outside_test_range;
			if (!Physics.Raycast(new Ray(val3, -val2), outside_test_range - 0.5f, 2097152))
			{
				return true;
			}
		}
		return false;
	}

	public override bool SupportsChildDeployables()
	{
		return true;
	}

	public void SetConditionalModel(int state)
	{
		if (state != modelState)
		{
			modelState = state;
			if (base.isServer)
			{
				GlobalNetworkHandler.server?.TrySendNetworkUpdate(this);
			}
		}
	}

	public bool GetConditionalModel(int index)
	{
		return (modelState & (1 << index)) != 0;
	}

	private bool CanChangeToGrade(BuildingGrade.Enum iGrade, ulong iSkin, BasePlayer player)
	{
		if (player.IsInCreativeMode && Creative.freeBuild)
		{
			return true;
		}
		if (HasUpgradePrivilege(iGrade, iSkin, player))
		{
			return !IsUpgradeBlocked();
		}
		return false;
	}

	private bool HasUpgradePrivilege(BuildingGrade.Enum iGrade, ulong iSkin, BasePlayer player)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (player.IsInCreativeMode && Creative.freeBuild)
		{
			return true;
		}
		if (iGrade < grade)
		{
			return false;
		}
		if (iGrade == grade && iSkin == skinID)
		{
			return false;
		}
		if (iGrade <= BuildingGrade.Enum.None)
		{
			return false;
		}
		if (iGrade >= BuildingGrade.Enum.Count)
		{
			return false;
		}
		return !player.IsBuildingBlocked(((Component)this).transform.position, ((Component)this).transform.rotation, bounds);
	}

	private bool IsUpgradeBlocked()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (!blockDefinition.checkVolumeOnUpgrade)
		{
			return false;
		}
		DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(prefabID);
		return DeployVolume.Check(((Component)this).transform.position, ((Component)this).transform.rotation, volumes, ~(1 << ((Component)this).gameObject.layer));
	}

	private bool CanAffordUpgrade(BuildingGrade.Enum iGrade, ulong iSkin, BasePlayer player)
	{
		if ((Object)(object)player != (Object)null && player.IsInCreativeMode && Creative.freeBuild)
		{
			return true;
		}
		foreach (ItemAmount item in blockDefinition.GetGrade(iGrade, iSkin).CostToBuild(grade))
		{
			if ((float)player.inventory.GetAmount(item.itemid) < item.amount)
			{
				return false;
			}
		}
		return true;
	}

	public void SetGrade(BuildingGrade.Enum iGrade)
	{
		if (blockDefinition.grades == null || iGrade <= BuildingGrade.Enum.None || iGrade >= BuildingGrade.Enum.Count)
		{
			Debug.LogError((object)("Tried to set to undefined grade! " + blockDefinition.fullName), (Object)(object)((Component)this).gameObject);
			return;
		}
		grade = iGrade;
		grade = currentGrade.gradeBase.type;
		UpdateGrade();
	}

	private void UpdateGrade()
	{
		baseProtection = currentGrade.gradeBase.damageProtecton;
	}

	protected override void OnSkinChanged(ulong oldSkinID, ulong newSkinID)
	{
		if (oldSkinID != newSkinID)
		{
			skinID = newSkinID;
		}
	}

	protected override void OnSkinPreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}

	public void SetHealthToMax()
	{
		base.health = MaxHealth();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void DoUpgradeToGrade_Delayed(RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		BuildingGrade.Enum @enum = (BuildingGrade.Enum)msg.read.Int32();
		ulong num = msg.read.UInt64();
		ConstructionGrade constructionGrade = blockDefinition.GetGrade(@enum, num);
		if (!(constructionGrade == null) && CanChangeToGrade(@enum, num, msg.player) && CanAffordUpgrade(@enum, num, msg.player) && !(base.SecondsSinceAttacked < 30f) && (num == 0L || msg.player.blueprints.steamInventory.HasItem((int)num)))
		{
			PayForUpgrade(constructionGrade, msg.player);
			if ((Object)(object)msg.player != (Object)null)
			{
				playerCustomColourToApply = msg.player.LastBlockColourChangeId;
			}
			ClientRPC(RpcTarget.NetworkGroup("DoUpgradeEffect"), (int)@enum, num);
			Analytics.Azure.OnBuildingBlockUpgraded(msg.player, this, @enum, playerCustomColourToApply, num);
			OnSkinChanged(skinID, num);
			ChangeGrade(@enum, playEffect: true);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void DoUpgradeToGrade(RPCMessage msg)
	{
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		if (!msg.player.CanInteract())
		{
			return;
		}
		ConstructionGrade constructionGrade = blockDefinition.GetGrade((BuildingGrade.Enum)msg.read.Int32(), msg.read.UInt64());
		if (!(constructionGrade == null) && CanChangeToGrade(constructionGrade.gradeBase.type, constructionGrade.gradeBase.skin, msg.player) && CanAffordUpgrade(constructionGrade.gradeBase.type, constructionGrade.gradeBase.skin, msg.player) && !(base.SecondsSinceAttacked < 30f) && (constructionGrade.gradeBase.alwaysUnlock || constructionGrade.gradeBase.skin == 0L || msg.player.blueprints.steamInventory.HasItem((int)constructionGrade.gradeBase.skin)))
		{
			PayForUpgrade(constructionGrade, msg.player);
			if ((Object)(object)msg.player != (Object)null)
			{
				playerCustomColourToApply = msg.player.LastBlockColourChangeId;
			}
			ClientRPC(RpcTarget.NetworkGroup("DoUpgradeEffect"), (int)constructionGrade.gradeBase.type, constructionGrade.gradeBase.skin);
			BuildingGrade.Enum @enum = grade;
			Analytics.Azure.OnBuildingBlockUpgraded(msg.player, this, constructionGrade.gradeBase.type, playerCustomColourToApply, constructionGrade.gradeBase.skin);
			OnSkinChanged(skinID, constructionGrade.gradeBase.skin);
			ChangeGrade(constructionGrade.gradeBase.type, playEffect: true);
			if ((Object)(object)msg.player != (Object)null && @enum != constructionGrade.gradeBase.type)
			{
				msg.player.ProcessMissionEvent(BaseMission.MissionEventType.UPGRADE_BUILDING_GRADE, new BaseMission.MissionEventPayload
				{
					NetworkIdentifier = net.ID,
					IntIdentifier = (int)constructionGrade.gradeBase.type
				}, 1f);
			}
		}
	}

	public void ChangeGradeAndSkin(BuildingGrade.Enum targetGrade, ulong skin, bool playEffect = false, bool updateSkin = true)
	{
		OnSkinChanged(skinID, skin);
		ChangeGrade(targetGrade, playEffect, updateSkin);
	}

	public void ChangeGrade(BuildingGrade.Enum targetGrade, bool playEffect = false, bool updateSkin = true)
	{
		SetGrade(targetGrade);
		if (grade != lastGrade)
		{
			SetHealthToMax();
			StartBeingRotatable();
		}
		if (updateSkin)
		{
			UpdateSkin();
		}
		SendNetworkUpdate();
		ResetUpkeepTime();
		UpdateSurroundingEntities();
		GlobalNetworkHandler.server.TrySendNetworkUpdate(this);
		BuildingManager.server.GetBuilding(buildingID)?.Dirty();
	}

	private void PayForUpgrade(ConstructionGrade g, BasePlayer player)
	{
		if (player.IsInCreativeMode && Creative.freeBuild)
		{
			return;
		}
		List<Item> list = new List<Item>();
		foreach (ItemAmount item in g.CostToBuild(grade))
		{
			player.inventory.Take(list, item.itemid, (int)item.amount);
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(item.itemid);
			Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "upgrade_block", itemDefinition.shortname, (int)item.amount, this, null, safezone: false, null, player.userID);
			player.Command("note.inv " + item.itemid + " " + item.amount * -1f);
		}
		foreach (Item item2 in list)
		{
			item2.Remove();
		}
	}

	public void SetCustomColour(uint newColour)
	{
		if (newColour != customColour)
		{
			customColour = newColour;
			SendNetworkUpdateImmediate();
			ClientRPC(RpcTarget.NetworkGroup("RefreshSkin"));
			GlobalNetworkHandler.server.TrySendNetworkUpdate(this);
		}
	}

	private bool NeedsSkinChange()
	{
		if (!((Object)(object)currentSkin == (Object)null) && !forceSkinRefresh && lastGrade == grade && lastModelState == modelState)
		{
			return lastSkinID != skinID;
		}
		return true;
	}

	public void UpdateSkin(bool force = false)
	{
		if (force)
		{
			forceSkinRefresh = true;
		}
		if (!NeedsSkinChange())
		{
			return;
		}
		if (cachedStability <= 0f || base.isServer)
		{
			ChangeSkin();
			return;
		}
		if (!skinChange)
		{
			skinChange = new DeferredAction((Object)(object)this, ChangeSkin);
		}
		if (skinChange.Idle)
		{
			skinChange.Invoke();
		}
	}

	private void DestroySkin()
	{
		if ((Object)(object)currentSkin != (Object)null)
		{
			currentSkin.Destroy(this);
			currentSkin = null;
		}
	}

	private void RefreshNeighbours(bool linkToNeighbours)
	{
		List<EntityLink> entityLinks = GetEntityLinks(linkToNeighbours);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				BuildingBlock buildingBlock = entityLink.connections[j].owner as BuildingBlock;
				if (!((Object)(object)buildingBlock == (Object)null))
				{
					if (Application.isLoading)
					{
						buildingBlock.UpdateSkin(force: true);
					}
					else
					{
						((ObjectWorkQueue<BuildingBlock>)updateSkinQueueServer).Add(buildingBlock);
					}
				}
			}
		}
	}

	private void UpdatePlaceholder(bool state)
	{
		if (Object.op_Implicit((Object)(object)placeholderRenderer))
		{
			((Renderer)placeholderRenderer).enabled = state;
		}
		if (Object.op_Implicit((Object)(object)placeholderCollider))
		{
			((Collider)placeholderCollider).enabled = state;
		}
	}

	private void ChangeSkin()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		ConstructionGrade constructionGrade = currentGrade;
		if (currentGrade == null)
		{
			Debug.LogWarning((object)"CurrentGrade is null!");
			return;
		}
		if (constructionGrade.skinObject.isValid)
		{
			ChangeSkin(constructionGrade.skinObject);
			return;
		}
		ConstructionGrade defaultGrade = blockDefinition.defaultGrade;
		if (defaultGrade.skinObject.isValid)
		{
			ChangeSkin(defaultGrade.skinObject);
		}
		else
		{
			Debug.LogWarning((object)("No skins found for " + (object)((Component)this).gameObject));
		}
	}

	private void ChangeSkin(GameObjectRef prefab)
	{
		bool flag = lastGrade != grade || lastSkinID != skinID;
		lastGrade = grade;
		lastSkinID = skinID;
		if (flag)
		{
			if ((Object)(object)currentSkin == (Object)null)
			{
				UpdatePlaceholder(state: false);
			}
			else
			{
				DestroySkin();
			}
			GameObject val = base.gameManager.CreatePrefab(prefab.resourcePath, ((Component)this).transform);
			currentSkin = val.GetComponent<ConstructionSkin>();
			if ((Object)(object)currentSkin != (Object)null && base.isServer && !Application.isLoading)
			{
				customColour = currentSkin.GetStartingDetailColour(playerCustomColourToApply);
			}
			Model component = ((Component)currentSkin).GetComponent<Model>();
			SetModel(component);
			Assert.IsTrue((Object)(object)model == (Object)(object)component, "Didn't manage to set model successfully!");
		}
		if (base.isServer)
		{
			SetConditionalModel(currentSkin.DetermineConditionalModelState(this));
		}
		bool flag2 = lastModelState != modelState;
		lastModelState = modelState;
		bool flag3 = lastCustomColour != customColour;
		lastCustomColour = customColour;
		if (flag || flag2 || forceSkinRefresh || flag3)
		{
			currentSkin.Refresh(this);
			if (base.isServer && flag2)
			{
				CheckForPipes();
			}
			forceSkinRefresh = false;
		}
		if (base.isServer)
		{
			if (flag)
			{
				RefreshNeighbours(linkToNeighbours: true);
			}
			if (flag2)
			{
				SendNetworkUpdate();
			}
		}
	}

	public override bool ShouldBlockProjectiles()
	{
		return grade != BuildingGrade.Enum.Twigs;
	}

	[ContextMenu("Check for pipes")]
	public void CheckForPipes()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckForPipesOnModelChange || !ConVar.Server.enforcePipeChecksOnBuildingBlockChanges || Application.isLoading)
		{
			return;
		}
		List<ColliderInfo_Pipe> list = Pool.GetList<ColliderInfo_Pipe>();
		Bounds val = bounds;
		((Bounds)(ref val)).extents = ((Bounds)(ref val)).extents * 0.97f;
		Vis.Components<ColliderInfo_Pipe>((OBB)(((Object)(object)AlternativePipeBounds != (Object)null) ? AlternativePipeBounds.GetObb() : new OBB(((Component)this).transform, val)), list, 536870912, (QueryTriggerInteraction)2);
		foreach (ColliderInfo_Pipe item in list)
		{
			if (!((Object)(object)item == (Object)null) && ((Component)item).gameObject.activeInHierarchy && item.HasFlag(ColliderInfo.Flags.OnlyBlockBuildingBlock) && (Object)(object)item.ParentEntity != (Object)null && item.ParentEntity.isServer)
			{
				WireTool.AttemptClearSlot(item.ParentEntity, null, item.OutputSlotIndex, isInput: false);
			}
		}
		Pool.FreeList<ColliderInfo_Pipe>(ref list);
	}

	private void OnHammered()
	{
	}

	public override float MaxHealth()
	{
		return currentGrade.maxHealth;
	}

	public override List<ItemAmount> BuildCost()
	{
		return currentGrade.CostToBuild();
	}

	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		base.OnHealthChanged(oldvalue, newvalue);
		if (base.isServer)
		{
			if (HasWallpaper() && newvalue < oldvalue)
			{
				RemoveWallpaper();
			}
			if (Mathf.RoundToInt(oldvalue) != Mathf.RoundToInt(newvalue))
			{
				SendNetworkUpdate(BasePlayer.NetworkQueue.UpdateDistance);
			}
		}
	}

	public override float RepairCostFraction()
	{
		return 1f;
	}

	private bool CanRotate(BasePlayer player)
	{
		if (IsRotatable() && HasRotationPrivilege(player))
		{
			return !IsRotationBlocked();
		}
		return false;
	}

	private bool IsRotatable()
	{
		if (blockDefinition.grades == null)
		{
			return false;
		}
		if (!blockDefinition.canRotateAfterPlacement)
		{
			return false;
		}
		if (!HasFlag(Flags.Reserved1))
		{
			return false;
		}
		return true;
	}

	private bool IsRotationBlocked()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (children != null)
		{
			foreach (BaseEntity child in children)
			{
				if (child is TimedExplosive)
				{
					return true;
				}
			}
		}
		if (!blockDefinition.checkVolumeOnRotate)
		{
			return false;
		}
		DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(prefabID);
		return DeployVolume.Check(((Component)this).transform.position, ((Component)this).transform.rotation, volumes, ~(1 << ((Component)this).gameObject.layer));
	}

	private bool HasRotationPrivilege(BasePlayer player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return !player.IsBuildingBlocked(((Component)this).transform.position, ((Component)this).transform.rotation, bounds);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void DoRotation(RPCMessage msg)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (msg.player.CanInteract() && CanRotate(msg.player) && blockDefinition.canRotateAfterPlacement)
		{
			Transform transform = ((Component)this).transform;
			transform.localRotation *= Quaternion.Euler(blockDefinition.rotationAmount);
			RefreshEntityLinks();
			UpdateSurroundingEntities();
			UpdateSkin(force: true);
			RefreshNeighbours(linkToNeighbours: false);
			SendNetworkUpdateImmediate();
			ClientRPC(RpcTarget.NetworkGroup("RefreshSkin"));
			if (!globalNetworkCooldown)
			{
				globalNetworkCooldown = true;
				GlobalNetworkHandler.server.TrySendNetworkUpdate(this);
				((FacepunchBehaviour)this).CancelInvoke((Action)ResetGlobalNetworkCooldown);
				((FacepunchBehaviour)this).Invoke((Action)ResetGlobalNetworkCooldown, 15f);
			}
		}
	}

	private void ResetGlobalNetworkCooldown()
	{
		globalNetworkCooldown = false;
		GlobalNetworkHandler.server.TrySendNetworkUpdate(this);
	}

	private void StopBeingRotatable()
	{
		SetFlag(Flags.Reserved1, b: false);
		SendNetworkUpdate();
	}

	private void StartBeingRotatable()
	{
		if (blockDefinition.grades != null && blockDefinition.canRotateAfterPlacement)
		{
			SetFlag(Flags.Reserved1, b: true);
			((FacepunchBehaviour)this).Invoke((Action)StopBeingRotatable, 600f);
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.buildingBlock = Pool.Get<BuildingBlock>();
		info.msg.buildingBlock.model = modelState;
		info.msg.buildingBlock.grade = (int)grade;
		info.msg.buildingBlock.hasWallpaper = hasWallpaper;
		info.msg.buildingBlock.wallpaperID = wallpaperID;
		if (customColour != 0)
		{
			info.msg.simpleUint = Pool.Get<SimpleUInt>();
			info.msg.simpleUint.value = customColour;
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		customColour = 0u;
		if (info.msg.simpleUint != null)
		{
			customColour = info.msg.simpleUint.value;
		}
		if (info.msg.buildingBlock != null)
		{
			hasWallpaper = info.msg.buildingBlock.hasWallpaper;
			wallpaperID = info.msg.buildingBlock.wallpaperID;
			SetConditionalModel(info.msg.buildingBlock.model);
			SetGrade((BuildingGrade.Enum)info.msg.buildingBlock.grade);
		}
		if (info.fromDisk)
		{
			SetFlag(Flags.Reserved1, b: false);
			UpdateSkin();
		}
	}

	public override void AttachToBuilding(DecayEntity other)
	{
		if ((Object)(object)other != (Object)null && other is BuildingBlock)
		{
			AttachToBuilding(other.buildingID);
			BuildingManager.server.CheckMerge(this);
		}
		else
		{
			AttachToBuilding(BuildingManager.server.NewBuildingID());
		}
	}

	public override void ServerInit()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		blockDefinition = PrefabAttribute.server.Find<Construction>(prefabID);
		if (blockDefinition == null)
		{
			Debug.LogError((object)("Couldn't find Construction for prefab " + prefabID));
		}
		base.ServerInit();
		UpdateSkin();
		if (HasFlag(Flags.Reserved1) || !Application.isLoadingSave)
		{
			StartBeingRotatable();
		}
		if (!CullBushes || Application.isLoadingSave)
		{
			return;
		}
		List<BushEntity> list = Pool.GetList<BushEntity>();
		Vis.Entities(WorldSpaceBounds(), list, 67108864, (QueryTriggerInteraction)2);
		foreach (BushEntity item in list)
		{
			if (item.isServer)
			{
				item.Kill();
			}
		}
		Pool.FreeList<BushEntity>(ref list);
	}

	public override void Hurt(HitInfo info)
	{
		if (ConVar.Server.pve && Object.op_Implicit((Object)(object)info.Initiator) && info.Initiator is BasePlayer)
		{
			(info.Initiator as BasePlayer).Hurt(info.damageTypes.Total(), DamageType.Generic);
		}
		else if (!Object.op_Implicit((Object)(object)info.Initiator) || !(info.Initiator is BasePlayer basePlayer) || !basePlayer.IsInTutorial)
		{
			base.Hurt(info);
		}
	}

	public bool HasWallpaper()
	{
		return hasWallpaper;
	}

	public void SetWallpaper(ulong id)
	{
		if (!hasWallpaper || id != wallpaperID)
		{
			wallpaperID = id;
			hasWallpaper = true;
			if (base.isServer)
			{
				SetConditionalModel(currentSkin.DetermineConditionalModelState(this));
				SendNetworkUpdateImmediate();
				ClientRPC(RpcTarget.NetworkGroup("RefreshSkin"));
			}
		}
	}

	public void RemoveWallpaper()
	{
		hasWallpaper = false;
		if (base.isServer)
		{
			SetConditionalModel(currentSkin.DetermineConditionalModelState(this));
			SendNetworkUpdateImmediate();
			ClientRPC(RpcTarget.NetworkGroup("RefreshSkin"));
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_PickupWallpaperStart(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && CanPickup(rpc.player))
		{
			Item item = ItemManager.Create(WallpaperPlanner.WallpaperItemDef, 1, skinID);
			rpc.player.GiveItem(item, GiveItemReason.PickedUp);
			RemoveWallpaper();
		}
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (HasWallpaper() && player.CanBuild())
		{
			return player.IsHoldingEntity<Hammer>();
		}
		return false;
	}

	static BuildingBlock()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		Vector3[] array = new Vector3[5];
		Vector3 val = new Vector3(0f, 1f, 0f);
		array[0] = ((Vector3)(ref val)).normalized;
		val = new Vector3(1f, 1f, 0f);
		array[1] = ((Vector3)(ref val)).normalized;
		val = new Vector3(-1f, 1f, 0f);
		array[2] = ((Vector3)(ref val)).normalized;
		val = new Vector3(0f, 1f, 1f);
		array[3] = ((Vector3)(ref val)).normalized;
		val = new Vector3(0f, 1f, -1f);
		array[4] = ((Vector3)(ref val)).normalized;
		outsideLookupOffsets = (Vector3[])(object)array;
		updateSkinQueueServer = new UpdateSkinWorkQueue();
		RotateTitle = new Phrase("rotate", "Rotate");
		RotateDesc = new Phrase("rotate_building_desc", "Rotate or flip this block to face a different direction");
	}
}
