using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class SleepingBagCamper : SleepingBag
{
	public EntityRef<BaseVehicleSeat> AssociatedSeat;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SleepingBagCamper.OnRpcMessage", 0);
		try
		{
			if (rpc == 2177887503u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ServerClearBed "));
				}
				TimeWarning val2 = TimeWarning.New("ServerClearBed", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2177887503u, "ServerClearBed", this, player, 3f))
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
							ServerClearBed(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerClearBed");
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

	public override void ServerInit()
	{
		base.ServerInit();
		SetFlag(Flags.Reserved3, b: true);
	}

	protected override void PostPlayerSpawn(BasePlayer p)
	{
		base.PostPlayerSpawn(p);
		BaseVehicleSeat baseVehicleSeat = AssociatedSeat.Get(base.isServer);
		if ((Object)(object)baseVehicleSeat != (Object)null)
		{
			if (p.IsConnected)
			{
				p.EndSleeping();
			}
			baseVehicleSeat.MountPlayer(p);
		}
	}

	public void SetSeat(BaseVehicleSeat seat, bool sendNetworkUpdate = false)
	{
		AssociatedSeat.Set(seat);
		if (sendNetworkUpdate)
		{
			SendNetworkUpdate();
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.sleepingBagCamper = Pool.Get<SleepingBagCamper>();
			info.msg.sleepingBagCamper.seatID = AssociatedSeat.uid;
		}
	}

	public override RespawnState GetRespawnState(ulong userID)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		RespawnState respawnState = base.GetRespawnState(userID);
		if ((int)respawnState != 1)
		{
			return respawnState;
		}
		if (AssociatedSeat.IsValid(base.isServer))
		{
			BasePlayer mounted = AssociatedSeat.Get(base.isServer).GetMounted();
			if ((Object)(object)mounted != (Object)null && mounted.userID != userID)
			{
				return (RespawnState)2;
			}
		}
		return (RespawnState)1;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void ServerClearBed(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && AssociatedSeat.IsValid(base.isServer) && !((Object)(object)AssociatedSeat.Get(base.isServer).GetMounted() != (Object)(object)player))
		{
			SleepingBag.RemoveBagForPlayer(this, deployerUserID);
			deployerUserID = 0uL;
			SendNetworkUpdate();
		}
	}
}
