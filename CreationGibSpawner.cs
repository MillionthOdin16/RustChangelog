using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreationGibSpawner : BaseMonoBehaviour
{
	[Serializable]
	public class GibReplacement
	{
		public GameObject oldGib;

		public GameObject newGib;
	}

	[Serializable]
	public class EffectMaterialPair
	{
		public PhysicMaterial material;

		public GameObjectRef effect;
	}

	[Serializable]
	public struct ConditionalGibSource
	{
		public GameObject source;

		public Vector3 pos;

		public Quaternion rot;
	}

	private GameObject gibSource;

	public GameObject gibsInstance;

	public float startTime = 0f;

	public float duration = 1f;

	public float buildScaleAdditionalAmount = 0.5f;

	[Tooltip("Entire object will be scaled on xyz during duration by this curve")]
	public AnimationCurve scaleCurve;

	[Tooltip("Object will be pushed out along transform.forward/right/up based on build direction by this amount")]
	public AnimationCurve buildCurve;

	[Tooltip("Additional scaling to apply to object based on build direction")]
	public AnimationCurve buildScaleCurve;

	public AnimationCurve xCurve;

	public AnimationCurve yCurve;

	public AnimationCurve zCurve;

	public Vector3[] spawnPositions;

	public GameObject[] particles;

	public float[] gibProgress;

	public PhysicMaterial physMaterial;

	public List<Transform> gibs;

	public bool started = false;

	public GameObjectRef placeEffect;

	public GameObject smokeEffect;

	public float effectSpacing = 0.2f;

	public bool invert = false;

	public Vector3 buildDirection;

	[Horizontal(1, 0)]
	public GibReplacement[] GibReplacements = null;

	public EffectMaterialPair[] effectLookup;

	private float startDelay = 0f;

	public List<ConditionalGibSource> conditionalGibSources = new List<ConditionalGibSource>();

	private float nextEffectTime = float.NegativeInfinity;

	public GameObjectRef GetEffectForMaterial(PhysicMaterial mat)
	{
		EffectMaterialPair[] array = effectLookup;
		foreach (EffectMaterialPair effectMaterialPair in array)
		{
			if ((Object)(object)effectMaterialPair.material == (Object)(object)mat)
			{
				return effectMaterialPair.effect;
			}
		}
		return effectLookup[0].effect;
	}

	public void SetDelay(float newDelay)
	{
		startDelay = newDelay;
	}

	public void FinishSpawn()
	{
		if (startDelay == 0f)
		{
			Init();
		}
		else
		{
			((FacepunchBehaviour)this).Invoke((Action)Init, startDelay);
		}
	}

	public float GetProgress(float delay)
	{
		if (!started)
		{
			return 0f;
		}
		if (duration == 0f)
		{
			return 1f;
		}
		return Mathf.Clamp01((Time.time - (startTime + delay)) / duration);
	}

	public void AddConditionalGibSource(GameObject cGibSource, Vector3 pos, Quaternion rot)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"Adding conditional gib source");
		ConditionalGibSource item = default(ConditionalGibSource);
		item.source = cGibSource;
		item.pos = pos;
		item.rot = rot;
		conditionalGibSources.Add(item);
	}

	public void SetGibSource(GameObject newGibSource)
	{
		GameObject val = newGibSource;
		for (int i = 0; i < GibReplacements.Length; i++)
		{
			if ((Object)(object)GibReplacements[i].oldGib == (Object)(object)newGibSource)
			{
				val = GibReplacements[i].newGib;
				break;
			}
		}
		gibSource = val;
	}

	private int SortsGibs(Transform a, Transform b)
	{
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		MeshRenderer component = ((Component)a).GetComponent<MeshRenderer>();
		MeshRenderer component2 = ((Component)b).GetComponent<MeshRenderer>();
		Bounds bounds;
		if (!invert)
		{
			float y;
			if (!((Object)(object)component == (Object)null))
			{
				bounds = ((Renderer)component).bounds;
				y = ((Bounds)(ref bounds)).center.y;
			}
			else
			{
				y = a.localPosition.y;
			}
			float num = y;
			float y2;
			if (!((Object)(object)component2 == (Object)null))
			{
				bounds = ((Renderer)component2).bounds;
				y2 = ((Bounds)(ref bounds)).center.y;
			}
			else
			{
				y2 = b.localPosition.y;
			}
			float value = y2;
			return num.CompareTo(value);
		}
		float y3;
		if (!((Object)(object)component == (Object)null))
		{
			bounds = ((Renderer)component).bounds;
			y3 = ((Bounds)(ref bounds)).center.y;
		}
		else
		{
			y3 = a.localPosition.y;
		}
		float value2 = y3;
		float y4;
		if (!((Object)(object)component2 == (Object)null))
		{
			bounds = ((Renderer)component2).bounds;
			y4 = ((Bounds)(ref bounds)).center.y;
		}
		else
		{
			y4 = b.localPosition.y;
		}
		float num2 = y4;
		return num2.CompareTo(value2);
	}

	public void Init()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		started = true;
		startTime = Time.time;
		gibsInstance = Object.Instantiate<GameObject>(gibSource, ((Component)this).transform.position, ((Component)this).transform.rotation);
		Transform[] componentsInChildren = gibsInstance.GetComponentsInChildren<Transform>();
		List<Transform> list = componentsInChildren.ToList();
		list.Remove(gibsInstance.transform);
		list.Sort(SortsGibs);
		gibs = list;
		spawnPositions = (Vector3[])(object)new Vector3[gibs.Count];
		gibProgress = new float[gibs.Count];
		particles = (GameObject[])(object)new GameObject[gibs.Count];
		for (int i = 0; i < gibs.Count; i++)
		{
			Transform val = gibs[i];
			spawnPositions[i] = val.localPosition;
			gibProgress[i] = 0f;
			particles[i] = null;
			val.localScale = Vector3.one * scaleCurve.Evaluate(0f);
			float num = ((spawnPositions[i].x >= 0f) ? (-1f) : 1f);
			Transform transform = ((Component)val).transform;
			transform.position += ((Component)this).transform.right * GetPushDir(spawnPositions[i], val) * buildCurve.Evaluate(0f) * buildDirection.x;
			Transform transform2 = ((Component)val).transform;
			transform2.position += ((Component)this).transform.up * yCurve.Evaluate(0f);
			Transform transform3 = ((Component)val).transform;
			transform3.position += ((Component)this).transform.forward * zCurve.Evaluate(0f);
		}
		((FacepunchBehaviour)this).Invoke((Action)DestroyMe, duration + 0.05f);
	}

	public float GetPushDir(Vector3 spawnPos, Transform theGib)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return (spawnPos.x >= 0f) ? (-1f) : 1f;
	}

	public void DestroyMe()
	{
		Object.Destroy((Object)(object)gibsInstance);
	}

	public float GetStartDelay(Transform gib)
	{
		return 0f;
	}

	public void Update()
	{
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		if (!started)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		int num = Mathf.CeilToInt((float)gibs.Count / 10f);
		for (int i = 0; i < gibs.Count; i++)
		{
			Transform val = gibs[i];
			if ((Object)(object)val == (Object)(object)((Component)this).transform)
			{
				continue;
			}
			if (deltaTime <= 0f)
			{
				break;
			}
			float num2 = 0.33f;
			float num3 = num2 / ((float)gibs.Count * num2) * (duration - num2);
			float num4 = (float)i * num3;
			float num5 = Time.time - startTime;
			if (num5 < num4)
			{
				continue;
			}
			MeshFilter component = ((Component)val).GetComponent<MeshFilter>();
			int seed = Random.seed;
			Random.seed = i + gibs.Count;
			bool flag = num <= 1 || Random.Range(0, num) == 0;
			Random.seed = seed;
			if (flag && (Object)(object)particles[i] == (Object)null && (Object)(object)component != (Object)null && (Object)(object)component.sharedMesh != (Object)null)
			{
				Bounds bounds = component.sharedMesh.bounds;
				Vector3 size = ((Bounds)(ref bounds)).size;
				if (((Vector3)(ref size)).magnitude == 0f)
				{
					continue;
				}
				GameObject val2 = Object.Instantiate<GameObject>(smokeEffect);
				val2.transform.SetParent(val);
				val2.transform.localPosition = Vector3.zero;
				val2.transform.localScale = Vector3.one;
				val2.transform.localRotation = Quaternion.identity;
				ParticleSystem component2 = val2.GetComponent<ParticleSystem>();
				MeshRenderer component3 = ((Component)component).GetComponent<MeshRenderer>();
				ShapeModule shape = component2.shape;
				((ShapeModule)(ref shape)).shapeType = (ParticleSystemShapeType)5;
				bounds = ((Renderer)component3).bounds;
				((ShapeModule)(ref shape)).boxThickness = ((Bounds)(ref bounds)).extents;
				particles[i] = val2;
			}
			float num6 = Mathf.Clamp01(gibProgress[i] / num2);
			float num7 = Mathf.Clamp01((num6 + Time.deltaTime) / num2);
			gibProgress[i] += Time.deltaTime;
			float num8 = scaleCurve.Evaluate(num7);
			((Component)val).transform.localScale = new Vector3(num8, num8, num8);
			Transform transform = ((Component)val).transform;
			transform.localScale += buildDirection * buildScaleCurve.Evaluate(num7) * buildScaleAdditionalAmount;
			((Component)val).transform.localPosition = spawnPositions[i];
			Transform transform2 = ((Component)val).transform;
			transform2.position += ((Component)this).transform.right * GetPushDir(spawnPositions[i], val) * buildCurve.Evaluate(num7) * buildDirection.x;
			Transform transform3 = ((Component)val).transform;
			transform3.position += ((Component)this).transform.up * buildCurve.Evaluate(num7) * buildDirection.y;
			Transform transform4 = ((Component)val).transform;
			transform4.position += ((Component)this).transform.forward * buildCurve.Evaluate(num7) * buildDirection.z;
			if (num7 >= 1f && num7 > num6 && Time.time > nextEffectTime)
			{
				nextEffectTime = Time.time + effectSpacing;
				if ((Object)(object)particles[i] != (Object)null)
				{
					ParticleSystem component4 = particles[i].GetComponent<ParticleSystem>();
					particles[i].transform.SetParent((Transform)null, true);
					particles[i].BroadcastOnParentDestroying();
				}
			}
		}
	}
}
