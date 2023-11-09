using UnityEngine;

public class CursorManager : SingletonComponent<CursorManager>
{
	private static int iHoldOpen;

	private static int iPreviousOpen;

	private static float lastTimeVisible;

	private static float lastTimeInvisible;

	private void Update()
	{
		if (!((Object)(object)SingletonComponent<CursorManager>.Instance != (Object)(object)this))
		{
			if (iHoldOpen == 0 && iPreviousOpen == 0)
			{
				SwitchToGame();
			}
			else
			{
				SwitchToUI();
			}
			iPreviousOpen = iHoldOpen;
			iHoldOpen = 0;
		}
	}

	public void SwitchToGame()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)Cursor.lockState != 1)
		{
			Cursor.lockState = (CursorLockMode)1;
		}
		if (Cursor.visible)
		{
			Cursor.visible = false;
		}
		lastTimeInvisible = Time.time;
	}

	private void SwitchToUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Cursor.lockState != 0)
		{
			Cursor.lockState = (CursorLockMode)0;
		}
		if (!Cursor.visible)
		{
			Cursor.visible = true;
		}
		lastTimeVisible = Time.time;
	}

	public static void HoldOpen(bool cursorVisible = false)
	{
		iHoldOpen++;
	}

	public static bool WasVisible(float deltaTime)
	{
		return Time.time - lastTimeVisible <= deltaTime;
	}

	public static bool WasInvisible(float deltaTime)
	{
		return Time.time - lastTimeInvisible <= deltaTime;
	}
}
