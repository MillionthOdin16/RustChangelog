using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class BaseMountable : BaseCombatEntity
{
	public enum MountStatType
	{
		None,
		Boating,
		Flying,
		Driving
	}

	public enum MountGestureType
	{
		None,
		UpperBody
	}

	public static Phrase dismountPhrase = new Phrase("dismount", "Dismount");

	[Header("View")]
	[FormerlySerializedAs("eyeOverride")]
	public Transform eyePositionOverride;

	[FormerlySerializedAs("eyeOverride")]
	public Transform eyeCenterOverride;

	public Vector2 pitchClamp = new Vector2(-80f, 50f);

	public Vector2 yawClamp = new Vector2(-80f, 80f);

	public bool canWieldItems = true;

	public bool relativeViewAngles = true;

	[Header("Mounting")]
	public Transform mountAnchor;

	public float mountLOSVertOffset = 0.5f;

	public PlayerModel.MountPoses mountPose = PlayerModel.MountPoses.Chair;

	public float maxMountDistance = 1.5f;

	public Transform[] dismountPositions;

	public bool checkPlayerLosOnMount = false;

	public bool disableMeshCullingForPlayers = false;

	public bool allowHeadLook = false;

	public bool ignoreVehicleParent = false;

	public bool legacyDismount = false;

	[FormerlySerializedAs("modifyPlayerCollider")]
	public bool modifiesPlayerCollider;

	public BasePlayer.CapsuleColliderInfo customPlayerCollider;

	public SoundDefinition mountSoundDef;

	public SoundDefinition swapSoundDef;

	public SoundDefinition dismountSoundDef;

	public MountStatType mountTimeStatType;

	public MountGestureType allowedGestures = MountGestureType.None;

	public bool canDrinkWhileMounted = true;

	public bool allowSleeperMounting = false;

	[Help("Set this to true if the mountable is enclosed so it doesn't move inside cars and such")]
	public bool animateClothInLocalSpace = true;

	[Header("Camera")]
	public BasePlayer.CameraMode MountedCameraMode = BasePlayer.CameraMode.FirstPerson;

	[FormerlySerializedAs("needsVehicleTick")]
	public bool isMobile = false;

	public float SideLeanAmount = 0.2f;

	public const float playerHeight = 1.8f;

	public const float playerRadius = 0.5f;

	protected BasePlayer _mounted;

	public static ListHashSet<BaseMountable> FixedUpdateMountables = new ListHashSet<BaseMountable>(8);

	protected override float PositionTickRate => 0.05f;

	public virtual bool IsSummerDlcVehicle => false;

	public virtual bool BlocksDoors => true;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseMountable.OnRpcMessage", 0);
		try
		{
			if (rpc == 1735799362 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_WantsDismount "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_WantsDismount", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg2 = rPCMessage;
						RPC_WantsDismount(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in RPC_WantsDismount");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4014300952u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_WantsMount "));
				}
				TimeWarning val4 = TimeWarning.New("RPC_WantsMount", 0);
				try
				{
					TimeWarning val5 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(4014300952u, "RPC_WantsMount", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val5)?.Dispose();
					}
					try
					{
						TimeWarning val6 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							RPC_WantsMount(msg3);
						}
						finally
						{
							((IDisposable)val6)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_WantsMount");
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
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

	public virtual bool CanHoldItems()
	{
		return canWieldItems;
	}

	public virtual BasePlayer.CameraMode GetMountedCameraMode()
	{
		return MountedCameraMode;
	}

	public virtual bool DirectlyMountable()
	{
		return true;
	}

	public virtual Transform GetEyeOverride()
	{
		if ((Object)(object)eyePositionOverride != (Object)null)
		{
			return eyePositionOverride;
		}
		return ((Component)this).transform;
	}

	public virtual Quaternion GetMountedBodyAngles()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return GetEyeOverride().rotation;
	}

	public virtual bool ModifiesThirdPersonCamera()
	{
		return false;
	}

	public virtual Vector2 GetPitchClamp()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return pitchClamp;
	}

	public virtual Vector2 GetYawClamp()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return yawClamp;
	}

	public virtual bool AnyMounted()
	{
		return IsBusy();
	}

	public bool IsMounted()
	{
		return AnyMounted();
	}

	public virtual Vector3 EyePositionForPlayer(BasePlayer player, Quaternion lookRot)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player.GetMounted() != (Object)(object)this)
		{
			return Vector3.zero;
		}
		return ((Component)eyePositionOverride).transform.position;
	}

	public virtual Vector3 EyeCenterForPlayer(BasePlayer player, Quaternion lookRot)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player.GetMounted() != (Object)(object)this)
		{
			return Vector3.zero;
		}
		return ((Component)eyeCenterOverride).transform.position;
	}

	public virtual float WaterFactorForPlayer(BasePlayer player)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		OBB val = player.WorldSpaceBounds();
		return WaterLevel.Factor(((OBB)(ref val)).ToBounds(), waves: true, volumes: true, this);
	}

	public override float MaxVelocity()
	{
		BaseEntity baseEntity = GetParentEntity();
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			return baseEntity.MaxVelocity();
		}
		return base.MaxVelocity();
	}

	public virtual bool PlayerIsMounted(BasePlayer player)
	{
		return player.IsValid() && (Object)(object)player.GetMounted() == (Object)(object)this;
	}

	public virtual BaseVehicle VehicleParent()
	{
		if (ignoreVehicleParent)
		{
			return null;
		}
		return GetParentEntity() as BaseVehicle;
	}

	public virtual bool HasValidDismountPosition(BasePlayer player)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle != (Object)null)
		{
			return baseVehicle.HasValidDismountPosition(player);
		}
		Transform[] array = dismountPositions;
		foreach (Transform val in array)
		{
			if (ValidDismountPosition(player, ((Component)val).transform.position))
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool ValidDismountPosition(BasePlayer player, Vector3 disPos)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		bool debugDismounts = Debugging.DebugDismounts;
		Vector3 dismountCheckStart = GetDismountCheckStart(player);
		if (debugDismounts)
		{
			Debug.Log((object)$"ValidDismountPosition debug: Checking dismount point {disPos} from {dismountCheckStart}.");
		}
		Vector3 val = disPos + new Vector3(0f, 0.5f, 0f);
		Vector3 val2 = disPos + new Vector3(0f, 1.3f, 0f);
		if (!Physics.CheckCapsule(val, val2, 0.5f, 1537286401))
		{
			Vector3 position = disPos + ((Component)this).transform.up * 0.5f;
			if (debugDismounts)
			{
				Debug.Log((object)$"ValidDismountPosition debug: Dismount point {disPos} capsule check is OK.");
			}
			if (IsVisible(position))
			{
				Vector3 val3 = disPos + player.NoClipOffset();
				if (debugDismounts)
				{
					Debug.Log((object)$"ValidDismountPosition debug: Dismount point {disPos} is visible.");
				}
				if (!AntiHack.TestNoClipping(dismountCheckStart, val3, player.NoClipRadius(ConVar.AntiHack.noclip_margin_dismount), ConVar.AntiHack.noclip_backtracking, sphereCast: true, out var _, vehicleLayer: false, legacyDismount ? null : this))
				{
					if (debugDismounts)
					{
						Debug.Log((object)$"<color=green>ValidDismountPosition debug: Dismount point {disPos} is valid</color>.");
						Debug.DrawLine(dismountCheckStart, val3, Color.green, 10f);
					}
					return true;
				}
			}
		}
		if (debugDismounts)
		{
			Debug.DrawLine(dismountCheckStart, disPos, Color.red, 10f);
			if (debugDismounts)
			{
				Debug.Log((object)$"<color=red>ValidDismountPosition debug: Dismount point {disPos} is invalid</color>.");
			}
		}
		return false;
	}

	public BasePlayer GetMounted()
	{
		return _mounted;
	}

	public virtual void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
	}

	public virtual void LightToggle(BasePlayer player)
	{
	}

	public virtual bool CanSwapToThis(BasePlayer player)
	{
		return true;
	}

	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !AnyMounted();
	}

	public override void OnKilled(HitInfo info)
	{
		DismountAllPlayers();
		base.OnKilled(info);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_WantsMount(RPCMessage msg)
	{
		WantsMount(msg.player);
	}

	public void WantsMount(BasePlayer player)
	{
		if (!player.IsValid() || !player.CanInteract())
		{
			return;
		}
		if (!DirectlyMountable())
		{
			BaseVehicle baseVehicle = VehicleParent();
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.WantsMount(player);
				return;
			}
		}
		AttemptMount(player);
	}

	public virtual void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_mounted != (Object)null || IsDead() || !player.CanMountMountablesNow())
		{
			return;
		}
		if (doMountChecks)
		{
			if (checkPlayerLosOnMount && Physics.Linecast(player.eyes.position, mountAnchor.position + ((Component)this).transform.up * mountLOSVertOffset, 1218652417))
			{
				Debug.Log((object)"No line of sight to mount pos");
				return;
			}
			if (!HasValidDismountPosition(player))
			{
				Debug.Log((object)"no valid dismount");
				return;
			}
		}
		MountPlayer(player);
	}

	public virtual bool AttemptDismount(BasePlayer player)
	{
		if ((Object)(object)player != (Object)(object)_mounted)
		{
			return false;
		}
		DismountPlayer(player);
		return true;
	}

	[RPC_Server]
	public void RPC_WantsDismount(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (HasValidDismountPosition(player))
		{
			AttemptDismount(player);
		}
	}

	public void MountPlayer(BasePlayer player)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)_mounted != (Object)null) && !((Object)(object)mountAnchor == (Object)null))
		{
			player.EnsureDismounted();
			_mounted = player;
			Transform transform = ((Component)mountAnchor).transform;
			player.MountObject(this);
			player.MovePosition(transform.position);
			((Component)player).transform.rotation = transform.rotation;
			player.ServerRotation = transform.rotation;
			Quaternion rotation = transform.rotation;
			player.OverrideViewAngles(((Quaternion)(ref rotation)).eulerAngles);
			_mounted.eyes.NetworkUpdate(transform.rotation);
			player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", ((Component)player).transform.position);
			OnPlayerMounted();
		}
	}

	public virtual void OnPlayerMounted()
	{
		UpdateMountFlags();
	}

	public virtual void OnPlayerDismounted(BasePlayer player)
	{
		UpdateMountFlags();
	}

	public virtual void UpdateMountFlags()
	{
		SetFlag(Flags.Busy, (Object)(object)_mounted != (Object)null);
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle != (Object)null)
		{
			baseVehicle.UpdateMountFlags();
		}
	}

	public virtual void DismountAllPlayers()
	{
		if (Object.op_Implicit((Object)(object)_mounted))
		{
			DismountPlayer(_mounted);
		}
	}

	public void DismountPlayer(BasePlayer player, bool lite = false)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_mounted == (Object)null || (Object)(object)_mounted != (Object)(object)player)
		{
			return;
		}
		BaseVehicle baseVehicle = VehicleParent();
		Vector3 res;
		if (lite)
		{
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PrePlayerDismount(player, this);
			}
			_mounted.DismountObject();
			_mounted = null;
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PlayerDismounted(player, this);
			}
			OnPlayerDismounted(player);
		}
		else if (!GetDismountPosition(player, out res) || Distance(res) > 10f)
		{
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PrePlayerDismount(player, this);
			}
			res = ((Component)player).transform.position;
			_mounted.DismountObject();
			_mounted.MovePosition(res);
			_mounted.ClientRPCPlayer<Vector3>(null, _mounted, "ForcePositionTo", res);
			BasePlayer mounted = _mounted;
			_mounted = null;
			Debug.LogWarning((object)("Killing player due to invalid dismount point :" + player.displayName + " / " + player.userID + " on obj : " + ((Object)((Component)this).gameObject).name));
			mounted.Hurt(1000f, DamageType.Suicide, mounted, useProtection: false);
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PlayerDismounted(player, this);
			}
			OnPlayerDismounted(player);
		}
		else
		{
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PrePlayerDismount(player, this);
			}
			_mounted.DismountObject();
			((Component)_mounted).transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
			_mounted.MovePosition(res);
			_mounted.SendNetworkUpdateImmediate();
			_mounted.SendModelState(force: true);
			_mounted = null;
			if ((Object)(object)baseVehicle != (Object)null)
			{
				baseVehicle.PlayerDismounted(player, this);
			}
			player.ForceUpdateTriggers();
			if (Object.op_Implicit((Object)(object)player.GetParentEntity()))
			{
				BaseEntity baseEntity = player.GetParentEntity();
				player.ClientRPCPlayer<Vector3, NetworkableId>(null, player, "ForcePositionToParentOffset", ((Component)baseEntity).transform.InverseTransformPoint(res), baseEntity.net.ID);
			}
			else
			{
				player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", res);
			}
			OnPlayerDismounted(player);
		}
	}

	public virtual bool GetDismountPosition(BasePlayer player, out Vector3 res)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle != (Object)null && baseVehicle.IsVehicleMountPoint(this))
		{
			return baseVehicle.GetDismountPosition(player, out res);
		}
		int num = 0;
		Transform[] array = dismountPositions;
		foreach (Transform val in array)
		{
			if (ValidDismountPosition(player, ((Component)val).transform.position))
			{
				res = ((Component)val).transform.position;
				return true;
			}
			num++;
		}
		Debug.LogWarning((object)("Failed to find dismount position for player :" + player.displayName + " / " + player.userID + " on obj : " + ((Object)((Component)this).gameObject).name));
		res = ((Component)player).transform.position;
		return false;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (isMobile)
		{
			FixedUpdateMountables.Add(this);
		}
	}

	internal override void DoServerDestroy()
	{
		FixedUpdateMountables.Remove(this);
		base.DoServerDestroy();
	}

	public static void FixedUpdateCycle()
	{
		int count = FixedUpdateMountables.Count;
		for (int num = count - 1; num >= 0; num--)
		{
			BaseMountable baseMountable = FixedUpdateMountables[num];
			if ((Object)(object)baseMountable == (Object)null)
			{
				FixedUpdateMountables.RemoveAt(num);
			}
			else if (baseMountable.isSpawned)
			{
				baseMountable.VehicleFixedUpdate();
			}
		}
		count = FixedUpdateMountables.Count;
		for (int num2 = count - 1; num2 >= 0; num2--)
		{
			BaseMountable baseMountable2 = FixedUpdateMountables[num2];
			if ((Object)(object)baseMountable2 == (Object)null)
			{
				FixedUpdateMountables.RemoveAt(num2);
			}
			else if (baseMountable2.isSpawned)
			{
				baseMountable2.PostVehicleFixedUpdate();
			}
		}
	}

	public virtual void VehicleFixedUpdate()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)_mounted))
		{
			((Component)_mounted).transform.rotation = ((Component)mountAnchor).transform.rotation;
			_mounted.ServerRotation = ((Component)mountAnchor).transform.rotation;
			_mounted.MovePosition(((Component)mountAnchor).transform.position);
		}
	}

	public virtual void PostVehicleFixedUpdate()
	{
	}

	public virtual void PlayerServerInput(InputState inputState, BasePlayer player)
	{
	}

	public virtual float GetComfort()
	{
		return 0f;
	}

	public virtual void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
	}

	public bool TryFireProjectile(StorageContainer ammoStorage, AmmoTypes ammoType, Vector3 firingPos, Vector3 firingDir, BasePlayer driver, float launchOffset, float minSpeed, out ServerProjectile projectile)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		projectile = null;
		if ((Object)(object)ammoStorage == (Object)null)
		{
			return false;
		}
		bool result = false;
		List<Item> list = Pool.GetList<Item>();
		ammoStorage.inventory.FindAmmo(list, ammoType);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num].amount <= 0)
			{
				list.RemoveAt(num);
			}
		}
		if (list.Count > 0)
		{
			RaycastHit val = default(RaycastHit);
			if (Physics.Raycast(firingPos, firingDir, ref val, launchOffset, 1237003025))
			{
				launchOffset = ((RaycastHit)(ref val)).distance - 0.1f;
			}
			Item item = list[list.Count - 1];
			ItemModProjectile component = ((Component)item.info).GetComponent<ItemModProjectile>();
			BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, firingPos + firingDir * launchOffset);
			projectile = ((Component)baseEntity).GetComponent<ServerProjectile>();
			Vector3 val2 = projectile.initialVelocity + firingDir * projectile.speed;
			if (minSpeed > 0f)
			{
				float num2 = Vector3.Dot(val2, firingDir);
				float num3 = num2 - minSpeed;
				if (num3 < 0f)
				{
					val2 += firingDir * (0f - num3);
				}
			}
			projectile.InitializeVelocity(val2);
			if (driver.IsValid())
			{
				baseEntity.creatorEntity = driver;
				baseEntity.OwnerID = driver.userID;
			}
			baseEntity.Spawn();
			Analytics.Azure.OnExplosiveLaunched(driver, baseEntity, this);
			item.UseItem();
			result = true;
		}
		Pool.FreeList<Item>(ref list);
		return result;
	}

	public virtual bool IsInstrument()
	{
		return false;
	}

	public Vector3 GetDismountCheckStart(BasePlayer player)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GetMountedPosition() + player.NoClipOffset();
		Vector3 val2 = (((Object)(object)mountAnchor == (Object)null) ? ((Component)this).transform.forward : ((Component)mountAnchor).transform.forward);
		Vector3 val3 = (((Object)(object)mountAnchor == (Object)null) ? ((Component)this).transform.up : ((Component)mountAnchor).transform.up);
		if (mountPose == PlayerModel.MountPoses.Chair)
		{
			val += -val2 * 0.32f;
			val += val3 * 0.25f;
		}
		else if (mountPose == PlayerModel.MountPoses.SitGeneric)
		{
			val += -val2 * 0.26f;
			val += val3 * 0.25f;
		}
		else if (mountPose == PlayerModel.MountPoses.SitGeneric)
		{
			val += -val2 * 0.26f;
		}
		return val;
	}

	public Vector3 GetMountedPosition()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mountAnchor == (Object)null)
		{
			return ((Component)this).transform.position;
		}
		return ((Component)mountAnchor).transform.position;
	}

	public bool NearMountPoint(BasePlayer player)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if ((Object)(object)mountAnchor == (Object)null)
		{
			return false;
		}
		float num = Vector3.Distance(((Component)player).transform.position, mountAnchor.position);
		if (num <= maxMountDistance)
		{
			RaycastHit hit = default(RaycastHit);
			if (!Physics.SphereCast(player.eyes.HeadRay(), 0.25f, ref hit, 2f, 1218652417))
			{
				return false;
			}
			BaseEntity entity = hit.GetEntity();
			if ((Object)(object)entity != (Object)null)
			{
				if ((Object)(object)entity == (Object)(object)this || EqualNetID((BaseNetworkable)entity))
				{
					return true;
				}
				if (entity is BasePlayer basePlayer)
				{
					BaseMountable mounted = basePlayer.GetMounted();
					if ((Object)(object)mounted == (Object)(object)this)
					{
						return true;
					}
					if ((Object)(object)mounted != (Object)null && (Object)(object)mounted.VehicleParent() == (Object)(object)this)
					{
						return true;
					}
				}
				BaseEntity baseEntity = entity.GetParentEntity();
				if (hit.IsOnLayer((Layer)13) && ((Object)(object)baseEntity == (Object)(object)this || EqualNetID((BaseNetworkable)baseEntity)))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static Vector3 ConvertVector(Vector3 vec)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 3; i++)
		{
			if (((Vector3)(ref vec))[i] > 180f)
			{
				ref Vector3 reference = ref vec;
				int num = i;
				((Vector3)(ref reference))[num] = ((Vector3)(ref reference))[num] - 360f;
			}
			else if (((Vector3)(ref vec))[i] < -180f)
			{
				ref Vector3 reference = ref vec;
				int num = i;
				((Vector3)(ref reference))[num] = ((Vector3)(ref reference))[num] + 360f;
			}
		}
		return vec;
	}
}
