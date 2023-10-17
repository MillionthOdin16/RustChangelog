using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[DisallowMultipleComponent]
public class CameraUpdateHook : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static CameraCallback _003C_003E9__4_0;

		public static CameraCallback _003C_003E9__4_1;

		public static CameraCallback _003C_003E9__4_2;

		internal void _003CAwake_003Eb__4_0(Camera args)
		{
			PreRender?.Invoke();
		}

		internal void _003CAwake_003Eb__4_1(Camera args)
		{
			PostRender?.Invoke();
		}

		internal void _003CAwake_003Eb__4_2(Camera args)
		{
			PreCull?.Invoke();
		}
	}

	public static Action PreCull;

	public static Action PreRender;

	public static Action PostRender;

	public static Action RustCamera_PreRender;

	private void Awake()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		CameraUpdateHook[] components = ((Component)this).GetComponents<CameraUpdateHook>();
		CameraUpdateHook[] array = components;
		foreach (CameraUpdateHook cameraUpdateHook in array)
		{
			if ((Object)(object)cameraUpdateHook != (Object)(object)this)
			{
				Object.DestroyImmediate((Object)(object)cameraUpdateHook);
			}
		}
		CameraCallback onPreRender = Camera.onPreRender;
		object obj = _003C_003Ec._003C_003E9__4_0;
		if (obj == null)
		{
			CameraCallback val = delegate
			{
				PreRender?.Invoke();
			};
			_003C_003Ec._003C_003E9__4_0 = val;
			obj = (object)val;
		}
		Camera.onPreRender = (CameraCallback)Delegate.Combine((Delegate)(object)onPreRender, (Delegate)obj);
		CameraCallback onPostRender = Camera.onPostRender;
		object obj2 = _003C_003Ec._003C_003E9__4_1;
		if (obj2 == null)
		{
			CameraCallback val2 = delegate
			{
				PostRender?.Invoke();
			};
			_003C_003Ec._003C_003E9__4_1 = val2;
			obj2 = (object)val2;
		}
		Camera.onPostRender = (CameraCallback)Delegate.Combine((Delegate)(object)onPostRender, (Delegate)obj2);
		CameraCallback onPreCull = Camera.onPreCull;
		object obj3 = _003C_003Ec._003C_003E9__4_2;
		if (obj3 == null)
		{
			CameraCallback val3 = delegate
			{
				PreCull?.Invoke();
			};
			_003C_003Ec._003C_003E9__4_2 = val3;
			obj3 = (object)val3;
		}
		Camera.onPreCull = (CameraCallback)Delegate.Combine((Delegate)(object)onPreCull, (Delegate)obj3);
	}
}
