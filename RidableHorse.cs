using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class RidableHorse : BaseRidableAnimal
{
	public Phrase SwapToSingleTitle;

	public Phrase SwapToSingleDescription;

	public Sprite SwapToSingleIcon;

	public Phrase SwapToDoubleTitle;

	public Phrase SwapToDoubleDescription;

	public Sprite SwapToDoubleIcon;

	public ItemDefinition WildSaddleItem;

	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 2f;

	public string distanceStatName = "";

	public HorseBreed[] breeds;

	public SkinnedMeshRenderer[] bodyRenderers;

	public SkinnedMeshRenderer[] hairRenderers;

	private int currentBreed = -1;

	private ProtectionProperties riderProtection;

	private ProtectionProperties baseHorseProtection;

	public const Flags Flag_HideHair = Flags.Reserved4;

	public const Flags Flag_WoodArmor = Flags.Reserved5;

	public const Flags Flag_RoadsignArmor = Flags.Reserved6;

	public const Flags Flag_HasSingleSaddle = Flags.Reserved9;

	public const Flags Flag_HasDoubleSaddle = Flags.Reserved10;

	private float equipmentSpeedMod;

	private int prevBreed;

	private int prevSlots;

	private static Material[] breedAssignmentArray = (Material[])(object)new Material[2];

	private float distanceRecordingSpacing = 5f;

	private HitchTrough currentHitch;

	private float totalDistance;

	private float kmDistance;

	private float tempDistanceTravelled;

	public override float RealisticMass => 550f;

	protected override float PositionTickRate => 0.05f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("RidableHorse.OnRpcMessage", 0);
		try
		{
			if (rpc == 1765203204 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_ReqSwapSaddleType "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ReqSwapSaddleType", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1765203204u, "RPC_ReqSwapSaddleType", this, player, 3f))
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
							RPC_ReqSwapSaddleType(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_ReqSwapSaddleType");
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

	public int GetStorageSlotCount()
	{
		return numStorageSlots;
	}

	public void ApplyBreed(int index)
	{
		if (currentBreed != index)
		{
			if (index >= breeds.Length || index < 0)
			{
				Debug.LogError((object)("ApplyBreed issue! index is " + index + " breed length is : " + breeds.Length));
				return;
			}
			ApplyBreedInternal(breeds[index]);
			currentBreed = index;
		}
	}

	protected void ApplyBreedInternal(HorseBreed breed)
	{
		if (base.isServer)
		{
			SetMaxHealth(StartHealth() * breed.maxHealth);
			base.health = MaxHealth();
		}
	}

	public HorseBreed GetBreed()
	{
		if (currentBreed == -1 || currentBreed >= breeds.Length)
		{
			return null;
		}
		return breeds[currentBreed];
	}

	public override float GetTrotSpeed()
	{
		float num = equipmentSpeedMod / (base.GetRunSpeed() * GetBreed().maxSpeed);
		return base.GetTrotSpeed() * GetBreed().maxSpeed * (1f + num);
	}

	public override float GetRunSpeed()
	{
		float num = base.GetRunSpeed();
		HorseBreed breed = GetBreed();
		return num * breed.maxSpeed + equipmentSpeedMod;
	}

	public override void OnInventoryFirstCreated(ItemContainer container)
	{
		base.OnInventoryFirstCreated(container);
		SpawnWildSaddle();
	}

	private void SpawnWildSaddle()
	{
		SetSeatCount(1);
	}

	public void SetForSale()
	{
		SetFlag(Flags.Reserved2, b: true);
		SetSeatCount(0);
	}

	public override bool IsStandCollisionClear()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		bool flag = false;
		if (HasSingleSaddle())
		{
			Vis.Colliders<Collider>(((Component)mountPoints[0].mountable.eyePositionOverride).transform.position - ((Component)this).transform.forward * 1f, 2f, list, 2162689, (QueryTriggerInteraction)2);
			flag = list.Count > 0;
		}
		else if (HasDoubleSaddle())
		{
			Vis.Colliders<Collider>(((Component)mountPoints[1].mountable.eyePositionOverride).transform.position - ((Component)this).transform.forward * 1f, 2f, list, 2162689, (QueryTriggerInteraction)2);
			flag = list.Count > 0;
			if (!flag)
			{
				Vis.Colliders<Collider>(((Component)mountPoints[2].mountable.eyePositionOverride).transform.position - ((Component)this).transform.forward * 1f, 2f, list, 2162689, (QueryTriggerInteraction)2);
				flag = list.Count > 0;
			}
		}
		Pool.FreeList<Collider>(ref list);
		return !flag;
	}

	public override bool IsPlayerSeatSwapValid(BasePlayer player, int fromIndex, int toIndex)
	{
		if (!HasSaddle())
		{
			return false;
		}
		if (HasSingleSaddle())
		{
			return false;
		}
		if (HasDoubleSaddle() && toIndex == 0)
		{
			return false;
		}
		return true;
	}

	public override int NumSwappableSeats()
	{
		return mountPoints.Count;
	}

	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (IsForSale() || !MountEligable(player))
		{
			return;
		}
		BaseMountable baseMountable;
		if (HasSingleSaddle())
		{
			baseMountable = mountPoints[0].mountable;
		}
		else
		{
			if (!HasDoubleSaddle())
			{
				return;
			}
			baseMountable = (HasDriver() ? mountPoints[2].mountable : mountPoints[1].mountable);
		}
		if ((Object)(object)baseMountable != (Object)null)
		{
			baseMountable.AttemptMount(player, doMountChecks);
		}
		if (PlayerIsMounted(player))
		{
			PlayerMounted(player, baseMountable);
		}
	}

	public override void SetupCorpse(BaseCorpse corpse)
	{
		base.SetupCorpse(corpse);
		HorseCorpse component = ((Component)corpse).GetComponent<HorseCorpse>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.breedIndex = currentBreed;
		}
		else
		{
			Debug.Log((object)"no horse corpse");
		}
	}

	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		riderProtection.Scale(info.damageTypes);
	}

	public override void OnKilled(HitInfo hitInfo = null)
	{
		TryLeaveHitch();
		base.OnKilled(hitInfo);
	}

	public void SetBreed(int index)
	{
		ApplyBreed(index);
		SendNetworkUpdate();
	}

	public override void LeadingChanged()
	{
		if (!IsLeading())
		{
			TryHitch();
		}
	}

	public override void ServerInit()
	{
		SetBreed(Random.Range(0, breeds.Length));
		baseHorseProtection = baseProtection;
		riderProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		baseProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		baseProtection.Add(baseHorseProtection, 1f);
		base.ServerInit();
		EquipmentUpdate();
	}

	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		((FacepunchBehaviour)this).InvokeRepeating((Action)RecordDistance, distanceRecordingSpacing, distanceRecordingSpacing);
		TryLeaveHitch();
	}

	public override void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		((FacepunchBehaviour)this).CancelInvoke((Action)RecordDistance);
		if (NumMounted() == 0)
		{
			TryHitch();
		}
	}

	public bool IsHitched()
	{
		return (Object)(object)currentHitch != (Object)null;
	}

	public void SetHitch(HitchTrough Hitch)
	{
		currentHitch = Hitch;
		SetFlag(Flags.Reserved3, (Object)(object)currentHitch != (Object)null);
	}

	public override float ReplenishRatio()
	{
		return 1f;
	}

	public override void EatNearbyFood()
	{
		if (Time.time < nextEatTime || (StaminaCoreFraction() >= 1f && base.healthFraction >= 1f))
		{
			return;
		}
		if (IsHitched())
		{
			Item foodItem = currentHitch.GetFoodItem();
			if (foodItem != null && foodItem.amount > 0)
			{
				ItemModConsumable component = ((Component)foodItem.info).GetComponent<ItemModConsumable>();
				if (Object.op_Implicit((Object)(object)component))
				{
					float amount = component.GetIfType(MetabolismAttribute.Type.Calories) * currentHitch.caloriesToDecaySeconds;
					AddDecayDelay(amount);
					ReplenishFromFood(component);
					foodItem.UseItem();
					nextEatTime = Time.time + Random.Range(2f, 3f) + Mathf.InverseLerp(0.5f, 1f, StaminaCoreFraction()) * 4f;
					return;
				}
			}
		}
		base.EatNearbyFood();
	}

	public void TryLeaveHitch()
	{
		if (Object.op_Implicit((Object)(object)currentHitch))
		{
			currentHitch.Unhitch(this);
		}
	}

	public void TryHitch()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		List<HitchTrough> list = Pool.GetList<HitchTrough>();
		Vis.Entities(((Component)this).transform.position, 2.5f, list, 256, (QueryTriggerInteraction)1);
		foreach (HitchTrough item in list)
		{
			if (!(Vector3.Dot(Vector3Ex.Direction2D(((Component)item).transform.position, ((Component)this).transform.position), ((Component)this).transform.forward) < 0.4f) && !item.isClient && item.HasSpace() && item.ValidHitchPosition(((Component)this).transform.position) && item.AttemptToHitch(this))
			{
				break;
			}
		}
		Pool.FreeList<HitchTrough>(ref list);
	}

	public void RecordDistance()
	{
		BasePlayer driver = GetDriver();
		if ((Object)(object)driver == (Object)null)
		{
			tempDistanceTravelled = 0f;
			return;
		}
		kmDistance += tempDistanceTravelled / 1000f;
		if (kmDistance >= 1f)
		{
			driver.stats.Add(distanceStatName + "_km", 1, (Stats)5);
			kmDistance -= 1f;
		}
		driver.stats.Add(distanceStatName, Mathf.FloorToInt(tempDistanceTravelled));
		driver.stats.Save();
		totalDistance += tempDistanceTravelled;
		tempDistanceTravelled = 0f;
	}

	public override void MarkDistanceTravelled(float amount)
	{
		tempDistanceTravelled += amount;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.horse = Pool.Get<Horse>();
		info.msg.horse.staminaSeconds = staminaSeconds;
		info.msg.horse.currentMaxStaminaSeconds = currentMaxStaminaSeconds;
		info.msg.horse.breedIndex = currentBreed;
		info.msg.horse.numStorageSlots = numStorageSlots;
		if (!info.forDisk)
		{
			info.msg.horse.runState = (int)currentRunState;
			info.msg.horse.maxSpeed = GetRunSpeed();
		}
	}

	public override void OnClaimedWithToken(Item tokenItem)
	{
		base.OnClaimedWithToken(tokenItem);
		SetSeatCount(GetSaddleItemSeatCount(tokenItem));
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
	}

	public override void OnInventoryDirty()
	{
		EquipmentUpdate();
	}

	public override bool CanAnimalAcceptItem(Item item, int targetSlot)
	{
		ItemModAnimalEquipment component = ((Component)item.info).GetComponent<ItemModAnimalEquipment>();
		if (IsForSale() && ItemIsSaddle(item))
		{
			return false;
		}
		if (!Object.op_Implicit((Object)(object)component))
		{
			return false;
		}
		if (ItemIsSaddle(item) && HasSaddle())
		{
			return false;
		}
		if (component.slot == ItemModAnimalEquipment.SlotType.Basic)
		{
			return true;
		}
		for (int i = 0; i < equipmentInventory.capacity; i++)
		{
			Item slot = equipmentInventory.GetSlot(i);
			if (slot != null)
			{
				ItemModAnimalEquipment component2 = ((Component)slot.info).GetComponent<ItemModAnimalEquipment>();
				if (!((Object)(object)component2 == (Object)null) && component2.slot == component.slot)
				{
					int slot2 = (int)component2.slot;
					string text = slot2.ToString();
					slot2 = (int)component.slot;
					Debug.Log((object)("rejecting because slot same, found : " + text + " new : " + slot2));
					return false;
				}
			}
		}
		return true;
	}

	public void EquipmentUpdate()
	{
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		SetFlag(Flags.Reserved4, b: false, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved5, b: false, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved6, b: false, recursive: false, networkupdate: false);
		riderProtection.Clear();
		baseProtection.Clear();
		equipmentSpeedMod = 0f;
		numStorageSlots = 0;
		for (int i = 0; i < equipmentInventory.capacity; i++)
		{
			Item slot = equipmentInventory.GetSlot(i);
			if (slot == null)
			{
				continue;
			}
			ItemModAnimalEquipment component = ((Component)slot.info).GetComponent<ItemModAnimalEquipment>();
			if (Object.op_Implicit((Object)(object)component))
			{
				SetFlag(component.WearableFlag, b: true, recursive: false, networkupdate: false);
				if (component.hideHair)
				{
					SetFlag(Flags.Reserved4, b: true);
				}
				if (Object.op_Implicit((Object)(object)component.riderProtection))
				{
					riderProtection.Add(component.riderProtection, 1f);
				}
				if (Object.op_Implicit((Object)(object)component.animalProtection))
				{
					baseProtection.Add(component.animalProtection, 1f);
				}
				equipmentSpeedMod += component.speedModifier;
				numStorageSlots += component.additionalInventorySlots;
			}
		}
		for (int j = 0; j < storageInventory.capacity; j++)
		{
			if (j >= numStorageSlots)
			{
				Item slot2 = storageInventory.GetSlot(j);
				if (slot2 != null)
				{
					slot2.RemoveFromContainer();
					slot2.Drop(((Component)this).transform.position + Vector3.up + Random.insideUnitSphere * 0.25f, Vector3.zero);
				}
			}
		}
		storageInventory.capacity = numStorageSlots;
		SendNetworkUpdate();
	}

	private void SetSeatCount(int count)
	{
		SetFlag(Flags.Reserved9, b: false, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved10, b: false, recursive: false, networkupdate: false);
		switch (count)
		{
		case 1:
			SetFlag(Flags.Reserved9, b: true, recursive: false, networkupdate: false);
			break;
		case 2:
			SetFlag(Flags.Reserved10, b: true, recursive: false, networkupdate: false);
			break;
		}
		UpdateMountFlags();
	}

	public override void DoNetworkUpdate()
	{
		bool num = false || prevStamina != staminaSeconds || prevMaxStamina != currentMaxStaminaSeconds || prevBreed != currentBreed || prevSlots != numStorageSlots || prevRunState != (int)currentRunState || prevMaxSpeed != GetRunSpeed();
		prevStamina = staminaSeconds;
		prevMaxStamina = currentMaxStaminaSeconds;
		prevRunState = (int)currentRunState;
		prevMaxSpeed = GetRunSpeed();
		prevBreed = currentBreed;
		prevSlots = numStorageSlots;
		if (num)
		{
			SendNetworkUpdate();
		}
	}

	public int GetSaddleItemSeatCount(Item item)
	{
		if (!ItemIsSaddle(item))
		{
			return 0;
		}
		ItemModAnimalEquipment component = ((Component)item.info).GetComponent<ItemModAnimalEquipment>();
		if (component.slot == ItemModAnimalEquipment.SlotType.Saddle)
		{
			return 1;
		}
		if (component.slot == ItemModAnimalEquipment.SlotType.SaddleDouble)
		{
			return 2;
		}
		return 0;
	}

	public bool HasSaddle()
	{
		if (!HasSingleSaddle())
		{
			return HasDoubleSaddle();
		}
		return true;
	}

	public bool HasSingleSaddle()
	{
		return HasFlag(Flags.Reserved9);
	}

	public bool HasDoubleSaddle()
	{
		return HasFlag(Flags.Reserved10);
	}

	private bool ItemIsSaddle(Item item)
	{
		if (item == null)
		{
			return false;
		}
		ItemModAnimalEquipment component = ((Component)item.info).GetComponent<ItemModAnimalEquipment>();
		if ((Object)(object)component == (Object)null)
		{
			return false;
		}
		if (component.slot == ItemModAnimalEquipment.SlotType.Saddle || component.slot == ItemModAnimalEquipment.SlotType.SaddleDouble)
		{
			return true;
		}
		return false;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.horse != null)
		{
			staminaSeconds = info.msg.horse.staminaSeconds;
			currentMaxStaminaSeconds = info.msg.horse.currentMaxStaminaSeconds;
			ApplyBreed(info.msg.horse.breedIndex);
		}
	}

	public override bool HasValidSaddle()
	{
		return HasSaddle();
	}

	public override bool HasSeatAvailable()
	{
		if (!HasValidSaddle())
		{
			return false;
		}
		if (HasFlag(Flags.Reserved11))
		{
			return false;
		}
		return true;
	}

	public int GetSeatCapacity()
	{
		if (HasDoubleSaddle())
		{
			return 2;
		}
		if (HasSingleSaddle())
		{
			return 1;
		}
		return 0;
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		return false;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_ReqSwapSaddleType(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && !IsForSale() && HasSaddle() && !AnyMounted())
		{
			int tokenItemID = msg.read.Int32();
			Item item = GetPurchaseToken(player, tokenItemID);
			if (item != null)
			{
				ItemDefinition template = (HasSingleSaddle() ? PurchaseOptions[0].TokenItem : PurchaseOptions[1].TokenItem);
				OnClaimedWithToken(item);
				item.UseItem();
				Item item2 = ItemManager.Create(template, 1, 0uL);
				player.GiveItem(item2);
				SendNetworkUpdateImmediate();
			}
		}
	}

	public override int MaxMounted()
	{
		return GetSeatCapacity();
	}

	[ServerVar]
	public static void setHorseBreed(Arg arg)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if ((Object)(object)basePlayer == (Object)null)
		{
			return;
		}
		int @int = arg.GetInt(0, 0);
		List<RidableHorse> list = Pool.GetList<RidableHorse>();
		Vis.Entities(basePlayer.eyes.position, basePlayer.eyes.position + basePlayer.eyes.HeadForward() * 5f, 0f, list, -1, (QueryTriggerInteraction)2);
		foreach (RidableHorse item in list)
		{
			item.SetBreed(@int);
		}
		Pool.FreeList<RidableHorse>(ref list);
	}
}
