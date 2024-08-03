using TMPro;
using UnityEngine.UI;

public class TweakUISlider : TweakUIBase
{
	public Slider sliderControl;

	public TextMeshProUGUI textControl;

	public static string lastConVarChanged;

	public static TimeSince timeSinceLastConVarChange;

	protected override void Init()
	{
		base.Init();
		ResetToConvar();
	}

	protected void OnEnable()
	{
		ResetToConvar();
	}

	public void OnChanged()
	{
		RefreshSliderDisplay(sliderControl.value);
		if (ApplyImmediatelyOnChange)
		{
			SetConvarValue();
		}
	}

	protected override void SetConvarValue()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		base.SetConvarValue();
		if (conVar != null)
		{
			float value = sliderControl.value;
			if (conVar.AsFloat != value)
			{
				conVar.Set(value);
				RefreshSliderDisplay(conVar.AsFloat);
				lastConVarChanged = conVar.FullName;
				timeSinceLastConVarChange = TimeSince.op_Implicit(0f);
			}
		}
	}

	public override void ResetToConvar()
	{
		base.ResetToConvar();
		if (conVar != null)
		{
			RefreshSliderDisplay(conVar.AsFloat);
		}
	}

	private void RefreshSliderDisplay(float value)
	{
		sliderControl.value = value;
		if (sliderControl.wholeNumbers)
		{
			((TMP_Text)textControl).text = sliderControl.value.ToString("N0");
		}
		else
		{
			((TMP_Text)textControl).text = sliderControl.value.ToString("0.0");
		}
	}
}
