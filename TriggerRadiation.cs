using UnityEngine;

public class TriggerRadiation : TriggerBase
{
	public enum RadiationTier
	{
		MINIMAL,
		LOW,
		MEDIUM,
		HIGH,
		NONE
	}

	public RadiationTier radiationTier = RadiationTier.LOW;

	public bool BypassArmor;

	public float RadiationAmountOverride;

	public float falloff = 0.1f;

	private SphereCollider sphereCollider;

	private float GetRadiationSize()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)sphereCollider))
		{
			sphereCollider = ((Component)this).GetComponent<SphereCollider>();
		}
		return sphereCollider.radius * Vector3Ex.Max(((Component)this).transform.localScale);
	}

	private float GetRadiationAmount()
	{
		if (RadiationAmountOverride > 0f)
		{
			return RadiationAmountOverride;
		}
		if (radiationTier == RadiationTier.NONE)
		{
			return 0f;
		}
		if (radiationTier == RadiationTier.MINIMAL)
		{
			return 2f;
		}
		if (radiationTier == RadiationTier.LOW)
		{
			return 10f;
		}
		if (radiationTier == RadiationTier.MEDIUM)
		{
			return 25f;
		}
		if (radiationTier == RadiationTier.HIGH)
		{
			return 51f;
		}
		return 1f;
	}

	public float GetRadiation(Vector3 position, float radProtection)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		float radiationSize = GetRadiationSize();
		float radiationAmount = GetRadiationAmount();
		float num = Vector3.Distance(((Component)this).gameObject.transform.position, position);
		float num2 = Mathf.InverseLerp(radiationSize, radiationSize * (1f - falloff), num);
		float num3 = radiationAmount;
		if (!BypassArmor)
		{
			num3 = Mathf.Clamp(radiationAmount - radProtection, 0f, radiationAmount);
		}
		return num3 * num2;
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
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		return ((Component)baseEntity).gameObject;
	}

	public void OnDrawGizmosSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		float radiationSize = GetRadiationSize();
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(((Component)this).transform.position, radiationSize);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(((Component)this).transform.position, radiationSize * (1f - falloff));
	}
}
