using System.Collections.Generic;
using System.Linq;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemListTools : MonoBehaviour
{
	public GameObject categoryButton;

	public GameObject itemButton;

	public RustInput searchInputText;

	internal Button lastCategory;

	private IOrderedEnumerable<ItemDefinition> currentItems;

	private IOrderedEnumerable<ItemDefinition> allItems;

	public void OnPanelOpened()
	{
		CacheAllItems();
		Refresh();
		searchInputText.InputField.ActivateInputField();
	}

	private void OnOpenDevTools()
	{
		searchInputText.InputField.ActivateInputField();
	}

	private void CacheAllItems()
	{
		if (allItems == null)
		{
			allItems = from x in ItemManager.GetItemDefinitions()
				orderby x.displayName.translated
				select x;
		}
	}

	public void Refresh()
	{
		RebuildCategories();
	}

	private void RebuildCategories()
	{
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		for (int i = 0; i < categoryButton.transform.parent.childCount; i++)
		{
			Transform child = categoryButton.transform.parent.GetChild(i);
			if (!((Object)(object)child == (Object)(object)categoryButton.transform))
			{
				GameManager.Destroy(((Component)child).gameObject);
			}
		}
		categoryButton.SetActive(true);
		IEnumerable<IGrouping<ItemCategory, ItemDefinition>> source = from x in ItemManager.GetItemDefinitions()
			group x by x.category;
		foreach (IGrouping<ItemCategory, ItemDefinition> item in source.OrderBy((IGrouping<ItemCategory, ItemDefinition> x) => x.First().category))
		{
			GameObject val = Object.Instantiate<GameObject>(categoryButton);
			val.transform.SetParent(categoryButton.transform.parent, false);
			TextMeshProUGUI componentInChildren = val.GetComponentInChildren<TextMeshProUGUI>();
			((TMP_Text)componentInChildren).text = item.First().category.ToString();
			Button btn = val.GetComponentInChildren<Button>();
			ItemDefinition[] itemArray = item.ToArray();
			((UnityEvent)btn.onClick).AddListener((UnityAction)delegate
			{
				if (Object.op_Implicit((Object)(object)lastCategory))
				{
					((Selectable)lastCategory).interactable = true;
				}
				lastCategory = btn;
				((Selectable)lastCategory).interactable = false;
				SwitchItemCategory(itemArray);
			});
			if ((Object)(object)lastCategory == (Object)null)
			{
				lastCategory = btn;
				((Selectable)lastCategory).interactable = false;
				SwitchItemCategory(itemArray);
			}
		}
		categoryButton.SetActive(false);
	}

	private void SwitchItemCategory(ItemDefinition[] defs)
	{
		currentItems = defs.OrderBy((ItemDefinition x) => x.displayName.translated);
		searchInputText.Text = "";
		FilterItems(null);
	}

	public void FilterItems(string searchText)
	{
		if ((Object)(object)itemButton == (Object)null)
		{
			return;
		}
		for (int i = 0; i < itemButton.transform.parent.childCount; i++)
		{
			Transform child = itemButton.transform.parent.GetChild(i);
			if (!((Object)(object)child == (Object)(object)itemButton.transform))
			{
				GameManager.Destroy(((Component)child).gameObject);
			}
		}
		itemButton.SetActive(true);
		bool flag = !string.IsNullOrEmpty(searchText);
		string value = (flag ? searchText.ToLower() : null);
		IOrderedEnumerable<ItemDefinition> orderedEnumerable = (flag ? allItems : currentItems);
		int num = 0;
		foreach (ItemDefinition item in orderedEnumerable)
		{
			if (!item.hidden && (!flag || item.displayName.translated.ToLower().Contains(value)))
			{
				GameObject val = Object.Instantiate<GameObject>(itemButton);
				val.transform.SetParent(itemButton.transform.parent, false);
				TextMeshProUGUI componentInChildren = val.GetComponentInChildren<TextMeshProUGUI>();
				((TMP_Text)componentInChildren).text = item.displayName.translated;
				val.GetComponentInChildren<ItemButtonTools>().itemDef = item;
				val.GetComponentInChildren<ItemButtonTools>().image.sprite = item.iconSprite;
				num++;
				if (num >= 160)
				{
					break;
				}
			}
		}
		itemButton.SetActive(false);
	}
}
