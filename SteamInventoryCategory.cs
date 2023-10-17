using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Generic Steam Inventory Category")]
public class SteamInventoryCategory : ScriptableObject
{
	public enum Price
	{
		CannotBuy,
		VLV25,
		VLV50,
		VLV75,
		VLV100,
		VLV150,
		VLV200,
		VLV250,
		VLV300,
		VLV350,
		VLV400,
		VLV450,
		VLV500,
		VLV550,
		VLV600,
		VLV650,
		VLV700,
		VLV750,
		VLV800,
		VLV850,
		VLV900,
		VLV950,
		VLV1000,
		VLV1100,
		VLV1200,
		VLV1300,
		VLV1400,
		VLV1500,
		VLV1600,
		VLV1700,
		VLV1800,
		VLV1900,
		VLV2000,
		VLV2500,
		VLV3000,
		VLV3500,
		VLV4000,
		VLV4500,
		VLV5000,
		VLV6000,
		VLV7000,
		VLV8000,
		VLV9000,
		VLV10000
	}

	public enum DropChance
	{
		NeverDrop,
		VeryRare,
		Rare,
		Common,
		VeryCommon,
		ExtremelyRare
	}

	[Header("Steam Inventory")]
	public bool canBeSoldToOtherUsers;

	public bool canBeTradedWithOtherUsers;

	public bool isCommodity;

	public Price price;

	public DropChance dropChance;

	public bool CanBeInCrates = true;
}
