using UnityEngine;

public class NetworkSleep : MonoBehaviour
{
	public static int totalBehavioursDisabled = 0;

	public static int totalCollidersDisabled = 0;

	public Behaviour[] behaviours;

	public Collider[] colliders;

	internal int BehavioursDisabled = 0;

	internal int CollidersDisabled = 0;
}
