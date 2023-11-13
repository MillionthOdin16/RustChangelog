using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicDungeon : BaseEntity, IMissionEntityListener
{
	public Transform exitEntitySpawn;

	public GameObjectRef exitEntity;

	public string exitString;

	public MonumentNavMesh monumentNavMesh;

	private static List<DynamicDungeon> _dungeons = new List<DynamicDungeon>();

	public GameObjectRef portalPrefab;

	public Transform portalSpawnPoint;

	public BasePortal exitPortal;

	public GameObjectRef doorPrefab;

	public Transform doorSpawnPoint;

	public Door doorInstance;

	public static Vector3 nextDungeonPos = Vector3.zero;

	public static Vector3 dungeonStartPoint = Vector3.zero;

	public static float dungeonSpacing = 50f;

	public SpawnGroup[] spawnGroups;

	public bool AutoMergeAIZones = true;

	public static void AddDungeon(DynamicDungeon newDungeon)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		_dungeons.Add(newDungeon);
		Vector3 position = ((Component)newDungeon).transform.position;
		if (position.y >= nextDungeonPos.y)
		{
			nextDungeonPos = position + Vector3.up * dungeonSpacing;
		}
	}

	public static void RemoveDungeon(DynamicDungeon dungeon)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)dungeon).transform.position;
		if (_dungeons.Contains(dungeon))
		{
			_dungeons.Remove(dungeon);
		}
		nextDungeonPos = position;
	}

	public static Vector3 GetNextDungeonPoint()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (nextDungeonPos == Vector3.zero)
		{
			nextDungeonPos = Vector3.one * 700f;
		}
		return nextDungeonPos;
	}

	public IEnumerator UpdateNavMesh()
	{
		Debug.Log((object)"Dungeon Building navmesh");
		yield return ((MonoBehaviour)this).StartCoroutine(monumentNavMesh.UpdateNavMeshAndWait());
		Debug.Log((object)"Dunngeon done!");
	}

	public override void DestroyShared()
	{
		if (base.isServer)
		{
			SpawnGroup[] array = spawnGroups;
			foreach (SpawnGroup spawnGroup in array)
			{
				spawnGroup.Clear();
			}
			if ((Object)(object)exitPortal != (Object)null)
			{
				exitPortal.Kill();
			}
			RemoveDungeon(this);
		}
		base.DestroyShared();
	}

	public override void ServerInit()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		AddDungeon(this);
		if (portalPrefab.isValid)
		{
			exitPortal = ((Component)GameManager.server.CreateEntity(portalPrefab.resourcePath, portalSpawnPoint.position, portalSpawnPoint.rotation)).GetComponent<BasePortal>();
			exitPortal.SetParent(this, worldPositionStays: true);
			exitPortal.Spawn();
		}
		if (doorPrefab.isValid)
		{
			doorInstance = ((Component)GameManager.server.CreateEntity(doorPrefab.resourcePath, doorSpawnPoint.position, doorSpawnPoint.rotation)).GetComponent<Door>();
			doorInstance.SetParent(this, worldPositionStays: true);
			doorInstance.Spawn();
		}
		MergeAIZones();
		((MonoBehaviour)this).StartCoroutine(UpdateNavMesh());
	}

	private void MergeAIZones()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		if (!AutoMergeAIZones)
		{
			return;
		}
		List<AIInformationZone> list = ((Component)this).GetComponentsInChildren<AIInformationZone>().ToList();
		foreach (AIInformationZone item in list)
		{
			item.AddInitialPoints();
		}
		GameObject val = new GameObject("AIZ");
		val.transform.position = ((Component)this).transform.position;
		AIInformationZone aIInformationZone = AIInformationZone.Merge(list, val);
		aIInformationZone.ShouldSleepAI = false;
		val.transform.SetParent(((Component)this).transform);
	}

	public void MissionStarted(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
		foreach (MissionEntity createdEntity in instance.createdEntities)
		{
			BunkerEntrance component = ((Component)createdEntity).GetComponent<BunkerEntrance>();
			if ((Object)(object)component != (Object)null)
			{
				BasePortal portalInstance = component.portalInstance;
				if (Object.op_Implicit((Object)(object)portalInstance))
				{
					portalInstance.targetPortal = exitPortal;
					exitPortal.targetPortal = portalInstance;
					Debug.Log((object)"Dungeon portal linked...");
				}
			}
		}
	}

	public void MissionEnded(BasePlayer assignee, BaseMission.MissionInstance instance)
	{
	}
}
