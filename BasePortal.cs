using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class BasePortal : BaseCombatEntity
{
	public bool isUsablePortal = true;

	private Vector3 destination_pos;

	private Quaternion destination_rot;

	public BasePortal targetPortal;

	public NetworkableId targetID;

	public Transform localEntryExitPos;

	public Transform relativeAnchor;

	public bool isMirrored = true;

	public GameObjectRef appearEffect;

	public GameObjectRef disappearEffect;

	public GameObjectRef transitionSoundEffect;

	public string useTagString = "";

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BasePortal.OnRpcMessage", 0);
		try
		{
			if (rpc == 561762999 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_UsePortal "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_UsePortal", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(561762999u, "RPC_UsePortal", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(561762999u, "RPC_UsePortal", this, player, 3f))
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
							RPCMessage msg2 = rPCMessage;
							RPC_UsePortal(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_UsePortal");
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

	public override void Save(SaveInfo info)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.ioEntity = Pool.Get<IOEntity>();
		info.msg.ioEntity.genericEntRef1 = targetID;
	}

	public override void Load(LoadInfo info)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			targetID = info.msg.ioEntity.genericEntRef1;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
	}

	public void LinkPortal()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)targetPortal != (Object)null)
		{
			targetID = targetPortal.net.ID;
		}
		if ((Object)(object)targetPortal == (Object)null && ((NetworkableId)(ref targetID)).IsValid)
		{
			BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(targetID);
			if ((Object)(object)baseNetworkable != (Object)null)
			{
				targetPortal = ((Component)baseNetworkable).GetComponent<BasePortal>();
			}
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		Debug.Log((object)"Post server load");
		LinkPortal();
	}

	public void SetDestination(Vector3 destPos, Quaternion destRot)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		destination_pos = destPos;
		destination_rot = destRot;
	}

	public Vector3 GetLocalEntryExitPosition()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)localEntryExitPos).transform.position;
	}

	public Quaternion GetLocalEntryExitRotation()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)localEntryExitPos).transform.rotation;
	}

	public BasePortal GetPortal()
	{
		LinkPortal();
		return targetPortal;
	}

	public virtual void UsePortal(BasePlayer player)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		LinkPortal();
		if ((Object)(object)targetPortal != (Object)null)
		{
			player.PauseFlyHackDetection();
			player.PauseSpeedHackDetection();
			Vector3 position = ((Component)player).transform.position;
			Vector3 val = targetPortal.GetLocalEntryExitPosition();
			Vector3 val2 = ((Component)this).transform.InverseTransformDirection(player.eyes.BodyForward());
			Vector3 val3 = val2;
			if (isMirrored)
			{
				Vector3 val4 = ((Component)this).transform.InverseTransformPoint(((Component)player).transform.position);
				val = ((Component)targetPortal.relativeAnchor).transform.TransformPoint(val4);
				val3 = ((Component)targetPortal.relativeAnchor).transform.TransformDirection(val2);
			}
			else
			{
				val3 = targetPortal.GetLocalEntryExitRotation() * Vector3.forward;
			}
			if (disappearEffect.isValid)
			{
				Effect.server.Run(disappearEffect.resourcePath, position, Vector3.up);
			}
			if (appearEffect.isValid)
			{
				Effect.server.Run(appearEffect.resourcePath, val, Vector3.up);
			}
			player.SetParent(null, worldPositionStays: true);
			player.Teleport(val);
			player.ForceUpdateTriggers();
			player.ClientRPCPlayer<Vector3>(null, player, "ForceViewAnglesTo", val3);
			if (transitionSoundEffect.isValid)
			{
				Effect.server.Run(transitionSoundEffect.resourcePath, ((Component)targetPortal.relativeAnchor).transform.position, Vector3.up);
			}
			player.UpdateNetworkGroup();
			player.SetPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot, b: true);
			SendNetworkUpdateImmediate();
			player.ClientRPCPlayer(null, player, "StartLoading_Quick", arg1: true);
		}
		else
		{
			Debug.Log((object)"No portal...");
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	public void RPC_UsePortal(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (IsActive())
		{
			UsePortal(player);
		}
	}

	public bool IsActive()
	{
		return true;
	}
}
