using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;

public class DynamicMouseCursor : MonoBehaviour
{
	public Texture2D RegularCursor;

	public Vector2 RegularCursorPos;

	public Texture2D HoverCursor;

	public Vector2 HoverCursorPos;

	private Texture2D current;

	private void LateUpdate()
	{
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		if (!Cursor.visible)
		{
			return;
		}
		Profiler.BeginSample("GetHoveredItem");
		GameObject val = CurrentlyHoveredItem();
		Profiler.EndSample();
		if ((Object)(object)val != (Object)null)
		{
			TimeWarning val2 = TimeWarning.New("RustControl", 0);
			try
			{
				RustControl componentInParent = val.GetComponentInParent<RustControl>();
				if ((Object)(object)componentInParent != (Object)null && componentInParent.IsDisabled)
				{
					UpdateCursor(RegularCursor, RegularCursorPos);
					return;
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			TimeWarning val3 = TimeWarning.New("ISubmitHandler", 0);
			try
			{
				if (val.GetComponentInParent<ISubmitHandler>() != null)
				{
					UpdateCursor(HoverCursor, HoverCursorPos);
					return;
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			TimeWarning val4 = TimeWarning.New("IPointerDownHandler", 0);
			try
			{
				if (val.GetComponentInParent<IPointerDownHandler>() != null)
				{
					UpdateCursor(HoverCursor, HoverCursorPos);
					return;
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
		}
		TimeWarning val5 = TimeWarning.New("UpdateCursor", 0);
		try
		{
			UpdateCursor(RegularCursor, RegularCursorPos);
		}
		finally
		{
			((IDisposable)val5)?.Dispose();
		}
	}

	private void UpdateCursor(Texture2D cursor, Vector2 offs)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)current == (Object)(object)cursor))
		{
			current = cursor;
			Cursor.SetCursor(cursor, offs, (CursorMode)0);
		}
	}

	private GameObject CurrentlyHoveredItem()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		FpStandaloneInputModule obj = EventSystem.current.currentInputModule as FpStandaloneInputModule;
		object result;
		if (obj == null)
		{
			result = null;
		}
		else
		{
			RaycastResult pointerCurrentRaycast = obj.CurrentData.pointerCurrentRaycast;
			result = ((RaycastResult)(ref pointerCurrentRaycast)).gameObject;
		}
		return (GameObject)result;
	}
}
