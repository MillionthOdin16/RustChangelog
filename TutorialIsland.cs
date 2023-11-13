using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Network.Visibility;
using ProtoBuf;
using UnityEngine;

public class TutorialIsland : BaseEntity, IEntityPingSource
{
	public struct IslandBounds
	{
		public OBB WorldBounds;

		public uint Id;

		public bool Contains(Vector3 pos)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((OBB)(ref WorldBounds)).Contains(pos);
		}
	}

	public EntityRef<BasePlayer> ForPlayer;

	public Transform InitialSpawnPoint;

	public int SpawnLocationIndex;

	public TutorialNPC StartTutorialNPC;

	public TutorialContainer TutorialContainer;

	public MonumentNavMesh MonumentNavMesh;

	public TimeChange OnStartTimeChange;

	private TutorialBuildTarget[] buildTargets;

	public FoliageGridBaked FoliageGrid;

	public MeshTerrainRoot MeshTerrain;

	public Transform KayakPoint;

	[Header("Debugging")]
	public BaseMission TestMission;

	public static ListHashSet<IslandBounds> BoundsListServer = new ListHashSet<IslandBounds>(8);

	public static float TutorialBoundsSize = 400f;

	[ServerVar(Saved = true)]
	public static bool SpawnTutorialIslandForNewPlayer = true;

	private static ListHashSet<TutorialIsland> ActiveIslandsServer = new ListHashSet<TutorialIsland>(8);

	[ServerVar(Saved = true)]
	public static bool EnforceTrespassChecks = true;

	private const string TutorialIslandAssetPath = "assets/prefabs/missions/tutorialisland/tutorialisland.prefab";

	private static float _tutorialWorldStart = 0f;

	public static Bounds WorldBoundsMinusTutorialIslands;

	private static List<Vector3> islandSpawnLocations;

	private static List<int> freeIslandLocations;

	private float disconnectedDuration;

	private bool readyToStartConversation;

	private float tickRate = 1f;

	public float CurrentIslandTime { get; set; }

	public static float TutorialWorldStart
	{
		get
		{
			if (_tutorialWorldStart <= 0f)
			{
				_tutorialWorldStart = ValidBounds.GetMaximumPoint() - TutorialBoundsSize;
			}
			return _tutorialWorldStart;
		}
	}

	public static float TutorialWorldNetworkThreshold => TutorialWorldStart - TutorialBoundsSize;

	public float DisconnectTimeOutDuration
	{
		get
		{
			if (AvailableIslandCount() > 0)
			{
				return 900f;
			}
			return 300f;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("TutorialIsland.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public static ListHashSet<TutorialIsland> GetTutorialList(bool isServer)
	{
		if (isServer)
		{
			return ActiveIslandsServer;
		}
		return null;
	}

	public static uint GetTutorialGroupId(int index)
	{
		return (uint)(2 + index);
	}

	public static void GenerateIslandSpawnPoints(bool loadingSave = false)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (islandSpawnLocations == null || islandSpawnLocations.Count <= 0)
		{
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(0f - ValidBounds.GetMaximumPointTutorial(), 0f, 0f - ValidBounds.GetMaximumPointTutorial());
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(ValidBounds.GetMaximumPointTutorial(), 0f, ValidBounds.GetMaximumPointTutorial());
			Vector3 cellSize = default(Vector3);
			((Vector3)(ref cellSize))._002Ector(200f, 0f, 200f);
			islandSpawnLocations = TutorialIslandSpawner.GetEdgeSpawnPoints(val, val2 - val, cellSize, 1, out WorldBoundsMinusTutorialIslands);
			freeIslandLocations = new List<int>();
			for (int i = 0; i < islandSpawnLocations.Count; i++)
			{
				freeIslandLocations.Add(i);
			}
		}
	}

	public static Group GetTutorialGroup(int index)
	{
		return Net.sv.visibility.Get((uint)(BaseNetworkable.LimboNetworkGroup.ID + 1 + index));
	}

	public static int AvailableIslandCount()
	{
		return freeIslandLocations.Count;
	}

	public static bool ShouldPlayerResumeTutorial(BasePlayer player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		return player.IsInTutorial;
	}

	public static bool ShouldPlayerBeAskedToStartTutorial(BasePlayer player)
	{
		if (player.IsNpc || player.IsBot)
		{
			return false;
		}
		if (player.IsInTutorial)
		{
			return false;
		}
		if (!SpawnTutorialIslandForNewPlayer)
		{
			return false;
		}
		bool infoBool = player.GetInfoBool("client.hasdeclinedtutorial", defaultVal: false);
		if (!player.GetInfoBool("client.hascompletedtutorial", defaultVal: false))
		{
			return !infoBool;
		}
		return false;
	}

	public static TutorialIsland RestoreOrCreateIslandForPlayer(BasePlayer player)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (player.IsNpc || player.IsBot)
		{
			return null;
		}
		bool flag = !player.HasPlayerFlag(BasePlayer.PlayerFlags.IsInTutorial);
		TutorialIsland tutorialIsland = (flag ? CreateIslandForPlayer(player) : player.GetCurrentTutorialIsland());
		if ((Object)(object)tutorialIsland == (Object)null)
		{
			Debug.Log((object)"Null tutorial island. Do something to handle this.");
			return null;
		}
		tutorialIsland.UpdateNetworkGroup();
		player.SetPlayerFlag(BasePlayer.PlayerFlags.IsInTutorial, b: true);
		if (flag)
		{
			player.ClientRPCPlayer(null, player, "OnTutorialStarted");
			tutorialIsland.CurrentIslandTime = TimeChange.Apply(tutorialIsland.OnStartTimeChange.value, tutorialIsland.OnStartTimeChange, player);
			player.Teleport(tutorialIsland.InitialSpawnPoint.position);
			player.ForceUpdateTriggers();
			player.ClientRPCPlayer<Vector3>(null, player, "ForceViewAnglesTo", tutorialIsland.InitialSpawnPoint.rotation * Vector3.forward);
			player.OnStartedTutorial();
		}
		else
		{
			player.net.SwitchGroup(BaseNetworkable.LimboNetworkGroup);
			player.UpdateNetworkGroup();
			foreach (BaseEntity child in tutorialIsland.children)
			{
				if (child is TutorialContainer tutorialContainer)
				{
					tutorialIsland.TutorialContainer = tutorialContainer;
				}
			}
			player.ClientRPCPlayer(null, player, "SetLocalTimeOverride", tutorialIsland.CurrentIslandTime);
		}
		player.UpdateNetworkGroup();
		player.SendNetworkUpdateImmediate();
		tutorialIsland.TestMission = null;
		if (flag)
		{
			if (tutorialIsland.TestMission == null)
			{
				((FacepunchBehaviour)SingletonComponent<InvokeHandler>.Instance).Invoke((Action)tutorialIsland.StartInitialConversation, 1.5f);
			}
			else
			{
				Debug.LogWarning((object)"Starting test mission instead of initial conversation, clear TestMission field to test actual tutorial");
			}
			if (tutorialIsland.TestMission != null)
			{
				BaseMission.AssignMission(player, tutorialIsland.StartTutorialNPC, tutorialIsland.TestMission);
			}
		}
		Debug.Log((object)(player.displayName + " is being placed on a tutorial island"));
		return tutorialIsland;
	}

	private static TutorialIsland CreateIslandForPlayer(BasePlayer player)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		Vector3 worldPos;
		Quaternion worldRot;
		int unusedTutorialIslandLocationRotation = GetUnusedTutorialIslandLocationRotation(out worldPos, out worldRot);
		if (unusedTutorialIslandLocationRotation == -1)
		{
			return null;
		}
		Group tutorialGroup = GetTutorialGroup(unusedTutorialIslandLocationRotation);
		OBB val = new OBB(worldPos, worldRot, new Bounds(new Vector3(0f, 25f, 0f), new Vector3(400f, 80f, 400f)));
		tutorialGroup.bounds = ((OBB)(ref val)).ToBounds();
		tutorialGroup.restricted = true;
		TutorialIsland tutorialIsland = GameManager.server.CreateEntity("assets/prefabs/missions/tutorialisland/tutorialisland.prefab", worldPos, worldRot) as TutorialIsland;
		tutorialIsland.SpawnLocationIndex = unusedTutorialIslandLocationRotation;
		tutorialIsland.GenerateNavMesh();
		ActiveIslandsServer.Add(tutorialIsland);
		AddIslandBounds(tutorialIsland.WorldSpaceBounds(), tutorialGroup.ID, isServer: true);
		tutorialIsland.ForPlayer.Set(player);
		tutorialIsland.Spawn();
		return tutorialIsland;
	}

	private static int GetUnusedTutorialIslandLocationRotation(out Vector3 worldPos, out Quaternion worldRot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		worldRot = Quaternion.identity;
		worldPos = Vector3.zero;
		if (AvailableIslandCount() == 0)
		{
			return -1;
		}
		int num = freeIslandLocations[0];
		worldPos = islandSpawnLocations[num];
		freeIslandLocations.RemoveAt(0);
		float height = TerrainMeta.HeightMap.GetHeight(worldPos);
		if (worldPos.y < height)
		{
			worldPos.y = height;
		}
		return num;
	}

	public static void AddIslandFromSave(TutorialIsland island)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)("Island being added! Location index: " + island.SpawnLocationIndex));
		if (ActiveIslandsServer.Contains(island))
		{
			Debug.Log((object)"Warning, attempting to add duplicate Island!");
			return;
		}
		ActiveIslandsServer.Add(island);
		AddIslandBounds(island.WorldSpaceBounds(), GetTutorialGroupId(island.SpawnLocationIndex), isServer: true);
		freeIslandLocations.Remove(island.SpawnLocationIndex);
		Debug.Log((object)("Free locations remaining: " + freeIslandLocations.Count + ". Next Index: " + freeIslandLocations[0]));
		island.GenerateNavMesh();
	}

	public void GenerateNavMesh()
	{
		if (!((Object)(object)MonumentNavMesh == (Object)null))
		{
			((MonoBehaviour)this).StartCoroutine(UpdateNavMesh());
		}
	}

	public IEnumerator UpdateNavMesh()
	{
		Debug.Log((object)"Tutorial island navmesh building...");
		yield return ((MonoBehaviour)this).StartCoroutine(MonumentNavMesh.UpdateNavMeshAndWait());
		Debug.Log((object)"Tutorial island navmesh finished.");
	}

	private void StartInitialConversation()
	{
		BasePlayer basePlayer = ForPlayer.Get(base.isServer);
		if ((Object)(object)basePlayer != (Object)null && (basePlayer.IsSleeping() || basePlayer.IsDucked()))
		{
			((FacepunchBehaviour)this).Invoke((Action)StartInitialConversation, 0.1f);
		}
		else if (!readyToStartConversation)
		{
			readyToStartConversation = true;
			((FacepunchBehaviour)this).Invoke((Action)StartInitialConversation, 0.5f);
		}
		else
		{
			StartTutorialNPC.Server_BeginTalking(ForPlayer.Get(base.isServer));
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)InitSpawnGroups, 1f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)Tick, tickRate, tickRate, 0.1f);
	}

	private void InitSpawnGroups()
	{
		List<SpawnGroup> list = Pool.GetList<SpawnGroup>();
		((Component)this).gameObject.GetComponentsInChildren<SpawnGroup>(list);
		foreach (SpawnGroup item in list)
		{
			if ((Object)(object)item != (Object)null)
			{
				item.Spawn();
			}
		}
		Pool.FreeList<SpawnGroup>(ref list);
	}

	public void OnPlayerBuiltConstruction(BasePlayer player)
	{
		ClientRPCPlayer(null, player, "ClientOnPlayerBuiltConstruction");
	}

	public override void Save(SaveInfo info)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (info.msg.TutorialIsland == null)
		{
			info.msg.TutorialIsland = Pool.Get<TutorialIsland>();
		}
		TutorialIsland tutorialIsland = info.msg.TutorialIsland;
		tutorialIsland.targetPlayer = ForPlayer.uid;
		tutorialIsland.disconnectDuration = disconnectedDuration;
		tutorialIsland.spawnLocationIndex = SpawnLocationIndex;
		tutorialIsland.currentIslandTime = CurrentIslandTime;
	}

	public void GetBuildTargets(List<TutorialBuildTarget> targetList, uint targetPrefab)
	{
		TutorialBuildTarget[] array = buildTargets;
		foreach (TutorialBuildTarget tutorialBuildTarget in array)
		{
			if (tutorialBuildTarget.TargetPrefab.isValid && tutorialBuildTarget.TargetPrefab.Get().prefabID == targetPrefab)
			{
				targetList.Add(tutorialBuildTarget);
			}
		}
	}

	public void StartEndingCinematic(BasePlayer player)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		BaseMountable mounted = player.GetMounted();
		if ((Object)(object)mounted != (Object)null && (Object)(object)mounted.VehicleParent() != (Object)null)
		{
			Kayak kayak = mounted.VehicleParent() as Kayak;
			if ((Object)(object)kayak != (Object)null)
			{
				kayak.PrepareForTutorialCinematic(KayakPoint.rotation);
			}
		}
		CinematicScenePlaybackEntity obj = GameManager.server.CreateEntity("assets/prefabs/missions/tutorialisland/endtutorialcinematic.prefab", Vector3Ex.WithY(((Component)player).transform.position, Env.oceanlevel), KayakPoint.rotation) as CinematicScenePlaybackEntity;
		obj.AssignPlayer(player);
		obj.Spawn();
	}

	public void OnPlayerCompletedTutorial(BasePlayer player)
	{
		if ((Object)(object)ForPlayer.Get(serverside: true) != (Object)(object)player)
		{
			Debug.LogWarning((object)$"Attempting to complete tutorial for non-matching player {ForPlayer.Get(serverside: true)} != {player}");
			return;
		}
		((FacepunchBehaviour)this).Invoke((Action)KillPlayerAtEndOfTutorial, 0.1f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)DelayedCompleteTutorial, 0.5f, 0.5f);
	}

	private void KillPlayerAtEndOfTutorial()
	{
		Debug.Log((object)"Kill player");
		BasePlayer basePlayer = ForPlayer.Get(serverside: true);
		basePlayer.ClientRPCPlayer(null, basePlayer, "NotifyTutorialCompleted");
		basePlayer.ClearTutorial();
		basePlayer.Hurt(9999f);
		if (basePlayer.IsGod())
		{
			Debug.LogWarning((object)("Attempting to kill player " + basePlayer.displayName + " at end of tutorial but god mode is active!"));
		}
	}

	private void DelayedCompleteTutorial()
	{
		BasePlayer basePlayer = ForPlayer.Get(serverside: true);
		if (!((Object)(object)basePlayer != (Object)null) || !basePlayer.IsDead())
		{
			ForPlayer.Set(null);
			Return();
		}
	}

	public void Return()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(WorldSpaceBounds(), list, -1, (QueryTriggerInteraction)2);
		foreach (BaseEntity item in list)
		{
			if (!(item is BasePlayer) && !((Object)(object)item == (Object)(object)this) && !item.isClient)
			{
				item.Kill();
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		ForPlayer.Set(null);
		ReturnIsland(this);
		disconnectedDuration = 0f;
	}

	private static void ReturnIsland(TutorialIsland island)
	{
		freeIslandLocations.Remove(island.SpawnLocationIndex);
		island.Kill();
	}

	public void Tick()
	{
		TickPlayerConnectionStatus();
	}

	private void TickPlayerConnectionStatus()
	{
		BasePlayer basePlayer = ForPlayer.Get(base.isServer);
		if ((Object)(object)basePlayer == (Object)null)
		{
			return;
		}
		if (basePlayer.IsSleeping())
		{
			disconnectedDuration += tickRate;
			if (disconnectedDuration >= DisconnectTimeOutDuration)
			{
				Return();
			}
		}
		else
		{
			disconnectedDuration = 0f;
		}
	}

	public bool IsPingValid(MapNote note)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		TutorialBuildTarget[] array = buildTargets;
		foreach (TutorialBuildTarget tutorialBuildTarget in array)
		{
			if (((Component)tutorialBuildTarget).gameObject.activeSelf && Vector3.Distance(((Component)tutorialBuildTarget).transform.position, note.worldPosition) < 0.1f)
			{
				return true;
			}
		}
		return false;
	}

	public static TutorialIsland GetClosestTutorialIsland(Vector3 position, float maxRange)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		float num2 = float.MaxValue;
		for (int i = 0; i < ActiveIslandsServer.Count; i++)
		{
			TutorialIsland tutorialIsland = ActiveIslandsServer[i];
			if ((Object)(object)tutorialIsland != (Object)null)
			{
				float num3 = tutorialIsland.Distance2D(position);
				if (num3 < maxRange && num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
		}
		if (num < 0)
		{
			return null;
		}
		return ActiveIslandsServer[num];
	}

	public void OnPlayerStartedMission(BasePlayer player)
	{
		TutorialBuildTarget[] array = buildTargets;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UpdateActive(player);
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.TutorialIsland != null)
		{
			TutorialIsland tutorialIsland = info.msg.TutorialIsland;
			ForPlayer.uid = tutorialIsland.targetPlayer;
			CurrentIslandTime = tutorialIsland.currentIslandTime;
			disconnectedDuration = tutorialIsland.disconnectDuration;
			SpawnLocationIndex = tutorialIsland.spawnLocationIndex;
			if (base.isServer && info.fromDisk)
			{
				AddIslandFromSave(this);
			}
		}
	}

	public bool CheckPlacement(Construction toConstruct, Construction.Target target, Construction.Placement placement)
	{
		TutorialBuildTarget[] array = buildTargets;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsValid(toConstruct, target, placement))
			{
				return true;
			}
		}
		return false;
	}

	public override void InitShared()
	{
		base.InitShared();
		buildTargets = ((Component)this).GetComponentsInChildren<TutorialBuildTarget>();
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer && ActiveIslandsServer.Contains(this))
		{
			RemoveIslandBounds(GetTutorialGroupId(SpawnLocationIndex), isServer: true);
			ActiveIslandsServer.Remove(this);
		}
	}

	private static void AddIslandBounds(OBB worldBounds, uint netId, bool isServer)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (isServer)
		{
			BoundsListServer.Add(new IslandBounds
			{
				Id = netId,
				WorldBounds = worldBounds
			});
		}
	}

	private static void RemoveIslandBounds(uint netId, bool isServer)
	{
		if (!isServer)
		{
			return;
		}
		for (int i = 0; i < BoundsListServer.Count; i++)
		{
			if (BoundsListServer[i].Id == netId)
			{
				BoundsListServer.RemoveAt(i);
				break;
			}
		}
	}

	public override bool SupportsChildDeployables()
	{
		return true;
	}

	public override bool ForceDeployableSetParent()
	{
		return true;
	}
}
