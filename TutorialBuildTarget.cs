using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class TutorialBuildTarget : MonoBehaviour
{
	public BaseEntityRef TargetPrefab;

	public GameObject VisualObject;

	public Vector3 PhysCheckOffset = Vector3.zero;

	public bool Snap = true;

	public float MaxDistance = 0.5f;

	public BaseMission RequiredMission;

	public int RequiredMissionStage = -1;

	public float MaxValidAngle = 180f;

	public bool IsValid(Construction toConstruct, Construction.Target target, Construction.Placement placement)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		UpdateActive(target.player);
		if (!((Component)this).gameObject.activeInHierarchy)
		{
			return false;
		}
		if (!TargetPrefab.isValid || toConstruct.prefabID != TargetPrefab.Get().prefabID)
		{
			return false;
		}
		if (Vector3.Distance(placement.position, ((Component)this).transform.position) < MaxDistance)
		{
			if (target.socket != null && MaxValidAngle < 180f && Vector3.Angle(((Component)this).transform.forward, placement.rotation * Vector3.forward) > MaxValidAngle)
			{
				return false;
			}
			if (Snap)
			{
				placement.position = ((Component)this).transform.position;
				placement.rotation = ((Component)this).transform.rotation;
			}
			return true;
		}
		return false;
	}

	public void UpdateActive(BasePlayer p)
	{
		if ((Object)(object)p == (Object)null || !p.HasActiveMission())
		{
			((Component)this).gameObject.SetActive(false);
			return;
		}
		BaseMission.MissionInstance activeMissionInstance = p.GetActiveMissionInstance();
		bool flag = activeMissionInstance != null && activeMissionInstance.GetMission() == RequiredMission;
		if (flag && RequiredMissionStage >= 0 && !activeMissionInstance.objectiveStatuses[RequiredMissionStage].started)
		{
			flag = false;
		}
		((Component)this).gameObject.SetActive(flag && !HasTargetBeenBuilt());
	}

	private bool HasTargetBeenBuilt()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!TargetPrefab.isValid)
		{
			return false;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(((Component)this).transform.position + PhysCheckOffset, 0.5f + MaxDistance, list, 1218652417, (QueryTriggerInteraction)2);
		bool flag = false;
		uint prefabID = TargetPrefab.Get().prefabID;
		foreach (BaseEntity item in list)
		{
			if (item.prefabID == prefabID)
			{
				flag = true;
				break;
			}
			if (item is Door)
			{
				foreach (BaseEntity child in item.children)
				{
					if (child.prefabID == prefabID)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				break;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return flag;
	}
}
