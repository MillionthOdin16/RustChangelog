using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class SprayCan : HeldEntity
{
	private enum SprayFailReason
	{
		None,
		MountedBlocked,
		IOConnection,
		LineOfSight,
		SkinNotOwned,
		InvalidItem
	}

	private struct ContainerSet
	{
		public int ContainerIndex;

		public uint PrefabId;
	}

	private struct ChildPreserveInfo
	{
		public BaseEntity TargetEntity;

		public uint TargetBone;

		public Vector3 LocalPosition;

		public Quaternion LocalRotation;
	}

	public const float MaxFreeSprayDistanceFromStart = 10f;

	public const float MaxFreeSprayStartingDistance = 3f;

	private SprayCanSpray_Freehand paintingLine = null;

	public const Flags IsFreeSpraying = Flags.Reserved1;

	public SoundDefinition SpraySound = null;

	public GameObjectRef SkinSelectPanel;

	public float SprayCooldown = 2f;

	public float ConditionLossPerSpray = 10f;

	public float ConditionLossPerReskin = 10f;

	public GameObjectRef LinePrefab = null;

	public Color[] SprayColours = (Color[])(object)new Color[0];

	public float[] SprayWidths = new float[3] { 0.1f, 0.2f, 0.3f };

	public ParticleSystem worldSpaceSprayFx;

	public GameObjectRef ReskinEffect;

	public ItemDefinition SprayDecalItem = null;

	public GameObjectRef SprayDecalEntityRef = null;

	public SteamInventoryItem FreeSprayUnlockItem = null;

	public MinMaxGradient DecalSprayGradient;

	public SoundDefinition SprayLoopDef;

	public static Phrase FreeSprayNamePhrase = new Phrase("freespray_radial", "Free Spray");

	public static Phrase FreeSprayDescPhrase = new Phrase("freespray_radial_desc", "Spray shapes freely with various colors");

	public static Phrase BuildingSkinDefaultPhrase = new Phrase("buildingskin_default", "Automatic colour");

	public static Phrase BuildingSkinDefaultDescPhrase = new Phrase("buildingskin_default_desc", "Reset the block to random colouring");

	public static Phrase BuildingSkinColourPhrase = new Phrase("buildingskin_colour", "Set colour");

	public static Phrase BuildingSkinColourDescPhrase = new Phrase("buildingskin_colour_desc", "Set the block to the highlighted colour");

	[FormerlySerializedAs("ShippingCOntainerColourLookup")]
	public ConstructionSkin_ColourLookup ShippingContainerColourLookup;

	public const string ENEMY_BASE_STAT = "sprayed_enemy_base";

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SprayCan.OnRpcMessage", 0);
		try
		{
			if (rpc == 3490735573u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - BeginFreehandSpray "));
				}
				TimeWarning val2 = TimeWarning.New("BeginFreehandSpray", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(3490735573u, "BeginFreehandSpray", this, player))
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
						TimeWarning val4 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							BeginFreehandSpray(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in BeginFreehandSpray");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 151738090 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ChangeItemSkin "));
				}
				TimeWarning val5 = TimeWarning.New("ChangeItemSkin", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(151738090u, "ChangeItemSkin", this, player, 2uL))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(151738090u, "ChangeItemSkin", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
					try
					{
						TimeWarning val7 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							ChangeItemSkin(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ChangeItemSkin");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 396000799 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - CreateSpray "));
				}
				TimeWarning val8 = TimeWarning.New("CreateSpray", 0);
				try
				{
					TimeWarning val9 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(396000799u, "CreateSpray", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val9)?.Dispose();
					}
					try
					{
						TimeWarning val10 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg4 = rPCMessage;
							CreateSpray(msg4);
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in CreateSpray");
					}
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
				}
				return true;
			}
			if (rpc == 14517645 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_SetBlockColourId "));
				}
				TimeWarning val11 = TimeWarning.New("Server_SetBlockColourId", 0);
				try
				{
					TimeWarning val12 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(14517645u, "Server_SetBlockColourId", this, player, 3uL))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(14517645u, "Server_SetBlockColourId", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val12)?.Dispose();
					}
					try
					{
						TimeWarning val13 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg5 = rPCMessage;
							Server_SetBlockColourId(msg5);
						}
						finally
						{
							((IDisposable)val13)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in Server_SetBlockColourId");
					}
				}
				finally
				{
					((IDisposable)val11)?.Dispose();
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

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void BeginFreehandSpray(RPCMessage msg)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		if (!IsBusy() && CanSprayFreehand(msg.player))
		{
			Vector3 val = msg.read.Vector3();
			Vector3 atNormal = msg.read.Vector3();
			int num = msg.read.Int32();
			int num2 = msg.read.Int32();
			if (num >= 0 && num < SprayColours.Length && num2 >= 0 && num2 < SprayWidths.Length && !(Vector3.Distance(val, ((Component)GetOwnerPlayer()).transform.position) > 3f))
			{
				SprayCanSpray_Freehand sprayCanSpray_Freehand = GameManager.server.CreateEntity(LinePrefab.resourcePath, val, Quaternion.identity) as SprayCanSpray_Freehand;
				sprayCanSpray_Freehand.AddInitialPoint(atNormal);
				sprayCanSpray_Freehand.SetColour(SprayColours[num]);
				sprayCanSpray_Freehand.SetWidth(SprayWidths[num2]);
				sprayCanSpray_Freehand.EnableChanges(msg.player);
				sprayCanSpray_Freehand.Spawn();
				paintingLine = sprayCanSpray_Freehand;
				ClientRPC(null, "Client_ChangeSprayColour", num);
				SetFlag(Flags.Busy, b: true);
				SetFlag(Flags.Reserved1, b: true);
				CheckAchievementPosition(val);
			}
		}
	}

	public void ClearPaintingLine(bool allowNewSprayImmediately)
	{
		paintingLine = null;
		LoseCondition(ConditionLossPerSpray);
		if (allowNewSprayImmediately)
		{
			ClearBusy();
		}
		else
		{
			((FacepunchBehaviour)this).Invoke((Action)ClearBusy, 0.1f);
		}
	}

	public bool CanSprayFreehand(BasePlayer player)
	{
		if (player.UnlockAllSkins)
		{
			return true;
		}
		return (Object)(object)FreeSprayUnlockItem != (Object)null && (player.blueprints.steamInventory.HasItem(FreeSprayUnlockItem.id) || FreeSprayUnlockItem.HasUnlocked(player.userID));
	}

	private bool IsSprayBlockedByTrigger(Vector3 pos)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)ownerPlayer == (Object)null)
		{
			return true;
		}
		TriggerNoSpray triggerNoSpray = ownerPlayer.FindTrigger<TriggerNoSpray>();
		if ((Object)(object)triggerNoSpray == (Object)null)
		{
			return false;
		}
		return !triggerNoSpray.IsPositionValid(pos);
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.CallsPerSecond(2uL)]
	private void ChangeItemSkin(RPCMessage msg)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_079e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_0525: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		if (IsBusy())
		{
			return;
		}
		NetworkableId uid = msg.read.EntityID();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		int targetSkin = msg.read.Int32();
		if ((Object)(object)msg.player == (Object)null || !msg.player.CanBuild())
		{
			return;
		}
		bool flag = false;
		if (msg.player.UnlockAllSkins)
		{
			flag = true;
		}
		if (targetSkin != 0 && !flag && !msg.player.blueprints.CheckSkinOwnership(targetSkin, msg.player.userID))
		{
			SprayFailResponse(SprayFailReason.SkinNotOwned);
			return;
		}
		if ((Object)(object)baseNetworkable != (Object)null && baseNetworkable is BaseEntity baseEntity2)
		{
			OBB val = baseEntity2.WorldSpaceBounds();
			Vector3 position = ((OBB)(ref val)).ClosestPoint(msg.player.eyes.position);
			if (!msg.player.IsVisible(position, 3f))
			{
				SprayFailResponse(SprayFailReason.LineOfSight);
				return;
			}
			if (baseNetworkable is Door door)
			{
				if (!door.GetPlayerLockPermission(msg.player))
				{
					msg.player.ChatMessage("Door must be openable");
					return;
				}
				if (door.IsOpen())
				{
					msg.player.ChatMessage("Door must be closed");
					return;
				}
			}
			if (!GetItemDefinitionForEntity(baseEntity2, out var def))
			{
				SprayFailResponse(SprayFailReason.InvalidItem);
				return;
			}
			ItemDefinition itemDefinition = null;
			ulong num = ItemDefinition.FindSkin(def.itemid, targetSkin);
			ItemSkinDirectory.Skin skin = def.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => x.id == targetSkin);
			if ((Object)(object)skin.invItem != (Object)null && skin.invItem is ItemSkin itemSkin)
			{
				if ((Object)(object)itemSkin.Redirect != (Object)null)
				{
					itemDefinition = itemSkin.Redirect;
				}
				else if (GetItemDefinitionForEntity(baseEntity2, out def, useRedirect: false) && (Object)(object)def.isRedirectOf != (Object)null)
				{
					itemDefinition = def.isRedirectOf;
				}
			}
			else if ((Object)(object)def.isRedirectOf != (Object)null || (GetItemDefinitionForEntity(baseEntity2, out def, useRedirect: false) && (Object)(object)def.isRedirectOf != (Object)null))
			{
				itemDefinition = def.isRedirectOf;
			}
			if ((Object)(object)itemDefinition == (Object)null)
			{
				baseEntity2.skinID = num;
				baseEntity2.SendNetworkUpdate();
				Analytics.Server.SkinUsed(def.shortname, targetSkin);
			}
			else
			{
				if (!CanEntityBeRespawned(baseEntity2, out var reason2))
				{
					SprayFailResponse(reason2);
					return;
				}
				if (!GetEntityPrefabPath(itemDefinition, out var resourcePath))
				{
					Debug.LogWarning((object)("Cannot find resource path of redirect entity to spawn! " + ((Object)((Component)itemDefinition).gameObject).name));
					SprayFailResponse(SprayFailReason.InvalidItem);
					return;
				}
				Vector3 localPosition = ((Component)baseEntity2).transform.localPosition;
				Quaternion localRotation = ((Component)baseEntity2).transform.localRotation;
				BaseEntity baseEntity3 = baseEntity2.GetParentEntity();
				float health = baseEntity2.Health();
				EntityRef[] slots = baseEntity2.GetSlots();
				float lastAttackedTime = ((baseEntity2 is BaseCombatEntity baseCombatEntity) ? baseCombatEntity.lastAttackedTime : 0f);
				bool flag2 = baseEntity2 is Door;
				Dictionary<ContainerSet, List<Item>> dictionary2 = new Dictionary<ContainerSet, List<Item>>();
				SaveEntityStorage(baseEntity2, dictionary2, 0);
				List<ChildPreserveInfo> list = Pool.GetList<ChildPreserveInfo>();
				if (flag2)
				{
					foreach (BaseEntity child in baseEntity2.children)
					{
						list.Add(new ChildPreserveInfo
						{
							TargetEntity = child,
							TargetBone = child.parentBone,
							LocalPosition = ((Component)child).transform.localPosition,
							LocalRotation = ((Component)child).transform.localRotation
						});
					}
					foreach (ChildPreserveInfo item in list)
					{
						item.TargetEntity.SetParent(null, worldPositionStays: true);
					}
				}
				else
				{
					for (int i = 0; i < baseEntity2.children.Count; i++)
					{
						BaseEntity baseEntity4 = baseEntity2.children[i];
						SaveEntityStorage(baseEntity4, dictionary2, -1);
					}
				}
				baseEntity2.Kill();
				baseEntity2 = GameManager.server.CreateEntity(resourcePath, ((Object)(object)baseEntity3 != (Object)null) ? ((Component)baseEntity3).transform.TransformPoint(localPosition) : localPosition, ((Object)(object)baseEntity3 != (Object)null) ? (((Component)baseEntity3).transform.rotation * localRotation) : localRotation);
				baseEntity2.SetParent(baseEntity3);
				((Component)baseEntity2).transform.localPosition = localPosition;
				((Component)baseEntity2).transform.localRotation = localRotation;
				if (GetItemDefinitionForEntity(baseEntity2, out var def2, useRedirect: false) && (Object)(object)def2.isRedirectOf != (Object)null)
				{
					baseEntity2.skinID = 0uL;
				}
				else
				{
					baseEntity2.skinID = num;
				}
				if (baseEntity2 is DecayEntity decayEntity)
				{
					decayEntity.AttachToBuilding(null);
				}
				baseEntity2.Spawn();
				if (baseEntity2 is BaseCombatEntity baseCombatEntity2)
				{
					baseCombatEntity2.SetHealth(health);
					baseCombatEntity2.lastAttackedTime = lastAttackedTime;
				}
				if (dictionary2.Count > 0)
				{
					RestoreEntityStorage(baseEntity2, 0, dictionary2);
					if (!flag2)
					{
						for (int j = 0; j < baseEntity2.children.Count; j++)
						{
							BaseEntity baseEntity5 = baseEntity2.children[j];
							RestoreEntityStorage(baseEntity5, -1, dictionary2);
						}
					}
					foreach (KeyValuePair<ContainerSet, List<Item>> item2 in dictionary2)
					{
						foreach (Item item3 in item2.Value)
						{
							Debug.Log((object)$"Deleting {item3} as it has no new container");
							item3.Remove();
						}
					}
					Analytics.Server.SkinUsed(def.shortname, targetSkin);
				}
				if (flag2)
				{
					foreach (ChildPreserveInfo item4 in list)
					{
						item4.TargetEntity.SetParent(baseEntity2, item4.TargetBone, worldPositionStays: true);
						((Component)item4.TargetEntity).transform.localPosition = item4.LocalPosition;
						((Component)item4.TargetEntity).transform.localRotation = item4.LocalRotation;
						item4.TargetEntity.SendNetworkUpdate();
					}
					baseEntity2.SetSlots(slots);
				}
				Pool.FreeList<ChildPreserveInfo>(ref list);
			}
			ClientRPC<int, NetworkableId>(null, "Client_ReskinResult", 1, baseEntity2.net.ID);
		}
		LoseCondition(ConditionLossPerReskin);
		ClientRPC(null, "Client_ChangeSprayColour", -1);
		SetFlag(Flags.Busy, b: true);
		((FacepunchBehaviour)this).Invoke((Action)ClearBusy, SprayCooldown);
		static void RestoreEntityStorage(BaseEntity baseEntity, int index, Dictionary<ContainerSet, List<Item>> copy)
		{
			if (baseEntity is IItemContainerEntity itemContainerEntity)
			{
				ContainerSet containerSet = default(ContainerSet);
				containerSet.ContainerIndex = index;
				containerSet.PrefabId = ((index != 0) ? baseEntity.prefabID : 0u);
				ContainerSet key = containerSet;
				if (copy.ContainsKey(key))
				{
					foreach (Item item5 in copy[key])
					{
						item5.MoveToContainer(itemContainerEntity.inventory);
					}
					copy.Remove(key);
				}
			}
		}
		static void SaveEntityStorage(BaseEntity baseEntity, Dictionary<ContainerSet, List<Item>> dictionary, int index)
		{
			if (baseEntity is IItemContainerEntity itemContainerEntity2)
			{
				ContainerSet containerSet2 = default(ContainerSet);
				containerSet2.ContainerIndex = index;
				containerSet2.PrefabId = ((index != 0) ? baseEntity.prefabID : 0u);
				ContainerSet key2 = containerSet2;
				if (!dictionary.ContainsKey(key2))
				{
					dictionary.Add(key2, new List<Item>());
					foreach (Item item6 in itemContainerEntity2.inventory.itemList)
					{
						dictionary[key2].Add(item6);
					}
					{
						foreach (Item item7 in dictionary[key2])
						{
							item7.RemoveFromContainer();
						}
						return;
					}
				}
				Debug.Log((object)"Multiple containers with the same prefab id being added during vehicle reskin");
			}
		}
		void SprayFailResponse(SprayFailReason reason)
		{
			ClientRPC(null, "Client_ReskinResult", 0, (int)reason);
		}
	}

	private bool GetEntityPrefabPath(ItemDefinition def, out string resourcePath)
	{
		resourcePath = string.Empty;
		ItemModDeployable itemModDeployable = default(ItemModDeployable);
		if (((Component)def).TryGetComponent<ItemModDeployable>(ref itemModDeployable))
		{
			resourcePath = itemModDeployable.entityPrefab.resourcePath;
			return true;
		}
		ItemModEntity itemModEntity = default(ItemModEntity);
		if (((Component)def).TryGetComponent<ItemModEntity>(ref itemModEntity))
		{
			resourcePath = itemModEntity.entityPrefab.resourcePath;
			return true;
		}
		ItemModEntityReference itemModEntityReference = default(ItemModEntityReference);
		if (((Component)def).TryGetComponent<ItemModEntityReference>(ref itemModEntityReference))
		{
			resourcePath = itemModEntityReference.entityPrefab.resourcePath;
			return true;
		}
		return false;
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void CreateSpray(RPCMessage msg)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		if (IsBusy())
		{
			return;
		}
		ClientRPC(null, "Client_ChangeSprayColour", -1);
		SetFlag(Flags.Busy, b: true);
		((FacepunchBehaviour)this).Invoke((Action)ClearBusy, SprayCooldown);
		Vector3 val = msg.read.Vector3();
		Vector3 val2 = msg.read.Vector3();
		Vector3 val3 = msg.read.Vector3();
		int num = msg.read.Int32();
		if (!(Vector3.Distance(val, ((Component)this).transform.position) > 4.5f))
		{
			Plane val4 = default(Plane);
			((Plane)(ref val4))._002Ector(val2, val);
			Vector3 val5 = ((Plane)(ref val4)).ClosestPointOnPlane(val3);
			Vector3 val6 = val5 - val;
			Quaternion val7 = Quaternion.LookRotation(((Vector3)(ref val6)).normalized, val2);
			val7 *= Quaternion.Euler(0f, 0f, 90f);
			bool flag = false;
			if (msg.player.IsDeveloper)
			{
				flag = true;
			}
			if (num != 0 && !flag && !msg.player.blueprints.CheckSkinOwnership(num, msg.player.userID))
			{
				Debug.Log((object)$"SprayCan.ChangeItemSkin player does not have item :{num}:");
				return;
			}
			ulong num2 = ItemDefinition.FindSkin(SprayDecalItem.itemid, num);
			BaseEntity baseEntity = GameManager.server.CreateEntity(SprayDecalEntityRef.resourcePath, val, val7);
			baseEntity.skinID = num2;
			baseEntity.OnDeployed(null, GetOwnerPlayer(), GetItem());
			baseEntity.Spawn();
			CheckAchievementPosition(val);
			LoseCondition(ConditionLossPerSpray);
		}
	}

	private void CheckAchievementPosition(Vector3 pos)
	{
	}

	private void LoseCondition(float amount)
	{
		GetOwnerItem()?.LoseCondition(amount);
	}

	public void ClearBusy()
	{
		SetFlag(Flags.Busy, b: false);
		SetFlag(Flags.Reserved1, b: false);
	}

	public override void OnHeldChanged()
	{
		if (IsDisabled())
		{
			ClearBusy();
			if ((Object)(object)paintingLine != (Object)null)
			{
				paintingLine.Kill();
			}
			paintingLine = null;
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.CallsPerSecond(3uL)]
	private void Server_SetBlockColourId(RPCMessage msg)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		NetworkableId uid = msg.read.EntityID();
		uint num = msg.read.UInt32();
		BasePlayer player = msg.player;
		SetFlag(Flags.Busy, b: true);
		((FacepunchBehaviour)this).Invoke((Action)ClearBusy, 0.1f);
		if ((Object)(object)player == (Object)null || !player.CanBuild())
		{
			return;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		BuildingBlock buildingBlock = BaseNetworkable.serverEntities.Find(uid) as BuildingBlock;
		if ((Object)(object)buildingBlock != (Object)null)
		{
			float num2 = player.Distance((BaseEntity)buildingBlock);
			if (num2 > 4f)
			{
				return;
			}
			uint customColour = buildingBlock.customColour;
			buildingBlock.SetCustomColour(num);
			Analytics.Azure.OnBuildingBlockColorChanged(ownerPlayer, buildingBlock, customColour, num);
		}
		if ((Object)(object)ownerPlayer != (Object)null)
		{
			ownerPlayer.LastBlockColourChangeId = num;
		}
	}

	private bool CanEntityBeRespawned(BaseEntity targetEntity, out SprayFailReason reason)
	{
		if (targetEntity is BaseMountable baseMountable && baseMountable.AnyMounted())
		{
			reason = SprayFailReason.MountedBlocked;
			return false;
		}
		if (targetEntity.isServer && targetEntity is BaseVehicle baseVehicle && (baseVehicle.HasDriver() || baseVehicle.AnyMounted()))
		{
			reason = SprayFailReason.MountedBlocked;
			return false;
		}
		if (targetEntity is IOEntity iOEntity && (iOEntity.GetConnectedInputCount() != 0 || iOEntity.GetConnectedOutputCount() != 0))
		{
			reason = SprayFailReason.IOConnection;
			return false;
		}
		reason = SprayFailReason.None;
		return true;
	}

	public static bool GetItemDefinitionForEntity(BaseEntity be, out ItemDefinition def, bool useRedirect = true)
	{
		def = null;
		if (be is BaseCombatEntity baseCombatEntity)
		{
			if (baseCombatEntity.pickup.enabled && (Object)(object)baseCombatEntity.pickup.itemTarget != (Object)null)
			{
				def = baseCombatEntity.pickup.itemTarget;
			}
			else if (baseCombatEntity.repair.enabled && (Object)(object)baseCombatEntity.repair.itemTarget != (Object)null)
			{
				def = baseCombatEntity.repair.itemTarget;
			}
		}
		if (useRedirect && (Object)(object)def != (Object)null && (Object)(object)def.isRedirectOf != (Object)null)
		{
			def = def.isRedirectOf;
		}
		return (Object)(object)def != (Object)null;
	}
}
