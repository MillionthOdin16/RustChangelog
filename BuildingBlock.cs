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

		public const Flags CanDemolish = Flags.Reserved2;
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

	private bool forceSkinRefresh;

	private ulong lastSkinID;

	private int modelState;

	private int lastModelState;

	private uint lastCustomColour;

	private uint playerCustomColourToApply;

	public BuildingGrade.Enum grade;

	private BuildingGrade.Enum lastGrade = BuildingGrade.Enum.None;

	private ConstructionSkin currentSkin;

	private DeferredAction skinChange;

	private MeshRenderer placeholderRenderer;

	private MeshCollider placeholderCollider;

	public static UpdateSkinWorkQueue updateSkinQueueServer = new UpdateSkinWorkQueue();

	public bool CullBushes;

	public bool CheckForPipesOnModelChange;

	[NonSerialized]
	public Construction blockDefinition;

	private static Vector3[] outsideLookupOffsets;

	public uint customColour { get; private set; }

	public ConstructionGrade currentGrade => blockDefinition.GetGrade(grade, skinID);

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BuildingBlock.OnRpcMessage", 0);
		try
		{
			if (rpc == 2858062413u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoDemolish "));
				}
				TimeWarning val2 = TimeWarning.New("DoDemolish", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2858062413u, "DoDemolish", this, player, 3f))
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
							DoDemolish(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoDemolish");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 216608990 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoImmediateDemolish "));
				}
				TimeWarning val2 = TimeWarning.New("DoImmediateDemolish", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(216608990u, "DoImmediateDemolish", this, player, 3f))
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
							DoImmediateDemolish(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in DoImmediateDemolish");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1956645865 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoRotation "));
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
							RPCMessage msg4 = rPCMessage;
							DoRotation(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
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
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoUpgradeToGrade "));
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
							RPCMessage msg5 = rPCMessage;
							DoUpgradeToGrade(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
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
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoUpgradeToGrade_Delayed "));
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
							RPCMessage msg6 = rPCMessage;
							DoUpgradeToGrade_Delayed(msg6);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in DoUpgradeToGrade_Delayed");
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

	private bool CanDemolish(BasePlayer player)
	{
		if (IsDemolishable())
		{
			return HasDemolishPrivilege(player);
		}
		return false;
	}

	private bool IsDemolishable()
	{
		if (!ConVar.Server.pve && !HasFlag(Flags.Reserved2))
		{
			return false;
		}
		return true;
	}

	private bool HasDemolishPrivilege(BasePlayer player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return player.IsBuildingAuthed(((Component)this).transform.position, ((Component)this).transform.rotation, bounds);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void DoDemolish(RPCMessage msg)
	{
		if (msg.player.CanInteract() && CanDemolish(msg.player))
		{
			Analytics.Azure.OnBuildingBlockDemolished(msg.player, this);
			Kill(DestroyMode.Gib);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void DoImmediateDemolish(RPCMessage msg)
	{
		if (msg.player.CanInteract() && msg.player.IsAdmin)
		{
			Analytics.Azure.OnBuildingBlockDemolished(msg.player, this);
			Kill(DestroyMode.Gib);
		}
	}

	private void StopBeingDemolishable()
	{
		SetFlag(Flags.Reserved2, b: false);
		SendNetworkUpdate();
	}

	private void StartBeingDemolishable()
	{
		SetFlag(Flags.Reserved2, b: true);
		((FacepunchBehaviour)this).Invoke((Action)StopBeingDemolishable, 600f);
	}

	public void SetConditionalModel(int state)
	{
		modelState = state;
	}

	public bool GetConditionalModel(int index)
	{
		return (modelState & (1 << index)) != 0;
	}

	private bool CanChangeToGrade(BuildingGrade.Enum iGrade, ulong iSkin, BasePlayer player)
	{
		if (HasUpgradePrivilege(iGrade, iSkin, player))
		{
			return !IsUpgradeBlocked();
		}
		return false;
	}

	private bool HasUpgradePrivilege(BuildingGrade.Enum iGrade, ulong iSkin, BasePlayer player)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
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
			ClientRPC(null, "DoUpgradeEffect", (int)@enum, num);
			Analytics.Azure.OnBuildingBlockUpgraded(msg.player, this, @enum, playerCustomColourToApply);
			OnSkinChanged(skinID, num);
			ChangeGrade(@enum, playEffect: true);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void DoUpgradeToGrade(RPCMessage msg)
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
			ClientRPC(null, "DoUpgradeEffect", (int)@enum, num);
			Analytics.Azure.OnBuildingBlockUpgraded(msg.player, this, @enum, playerCustomColourToApply);
			OnSkinChanged(skinID, num);
			ChangeGrade(@enum, playEffect: true);
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
		BuildingManager.server.GetBuilding(buildingID)?.Dirty();
	}

	private void PayForUpgrade(ConstructionGrade g, BasePlayer player)
	{
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
			ClientRPC(null, "RefreshSkin");
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
			Debug.LogWarning((object)("No skins found for " + ((Component)this).gameObject));
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
			modelState = currentSkin.DetermineConditionalModelState(this);
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

	public void CheckForPipes()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckForPipesOnModelChange || !ConVar.Server.enforcePipeChecksOnBuildingBlockChanges || Application.isLoading)
		{
			return;
		}
		List<ColliderInfo_Pipe> list = Pool.GetList<ColliderInfo_Pipe>();
		Vis.Components<ColliderInfo_Pipe>(new OBB(((Component)this).transform, bounds), list, 536870912, (QueryTriggerInteraction)2);
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
		if (base.isServer && Mathf.RoundToInt(oldvalue) != Mathf.RoundToInt(newvalue))
		{
			SendNetworkUpdate(BasePlayer.NetworkQueue.UpdateDistance);
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
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
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
			ClientRPC(null, "RefreshSkin");
		}
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
			SetConditionalModel(info.msg.buildingBlock.model);
			SetGrade((BuildingGrade.Enum)info.msg.buildingBlock.grade);
		}
		if (info.fromDisk)
		{
			SetFlag(Flags.Reserved2, b: false);
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
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
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
		if (HasFlag(Flags.Reserved2) || !Application.isLoadingSave)
		{
			StartBeingDemolishable();
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
		else
		{
			base.Hurt(info);
		}
	}

	public override void ResetState()
	{
		base.ResetState();
		blockDefinition = null;
		forceSkinRefresh = false;
		modelState = 0;
		lastModelState = 0;
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

	static BuildingBlock()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
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
	}
}
