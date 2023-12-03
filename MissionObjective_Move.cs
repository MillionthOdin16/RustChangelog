using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Missions/OBJECTIVES/Move")]
public class MissionObjective_Move : MissionObjective
{
	public string positionName = "default";

	public float distForCompletion = 3f;

	public bool use2D = false;

	public override void ObjectiveStarted(BasePlayer playerFor, int index, BaseMission.MissionInstance instance)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		base.ObjectiveStarted(playerFor, index, instance);
		instance.missionLocation = instance.GetMissionPoint(positionName, playerFor);
		playerFor.MissionDirty();
	}

	public override void Think(int index, BaseMission.MissionInstance instance, BasePlayer assignee, float delta)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.Think(index, instance, assignee, delta);
		if (ShouldThink(index, instance))
		{
			Vector3 missionPoint = instance.GetMissionPoint(positionName, assignee);
			float num = (use2D ? Vector3Ex.Distance2D(missionPoint, ((Component)assignee).transform.position) : Vector3.Distance(missionPoint, ((Component)assignee).transform.position));
			if (num <= distForCompletion)
			{
				CompleteObjective(index, instance, assignee);
				assignee.MissionDirty();
			}
		}
	}
}
