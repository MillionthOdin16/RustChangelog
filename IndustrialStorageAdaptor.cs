using UnityEngine;

public class IndustrialStorageAdaptor : IndustrialEntity, IIndustrialStorage
{
	public GameObject GreenLight;

	public GameObject RedLight;

	private BaseEntity _cachedParent;

	private ItemContainer cachedContainer;

	public BaseEntity cachedParent
	{
		get
		{
			if ((Object)(object)_cachedParent == (Object)null)
			{
				_cachedParent = GetParentEntity();
			}
			return _cachedParent;
		}
	}

	public ItemContainer Container
	{
		get
		{
			if (cachedContainer == null)
			{
				cachedContainer = (cachedParent as StorageContainer)?.inventory;
			}
			return cachedContainer;
		}
	}

	public BaseEntity IndustrialEntity => this;

	public override void ServerInit()
	{
		base.ServerInit();
		_cachedParent = null;
		cachedContainer = null;
	}

	public Vector2i InputSlotRange(int slotIndex)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)cachedParent != (Object)null)
		{
			if (cachedParent is IIndustrialStorage industrialStorage)
			{
				return industrialStorage.InputSlotRange(slotIndex);
			}
			if (cachedParent is Locker locker)
			{
				Vector3 localPosition = ((Component)this).transform.localPosition;
				return locker.GetIndustrialSlotRange(localPosition);
			}
		}
		if (Container != null)
		{
			return new Vector2i(0, Container.capacity - 1);
		}
		return new Vector2i(0, 0);
	}

	public Vector2i OutputSlotRange(int slotIndex)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)cachedParent != (Object)null)
		{
			if (cachedParent is DropBox && Container != null)
			{
				return new Vector2i(0, Container.capacity - 2);
			}
			if (cachedParent is IIndustrialStorage industrialStorage)
			{
				return industrialStorage.OutputSlotRange(slotIndex);
			}
			if (cachedParent is Locker locker)
			{
				Vector3 localPosition = ((Component)this).transform.localPosition;
				return locker.GetIndustrialSlotRange(localPosition);
			}
		}
		if (Container != null)
		{
			return new Vector2i(0, Container.capacity - 1);
		}
		return new Vector2i(0, 0);
	}

	public void OnStorageItemTransferBegin()
	{
		if ((Object)(object)cachedParent != (Object)null)
		{
			if (cachedParent is VendingMachine vendingMachine)
			{
				vendingMachine.OnIndustrialItemTransferBegins();
			}
			else if (cachedParent is Locker locker)
			{
				locker.OnIndustrialItemTransferBegin();
			}
		}
	}

	public void OnStorageItemTransferEnd()
	{
		if ((Object)(object)cachedParent != (Object)null)
		{
			if (cachedParent is VendingMachine vendingMachine)
			{
				vendingMachine.OnIndustrialItemTransferEnds();
			}
			else if (cachedParent is Locker locker)
			{
				locker.OnIndustrialItemTransferEnd();
			}
		}
	}

	public override int ConsumptionAmount()
	{
		return 0;
	}

	public void ClientNotifyItemAddRemoved(bool add)
	{
		if (add)
		{
			GreenLight.SetActive(false);
			GreenLight.SetActive(true);
		}
		else
		{
			RedLight.SetActive(false);
			RedLight.SetActive(true);
		}
	}
}
