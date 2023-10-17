using UnityEngine;

public class NPCMissionProvider : NPCTalking, IMissionProvider
{
	public MissionManifest manifest;

	public NetworkableId ProviderID()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return net.ID;
	}

	public Vector3 ProviderPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position;
	}

	public BaseEntity Entity()
	{
		return this;
	}

	public override void OnConversationEnded(BasePlayer player)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		player.ProcessMissionEvent(BaseMission.MissionEventType.CONVERSATION, ProviderID().Value.ToString(), 0f);
		base.OnConversationEnded(player);
	}

	public override void OnConversationStarted(BasePlayer speakingTo)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		speakingTo.ProcessMissionEvent(BaseMission.MissionEventType.CONVERSATION, ProviderID().Value.ToString(), 1f);
		base.OnConversationStarted(speakingTo);
	}

	public bool ContainsSpeech(string speech)
	{
		ConversationData[] array = conversations;
		for (int i = 0; i < array.Length; i++)
		{
			ConversationData.SpeechNode[] speeches = array[i].speeches;
			for (int j = 0; j < speeches.Length; j++)
			{
				if (speeches[j].shortname == speech)
				{
					return true;
				}
			}
		}
		return false;
	}

	public string IntroOverride(string overrideSpeech)
	{
		if (!ContainsSpeech(overrideSpeech))
		{
			return "intro";
		}
		return overrideSpeech;
	}

	public override string GetConversationStartSpeech(BasePlayer player)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		foreach (BaseMission.MissionInstance mission in player.missions)
		{
			if (mission.status == BaseMission.MissionStatus.Active)
			{
				text = IntroOverride("missionactive");
			}
			if (mission.status == BaseMission.MissionStatus.Completed && mission.providerID == ProviderID() && Time.time - mission.endTime < 5f)
			{
				text = IntroOverride("missionreturn");
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			text = base.GetConversationStartSpeech(player);
		}
		return text;
	}

	public override void OnConversationAction(BasePlayer player, string action)
	{
		if (action.Contains("assignmission"))
		{
			int num = action.IndexOf(" ");
			BaseMission fromShortName = MissionManifest.GetFromShortName(action.Substring(num + 1));
			if (Object.op_Implicit((Object)(object)fromShortName))
			{
				BaseMission.AssignMission(player, this, fromShortName);
			}
		}
		base.OnConversationAction(player, action);
	}
}
