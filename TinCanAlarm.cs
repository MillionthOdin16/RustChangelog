using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class TinCanAlarm : DecayEntity, IDetector
{
	[Space]
	public LineRenderer lineRenderer;

	public Transform wireOrigin;

	public Transform wireOriginClient;

	public PlayerDetectionTrigger trigger;

	public Transform wireEndCollider;

	public GroundWatch groundWatch;

	public GroundWatch wireGroundWatch;

	public Animator animator;

	[Space]
	public SoundDefinition alarmSoundDef;

	public SoundDefinition armSoundDef;

	private Vector3 endPoint;

	private const Flags Flag_Used = Flags.Reserved5;

	private BaseEntity lastTriggerEntity;

	private float lastTriggerTime;

	[Space]
	public float maxWireLength = 10f;

	public float maxPlaceDistance = 3f;

	private const int WIRE_PLACEMENT_LAYER = 1084293377;

	public Transform WireOrigin
	{
		get
		{
			_ = base.isServer;
			return wireOrigin;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("TinCanAlarm.OnRpcMessage", 0);
		try
		{
			if (rpc == 547827602 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_Arm "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Arm", 0);
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
						RPC_Arm(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in RPC_Arm");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3384266798u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_SetEndPoint "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_SetEndPoint", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg3 = rPCMessage;
						RPC_SetEndPoint(msg3);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in RPC_SetEndPoint");
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

	public bool IsUsed()
	{
		return HasFlag(Flags.Reserved5);
	}

	private bool IsArmed()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return endPoint != Vector3.zero;
	}

	public bool ShouldTrigger()
	{
		return IsArmed();
	}

	public void OnObjects()
	{
	}

	public void OnObjectAdded(GameObject obj, Collider col)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (!((Object)(object)baseEntity != (Object)null))
		{
			return;
		}
		if (baseEntity is BasePlayer basePlayer && basePlayer.isMounted)
		{
			baseEntity = basePlayer.GetMounted();
		}
		else
		{
			BaseEntity baseEntity2 = baseEntity.GetParentEntity();
			if ((Object)(object)baseEntity2 != (Object)null)
			{
				baseEntity = baseEntity2;
			}
		}
		if ((!(Time.realtimeSinceStartup - lastTriggerTime < 1f) || !((Object)(object)baseEntity == (Object)(object)lastTriggerEntity)) && (baseEntity is BasePlayer || baseEntity is Door || baseEntity is BaseNpc || baseEntity is BaseVehicle || baseEntity is Elevator))
		{
			lastTriggerTime = Time.realtimeSinceStartup;
			lastTriggerEntity = baseEntity;
			TriggerAlarm();
		}
	}

	public void OnEmpty()
	{
	}

	private void TriggerAlarm()
	{
		ClientRPC(RpcTarget.NetworkGroup("RPC_TriggerAlarm"));
	}

	public void PlayerStartsArming(BasePlayer player)
	{
		if (!IsUsed() && (Object)(object)player != (Object)null)
		{
			SetFlag(Flags.Reserved5, b: true);
			ClientRPC(RpcTarget.Player("RPC_ArmUse", player), arg1: true);
		}
	}

	public void PlayerStopsArming(BasePlayer player)
	{
		SetFlag(Flags.Reserved5, b: false);
		ClientRPC(RpcTarget.Player("RPC_ArmUse", player), arg1: false);
	}

	[RPC_Server]
	public void RPC_Arm(RPCMessage msg)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		bool flag = msg.read.Bit();
		if ((flag && IsUsed()) || !msg.player.CanBuild())
		{
			return;
		}
		if (flag)
		{
			if (Distance(player.eyes.position) <= 3f)
			{
				PlayerStartsArming(player);
			}
		}
		else
		{
			PlayerStopsArming(player);
		}
	}

	public void CutWire()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TriggerAlarm();
		endPoint = Vector3.zero;
		SendNetworkUpdate();
	}

	private void UpdateWireTip()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			if (!IsArmed())
			{
				ComponentExtensions.SetActive<Transform>(wireEndCollider, false);
				return;
			}
			wireEndCollider.position = endPoint;
			ComponentExtensions.SetActive<Transform>(wireEndCollider, true);
		}
	}

	private void OnGroundMissing()
	{
		if (!base.IsDestroyed && !base.isClient)
		{
			if (!groundWatch.OnGround())
			{
				Kill(DestroyMode.Gib);
			}
			else if (!wireGroundWatch.OnGround())
			{
				CutWire();
			}
		}
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		if (base.isServer)
		{
			PlayerStartsArming(deployedBy);
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (info.hasDamage && !info.damageTypes.Has(DamageType.Heat))
		{
			TriggerAlarm();
		}
	}

	private void UpdateTrigger()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if (!IsArmed())
		{
			ComponentExtensions.SetActive<PlayerDetectionTrigger>(trigger, false);
			return;
		}
		ComponentExtensions.SetActive<PlayerDetectionTrigger>(trigger, true);
		Vector3 position = wireOrigin.position;
		Vector3 val = endPoint;
		Vector3 position2 = (position + val) / 2f;
		Vector3 val2 = val - position;
		float magnitude = ((Vector3)(ref val2)).magnitude;
		((Component)trigger).transform.position = position2;
		Vector3 localScale = ((Component)trigger).transform.localScale;
		localScale.z = magnitude;
		((Component)trigger).transform.rotation = Quaternion.LookRotation(val2);
		((Component)trigger).transform.localScale = new Vector3(0.15f, 0.15f, localScale.z);
	}

	public override void Save(SaveInfo info)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.tinCanAlarm = Pool.Get<TinCanAlarm>();
		info.msg.tinCanAlarm.endPoint = endPoint;
		UpdateTrigger();
		UpdateWireTip();
	}

	public override void Load(LoadInfo info)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.tinCanAlarm != null)
		{
			endPoint = info.msg.tinCanAlarm.endPoint;
			UpdateTrigger();
		}
	}

	[RPC_Server]
	public void RPC_SetEndPoint(RPCMessage msg)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		Vector3 val = msg.read.Vector3();
		if (player.CanBuild() && player.IsVisibleAndCanSee(val) && !IsGoingThroughWalls(val) && IsInValidVolume(val) && IsOnValidEntities(val) && !(Vector3.Distance(val, player.eyes.position) > maxPlaceDistance) && !(Vector3.Distance(wireOrigin.position, val) > maxWireLength))
		{
			endPoint = val;
			SendNetworkUpdate();
			PlayerStopsArming(player);
		}
	}

	private bool IsGoingThroughWalls(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		float maxDistance = Vector3.Distance(wireOrigin.position, position);
		Vector3 val = position - wireOrigin.position;
		RaycastHit hitInfo;
		bool flag = GamePhysics.Trace(new Ray(wireOrigin.position, val), 0f, out hitInfo, maxDistance, 1218519297, (QueryTriggerInteraction)1, this);
		if (!flag)
		{
			flag = GamePhysics.Trace(new Ray(position, -val), 0f, out var _, maxDistance, 1218519297, (QueryTriggerInteraction)1, this);
		}
		return flag;
	}

	private bool IsInValidVolume(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(position, 0.1f, list, 536870912, (QueryTriggerInteraction)2);
		bool result = true;
		foreach (Collider item in list)
		{
			if (((Component)item).gameObject.HasCustomTag(GameObjectTag.BlockPlacement))
			{
				result = false;
				break;
			}
			if (!((Object)(object)item.GetMonument() != (Object)null))
			{
				ColliderInfo component = ((Component)item).GetComponent<ColliderInfo>();
				if (!((Object)(object)component != (Object)null) || !component.HasFlag(ColliderInfo.Flags.Tunnels))
				{
					result = false;
				}
			}
		}
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	private bool IsOnValidEntities(Vector3 position)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(position, 0.1f, list, 1084293377, (QueryTriggerInteraction)2);
		bool result = true;
		foreach (BaseEntity item in list)
		{
			if (item is AnimatedBuildingBlock || item is ElevatorLift || item is Elevator)
			{
				result = false;
				break;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return result;
	}
}
