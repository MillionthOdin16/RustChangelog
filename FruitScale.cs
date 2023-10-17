using UnityEngine;

public class FruitScale : MonoBehaviour, IClientComponent
{
	public void SetProgress(float progress)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localScale = Vector3.one * progress;
	}
}
