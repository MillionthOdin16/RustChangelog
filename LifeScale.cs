using System;
using UnityEngine;

public class LifeScale : BaseMonoBehaviour
{
	[NonSerialized]
	private bool initialized = false;

	[NonSerialized]
	private Vector3 initialScale;

	public Vector3 finalScale = Vector3.one;

	private Vector3 targetLerpScale = Vector3.zero;

	private Action updateScaleAction;

	protected void Awake()
	{
		updateScaleAction = UpdateScale;
	}

	public void OnEnable()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Init();
		((Component)this).transform.localScale = initialScale;
	}

	public void SetProgress(float progress)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Init();
		targetLerpScale = Vector3.Lerp(initialScale, finalScale, progress);
		((FacepunchBehaviour)this).InvokeRepeating(updateScaleAction, 0f, 0.015f);
	}

	public void Init()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			initialScale = ((Component)this).transform.localScale;
			initialized = true;
		}
	}

	public void UpdateScale()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localScale = Vector3.Lerp(((Component)this).transform.localScale, targetLerpScale, Time.deltaTime);
		if (((Component)this).transform.localScale == targetLerpScale)
		{
			targetLerpScale = Vector3.zero;
			((FacepunchBehaviour)this).CancelInvoke(updateScaleAction);
		}
	}
}
