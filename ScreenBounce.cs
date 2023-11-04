using UnityEngine;

public class ScreenBounce : BaseScreenShake
{
	public AnimationCurve bounceScale;

	public AnimationCurve bounceSpeed;

	public AnimationCurve bounceViewmodel;

	private float bounceTime = 0f;

	private Vector3 bounceVelocity = Vector3.zero;

	public override void Setup()
	{
		bounceTime = Random.Range(0f, 1000f);
	}

	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		bounceTime += Time.deltaTime * bounceSpeed.Evaluate(delta);
		float num = bounceScale.Evaluate(delta) * 0.1f;
		bounceVelocity.x = Mathf.Sin(bounceTime * 20f) * num;
		bounceVelocity.y = Mathf.Cos(bounceTime * 25f) * num;
		bounceVelocity.z = 0f;
		Vector3 zero = Vector3.zero;
		zero += bounceVelocity.x * cam.right;
		zero += bounceVelocity.y * cam.up;
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
