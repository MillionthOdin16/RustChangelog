using UnityEngine;

public class RandomDynamicPrefab : MonoBehaviour, IClientComponent, ILOD
{
	public uint Seed = 0u;

	public float Distance = 100f;

	public float Probability = 0.5f;

	public string ResourceFolder = string.Empty;
}
