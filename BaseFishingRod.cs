using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseFishingRod : HeldEntity
{
	public enum CatchState
	{
		None,
		Aiming,
		Waiting,
		Catching,
		Caught
	}

	[Flags]
	public enum FishState
	{
		PullingLeft = 1,
		PullingRight = 2,
		PullingBack = 4
	}

	public enum FailReason
	{
		UserRequested,
		BadAngle,
		TensionBreak,
		Unequipped,
		TimeOut,
		Success,
		NoWaterFound,
		Obstructed,
		NoLure,
		TooShallow,
		TooClose,
		TooFarAway,
		PlayerMoved
	}

	public class UpdateFishingRod : ObjectWorkQueue<BaseFishingRod>
	{
		protected override void RunJob(BaseFishingRod entity)
		{
			if (((ObjectWorkQueue<BaseFishingRod>)this).ShouldAdd(entity))
			{
				entity.CatchProcessBudgeted();
			}
		}

		protected override bool ShouldAdd(BaseFishingRod entity)
		{
			if (base.ShouldAdd(entity))
			{
				return entity.IsValid();
			}
			return false;
		}
	}

	public GameObjectRef FishingBobberRef;

	public float FishCatchDistance = 0.5f;

	public LineRenderer ReelLineRenderer;

	public Transform LineRendererWorldStartPos;

	private FishState currentFishState;

	private EntityRef<FishingBobber> currentBobber;

	public float ConditionLossOnSuccess = 0.02f;

	public float ConditionLossOnFail = 0.04f;

	public float GlobalStrainSpeedMultiplier = 1f;

	public float MaxCastDistance = 10f;

	public const Flags Straining = Flags.Reserved1;

	public ItemModFishable ForceFish;

	public static Flags PullingLeftFlag = Flags.Reserved6;

	public static Flags PullingRightFlag = Flags.Reserved7;

	public static Flags ReelingInFlag = Flags.Reserved8;

	public GameObjectRef BobberPreview;

	public SoundDefinition onLineSoundDef;

	public SoundDefinition strainSoundDef;

	public AnimationCurve strainGainCurve;

	public SoundDefinition tensionBreakSoundDef;

	public static UpdateFishingRod updateFishingRodQueue = new UpdateFishingRod();

	private FishLookup fishLookup;

	private TimeUntil nextFishStateChange;

	private TimeSince fishCatchDuration;

	private float strainTimer;

	private const float strainMax = 6f;

	private TimeSince lastStrainUpdate;

	private TimeUntil catchTime;

	private TimeSince lastSightCheck;

	private Vector3 playerStartPosition;

	private WaterBody surfaceBody;

	private ItemDefinition lureUsed;

	private ItemDefinition currentFishTarget;

	private ItemModFishable fishableModifier;

	private ItemModFishable lastFish;

	private bool inQueue;

	[ServerVar]
	public static bool ForceSuccess = false;

	[ServerVar]
	public static bool ForceFail = false;

	[ServerVar]
	public static bool ImmediateHook = false;

	public CatchState CurrentState { get; private set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseFishingRod.OnRpcMessage", 0);
		try
		{
			if (rpc == 4237324865u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_Cancel "));
				}
				TimeWarning val2 = TimeWarning.New("Server_Cancel", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(4237324865u, "Server_Cancel", this, player))
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
							Server_Cancel(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_Cancel");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4238539495u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_RequestCast "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RequestCast", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(4238539495u, "Server_RequestCast", this, player))
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
							Server_RequestCast(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_RequestCast");
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

	public override void Load(LoadInfo info)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if ((!base.isServer || !info.fromDisk) && info.msg.simpleUID != null)
		{
			currentBobber.uid = info.msg.simpleUID.uid;
		}
	}

	public override bool BlocksGestures()
	{
		return CurrentState != CatchState.None;
	}

	private bool AllowPullInDirection(Vector3 worldDirection, Vector3 bobberPosition)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		Vector3 val = Vector3Ex.WithY(bobberPosition, position.y);
		Vector3 val2 = val - position;
		return Vector3.Dot(worldDirection, ((Vector3)(ref val2)).normalized) < 0f;
	}

	private bool EvaluateFishingPosition(ref Vector3 pos, BasePlayer ply, out FailReason reason, out WaterBody waterBody)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit hitInfo;
		bool num = GamePhysics.Trace(new Ray(pos + Vector3.up, Vector3.down), 0f, out hitInfo, 1.5f, 16, (QueryTriggerInteraction)0);
		if (num)
		{
			waterBody = hitInfo.GetWaterBody();
			pos.y = ((RaycastHit)(ref hitInfo)).point.y;
		}
		else
		{
			waterBody = null;
		}
		if (!num)
		{
			reason = FailReason.NoWaterFound;
			return false;
		}
		if (Vector3.Distance(Vector3Ex.WithY(((Component)ply).transform.position, pos.y), pos) < 5f)
		{
			reason = FailReason.TooClose;
			return false;
		}
		if (!GamePhysics.LineOfSight(ply.eyes.position, pos, 1218652417))
		{
			reason = FailReason.Obstructed;
			return false;
		}
		Vector3 p = pos + Vector3.up * 2f;
		if (!GamePhysics.LineOfSight(ply.eyes.position, p, 1218652417))
		{
			reason = FailReason.Obstructed;
			return false;
		}
		Vector3 position = ((Component)ply).transform.position;
		position.y = pos.y;
		float num2 = Vector3.Distance(pos, position);
		Vector3 val = pos;
		Vector3 val2 = position - pos;
		Vector3 p2 = val + ((Vector3)(ref val2)).normalized * (num2 - FishCatchDistance);
		if (!GamePhysics.LineOfSight(pos, p2, 1218652417))
		{
			reason = FailReason.Obstructed;
			return false;
		}
		if (WaterLevel.GetOverallWaterDepth(Vector3.Lerp(pos, Vector3Ex.WithY(((Component)ply).transform.position, pos.y), 0.95f), waves: true, volumes: false, null, noEarlyExit: true) < 0.1f && ply.eyes.position.y > 0f)
		{
			reason = FailReason.TooShallow;
			return false;
		}
		if (WaterLevel.GetOverallWaterDepth(pos, waves: true, volumes: false, null, noEarlyExit: true) < 0.3f && ply.eyes.position.y > 0f)
		{
			reason = FailReason.TooShallow;
			return false;
		}
		Vector3 p3 = Vector3.MoveTowards(Vector3Ex.WithY(((Component)ply).transform.position, pos.y), pos, 1f);
		if (!GamePhysics.LineOfSight(ply.eyes.position, p3, 1218652417))
		{
			reason = FailReason.Obstructed;
			return false;
		}
		reason = FailReason.Success;
		return true;
	}

	private Item GetCurrentLure()
	{
		if (GetItem() == null)
		{
			return null;
		}
		if (GetItem().contents == null)
		{
			return null;
		}
		return GetItem().contents.GetSlot(0);
	}

	private bool HasReelInInput(InputState state)
	{
		if (!state.IsDown(BUTTON.BACKWARD))
		{
			return state.IsDown(BUTTON.FIRE_PRIMARY);
		}
		return true;
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void Server_RequestCast(RPCMessage msg)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pos = msg.read.Vector3();
		BasePlayer ownerPlayer = GetOwnerPlayer();
		Item currentLure = GetCurrentLure();
		if (currentLure == null)
		{
			FailedCast(FailReason.NoLure);
			return;
		}
		if (!EvaluateFishingPosition(ref pos, ownerPlayer, out var reason, out surfaceBody))
		{
			FailedCast(reason);
			return;
		}
		FishingBobber component = ((Component)base.gameManager.CreateEntity(FishingBobberRef.resourcePath, ((Component)this).transform.position + Vector3.up * 2.8f + ownerPlayer.eyes.BodyForward() * 1.8f, GetOwnerPlayer().ServerRotation)).GetComponent<FishingBobber>();
		((Component)component).transform.forward = GetOwnerPlayer().eyes.BodyForward();
		component.Spawn();
		component.InitialiseBobber(ownerPlayer, surfaceBody, pos);
		lureUsed = currentLure.info;
		currentLure.UseItem();
		if (fishLookup == null)
		{
			fishLookup = PrefabAttribute.server.Find<FishLookup>(prefabID);
		}
		currentFishTarget = fishLookup.GetFish(((Component)component).transform.position, surfaceBody, lureUsed, out fishableModifier, lastFish);
		lastFish = fishableModifier;
		currentBobber.Set(component);
		ClientRPC<NetworkableId>(null, "Client_ReceiveCastPoint", component.net.ID);
		ownerPlayer.SignalBroadcast(Signal.Attack);
		catchTime = TimeUntil.op_Implicit(ImmediateHook ? 0f : Random.Range(10f, 20f));
		catchTime = TimeUntil.op_Implicit(TimeUntil.op_Implicit(catchTime) * fishableModifier.CatchWaitTimeMultiplier);
		ItemModCompostable itemModCompostable = default(ItemModCompostable);
		float num = (((Component)lureUsed).TryGetComponent<ItemModCompostable>(ref itemModCompostable) ? itemModCompostable.BaitValue : 0f);
		num = Mathx.RemapValClamped(num, 0f, 20f, 1f, 10f);
		catchTime = TimeUntil.op_Implicit(Mathf.Clamp(TimeUntil.op_Implicit(catchTime) - num, 3f, 20f));
		playerStartPosition = ((Component)ownerPlayer).transform.position;
		SetFlag(Flags.Busy, b: true);
		CurrentState = CatchState.Waiting;
		((FacepunchBehaviour)this).InvokeRepeating((Action)CatchProcess, 0f, 0f);
		inQueue = false;
	}

	private void FailedCast(FailReason reason)
	{
		CurrentState = CatchState.None;
		ClientRPC(null, "Client_ResetLine", (int)reason);
	}

	private void CatchProcess()
	{
		if (!inQueue)
		{
			inQueue = true;
			((ObjectWorkQueue<BaseFishingRod>)updateFishingRodQueue).Add(this);
		}
	}

	private void CatchProcessBudgeted()
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		inQueue = false;
		FishingBobber fishingBobber = currentBobber.Get(serverside: true);
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)ownerPlayer == (Object)null || ownerPlayer.IsSleeping() || ownerPlayer.IsWounded() || ownerPlayer.IsDead() || (Object)(object)fishingBobber == (Object)null)
		{
			Server_Cancel(FailReason.UserRequested);
			return;
		}
		Vector3 position = ((Component)ownerPlayer).transform.position;
		Vector3 val = Vector3Ex.WithY(((Component)fishingBobber).transform.position, 0f) - Vector3Ex.WithY(position, 0f);
		float num = Vector3.Angle(((Vector3)(ref val)).normalized, Vector3Ex.WithY(ownerPlayer.eyes.HeadForward(), 0f));
		float num2 = Vector3.Distance(position, Vector3Ex.WithY(((Component)fishingBobber).transform.position, position.y));
		if (num > ((num2 > 1.2f) ? 60f : 180f))
		{
			Server_Cancel(FailReason.BadAngle);
			return;
		}
		if (num2 > 1.2f && TimeSince.op_Implicit(lastSightCheck) > 0.4f)
		{
			if (!GamePhysics.LineOfSight(ownerPlayer.eyes.position, ((Component)fishingBobber).transform.position, 1084293377))
			{
				Server_Cancel(FailReason.Obstructed);
				return;
			}
			lastSightCheck = TimeSince.op_Implicit(0f);
		}
		if (Vector3.Distance(position, ((Component)fishingBobber).transform.position) > MaxCastDistance * 2f)
		{
			Server_Cancel(FailReason.TooFarAway);
			return;
		}
		if (Vector3.Distance(playerStartPosition, position) > 1f)
		{
			Server_Cancel(FailReason.PlayerMoved);
			return;
		}
		if (CurrentState == CatchState.Waiting)
		{
			if (TimeUntil.op_Implicit(catchTime) < 0f)
			{
				ClientRPC(null, "Client_HookedSomething");
				CurrentState = CatchState.Catching;
				fishingBobber.SetFlag(Flags.Reserved1, b: true);
				nextFishStateChange = TimeUntil.op_Implicit(0f);
				fishCatchDuration = TimeSince.op_Implicit(0f);
				strainTimer = 0f;
			}
			return;
		}
		FishState fishState = currentFishState;
		if (TimeUntil.op_Implicit(nextFishStateChange) < 0f)
		{
			float num3 = Mathx.RemapValClamped(fishingBobber.TireAmount, 0f, 20f, 0f, 1f);
			if (currentFishState != 0)
			{
				currentFishState = (FishState)0;
				nextFishStateChange = TimeUntil.op_Implicit(Random.Range(2f, 4f) * (num3 + 1f));
			}
			else
			{
				nextFishStateChange = TimeUntil.op_Implicit(Random.Range(3f, 7f) * (1f - num3));
				if (Random.Range(0, 100) < 50)
				{
					currentFishState = FishState.PullingLeft;
				}
				else
				{
					currentFishState = FishState.PullingRight;
				}
				if (Random.Range(0, 100) > 60 && Vector3.Distance(((Component)fishingBobber).transform.position, ((Component)ownerPlayer).transform.position) < MaxCastDistance - 2f)
				{
					currentFishState |= FishState.PullingBack;
				}
			}
		}
		if (TimeSince.op_Implicit(fishCatchDuration) > 120f)
		{
			Server_Cancel(FailReason.TimeOut);
			return;
		}
		bool flag = ownerPlayer.serverInput.IsDown(BUTTON.RIGHT);
		bool flag2 = ownerPlayer.serverInput.IsDown(BUTTON.LEFT);
		bool flag3 = HasReelInInput(ownerPlayer.serverInput);
		if (flag2 && flag)
		{
			flag2 = (flag = false);
		}
		UpdateFlags(flag2, flag, flag3);
		if (CurrentState == CatchState.Waiting)
		{
			flag = (flag2 = (flag3 = false));
		}
		if (flag2 && !AllowPullInDirection(-ownerPlayer.eyes.HeadRight(), ((Component)fishingBobber).transform.position))
		{
			flag2 = false;
		}
		if (flag && !AllowPullInDirection(ownerPlayer.eyes.HeadRight(), ((Component)fishingBobber).transform.position))
		{
			flag = false;
		}
		fishingBobber.ServerMovementUpdate(flag2, flag, flag3, ref currentFishState, position, fishableModifier);
		bool flag4 = false;
		float num4 = 0f;
		if (flag3 || flag2 || flag)
		{
			flag4 = true;
			num4 = 0.5f;
		}
		if (currentFishState != 0 && flag4)
		{
			if (currentFishState.Contains(FishState.PullingBack) && flag3)
			{
				num4 = 1.5f;
			}
			else if ((currentFishState.Contains(FishState.PullingLeft) || currentFishState.Contains(FishState.PullingRight)) && flag3)
			{
				num4 = 1.2f;
			}
			else if (currentFishState.Contains(FishState.PullingLeft) && flag)
			{
				num4 = 0.8f;
			}
			else if (currentFishState.Contains(FishState.PullingRight) && flag2)
			{
				num4 = 0.8f;
			}
		}
		if (flag3 && currentFishState != 0)
		{
			num4 += 1f;
		}
		num4 *= fishableModifier.StrainModifier * GlobalStrainSpeedMultiplier;
		if (flag4)
		{
			strainTimer += Time.deltaTime * num4;
		}
		else
		{
			strainTimer = Mathf.MoveTowards(strainTimer, 0f, Time.deltaTime * 1.5f);
		}
		float num5 = strainTimer / 6f;
		SetFlag(Flags.Reserved1, flag4 && num5 > 0.25f);
		if (TimeSince.op_Implicit(lastStrainUpdate) > 0.4f || fishState != currentFishState)
		{
			ClientRPC(null, "Client_UpdateFishState", (int)currentFishState, num5);
			lastStrainUpdate = TimeSince.op_Implicit(0f);
		}
		if (strainTimer > 7f || ForceFail)
		{
			Server_Cancel(FailReason.TensionBreak);
		}
		else
		{
			if (!(num2 <= FishCatchDistance) && !ForceSuccess)
			{
				return;
			}
			CurrentState = CatchState.Caught;
			if ((Object)(object)currentFishTarget != (Object)null)
			{
				Item item = ItemManager.Create(currentFishTarget, 1, 0uL);
				ownerPlayer.GiveItem(item, GiveItemReason.Crafted);
				if (currentFishTarget.shortname == "skull.human")
				{
					item.name = RandomUsernames.Get(Random.Range(0, 1000));
				}
				if (GameInfo.HasAchievements && !string.IsNullOrEmpty(fishableModifier.SteamStatName))
				{
					ownerPlayer.stats.Add(fishableModifier.SteamStatName, 1);
					ownerPlayer.stats.Save(forceSteamSave: true);
					fishLookup.CheckCatchAllAchievement(ownerPlayer);
				}
			}
			Analytics.Server.FishCaught(currentFishTarget);
			ClientRPC(null, "Client_OnCaughtFish", currentFishTarget.itemid);
			ownerPlayer.SignalBroadcast(Signal.Alt_Attack);
			((FacepunchBehaviour)this).Invoke((Action)ResetLine, 6f);
			fishingBobber.Kill();
			currentBobber.Set(null);
			((FacepunchBehaviour)this).CancelInvoke((Action)CatchProcess);
		}
	}

	private void ResetLine()
	{
		Server_Cancel(FailReason.Success);
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void Server_Cancel(RPCMessage msg)
	{
		if (CurrentState != CatchState.Caught)
		{
			Server_Cancel(FailReason.UserRequested);
		}
	}

	private void Server_Cancel(FailReason reason)
	{
		if (GetItem() != null)
		{
			GetItem().LoseCondition((reason == FailReason.Success) ? ConditionLossOnSuccess : ConditionLossOnFail);
		}
		SetFlag(Flags.Busy, b: false);
		UpdateFlags();
		((FacepunchBehaviour)this).CancelInvoke((Action)CatchProcess);
		CurrentState = CatchState.None;
		SetFlag(Flags.Reserved1, b: false);
		FishingBobber fishingBobber = currentBobber.Get(serverside: true);
		if ((Object)(object)fishingBobber != (Object)null)
		{
			fishingBobber.Kill();
			currentBobber.Set(null);
		}
		ClientRPC(null, "Client_ResetLine", (int)reason);
	}

	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		if (CurrentState != 0)
		{
			Server_Cancel(FailReason.Unequipped);
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (currentBobber.IsSet && info.msg.simpleUID == null)
		{
			info.msg.simpleUID = Pool.Get<SimpleUID>();
			info.msg.simpleUID.uid = currentBobber.uid;
		}
	}

	private void UpdateFlags(bool inputLeft = false, bool inputRight = false, bool back = false)
	{
		SetFlag(PullingLeftFlag, CurrentState == CatchState.Catching && inputLeft);
		SetFlag(PullingRightFlag, CurrentState == CatchState.Catching && inputRight);
		SetFlag(ReelingInFlag, CurrentState == CatchState.Catching && back);
	}
}
