using UnityEngine;

public class RandomDynamicObject : MonoBehaviour, IClientComponent, ILOD
{
	public uint Seed = 0u;

	public float Distance = 100f;

	public float Probability = 0.5f;

	public GameObject[] Candidates = null;
}
