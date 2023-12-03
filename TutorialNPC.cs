using System;
using Network;
using UnityEngine;

public class TutorialNPC : NPCMissionProvider
{
	public ConversationData StartConversation;

	public ConversationData MissionInProgressConversation;

	public ConversationData ForageCompleteConversation;

	public ConversationData FirstBuildCompleteConversation;

	public ConversationData SecondBuildCompleteConversation;

	public ConversationData PrepareForCombatConversation;

	public ConversationData BuildKayakConversation;

	public ConversationData SetSailConversation;

	public GestureConfig LoopingGesture;

	public GameObjectRef BearRoarSfx;

	public Transform BearRoarSpawnPos;

	private const uint FORAGE_MISSION = 2265941643u;

	private const uint BUILD1_MISSION = 1726435040u;

	private const uint BUILD2_MISSION = 1928576498u;

	private const uint COOK_MISSION = 3432877204u;

	private const uint KILL_BEAR_MISSION = 3396482113u;

	private const uint CRAFT_KAYAK_MISSION = 3197637569u;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("TutorialNPC.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	protected override bool CanTalkTo(BasePlayer bp)
	{
		if (base.CanTalkTo(bp))
		{
			return !bp.HasPendingFollowupMission;
		}
		return false;
	}

	public override ConversationData GetConversationFor(BasePlayer player)
	{
		if (player.HasActiveMission())
		{
			return MissionInProgressConversation;
		}
		if (player.HasCompletedMission(3197637569u))
		{
			return SetSailConversation;
		}
		if (player.HasCompletedMission(3396482113u))
		{
			return BuildKayakConversation;
		}
		if (player.HasCompletedMission(3432877204u))
		{
			return PrepareForCombatConversation;
		}
		if (player.HasCompletedMission(1928576498u))
		{
			return SecondBuildCompleteConversation;
		}
		if (player.HasCompletedMission(1726435040u))
		{
			return FirstBuildCompleteConversation;
		}
		if (player.HasCompletedMission(2265941643u))
		{
			return ForageCompleteConversation;
		}
		return StartConversation;
	}

	public override void OnConversationAction(BasePlayer player, string action)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		base.OnConversationAction(player, action);
		if (action == "openhelp")
		{
			ClientRPCPlayer(null, player, "Client_OpenHelp");
		}
		else if (action == "playbearsfx")
		{
			Effect.server.Run(BearRoarSfx.resourcePath, BearRoarSpawnPos.position);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if ((Object)(object)LoopingGesture != (Object)null)
		{
			Server_StartGesture(LoopingGesture);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		EndSleeping();
		if ((Object)(object)LoopingGesture != (Object)null)
		{
			Server_StartGesture(LoopingGesture);
		}
	}

	public override void GreetPlayer(BasePlayer player)
	{
	}

	public override void Greeting()
	{
	}
}
