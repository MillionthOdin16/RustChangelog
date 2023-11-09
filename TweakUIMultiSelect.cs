using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TweakUIMultiSelect : TweakUIBase
{
	public ToggleGroup toggleGroup;

	protected override void Init()
	{
		base.Init();
		UpdateToggleGroup();
	}

	protected void OnEnable()
	{
		UpdateToggleGroup();
	}

	public void OnChanged()
	{
		UpdateConVar();
	}

	private void UpdateToggleGroup()
	{
		if (conVar != null)
		{
			string @string = conVar.String;
			Toggle[] componentsInChildren = ((Component)toggleGroup).GetComponentsInChildren<Toggle>();
			foreach (Toggle val in componentsInChildren)
			{
				val.isOn = ((Object)val).name == @string;
			}
		}
	}

	private void UpdateConVar()
	{
		if (conVar != null)
		{
			Toggle val = (from x in ((Component)toggleGroup).GetComponentsInChildren<Toggle>()
				where x.isOn
				select x).FirstOrDefault();
			if (!((Object)(object)val == (Object)null) && !(conVar.String == ((Object)val).name))
			{
				conVar.Set(((Object)val).name);
			}
		}
	}
}
