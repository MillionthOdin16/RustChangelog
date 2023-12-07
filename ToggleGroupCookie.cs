using System.Linq;
using Rust;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ToggleGroupCookie : MonoBehaviour
{
	public ToggleGroup group => ((Component)this).GetComponent<ToggleGroup>();

	private void OnEnable()
	{
		string @string = PlayerPrefs.GetString("ToggleGroupCookie_" + ((Object)this).name);
		if (!string.IsNullOrEmpty(@string))
		{
			Transform val = FindChild(((Component)this).transform, @string);
			if (Object.op_Implicit((Object)(object)val))
			{
				Toggle component = ((Component)val).GetComponent<Toggle>();
				if (Object.op_Implicit((Object)(object)component))
				{
					Toggle[] componentsInChildren = ((Component)this).GetComponentsInChildren<Toggle>(true);
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].isOn = false;
					}
					component.isOn = false;
					component.isOn = true;
					SetupListeners();
					return;
				}
			}
		}
		Toggle val2 = group.ActiveToggles().FirstOrDefault((Toggle x) => x.isOn);
		if (Object.op_Implicit((Object)(object)val2))
		{
			val2.isOn = false;
			val2.isOn = true;
		}
		SetupListeners();
	}

	private void OnDisable()
	{
		if (!Application.isQuitting)
		{
			Toggle[] componentsInChildren = ((Component)this).GetComponentsInChildren<Toggle>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				((UnityEvent<bool>)(object)componentsInChildren[i].onValueChanged).RemoveListener((UnityAction<bool>)OnToggleChanged);
			}
		}
	}

	private void SetupListeners()
	{
		Toggle[] componentsInChildren = ((Component)this).GetComponentsInChildren<Toggle>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			((UnityEvent<bool>)(object)componentsInChildren[i].onValueChanged).AddListener((UnityAction<bool>)OnToggleChanged);
		}
	}

	private void OnToggleChanged(bool b)
	{
		Toggle val = ((Component)this).GetComponentsInChildren<Toggle>().FirstOrDefault((Toggle x) => x.isOn);
		if (Object.op_Implicit((Object)(object)val))
		{
			PlayerPrefs.SetString("ToggleGroupCookie_" + ((Object)this).name, ((Object)((Component)val).gameObject).name);
		}
	}

	private static Transform FindChild(Transform parent, string name)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		foreach (Transform item in parent)
		{
			Transform val = item;
			if (((Object)val).name == name)
			{
				return val;
			}
		}
		return null;
	}
}
