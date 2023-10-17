using System;
using Rust;
using UnityEngine;
using UnityEngine.AI;

public class Barricade : DecayEntity
{
	public float reflectDamage = 5f;

	public GameObjectRef reflectEffect;

	public bool canNpcSmash = true;

	public NavMeshModifierVolume NavMeshVolumeAnimals;

	public NavMeshModifierVolume NavMeshVolumeHumanoids;

	[NonSerialized]
	public NPCBarricadeTriggerBox NpcTriggerBox;

	private static int nonWalkableArea = -1;

	private static int animalAgentTypeId = -1;

	private static int humanoidAgentTypeId = -1;

	public override void ServerInit()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (nonWalkableArea < 0)
		{
			nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
		}
		NavMeshBuildSettings settingsByIndex;
		if (animalAgentTypeId < 0)
		{
			settingsByIndex = NavMesh.GetSettingsByIndex(1);
			animalAgentTypeId = ((NavMeshBuildSettings)(ref settingsByIndex)).agentTypeID;
		}
		if ((Object)(object)NavMeshVolumeAnimals == (Object)null)
		{
			NavMeshVolumeAnimals = ((Component)this).gameObject.AddComponent<NavMeshModifierVolume>();
			NavMeshVolumeAnimals.area = nonWalkableArea;
			NavMeshVolumeAnimals.AddAgentType(animalAgentTypeId);
			NavMeshVolumeAnimals.center = Vector3.zero;
			NavMeshVolumeAnimals.size = Vector3.one;
		}
		if (!canNpcSmash)
		{
			if (humanoidAgentTypeId < 0)
			{
				settingsByIndex = NavMesh.GetSettingsByIndex(0);
				humanoidAgentTypeId = ((NavMeshBuildSettings)(ref settingsByIndex)).agentTypeID;
			}
			if ((Object)(object)NavMeshVolumeHumanoids == (Object)null)
			{
				NavMeshVolumeHumanoids = ((Component)this).gameObject.AddComponent<NavMeshModifierVolume>();
				NavMeshVolumeHumanoids.area = nonWalkableArea;
				NavMeshVolumeHumanoids.AddAgentType(humanoidAgentTypeId);
				NavMeshVolumeHumanoids.center = Vector3.zero;
				NavMeshVolumeHumanoids.size = Vector3.one;
			}
		}
		else if ((Object)(object)NpcTriggerBox == (Object)null)
		{
			NpcTriggerBox = new GameObject("NpcTriggerBox").AddComponent<NPCBarricadeTriggerBox>();
			NpcTriggerBox.Setup(this);
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer && info.WeaponPrefab is BaseMelee && !info.IsProjectile())
		{
			BasePlayer basePlayer = info.Initiator as BasePlayer;
			if (Object.op_Implicit((Object)(object)basePlayer) && reflectDamage > 0f)
			{
				basePlayer.Hurt(reflectDamage * Random.Range(0.75f, 1.25f), DamageType.Stab, this);
				if (reflectEffect.isValid)
				{
					Effect.server.Run(reflectEffect.resourcePath, basePlayer, StringPool.closest, ((Component)this).transform.position, Vector3.up);
				}
			}
		}
		base.OnAttacked(info);
	}
}
