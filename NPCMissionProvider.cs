using UnityEngine;

public class NPCMissionProvider : NPCTalking, IMissionProvider
{
	public MissionManifest manifest;

	public NetworkableId ProviderID()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return net.ID;
	}

	public Vector3 ProviderPosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position;
	}

	public BaseEntity Entity()
	{
		return this;
	}

	public override void OnConversationEnded(BasePlayer player)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		player.ProcessMissionEvent(BaseMission.MissionEventType.CONVERSATION, ProviderID().Value.ToString(), 0f);
		base.OnConversationEnded(player);
	}

	public override void OnConversationStarted(BasePlayer speakingTo)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		speakingTo.ProcessMissionEvent(BaseMission.MissionEventType.CONVERSATION, ProviderID().Value.ToString(), 1f);
		base.OnConversationStarted(speakingTo);
	}

	public bool ContainsSpeech(string speech)
	{
		ConversationData[] array = conversations;
		foreach (ConversationData conversationData in array)
		{
			ConversationData.SpeechNode[] speeches = conversationData.speeches;
			foreach (ConversationData.SpeechNode speechNode in speeches)
			{
				if (speechNode.shortname == speech)
				{
					return true;
				}
			}
		}
		return false;
	}

	public string IntroOverride(string overrideSpeech)
	{
		return ContainsSpeech(overrideSpeech) ? overrideSpeech : "intro";
	}

	public override string GetConversationStartSpeech(BasePlayer player)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		foreach (BaseMission.MissionInstance mission in player.missions)
		{
			if (mission.status == BaseMission.MissionStatus.Active)
			{
				text = IntroOverride("missionactive");
			}
			if (mission.status == BaseMission.MissionStatus.Completed && mission.providerID == ProviderID())
			{
				float num = Time.time - mission.endTime;
				if (num < 5f)
				{
					text = IntroOverride("missionreturn");
				}
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
			string shortname = action.Substring(num + 1);
			BaseMission fromShortName = MissionManifest.GetFromShortName(shortname);
			if (Object.op_Implicit((Object)(object)fromShortName))
			{
				BaseMission.AssignMission(player, this, fromShortName);
			}
		}
		base.OnConversationAction(player, action);
	}
}
