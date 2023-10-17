using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace CompanionServer.Cameras;

internal static class BurstUtil
{
	private struct RaycastHitPublic
	{
		public Vector3 m_Point;

		public Vector3 m_Normal;

		public uint m_FaceID;

		public float m_Distance;

		public Vector2 m_UV;

		public int m_Collider;
	}

	public unsafe static ref readonly T GetReadonly<T>(this in NativeArray<T> array, int index) where T : unmanaged
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (index < 0 || index >= array.Length)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		T* unsafeReadOnlyPtr = (T*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<T>(array);
		return ref unsafeReadOnlyPtr[index];
	}

	public unsafe static ref T Get<T>(this in NativeArray<T> array, int index) where T : unmanaged
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (index < 0 || index >= array.Length)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		T* unsafePtr = (T*)NativeArrayUnsafeUtility.GetUnsafePtr<T>(array);
		return ref unsafePtr[index];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static int GetColliderId(this RaycastHit hit)
	{
		return ((RaycastHitPublic*)(&hit))->m_Collider;
	}

	public unsafe static Collider GetCollider(int colliderInstanceId)
	{
		RaycastHitPublic raycastHitPublic = default(RaycastHitPublic);
		raycastHitPublic.m_Collider = colliderInstanceId;
		RaycastHitPublic raycastHitPublic2 = raycastHitPublic;
		return ((RaycastHit)(&raycastHitPublic2)).collider;
	}
}
