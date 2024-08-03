using UnityEngine;

public class ScreenRotate : BaseScreenShake
{
	public AnimationCurve Pitch;

	public AnimationCurve Yaw;

	public AnimationCurve Roll;

	public AnimationCurve ViewmodelEffect;

	public float scale = 1f;

	public bool useViewModelEffect = true;

	public override void Setup()
	{
	}

	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.zero;
		zero.x = Pitch.Evaluate(delta);
		zero.y = Yaw.Evaluate(delta);
		zero.z = Roll.Evaluate(delta);
		if ((bool)cam)
		{
			ref Quaternion rotation = ref cam.rotation;
			rotation *= Quaternion.Euler(zero * scale);
		}
		if ((bool)vm && useViewModelEffect)
		{
			ref Quaternion rotation2 = ref vm.rotation;
			rotation2 *= Quaternion.Euler(zero * scale * -1f * (1f - ViewmodelEffect.Evaluate(delta)));
		}
	}
}
