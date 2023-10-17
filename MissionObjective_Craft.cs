using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Craft")]
public class MissionObjective_Craft : MissionObjective
{
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition[] targetItems;

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
		if (type != BaseMission.MissionEventType.CRAFT_ITEM || IsCompleted(index, instance) || !CanProgress(index, instance))
		{
			return;
		}
		ItemDefinition[] array = targetItems;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].itemid == payload.IntIdentifier)
			{
				instance.objectiveStatuses[index].progressCurrent += (int)amount;
				if (instance.objectiveStatuses[index].progressCurrent >= (float)targetItemAmount)
				{
					CompleteObjective(index, instance, playerFor);
				}
				playerFor.MissionDirty();
				break;
			}
		}
	}
}
