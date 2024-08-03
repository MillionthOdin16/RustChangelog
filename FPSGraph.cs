using ConVar;
using UnityEngine;

public class FPSGraph : Graph
{
	public void Refresh()
	{
		((Behaviour)this).enabled = FPS.graph > 0;
		((Rect)(ref Area)).width = (Resolution = Mathf.Clamp(FPS.graph, 0, Screen.width));
	}

	protected void OnEnable()
	{
		Refresh();
	}

	protected override float GetValue()
	{
		return 1f / Time.deltaTime;
	}

	protected override Color GetColor(float value)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return (value < 10f) ? Color.red : ((value < 30f) ? Color.yellow : Color.green);
	}
}
