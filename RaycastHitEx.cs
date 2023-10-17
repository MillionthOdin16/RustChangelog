using Rust;
using UnityEngine;

public static class RaycastHitEx
{
	public static Transform GetTransform(this RaycastHit hit)
	{
		return ((RaycastHit)(ref hit)).transform;
	}

	public static Rigidbody GetRigidbody(this RaycastHit hit)
	{
		return ((RaycastHit)(ref hit)).rigidbody;
	}

	public static Collider GetCollider(this RaycastHit hit)
	{
		return ((RaycastHit)(ref hit)).collider;
	}

	public static BaseEntity GetEntity(this RaycastHit hit)
	{
		return ((Object)(object)((RaycastHit)(ref hit)).collider != (Object)null) ? ((RaycastHit)(ref hit)).collider.ToBaseEntity() : null;
	}

	public static bool IsOnLayer(this RaycastHit hit, Layer rustLayer)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return (Object)(object)((RaycastHit)(ref hit)).collider != (Object)null && ((Component)((RaycastHit)(ref hit)).collider).gameObject.IsOnLayer(rustLayer);
	}

	public static bool IsOnLayer(this RaycastHit hit, int layer)
	{
		return (Object)(object)((RaycastHit)(ref hit)).collider != (Object)null && ((Component)((RaycastHit)(ref hit)).collider).gameObject.IsOnLayer(layer);
	}

	public static bool IsWaterHit(this RaycastHit hit)
	{
		return (Object)(object)((RaycastHit)(ref hit)).collider == (Object)null || ((Component)((RaycastHit)(ref hit)).collider).gameObject.IsOnLayer((Layer)4);
	}

	public static WaterBody GetWaterBody(this RaycastHit hit)
	{
		if ((Object)(object)((RaycastHit)(ref hit)).collider == (Object)null)
		{
			return WaterSystem.Ocean;
		}
		Transform transform = ((Component)((RaycastHit)(ref hit)).collider).transform;
		WaterBody result = default(WaterBody);
		if (((Component)transform).TryGetComponent<WaterBody>(ref result))
		{
			return result;
		}
		return ((Component)transform.parent).GetComponentInChildren<WaterBody>();
	}
}
