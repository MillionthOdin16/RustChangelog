using UnityEngine;

public class EnvironmentVolume : MonoBehaviour
{
	[InspectorFlags]
	public EnvironmentType Type = EnvironmentType.Underground;

	public Vector3 Center = Vector3.zero;

	public Vector3 Size = Vector3.one;

	public Collider trigger { get; private set; }

	protected virtual void Awake()
	{
		UpdateTrigger();
	}

	protected void OnEnable()
	{
		if (Object.op_Implicit((Object)(object)trigger) && !trigger.enabled)
		{
			trigger.enabled = true;
		}
	}

	protected void OnDisable()
	{
		if (Object.op_Implicit((Object)(object)trigger) && trigger.enabled)
		{
			trigger.enabled = false;
		}
	}

	public void UpdateTrigger()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)trigger))
		{
			trigger = ((Component)this).gameObject.GetComponent<Collider>();
		}
		if (!Object.op_Implicit((Object)(object)trigger))
		{
			trigger = (Collider)(object)((Component)this).gameObject.AddComponent<BoxCollider>();
		}
		trigger.isTrigger = true;
		Collider obj = trigger;
		BoxCollider val = (BoxCollider)(object)((obj is BoxCollider) ? obj : null);
		if (Object.op_Implicit((Object)(object)val))
		{
			val.center = Center;
			val.size = Size;
		}
	}
}
