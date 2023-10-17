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
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Expected O, but got Unknown
		for (int i = 0; i < categoryButton.transform.parent.childCount; i++)
		{
			Transform child = categoryButton.transform.parent.GetChild(i);
			if (!((Object)(object)child == (Object)(object)categoryButton.transform))
			{
				GameManager.Destroy(((Component)child).gameObject);
			}
		}
		categoryButton.SetActive(true);
		foreach (IGrouping<ItemCategory, ItemDefinition> item in from x in ItemManager.GetItemDefinitions()
			group x by x.category into x
			orderby x.First().category
			select x)
		{
			GameObject val = Object.Instantiate<GameObject>(categoryButton);
			val.transform.SetParent(categoryButton.transform.parent, false);
			((TMP_Text)val.GetComponentInChildren<TextMeshProUGUI>()).text = item.First().category.ToString();
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
		IOrderedEnumerable<ItemDefinition> obj = (flag ? allItems : currentItems);
		int num = 0;
		foreach (ItemDefinition item in obj)
		{
			if (!item.hidden && (!flag || item.displayName.translated.ToLower().Contains(value)))
			{
				GameObject obj2 = Object.Instantiate<GameObject>(itemButton);
				obj2.transform.SetParent(itemButton.transform.parent, false);
				((TMP_Text)obj2.GetComponentInChildren<TextMeshProUGUI>()).text = item.displayName.translated;
				obj2.GetComponentInChildren<ItemButtonTools>().itemDef = item;
				obj2.GetComponentInChildren<ItemButtonTools>().image.sprite = item.iconSprite;
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
