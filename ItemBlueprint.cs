using System.Collections.Generic;
using Rust;
using UnityEngine;

public class ItemBlueprint : MonoBehaviour
{
	public List<ItemAmount> ingredients = new List<ItemAmount>();

	public List<ItemDefinition> additionalUnlocks = new List<ItemDefinition>();

	public bool defaultBlueprint = false;

	public bool userCraftable = true;

	public bool isResearchable = true;

	public bool forceShowInConveyorFilter = false;

	public Rarity rarity;

	[Header("Workbench")]
	public int workbenchLevelRequired = 0;

	[Header("Scrap")]
	public int scrapRequired = 0;

	public int scrapFromRecycle = 0;

	[Header("Unlocking")]
	[Tooltip("This item won't show anywhere unless you have the corresponding SteamItem in your inventory - which is defined on the ItemDefinition")]
	public bool NeedsSteamItem = false;

	public int blueprintStackSize = -1;

	public float time = 1f;

	public int amountToCreate = 1;

	public string UnlockAchievment;

	public string RecycleStat;

	public ItemDefinition targetItem => ((Component)this).GetComponent<ItemDefinition>();

	public bool NeedsSteamDLC => (Object)(object)targetItem.steamDlc != (Object)null;
}
