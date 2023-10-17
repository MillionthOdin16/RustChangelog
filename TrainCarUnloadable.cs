using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class TrainCarUnloadable : TrainCar
{
	public enum WagonType
	{
		Ore,
		Lootboxes,
		Fuel
	}

	[Header("Train Car Unloadable")]
	[SerializeField]
	private GameObjectRef storagePrefab;

	[SerializeField]
	private BoxCollider[] unloadingAreas;

	[SerializeField]
	private TrainCarFuelHatches fuelHatches;

	[SerializeField]
	private Transform orePlaneVisuals;

	[SerializeField]
	private Transform orePlaneColliderDetailed;

	[SerializeField]
	private Transform orePlaneColliderWorld;

	[SerializeField]
	[Range(0f, 1f)]
	public float vacuumStretchPercent = 0.5f;

	[SerializeField]
	private ParticleSystemContainer unloadingFXContainer;

	[SerializeField]
	private ParticleSystem unloadingFX;

	public WagonType wagonType;

	private int lootTypeIndex = -1;

	private List<EntityRef<LootContainer>> lootContainers = new List<EntityRef<LootContainer>>();

	private Vector3 _oreScale = Vector3.one;

	private float animPercent;

	private float prevAnimTime;

	[ServerVar(Help = "How long before an unloadable train car despawns afer being unloaded")]
	public static float decayminutesafterunload = 10f;

	private EntityRef<StorageContainer> storageInstance;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("TrainCarUnloadable.OnRpcMessage", 0);
		try
		{
			if (rpc == 4254195175u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_Open "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Open", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4254195175u, "RPC_Open", this, player, 3f))
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
							RPC_Open(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Open");
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

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old.HasFlag(Flags.Reserved4) != next.HasFlag(Flags.Reserved4) && (Object)(object)fuelHatches != (Object)null)
		{
			fuelHatches.LinedUpStateChanged(base.LinedUpToUnload);
		}
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (IsDead() || base.IsDestroyed)
		{
			return;
		}
		LootContainer lootContainer = default(LootContainer);
		if (((Component)child).TryGetComponent<LootContainer>(ref lootContainer))
		{
			if (base.isServer)
			{
				lootContainer.inventory.SetLocked(!IsEmpty());
			}
			lootContainers.Add(new EntityRef<LootContainer>(lootContainer.net.ID));
		}
		if (base.isServer && child.prefabID == storagePrefab.GetEntity().prefabID)
		{
			StorageContainer storageContainer = (StorageContainer)child;
			storageInstance.Set(storageContainer);
			if (!Application.isLoadingSave)
			{
				FillWithLoot(storageContainer);
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseTrain != null)
		{
			lootTypeIndex = info.msg.baseTrain.lootTypeIndex;
			if (base.isServer)
			{
				SetVisualOreLevel(info.msg.baseTrain.lootPercent);
			}
		}
	}

	public bool IsEmpty()
	{
		return GetOrePercent() == 0f;
	}

	public bool TryGetLootType(out TrainWagonLootData.LootOption lootOption)
	{
		return TrainWagonLootData.instance.TryGetLootFromIndex(lootTypeIndex, out lootOption);
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		if (!base.CanBeLooted(player))
		{
			return false;
		}
		return !IsEmpty();
	}

	public int GetFilledLootAmount()
	{
		if (TryGetLootType(out var lootOption))
		{
			return lootOption.maxLootAmount;
		}
		Debug.LogWarning((object)(((object)this).GetType().Name + ": Called GetFilledLootAmount without a lootTypeIndex set."));
		return 0;
	}

	public void SetVisualOreLevel(float percent)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)orePlaneColliderDetailed == (Object)null))
		{
			_oreScale.y = Mathf.Clamp01(percent);
			orePlaneColliderDetailed.localScale = _oreScale;
			if (base.isClient)
			{
				orePlaneVisuals.localScale = _oreScale;
				((Component)orePlaneVisuals).gameObject.SetActive(percent > 0f);
			}
			if (base.isServer)
			{
				orePlaneColliderWorld.localScale = _oreScale;
			}
		}
	}

	private void AnimateUnload(float startPercent)
	{
		prevAnimTime = Time.time;
		animPercent = startPercent;
		if (base.isClient && (Object)(object)unloadingFXContainer != (Object)null)
		{
			unloadingFXContainer.Play();
		}
		((FacepunchBehaviour)this).InvokeRepeating((Action)UnloadAnimTick, 0f, 0f);
	}

	private void UnloadAnimTick()
	{
		animPercent -= (Time.time - prevAnimTime) / 40f;
		SetVisualOreLevel(animPercent);
		prevAnimTime = Time.time;
		if (animPercent <= 0f)
		{
			EndUnloadAnim();
		}
	}

	private void EndUnloadAnim()
	{
		if (base.isClient && (Object)(object)unloadingFXContainer != (Object)null)
		{
			unloadingFXContainer.Stop();
		}
		((FacepunchBehaviour)this).CancelInvoke((Action)UnloadAnimTick);
	}

	public float GetOrePercent()
	{
		if (base.isServer)
		{
			return TrainWagonLootData.GetOrePercent(lootTypeIndex, GetStorageContainer());
		}
		return 0f;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.baseTrain = Pool.Get<BaseTrain>();
		info.msg.baseTrain.lootTypeIndex = lootTypeIndex;
		info.msg.baseTrain.lootPercent = GetOrePercent();
	}

	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			foreach (EntityRef<LootContainer> lootContainer2 in lootContainers)
			{
				LootContainer lootContainer = lootContainer2.Get(base.isServer);
				if ((Object)(object)lootContainer != (Object)null && lootContainer.inventory != null && !lootContainer.inventory.IsLocked())
				{
					lootContainer.DropItems();
				}
			}
		}
		base.DoServerDestroy();
	}

	public bool IsLinedUpToUnload(BoxCollider unloaderBounds)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		BoxCollider[] array = unloadingAreas;
		foreach (BoxCollider val in array)
		{
			Bounds val2 = ((Collider)unloaderBounds).bounds;
			if (((Bounds)(ref val2)).Intersects(((Collider)val).bounds))
			{
				return true;
			}
		}
		return false;
	}

	public void FillWithLoot(StorageContainer sc)
	{
		sc.inventory.Clear();
		ItemManager.DoRemoves();
		TrainWagonLootData.LootOption lootOption = TrainWagonLootData.instance.GetLootOption(wagonType, out lootTypeIndex);
		int amount = Random.Range(lootOption.minLootAmount, lootOption.maxLootAmount);
		ItemDefinition itemToCreate = ItemManager.FindItemDefinition(lootOption.lootItem.itemid);
		sc.inventory.AddItem(itemToCreate, amount, 0uL, ItemContainer.LimitStack.All);
		sc.inventory.SetLocked(isLocked: true);
		SetVisualOreLevel(GetOrePercent());
		SendNetworkUpdate();
	}

	public void EmptyOutLoot(StorageContainer sc)
	{
		sc.inventory.Clear();
		ItemManager.DoRemoves();
		SetVisualOreLevel(GetOrePercent());
		SendNetworkUpdate();
	}

	public void BeginUnloadAnimation()
	{
		float orePercent = GetOrePercent();
		AnimateUnload(orePercent);
		ClientRPC(null, "RPC_AnimateUnload", orePercent);
	}

	public void EndEmptyProcess()
	{
		float orePercent = GetOrePercent();
		if (!(orePercent > 0f))
		{
			lootTypeIndex = -1;
			foreach (EntityRef<LootContainer> lootContainer2 in lootContainers)
			{
				LootContainer lootContainer = lootContainer2.Get(base.isServer);
				if ((Object)(object)lootContainer != (Object)null && lootContainer.inventory != null)
				{
					lootContainer.inventory.SetLocked(isLocked: false);
				}
			}
		}
		SetVisualOreLevel(orePercent);
		ClientRPC(null, "RPC_StopAnimateUnload", orePercent);
		decayingFor = 0f;
	}

	public StorageContainer GetStorageContainer()
	{
		StorageContainer storageContainer = storageInstance.Get(base.isServer);
		if (storageContainer.IsValid())
		{
			return storageContainer;
		}
		return null;
	}

	protected override float GetDecayMinutes(bool hasPassengers)
	{
		if ((wagonType == WagonType.Ore || wagonType == WagonType.Fuel) && !hasPassengers && IsEmpty())
		{
			return decayminutesafterunload;
		}
		return base.GetDecayMinutes(hasPassengers);
	}

	protected override bool CanDieFromDecayNow()
	{
		if (IsEmpty())
		{
			return true;
		}
		return base.CanDieFromDecayNow();
	}

	public override bool AdminFixUp(int tier)
	{
		if (!base.AdminFixUp(tier))
		{
			return false;
		}
		StorageContainer storageContainer = GetStorageContainer();
		if (storageContainer.IsValid())
		{
			if (tier > 1)
			{
				FillWithLoot(storageContainer);
			}
			else
			{
				EmptyOutLoot(storageContainer);
			}
		}
		return true;
	}

	public float MinDistToUnloadingArea(Vector3 point)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		point.y = 0f;
		BoxCollider[] array = unloadingAreas;
		foreach (BoxCollider val in array)
		{
			Vector3 val2 = ((Component)val).transform.position + ((Component)val).transform.rotation * val.center;
			val2.y = 0f;
			float num2 = Vector3.Distance(point, val2);
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_Open(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && CanBeLooted(player))
		{
			StorageContainer storageContainer = GetStorageContainer();
			if (storageContainer.IsValid())
			{
				storageContainer.PlayerOpenLoot(player);
			}
			else
			{
				Debug.LogError((object)(((object)this).GetType().Name + ": No container component found."));
			}
		}
	}
}
