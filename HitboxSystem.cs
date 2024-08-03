using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

public class HitboxSystem : MonoBehaviour, IPrefabPreProcess
{
	[Serializable]
	public class HitboxShape
	{
		public Transform bone;

		public HitboxDefinition.Type type;

		public Matrix4x4 localTransform;

		public PhysicMaterial colliderMaterial;

		private Matrix4x4 transform;

		private Matrix4x4 inverseTransform;

		public Matrix4x4 Transform => transform;

		public Vector3 Position => ((Matrix4x4)(ref transform)).MultiplyPoint(Vector3.zero);

		public Quaternion Rotation => ((Matrix4x4)(ref transform)).rotation;

		public Vector3 Size { get; private set; }

		public void UpdateTransform()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			TimeWarning val = TimeWarning.New("HitboxSystem.UpdateTransform", 0);
			try
			{
				transform = bone.localToWorldMatrix * localTransform;
				Size = ((Matrix4x4)(ref transform)).lossyScale;
				transform = Matrix4x4.TRS(Position, Rotation, Vector3.one);
				inverseTransform = ((Matrix4x4)(ref transform)).inverse;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		public Vector3 TransformPoint(Vector3 pt)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return ((Matrix4x4)(ref transform)).MultiplyPoint(pt);
		}

		public Vector3 InverseTransformPoint(Vector3 pt)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return ((Matrix4x4)(ref inverseTransform)).MultiplyPoint(pt);
		}

		public Vector3 TransformDirection(Vector3 pt)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return ((Matrix4x4)(ref transform)).MultiplyVector(pt);
		}

		public Vector3 InverseTransformDirection(Vector3 pt)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			return ((Matrix4x4)(ref inverseTransform)).MultiplyVector(pt);
		}

		public bool Trace(Ray ray, out RaycastHit hit, float forgivness = 0f, float maxDistance = float.PositiveInfinity)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			TimeWarning val = TimeWarning.New("Hitbox.Trace", 0);
			try
			{
				((Ray)(ref ray)).origin = InverseTransformPoint(((Ray)(ref ray)).origin);
				((Ray)(ref ray)).direction = InverseTransformDirection(((Ray)(ref ray)).direction);
				if (type == HitboxDefinition.Type.BOX)
				{
					AABB val2 = default(AABB);
					((AABB)(ref val2))._002Ector(Vector3.zero, Size);
					if (!((AABB)(ref val2)).Trace(ray, ref hit, forgivness, maxDistance))
					{
						return false;
					}
				}
				else
				{
					Capsule val3 = default(Capsule);
					((Capsule)(ref val3))._002Ector(Vector3.zero, Size.x, Size.y * 0.5f);
					if (!((Capsule)(ref val3)).Trace(ray, ref hit, forgivness, maxDistance))
					{
						return false;
					}
				}
				((RaycastHit)(ref hit)).point = TransformPoint(((RaycastHit)(ref hit)).point);
				((RaycastHit)(ref hit)).normal = TransformDirection(((RaycastHit)(ref hit)).normal);
				return true;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}

		public Bounds GetBounds()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			Matrix4x4 val = Transform;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					((Matrix4x4)(ref val))[i, j] = Mathf.Abs(((Matrix4x4)(ref val))[i, j]);
				}
			}
			Bounds result = default(Bounds);
			Matrix4x4 val2 = Transform;
			((Bounds)(ref result)).center = ((Matrix4x4)(ref val2)).MultiplyPoint(Vector3.zero);
			((Bounds)(ref result)).extents = ((Matrix4x4)(ref val)).MultiplyVector(Size);
			return result;
		}
	}

	public List<HitboxShape> hitboxes = new List<HitboxShape>();

	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		List<HitboxDefinition> list = Pool.GetList<HitboxDefinition>();
		((Component)this).GetComponentsInChildren<HitboxDefinition>(list);
		if (serverside)
		{
			foreach (HitboxDefinition item2 in list)
			{
				if (preProcess != null)
				{
					preProcess.RemoveComponent((Component)(object)item2);
				}
			}
			if (preProcess != null)
			{
				preProcess.RemoveComponent((Component)(object)this);
			}
		}
		if (clientside)
		{
			hitboxes.Clear();
			foreach (HitboxDefinition item3 in list.OrderBy((HitboxDefinition x) => x.priority))
			{
				HitboxShape item = new HitboxShape
				{
					bone = ((Component)item3).transform,
					localTransform = item3.LocalMatrix,
					colliderMaterial = item3.physicMaterial,
					type = item3.type
				};
				hitboxes.Add(item);
				if (preProcess != null)
				{
					preProcess.RemoveComponent((Component)(object)item3);
				}
			}
		}
		Pool.FreeList<HitboxDefinition>(ref list);
	}
}
