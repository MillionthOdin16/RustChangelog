using System;
using System.Runtime.CompilerServices;
using ConVar;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraUpdateHook : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static CameraCallback _003C_003E9__5_0;

		public static CameraCallback _003C_003E9__5_1;

		public static CameraCallback _003C_003E9__5_2;

		internal void _003CAwake_003Eb__5_0(Camera args)
		{
			Camera mainCamera = MainCamera.mainCamera;
			LastFrameFOV = ((mainCamera != null) ? mainCamera.fieldOfView : Graphics.fov);
			PreRender?.Invoke();
		}

		internal void _003CAwake_003Eb__5_1(Camera args)
		{
			PostRender?.Invoke();
		}

		internal void _003CAwake_003Eb__5_2(Camera args)
		{
			PreCull?.Invoke();
		}
	}

	public static Action PreCull;

	public static Action PreRender;

	public static Action PostRender;

	public static Action RustCamera_PreRender;

	public static float LastFrameFOV = Graphics.fov;

	private void Awake()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		CameraUpdateHook[] components = ((Component)this).GetComponents<CameraUpdateHook>();
		foreach (CameraUpdateHook cameraUpdateHook in components)
		{
			if ((Object)(object)cameraUpdateHook != (Object)(object)this)
			{
				Object.DestroyImmediate((Object)(object)cameraUpdateHook);
			}
		}
		CameraCallback onPreRender = Camera.onPreRender;
		object obj = _003C_003Ec._003C_003E9__5_0;
		if (obj == null)
		{
			CameraCallback val = delegate
			{
				Camera mainCamera = MainCamera.mainCamera;
				LastFrameFOV = ((mainCamera != null) ? mainCamera.fieldOfView : Graphics.fov);
				PreRender?.Invoke();
			};
			_003C_003Ec._003C_003E9__5_0 = val;
			obj = (object)val;
		}
		Camera.onPreRender = (CameraCallback)Delegate.Combine((Delegate)(object)onPreRender, (Delegate)obj);
		CameraCallback onPostRender = Camera.onPostRender;
		object obj2 = _003C_003Ec._003C_003E9__5_1;
		if (obj2 == null)
		{
			CameraCallback val2 = delegate
			{
				PostRender?.Invoke();
			};
			_003C_003Ec._003C_003E9__5_1 = val2;
			obj2 = (object)val2;
		}
		Camera.onPostRender = (CameraCallback)Delegate.Combine((Delegate)(object)onPostRender, (Delegate)obj2);
		CameraCallback onPreCull = Camera.onPreCull;
		object obj3 = _003C_003Ec._003C_003E9__5_2;
		if (obj3 == null)
		{
			CameraCallback val3 = delegate
			{
				PreCull?.Invoke();
			};
			_003C_003Ec._003C_003E9__5_2 = val3;
			obj3 = (object)val3;
		}
		Camera.onPreCull = (CameraCallback)Delegate.Combine((Delegate)(object)onPreCull, (Delegate)obj3);
	}
}
