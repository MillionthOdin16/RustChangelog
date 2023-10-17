using System;
using UnityEngine;

public class BuoyancyPoint : MonoBehaviour
{
	public float buoyancyForce = 10f;

	public float size = 0.1f;

	public float waveScale = 0.2f;

	public float waveFrequency = 1f;

	public bool doSplashEffects = true;

	[NonSerialized]
	public float randomOffset = 0f;

	[NonSerialized]
	public bool wasSubmergedLastFrame = false;

	[NonSerialized]
	public float nexSplashTime = 0f;

	private static readonly Color gizmoColour = new Color(1f, 0f, 0f, 0.25f);

	public void Start()
	{
		randomOffset = Random.Range(0f, 20f);
	}

	public void OnDrawGizmos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = gizmoColour;
		Gizmos.DrawSphere(((Component)this).transform.position, size * 0.5f);
	}
}
