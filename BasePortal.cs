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

	public uint targetID;

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
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_UsePortal "));
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
						val3 = TimeWarning.New("Call", 0);
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
							((IDisposable)val3)?.Dispose();
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
		base.Save(info);
		info.msg.ioEntity = Pool.Get<IOEntity>();
		info.msg.ioEntity.genericEntRef1 = targetID;
	}

	public override void Load(LoadInfo info)
	{
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
		if ((Object)(object)targetPortal != (Object)null)
		{
			targetID = targetPortal.net.ID;
		}
		if ((Object)(object)targetPortal == (Object)null && targetID != 0)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		destination_pos = destPos;
		destination_rot = destRot;
	}

	public Vector3 GetLocalEntryExitPosition()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)localEntryExitPos).transform.position;
	}

	public Quaternion GetLocalEntryExitRotation()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)localEntryExitPos).transform.rotation;
	}

	public BasePortal GetPortal()
	{
		LinkPortal();
		return targetPortal;
	}

	public virtual void UsePortal(BasePlayer player)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
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
