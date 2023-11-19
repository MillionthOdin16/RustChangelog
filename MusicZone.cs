using System.Collections.Generic;
using UnityEngine;

public class MusicZone : MonoBehaviour, IClientComponent
{
	public List<MusicTheme> themes;

	public float priority = 0f;

	public bool suppressAutomaticMusic = false;
}
