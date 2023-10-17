using System;
using System.Collections.Generic;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TweakUIDropdown : TweakUIBase
{
	[Serializable]
	public class NameValue
	{
		public string value;

		public Color imageColor;

		public Phrase label;
	}

	public RustText Current;

	public Image BackgroundImage;

	public RustButton Opener;

	public RectTransform Dropdown;

	public RectTransform DropdownContainer;

	public GameObject DropdownItemPrefab;

	public NameValue[] nameValues;

	public bool assignImageColor;

	public UnityEvent onValueChanged = new UnityEvent();

	public int currentValue;

	protected override void Init()
	{
		base.Init();
		DropdownItemPrefab.SetActive(false);
		UpdateDropdownOptions();
		Opener.SetToggleFalse();
		ResetToConvar();
	}

	protected void OnEnable()
	{
		ResetToConvar();
	}

	public void UpdateDropdownOptions()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		List<RustButton> list = Pool.GetList<RustButton>();
		((Component)DropdownContainer).GetComponentsInChildren<RustButton>(false, list);
		foreach (RustButton item in list)
		{
			Object.Destroy((Object)(object)((Component)item).gameObject);
		}
		Pool.FreeList<RustButton>(ref list);
		for (int i = 0; i < nameValues.Length; i++)
		{
			GameObject obj = Object.Instantiate<GameObject>(DropdownItemPrefab, (Transform)(object)DropdownContainer);
			int itemIndex = i;
			RustButton component = obj.GetComponent<RustButton>();
			component.Text.SetPhrase(nameValues[i].label);
			component.OnPressed.AddListener((UnityAction)delegate
			{
				ChangeValue(itemIndex);
			});
			obj.SetActive(true);
		}
	}

	public void OnValueChanged()
	{
		if (ApplyImmediatelyOnChange)
		{
			SetConvarValue();
		}
	}

	public void OnDropdownOpen()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		RectTransform val = (RectTransform)((Component)this).transform;
		if (((Transform)val).position.y <= (float)Screen.height / 2f)
		{
			Dropdown.pivot = new Vector2(0.5f, 0f);
			Dropdown.anchoredPosition = Vector2Ex.WithY(Dropdown.anchoredPosition, 0f);
			return;
		}
		Dropdown.pivot = new Vector2(0.5f, 1f);
		RectTransform dropdown = Dropdown;
		Vector2 anchoredPosition = Dropdown.anchoredPosition;
		Rect rect = val.rect;
		dropdown.anchoredPosition = Vector2Ex.WithY(anchoredPosition, 0f - ((Rect)(ref rect)).height);
	}

	public void ChangeValue(int index)
	{
		Opener.SetToggleFalse();
		int num = Mathf.Clamp(index, 0, nameValues.Length - 1);
		bool num2 = num != currentValue;
		currentValue = num;
		if (ApplyImmediatelyOnChange)
		{
			SetConvarValue();
		}
		else
		{
			ShowValue(nameValues[currentValue].value);
		}
		if (num2)
		{
			UnityEvent obj = onValueChanged;
			if (obj != null)
			{
				obj.Invoke();
			}
		}
	}

	protected override void SetConvarValue()
	{
		base.SetConvarValue();
		NameValue nameValue = nameValues[currentValue];
		if (conVar != null && !(conVar.String == nameValue.value))
		{
			conVar.Set(nameValue.value);
		}
	}

	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (conVar != null)
		{
			string @string = conVar.String;
			ShowValue(@string);
		}
	}

	private void ShowValue(string value)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < nameValues.Length; i++)
		{
			if (!(nameValues[i].value != value))
			{
				Current.SetPhrase(nameValues[i].label);
				currentValue = i;
				if (assignImageColor)
				{
					((Graphic)BackgroundImage).color = nameValues[i].imageColor;
				}
				break;
			}
		}
	}
}
