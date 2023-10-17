namespace UnityEngine.UI;

public class ScrollRectSettable : ScrollRect
{
	public void SetHorizNormalizedPosition(float value)
	{
		((ScrollRect)this).SetNormalizedPosition(value, 0);
	}

	public void SetVertNormalizedPosition(float value)
	{
		((ScrollRect)this).SetNormalizedPosition(value, 1);
	}
}
