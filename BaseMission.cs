using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Rust;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Rust/Missions/BaseMission")]
public class BaseMission : BaseScriptableObject
{
	[Serializable]
	public class MissionDependancy
	{
		public BaseMission targetMission;

		public MissionStatus targetMissionDesiredStatus;

		public bool everAttempted;

		public uint targetMissionID
		{
			get
			{
				if (!(targetMission != null))
				{
					return 0u;
				}
				return StringEx.ManifestHash(targetMission.shortname);
			}
		}
	}

	public enum MissionStatus
	{
		Default,
		Active,
		Accomplished,
		Failed,
		Completed
	}

	public enum MissionEventType
	{
		CUSTOM,
		HARVEST,
		CONVERSATION,
		KILL_ENTITY,
		ACQUIRE_ITEM,
		FREE_CRATE,
		MOUNT_ENTITY,
		HURT_ENTITY,
		PLAYER_TICK,
		CRAFT_ITEM,
		DEPLOY,
		HEAL,
		CLOTHINGCHANGED,
		STARTOVEN,
		CONSUME,
		ACQUITE_ITEM_STACK,
		OPEN_STORAGE,
		COOK,
		ENTER_TRIGGER
	}

	[Serializable]
	public class MissionObjectiveEntry
	{
		public Phrase description;

		public int[] startAfterCompletedObjectives;

		public int[] autoCompleteOtherObjectives;

		public bool onlyProgressIfStarted = true;

		public bool isRequired = true;

		public MissionObjective objective;

		public string[] requiredEntities;

		public ItemAmount[] bonusRewards;

		public MissionObjective Get()
		{
			return objective;
		}
	}

	public struct MissionEventPayload
	{
		public NetworkableId NetworkIdentifier;

		public uint UintIdentifier;

		public int IntIdentifier;

		public Vector3 WorldPosition;
	}

	public class MissionInstance : IPooled
	{
		[Serializable]
		public class ObjectiveStatus
		{
			public bool started;

			public bool completed;

			public bool failed;

			public float progressTarget;

			public float progressCurrent;

			public RealTimeSince sinceLastThink;
		}

		public enum ObjectiveType
		{
			MOVE,
			KILL
		}

		private BaseEntity _cachedProviderEntity;

		private BaseMission _cachedMission;

		public NetworkableId providerID;

		public uint missionID;

		public MissionStatus status;

		public float startTime;

		public float endTime;

		public Vector3 missionLocation;

		public float timePassed;

		public Dictionary<string, Vector3> missionPoints = new Dictionary<string, Vector3>();

		public Dictionary<string, MissionEntity> missionEntities = new Dictionary<string, MissionEntity>();

		private int playerInputCounter;

		public ObjectiveStatus[] objectiveStatuses;

