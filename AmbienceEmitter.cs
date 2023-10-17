using System;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceEmitter : MonoBehaviour, IClientComponent, IComparable<AmbienceEmitter>
{
	public AmbienceDefinitionList baseAmbience;

	public AmbienceDefinitionList stings;

	public bool isStatic = true;

	public bool followCamera = false;

	public bool isBaseEmitter = false;

	public bool active = false;

	public float cameraDistanceSq = float.PositiveInfinity;

	public BoundingSphere boundingSphere;

	public float crossfadeTime = 2f;

	public Dictionary<AmbienceDefinition, float> nextStingTime = new Dictionary<AmbienceDefinition, float>();

	public float deactivateTime = float.PositiveInfinity;

	public bool playUnderwater = true;

	public bool playAbovewater = true;

	public Enum currentTopology { get; private set; }

	public Enum currentBiome { get; private set; }

	public int CompareTo(AmbienceEmitter other)
	{
		return cameraDistanceSq.CompareTo(other.cameraDistanceSq);
	}
}
