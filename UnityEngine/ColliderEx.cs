using Rust;

namespace UnityEngine;

public static class ColliderEx
{
	public static PhysicMaterial GetMaterialAt(this Collider obj, Vector3 pos)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)obj == (Object)null)
		{
			return TerrainMeta.Config.WaterMaterial;
		}
		if (obj is TerrainCollider)
		{
			return TerrainMeta.Physics.GetMaterial(pos);
		}
		return obj.sharedMaterial;
	}

	public static bool IsOnLayer(this Collider col, Layer rustLayer)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return (Object)(object)col != (Object)null && ((Component)col).gameObject.IsOnLayer(rustLayer);
	}

	public static bool IsOnLayer(this Collider col, int layer)
	{
		return (Object)(object)col != (Object)null && ((Component)col).gameObject.IsOnLayer(layer);
	}

	public static float GetRadius(this Collider col, Vector3 transformScale)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		float result = 1f;
		SphereCollider val;
		BoxCollider val2;
		CapsuleCollider val4;
		MeshCollider val5;
		if ((val = (SphereCollider)(object)((col is SphereCollider) ? col : null)) != null)
		{
			result = val.radius * Vector3Ex.Max(transformScale);
		}
		else if ((val2 = (BoxCollider)(object)((col is BoxCollider) ? col : null)) != null)
		{
			Vector3 val3 = Vector3.Scale(val2.size, transformScale);
			result = Vector3Ex.Max(val3) * 0.5f;
		}
		else if ((val4 = (CapsuleCollider)(object)((col is CapsuleCollider) ? col : null)) != null)
		{
			float num = val4.direction switch
			{
				0 => transformScale.y, 
				1 => transformScale.x, 
				_ => transformScale.x, 
			};
			result = val4.radius * num;
		}
		else if ((val5 = (MeshCollider)(object)((col is MeshCollider) ? col : null)) != null)
		{
			Bounds bounds = ((Collider)val5).bounds;
			Vector3 val6 = Vector3.Scale(((Bounds)(ref bounds)).size, transformScale);
			result = Vector3Ex.Max(val6) * 0.5f;
		}
		return result;
	}
}
