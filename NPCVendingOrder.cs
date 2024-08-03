using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/NPC Vending Order")]
public class NPCVendingOrder : ScriptableObject
{
	[Serializable]
	public class Entry
	{
		public ItemDefinition sellItem;

		public int sellItemAmount;

		public bool sellItemAsBP;

		public ItemDefinition currencyItem;

		public int currencyAmount;

		public bool currencyAsBP;

		public int refillAmount = 1;

		public float refillDelay = 10f;

		public EntryRandom randomDetails;
	}

	[Serializable]
	public struct EntryRandom
	{
		public bool useRandom;

		[Tooltip("The higher this number, the more likely this will be chosen")]
		[Range(0f, 1f)]
		public float weight;

		[Tooltip("Minimum price for the currency item")]
		public int minPrice;

		[Tooltip("Maximum price for the currency item")]
		public int maxPrice;

		[Tooltip("Chance for a very low price to occur (0 to 1)")]
		[Range(0f, 1f)]
		public float veryLowPriceChance;

		[Tooltip("Minimum very low price")]
		public int veryLowPriceMin;

		[Tooltip("Maximum very low price")]
		public int veryLowPriceMax;

		public int GetRandomPrice()
		{
			int num = ((!(Random.value < veryLowPriceChance)) ? Random.Range(minPrice, maxPrice + 1) : Random.Range(veryLowPriceMin, veryLowPriceMax + 1));
			return Mathf.RoundToInt(((float)num + 2.5f) / 5f) * 5;
		}
	}

	public Entry[] orders;

	public List<Entry> GetRandomEntries(int count)
	{
		if (orders == null || orders.Length == 0 || count <= 0)
		{
			return null;
		}
		List<Entry> list = Pool.GetList<Entry>();
		bool[] array = new bool[orders.Length];
		float num = 0f;
		Entry[] array2 = orders;
		foreach (Entry entry in array2)
		{
			num += entry.randomDetails.weight;
		}
		int num2 = Mathf.Min(count, orders.Length);
		for (int j = 0; j < num2; j++)
		{
			if (num == 0f)
			{
				break;
			}
			float num3 = Random.Range(0f, num);
			for (int k = 0; k < orders.Length; k++)
			{
				if (!array[k])
				{
					if (num3 < orders[k].randomDetails.weight)
					{
						list.Add(orders[k]);
						array[k] = true;
						num -= orders[k].randomDetails.weight;
						break;
					}
					num3 -= orders[k].randomDetails.weight;
				}
			}
		}
		return list;
	}
}
