using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class Item
{
	[Flags]
	public enum Flag
	{
		None = 0,
		Placeholder = 1,
		IsOn = 2,
		OnFire = 4,
		IsLocked = 8,
		Cooking = 0x10
	}

	private const string DefaultArmourBreakEffectPath = "assets/bundled/prefabs/fx/armor_break.prefab";

	private float _condition;

	private float _maxCondition = 100f;

	public ItemDefinition info;

	public ItemId uid;

	public bool dirty;

	public int amount = 1;

	public int position;

	public float busyTime;

	public float removeTime;

	public float fuel;

	public bool isServer;

	public InstanceData instanceData;

	public ulong skin;

	public string name;

	public string streamerName;

	public string text;

	public float cookTimeLeft;

	public Flag flags;

	public ItemContainer contents;

	public ItemContainer parent;

	private EntityRef worldEnt;

	private EntityRef heldEntity;

	public float condition
	{
		get
		{
			return _condition;
		}
		set
		{
			float num = _condition;
			_condition = Mathf.Clamp(value, 0f, maxCondition);
			if (isServer && Mathf.Ceil(value) != Mathf.Ceil(num))
			{
				MarkDirty();
			}
		}
	}

	public float maxCondition
	{
		get
		{
			return _maxCondition;
		}
		set
		{
			_maxCondition = Mathf.Clamp(value, 0f, info.condition.max);
			if (isServer)
			{
				MarkDirty();
			}
		}
	}

	public float maxConditionNormalized => _maxCondition / info.condition.max;

	public float conditionNormalized
	{
		get
		{
			if (!hasCondition)
			{
				return 1f;
			}
			return condition / maxCondition;
		}
		set
		{
			if (hasCondition)
			{
				condition = value * maxCondition;
			}
		}
	}

	public bool hasCondition
	{
		get
		{
			if ((Object)(object)info != (Object)null && info.condition.enabled)
			{
				return info.condition.max > 0f;
			}
			return false;
		}
	}

	public bool isBroken
	{
		get
		{
			if (hasCondition)
			{
				return condition <= 0f;
			}
			return false;
		}
	}

	public int? ammoCount { get; set; }

	public int despawnMultiplier
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected I4, but got Unknown
			Rarity val = info.despawnRarity;
			if ((int)val == 0)
			{
				val = info.rarity;
			}
			if (!((Object)(object)info != (Object)null))
			{
				return 1;
			}
			return Mathf.Clamp((val - 1) * 4, 1, 100);
		}
	}

	public ItemDefinition blueprintTargetDef
	{
		get
		{
			if (!IsBlueprint())
			{
				return null;
			}
			return ItemManager.FindItemDefinition(blueprintTarget);
		}
	}

	public int blueprintTarget
	{
		get
		{
			if (instanceData == null)
			{
				return 0;
			}
			return instanceData.blueprintTarget;
		}
		set
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Expected O, but got Unknown
			if (instanceData == null)
			{
				instanceData = new InstanceData();
			}
			instanceData.ShouldPool = false;
			instanceData.blueprintTarget = value;
		}
	}

	public int blueprintAmount
	{
		get
		{
			return amount;
		}
		set
		{
			amount = value;
		}
	}

	public Item parentItem
	{
		get
		{
			if (parent == null)
			{
				return null;
			}
			return parent.parent;
		}
	}

	public float temperature
	{
		get
		{
			if (parent != null)
			{
				return parent.GetTemperature(position);
			}
			return 15f;
		}
	}

	public BaseEntity.TraitFlag Traits => info.Traits;

	public event Action<Item> OnDirty;

	public event Action<Item, float> onCycle;

	public void LoseCondition(float amount)
	{
		if (hasCondition && !Debugging.disablecondition)
		{
			float num = condition;
			condition -= amount;
			if (Global.developer > 0)
			{
				Debug.Log((object)(info.shortname + " was damaged by: " + amount + "cond is: " + condition + "/" + maxCondition));
			}
			if (condition <= 0f && condition < num)
			{
				OnBroken();
			}
		}
	}

	public void RepairCondition(float amount)
	{
		if (hasCondition)
		{
			condition += amount;
		}
	}

	public void DoRepair(float maxLossFraction)
	{
		if (hasCondition)
		{
			if (info.condition.maintainMaxCondition)
			{
				maxLossFraction = 0f;
			}
			float num = 1f - condition / maxCondition;
			maxLossFraction = Mathf.Clamp(maxLossFraction, 0f, info.condition.max);
			maxCondition *= 1f - maxLossFraction * num;
			condition = maxCondition;
			BaseEntity baseEntity = GetHeldEntity();
			if ((Object)(object)baseEntity != (Object)null)
			{
				baseEntity.SetFlag(BaseEntity.Flags.Broken, b: false);
			}
			if (Global.developer > 0)
			{
				Debug.Log((object)(info.shortname + " was repaired! new cond is: " + condition + "/" + maxCondition));
			}
		}
	}

	public ItemContainer GetRootContainer()
	{
		ItemContainer itemContainer = parent;
		int num = 0;
		while (itemContainer != null && num <= 8 && itemContainer.parent != null && itemContainer.parent.parent != null)
		{
			itemContainer = itemContainer.parent.parent;
			num++;
		}
		if (num == 8)
		{
			Debug.LogWarning((object)"GetRootContainer failed with 8 iterations");
		}
		return itemContainer;
	}

	public virtual void OnBroken()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		if (!hasCondition)
		{
			return;
		}
		BaseEntity baseEntity = GetHeldEntity();
		if ((Object)(object)baseEntity != (Object)null)
		{
			baseEntity.SetFlag(BaseEntity.Flags.Broken, b: true);
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (Object.op_Implicit((Object)(object)ownerPlayer))
		{
			if (ownerPlayer.GetActiveItem() == this)
			{
				Effect.server.Run("assets/bundled/prefabs/fx/item_break.prefab", ownerPlayer, 0u, Vector3.zero, Vector3.zero);
				ownerPlayer.ChatMessage("Your active item was broken!");
			}
			ItemModWearable itemModWearable = default(ItemModWearable);
			if (((Component)info).TryGetComponent<ItemModWearable>(ref itemModWearable) && ownerPlayer.inventory.containerWear.itemList.Contains(this))
			{
				if (itemModWearable.breakEffect.isValid)
				{
					Effect.server.Run(itemModWearable.breakEffect.resourcePath, ownerPlayer, 0u, Vector3.zero, Vector3.zero);
				}
				else
				{
					Effect.server.Run("assets/bundled/prefabs/fx/armor_break.prefab", ownerPlayer, 0u, Vector3.zero, Vector3.zero);
				}
			}
		}
		if ((!info.condition.repairable && !Object.op_Implicit((Object)(object)((Component)info).GetComponent<ItemModRepair>())) || maxCondition <= 5f)
		{
			Remove();
		}
		else if (parent != null && parent.HasFlag(ItemContainer.Flag.NoBrokenItems))
		{
			ItemContainer rootContainer = GetRootContainer();
			if (rootContainer.HasFlag(ItemContainer.Flag.NoBrokenItems))
			{
				Remove();
			}
			else
			{
				BasePlayer playerOwner = rootContainer.playerOwner;
				if ((Object)(object)playerOwner != (Object)null && !MoveToContainer(playerOwner.inventory.containerMain))
				{
					Drop(((Component)playerOwner).transform.position, playerOwner.eyes.BodyForward() * 1.5f);
				}
			}
		}
		MarkDirty();
	}

	public string GetName(bool? streamerModeOverride = null)
	{
		if (streamerModeOverride.HasValue)
		{
			if (!streamerModeOverride.Value)
			{
				return name;
			}
			return streamerName ?? name;
		}
		return name;
	}

	public bool IsBlueprint()
	{
		return blueprintTarget != 0;
	}

	public bool HasFlag(Flag f)
	{
		return (flags & f) == f;
	}

	public void SetFlag(Flag f, bool b)
	{
		if (b)
		{
			flags |= f;
		}
		else
		{
			flags &= ~f;
		}
	}

	public bool IsOn()
	{
		return HasFlag(Flag.IsOn);
	}

	public bool IsOnFire()
	{
		return HasFlag(Flag.OnFire);
	}

	public bool IsCooking()
	{
		return HasFlag(Flag.Cooking);
	}

	public bool IsLocked()
	{
		if (!HasFlag(Flag.IsLocked))
		{
			if (parent != null)
			{
				return parent.IsLocked();
			}
			return false;
		}
		return true;
	}

	public void MarkDirty()
	{
		OnChanged();
		dirty = true;
		if (parent != null)
		{
			parent.MarkDirty();
		}
		if (this.OnDirty != null)
		{
			this.OnDirty(this);
		}
	}

	public void OnChanged()
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnChanged(this);
		}
		if (contents != null)
		{
			contents.OnChanged();
		}
	}

	public void CollectedForCrafting(BasePlayer crafter)
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].CollectedForCrafting(this, crafter);
		}
	}

	public void ReturnedFromCancelledCraft(BasePlayer crafter)
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].ReturnedFromCancelledCraft(this, crafter);
		}
	}

	public void Initialize(ItemDefinition template)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		uid = new ItemId(Net.sv.TakeUID());
		float num = (maxCondition = info.condition.max);
		condition = num;
		OnItemCreated();
	}

	public void OnItemCreated()
	{
		this.onCycle = null;
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnItemCreated(this);
		}
	}

	public void OnVirginSpawn()
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnVirginItem(this);
		}
	}

	public float GetDespawnDuration()
	{
		if (info.quickDespawn)
		{
			return ConVar.Server.itemdespawn_quick;
		}
		return ConVar.Server.itemdespawn * (float)despawnMultiplier;
	}

	protected void RemoveFromWorld()
	{
		BaseEntity worldEntity = GetWorldEntity();
		if ((Object)(object)worldEntity == (Object)null)
		{
			return;
		}
		SetWorldEntity(null);
		OnRemovedFromWorld();
		if (contents != null)
		{
			contents.OnRemovedFromWorld();
		}
		if (worldEntity.IsValid())
		{
			if (worldEntity is WorldItem worldItem)
			{
				worldItem.RemoveItem();
			}
			worldEntity.Kill();
		}
	}

	public void OnRemovedFromWorld()
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnRemovedFromWorld(this);
		}
	}

	public void RemoveFromContainer()
	{
		if (parent != null)
		{
			SetParent(null);
		}
	}

	public bool DoItemSlotsConflict(Item other)
	{
		return (info.occupySlots & other.info.occupySlots) != 0;
	}

	public void SetParent(ItemContainer target)
	{
		if (target == parent)
		{
			return;
		}
		if (parent != null)
		{
			parent.Remove(this);
			parent = null;
		}
		if (target == null)
		{
			position = 0;
		}
		else
		{
			parent = target;
			if (!parent.Insert(this))
			{
				Remove();
				Debug.LogError((object)"Item.SetParent caused remove - this shouldn't ever happen");
			}
		}
		MarkDirty();
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnParentChanged(this);
		}
	}

	public void OnAttacked(HitInfo hitInfo)
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnAttacked(this, hitInfo);
		}
	}

	public BaseEntity GetEntityOwner()
	{
		return parent?.GetEntityOwner();
	}

	public bool IsChildContainer(ItemContainer c)
	{
		if (contents == null)
		{
			return false;
		}
		if (contents == c)
		{
			return true;
		}
		foreach (Item item in contents.itemList)
		{
			if (item.IsChildContainer(c))
			{
				return true;
			}
		}
		return false;
	}

	public bool CanMoveTo(ItemContainer newcontainer, int iTargetPos = -1)
	{
		if (IsChildContainer(newcontainer))
		{
			return false;
		}
		if (newcontainer.CanAcceptItem(this, iTargetPos) != 0)
		{
			return false;
		}
		if (iTargetPos >= newcontainer.capacity)
		{
			return false;
		}
		if (parent != null && newcontainer == parent && iTargetPos == position)
		{
			return false;
		}
		return true;
	}

	public bool MoveToContainer(ItemContainer newcontainer, int iTargetPos = -1, bool allowStack = true, bool ignoreStackLimit = false, BasePlayer sourcePlayer = null, bool allowSwap = true)
	{
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("MoveToContainer", 0);
		try
		{
			bool flag = iTargetPos == -1;
			ItemContainer itemContainer = parent;
			if (iTargetPos == -1)
			{
				if (allowStack && info.stackable > 1)
				{
					foreach (Item item2 in from x in newcontainer.FindItemsByItemID(info.itemid)
						orderby x.position
						select x)
					{
						if (item2.CanStack(this) && (ignoreStackLimit || item2.amount < item2.MaxStackable()))
						{
							iTargetPos = item2.position;
						}
					}
				}
				if (iTargetPos == -1 && newcontainer.GetEntityOwner(returnHeldEntity: true) is IItemContainerEntity itemContainerEntity)
				{
					iTargetPos = itemContainerEntity.GetIdealSlot(sourcePlayer, this);
					if (iTargetPos == int.MinValue)
					{
						return false;
					}
				}
				if (iTargetPos == -1)
				{
					if (newcontainer == parent)
					{
						return false;
					}
					bool flag2 = newcontainer.HasFlag(ItemContainer.Flag.Clothing) && info.isWearable;
					ItemModWearable itemModWearable = info.ItemModWearable;
					for (int i = 0; i < newcontainer.capacity; i++)
					{
						Item slot = newcontainer.GetSlot(i);
						if (slot == null)
						{
							if (CanMoveTo(newcontainer, i))
							{
								iTargetPos = i;
								break;
							}
							continue;
						}
						if (flag2 && slot != null && !slot.info.ItemModWearable.CanExistWith(itemModWearable))
						{
							iTargetPos = i;
							break;
						}
						if (newcontainer.availableSlots != null && newcontainer.availableSlots.Count > 0 && DoItemSlotsConflict(slot))
						{
							iTargetPos = i;
							break;
						}
					}
					if (flag2 && iTargetPos == -1)
					{
						iTargetPos = newcontainer.capacity - 1;
					}
				}
			}
			if (iTargetPos == -1)
			{
				return false;
			}
			if (!CanMoveTo(newcontainer, iTargetPos))
			{
				return false;
			}
			if (iTargetPos >= 0 && newcontainer.SlotTaken(this, iTargetPos))
			{
				Item slot2 = newcontainer.GetSlot(iTargetPos);
				if (slot2 == this)
				{
					return false;
				}
				if (allowStack && slot2 != null)
				{
					int num = slot2.MaxStackable();
					if (slot2.CanStack(this))
					{
						if (ignoreStackLimit)
						{
							num = int.MaxValue;
						}
						if (slot2.amount >= num)
						{
							return false;
						}
						int num2 = Mathf.Min(num - slot2.amount, amount);
						slot2.amount += num2;
						amount -= num2;
						slot2.MarkDirty();
						MarkDirty();
						if (amount <= 0)
						{
							RemoveFromWorld();
							RemoveFromContainer();
							Remove();
							return true;
						}
						if (flag)
						{
							return MoveToContainer(newcontainer, -1, allowStack, ignoreStackLimit, sourcePlayer);
						}
						return false;
					}
				}
				if (parent != null && allowSwap && slot2 != null)
				{
					ItemContainer itemContainer2 = parent;
					int iTargetPos2 = position;
					ItemContainer itemContainer3 = slot2.parent;
					int num3 = slot2.position;
					if (!slot2.CanMoveTo(itemContainer2, iTargetPos2))
					{
						return false;
					}
					BaseEntity entityOwner = GetEntityOwner();
					BaseEntity entityOwner2 = slot2.GetEntityOwner();
					RemoveFromContainer();
					slot2.RemoveFromContainer();
					RemoveConflictingSlots(newcontainer, entityOwner, sourcePlayer);
					slot2.RemoveConflictingSlots(itemContainer2, entityOwner2, sourcePlayer);
					if (!slot2.MoveToContainer(itemContainer2, iTargetPos2, allowStack: true, ignoreStackLimit: false, sourcePlayer) || !MoveToContainer(newcontainer, iTargetPos, allowStack: true, ignoreStackLimit: false, sourcePlayer))
					{
						RemoveFromContainer();
						slot2.RemoveFromContainer();
						SetParent(itemContainer2);
						position = iTargetPos2;
						slot2.SetParent(itemContainer3);
						slot2.position = num3;
						return true;
					}
					return true;
				}
				return false;
			}
			if (parent == newcontainer)
			{
				if (iTargetPos >= 0 && iTargetPos != position && !parent.SlotTaken(this, iTargetPos))
				{
					position = iTargetPos;
					MarkDirty();
					return true;
				}
				return false;
			}
			if (newcontainer.maxStackSize > 0 && newcontainer.maxStackSize < amount)
			{
				Item item = SplitItem(newcontainer.maxStackSize);
				if (item != null && !item.MoveToContainer(newcontainer, iTargetPos, allowStack: false, ignoreStackLimit: false, sourcePlayer) && (itemContainer == null || !item.MoveToContainer(itemContainer, -1, allowStack: true, ignoreStackLimit: false, sourcePlayer)))
				{
					item.Drop(newcontainer.dropPosition, newcontainer.dropVelocity);
				}
				return true;
			}
			if (!newcontainer.CanAccept(this))
			{
				return false;
			}
			BaseEntity entityOwner3 = GetEntityOwner();
			RemoveFromContainer();
			RemoveFromWorld();
			RemoveConflictingSlots(newcontainer, entityOwner3, sourcePlayer);
			position = iTargetPos;
			SetParent(newcontainer);
			return true;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void RemoveConflictingSlots(ItemContainer container, BaseEntity entityOwner, BasePlayer sourcePlayer)
	{
		if (!isServer || container.availableSlots == null || container.availableSlots.Count <= 0)
		{
			return;
		}
		List<Item> list = Pool.GetList<Item>();
		list.AddRange(container.itemList);
		foreach (Item item in list)
		{
			if (item.DoItemSlotsConflict(this))
			{
				item.RemoveFromContainer();
				if (entityOwner is BasePlayer basePlayer)
				{
					basePlayer.GiveItem(item);
				}
				else if (entityOwner is IItemContainerEntity itemContainerEntity)
				{
					item.MoveToContainer(itemContainerEntity.inventory, -1, allowStack: true, ignoreStackLimit: false, sourcePlayer);
				}
			}
		}
		Pool.FreeList<Item>(ref list);
	}

	public BaseEntity CreateWorldObject(Vector3 pos, Quaternion rotation = default(Quaternion), BaseEntity parentEnt = null, uint parentBone = 0u)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity worldEntity = GetWorldEntity();
		if ((Object)(object)worldEntity != (Object)null)
		{
			return worldEntity;
		}
		worldEntity = GameManager.server.CreateEntity("assets/prefabs/misc/burlap sack/generic_world.prefab", pos, rotation);
		if ((Object)(object)worldEntity == (Object)null)
		{
			Debug.LogWarning((object)"Couldn't create world object for prefab: items/generic_world");
			return null;
		}
		WorldItem worldItem = worldEntity as WorldItem;
		if ((Object)(object)worldItem != (Object)null)
		{
			worldItem.InitializeItem(this);
		}
		if ((Object)(object)parentEnt != (Object)null)
		{
			worldEntity.SetParent(parentEnt, parentBone);
		}
		worldEntity.Spawn();
		SetWorldEntity(worldEntity);
		return GetWorldEntity();
	}

	public BaseEntity Drop(Vector3 vPos, Vector3 vVelocity, Quaternion rotation = default(Quaternion))
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		RemoveFromWorld();
		BaseEntity baseEntity = null;
		if (vPos != Vector3.zero && !info.HasFlag(ItemDefinition.Flag.NoDropping))
		{
			baseEntity = CreateWorldObject(vPos, rotation);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.SetVelocity(vVelocity);
			}
		}
		else
		{
			Remove();
		}
		RemoveFromContainer();
		return baseEntity;
	}

	public BaseEntity DropAndTossUpwards(Vector3 vPos, float force = 2f)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		float num = Random.value * (float)Math.PI * 2f;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Sin(num), 1f, Mathf.Cos(num));
		return Drop(vPos + Vector3.up * 0.1f, val * force);
	}

	public bool IsBusy()
	{
		if (busyTime > Time.time)
		{
			return true;
		}
		return false;
	}

	public void BusyFor(float fTime)
	{
		busyTime = Time.time + fTime;
	}

	public void Remove(float fTime = 0f)
	{
		if (removeTime > 0f)
		{
			return;
		}
		if (isServer)
		{
			ItemMod[] itemMods = info.itemMods;
			for (int i = 0; i < itemMods.Length; i++)
			{
				itemMods[i].OnRemove(this);
			}
		}
		this.onCycle = null;
		removeTime = Time.time + fTime;
		this.OnDirty = null;
		position = -1;
		if (isServer)
		{
			ItemManager.RemoveItem(this, fTime);
		}
	}

	internal void DoRemove()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		this.OnDirty = null;
		this.onCycle = null;
		if (isServer && ((ItemId)(ref uid)).IsValid && Net.sv != null)
		{
			Net.sv.ReturnUID(uid.Value);
			uid = default(ItemId);
		}
		if (contents != null)
		{
			contents.Kill();
			contents = null;
		}
		if (isServer)
		{
			RemoveFromWorld();
			RemoveFromContainer();
		}
		BaseEntity baseEntity = GetHeldEntity();
		if (baseEntity.IsValid())
		{
			Debug.LogWarning((object)("Item's Held Entity not removed!" + info.displayName.english + " -> " + (object)baseEntity), (Object)(object)baseEntity);
		}
	}

	public void SwitchOnOff(bool bNewState)
	{
		if (HasFlag(Flag.IsOn) != bNewState)
		{
			SetFlag(Flag.IsOn, bNewState);
			MarkDirty();
		}
	}

	public void LockUnlock(bool bNewState)
	{
		if (HasFlag(Flag.IsLocked) != bNewState)
		{
			SetFlag(Flag.IsLocked, bNewState);
			MarkDirty();
		}
	}

	public BasePlayer GetOwnerPlayer()
	{
		if (parent == null)
		{
			return null;
		}
		return parent.GetOwnerPlayer();
	}

	public bool IsBackpack()
	{
		if ((Object)(object)info != (Object)null)
		{
			return (info.flags & ItemDefinition.Flag.Backpack) != 0;
		}
		return false;
	}

	public Item SplitItem(int split_Amount)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		Assert.IsTrue(split_Amount > 0, "split_Amount <= 0");
		if (split_Amount <= 0)
		{
			return null;
		}
		if (split_Amount >= amount)
		{
			return null;
		}
		amount -= split_Amount;
		Item item = ItemManager.CreateByItemID(info.itemid, 1, 0uL);
		item.amount = split_Amount;
		item.skin = skin;
		if (IsBlueprint())
		{
			item.blueprintTarget = blueprintTarget;
		}
		if (info.amountType == ItemDefinition.AmountType.Genetics && instanceData != null && instanceData.dataInt != 0)
		{
			item.instanceData = new InstanceData();
			item.instanceData.dataInt = instanceData.dataInt;
			item.instanceData.ShouldPool = false;
		}
		if (instanceData != null && instanceData.dataInt > 0 && (Object)(object)info != (Object)null && (Object)(object)info.Blueprint != (Object)null && info.Blueprint.workbenchLevelRequired == 3)
		{
			item.instanceData = new InstanceData();
			item.instanceData.dataInt = instanceData.dataInt;
			item.instanceData.ShouldPool = false;
			item.SetFlag(Flag.IsOn, IsOn());
		}
		MarkDirty();
		return item;
	}

	public bool CanBeHeld()
	{
		if (isBroken)
		{
			return false;
		}
		return true;
	}

	public bool CanStack(Item item)
	{
		if (item == this)
		{
			return false;
		}
		if (MaxStackable() <= 1)
		{
			return false;
		}
		if (item.MaxStackable() <= 1)
		{
			return false;
		}
		if (item.info.itemid != info.itemid)
		{
			return false;
		}
		if (hasCondition && condition != item.info.condition.max)
		{
			return false;
		}
		if (item.hasCondition && item.condition != item.info.condition.max)
		{
			return false;
		}
		if (!IsValid())
		{
			return false;
		}
		if (IsBlueprint() && blueprintTarget != item.blueprintTarget)
		{
			return false;
		}
		if (item.skin != skin)
		{
			return false;
		}
		if (item.info.amountType == ItemDefinition.AmountType.Genetics || info.amountType == ItemDefinition.AmountType.Genetics)
		{
			int num = ((item.instanceData != null) ? item.instanceData.dataInt : (-1));
			int num2 = ((instanceData != null) ? instanceData.dataInt : (-1));
			if (num != num2)
			{
				return false;
			}
		}
		if (item.instanceData != null && instanceData != null && (item.IsOn() != IsOn() || (item.instanceData.dataInt != instanceData.dataInt && (Object)(object)item.info.Blueprint != (Object)null && item.info.Blueprint.workbenchLevelRequired == 3)))
		{
			return false;
		}
		if (instanceData != null && ((NetworkableId)(ref instanceData.subEntity)).IsValid && Object.op_Implicit((Object)(object)((Component)info).GetComponent<ItemModSign>()))
		{
			return false;
		}
		if (item.instanceData != null && ((NetworkableId)(ref item.instanceData.subEntity)).IsValid && Object.op_Implicit((Object)(object)((Component)item.info).GetComponent<ItemModSign>()))
		{
			return false;
		}
		return true;
	}

	public bool IsValid()
	{
		if (removeTime > 0f)
		{
			return false;
		}
		return true;
	}

	public void SetWorldEntity(BaseEntity ent)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.IsValid())
		{
			worldEnt.Set(null);
			MarkDirty();
		}
		else if (!(worldEnt.uid == ent.net.ID))
		{
			worldEnt.Set(ent);
			MarkDirty();
			OnMovedToWorld();
			if (contents != null)
			{
				contents.OnMovedToWorld();
			}
		}
	}

	public void OnMovedToWorld()
	{
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].OnMovedToWorld(this);
		}
	}

	public BaseEntity GetWorldEntity()
	{
		return worldEnt.Get(isServer);
	}

	public void SetHeldEntity(BaseEntity ent)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.IsValid())
		{
			this.heldEntity.Set(null);
			MarkDirty();
		}
		else
		{
			if (this.heldEntity.uid == ent.net.ID)
			{
				return;
			}
			this.heldEntity.Set(ent);
			MarkDirty();
			if (ent.IsValid())
			{
				HeldEntity heldEntity = ent as HeldEntity;
				if ((Object)(object)heldEntity != (Object)null)
				{
					heldEntity.SetupHeldEntity(this);
				}
			}
		}
	}

	public BaseEntity GetHeldEntity()
	{
		return heldEntity.Get(isServer);
	}

	public void OnCycle(float delta)
	{
		if (this.onCycle != null)
		{
			this.onCycle(this, delta);
		}
	}

	public void ServerCommand(string command, BasePlayer player)
	{
		HeldEntity heldEntity = GetHeldEntity() as HeldEntity;
		if ((Object)(object)heldEntity != (Object)null)
		{
			heldEntity.ServerCommand(this, command, player);
		}
		ItemMod[] itemMods = info.itemMods;
		for (int i = 0; i < itemMods.Length; i++)
		{
			itemMods[i].ServerCommand(this, command, player);
		}
	}

	public void UseItem(int amountToConsume = 1)
	{
		if (amountToConsume > 0)
		{
			amount -= amountToConsume;
			if (amount <= 0)
			{
				amount = 0;
				Remove();
			}
			else
			{
				MarkDirty();
			}
		}
	}

	public bool HasAmmo(AmmoTypes ammoType)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		ItemModProjectile itemModProjectile = default(ItemModProjectile);
		if (((Component)info).TryGetComponent<ItemModProjectile>(ref itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			return true;
		}
		if (contents != null)
		{
			return contents.HasAmmo(ammoType);
		}
		return false;
	}

	public void FindAmmo(List<Item> list, AmmoTypes ammoType)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		ItemModProjectile itemModProjectile = default(ItemModProjectile);
		if (((Component)info).TryGetComponent<ItemModProjectile>(ref itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			list.Add(this);
		}
		else if (contents != null)
		{
			contents.FindAmmo(list, ammoType);
		}
	}

	public int GetAmmoAmount(AmmoTypes ammoType)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		ItemModProjectile itemModProjectile = default(ItemModProjectile);
		if (((Component)info).TryGetComponent<ItemModProjectile>(ref itemModProjectile) && itemModProjectile.IsAmmo(ammoType))
		{
			num += amount;
		}
		if (contents != null)
		{
			num += contents.GetAmmoAmount(ammoType);
		}
		return num;
	}

	public override string ToString()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[6]
		{
			"Item.",
			info.shortname,
			"x",
			amount.ToString(),
			".",
			null
		};
		ItemId val = uid;
		obj[5] = ((object)(ItemId)(ref val)).ToString();
		return string.Concat(obj);
	}

	public Item FindItem(ItemId iUID)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (uid == iUID)
		{
			return this;
		}
		if (contents == null)
		{
			return null;
		}
		return contents.FindItemByUID(iUID);
	}

	public int MaxStackable()
	{
		int num = info.stackable;
		if (parent != null && parent.maxStackSize > 0)
		{
			num = Mathf.Min(parent.maxStackSize, num);
		}
		return num;
	}

	public GameObjectRef GetWorldModel()
	{
		return info.GetWorldModel(amount);
	}

	public virtual Item Save(bool bIncludeContainer = false, bool bIncludeOwners = true)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		dirty = false;
		Item val = Pool.Get<Item>();
		val.UID = uid;
		val.itemid = info.itemid;
		val.slot = position;
		val.amount = amount;
		val.flags = (int)flags;
		val.removetime = removeTime;
		val.locktime = busyTime;
		val.instanceData = instanceData;
		val.worldEntity = worldEnt.uid;
		val.heldEntity = heldEntity.uid;
		val.skinid = skin;
		val.name = name;
		val.streamerName = streamerName;
		val.text = text;
		val.cooktime = cookTimeLeft;
		val.ammoCount = 0;
		NetworkableId val2 = heldEntity.uid;
		if (((NetworkableId)(ref val2)).IsValid)
		{
			BaseProjectile baseProjectile = GetHeldEntity() as BaseProjectile;
			if ((Object)(object)baseProjectile != (Object)null)
			{
				val.ammoCount = baseProjectile.primaryMagazine.contents + 1;
			}
		}
		if (hasCondition)
		{
			val.conditionData = Pool.Get<ConditionData>();
			val.conditionData.maxCondition = _maxCondition;
			val.conditionData.condition = _condition;
		}
		if (contents != null && bIncludeContainer)
		{
			val.contents = contents.Save();
		}
		return val;
	}

	public virtual void Load(Item load)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)info == (Object)null || info.itemid != load.itemid)
		{
			info = ItemManager.FindItemDefinition(load.itemid);
		}
		uid = load.UID;
		name = load.name;
		streamerName = load.streamerName;
		text = load.text;
		cookTimeLeft = load.cooktime;
		amount = load.amount;
		position = load.slot;
		busyTime = load.locktime;
		removeTime = load.removetime;
		flags = (Flag)load.flags;
		worldEnt.uid = load.worldEntity;
		heldEntity.uid = load.heldEntity;
		if (load.ammoCount == 0)
		{
			ammoCount = null;
		}
		else
		{
			ammoCount = load.ammoCount - 1;
		}
		if (isServer)
		{
			Net.sv.RegisterUID(uid.Value);
		}
		if (instanceData != null)
		{
			instanceData.ShouldPool = true;
			instanceData.ResetToPool();
			instanceData = null;
		}
		instanceData = load.instanceData;
		if (instanceData != null)
		{
			instanceData.ShouldPool = false;
		}
		skin = load.skinid;
		if ((Object)(object)info == (Object)null || info.itemid != load.itemid)
		{
			info = ItemManager.FindItemDefinition(load.itemid);
		}
		if ((Object)(object)info == (Object)null)
		{
			return;
		}
		_condition = 0f;
		_maxCondition = 0f;
		if (load.conditionData != null)
		{
			_condition = load.conditionData.condition;
			_maxCondition = load.conditionData.maxCondition;
		}
		else if (info.condition.enabled)
		{
			_condition = info.condition.max;
			_maxCondition = info.condition.max;
		}
		if (load.contents != null)
		{
			if (contents == null)
			{
				contents = new ItemContainer();
				if (isServer)
				{
					contents.ServerInitialize(this, load.contents.slots);
				}
			}
			contents.Load(load.contents);
		}
		if (isServer)
		{
			removeTime = 0f;
			OnItemCreated();
		}
	}
}
