using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeIntensity : MonoBehaviour
{
	[Serializable]
	public class DistanceIntensity
	{
		public float distance = 60f;

		public float raiseTo = 0f;

		public bool forceStartMusicInSuppressedMusicZones = false;
	}

	public float raiseTo = 0f;

	public List<DistanceIntensity> distanceIntensities = new List<DistanceIntensity>();

	public float tickInterval = 0.2f;
}
