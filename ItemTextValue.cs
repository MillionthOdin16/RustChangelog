using UnityEngine;
using UnityEngine.UI;

public class ItemTextValue : MonoBehaviour
{
	public Text text;

	public Color bad;

	public Color good;

	public bool negativestat;

	public bool asPercentage;

	public bool useColors = true;

	public bool signed = true;

	public string suffix;

	public float multiplier = 1f;

	public void SetValue(float val, int numDecimals = 0, string overrideText = "")
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		val *= multiplier;
		text.text = ((overrideText == "") ? string.Format("{0}{1:n" + numDecimals + "}", (val > 0f && signed) ? "+" : "", val) : overrideText);
		if (asPercentage)
		{
			Text obj = text;
			obj.text += " %";
		}
		if (suffix != "" && !float.IsPositiveInfinity(val))
		{
			Text obj2 = text;
			obj2.text += suffix;
		}
		bool flag = val > 0f;
		if (negativestat)
		{
			flag = !flag;
		}
		if (useColors)
		{
			((Graphic)text).color = (flag ? good : bad);
		}
	}
}
