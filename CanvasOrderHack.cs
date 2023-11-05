using UnityEngine;

public class CanvasOrderHack : MonoBehaviour
{
	private void OnEnable()
	{
		Canvas[] componentsInChildren = ((Component)this).GetComponentsInChildren<Canvas>(true);
		foreach (Canvas val in componentsInChildren)
		{
			if (val.overrideSorting)
			{
				int sortingOrder = val.sortingOrder;
				val.sortingOrder = sortingOrder + 1;
			}
		}
		Canvas[] componentsInChildren2 = ((Component)this).GetComponentsInChildren<Canvas>(true);
		foreach (Canvas val2 in componentsInChildren2)
		{
			if (val2.overrideSorting)
			{
				int sortingOrder = val2.sortingOrder;
				val2.sortingOrder = sortingOrder - 1;
			}
		}
	}
}
