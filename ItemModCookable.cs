using UnityEngine;

public class ItemModCookable : ItemMod
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition becomeOnCooked;

	public float cookTime = 30f;

	public int amountOfBecome = 1;

	public int lowTemp;

	public int highTemp;

	public bool setCookingFlag;

	public void OnValidate()
	{
		if (amountOfBecome < 1)
		{
			amountOfBecome = 1;
		}
		if ((Object)(object)becomeOnCooked == (Object)null)
		{
			Debug.LogWarning((object)("[ItemModCookable] becomeOnCooked is unset! [" + ((Object)this).name + "]"), (Object)(object)((Component)this).gameObject);
		}
	}

	public bool CanBeCookedByAtTemperature(float temperature)
	{
		if (temperature > (float)lowTemp)
		{
			return temperature < (float)highTemp;
		}
		return false;
	}

	private void CycleCooking(Item item, float delta)
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		if (!CanBeCookedByAtTemperature(item.temperature) || item.cookTimeLeft < 0f)
		{
			if (setCookingFlag && item.HasFlag(Item.Flag.Cooking))
			{
				item.SetFlag(Item.Flag.Cooking, b: false);
				item.MarkDirty();
			}
			return;
		}
		if (setCookingFlag && !item.HasFlag(Item.Flag.Cooking))
		{
			item.SetFlag(Item.Flag.Cooking, b: true);
			item.MarkDirty();
		}
		item.cookTimeLeft -= delta;
		if (item.cookTimeLeft > 0f)
		{
			item.MarkDirty();
			return;
		}
		float num = item.cookTimeLeft * -1f;
		int num2 = 1 + Mathf.FloorToInt(num / cookTime);
		item.cookTimeLeft = cookTime - num % cookTime;
		BaseOven baseOven = item.GetEntityOwner() as BaseOven;
		num2 = Mathf.Min(num2, item.amount);
		if (item.amount > num2)
		{
			item.amount -= num2;
			item.MarkDirty();
		}
		else
		{
			item.Remove();
		}
		if (!((Object)(object)becomeOnCooked != (Object)null))
		{
			return;
		}
		Item item2 = ItemManager.Create(becomeOnCooked, amountOfBecome * num2, 0uL);
		if (item2 != null && !item2.MoveToContainer(item.parent) && !item2.MoveToContainer(item.parent))
		{
			item2.Drop(item.parent.dropPosition, item.parent.dropVelocity);
			if (Object.op_Implicit((Object)(object)item.parent.entityOwner) && (Object)(object)baseOven != (Object)null)
			{
				baseOven.OvenFull();
			}
		}
	}

	public override void OnItemCreated(Item itemcreated)
	{
		itemcreated.cookTimeLeft = cookTime;
		itemcreated.onCycle += CycleCooking;
	}
}
