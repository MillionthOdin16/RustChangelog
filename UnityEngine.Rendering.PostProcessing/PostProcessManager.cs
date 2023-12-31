using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing;

public sealed class PostProcessManager
{
	private static PostProcessManager s_Instance;

	private const int k_MaxLayerCount = 32;

	private readonly Dictionary<int, List<PostProcessVolume>> m_SortedVolumes;

	private readonly List<PostProcessVolume> m_Volumes;

	private readonly Dictionary<int, bool> m_SortNeeded;

	private readonly List<PostProcessEffectSettings> m_BaseSettings;

	public readonly Dictionary<Type, PostProcessAttribute> settingsTypes;

	public static PostProcessManager instance
	{
		get
		{
			if (s_Instance == null)
			{
				s_Instance = new PostProcessManager();
			}
			return s_Instance;
		}
	}

	private PostProcessManager()
	{
		m_SortedVolumes = new Dictionary<int, List<PostProcessVolume>>();
		m_Volumes = new List<PostProcessVolume>();
		m_SortNeeded = new Dictionary<int, bool>();
		m_BaseSettings = new List<PostProcessEffectSettings>();
		settingsTypes = new Dictionary<Type, PostProcessAttribute>();
		ReloadBaseTypes();
	}

	private void CleanBaseTypes()
	{
		settingsTypes.Clear();
		foreach (PostProcessEffectSettings baseSetting in m_BaseSettings)
		{
			RuntimeUtilities.Destroy((Object)(object)baseSetting);
		}
		m_BaseSettings.Clear();
	}

	private void ReloadBaseTypes()
	{
		CleanBaseTypes();
		foreach (Type item in from t in RuntimeUtilities.GetAllAssemblyTypes()
			where t.IsSubclassOf(typeof(PostProcessEffectSettings)) && t.IsDefined(typeof(PostProcessAttribute), inherit: false) && !t.IsAbstract
			select t)
		{
			settingsTypes.Add(item, item.GetAttribute<PostProcessAttribute>());
			PostProcessEffectSettings postProcessEffectSettings = (PostProcessEffectSettings)(object)ScriptableObject.CreateInstance(item);
			postProcessEffectSettings.SetAllOverridesTo(state: true, excludeEnabled: false);
			m_BaseSettings.Add(postProcessEffectSettings);
		}
	}

