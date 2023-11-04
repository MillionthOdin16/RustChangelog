using System;
using UnityEngine;

public class RandomItemDispenser : PrefabAttribute, IServerComponent
{
	[Serializable]
	public struct RandomItemChance
	{
		public ItemDefinition Item;

		public int Amount;

		[Range(0f, 1f)]
		public float Chance;
	}

	public RandomItemChance[] Chances;

	public bool OnlyAwardOne = true;

	protected override Type GetIndexedType()
	{
		return typeof(RandomItemDispenser);
	}

	public void DistributeItems(BasePlayer forPlayer, Vector3 distributorPosition)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		RandomItemChance[] chances = Chances;
		foreach (RandomItemChance itemChance in chances)
		{
			bool flag = TryAward(itemChance, forPlayer, distributorPosition);
			if (OnlyAwardOne && flag)
			{
				break;
			}
		}
	}

	private bool TryAward(RandomItemChance itemChance, BasePlayer forPlayer, Vector3 distributorPosition)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		float num = Random.Range(0f, 1f);
		if (itemChance.Chance >= num)
		{
			Item item = ItemManager.Create(itemChance.Item, itemChance.Amount, 0uL);
			if (item != null)
			{
				if (Object.op_Implicit((Object)(object)forPlayer))
				{
					forPlayer.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
				}
				else
				{
					item.Drop(distributorPosition + Vector3.up * 0.5f, Vector3.up);
				}
			}
			return true;
		}
		return false;
	}
}
