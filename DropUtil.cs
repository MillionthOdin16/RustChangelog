using ConVar;
using UnityEngine;

public class DropUtil
{
	public static void DropItems(ItemContainer container, Vector3 position)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		if (!Server.dropitems || container == null || container.itemList == null)
		{
			return;
		}
		float num = 0.25f;
		Item[] array = container.itemList.ToArray();
		foreach (Item item in array)
		{
			float num2 = Random.Range(0f, 2f);
			item.RemoveFromContainer();
			BaseEntity baseEntity = item.CreateWorldObject(position + new Vector3(Random.Range(0f - num, num), 1f, Random.Range(0f - num, num)));
			if ((Object)(object)baseEntity == (Object)null)
			{
				item.Remove();
				continue;
			}
			if (baseEntity is DroppedItem droppedItem && container.entityOwner is LootContainer)
			{
				droppedItem.DropReason = DroppedItem.DropReasonEnum.Loot;
			}
			if (num2 > 0f)
			{
				baseEntity.SetVelocity(new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)) * num2);
				baseEntity.SetAngularVelocity(new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)) * num2);
			}
		}
	}
}
