using System;
using Rust.UI;
using TMPro;
using UnityEngine;

public class ItemStoreItem : MonoBehaviour
{
	public HttpImage Icon;

	public RustText Name;

	public TextMeshProUGUI Price;

	public RustText ItemName;

	public GameObject InCartTag;

	private IPlayerItemDefinition item;

	internal void Init(IPlayerItemDefinition item, bool inCart)
	{
		this.item = item;
		Icon.Load(item.IconUrl);
		Name.SetText(item.Name);
		((TMP_Text)Price).text = item.LocalPriceFormatted;
		InCartTag.SetActive(inCart);
		if (!string.IsNullOrWhiteSpace(item.ItemShortName))
		{
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(item.ItemShortName);
			if ((Object)(object)itemDefinition != (Object)null && !string.Equals(itemDefinition.displayName.english, item.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				ItemName.SetPhrase(itemDefinition.displayName);
			}
			else
			{
				ItemName.SetText("");
			}
		}
		else
		{
			ItemName.SetText("");
		}
	}
}
