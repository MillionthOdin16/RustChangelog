using UnityEngine;

public class ItemInformationPanel : MonoBehaviour
{
	public bool ForceHidden(ItemDefinition info)
	{
		if ((Object)(object)info == (Object)null)
		{
			return false;
		}
		return (Object)(object)((Component)info).GetComponent<ItemModHideInfoPanel>() != (Object)null;
	}

	public virtual bool EligableForDisplay(ItemDefinition info)
	{
		Debug.LogWarning((object)"ItemInformationPanel.EligableForDisplay");
		return false;
	}

	public virtual void SetupForItem(ItemDefinition info, Item item = null)
	{
		Debug.LogWarning((object)"ItemInformationPanel.SetupForItem");
	}
}