	public void GetActiveVolumes(PostProcessLayer layer, List<PostProcessVolume> results, bool skipDisabled = true, bool skipZeroWeight = true)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		int value = ((LayerMask)(ref layer.volumeLayer)).value;
		Transform volumeTrigger = layer.volumeTrigger;
		bool flag = (Object)(object)volumeTrigger == (Object)null;
		Vector3 val = (flag ? Vector3.zero : volumeTrigger.position);
		OBB val2 = default(OBB);
		foreach (PostProcessVolume item in GrabVolumes(LayerMask.op_Implicit(value)))
		{
			if ((skipDisabled && !((Behaviour)item).enabled) || (Object)(object)item.profileRef == (Object)null || (skipZeroWeight && item.weight <= 0f))
			{
				continue;
			}
			if (item.isGlobal)
			{
				results.Add(item);
			}
			else if (!flag)
			{
				((OBB)(ref val2))._002Ector(((Component)item).transform, item.bounds);
				Vector3 val3 = (((OBB)(ref val2)).ClosestPoint(val) - val) / 2f;
				float sqrMagnitude = ((Vector3)(ref val3)).sqrMagnitude;
				float num = item.blendDistance * item.blendDistance;
				if (sqrMagnitude <= num)
				{
					results.Add(item);
				}
			}
		}
	}

	public PostProcessVolume GetHighestPriorityVolume(PostProcessLayer layer)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)layer == (Object)null)
		{
			throw new ArgumentNullException("layer");
		}
		return GetHighestPriorityVolume(layer.volumeLayer);
	}

	public PostProcessVolume GetHighestPriorityVolume(LayerMask mask)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		float num = float.NegativeInfinity;
		PostProcessVolume result = null;
		if (m_SortedVolumes.TryGetValue(LayerMask.op_Implicit(mask), out var value))
		{
			foreach (PostProcessVolume item in value)
			{
				if (item.priority > num)
				{
					num = item.priority;
					result = item;
				}
			}
		}
		return result;
	}

	public PostProcessVolume QuickVolume(int layer, float priority, params PostProcessEffectSettings[] settings)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		PostProcessVolume postProcessVolume = new GameObject
		{
			name = "Quick Volume",
			layer = layer,
			hideFlags = (HideFlags)61
		}.AddComponent<PostProcessVolume>();
		postProcessVolume.priority = priority;
		postProcessVolume.isGlobal = true;
		PostProcessProfile profile = postProcessVolume.profile;
		foreach (PostProcessEffectSettings postProcessEffectSettings in settings)
		{
			Assert.IsNotNull<PostProcessEffectSettings>(postProcessEffectSettings, "Trying to create a volume with null effects");
			profile.AddSettings(postProcessEffectSettings);
		}
		return postProcessVolume;
	}

	internal void SetLayerDirty(int layer)
	{
		Assert.IsTrue(layer >= 0 && layer <= 32, "Invalid layer bit");
		foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
		{
			int key = sortedVolume.Key;
			if ((key & (1 << layer)) != 0)
			{
				m_SortNeeded[key] = true;
			}
		}
	}

	internal void UpdateVolumeLayer(PostProcessVolume volume, int prevLayer, int newLayer)
	{
		Assert.IsTrue(prevLayer >= 0 && prevLayer <= 32, "Invalid layer bit");
		Unregister(volume, prevLayer);
		Register(volume, newLayer);
	}

	private void Register(PostProcessVolume volume, int layer)
	{
		m_Volumes.Add(volume);
		foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
		{
			if ((sortedVolume.Key & (1 << layer)) != 0)
			{
				sortedVolume.Value.Add(volume);
			}
		}
		SetLayerDirty(layer);
	}

	internal void Register(PostProcessVolume volume)
	{
		int layer = ((Component)volume).gameObject.layer;
		Register(volume, layer);
	}

	private void Unregister(PostProcessVolume volume, int layer)
	{
		m_Volumes.Remove(volume);
		foreach (KeyValuePair<int, List<PostProcessVolume>> sortedVolume in m_SortedVolumes)
		{
			if ((sortedVolume.Key & (1 << layer)) != 0)
			{
				sortedVolume.Value.Remove(volume);
			}
		}
	}

	internal void Unregister(PostProcessVolume volume)
	{
		int layer = ((Component)volume).gameObject.layer;
		Unregister(volume, layer);
	}

	private void ReplaceData(PostProcessLayer postProcessLayer)
	{
		foreach (PostProcessEffectSettings baseSetting in m_BaseSettings)
		{
			PostProcessEffectSettings settings = postProcessLayer.GetBundle(((object)baseSetting).GetType()).settings;
			int count = baseSetting.parameters.Count;
			for (int i = 0; i < count; i++)
			{
				settings.parameters[i].SetValue(baseSetting.parameters[i]);
			}
		}
	}

	internal void UpdateSettings(PostProcessLayer postProcessLayer, Camera camera)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		ReplaceData(postProcessLayer);
		int value = ((LayerMask)(ref postProcessLayer.volumeLayer)).value;
		Transform volumeTrigger = postProcessLayer.volumeTrigger;
		bool flag = (Object)(object)volumeTrigger == (Object)null;
		Vector3 val = (flag ? Vector3.zero : volumeTrigger.position);
		OBB val2 = default(OBB);
		foreach (PostProcessVolume item in GrabVolumes(LayerMask.op_Implicit(value)))
		{
			if (!((Behaviour)item).enabled || (Object)(object)item.profileRef == (Object)null || item.weight <= 0f)
			{
				continue;
			}
			List<PostProcessEffectSettings> settings = item.profileRef.settings;
			if (item.isGlobal)
			{
				postProcessLayer.OverrideSettings(settings, Mathf.Clamp01(item.weight));
			}
			else
			{
				if (flag)
				{
					continue;
				}
				((OBB)(ref val2))._002Ector(((Component)item).transform, item.bounds);
				Vector3 val3 = (((OBB)(ref val2)).ClosestPoint(val) - val) / 2f;
				float sqrMagnitude = ((Vector3)(ref val3)).sqrMagnitude;
				float num = item.blendDistance * item.blendDistance;
				if (!(sqrMagnitude > num))
				{
					float num2 = 1f;
					if (num > 0f)
					{
						num2 = 1f - sqrMagnitude / num;
					}
					postProcessLayer.OverrideSettings(settings, num2 * Mathf.Clamp01(item.weight));
				}
			}
		}
	}

	private List<PostProcessVolume> GrabVolumes(LayerMask mask)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (!m_SortedVolumes.TryGetValue(LayerMask.op_Implicit(mask), out var value))
		{
			value = new List<PostProcessVolume>();
			foreach (PostProcessVolume volume in m_Volumes)
			{
				if ((LayerMask.op_Implicit(mask) & (1 << ((Component)volume).gameObject.layer)) != 0)
				{
					value.Add(volume);
					m_SortNeeded[LayerMask.op_Implicit(mask)] = true;
				}
			}
			m_SortedVolumes.Add(LayerMask.op_Implicit(mask), value);
		}
		if (m_SortNeeded.TryGetValue(LayerMask.op_Implicit(mask), out var value2) && value2)
		{
			m_SortNeeded[LayerMask.op_Implicit(mask)] = false;
			SortByPriority(value);
		}
		return value;
	}

	private static void SortByPriority(List<PostProcessVolume> volumes)
	{
		Assert.IsNotNull<List<PostProcessVolume>>(volumes, "Trying to sort volumes of non-initialized layer");
		for (int i = 1; i < volumes.Count; i++)
		{
			PostProcessVolume postProcessVolume = volumes[i];
			int num = i - 1;
			while (num >= 0 && volumes[num].priority > postProcessVolume.priority)
			{
				volumes[num + 1] = volumes[num];
				num--;
			}
			volumes[num + 1] = postProcessVolume;
		}
	}

	private static bool IsVolumeRenderedByCamera(PostProcessVolume volume, Camera camera)
	{
		return true;
	}
}
