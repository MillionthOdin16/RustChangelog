using System;
using UnityEngine.Events;

public class UIEscapeCapture : ListComponent<UIEscapeCapture>
{
	public UnityEvent onEscape = new UnityEvent();

	public static bool EscapePressed()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<UIEscapeCapture> enumerator = ListComponent<UIEscapeCapture>.InstanceList.GetEnumerator();
		try
		{
			if (enumerator.MoveNext())
			{
				UIEscapeCapture current = enumerator.Current;
				current.onEscape.Invoke();
				return true;
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return false;
	}
}
