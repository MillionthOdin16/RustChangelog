using UnityEngine;

public static class DecorComponentEx
{
	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components, ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		foreach (DecorComponent decorComponent in components)
		{
			if (!decorComponent.isRoot)
			{
				break;
			}
			decorComponent.Apply(ref pos, ref rot, ref scale);
		}
	}

	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pos = transform.position;
		Quaternion rot = transform.rotation;
		Vector3 scale = transform.localScale;
		transform.ApplyDecorComponents(components, ref pos, ref rot, ref scale);
		transform.position = pos;
		transform.rotation = rot;
		transform.localScale = scale;
	}

	public static void ApplyDecorComponentsScaleOnly(this Transform transform, DecorComponent[] components)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pos = transform.position;
		Quaternion rot = transform.rotation;
		Vector3 scale = transform.localScale;
		transform.ApplyDecorComponents(components, ref pos, ref rot, ref scale);
		transform.localScale = scale;
	}
}
