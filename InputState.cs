using UnityEngine;

public class InputState
{
	public InputMessage current = new InputMessage
	{
		ShouldPool = false
	};

	public InputMessage previous = new InputMessage
	{
		ShouldPool = false
	};

	private int SwallowedButtons;

	public bool IsDown(BUTTON btn)
	{
		if (current == null)
		{
			return false;
		}
		if (((uint)SwallowedButtons & (uint)btn) == (uint)btn)
		{
			return false;
		}
		return ((uint)current.buttons & (uint)btn) == (uint)btn;
	}

	public bool WasDown(BUTTON btn)
	{
		if (previous == null)
		{
			return false;
		}
		return ((uint)previous.buttons & (uint)btn) == (uint)btn;
	}

	public bool IsAnyDown()
	{
		if (current == null)
		{
			return false;
		}
		return (float)(current.buttons & ~SwallowedButtons) > 0f;
	}

	public bool WasJustPressed(BUTTON btn)
	{
		if (IsDown(btn))
		{
			return !WasDown(btn);
		}
		return false;
	}

	public bool WasJustReleased(BUTTON btn)
	{
		if (!IsDown(btn))
		{
			return WasDown(btn);
		}
		return false;
	}

	public void SwallowButton(BUTTON btn)
	{
		if (current != null)
		{
			SwallowedButtons |= (int)btn;
		}
	}

	public Quaternion AimAngle()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (current == null)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler(current.aimAngles);
	}

	public Vector3 MouseDelta()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (current == null)
		{
			return Vector3.zero;
		}
		return current.mouseDelta;
	}

	public void Flip(InputMessage newcurrent)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		SwallowedButtons = 0;
		previous.aimAngles = current.aimAngles;
		previous.buttons = current.buttons;
		previous.mouseDelta = current.mouseDelta;
		current.aimAngles = newcurrent.aimAngles;
		current.buttons = newcurrent.buttons;
		current.mouseDelta = newcurrent.mouseDelta;
	}

	public void Clear()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		current.buttons = 0;
		previous.buttons = 0;
		current.mouseDelta = Vector3.zero;
		SwallowedButtons = 0;
	}
}
