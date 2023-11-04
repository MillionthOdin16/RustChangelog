using UnityEngine;

public class ScaleTransform : ScaleRenderer
{
	private Vector3 initialScale;

	public override void SetScale_Internal(float scale)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		base.SetScale_Internal(scale);
		((Component)myRenderer).transform.localScale = initialScale * scale;
	}

	public override void GatherInitialValues()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		initialScale = ((Component)myRenderer).transform.localScale;
		base.GatherInitialValues();
	}
}
