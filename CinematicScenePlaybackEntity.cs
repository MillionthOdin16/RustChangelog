using System;
using UnityEngine;

public class CinematicScenePlaybackEntity : BaseEntity
{
	public Animator RootAnimator;

	public GameObjectRef CinematicUI;

	public float Duration = 10f;

	public GameObject DebugRoot;

	public bool ShowDebugRoot;

	private BasePlayer currentPlayer;

	public void SignalKillPlayer()
	{
		if (base.isServer && (Object)(object)currentPlayer != (Object)null)
		{
			TutorialIsland currentTutorialIsland = currentPlayer.GetCurrentTutorialIsland();
			if ((Object)(object)currentTutorialIsland != (Object)null)
			{
				currentTutorialIsland.OnPlayerCompletedTutorial(currentPlayer);
			}
		}
	}

	public void AssignPlayer(BasePlayer bp)
	{
		currentPlayer = bp;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)Timeout, Duration);
	}

	private void Timeout()
	{
		Kill();
	}
}
