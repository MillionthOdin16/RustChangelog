using System;
using UnityEngine;

public class FishingRodViewmodel : MonoBehaviour
{
	[Serializable]
	public struct FishViewmodel
	{
		public ItemDefinition Item;

		public GameObject Root;
	}

	public Transform PitchTransform = null;

	public Transform YawTransform = null;

	public float YawLerpSpeed = 1f;

	public float PitchLerpSpeed = 1f;

	public Transform LineRendererStartPos;

	public ParticleSystem[] StrainParticles;

	public bool ApplyTransformRotation = true;

	public GameObject CatchRoot = null;

	public Transform CatchLinePoint = null;

	public FishViewmodel[] FishViewmodels;

	public float ShakeMaxScale = 0.1f;
}
