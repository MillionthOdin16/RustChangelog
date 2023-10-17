using UnityEngine;

public class TorpedoServerProjectile : ServerProjectile
{
	[Tooltip("Make sure to leave some allowance for waves, which affect the true depth.")]
	[SerializeField]
	private float minWaterDepth = 0.5f;

	[SerializeField]
	private float shallowWaterInaccuracy;

	[SerializeField]
	private float deepWaterInaccuracy;

	[SerializeField]
	private float shallowWaterCutoff = 2f;

	public override bool HasRangeLimit => false;

	protected override int mask => 1237003009;

	public override bool DoMovement()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (!base.DoMovement())
		{
			return false;
		}
		float num = WaterLevel.GetWaterInfo(((Component)this).transform.position, waves: true, volumes: false).surfaceLevel - ((Component)this).transform.position.y;
		if (num < -1f)
		{
			gravityModifier = 1f;
		}
		else if (num <= minWaterDepth)
		{
			Vector3 currentVelocity = base.CurrentVelocity;
			currentVelocity.y = 0f;
			base.CurrentVelocity = currentVelocity;
			gravityModifier = 0.1f;
		}
		else if (num > minWaterDepth + 0.3f && num <= minWaterDepth + 0.7f)
		{
			gravityModifier = -0.1f;
		}
		else
		{
			gravityModifier = Mathf.Clamp(base.CurrentVelocity.y, -0.1f, 0.1f);
		}
		return true;
	}

	public override void InitializeVelocity(Vector3 overrideVel)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		base.InitializeVelocity(overrideVel);
		float num = WaterLevel.GetWaterInfo(((Component)this).transform.position, waves: true, volumes: false).surfaceLevel - ((Component)this).transform.position.y;
		float num2 = Mathf.InverseLerp(shallowWaterCutoff, shallowWaterCutoff + 2f, num);
		float num3 = Mathf.Lerp(shallowWaterInaccuracy, deepWaterInaccuracy, num2);
		initialVelocity = Vector3Ex.GetWithInaccuracy(initialVelocity, num3);
		base.CurrentVelocity = initialVelocity;
	}
}
