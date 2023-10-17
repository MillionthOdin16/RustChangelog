using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Ignite Oven")]
public class MissionObjective_IgniteOven : MissionObjective
{
	public PrefabIdRef TargetOven;

	public override void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, BaseMission.MissionEventPayload payload, float amount)
	{
		base.ProcessMissionEvent(playerFor, instance, index, type, payload, amount);
		if (type == BaseMission.MissionEventType.STARTOVEN && !IsCompleted(index, instance) && CanProgress(index, instance))
		{
			if (TargetOven.Equals(payload.UintIdentifier))
			{
				CompleteObjective(index, instance, playerFor);
			}
			playerFor.MissionDirty();
		}
	}
}
