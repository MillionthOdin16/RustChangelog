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
	public enum ClippingCheckLocation
	{
		HeadOnly,
		WholeBody
	}

	public enum DismountConvarType
	{
		Misc,
		Boating,
		Flying,
		GroundVehicle,
		Horse
	}

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
	public Transform eyePositionOverride;

	public Transform eyeCenterOverride;

	public Vector2 pitchClamp = new Vector2(-80f, 50f);

	public Vector2 yawClamp = new Vector2(-80f, 80f);

	public bool canWieldItems = true;

	public bool relativeViewAngles = true;

	[Header("Mounting")]
	public bool AllowForceMountWhenRestrained;

	public Transform mountAnchor;

	public float mountLOSVertOffset = 0.5f;

	public PlayerModel.MountPoses mountPose;

	public float maxMountDistance = 1.5f;

	public Transform[] dismountPositions;

	public bool checkPlayerLosOnMount;

	public bool disableMeshCullingForPlayers;

	public bool allowHeadLook;

	public bool ignoreVehicleParent;

	public bool legacyDismount;

	public ItemModWearable wearWhileMounted;

	public bool modifiesPlayerCollider;

	public BasePlayer.CapsuleColliderInfo customPlayerCollider;

	public float clippingCheckRadius = 0.4f;

	public bool clippingAndVisChecks;

	public ClippingCheckLocation clippingChecksLocation;

	public SoundDefinition mountSoundDef;

	public SoundDefinition swapSoundDef;

	public SoundDefinition dismountSoundDef;

	public DismountConvarType dismountHoldType;

	public MountStatType mountTimeStatType;

	public MountGestureType allowedGestures;

	public bool canDrinkWhileMounted = true;

	public bool allowSleeperMounting;

	[Help("Set this to true if the mountable is enclosed so it doesn't move inside cars and such")]
	public bool animateClothInLocalSpace = true;

	[Header("Camera")]
	public BasePlayer.CameraMode MountedCameraMode;

	[Header("Rigidbody (Optional)")]
	public Rigidbody rigidBody;

	[FormerlySerializedAs("needsVehicleTick")]
	public bool isMobile;

	public float SideLeanAmount = 0.2f;

	public const float playerHeight = 1.8f;

	public const float playerRadius = 0.5f;

	protected BasePlayer _mounted;

	public static ListHashSet<BaseMountable> FixedUpdateMountables = new ListHashSet<BaseMountable>(8);

	public const float MOUNTABLE_TICK_RATE = 0.05f;

	protected override float PositionTickRate => 0.05f;

	public virtual bool IsSummerDlcVehicle => false;

	protected virtual bool BypassClothingMountBlocks => false;

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
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_WantsDismount "));
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
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_WantsMount "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_WantsMount", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(4014300952u, "RPC_WantsMount", this, player, 3f))
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
							RPC_WantsMount(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
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

	public virtual bool ModifiesThirdPersonCamera()
	{
		return false;
	}

	public virtual Vector2 GetPitchClamp()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return pitchClamp;
	}

	public virtual Vector2 GetYawClamp()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player.GetMounted() != (Object)(object)this)
		{
			return Vector3.zero;
		}
		return GetEyeOverride().position;
	}

	public virtual Vector3 EyeCenterForPlayer(BasePlayer player, Quaternion lookRot)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player.GetMounted() != (Object)(object)this)
		{
			return Vector3.zero;
		}
		return ((Component)eyeCenterOverride).transform.position;
	}

	public virtual float WaterFactorForPlayer(BasePlayer player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
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
		if (player.IsValid())
		{
			return (Object)(object)player.GetMounted() == (Object)(object)this;
		}
		return false;
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
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
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
			if (IsVisibleAndCanSee(position))
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

	public virtual void OnWeaponFired(BaseProjectile weapon)
	{
	}

	public virtual bool CanSwapToThis(BasePlayer player)
	{
		return true;
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (base.CanPickup(player))
		{
			return !AnyMounted();
		}
		return false;
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
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_mounted != (Object)null || IsDead() || !player.CanMountMountablesNow() || IsTransferring() || IsSeatClipping(this) || ClothingBlocksMounting(player))
		{
			return;
		}
		if (doMountChecks)
		{
			RaycastHit hit = default(RaycastHit);
			if (checkPlayerLosOnMount && Physics.Linecast(player.eyes.position, mountAnchor.position + ((Component)this).transform.up * mountLOSVertOffset, ref hit, 1218652417))
			{
				bool flag = false;
				BaseEntity entity = hit.GetEntity();
				if ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)this || (Object)(object)entity == (Object)(object)VehicleParent()))
				{
					flag = true;
				}
				if (!flag)
				{
					Debug.Log((object)"No line of sight to mount pos");
					return;
				}
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
		if (IsTransferring())
		{
			return false;
		}
		if ((Object)(object)VehicleParent() != (Object)null && !VehicleParent().AllowPlayerInstigatedDismount(player))
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
		if (HasValidDismountPosition(player) && (!((Object)(object)player != (Object)null) || !player.IsRestrained))
		{
			AttemptDismount(player);
		}
	}

	public void MountPlayer(BasePlayer player)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)_mounted != (Object)null) && !((Object)(object)mountAnchor == (Object)null))
		{
			player.EnsureDismounted();
			_mounted = player;
			Transform val = mountAnchor;
			player.MountObject(this);
			player.MovePosition(val.position);
			((Component)player).transform.rotation = val.rotation;
			player.ServerRotation = val.rotation;
			Quaternion rotation = val.rotation;
			player.OverrideViewAngles(((Quaternion)(ref rotation)).eulerAngles);
			_mounted.eyes.NetworkUpdate(val.rotation);
			player.ClientRPC<Vector3>(RpcTarget.Player("ForcePositionTo", player), ((Component)player).transform.position);
			Analytics.Azure.OnMountEntity(player, this, VehicleParent());
			OnPlayerMounted();
			if (this.IsValid() && player.IsValid())
			{
				player.ProcessMissionEvent(BaseMission.MissionEventType.MOUNT_ENTITY, net.ID, 1f);
			}
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
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
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
			_mounted.ClientRPC<Vector3>(RpcTarget.Player("ForcePositionTo", _mounted), res);
			BasePlayer mounted = _mounted;
			_mounted = null;
			string[] obj = new string[6] { "Killing player due to invalid dismount point :", player.displayName, " / ", null, null, null };
			BasePlayer.EncryptedValue<ulong> userID = player.userID;
			obj[3] = userID.ToString();
			obj[4] = " on obj : ";
			obj[5] = ((Object)((Component)this).gameObject).name;
			Debug.LogWarning((object)string.Concat(obj));
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
				player.ClientRPC<Vector3, NetworkableId>(RpcTarget.Player("ForcePositionToParentOffset", player), ((Component)baseEntity).transform.InverseTransformPoint(res), baseEntity.net.ID);
			}
			else
			{
				player.ClientRPC<Vector3>(RpcTarget.Player("ForcePositionTo", player), res);
			}
			Analytics.Azure.OnDismountEntity(player, this, baseVehicle);
			OnPlayerDismounted(player);
		}
	}

	public virtual bool GetDismountPosition(BasePlayer player, out Vector3 res)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
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
		string[] obj = new string[6] { "Failed to find dismount position for player :", player.displayName, " / ", null, null, null };
		BasePlayer.EncryptedValue<ulong> userID = player.userID;
		obj[3] = userID.ToString();
		obj[4] = " on obj : ";
		obj[5] = ((Object)((Component)this).gameObject).name;
		Debug.LogWarning((object)string.Concat(obj));
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
		for (int num = FixedUpdateMountables.Count - 1; num >= 0; num--)
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
		for (int num2 = FixedUpdateMountables.Count - 1; num2 >= 0; num2--)
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
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)_mounted))
		{
			((Component)_mounted).transform.rotation = ((Component)mountAnchor).transform.rotation;
			_mounted.ServerRotation = ((Component)mountAnchor).transform.rotation;
			_mounted.MovePosition(((Component)mountAnchor).transform.position);
		}
		if (!((Object)(object)rigidBody != (Object)null) || rigidBody.IsSleeping() || rigidBody.isKinematic)
		{
			return;
		}
		float num = ValidBounds.TestDist(this, ((Component)this).transform.position) - 25f;
		if (num < 0f)
		{
			num = 0f;
		}
		if (!(num < 100f))
		{
			return;
		}
		Vector3 position = ((Component)this).transform.position;
		Vector3 normalized = ((Vector3)(ref position)).normalized;
		float num2 = Vector3.Dot(rigidBody.velocity, normalized);
		if (num2 > 0f)
		{
			float num3 = 1f - num / 100f;
			Rigidbody obj = rigidBody;
			obj.velocity -= normalized * num2 * (num3 * num3);
			if (num < 25f)
			{
				float num4 = 1f - num / 25f;
				rigidBody.AddForce(-normalized * 20f * num4, (ForceMode)5);
			}
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

	public bool TryFireProjectile(StorageContainer ammoStorage, AmmoTypes ammoType, Vector3 firingPos, Vector3 firingDir, BasePlayer shooter, float launchOffset, float minSpeed, out ServerProjectile projectile)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
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
				float num2 = Vector3.Dot(val2, firingDir) - minSpeed;
				if (num2 < 0f)
				{
					val2 += firingDir * (0f - num2);
				}
			}
			projectile.InitializeVelocity(val2);
			if (shooter.IsValid())
			{
				baseEntity.creatorEntity = shooter;
				baseEntity.OwnerID = shooter.userID;
			}
			baseEntity.Spawn();
			Analytics.Azure.OnExplosiveLaunched(shooter, baseEntity, this);
			item.UseItem();
			result = true;
		}
		Pool.FreeList<Item>(ref list);
		return result;
	}

	public override void DisableTransferProtection()
	{
		base.DisableTransferProtection();
		BasePlayer mounted = GetMounted();
		if ((Object)(object)mounted != (Object)null && mounted.IsTransferProtected())
		{
			mounted.DisableTransferProtection();
		}
	}

	protected virtual int GetClipCheckMask()
	{
		return 1210122497;
	}

	public virtual bool IsSeatClipping(BaseMountable mountable)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!clippingAndVisChecks)
		{
			return false;
		}
		if ((Object)(object)mountable == (Object)null)
		{
			return false;
		}
		int clipCheckMask = GetClipCheckMask();
		Vector3 position = ((Component)mountable.eyePositionOverride).transform.position;
		Vector3 position2 = ((Component)mountable).transform.position;
		Vector3 val = position - position2;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		float num = clippingCheckRadius;
		if (mountable.modifiesPlayerCollider)
		{
			num = Mathf.Min(num, mountable.customPlayerCollider.radius);
		}
		Vector3 startPos = position - normalized * (num - 0.2f);
		return IsSeatClipping(mountable, startPos, num, clipCheckMask, position2, normalized);
	}

	public virtual Vector3 GetMountRagdollVelocity(BasePlayer player)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.zero;
	}

	protected virtual bool IsSeatClipping(BaseMountable mountable, Vector3 startPos, float radius, int mask, Vector3 seatPos, Vector3 direction)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (clippingChecksLocation == ClippingCheckLocation.HeadOnly)
		{
			return GamePhysics.CheckSphere(startPos, radius, mask, (QueryTriggerInteraction)1);
		}
		Vector3 end = seatPos + direction * (radius + 0.05f);
		return GamePhysics.CheckCapsule(startPos, end, radius, mask, (QueryTriggerInteraction)1);
	}

	public virtual bool IsInstrument()
	{
		return false;
	}

	public Vector3 GetDismountCheckStart(BasePlayer player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mountAnchor == (Object)null)
		{
			return ((Component)this).transform.position;
		}
		return ((Component)mountAnchor).transform.position;
	}

	public virtual float GetSpeed()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!isMobile)
		{
			return 0f;
		}
		return Vector3.Dot(GetLocalVelocity(), ((Component)this).transform.forward);
	}

	public bool CanPlayerSeeMountPoint(Ray ray, BasePlayer player, float maxDistance)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if ((Object)(object)mountAnchor == (Object)null)
		{
			return false;
		}
		RaycastHit hit = default(RaycastHit);
		if (Physics.SphereCast(ray, 0.25f, ref hit, maxDistance, 1218652417))
		{
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

	public bool NearMountPoint(BasePlayer player)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if ((Object)(object)mountAnchor == (Object)null)
		{
			return false;
		}
		if (Vector3.Distance(((Component)player).transform.position, mountAnchor.position) <= maxMountDistance)
		{
			return CanPlayerSeeMountPoint(player.eyes.HeadRay(), player, 2f);
		}
		return false;
	}

	protected bool ClothingBlocksMounting(BasePlayer player)
	{
		if (BypassClothingMountBlocks)
		{
			return false;
		}
		foreach (Item item in player.inventory.containerWear.itemList)
		{
			if ((Object)(object)item.info.ItemModWearable != (Object)null && item.info.ItemModWearable.preventsMounting)
			{
				return true;
			}
		}
		return false;
	}

	public static Vector3 ConvertVector(Vector3 vec)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
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
