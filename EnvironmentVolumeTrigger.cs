using UnityEngine;

public class EnvironmentVolumeTrigger : MonoBehaviour
{
	[HideInInspector]
	public Vector3 Center = Vector3.zero;

	[HideInInspector]
	public Vector3 Size = Vector3.one;

	public EnvironmentVolume volume { get; private set; }

	protected void Awake()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		volume = ((Component)this).gameObject.GetComponent<EnvironmentVolume>();
		if ((Object)(object)volume == (Object)null)
		{
			volume = ((Component)this).gameObject.AddComponent<EnvironmentVolume>();
			volume.Center = Center;
			volume.Size = Size;
			volume.UpdateTrigger();
		}
	}
}
