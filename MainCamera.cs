using UnityEngine;

[ExecuteInEditMode]
public class MainCamera : RustCamera<MainCamera>
{
	public static Camera mainCamera;

	public static Transform mainCameraTransform;

	public static bool isValid => (Object)(object)mainCamera != (Object)null && ((Behaviour)mainCamera).enabled;

	public static Vector3 velocity { get; private set; }

	public static Vector3 position
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return mainCameraTransform.position;
		}
		set
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			mainCameraTransform.position = value;
		}
	}

	public static Vector3 forward
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return mainCameraTransform.forward;
		}
		set
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (((Vector3)(ref value)).sqrMagnitude > 0f)
			{
				mainCameraTransform.forward = value;
			}
		}
	}

	public static Vector3 right
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return mainCameraTransform.right;
		}
		set
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (((Vector3)(ref value)).sqrMagnitude > 0f)
			{
				mainCameraTransform.right = value;
			}
		}
	}

	public static Vector3 up
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return mainCameraTransform.up;
		}
		set
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if (((Vector3)(ref value)).sqrMagnitude > 0f)
			{
				((Component)mainCamera).transform.up = value;
			}
		}
	}

	public static Quaternion rotation
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return mainCameraTransform.rotation;
		}
		set
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			mainCameraTransform.rotation = value;
		}
	}

	public static Ray Ray => new Ray(position, forward);
}