		public BaseEntity ProviderEntity()
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)_cachedProviderEntity == (Object)null)
			{
				_cachedProviderEntity = BaseNetworkable.serverEntities.Find(providerID) as BaseEntity;
			}
			return _cachedProviderEntity;
		}

		public BaseMission GetMission()
		{
			if (_cachedMission == null)
			{
				_cachedMission = MissionManifest.GetFromID(missionID);
			}
			return _cachedMission;
		}

		public bool ShouldShowOnMap()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (status == MissionStatus.Active || status == MissionStatus.Accomplished)
			{
				return missionLocation != Vector3.zero;
			}
			return false;
		}

		public bool ShouldShowOnCompass()
		{
			return ShouldShowOnMap();
		}

		public bool NeedsPlayerInput()
		{
			return playerInputCounter > 0;
		}

		public void EnablePlayerInput()
		{
			playerInputCounter++;
		}

		public void DisablePlayerInput()
		{
			playerInputCounter--;
			if (playerInputCounter < 0)
			{
				playerInputCounter = 0;
			}
		}

		public virtual void ProcessMissionEvent(BasePlayer playerFor, MissionEventType type, MissionEventPayload payload, float amount)
		{
			if (status == MissionStatus.Active)
			{
				BaseMission mission = GetMission();
				for (int i = 0; i < mission.objectives.Length; i++)
				{
					mission.objectives[i].objective.ProcessMissionEvent(playerFor, this, i, type, payload, amount);
				}
			}
		}

		public void Think(BasePlayer assignee, float delta)
		{
			if (status != MissionStatus.Failed && status != MissionStatus.Completed)
			{
				BaseMission mission = GetMission();
				timePassed += delta;
				mission.Think(this, assignee, delta);
				if (mission.timeLimitSeconds > 0f && timePassed >= mission.timeLimitSeconds)
				{
					mission.MissionFailed(this, assignee, MissionFailReason.TimeOut);
				}
			}
		}

		public Vector3 GetMissionPoint(string identifier, BasePlayer playerFor, int depth = 0)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			if (identifier == null)
			{
				identifier = "";
			}
			if (missionPoints.TryGetValue(identifier, out var value))
			{
				return value;
			}
			BaseMission mission = GetMission();
			if ((Object)(object)playerFor == (Object)null)
			{
				Debug.LogError((object)("Massive mission failure to get point, correct mission definition of: " + mission.shortname + " (player is null)"));
				return Vector3.zero;
			}
			PositionGenerator positionGenerator = List.FindWith<PositionGenerator, string>((IReadOnlyCollection<PositionGenerator>)(object)mission.positionGenerators, (Func<PositionGenerator, string>)((PositionGenerator p) => p.identifier), identifier, (IEqualityComparer<string>)null);
			if (positionGenerator == null)
			{
				Debug.LogError((object)("Massive mission failure to get point, correct mission definition of: " + mission.shortname + " (cannot find position '" + identifier + "')"));
				return Vector3.zero;
			}
			Vector3 position = positionGenerator.GetPosition(this, playerFor, depth);
			missionPoints.Add(identifier, position);
			AddBlocker(position);
			return position;
		}

		public MissionEntity GetMissionEntity(string identifier, BasePlayer playerFor)
		{
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			if (identifier == null)
			{
				identifier = "";
			}
			if (missionEntities.TryGetValue(identifier, out var value))
			{
				return value;
			}
			MissionEntityEntry missionEntityEntry = List.FindWith<MissionEntityEntry, string>((IReadOnlyCollection<MissionEntityEntry>)(object)GetMission().missionEntities, (Func<MissionEntityEntry, string>)((MissionEntityEntry e) => e.identifier), identifier, (IEqualityComparer<string>)null);
			if (missionEntityEntry == null)
			{
				Debug.LogError((object)$"Cannot spawn mission entity, identifier '{identifier}' not found in mission ID {missionID}");
				value = null;
			}
			else if (!missionEntityEntry.entityRef.isValid)
			{
				Debug.LogError((object)$"Cannot spawn mission entity, identifier '{identifier}' has no entity set in mission ID {missionID}");
				value = null;
			}
			else
			{
				Vector3 missionPoint = GetMissionPoint(missionEntityEntry.spawnPositionToUse, playerFor);
				BaseEntity baseEntity = GameManager.server.CreateEntity(missionEntityEntry.entityRef.resourcePath, missionPoint, Quaternion.identity);
				MissionEntity missionEntity = default(MissionEntity);
				MissionEntity obj = (((Component)baseEntity).gameObject.TryGetComponent<MissionEntity>(ref missionEntity) ? missionEntity : ((Component)baseEntity).gameObject.AddComponent<MissionEntity>());
				obj.Setup(playerFor, this, identifier, missionEntityEntry.cleanupOnMissionSuccess, missionEntityEntry.cleanupOnMissionFailed);
				baseEntity.Spawn();
				value = obj;
			}
			missionEntities.Add(identifier, value);
			if ((Object)(object)value != (Object)null)
			{
				value.MissionStarted(playerFor, this);
			}
			return value;
		}

		public void PostServerLoad(BasePlayer player)
		{
			BaseMission mission = GetMission();
			for (int i = 0; i < mission.objectives.Length; i++)
			{
				if (i >= 0 && i < objectiveStatuses.Length)
				{
					mission.objectives[i].objective.PostServerLoad(player, objectiveStatuses[i]);
				}
			}
		}

		public void Reset()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			providerID = default(NetworkableId);
			missionID = 0u;
			status = MissionStatus.Default;
			startTime = -1f;
			endTime = -1f;
			missionLocation = Vector3.zero;
			_cachedMission = null;
			timePassed = 0f;
			missionPoints.Clear();
			missionEntities.Clear();
		}

		public void EnterPool()
		{
			Reset();
		}

		public void LeavePool()
		{
		}
	}

	[Serializable]
	public class PositionGenerator
	{
		public enum RelativeType
		{
			Player,
			Provider,
			Position
		}

		public enum PositionType
		{
			MissionPoint,
			WorldPositionGenerator,
			DungeonPoint,
			Radius
		}

		public string identifier;

		public float minDistForMovePoint;

		public float maxDistForMovePoint = 25f;

		public RelativeType relativeTo;

		public PositionType positionType;

		public string centerOnPositionIdentifier = "";

		[InspectorFlags]
		public MissionPoint.MissionPointEnum Flags = (MissionPoint.MissionPointEnum)(-1);

		[InspectorFlags]
		public MissionPoint.MissionPointEnum ExclusionFlags;

		public WorldPositionGenerator worldPositionGenerator;

		public bool IsDependant()
		{
			return !string.IsNullOrEmpty(centerOnPositionIdentifier);
		}

		public bool Validate(BasePlayer assignee, BaseMission missionDef)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			Vector3 position;
			if (positionType == PositionType.MissionPoint)
			{
				List<MissionPoint> points = Pool.GetList<MissionPoint>();
				bool missionPoints = MissionPoint.GetMissionPoints(ref points, ((Component)assignee).transform.position, minDistForMovePoint, maxDistForMovePoint, (int)Flags, (int)ExclusionFlags);
				Pool.FreeList<MissionPoint>(ref points);
				if (!missionPoints)
				{
					Debug.Log((object)"FAILED TO FIND MISSION POINTS");
					return false;
				}
			}
			else if (positionType == PositionType.WorldPositionGenerator && (Object)(object)worldPositionGenerator != (Object)null && !worldPositionGenerator.TrySample(((Component)assignee).transform.position, minDistForMovePoint, maxDistForMovePoint, out position, blockedPoints))
			{
				Debug.Log((object)"FAILED TO GENERATE WORLD POSITION!!!!!");
				return false;
			}
			return true;
		}

		public Vector3 GetPosition(MissionInstance instance, BasePlayer assignee, int depth = 0)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0150: Unknown result type (might be due to invalid IL or missing references)
			//IL_0155: Unknown result type (might be due to invalid IL or missing references)
			//IL_0157: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0196: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			if (depth > 10)
			{
				Debug.LogError((object)$"Exceeded max depth while calculating position! missionID={instance.missionID} identifier={identifier}");
				return ((Component)assignee).transform.position;
			}
			Vector3 relativeToPosition = GetRelativeToPosition(instance, assignee, depth);
			Vector3 result;
			if (positionType == PositionType.MissionPoint)
			{
				List<MissionPoint> points = Pool.GetList<MissionPoint>();
				if (MissionPoint.GetMissionPoints(ref points, relativeToPosition, minDistForMovePoint, maxDistForMovePoint, (int)Flags, (int)ExclusionFlags))
				{
					result = points[Random.Range(0, points.Count)].GetPosition();
				}
				else
				{
					Debug.LogError((object)"UNABLE TO FIND MISSIONPOINT FOR MISSION!");
					result = relativeToPosition;
				}
				Pool.FreeList<MissionPoint>(ref points);
			}
			else if (positionType == PositionType.WorldPositionGenerator && (Object)(object)worldPositionGenerator != (Object)null)
			{
				int num = 0;
				while (true)
				{
					if (worldPositionGenerator.TrySample(relativeToPosition, minDistForMovePoint, maxDistForMovePoint, out var position, blockedPoints) && TryAlignToGround(position, out var correctedPosition))
					{
						result = correctedPosition;
						break;
					}
					if (num >= 10)
					{
						Debug.LogError((object)"UNABLE TO FIND WORLD POINT FOR MISSION!");
						result = relativeToPosition;
						break;
					}
					num++;
				}
			}
			else if (positionType == PositionType.DungeonPoint)
			{
				result = DynamicDungeon.GetNextDungeonPoint();
			}
			else
			{
				int num2 = 0;
				while (true)
				{
					Vector3 onUnitSphere = Random.onUnitSphere;
					onUnitSphere.y = 0f;
					((Vector3)(ref onUnitSphere)).Normalize();
					Vector3 val = relativeToPosition + onUnitSphere * Random.Range(minDistForMovePoint, maxDistForMovePoint);
					float num3 = val.y;
					float num4 = val.y;
					if ((Object)(object)TerrainMeta.WaterMap != (Object)null)
					{
						num4 = TerrainMeta.WaterMap.GetHeight(val);
					}
					if ((Object)(object)TerrainMeta.HeightMap != (Object)null)
					{
						num3 = TerrainMeta.HeightMap.GetHeight(val);
					}
					val.y = Mathf.Max(num4, num3);
					if (TryAlignToGround(val, out var correctedPosition2))
					{
						result = correctedPosition2;
						break;
					}
					if (num2 >= 10)
					{
						Debug.LogError((object)"UNABLE TO FIND WORLD POINT FOR MISSION!");
						result = relativeToPosition;
						break;
					}
					num2++;
				}
			}
			return result;
		}

		private Vector3 GetRelativeToPosition(MissionInstance instance, BasePlayer assignee, int depth)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			switch (relativeTo)
			{
			case RelativeType.Position:
				return instance.GetMissionPoint(centerOnPositionIdentifier, assignee, depth + 1);
			case RelativeType.Provider:
			{
				BaseEntity baseEntity = instance.ProviderEntity();
				if ((Object)(object)baseEntity != (Object)null)
				{
					return ((Component)baseEntity).transform.position;
				}
				break;
			}
			}
			if ((Object)(object)assignee != (Object)null)
			{
				return ((Component)assignee).transform.position;
			}
			Debug.LogError((object)$"Cannot get mission point origin - assigne playere is null! missionID={instance.missionID} relativeTo={relativeTo}");
			return Vector3.zero;
		}

		private static bool TryAlignToGround(Vector3 wishPosition, out Vector3 correctedPosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3Ex.WithY(wishPosition, wishPosition.y + 50f);
			RaycastHit hit = default(RaycastHit);
			if (!Physics.Raycast(new Ray(val, Vector3.down), ref hit, 50f, 1218652417, (QueryTriggerInteraction)1))
			{
				correctedPosition = wishPosition;
				return true;
			}
			if ((Object)(object)hit.GetEntity() != (Object)null)
			{
				correctedPosition = wishPosition;
				return false;
			}
			correctedPosition = ((RaycastHit)(ref hit)).point;
			return true;
		}
	}

	[Serializable]
	public class MissionEntityEntry
	{
		[FormerlySerializedAs("entityIdentifier")]
		public string identifier;

		public GameObjectRef entityRef;

		public string spawnPositionToUse;

		public bool spawnOnMissionStart = true;

		public bool cleanupOnMissionFailed;

		public bool cleanupOnMissionSuccess;
	}

	public enum MissionFailReason
	{
		TimeOut,
		Disconnect,
		ResetPlayerState,
		Abandon,
		ObjectiveFailed
	}

	[ServerVar]
	public static bool missionsenabled = true;

	public string shortname;

	public Phrase missionName;

	public Phrase missionDesc;

	public bool canBeAbandoned = true;

	public bool completeSilently;

	public TutorialFullScreenHelpInfo showHelpInfo;

	public MissionObjectiveEntry[] objectives;

	public static List<Vector3> blockedPoints = new List<Vector3>();

	public const string MISSION_COMPLETE_STAT = "missions_completed";

	public GameObjectRef acceptEffect;

	public GameObjectRef failedEffect;

	public GameObjectRef victoryEffect;

	public BasePlayer.TutorialItemAllowance AllowedTutorialItems;

	public BaseMission followupMission;

	public float advanceTimeByOnComplete;

	public TimeChange onStartTimeChange;

	public TimeChange onCompleteTimeChange;

	public int repeatDelaySecondsSuccess = -1;

	public int repeatDelaySecondsFailed = -1;

	public float timeLimitSeconds;

	public Sprite icon;

	public Sprite providerIcon;

	public bool finishTutorial;

	public bool hideStagesNotStarted;

	public MissionDependancy[] acceptDependancies;

	public MissionEntityEntry[] missionEntities;

	public PositionGenerator[] positionGenerators;

	public ItemAmount[] baseRewards;

	public uint id => StringEx.ManifestHash(shortname);

	public bool isRepeatable
	{
		get
		{
			if (repeatDelaySecondsSuccess < 0)
			{
				return repeatDelaySecondsFailed >= 0;
			}
			return true;
		}
	}

	public static void PlayerDisconnected(BasePlayer player)
	{
		if (player.IsNpc)
		{
			return;
		}
		int activeMission = player.GetActiveMission();
		if (activeMission != -1 && activeMission < player.missions.Count)
		{
			MissionInstance missionInstance = player.missions[activeMission];
			BaseMission mission = missionInstance.GetMission();
			if (mission.missionEntities.Length != 0)
			{
				mission.MissionFailed(missionInstance, player, MissionFailReason.Disconnect);
			}
		}
	}

	public static void PlayerKilled(BasePlayer player)
	{
	}

	public virtual Sprite GetIcon(MissionInstance instance)
	{
		return icon;
	}

	public static void AddBlocker(Vector3 point)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (point != Vector3.zero && !blockedPoints.Contains(point))
		{
			blockedPoints.Add(point);
		}
	}

	public static void RemoveBlockers(MissionInstance instance)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<string, Vector3> missionPoint in instance.missionPoints)
		{
			blockedPoints.Remove(missionPoint.Value);
		}
	}

	public static void DoMissionEffect(string effectString, BasePlayer assignee)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Effect effect = new Effect();
		effect.Init(Effect.Type.Generic, assignee, StringPool.Get("head"), Vector3.zero, Vector3.forward);
		effect.pooledString = effectString;
		EffectNetwork.Send(effect, assignee.net.connection);
	}

	public virtual void MissionStart(MissionInstance instance, BasePlayer assignee)
	{
		for (int i = 0; i < objectives.Length; i++)
		{
			objectives[i].Get().MissionStarted(i, instance, assignee);
		}
		if (acceptEffect.isValid)
		{
			DoMissionEffect(acceptEffect.resourcePath, assignee);
		}
		MissionEntityEntry[] array = missionEntities;
		foreach (MissionEntityEntry missionEntityEntry in array)
		{
			if (missionEntityEntry.spawnOnMissionStart)
			{
				instance.GetMissionEntity(missionEntityEntry.identifier, assignee);
			}
		}
		if (AllowedTutorialItems != 0)
		{
			assignee.SetTutorialAllowance(AllowedTutorialItems);
		}
		ApplyTimeChange(onStartTimeChange, assignee);
	}

	private void ApplyTimeChange(TimeChange timeChange, BasePlayer player)
	{
		if (timeChange == null || (Object)(object)player == (Object)null)
		{
			return;
		}
		if (player.IsInTutorial)
		{
			TutorialIsland currentTutorialIsland = player.GetCurrentTutorialIsland();
			if ((Object)(object)currentTutorialIsland != (Object)null)
			{
				currentTutorialIsland.CurrentIslandTime = TimeChange.Apply(currentTutorialIsland.CurrentIslandTime, timeChange, player);
			}
		}
		else
		{
			TimeChange.Apply(0f, timeChange, player);
		}
	}

	public void CheckObjectives(MissionInstance instance, BasePlayer assignee)
	{
		bool flag = true;
		bool flag2 = false;
		for (int i = 0; i < objectives.Length; i++)
		{
			if (objectives[i].isRequired && (!instance.objectiveStatuses[i].completed || instance.objectiveStatuses[i].failed))
			{
				flag = false;
			}
			if (instance.objectiveStatuses[i].failed && objectives[i].isRequired)
			{
				flag2 = true;
			}
		}
		if (instance.status == MissionStatus.Active)
		{
			if (flag2)
			{
				MissionFailed(instance, assignee, MissionFailReason.ObjectiveFailed);
			}
			else if (flag)
			{
				MissionSuccess(instance, assignee);
			}
		}
	}

	public virtual void Think(MissionInstance instance, BasePlayer assignee, float delta)
	{
		for (int i = 0; i < objectives.Length; i++)
		{
			objectives[i].Get().Think(i, instance, assignee, delta);
		}
		CheckObjectives(instance, assignee);
	}

	public virtual void MissionComplete(MissionInstance instance, BasePlayer assignee)
	{
		DoMissionEffect(victoryEffect.resourcePath, assignee);
		if (!instance.GetMission().completeSilently)
		{
			assignee.ChatMessage("You have completed the mission : " + missionName.english);
		}
		BaseMission mission = instance.GetMission();
		if (mission != null)
		{
			if (mission.baseRewards != null)
			{
				ItemAmount[] array = mission.baseRewards;
				foreach (ItemAmount reward2 in array)
				{
					GiveReward(assignee, reward2);
				}
			}
			for (int j = 0; j < mission.objectives.Length; j++)
			{
				MissionObjectiveEntry missionObjectiveEntry = mission.objectives[j];
				if (!missionObjectiveEntry.isRequired && missionObjectiveEntry.bonusRewards != null && instance.objectiveStatuses[j].completed && !instance.objectiveStatuses[j].failed)
				{
					ItemAmount[] array = missionObjectiveEntry.bonusRewards;
					foreach (ItemAmount reward3 in array)
					{
						GiveReward(assignee, reward3);
					}
				}
			}
		}
		Analytics.Server.MissionComplete(this);
		Analytics.Azure.OnMissionComplete(assignee, this);
		instance.status = MissionStatus.Completed;
		assignee.SetActiveMission(-1);
		assignee.MissionDirty();
		ApplyTimeChange(onCompleteTimeChange, assignee);
		if (followupMission != null)
		{
			assignee.RegisterFollowupMission(followupMission, instance.ProviderEntity() as IMissionProvider);
		}
		if (GameInfo.HasAchievements)
		{
			assignee.stats.Add("missions_completed", 1, Stats.All);
			assignee.stats.Save(forceSteamSave: true);
		}
		if (finishTutorial)
		{
			TutorialIsland currentTutorialIsland = assignee.GetCurrentTutorialIsland();
			if ((Object)(object)currentTutorialIsland != (Object)null)
			{
				currentTutorialIsland.StartEndingCinematic(assignee);
			}
			else
			{
				Debug.LogError((object)"Missing tutorial island");
			}
		}
		static void GiveReward(BasePlayer player, ItemAmount reward)
		{
			if ((Object)(object)reward.itemDef == (Object)null || reward.amount == 0f)
			{
				Debug.LogError((object)"BIG REWARD SCREWUP, NULL ITEM DEF");
			}
			else
			{
				Item item = ItemManager.Create(reward.itemDef, Mathf.CeilToInt(reward.amount), 0uL);
				if (item != null)
				{
					player.GiveItem(item, BaseEntity.GiveItemReason.PickedUp);
				}
			}
		}
	}

	public virtual void MissionSuccess(MissionInstance instance, BasePlayer assignee)
	{
		instance.status = MissionStatus.Accomplished;
		MissionEnded(instance, assignee);
		MissionComplete(instance, assignee);
	}

	public virtual void MissionFailed(MissionInstance instance, BasePlayer assignee, MissionFailReason failReason)
	{
		if (!instance.GetMission().completeSilently)
		{
			assignee.ChatMessage("You have failed the mission : " + missionName.english);
		}
		DoMissionEffect(failedEffect.resourcePath, assignee);
		Analytics.Server.MissionFailed(this, failReason);
		Analytics.Azure.OnMissionComplete(assignee, this, failReason);
		instance.status = MissionStatus.Failed;
		MissionEnded(instance, assignee);
	}

	public virtual void MissionEnded(MissionInstance instance, BasePlayer assignee)
	{
		if (instance.missionEntities != null)
		{
			List<MissionEntity> list = Pool.GetList<MissionEntity>();
			foreach (MissionEntity value in instance.missionEntities.Values)
			{
				list.Add(value);
			}
			foreach (MissionEntity item in list)
			{
				if (!((Object)(object)item == (Object)null))
				{
					item.MissionEnded(assignee, instance);
				}
			}
			Pool.FreeList<MissionEntity>(ref list);
		}
		RemoveBlockers(instance);
		instance.endTime = Time.time;
		assignee.SetActiveMission(-1);
		assignee.MissionDirty();
	}

	public void OnObjectiveCompleted(int objectiveIndex, MissionInstance instance, BasePlayer playerFor)
	{
		MissionObjectiveEntry missionObjectiveEntry = objectives[objectiveIndex];
		if (missionObjectiveEntry.autoCompleteOtherObjectives.Length != 0)
		{
			int[] autoCompleteOtherObjectives = missionObjectiveEntry.autoCompleteOtherObjectives;
			foreach (int num in autoCompleteOtherObjectives)
			{
				MissionObjectiveEntry missionObjectiveEntry2 = objectives[num];
				if (!instance.objectiveStatuses[num].completed)
				{
					missionObjectiveEntry2.objective.CompleteObjective(num, instance, playerFor);
				}
			}
		}
		CheckObjectives(instance, playerFor);
	}

	public void OnObjectiveFailed(int objectiveIndex, MissionInstance instance, BasePlayer playerFor)
	{
		CheckObjectives(instance, playerFor);
	}

	public static bool AssignMission(BasePlayer assignee, IMissionProvider provider, BaseMission mission)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (!missionsenabled)
		{
			return false;
		}
		if (!mission.IsEligableForMission(assignee, provider))
		{
			return false;
		}
		int num = List.FindIndexWith<MissionInstance, uint>((IReadOnlyList<MissionInstance>)assignee.missions, (Func<MissionInstance, uint>)((MissionInstance i) => i.missionID), mission.id, (IEqualityComparer<uint>)null);
		MissionInstance missionInstance;
		int activeMission;
		if (num >= 0)
		{
			missionInstance = assignee.missions[num];
			activeMission = num;
			missionInstance.Reset();
		}
		else
		{
			missionInstance = Pool.Get<MissionInstance>();
			activeMission = assignee.missions.Count;
			assignee.missions.Add(missionInstance);
		}
		missionInstance.missionID = mission.id;
		missionInstance.startTime = Time.time;
		missionInstance.providerID = provider.ProviderID();
		missionInstance.status = MissionStatus.Active;
		missionInstance.objectiveStatuses = new MissionInstance.ObjectiveStatus[mission.objectives.Length];
		for (int j = 0; j < mission.objectives.Length; j++)
		{
			missionInstance.objectiveStatuses[j] = new MissionInstance.ObjectiveStatus();
		}
		mission.MissionStart(missionInstance, assignee);
		assignee.SetActiveMission(activeMission);
		assignee.MissionDirty();
		return true;
	}

	public bool IsEligableForMission(BasePlayer player, IMissionProvider provider)
	{
		if (!missionsenabled)
		{
			return false;
		}
		foreach (MissionInstance mission in player.missions)
		{
			if (mission.status == MissionStatus.Accomplished || mission.status == MissionStatus.Active)
			{
				return false;
			}
		}
		return true;
	}
}
