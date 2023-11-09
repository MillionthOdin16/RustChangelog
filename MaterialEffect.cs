using System;
using UnityEngine;

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

	public bool ScaleVolumeWithSpeed;

	public AnimationCurve SpeedGainCurve;

	public Entry GetEntryFromMaterial(PhysicMaterial mat)
	{
		Entry[] entries = Entries;
		foreach (Entry entry in entries)
		{
			if ((Object)(object)entry.Material == (Object)(object)mat)
			{
				return entry;
			}
		}
		return null;
	}

	public Entry GetWaterEntry()
	{
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
		if (waterFootstepIndex != -1)
		{
			return Entries[waterFootstepIndex];
		}
		Debug.LogWarning((object)("Unable to find water effect for :" + ((Object)this).name));
		return null;
	}

	public void SpawnOnRay(Ray ray, int mask, float length = 0.5f, Vector3 forward = default(Vector3), float speed = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
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
