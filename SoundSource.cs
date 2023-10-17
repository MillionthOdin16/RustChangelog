using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour, IClientComponentEx, ILOD
{
	[Serializable]
	public class OcclusionPoint
	{
		public Vector3 offset = Vector3.zero;

		public bool isOccluded = false;
	}

	[Header("Occlusion")]
	public bool handleOcclusionChecks = false;

	public LayerMask occlusionLayerMask;

	public List<OcclusionPoint> occlusionPoints = new List<OcclusionPoint>();

	public bool isOccluded = false;

	public float occlusionAmount = 0f;

	public float lodDistance = 100f;

	public bool inRange = false;

	public virtual void PreClientComponentCull(IPrefabProcessor p)
	{
		p.RemoveComponent((Component)(object)this);
	}

	public bool IsSyncedToParent()
	{
		return false;
	}
}
