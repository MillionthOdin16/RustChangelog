using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

public class AutoTurret : ContainerIOEntity, IRemoteControllable
{
	public static class TurretFlags
	{
		public const Flags Peacekeeper = Flags.Reserved1;
	}

	public class UpdateAutoTurretScanQueue : ObjectWorkQueue<AutoTurret>
	{
		protected override void RunJob(AutoTurret entity)
		{
			if (((ObjectWorkQueue<AutoTurret>)this).ShouldAdd(entity))
			{
				entity.TargetScan();
			}
		}

		protected override bool ShouldAdd(AutoTurret entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}

	public GameObjectRef gun_fire_effect;

	public GameObjectRef bulletEffect;

	public float bulletSpeed = 200f;

	public AmbienceEmitter ambienceEmitter;

	public GameObject assignDialog;

	public LaserBeam laserBeam;

	public static UpdateAutoTurretScanQueue updateAutoTurretScanQueue = new UpdateAutoTurretScanQueue();

	[Header("RC")]
	public float rcTurnSensitivity = 4f;

	public Transform RCEyes;

	public GameObjectRef IDPanelPrefab;

	public RemoteControllableControls rcControls = RemoteControllableControls.None;

	public string rcIdentifier = "";

	public TargetTrigger targetTrigger;

	public Transform socketTransform;

	private float nextShotTime = 0f;

	private float lastShotTime = 0f;

	private float nextVisCheck = 0f;

	private float lastTargetSeenTime = 0f;

	private bool targetVisible = true;

	private bool booting = false;

	private float nextIdleAimTime = 0f;

	private Vector3 targetAimDir = Vector3.forward;

	private const float bulletDamage = 15f;

	private RealTimeSinceEx timeSinceLastServerTick;

	private float nextForcedAimTime = 0f;

	private Vector3 lastSentAimDir = Vector3.zero;

	private static float[] visibilityOffsets = new float[3] { 0f, 0.15f, -0.15f };

	private int peekIndex = 0;

	[NonSerialized]
	private int numConsecutiveMisses = 0;

	[NonSerialized]
	private int totalAmmo = 0;

	private float nextAmmoCheckTime = 0f;

	private bool totalAmmoDirty = true;

	private float currentAmmoGravity = 0f;

	private float currentAmmoVelocity = 0f;

	private HeldEntity AttachedWeapon = null;

	public float attachedWeaponZOffsetScale = -0.5f;

	public BaseCombatEntity target;

	public Transform eyePos;

	public Transform muzzlePos;

	public Vector3 aimDir;

	public Transform gun_yaw;

	public Transform gun_pitch;

	public float sightRange = 30f;

	public SoundDefinition turnLoopDef;

	public SoundDefinition movementChangeDef;

	public SoundDefinition ambientLoopDef;

	public SoundDefinition focusCameraDef;

	public float focusSoundFreqMin = 2.5f;

	public float focusSoundFreqMax = 7f;

	public GameObjectRef peacekeeperToggleSound;

	public GameObjectRef onlineSound;

	public GameObjectRef offlineSound;

	public GameObjectRef targetAcquiredEffect;

	public GameObjectRef targetLostEffect;

	public GameObjectRef reloadEffect;

	public float aimCone;

	public const Flags Flag_Equipped = Flags.Reserved3;

	public const Flags Flag_MaxAuths = Flags.Reserved4;

	[NonSerialized]
	public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	public bool CanPing => false;

	public virtual bool RequiresMouse => true;

	public float MaxRange => 10000f;

	public RemoteControllableControls RequiredControls => rcControls;

	public int ViewerCount { get; private set; }

	public CameraViewerId? ControllingViewerId { get; private set; }

