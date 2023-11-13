using ConVar;
using UnityEngine;

public class JunkpileNPCSpawner : NPCSpawner
{
	[Header("Junkpile NPC Spawner")]
	public bool UseSpawnChance = false;

	protected override void Spawn(int numToSpawn)
	{
		if (!UseSpawnChance || !(Random.value > AI.npc_junkpilespawn_chance))
		{
			base.Spawn(numToSpawn);
		}
	}
}
