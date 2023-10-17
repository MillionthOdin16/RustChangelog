using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

public class ArticulatedOccludee : BaseMonoBehaviour
{
	private const float UpdateBoundsFadeStart = 20f;

	private const float UpdateBoundsFadeLength = 1000f;

	private const float UpdateBoundsMaxFrequency = 15f;

	private const float UpdateBoundsMinFrequency = 0.5f;

	private LODGroup lodGroup = null;

	public List<Collider> colliders = new List<Collider>();

	private OccludeeSphere localOccludee = new OccludeeSphere(-1);

	private List<Renderer> renderers = new List<Renderer>();

	private bool isVisible = true;

	private Action TriggerUpdateVisibilityBoundsDelegate;

	public bool IsVisible => isVisible;

	protected virtual void OnDisable()
	{
		if (!Application.isQuitting)
		{
			UnregisterFromCulling();
			ClearVisibility();
		}
	}

	private void ClearVisibility()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)lodGroup != (Object)null)
		{
			lodGroup.localReferencePoint = Vector3.zero;
			lodGroup.RecalculateBounds();
			lodGroup = null;
		}
		if (renderers != null)
		{
			renderers.Clear();
		}
		localOccludee = new OccludeeSphere(-1);
	}

	public void ProcessVisibility(LODGroup lod)
	{
		lodGroup = lod;
		if ((Object)(object)lod != (Object)null)
		{
			renderers = new List<Renderer>(16);
			LOD[] lODs = lod.GetLODs();
			for (int i = 0; i < lODs.Length; i++)
			{
				Renderer[] array = lODs[i].renderers;
				foreach (Renderer val in array)
				{
					if ((Object)(object)val != (Object)null)
					{
						renderers.Add(val);
					}
				}
			}
		}
		UpdateCullingBounds();
	}

	private void RegisterForCulling(OcclusionCulling.Sphere sphere, bool visible)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (localOccludee.IsRegistered)
		{
			UnregisterFromCulling();
		}
		int num = OcclusionCulling.RegisterOccludee(sphere.position, sphere.radius, visible, 0.25f, isStatic: false, ((Component)this).gameObject.layer, OnVisibilityChanged);
		if (num >= 0)
		{
			localOccludee = new OccludeeSphere(num, localOccludee.sphere);
			return;
		}
		localOccludee.Invalidate();
		Debug.LogWarning((object)("[OcclusionCulling] Occludee registration failed for " + ((Object)this).name + ". Too many registered."));
	}

	private void UnregisterFromCulling()
	{
		if (localOccludee.IsRegistered)
		{
			OcclusionCulling.UnregisterOccludee(localOccludee.id);
			localOccludee.Invalidate();
		}
	}

	public void UpdateCullingBounds()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.zero;
		Vector3 val2 = Vector3.zero;
		bool flag = false;
		int num = ((renderers != null) ? renderers.Count : 0);
		int num2 = ((colliders != null) ? colliders.Count : 0);
		if (num > 0 && (num2 == 0 || num < num2))
		{
			Profiler.BeginSample("RendererBounds");
			for (int i = 0; i < renderers.Count; i++)
			{
				if (renderers[i].isVisible)
				{
					Bounds bounds = renderers[i].bounds;
					Vector3 min = ((Bounds)(ref bounds)).min;
					Vector3 max = ((Bounds)(ref bounds)).max;
					if (!flag)
					{
						val = min;
						val2 = max;
						flag = true;
						continue;
					}
					val.x = ((val.x < min.x) ? val.x : min.x);
					val.y = ((val.y < min.y) ? val.y : min.y);
					val.z = ((val.z < min.z) ? val.z : min.z);
					val2.x = ((val2.x > max.x) ? val2.x : max.x);
					val2.y = ((val2.y > max.y) ? val2.y : max.y);
					val2.z = ((val2.z > max.z) ? val2.z : max.z);
				}
			}
			Profiler.EndSample();
		}
		if (!flag && num2 > 0)
		{
			Profiler.BeginSample("ColliderBounds");
			flag = true;
			Bounds bounds2 = colliders[0].bounds;
			val = ((Bounds)(ref bounds2)).min;
			bounds2 = colliders[0].bounds;
			val2 = ((Bounds)(ref bounds2)).max;
			for (int j = 1; j < colliders.Count; j++)
			{
				Bounds bounds3 = colliders[j].bounds;
				Vector3 min2 = ((Bounds)(ref bounds3)).min;
				Vector3 max2 = ((Bounds)(ref bounds3)).max;
				val.x = ((val.x < min2.x) ? val.x : min2.x);
				val.y = ((val.y < min2.y) ? val.y : min2.y);
				val.z = ((val.z < min2.z) ? val.z : min2.z);
				val2.x = ((val2.x > max2.x) ? val2.x : max2.x);
				val2.y = ((val2.y > max2.y) ? val2.y : max2.y);
				val2.z = ((val2.z > max2.z) ? val2.z : max2.z);
			}
			Profiler.EndSample();
		}
		if (!flag)
		{
			return;
		}
		Vector3 val3 = val2 - val;
		Vector3 position = val + val3 * 0.5f;
		float radius = Mathf.Max(Mathf.Max(val3.x, val3.y), val3.z) * 0.5f;
		OcclusionCulling.Sphere sphere = new OcclusionCulling.Sphere(position, radius);
		if (localOccludee.IsRegistered)
		{
			OcclusionCulling.UpdateDynamicOccludee(localOccludee.id, sphere.position, sphere.radius);
			localOccludee.sphere = sphere;
			return;
		}
		bool visible = true;
		if ((Object)(object)lodGroup != (Object)null)
		{
			visible = lodGroup.enabled;
		}
		RegisterForCulling(sphere, visible);
	}

	protected virtual bool CheckVisibility()
	{
		if (localOccludee.state != null)
		{
			return localOccludee.state.isVisible;
		}
		return true;
	}

	private void ApplyVisibility(bool vis)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)lodGroup != (Object)null)
		{
			float num = ((!vis) ? 100000 : 0);
			if (num != lodGroup.localReferencePoint.x)
			{
				lodGroup.localReferencePoint = new Vector3(num, num, num);
			}
		}
	}

	protected virtual void OnVisibilityChanged(bool visible)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)MainCamera.mainCamera != (Object)null && localOccludee.IsRegistered)
		{
			float dist = Vector3.Distance(MainCamera.position, ((Component)this).transform.position);
			VisUpdateUsingCulling(dist, visible);
			ApplyVisibility(isVisible);
		}
	}

	private void UpdateVisibility(float delay)
	{
	}

	private void VisUpdateUsingCulling(float dist, bool visibility)
	{
	}

	public virtual void TriggerUpdateVisibilityBounds()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((Behaviour)this).enabled)
		{
			Profiler.BeginSample("TriggerUpdateVisibilityBounds");
			Profiler.BeginSample("UpdateFrequency");
			Vector3 val = ((Component)this).transform.position - MainCamera.position;
			float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
			float num = 400f;
			float num2;
			if (sqrMagnitude < num)
			{
				num2 = 1f / Random.Range(5f, 25f);
			}
			else
			{
				float num3 = Mathf.Clamp01((Mathf.Sqrt(sqrMagnitude) - 20f) * 0.001f);
				float num4 = Mathf.Lerp(1f / 15f, 2f, num3);
				num2 = Random.Range(num4, num4 + 1f / 15f);
			}
			Profiler.EndSample();
			Profiler.BeginSample("UpdateVisibility");
			UpdateVisibility(num2);
			Profiler.EndSample();
			Profiler.BeginSample("ApplyVisibility");
			ApplyVisibility(isVisible);
			Profiler.EndSample();
			if (TriggerUpdateVisibilityBoundsDelegate == null)
			{
				TriggerUpdateVisibilityBoundsDelegate = TriggerUpdateVisibilityBounds;
			}
			((FacepunchBehaviour)this).Invoke(TriggerUpdateVisibilityBoundsDelegate, num2);
			Profiler.EndSample();
		}
	}
}
