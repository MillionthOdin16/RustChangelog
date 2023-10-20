using UnityEngine;

public class GestureDoor : Door
{
	public GestureConfig OpenGesture;

	public GestureConfig SprintOpenGesture;

	public float SprintAnimationStartDelay = 1f;

	public float NonSprintAnimationStartDelay = 1f;

	protected override void OnPlayerOpenedDoor(BasePlayer p)
	{
		base.OnPlayerOpenedDoor(p);
		if (p.serverInput.IsDown(BUTTON.SPRINT) && (Object)(object)SprintOpenGesture != (Object)null)
		{
			p.Server_StartGesture(SprintOpenGesture);
		}
		else if ((Object)(object)OpenGesture != (Object)null)
		{
			p.Server_StartGesture(OpenGesture);
		}
	}

	protected override bool ShouldDelayOpen(BasePlayer player, out float delay)
	{
		delay = NonSprintAnimationStartDelay;
		if (player.serverInput.IsDown(BUTTON.SPRINT))
		{
			delay = SprintAnimationStartDelay;
		}
		return delay > 0f;
	}
}
