using UnityEngine;

public class AnimatedScreenShake : BaseScreenShake
{
	public AnimationClip TargetClip;

	[ReadOnly]
	public AnimationCurve rotX;

	[ReadOnly]
	public AnimationCurve rotY;

	[ReadOnly]
	public AnimationCurve rotZ;

	[ReadOnly]
	public AnimationCurve posX;

	[ReadOnly]
	public AnimationCurve posY;

	[ReadOnly]
	public AnimationCurve posZ;

	private const float VALID_RANGE = 0.1f;

	private bool canPlay;

	public override void Setup()
	{
	}

	public override void Run(float delta, ref CachedTransform<Camera> cam, ref CachedTransform<BaseViewModel> vm)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (canPlay)
		{
			float num = rotX.Evaluate(delta);
			float num2 = rotY.Evaluate(delta);
			float num3 = rotZ.Evaluate(delta);
			float num4 = posX.Evaluate(delta);
			float num5 = posY.Evaluate(delta);
			float num6 = posZ.Evaluate(delta);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(num, num2, num3);
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(num4, num5, num6);
			if ((bool)cam)
			{
				cam.rotation = Quaternion.Euler(((Quaternion)(ref cam.rotation)).eulerAngles + val);
				ref Vector3 position = ref cam.position;
				position += val2;
			}
			if ((bool)vm)
			{
				vm.rotation = Quaternion.Euler(((Quaternion)(ref vm.rotation)).eulerAngles + val);
				ref Vector3 position2 = ref vm.position;
				position2 += val2;
			}
		}
	}
}
