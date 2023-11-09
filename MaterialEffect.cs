using System;
using UnityEngine;
using UnityEngine.Profiling;

[CreateAssetMenu(menuName = "Rust/MaterialEffect")]
public class MaterialEffect : ScriptableObject
{
	[Serializable]
	public class Entry
	{
		public PhysicMaterial Material;

		public GameObjectRef Effect;

		public SoundDefinition SoundDefinition;
	}

	public GameObjectRef DefaultEffect;

	public SoundDefinition DefaultSoundDefinition;

	public Entry[] Entries;

	public int waterFootstepIndex = -1;

	public Entry deepWaterEntry;

	public float deepWaterDepth = -1f;

	public Entry submergedWaterEntry;

	public float submergedWaterDepth = -1f;

	public bool ScaleVolumeWithSpeed = false;

	public AnimationCurve SpeedGainCurve;

	public Entry GetEntryFromMaterial(PhysicMaterial mat)
	{
		Profiler.BeginSample("MaterialEffect.GetEntryFromMaterial");
		Entry[] entries = Entries;
		foreach (Entry entry in entries)
		{
			if ((Object)(object)entry.Material == (Object)(object)mat)
			{
				Profiler.EndSample();
				return entry;
			}
		}
		Profiler.EndSample();
		return null;
	}

	public Entry GetWaterEntry()
	{
		Profiler.BeginSample("MaterialEffect.GetWaterEffect");
		if (waterFootstepIndex == -1)
		{
			for (int i = 0; i < Entries.Length; i++)
			{
				if (((Object)Entries[i].Material).name == "Water")
				{
					waterFootstepIndex = i;
					break;
				}
			}
		}
		Profiler.EndSample();
		if (waterFootstepIndex != -1)
		{
			return Entries[waterFootstepIndex];
		}
		Debug.LogWarning((object)("Unable to find water effect for :" + ((Object)this).name));
		return null;
	}

	public void SpawnOnRay(Ray ray, int mask, float length = 0.5f, Vector3 forward = default(Vector3), float speed = 0f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		if (!GamePhysics.Trace(ray, 0f, out var hitInfo, length, mask, (QueryTriggerInteraction)0))
		{
			Effect.client.Run(DefaultEffect.resourcePath, ((Ray)(ref ray)).origin, ((Ray)(ref ray)).direction * -1f, forward);
			if ((Object)(object)DefaultSoundDefinition != (Object)null)
			{
				PlaySound(DefaultSoundDefinition, ((RaycastHit)(ref hitInfo)).point, speed);
			}
			return;
		}
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(((Ray)(ref ray)).origin, waves: true, volumes: false);
		if (waterInfo.isValid)
		{
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(((Ray)(ref ray)).origin.x, WaterSystem.GetHeight(((Ray)(ref ray)).origin), ((Ray)(ref ray)).origin.z);
			Entry waterEntry = GetWaterEntry();
			if (submergedWaterDepth > 0f && waterInfo.currentDepth >= submergedWaterDepth)
			{
				waterEntry = submergedWaterEntry;
			}
			else if (deepWaterDepth > 0f && waterInfo.currentDepth >= deepWaterDepth)
			{
				waterEntry = deepWaterEntry;
			}
			if (waterEntry != null)
			{
				Effect.client.Run(waterEntry.Effect.resourcePath, val, Vector3.up);
				if ((Object)(object)waterEntry.SoundDefinition != (Object)null)
				{
					PlaySound(waterEntry.SoundDefinition, val, speed);
				}
			}
			return;
		}
		PhysicMaterial materialAt = ((RaycastHit)(ref hitInfo)).collider.GetMaterialAt(((RaycastHit)(ref hitInfo)).point);
		Entry entryFromMaterial = GetEntryFromMaterial(materialAt);
		if (entryFromMaterial == null)
		{
			Effect.client.Run(DefaultEffect.resourcePath, ((RaycastHit)(ref hitInfo)).point, ((RaycastHit)(ref hitInfo)).normal, forward);
			if ((Object)(object)DefaultSoundDefinition != (Object)null)
			{
				PlaySound(DefaultSoundDefinition, ((RaycastHit)(ref hitInfo)).point, speed);
			}
		}
		else
		{
			Effect.client.Run(entryFromMaterial.Effect.resourcePath, ((RaycastHit)(ref hitInfo)).point, ((RaycastHit)(ref hitInfo)).normal, forward);
			if ((Object)(object)entryFromMaterial.SoundDefinition != (Object)null)
			{
				PlaySound(entryFromMaterial.SoundDefinition, ((RaycastHit)(ref hitInfo)).point, speed);
			}
		}
	}

	public void PlaySound(SoundDefinition definition, Vector3 position, float velocity = 0f)
	{
	}
}
