using UnityEngine;

public class ProceduralDungeonCell : BaseMonoBehaviour
{
	public bool north;

	public bool east;

	public bool south;

	public bool west;

	public bool entrance = false;

	public bool hasSpawn = false;

	public Transform exitPointHack;

	public SpawnGroup[] spawnGroups;

	public MeshRenderer[] mapRenderers;

	public void Awake()
	{
		spawnGroups = ((Component)this).GetComponentsInChildren<SpawnGroup>();
	}
}
