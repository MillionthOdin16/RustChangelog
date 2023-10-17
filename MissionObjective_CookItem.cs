using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/CookItem")]
public class MissionObjective_CookItem : MissionObjective
{
	[ItemSelector(ItemCategory.All)]
	[Tooltip("The cooked result that this objective is looking for (eg cooked chicken, not raw)")]
	public ItemDefinition targetItem;

	public int targetItemAmount;

	public override void MissionStarted(int index, BaseMission.MissionInstance instance, BasePlayer forPlayer)
	{
		base.MissionStarted(index, instance, forPlayer);
		instance.objectiveStatuses[index].progressCurrent = 0f;
		instance.objectiveStatuses[index].progressTarget = targetItemAmount;
	}

	public override void ProcessMissionEvent(BasePlayer playerFor, BaseMission.MissionInstance instance, int index, BaseMission.MissionEventType type, BaseMission.MissionEventPayload payload, float amount)
	{
		base.ProcessMissionEvent(playerFor, instance, index, type, payload, amount);
		if (type == BaseMission.MissionEventType.COOK && !IsCompleted(index, instance) && CanProgress(index, instance) && targetItem.itemid == payload.IntIdentifier)
		{
			instance.objectiveStatuses[index].progressCurrent += (int)amount;
			if (instance.objectiveStatuses[index].progressCurrent >= (float)targetItemAmount)
			{
				CompleteObjective(index, instance, playerFor);
			}
			playerFor.MissionDirty();
		}
	}
}
