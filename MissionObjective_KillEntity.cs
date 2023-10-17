using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Extend;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Kill")]
public class MissionObjective_KillEntity : MissionObjective
{
	public BaseEntityRef[] targetEntities;

	public int numToKill;

	public bool shouldUpdateMissionLocation;

	public bool pingTargets;

	private bool isInitalized;

	private uint[] targetPrefabIDs;

	private Func<BaseCombatEntity, bool> searchFilter;

	private void EnsureInitialized()
	{
		if (!isInitalized)
		{
			isInitalized = true;
			targetPrefabIDs = (from e in targetEntities
				where e.isValid
				select e.Get().prefabID).ToArray();
		}
	}

	public override void MissionStarted(int index, BaseMission.MissionInstance instance, BasePlayer forPlayer)
	{
		base.MissionStarted(index, instance, forPlayer);
		instance.objectiveStatuses[index].progressCurrent = 0f;
		instance.objectiveStatuses[index].progressTarget = numToKill;
	}

	public override void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, BaseMission.MissionEventPayload payload, float amount)
	{
		base.ProcessMissionEvent(playerFor, instance, index, type, payload, amount);
		if (type != BaseMission.MissionEventType.KILL_ENTITY || IsCompleted(index, instance) || !CanProgress(index, instance))
		{
			return;
		}
		EnsureInitialized();
		uint[] array = targetPrefabIDs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == payload.UintIdentifier)
			{
				instance.objectiveStatuses[index].progressCurrent += (int)amount;
				if (instance.objectiveStatuses[index].progressCurrent >= (float)numToKill)
				{
					CompleteObjective(index, instance, playerFor);
				}
				playerFor.MissionDirty();
				break;
			}
		}
	}

	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		base.Think(index, instance, assignee, delta);
		if (!shouldUpdateMissionLocation || !IsStarted(index, instance))
		{
			return;
		}
		ref RealTimeSince sinceLastThink = ref instance.objectiveStatuses[index].sinceLastThink;
		if (RealTimeSince.op_Implicit(sinceLastThink) < 1f)
		{
			return;
		}
		EnsureInitialized();
		sinceLastThink = RealTimeSince.op_Implicit(0f);
		if (searchFilter == null)
		{
			searchFilter = (BaseCombatEntity e) => List.TryFindWith<uint, uint>((IReadOnlyCollection<uint>)(object)targetPrefabIDs, (Func<uint, uint>)((uint id) => id), e.prefabID, (IEqualityComparer<uint>)null).HasValue && e.IsAlive();
		}
		if (TryFindNearby(((Component)assignee).transform.position, searchFilter, out var entity, pingTargets ? 100f : 20f))
		{
			instance.missionLocation = ((Component)entity).transform.position;
			assignee.MissionDirty();
			if (pingTargets)
			{
				assignee.RegisterPingedEntity(entity, BasePlayer.PingType.Hostile);
			}
		}
	}
}
