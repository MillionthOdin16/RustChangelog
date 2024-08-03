using ConVar;
using UnityEngine;

public class TriggerTemperature : TriggerBase
{
	public float Temperature = 50f;

	public float triggerSize;

	public float minSize = 0f;

	public bool sunlightBlocker = false;

	public float sunlightBlockAmount;

	[Range(0f, 24f)]
	public float blockMinHour = 8.5f;

	[Range(0f, 24f)]
	public float blockMaxHour = 18.5f;

	private void OnValidate()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		SphereCollider component = ((Component)this).GetComponent<SphereCollider>();
		if ((Object)(object)component != (Object)null)
		{
			triggerSize = ((Component)this).GetComponent<SphereCollider>().radius * ((Component)this).transform.localScale.y;
			return;
		}
		BoxCollider component2 = ((Component)this).GetComponent<BoxCollider>();
		Vector3 val = Vector3.Scale(component2.size, ((Component)this).transform.localScale);
		triggerSize = Vector3Ex.Max(val) * 0.5f;
	}

	public float WorkoutTemperature(Vector3 position, float oldTemperature)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (sunlightBlocker)
		{
			float time = Env.time;
			if (time >= blockMinHour && time <= blockMaxHour)
			{
				Vector3 position2 = TOD_Sky.Instance.Components.SunTransform.position;
				if (!GamePhysics.LineOfSight(position, position2, 256))
				{
					return oldTemperature - sunlightBlockAmount;
				}
			}
			return oldTemperature;
		}
		float num = Vector3.Distance(((Component)this).gameObject.transform.position, position);
		float num2 = Mathf.InverseLerp(triggerSize, minSize, num);
		return Mathf.Lerp(oldTemperature, Temperature, num2);
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return ((Component)baseEntity).gameObject;
	}
}
