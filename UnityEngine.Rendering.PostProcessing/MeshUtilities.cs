using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing;

internal static class MeshUtilities
{
	private static Dictionary<PrimitiveType, Mesh> s_Primitives;

	private static Dictionary<Type, PrimitiveType> s_ColliderPrimitives;

	static MeshUtilities()
	{
		s_Primitives = new Dictionary<PrimitiveType, Mesh>();
		s_ColliderPrimitives = new Dictionary<Type, PrimitiveType>
		{
			{
				typeof(BoxCollider),
				(PrimitiveType)3
			},
			{
				typeof(SphereCollider),
				(PrimitiveType)0
			},
			{
				typeof(CapsuleCollider),
				(PrimitiveType)1
			}
		};
	}

	internal static Mesh GetColliderMesh(Collider collider)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Type type = ((object)collider).GetType();
		if (type == typeof(MeshCollider))
		{
			return ((MeshCollider)collider).sharedMesh;
		}
		Assert.IsTrue(s_ColliderPrimitives.ContainsKey(type), "Unknown collider");
		return GetPrimitive(s_ColliderPrimitives[type]);
	}

	internal static Mesh GetPrimitive(PrimitiveType primitiveType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (!s_Primitives.TryGetValue(primitiveType, out var value))
		{
			value = GetBuiltinMesh(primitiveType);
			s_Primitives.Add(primitiveType, value);
		}
		return value;
	}

	private static Mesh GetBuiltinMesh(PrimitiveType primitiveType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameObject.CreatePrimitive(primitiveType);
		Mesh sharedMesh = val.GetComponent<MeshFilter>().sharedMesh;
		RuntimeUtilities.Destroy((Object)(object)val);
		return sharedMesh;
	}
}
