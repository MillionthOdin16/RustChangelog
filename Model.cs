using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public class Model : MonoBehaviour, IPrefabPreProcess
{
	public SphereCollider collision;

	public Transform rootBone;

	public Transform headBone;

	public Transform eyeBone;

	public Animator animator;

	public Skeleton skeleton;

	[HideInInspector]
	public Transform[] boneTransforms = null;

	[HideInInspector]
	public string[] boneNames = null;

	internal BoneDictionary boneDict = null;

	internal int skin;

	protected void OnEnable()
	{
		skin = -1;
	}

	public void BuildBoneDictionary()
	{
		if (boneDict == null)
		{
			boneDict = new BoneDictionary(((Component)this).transform, boneTransforms, boneNames);
		}
	}

	public int GetSkin()
	{
		return skin;
	}

	private Transform FindBoneInternal(string name)
	{
		BuildBoneDictionary();
		Profiler.BeginSample("Model.FindBoneInternal");
		Transform result = boneDict.FindBone(name, defaultToRoot: false);
		Profiler.EndSample();
		return result;
	}

	public Transform FindBone(string name)
	{
		BuildBoneDictionary();
		Transform result = rootBone;
		if (string.IsNullOrEmpty(name))
		{
			return result;
		}
		Profiler.BeginSample("Model.FindBone");
		result = boneDict.FindBone(name);
		Profiler.EndSample();
		return result;
	}

	public Transform FindBone(uint hash)
	{
		BuildBoneDictionary();
		Transform result = rootBone;
		if (hash == 0)
		{
			return result;
		}
		Profiler.BeginSample("Model.FindBone");
		result = boneDict.FindBone(hash);
		Profiler.EndSample();
		return result;
	}

	public uint FindBoneID(Transform transform)
	{
		BuildBoneDictionary();
		Profiler.BeginSample("Model.FindBoneID");
		uint result = boneDict.FindBoneID(transform);
		Profiler.EndSample();
		return result;
	}

	public Transform[] GetBones()
	{
		BuildBoneDictionary();
		return boneDict.transforms;
	}

	public Transform FindClosestBone(Vector3 worldPos)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Transform result = rootBone;
		float num = float.MaxValue;
		Profiler.BeginSample("Model.FindClosestBone");
		for (int i = 0; i < boneTransforms.Length; i++)
		{
			Transform val = boneTransforms[i];
			if (!((Object)(object)val == (Object)null))
			{
				float num2 = Vector3.Distance(val.position, worldPos);
				if (!(num2 >= num))
				{
					result = val;
					num = num2;
				}
			}
		}
		Profiler.EndSample();
		return result;
	}

	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (!((Object)(object)this == (Object)null))
		{
			if ((Object)(object)animator == (Object)null)
			{
				animator = ((Component)this).GetComponent<Animator>();
			}
			if ((Object)(object)rootBone == (Object)null)
			{
				rootBone = ((Component)this).transform;
			}
			boneTransforms = ((Component)rootBone).GetComponentsInChildren<Transform>(true);
			boneNames = new string[boneTransforms.Length];
			for (int i = 0; i < boneTransforms.Length; i++)
			{
				boneNames[i] = ((Object)boneTransforms[i]).name;
			}
		}
	}
}
