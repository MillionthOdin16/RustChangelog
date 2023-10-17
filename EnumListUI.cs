using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnumListUI : MonoBehaviour
{
	public Transform PrefabItem;

	public Transform Container;

	private Action<object> clickedAction;

	private CanvasScaler canvasScaler;

	private void Awake()
	{
		Hide();
	}

	public void Show(List<object> values, Action<object> clicked)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		((Component)this).gameObject.SetActive(true);
		clickedAction = clicked;
		foreach (Transform item in Container)
		{
			Transform val = item;
			Object.Destroy((Object)(object)((Component)val).gameObject);
		}
		foreach (object value in values)
		{
			Transform val2 = Object.Instantiate<Transform>(PrefabItem);
			val2.SetParent(Container, false);
			((Component)val2).GetComponent<EnumListItemUI>().Init(value, value.ToString(), this);
		}
	}

	public void ItemClicked(object value)
	{
		clickedAction?.Invoke(value);
		Hide();
	}

	public void Hide()
	{
		((Component)this).gameObject.SetActive(false);
	}
}