	public bool IsBeingControlled => ViewerCount > 0 && ControllingViewerId.HasValue;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("AutoTurret.OnRpcMessage", 0);
		try
		{
			if (rpc == 1092560690 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AddSelfAuthorize "));
				}
				TimeWarning val2 = TimeWarning.New("AddSelfAuthorize", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1092560690u, "AddSelfAuthorize", this, player, 3f))
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
						TimeWarning val4 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc2 = rPCMessage;
							AddSelfAuthorize(rpc2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AddSelfAuthorize");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3057055788u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AssignToFriend "));
				}
				TimeWarning val5 = TimeWarning.New("AssignToFriend", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3057055788u, "AssignToFriend", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
					try
					{
						TimeWarning val7 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							AssignToFriend(msg2);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in AssignToFriend");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 253307592 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ClearList "));
				}
				TimeWarning val8 = TimeWarning.New("ClearList", 0);
				try
				{
					TimeWarning val9 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(253307592u, "ClearList", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val9)?.Dispose();
					}
					try
					{
						TimeWarning val10 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc3 = rPCMessage;
							ClearList(rpc3);
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in ClearList");
					}
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
				}
				return true;
			}
			if (rpc == 1500257773 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - FlipAim "));
				}
				TimeWarning val11 = TimeWarning.New("FlipAim", 0);
				try
				{
					TimeWarning val12 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1500257773u, "FlipAim", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val12)?.Dispose();
					}
					try
					{
						TimeWarning val13 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc4 = rPCMessage;
							FlipAim(rpc4);
						}
						finally
						{
							((IDisposable)val13)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in FlipAim");
					}
				}
				finally
				{
					((IDisposable)val11)?.Dispose();
				}
				return true;
			}
			if (rpc == 3617985969u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RemoveSelfAuthorize "));
				}
				TimeWarning val14 = TimeWarning.New("RemoveSelfAuthorize", 0);
				try
				{
					TimeWarning val15 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3617985969u, "RemoveSelfAuthorize", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val15)?.Dispose();
					}
					try
					{
						TimeWarning val16 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc5 = rPCMessage;
							RemoveSelfAuthorize(rpc5);
						}
						finally
						{
							((IDisposable)val16)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in RemoveSelfAuthorize");
					}
				}
				finally
				{
					((IDisposable)val14)?.Dispose();
				}
				return true;
			}
			if (rpc == 1770263114 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SERVER_AttackAll "));
				}
				TimeWarning val17 = TimeWarning.New("SERVER_AttackAll", 0);
				try
				{
					TimeWarning val18 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1770263114u, "SERVER_AttackAll", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val18)?.Dispose();
					}
					try
					{
						TimeWarning val19 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc6 = rPCMessage;
							SERVER_AttackAll(rpc6);
						}
						finally
						{
							((IDisposable)val19)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in SERVER_AttackAll");
					}
				}
				finally
				{
					((IDisposable)val17)?.Dispose();
				}
				return true;
			}
			if (rpc == 3265538831u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SERVER_Peacekeeper "));
				}
				TimeWarning val20 = TimeWarning.New("SERVER_Peacekeeper", 0);
				try
				{
					TimeWarning val21 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3265538831u, "SERVER_Peacekeeper", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val21)?.Dispose();
					}
					try
					{
						TimeWarning val22 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc7 = rPCMessage;
							SERVER_Peacekeeper(rpc7);
						}
						finally
						{
							((IDisposable)val22)?.Dispose();
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in SERVER_Peacekeeper");
					}
				}
				finally
				{
					((IDisposable)val20)?.Dispose();
				}
				return true;
			}
			if (rpc == 1053317251 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_SetID "));
				}
				TimeWarning val23 = TimeWarning.New("Server_SetID", 0);
				try
				{
					TimeWarning val24 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1053317251u, "Server_SetID", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val24)?.Dispose();
					}
					try
					{
						TimeWarning val25 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							Server_SetID(msg3);
						}
						finally
						{
							((IDisposable)val25)?.Dispose();
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in Server_SetID");
					}
				}
				finally
				{
					((IDisposable)val23)?.Dispose();
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

	public bool PeacekeeperMode()
	{
		return HasFlag(Flags.Reserved1);
	}

	public Transform GetEyes()
	{
		return RCEyes;
	}

	public float GetFovScale()
	{
		return 1f;
	}

	public BaseEntity GetEnt()
	{
		return this;
	}

	public virtual bool CanControl(ulong playerID)
	{
		if (booting)
		{
			return false;
		}
		return IsPowered() && !PeacekeeperMode();
	}

	public bool InitializeControl(CameraViewerId viewerID)
	{
		ViewerCount++;
		if (!ControllingViewerId.HasValue)
		{
			ControllingViewerId = viewerID;
			SetTarget(null);
			SendAimDirImmediate();
			return true;
		}
		return false;
	}

	public void StopControl(CameraViewerId viewerID)
	{
		ViewerCount--;
		if (ControllingViewerId == viewerID)
		{
			ControllingViewerId = null;
		}
	}

	public void UserInput(InputState inputState, CameraViewerId viewerID)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		CameraViewerId? controllingViewerId = ControllingViewerId;
		if (viewerID != controllingViewerId)
		{
			return;
		}
		UpdateManualAim(inputState);
		if (Time.time < nextShotTime)
		{
			return;
		}
		if (inputState.WasJustPressed(BUTTON.RELOAD))
		{
			Reload();
		}
		else
		{
			if (EnsureReloaded() || !inputState.IsDown(BUTTON.FIRE_PRIMARY))
			{
				return;
			}
			BaseProjectile attachedWeapon = GetAttachedWeapon();
			if (Object.op_Implicit((Object)(object)attachedWeapon))
			{
				if (attachedWeapon.primaryMagazine.contents > 0)
				{
					FireAttachedGun(Vector3.zero, aimCone);
					float delay = (attachedWeapon.isSemiAuto ? (attachedWeapon.repeatDelay * 1.5f) : attachedWeapon.repeatDelay);
					delay = attachedWeapon.ScaleRepeatDelay(delay);
					nextShotTime = Time.time + delay;
				}
				else
				{
					nextShotTime = Time.time + 5f;
				}
			}
			else if (HasGenericFireable())
			{
				AttachedWeapon.ServerUse();
				nextShotTime = Time.time + 0.115f;
			}
			else
			{
				nextShotTime = Time.time + 1f;
			}
		}
	}

	private bool UpdateManualAim(InputState inputState)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		float num = (0f - inputState.current.mouseDelta.y) * rcTurnSensitivity;
		float num2 = inputState.current.mouseDelta.x * rcTurnSensitivity;
		Quaternion val = Quaternion.LookRotation(aimDir, ((Component)this).transform.up);
		Vector3 eulerAngles = ((Quaternion)(ref val)).eulerAngles;
		Vector3 val2 = eulerAngles + new Vector3(num, num2, 0f);
		if (val2.x >= 0f && val2.x <= 135f)
		{
			val2.x = Mathf.Clamp(val2.x, 0f, 45f);
		}
		if (val2.x >= 225f && val2.x <= 360f)
		{
			val2.x = Mathf.Clamp(val2.x, 285f, 360f);
		}
		Quaternion val3 = Quaternion.Euler(val2);
		Vector3 val4 = val3 * Vector3.forward;
		bool result = !Mathf.Approximately(aimDir.x, val4.x) || !Mathf.Approximately(aimDir.y, val4.y) || !Mathf.Approximately(aimDir.z, val4.z);
		aimDir = val4;
		return result;
	}

	public override void InitShared()
	{
		base.InitShared();
		RCSetup();
	}

	public override void DestroyShared()
	{
		RCShutdown();
		base.DestroyShared();
	}

	public void RCSetup()
	{
		if (base.isServer)
		{
			RemoteControlEntity.InstallControllable(this);
		}
	}

	public void RCShutdown()
	{
		if (base.isServer)
		{
			RemoteControlEntity.RemoveControllable(this);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void Server_SetID(RPCMessage msg)
	{
		if ((Object)(object)msg.player == (Object)null || !CanChangeID(msg.player))
		{
			return;
		}
		string text = msg.read.String(256);
		if (string.IsNullOrEmpty(text) || ComputerStation.IsValidIdentifier(text))
		{
			string text2 = msg.read.String(256);
			if (ComputerStation.IsValidIdentifier(text2) && text == GetIdentifier())
			{
				Debug.Log((object)"SetID success!");
				UpdateIdentifier(text2);
			}
		}
	}

	public void UpdateIdentifier(string newID, bool clientSend = false)
	{
		string text = rcIdentifier;
		if (base.isServer)
		{
			if (!RemoteControlEntity.IDInUse(newID))
			{
				rcIdentifier = newID;
			}
			SendNetworkUpdate();
		}
	}

	public string GetIdentifier()
	{
		return rcIdentifier;
	}

	protected virtual bool CanChangeID(BasePlayer player)
	{
		return CanChangeSettings(player);
	}

	public override int ConsumptionAmount()
	{
		return 10;
	}

	public void SetOnline()
	{
		SetIsOnline(online: true);
	}

	public void SetIsOnline(bool online)
	{
		if (online != HasFlag(Flags.On))
		{
			SetFlag(Flags.On, online);
			booting = false;
			GetAttachedWeapon()?.SetLightsOn(online);
			SendNetworkUpdate();
			if (IsOffline())
			{
				SetTarget(null);
				isLootable = true;
			}
			else
			{
				isLootable = false;
			}
		}
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int num = Mathf.Min(1, GetCurrentEnergy());
		return outputSlot switch
		{
			0 => HasTarget() ? num : 0, 
			1 => (totalAmmo <= 50) ? num : 0, 
			2 => (totalAmmo == 0) ? num : 0, 
			_ => 0, 
		};
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (IsPowered() && !IsOn())
		{
			InitiateStartup();
		}
		else if ((!IsPowered() && IsOn()) || booting)
		{
			InitiateShutdown();
		}
	}

	public void InitiateShutdown()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOffline() || booting)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)SetOnline);
			booting = false;
			Effect.server.Run(offlineSound.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
			SetIsOnline(online: false);
		}
	}

	public void InitiateStartup()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOnline() && !booting)
		{
			Effect.server.Run(onlineSound.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
			((FacepunchBehaviour)this).Invoke((Action)SetOnline, 2f);
			booting = true;
		}
	}

	public void SetPeacekeepermode(bool isOn)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		bool flag = PeacekeeperMode();
		if (flag != isOn)
		{
			SetFlag(Flags.Reserved1, isOn);
			Effect.server.Run(peacekeeperToggleSound.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
		}
	}

	public bool IsValidWeapon(Item item)
	{
		ItemDefinition info = item.info;
		if (item.isBroken)
		{
			return false;
		}
		ItemModEntity component = ((Component)info).GetComponent<ItemModEntity>();
		if ((Object)(object)component == (Object)null)
		{
			return false;
		}
		HeldEntity component2 = component.entityPrefab.Get().GetComponent<HeldEntity>();
		if ((Object)(object)component2 == (Object)null)
		{
			return false;
		}
		if (!component2.IsUsableByTurret)
		{
			return false;
		}
		return true;
	}

	public bool CanAcceptItem(Item item, int targetSlot)
	{
		Item slot = base.inventory.GetSlot(0);
		if (IsValidWeapon(item) && targetSlot == 0)
		{
			return true;
		}
		if (item.info.category == ItemCategory.Ammunition)
		{
			if (slot == null || !Object.op_Implicit((Object)(object)GetAttachedWeapon()))
			{
				return false;
			}
			if (targetSlot == 0)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public bool AtMaxAuthCapacity()
	{
		return HasFlag(Flags.Reserved4);
	}

	public void UpdateMaxAuthCapacity()
	{
		if (authorizedPlayers.Count >= 200)
		{
			SetFlag(Flags.Reserved4, b: true);
			return;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		bool b = (Object)(object)activeGameMode != (Object)null && activeGameMode.limitTeamAuths && authorizedPlayers.Count >= activeGameMode.GetMaxRelationshipTeamSize();
		SetFlag(Flags.Reserved4, b);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void FlipAim(RPCMessage rpc)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOnline() && IsAuthed(rpc.player) && !booting)
		{
			((Component)this).transform.rotation = Quaternion.LookRotation(-((Component)this).transform.forward, ((Component)this).transform.up);
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void AddSelfAuthorize(RPCMessage rpc)
	{
		AddSelfAuthorize(rpc.player);
	}

	private void AddSelfAuthorize(BasePlayer player)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		if (!IsOnline() && player.CanBuild() && !AtMaxAuthCapacity())
		{
			authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == player.userID);
			PlayerNameID val = new PlayerNameID();
			val.userid = player.userID;
			val.username = player.displayName;
			authorizedPlayers.Add(val);
			Analytics.Azure.OnEntityAuthChanged(this, player, authorizedPlayers.Select((PlayerNameID x) => x.userid), "added", player.userID);
			UpdateMaxAuthCapacity();
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RemoveSelfAuthorize(RPCMessage rpc)
	{
		if (!booting && !IsOnline() && IsAuthed(rpc.player))
		{
			authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
			Analytics.Azure.OnEntityAuthChanged(this, rpc.player, authorizedPlayers.Select((PlayerNameID x) => x.userid), "removed", rpc.player.userID);
			UpdateMaxAuthCapacity();
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void ClearList(RPCMessage rpc)
	{
		if (!booting && !IsOnline() && IsAuthed(rpc.player))
		{
			authorizedPlayers.Clear();
			Analytics.Azure.OnEntityAuthChanged(this, rpc.player, authorizedPlayers.Select((PlayerNameID x) => x.userid), "clear", rpc.player.userID);
			UpdateMaxAuthCapacity();
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void AssignToFriend(RPCMessage msg)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		if (AtMaxAuthCapacity() || (Object)(object)msg.player == (Object)null || !msg.player.CanInteract() || !CanChangeSettings(msg.player))
		{
			return;
		}
		ulong num = msg.read.UInt64();
		if (num != 0L && !IsAuthed(num))
		{
			string username = BasePlayer.SanitizePlayerNameString(msg.read.String(256), num);
			PlayerNameID val = new PlayerNameID();
			val.userid = num;
			val.username = username;
			Analytics.Azure.OnEntityAuthChanged(this, msg.player, authorizedPlayers.Select((PlayerNameID x) => x.userid), "added", num);
			authorizedPlayers.Add(val);
			UpdateMaxAuthCapacity();
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void SERVER_Peacekeeper(RPCMessage rpc)
	{
		if (IsAuthed(rpc.player))
		{
			SetPeacekeepermode(isOn: true);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void SERVER_AttackAll(RPCMessage rpc)
	{
		if (IsAuthed(rpc.player))
		{
			SetPeacekeepermode(isOn: false);
		}
	}

	public virtual float TargetScanRate()
	{
		return 1f;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer itemContainer = base.inventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(CanAcceptItem));
		timeSinceLastServerTick = 0.0;
		((FacepunchBehaviour)this).InvokeRepeating((Action)ServerTick, Random.Range(0f, 1f), 0.015f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)SendAimDir, Random.Range(0f, 1f), 0.2f, 0.05f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)ScheduleForTargetScan, Random.Range(0f, 1f), TargetScanRate(), 0.2f);
		((Component)targetTrigger).GetComponent<SphereCollider>().radius = sightRange;
	}

	public void SendAimDir()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (Time.realtimeSinceStartup > nextForcedAimTime || HasTarget() || Vector3.Angle(lastSentAimDir, aimDir) > 0.03f)
		{
			SendAimDirImmediate();
		}
	}

	public void SendAimDirImmediate()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		lastSentAimDir = aimDir;
		ClientRPC<Vector3>(null, "CLIENT_ReceiveAimDir", aimDir);
		nextForcedAimTime = Time.realtimeSinceStartup + 2f;
	}

	public void SetTarget(BaseCombatEntity targ)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)targ != (Object)(object)target)
		{
			Effect.server.Run(((Object)(object)targ == (Object)null) ? targetLostEffect.resourcePath : targetAcquiredEffect.resourcePath, ((Component)this).transform.position, Vector3.up);
			MarkDirtyForceUpdateOutputs();
			nextShotTime += 0.1f;
		}
		target = targ;
	}

	public virtual bool CheckPeekers()
	{
		return true;
	}

	public bool ObjectVisible(BaseCombatEntity obj)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		Vector3 position = ((Component)eyePos).transform.position;
		if (GamePhysics.CheckSphere(position, 0.1f, 2097152, (QueryTriggerInteraction)0))
		{
			return false;
		}
		Vector3 val = AimOffset(obj);
		float num = Vector3.Distance(val, position);
		Vector3 val2 = val - position;
		Vector3 normalized = ((Vector3)(ref val2)).normalized;
		Vector3 val3 = Vector3.Cross(normalized, Vector3.up);
		for (int i = 0; (float)i < (CheckPeekers() ? 3f : 1f); i++)
		{
			Vector3 val4 = val + val3 * visibilityOffsets[i];
			val2 = val4 - position;
			Vector3 normalized2 = ((Vector3)(ref val2)).normalized;
			list.Clear();
			GamePhysics.TraceAll(new Ray(position, normalized2), 0f, list, num * 1.1f, 1218652417, (QueryTriggerInteraction)0);
			for (int j = 0; j < list.Count; j++)
			{
				RaycastHit hit = list[j];
				BaseEntity entity = hit.GetEntity();
				if ((!((Object)(object)entity != (Object)null) || !entity.isClient) && (!((Object)(object)entity != (Object)null) || !((Object)(object)entity.ToPlayer() != (Object)null) || entity.EqualNetID((BaseNetworkable)obj)) && (!((Object)(object)entity != (Object)null) || !entity.EqualNetID((BaseNetworkable)this)))
				{
					if ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)obj || entity.EqualNetID((BaseNetworkable)obj)))
					{
						Pool.FreeList<RaycastHit>(ref list);
						peekIndex = i;
						return true;
					}
					if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return false;
	}

	public virtual void FireAttachedGun(Vector3 targetPos, float aimCone, Transform muzzleToUse = null, BaseCombatEntity target = null)
	{
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if (!((Object)(object)attachedWeapon == (Object)null) && !IsOffline())
		{
			attachedWeapon.ServerUse(1f, IsBeingControlled ? RCEyes : gun_pitch);
		}
	}

	public virtual void FireGun(Vector3 targetPos, float aimCone, Transform muzzleToUse = null, BaseCombatEntity target = null)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		if (IsOffline())
		{
			return;
		}
		if ((Object)(object)muzzleToUse == (Object)null)
		{
			muzzleToUse = muzzlePos;
		}
		Vector3 val = ((Component)GetCenterMuzzle()).transform.position - GetCenterMuzzle().forward * 0.25f;
		Vector3 val2 = ((Component)GetCenterMuzzle()).transform.forward;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, val2);
		targetPos = val + modifiedAimConeDirection * 300f;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(val, modifiedAimConeDirection), 0f, list, 300f, 1220225809, (QueryTriggerInteraction)0);
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit hit = list[i];
			BaseEntity entity = hit.GetEntity();
			if (((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)this || entity.EqualNetID((BaseNetworkable)this))) || (PeacekeeperMode() && (Object)(object)target != (Object)null && (Object)(object)entity != (Object)null && (Object)(object)((Component)entity).GetComponent<BasePlayer>() != (Object)null && !entity.EqualNetID((BaseNetworkable)target)))
			{
				continue;
			}
			BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
			if ((Object)(object)baseCombatEntity != (Object)null)
			{
				ApplyDamage(baseCombatEntity, ((RaycastHit)(ref hit)).point, modifiedAimConeDirection);
				if (baseCombatEntity.EqualNetID((BaseNetworkable)target))
				{
					flag = true;
				}
			}
			if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
			{
				targetPos = ((RaycastHit)(ref hit)).point;
				Vector3 val3 = targetPos - val;
				val2 = ((Vector3)(ref val3)).normalized;
				break;
			}
		}
		int num = 2;
		if (!flag)
		{
			numConsecutiveMisses++;
		}
		else
		{
			numConsecutiveMisses = 0;
		}
		if ((Object)(object)target != (Object)null && targetVisible && numConsecutiveMisses > num)
		{
			ApplyDamage(target, ((Component)target).transform.position - val2 * 0.25f, val2);
			numConsecutiveMisses = 0;
		}
		ClientRPC<uint, Vector3>(null, "CLIENT_FireGun", StringPool.Get(((Object)((Component)muzzleToUse).gameObject).name), targetPos);
		Pool.FreeList<RaycastHit>(ref list);
	}

	private void ApplyDamage(BaseCombatEntity entity, Vector3 point, Vector3 normal)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		float num = 15f * Random.Range(0.9f, 1.1f);
		if (entity is BasePlayer && (Object)(object)entity != (Object)(object)target)
		{
			num *= 0.5f;
		}
		if (PeacekeeperMode() && (Object)(object)entity == (Object)(object)target)
		{
			target.MarkHostileFor(300f);
		}
		HitInfo info = new HitInfo(this, entity, DamageType.Bullet, num, point);
		entity.OnAttacked(info);
		if (entity is BasePlayer || entity is BaseNpc)
		{
			HitInfo hitInfo = new HitInfo();
			hitInfo.HitPositionWorld = point;
			hitInfo.HitNormalWorld = -normal;
			hitInfo.HitMaterial = StringPool.Get("Flesh");
			Effect.server.ImpactEffect(hitInfo);
		}
	}

	public void IdleTick(float dt)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (Time.realtimeSinceStartup > nextIdleAimTime)
		{
			nextIdleAimTime = Time.realtimeSinceStartup + Random.Range(4f, 5f);
			Quaternion val = Quaternion.LookRotation(((Component)this).transform.forward, Vector3.up);
			val *= Quaternion.AngleAxis(Random.Range(-45f, 45f), Vector3.up);
			targetAimDir = val * Vector3.forward;
		}
		if (!HasTarget())
		{
			aimDir = Mathx.Lerp(aimDir, targetAimDir, 2f, dt);
		}
	}

	public virtual bool HasClipAmmo()
	{
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon == (Object)null)
		{
			return false;
		}
		return attachedWeapon.primaryMagazine.contents > 0;
	}

	public virtual bool HasReserveAmmo()
	{
		return totalAmmo > 0;
	}

	public int GetTotalAmmo()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Autoturret.GetTotalAmmo");
		int num = 0;
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon == (Object)null)
		{
			Profiler.EndSample();
			return num;
		}
		List<Item> list = Pool.GetList<Item>();
		base.inventory.FindAmmo(list, attachedWeapon.primaryMagazine.definition.ammoTypes);
		for (int i = 0; i < list.Count; i++)
		{
			num += list[i].amount;
		}
		Pool.FreeList<Item>(ref list);
		Profiler.EndSample();
		return num;
	}

	public AmmoTypes GetValidAmmoTypes()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon == (Object)null)
		{
			return (AmmoTypes)2;
		}
		return attachedWeapon.primaryMagazine.definition.ammoTypes;
	}

	public ItemDefinition GetDesiredAmmo()
	{
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon == (Object)null)
		{
			return null;
		}
		return attachedWeapon.primaryMagazine.ammoType;
	}

	public void Reload()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon == (Object)null)
		{
			return;
		}
		nextShotTime = Mathf.Max(nextShotTime, Time.time + Mathf.Min(attachedWeapon.GetReloadDuration() * 0.5f, 2f));
		AmmoTypes ammoTypes = attachedWeapon.primaryMagazine.definition.ammoTypes;
		if (attachedWeapon.primaryMagazine.contents > 0)
		{
			bool flag = false;
			if (base.inventory.capacity > base.inventory.itemList.Count)
			{
				flag = true;
			}
			else
			{
				int num = 0;
				foreach (Item item in base.inventory.itemList)
				{
					if ((Object)(object)item.info == (Object)(object)attachedWeapon.primaryMagazine.ammoType)
					{
						num += item.MaxStackable() - item.amount;
					}
				}
				flag = num >= attachedWeapon.primaryMagazine.contents;
			}
			if (!flag)
			{
				return;
			}
			base.inventory.AddItem(attachedWeapon.primaryMagazine.ammoType, attachedWeapon.primaryMagazine.contents, 0uL);
			attachedWeapon.primaryMagazine.contents = 0;
		}
		List<Item> list = Pool.GetList<Item>();
		base.inventory.FindAmmo(list, ammoTypes);
		if (list.Count > 0)
		{
			Effect.server.Run(reloadEffect.resourcePath, this, StringPool.Get("WeaponAttachmentPoint"), Vector3.zero, Vector3.zero);
			totalAmmoDirty = true;
			attachedWeapon.primaryMagazine.ammoType = list[0].info;
			int num2 = 0;
			while (attachedWeapon.primaryMagazine.contents < attachedWeapon.primaryMagazine.capacity && num2 < list.Count)
			{
				if ((Object)(object)list[num2].info == (Object)(object)attachedWeapon.primaryMagazine.ammoType)
				{
					int num3 = attachedWeapon.primaryMagazine.capacity - attachedWeapon.primaryMagazine.contents;
					num3 = Mathf.Min(list[num2].amount, num3);
					list[num2].UseItem(num3);
					attachedWeapon.primaryMagazine.contents += num3;
				}
				num2++;
			}
		}
		ItemDefinition ammoType = attachedWeapon.primaryMagazine.ammoType;
		if (Object.op_Implicit((Object)(object)ammoType))
		{
			ItemModProjectile component = ((Component)ammoType).GetComponent<ItemModProjectile>();
			GameObject val = component.projectileObject.Get();
			if (Object.op_Implicit((Object)(object)val))
			{
				Projectile component2 = val.GetComponent<Projectile>();
				if (Object.op_Implicit((Object)(object)component2))
				{
					currentAmmoGravity = 0f;
					currentAmmoVelocity = component.GetMaxVelocity();
				}
				else
				{
					ServerProjectile component3 = val.GetComponent<ServerProjectile>();
					if (Object.op_Implicit((Object)(object)component3))
					{
						currentAmmoGravity = component3.gravityModifier;
						currentAmmoVelocity = component3.speed;
					}
				}
			}
		}
		Pool.FreeList<Item>(ref list);
		attachedWeapon.SendNetworkUpdate();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		totalAmmoDirty = true;
		Reload();
	}

	public void UpdateTotalAmmo()
	{
		int num = totalAmmo;
		totalAmmo = GetTotalAmmo();
		if (num != totalAmmo)
		{
			MarkDirtyForceUpdateOutputs();
		}
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		if (Object.op_Implicit((Object)(object)((Component)item.info).GetComponent<ItemModEntity>()))
		{
			if (((FacepunchBehaviour)this).IsInvoking((Action)UpdateAttachedWeapon))
			{
				UpdateAttachedWeapon();
			}
			((FacepunchBehaviour)this).Invoke((Action)UpdateAttachedWeapon, 0.5f);
		}
	}

	public bool EnsureReloaded(bool onlyReloadIfEmpty = true)
	{
		bool flag = HasReserveAmmo();
		if (onlyReloadIfEmpty)
		{
			if (flag && !HasClipAmmo())
			{
				Reload();
				return true;
			}
		}
		else if (flag)
		{
			Reload();
			return true;
		}
		return false;
	}

	public BaseProjectile GetAttachedWeapon()
	{
		return AttachedWeapon as BaseProjectile;
	}

	public virtual bool HasFallbackWeapon()
	{
		return false;
	}

	private bool HasGenericFireable()
	{
		return (Object)(object)AttachedWeapon != (Object)null && AttachedWeapon.IsInstrument();
	}

	public void UpdateAttachedWeapon()
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		Item slot = base.inventory.GetSlot(0);
		HeldEntity heldEntity = null;
		if (slot != null && (slot.info.category == ItemCategory.Weapon || slot.info.category == ItemCategory.Fun))
		{
			BaseEntity heldEntity2 = slot.GetHeldEntity();
			if ((Object)(object)heldEntity2 != (Object)null)
			{
				HeldEntity component = ((Component)heldEntity2).GetComponent<HeldEntity>();
				if ((Object)(object)component != (Object)null && component.IsUsableByTurret)
				{
					heldEntity = component;
				}
			}
		}
		SetFlag(Flags.Reserved3, (Object)(object)heldEntity != (Object)null);
		if ((Object)(object)heldEntity == (Object)null)
		{
			if (Object.op_Implicit((Object)(object)GetAttachedWeapon()))
			{
				GetAttachedWeapon().SetGenericVisible(wantsVis: false);
				GetAttachedWeapon().SetLightsOn(isOn: false);
			}
			AttachedWeapon = null;
			return;
		}
		heldEntity.SetLightsOn(isOn: true);
		Transform transform = ((Component)heldEntity).transform;
		Transform muzzleTransform = heldEntity.MuzzleTransform;
		heldEntity.SetParent(null);
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		Quaternion val = transform.rotation * Quaternion.Inverse(muzzleTransform.rotation);
		heldEntity.limitNetworking = false;
		heldEntity.SetFlag(Flags.Disabled, b: false);
		heldEntity.SetParent(this, StringPool.Get(((Object)socketTransform).name));
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.rotation *= val;
		Vector3 val2 = socketTransform.InverseTransformPoint(muzzleTransform.position);
		transform.localPosition = Vector3.left * val2.x;
		float num = Vector3.Distance(muzzleTransform.position, transform.position);
		transform.localPosition += Vector3.forward * num * attachedWeaponZOffsetScale;
		heldEntity.SetGenericVisible(wantsVis: true);
		AttachedWeapon = heldEntity;
		totalAmmoDirty = true;
		Reload();
		UpdateTotalAmmo();
	}

	public override void OnKilled(HitInfo info)
	{
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if ((Object)(object)attachedWeapon != (Object)null)
		{
			attachedWeapon.SetGenericVisible(wantsVis: false);
			attachedWeapon.SetLightsOn(isOn: false);
		}
		AttachedWeapon = null;
		base.OnKilled(info);
	}

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if (!IsAuthed(baseEntity))
		{
			return false;
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		UpdateTotalAmmo();
		EnsureReloaded(onlyReloadIfEmpty: false);
		UpdateTotalAmmo();
		nextShotTime = Time.time;
	}

	public virtual float GetMaxAngleForEngagement()
	{
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		float result = (((Object)(object)attachedWeapon == (Object)null) ? 1f : ((1f - Mathf.InverseLerp(0.2f, 1f, attachedWeapon.repeatDelay)) * 7f));
		if (Time.time - lastShotTime > 1f)
		{
			result = 1f;
		}
		return result;
	}

	public void TargetTick()
	{
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (Time.realtimeSinceStartup >= nextVisCheck)
		{
			nextVisCheck = Time.realtimeSinceStartup + Random.Range(0.2f, 0.3f);
			targetVisible = ObjectVisible(target);
			if (targetVisible)
			{
				lastTargetSeenTime = Time.realtimeSinceStartup;
			}
		}
		EnsureReloaded();
		BaseProjectile attachedWeapon = GetAttachedWeapon();
		if (Time.time >= nextShotTime && targetVisible && Mathf.Abs(AngleToTarget(target, currentAmmoGravity != 0f)) < GetMaxAngleForEngagement())
		{
			if (Object.op_Implicit((Object)(object)attachedWeapon))
			{
				if (attachedWeapon.primaryMagazine.contents > 0)
				{
					FireAttachedGun(AimOffset(target), aimCone, null, PeacekeeperMode() ? target : null);
					float delay = (attachedWeapon.isSemiAuto ? (attachedWeapon.repeatDelay * 1.5f) : attachedWeapon.repeatDelay);
					delay = attachedWeapon.ScaleRepeatDelay(delay);
					nextShotTime = Time.time + delay;
				}
				else
				{
					nextShotTime = Time.time + 5f;
				}
			}
			else if (HasFallbackWeapon())
			{
				FireGun(AimOffset(target), aimCone, null, target);
				nextShotTime = Time.time + 0.115f;
			}
			else if (HasGenericFireable())
			{
				AttachedWeapon.ServerUse();
				nextShotTime = Time.time + 0.115f;
			}
			else
			{
				nextShotTime = Time.time + 1f;
			}
		}
		if ((Object)(object)target == (Object)null || target.IsDead() || Time.realtimeSinceStartup - lastTargetSeenTime > 3f || Vector3.Distance(((Component)this).transform.position, ((Component)target).transform.position) > sightRange || (PeacekeeperMode() && !IsEntityHostile(target)))
		{
			SetTarget(null);
		}
	}

	public bool HasTarget()
	{
		return (Object)(object)target != (Object)null && target.IsAlive();
	}

	public void OfflineTick()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		aimDir = Vector3.up;
	}

	public virtual bool IsEntityHostile(BaseCombatEntity ent)
	{
		if (ent is ScarecrowNPC)
		{
			return true;
		}
		if (ent is BasePet basePet && (Object)(object)basePet.Brain.OwningPlayer != (Object)null)
		{
			return basePet.Brain.OwningPlayer.IsHostile() || ent.IsHostile();
		}
		return ent.IsHostile();
	}

	public bool ShouldTarget(BaseCombatEntity targ)
	{
		if (targ is AutoTurret)
		{
			return false;
		}
		if (targ is RidableHorse)
		{
			return false;
		}
		if (targ is BasePet basePet && (Object)(object)basePet.Brain.OwningPlayer != (Object)null && IsAuthed(basePet.Brain.OwningPlayer))
		{
			return false;
		}
		return true;
	}

	private void ScheduleForTargetScan()
	{
		((ObjectWorkQueue<AutoTurret>)updateAutoTurretScanQueue).Add(this);
	}

	public void TargetScan()
	{
		if (HasTarget() || IsOffline() || IsBeingControlled)
		{
			return;
		}
		Profiler.BeginSample("AutoTurret.TargetScan");
		if (targetTrigger.entityContents != null)
		{
			foreach (BaseEntity entityContent in targetTrigger.entityContents)
			{
				BaseCombatEntity baseCombatEntity = entityContent as BaseCombatEntity;
				if ((Object)(object)baseCombatEntity == (Object)null)
				{
					continue;
				}
				if (!Sentry.targetall)
				{
					BasePlayer basePlayer = baseCombatEntity as BasePlayer;
					if ((Object)(object)basePlayer != (Object)null && (IsAuthed(basePlayer) || Ignore(basePlayer)))
					{
						continue;
					}
				}
				if ((PeacekeeperMode() && !IsEntityHostile(baseCombatEntity)) || !baseCombatEntity.IsAlive() || !ShouldTarget(baseCombatEntity) || !InFiringArc(baseCombatEntity) || !ObjectVisible(baseCombatEntity))
				{
					continue;
				}
				SetTarget(baseCombatEntity);
				break;
			}
		}
		if (PeacekeeperMode() && (Object)(object)target == (Object)null)
		{
			nextShotTime = Time.time + 1f;
		}
		Profiler.EndSample();
	}

	protected virtual bool Ignore(BasePlayer player)
	{
		return false;
	}

	public void ServerTick()
	{
		if (base.isClient || base.IsDestroyed)
		{
			return;
		}
		Profiler.BeginSample("AutoTurret.ServerTick");
		float dt = (float)(double)timeSinceLastServerTick;
		timeSinceLastServerTick = 0.0;
		if (!IsOnline())
		{
			OfflineTick();
		}
		else if (!IsBeingControlled)
		{
			if (HasTarget())
			{
				TargetTick();
			}
			else
			{
				IdleTick(dt);
			}
		}
		UpdateFacingToTarget(dt);
		if (totalAmmoDirty && Time.time > nextAmmoCheckTime)
		{
			UpdateTotalAmmo();
			totalAmmoDirty = false;
			nextAmmoCheckTime = Time.time + 0.5f;
		}
		Profiler.EndSample();
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if ((!IsOnline() || HasTarget()) && targetVisible)
		{
			return;
		}
		AutoTurret autoTurret = info.Initiator as AutoTurret;
		if ((Object)(object)autoTurret != (Object)null)
		{
			return;
		}
		SamSite samSite = info.Initiator as SamSite;
		if ((Object)(object)samSite != (Object)null)
		{
			return;
		}
		GunTrap gunTrap = info.Initiator as GunTrap;
		if (!((Object)(object)gunTrap != (Object)null))
		{
			BasePlayer basePlayer = info.Initiator as BasePlayer;
			if (!Object.op_Implicit((Object)(object)basePlayer) || !IsAuthed(basePlayer))
			{
				SetTarget(info.Initiator as BaseCombatEntity);
			}
		}
	}

	public void UpdateFacingToTarget(float dt)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)target != (Object)null && targetVisible && !IsBeingControlled)
		{
			Vector3 val = AimOffset(target);
			Vector3 val2;
			if (peekIndex != 0)
			{
				Vector3 position = ((Component)eyePos).transform.position;
				float num = Vector3.Distance(val, position);
				val2 = val - position;
				Vector3 normalized = ((Vector3)(ref val2)).normalized;
				Vector3 val3 = Vector3.Cross(normalized, Vector3.up);
				val += val3 * visibilityOffsets[peekIndex];
			}
			val2 = val - ((Component)eyePos).transform.position;
			Vector3 val4 = ((Vector3)(ref val2)).normalized;
			if (currentAmmoGravity != 0f)
			{
				float num2 = 0.2f;
				if (target is BasePlayer)
				{
					float num3 = Mathf.Clamp01(target.WaterFactor()) * 1.8f;
					if (num3 > num2)
					{
						num2 = num3;
					}
				}
				val = ((Component)target).transform.position + Vector3.up * num2;
				float angle = GetAngle(((Component)eyePos).transform.position, val, currentAmmoVelocity, currentAmmoGravity);
				Vector3 val5 = Vector3Ex.XZ3D(val) - Vector3Ex.XZ3D(((Component)eyePos).transform.position);
				val5 = ((Vector3)(ref val5)).normalized;
				Vector3 val6 = Quaternion.LookRotation(val5) * Quaternion.Euler(angle, 0f, 0f) * Vector3.forward;
				val4 = val6;
			}
			aimDir = val4;
		}
		UpdateAiming(dt);
	}

	private float GetAngle(Vector3 launchPosition, Vector3 targetPosition, float launchVelocity, float gravityScale)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		float num = Physics.gravity.y * gravityScale;
		float num2 = Vector3.Distance(Vector3Ex.XZ3D(launchPosition), Vector3Ex.XZ3D(targetPosition));
		float num3 = launchPosition.y - targetPosition.y;
		float num4 = Mathf.Pow(launchVelocity, 2f);
		float num5 = Mathf.Pow(launchVelocity, 4f);
		float num6 = Mathf.Atan((num4 + Mathf.Sqrt(num5 - num * (num * Mathf.Pow(num2, 2f) + 2f * num3 * num4))) / (num * num2)) * 57.29578f;
		float num7 = Mathf.Atan((num4 - Mathf.Sqrt(num5 - num * (num * Mathf.Pow(num2, 2f) + 2f * num3 * num4))) / (num * num2)) * 57.29578f;
		if (float.IsNaN(num6) && float.IsNaN(num7))
		{
			return -45f;
		}
		if (float.IsNaN(num6))
		{
			return num7;
		}
		return (num6 > num7) ? num6 : num7;
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		AddSelfAuthorize(deployedBy);
	}

	public override ItemContainerId GetIdealContainer(BasePlayer player, Item item, bool altMove)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return default(ItemContainerId);
	}

	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		bool flag = item.info.category == ItemCategory.Weapon;
		bool flag2 = item.info.category == ItemCategory.Ammunition;
		if (flag)
		{
			return 0;
		}
		if (flag2)
		{
			for (int i = 1; i < base.inventory.capacity; i++)
			{
				if (!base.inventory.SlotTaken(item, i))
				{
					return i;
				}
			}
		}
		return -1;
	}

	public bool IsOnline()
	{
		return IsOn();
	}

	public bool IsOffline()
	{
		return !IsOnline();
	}

	public override void ResetState()
	{
		base.ResetState();
	}

	public virtual Transform GetCenterMuzzle()
	{
		return gun_pitch;
	}

	public float AngleToTarget(BaseCombatEntity potentialtarget, bool use2D = false)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		use2D = true;
		Transform centerMuzzle = GetCenterMuzzle();
		Vector3 position = centerMuzzle.position;
		Vector3 val = AimOffset(potentialtarget);
		Vector3 zero = Vector3.zero;
		Vector3 val2;
		if (use2D)
		{
			zero = Vector3Ex.Direction2D(val, position);
		}
		else
		{
			val2 = val - position;
			zero = ((Vector3)(ref val2)).normalized;
		}
		Vector3 val3;
		if (!use2D)
		{
			val3 = centerMuzzle.forward;
		}
		else
		{
			val2 = Vector3Ex.XZ3D(centerMuzzle.forward);
			val3 = ((Vector3)(ref val2)).normalized;
		}
		return Vector3.Angle(val3, zero);
	}

	public virtual bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return Mathf.Abs(AngleToTarget(potentialtarget)) <= 90f;
	}

	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && IsOffline() && IsAuthed(player);
	}

	public override bool CanUseNetworkCache(Connection connection)
	{
		return false;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.autoturret = Pool.Get<AutoTurret>();
		info.msg.autoturret.users = authorizedPlayers;
		if (info.forDisk || ((Object)(object)info.forConnection?.player != (Object)null && CanChangeID(info.forConnection.player as BasePlayer)))
		{
			info.msg.rcEntity = Pool.Get<RCEntity>();
			info.msg.rcEntity.identifier = GetIdentifier();
		}
	}

	public override void PostSave(SaveInfo info)
	{
		base.PostSave(info);
		info.msg.autoturret.users = null;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.autoturret != null)
		{
			authorizedPlayers = info.msg.autoturret.users;
			info.msg.autoturret.users = null;
		}
		if (info.msg.rcEntity != null)
		{
			UpdateIdentifier(info.msg.rcEntity.identifier);
		}
	}

	public Vector3 AimOffset(BaseCombatEntity aimat)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = aimat as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null)
		{
			if (basePlayer.IsSleeping())
			{
				return ((Component)basePlayer).transform.position + Vector3.up * 0.1f;
			}
			if (basePlayer.IsWounded())
			{
				return ((Component)basePlayer).transform.position + Vector3.up * 0.25f;
			}
			return basePlayer.eyes.position;
		}
		return aimat.CenterPoint();
	}

	public float GetAimSpeed()
	{
		if (HasTarget())
		{
			return 5f;
		}
		return 1f;
	}

	public void UpdateAiming(float dt)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (!(aimDir == Vector3.zero))
		{
			float num = 5f;
			if (base.isServer && !IsBeingControlled)
			{
				num = ((!HasTarget()) ? 15f : 35f);
			}
			Quaternion val = Quaternion.LookRotation(aimDir);
			Quaternion val2 = Quaternion.Euler(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f);
			Quaternion val3 = Quaternion.Euler(((Quaternion)(ref val)).eulerAngles.x, 0f, 0f);
			if (((Component)gun_yaw).transform.rotation != val2)
			{
				((Component)gun_yaw).transform.rotation = Mathx.Lerp(((Component)gun_yaw).transform.rotation, val2, num, dt);
			}
			if (((Component)gun_pitch).transform.localRotation != val3)
			{
				((Component)gun_pitch).transform.localRotation = Mathx.Lerp(((Component)gun_pitch).transform.localRotation, val3, num, dt);
			}
		}
	}

	public bool IsAuthed(ulong id)
	{
		foreach (PlayerNameID authorizedPlayer in authorizedPlayers)
		{
			if (authorizedPlayer.userid == id)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAuthed(BasePlayer player)
	{
		return IsAuthed(player.userID);
	}

	public bool AnyAuthed()
	{
		return authorizedPlayers.Count > 0;
	}

	public virtual bool CanChangeSettings(BasePlayer player)
	{
		return IsAuthed(player) && IsOffline() && player.CanBuild();
	}
}
