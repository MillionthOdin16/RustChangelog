using UnityEngine;
using UnityEngine.UI;

public class MissionMapMarker : MonoBehaviour
{
	public Image Icon = null;

	public Tooltip TooltipComponent = null;

	public void Populate(BaseMission.MissionInstance mission)
	{
		BaseMission mission2 = mission.GetMission();
		Icon.sprite = mission2.icon;
		TooltipComponent.token = mission2.missionName.token;
		TooltipComponent.Text = mission2.missionName.english;
	}
}
