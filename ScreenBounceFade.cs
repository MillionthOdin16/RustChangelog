using UnityEngine;

public class ScreenBounceFade : BaseScreenShake
{
	public AnimationCurve bounceScale;

	public AnimationCurve bounceSpeed;

	public AnimationCurve bounceViewmodel;

	public AnimationCurve distanceFalloff;

	public AnimationCurve timeFalloff;

	private float bounceTime = 0f;

	private Vector3 bounceVelocity = Vector3.zero;

	public float maxDistance = 10f;

	public float scale = 1f;

	public override void Setup()
	{
		bounceTime = Random.Range(0f, 1000f);
	}

	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(cam.position, ((Component)this).transform.position);
		float num2 = 1f - Mathf.InverseLerp(0f, maxDistance, num);
		bounceTime += Time.deltaTime * bounceSpeed.Evaluate(delta);
		float num3 = distanceFalloff.Evaluate(num2);
		float num4 = bounceScale.Evaluate(delta) * 0.1f * num3 * scale * timeFalloff.Evaluate(delta);
		bounceVelocity.x = Mathf.Sin(bounceTime * 20f) * num4;
		bounceVelocity.y = Mathf.Cos(bounceTime * 25f) * num4;
		bounceVelocity.z = 0f;
		Vector3 zero = Vector3.zero;
		zero += bounceVelocity.x * cam.right;
		zero += bounceVelocity.y * cam.up;
		zero *= num2;
		if ((bool)cam)
		{
			ref Vector3 position = ref cam.position;
			position += zero;
		}
		if ((bool)vm)
		{
			ref Vector3 position2 = ref vm.position;
			position2 += zero * -1f * bounceViewmodel.Evaluate(delta);
		}
	}
}
