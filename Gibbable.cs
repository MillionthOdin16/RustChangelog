using System;
using System.Collections.Generic;
using UnityEngine;

public class Gibbable : PrefabAttribute, IClientComponent
{
	[Serializable]
	public struct OverrideMesh
	{
		public bool enabled;

		public ColliderType ColliderType;

		public Vector3 BoxSize;

		public Vector3 ColliderCentre;

		public float ColliderRadius;

		public float CapsuleHeight;

		public int CapsuleDirection;

		public bool BlockMaterialCopy;
	}

	public enum ColliderType
	{
		Box,
		Sphere,
		Capsule
	}

	public enum ParentingType
	{
		None,
		GibsOnly,
		FXOnly,
		All
	}

	public enum BoundsEffectType
	{
		None,
		Electrical,
		Glass,
		Scrap,
		Stone,
		Wood
	}

	public GameObject gibSource;

	public Material[] customMaterials;

	public GameObject materialSource;

	public bool copyMaterialBlock = true;

	public bool applyDamageTexture;

	public PhysicMaterial physicsMaterial;

	public GameObjectRef fxPrefab;

	public bool spawnFxPrefab = true;

	[Tooltip("If enabled, gibs will spawn even though we've hit a gib limit")]
	public bool important = false;

	public bool useContinuousCollision = false;

	public float explodeScale = 0f;

	public float scaleOverride = 1f;

	[ReadOnly]
	public int uniqueId = 0;

	public BoundsEffectType boundsEffectType = BoundsEffectType.None;

	public bool isConditional = false;

	[ReadOnly]
	public Bounds effectBounds;

	public List<OverrideMesh> MeshOverrides = new List<OverrideMesh>();

	public bool UsePerGibWaterCheck = false;

	protected override Type GetIndexedType()
	{
		return typeof(Gibbable);
	}
}
