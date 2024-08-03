using System;
using System.Buffers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Facepunch.Models;
using Facepunch.Rust;
using JetBrains.Annotations;
using Network;
using Network.Visibility;
using Newtonsoft.Json;
using ProtoBuf;
using ProtoBuf.Nexus;
using Rust;
using SilentOrbit.ProtocolBuffers;
using UnityEngine;
using UnityEngine.Assertions;

public class BasePlayer : BaseCombatEntity, LootPanel.IHasLootPanel, IIdealSlotEntity, PlayerInventory.ICanMoveFrom
{
	public enum CameraMode
	{
		FirstPerson = 0,
		ThirdPerson = 1,
		Eyes = 2,
		FirstPersonWithArms = 3,
		DeathCamClassic = 4,
		Last = 3
	}

	public enum NetworkQueue
	{
		Update,
		UpdateDistance,
		Count
	}

	private class NetworkQueueList
	{
		public HashSet<BaseNetworkable> queueInternal = new HashSet<BaseNetworkable>();

		public int MaxLength;

		public int Length => queueInternal.Count;

		public bool Contains(BaseNetworkable ent)
		{
			return queueInternal.Contains(ent);
		}

		public void Add(BaseNetworkable ent)
		{
			if (!Contains(ent))
			{
				queueInternal.Add(ent);
			}
			MaxLength = Mathf.Max(MaxLength, queueInternal.Count);
		}

		public void Add(BaseNetworkable[] ent)
		{
			foreach (BaseNetworkable ent2 in ent)
			{
				Add(ent2);
			}
		}

		public void Clear(Group group)
		{
			TimeWarning val = TimeWarning.New("NetworkQueueList.Clear", 0);
			try
			{
				if (group != null)
				{
					if (group.isGlobal)
					{
						return;
					}
					List<BaseNetworkable> list = Pool.GetList<BaseNetworkable>();
					foreach (BaseNetworkable item in queueInternal)
					{
						if ((Object)(object)item == (Object)null || item.net?.group == null || item.net.group == group)
						{
							list.Add(item);
						}
					}
					foreach (BaseNetworkable item2 in list)
					{
						queueInternal.Remove(item2);
					}
					Pool.FreeList<BaseNetworkable>(ref list);
				}
				else
				{
					queueInternal.RemoveWhere((BaseNetworkable x) => (Object)(object)x == (Object)null || x.net?.group == null || !x.net.group.isGlobal);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	[Flags]
	public enum PlayerFlags
	{
		Unused1 = 1,
		Unused2 = 2,
		IsAdmin = 4,
		ReceivingSnapshot = 8,
		Sleeping = 0x10,
		Spectating = 0x20,
		Wounded = 0x40,
		IsDeveloper = 0x80,
		Connected = 0x100,
		ThirdPersonViewmode = 0x400,
		EyesViewmode = 0x800,
		ChatMute = 0x1000,
		NoSprint = 0x2000,
		Aiming = 0x4000,
		DisplaySash = 0x8000,
		Relaxed = 0x10000,
		SafeZone = 0x20000,
		ServerFall = 0x40000,
		Incapacitated = 0x80000,
		Workbench1 = 0x100000,
		Workbench2 = 0x200000,
		Workbench3 = 0x400000,
		VoiceRangeBoost = 0x800000,
		ModifyClan = 0x1000000,
		LoadingAfterTransfer = 0x2000000,
		NoRespawnZone = 0x4000000,
		IsInTutorial = 0x8000000,
		IsRestrained = 0x10000000,
		CreativeMode = 0x20000000
	}

	public static class GestureIds
	{
		public const uint FlashBlindId = 235662700u;
	}

	public enum GestureStartSource
	{
		ServerAction,
		Player
	}

	public enum MapNoteType
	{
		Death,
		PointOfInterest
	}

	public enum PingType
	{
		Hostile = 0,
		GoTo = 1,
		Dollar = 2,
		Loot = 3,
		Node = 4,
		Gun = 5,
		Build = 6,
		LAST = 6
	}

	public struct PingStyle
	{
		public int IconIndex;

		public int ColourIndex;

		public Phrase PingTitle;

		public Phrase PingDescription;

		public PingType Type;

		public PingStyle(int icon, int colour, Phrase title, Phrase desc, PingType pType)
		{
			IconIndex = icon;
			ColourIndex = colour;
			PingTitle = title;
			PingDescription = desc;
			Type = pType;
		}
	}

	public struct FiredProjectileUpdate
	{
		public Vector3 OldPosition;

		public Vector3 NewPosition;

		public Vector3 OldVelocity;

		public Vector3 NewVelocity;

		public float Mismatch;

		public float PartialTime;
	}

	public class FiredProjectile : IPooled
	{
		public ItemDefinition itemDef;

		public ItemModProjectile itemMod;

		public Projectile projectilePrefab;

		public float firedTime;

		public float travelTime;

		public float partialTime;

		public AttackEntity weaponSource;

		public AttackEntity weaponPrefab;

		public Projectile.Modifier projectileModifier;

		public Item pickupItem;

		public float integrity;

		public float trajectoryMismatch;

		public Vector3 position;

		public Vector3 initialPositionOffset;

		public Vector3 positionOffset;

		public Vector3 velocity;

		public Vector3 initialPosition;

		public Vector3 initialVelocity;

		public Vector3 inheritedVelocity;

		public int protection;

		public int ricochets;

		public int hits;

		public BaseEntity lastEntityHit;

		public float desyncLifeTime;

		public int id;

		public BasePlayer attacker;

		public List<FiredProjectileUpdate> updates = new List<FiredProjectileUpdate>();

		public List<Vector3> simulatedPositions = new List<Vector3>();

		public void EnterPool()
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			itemDef = null;
			itemMod = null;
			projectilePrefab = null;
			firedTime = 0f;
			travelTime = 0f;
			partialTime = 0f;
			weaponSource = null;
			weaponPrefab = null;
			projectileModifier = default(Projectile.Modifier);
			pickupItem = null;
			integrity = 0f;
			trajectoryMismatch = 0f;
			position = default(Vector3);
			velocity = default(Vector3);
			initialPosition = default(Vector3);
			initialVelocity = default(Vector3);
			inheritedVelocity = default(Vector3);
			protection = 0;
			ricochets = 0;
			hits = 0;
			lastEntityHit = null;
			desyncLifeTime = 0f;
			id = 0;
			attacker = null;
			updates.Clear();
			simulatedPositions.Clear();
		}

		public void LeavePool()
		{
		}
	}

	public class SpawnPoint
	{
		public Vector3 pos;

		public Quaternion rot;
	}

	public enum TimeCategory
	{
		Wilderness = 1,
		Monument = 2,
		Base = 4,
		Flying = 8,
		Boating = 0x10,
		Swimming = 0x20,
		Driving = 0x40
	}

	public class LifeStoryWorkQueue : ObjectWorkQueue<BasePlayer>
	{
		protected override void RunJob(BasePlayer entity)
		{
			entity.UpdateTimeCategory();
		}

		protected override bool ShouldAdd(BasePlayer entity)
		{
			if (base.ShouldAdd(entity))
			{
				return entity.IsValid();
			}
			return false;
		}
	}

	private class NearbyStash
	{
		public StashContainer Entity;

		public float LookingAtTime;

		public NearbyStash(StashContainer stash)
		{
			Entity = stash;
			LookingAtTime = 0f;
		}
	}

	public enum TutorialItemAllowance
	{
		AlwaysAllowed = -1,
		None = 0,
		Level1_HatchetPickaxe = 10,
		Level2_Planner = 20,
		Level3_Bag_TC_Door = 30,
		Level3_Hammer = 35,
		Level4_Spear_Fire = 40,
		Level5_PrepareForCombat = 50,
		Level6_Furnace = 60,
		Level7_WorkBench = 70,
		Level8_Kayak = 80
	}

	[Serializable]
	public struct CapsuleColliderInfo
	{
		public float height;

		public float radius;

		public Vector3 center;

		public CapsuleColliderInfo(float height, float radius, Vector3 center)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			this.height = height;
			this.radius = radius;
			this.center = center;
		}
	}

	public struct EncryptedValue<TInner> where TInner : unmanaged
	{
		private TInner _value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TInner Get()
		{
			return _value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(TInner value)
		{
			_value = value;
		}

		public override string ToString()
		{
			return Get().ToString();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator EncryptedValue<TInner>(TInner value)
		{
			EncryptedValue<TInner> result = default(EncryptedValue<TInner>);
			result.Set(value);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator TInner(EncryptedValue<TInner> encrypted)
		{
			return encrypted.Get();
		}
	}

	[NonSerialized]
	public bool isInAir;

	[NonSerialized]
	public bool isOnPlayer;

	[NonSerialized]
	public float violationLevel;

	[NonSerialized]
	public float lastViolationTime;

	[NonSerialized]
	public float lastAdminCheatTime;

	[NonSerialized]
	public AntiHackType lastViolationType;

	[NonSerialized]
	public float vehiclePauseTime;

	[NonSerialized]
	public float speedhackPauseTime;

	[NonSerialized]
	public float speedhackDistance;

	[NonSerialized]
	public float flyhackPauseTime;

	[NonSerialized]
	public float flyhackDistanceVertical;

	[NonSerialized]
	public float flyhackDistanceHorizontal;

	[NonSerialized]
	public Vector3 lastGroundedPosition;

	[NonSerialized]
	public float fallingVelocity;

	[NonSerialized]
	public float fallingDistance;

	[NonSerialized]
	public float timeInAir;

	[NonSerialized]
	public float waterDelay;

	[NonSerialized]
	public Vector3 initialVelocity;

	[NonSerialized]
	public TimeAverageValueLookup<uint> rpcHistory = new TimeAverageValueLookup<uint>();

	public static readonly Phrase ClanInviteSuccess = new Phrase("clan.action.invite.success", "Invited {name} to your clan.");

	public static readonly Phrase ClanInviteFailure = new Phrase("clan.action.invite.failure", "Failed to invite {name} to your clan. Please wait a minute and try again.");

	public static readonly Phrase ClanInviteFull = new Phrase("clan.action.invite.full", "Cannot invite {name} to your clan because your clan is full.");

	[NonSerialized]
	public long clanId;

	[NonSerialized]
	public IClan serverClan;

	public ViewModel GestureViewModel;

	private const float drinkRange = 1.5f;

	private const float drinkMovementSpeed = 0.1f;

	[NonSerialized]
	private NetworkQueueList[] networkQueue = new NetworkQueueList[2]
	{
		new NetworkQueueList(),
		new NetworkQueueList()
	};

	[NonSerialized]
	private NetworkQueueList SnapshotQueue = new NetworkQueueList();

	public const string GestureCancelString = "cancel";

	public GestureCollection gestureList;

	private TimeUntil gestureFinishedTime;

	private TimeSince blockHeldInputTimer;

	private GestureConfig currentGesture;

	private HashSet<NetworkableId> recentWaveTargets = new HashSet<NetworkableId>();

	public const string WAVED_PLAYERS_STAT = "waved_at_players";

	public ulong currentTeam;

	public static readonly Phrase MaxTeamSizeToast = new Phrase("maxteamsizetip", "Your team is full. Remove a member to invite another player.");

	private bool sentInstrumentTeamAchievement;

	private bool sentSummerTeamAchievement;

	private const int TEAMMATE_INSTRUMENT_COUNT_ACHIEVEMENT = 4;

	private const int TEAMMATE_SUMMER_FLOATING_COUNT_ACHIEVEMENT = 4;

	private const string TEAMMATE_INSTRUMENT_ACHIEVEMENT = "TEAM_INSTRUMENTS";

	private const string TEAMMATE_SUMMER_ACHIEVEMENT = "SUMMER_INFLATABLE";

	public static Phrase MarkerLimitPhrase = new Phrase("map.marker.limited", "Cannot place more than {0} markers.");

	public const int MaxMapNoteLabelLength = 10;

	public List<BaseMission.MissionInstance> missions = new List<BaseMission.MissionInstance>();

	private float thinkEvery = 1f;

	private float timeSinceMissionThink;

	private BaseMission followupMission;

	private IMissionProvider followupMissionProvider;

	private int _activeMission = -1;

	[NonSerialized]
	public ModelState modelState = new ModelState();

	[NonSerialized]
	private bool wantsSendModelState;

	[NonSerialized]
	private float nextModelStateUpdate;

	[NonSerialized]
	private EntityRef mounted;

	private float nextSeatSwapTime;

	public BaseEntity PetEntity;

	[NonSerialized]
	public IPet Pet;

	private float lastPetCommandIssuedTime;

	private static readonly Phrase HostileTitle = new Phrase("ping_hostile", "Hostile");

	private static readonly Phrase HostileDesc = new Phrase("ping_hostile_desc", "Danger in area");

	private static readonly PingStyle HostileMarker = new PingStyle(4, 3, HostileTitle, HostileDesc, PingType.Hostile);

	private static readonly Phrase GoToTitle = new Phrase("ping_goto", "Go To");

	private static readonly Phrase GoToDesc = new Phrase("ping_goto_desc", "Look at this");

	private static readonly PingStyle GoToMarker = new PingStyle(0, 2, GoToTitle, GoToDesc, PingType.GoTo);

	private static readonly Phrase DollarTitle = new Phrase("ping_dollar", "Value");

	private static readonly Phrase DollarDesc = new Phrase("ping_dollar_desc", "Something valuable is here");

	private static readonly PingStyle DollarMarker = new PingStyle(1, 1, DollarTitle, DollarDesc, PingType.Dollar);

	private static readonly Phrase LootTitle = new Phrase("ping_loot", "Loot");

	private static readonly Phrase LootDesc = new Phrase("ping_loot_desc", "Loot is here");

	private static readonly PingStyle LootMarker = new PingStyle(11, 0, LootTitle, LootDesc, PingType.Loot);

	private static readonly Phrase NodeTitle = new Phrase("ping_node", "Node");

	private static readonly Phrase NodeDesc = new Phrase("ping_node_desc", "An ore node is here");

	private static readonly PingStyle NodeMarker = new PingStyle(10, 4, NodeTitle, NodeDesc, PingType.Node);

	private static readonly Phrase GunTitle = new Phrase("ping_gun", "Weapon");

	private static readonly Phrase GunDesc = new Phrase("ping_weapon_desc", "A dropped weapon is here");

	private static readonly PingStyle GunMarker = new PingStyle(9, 5, GunTitle, GunDesc, PingType.Gun);

	private static readonly PingStyle BuildMarker = new PingStyle(12, 5, new Phrase("", ""), new Phrase("", ""), PingType.Build);

	private TimeSince lastTick;

	private List<(ItemDefinition item, PingType pingType)> tutorialDesiredResource = new List<(ItemDefinition, PingType)>();

	private List<(NetworkableId id, PingType pingType)> pingedEntities = new List<(NetworkableId, PingType)>();

	private TimeSince lastResourcePingUpdate;

	private bool _playerStateDirty;

	private string _wipeId;

	private BaseEntity cachedPrivilegeFromOther;

	private float cachedPrivilegeFromOtherTime;

	[NonSerialized]
	public Dictionary<int, FiredProjectile> firedProjectiles = new Dictionary<int, FiredProjectile>();

	[NonSerialized]
	public PlayerStatistics stats;

	[NonSerialized]
	public ItemId svActiveItemID;

	[NonSerialized]
	public float NextChatTime;

	[NonSerialized]
	public float nextSuicideTime;

	[NonSerialized]
	public float nextRespawnTime;

	[NonSerialized]
	public string respawnId;

	private RealTimeUntil timeUntilLoadingExpires;

	protected Vector3 viewAngles;

	private float lastSubscriptionTick;

	private float lastPlayerTick;

	private float sleepStartTime = -1f;

	private float fallTickRate = 0.1f;

	private float lastFallTime;

	private float fallVelocity;

	private HitInfo cachedNonSuicideHitInfo;

	public static ListHashSet<BasePlayer> activePlayerList = new ListHashSet<BasePlayer>(8);

	public static ListHashSet<BasePlayer> sleepingPlayerList = new ListHashSet<BasePlayer>(8);

	public static ListHashSet<BasePlayer> bots = new ListHashSet<BasePlayer>(8);

	private float cachedCraftLevel;

	private float nextCheckTime;

	private Workbench _cachedWorkbench;

	private PersistantPlayer cachedPersistantPlayer;

	private static OceanPaths cachedOceanPaths = null;

	private const int WILDERNESS = 1;

	private const int MONUMENT = 2;

	private const int BASE = 4;

	private const int FLYING = 8;

	private const int BOATING = 16;

	private const int SWIMMING = 32;

	private const int DRIVING = 64;

	[ServerVar]
	[Help("How many milliseconds to budget for processing life story updates per frame")]
	public static float lifeStoryFramebudgetms = 0.25f;

	[NonSerialized]
	public PlayerLifeStory lifeStory;

	[NonSerialized]
	public PlayerLifeStory previousLifeStory;

	private const float TimeCategoryUpdateFrequency = 7f;

	private float nextTimeCategoryUpdate;

	private bool hasSentPresenceState;

	private bool LifeStoryInWilderness;

	private bool LifeStoryInMonument;

	private bool LifeStoryInBase;

	private bool LifeStoryFlying;

	private bool LifeStoryBoating;

	private bool LifeStorySwimming;

	private bool LifeStoryDriving;

	private bool waitingForLifeStoryUpdate;

	public static LifeStoryWorkQueue lifeStoryQueue = new LifeStoryWorkQueue();

	[CanBeNull]
	private DeathInfo cachedOverrideDeathInfo;

	private bool IsSpectatingTeamInfo;

	private TimeSince lastSpectateTeamInfoUpdate;

	private int SpectateOffset = 1000000;

	private string spectateFilter = "";

	private List<NearbyStash> nearbyStashes = new List<NearbyStash>();

	private float lastUpdateTime = float.NegativeInfinity;

	private float cachedThreatLevel;

	[NonSerialized]
	public float weaponDrawnDuration;

	[NonSerialized]
	private float lastTickTime;

	[NonSerialized]
	private float lastStallTime;

	[NonSerialized]
	private float lastInputTime;

	[NonSerialized]
	private float tutorialKickTime;

	[NonSerialized]
	public ItemId? restraintItemId;

	private PlayerTick lastReceivedTick = new PlayerTick();

	private float tickDeltaTime;

	private bool tickNeedsFinalizing;

	private readonly TimeAverageValue ticksPerSecond = new TimeAverageValue();

	private readonly TickInterpolator tickInterpolator = new TickInterpolator();

	public Deque<Vector3> eyeHistory = new Deque<Vector3>(8);

	public TickHistory tickHistory = new TickHistory();

	private float startTutorialCooldown;

	private float nextUnderwearValidationTime;

	private uint lastValidUnderwearSkin;

	private float woundedDuration;

	private float lastWoundedStartTime = float.NegativeInfinity;

	private float healingWhileCrawling;

	private bool woundedByFallDamage;

	private const float INCAPACITATED_HEALTH_MIN = 2f;

	private const float INCAPACITATED_HEALTH_MAX = 6f;

	public const int MaxBotIdRange = 10000000;

	[Header("BasePlayer")]
	public GameObjectRef fallDamageEffect;

	public GameObjectRef drownEffect;

	[InspectorFlags]
	public PlayerFlags playerFlags;

	private HiddenValue<PlayerEyes> eyesValue = Pool.Get<HiddenValue<PlayerEyes>>();

	private HiddenValue<PlayerInventory> inventoryValue = Pool.Get<HiddenValue<PlayerInventory>>();

	[NonSerialized]
	public PlayerBlueprints blueprints;

	[NonSerialized]
	public PlayerMetabolism metabolism;

	[NonSerialized]
	public PlayerModifiers modifiers;

	private HiddenValue<CapsuleCollider> colliderValue = Pool.Get<HiddenValue<CapsuleCollider>>();

	public PlayerBelt Belt;

	private Rigidbody playerRigidbody;

	[NonSerialized]
	public EncryptedValue<ulong> userID = 0uL;

	[NonSerialized]
	public string UserIDString;

	[NonSerialized]
	public int gamemodeteam = -1;

	[NonSerialized]
	public int reputation;

	protected string _displayName;

	private string _lastSetName;

	public const float crouchSpeed = 1.7f;

	public const float walkSpeed = 2.8f;

	public const float runSpeed = 5.5f;

	public const float crawlSpeed = 0.72f;

	private CapsuleColliderInfo playerColliderStanding;

	private CapsuleColliderInfo playerColliderDucked;

	private CapsuleColliderInfo playerColliderCrawling;

	private CapsuleColliderInfo playerColliderLyingDown;

	private ProtectionProperties cachedProtection;

	private float nextColliderRefreshTime = -1f;

	public float weaponMoveSpeedScale = 1f;

	public bool clothingBlocksAiming;

	public float clothingMoveSpeedReduction;

	public float clothingWaterSpeedBonus;

	public float clothingAccuracyBonus;

	public bool equippingBlocked;

	public float eggVision;

	private PhoneController activeTelephone;

	public BaseEntity designingAIEntity;

	public Phrase LootPanelTitle => Phrase.op_Implicit(displayName);

	public bool IsReceivingSnapshot => HasPlayerFlag(PlayerFlags.ReceivingSnapshot);

	public bool IsAdmin => HasPlayerFlag(PlayerFlags.IsAdmin);

	public bool IsDeveloper => HasPlayerFlag(PlayerFlags.IsDeveloper);

	public bool IsInCreativeMode
	{
		get
		{
			if (!Creative.allUsers)
			{
				return HasPlayerFlag(PlayerFlags.CreativeMode);
			}
			return true;
		}
	}

	public bool UnlockAllSkins
	{
		get
		{
			if (!IsDeveloper)
			{
				return false;
			}
			if (base.isServer)
			{
				return net.connection.info.GetBool("client.unlock_all_skins", false);
			}
			return false;
		}
	}

	public bool IsAiming => HasPlayerFlag(PlayerFlags.Aiming);

	public bool IsFlying
	{
		get
		{
			if (modelState == null)
			{
				return false;
			}
			return modelState.flying;
		}
	}

	public bool IsConnected
	{
		get
		{
			if (base.isServer)
			{
				if (Net.sv == null)
				{
					return false;
				}
				if (net == null)
				{
					return false;
				}
				if (net.connection == null)
				{
					return false;
				}
				return true;
			}
			return false;
		}
	}

	public bool IsInTutorial => HasPlayerFlag(PlayerFlags.IsInTutorial);

	public bool IsRestrained
	{
		get
		{
			if (IsAlive())
			{
				return HasPlayerFlag(PlayerFlags.IsRestrained);
			}
			return false;
		}
	}

	public bool IsRestrainedOrSurrendering
	{
		get
		{
			if (!IsRestrained)
			{
				return CurrentGestureIsSurrendering;
			}
			return true;
		}
	}

	public bool InGesture
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)currentGesture != (Object)null)
			{
				if (!(TimeUntil.op_Implicit(gestureFinishedTime) > 0f))
				{
					return currentGesture.animationType == GestureConfig.AnimationType.Loop;
				}
				return true;
			}
			return false;
		}
	}

	private bool CurrentGestureBlocksMovement
	{
		get
		{
			if (InGesture)
			{
				return currentGesture.movementMode == GestureConfig.MovementCapabilities.NoMovement;
			}
			return false;
		}
	}

	public bool CurrentGestureIsDance
	{
		get
		{
			if (InGesture)
			{
				return currentGesture.actionType == GestureConfig.GestureActionType.DanceAchievement;
			}
			return false;
		}
	}

	public bool CurrentGestureIsFullBody
	{
		get
		{
			if (InGesture)
			{
				return currentGesture.playerModelLayer == GestureConfig.PlayerModelLayer.FullBody;
			}
			return false;
		}
	}

	public bool CurrentGestureIsUpperBody
	{
		get
		{
			if (InGesture)
			{
				return currentGesture.playerModelLayer == GestureConfig.PlayerModelLayer.UpperBody;
			}
			return false;
		}
	}

	public bool CurrentGestureIsSurrendering
	{
		get
		{
			if (InGesture)
			{
				return currentGesture.actionType == GestureConfig.GestureActionType.Surrender;
			}
			return false;
		}
	}

	private bool InGestureCancelCooldown => TimeSince.op_Implicit(blockHeldInputTimer) < 0.5f;

	public RelationshipManager.PlayerTeam Team
	{
		get
		{
			if ((Object)(object)RelationshipManager.ServerInstance == (Object)null)
			{
				return null;
			}
			return RelationshipManager.ServerInstance.FindTeam(currentTeam);
		}
	}

	public MapNote ServerCurrentDeathNote
	{
		get
		{
			return State.deathMarker;
		}
		set
		{
			State.deathMarker = value;
		}
	}

	public bool HasPendingFollowupMission => ((FacepunchBehaviour)this).IsInvoking((Action)AssignFollowUpMission);

	public ModelState modelStateTick { get; private set; }

	public bool isMounted => mounted.IsValid(base.isServer);

	public bool isMountingHidingWeapon
	{
		get
		{
			if (isMounted)
			{
				return GetMounted().CanHoldItems();
			}
			return false;
		}
	}

	private int TotalPingCount
	{
		get
		{
			if (State.pings == null)
			{
				return 0;
			}
			return State.pings.Count;
		}
	}

	public PlayerState State
	{
		get
		{
			if ((ulong)userID == 0L)
			{
				throw new InvalidOperationException("Cannot get player state without a SteamID");
			}
			return SingletonComponent<ServerMgr>.Instance.playerStateManager.Get(userID);
		}
	}

	public string WipeId
	{
		get
		{
			if (_wipeId == null)
			{
				_wipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(userID);
			}
			return _wipeId;
		}
	}

	public virtual BaseNpc.AiStatistics.FamilyEnum Family => BaseNpc.AiStatistics.FamilyEnum.Player;

	protected override float PositionTickRate => -1f;

	public int DebugMapMarkerIndex { get; set; }

	public uint LastBlockColourChangeId { get; set; }

	public bool PlayHeavyLandingAnimation { get; set; }

	public Vector3 estimatedVelocity { get; private set; }

	public float estimatedSpeed { get; private set; }

	public float estimatedSpeed2D { get; private set; }

	public int secondsConnected { get; private set; }

	public float desyncTimeRaw { get; private set; }

	public float desyncTimeClamped { get; private set; }

	public float secondsSleeping
	{
		get
		{
			if (sleepStartTime == -1f || !IsSleeping())
			{
				return 0f;
			}
			return Time.time - sleepStartTime;
		}
	}

	public static IEnumerable<BasePlayer> allPlayerList
	{
		get
		{
			Enumerator<BasePlayer> enumerator = sleepingPlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					yield return enumerator.Current;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			enumerator = activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					yield return enumerator.Current;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
	}

	public float currentCraftLevel
	{
		get
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			if (triggers == null)
			{
				_cachedWorkbench = null;
				return 0f;
			}
			if (nextCheckTime > Time.realtimeSinceStartup)
			{
				return cachedCraftLevel;
			}
			_cachedWorkbench = null;
			nextCheckTime = Time.realtimeSinceStartup + Random.Range(0.4f, 0.5f);
			float num = 0f;
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerWorkbench triggerWorkbench = triggers[i] as TriggerWorkbench;
				if (!((Object)(object)triggerWorkbench == (Object)null) && !((Object)(object)triggerWorkbench.parentBench == (Object)null) && triggerWorkbench.parentBench.IsVisible(eyes.position))
				{
					_cachedWorkbench = triggerWorkbench.parentBench;
					float num2 = triggerWorkbench.WorkbenchLevel();
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			cachedCraftLevel = num;
			return num;
		}
	}

	public float currentComfort
	{
		get
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			if (isMounted)
			{
				num = GetMounted().GetComfort();
			}
			if (triggers == null)
			{
				return num;
			}
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerComfort triggerComfort = triggers[i] as TriggerComfort;
				if (!((Object)(object)triggerComfort == (Object)null))
				{
					float num2 = triggerComfort.CalculateComfort(((Component)this).transform.position, this);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}
	}

	public PersistantPlayer PersistantPlayerInfo
	{
		get
		{
			if (cachedPersistantPlayer == null)
			{
				cachedPersistantPlayer = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerInfo(userID);
			}
			return cachedPersistantPlayer;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			cachedPersistantPlayer = value;
			SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerInfo(userID, value);
		}
	}

	public bool hasPreviousLife => previousLifeStory != null;

	public int currentTimeCategory { get; private set; }

	public bool IsBeingSpectated { get; private set; }

	public InputState serverInput { get; private set; } = new InputState();


	public float timeSinceLastTick
	{
		get
		{
			if (lastTickTime == 0f)
			{
				return 0f;
			}
			return Time.time - lastTickTime;
		}
	}

	public float timeSinceLastStall
	{
		get
		{
			if (lastStallTime == 0f)
			{
				return 60f;
			}
			return Time.time - lastStallTime;
		}
	}

	public float IdleTime
	{
		get
		{
			if (lastInputTime == 0f)
			{
				return 0f;
			}
			return Time.time - lastInputTime;
		}
	}

	public bool isStalled
	{
		get
		{
			if (IsDead() || IsSleeping())
			{
				lastStallTime = 0f;
				return false;
			}
			if (timeSinceLastTick != 0f && timeSinceLastTick > ConVar.AntiHack.rpcstallthreshold)
			{
				lastStallTime = Time.time;
				return true;
			}
			return false;
		}
	}

	public bool wasStalled
	{
		get
		{
			if (!isStalled)
			{
				return timeSinceLastStall < ConVar.AntiHack.rpcstallfade;
			}
			return true;
		}
	}

	public Vector3 tickViewAngles { get; private set; }

	public int tickHistoryCapacity => Mathf.Max(1, Mathf.CeilToInt((float)ticksPerSecond.Calculate() * ConVar.AntiHack.tickhistorytime));

	public Matrix4x4 tickHistoryMatrix
	{
		get
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (!Object.op_Implicit((Object)(object)((Component)this).transform.parent))
			{
				return Matrix4x4.identity;
			}
			return ((Component)this).transform.parent.localToWorldMatrix;
		}
	}

	public TutorialItemAllowance CurrentTutorialAllowance { get; private set; }

	public float TimeSinceWoundedStarted => Time.realtimeSinceStartup - lastWoundedStartTime;

	public Connection Connection
	{
		get
		{
			if (net != null)
			{
				return net.connection;
			}
			return null;
		}
	}

	public bool IsBot => (ulong)userID < 10000000;

	public PlayerEyes eyes
	{
		get
		{
			return eyesValue?.Get();
		}
		set
		{
			eyesValue.Set(value);
		}
	}

	public PlayerInventory inventory => inventoryValue?.Get();

	private CapsuleCollider playerCollider => colliderValue?.Get();

	public string displayName
	{
		get
		{
			return NameHelper.Get(userID, _displayName, base.isClient);
		}
		set
		{
			if (!(_lastSetName == value))
			{
				_lastSetName = value;
				_displayName = SanitizePlayerNameString(value, userID);
			}
		}
	}

	public override TraitFlag Traits => base.Traits | TraitFlag.Human | TraitFlag.Food | TraitFlag.Meat | TraitFlag.Alive;

	public bool HasActiveTelephone => (Object)(object)activeTelephone != (Object)null;

	public bool IsDesigningAI => (Object)(object)designingAIEntity != (Object)null;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BasePlayer.OnRpcMessage", 0);
		try
		{
			if (rpc == 935768323 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ClientKeepConnectionAlive "));
				}
				TimeWarning val2 = TimeWarning.New("ClientKeepConnectionAlive", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(935768323u, "ClientKeepConnectionAlive", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							ClientKeepConnectionAlive(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ClientKeepConnectionAlive");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3782818894u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ClientLoadingComplete "));
				}
				TimeWarning val2 = TimeWarning.New("ClientLoadingComplete", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(3782818894u, "ClientLoadingComplete", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							ClientLoadingComplete(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ClientLoadingComplete");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1497207530 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - IssuePetCommand "));
				}
				TimeWarning val2 = TimeWarning.New("IssuePetCommand", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg4 = rPCMessage;
						IssuePetCommand(msg4);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex3)
				{
					Debug.LogException(ex3);
					player.Kick("RPC Error in IssuePetCommand");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2041023702 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - IssuePetCommandRaycast "));
				}
				TimeWarning val2 = TimeWarning.New("IssuePetCommandRaycast", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg5 = rPCMessage;
						IssuePetCommandRaycast(msg5);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex4)
				{
					Debug.LogException(ex4);
					player.Kick("RPC Error in IssuePetCommandRaycast");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 495414158 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - NotifyDebugCameraEnded "));
				}
				TimeWarning val2 = TimeWarning.New("NotifyDebugCameraEnded", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg6 = rPCMessage;
						NotifyDebugCameraEnded(msg6);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex5)
				{
					Debug.LogException(ex5);
					player.Kick("RPC Error in NotifyDebugCameraEnded");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3441821928u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - OnFeedbackReport "));
				}
				TimeWarning val2 = TimeWarning.New("OnFeedbackReport", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3441821928u, "OnFeedbackReport", this, player, 1uL))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg7 = rPCMessage;
							OnFeedbackReport(msg7);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in OnFeedbackReport");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1998170713 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - OnPlayerLanded "));
				}
				TimeWarning val2 = TimeWarning.New("OnPlayerLanded", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(1998170713u, "OnPlayerLanded", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg8 = rPCMessage;
							OnPlayerLanded(msg8);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in OnPlayerLanded");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2147041557 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - OnPlayerReported "));
				}
				TimeWarning val2 = TimeWarning.New("OnPlayerReported", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2147041557u, "OnPlayerReported", this, player, 1uL))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg9 = rPCMessage;
							OnPlayerReported(msg9);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in OnPlayerReported");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 363681694 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - OnProjectileAttack "));
				}
				TimeWarning val2 = TimeWarning.New("OnProjectileAttack", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(363681694u, "OnProjectileAttack", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg10 = rPCMessage;
							OnProjectileAttack(msg10);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex9)
					{
						Debug.LogException(ex9);
						player.Kick("RPC Error in OnProjectileAttack");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1500391289 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - OnProjectileRicochet "));
				}
				TimeWarning val2 = TimeWarning.New("OnProjectileRicochet", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(1500391289u, "OnProjectileRicochet", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg11 = rPCMessage;
							OnProjectileRicochet(msg11);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex10)
					{
						Debug.LogException(ex10);
						player.Kick("RPC Error in OnProjectileRicochet");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2324190493u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - OnProjectileUpdate "));
				}
				TimeWarning val2 = TimeWarning.New("OnProjectileUpdate", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(2324190493u, "OnProjectileUpdate", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg12 = rPCMessage;
							OnProjectileUpdate(msg12);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex11)
					{
						Debug.LogException(ex11);
						player.Kick("RPC Error in OnProjectileUpdate");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3167788018u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - PerformanceReport "));
				}
				TimeWarning val2 = TimeWarning.New("PerformanceReport", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3167788018u, "PerformanceReport", this, player, 1uL))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg13 = rPCMessage;
							PerformanceReport(msg13);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex12)
					{
						Debug.LogException(ex12);
						player.Kick("RPC Error in PerformanceReport");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 420048204 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - PerformanceReport_Frametime "));
				}
				TimeWarning val2 = TimeWarning.New("PerformanceReport_Frametime", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(420048204u, "PerformanceReport_Frametime", this, player, 1uL))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg14 = rPCMessage;
							PerformanceReport_Frametime(msg14);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex13)
					{
						Debug.LogException(ex13);
						player.Kick("RPC Error in PerformanceReport_Frametime");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4081064578u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - PlayerRequestedTutorialStart "));
				}
				TimeWarning val2 = TimeWarning.New("PlayerRequestedTutorialStart", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(4081064578u, "PlayerRequestedTutorialStart", this, player, 1uL))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg15 = rPCMessage;
							PlayerRequestedTutorialStart(msg15);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex14)
					{
						Debug.LogException(ex14);
						player.Kick("RPC Error in PlayerRequestedTutorialStart");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1024003327 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RequestParachuteDeploy "));
				}
				TimeWarning val2 = TimeWarning.New("RequestParachuteDeploy", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1024003327u, "RequestParachuteDeploy", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(1024003327u, "RequestParachuteDeploy", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg16 = rPCMessage;
							RequestParachuteDeploy(msg16);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex15)
					{
						Debug.LogException(ex15);
						player.Kick("RPC Error in RequestParachuteDeploy");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 52352806 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RequestRespawnInformation "));
				}
				TimeWarning val2 = TimeWarning.New("RequestRespawnInformation", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(52352806u, "RequestRespawnInformation", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(52352806u, "RequestRespawnInformation", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg17 = rPCMessage;
							RequestRespawnInformation(msg17);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex16)
					{
						Debug.LogException(ex16);
						player.Kick("RPC Error in RequestRespawnInformation");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1774681338 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RequestServerEmoji "));
				}
				TimeWarning val2 = TimeWarning.New("RequestServerEmoji", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1774681338u, "RequestServerEmoji", this, player, 1uL))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RequestServerEmoji();
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex17)
					{
						Debug.LogException(ex17);
						player.Kick("RPC Error in RequestServerEmoji");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 970468557 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_Assist "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Assist", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(970468557u, "RPC_Assist", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg18 = rPCMessage;
							RPC_Assist(msg18);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex18)
					{
						Debug.LogException(ex18);
						player.Kick("RPC Error in RPC_Assist");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3263238541u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_KeepAlive "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_KeepAlive", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3263238541u, "RPC_KeepAlive", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg19 = rPCMessage;
							RPC_KeepAlive(msg19);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex19)
					{
						Debug.LogException(ex19);
						player.Kick("RPC Error in RPC_KeepAlive");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3692395068u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_LootPlayer "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_LootPlayer", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3692395068u, "RPC_LootPlayer", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg20 = rPCMessage;
							RPC_LootPlayer(msg20);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex20)
					{
						Debug.LogException(ex20);
						player.Kick("RPC Error in RPC_LootPlayer");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 439739525 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_ReqDoPush "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ReqDoPush", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(439739525u, "RPC_ReqDoPush", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(439739525u, "RPC_ReqDoPush", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(439739525u, "RPC_ReqDoPush", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc2 = rPCMessage;
							RPC_ReqDoPush(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex21)
					{
						Debug.LogException(ex21);
						player.Kick("RPC Error in RPC_ReqDoPush");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3974264977u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_ReqEquipHood "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ReqEquipHood", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3974264977u, "RPC_ReqEquipHood", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3974264977u, "RPC_ReqEquipHood", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3974264977u, "RPC_ReqEquipHood", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc3 = rPCMessage;
							RPC_ReqEquipHood(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex22)
					{
						Debug.LogException(ex22);
						player.Kick("RPC Error in RPC_ReqEquipHood");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4144905368u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_ReqForceMountNearest "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ReqForceMountNearest", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(4144905368u, "RPC_ReqForceMountNearest", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(4144905368u, "RPC_ReqForceMountNearest", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(4144905368u, "RPC_ReqForceMountNearest", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc4 = rPCMessage;
							RPC_ReqForceMountNearest(rpc4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex23)
					{
						Debug.LogException(ex23);
						player.Kick("RPC Error in RPC_ReqForceMountNearest");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3816898909u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_ReqForceSwapSeat "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ReqForceSwapSeat", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3816898909u, "RPC_ReqForceSwapSeat", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3816898909u, "RPC_ReqForceSwapSeat", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3816898909u, "RPC_ReqForceSwapSeat", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc5 = rPCMessage;
							RPC_ReqForceSwapSeat(rpc5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex24)
					{
						Debug.LogException(ex24);
						player.Kick("RPC Error in RPC_ReqForceSwapSeat");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 626234931 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_ReqRemoveCuffs "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ReqRemoveCuffs", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(626234931u, "RPC_ReqRemoveCuffs", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(626234931u, "RPC_ReqRemoveCuffs", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(626234931u, "RPC_ReqRemoveCuffs", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc6 = rPCMessage;
							RPC_ReqRemoveCuffs(rpc6);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex25)
					{
						Debug.LogException(ex25);
						player.Kick("RPC Error in RPC_ReqRemoveCuffs");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2289764809u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_ReqRemoveHood "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ReqRemoveHood", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2289764809u, "RPC_ReqRemoveHood", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(2289764809u, "RPC_ReqRemoveHood", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(2289764809u, "RPC_ReqRemoveHood", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc7 = rPCMessage;
							RPC_ReqRemoveHood(rpc7);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex26)
					{
						Debug.LogException(ex26);
						player.Kick("RPC Error in RPC_ReqRemoveHood");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1539133504 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_StartClimb "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_StartClimb", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg21 = rPCMessage;
						RPC_StartClimb(msg21);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex27)
				{
					Debug.LogException(ex27);
					player.Kick("RPC Error in RPC_StartClimb");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3047177092u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_AddMarker "));
				}
				TimeWarning val2 = TimeWarning.New("Server_AddMarker", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3047177092u, "Server_AddMarker", this, player, 8uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(3047177092u, "Server_AddMarker", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg22 = rPCMessage;
							Server_AddMarker(msg22);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex28)
					{
						Debug.LogException(ex28);
						player.Kick("RPC Error in Server_AddMarker");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3618659425u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_AddPing "));
				}
				TimeWarning val2 = TimeWarning.New("Server_AddPing", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3618659425u, "Server_AddPing", this, player, 3uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(3618659425u, "Server_AddPing", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg23 = rPCMessage;
							Server_AddPing(msg23);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex29)
					{
						Debug.LogException(ex29);
						player.Kick("RPC Error in Server_AddPing");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1005040107 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_CancelGesture "));
				}
				TimeWarning val2 = TimeWarning.New("Server_CancelGesture", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1005040107u, "Server_CancelGesture", this, player, 10uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(1005040107u, "Server_CancelGesture", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							Server_CancelGesture();
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex30)
					{
						Debug.LogException(ex30);
						player.Kick("RPC Error in Server_CancelGesture");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 706157120 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_ClearMapMarkers "));
				}
				TimeWarning val2 = TimeWarning.New("Server_ClearMapMarkers", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(706157120u, "Server_ClearMapMarkers", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(706157120u, "Server_ClearMapMarkers", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg24 = rPCMessage;
							Server_ClearMapMarkers(msg24);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex31)
					{
						Debug.LogException(ex31);
						player.Kick("RPC Error in Server_ClearMapMarkers");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1032755717 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_RemovePing "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RemovePing", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1032755717u, "Server_RemovePing", this, player, 3uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(1032755717u, "Server_RemovePing", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg25 = rPCMessage;
							Server_RemovePing(msg25);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex32)
					{
						Debug.LogException(ex32);
						player.Kick("RPC Error in Server_RemovePing");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 31713840 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_RemovePointOfInterest "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RemovePointOfInterest", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(31713840u, "Server_RemovePointOfInterest", this, player, 10uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(31713840u, "Server_RemovePointOfInterest", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg26 = rPCMessage;
							Server_RemovePointOfInterest(msg26);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex33)
					{
						Debug.LogException(ex33);
						player.Kick("RPC Error in Server_RemovePointOfInterest");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2567683804u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_RequestMarkers "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RequestMarkers", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2567683804u, "Server_RequestMarkers", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(2567683804u, "Server_RequestMarkers", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg27 = rPCMessage;
							Server_RequestMarkers(msg27);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex34)
					{
						Debug.LogException(ex34);
						player.Kick("RPC Error in Server_RequestMarkers");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1572722245 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_StartGesture "));
				}
				TimeWarning val2 = TimeWarning.New("Server_StartGesture", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1572722245u, "Server_StartGesture", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(1572722245u, "Server_StartGesture", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg28 = rPCMessage;
							Server_StartGesture(msg28);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex35)
					{
						Debug.LogException(ex35);
						player.Kick("RPC Error in Server_StartGesture");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1180369886 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_UpdateMarker "));
				}
				TimeWarning val2 = TimeWarning.New("Server_UpdateMarker", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1180369886u, "Server_UpdateMarker", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(1180369886u, "Server_UpdateMarker", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg29 = rPCMessage;
							Server_UpdateMarker(msg29);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex36)
					{
						Debug.LogException(ex36);
						player.Kick("RPC Error in Server_UpdateMarker");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2192544725u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ServerRequestEmojiData "));
				}
				TimeWarning val2 = TimeWarning.New("ServerRequestEmojiData", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2192544725u, "ServerRequestEmojiData", this, player, 3uL))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg30 = rPCMessage;
							ServerRequestEmojiData(msg30);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex37)
					{
						Debug.LogException(ex37);
						player.Kick("RPC Error in ServerRequestEmojiData");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3635568749u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ServerRPC_UnderwearChange "));
				}
				TimeWarning val2 = TimeWarning.New("ServerRPC_UnderwearChange", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg31 = rPCMessage;
						ServerRPC_UnderwearChange(msg31);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex38)
				{
					Debug.LogException(ex38);
					player.Kick("RPC Error in ServerRPC_UnderwearChange");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3222472445u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - StartTutorial "));
				}
				TimeWarning val2 = TimeWarning.New("StartTutorial", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg32 = rPCMessage;
						StartTutorial(msg32);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex39)
				{
					Debug.LogException(ex39);
					player.Kick("RPC Error in StartTutorial");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 970114602 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - SV_Drink "));
				}
				TimeWarning val2 = TimeWarning.New("SV_Drink", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg33 = rPCMessage;
						SV_Drink(msg33);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex40)
				{
					Debug.LogException(ex40);
					player.Kick("RPC Error in SV_Drink");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1361044246 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - UpdateSpectatePositionFromDebugCamera "));
				}
				TimeWarning val2 = TimeWarning.New("UpdateSpectatePositionFromDebugCamera", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1361044246u, "UpdateSpectatePositionFromDebugCamera", this, player, 10uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(1361044246u, "UpdateSpectatePositionFromDebugCamera", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg34 = rPCMessage;
							UpdateSpectatePositionFromDebugCamera(msg34);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex41)
					{
						Debug.LogException(ex41);
						player.Kick("RPC Error in UpdateSpectatePositionFromDebugCamera");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public bool TriggeredAntiHack(float seconds = 1f, float score = float.PositiveInfinity)
	{
		if (!(Time.realtimeSinceStartup - lastViolationTime < seconds))
		{
			return violationLevel > score;
		}
		return true;
	}

	public bool UsedAdminCheat(float seconds = 2f)
	{
		return Time.realtimeSinceStartup - lastAdminCheatTime < seconds;
	}

	public void PauseVehicleNoClipDetection(float seconds = 1f)
	{
		vehiclePauseTime = Mathf.Max(vehiclePauseTime, seconds);
	}

	public void PauseFlyHackDetection(float seconds = 1f)
	{
		flyhackPauseTime = Mathf.Max(flyhackPauseTime, seconds);
	}

	public void PauseSpeedHackDetection(float seconds = 1f)
	{
		speedhackPauseTime = Mathf.Max(speedhackPauseTime, seconds);
	}

	public int GetAntiHackKicks()
	{
		return AntiHack.GetKickRecord(this);
	}

	public void ResetAntiHack()
	{
		violationLevel = 0f;
		lastViolationTime = 0f;
		lastAdminCheatTime = 0f;
		speedhackPauseTime = 0f;
		speedhackDistance = 0f;
		flyhackPauseTime = 0f;
		flyhackDistanceVertical = 0f;
		flyhackDistanceHorizontal = 0f;
		rpcHistory.Clear();
	}

	public bool CanModifyClan()
	{
		if (!Clan.editsRequireClanTable)
		{
			return true;
		}
		if (base.isServer)
		{
			if (triggers == null || (Object)(object)ClanManager.ServerInstance == (Object)null)
			{
				return false;
			}
			foreach (TriggerBase trigger in triggers)
			{
				if (trigger is TriggerClanModify)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public void LoadClanInfo()
	{
		ClanManager clanManager = ClanManager.ServerInstance;
		if (!((Object)(object)clanManager == (Object)null))
		{
			LoadClanInfoImpl();
		}
		async void LoadClanInfoImpl()
		{
			try
			{
				ClanValueResult<IClan> val = await clanManager.Backend.GetByMember((ulong)userID);
				if (!val.IsSuccess)
				{
					if ((int)val.Result != 3)
					{
						Debug.LogError((object)$"Failed to find clan for {userID}: {val.Result}");
						((FacepunchBehaviour)this).Invoke((Action)LoadClanInfo, (float)(45 + Random.Range(0, 30)));
						return;
					}
					serverClan = null;
					clanId = 0L;
				}
				else
				{
					serverClan = val.Value;
					clanId = serverClan.ClanId;
				}
				SendNetworkUpdate();
				Networkable obj = net;
				if (((obj != null) ? obj.connection : null) != null)
				{
					UpdateClanLastSeen();
					if (clanId != 0L)
					{
						clanManager.ClanMemberConnectionsChanged(clanId);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}

	public void UpdateClanLastSeen()
	{
		ClanManager clanManager = ClanManager.ServerInstance;
		if (!((Object)(object)clanManager == (Object)null) && clanId != 0L)
		{
			UpdateClanLastSeenImpl();
		}
		async void UpdateClanLastSeenImpl()
		{
			_ = 1;
			try
			{
				ClanValueResult<IClan> val = await clanManager.Backend.Get(clanId);
				if (!val.IsSuccess)
				{
					LoadClanInfo();
				}
				else
				{
					ClanResult val2 = await val.Value.UpdateLastSeen((ulong)userID);
					if ((int)val2 != 1)
					{
						Debug.LogWarning((object)$"Couldn't update clan last seen for {userID}: {val2}");
					}
				}
			}
			catch (Exception arg)
			{
				Debug.LogError((object)$"Failed to update clan last seen for {userID}: {arg}");
			}
		}
	}

	public void AddClanScore(ClanScoreEventType type, int multiplier = 1, BasePlayer otherPlayer = null, IClan otherClan = null, string arg1 = null, string arg2 = null)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		ClanManager serverInstance = ClanManager.ServerInstance;
		if (!((Object)(object)serverInstance == (Object)null) && serverClan != null && !IsBot && !IsNpc && multiplier != 0)
		{
			int scoreForEvent = Clan.GetScoreForEvent(type);
			if (scoreForEvent != 0)
			{
				bool flag = (Object)(object)otherPlayer != (Object)null && !otherPlayer.IsBot && !otherPlayer.IsNpc;
				serverInstance.AddScore(serverClan, new ClanScoreEvent
				{
					Type = type,
					SteamId = userID,
					Score = scoreForEvent,
					Multiplier = multiplier,
					OtherSteamId = (flag ? new ulong?(otherPlayer.userID) : null),
					OtherClanId = ((otherClan != null && otherClan != serverClan) ? new long?(otherClan.ClanId) : ((flag && otherPlayer.clanId != 0L) ? new long?(otherPlayer.clanId) : null)),
					Arg1 = arg1,
					Arg2 = arg2
				});
			}
		}
	}

	private void HandleClanPlayerKilled(BasePlayer killedByPlayer)
	{
		if (serverClan != null && killedByPlayer.serverClan != null && serverClan != killedByPlayer.serverClan)
		{
			AddClanScore((ClanScoreEventType)2, 1, killedByPlayer);
			killedByPlayer.AddClanScore((ClanScoreEventType)1, 1, this);
		}
		if (!HasPlayerFlag(PlayerFlags.DisplaySash) && killedByPlayer.serverClan != null)
		{
			killedByPlayer.AddClanScore((ClanScoreEventType)3, 1, this);
		}
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		if ((Object)(object)player == (Object)(object)this)
		{
			return false;
		}
		if ((IsWounded() || IsSleeping() || CurrentGestureIsSurrendering || IsRestrainedOrSurrendering) && !IsLoadingAfterTransfer())
		{
			return !IsTransferring();
		}
		return false;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_LootPlayer(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (Object.op_Implicit((Object)(object)player) && player.CanInteract() && CanBeLooted(player) && player.inventory.loot.StartLootingEntity(this))
		{
			player.inventory.loot.AddContainer(inventory.containerMain);
			player.inventory.loot.AddContainer(inventory.containerWear);
			player.inventory.loot.AddContainer(inventory.containerBelt);
			player.inventory.loot.SendImmediate();
			player.ClientRPC(RpcTarget.Player("RPC_OpenLootPanel", player), "player_corpse");
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Assist(RPCMessage msg)
	{
		if (msg.player.CanInteract() && !((Object)(object)msg.player == (Object)(object)this) && IsWounded())
		{
			StopWounded(msg.player);
			msg.player.stats.Add("wounded_assisted", 1, (Stats)5);
			stats.Add("wounded_healed", 1);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_KeepAlive(RPCMessage msg)
	{
		if (msg.player.CanInteract() && !((Object)(object)msg.player == (Object)(object)this) && IsWounded())
		{
			ProlongWounding(10f);
		}
	}

	[RPC_Server]
	private void SV_Drink(RPCMessage msg)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		Vector3 val = msg.read.Vector3();
		if (Vector3Ex.IsNaNOrInfinity(val) || !Object.op_Implicit((Object)(object)player) || !player.metabolism.CanConsume() || Vector3.Distance(((Component)player).transform.position, val) > 5f || !WaterLevel.Test(val, waves: true, volumes: true, this) || (isMounted && !GetMounted().canDrinkWhileMounted))
		{
			return;
		}
		ItemDefinition atPoint = WaterResource.GetAtPoint(val);
		if (!((Object)(object)atPoint == (Object)null))
		{
			ItemModConsumable component = ((Component)atPoint).GetComponent<ItemModConsumable>();
			Item item = ItemManager.Create(atPoint, component.amountToConsume, 0uL);
			ItemModConsume component2 = ((Component)item.info).GetComponent<ItemModConsume>();
			if (component2.CanDoAction(item, player))
			{
				component2.DoAction(item, player);
			}
			item?.Remove();
			player.metabolism.MarkConsumption();
		}
	}

	[RPC_Server]
	public void RPC_StartClimb(RPCMessage msg)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		bool flag = msg.read.Bit();
		Vector3 val = msg.read.Vector3();
		NetworkableId val2 = msg.read.EntityID();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(val2);
		Vector3 val3 = (flag ? ((Component)baseNetworkable).transform.TransformPoint(val) : val);
		if (!player.isMounted || player.Distance(val3) > 5f || !GamePhysics.LineOfSight(player.eyes.position, val3, 1218519041) || !GamePhysics.LineOfSight(val3, val3 + player.eyes.offset, 1218519041))
		{
			return;
		}
		Vector3 val4 = val3 - player.eyes.position;
		Vector3 end = val3 - ((Vector3)(ref val4)).normalized * 0.25f;
		if (!GamePhysics.CheckCapsule(player.eyes.position, end, 0.25f, 1218519041, (QueryTriggerInteraction)0) && !AntiHack.TestNoClipping(val3 + player.NoClipOffset(), val3 + player.NoClipOffset(), player.NoClipRadius(ConVar.AntiHack.noclip_margin), ConVar.AntiHack.noclip_backtracking, sphereCast: true, out var _))
		{
			player.EnsureDismounted();
			((Component)player).transform.position = val3;
			Collider component = ((Component)player).GetComponent<Collider>();
			component.enabled = false;
			component.enabled = true;
			player.ForceUpdateTriggers();
			if (flag)
			{
				player.ClientRPC<Vector3, NetworkableId>(RpcTarget.Player("ForcePositionToParentOffset", player), val, val2);
			}
			else
			{
				player.ClientRPC<Vector3>(RpcTarget.Player("ForcePositionTo", player), val3);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(1uL)]
	private void RequestServerEmoji()
	{
		RustEmojiLibrary.FindAllServerEmoji();
		if (RustEmojiLibrary.allServerEmoji.Count > 0)
		{
			ClientRPCPlayerList(null, this, "ClientReceiveEmojiList", RustEmojiLibrary.cachedServerList);
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	private void ServerRequestEmojiData(RPCMessage msg)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		string text = msg.read.String(256, false);
		if (RustEmojiLibrary.allServerEmoji.TryGetValue(text, out var value))
		{
			byte[] array = FileStorage.server.Get(value.CRC, value.FileType, RustEmojiLibrary.EmojiStorageNetworkId);
			ClientRPC(RpcTarget.Player("ClientReceiveEmojiData", msg.player), (uint)array.Length, array, text, value.CRC, (int)value.FileType);
		}
	}

	public int GetQueuedUpdateCount(NetworkQueue queue)
	{
		return networkQueue[(int)queue].Length;
	}

	public void SendSnapshots(ListHashSet<Networkable> ents)
	{
		TimeWarning val = TimeWarning.New("SendSnapshots", 0);
		try
		{
			int count = ents.Values.Count;
			Networkable[] buffer = ents.Values.Buffer;
			for (int i = 0; i < count; i++)
			{
				SnapshotQueue.Add(buffer[i].handler as BaseNetworkable);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void QueueUpdate(NetworkQueue queue, BaseNetworkable ent)
	{
		if (!IsConnected)
		{
			return;
		}
		switch (queue)
		{
		case NetworkQueue.Update:
			networkQueue[0].Add(ent);
			break;
		case NetworkQueue.UpdateDistance:
			if (!IsReceivingSnapshot && !networkQueue[1].Contains(ent) && !networkQueue[0].Contains(ent))
			{
				NetworkQueueList networkQueueList = networkQueue[1];
				if (Distance(ent as BaseEntity) < 20f)
				{
					QueueUpdate(NetworkQueue.Update, ent);
				}
				else
				{
					networkQueueList.Add(ent);
				}
			}
			break;
		}
	}

	public void SendEntityUpdate()
	{
		TimeWarning val = TimeWarning.New("SendEntityUpdate", 0);
		try
		{
			SendEntityUpdates(SnapshotQueue);
			SendEntityUpdates(networkQueue[0]);
			SendEntityUpdates(networkQueue[1]);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void ClearEntityQueue(Group group = null)
	{
		SnapshotQueue.Clear(group);
		networkQueue[0].Clear(group);
		networkQueue[1].Clear(group);
	}

	private void SendEntityUpdates(NetworkQueueList queue)
	{
		if (queue.queueInternal.Count == 0)
		{
			return;
		}
		int num = (IsReceivingSnapshot ? ConVar.Server.updatebatchspawn : ConVar.Server.updatebatch);
		List<BaseNetworkable> list = Pool.GetList<BaseNetworkable>();
		TimeWarning val = TimeWarning.New("SendEntityUpdates.SendEntityUpdates", 0);
		try
		{
			int num2 = 0;
			foreach (BaseNetworkable item in queue.queueInternal)
			{
				SendEntitySnapshot(item);
				list.Add(item);
				num2++;
				if (num2 > num)
				{
					break;
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (num > queue.queueInternal.Count)
		{
			queue.queueInternal.Clear();
		}
		else
		{
			val = TimeWarning.New("SendEntityUpdates.Remove", 0);
			try
			{
				for (int i = 0; i < list.Count; i++)
				{
					queue.queueInternal.Remove(list[i]);
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (queue.queueInternal.Count == 0 && queue.MaxLength > 2048)
		{
			queue.queueInternal.Clear();
			queue.queueInternal = new HashSet<BaseNetworkable>();
			queue.MaxLength = 0;
		}
		Pool.FreeList<BaseNetworkable>(ref list);
	}

	private void SendEntitySnapshot(BaseNetworkable ent)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("SendEntitySnapshot", 0);
		try
		{
			if (!((Object)(object)ent == (Object)null) && ent.net != null && ent.ShouldNetworkTo(this))
			{
				NetWrite val2 = ((BaseNetwork)Net.sv).StartWrite();
				net.connection.validate.entityUpdates++;
				SaveInfo saveInfo = default(SaveInfo);
				saveInfo.forConnection = net.connection;
				saveInfo.forDisk = false;
				SaveInfo saveInfo2 = saveInfo;
				val2.PacketID((Type)5);
				val2.UInt32(net.connection.validate.entityUpdates);
				ent.ToStreamForNetwork((Stream)(object)val2, saveInfo2);
				val2.Send(new SendInfo(net.connection));
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public bool HasPlayerFlag(PlayerFlags f)
	{
		return (playerFlags & f) == f;
	}

	public void SetPlayerFlag(PlayerFlags f, bool b)
	{
		if (b)
		{
			if (HasPlayerFlag(f))
			{
				return;
			}
			playerFlags |= f;
		}
		else
		{
			if (!HasPlayerFlag(f))
			{
				return;
			}
			playerFlags &= ~f;
		}
		SendNetworkUpdate();
	}

	public void LightToggle(bool mask = true)
	{
		Item activeItem = GetActiveItem();
		if (activeItem != null)
		{
			BaseEntity heldEntity = activeItem.GetHeldEntity();
			if ((Object)(object)heldEntity != (Object)null)
			{
				HeldEntity component = ((Component)heldEntity).GetComponent<HeldEntity>();
				if (Object.op_Implicit((Object)(object)component))
				{
					((Component)component).SendMessage("SetLightsOn", (object)(mask && !component.LightsOn()), (SendMessageOptions)1);
				}
			}
		}
		foreach (Item item in inventory.containerWear.itemList)
		{
			ItemModWearable component2 = ((Component)item.info).GetComponent<ItemModWearable>();
			if (Object.op_Implicit((Object)(object)component2) && component2.emissive)
			{
				item.SetFlag(Item.Flag.IsOn, mask && !item.HasFlag(Item.Flag.IsOn));
				item.MarkDirty();
			}
		}
		if (isMounted)
		{
			GetMounted().LightToggle(this);
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(1uL)]
	private void Server_StartGesture(RPCMessage msg)
	{
		if (!IsGestureBlocked())
		{
			uint id = msg.read.UInt32();
			if (!((Object)(object)gestureList == (Object)null))
			{
				GestureConfig toPlay = gestureList.IdToGesture(id);
				Server_StartGesture(toPlay);
			}
		}
	}

	public void Server_StartGesture(uint gestureId)
	{
		if (!((Object)(object)gestureList == (Object)null))
		{
			GestureConfig toPlay = gestureList.IdToGesture(gestureId);
			Server_StartGesture(toPlay);
		}
	}

	public void Server_StartGesture(GestureConfig toPlay, GestureStartSource startSource = GestureStartSource.Player)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		if (((Object)(object)toPlay != (Object)null && toPlay.hideInWheel && startSource == GestureStartSource.Player && !ConVar.Server.cinematic) || !((Object)(object)toPlay != (Object)null) || !toPlay.IsOwnedBy(this) || !toPlay.CanBeUsedBy(this))
		{
			return;
		}
		if (toPlay.animationType == GestureConfig.AnimationType.OneShot)
		{
			((FacepunchBehaviour)this).Invoke((Action)TimeoutGestureServer, toPlay.duration);
		}
		else if (toPlay.animationType == GestureConfig.AnimationType.Loop)
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)MonitorLoopingGesture, 0f, 0f);
		}
		ClientRPC(RpcTarget.NetworkGroup("Client_StartGesture"), toPlay.gestureId);
		gestureFinishedTime = TimeUntil.op_Implicit(toPlay.duration);
		currentGesture = toPlay;
		switch (toPlay.actionType)
		{
		case GestureConfig.GestureActionType.Surrender:
			inventory.SetLockedByRestraint(flag: true);
			break;
		case GestureConfig.GestureActionType.ShowNameTag:
			if (GameInfo.HasAchievements)
			{
				int val = CountWaveTargets(((Component)this).transform.position, 4f, 0.6f, eyes.HeadForward(), recentWaveTargets, 5);
				stats.Add("waved_at_players", val);
				stats.Save(forceSteamSave: true);
			}
			break;
		case GestureConfig.GestureActionType.DanceAchievement:
		{
			TriggerDanceAchievement triggerDanceAchievement = FindTrigger<TriggerDanceAchievement>();
			if ((Object)(object)triggerDanceAchievement != (Object)null)
			{
				triggerDanceAchievement.NotifyDanceStarted();
			}
			break;
		}
		}
		if (toPlay.animationType == GestureConfig.AnimationType.Loop)
		{
			SendNetworkUpdate();
		}
	}

	private void TimeoutGestureServer()
	{
		currentGesture = null;
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(10uL)]
	public void Server_CancelGesture()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)currentGesture != (Object)null && currentGesture.actionType == GestureConfig.GestureActionType.Surrender)
		{
			Handcuffs handcuffs = GetHeldEntity() as Handcuffs;
			if ((Object)(object)handcuffs == (Object)null || !handcuffs.Locked)
			{
				inventory.SetLockedByRestraint(flag: false);
			}
		}
		currentGesture = null;
		blockHeldInputTimer = TimeSince.op_Implicit(0f);
		ClientRPC(RpcTarget.NetworkGroup("Client_RemoteCancelledGesture"));
		((FacepunchBehaviour)this).CancelInvoke((Action)MonitorLoopingGesture);
	}

	private void MonitorLoopingGesture()
	{
		bool flag = (Object)(object)currentGesture != (Object)null && currentGesture.canDuckDuringGesture;
		if (modelState == null || (!flag && modelState.ducked) || modelState.sleeping || IsWounded() || IsSwimming() || IsDead() || (isMounted && GetMounted().allowedGestures == BaseMountable.MountGestureType.UpperBody && CurrentGestureIsUpperBody) || (isMounted && GetMounted().allowedGestures == BaseMountable.MountGestureType.None))
		{
			Server_CancelGesture();
		}
	}

	private void NotifyGesturesNewItemEquipped()
	{
		if (InGesture)
		{
			Server_CancelGesture();
		}
	}

	public int CountWaveTargets(Vector3 position, float distance, float minimumDot, Vector3 forward, HashSet<NetworkableId> workingList, int maxCount)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		float sqrDistance = distance * distance;
		Group group = net.group;
		if (group == null)
		{
			return 0;
		}
		List<Connection> subscribers = group.subscribers;
		int num = 0;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection val = subscribers[i];
			if (!val.active)
			{
				continue;
			}
			BasePlayer basePlayer = val.player as BasePlayer;
			if (CheckPlayer(basePlayer))
			{
				workingList.Add(basePlayer.net.ID);
				num++;
				if (num >= maxCount)
				{
					break;
				}
			}
		}
		return num;
		bool CheckPlayer(BasePlayer player)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)player == (Object)null)
			{
				return false;
			}
			if ((Object)(object)player == (Object)(object)this)
			{
				return false;
			}
			if (player.SqrDistance(position) > sqrDistance)
			{
				return false;
			}
			Vector3 val2 = ((Component)player).transform.position - position;
			if (Vector3.Dot(((Vector3)(ref val2)).normalized, forward) < minimumDot)
			{
				return false;
			}
			if (workingList.Contains(player.net.ID))
			{
				return false;
			}
			return true;
		}
	}

	private bool IsGestureBlocked()
	{
		if (isMounted && GetMounted().allowedGestures == BaseMountable.MountGestureType.None)
		{
			return true;
		}
		if (Object.op_Implicit((Object)(object)GetHeldEntity()) && GetHeldEntity().BlocksGestures())
		{
			return true;
		}
		bool flag = (Object)(object)currentGesture != (Object)null;
		if (flag && currentGesture.gestureType == GestureConfig.GestureType.Cinematic)
		{
			flag = false;
		}
		if (!(IsWounded() || flag) && !IsDead() && !IsSleeping())
		{
			return IsRestrained;
		}
		return true;
	}

	public void DelayedTeamUpdate()
	{
		UpdateTeam(currentTeam);
	}

	public void TeamUpdate()
	{
		TeamUpdate(fullTeamUpdate: false);
	}

	public void TeamUpdate(bool fullTeamUpdate)
	{
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		if (!RelationshipManager.TeamsEnabled() || !IsConnected || currentTeam == 0L)
		{
			return;
		}
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindTeam(currentTeam);
		if (playerTeam == null)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		PlayerTeam val = Pool.Get<PlayerTeam>();
		try
		{
			val.teamLeader = playerTeam.teamLeader;
			val.teamID = playerTeam.teamID;
			val.teamName = playerTeam.teamName;
			val.members = Pool.GetList<TeamMember>();
			val.teamLifetime = playerTeam.teamLifetime;
			val.teamPings = Pool.GetList<MapNote>();
			foreach (ulong member in playerTeam.members)
			{
				BasePlayer basePlayer = RelationshipManager.FindByID(member);
				if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsInTutorial)
				{
					continue;
				}
				TeamMember val2 = Pool.Get<TeamMember>();
				val2.displayName = (((Object)(object)basePlayer != (Object)null) ? basePlayer.displayName : (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(member) ?? "DEAD"));
				val2.healthFraction = (((Object)(object)basePlayer != (Object)null && basePlayer.IsAlive()) ? basePlayer.healthFraction : 0f);
				val2.position = (((Object)(object)basePlayer != (Object)null) ? ((Component)basePlayer).transform.position : Vector3.zero);
				val2.online = (Object)(object)basePlayer != (Object)null && !basePlayer.IsSleeping();
				val2.wounded = (Object)(object)basePlayer != (Object)null && basePlayer.IsWounded();
				if ((!sentInstrumentTeamAchievement || !sentSummerTeamAchievement) && (Object)(object)basePlayer != (Object)null)
				{
					if (Object.op_Implicit((Object)(object)basePlayer.GetHeldEntity()) && basePlayer.GetHeldEntity().IsInstrument())
					{
						num++;
					}
					if (basePlayer.isMounted)
					{
						if (basePlayer.GetMounted().IsInstrument())
						{
							num++;
						}
						if (basePlayer.GetMounted().IsSummerDlcVehicle)
						{
							num2++;
						}
					}
					if (num >= 4 && !sentInstrumentTeamAchievement)
					{
						GiveAchievement("TEAM_INSTRUMENTS");
						sentInstrumentTeamAchievement = true;
					}
					if (num2 >= 4)
					{
						GiveAchievement("SUMMER_INFLATABLE");
						sentSummerTeamAchievement = true;
					}
				}
				val2.userID = member;
				val.members.Add(val2);
				if ((Object)(object)basePlayer != (Object)null)
				{
					if (basePlayer.State.pings != null && basePlayer.State.pings.Count > 0 && (Object)(object)basePlayer != (Object)(object)this)
					{
						val.teamPings.AddRange(basePlayer.State.pings);
					}
					if (fullTeamUpdate && (Object)(object)basePlayer != (Object)(object)this)
					{
						basePlayer.TeamUpdate(fullTeamUpdate: false);
					}
				}
			}
			val.leaderMapNotes = Pool.GetList<MapNote>();
			PlayerState val3 = SingletonComponent<ServerMgr>.Instance.playerStateManager.Get(playerTeam.teamLeader);
			if (val3?.pointsOfInterest != null)
			{
				foreach (MapNote item in val3.pointsOfInterest)
				{
					val.leaderMapNotes.Add(item);
				}
			}
			ClientRPC<PlayerTeam>(RpcTarget.PlayerAndSpectators("CLIENT_ReceiveTeamInfo", this), val);
			if (val.leaderMapNotes != null)
			{
				val.leaderMapNotes.Clear();
			}
			if (val.teamPings != null)
			{
				val.teamPings.Clear();
			}
			BasePlayer basePlayer2 = FindByID(playerTeam.teamLeader);
			if (fullTeamUpdate && (Object)(object)basePlayer2 != (Object)null && (Object)(object)basePlayer2 != (Object)(object)this)
			{
				basePlayer2.TeamUpdate(fullTeamUpdate: false);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void UpdateTeam(ulong newTeam)
	{
		currentTeam = newTeam;
		SendNetworkUpdate();
		if (RelationshipManager.ServerInstance.FindTeam(newTeam) == null)
		{
			ClearTeam();
		}
		else
		{
			TeamUpdate();
		}
	}

	public void ClearTeam()
	{
		currentTeam = 0uL;
		ClientRPC(RpcTarget.PlayerAndSpectators("CLIENT_ClearTeam", this));
		SendNetworkUpdate();
	}

	public void ClearPendingInvite()
	{
		ClientRPC(RpcTarget.Player("CLIENT_PendingInvite", this), "", 0);
	}

	public HeldEntity GetHeldEntity()
	{
		if (base.isServer)
		{
			Item activeItem = GetActiveItem();
			if (activeItem == null)
			{
				return null;
			}
			return activeItem.GetHeldEntity() as HeldEntity;
		}
		return null;
	}

	public bool IsHoldingEntity<T>()
	{
		HeldEntity heldEntity = GetHeldEntity();
		if ((Object)(object)heldEntity == (Object)null)
		{
			return false;
		}
		return heldEntity is T;
	}

	public bool IsHostileItem(Item item)
	{
		if (!item.info.isHoldable)
		{
			return false;
		}
		ItemModEntity component = ((Component)item.info).GetComponent<ItemModEntity>();
		if ((Object)(object)component == (Object)null)
		{
			return false;
		}
		GameObject val = component.entityPrefab.Get();
		if ((Object)(object)val == (Object)null)
		{
			return false;
		}
		AttackEntity component2 = val.GetComponent<AttackEntity>();
		if ((Object)(object)component2 == (Object)null)
		{
			return false;
		}
		return component2.hostile;
	}

	public bool IsItemHoldRestricted(Item item)
	{
		if (IsNpc)
		{
			return false;
		}
		if (InSafeZone() && item != null && IsHostileItem(item))
		{
			return true;
		}
		return false;
	}

	public void ClearDeathMarker(bool sendToClient = false)
	{
		if (!IsNpc)
		{
			if (ServerCurrentDeathNote != null)
			{
				Pool.Free<MapNote>(ref State.deathMarker);
			}
			DirtyPlayerState();
			if (sendToClient)
			{
				SendMarkersToClient();
			}
		}
	}

	public void Server_LogDeathMarker(Vector3 position)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!IsNpc)
		{
			if (ServerCurrentDeathNote == null)
			{
				ServerCurrentDeathNote = Pool.Get<MapNote>();
				ServerCurrentDeathNote.noteType = 0;
			}
			ServerCurrentDeathNote.worldPosition = position;
			ClientRPC<MapNote>(RpcTarget.Player("Client_AddNewDeathMarker", this), ServerCurrentDeathNote);
			DirtyPlayerState();
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(8uL)]
	public void Server_AddMarker(RPCMessage msg)
	{
		if (State.pointsOfInterest == null)
		{
			State.pointsOfInterest = Pool.GetList<MapNote>();
		}
		if (State.pointsOfInterest.Count >= ConVar.Server.maximumMapMarkers)
		{
			msg.player.ShowToast(GameTip.Styles.Blue_Short, MarkerLimitPhrase, ConVar.Server.maximumMapMarkers.ToString());
			return;
		}
		MapNote val = MapNote.Deserialize((Stream)(object)msg.read);
		ValidateMapNote(val);
		val.colourIndex = FindUnusedPointOfInterestColour();
		State.pointsOfInterest.Add(val);
		DirtyPlayerState();
		SendMarkersToClient();
		TeamUpdate();
	}

	private int FindUnusedPointOfInterestColour()
	{
		if (State.pointsOfInterest == null)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			if (HasColour(num))
			{
				num++;
			}
		}
		return num;
		bool HasColour(int index)
		{
			foreach (MapNote item in State.pointsOfInterest)
			{
				if (item.colourIndex == index)
				{
					return true;
				}
			}
			return false;
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(1uL)]
	public void Server_UpdateMarker(RPCMessage msg)
	{
		if (State.pointsOfInterest == null)
		{
			State.pointsOfInterest = Pool.GetList<MapNote>();
		}
		int num = msg.read.Int32();
		if (State.pointsOfInterest.Count <= num)
		{
			return;
		}
		MapNote val = MapNote.Deserialize((Stream)(object)msg.read);
		try
		{
			ValidateMapNote(val);
			val.CopyTo(State.pointsOfInterest[num]);
			DirtyPlayerState();
			SendMarkersToClient();
			TeamUpdate();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void ValidateMapNote(MapNote n)
	{
		if (n.label != null)
		{
			n.label = StringExtensions.Truncate(n.label, 10, (string)null).ToUpperInvariant();
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(10uL)]
	public void Server_RemovePointOfInterest(RPCMessage msg)
	{
		int num = msg.read.Int32();
		if (State.pointsOfInterest != null && State.pointsOfInterest.Count > num && num >= 0)
		{
			State.pointsOfInterest[num].Dispose();
			State.pointsOfInterest.RemoveAt(num);
			DirtyPlayerState();
			SendMarkersToClient();
			TeamUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(1uL)]
	public void Server_RequestMarkers(RPCMessage msg)
	{
		SendMarkersToClient();
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(1uL)]
	public void Server_ClearMapMarkers(RPCMessage msg)
	{
		MapNote serverCurrentDeathNote = ServerCurrentDeathNote;
		if (serverCurrentDeathNote != null)
		{
			serverCurrentDeathNote.Dispose();
		}
		ServerCurrentDeathNote = null;
		if (State.pointsOfInterest != null)
		{
			foreach (MapNote item in State.pointsOfInterest)
			{
				if (item != null)
				{
					item.Dispose();
				}
			}
			State.pointsOfInterest.Clear();
		}
		DirtyPlayerState();
		TeamUpdate();
	}

	private void SendMarkersToClient()
	{
		MapNoteList val = Pool.Get<MapNoteList>();
		try
		{
			val.notes = Pool.GetList<MapNote>();
			if (ServerCurrentDeathNote != null)
			{
				val.notes.Add(ServerCurrentDeathNote);
			}
			if (State.pointsOfInterest != null)
			{
				val.notes.AddRange(State.pointsOfInterest);
			}
			ClientRPC<MapNoteList>(RpcTarget.Player("Client_ReceiveMarkers", this), val);
			val.notes.Clear();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public bool HasAttemptedMission(uint missionID)
	{
		foreach (BaseMission.MissionInstance mission in missions)
		{
			if (mission.missionID == missionID)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanAcceptMission(BaseMission mission)
	{
		return CanAcceptMission(mission.id);
	}

	public bool CanAcceptMission(uint missionID)
	{
		if (HasActiveMission())
		{
			return false;
		}
		if (!BaseMission.missionsenabled)
		{
			return false;
		}
		BaseMission fromID = MissionManifest.GetFromID(missionID);
		if (fromID == null)
		{
			Debug.LogError((object)("MISSION NOT FOUND IN MANIFEST, ID :" + missionID));
			return false;
		}
		if (fromID.acceptDependancies != null && fromID.acceptDependancies.Length != 0)
		{
			BaseMission.MissionDependancy[] acceptDependancies = fromID.acceptDependancies;
			foreach (BaseMission.MissionDependancy missionDependancy in acceptDependancies)
			{
				if (missionDependancy.everAttempted)
				{
					continue;
				}
				bool flag = false;
				foreach (BaseMission.MissionInstance mission in missions)
				{
					if (mission.missionID == missionDependancy.targetMissionID && mission.status == missionDependancy.targetMissionDesiredStatus)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
		}
		if (IsMissionActive(missionID))
		{
			return false;
		}
		if (fromID.isRepeatable)
		{
			bool num = HasCompletedMission(missionID);
			bool flag2 = HasFailedMission(missionID);
			if (num && fromID.repeatDelaySecondsSuccess <= -1)
			{
				return false;
			}
			if (flag2 && fromID.repeatDelaySecondsFailed <= -1)
			{
				return false;
			}
			foreach (BaseMission.MissionInstance mission2 in missions)
			{
				if (mission2.missionID == missionID)
				{
					float num2 = 0f;
					if (mission2.status == BaseMission.MissionStatus.Completed)
					{
						num2 = fromID.repeatDelaySecondsSuccess;
					}
					else if (mission2.status == BaseMission.MissionStatus.Failed)
					{
						num2 = fromID.repeatDelaySecondsFailed;
					}
					float endTime = mission2.endTime;
					if (Time.time - endTime < num2)
					{
						return false;
					}
				}
			}
		}
		BaseMission.PositionGenerator[] positionGenerators = fromID.positionGenerators;
		for (int i = 0; i < positionGenerators.Length; i++)
		{
			if (!positionGenerators[i].Validate(this, fromID))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsMissionActive(uint missionID)
	{
		foreach (BaseMission.MissionInstance mission in missions)
		{
			if (mission.missionID == missionID && (mission.status == BaseMission.MissionStatus.Active || mission.status == BaseMission.MissionStatus.Accomplished))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasCompletedMission(uint missionID)
	{
		foreach (BaseMission.MissionInstance mission in missions)
		{
			if (mission.missionID == missionID && mission.status == BaseMission.MissionStatus.Completed)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasFailedMission(uint missionID)
	{
		foreach (BaseMission.MissionInstance mission in missions)
		{
			if (mission.missionID == missionID && mission.status == BaseMission.MissionStatus.Failed)
			{
				return true;
			}
		}
		return false;
	}

	private void WipeMissions()
	{
		if (missions.Count > 0)
		{
			for (int num = missions.Count - 1; num >= 0; num--)
			{
				BaseMission.MissionInstance missionInstance = missions[num];
				if (missionInstance != null)
				{
					missionInstance.GetMission().MissionFailed(missionInstance, this, BaseMission.MissionFailReason.ResetPlayerState);
					Pool.Free<BaseMission.MissionInstance>(ref missionInstance);
				}
			}
		}
		missions.Clear();
		SetActiveMission(-1);
		MissionDirty();
	}

	public void AbandonActiveMission()
	{
		if (HasActiveMission())
		{
			int activeMission = GetActiveMission();
			if (activeMission != -1 && activeMission < missions.Count)
			{
				BaseMission.MissionInstance missionInstance = missions[activeMission];
				missionInstance.GetMission().MissionFailed(missionInstance, this, BaseMission.MissionFailReason.Abandon);
			}
		}
	}

	public void ThinkMissions(float delta)
	{
		if (!BaseMission.missionsenabled)
		{
			return;
		}
		if (timeSinceMissionThink < thinkEvery)
		{
			timeSinceMissionThink += delta;
			return;
		}
		foreach (BaseMission.MissionInstance mission in missions)
		{
			mission.Think(this, timeSinceMissionThink);
		}
		timeSinceMissionThink = 0f;
	}

	public void MissionDirty(bool shouldSendNetworkUpdate = true)
	{
		if (BaseMission.missionsenabled)
		{
			State.missions = SaveMissions();
			DirtyPlayerState();
			if (shouldSendNetworkUpdate)
			{
				SendNetworkUpdate();
			}
		}
	}

	public void ProcessMissionEvent(BaseMission.MissionEventType type, uint identifier, float amount)
	{
		ProcessMissionEvent(type, new BaseMission.MissionEventPayload
		{
			UintIdentifier = identifier
		}, amount);
	}

	public void ProcessMissionEvent(BaseMission.MissionEventType type, uint identifier, float amount, Vector3 worldPos)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ProcessMissionEvent(type, new BaseMission.MissionEventPayload
		{
			UintIdentifier = identifier,
			WorldPosition = worldPos
		}, amount);
	}

	public void ProcessMissionEvent(BaseMission.MissionEventType type, int identifier, float amount)
	{
		ProcessMissionEvent(type, new BaseMission.MissionEventPayload
		{
			IntIdentifier = identifier
		}, amount);
	}

	public void ProcessMissionEvent(BaseMission.MissionEventType type, NetworkableId identifier, float amount)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ProcessMissionEvent(type, new BaseMission.MissionEventPayload
		{
			NetworkIdentifier = identifier
		}, amount);
	}

	public void ProcessMissionEvent(BaseMission.MissionEventType type, BaseMission.MissionEventPayload payload, float amount)
	{
		if (!BaseMission.missionsenabled)
		{
			return;
		}
		foreach (BaseMission.MissionInstance mission in missions)
		{
			mission.ProcessMissionEvent(this, type, payload, amount);
		}
	}

	private void AssignFollowUpMission()
	{
		if (followupMission != null && followupMissionProvider != null)
		{
			BaseMission.AssignMission(this, followupMissionProvider, followupMission);
		}
		followupMission = null;
		followupMissionProvider = null;
	}

	public void RegisterFollowupMission(BaseMission targetMission, IMissionProvider provider)
	{
		followupMission = targetMission;
		followupMissionProvider = provider;
		if (followupMission != null && followupMissionProvider != null)
		{
			((FacepunchBehaviour)this).Invoke((Action)AssignFollowUpMission, 1.5f);
		}
	}

	private Missions SaveMissions()
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		Missions val = Pool.Get<Missions>();
		val.missions = Pool.GetList<MissionInstance>();
		val.activeMission = GetActiveMission();
		val.protocol = 252;
		val.seed = World.Seed;
		val.saveCreatedTime = Epoch.FromDateTime(SaveRestore.SaveCreatedTime);
		foreach (BaseMission.MissionInstance mission in missions)
		{
			MissionInstance val2 = Pool.Get<MissionInstance>();
			val2.missionID = mission.missionID;
			val2.missionStatus = (uint)mission.status;
			val.missions.Add(val2);
			if (mission.status != BaseMission.MissionStatus.Active && mission.status != BaseMission.MissionStatus.Accomplished)
			{
				continue;
			}
			MissionInstanceData val3 = (val2.instanceData = Pool.Get<MissionInstanceData>());
			val3.providerID = mission.providerID;
			val3.startTime = Time.time - mission.startTime;
			val3.endTime = mission.endTime;
			val3.missionLocation = mission.missionLocation;
			val3.missionPoints = Pool.GetList<MissionPoint>();
			foreach (KeyValuePair<string, Vector3> missionPoint in mission.missionPoints)
			{
				MissionPoint val4 = Pool.Get<MissionPoint>();
				val4.identifier = missionPoint.Key;
				val4.location = missionPoint.Value;
				val3.missionPoints.Add(val4);
			}
			val3.objectiveStatuses = Pool.GetList<ObjectiveStatus>();
			BaseMission.MissionInstance.ObjectiveStatus[] objectiveStatuses = mission.objectiveStatuses;
			foreach (BaseMission.MissionInstance.ObjectiveStatus objectiveStatus in objectiveStatuses)
			{
				ObjectiveStatus val5 = Pool.Get<ObjectiveStatus>();
				val5.completed = objectiveStatus.completed;
				val5.failed = objectiveStatus.failed;
				val5.started = objectiveStatus.started;
				val5.progressCurrent = objectiveStatus.progressCurrent;
				val5.progressTarget = objectiveStatus.progressTarget;
				val3.objectiveStatuses.Add(val5);
			}
			val3.missionEntities = Pool.GetList<MissionEntity>();
			foreach (KeyValuePair<string, MissionEntity> missionEntity in mission.missionEntities)
			{
				BaseEntity baseEntity = (((Object)(object)missionEntity.Value != (Object)null) ? missionEntity.Value.GetEntity() : null);
				if (baseEntity.IsValid())
				{
					MissionEntity val6 = Pool.Get<MissionEntity>();
					val6.identifier = missionEntity.Key;
					val6.entityID = baseEntity.net.ID;
					val3.missionEntities.Add(val6);
				}
			}
		}
		return val;
	}

	public void SetActiveMission(int index)
	{
		_ = _activeMission;
		_activeMission = index;
		if (IsInTutorial && (Object)(object)GetCurrentTutorialIsland() != (Object)null)
		{
			GetCurrentTutorialIsland().OnPlayerStartedMission(this);
		}
	}

	public int GetActiveMission()
	{
		return _activeMission;
	}

	public bool HasActiveMission()
	{
		return GetActiveMission() != -1;
	}

	public BaseMission.MissionInstance GetActiveMissionInstance()
	{
		int activeMission = GetActiveMission();
		if (activeMission >= 0 && activeMission < missions.Count)
		{
			return missions[activeMission];
		}
		return null;
	}

	private void LoadMissions(Missions loadedMissions)
	{
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		if (missions.Count > 0)
		{
			for (int num = missions.Count - 1; num >= 0; num--)
			{
				BaseMission.MissionInstance missionInstance = missions[num];
				if (missionInstance != null)
				{
					Pool.Free<BaseMission.MissionInstance>(ref missionInstance);
				}
			}
		}
		missions.Clear();
		if (base.isServer && loadedMissions != null)
		{
			int protocol = loadedMissions.protocol;
			uint seed = loadedMissions.seed;
			int saveCreatedTime = loadedMissions.saveCreatedTime;
			int num2 = Epoch.FromDateTime(SaveRestore.SaveCreatedTime);
			if (252 != protocol || World.Seed != seed || num2 != saveCreatedTime)
			{
				Debug.Log((object)"Missions were from old protocol or different seed, or not from a loaded save. Clearing");
				loadedMissions.activeMission = -1;
				SetActiveMission(-1);
				State.missions = SaveMissions();
				return;
			}
		}
		if (loadedMissions != null && loadedMissions.missions.Count > 0)
		{
			MissionEntity missionEntity2 = default(MissionEntity);
			foreach (MissionInstance mission2 in loadedMissions.missions)
			{
				if (MissionManifest.GetFromID(mission2.missionID) == null)
				{
					continue;
				}
				BaseMission.MissionInstance missionInstance2 = Pool.Get<BaseMission.MissionInstance>();
				missionInstance2.missionID = mission2.missionID;
				missionInstance2.status = (BaseMission.MissionStatus)mission2.missionStatus;
				MissionInstanceData instanceData = mission2.instanceData;
				if (instanceData != null)
				{
					missionInstance2.providerID = instanceData.providerID;
					missionInstance2.startTime = Time.time - instanceData.startTime;
					missionInstance2.endTime = instanceData.endTime;
					missionInstance2.missionLocation = instanceData.missionLocation;
					if (base.isServer && instanceData.missionPoints != null)
					{
						foreach (MissionPoint missionPoint in instanceData.missionPoints)
						{
							missionInstance2.missionPoints.Add(missionPoint.identifier, missionPoint.location);
							BaseMission.AddBlocker(missionPoint.location);
						}
					}
					missionInstance2.objectiveStatuses = new BaseMission.MissionInstance.ObjectiveStatus[instanceData.objectiveStatuses.Count];
					for (int i = 0; i < instanceData.objectiveStatuses.Count; i++)
					{
						ObjectiveStatus val = instanceData.objectiveStatuses[i];
						BaseMission.MissionInstance.ObjectiveStatus objectiveStatus = new BaseMission.MissionInstance.ObjectiveStatus();
						objectiveStatus.completed = val.completed;
						objectiveStatus.failed = val.failed;
						objectiveStatus.started = val.started;
						objectiveStatus.progressCurrent = val.progressCurrent;
						objectiveStatus.progressTarget = val.progressTarget;
						missionInstance2.objectiveStatuses[i] = objectiveStatus;
					}
					if (base.isServer && instanceData.missionEntities != null)
					{
						missionInstance2.missionEntities.Clear();
						BaseMission mission = missionInstance2.GetMission();
						foreach (MissionEntity missionEntity3 in instanceData.missionEntities)
						{
							MissionEntity missionEntity = null;
							BaseNetworkable baseNetworkable = ((missionEntity3.entityID != default(NetworkableId)) ? BaseNetworkable.serverEntities.Find(missionEntity3.entityID) : null);
							if ((Object)(object)baseNetworkable != (Object)null)
							{
								missionEntity = (((Component)baseNetworkable).gameObject.TryGetComponent<MissionEntity>(ref missionEntity2) ? missionEntity2 : ((Component)baseNetworkable).gameObject.AddComponent<MissionEntity>());
								BaseMission.MissionEntityEntry missionEntityEntry = ((mission != null) ? List.FindWith<BaseMission.MissionEntityEntry, string>((IReadOnlyCollection<BaseMission.MissionEntityEntry>)(object)mission.missionEntities, (Func<BaseMission.MissionEntityEntry, string>)((BaseMission.MissionEntityEntry ed) => ed.identifier), missionEntity3.identifier, (IEqualityComparer<string>)null) : null);
								missionEntity.Setup(this, missionInstance2, missionEntity3.identifier, missionEntityEntry?.cleanupOnMissionSuccess ?? true, missionEntityEntry?.cleanupOnMissionFailed ?? true);
							}
							missionInstance2.missionEntities.Add(missionEntity3.identifier, missionEntity);
						}
					}
				}
				missions.Add(missionInstance2);
			}
			SetActiveMission(loadedMissions.activeMission);
		}
		else
		{
			SetActiveMission(-1);
		}
		if (base.isServer)
		{
			GetActiveMissionInstance()?.PostServerLoad(this);
		}
	}

	private void UpdateModelState()
	{
		if (!IsDead() && !IsSpectating())
		{
			wantsSendModelState = true;
		}
	}

	public void SendModelState(bool force = false)
	{
		if (force || (wantsSendModelState && !(nextModelStateUpdate > Time.time)))
		{
			wantsSendModelState = false;
			nextModelStateUpdate = Time.time + 0.1f;
			if (!IsDead() && !IsSpectating())
			{
				modelState.sleeping = IsSleeping();
				modelState.mounted = isMounted;
				modelState.relaxed = IsRelaxed();
				modelState.onPhone = HasActiveTelephone && !activeTelephone.IsMobile;
				modelState.crawling = IsCrawling();
				modelState.loading = IsLoadingAfterTransfer();
				ClientRPC<ModelState>(RpcTarget.NetworkGroup("OnModelState"), modelState);
			}
		}
	}

	public BaseMountable GetMounted()
	{
		return mounted.Get(base.isServer) as BaseMountable;
	}

	public BaseVehicle GetMountedVehicle()
	{
		BaseMountable baseMountable = GetMounted();
		if (!baseMountable.IsValid())
		{
			return null;
		}
		return baseMountable.VehicleParent();
	}

	public void MarkSwapSeat()
	{
		nextSeatSwapTime = Time.time + 0.75f;
	}

	public bool SwapSeatCooldown()
	{
		return Time.time < nextSeatSwapTime;
	}

	public bool CanMountMountablesNow()
	{
		if (!IsDead())
		{
			return !IsWounded();
		}
		return false;
	}

	public void MountObject(BaseMountable mount, int desiredSeat = 0)
	{
		mounted.Set(mount);
		SendNetworkUpdate();
	}

	public void EnsureDismounted()
	{
		if (isMounted)
		{
			GetMounted().DismountPlayer(this);
		}
	}

	public virtual void DismountObject()
	{
		mounted.Set(null);
		SendNetworkUpdate();
		PauseSpeedHackDetection(5f);
		PauseVehicleNoClipDetection(5f);
	}

	public void HandleMountedOnLoad()
	{
		if (!mounted.IsValid(base.isServer))
		{
			return;
		}
		BaseMountable baseMountable = mounted.Get(base.isServer) as BaseMountable;
		if ((Object)(object)baseMountable != (Object)null)
		{
			baseMountable.MountPlayer(this);
			if (!AllowSleeperMounting(baseMountable))
			{
				baseMountable.DismountPlayer(this);
			}
		}
		else
		{
			mounted.Set(null);
		}
	}

	public bool AllowSleeperMounting(BaseMountable mountable)
	{
		if (mountable.allowSleeperMounting)
		{
			return true;
		}
		if (!IsLoadingAfterTransfer())
		{
			return IsTransferProtected();
		}
		return true;
	}

	public PlayerSecondaryData SaveSecondaryData()
	{
		PlayerSecondaryData val = Pool.Get<PlayerSecondaryData>();
		val.userId = userID;
		PlayerState val2 = State.Copy();
		if (val2.pointsOfInterest != null)
		{
			Pool.FreeListAndItems<MapNote>(ref val2.pointsOfInterest);
		}
		if (val2.pings != null)
		{
			Pool.FreeListAndItems<MapNote>(ref val2.pings);
		}
		MapNote deathMarker = val2.deathMarker;
		if (deathMarker != null)
		{
			deathMarker.Dispose();
		}
		val2.deathMarker = null;
		Missions obj = val2.missions;
		if (obj != null)
		{
			obj.Dispose();
		}
		val2.missions = null;
		val.playerState = val2;
		if (currentTeam != 0L)
		{
			RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindTeam(currentTeam);
			if (playerTeam != null)
			{
				val.teamId = playerTeam.teamID;
				val.isTeamLeader = playerTeam.teamLeader == (ulong)userID;
			}
		}
		val.relationships = Pool.GetList<RelationshipData>();
		foreach (RelationshipManager.PlayerRelationshipInfo value in RelationshipManager.ServerInstance.GetRelationships(userID).relations.Values)
		{
			RelationshipData val3 = Pool.Get<RelationshipData>();
			val3.info = value.ToProto();
			val3.mugshotData = GetPoolableMugshotData(value);
			val.relationships.Add(val3);
		}
		return val;
		ArraySegment<byte> GetPoolableMugshotData(RelationshipManager.PlayerRelationshipInfo relationshipInfo)
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			if (relationshipInfo.mugshotCrc == 0)
			{
				return default(ArraySegment<byte>);
			}
			try
			{
				uint steamIdHash = RelationshipManager.GetSteamIdHash(userID, relationshipInfo.player);
				byte[] array = FileStorage.server.Get(relationshipInfo.mugshotCrc, FileStorage.Type.jpg, RelationshipManager.ServerInstance.net.ID, steamIdHash);
				if (array == null)
				{
					return default(ArraySegment<byte>);
				}
				byte[] array2 = ArrayPool<byte>.Shared.Rent(array.Length);
				new Span<byte>(array).CopyTo(Span<byte>.op_Implicit(array2));
				return new ArraySegment<byte>(array2, 0, array.Length);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				return default(ArraySegment<byte>);
			}
		}
	}

	public void LoadSecondaryData(PlayerSecondaryData data)
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (data == null)
		{
			return;
		}
		if (data.userId != (ulong)userID)
		{
			Debug.LogError((object)$"Attempted to load secondary data with an incorrect userID! Expected {data.userId} but player has {userID}, not loading it.");
			return;
		}
		if (data.playerState != null)
		{
			State.unHostileTimestamp = data.playerState.unHostileTimestamp;
			DirtyPlayerState();
		}
		if (data.relationships == null)
		{
			return;
		}
		RelationshipManager.PlayerRelationships relationships = RelationshipManager.ServerInstance.GetRelationships(userID);
		relationships.relations.Clear();
		foreach (RelationshipData relationship in data.relationships)
		{
			if (relationship.mugshotData.Count > 0)
			{
				try
				{
					byte[] array = new byte[relationship.mugshotData.Count];
					MemoryExtensions.AsSpan<byte>(relationship.mugshotData).CopyTo(Span<byte>.op_Implicit(array));
					uint steamIdHash = RelationshipManager.GetSteamIdHash(userID, relationship.info.playerID);
					uint num = FileStorage.server.Store(array, FileStorage.Type.jpg, RelationshipManager.ServerInstance.net.ID, steamIdHash);
					if (num != relationship.info.mugshotCrc)
					{
						Debug.LogWarning((object)$"Mugshot data for {userID}->{relationship.info.playerID} had a CRC mismatch, updating it");
						relationship.info.mugshotCrc = num;
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
			}
			relationships.relations.Add(relationship.info.playerID, RelationshipManager.PlayerRelationshipInfo.FromProto(relationship.info));
		}
		RelationshipManager.ServerInstance.MarkRelationshipsDirtyFor(this);
	}

	public override void DisableTransferProtection()
	{
		BaseVehicle vehicleParent = GetVehicleParent();
		if ((Object)(object)vehicleParent != (Object)null && vehicleParent.IsTransferProtected())
		{
			vehicleParent.DisableTransferProtection();
		}
		BaseMountable baseMountable = GetMounted();
		if ((Object)(object)baseMountable != (Object)null && baseMountable.IsTransferProtected())
		{
			baseMountable.DisableTransferProtection();
		}
		base.DisableTransferProtection();
	}

	public void KickAfterServerTransfer()
	{
		if (IsConnected)
		{
			Kick("Redirecting to another zone...");
		}
		Kill();
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(5uL)]
	private void RequestParachuteDeploy(RPCMessage msg)
	{
		RequestParachuteDeploy();
	}

	public void RequestParachuteDeploy()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (isMounted || !CheckParachuteClearance())
		{
			return;
		}
		Item slot = inventory.containerWear.GetSlot(7);
		ItemModParachute itemModParachute = default(ItemModParachute);
		if (slot == null || !(slot.conditionNormalized > 0f) || slot.isBroken || !((Component)slot.info).TryGetComponent<ItemModParachute>(ref itemModParachute))
		{
			return;
		}
		Parachute parachute = GameManager.server.CreateEntity(itemModParachute.ParachuteVehiclePrefab.resourcePath, ((Component)this).transform.position, eyes.rotation) as Parachute;
		if ((Object)(object)parachute != (Object)null)
		{
			parachute.skinID = slot.skin;
			parachute.Spawn();
			parachute.SetHealth(parachute.MaxHealth() * slot.conditionNormalized);
			parachute.AttemptMount(this);
			if (isMounted)
			{
				slot.Remove();
				ItemManager.DoRemoves();
				SendNetworkUpdate();
			}
			else
			{
				parachute.Kill();
			}
		}
	}

	public bool CheckParachuteClearance()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		if (!WaterLevel.Test(position - Vector3.up * 5f, waves: false, volumes: true, this) && !GamePhysics.Trace(new Ray(position, -Vector3.up), 1f, out var _, 6f, 1218543873, (QueryTriggerInteraction)0))
		{
			return !GamePhysics.CheckSphere(position + Vector3.up * 3.5f, 2f, 1218543873, (QueryTriggerInteraction)0);
		}
		return false;
	}

	public bool HasValidParachuteEquipped()
	{
		if ((Object)(object)inventory == (Object)null || inventory.containerWear == null)
		{
			return false;
		}
		Item slot = inventory.containerWear.GetSlot(7);
		ItemModParachute itemModParachute = default(ItemModParachute);
		if (slot != null && slot.conditionNormalized > 0f && !slot.isBroken && ((Component)slot.info).TryGetComponent<ItemModParachute>(ref itemModParachute))
		{
			return true;
		}
		return false;
	}

	public void ClearClientPetLink()
	{
		ClientRPC(RpcTarget.Player("CLIENT_SetPetPrefabID", this), 0, 0);
	}

	public void SendClientPetLink()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)PetEntity == (Object)null && BasePet.ActivePetByOwnerID.TryGetValue(userID, out var value) && (Object)(object)value.Brain != (Object)null)
		{
			value.Brain.SetOwningPlayer(this);
		}
		ClientRPC<uint, NetworkableId>(RpcTarget.Player("CLIENT_SetPetPrefabID", this), ((Object)(object)PetEntity != (Object)null) ? PetEntity.prefabID : 0u, (NetworkableId)(((Object)(object)PetEntity != (Object)null) ? PetEntity.net.ID : default(NetworkableId)));
		if ((Object)(object)PetEntity != (Object)null)
		{
			SendClientPetStateIndex();
		}
	}

	public void SendClientPetStateIndex()
	{
		BasePet basePet = PetEntity as BasePet;
		if (!((Object)(object)basePet == (Object)null))
		{
			ClientRPC(RpcTarget.Player("CLIENT_SetPetPetLoadedStateIndex", this), basePet.Brain.LoadedDesignIndex());
		}
	}

	[RPC_Server]
	private void IssuePetCommand(RPCMessage msg)
	{
		ParsePetCommand(msg, raycast: false);
	}

	[RPC_Server]
	private void IssuePetCommandRaycast(RPCMessage msg)
	{
		ParsePetCommand(msg, raycast: true);
	}

	private void ParsePetCommand(RPCMessage msg, bool raycast)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time - lastPetCommandIssuedTime <= 1f)
		{
			return;
		}
		lastPetCommandIssuedTime = Time.time;
		if (!((Object)(object)msg.player == (Object)null) && Pet != null && Pet.IsOwnedBy(msg.player))
		{
			int cmd = msg.read.Int32();
			int param = msg.read.Int32();
			if (raycast)
			{
				Ray value = msg.read.Ray();
				Pet.IssuePetCommand((PetCommandType)cmd, param, value);
			}
			else
			{
				Pet.IssuePetCommand((PetCommandType)cmd, param, null);
			}
		}
	}

	public bool CanPing(bool disregardHeldEntity = false)
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(base.isServer);
		if ((Object)(object)activeGameMode != (Object)null && !activeGameMode.allowPings)
		{
			return false;
		}
		if ((disregardHeldEntity || GetHeldEntity() is Binocular || (isMounted && GetMounted() is ComputerStation computerStation && computerStation.AllowPings())) && IsAlive() && !IsWounded())
		{
			return !IsSpectating();
		}
		return false;
	}

	public static PingStyle GetPingStyle(PingType t)
	{
		PingStyle pingStyle = default(PingStyle);
		return t switch
		{
			PingType.Hostile => HostileMarker, 
			PingType.GoTo => GoToMarker, 
			PingType.Dollar => DollarMarker, 
			PingType.Loot => LootMarker, 
			PingType.Node => NodeMarker, 
			PingType.Gun => GunMarker, 
			PingType.Build => BuildMarker, 
			_ => pingStyle, 
		};
	}

	private void ApplyPingStyle(MapNote note, PingType type)
	{
		PingStyle pingStyle = GetPingStyle(type);
		note.colourIndex = pingStyle.ColourIndex;
		note.icon = pingStyle.IconIndex;
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(3uL)]
	private void Server_AddPing(RPCMessage msg)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		if (State.pings == null)
		{
			State.pings = new List<MapNote>();
		}
		if (ConVar.Server.maximumPings == 0 || !CanPing())
		{
			return;
		}
		Vector3 val = msg.read.Vector3();
		PingType pingType = (PingType)Mathf.Clamp(msg.read.Int32(), 0, 6);
		bool wasViaWheel = msg.read.Bit();
		PingStyle pingStyle = GetPingStyle(pingType);
		foreach (MapNote ping in State.pings)
		{
			if (ping.icon == pingStyle.IconIndex)
			{
				Vector3 val2 = ping.worldPosition - val;
				if (((Vector3)(ref val2)).sqrMagnitude < 0.75f)
				{
					return;
				}
			}
		}
		if (State.pings.Count >= ConVar.Server.maximumPings)
		{
			State.pings.RemoveAt(0);
		}
		MapNote val3 = Pool.Get<MapNote>();
		val3.worldPosition = val;
		val3.isPing = true;
		val3.timeRemaining = (val3.totalDuration = ConVar.Server.pingDuration);
		ApplyPingStyle(val3, pingType);
		State.pings.Add(val3);
		DirtyPlayerState();
		SendPingsToClient();
		TeamUpdate(fullTeamUpdate: true);
		Analytics.Azure.OnPlayerPinged(this, pingType, wasViaWheel);
	}

	public void AddPingAtLocation(PingType type, Vector3 location, float time, NetworkableId associatedId)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (State.pings != null)
		{
			PingStyle pingStyle = GetPingStyle(type);
			foreach (MapNote ping in State.pings)
			{
				if (ping.icon == pingStyle.IconIndex && Vector3.Distance(location, ping.worldPosition) < 0.25f)
				{
					return;
				}
			}
		}
		if (State.pings == null)
		{
			State.pings = new List<MapNote>();
		}
		MapNote val = Pool.Get<MapNote>();
		val.worldPosition = location;
		val.isPing = true;
		val.timeRemaining = (val.totalDuration = time);
		val.associatedId = associatedId;
		ApplyPingStyle(val, type);
		State.pings.Add(val);
		DirtyPlayerState();
		SendPingsToClient();
		TeamUpdate(fullTeamUpdate: false);
	}

	public void RemovePingAtLocation(PingType type, Vector3 location, float tolerance, NetworkableId associatedId)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (State.pings == null)
		{
			return;
		}
		PingStyle pingStyle = GetPingStyle(type);
		for (int i = 0; i < State.pings.Count; i++)
		{
			MapNote val = State.pings[i];
			if (val.icon == pingStyle.IconIndex && Vector3.Distance(location, val.worldPosition) < tolerance)
			{
				State.pings.RemoveAt(i);
				DirtyPlayerState();
				SendPingsToClient();
				TeamUpdate(fullTeamUpdate: false);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(3uL)]
	private void Server_RemovePing(RPCMessage msg)
	{
		if (State.pings == null)
		{
			State.pings = new List<MapNote>();
		}
		int num = msg.read.Int32();
		if (num >= 0 && num < State.pings.Count)
		{
			State.pings.RemoveAt(num);
			DirtyPlayerState();
			SendPingsToClient();
			TeamUpdate(fullTeamUpdate: true);
		}
	}

	private void SendPingsToClient()
	{
		MapNoteList val = Pool.Get<MapNoteList>();
		try
		{
			val.notes = Pool.GetList<MapNote>();
			val.notes.AddRange(State.pings);
			ClientRPC<MapNoteList>(RpcTarget.Player("Client_ReceivePings", this), val);
			val.notes.Clear();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void TickPings()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (TimeSince.op_Implicit(lastTick) < 0.5f)
		{
			return;
		}
		TimeSince val = lastTick;
		lastTick = TimeSince.op_Implicit(0f);
		UpdateResourcePings();
		if (State.pings == null)
		{
			return;
		}
		List<MapNote> list = Pool.GetList<MapNote>();
		foreach (MapNote ping in State.pings)
		{
			ping.timeRemaining -= TimeSince.op_Implicit(val);
			if (ping.timeRemaining <= 0f)
			{
				list.Add(ping);
			}
		}
		int count = list.Count;
		foreach (MapNote item in list)
		{
			if (State.pings.Contains(item))
			{
				State.pings.Remove(item);
			}
		}
		Pool.FreeList<MapNote>(ref list);
		if (count > 0)
		{
			DirtyPlayerState();
			SendPingsToClient();
			TeamUpdate(fullTeamUpdate: true);
		}
	}

	public void RegisterPingedEntity(BaseEntity entity, PingType type)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!pingedEntities.Contains((entity.net.ID, type)))
		{
			pingedEntities.Add((entity.net.ID, type));
		}
	}

	public void DeregisterPingedEntitiesOfType(PingType type)
	{
		List<(NetworkableId, PingType)> list = Pool.GetList<(NetworkableId, PingType)>();
		foreach (var pingedEntity in pingedEntities)
		{
			if (pingedEntity.pingType == type)
			{
				list.Add(pingedEntity);
			}
		}
		foreach (var item in list)
		{
			pingedEntities.Remove(item);
		}
		Pool.FreeList<(NetworkableId, PingType)>(ref list);
	}

	public void DeregisterPingedEntity(NetworkableId id, PingType type)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (!pingedEntities.Contains((id, type)))
		{
			return;
		}
		pingedEntities.Remove((id, type));
		for (int i = 0; i < State.pings.Count; i++)
		{
			if (State.pings[i].associatedId == id)
			{
				State.pings.RemoveAt(i);
				break;
			}
		}
		DirtyPlayerState();
		SendPingsToClient();
	}

	public void EnableResourcePings(ItemDefinition forItem, PingType pingType)
	{
		if (!tutorialDesiredResource.Contains((forItem, pingType)))
		{
			tutorialDesiredResource.Add((forItem, pingType));
		}
	}

	private void UpdateResourcePings()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		if (State == null || TimeSince.op_Implicit(lastResourcePingUpdate) < 3f || !IsInTutorial)
		{
			return;
		}
		lastResourcePingUpdate = TimeSince.op_Implicit(0f);
		if (State.pings == null)
		{
			State.pings = new List<MapNote>();
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		List<(BaseEntity, PingType)> list2 = Pool.GetList<(BaseEntity, PingType)>();
		List<BaseEntity> list3 = Pool.GetList<BaseEntity>();
		ResourceDispenser resourceDispenser = default(ResourceDispenser);
		foreach (var item2 in tutorialDesiredResource)
		{
			list3.Clear();
			Enumerator<Networkable> enumerator2 = net.group.networkables.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Networkable current2 = enumerator2.Current;
					BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(current2.ID) as BaseEntity;
					if ((Object)(object)baseEntity != (Object)null && Distance(baseEntity) < 128f && baseEntity.isServer)
					{
						if (((Component)baseEntity).TryGetComponent<ResourceDispenser>(ref resourceDispenser) && resourceDispenser.HasItemToDispense(item2.item))
						{
							list3.Add(baseEntity);
						}
						else if (baseEntity is CollectibleEntity collectibleEntity && collectibleEntity.HasItem(item2.item))
						{
							list3.Add(baseEntity);
						}
						else if (baseEntity is StorageContainer storageContainer && storageContainer.inventory != null && storageContainer.inventory.HasItem(item2.item))
						{
							list3.Add(baseEntity);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			if (list3.Count <= 0)
			{
				continue;
			}
			float num = float.MaxValue;
			BaseEntity baseEntity2 = null;
			foreach (BaseEntity item3 in list3)
			{
				float num2 = Distance(item3);
				if (num2 < num)
				{
					num = num2;
					baseEntity2 = item3;
				}
			}
			if ((Object)(object)baseEntity2 != (Object)null)
			{
				list2.Add((baseEntity2, item2.pingType));
			}
		}
		List<(NetworkableId, PingType)> list4 = Pool.GetList<(NetworkableId, PingType)>();
		foreach (var pingedEntity in pingedEntities)
		{
			BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(pingedEntity.id);
			if ((Object)(object)baseNetworkable != (Object)null && !baseNetworkable.IsDestroyed)
			{
				list2.Add((baseNetworkable as BaseEntity, pingedEntity.pingType));
			}
			else
			{
				list4.Add(pingedEntity);
			}
		}
		foreach (var item4 in list4)
		{
			pingedEntities.Remove(item4);
		}
		Pool.FreeList<(NetworkableId, PingType)>(ref list4);
		Pool.FreeList<BaseEntity>(ref list3);
		List<MapNote> list5 = Pool.GetList<MapNote>();
		foreach (MapNote ping in State.pings)
		{
			if (ping.associatedId.Value == 0L)
			{
				continue;
			}
			bool flag = false;
			foreach (var item5 in list2)
			{
				if (ping.associatedId == item5.Item1.net.ID)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(ping.associatedId);
				if ((Object)(object)baseNetworkable2 != (Object)null && baseNetworkable2 is IEntityPingSource entityPingSource && entityPingSource.IsPingValid(ping))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				list5.Add(ping);
			}
		}
		bool flag2 = list5.Count > 0;
		foreach (MapNote item6 in list5)
		{
			if (State.pings.Contains(item6))
			{
				State.pings.Remove(item6);
			}
		}
		Pool.FreeList<MapNote>(ref list5);
		foreach (var item7 in list2)
		{
			if (HasPingForEntity(item7.Item1))
			{
				continue;
			}
			PingType item = item7.Item2;
			foreach (var pingedEntity2 in pingedEntities)
			{
				if (pingedEntity2.id == item7.Item1.net.ID)
				{
					item = pingedEntity2.pingType;
				}
			}
			State.pings.Add(CreatePingForEntity(item7.Item1, item));
			flag2 = true;
		}
		if (flag2)
		{
			DirtyPlayerState();
			SendPingsToClient();
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	private MapNote CreatePingForEntity(BaseEntity baseEntity, PingType type)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		MapNote val = Pool.Get<MapNote>();
		val.worldPosition = ((Component)baseEntity).transform.position;
		val.isPing = true;
		val.timeRemaining = (val.totalDuration = 30f);
		val.associatedId = baseEntity.net.ID;
		ApplyPingStyle(val, type);
		return val;
	}

	private bool HasPingForEntity(BaseEntity ent)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return HasPingForEntity(ent.net.ID);
	}

	private bool HasPingForEntity(NetworkableId id)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		foreach (MapNote ping in State.pings)
		{
			if (ping.associatedId == id)
			{
				return true;
			}
		}
		return false;
	}

	public void DisableResourcePings(ItemDefinition forItem, PingType type)
	{
		if (tutorialDesiredResource.Contains((forItem, type)))
		{
			tutorialDesiredResource.Remove((forItem, type));
		}
		if (tutorialDesiredResource.Count == 0)
		{
			UpdateResourcePings();
		}
	}

	private void ClearAllPings()
	{
		if (State != null && State.pings != null)
		{
			State.pings.Clear();
		}
		tutorialDesiredResource.Clear();
		pingedEntities.Clear();
	}

	public void DirtyPlayerState()
	{
		_playerStateDirty = true;
	}

	public void SavePlayerState()
	{
		if (_playerStateDirty)
		{
			_playerStateDirty = false;
			SingletonComponent<ServerMgr>.Instance.playerStateManager.Save(userID);
		}
	}

	public void ResetPlayerState()
	{
		SingletonComponent<ServerMgr>.Instance.playerStateManager.Reset(userID);
		ClientRPC(RpcTarget.Player("SetHostileLength", this), 0f);
		SendMarkersToClient();
		WipeMissions();
		MissionDirty();
	}

	public bool IsSleeping()
	{
		return HasPlayerFlag(PlayerFlags.Sleeping);
	}

	public bool IsSpectating()
	{
		return HasPlayerFlag(PlayerFlags.Spectating);
	}

	public bool IsRelaxed()
	{
		return HasPlayerFlag(PlayerFlags.Relaxed);
	}

	public bool IsServerFalling()
	{
		return HasPlayerFlag(PlayerFlags.ServerFall);
	}

	public bool IsLoadingAfterTransfer()
	{
		return HasPlayerFlag(PlayerFlags.LoadingAfterTransfer);
	}

	public bool CanBuild()
	{
		if (IsBuildingBlockedByVehicle())
		{
			return false;
		}
		if (IsBuildingBlockedByEntity())
		{
			return false;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return true;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool CanBuild(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		OBB obb = default(OBB);
		((OBB)(ref obb))._002Ector(position, rotation, bounds);
		if (IsBuildingBlockedByVehicle(obb))
		{
			return false;
		}
		if (IsBuildingBlockedByEntity(obb))
		{
			return false;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return true;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool CanBuild(OBB obb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (IsBuildingBlockedByVehicle(obb))
		{
			return false;
		}
		if (IsBuildingBlockedByEntity(obb))
		{
			return false;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return true;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingBlocked()
	{
		if (IsBuildingBlockedByVehicle())
		{
			return true;
		}
		if (IsBuildingBlockedByEntity())
		{
			return true;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return !buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingBlocked(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		OBB obb = default(OBB);
		((OBB)(ref obb))._002Ector(position, rotation, bounds);
		if (IsBuildingBlockedByVehicle(obb))
		{
			return true;
		}
		if (IsBuildingBlockedByEntity(obb))
		{
			return true;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return !buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingBlocked(OBB obb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (IsBuildingBlockedByVehicle(obb))
		{
			return true;
		}
		if (IsBuildingBlockedByEntity(obb))
		{
			return true;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return !buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingAuthed()
	{
		if (IsBuildingBlockedByVehicle())
		{
			return false;
		}
		if (IsBuildingBlockedByEntity())
		{
			return false;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingAuthed(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		OBB obb = default(OBB);
		((OBB)(ref obb))._002Ector(position, rotation, bounds);
		if (IsBuildingBlockedByVehicle(obb))
		{
			return false;
		}
		if (IsBuildingBlockedByEntity(obb))
		{
			return false;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingAuthed(OBB obb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (IsBuildingBlockedByVehicle(obb))
		{
			return false;
		}
		if (IsBuildingBlockedByEntity(obb))
		{
			return false;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool CanPlaceBuildingPrivilege()
	{
		if (IsBuildingBlockedByVehicle())
		{
			return false;
		}
		if (IsBuildingBlockedByEntity())
		{
			return false;
		}
		return (Object)(object)GetBuildingPrivilege() == (Object)null;
	}

	public bool CanPlaceBuildingPrivilege(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		OBB obb = default(OBB);
		((OBB)(ref obb))._002Ector(position, rotation, bounds);
		if (IsBuildingBlockedByVehicle(obb))
		{
			return false;
		}
		if (IsBuildingBlockedByEntity(obb))
		{
			return false;
		}
		return (Object)(object)GetBuildingPrivilege(obb) == (Object)null;
	}

	public bool CanPlaceBuildingPrivilege(OBB obb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (IsBuildingBlockedByVehicle(obb))
		{
			return false;
		}
		if (IsBuildingBlockedByEntity(obb))
		{
			return false;
		}
		return (Object)(object)GetBuildingPrivilege(obb) == (Object)null;
	}

	public bool IsNearEnemyBase()
	{
		if (IsBuildingBlockedByVehicle())
		{
			return true;
		}
		if (IsBuildingBlockedByEntity())
		{
			return true;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		if (!buildingPrivilege.IsAuthed(this))
		{
			return buildingPrivilege.AnyAuthed();
		}
		return false;
	}

	public bool IsNearEnemyBase(Vector3 position, Quaternion rotation, Bounds bounds)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		OBB obb = default(OBB);
		((OBB)(ref obb))._002Ector(position, rotation, bounds);
		if (IsBuildingBlockedByVehicle(obb))
		{
			return true;
		}
		if (IsBuildingBlockedByEntity(obb))
		{
			return true;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		if (!buildingPrivilege.IsAuthed(this))
		{
			return buildingPrivilege.AnyAuthed();
		}
		return false;
	}

	public bool IsNearEnemyBase(OBB obb)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (IsBuildingBlockedByVehicle(obb))
		{
			return true;
		}
		if (IsBuildingBlockedByEntity(obb))
		{
			return true;
		}
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege(obb);
		if ((Object)(object)buildingPrivilege == (Object)null)
		{
			return false;
		}
		if (!buildingPrivilege.IsAuthed(this))
		{
			return buildingPrivilege.AnyAuthed();
		}
		return false;
	}

	public bool IsBuildingBlockedByVehicle()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return IsBuildingBlockedByVehicle(WorldSpaceBounds());
	}

	public bool IsBuildingBlockedByEntity()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return IsBuildingBlockedByEntity(WorldSpaceBounds());
	}

	public bool HasPrivilegeFromOther()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time - cachedPrivilegeFromOtherTime > 1f)
		{
			cachedPrivilegeFromOtherTime = Time.time;
			cachedPrivilegeFromOther = null;
			IsBuildingBlockedByEntity(WorldSpaceBounds());
			IsBuildingBlockedByVehicle(WorldSpaceBounds());
		}
		return (Object)(object)cachedPrivilegeFromOther != (Object)null;
	}

	private bool IsBuildingBlockedByVehicle(OBB obb)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		List<BaseVehicle> list = Pool.GetList<BaseVehicle>();
		Vis.Entities(obb.position, 2f + ((Vector3)(ref obb.extents)).magnitude, list, 134217728, (QueryTriggerInteraction)2);
		for (int i = 0; i < list.Count; i++)
		{
			BaseVehicle baseVehicle = list[i];
			if (baseVehicle.isServer == base.isServer && !baseVehicle.IsDead() && !(((OBB)(ref obb)).Distance(baseVehicle.WorldSpaceBounds()) > 2f))
			{
				if (!baseVehicle.IsAuthed(this))
				{
					Pool.FreeList<BaseVehicle>(ref list);
					return true;
				}
				cachedPrivilegeFromOther = baseVehicle;
			}
		}
		Pool.FreeList<BaseVehicle>(ref list);
		return false;
	}

	private bool IsBuildingBlockedByEntity(OBB obb)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(obb.position, 3f + ((Vector3)(ref obb.extents)).magnitude, list, 2097152, (QueryTriggerInteraction)2);
		for (int i = 0; i < list.Count; i++)
		{
			BaseEntity baseEntity = list[i];
			if (baseEntity.isServer != base.isServer || ((OBB)(ref obb)).Distance(baseEntity.WorldSpaceBounds()) > 3f)
			{
				continue;
			}
			EntityPrivilege entityBuildingPrivilege = baseEntity.GetEntityBuildingPrivilege();
			if (!((Object)(object)entityBuildingPrivilege == (Object)null))
			{
				if (!entityBuildingPrivilege.IsAuthed(this))
				{
					Pool.FreeList<BaseEntity>(ref list);
					return true;
				}
				cachedPrivilegeFromOther = baseEntity;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return false;
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void OnProjectileAttack(RPCMessage msg)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_1518: Unknown result type (might be due to invalid IL or missing references)
		//IL_151d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1524: Unknown result type (might be due to invalid IL or missing references)
		//IL_1529: Unknown result type (might be due to invalid IL or missing references)
		//IL_1565: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0671: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_088f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1050: Unknown result type (might be due to invalid IL or missing references)
		//IL_1055: Unknown result type (might be due to invalid IL or missing references)
		//IL_1058: Unknown result type (might be due to invalid IL or missing references)
		//IL_105d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1060: Unknown result type (might be due to invalid IL or missing references)
		//IL_1065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b67: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b90: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_108a: Unknown result type (might be due to invalid IL or missing references)
		//IL_108c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1091: Unknown result type (might be due to invalid IL or missing references)
		//IL_1093: Unknown result type (might be due to invalid IL or missing references)
		//IL_1098: Unknown result type (might be due to invalid IL or missing references)
		//IL_109a: Unknown result type (might be due to invalid IL or missing references)
		//IL_109f: Unknown result type (might be due to invalid IL or missing references)
		//IL_106b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1073: Unknown result type (might be due to invalid IL or missing references)
		//IL_107d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1082: Unknown result type (might be due to invalid IL or missing references)
		//IL_1087: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_10af: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_110a: Unknown result type (might be due to invalid IL or missing references)
		//IL_110c: Unknown result type (might be due to invalid IL or missing references)
		//IL_110e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0de6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1124: Unknown result type (might be due to invalid IL or missing references)
		//IL_1126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0df7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1141: Unknown result type (might be due to invalid IL or missing references)
		//IL_1143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0783: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1154: Unknown result type (might be due to invalid IL or missing references)
		//IL_1156: Unknown result type (might be due to invalid IL or missing references)
		//IL_1191: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_11cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1392: Unknown result type (might be due to invalid IL or missing references)
		//IL_1397: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_13cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_130c: Unknown result type (might be due to invalid IL or missing references)
		//IL_130e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1329: Unknown result type (might be due to invalid IL or missing references)
		//IL_132b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1347: Unknown result type (might be due to invalid IL or missing references)
		//IL_1349: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_13fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_146a: Unknown result type (might be due to invalid IL or missing references)
		//IL_146c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1486: Unknown result type (might be due to invalid IL or missing references)
		//IL_1488: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c3: Unknown result type (might be due to invalid IL or missing references)
		PlayerProjectileAttack val = PlayerProjectileAttack.Deserialize((Stream)(object)msg.read);
		if (val == null)
		{
			return;
		}
		PlayerAttack playerAttack = val.playerAttack;
		HitInfo hitInfo = new HitInfo();
		hitInfo.LoadFromAttack(playerAttack.attack, serverSide: true);
		hitInfo.Initiator = this;
		hitInfo.ProjectileID = playerAttack.projectileID;
		hitInfo.ProjectileDistance = val.hitDistance;
		hitInfo.ProjectileVelocity = val.hitVelocity;
		hitInfo.ProjectileTravelTime = val.travelTime;
		hitInfo.Predicted = msg.connection;
		if (hitInfo.IsNaNOrInfinity() || float.IsNaN(val.travelTime) || float.IsInfinity(val.travelTime))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + playerAttack.projectileID + ")");
			val.ResetToPool();
			val = null;
			stats.combat.LogInvalid(hitInfo, "projectile_nan");
			return;
		}
		if (!firedProjectiles.TryGetValue(playerAttack.projectileID, out var value))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + playerAttack.projectileID + ")", logToAnalytics: false);
			val.ResetToPool();
			val = null;
			stats.combat.LogInvalid(hitInfo, "projectile_invalid");
			return;
		}
		hitInfo.ProjectileHits = value.hits;
		hitInfo.ProjectileIntegrity = value.integrity;
		hitInfo.ProjectileTrajectoryMismatch = value.trajectoryMismatch;
		if (value.integrity <= 0f)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Integrity is zero (" + playerAttack.projectileID + ")");
			Analytics.Azure.OnProjectileHackViolation(value);
			val.ResetToPool();
			val = null;
			stats.combat.LogInvalid(hitInfo, "projectile_integrity");
			return;
		}
		if (value.firedTime < Time.realtimeSinceStartup - 8f)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + playerAttack.projectileID + ")");
			Analytics.Azure.OnProjectileHackViolation(value);
			val.ResetToPool();
			val = null;
			stats.combat.LogInvalid(hitInfo, "projectile_lifetime");
			return;
		}
		if (value.ricochets > 0)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile is ricochet (" + playerAttack.projectileID + ")");
			Analytics.Azure.OnProjectileHackViolation(value);
			val.ResetToPool();
			val = null;
			stats.combat.LogInvalid(hitInfo, "projectile_ricochet");
			return;
		}
		hitInfo.Weapon = value.weaponSource;
		hitInfo.WeaponPrefab = value.weaponPrefab;
		hitInfo.ProjectilePrefab = value.projectilePrefab;
		hitInfo.damageProperties = value.projectilePrefab.damageProperties;
		Vector3 position = value.position;
		Vector3 initialPositionOffset = value.initialPositionOffset;
		Vector3 positionOffset = value.positionOffset;
		Vector3 velocity = value.velocity;
		float partialTime = value.partialTime;
		float travelTime = value.travelTime;
		float num = Mathf.Clamp(val.travelTime, value.travelTime, 8f);
		Vector3 gravity = Physics.gravity * value.projectilePrefab.gravityModifier;
		float drag = value.projectilePrefab.drag;
		BaseEntity hitEntity = hitInfo.HitEntity;
		BasePlayer basePlayer = hitEntity as BasePlayer;
		bool flag = (Object)(object)basePlayer != (Object)null;
		bool flag2 = flag && basePlayer.IsSleeping();
		bool flag3 = flag && basePlayer.IsWounded();
		bool flag4 = flag && basePlayer.isMounted;
		bool flag5 = flag && basePlayer.HasParent();
		bool flag6 = (Object)(object)hitEntity != (Object)null;
		bool flag7 = flag6 && hitEntity.IsNpc;
		bool flag8 = hitInfo.HitMaterial == Projectile.WaterMaterialID();
		bool flag9;
		int num15;
		Vector3 val2;
		Vector3 position2;
		Vector3 pointStart;
		Vector3 val4;
		Vector3 val5;
		bool flag10;
		int num36;
		if (value.protection > 0)
		{
			flag9 = true;
			float num2 = 1f + ConVar.AntiHack.projectile_forgiveness;
			float num3 = 1f - ConVar.AntiHack.projectile_forgiveness;
			float projectile_clientframes = ConVar.AntiHack.projectile_clientframes;
			float projectile_serverframes = ConVar.AntiHack.projectile_serverframes;
			float num4 = Mathx.Decrement(value.firedTime);
			float num5 = Mathf.Clamp(Mathx.Increment(Time.realtimeSinceStartup) - num4, 0f, 8f);
			float num6 = num;
			float num7 = (value.desyncLifeTime = Mathf.Abs(num5 - num6));
			float num8 = Mathf.Min(num5, num6);
			float num9 = projectile_clientframes / 60f;
			float num10 = projectile_serverframes * Mathx.Max(Time.deltaTime, Time.smoothDeltaTime, Time.fixedDeltaTime);
			float num11 = (desyncTimeClamped + num8 + num9 + num10) * num2;
			float num12 = ((value.protection >= 6) ? ((desyncTimeClamped + num9 + num10) * num2) : num11);
			float num13 = (num5 - desyncTimeClamped - num9 - num10) * num3;
			float num14 = Vector3.Distance(value.initialPosition, hitInfo.HitPositionWorld);
			num15 = 1075904512;
			if (ConVar.AntiHack.projectile_terraincheck)
			{
				num15 |= 0x800000;
			}
			if (ConVar.AntiHack.projectile_vehiclecheck)
			{
				num15 |= 0x8000000;
			}
			if (flag && hitInfo.boneArea == (HitArea)(-1))
			{
				string name = ((Object)hitInfo.ProjectilePrefab).name;
				string text = (flag6 ? hitEntity.ShortPrefabName : "world");
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Bone is invalid (" + name + " on " + text + " bone " + hitInfo.HitBone + ")");
				stats.combat.LogInvalid(hitInfo, "projectile_bone");
				flag9 = false;
			}
			if (flag8)
			{
				if (flag6)
				{
					string name2 = ((Object)hitInfo.ProjectilePrefab).name;
					string text2 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile water hit on entity (" + name2 + " on " + text2 + ")");
					Analytics.Azure.OnProjectileHackViolation(value);
					stats.combat.LogInvalid(hitInfo, "water_entity");
					flag9 = false;
				}
				if (!WaterLevel.Test(hitInfo.HitPositionWorld - 0.5f * Vector3.up, waves: true, volumes: true, this))
				{
					string name3 = ((Object)hitInfo.ProjectilePrefab).name;
					string text3 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile water level (" + name3 + " on " + text3 + ")");
					Analytics.Azure.OnProjectileHackViolation(value);
					stats.combat.LogInvalid(hitInfo, "water_level");
					flag9 = false;
				}
			}
			if (value.protection >= 2)
			{
				if (flag6 || (value.protection < 6 && flag))
				{
					float num16 = hitEntity.MaxVelocity();
					val2 = hitEntity.GetParentVelocity();
					float num17 = num16 + ((Vector3)(ref val2)).magnitude;
					float num18 = hitEntity.BoundsPadding() + num12 * num17;
					float num19 = hitEntity.Distance(hitInfo.HitPositionWorld);
					if (num19 > num18)
					{
						string name4 = ((Object)hitInfo.ProjectilePrefab).name;
						string shortPrefabName = hitEntity.ShortPrefabName;
						AntiHack.Log(this, AntiHackType.ProjectileHack, "Entity too far away (" + name4 + " on " + shortPrefabName + " with " + num19 + "m > " + num18 + "m in " + num12 + "s)");
						Analytics.Azure.OnProjectileHackViolation(value);
						stats.combat.LogInvalid(hitInfo, "entity_distance");
						flag9 = false;
					}
				}
				if (value.protection >= 6 && flag9 && flag && !flag7 && !flag2 && !flag3 && !flag4 && !flag5)
				{
					val2 = basePlayer.GetParentVelocity();
					float magnitude = ((Vector3)(ref val2)).magnitude;
					float num20 = basePlayer.BoundsPadding() + num12 * magnitude + ConVar.AntiHack.tickhistoryforgiveness;
					float num21 = basePlayer.tickHistory.Distance(basePlayer, hitInfo.HitPositionWorld);
					if (num21 > num20)
					{
						string name5 = ((Object)hitInfo.ProjectilePrefab).name;
						string shortPrefabName2 = basePlayer.ShortPrefabName;
						AntiHack.Log(this, AntiHackType.ProjectileHack, "Player too far away (" + name5 + " on " + shortPrefabName2 + " with " + num21 + "m > " + num20 + "m in " + num12 + "s)");
						Analytics.Azure.OnProjectileHackViolation(value);
						stats.combat.LogInvalid(hitInfo, "player_distance");
						flag9 = false;
					}
				}
			}
			if (value.protection >= 1)
			{
				float num22;
				if (!flag6)
				{
					num22 = 0f;
				}
				else
				{
					float num23 = hitEntity.MaxVelocity();
					val2 = hitEntity.GetParentVelocity();
					num22 = num23 + ((Vector3)(ref val2)).magnitude;
				}
				float num24 = num22;
				float num25 = (flag6 ? (num12 * num24) : 0f);
				float magnitude2 = ((Vector3)(ref value.initialVelocity)).magnitude;
				float num26 = hitInfo.ProjectilePrefab.initialDistance + num11 * magnitude2;
				float num27 = hitInfo.ProjectileDistance + 1f + ((Vector3)(ref positionOffset)).magnitude + num25;
				val2 = estimatedVelocity;
				float num28 = num27 + ((Vector3)(ref val2)).magnitude;
				if (num14 > num26)
				{
					string name6 = ((Object)hitInfo.ProjectilePrefab).name;
					string text4 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile too fast (" + name6 + " on " + text4 + " with " + num14 + "m > " + num26 + "m in " + num11 + "s)");
					Analytics.Azure.OnProjectileHackViolation(value);
					stats.combat.LogInvalid(hitInfo, "projectile_maxspeed");
					flag9 = false;
				}
				if (num14 > num28)
				{
					string name7 = ((Object)hitInfo.ProjectilePrefab).name;
					string text5 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile too far away (" + name7 + " on " + text5 + " with " + num14 + "m > " + num28 + "m in " + num11 + "s)");
					Analytics.Azure.OnProjectileHackViolation(value);
					stats.combat.LogInvalid(hitInfo, "projectile_distance");
					flag9 = false;
				}
				if (num7 > ConVar.AntiHack.projectile_desync)
				{
					string name8 = ((Object)hitInfo.ProjectilePrefab).name;
					string text6 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile desync (" + name8 + " on " + text6 + " with " + num7 + "s > " + ConVar.AntiHack.projectile_desync + "s)");
					Analytics.Azure.OnProjectileHackViolation(value);
					stats.combat.LogInvalid(hitInfo, "projectile_desync");
					flag9 = false;
				}
			}
			if (value.protection >= 4)
			{
				float num29 = 0f;
				if (flag6)
				{
					val2 = hitEntity.GetParentVelocity();
					float num30 = ((Vector3)(ref val2)).magnitude;
					if (hitEntity is CargoShip || hitEntity is Tugboat)
					{
						num30 += hitEntity.MaxVelocity();
					}
					num29 = num12 * num30;
				}
				SimulateProjectile(ref position, ref velocity, ref partialTime, num - travelTime, gravity, drag, out var prevPosition, out var prevVelocity);
				Line val3 = default(Line);
				((Line)(ref val3))._002Ector(prevPosition - prevVelocity, position + prevVelocity);
				float num31 = Mathf.Max(((Line)(ref val3)).Distance(hitInfo.PointStart) - ((Vector3)(ref initialPositionOffset)).magnitude - num29, 0f);
				float num32 = Mathf.Max(((Line)(ref val3)).Distance(hitInfo.HitPositionWorld) - ((Vector3)(ref initialPositionOffset)).magnitude - num29, 0f);
				if (num31 > ConVar.AntiHack.projectile_trajectory)
				{
					string name9 = ((Object)value.projectilePrefab).name;
					string text7 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Start position trajectory (" + name9 + " on " + text7 + " with " + num31 + "m > " + ConVar.AntiHack.projectile_trajectory + "m)");
					Analytics.Azure.OnProjectileHackViolation(value);
					stats.combat.LogInvalid(hitInfo, "trajectory_start");
					flag9 = false;
				}
				if (num32 > ConVar.AntiHack.projectile_trajectory)
				{
					string name10 = ((Object)value.projectilePrefab).name;
					string text8 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "End position trajectory (" + name10 + " on " + text8 + " with " + num32 + "m > " + ConVar.AntiHack.projectile_trajectory + "m)");
					Analytics.Azure.OnProjectileHackViolation(value);
					stats.combat.LogInvalid(hitInfo, "trajectory_end");
					flag9 = false;
				}
				if (hitInfo.ProjectileTrajectoryMismatch > ConVar.AntiHack.projectile_trajectory)
				{
					string name11 = ((Object)value.projectilePrefab).name;
					string text9 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Update position trajectory (" + name11 + " on " + text9 + " with " + hitInfo.ProjectileTrajectoryMismatch + "m > " + ConVar.AntiHack.projectile_trajectory + "m)");
					Analytics.Azure.OnProjectileHackViolation(value);
					stats.combat.LogInvalid(hitInfo, "trajectory_update_total");
					flag9 = false;
				}
				hitInfo.ProjectileVelocity = velocity;
				if (val.hitVelocity != Vector3.zero && velocity != Vector3.zero)
				{
					float num33 = Vector3.Angle(val.hitVelocity, velocity);
					float num34 = ((Vector3)(ref val.hitVelocity)).magnitude / ((Vector3)(ref velocity)).magnitude;
					if (num33 > ConVar.AntiHack.projectile_anglechange)
					{
						string name12 = ((Object)value.projectilePrefab).name;
						string text10 = (flag6 ? hitEntity.ShortPrefabName : "world");
						AntiHack.Log(this, AntiHackType.ProjectileHack, "Trajectory angle change (" + name12 + " on " + text10 + " with " + num33 + "deg > " + ConVar.AntiHack.projectile_anglechange + "deg)");
						Analytics.Azure.OnProjectileHackViolation(value);
						stats.combat.LogInvalid(hitInfo, "angle_change");
						flag9 = false;
					}
					if (num34 > ConVar.AntiHack.projectile_velocitychange)
					{
						string name13 = ((Object)value.projectilePrefab).name;
						string text11 = (flag6 ? hitEntity.ShortPrefabName : "world");
						AntiHack.Log(this, AntiHackType.ProjectileHack, "Trajectory velocity change (" + name13 + " on " + text11 + " with " + num34 + " > " + ConVar.AntiHack.projectile_velocitychange + ")");
						Analytics.Azure.OnProjectileHackViolation(value);
						stats.combat.LogInvalid(hitInfo, "velocity_change");
						flag9 = false;
					}
				}
				float magnitude3 = ((Vector3)(ref velocity)).magnitude;
				float num35 = num13 * magnitude3;
				if (num14 < num35)
				{
					string name14 = ((Object)hitInfo.ProjectilePrefab).name;
					string text12 = (flag6 ? hitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile too slow (" + name14 + " on " + text12 + " with " + num14 + "m < " + num35 + "m in " + num13 + "s)");
					Analytics.Azure.OnProjectileHackViolation(value);
					stats.combat.LogInvalid(hitInfo, "projectile_minspeed");
					flag9 = false;
				}
			}
			if (value.protection >= 3)
			{
				position2 = value.position;
				pointStart = hitInfo.PointStart;
				val4 = hitInfo.HitPositionWorld;
				if (!flag8)
				{
					val4 -= ((Vector3)(ref hitInfo.ProjectileVelocity)).normalized * 0.001f;
				}
				val5 = hitInfo.PositionOnRay(val4);
				Vector3 val6 = Vector3.zero;
				Vector3 val7 = Vector3.zero;
				if (ConVar.AntiHack.projectile_backtracking > 0f)
				{
					val2 = pointStart - position2;
					val6 = ((Vector3)(ref val2)).normalized * ConVar.AntiHack.projectile_backtracking;
					val2 = val5 - pointStart;
					val7 = ((Vector3)(ref val2)).normalized * ConVar.AntiHack.projectile_backtracking;
				}
				flag10 = GamePhysics.LineOfSight(position2 - val6, pointStart + val6, num15, value.lastEntityHit) && GamePhysics.LineOfSight(pointStart - val7, val5, num15, value.lastEntityHit) && GamePhysics.LineOfSight(val5, val4, num15, value.lastEntityHit);
				bool flag11 = true;
				if (flag10)
				{
					flag11 = GamePhysics.LineOfSight(position2, val4, num15, value.lastEntityHit) && GamePhysics.LineOfSight(val4, position2, num15, value.lastEntityHit);
				}
				bool flag12 = true;
				if (flag10)
				{
					List<Vector3> simulatedPositions = value.simulatedPositions;
					if (simulatedPositions.Count > ConVar.AntiHack.projectile_update_limit)
					{
						flag12 = false;
					}
					else
					{
						simulatedPositions.Add(position2);
						for (int i = 1; i < simulatedPositions.Count; i++)
						{
							if (!GamePhysics.LineOfSight(simulatedPositions[i - 1], simulatedPositions[i], num15, value.lastEntityHit) || !GamePhysics.LineOfSight(simulatedPositions[i], simulatedPositions[i - 1], num15, value.lastEntityHit))
							{
								flag12 = false;
								break;
							}
						}
					}
				}
				if (flag10)
				{
					if (!(value.simulatedPositions.Count > 1 && flag12))
					{
						num36 = ((value.simulatedPositions.Count <= 1 && flag11) ? 1 : 0);
						if (num36 == 0)
						{
							goto IL_122d;
						}
					}
					else
					{
						num36 = 1;
					}
					stats.Add("hit_" + (flag6 ? hitEntity.Categorize() : "world") + "_direct_los", 1, Stats.Server);
					goto IL_128b;
				}
				num36 = 0;
				goto IL_122d;
			}
			goto IL_14fd;
		}
		goto IL_1516;
		IL_128b:
		if (num36 == 0)
		{
			string name15 = ((Object)hitInfo.ProjectilePrefab).name;
			string text13 = (flag6 ? hitEntity.ShortPrefabName : "world");
			string description = ((!flag10) ? "projectile_los" : "projectile_los_detailed");
			string[] obj = new string[12]
			{
				"Line of sight (", name15, " on ", text13, ") ", null, null, null, null, null,
				null, null
			};
			val2 = position2;
			obj[5] = ((object)(Vector3)(ref val2)).ToString();
			obj[6] = " ";
			val2 = pointStart;
			obj[7] = ((object)(Vector3)(ref val2)).ToString();
			obj[8] = " ";
			val2 = val5;
			obj[9] = ((object)(Vector3)(ref val2)).ToString();
			obj[10] = " ";
			val2 = val4;
			obj[11] = ((object)(Vector3)(ref val2)).ToString();
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(obj));
			Analytics.Azure.OnProjectileHackViolation(value);
			stats.combat.LogInvalid(hitInfo, description);
			flag9 = false;
		}
		if (flag9 && flag && !flag7)
		{
			Vector3 hitPositionWorld = hitInfo.HitPositionWorld;
			Vector3 position3 = basePlayer.eyes.position;
			Vector3 val8 = basePlayer.CenterPoint();
			float projectile_losforgiveness = ConVar.AntiHack.projectile_losforgiveness;
			bool flag13 = GamePhysics.LineOfSight(hitPositionWorld, position3, num15, 0f, projectile_losforgiveness) && GamePhysics.LineOfSight(position3, hitPositionWorld, num15, projectile_losforgiveness, 0f);
			if (!flag13)
			{
				flag13 = GamePhysics.LineOfSight(hitPositionWorld, val8, num15, 0f, projectile_losforgiveness) && GamePhysics.LineOfSight(val8, hitPositionWorld, num15, projectile_losforgiveness, 0f);
			}
			if (!flag13)
			{
				string name16 = ((Object)hitInfo.ProjectilePrefab).name;
				string text14 = (flag6 ? hitEntity.ShortPrefabName : "world");
				string[] obj2 = new string[12]
				{
					"Line of sight (", name16, " on ", text14, ") ", null, null, null, null, null,
					null, null
				};
				val2 = hitPositionWorld;
				obj2[5] = ((object)(Vector3)(ref val2)).ToString();
				obj2[6] = " ";
				val2 = position3;
				obj2[7] = ((object)(Vector3)(ref val2)).ToString();
				obj2[8] = " or ";
				val2 = hitPositionWorld;
				obj2[9] = ((object)(Vector3)(ref val2)).ToString();
				obj2[10] = " ";
				val2 = val8;
				obj2[11] = ((object)(Vector3)(ref val2)).ToString();
				AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(obj2));
				Analytics.Azure.OnProjectileHackViolation(value);
				stats.combat.LogInvalid(hitInfo, "projectile_los");
				flag9 = false;
			}
		}
		goto IL_14fd;
		IL_14fd:
		if (!flag9)
		{
			AntiHack.AddViolation(this, AntiHackType.ProjectileHack, ConVar.AntiHack.projectile_penalty);
			val.ResetToPool();
			val = null;
			return;
		}
		goto IL_1516;
		IL_122d:
		stats.Add("hit_" + (flag6 ? hitEntity.Categorize() : "world") + "_indirect_los", 1, Stats.Server);
		goto IL_128b;
		IL_1516:
		value.position = hitInfo.HitPositionWorld;
		value.velocity = val.hitVelocity;
		value.travelTime = num;
		value.partialTime = partialTime;
		value.hits++;
		value.lastEntityHit = hitEntity;
		value.simulatedPositions.Clear();
		value.simulatedPositions.Add(position);
		hitInfo.ProjectilePrefab.CalculateDamage(hitInfo, value.projectileModifier, value.integrity);
		if (flag8)
		{
			if (hitInfo.ProjectilePrefab.waterIntegrityLoss > 0f)
			{
				value.integrity = Mathf.Clamp01(value.integrity - hitInfo.ProjectilePrefab.waterIntegrityLoss);
			}
		}
		else if (hitInfo.ProjectilePrefab.penetrationPower <= 0f || !flag6)
		{
			value.integrity = 0f;
		}
		else
		{
			float num37 = hitEntity.PenetrationResistance(hitInfo) / hitInfo.ProjectilePrefab.penetrationPower;
			value.integrity = Mathf.Clamp01(value.integrity - num37);
		}
		if (flag6)
		{
			stats.Add(value.itemMod.category + "_hit_" + hitEntity.Categorize(), 1);
		}
		if (value.integrity <= 0f)
		{
			if (value.hits <= ConVar.AntiHack.projectile_impactspawndepth)
			{
				value.itemMod.ServerProjectileHit(hitInfo);
			}
			if (hitInfo.ProjectilePrefab.remainInWorld)
			{
				CreateWorldProjectile(hitInfo, value.itemDef, value.itemMod, hitInfo.ProjectilePrefab, value.pickupItem);
			}
		}
		else if (value.hits == ConVar.AntiHack.projectile_impactspawndepth)
		{
			value.itemMod.ServerProjectileHit(hitInfo);
		}
		firedProjectiles[playerAttack.projectileID] = value;
		if (flag6)
		{
			if (value.hits <= ConVar.AntiHack.projectile_damagedepth)
			{
				hitEntity.OnAttacked(hitInfo);
			}
			else
			{
				stats.combat.LogInvalid(hitInfo, "ricochet");
			}
		}
		hitInfo.DoHitEffects = hitInfo.ProjectilePrefab.doDefaultHitEffects;
		Effect.server.ImpactEffect(hitInfo);
		val.ResetToPool();
		val = null;
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void OnProjectileRicochet(RPCMessage msg)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		PlayerProjectileRicochet val = PlayerProjectileRicochet.Deserialize((Stream)(object)msg.read);
		if (val != null)
		{
			FiredProjectile value;
			if (Vector3Ex.IsNaNOrInfinity(val.hitPosition) || Vector3Ex.IsNaNOrInfinity(val.inVelocity) || Vector3Ex.IsNaNOrInfinity(val.outVelocity) || Vector3Ex.IsNaNOrInfinity(val.hitNormal) || float.IsNaN(val.travelTime) || float.IsInfinity(val.travelTime))
			{
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + val.projectileID + ")");
				val.ResetToPool();
				val = null;
			}
			else if (!firedProjectiles.TryGetValue(val.projectileID, out value))
			{
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + val.projectileID + ")", logToAnalytics: false);
				val.ResetToPool();
				val = null;
			}
			else if (value.firedTime < Time.realtimeSinceStartup - 8f)
			{
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + val.projectileID + ")");
				val.ResetToPool();
				val = null;
			}
			else
			{
				value.ricochets++;
				firedProjectiles[val.projectileID] = value;
				val.ResetToPool();
				val = null;
			}
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void OnProjectileUpdate(RPCMessage msg)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0801: Unknown result type (might be due to invalid IL or missing references)
		//IL_0828: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_061d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0622: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0604: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_0612: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0740: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0495: Unknown result type (might be due to invalid IL or missing references)
		//IL_049a: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		PlayerProjectileUpdate val = PlayerProjectileUpdate.Deserialize((Stream)(object)msg.read);
		if (val == null)
		{
			return;
		}
		if (Vector3Ex.IsNaNOrInfinity(val.curPosition) || Vector3Ex.IsNaNOrInfinity(val.curVelocity) || float.IsNaN(val.travelTime) || float.IsInfinity(val.travelTime))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + val.projectileID + ")");
			val.ResetToPool();
			val = null;
			return;
		}
		if (!firedProjectiles.TryGetValue(val.projectileID, out var value))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Missing ID (" + val.projectileID + ")", logToAnalytics: false);
			val.ResetToPool();
			val = null;
			return;
		}
		if (value.firedTime < Time.realtimeSinceStartup - 8f)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Lifetime is zero (" + val.projectileID + ")");
			Analytics.Azure.OnProjectileHackViolation(value);
			val.ResetToPool();
			val = null;
			return;
		}
		if (value.ricochets > 0)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile is ricochet (" + val.projectileID + ")");
			Analytics.Azure.OnProjectileHackViolation(value);
			val.ResetToPool();
			val = null;
			return;
		}
		Vector3 position = value.position;
		Vector3 positionOffset = value.positionOffset;
		Vector3 velocity = value.velocity;
		float num = value.trajectoryMismatch;
		float partialTime = value.partialTime;
		float travelTime = value.travelTime;
		float num2 = Mathf.Clamp(val.travelTime, value.travelTime, 8f);
		Vector3 val2 = Physics.gravity * value.projectilePrefab.gravityModifier;
		float drag = value.projectilePrefab.drag;
		if (value.protection > 0)
		{
			float num3 = 1f - ConVar.AntiHack.projectile_forgiveness;
			float num4 = 1f + ConVar.AntiHack.projectile_forgiveness;
			float projectile_clientframes = ConVar.AntiHack.projectile_clientframes;
			float projectile_serverframes = ConVar.AntiHack.projectile_serverframes;
			float num5 = Mathx.Decrement(value.firedTime);
			float num6 = Mathf.Clamp(Mathx.Increment(Time.realtimeSinceStartup) - num5, 0f, 8f);
			float num7 = num2;
			float num8 = (value.desyncLifeTime = Mathf.Abs(num6 - num7));
			float num9 = Mathf.Min(num6, num7);
			float num10 = projectile_clientframes / 60f;
			float num11 = projectile_serverframes * Mathx.Max(Time.deltaTime, Time.smoothDeltaTime, Time.fixedDeltaTime);
			float num12 = (num9 + desyncTimeClamped + num10 + num11) * num4;
			float num13 = Mathf.Max(0f, (num9 - desyncTimeClamped - num10 - num11) * num3);
			int num14 = 1075904512;
			if (ConVar.AntiHack.projectile_terraincheck)
			{
				num14 |= 0x800000;
			}
			if (ConVar.AntiHack.projectile_vehiclecheck)
			{
				num14 |= 0x8000000;
			}
			if (value.protection >= 1)
			{
				float num15 = value.projectilePrefab.initialDistance + num12 * ((Vector3)(ref value.initialVelocity)).magnitude;
				float num16 = Vector3.Distance(value.initialPosition, val.curPosition);
				if (num16 > num15)
				{
					string name = ((Object)value.projectilePrefab).name;
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile distance (" + name + " with " + num16 + "m > " + num15 + "m in " + num12 + "s)");
					Analytics.Azure.OnProjectileHackViolation(value);
					val.ResetToPool();
					val = null;
					return;
				}
				if (num8 > ConVar.AntiHack.projectile_desync)
				{
					string name2 = ((Object)value.projectilePrefab).name;
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile desync (" + name2 + " with " + num8 + "s > " + ConVar.AntiHack.projectile_desync + "s)");
					Analytics.Azure.OnProjectileHackViolation(value);
					val.ResetToPool();
					val = null;
					return;
				}
				Vector3 curVelocity = val.curVelocity;
				Vector3 val3 = value.initialVelocity;
				Vector3 val4 = ((value.hits == 0) ? val3 : value.velocity);
				float num17 = drag * (1f / 32f);
				Vector3 val5 = val2 * (1f / 32f);
				int num18 = Mathf.FloorToInt(num13 / (1f / 32f));
				int num19 = Mathf.CeilToInt(num12 / (1f / 32f));
				for (int i = 0; i < num18; i++)
				{
					val3 += val5;
					val3 -= val3 * num17;
					val4 += val5;
					val4 -= val4 * num17;
				}
				float magnitude = ((Vector3)(ref curVelocity)).magnitude;
				float magnitude2 = ((Vector3)(ref val3)).magnitude;
				float magnitude3 = ((Vector3)(ref val4)).magnitude;
				for (int j = num18; j < num19; j++)
				{
					val3 += val5;
					val3 -= val3 * num17;
					val4 += val5;
					val4 -= val4 * num17;
				}
				magnitude3 = Mathf.Min(magnitude3, ((Vector3)(ref val4)).magnitude);
				magnitude2 = Mathf.Max(magnitude2, ((Vector3)(ref val3)).magnitude);
				if (magnitude < magnitude3 * num3)
				{
					string name3 = ((Object)value.projectilePrefab).name;
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile velocity too low (" + name3 + " with " + magnitude + " < " + magnitude3 + ")");
					Analytics.Azure.OnProjectileHackViolation(value);
					val.ResetToPool();
					val = null;
					return;
				}
				if (magnitude > magnitude2 * num4)
				{
					string name4 = ((Object)value.projectilePrefab).name;
					AntiHack.Log(this, AntiHackType.ProjectileHack, "Projectile velocity too high (" + name4 + " with " + magnitude + " > " + magnitude2 + ")");
					Analytics.Azure.OnProjectileHackViolation(value);
					val.ResetToPool();
					val = null;
					return;
				}
			}
			if (value.protection >= 3)
			{
				Vector3 position2 = value.position;
				Vector3 curPosition = val.curPosition;
				Vector3 val6 = Vector3.zero;
				Vector3 val7;
				if (ConVar.AntiHack.projectile_backtracking > 0f)
				{
					val7 = curPosition - position2;
					val6 = ((Vector3)(ref val7)).normalized * ConVar.AntiHack.projectile_backtracking;
				}
				if (!GamePhysics.LineOfSight(position2 - val6, curPosition + val6, num14, value.lastEntityHit))
				{
					string name5 = ((Object)value.projectilePrefab).name;
					string[] obj = new string[6] { "Line of sight (", name5, " on update) ", null, null, null };
					val7 = position2;
					obj[3] = ((object)(Vector3)(ref val7)).ToString();
					obj[4] = " ";
					val7 = curPosition;
					obj[5] = ((object)(Vector3)(ref val7)).ToString();
					AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(obj));
					Analytics.Azure.OnProjectileHackViolation(value);
					val.ResetToPool();
					val = null;
					return;
				}
			}
			if (value.protection >= 4)
			{
				SimulateProjectile(ref position, ref velocity, ref partialTime, num2 - travelTime, val2, drag, out var prevPosition, out var prevVelocity);
				value.simulatedPositions.Add(position);
				Line val8 = default(Line);
				((Line)(ref val8))._002Ector(prevPosition - prevVelocity, position + prevVelocity);
				num += Mathf.Max(((Line)(ref val8)).Distance(val.curPosition) - ((Vector3)(ref positionOffset)).magnitude, 0f);
			}
			if (value.protection >= 5)
			{
				if (value.inheritedVelocity != Vector3.zero)
				{
					Vector3 curVelocity2 = value.inheritedVelocity + velocity;
					Vector3 curVelocity3 = val.curVelocity;
					if (((Vector3)(ref curVelocity3)).magnitude > 2f * ((Vector3)(ref curVelocity2)).magnitude || ((Vector3)(ref curVelocity3)).magnitude < 0.5f * ((Vector3)(ref curVelocity2)).magnitude)
					{
						val.curVelocity = curVelocity2;
					}
					value.inheritedVelocity = Vector3.zero;
				}
				else
				{
					val.curVelocity = velocity;
				}
			}
		}
		value.updates.Add(new FiredProjectileUpdate
		{
			OldPosition = value.position,
			NewPosition = val.curPosition,
			OldVelocity = value.velocity,
			NewVelocity = val.curVelocity,
			Mismatch = num,
			PartialTime = partialTime
		});
		value.position = val.curPosition;
		value.velocity = val.curVelocity;
		value.travelTime = val.travelTime;
		value.partialTime = partialTime;
		value.trajectoryMismatch = num;
		value.positionOffset = default(Vector3);
		firedProjectiles[val.projectileID] = value;
		val.ResetToPool();
		val = null;
	}

	private void SimulateProjectile(ref Vector3 position, ref Vector3 velocity, ref float partialTime, float travelTime, Vector3 gravity, float drag, out Vector3 prevPosition, out Vector3 prevVelocity)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f / 32f;
		prevPosition = position;
		prevVelocity = velocity;
		if (partialTime > Mathf.Epsilon)
		{
			float num2 = num - partialTime;
			if (travelTime < num2)
			{
				prevPosition = position;
				prevVelocity = velocity;
				position += velocity * travelTime;
				partialTime += travelTime;
				return;
			}
			prevPosition = position;
			prevVelocity = velocity;
			position += velocity * num2;
			velocity += gravity * num;
			velocity -= velocity * (drag * num);
			travelTime -= num2;
		}
		int num3 = Mathf.FloorToInt(travelTime / num);
		for (int i = 0; i < num3; i++)
		{
			prevPosition = position;
			prevVelocity = velocity;
			position += velocity * num;
			velocity += gravity * num;
			velocity -= velocity * (drag * num);
		}
		partialTime = travelTime - num * (float)num3;
		if (partialTime > Mathf.Epsilon)
		{
			prevPosition = position;
			prevVelocity = velocity;
			position += velocity * partialTime;
		}
	}

	protected virtual void CreateWorldProjectile(HitInfo info, ItemDefinition itemDef, ItemModProjectile itemMod, Projectile projectilePrefab, Item recycleItem)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		Vector3 projectileVelocity = info.ProjectileVelocity;
		Item item = ((recycleItem != null) ? recycleItem : ItemManager.Create(itemDef, 1, 0uL));
		BaseEntity baseEntity = null;
		if (!info.DidHit)
		{
			baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3)(ref projectileVelocity)).normalized));
			baseEntity.Kill(DestroyMode.Gib);
			return;
		}
		if (projectilePrefab.breakProbability > 0f && Random.value <= projectilePrefab.breakProbability)
		{
			baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3)(ref projectileVelocity)).normalized));
			baseEntity.Kill(DestroyMode.Gib);
			return;
		}
		if (projectilePrefab.conditionLoss > 0f)
		{
			item.LoseCondition(projectilePrefab.conditionLoss * 100f);
			if (item.isBroken)
			{
				baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3)(ref projectileVelocity)).normalized));
				baseEntity.Kill(DestroyMode.Gib);
				return;
			}
		}
		if (projectilePrefab.stickProbability > 0f && Random.value <= projectilePrefab.stickProbability)
		{
			baseEntity = (((Object)(object)info.HitEntity == (Object)null) ? item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3)(ref projectileVelocity)).normalized)) : ((info.HitBone != 0) ? item.CreateWorldObject(info.HitPositionLocal, Quaternion.LookRotation(info.HitNormalLocal * -1f), info.HitEntity, info.HitBone) : item.CreateWorldObject(info.HitPositionLocal, Quaternion.LookRotation(((Component)info.HitEntity).transform.InverseTransformDirection(((Vector3)(ref projectileVelocity)).normalized)), info.HitEntity)));
			DroppedItem droppedItem = baseEntity as DroppedItem;
			if ((Object)(object)droppedItem != (Object)null)
			{
				droppedItem.StickIn();
			}
			else
			{
				((Component)baseEntity).GetComponent<Rigidbody>().isKinematic = true;
			}
		}
		else
		{
			baseEntity = item.CreateWorldObject(info.HitPositionWorld, Quaternion.LookRotation(((Vector3)(ref projectileVelocity)).normalized));
			Rigidbody component = ((Component)baseEntity).GetComponent<Rigidbody>();
			component.AddForce(((Vector3)(ref projectileVelocity)).normalized * 200f);
			component.WakeUp();
		}
	}

	public void CleanupExpiredProjectiles()
	{
		foreach (KeyValuePair<int, FiredProjectile> item in firedProjectiles.Where((KeyValuePair<int, FiredProjectile> x) => x.Value.firedTime < Time.realtimeSinceStartup - 8f - 1f).ToList())
		{
			Analytics.Azure.OnFiredProjectileRemoved(this, item.Value);
			firedProjectiles.Remove(item.Key);
			FiredProjectile value = item.Value;
			Pool.Free<FiredProjectile>(ref value);
		}
	}

	public bool HasFiredProjectile(int id)
	{
		return firedProjectiles.ContainsKey(id);
	}

	public void NoteFiredProjectile(int projectileid, Vector3 startPos, Vector3 startVel, AttackEntity attackEnt, ItemDefinition firedItemDef, Guid projectileGroupId, Vector3 positionOffset, Item pickupItem = null)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		BaseProjectile baseProjectile = attackEnt as BaseProjectile;
		ItemModProjectile component = ((Component)firedItemDef).GetComponent<ItemModProjectile>();
		Projectile component2 = component.projectileObject.Get().GetComponent<Projectile>();
		if (Vector3Ex.IsNaNOrInfinity(startPos) || Vector3Ex.IsNaNOrInfinity(startVel))
		{
			string name = ((Object)component2).name;
			AntiHack.Log(this, AntiHackType.ProjectileHack, "Contains NaN (" + name + ")");
			stats.combat.LogInvalid(this, baseProjectile, "projectile_nan");
			return;
		}
		int projectile_protection = ConVar.AntiHack.projectile_protection;
		Vector3 inheritedVelocity = (((Object)(object)attackEnt != (Object)null) ? attackEnt.GetInheritedVelocity(this, ((Vector3)(ref startVel)).normalized) : Vector3.zero);
		if (projectile_protection >= 1)
		{
			float num = 1f - ConVar.AntiHack.projectile_forgiveness;
			float num2 = 1f + ConVar.AntiHack.projectile_forgiveness;
			float magnitude = ((Vector3)(ref startVel)).magnitude;
			float num3 = component.GetMinVelocity();
			float num4 = component.GetMaxVelocity();
			BaseProjectile baseProjectile2 = attackEnt as BaseProjectile;
			if (Object.op_Implicit((Object)(object)baseProjectile2))
			{
				num3 *= baseProjectile2.GetProjectileVelocityScale();
				num4 *= baseProjectile2.GetProjectileVelocityScale(getMax: true);
			}
			num3 *= num;
			num4 *= num2;
			if (magnitude < num3)
			{
				string name2 = ((Object)component2).name;
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Velocity (" + name2 + " with " + magnitude + " < " + num3 + ")");
				stats.combat.LogInvalid(this, baseProjectile, "projectile_minvelocity");
				return;
			}
			if (magnitude > num4)
			{
				string name3 = ((Object)component2).name;
				AntiHack.Log(this, AntiHackType.ProjectileHack, "Velocity (" + name3 + " with " + magnitude + " > " + num4 + ")");
				stats.combat.LogInvalid(this, baseProjectile, "projectile_maxvelocity");
				return;
			}
		}
		FiredProjectile firedProjectile = Pool.Get<FiredProjectile>();
		firedProjectile.itemDef = firedItemDef;
		firedProjectile.itemMod = component;
		firedProjectile.projectilePrefab = component2;
		firedProjectile.firedTime = Time.realtimeSinceStartup;
		firedProjectile.travelTime = 0f;
		firedProjectile.weaponSource = attackEnt;
		firedProjectile.weaponPrefab = (((Object)(object)attackEnt == (Object)null) ? null : GameManager.server.FindPrefab(StringPool.Get(attackEnt.prefabID)).GetComponent<AttackEntity>());
		firedProjectile.projectileModifier = (((Object)(object)baseProjectile == (Object)null) ? Projectile.Modifier.Default : baseProjectile.GetProjectileModifier());
		firedProjectile.pickupItem = pickupItem;
		firedProjectile.integrity = 1f;
		firedProjectile.position = startPos;
		firedProjectile.initialPositionOffset = positionOffset;
		firedProjectile.positionOffset = positionOffset;
		firedProjectile.velocity = startVel;
		firedProjectile.initialPosition = startPos;
		firedProjectile.initialVelocity = startVel;
		firedProjectile.inheritedVelocity = inheritedVelocity;
		firedProjectile.protection = projectile_protection;
		firedProjectile.ricochets = 0;
		firedProjectile.hits = 0;
		firedProjectile.id = projectileid;
		firedProjectile.attacker = this;
		firedProjectiles.Add(projectileid, firedProjectile);
		Analytics.Azure.OnFiredProjectile(this, firedProjectile, projectileGroupId);
	}

	public void ServerNoteFiredProjectile(int projectileid, Vector3 startPos, Vector3 startVel, AttackEntity attackEnt, ItemDefinition firedItemDef, Item pickupItem = null)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		BaseProjectile baseProjectile = attackEnt as BaseProjectile;
		ItemModProjectile component = ((Component)firedItemDef).GetComponent<ItemModProjectile>();
		Projectile component2 = component.projectileObject.Get().GetComponent<Projectile>();
		int protection = 0;
		Vector3 zero = Vector3.zero;
		if (!Vector3Ex.IsNaNOrInfinity(startPos) && !Vector3Ex.IsNaNOrInfinity(startVel))
		{
			FiredProjectile firedProjectile = Pool.Get<FiredProjectile>();
			firedProjectile.itemDef = firedItemDef;
			firedProjectile.itemMod = component;
			firedProjectile.projectilePrefab = component2;
			firedProjectile.firedTime = Time.realtimeSinceStartup;
			firedProjectile.travelTime = 0f;
			firedProjectile.weaponSource = attackEnt;
			firedProjectile.weaponPrefab = (((Object)(object)attackEnt == (Object)null) ? null : GameManager.server.FindPrefab(StringPool.Get(attackEnt.prefabID)).GetComponent<AttackEntity>());
			firedProjectile.projectileModifier = (((Object)(object)baseProjectile == (Object)null) ? Projectile.Modifier.Default : baseProjectile.GetProjectileModifier());
			firedProjectile.pickupItem = pickupItem;
			firedProjectile.integrity = 1f;
			firedProjectile.trajectoryMismatch = 0f;
			firedProjectile.position = startPos;
			firedProjectile.positionOffset = Vector3.zero;
			firedProjectile.velocity = startVel;
			firedProjectile.initialPosition = startPos;
			firedProjectile.initialVelocity = startVel;
			firedProjectile.inheritedVelocity = zero;
			firedProjectile.protection = protection;
			firedProjectile.ricochets = 0;
			firedProjectile.hits = 0;
			firedProjectile.id = projectileid;
			firedProjectile.attacker = this;
			firedProjectile.simulatedPositions.Add(startPos);
			firedProjectiles.Add(projectileid, firedProjectile);
		}
	}

	public override bool CanUseNetworkCache(Connection connection)
	{
		if (net == null)
		{
			return true;
		}
		if (connection.authLevel != 0)
		{
			return false;
		}
		if (net.connection != connection)
		{
			return true;
		}
		return false;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		HandleMountedOnLoad();
	}

	public override void Save(SaveInfo info)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		bool flag = net != null && net.connection == info.forConnection;
		bool flag2 = !info.forDisk && (Object)(object)info.forConnection.player != (Object)null && info.forConnection.player is BasePlayer basePlayer && basePlayer.IsAdmin;
		info.msg.basePlayer = Pool.Get<BasePlayer>();
		info.msg.basePlayer.userid = userID;
		info.msg.basePlayer.name = displayName;
		info.msg.basePlayer.playerFlags = (int)playerFlags;
		info.msg.basePlayer.currentTeam = currentTeam;
		info.msg.basePlayer.heldEntity = svActiveItemID;
		info.msg.basePlayer.reputation = reputation;
		if (!info.forDisk && (Object)(object)currentGesture != (Object)null && currentGesture.animationType == GestureConfig.AnimationType.Loop)
		{
			info.msg.basePlayer.loopingGesture = currentGesture.gestureId;
		}
		if (IsConnected && (IsAdmin || IsDeveloper))
		{
			info.msg.basePlayer.skinCol = net.connection.info.GetFloat("global.skincol", -1f);
			info.msg.basePlayer.skinTex = net.connection.info.GetFloat("global.skintex", -1f);
			info.msg.basePlayer.skinMesh = net.connection.info.GetFloat("global.skinmesh", -1f);
		}
		else
		{
			info.msg.basePlayer.skinCol = -1f;
			info.msg.basePlayer.skinTex = -1f;
			info.msg.basePlayer.skinMesh = -1f;
		}
		info.msg.basePlayer.underwear = GetUnderwearSkin();
		if (info.forDisk || flag)
		{
			info.msg.basePlayer.metabolism = metabolism.Save();
			info.msg.basePlayer.modifiers = null;
			if ((Object)(object)modifiers != (Object)null)
			{
				info.msg.basePlayer.modifiers = modifiers.Save();
			}
		}
		if (!info.forDisk && !flag)
		{
			BasePlayer basePlayer2 = info.msg.basePlayer;
			basePlayer2.playerFlags &= -5;
			BasePlayer basePlayer3 = info.msg.basePlayer;
			basePlayer3.playerFlags &= -129;
			if (info.msg.baseCombat != null && !flag2)
			{
				info.msg.baseCombat.health = 100f;
			}
		}
		info.msg.basePlayer.inventory = inventory.Save(info.forDisk || flag);
		modelState.sleeping = IsSleeping();
		modelState.relaxed = IsRelaxed();
		modelState.crawling = IsCrawling();
		modelState.loading = IsLoadingAfterTransfer();
		info.msg.basePlayer.modelState = modelState.Copy();
		if (info.forDisk)
		{
			BaseEntity baseEntity = mounted.Get(base.isServer);
			if (baseEntity.IsValid())
			{
				if (baseEntity.enableSaving)
				{
					info.msg.basePlayer.mounted = mounted.uid;
				}
				else
				{
					BaseVehicle mountedVehicle = GetMountedVehicle();
					if (mountedVehicle.IsValid() && mountedVehicle.enableSaving)
					{
						info.msg.basePlayer.mounted = mountedVehicle.net.ID;
					}
				}
			}
			info.msg.basePlayer.respawnId = respawnId;
		}
		else
		{
			info.msg.basePlayer.mounted = mounted.uid;
		}
		if (flag)
		{
			info.msg.basePlayer.persistantData = PersistantPlayerInfo.Copy();
			if (!info.forDisk && State.missions != null)
			{
				info.msg.basePlayer.missions = State.missions.Copy();
			}
		}
		info.msg.basePlayer.bagCount = SleepingBag.GetSleepingBagCount(userID);
		info.msg.basePlayer.shelterCount = LegacyShelter.GetShelterCount(userID);
		if (info.forDisk)
		{
			info.msg.basePlayer.loadingTimeout = RealTimeUntil.op_Implicit(timeUntilLoadingExpires);
			info.msg.basePlayer.currentLife = lifeStory;
			info.msg.basePlayer.previousLife = previousLifeStory;
		}
		if (!info.forDisk)
		{
			info.msg.basePlayer.clanId = clanId;
		}
		if (info.forDisk)
		{
			info.msg.basePlayer.itemCrafter = inventory.crafting.Save();
		}
		if (info.forDisk)
		{
			SavePlayerState();
		}
		info.msg.basePlayer.tutorialAllowance = (int)CurrentTutorialAllowance;
	}

	public override void Load(LoadInfo info)
	{
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.basePlayer == null)
		{
			return;
		}
		BasePlayer basePlayer = info.msg.basePlayer;
		userID = basePlayer.userid;
		UserIDString = userID.ToString();
		if (basePlayer.name != null)
		{
			displayName = basePlayer.name;
		}
		_ = playerFlags;
		playerFlags = (PlayerFlags)basePlayer.playerFlags;
		currentTeam = basePlayer.currentTeam;
		reputation = basePlayer.reputation;
		if (basePlayer.metabolism != null)
		{
			metabolism.Load(basePlayer.metabolism);
		}
		if (basePlayer.modifiers != null && (Object)(object)modifiers != (Object)null)
		{
			modifiers.Load(basePlayer.modifiers);
		}
		if (basePlayer.inventory != null)
		{
			inventory.Load(basePlayer.inventory);
		}
		if (basePlayer.modelState != null)
		{
			if (modelState != null)
			{
				modelState.ResetToPool();
				modelState = null;
			}
			modelState = basePlayer.modelState;
			basePlayer.modelState = null;
		}
		if (info.fromDisk)
		{
			timeUntilLoadingExpires = RealTimeUntil.op_Implicit(info.msg.basePlayer.loadingTimeout);
			if (RealTimeUntil.op_Implicit(timeUntilLoadingExpires) > 0f)
			{
				float num = Mathf.Clamp(RealTimeUntil.op_Implicit(timeUntilLoadingExpires), 0f, Nexus.loadingTimeout);
				((FacepunchBehaviour)this).Invoke((Action)RemoveLoadingPlayerFlag, num);
			}
			lifeStory = info.msg.basePlayer.currentLife;
			if (lifeStory != null)
			{
				lifeStory.ShouldPool = false;
			}
			previousLifeStory = info.msg.basePlayer.previousLife;
			if (previousLifeStory != null)
			{
				previousLifeStory.ShouldPool = false;
			}
			SetPlayerFlag(PlayerFlags.Sleeping, b: false);
			StartSleeping();
			SetPlayerFlag(PlayerFlags.Connected, b: false);
			if (lifeStory == null && IsAlive())
			{
				LifeStoryStart();
			}
			mounted.uid = info.msg.basePlayer.mounted;
			if (IsWounded())
			{
				Die();
			}
			respawnId = info.msg.basePlayer.respawnId;
			if (info.msg.basePlayer.itemCrafter?.queue != null)
			{
				inventory.crafting.Load(info.msg.basePlayer.itemCrafter);
			}
		}
		if (!info.fromDisk)
		{
			clanId = info.msg.basePlayer.clanId;
		}
		CurrentTutorialAllowance = (TutorialItemAllowance)info.msg.basePlayer.tutorialAllowance;
	}

	internal override void OnParentRemoved()
	{
		if (IsNpc)
		{
			base.OnParentRemoved();
		}
		else
		{
			SetParent(null, worldPositionStays: true, sendImmediate: true);
		}
	}

	public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)oldParent != (Object)null)
		{
			TransformState(((Component)oldParent).transform.localToWorldMatrix);
		}
		if ((Object)(object)newParent != (Object)null)
		{
			TransformState(((Component)newParent).transform.worldToLocalMatrix);
		}
	}

	private void TransformState(Matrix4x4 matrix)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		tickInterpolator.TransformEntries(matrix);
		tickHistory.TransformEntries(matrix);
		Quaternion rotation = ((Matrix4x4)(ref matrix)).rotation;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0f, ((Quaternion)(ref rotation)).eulerAngles.y, 0f);
		eyes.bodyRotation = Quaternion.Euler(val) * eyes.bodyRotation;
	}

	public bool CanSuicide()
	{
		if (IsAdmin || IsDeveloper)
		{
			return true;
		}
		return Time.realtimeSinceStartup > nextSuicideTime;
	}

	public void MarkSuicide()
	{
		nextSuicideTime = Time.realtimeSinceStartup + 60f;
	}

	public bool CanRespawn()
	{
		return Time.realtimeSinceStartup > nextRespawnTime;
	}

	public void MarkRespawn(float nextSpawnDelay = 5f)
	{
		nextRespawnTime = Time.realtimeSinceStartup + nextSpawnDelay;
	}

	public Item GetActiveItem()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!((ItemId)(ref svActiveItemID)).IsValid)
		{
			return null;
		}
		if (IsDead())
		{
			return null;
		}
		if ((Object)(object)inventory == (Object)null || inventory.containerBelt == null)
		{
			return null;
		}
		return inventory.containerBelt.FindItemByUID(svActiveItemID);
	}

	public void MovePosition(Vector3 newPos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = newPos;
		if ((Object)(object)parentEntity.Get(base.isServer) != (Object)null)
		{
			tickInterpolator.Reset(((Component)parentEntity.Get(base.isServer)).transform.InverseTransformPoint(newPos));
		}
		else
		{
			tickInterpolator.Reset(newPos);
		}
		ticksPerSecond.Increment();
		tickHistory.AddPoint(newPos, tickHistoryCapacity);
		NetworkPositionTick();
	}

	public void OverrideViewAngles(Vector3 newAng)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		viewAngles = newAng;
	}

	public override void ServerInit()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		stats = new PlayerStatistics(this);
		if ((ulong)userID == 0L)
		{
			userID = (ulong)Random.Range(0, 10000000);
			UserIDString = userID.ToString();
			displayName = UserIDString;
			bots.Add(this);
		}
		EnablePlayerCollider();
		SetPlayerRigidbodyState(!IsSleeping());
		base.ServerInit();
		eyes.bodyRotation = ((Component)this).transform.rotation;
		Query.Server.AddPlayer(this);
		inventory.ServerInit(this);
		metabolism.ServerInit(this);
		if ((Object)(object)modifiers != (Object)null)
		{
			modifiers.ServerInit(this);
		}
		if (recentWaveTargets != null)
		{
			recentWaveTargets.Clear();
		}
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		Query.Server.RemovePlayer(this);
		if (Object.op_Implicit((Object)(object)inventory))
		{
			inventory.DoDestroy();
		}
		sleepingPlayerList.Remove(this);
		SavePlayerState();
		if (cachedPersistantPlayer != null)
		{
			Pool.Free<PersistantPlayer>(ref cachedPersistantPlayer);
		}
	}

	protected void ServerUpdate(float deltaTime)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (!((BaseNetwork)Net.sv).IsConnected())
		{
			return;
		}
		LifeStoryUpdate(deltaTime, IsOnGround() ? estimatedSpeed : 0f);
		FinalizeTick(deltaTime);
		ThinkMissions(deltaTime);
		desyncTimeRaw = Mathf.Max(timeSinceLastTick - deltaTime, 0f);
		desyncTimeClamped = Mathf.Min(desyncTimeRaw, ConVar.AntiHack.maxdesync);
		if (ConVar.AntiHack.terrain_protection > 0 && Time.frameCount % ConVar.AntiHack.terrain_timeslice == (uint)net.ID.Value % ConVar.AntiHack.terrain_timeslice && !AntiHack.ShouldIgnore(this))
		{
			bool flag = false;
			if (AntiHack.IsInsideTerrain(this))
			{
				flag = true;
				AntiHack.AddViolation(this, AntiHackType.InsideTerrain, ConVar.AntiHack.terrain_penalty);
			}
			else if (ConVar.AntiHack.terrain_check_geometry && AntiHack.IsInsideMesh(eyes.position))
			{
				flag = true;
				AntiHack.AddViolation(this, AntiHackType.InsideGeometry, ConVar.AntiHack.terrain_penalty);
				AntiHack.Log(this, AntiHackType.InsideGeometry, "Seems to be clipped inside " + ((Object)((RaycastHit)(ref AntiHack.isInsideRayHit)).collider).name);
			}
			if (flag && ConVar.AntiHack.terrain_kill)
			{
				Analytics.Azure.OnTerrainHackViolation(this);
				Hurt(1000f, DamageType.Suicide, this, useProtection: false);
				return;
			}
		}
		float serverTickInterval = Player.serverTickInterval;
		if (!(Time.realtimeSinceStartup < lastPlayerTick + serverTickInterval))
		{
			if (lastPlayerTick < Time.realtimeSinceStartup - serverTickInterval * 100f)
			{
				lastPlayerTick = Time.realtimeSinceStartup - Random.Range(0f, serverTickInterval);
			}
			while (lastPlayerTick < Time.realtimeSinceStartup)
			{
				lastPlayerTick += serverTickInterval;
			}
			if (IsConnected)
			{
				ConnectedPlayerUpdate(serverTickInterval);
			}
			if (!IsNpc)
			{
				TickPings();
			}
		}
	}

	private void ServerUpdateBots(float deltaTime)
	{
		RefreshColliderSize(forced: false);
	}

	private void ConnectedPlayerUpdate(float deltaTime)
	{
		if (IsReceivingSnapshot)
		{
			net.UpdateSubscriptions(int.MaxValue, int.MaxValue);
		}
		else if (Time.realtimeSinceStartup > lastSubscriptionTick + ConVar.Server.entitybatchtime && net.UpdateSubscriptions(ConVar.Server.entitybatchsize * 2, ConVar.Server.entitybatchsize))
		{
			lastSubscriptionTick = Time.realtimeSinceStartup;
		}
		SendEntityUpdate();
		if (IsReceivingSnapshot)
		{
			if (SnapshotQueue.Length == 0 && EACServer.IsAuthenticated(net.connection))
			{
				EnterGame();
			}
			return;
		}
		if (IsAlive())
		{
			metabolism.ServerUpdate(this, deltaTime);
			if (isMounted)
			{
				PauseVehicleNoClipDetection();
			}
			if ((Object)(object)modifiers != (Object)null && !IsReceivingSnapshot)
			{
				modifiers.ServerUpdate(this);
			}
			if (InSafeZone())
			{
				float num = 0f;
				HeldEntity heldEntity = GetHeldEntity();
				if (Object.op_Implicit((Object)(object)heldEntity) && heldEntity.hostile)
				{
					num = deltaTime;
				}
				if (num == 0f)
				{
					MarkWeaponDrawnDuration(0f);
				}
				else
				{
					AddWeaponDrawnDuration(num);
				}
				if (weaponDrawnDuration >= 8f)
				{
					MarkHostileFor(30f);
				}
			}
			else
			{
				MarkWeaponDrawnDuration(0f);
			}
			if (PlayHeavyLandingAnimation && !modelState.mounted && modelState.onground && Parachute.LandingAnimations)
			{
				Server_StartGesture(GestureCollection.HeavyLandingId);
				PlayHeavyLandingAnimation = false;
			}
			if (timeSinceLastTick > (float)ConVar.Server.playertimeout)
			{
				lastTickTime = 0f;
				Kick("Unresponsive");
				return;
			}
		}
		int num2 = (int)net.connection.GetSecondsConnected();
		int num3 = num2 - secondsConnected;
		if (num3 > 0)
		{
			stats.Add("time", num3, Stats.Server);
			secondsConnected = num2;
		}
		if (IsLoadingAfterTransfer())
		{
			Debug.LogWarning((object)"Force removing loading flag for player (sanity check failed)", (Object)(object)this);
			SetPlayerFlag(PlayerFlags.LoadingAfterTransfer, b: false);
		}
		RefreshColliderSize(forced: false);
		SendModelState();
	}

	private void EnterGame()
	{
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		SetPlayerFlag(PlayerFlags.ReceivingSnapshot, b: false);
		bool flag = false;
		if (IsLoadingAfterTransfer())
		{
			SetPlayerFlag(PlayerFlags.LoadingAfterTransfer, b: false);
			EndSleeping();
			flag = true;
		}
		if (IsTransferProtected())
		{
			BaseVehicle vehicleParent = GetVehicleParent();
			if ((Object)(object)vehicleParent == (Object)null || vehicleParent.ShouldDisableTransferProtectionOnLoad(this))
			{
				DisableTransferProtection();
				flag = true;
			}
		}
		if (flag)
		{
			SendNetworkUpdateImmediate();
		}
		ClientRPC(RpcTarget.Player("FinishLoading", this));
		((FacepunchBehaviour)this).Invoke((Action)DelayedTeamUpdate, 1f);
		LoadMissions(State.missions);
		MissionDirty();
		double num = State.unHostileTimestamp - TimeEx.currentTimestamp;
		if (num > 0.0)
		{
			ClientRPC(RpcTarget.Player("SetHostileLength", this), (float)num);
		}
		if (IsTransferProtected() && base.TransferProtectionRemaining > 0f)
		{
			ClientRPC(RpcTarget.Player("SetTransferProtectionDuration", this), base.TransferProtectionRemaining);
		}
		if ((Object)(object)modifiers != (Object)null)
		{
			modifiers.ResetTicking();
		}
		if (net != null)
		{
			EACServer.OnFinishLoading(net.connection);
		}
		Debug.Log((object)$"{this} has spawned");
		if ((Demo.recordlistmode == 0) ? Demo.recordlist.Contains(UserIDString) : (!Demo.recordlist.Contains(UserIDString)))
		{
			StartDemoRecording();
		}
		SendClientPetLink();
		ClientRPC<Vector3>(RpcTarget.Player("ForceViewAnglesTo", this), ((Component)this).transform.forward);
		HandleTutorialOnGameEnter();
	}

	private void HandleTutorialOnGameEnter()
	{
		if (TutorialIsland.ShouldPlayerResumeTutorial(this) && (Object)(object)TutorialIsland.RestoreOrCreateIslandForPlayer(this, triggerAnalytics: false) == (Object)null)
		{
			ClearTutorial();
			Hurt(999999f);
			ClearTutorial_PostDeath();
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	private void ClientKeepConnectionAlive(RPCMessage msg)
	{
		lastTickTime = Time.time;
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	private void ClientLoadingComplete(RPCMessage msg)
	{
	}

	public void PlayerInit(Connection c)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("PlayerInit", 10);
		try
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)base.KillMessage);
			SetPlayerFlag(PlayerFlags.Connected, b: true);
			activePlayerList.Add(this);
			bots.Remove(this);
			userID = c.userid;
			UserIDString = userID.ToString();
			displayName = c.username;
			c.player = (MonoBehaviour)(object)this;
			secondsConnected = 0;
			currentTeam = RelationshipManager.ServerInstance.FindPlayersTeam(userID)?.teamID ?? 0;
			SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerName(userID, displayName);
			tickInterpolator.Reset(((Component)this).transform.position);
			tickHistory.Reset(((Component)this).transform.position);
			eyeHistory.Clear();
			lastTickTime = 0f;
			lastInputTime = 0f;
			SetPlayerFlag(PlayerFlags.ReceivingSnapshot, b: true);
			stats.Init();
			((FacepunchBehaviour)this).InvokeRandomized((Action)StatSave, Random.Range(5f, 10f), 30f, Random.Range(0f, 6f));
			previousLifeStory = SingletonComponent<ServerMgr>.Instance.persistance.GetLastLifeStory(userID);
			SetPlayerFlag(PlayerFlags.IsAdmin, c.authLevel != 0);
			SetPlayerFlag(PlayerFlags.IsDeveloper, DeveloperList.IsDeveloper(this));
			if (IsDead() && net.SwitchGroup(BaseNetworkable.LimboNetworkGroup))
			{
				SendNetworkGroupChange();
			}
			net.OnConnected(c);
			net.StartSubscriber();
			SendAsSnapshot(net.connection);
			GlobalNetworkHandler.server.StartSendingSnapshot(this);
			ClientRPC(RpcTarget.Player("StartLoading", this));
			if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(serverside: true)))
			{
				BaseGameMode.GetActiveGameMode(serverside: true).OnPlayerConnected(this);
			}
			if (net != null)
			{
				EACServer.OnStartLoading(net.connection);
			}
			if (IsAdmin)
			{
				if (ConVar.AntiHack.noclip_protection <= 0)
				{
					ChatMessage("antihack.noclip_protection is disabled!");
				}
				if (ConVar.AntiHack.speedhack_protection <= 0)
				{
					ChatMessage("antihack.speedhack_protection is disabled!");
				}
				if (ConVar.AntiHack.flyhack_protection <= 0)
				{
					ChatMessage("antihack.flyhack_protection is disabled!");
				}
				if (ConVar.AntiHack.projectile_protection <= 0)
				{
					ChatMessage("antihack.projectile_protection is disabled!");
				}
				if (ConVar.AntiHack.melee_protection <= 0)
				{
					ChatMessage("antihack.melee_protection is disabled!");
				}
				if (ConVar.AntiHack.eye_protection <= 0)
				{
					ChatMessage("antihack.eye_protection is disabled!");
				}
			}
			inventory.crafting.SendToOwner();
			if ((Object)(object)TerrainMeta.Path != (Object)null && TerrainMeta.Path.OceanPatrolFar != null)
			{
				SendCargoPatrolPath();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void StatSave()
	{
		if (stats != null)
		{
			stats.Save();
		}
	}

	public void SendDeathInformation()
	{
		ClientRPC(RpcTarget.Player("OnDied", this));
	}

	public void SendRespawnOptions()
	{
		if (NexusServer.Started && ZoneController.Instance.CanRespawnAcrossZones(this))
		{
			CollectExternalAndSend();
			return;
		}
		List<SpawnOptions> list = Pool.GetList<SpawnOptions>();
		GetRespawnOptionsForPlayer(list, userID);
		SendToPlayer(list, loading: false);
		async void CollectExternalAndSend()
		{
			List<SpawnOptions> list2 = Pool.GetList<SpawnOptions>();
			GetRespawnOptionsForPlayer(list2, userID);
			List<SpawnOptions> allSpawnOptions = Pool.GetList<SpawnOptions>();
			foreach (SpawnOptions item in list2)
			{
				allSpawnOptions.Add(item.Copy());
			}
			SendToPlayer(list2, loading: true);
			try
			{
				Request obj = Pool.Get<Request>();
				obj.spawnOptions = Pool.Get<SpawnOptionsRequest>();
				obj.spawnOptions.userId = userID;
				using (NexusRpcResult nexusRpcResult = await NexusServer.BroadcastRpc(obj, 10f))
				{
					foreach (KeyValuePair<string, Response> response in nexusRpcResult.Responses)
					{
						string key = response.Key;
						SpawnOptionsResponse spawnOptions2 = response.Value.spawnOptions;
						if (spawnOptions2 != null && spawnOptions2.spawnOptions.Count != 0)
						{
							foreach (SpawnOptions spawnOption in spawnOptions2.spawnOptions)
							{
								SpawnOptions val2 = spawnOption.Copy();
								val2.nexusZone = key;
								allSpawnOptions.Add(val2);
							}
						}
					}
				}
				SendToPlayer(allSpawnOptions, loading: false);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		void SendToPlayer(List<SpawnOptions> spawnOptions, bool loading)
		{
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			RespawnInformation val = Pool.Get<RespawnInformation>();
			try
			{
				val.spawnOptions = spawnOptions;
				val.loading = loading;
				if (ConVar.Server.max_shelters == LegacyShelter.FpShelterDefault && LegacyShelter.SheltersPerPlayer.ContainsKey(userID) && LegacyShelter.SheltersPerPlayer[userID].Count > 0)
				{
					val.shelterPositions = Pool.GetList<Vector3>();
					foreach (LegacyShelter item2 in LegacyShelter.SheltersPerPlayer[userID])
					{
						val.shelterPositions.Add(((Component)item2).transform.position);
					}
				}
				if (IsDead())
				{
					val.previousLife = previousLifeStory;
					if (!ConVar.Server.skipDeathScreenFade)
					{
						val.fadeIn = previousLifeStory != null && previousLifeStory.timeDied > Epoch.Current - 5;
					}
					else
					{
						val.fadeIn = false;
					}
				}
				ClientRPC<RespawnInformation>(RpcTarget.Player("OnRespawnInformation", this), val);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public static void GetRespawnOptionsForPlayer(List<SpawnOptions> spawnOptions, ulong userID)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = FindByID(userID);
		SleepingBag[] array = SleepingBag.FindForPlayer(userID, ignoreTimers: true);
		foreach (SleepingBag sleepingBag in array)
		{
			if (!((Object)(object)basePlayer != (Object)null) || basePlayer.IsInTutorial == sleepingBag.IsTutorialBag)
			{
				SpawnOptions val = Pool.Get<SpawnOptions>();
				val.id = sleepingBag.net.ID;
				val.name = sleepingBag.niceName;
				val.worldPosition = ((Component)sleepingBag).transform.position;
				val.type = (RespawnType)(sleepingBag.isStatic ? 5 : ((int)sleepingBag.RespawnType));
				val.unlockSeconds = sleepingBag.GetUnlockSeconds(userID);
				val.respawnState = sleepingBag.GetRespawnState(userID);
				val.mobile = sleepingBag.IsMobile();
				spawnOptions.Add(val);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(1uL)]
	private void RequestRespawnInformation(RPCMessage msg)
	{
		SendRespawnOptions();
	}

	public void ScheduledDeath()
	{
		DeathInfo val = Pool.Get<DeathInfo>();
		val.attackerName = "safezone";
		Kill();
		lifeStory.deathInfo = val;
		lifeStory.timeDied = (uint)Epoch.Current;
		LifeStoryEnd();
	}

	public virtual void StartSleeping()
	{
		if (!IsSleeping())
		{
			if (IsRestrained)
			{
				inventory.SetLockedByRestraint(flag: false);
			}
			if (InSafeZone() && !((FacepunchBehaviour)this).IsInvoking((Action)ScheduledDeath))
			{
				((FacepunchBehaviour)this).Invoke((Action)ScheduledDeath, NPCAutoTurret.sleeperhostiledelay);
			}
			BaseMountable baseMountable = GetMounted();
			if ((Object)(object)baseMountable != (Object)null && !AllowSleeperMounting(baseMountable))
			{
				EnsureDismounted();
			}
			SetPlayerFlag(PlayerFlags.Sleeping, b: true);
			sleepStartTime = Time.time;
			sleepingPlayerList.TryAdd(this);
			bots.Remove(this);
			((FacepunchBehaviour)this).CancelInvoke((Action)InventoryUpdate);
			((FacepunchBehaviour)this).CancelInvoke((Action)TeamUpdate);
			((FacepunchBehaviour)this).CancelInvoke((Action)UpdateClanLastSeen);
			inventory.loot.Clear();
			inventory.containerMain.OnChanged();
			inventory.containerBelt.OnChanged();
			inventory.containerWear.OnChanged();
			EnablePlayerCollider();
			if (!IsLoadingAfterTransfer())
			{
				RemovePlayerRigidbody();
				TurnOffAllLights();
			}
			SetServerFall(wantsOn: true);
		}
	}

	private void TurnOffAllLights()
	{
		LightToggle(mask: false);
		HeldEntity heldEntity = GetHeldEntity();
		if ((Object)(object)heldEntity != (Object)null)
		{
			TorchWeapon component = ((Component)heldEntity).GetComponent<TorchWeapon>();
			if ((Object)(object)component != (Object)null)
			{
				component.SetIsOn(isOn: false);
			}
		}
	}

	private void OnPhysicsNeighbourChanged()
	{
		if (IsSleeping() || IsIncapacitated())
		{
			((FacepunchBehaviour)this).Invoke((Action)DelayedServerFall, 0.05f);
		}
	}

	private void DelayedServerFall()
	{
		SetServerFall(wantsOn: true);
	}

	public void SetServerFall(bool wantsOn)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (wantsOn && ConVar.Server.playerserverfall)
		{
			if (!((FacepunchBehaviour)this).IsInvoking((Action)ServerFall))
			{
				SetPlayerFlag(PlayerFlags.ServerFall, b: true);
				lastFallTime = Time.time - fallTickRate;
				((FacepunchBehaviour)this).InvokeRandomized((Action)ServerFall, 0f, fallTickRate, fallTickRate * 0.1f);
				fallVelocity = estimatedVelocity.y;
			}
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)ServerFall);
			SetPlayerFlag(PlayerFlags.ServerFall, b: false);
		}
	}

	public void ServerFall()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		if (IsDead() || HasParent() || (!IsIncapacitated() && !IsSleeping()))
		{
			SetServerFall(wantsOn: false);
			return;
		}
		float num = Time.time - lastFallTime;
		lastFallTime = Time.time;
		float radius = GetRadius();
		float num2 = GetHeight(ducked: true) * 0.5f;
		float num3 = 2.5f;
		float num4 = 0.5f;
		fallVelocity += Physics.gravity.y * num3 * num4 * num;
		float num5 = Mathf.Abs(fallVelocity * num);
		Vector3 val = ((Component)this).transform.position + Vector3.up * (radius + num2);
		Vector3 position = ((Component)this).transform.position;
		Vector3 val2 = ((Component)this).transform.position;
		RaycastHit val3 = default(RaycastHit);
		if (Physics.SphereCast(val, radius, Vector3.down, ref val3, num5 + num2, 1537286401, (QueryTriggerInteraction)1))
		{
			SetServerFall(wantsOn: false);
			if (((RaycastHit)(ref val3)).distance > num2)
			{
				val2 += Vector3.down * (((RaycastHit)(ref val3)).distance - num2);
			}
			ApplyFallDamageFromVelocity(fallVelocity);
			UpdateEstimatedVelocity(val2, val2, num);
			fallVelocity = 0f;
		}
		else if (Physics.Raycast(val, Vector3.down, ref val3, num5 + radius + num2, 1537286401, (QueryTriggerInteraction)1))
		{
			SetServerFall(wantsOn: false);
			if (((RaycastHit)(ref val3)).distance > num2 - radius)
			{
				val2 += Vector3.down * (((RaycastHit)(ref val3)).distance - num2 - radius);
			}
			ApplyFallDamageFromVelocity(fallVelocity);
			UpdateEstimatedVelocity(val2, val2, num);
			fallVelocity = 0f;
		}
		else
		{
			val2 += Vector3.down * num5;
			UpdateEstimatedVelocity(position, val2, num);
			if (WaterLevel.Test(val2, waves: true, volumes: true, this) || AntiHack.TestInsideTerrain(val2))
			{
				SetServerFall(wantsOn: false);
			}
		}
		MovePosition(val2);
	}

	public void DelayedRigidbodyDisable()
	{
		RemovePlayerRigidbody();
	}

	public virtual void EndSleeping()
	{
		if (IsSleeping())
		{
			if (IsRestrained)
			{
				inventory.SetLockedByRestraint(flag: true);
			}
			SetPlayerFlag(PlayerFlags.Sleeping, b: false);
			sleepStartTime = -1f;
			sleepingPlayerList.Remove(this);
			if ((ulong)userID < 10000000 && !bots.Contains(this))
			{
				bots.Add(this);
			}
			((FacepunchBehaviour)this).CancelInvoke((Action)ScheduledDeath);
			((FacepunchBehaviour)this).InvokeRepeating((Action)InventoryUpdate, 1f, 0.1f * Random.Range(0.99f, 1.01f));
			if (RelationshipManager.TeamsEnabled())
			{
				((FacepunchBehaviour)this).InvokeRandomized((Action)TeamUpdate, 1f, 4f, 1f);
			}
			((FacepunchBehaviour)this).InvokeRandomized((Action)UpdateClanLastSeen, 300f, 300f, 60f);
			EnablePlayerCollider();
			AddPlayerRigidbody();
			SetServerFall(wantsOn: false);
			if (HasParent())
			{
				SetParent(null, worldPositionStays: true);
				ForceUpdateTriggers();
			}
			inventory.containerMain.OnChanged();
			inventory.containerBelt.OnChanged();
			inventory.containerWear.OnChanged();
			EACServer.LogPlayerSpawn(this);
			if (TotalPingCount > 0)
			{
				SendPingsToClient();
			}
			if (TutorialIsland.ShouldPlayerBeAskedToStartTutorial(this))
			{
				ClientRPC(RpcTarget.Player("PromptToStartTutorial", this));
			}
		}
	}

	public virtual void EndLooting()
	{
		if (Object.op_Implicit((Object)(object)inventory.loot))
		{
			inventory.loot.Clear();
		}
	}

	public virtual void OnDisconnected()
	{
		startTutorialCooldown = 0f;
		stats.Save(forceSteamSave: true);
		EndLooting();
		ClearDesigningAIEntity();
		if (IsAlive() || IsSleeping())
		{
			StartSleeping();
		}
		else
		{
			((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0f);
		}
		activePlayerList.Remove(this);
		SetPlayerFlag(PlayerFlags.Connected, b: false);
		StopDemoRecording();
		if (net != null)
		{
			net.OnDisconnected();
		}
		ResetAntiHack();
		RefreshColliderSize(forced: true);
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(serverside: true)))
		{
			BaseGameMode.GetActiveGameMode(serverside: true).OnPlayerDisconnected(this);
		}
		BaseMission.PlayerDisconnected(this);
		ClanManager serverInstance = ClanManager.ServerInstance;
		if (clanId != 0L && (Object)(object)serverInstance != (Object)null)
		{
			serverInstance.ClanMemberConnectionsChanged(clanId);
		}
		UpdateClanLastSeen();
	}

	private void InventoryUpdate()
	{
		if (IsConnected && !IsDead())
		{
			inventory.ServerUpdate(0.1f);
		}
	}

	public void ApplyFallDamageFromVelocity(float velocity)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.InverseLerp(-15f, -100f, velocity);
		if (num != 0f)
		{
			metabolism.bleeding.Add(num * 0.5f);
			float num2 = num * 500f;
			Analytics.Azure.OnFallDamage(this, velocity, num2);
			Hurt(num2, DamageType.Fall);
			if (num2 > 20f && fallDamageEffect.isValid)
			{
				Effect.server.Run(fallDamageEffect.resourcePath, ((Component)this).transform.position, Vector3.zero);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	private void OnPlayerLanded(RPCMessage msg)
	{
		float num = msg.read.Float();
		if (!float.IsNaN(num) && !float.IsInfinity(num) && !ConVar.AntiHack.serverside_fall_damage)
		{
			ApplyFallDamageFromVelocity(num);
			fallVelocity = 0f;
		}
	}

	public void SendGlobalSnapshot()
	{
		TimeWarning val = TimeWarning.New("SendGlobalSnapshot", 10);
		try
		{
			EnterVisibility(Net.sv.visibility.Get(0u));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void SendFullSnapshot()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("SendFullSnapshot", 0);
		try
		{
			Enumerator<Group> enumerator = net.subscriber.subscribed.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Group current = enumerator.Current;
					if (current.ID != 0)
					{
						EnterVisibility(current);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void OnNetworkGroupLeave(Group group)
	{
		base.OnNetworkGroupLeave(group);
		LeaveVisibility(group);
	}

	private void LeaveVisibility(Group group)
	{
		ServerMgr.OnLeaveVisibility(net.connection, group);
		ClearEntityQueue(group);
	}

	public override void OnNetworkGroupEnter(Group group)
	{
		base.OnNetworkGroupEnter(group);
		EnterVisibility(group);
	}

	private void EnterVisibility(Group group)
	{
		ServerMgr.OnEnterVisibility(net.connection, group);
		SendSnapshots(group.networkables);
	}

	public void CheckDeathCondition(HitInfo info = null)
	{
		Assert.IsTrue(base.isServer, "CheckDeathCondition called on client!");
		if (!IsSpectating() && !IsDead() && metabolism.ShouldDie())
		{
			Die(info);
		}
	}

	public virtual BaseCorpse CreateCorpse(PlayerFlags flagsOnDeath, Vector3 posOnDeath, Quaternion rotOnDeath, List<TriggerBase> triggersOnDeath)
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Create corpse", 0);
		try
		{
			string strCorpsePrefab = ((!Physics.serversideragdolls) ? "assets/prefabs/player/player_corpse.prefab" : "assets/prefabs/player/player_corpse_new.prefab");
			bool flag = false;
			if (Global.cinematicGingerbreadCorpses)
			{
				ItemCorpseOverride itemCorpseOverride = default(ItemCorpseOverride);
				foreach (Item item in inventory.containerWear.itemList)
				{
					if (item != null && ((Component)item.info).TryGetComponent<ItemCorpseOverride>(ref itemCorpseOverride))
					{
						strCorpsePrefab = ((GetFloatBasedOnUserID(userID, 4332uL) > 0.5f) ? itemCorpseOverride.FemaleCorpse.resourcePath : itemCorpseOverride.MaleCorpse.resourcePath);
						flag = itemCorpseOverride.BlockWearableCopy;
						break;
					}
				}
			}
			PlayerCorpse playerCorpse = DropCorpse(strCorpsePrefab, posOnDeath, rotOnDeath, flagsOnDeath, modelState) as PlayerCorpse;
			if (Object.op_Implicit((Object)(object)playerCorpse))
			{
				playerCorpse.SetFlag(Flags.Reserved5, HasPlayerFlag(PlayerFlags.DisplaySash));
				if (!flag)
				{
					playerCorpse.TakeFrom(this, inventory.containerMain, inventory.containerWear, inventory.containerBelt);
				}
				playerCorpse.playerName = displayName;
				playerCorpse.streamerName = RandomUsernames.Get((ulong)userID);
				playerCorpse.playerSteamID = userID;
				playerCorpse.underwearSkin = GetUnderwearSkin();
				if (!triggersOnDeath.IsNullOrEmpty())
				{
					foreach (TriggerBase item2 in triggersOnDeath)
					{
						if (item2 is TriggerParent triggerParent)
						{
							triggerParent.ForceParentEarly(playerCorpse);
						}
					}
				}
				playerCorpse.Spawn();
				playerCorpse.TakeChildren(this);
				ResourceDispenser component = ((Component)playerCorpse).GetComponent<ResourceDispenser>();
				int num = 2;
				if (lifeStory != null)
				{
					num += Mathf.Clamp(Mathf.FloorToInt(lifeStory.secondsAlive / 180f), 0, 20);
				}
				component.containedItems.Add(new ItemAmount(ItemManager.FindItemDefinition("fat.animal"), num));
				return playerCorpse;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return null;
		static float GetFloatBasedOnUserID(ulong steamid, ulong seed)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			State state = Random.state;
			Random.InitState((int)(seed + steamid));
			float result = Random.Range(0f, 1f);
			Random.state = state;
			return result;
		}
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Unknown result type (might be due to invalid IL or missing references)
		//IL_07cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0736: Unknown result type (might be due to invalid IL or missing references)
		//IL_073b: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_0953: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a3: Unknown result type (might be due to invalid IL or missing references)
		PlayerFlags flagsOnDeath = playerFlags;
		Vector3 position = ((Component)this).transform.position;
		List<TriggerBase> list = Pool.GetList<TriggerBase>();
		if (triggers != null)
		{
			foreach (TriggerBase trigger in triggers)
			{
				if ((Object)(object)trigger != (Object)null)
				{
					list.Add(trigger);
				}
			}
		}
		BaseMountable baseMountable = GetMounted();
		Vector3 val = Vector3.zero;
		Quaternion rotOnDeath;
		if (baseMountable.IsValid())
		{
			rotOnDeath = baseMountable.mountAnchor.rotation;
			val = baseMountable.GetMountRagdollVelocity(this);
		}
		else
		{
			float x = ((Component)this).transform.eulerAngles.x;
			Quaternion bodyRotation = eyes.bodyRotation;
			rotOnDeath = Quaternion.Euler(x, ((Quaternion)(ref bodyRotation)).eulerAngles.y, ((Component)this).transform.eulerAngles.z);
		}
		SetPlayerFlag(PlayerFlags.Unused2, b: false);
		SetPlayerFlag(PlayerFlags.Unused1, b: false);
		EnsureDismounted();
		EndSleeping();
		EndLooting();
		stats.Add("deaths", 1, Stats.All);
		if (info != null && (Object)(object)info.InitiatorPlayer != (Object)null && !info.InitiatorPlayer.IsNpc && !IsNpc)
		{
			RelationshipManager.ServerInstance.SetSeen(info.InitiatorPlayer, this);
			RelationshipManager.ServerInstance.SetSeen(this, info.InitiatorPlayer);
			RelationshipManager.ServerInstance.SetRelationship(this, info.InitiatorPlayer, RelationshipManager.RelationshipType.Enemy);
			HandleClanPlayerKilled(info.InitiatorPlayer);
		}
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(serverside: true)))
		{
			BasePlayer instigator = info?.InitiatorPlayer;
			BaseGameMode.GetActiveGameMode(serverside: true).OnPlayerDeath(instigator, this, info);
		}
		BaseMission.PlayerKilled(this);
		DisablePlayerCollider();
		RemovePlayerRigidbody();
		List<BasePlayer> list2 = Pool.GetList<BasePlayer>();
		if (IsIncapacitated())
		{
			Enumerator<BasePlayer> enumerator2 = activePlayerList.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					BasePlayer current2 = enumerator2.Current;
					if ((Object)(object)current2 != (Object)null && (Object)(object)current2.inventory != (Object)null && (Object)(object)current2.inventory.loot != (Object)null && (Object)(object)current2.inventory.loot.entitySource == (Object)(object)this)
					{
						list2.Add(current2);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
		}
		bool flag = IsWounded();
		StopWounded();
		if ((Object)(object)inventory.crafting != (Object)null)
		{
			inventory.crafting.CancelAll(returnItems: true);
		}
		EACServer.LogPlayerDespawn(this);
		BaseCorpse baseCorpse = CreateCorpse(flagsOnDeath, position, rotOnDeath, list);
		Vector3 val2;
		if ((Object)(object)baseCorpse != (Object)null)
		{
			if (baseCorpse.CorpseIsRagdoll && (Object)(object)baseMountable != (Object)null)
			{
				BaseVehicle baseVehicle = baseMountable.VehicleParent();
				if ((Object)(object)baseVehicle != (Object)null && baseVehicle.mountedPlayerRagdolls == BaseVehicle.RagdollMode.FallThrough)
				{
					GameObjectExtensions.SetIgnoreCollisions(((Component)baseCorpse).gameObject, ((Component)baseVehicle).gameObject, true);
				}
			}
			if (info != null)
			{
				Rigidbody component = ((Component)baseCorpse).GetComponent<Rigidbody>();
				if ((Object)(object)component != (Object)null)
				{
					float num = (baseCorpse.CorpseIsRagdoll ? 5f : 1f);
					val2 = info.attackNormal + Vector3.up * 0.5f;
					Vector3 val3 = ((Vector3)(ref val2)).normalized * num;
					component.AddForce(val3 + val, (ForceMode)2);
				}
			}
			if (baseCorpse is PlayerCorpse playerCorpse && playerCorpse.containers != null)
			{
				foreach (BasePlayer item in list2)
				{
					if ((Object)(object)item == (Object)null)
					{
						continue;
					}
					item.inventory.loot.StartLootingEntity(playerCorpse);
					ItemContainer[] containers = playerCorpse.containers;
					foreach (ItemContainer itemContainer in containers)
					{
						if (itemContainer != null)
						{
							item.inventory.loot.AddContainer(itemContainer);
						}
					}
					item.inventory.loot.SendImmediate();
				}
			}
		}
		Pool.FreeList<BasePlayer>(ref list2);
		inventory.Strip();
		if (flag && lastDamage == DamageType.Suicide && cachedNonSuicideHitInfo != null)
		{
			info = cachedNonSuicideHitInfo;
			lastDamage = info.damageTypes.GetMajorityDamageType();
		}
		cachedNonSuicideHitInfo = null;
		if (lastDamage == DamageType.Fall)
		{
			stats.Add("death_fall", 1);
		}
		string text = "";
		string text2 = "";
		if (info != null)
		{
			if (Object.op_Implicit((Object)(object)info.Initiator))
			{
				if ((Object)(object)info.Initiator == (Object)(object)this)
				{
					string[] obj = new string[5]
					{
						((object)this).ToString(),
						" was killed by ",
						lastDamage.ToString(),
						" at ",
						null
					};
					val2 = ((Component)this).transform.position;
					obj[4] = ((object)(Vector3)(ref val2)).ToString();
					text = string.Concat(obj);
					text2 = "You died: killed by " + lastDamage;
					if (lastDamage == DamageType.Suicide)
					{
						Analytics.Server.Death("suicide", ServerPosition);
						stats.Add("death_suicide", 1, Stats.All);
					}
					else
					{
						Analytics.Server.Death("selfinflicted", ServerPosition);
						stats.Add("death_selfinflicted", 1);
					}
				}
				else
				{
					if (info.Initiator is BasePlayer)
					{
						BasePlayer basePlayer = info.Initiator.ToPlayer();
						string[] obj2 = new string[5]
						{
							((object)this).ToString(),
							" was killed by ",
							((object)basePlayer).ToString(),
							" at ",
							null
						};
						val2 = ((Component)this).transform.position;
						obj2[4] = ((object)(Vector3)(ref val2)).ToString();
						text = string.Concat(obj2);
						string[] obj3 = new string[5] { "You died: killed by ", basePlayer.displayName, " (", null, null };
						EncryptedValue<ulong> encryptedValue = basePlayer.userID;
						obj3[3] = encryptedValue.ToString();
						obj3[4] = ")";
						text2 = string.Concat(obj3);
						basePlayer.stats.Add("kill_player", 1, Stats.All);
						basePlayer.LifeStoryKill(this);
						OnKilledByPlayer(basePlayer);
						if (lastDamage == DamageType.Fun_Water)
						{
							basePlayer.GiveAchievement("SUMMER_LIQUIDATOR");
							LiquidWeapon liquidWeapon = basePlayer.GetHeldEntity() as LiquidWeapon;
							if ((Object)(object)liquidWeapon != (Object)null && liquidWeapon.RequiresPumping && liquidWeapon.PressureFraction <= liquidWeapon.MinimumPressureFraction)
							{
								basePlayer.GiveAchievement("SUMMER_NO_PRESSURE");
							}
						}
						else if (GameInfo.HasAchievements && lastDamage == DamageType.Explosion && (Object)(object)info.WeaponPrefab != (Object)null && info.WeaponPrefab.ShortPrefabName.Contains("mlrs") && (Object)(object)basePlayer != (Object)null)
						{
							basePlayer.stats.Add("mlrs_kills", 1, Stats.All);
							basePlayer.stats.Save(forceSteamSave: true);
						}
						Analytics.Azure.OnPlayerDeath(this, basePlayer);
					}
					else
					{
						string[] obj4 = new string[7]
						{
							((object)this).ToString(),
							" was killed by ",
							info.Initiator.ShortPrefabName,
							" (",
							info.Initiator.Categorize(),
							") at ",
							null
						};
						val2 = ((Component)this).transform.position;
						obj4[6] = ((object)(Vector3)(ref val2)).ToString();
						text = string.Concat(obj4);
						text2 = "You died: killed by " + info.Initiator.Categorize();
						stats.Add("death_" + info.Initiator.Categorize(), 1);
					}
					if (!IsNpc)
					{
						Analytics.Server.Death(info.Initiator, info.WeaponPrefab, ServerPosition);
					}
				}
			}
			else if (lastDamage == DamageType.Fall)
			{
				string text3 = ((object)this).ToString();
				val2 = ((Component)this).transform.position;
				text = text3 + " was killed by fall at " + ((object)(Vector3)(ref val2)).ToString();
				text2 = "You died: killed by fall";
				Analytics.Server.Death("fall", ServerPosition);
			}
			else
			{
				string[] obj5 = new string[5]
				{
					((object)this).ToString(),
					" was killed by ",
					info.damageTypes.GetMajorityDamageType().ToString(),
					" at ",
					null
				};
				val2 = ((Component)this).transform.position;
				obj5[4] = ((object)(Vector3)(ref val2)).ToString();
				text = string.Concat(obj5);
				text2 = "You died: " + info.damageTypes.GetMajorityDamageType();
			}
		}
		else
		{
			text = ((object)this).ToString() + " died (" + lastDamage.ToString() + ")";
			text2 = "You died: " + lastDamage;
		}
		TimeWarning val4 = TimeWarning.New("LogMessage", 0);
		try
		{
			DebugEx.Log((object)text, (StackTraceLogType)0);
			ConsoleMessage(text2);
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
		if (net.connection == null && (Object)(object)info?.Initiator != (Object)null && (Object)(object)info.Initiator != (Object)(object)this)
		{
			CompanionServer.Util.SendDeathNotification(this, info.Initiator);
		}
		SendNetworkUpdateImmediate();
		LifeStoryLogDeath(info, lastDamage);
		Server_LogDeathMarker(((Component)this).transform.position);
		LifeStoryEnd();
		LastBlockColourChangeId = 0u;
		if (net.connection == null)
		{
			((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0f);
		}
		else
		{
			SendRespawnOptions();
			SendDeathInformation();
			stats.Save();
		}
		Pool.FreeList<TriggerBase>(ref list);
	}

	public void RespawnAt(Vector3 position, Quaternion rotation, BaseEntity spawnPointEntity = null)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if (Object.op_Implicit((Object)(object)activeGameMode) && !activeGameMode.CanPlayerRespawn(this))
		{
			return;
		}
		SetPlayerFlag(PlayerFlags.Wounded, b: false);
		SetPlayerFlag(PlayerFlags.Unused2, b: false);
		SetPlayerFlag(PlayerFlags.Unused1, b: false);
		SetPlayerFlag(PlayerFlags.ReceivingSnapshot, b: true);
		SetPlayerFlag(PlayerFlags.DisplaySash, b: false);
		respawnId = Guid.NewGuid().ToString("N");
		ServerPerformance.spawns++;
		SetParent(null, worldPositionStays: true);
		((Component)this).transform.SetPositionAndRotation(position, rotation);
		tickInterpolator.Reset(position);
		tickHistory.Reset(position);
		eyeHistory.Clear();
		estimatedVelocity = Vector3.zero;
		estimatedSpeed = 0f;
		estimatedSpeed2D = 0f;
		lastTickTime = 0f;
		StopWounded();
		ResetWoundingVars();
		StopSpectating();
		UpdateNetworkGroup();
		EnablePlayerCollider();
		RemovePlayerRigidbody();
		StartSleeping();
		LifeStoryStart();
		metabolism.Reset();
		if ((Object)(object)modifiers != (Object)null)
		{
			modifiers.RemoveAll();
		}
		InitializeHealth(StartHealth(), StartMaxHealth());
		bool flag = false;
		if (ConVar.Server.respawnWithLoadout)
		{
			string infoString = GetInfoString("client.respawnloadout", string.Empty);
			if (!string.IsNullOrEmpty(infoString) && Inventory.LoadLoadout(infoString, out var so))
			{
				so.LoadItemsOnTo(this);
				flag = true;
			}
		}
		if (!flag)
		{
			inventory.GiveDefaultItems();
		}
		SendNetworkUpdateImmediate();
		ClientRPC(RpcTarget.Player("StartLoading", this));
		Analytics.Azure.OnPlayerRespawned(this, spawnPointEntity);
		if (Object.op_Implicit((Object)(object)activeGameMode))
		{
			BaseGameMode.GetActiveGameMode(serverside: true).OnPlayerRespawn(this);
		}
		if (IsConnected)
		{
			EACServer.OnStartLoading(net.connection);
		}
		ProcessMissionEvent(BaseMission.MissionEventType.RESPAWN, 0, 0f);
	}

	public void Respawn()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		SpawnPoint spawnPoint = ServerMgr.FindSpawnPoint(this);
		if (ConVar.Server.respawnAtDeathPosition && ServerCurrentDeathNote != null)
		{
			spawnPoint.pos = ServerCurrentDeathNote.worldPosition;
		}
		RespawnAt(spawnPoint.pos, spawnPoint.rot);
	}

	public bool IsImmortalTo(HitInfo info)
	{
		if (IsGod())
		{
			return true;
		}
		if (WoundingCausingImmortality(info))
		{
			return true;
		}
		BaseVehicle mountedVehicle = GetMountedVehicle();
		if ((Object)(object)mountedVehicle != (Object)null && mountedVehicle.ignoreDamageFromOutside)
		{
			BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if ((Object)(object)initiatorPlayer != (Object)null && (Object)(object)initiatorPlayer.GetMountedVehicle() != (Object)(object)mountedVehicle)
			{
				return true;
			}
		}
		if (IsInTutorial)
		{
			_ = (Object)(object)info.InitiatorPlayer != (Object)(object)this;
			return false;
		}
		return false;
	}

	public float TimeAlive()
	{
		return lifeStory.secondsAlive;
	}

	public override void Hurt(HitInfo info)
	{
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		if (IsDead() || IsTransferProtected() || (IsImmortalTo(info) && info.damageTypes.Total() >= 0f))
		{
			return;
		}
		if (ConVar.Server.pve && !IsNpc && Object.op_Implicit((Object)(object)info.Initiator) && info.Initiator is BasePlayer && (Object)(object)info.Initiator != (Object)(object)this)
		{
			(info.Initiator as BasePlayer).Hurt(info.damageTypes.Total(), DamageType.Generic);
			return;
		}
		if (info.damageTypes.Has(DamageType.Fun_Water))
		{
			bool flag = true;
			Item activeItem = GetActiveItem();
			if (activeItem != null && (activeItem.info.shortname == "gun.water" || activeItem.info.shortname == "pistol.water"))
			{
				float value = metabolism.wetness.value;
				metabolism.wetness.Add(ConVar.Server.funWaterWetnessGain);
				bool flag2 = metabolism.wetness.value >= ConVar.Server.funWaterDamageThreshold;
				flag = !flag2;
				if ((Object)(object)info.InitiatorPlayer != (Object)null)
				{
					if (flag2 && value < ConVar.Server.funWaterDamageThreshold)
					{
						info.InitiatorPlayer.GiveAchievement("SUMMER_SOAKED");
					}
					if (metabolism.radiation_level.Fraction() > 0.2f && !string.IsNullOrEmpty("SUMMER_RADICAL"))
					{
						info.InitiatorPlayer.GiveAchievement("SUMMER_RADICAL");
					}
				}
			}
			if (flag)
			{
				info.damageTypes.Scale(DamageType.Fun_Water, 0f);
			}
		}
		if (info.damageTypes.Get(DamageType.Drowned) > 5f && drownEffect.isValid)
		{
			Effect.server.Run(drownEffect.resourcePath, this, StringPool.Get("head"), Vector3.zero, Vector3.zero);
		}
		if ((Object)(object)modifiers != (Object)null)
		{
			if (info.damageTypes.Has(DamageType.Radiation))
			{
				info.damageTypes.Scale(DamageType.Radiation, 1f - Mathf.Clamp01(modifiers.GetValue(Modifier.ModifierType.Radiation_Resistance)));
			}
			if (info.damageTypes.Has(DamageType.RadiationExposure))
			{
				info.damageTypes.Scale(DamageType.RadiationExposure, 1f - Mathf.Clamp01(modifiers.GetValue(Modifier.ModifierType.Radiation_Exposure_Resistance)));
			}
		}
		metabolism.pending_health.Subtract(info.damageTypes.Total() * 10f);
		BasePlayer initiatorPlayer = info.InitiatorPlayer;
		if (Object.op_Implicit((Object)(object)initiatorPlayer) && (Object)(object)initiatorPlayer != (Object)(object)this)
		{
			if (initiatorPlayer.InSafeZone() || InSafeZone())
			{
				initiatorPlayer.MarkHostileFor(300f);
			}
			if (initiatorPlayer.InSafeZone() && !initiatorPlayer.IsNpc)
			{
				info.damageTypes.ScaleAll(0f);
				return;
			}
			if (initiatorPlayer.IsNpc && initiatorPlayer.Family == BaseNpc.AiStatistics.FamilyEnum.Murderer && info.damageTypes.Get(DamageType.Explosion) > 0f)
			{
				info.damageTypes.ScaleAll(Halloween.scarecrow_beancan_vs_player_dmg_modifier);
			}
		}
		base.Hurt(info);
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(serverside: true)))
		{
			BasePlayer instigator = info?.InitiatorPlayer;
			BaseGameMode.GetActiveGameMode(serverside: true).OnPlayerHurt(instigator, this, info);
		}
		if (IsRestrained && info.damageTypes.GetMajorityDamageType().InterruptsRestraintMinigame())
		{
			Handcuffs handcuffs = GetHeldEntity() as Handcuffs;
			if ((Object)(object)handcuffs != (Object)null)
			{
				handcuffs.InterruptUnlockMiniGame(wasPushedOrDamaged: true);
			}
		}
		EACServer.LogPlayerTakeDamage(this, info);
		metabolism.SendChangesToClient();
		if (info.PointStart != Vector3.zero && (info.damageTypes.Total() >= 0f || IsGod()))
		{
			int arg = (int)info.damageTypes.GetMajorityDamageType();
			if ((Object)(object)info.Weapon != (Object)null && info.damageTypes.Has(DamageType.Bullet))
			{
				BaseProjectile component = ((Component)info.Weapon).GetComponent<BaseProjectile>();
				if ((Object)(object)component != (Object)null && component.IsSilenced())
				{
					arg = 12;
				}
			}
			ClientRPC<Vector3, int, int>(RpcTarget.PlayerAndSpectators("DirectionalDamage", this), info.PointStart, arg, Mathf.CeilToInt(info.damageTypes.Total()));
		}
		cachedNonSuicideHitInfo = info;
	}

	public override void Heal(float amount)
	{
		if (IsCrawling())
		{
			float num = base.health;
			base.Heal(amount);
			healingWhileCrawling += base.health - num;
		}
		else
		{
			base.Heal(amount);
		}
		ProcessMissionEvent(BaseMission.MissionEventType.HEAL, 0, amount);
	}

	public static BasePlayer FindBot(ulong userId)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<BasePlayer> enumerator = bots.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				if ((ulong)current.userID == userId)
				{
					return current;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return FindBotClosestMatch(userId.ToString());
	}

	public static BasePlayer FindBotClosestMatch(string name)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(name))
		{
			return null;
		}
		Enumerator<BasePlayer> enumerator = bots.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				if (current.displayName.Contains(name))
				{
					return current;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return null;
	}

	public static BasePlayer FindByID(ulong userID)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("BasePlayer.FindByID", 0);
		try
		{
			Enumerator<BasePlayer> enumerator = activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if ((ulong)current.userID == userID)
					{
						return current;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return null;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool TryFindByID(ulong userID, out BasePlayer basePlayer)
	{
		basePlayer = FindByID(userID);
		return (Object)(object)basePlayer != (Object)null;
	}

	public static BasePlayer FindSleeping(ulong userID)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("BasePlayer.FindSleeping", 0);
		try
		{
			Enumerator<BasePlayer> enumerator = sleepingPlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if ((ulong)current.userID == userID)
					{
						return current;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return null;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static BasePlayer FindAwakeOrSleepingByID(ulong userID)
	{
		if (userID == 0L)
		{
			return null;
		}
		BasePlayer basePlayer = FindByID(userID);
		if (!((Object)(object)basePlayer != (Object)null))
		{
			return FindSleeping(userID);
		}
		return basePlayer;
	}

	public void Command(string strCommand, params object[] arguments)
	{
		if (net.connection != null)
		{
			ConsoleNetwork.SendClientCommand(net.connection, strCommand, arguments);
		}
	}

	public override void OnInvalidPosition()
	{
		if (!IsDead())
		{
			Die();
		}
	}

	private static BasePlayer Find(string strNameOrIDOrIP, IEnumerable<BasePlayer> list)
	{
		BasePlayer basePlayer = list.FirstOrDefault((BasePlayer x) => x.UserIDString == strNameOrIDOrIP);
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			return basePlayer;
		}
		BasePlayer basePlayer2 = list.FirstOrDefault((BasePlayer x) => x.displayName.StartsWith(strNameOrIDOrIP, StringComparison.CurrentCultureIgnoreCase));
		if (Object.op_Implicit((Object)(object)basePlayer2))
		{
			return basePlayer2;
		}
		BasePlayer basePlayer3 = list.FirstOrDefault((BasePlayer x) => x.net != null && x.net.connection != null && x.net.connection.ipaddress == strNameOrIDOrIP);
		if (Object.op_Implicit((Object)(object)basePlayer3))
		{
			return basePlayer3;
		}
		return null;
	}

	public static BasePlayer Find(string strNameOrIDOrIP)
	{
		return Find(strNameOrIDOrIP, (IEnumerable<BasePlayer>)activePlayerList);
	}

	public static BasePlayer FindSleeping(string strNameOrIDOrIP)
	{
		return Find(strNameOrIDOrIP, (IEnumerable<BasePlayer>)sleepingPlayerList);
	}

	public static BasePlayer FindAwakeOrSleeping(string strNameOrIDOrIP)
	{
		return Find(strNameOrIDOrIP, allPlayerList);
	}

	public void SendConsoleCommand(string command, params object[] obj)
	{
		ConsoleNetwork.SendClientCommand(net.connection, command, obj);
	}

	public void UpdateRadiation(float fAmount)
	{
		metabolism.radiation_level.Increase(fAmount);
	}

	public override float RadiationExposureFraction()
	{
		float num = Mathf.Clamp(baseProtection.amounts[17], 0f, 1f);
		return 1f - num;
	}

	public override float RadiationProtection()
	{
		return baseProtection.amounts[17] * 100f;
	}

	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		base.OnHealthChanged(oldvalue, newvalue);
		if (base.isServer)
		{
			if (oldvalue > newvalue)
			{
				LifeStoryHurt(oldvalue - newvalue);
			}
			else
			{
				LifeStoryHeal(newvalue - oldvalue);
			}
			metabolism.isDirty = true;
		}
	}

	public void SV_ClothingChanged()
	{
		UpdateProtectionFromClothing();
		UpdateMoveSpeedFromClothing();
	}

	public bool IsNoob()
	{
		return !HasPlayerFlag(PlayerFlags.DisplaySash);
	}

	public bool HasHostileItem()
	{
		TimeWarning val = TimeWarning.New("BasePlayer.HasHostileItem", 0);
		try
		{
			foreach (Item item in inventory.containerBelt.itemList)
			{
				if (IsHostileItem(item))
				{
					return true;
				}
			}
			foreach (Item item2 in inventory.containerMain.itemList)
			{
				if (IsHostileItem(item2))
				{
					return true;
				}
			}
			return false;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void GiveItem(Item item, GiveItemReason reason = GiveItemReason.Generic)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		if (reason == GiveItemReason.ResourceHarvested)
		{
			stats.Add($"harvest.{item.info.shortname}", item.amount, (Stats)6);
		}
		if (reason == GiveItemReason.ResourceHarvested || reason == GiveItemReason.Crafted)
		{
			ProcessMissionEvent(BaseMission.MissionEventType.HARVEST, item.info.itemid, item.amount);
		}
		int amount = item.amount;
		if (inventory.GiveItem(item))
		{
			bool infoBool = GetInfoBool("global.streamermode", defaultVal: false);
			string name = item.GetName(infoBool);
			if (!string.IsNullOrEmpty(name))
			{
				Command("note.inv", item.info.itemid, amount, name, (int)reason);
			}
			else
			{
				Command("note.inv", item.info.itemid, amount, string.Empty, (int)reason);
			}
		}
		else
		{
			item.Drop(inventory.containerMain.dropPosition, inventory.containerMain.dropVelocity);
		}
	}

	public override void AttackerInfo(DeathInfo info)
	{
		info.attackerName = displayName;
		info.attackerSteamID = userID;
	}

	public void InvalidateWorkbenchCache()
	{
		nextCheckTime = 0f;
	}

	public Workbench GetCachedCraftLevelWorkbench()
	{
		return _cachedWorkbench;
	}

	public virtual bool ShouldDropActiveItem()
	{
		return true;
	}

	public override void Die(HitInfo info = null)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Player.Die", 0);
		try
		{
			if (!IsDead())
			{
				Handcuffs restraintItem = Belt.GetRestraintItem();
				if ((Object)(object)restraintItem != (Object)null)
				{
					restraintItem.HeldWhenOwnerDied(this);
				}
				if (Belt != null && ShouldDropActiveItem())
				{
					Vector3 val2 = default(Vector3);
					((Vector3)(ref val2))._002Ector(Random.Range(-2f, 2f), 0.2f, Random.Range(-2f, 2f));
					Belt.DropActive(GetDropPosition(), GetInheritedDropVelocity() + ((Vector3)(ref val2)).normalized * 3f);
					inventory.DropBackpackOnDeath();
				}
				if (!WoundInsteadOfDying(info))
				{
					SleepingBag.OnPlayerDeath(this);
					base.Die(info);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void Kick(string reason)
	{
		if (IsConnected)
		{
			Net.sv.Kick(net.connection, reason, false);
		}
	}

	public override Vector3 GetDropPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return eyes.position;
	}

	public override Vector3 GetDropVelocity()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		return GetInheritedDropVelocity() + eyes.BodyForward() * 4f + Vector3Ex.Range(-0.5f, 0.5f);
	}

	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GetParentEntity();
		if ((Object)(object)baseEntity != (Object)null)
		{
			ClientRPC<Vector3, NetworkableId>(RpcTarget.Player("SetInheritedVelocity", this), ((Component)baseEntity).transform.InverseTransformDirection(velocity), baseEntity.net.ID);
		}
		else
		{
			ClientRPC<Vector3>(RpcTarget.Player("SetInheritedVelocity", this), velocity);
		}
		PauseSpeedHackDetection();
	}

	public virtual void SetInfo(string key, string val)
	{
		if (IsConnected)
		{
			net.connection.info.Set(key, val);
		}
	}

	public virtual int GetInfoInt(string key, int defaultVal)
	{
		if (!IsConnected)
		{
			return defaultVal;
		}
		return net.connection.info.GetInt(key, defaultVal);
	}

	public virtual bool GetInfoBool(string key, bool defaultVal)
	{
		if (!IsConnected)
		{
			return defaultVal;
		}
		return net.connection.info.GetBool(key, defaultVal);
	}

	public virtual string GetInfoString(string key, string defaultVal)
	{
		if (!IsConnected)
		{
			return defaultVal;
		}
		return net.connection.info.GetString(key, defaultVal);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(1uL)]
	public void PerformanceReport(RPCMessage msg)
	{
		string text = msg.read.String(256, false);
		string text2 = msg.read.StringRaw(8388608, false);
		ClientPerformanceReport clientPerformanceReport = JsonConvert.DeserializeObject<ClientPerformanceReport>(text2);
		if (clientPerformanceReport.user_id != UserIDString)
		{
			DebugEx.Log((object)$"Client performance report from {this} has incorrect user_id ({UserIDString})", (StackTraceLogType)0);
			return;
		}
		switch (text)
		{
		case "json":
			DebugEx.Log((object)text2, (StackTraceLogType)0);
			break;
		case "legacy":
		{
			string text3 = (clientPerformanceReport.memory_managed_heap + "MB").PadRight(9);
			string text4 = (clientPerformanceReport.memory_system + "MB").PadRight(9);
			string text5 = (clientPerformanceReport.fps.ToString("0") + "FPS").PadRight(8);
			string text6 = NumberExtensions.FormatSeconds((long)clientPerformanceReport.fps).PadRight(9);
			string text7 = UserIDString.PadRight(20);
			string text8 = clientPerformanceReport.streamer_mode.ToString().PadRight(7);
			DebugEx.Log((object)(text3 + text4 + text5 + text6 + text8 + text7 + displayName), (StackTraceLogType)0);
			break;
		}
		case "rcon":
			RCon.Broadcast(RCon.LogType.ClientPerf, text2);
			break;
		default:
			Debug.LogError((object)("Unknown PerformanceReport format '" + text + "'"));
			break;
		case "none":
			break;
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(1uL)]
	public void PerformanceReport_Frametime(RPCMessage msg)
	{
		int request_id = msg.read.Int32();
		int start_frame = msg.read.Int32();
		int num = msg.read.Int32();
		List<int> list = Pool.GetList<int>();
		for (int i = 0; i < num; i++)
		{
			list.Add(ProtocolParser.ReadInt32((Stream)(object)msg.read));
		}
		ClientFrametimeReport obj = new ClientFrametimeReport
		{
			frame_times = list,
			request_id = request_id,
			start_frame = start_frame
		};
		DebugEx.Log((object)JsonConvert.SerializeObject((object)obj), (StackTraceLogType)0);
		Pool.FreeList<int>(ref obj.frame_times);
	}

	public override bool ShouldNetworkTo(BasePlayer player)
	{
		if (IsSpectating() && (Object)(object)player != (Object)(object)this)
		{
			return false;
		}
		return base.ShouldNetworkTo(player);
	}

	internal void GiveAchievement(string name, bool allowTutorial = false)
	{
		if (GameInfo.HasAchievements && (!IsInTutorial || allowTutorial))
		{
			ClientRPC(RpcTarget.Player("RecieveAchievement", this), name);
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(1uL)]
	public void OnPlayerReported(RPCMessage msg)
	{
		string text = msg.read.String(256, false);
		string message = msg.read.StringMultiLine(2048, false);
		string type = msg.read.String(256, false);
		string text2 = msg.read.String(256, false);
		string text3 = msg.read.String(256, false);
		DebugEx.Log((object)$"[PlayerReport] {this} reported {text3}[{text2}] - \"{text}\"", (StackTraceLogType)0);
		RCon.Broadcast(RCon.LogType.Report, new
		{
			PlayerId = UserIDString,
			PlayerName = displayName,
			TargetId = text2,
			TargetName = text3,
			Subject = text,
			Message = message,
			Type = type
		});
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(1uL)]
	public void OnFeedbackReport(RPCMessage msg)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (ConVar.Server.printReportsToConsole || !string.IsNullOrEmpty(ConVar.Server.reportsServerEndpoint))
		{
			string text = msg.read.String(256, false);
			string text2 = msg.read.StringMultiLine(2048, false);
			ReportType val = (ReportType)Mathf.Clamp(msg.read.Int32(), 0, 5);
			if (ConVar.Server.printReportsToConsole)
			{
				DebugEx.Log((object)$"[FeedbackReport] {this} reported {val} - \"{text}\" \"{text2}\"", (StackTraceLogType)0);
				RCon.Broadcast(RCon.LogType.Report, new
				{
					PlayerId = UserIDString,
					PlayerName = displayName,
					Subject = text,
					Message = text2,
					Type = val
				});
			}
			if (!string.IsNullOrEmpty(ConVar.Server.reportsServerEndpoint))
			{
				string image = msg.read.StringMultiLine(60000, false);
				Feedback val2 = default(Feedback);
				val2.Type = val;
				val2.Message = text2;
				val2.Subject = text;
				Feedback val3 = val2;
				((AppInfo)(ref val3.AppInfo)).Image = image;
				Feedback.ServerReport(ConVar.Server.reportsServerEndpoint, (ulong)userID, ConVar.Server.reportsServerEndpointKey, val3);
			}
		}
	}

	public void StartDemoRecording()
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if (net != null && net.connection != null && !net.connection.IsRecording)
		{
			string text = $"demos/{UserIDString}/{DateTime.Now:yyyy-MM-dd-hhmmss}.dem";
			Debug.Log((object)(((object)this).ToString() + " recording started: " + text));
			net.connection.StartRecording(text, (IDemoHeader)(object)new Demo.Header
			{
				version = Demo.Version,
				level = Application.loadedLevelName,
				levelSeed = World.Seed,
				levelSize = World.Size,
				checksum = World.Checksum,
				localclient = userID,
				position = eyes.position,
				rotation = eyes.HeadForward(),
				levelUrl = World.Url,
				recordedTime = DateTime.Now.ToBinary()
			});
			SendNetworkUpdateImmediate();
			SendGlobalSnapshot();
			SendFullSnapshot();
			SendEntityUpdate();
			TreeManager.SendSnapshot(this);
			ServerMgr.SendReplicatedVars(net.connection);
			((FacepunchBehaviour)this).InvokeRepeating((Action)MonitorDemoRecording, 10f, 10f);
		}
	}

	public void StopDemoRecording()
	{
		if (net != null && net.connection != null && net.connection.IsRecording)
		{
			Debug.Log((object)(((object)this).ToString() + " recording stopped: " + net.connection.RecordFilename));
			net.connection.StopRecording();
			((FacepunchBehaviour)this).CancelInvoke((Action)MonitorDemoRecording);
		}
	}

	public void MonitorDemoRecording()
	{
		if (net != null && net.connection != null && net.connection.IsRecording && (net.connection.RecordTimeElapsed.TotalSeconds >= (double)Demo.splitseconds || (float)net.connection.RecordFilesize >= Demo.splitmegabytes * 1024f * 1024f))
		{
			StopDemoRecording();
			StartDemoRecording();
		}
	}

	public void InvalidateCachedPeristantPlayer()
	{
		cachedPersistantPlayer = null;
	}

	public bool IsPlayerVisibleToUs(BasePlayer otherPlayer, Vector3 fromOffset, int layerMask)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)otherPlayer == (Object)null)
		{
			return false;
		}
		Vector3 val = (isMounted ? eyes.worldMountedPosition : (IsDucked() ? eyes.worldCrouchedPosition : ((!IsCrawling()) ? eyes.worldStandingPosition : eyes.worldCrawlingPosition)));
		val += fromOffset;
		if (!otherPlayer.IsVisibleSpecificLayers(val, otherPlayer.CenterPoint(), layerMask) && !otherPlayer.IsVisibleSpecificLayers(val, ((Component)otherPlayer).transform.position, layerMask) && !otherPlayer.IsVisibleSpecificLayers(val, otherPlayer.eyes.position, layerMask))
		{
			return false;
		}
		if (!IsVisibleSpecificLayers(otherPlayer.CenterPoint(), val, layerMask) && !IsVisibleSpecificLayers(((Component)otherPlayer).transform.position, val, layerMask) && !IsVisibleSpecificLayers(otherPlayer.eyes.position, val, layerMask))
		{
			return false;
		}
		return true;
	}

	protected virtual void OnKilledByPlayer(BasePlayer p)
	{
	}

	public int GetIdealSlot(BasePlayer player, ItemContainer container, Item item)
	{
		if (container.HasFlag(ItemContainer.Flag.Clothing))
		{
			if (item.IsBackpack())
			{
				return ItemContainer.BackpackSlotIndex;
			}
			if (!item.info.isWearable)
			{
				return -1;
			}
			foreach (Item item2 in container.itemList)
			{
				if (!item2.info.ItemModWearable.CanExistWith(item.info.ItemModWearable) && item2.position == ItemContainer.BackpackSlotIndex == item.IsBackpack())
				{
					return item2.position;
				}
			}
		}
		return -1;
	}

	public ItemContainerId GetIdealContainer(BasePlayer looter, Item item, ItemMoveModifier modifier)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !((Enum)modifier).HasFlag((Enum)(object)(ItemMoveModifier)2) && looter.inventory.loot.containers.Count > 0;
		ItemContainer parent = item.parent;
		Item activeItem = looter.GetActiveItem();
		Item backpackWithInventory = inventory.GetBackpackWithInventory();
		bool flag2 = backpackWithInventory != null && backpackWithInventory == item.parentItem;
		bool flag3 = false;
		if (((Enum)modifier).HasFlag((Enum)(object)(ItemMoveModifier)16) && (Object)(object)looter == (Object)(object)this && backpackWithInventory != null)
		{
			if (backpackWithInventory.contents.HasSpaceFor(item))
			{
				if (!flag)
				{
					if (item.parentItem == null || !item.parentItem.IsBackpack() || item.parentItem.parent != inventory.containerWear)
					{
						return backpackWithInventory.contents.uid;
					}
				}
				else if (inventory.loot.FindItem(item.uid) != null && !inventory.containerMain.HasSpaceFor(item))
				{
					return backpackWithInventory.contents.uid;
				}
			}
			else
			{
				flag3 = true;
			}
		}
		if (activeItem != null && !flag3 && !flag && activeItem.contents != null && activeItem.contents != item.parent && activeItem.contents.capacity > 0 && activeItem.contents.CanAcceptItem(item, -1) == ItemContainer.CanAcceptResult.CanAccept)
		{
			return activeItem.contents.uid;
		}
		if (item.info.isWearable && item.info.ItemModWearable.equipOnRightClick && item.parent != inventory.containerWear && !flag && !flag2)
		{
			if (flag3)
			{
				return ItemContainerId.Invalid;
			}
			if (backpackWithInventory == null || item.parent != backpackWithInventory.contents)
			{
				return inventory.containerWear.uid;
			}
		}
		if (parent == inventory.containerMain)
		{
			if (flag)
			{
				return default(ItemContainerId);
			}
			return inventory.containerBelt.uid;
		}
		if (parent == inventory.containerWear)
		{
			return inventory.containerMain.uid;
		}
		if (parent == inventory.containerBelt)
		{
			return inventory.containerMain.uid;
		}
		return default(ItemContainerId);
	}

	private BaseVehicle GetVehicleParent()
	{
		BaseVehicle mountedVehicle = GetMountedVehicle();
		if ((Object)(object)mountedVehicle != (Object)null)
		{
			return mountedVehicle;
		}
		BaseEntity baseEntity = GetParentEntity();
		if ((Object)(object)baseEntity != (Object)null && baseEntity is BaseVehicle result)
		{
			return result;
		}
		return null;
	}

	private void RemoveLoadingPlayerFlag()
	{
		if (IsLoadingAfterTransfer())
		{
			SetPlayerFlag(PlayerFlags.LoadingAfterTransfer, b: false);
			if (IsSleeping())
			{
				SetPlayerFlag(PlayerFlags.Sleeping, b: false);
				StartSleeping();
			}
		}
	}

	public bool InNoRespawnZone()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Vector3 position = ((Component)this).transform.position;
		if (triggers != null)
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerNoRespawnZone triggerNoRespawnZone = triggers[i] as TriggerNoRespawnZone;
				if (!((Object)(object)triggerNoRespawnZone == (Object)null))
				{
					flag = triggerNoRespawnZone.InNoRespawnZone(position, checkRadius: false);
					if (flag)
					{
						break;
					}
				}
			}
		}
		return flag;
	}

	private void SendCargoPatrolPath()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		if (!BaseBoat.generate_paths)
		{
			return;
		}
		if (cachedOceanPaths == null)
		{
			cachedOceanPaths = Pool.Get<OceanPaths>();
			cachedOceanPaths.cargoPatrolPath = TerrainMeta.Path.OceanPatrolFar;
			cachedOceanPaths.harborApproaches = new List<VectorList>();
			for (int i = 0; i < CargoShip.TotalAvailableHarborDockingPaths; i++)
			{
				VectorList val = new VectorList();
				val.vectorPoints = CargoShip.GetCargoApproachPath(i);
				cachedOceanPaths.harborApproaches.Add(val);
			}
		}
		ClientRPCPlayer<OceanPaths>(null, this, "ReceiveCargoPatrolPath", cachedOceanPaths);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	private void RPC_ReqDoPush(RPCMessage rpc)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (IsSleeping() || IsDead() || !IsRestrained)
		{
			return;
		}
		BasePlayer player = rpc.player;
		if ((Object)(object)player == (Object)null || (Object)(object)player == (Object)(object)this)
		{
			return;
		}
		Handcuffs handcuffs = GetHeldEntity() as Handcuffs;
		if ((Object)(object)handcuffs != (Object)null)
		{
			handcuffs.InterruptUnlockMiniGame(wasPushedOrDamaged: true);
			handcuffs.RepairOnPush();
		}
		if (isMounted)
		{
			BaseMountable baseMountable = GetMounted();
			if ((Object)(object)baseMountable != (Object)null)
			{
				baseMountable.DismountPlayer(this);
				return;
			}
		}
		Vector3 val = player.eyes.BodyForward() * 10f;
		val.y = 0f;
		val += Vector3.up * 3f;
		ClientRPC<Vector3>(RpcTarget.Player("RPC_DoPush", this), val);
		Hurt(Handcuffs.restrainedPushDamage, DamageType.Generic, player, useProtection: false);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	private void RPC_ReqRemoveCuffs(RPCMessage rpc)
	{
		if (IsDead() || !IsRestrained)
		{
			return;
		}
		BasePlayer player = rpc.player;
		if (!((Object)(object)player == (Object)null))
		{
			Handcuffs handcuffs = GetHeldEntity() as Handcuffs;
			if ((Object)(object)handcuffs != (Object)null)
			{
				handcuffs.UnlockAndReturnToPlayer(player);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	private void RPC_ReqRemoveHood(RPCMessage rpc)
	{
		BasePlayer player = rpc.player;
		if (!((Object)(object)player == (Object)null))
		{
			RemoveAndReturnPrisonerHood(player);
		}
	}

	private void RemoveAndReturnPrisonerHood(BasePlayer returnToPlayer)
	{
		if (!((Object)(object)returnToPlayer == (Object)null) && !IsDead() && IsRestrained)
		{
			Item equippedPrisonerHoodItem = inventory.GetEquippedPrisonerHoodItem();
			if (equippedPrisonerHoodItem != null)
			{
				bool locked = inventory.containerWear.IsLocked();
				inventory.containerWear.SetLocked(isLocked: false);
				returnToPlayer.GiveItem(equippedPrisonerHoodItem);
				inventory.containerWear.SetLocked(locked);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	private void RPC_ReqEquipHood(RPCMessage rpc)
	{
		BasePlayer player = rpc.player;
		if (!((Object)(object)player == (Object)null))
		{
			EquipPrisonerHood(player);
		}
	}

	private void EquipPrisonerHood(BasePlayer placingPlayer)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)placingPlayer == (Object)null || IsDead() || !IsRestrained || (Object)(object)inventory == (Object)null || inventory.GetEquippedPrisonerHoodItem() != null)
		{
			return;
		}
		Item usableHoodItem = placingPlayer.inventory.GetUsableHoodItem();
		if (usableHoodItem == null)
		{
			return;
		}
		inventory.SetLockedByRestraint(flag: false);
		if (!usableHoodItem.MoveToContainer(inventory.containerBelt))
		{
			Item slot = inventory.containerBelt.GetSlot(0);
			if (slot != null && slot == Belt.GetRestraintItem()?.GetItem())
			{
				slot = inventory.containerBelt.GetSlot(1);
			}
			if (slot != null)
			{
				if (!slot.MoveToContainer(inventory.containerMain))
				{
					slot.DropAndTossUpwards(((Component)this).transform.position);
				}
				usableHoodItem.MoveToContainer(inventory.containerBelt);
			}
		}
		inventory.SetLockedByRestraint(flag: true);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	private void RPC_ReqForceMountNearest(RPCMessage rpc)
	{
		BasePlayer player = rpc.player;
		if (!((Object)(object)player == (Object)null))
		{
			ForceRestrainedMountNearest(player);
		}
	}

	private void ForceRestrainedMountNearest(BasePlayer forcingPlayer)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)forcingPlayer == (Object)null || isMounted || !IsRestrained || IsDead() || IsSleeping() || IsWounded())
		{
			return;
		}
		List<BaseMountable> list = Pool.GetList<BaseMountable>();
		Vis.Entities(((Component)this).transform.position, 2f, list, -1, (QueryTriggerInteraction)2);
		list.Sort(delegate(BaseMountable a, BaseMountable b)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = ((Component)this).transform.position - ((Component)a).transform.position;
			float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
			val = ((Component)this).transform.position - ((Component)b).transform.position;
			return sqrMagnitude.CompareTo(((Vector3)(ref val)).sqrMagnitude);
		});
		foreach (BaseMountable item in list)
		{
			if (item.isClient || !item.AllowForceMountWhenRestrained || (Object)(object)item.VehicleParent() != (Object)null || !item.DirectlyMountable() || item.Distance(eyes.position) > 3f || !GamePhysics.LineOfSight(eyes.center, eyes.position, 1218519041) || (!item.IsVisible(eyes.HeadRay(), 1218519041, 3f) && !item.IsVisible(eyes.position, 3f)))
			{
				continue;
			}
			bool flag = false;
			ModularCar modularCar = item as ModularCar;
			if ((Object)(object)modularCar != (Object)null && modularCar.CarLock.HasALock)
			{
				flag = !modularCar.CarLock.HasLockPermission(this);
				if (modularCar.CarLock.HasLockPermission(forcingPlayer))
				{
					modularCar.CarLock.TryAddPlayer(userID);
				}
			}
			item.AttemptMount(this);
			if ((Object)(object)modularCar != (Object)null && modularCar.CarLock.HasALock && flag)
			{
				modularCar.CarLock.TryRemovePlayer(userID);
			}
			if (isMounted)
			{
				break;
			}
		}
		Pool.FreeList<BaseMountable>(ref list);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	private void RPC_ReqForceSwapSeat(RPCMessage rpc)
	{
		if (!isMounted || !IsRestrained || IsDead() || IsSleeping() || IsWounded() || (Object)(object)rpc.player == (Object)null)
		{
			return;
		}
		BasePlayer player = rpc.player;
		BaseMountable baseMountable = GetMounted();
		if ((Object)(object)baseMountable == (Object)null)
		{
			return;
		}
		BaseVehicle baseVehicle = ((Component)baseMountable).GetComponent<BaseVehicle>();
		if ((Object)(object)baseVehicle == (Object)null)
		{
			baseVehicle = baseMountable.VehicleParent();
		}
		if ((Object)(object)baseVehicle == (Object)null)
		{
			return;
		}
		bool flag = false;
		ModularCar modularCar = baseVehicle as ModularCar;
		if ((Object)(object)modularCar != (Object)null && modularCar.CarLock.HasALock)
		{
			flag = !modularCar.CarLock.HasLockPermission(this);
			if (modularCar.CarLock.HasLockPermission(player))
			{
				modularCar.CarLock.TryAddPlayer(userID);
			}
		}
		baseVehicle.SwapSeats(this, 0, forcingRestrainedPlayer: true);
		if ((Object)(object)modularCar != (Object)null && modularCar.CarLock.HasALock && flag)
		{
			modularCar.CarLock.TryRemovePlayer(userID);
		}
	}

	public bool CanMoveFrom(BasePlayer player, Item item)
	{
		if (IsRestrainedOrSurrendering)
		{
			ItemContainer itemContainer = item?.parent;
			if (itemContainer == null)
			{
				return true;
			}
			if (itemContainer.IsLocked())
			{
				return false;
			}
			if (itemContainer == inventory.containerBelt && item.IsOn() && (Object)(object)((Component)item.info).GetComponent<ItemModRestraint>() != (Object)null)
			{
				return false;
			}
		}
		return true;
	}

	internal void LifeStoryStart()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		if (lifeStory != null)
		{
			lifeStory = null;
		}
		lifeStory = new PlayerLifeStory
		{
			ShouldPool = false
		};
		lifeStory.timeBorn = (uint)Epoch.Current;
		hasSentPresenceState = false;
	}

	internal void LifeStoryEnd()
	{
		SingletonComponent<ServerMgr>.Instance.persistance.AddLifeStory(userID, lifeStory);
		previousLifeStory = lifeStory;
		lifeStory = null;
	}

	internal void LifeStoryUpdate(float deltaTime, float moveSpeed)
	{
		if (lifeStory != null)
		{
			PlayerLifeStory obj = lifeStory;
			obj.secondsAlive += deltaTime;
			nextTimeCategoryUpdate -= deltaTime * ((moveSpeed > 0.1f) ? 1f : 0.25f);
			if (nextTimeCategoryUpdate <= 0f && !waitingForLifeStoryUpdate)
			{
				nextTimeCategoryUpdate = 7f + 7f * Random.Range(0.2f, 1f);
				waitingForLifeStoryUpdate = true;
				((ObjectWorkQueue<BasePlayer>)lifeStoryQueue).Add(this);
			}
			if (LifeStoryInWilderness)
			{
				PlayerLifeStory obj2 = lifeStory;
				obj2.secondsWilderness += deltaTime;
			}
			if (LifeStoryInMonument)
			{
				PlayerLifeStory obj3 = lifeStory;
				obj3.secondsInMonument += deltaTime;
			}
			if (LifeStoryInBase)
			{
				PlayerLifeStory obj4 = lifeStory;
				obj4.secondsInBase += deltaTime;
			}
			if (LifeStoryFlying)
			{
				PlayerLifeStory obj5 = lifeStory;
				obj5.secondsFlying += deltaTime;
			}
			if (LifeStoryBoating)
			{
				PlayerLifeStory obj6 = lifeStory;
				obj6.secondsBoating += deltaTime;
			}
			if (LifeStorySwimming)
			{
				PlayerLifeStory obj7 = lifeStory;
				obj7.secondsSwimming += deltaTime;
			}
			if (LifeStoryDriving)
			{
				PlayerLifeStory obj8 = lifeStory;
				obj8.secondsDriving += deltaTime;
			}
			if (IsSleeping())
			{
				PlayerLifeStory obj9 = lifeStory;
				obj9.secondsSleeping += deltaTime;
			}
			else if (IsRunning())
			{
				PlayerLifeStory obj10 = lifeStory;
				obj10.metersRun += moveSpeed * deltaTime;
			}
			else
			{
				PlayerLifeStory obj11 = lifeStory;
				obj11.metersWalked += moveSpeed * deltaTime;
			}
		}
	}

	public void UpdateTimeCategory()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("UpdateTimeCategory", 0);
		try
		{
			waitingForLifeStoryUpdate = false;
			int num = currentTimeCategory;
			currentTimeCategory = 1;
			if (IsBuildingAuthed())
			{
				currentTimeCategory = 4;
			}
			Vector3 position = ((Component)this).transform.position;
			if ((Object)(object)TerrainMeta.TopologyMap != (Object)null && ((uint)TerrainMeta.TopologyMap.GetTopology(position) & 0x400u) != 0)
			{
				foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
				{
					if (monument.shouldDisplayOnMap && monument.IsInBounds(position))
					{
						currentTimeCategory = 2;
						break;
					}
				}
			}
			if (IsSwimming())
			{
				currentTimeCategory |= 32;
			}
			if (isMounted)
			{
				BaseMountable baseMountable = GetMounted();
				if (baseMountable.mountTimeStatType == BaseMountable.MountStatType.Boating)
				{
					currentTimeCategory |= 16;
				}
				else if (baseMountable.mountTimeStatType == BaseMountable.MountStatType.Flying)
				{
					currentTimeCategory |= 8;
				}
				else if (baseMountable.mountTimeStatType == BaseMountable.MountStatType.Driving)
				{
					currentTimeCategory |= 64;
				}
			}
			else if (HasParent() && GetParentEntity() is BaseMountable baseMountable2)
			{
				if (baseMountable2.mountTimeStatType == BaseMountable.MountStatType.Boating)
				{
					currentTimeCategory |= 16;
				}
				else if (baseMountable2.mountTimeStatType == BaseMountable.MountStatType.Flying)
				{
					currentTimeCategory |= 8;
				}
				else if (baseMountable2.mountTimeStatType == BaseMountable.MountStatType.Driving)
				{
					currentTimeCategory |= 64;
				}
			}
			if (num != currentTimeCategory || !hasSentPresenceState)
			{
				LifeStoryInWilderness = (1 & currentTimeCategory) != 0;
				LifeStoryInMonument = (2 & currentTimeCategory) != 0;
				LifeStoryInBase = (4 & currentTimeCategory) != 0;
				LifeStoryFlying = (8 & currentTimeCategory) != 0;
				LifeStoryBoating = (0x10 & currentTimeCategory) != 0;
				LifeStorySwimming = (0x20 & currentTimeCategory) != 0;
				LifeStoryDriving = (0x40 & currentTimeCategory) != 0;
				ClientRPC(RpcTarget.Player("UpdateRichPresenceState", this), currentTimeCategory);
				hasSentPresenceState = true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void LifeStoryShotFired(BaseEntity withWeapon)
	{
		if (lifeStory == null)
		{
			return;
		}
		if (lifeStory.weaponStats == null)
		{
			lifeStory.weaponStats = Pool.GetList<WeaponStats>();
		}
		foreach (WeaponStats weaponStat in lifeStory.weaponStats)
		{
			if (weaponStat.weaponName == withWeapon.ShortPrefabName)
			{
				weaponStat.shotsFired++;
				return;
			}
		}
		WeaponStats val = Pool.Get<WeaponStats>();
		val.weaponName = withWeapon.ShortPrefabName;
		val.shotsFired++;
		lifeStory.weaponStats.Add(val);
	}

	public void LifeStoryShotHit(BaseEntity withWeapon)
	{
		if (lifeStory == null || (Object)(object)withWeapon == (Object)null)
		{
			return;
		}
		if (lifeStory.weaponStats == null)
		{
			lifeStory.weaponStats = Pool.GetList<WeaponStats>();
		}
		foreach (WeaponStats weaponStat in lifeStory.weaponStats)
		{
			if (weaponStat.weaponName == withWeapon.ShortPrefabName)
			{
				weaponStat.shotsHit++;
				return;
			}
		}
		WeaponStats val = Pool.Get<WeaponStats>();
		val.weaponName = withWeapon.ShortPrefabName;
		val.shotsHit++;
		lifeStory.weaponStats.Add(val);
	}

	public void LifeStoryKill(BaseCombatEntity killed)
	{
		if (lifeStory != null)
		{
			if (killed is ScientistNPC)
			{
				PlayerLifeStory obj = lifeStory;
				obj.killedScientists++;
			}
			else if (killed is BasePlayer)
			{
				PlayerLifeStory obj2 = lifeStory;
				obj2.killedPlayers++;
			}
			else if (killed is BaseAnimalNPC)
			{
				PlayerLifeStory obj3 = lifeStory;
				obj3.killedAnimals++;
			}
		}
	}

	public void LifeStoryGenericStat(string key, int value)
	{
		if (lifeStory == null)
		{
			return;
		}
		if (lifeStory.genericStats == null)
		{
			lifeStory.genericStats = Pool.GetList<GenericStat>();
		}
		foreach (GenericStat genericStat in lifeStory.genericStats)
		{
			if (genericStat.key == key)
			{
				genericStat.value += value;
				return;
			}
		}
		GenericStat val = Pool.Get<GenericStat>();
		val.key = key;
		val.value = value;
		lifeStory.genericStats.Add(val);
	}

	public void LifeStoryHurt(float amount)
	{
		if (lifeStory != null)
		{
			PlayerLifeStory obj = lifeStory;
			obj.totalDamageTaken += amount;
		}
	}

	public void LifeStoryHeal(float amount)
	{
		if (lifeStory != null)
		{
			PlayerLifeStory obj = lifeStory;
			obj.totalHealing += amount;
		}
	}

	public void SetOverrideDeathBlow(DeathInfo info)
	{
		cachedOverrideDeathInfo = info;
	}

	internal void LifeStoryLogDeath(HitInfo deathBlow, DamageType lastDamage)
	{
		if (lifeStory == null)
		{
			return;
		}
		lifeStory.timeDied = (uint)Epoch.Current;
		DeathInfo val = cachedOverrideDeathInfo ?? Pool.Get<DeathInfo>();
		val.lastDamageType = (int)lastDamage;
		cachedOverrideDeathInfo = null;
		if (deathBlow != null)
		{
			if ((Object)(object)deathBlow.Initiator != (Object)null)
			{
				deathBlow.Initiator.AttackerInfo(val);
				val.attackerDistance = Distance(deathBlow.Initiator);
			}
			if ((Object)(object)deathBlow.WeaponPrefab != (Object)null)
			{
				val.inflictorName = deathBlow.WeaponPrefab.ShortPrefabName;
			}
			if (deathBlow.HitBone != 0)
			{
				val.hitBone = StringPool.Get(deathBlow.HitBone);
			}
			else
			{
				val.hitBone = "";
			}
		}
		else if (base.SecondsSinceAttacked <= 60f && (Object)(object)lastAttacker != (Object)null)
		{
			lastAttacker.AttackerInfo(val);
		}
		lifeStory.deathInfo = val;
	}

	public void SetSpectateTeamInfo(bool state)
	{
		IsSpectatingTeamInfo = state;
	}

	private void Tick_Spectator()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if (serverInput.WasJustPressed(BUTTON.JUMP))
		{
			num++;
		}
		if (serverInput.WasJustPressed(BUTTON.DUCK))
		{
			num--;
		}
		if (num != 0)
		{
			SpectateOffset += num;
			TimeWarning val = TimeWarning.New("UpdateSpectateTarget", 0);
			try
			{
				UpdateSpectateTarget(spectateFilter);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (!(TimeSince.op_Implicit(lastSpectateTeamInfoUpdate) > 0.5f) || !IsSpectatingTeamInfo)
		{
			return;
		}
		lastSpectateTeamInfoUpdate = TimeSince.op_Implicit(0f);
		SpectateTeamInfo val2 = Pool.Get<SpectateTeamInfo>();
		val2.teams = Pool.GetList<SpectateTeam>();
		val2.teams.Clear();
		foreach (KeyValuePair<ulong, RelationshipManager.PlayerTeam> team in RelationshipManager.ServerInstance.teams)
		{
			SpectateTeam val3 = Pool.Get<SpectateTeam>();
			val3.teamId = team.Key;
			val3.teamMembers = Pool.GetList<TeamMember>();
			val3.teamMembers.Clear();
			foreach (ulong member in team.Value.members)
			{
				TeamMember val4 = Pool.Get<TeamMember>();
				val4.userID = member;
				BasePlayer basePlayer = RelationshipManager.FindByID(member);
				val4.displayName = (((Object)(object)basePlayer != (Object)null) ? basePlayer.displayName : (SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(member) ?? "DEAD"));
				val4.healthFraction = (((Object)(object)basePlayer != (Object)null && basePlayer.IsAlive()) ? basePlayer.healthFraction : 0f);
				val4.position = (((Object)(object)basePlayer != (Object)null) ? ((Component)basePlayer).transform.position : Vector3.zero);
				val4.online = (Object)(object)basePlayer != (Object)null && !basePlayer.IsSleeping();
				val4.wounded = (Object)(object)basePlayer != (Object)null && basePlayer.IsWounded();
				val3.teamMembers.Add(val4);
			}
			val2.teams.Add(val3);
		}
		ClientRPC<SpectateTeamInfo>(RpcTarget.Player("ReceiveSpectateTeamInfo", this), val2);
	}

	public void UpdateSpectateTarget(string strName)
	{
		spectateFilter = strName;
		IEnumerable<BaseEntity> enumerable = null;
		if (spectateFilter.StartsWith("@"))
		{
			string filter = spectateFilter.Substring(1);
			enumerable = (from x in BaseNetworkable.serverEntities
				where StringEx.Contains(((Object)x).name, filter, CompareOptions.IgnoreCase)
				where (Object)(object)x != (Object)(object)this
				select x).Cast<BaseEntity>();
		}
		else
		{
			IEnumerable<BasePlayer> source = ((IEnumerable<BasePlayer>)activePlayerList).Where((BasePlayer x) => !x.IsSpectating() && !x.IsDead() && !x.IsSleeping());
			if (strName.Length > 0)
			{
				source = from x in source
					where StringEx.Contains(x.displayName, spectateFilter, CompareOptions.IgnoreCase) || x.UserIDString.Contains(spectateFilter)
					where (Object)(object)x != (Object)(object)this
					select x;
			}
			source = source.OrderBy((BasePlayer x) => x.displayName);
			enumerable = source.Cast<BaseEntity>();
		}
		BaseEntity[] array = enumerable.ToArray();
		if (array.Length == 0)
		{
			ChatMessage("No valid spectate targets!");
			return;
		}
		BaseEntity baseEntity = array[SpectateOffset % array.Length];
		if ((Object)(object)baseEntity != (Object)null)
		{
			SpectatePlayer(baseEntity);
		}
	}

	public void UpdateSpectateTarget(ulong id)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<BasePlayer> enumerator = activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				if ((Object)(object)current != (Object)null && (ulong)current.userID == id)
				{
					spectateFilter = string.Empty;
					SpectatePlayer(current);
					break;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	private void SpectatePlayer(BaseEntity target)
	{
		if (target is BasePlayer)
		{
			ChatMessage("Spectating: " + (target as BasePlayer).displayName);
		}
		else
		{
			ChatMessage("Spectating: " + ((object)target).ToString());
		}
		TimeWarning val = TimeWarning.New("SendEntitySnapshot", 0);
		try
		{
			SendEntitySnapshot(target);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		((Component)this).gameObject.Identity();
		val = TimeWarning.New("SetParent", 0);
		try
		{
			SetParent(target);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void StartSpectating()
	{
		if (!IsSpectating())
		{
			SetPlayerFlag(PlayerFlags.Spectating, b: true);
			((Component)this).gameObject.SetLayerRecursive(10);
			((FacepunchBehaviour)this).CancelInvoke((Action)InventoryUpdate);
			ChatMessage("Becoming Spectator");
			UpdateSpectateTarget(spectateFilter);
		}
	}

	public void StopSpectating()
	{
		if (IsSpectating())
		{
			SetParent(null);
			SetPlayerFlag(PlayerFlags.Spectating, b: false);
			((Component)this).gameObject.SetLayerRecursive(17);
		}
	}

	public void Teleport(BasePlayer player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Teleport(((Component)player).transform.position);
	}

	public void Teleport(string strName, bool playersOnly)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity[] array = Util.FindTargets(strName, playersOnly);
		if (array != null && array.Length != 0)
		{
			BaseEntity baseEntity = array[Random.Range(0, array.Length)];
			Teleport(((Component)baseEntity).transform.position);
		}
	}

	public void Teleport(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		MovePosition(position);
		ClientRPC<Vector3>(RpcTarget.Player("ForcePositionTo", this), position);
	}

	public void CopyRotation(BasePlayer player)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		viewAngles = player.viewAngles;
		SendNetworkUpdate_Position();
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (child is BasePlayer)
		{
			IsBeingSpectated = true;
		}
	}

	protected override void OnChildRemoved(BaseEntity child)
	{
		base.OnChildRemoved(child);
		if (!(child is BasePlayer))
		{
			return;
		}
		IsBeingSpectated = false;
		foreach (BaseEntity child2 in children)
		{
			if (child2 is BasePlayer)
			{
				IsBeingSpectated = true;
			}
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(10uL)]
	private void UpdateSpectatePositionFromDebugCamera(RPCMessage msg)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (IsSpectating() && Global.updateNetworkPositionWithDebugCameraWhileSpectating)
		{
			Vector3 position = msg.read.Vector3();
			((Component)this).transform.position = position;
			SetParent(null);
		}
	}

	[RPC_Server]
	private void NotifyDebugCameraEnded(RPCMessage msg)
	{
		if (IsSpectating() && Global.updateNetworkPositionWithDebugCameraWhileSpectating)
		{
			UpdateSpectateTarget(spectateFilter);
		}
	}

	public void AddNeabyStash(StashContainer newStash)
	{
		if ((Object)(object)newStash == (Object)null)
		{
			return;
		}
		foreach (NearbyStash nearbyStash in nearbyStashes)
		{
			if ((Object)(object)nearbyStash.Entity == (Object)(object)newStash)
			{
				return;
			}
		}
		if (nearbyStashes.Count == 0)
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)CheckStashRevealInvoke, 0f, StashContainer.PlayerDetectionTickRate);
		}
		nearbyStashes.Add(new NearbyStash(newStash));
	}

	public void RemoveNearbyStash(StashContainer stash)
	{
		for (int i = 0; i < nearbyStashes.Count; i++)
		{
			if (!((Object)(object)nearbyStashes[i].Entity != (Object)(object)stash))
			{
				nearbyStashes.RemoveAt(i);
				break;
			}
		}
		if (nearbyStashes.Count == 0)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)CheckStashRevealInvoke);
		}
	}

	private void CheckStashRevealInvoke()
	{
		for (int i = 0; i < nearbyStashes.Count; i++)
		{
			NearbyStash nearbyStash = nearbyStashes[i];
			if ((Object)(object)nearbyStash.Entity == (Object)null || nearbyStash.Entity.IsDestroyed)
			{
				nearbyStashes.RemoveAt(i);
			}
			else if (nearbyStash.Entity.IsHidden() && nearbyStash.Entity.PlayerInRange(this))
			{
				nearbyStash.LookingAtTime += StashContainer.PlayerDetectionTickRate;
				if (nearbyStash.LookingAtTime >= nearbyStash.Entity.uncoverTime)
				{
					nearbyStash.Entity.SetHidden(isHidden: false);
					Analytics.Azure.OnStashRevealed(this, nearbyStash.Entity);
				}
			}
			else
			{
				nearbyStash.LookingAtTime = 0f;
			}
		}
	}

	public override float GetThreatLevel()
	{
		EnsureUpdated();
		return cachedThreatLevel;
	}

	public void EnsureUpdated()
	{
		if (Time.realtimeSinceStartup - lastUpdateTime < 30f)
		{
			return;
		}
		lastUpdateTime = Time.realtimeSinceStartup;
		cachedThreatLevel = 0f;
		if (IsSleeping())
		{
			return;
		}
		if (inventory.containerWear.itemList.Count > 2)
		{
			cachedThreatLevel += 1f;
		}
		foreach (Item item in inventory.containerBelt.itemList)
		{
			BaseEntity heldEntity = item.GetHeldEntity();
			if (Object.op_Implicit((Object)(object)heldEntity) && heldEntity is BaseProjectile && !(heldEntity is BowWeapon))
			{
				cachedThreatLevel += 2f;
				break;
			}
		}
	}

	public override bool IsHostile()
	{
		return State.unHostileTimestamp > TimeEx.currentTimestamp;
	}

	public virtual float GetHostileDuration()
	{
		return Mathf.Clamp((float)(State.unHostileTimestamp - TimeEx.currentTimestamp), 0f, float.PositiveInfinity);
	}

	public override void MarkHostileFor(float duration = 60f)
	{
		double currentTimestamp = TimeEx.currentTimestamp;
		double val = currentTimestamp + (double)duration;
		State.unHostileTimestamp = Math.Max(State.unHostileTimestamp, val);
		DirtyPlayerState();
		double num = Math.Max(State.unHostileTimestamp - currentTimestamp, 0.0);
		ClientRPC(RpcTarget.Player("SetHostileLength", this), (float)num);
	}

	public void MarkWeaponDrawnDuration(float newDuration)
	{
		float num = weaponDrawnDuration;
		weaponDrawnDuration = newDuration;
		if ((float)Mathf.FloorToInt(newDuration) != num)
		{
			ClientRPC(RpcTarget.Player("SetWeaponDrawnDuration", this), weaponDrawnDuration);
		}
	}

	public void AddWeaponDrawnDuration(float duration)
	{
		MarkWeaponDrawnDuration(weaponDrawnDuration + duration);
	}

	public void OnReceivedTick(Stream stream)
	{
		TimeWarning val = TimeWarning.New("OnReceiveTickFromStream", 0);
		try
		{
			PlayerTick val2 = null;
			TimeWarning val3 = TimeWarning.New("PlayerTick.Deserialize", 0);
			try
			{
				val2 = PlayerTick.Deserialize(stream, lastReceivedTick, true);
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			val3 = TimeWarning.New("RecordPacket", 0);
			try
			{
				net.connection.RecordPacket((byte)15, (IProto)(object)val2);
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			val3 = TimeWarning.New("PlayerTick.Copy", 0);
			try
			{
				lastReceivedTick = val2.Copy();
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			val3 = TimeWarning.New("OnReceiveTick", 0);
			try
			{
				OnReceiveTick(val2, wasStalled);
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			lastTickTime = Time.time;
			val2.Dispose();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void OnReceivedVoice(byte[] data)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
		obj.PacketID((Type)21);
		obj.EntityID(net.ID);
		obj.BytesWithSize(data, false);
		float num = 0f;
		if (HasPlayerFlag(PlayerFlags.VoiceRangeBoost))
		{
			num = Voice.voiceRangeBoostAmount;
		}
		SendInfo val = default(SendInfo);
		((SendInfo)(ref val))._002Ector(BaseNetworkable.GetConnectionsWithin(((Component)this).transform.position, 100f + num));
		val.priority = (Priority)0;
		obj.Send(val);
		if ((Object)(object)activeTelephone != (Object)null)
		{
			activeTelephone.OnReceivedVoiceFromUser(data);
		}
	}

	public void ResetInputIdleTime()
	{
		lastInputTime = Time.time;
	}

	private void EACStateUpdate()
	{
		if (!IsReceivingSnapshot)
		{
			EACServer.LogPlayerTick(this);
		}
	}

	private void OnReceiveTick(PlayerTick msg, bool wasPlayerStalled)
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		if (msg.inputState != null)
		{
			serverInput.Flip(msg.inputState);
		}
		if (serverInput.current.buttons != serverInput.previous.buttons)
		{
			ResetInputIdleTime();
		}
		if (IsReceivingSnapshot)
		{
			return;
		}
		if (IsSpectating())
		{
			TimeWarning val = TimeWarning.New("Tick_Spectator", 0);
			try
			{
				Tick_Spectator();
				return;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (IsDead())
		{
			return;
		}
		if (IsSleeping())
		{
			if (serverInput.WasJustPressed(BUTTON.FIRE_PRIMARY) || serverInput.WasJustPressed(BUTTON.FIRE_SECONDARY) || serverInput.WasJustPressed(BUTTON.JUMP) || serverInput.WasJustPressed(BUTTON.DUCK))
			{
				EndSleeping();
				SendNetworkUpdateImmediate();
			}
			UpdateActiveItem(default(ItemId));
			return;
		}
		if (IsRestrained && restraintItemId.HasValue && restraintItemId.HasValue)
		{
			UpdateActiveItem(restraintItemId.Value);
		}
		else
		{
			UpdateActiveItem(msg.activeItem);
		}
		UpdateModelStateFromTick(msg);
		if (IsIncapacitated())
		{
			return;
		}
		if (isMounted)
		{
			GetMounted().PlayerServerInput(serverInput, this);
		}
		UpdatePositionFromTick(msg, wasPlayerStalled);
		UpdateRotationFromTick(msg);
		int activeMission = GetActiveMission();
		if (activeMission >= 0 && activeMission < missions.Count)
		{
			BaseMission.MissionInstance missionInstance = missions[activeMission];
			if (missionInstance.status == BaseMission.MissionStatus.Active && missionInstance.NeedsPlayerInput())
			{
				ProcessMissionEvent(BaseMission.MissionEventType.PLAYER_TICK, net.ID, 0f);
			}
		}
		if (!TutorialIsland.EnforceTrespassChecks || IsAdmin || IsNpc || net == null || net.group == null)
		{
			return;
		}
		if (net.group.restricted)
		{
			bool flag = false;
			if (!IsInTutorial)
			{
				flag = true;
			}
			else
			{
				TutorialIsland currentTutorialIsland = GetCurrentTutorialIsland();
				if ((Object)(object)currentTutorialIsland == (Object)null || currentTutorialIsland.net.group != net.group)
				{
					flag = true;
				}
			}
			if (flag)
			{
				tutorialKickTime += Time.deltaTime;
				if (tutorialKickTime > 3f)
				{
					Debug.LogWarning((object)$"Killing player {displayName}/{userID} as they are on a tutorial island that doesn't belong them");
					Hurt(999f);
					tutorialKickTime = 0f;
				}
			}
			else
			{
				tutorialKickTime = 0f;
			}
		}
		else
		{
			if (!IsInTutorial || net.group.restricted)
			{
				return;
			}
			bool flag2 = false;
			TutorialIsland currentTutorialIsland2 = GetCurrentTutorialIsland();
			if ((Object)(object)currentTutorialIsland2 == (Object)null || currentTutorialIsland2.net.group != net.group)
			{
				flag2 = true;
			}
			if (flag2)
			{
				tutorialKickTime += Time.deltaTime;
				if (tutorialKickTime > 3f)
				{
					Debug.LogWarning((object)$"Killing player {displayName}/{userID} as they are no longer on a tutorial island and are marked as being in a tutorial");
					Hurt(999f);
					tutorialKickTime = 0f;
				}
			}
			else
			{
				tutorialKickTime = 0f;
			}
		}
	}

	public void UpdateActiveItem(ItemId itemID)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(base.isServer, "Realm should be server!");
		if (svActiveItemID == itemID)
		{
			return;
		}
		if (equippingBlocked)
		{
			itemID = default(ItemId);
		}
		Item item = inventory.containerBelt.FindItemByUID(itemID);
		if (IsItemHoldRestricted(item))
		{
			itemID = default(ItemId);
		}
		Item activeItem = GetActiveItem();
		svActiveItemID = default(ItemId);
		if (activeItem != null)
		{
			HeldEntity heldEntity = activeItem.GetHeldEntity() as HeldEntity;
			if ((Object)(object)heldEntity != (Object)null)
			{
				heldEntity.SetHeld(bHeld: false);
			}
		}
		svActiveItemID = itemID;
		SendNetworkUpdate();
		Item activeItem2 = GetActiveItem();
		if (activeItem2 != null)
		{
			HeldEntity heldEntity2 = activeItem2.GetHeldEntity() as HeldEntity;
			if ((Object)(object)heldEntity2 != (Object)null)
			{
				heldEntity2.SetHeld(bHeld: true);
			}
			NotifyGesturesNewItemEquipped();
		}
		inventory.UpdatedVisibleHolsteredItems();
	}

	internal void UpdateModelStateFromTick(PlayerTick tick)
	{
		if (tick.modelState != null && !ModelState.Equal(modelStateTick, tick.modelState))
		{
			if (modelStateTick != null)
			{
				modelStateTick.ResetToPool();
			}
			modelStateTick = tick.modelState;
			tick.modelState = null;
			tickNeedsFinalizing = true;
		}
	}

	internal void UpdatePositionFromTick(PlayerTick tick, bool wasPlayerStalled)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3Ex.IsNaNOrInfinity(tick.position) || Vector3Ex.IsNaNOrInfinity(tick.eyePos))
		{
			Kick("Kicked: Invalid Position");
		}
		else
		{
			if (tick.parentID != parentEntity.uid || isMounted || (modelState != null && modelState.mounted) || (modelStateTick != null && modelStateTick.mounted) || (IsWounded() && IsRestrained))
			{
				return;
			}
			if (wasPlayerStalled)
			{
				float num = Vector3.Distance(tick.position, tickInterpolator.EndPoint);
				if (num > 0.01f)
				{
					AntiHack.ResetTimer(this);
				}
				if (num > 0.5f)
				{
					ClientRPC<Vector3, NetworkableId>(RpcTarget.Player("ForcePositionToParentOffset", this), tickInterpolator.EndPoint, parentEntity.uid);
				}
			}
			else if ((modelState == null || !modelState.flying || (!IsAdmin && !IsDeveloper)) && Vector3.Distance(tick.position, tickInterpolator.EndPoint) > 5f)
			{
				AntiHack.ResetTimer(this);
				ClientRPC<Vector3, NetworkableId>(RpcTarget.Player("ForcePositionToParentOffset", this), tickInterpolator.EndPoint, parentEntity.uid);
			}
			else
			{
				tickInterpolator.AddPoint(tick.position);
				tickNeedsFinalizing = true;
			}
		}
	}

	internal void UpdateRotationFromTick(PlayerTick tick)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (tick.inputState != null)
		{
			if (Vector3Ex.IsNaNOrInfinity(tick.inputState.aimAngles))
			{
				Kick("Kicked: Invalid Rotation");
				return;
			}
			tickViewAngles = tick.inputState.aimAngles;
			tickNeedsFinalizing = true;
		}
	}

	public void UpdateEstimatedVelocity(Vector3 lastPos, Vector3 currentPos, float deltaTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		estimatedVelocity = (currentPos - lastPos) / deltaTime;
		Vector3 val = estimatedVelocity;
		estimatedSpeed = ((Vector3)(ref val)).magnitude;
		estimatedSpeed2D = Vector3Ex.Magnitude2D(estimatedVelocity);
		if (estimatedSpeed < 0.01f)
		{
			estimatedSpeed = 0f;
		}
		if (estimatedSpeed2D < 0.01f)
		{
			estimatedSpeed2D = 0f;
		}
	}

	private void FinalizeTick(float deltaTime)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		tickDeltaTime += deltaTime;
		if (IsReceivingSnapshot || !tickNeedsFinalizing)
		{
			return;
		}
		tickNeedsFinalizing = false;
		TimeWarning val = TimeWarning.New("ModelState", 0);
		try
		{
			if (modelStateTick != null)
			{
				if (modelStateTick.flying && !IsAdmin && !IsDeveloper)
				{
					AntiHack.NoteAdminHack(this);
				}
				if (modelStateTick.inheritedVelocity != Vector3.zero && (Object)(object)FindTrigger<TriggerForce>() == (Object)null)
				{
					modelStateTick.inheritedVelocity = Vector3.zero;
				}
				if (modelState != null)
				{
					if (ConVar.AntiHack.modelstate && TriggeredAntiHack())
					{
						modelStateTick.ducked = modelState.ducked;
					}
					modelState.ResetToPool();
					modelState = null;
				}
				modelState = modelStateTick;
				modelStateTick = null;
				UpdateModelState();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("Transform", 0);
		try
		{
			UpdateEstimatedVelocity(tickInterpolator.StartPoint, tickInterpolator.EndPoint, tickDeltaTime);
			bool flag = tickInterpolator.StartPoint != tickInterpolator.EndPoint;
			bool flag2 = tickViewAngles != viewAngles;
			if (flag)
			{
				if (AntiHack.ValidateMove(this, tickInterpolator, tickDeltaTime))
				{
					((Component)this).transform.localPosition = tickInterpolator.EndPoint;
					ticksPerSecond.Increment();
					tickHistory.AddPoint(tickInterpolator.EndPoint, tickHistoryCapacity);
					AntiHack.FadeViolations(this, tickDeltaTime);
				}
				else
				{
					flag = false;
					if (ConVar.AntiHack.forceposition)
					{
						ClientRPC<Vector3, NetworkableId>(RpcTarget.Player("ForcePositionToParentOffset", this), ((Component)this).transform.localPosition, parentEntity.uid);
					}
				}
			}
			tickInterpolator.Reset(((Component)this).transform.localPosition);
			if (flag2)
			{
				viewAngles = tickViewAngles;
				((Component)this).transform.rotation = Quaternion.identity;
				((Component)this).transform.hasChanged = true;
			}
			if (flag || flag2)
			{
				eyes.NetworkUpdate(Quaternion.Euler(viewAngles));
				NetworkPositionTick();
			}
			AntiHack.ValidateEyeHistory(this);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("ModelState", 0);
		try
		{
			if (modelState != null)
			{
				modelState.waterLevel = WaterFactor();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("EACStateUpdate", 0);
		try
		{
			EACStateUpdate();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("AntiHack.EnforceViolations", 0);
		try
		{
			AntiHack.EnforceViolations(this);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		tickDeltaTime = 0f;
	}

	public bool IsCraftingTutorialBlocked(ItemDefinition def, out bool forceUnlock)
	{
		forceUnlock = false;
		if (!IsInTutorial)
		{
			return false;
		}
		if (def.tutorialAllowance == TutorialItemAllowance.None)
		{
			return true;
		}
		bool num = CurrentTutorialAllowance >= def.tutorialAllowance;
		if (num && (Object)(object)def.Blueprint != (Object)null && !def.Blueprint.defaultBlueprint)
		{
			forceUnlock = true;
		}
		return !num;
	}

	public bool CanModifyCraftAmountDuringTutorial()
	{
		if (IsInTutorial)
		{
			return CurrentTutorialAllowance >= TutorialItemAllowance.Level4_Spear_Fire;
		}
		return false;
	}

	public TutorialIsland GetCurrentTutorialIsland()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!IsInTutorial)
		{
			return null;
		}
		Enumerator<TutorialIsland> enumerator = TutorialIsland.GetTutorialList(base.isServer).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				TutorialIsland current = enumerator.Current;
				if ((Object)(object)current.ForPlayer.Get(base.isServer) == (Object)(object)this)
				{
					return current;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return null;
	}

	public void ClearTutorial()
	{
		SetPlayerFlag(PlayerFlags.IsInTutorial, b: false);
		SleepingBag.ClearTutorialBagsForPlayer(userID);
	}

	public void ClearTutorial_PostDeath()
	{
		ClearAllPings();
		ClearDeathMarker();
		WipeMissions();
		SendPingsToClient();
		SendMarkersToClient();
	}

	public void OnStartedTutorial()
	{
		ClearAllPings();
		WipeMissions();
	}

	public void SetTutorialAllowance(TutorialItemAllowance newAllowance)
	{
		if (newAllowance >= CurrentTutorialAllowance)
		{
			CurrentTutorialAllowance = newAllowance;
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	private void StartTutorial(RPCMessage msg)
	{
		if (!((Object)(object)msg.player != (Object)(object)this))
		{
			StartTutorial(triggerAnalytics: true);
		}
	}

	public void StartTutorial(bool triggerAnalytics)
	{
		if (ConVar.Server.tutorialEnabled)
		{
			if (!TutorialIsland.HasAvailableTutorialIsland)
			{
				ShowToast(GameTip.Styles.Red_Normal, TutorialIsland.NoTutorialIslandsAvailablePhrase);
			}
			else if (startTutorialCooldown > Time.realtimeSinceStartup)
			{
				int num = Mathf.CeilToInt(startTutorialCooldown - Time.realtimeSinceStartup);
				ShowToast(GameTip.Styles.Red_Normal, TutorialIsland.TutorialIslandStartCooldown, num.ToString());
			}
			else
			{
				startTutorialCooldown = Time.realtimeSinceStartup + (float)Debugging.tutorial_start_cooldown;
				Hurt(99999f);
				Respawn();
				TutorialIsland.RestoreOrCreateIslandForPlayer(this, triggerAnalytics);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(1uL)]
	private void PlayerRequestedTutorialStart(RPCMessage msg)
	{
		if (ConVar.Server.tutorialEnabled)
		{
			if (!TutorialIsland.HasAvailableTutorialIsland)
			{
				ShowToast(GameTip.Styles.Red_Normal, TutorialIsland.NoTutorialIslandsAvailablePhrase);
			}
			else
			{
				ClientRPC(RpcTarget.Player("PromptToStartTutorial", this));
			}
		}
	}

	public uint GetUnderwearSkin()
	{
		uint infoInt = (uint)GetInfoInt("client.underwearskin", 0);
		if (infoInt != lastValidUnderwearSkin && Time.time > nextUnderwearValidationTime)
		{
			UnderwearManifest underwearManifest = UnderwearManifest.Get();
			nextUnderwearValidationTime = Time.time + 0.2f;
			Underwear underwear = underwearManifest.GetUnderwear(infoInt);
			if ((Object)(object)underwear == (Object)null)
			{
				lastValidUnderwearSkin = 0u;
			}
			else if (Underwear.Validate(underwear, this))
			{
				lastValidUnderwearSkin = infoInt;
			}
		}
		return lastValidUnderwearSkin;
	}

	[RPC_Server]
	public void ServerRPC_UnderwearChange(RPCMessage msg)
	{
		if (!((Object)(object)msg.player != (Object)(object)this))
		{
			uint num = lastValidUnderwearSkin;
			uint underwearSkin = GetUnderwearSkin();
			if (num != underwearSkin)
			{
				SendNetworkUpdate();
			}
		}
	}

	public bool IsWounded()
	{
		return HasPlayerFlag(PlayerFlags.Wounded);
	}

	public bool IsCrawling()
	{
		if (HasPlayerFlag(PlayerFlags.Wounded))
		{
			return !HasPlayerFlag(PlayerFlags.Incapacitated);
		}
		return false;
	}

	public bool IsIncapacitated()
	{
		return HasPlayerFlag(PlayerFlags.Incapacitated);
	}

	private bool WoundInsteadOfDying(HitInfo info)
	{
		if (!EligibleForWounding(info))
		{
			return false;
		}
		BecomeWounded(info);
		return true;
	}

	private void ResetWoundingVars()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)WoundingTick);
		woundedDuration = 0f;
		lastWoundedStartTime = float.NegativeInfinity;
		healingWhileCrawling = 0f;
		woundedByFallDamage = false;
	}

	public virtual bool EligibleForWounding(HitInfo info)
	{
		if (!ConVar.Server.woundingenabled)
		{
			return false;
		}
		if (IsWounded())
		{
			return false;
		}
		if (IsSleeping())
		{
			return false;
		}
		if (isMounted)
		{
			return false;
		}
		if (info == null)
		{
			return false;
		}
		if (!IsWounded() && Time.realtimeSinceStartup - lastWoundedStartTime < ConVar.Server.rewounddelay)
		{
			return false;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if (Object.op_Implicit((Object)(object)activeGameMode) && !activeGameMode.allowWounding)
		{
			return false;
		}
		if (triggers != null)
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				if (triggers[i] is IHurtTrigger)
				{
					return false;
				}
			}
		}
		if (info.WeaponPrefab is BaseMelee)
		{
			return true;
		}
		if (info.WeaponPrefab is BaseProjectile)
		{
			return !info.isHeadshot;
		}
		return info.damageTypes.GetMajorityDamageType() switch
		{
			DamageType.Suicide => false, 
			DamageType.Fall => true, 
			DamageType.Bite => true, 
			DamageType.Bleeding => true, 
			DamageType.Hunger => true, 
			DamageType.Thirst => true, 
			DamageType.Poison => true, 
			_ => false, 
		};
	}

	public void BecomeWounded(HitInfo info = null)
	{
		if (IsWounded())
		{
			return;
		}
		bool flag = info != null && info.damageTypes.GetMajorityDamageType() == DamageType.Fall;
		if (IsCrawling())
		{
			woundedByFallDamage |= flag;
			GoToIncapacitated(info);
			return;
		}
		woundedByFallDamage = flag;
		if (flag || !ConVar.Server.crawlingenabled)
		{
			GoToIncapacitated(info);
		}
		else
		{
			GoToCrawling(info);
		}
	}

	public void StopWounded(BasePlayer source = null)
	{
		if (IsWounded())
		{
			RecoverFromWounded();
			((FacepunchBehaviour)this).CancelInvoke((Action)WoundingTick);
			EACServer.LogPlayerRevive(source, this);
		}
	}

	public void ProlongWounding(float delay)
	{
		if (!IsRestrained)
		{
			woundedDuration = Mathf.Max(woundedDuration, Mathf.Min(TimeSinceWoundedStarted + delay, woundedDuration + delay));
			SendWoundedInformation(woundedDuration);
		}
	}

	public void SendWoundedInformation(float timeLeft)
	{
		float recoveryChance = GetRecoveryChance();
		ClientRPC(RpcTarget.Player("CLIENT_GetWoundedInformation", this), recoveryChance, timeLeft, woundedDuration);
	}

	public float GetRecoveryChance()
	{
		float num = (IsIncapacitated() ? ConVar.Server.incapacitatedrecoverchance : ConVar.Server.woundedrecoverchance);
		float num2 = (metabolism.hydration.Fraction() + metabolism.calories.Fraction()) / 2f;
		float num3 = Mathf.Lerp(0f, ConVar.Server.woundedmaxfoodandwaterbonus, num2);
		float result = Mathf.Clamp01(num + num3);
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("largemedkit");
		if (inventory.containerBelt.FindItemByItemID(itemDefinition.itemid) != null && !woundedByFallDamage)
		{
			return 1f;
		}
		return result;
	}

	private void WoundingTick()
	{
		TimeWarning val = TimeWarning.New("WoundingTick", 0);
		try
		{
			if (IsDead())
			{
				return;
			}
			if (!Player.woundforever && TimeSinceWoundedStarted >= woundedDuration)
			{
				float num = (IsIncapacitated() ? ConVar.Server.incapacitatedrecoverchance : ConVar.Server.woundedrecoverchance);
				float num2 = (metabolism.hydration.Fraction() + metabolism.calories.Fraction()) / 2f;
				float num3 = Mathf.Lerp(0f, ConVar.Server.woundedmaxfoodandwaterbonus, num2);
				float num4 = Mathf.Clamp01(num + num3);
				if (Random.value < num4)
				{
					RecoverFromWounded();
					return;
				}
				if (woundedByFallDamage)
				{
					Die();
					return;
				}
				ItemDefinition itemDefinition = ItemManager.FindItemDefinition("largemedkit");
				Item item = inventory.containerBelt.FindItemByItemID(itemDefinition.itemid);
				if (item != null)
				{
					item.UseItem();
					RecoverFromWounded();
				}
				else
				{
					Die();
				}
			}
			else
			{
				if (IsSwimming() && IsCrawling())
				{
					GoToIncapacitated(null);
				}
				((FacepunchBehaviour)this).Invoke((Action)WoundingTick, 1f);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void GoToCrawling(HitInfo info)
	{
		base.health = Random.Range(ConVar.Server.crawlingminimumhealth, ConVar.Server.crawlingmaximumhealth);
		metabolism.bleeding.value = 0f;
		healingWhileCrawling = 0f;
		WoundedStartSharedCode(info);
		StartWoundedTick(40, 50);
		SendWoundedInformation(woundedDuration);
		SendNetworkUpdateImmediate();
	}

	public void GoToIncapacitated(HitInfo info)
	{
		if (!IsWounded())
		{
			WoundedStartSharedCode(info);
		}
		base.health = Random.Range(2f, 6f);
		metabolism.bleeding.value = 0f;
		healingWhileCrawling = 0f;
		SetPlayerFlag(PlayerFlags.Incapacitated, b: true);
		SetServerFall(wantsOn: true);
		inventory.DropBackpackOnDeath();
		StartWoundedTick(10, 25);
		SendWoundedInformation(woundedDuration);
		SendNetworkUpdateImmediate();
	}

	private void WoundedStartSharedCode(HitInfo info)
	{
		stats.Add("wounded", 1, (Stats)5);
		SetPlayerFlag(PlayerFlags.Wounded, b: true);
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(base.isServer)))
		{
			BaseGameMode.GetActiveGameMode(base.isServer).OnPlayerWounded(info.InitiatorPlayer, this, info);
		}
	}

	private void StartWoundedTick(int minTime, int maxTime)
	{
		woundedDuration = Random.Range(minTime, maxTime + 1);
		ApplyWoundedStartTime();
		((FacepunchBehaviour)this).Invoke((Action)WoundingTick, 1f);
	}

	public void ApplyWoundedStartTime()
	{
		lastWoundedStartTime = Time.realtimeSinceStartup;
	}

	private void RecoverFromWounded()
	{
		if (IsCrawling())
		{
			base.health = Random.Range(2f, 6f) + healingWhileCrawling;
		}
		healingWhileCrawling = 0f;
		SetPlayerFlag(PlayerFlags.Wounded, b: false);
		SetPlayerFlag(PlayerFlags.Incapacitated, b: false);
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(base.isServer)))
		{
			BaseGameMode.GetActiveGameMode(base.isServer).OnPlayerRevived(null, this);
		}
	}

	private bool WoundingCausingImmortality(HitInfo info)
	{
		if (!IsWounded())
		{
			return false;
		}
		if (TimeSinceWoundedStarted > 0.25f)
		{
			return false;
		}
		if (info != null && info.damageTypes.GetMajorityDamageType() == DamageType.Fall)
		{
			return false;
		}
		return true;
	}

	public override BasePlayer ToPlayer()
	{
		return this;
	}

	public static string SanitizePlayerNameString(string playerName, ulong userId)
	{
		playerName = StringEx.EscapeRichText(StringEx.ToPrintable(playerName, 32)).Trim();
		if (string.IsNullOrWhiteSpace(playerName))
		{
			playerName = userId.ToString();
		}
		return playerName;
	}

	public bool IsGod()
	{
		if (base.isServer && (IsAdmin || IsDeveloper) && IsConnected && net.connection != null && net.connection.info.GetBool("global.god", false))
		{
			return true;
		}
		return false;
	}

	public override Quaternion GetNetworkRotation()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			return Quaternion.Euler(viewAngles);
		}
		return Quaternion.identity;
	}

	public bool CanInteract()
	{
		return CanInteract(usableWhileCrawling: false);
	}

	public bool CanInteract(bool usableWhileCrawling)
	{
		bool flag = CurrentGestureIsSurrendering;
		if (!flag && IsRestrained)
		{
			Handcuffs restraintItem = Belt.GetRestraintItem();
			flag = (Object)(object)restraintItem != (Object)null && restraintItem.BlockUse;
		}
		if (!IsDead() && !IsSleeping() && !IsSpectating() && (usableWhileCrawling ? (!IsIncapacitated()) : (!IsWounded())) && !HasActiveTelephone)
		{
			return !flag;
		}
		return false;
	}

	public override float StartHealth()
	{
		return Random.Range(50f, 60f);
	}

	public override float StartMaxHealth()
	{
		return 100f;
	}

	public override float MaxHealth()
	{
		return 100f * (1f + (((Object)(object)modifiers != (Object)null) ? modifiers.GetValue(Modifier.ModifierType.Max_Health) : 0f));
	}

	public override float MaxVelocity()
	{
		if (IsSleeping())
		{
			return 0f;
		}
		if (isMounted)
		{
			return GetMounted().MaxVelocity();
		}
		return GetMaxSpeed();
	}

	public override OBB WorldSpaceBounds()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (IsSleeping())
		{
			Vector3 center = ((Bounds)(ref bounds)).center;
			Vector3 size = ((Bounds)(ref bounds)).size;
			center.y /= 2f;
			size.y /= 2f;
			return new OBB(((Component)this).transform.position, ((Component)this).transform.lossyScale, ((Component)this).transform.rotation, new Bounds(center, size));
		}
		return base.WorldSpaceBounds();
	}

	public Vector3 GetMountVelocity()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		BaseMountable baseMountable = GetMounted();
		if (!((Object)(object)baseMountable != (Object)null))
		{
			return Vector3.zero;
		}
		return baseMountable.GetWorldVelocity();
	}

	public override Vector3 GetInheritedProjectileVelocity(Vector3 direction)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		BaseMountable baseMountable = GetMounted();
		if (!Object.op_Implicit((Object)(object)baseMountable))
		{
			return base.GetInheritedProjectileVelocity(direction);
		}
		return baseMountable.GetInheritedProjectileVelocity(direction);
	}

	public override Vector3 GetInheritedThrowVelocity(Vector3 direction)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		BaseMountable baseMountable = GetMounted();
		if (!Object.op_Implicit((Object)(object)baseMountable))
		{
			return base.GetInheritedThrowVelocity(direction);
		}
		return baseMountable.GetInheritedThrowVelocity(direction);
	}

	public override Vector3 GetInheritedDropVelocity()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		BaseMountable baseMountable = GetMounted();
		if (!Object.op_Implicit((Object)(object)baseMountable))
		{
			return base.GetInheritedDropVelocity();
		}
		return baseMountable.GetInheritedDropVelocity();
	}

	public override void PreInitShared()
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		base.PreInitShared();
		cachedProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		baseProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		inventoryValue.Set(((Component)this).GetComponent<PlayerInventory>());
		blueprints = ((Component)this).GetComponent<PlayerBlueprints>();
		metabolism = ((Component)this).GetComponent<PlayerMetabolism>();
		modifiers = ((Component)this).GetComponent<PlayerModifiers>();
		colliderValue.Set(((Component)this).GetComponent<CapsuleCollider>());
		eyesValue.Set(((Component)this).GetComponent<PlayerEyes>());
		playerColliderStanding = new CapsuleColliderInfo(playerCollider.height, playerCollider.radius, playerCollider.center);
		playerColliderDucked = new CapsuleColliderInfo(1.5f, playerCollider.radius, Vector3.up * 0.75f);
		playerColliderCrawling = new CapsuleColliderInfo(playerCollider.radius, playerCollider.radius, Vector3.up * playerCollider.radius);
		playerColliderLyingDown = new CapsuleColliderInfo(0.4f, playerCollider.radius, Vector3.up * 0.2f);
		Belt = new PlayerBelt(this);
	}

	public override void DestroyShared()
	{
		Object.Destroy((Object)(object)cachedProtection);
		Object.Destroy((Object)(object)baseProtection);
		base.DestroyShared();
	}

	public override bool InSafeZone()
	{
		if (base.isServer)
		{
			return base.InSafeZone();
		}
		return false;
	}

	public bool IsInNoRespawnZone()
	{
		if (base.isServer)
		{
			return InNoRespawnZone();
		}
		return false;
	}

	public bool IsOnATugboat()
	{
		if (GetMountedVehicle() is Tugboat)
		{
			return true;
		}
		if (GetParentEntity() is Tugboat)
		{
			return true;
		}
		return false;
	}

	public static void ServerCycle(float deltaTime)
	{
		for (int i = 0; i < activePlayerList.Values.Count; i++)
		{
			if ((Object)(object)activePlayerList.Values[i] == (Object)null)
			{
				activePlayerList.RemoveAt(i--);
			}
		}
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		for (int j = 0; j < activePlayerList.Count; j++)
		{
			list.Add(activePlayerList[j]);
		}
		for (int k = 0; k < list.Count; k++)
		{
			if (!((Object)(object)list[k] == (Object)null))
			{
				list[k].ServerUpdate(deltaTime);
			}
		}
		for (int l = 0; l < bots.Count; l++)
		{
			if (!((Object)(object)bots[l] == (Object)null))
			{
				bots[l].ServerUpdateBots(deltaTime);
			}
		}
		if (ConVar.Server.idlekick > 0 && ((SingletonComponent<ServerMgr>.Instance.AvailableSlots <= 0 && ConVar.Server.idlekickmode == 1) || ConVar.Server.idlekickmode == 2))
		{
			for (int m = 0; m < list.Count; m++)
			{
				if (!(list[m].IdleTime < (float)(ConVar.Server.idlekick * 60)) && (!list[m].IsAdmin || ConVar.Server.idlekickadmins != 0) && (!list[m].IsDeveloper || ConVar.Server.idlekickadmins != 0))
				{
					list[m].Kick("Idle for " + ConVar.Server.idlekick + " minutes");
				}
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
	}

	private bool ManuallyCheckSafezone()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return false;
		}
		List<Collider> list = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(((Component)this).transform.position, 0f, list, -1, (QueryTriggerInteraction)2);
		foreach (Collider item in list)
		{
			if ((Object)(object)((Component)item).GetComponent<TriggerSafeZone>() != (Object)null)
			{
				Pool.FreeList<Collider>(ref list);
				return true;
			}
		}
		Pool.FreeList<Collider>(ref list);
		return false;
	}

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if ((baseEntity.InSafeZone() || InSafeZone() || ManuallyCheckSafezone()) && (ulong)baseEntity.userID != (ulong)userID)
		{
			return false;
		}
		if ((Object)(object)RelationshipManager.ServerInstance != (Object)null)
		{
			if ((IsSleeping() || IsIncapacitated()) && !RelationshipManager.ServerInstance.HasRelations(baseEntity.userID, userID))
			{
				RelationshipManager.ServerInstance.SetRelationship(baseEntity, this, RelationshipManager.RelationshipType.Acquaintance);
			}
			RelationshipManager.ServerInstance.SetSeen(baseEntity, this);
		}
		if (IsCrawling())
		{
			GoToIncapacitated(null);
		}
		if ((Object)(object)inventory.crafting != (Object)null)
		{
			inventory.crafting.CancelAll(returnItems: true);
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	public Bounds GetBounds(bool ducked)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new Bounds(((Component)this).transform.position + GetOffset(ducked), GetSize(ducked));
	}

	public Bounds GetBounds()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetBounds(modelState.ducked);
	}

	public Vector3 GetCenter(bool ducked)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position + GetOffset(ducked);
	}

	public Vector3 GetCenter()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetCenter(modelState.ducked);
	}

	public Vector3 GetOffset(bool ducked)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (ducked)
		{
			return new Vector3(0f, 0.55f, 0f);
		}
		return new Vector3(0f, 0.9f, 0f);
	}

	public Vector3 GetOffset()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetOffset(modelState.ducked);
	}

	public Vector3 GetSize(bool ducked)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (ducked)
		{
			return new Vector3(1f, 1.1f, 1f);
		}
		return new Vector3(1f, 1.8f, 1f);
	}

	public Vector3 GetSize()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetSize(modelState.ducked);
	}

	public float GetHeight(bool ducked)
	{
		if (ducked)
		{
			return 1.1f;
		}
		return 1.8f;
	}

	public float GetHeight()
	{
		return GetHeight(modelState.ducked);
	}

	public float GetRadius()
	{
		return 0.5f;
	}

	public float GetJumpHeight()
	{
		return 1.5f;
	}

	public override Vector3 TriggerPoint()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position + NoClipOffset();
	}

	public Vector3 NoClipOffset()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(0f, GetHeight(ducked: true) - GetRadius(), 0f);
	}

	public float NoClipRadius(float margin)
	{
		return GetRadius() - margin;
	}

	public float MaxDeployDistance(Item item)
	{
		return 8f;
	}

	public float GetMinSpeed()
	{
		return GetSpeed(0f, 0f, 1f);
	}

	public float GetMaxSpeed()
	{
		return GetSpeed(1f, 0f, 0f);
	}

	public float GetSpeed(float running, float ducking, float crawling)
	{
		float num = 1f;
		num -= clothingMoveSpeedReduction;
		if (IsSwimming())
		{
			num += clothingWaterSpeedBonus;
		}
		if (crawling > 0f)
		{
			return Mathf.Lerp(2.8f, 0.72f, crawling) * num;
		}
		return Mathf.Lerp(Mathf.Lerp(2.8f, 5.5f, running), 1.7f, ducking) * num * weaponMoveSpeedScale;
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		float oldHealth = base.health;
		if (InSafeZone() && !IsHostile() && (Object)(object)info.Initiator != (Object)null && (Object)(object)info.Initiator != (Object)(object)this)
		{
			info.damageTypes.ScaleAll(0f);
		}
		if (base.isServer)
		{
			HitArea boneArea = info.boneArea;
			if (boneArea != (HitArea)(-1))
			{
				List<Item> list = Pool.GetList<Item>();
				list.AddRange(inventory.containerWear.itemList);
				for (int i = 0; i < list.Count; i++)
				{
					Item item = list[i];
					if (item != null)
					{
						ItemModWearable component = ((Component)item.info).GetComponent<ItemModWearable>();
						if (!((Object)(object)component == (Object)null) && component.ProtectsArea(boneArea))
						{
							item.OnAttacked(info);
						}
					}
				}
				Pool.FreeList<Item>(ref list);
				inventory.ServerUpdate(0f);
			}
		}
		base.OnAttacked(info);
		if (base.isServer && base.isServer && info.hasDamage)
		{
			if (!info.damageTypes.Has(DamageType.Bleeding) && info.damageTypes.IsBleedCausing() && !IsWounded() && !IsImmortalTo(info))
			{
				metabolism.bleeding.Add(info.damageTypes.Total() * 0.2f);
			}
			if (isMounted)
			{
				GetMounted().MounteeTookDamage(this, info);
			}
			CheckDeathCondition(info);
			if (net != null && net.connection != null)
			{
				ClientRPC(RpcTarget.Player("TakeDamageHit", this));
			}
			string text = StringPool.Get(info.HitBone);
			Vector3 val = info.PointEnd - info.PointStart;
			bool flag = Vector3.Dot(((Vector3)(ref val)).normalized, eyes.BodyForward()) > 0.4f;
			BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (Object.op_Implicit((Object)(object)initiatorPlayer) && !info.damageTypes.IsMeleeType())
			{
				initiatorPlayer.LifeStoryShotHit(info.Weapon);
			}
			if (info.isHeadshot)
			{
				if (flag)
				{
					SignalBroadcast(Signal.Flinch_RearHead, string.Empty);
				}
				else
				{
					SignalBroadcast(Signal.Flinch_Head, string.Empty);
				}
				Effect.server.Run("assets/bundled/prefabs/fx/headshot.prefab", this, 0u, new Vector3(0f, 2f, 0f), Vector3.zero, ((Object)(object)initiatorPlayer != (Object)null) ? initiatorPlayer.net.connection : null);
				if (Object.op_Implicit((Object)(object)initiatorPlayer))
				{
					initiatorPlayer.stats.Add("headshot", 1, (Stats)5);
					if (initiatorPlayer.IsBeingSpectated)
					{
						foreach (BaseEntity child in initiatorPlayer.children)
						{
							if (child is BasePlayer basePlayer)
							{
								basePlayer.ClientRPC(RpcTarget.Player("SpectatedPlayerHeadshot", basePlayer));
							}
						}
					}
				}
			}
			else if (flag)
			{
				SignalBroadcast(Signal.Flinch_RearTorso, string.Empty);
			}
			else if (text == "spine" || text == "spine2")
			{
				SignalBroadcast(Signal.Flinch_Stomach, string.Empty);
			}
			else
			{
				SignalBroadcast(Signal.Flinch_Chest, string.Empty);
			}
		}
		if (stats != null)
		{
			if (IsWounded())
			{
				stats.combat.LogAttack(info, "wounded", oldHealth);
			}
			else if (IsDead())
			{
				stats.combat.LogAttack(info, "killed", oldHealth);
			}
			else
			{
				stats.combat.LogAttack(info, "", oldHealth);
			}
		}
		if (Global.cinematicGingerbreadCorpses)
		{
			info.HitMaterial = Global.GingerbreadMaterialID();
		}
	}

	private void EnablePlayerCollider()
	{
		if (!((Collider)playerCollider).enabled)
		{
			RefreshColliderSize(forced: true);
			((Collider)playerCollider).enabled = true;
		}
	}

	private void DisablePlayerCollider()
	{
		if (((Collider)playerCollider).enabled)
		{
			RemoveFromTriggers();
			((Collider)playerCollider).enabled = false;
		}
	}

	private void RefreshColliderSize(bool forced)
	{
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (forced || (((Collider)playerCollider).enabled && !(Time.time < nextColliderRefreshTime)))
		{
			nextColliderRefreshTime = Time.time + 0.25f + Random.Range(-0.05f, 0.05f);
			BaseMountable baseMountable = GetMounted();
			CapsuleColliderInfo capsuleColliderInfo = (((Object)(object)baseMountable != (Object)null && baseMountable.IsValid()) ? ((!baseMountable.modifiesPlayerCollider) ? playerColliderStanding : baseMountable.customPlayerCollider) : ((!IsIncapacitated() && !IsSleeping()) ? (IsCrawling() ? playerColliderCrawling : ((!modelState.ducked && !IsSwimming()) ? playerColliderStanding : playerColliderDucked)) : playerColliderLyingDown));
			if (playerCollider.height != capsuleColliderInfo.height || playerCollider.radius != capsuleColliderInfo.radius || playerCollider.center != capsuleColliderInfo.center)
			{
				playerCollider.height = capsuleColliderInfo.height;
				playerCollider.radius = capsuleColliderInfo.radius;
				playerCollider.center = capsuleColliderInfo.center;
			}
		}
	}

	private void SetPlayerRigidbodyState(bool isEnabled)
	{
		if (isEnabled)
		{
			AddPlayerRigidbody();
		}
		else
		{
			RemovePlayerRigidbody();
		}
	}

	private void AddPlayerRigidbody()
	{
		if ((Object)(object)playerRigidbody == (Object)null)
		{
			playerRigidbody = ((Component)this).gameObject.GetComponent<Rigidbody>();
		}
		if ((Object)(object)playerRigidbody == (Object)null)
		{
			playerRigidbody = ((Component)this).gameObject.AddComponent<Rigidbody>();
			playerRigidbody.useGravity = false;
			playerRigidbody.isKinematic = true;
			playerRigidbody.mass = 1f;
			playerRigidbody.interpolation = (RigidbodyInterpolation)0;
			playerRigidbody.collisionDetectionMode = (CollisionDetectionMode)0;
		}
	}

	private void RemovePlayerRigidbody()
	{
		if ((Object)(object)playerRigidbody == (Object)null)
		{
			playerRigidbody = ((Component)this).gameObject.GetComponent<Rigidbody>();
		}
		if ((Object)(object)playerRigidbody != (Object)null)
		{
			RemoveFromTriggers();
			Object.DestroyImmediate((Object)(object)playerRigidbody);
			playerRigidbody = null;
		}
	}

	public bool IsEnsnared()
	{
		if (triggers == null)
		{
			return false;
		}
		for (int i = 0; i < triggers.Count; i++)
		{
			if (triggers[i] is TriggerEnsnare)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAttacking()
	{
		HeldEntity heldEntity = GetHeldEntity();
		if ((Object)(object)heldEntity == (Object)null)
		{
			return false;
		}
		AttackEntity attackEntity = heldEntity as AttackEntity;
		if ((Object)(object)attackEntity == (Object)null)
		{
			return false;
		}
		return attackEntity.NextAttackTime - Time.time > attackEntity.repeatDelay - 1f;
	}

	public bool CanAttack()
	{
		HeldEntity heldEntity = GetHeldEntity();
		if ((Object)(object)heldEntity == (Object)null)
		{
			return false;
		}
		bool flag = IsSwimming();
		bool flag2 = heldEntity.CanBeUsedInWater();
		if (modelState.onLadder)
		{
			return false;
		}
		if (!flag && !modelState.onground)
		{
			return false;
		}
		if (flag && !flag2)
		{
			return false;
		}
		if (IsEnsnared())
		{
			return false;
		}
		return true;
	}

	public bool OnLadder()
	{
		if (modelState.onLadder && !IsWounded())
		{
			return Object.op_Implicit((Object)(object)FindTrigger<TriggerLadder>());
		}
		return false;
	}

	public bool IsSwimming()
	{
		return WaterFactor() >= 0.65f;
	}

	public bool IsHeadUnderwater()
	{
		return WaterFactor() > 0.75f;
	}

	public virtual bool IsOnGround()
	{
		return modelState.onground;
	}

	public bool IsRunning()
	{
		if (modelState != null)
		{
			return modelState.sprinting;
		}
		return false;
	}

	public bool IsDucked()
	{
		if (modelState != null)
		{
			return modelState.ducked;
		}
		return false;
	}

	public void ShowToast(GameTip.Styles style, Phrase phrase, params string[] arguments)
	{
		if (base.isServer)
		{
			SendConsoleCommand("gametip.showtoast_translated", (int)style, phrase.token, phrase.english, arguments);
		}
	}

	public void ChatMessage(string msg)
	{
		if (base.isServer)
		{
			SendConsoleCommand("chat.add", 2, 0, msg);
		}
	}

	public void ConsoleMessage(string msg)
	{
		if (base.isServer)
		{
			SendConsoleCommand("echo " + msg);
		}
	}

	public override float PenetrationResistance(HitInfo info)
	{
		return 100f;
	}

	public override void ScaleDamage(HitInfo info)
	{
		if (isMounted)
		{
			GetMounted().ScaleDamageForPlayer(this, info);
		}
		if (info.UseProtection)
		{
			HitArea boneArea = info.boneArea;
			if (boneArea != (HitArea)(-1))
			{
				cachedProtection.Clear();
				cachedProtection.Add(inventory.containerWear.itemList, boneArea);
				cachedProtection.Multiply(DamageType.Arrow, ConVar.Server.arrowarmor);
				cachedProtection.Multiply(DamageType.Bullet, ConVar.Server.bulletarmor);
				cachedProtection.Multiply(DamageType.Slash, ConVar.Server.meleearmor);
				cachedProtection.Multiply(DamageType.Blunt, ConVar.Server.meleearmor);
				cachedProtection.Multiply(DamageType.Stab, ConVar.Server.meleearmor);
				cachedProtection.Multiply(DamageType.Bleeding, ConVar.Server.bleedingarmor);
				cachedProtection.Scale(info.damageTypes);
			}
			else
			{
				baseProtection.Scale(info.damageTypes);
			}
		}
		if (Object.op_Implicit((Object)(object)info.damageProperties))
		{
			info.damageProperties.ScaleDamage(info);
		}
	}

	public void ResetWeaponMoveSpeedScale()
	{
		weaponMoveSpeedScale = 1f;
	}

	private void UpdateMoveSpeedFromClothing()
	{
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		bool flag = false;
		bool flag2 = false;
		float num4 = 0f;
		eggVision = 0f;
		base.Weight = 0f;
		foreach (Item item in inventory.containerWear.itemList)
		{
			ItemModWearable component = ((Component)item.info).GetComponent<ItemModWearable>();
			if (Object.op_Implicit((Object)(object)component))
			{
				if (component.blocksAiming)
				{
					flag = true;
				}
				if (component.blocksEquipping)
				{
					flag2 = true;
				}
				num4 += component.accuracyBonus;
				eggVision += component.eggVision;
				base.Weight += component.weight;
				if ((Object)(object)component.movementProperties != (Object)null)
				{
					num2 += component.movementProperties.speedReduction;
					num = Mathf.Max(num, component.movementProperties.minSpeedReduction);
					num3 += component.movementProperties.waterSpeedBonus;
				}
			}
		}
		clothingAccuracyBonus = num4;
		clothingMoveSpeedReduction = Mathf.Max(num2, num);
		clothingBlocksAiming = flag;
		clothingWaterSpeedBonus = num3;
		equippingBlocked = flag2;
		if (base.isServer && equippingBlocked)
		{
			UpdateActiveItem(default(ItemId));
		}
	}

	public virtual void UpdateProtectionFromClothing()
	{
		baseProtection.Clear();
		baseProtection.Add(inventory.containerWear.itemList);
		float num = 1f / 6f;
		for (int i = 0; i < baseProtection.amounts.Length; i++)
		{
			switch (i)
			{
			case 22:
				baseProtection.amounts[i] = 1f;
				break;
			default:
				baseProtection.amounts[i] *= num;
				break;
			case 17:
				break;
			}
		}
	}

	public override string Categorize()
	{
		return "player";
	}

	public override string ToString()
	{
		if (_name == null)
		{
			if (base.isServer)
			{
				_name = $"{displayName}[{userID}]";
			}
			else
			{
				_name = base.ShortPrefabName;
			}
		}
		return _name;
	}

	public string GetDebugStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Entity: {0}\n", ((object)this).ToString());
		stringBuilder.AppendFormat("Name: {0}\n", displayName);
		stringBuilder.AppendFormat("SteamID: {0}\n", userID);
		foreach (PlayerFlags value in Enum.GetValues(typeof(PlayerFlags)))
		{
			stringBuilder.AppendFormat("{1}: {0}\n", HasPlayerFlag(value), value);
		}
		return stringBuilder.ToString();
	}

	public override Item GetItem(ItemId itemId)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)inventory == (Object)null)
		{
			return null;
		}
		return inventory.FindItemByUID(itemId);
	}

	public override float WaterFactor()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (GetMounted().IsValid())
		{
			return GetMounted().WaterFactorForPlayer(this);
		}
		if ((Object)(object)GetParentEntity() != (Object)null && GetParentEntity().BlocksWaterFor(this))
		{
			return 0f;
		}
		float radius = playerCollider.radius;
		float num = playerCollider.height * 0.5f;
		Vector3 start = ((Component)playerCollider).transform.position + ((Component)playerCollider).transform.rotation * (playerCollider.center - Vector3.up * (num - radius));
		Vector3 end = ((Component)playerCollider).transform.position + ((Component)playerCollider).transform.rotation * (playerCollider.center + Vector3.up * (num - radius));
		return WaterLevel.Factor(start, end, radius, waves: true, volumes: true, this);
	}

	public override float AirFactor()
	{
		float num = ((WaterFactor() > 0.85f) ? 0f : 1f);
		BaseMountable baseMountable = GetMounted();
		if (baseMountable.IsValid() && baseMountable.BlocksWaterFor(this))
		{
			float num2 = baseMountable.AirFactor();
			if (num2 < num)
			{
				num = num2;
			}
		}
		return num;
	}

	public float GetOxygenTime(out ItemModGiveOxygen.AirSupplyType airSupplyType)
	{
		BaseVehicle mountedVehicle = GetMountedVehicle();
		if (mountedVehicle.IsValid() && mountedVehicle is IAirSupply airSupply)
		{
			float airTimeRemaining = airSupply.GetAirTimeRemaining();
			if (airTimeRemaining > 0f)
			{
				airSupplyType = airSupply.AirType;
				return airTimeRemaining;
			}
		}
		foreach (Item item in inventory.containerWear.itemList)
		{
			IAirSupply componentInChildren = ((Component)item.info).GetComponentInChildren<IAirSupply>();
			if (componentInChildren != null)
			{
				float airTimeRemaining2 = componentInChildren.GetAirTimeRemaining();
				if (airTimeRemaining2 > 0f)
				{
					airSupplyType = componentInChildren.AirType;
					return airTimeRemaining2;
				}
			}
		}
		airSupplyType = ItemModGiveOxygen.AirSupplyType.Lungs;
		if (metabolism.oxygen.value > 0.5f)
		{
			float num = Mathf.InverseLerp(0.5f, 1f, metabolism.oxygen.value);
			return 5f * num;
		}
		return 0f;
	}

	public override bool ShouldInheritNetworkGroup()
	{
		return IsSpectating();
	}

	public static bool AnyPlayersVisibleToEntity(Vector3 pos, float radius, BaseEntity source, Vector3 entityEyePos, bool ignorePlayersWithPriv = false)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		List<BasePlayer> list2 = Pool.GetList<BasePlayer>();
		Vis.Entities(pos, radius, list2, 131072, (QueryTriggerInteraction)2);
		bool flag = false;
		foreach (BasePlayer item in list2)
		{
			if (item.IsSleeping() || !item.IsAlive() || (item.IsBuildingAuthed() && ignorePlayersWithPriv))
			{
				continue;
			}
			list.Clear();
			Vector3 position = item.eyes.position;
			Vector3 val = entityEyePos - item.eyes.position;
			GamePhysics.TraceAll(new Ray(position, ((Vector3)(ref val)).normalized), 0f, list, 9f, 1218519297, (QueryTriggerInteraction)0);
			for (int i = 0; i < list.Count; i++)
			{
				BaseEntity entity = list[i].GetEntity();
				if ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)source || entity.EqualNetID((BaseNetworkable)source)))
				{
					flag = true;
					break;
				}
				if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
				{
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		Pool.FreeList<BasePlayer>(ref list2);
		return flag;
	}

	public bool IsStandingOnEntity(BaseEntity standingOn, int layerMask)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOnGround())
		{
			return false;
		}
		RaycastHit hit = default(RaycastHit);
		if (Physics.SphereCast(((Component)this).transform.position + Vector3.up * (0.25f + GetRadius()), GetRadius() * 0.95f, Vector3.down, ref hit, 4f, layerMask))
		{
			BaseEntity entity = hit.GetEntity();
			if ((Object)(object)entity != (Object)null)
			{
				if (entity.EqualNetID((BaseNetworkable)standingOn))
				{
					return true;
				}
				BaseEntity baseEntity = entity.GetParentEntity();
				if ((Object)(object)baseEntity != (Object)null && baseEntity.EqualNetID((BaseNetworkable)standingOn))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SetActiveTelephone(PhoneController t)
	{
		activeTelephone = t;
	}

	public void ClearDesigningAIEntity()
	{
		if (IsDesigningAI)
		{
			((Component)designingAIEntity).GetComponent<IAIDesign>()?.StopDesigning();
		}
		designingAIEntity = null;
	}
}
