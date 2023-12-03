using System;
using System.Linq;
using ConVar;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class VehiclePrivilege : SimplePrivilege
{
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("VehiclePrivilege.OnRpcMessage", 0);
		try
		{
			if (rpc == 1092560690 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - AddSelfAuthorize "));
				}
				TimeWarning val2 = TimeWarning.New("AddSelfAuthorize", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1092560690u, "AddSelfAuthorize", this, player, 3f))
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
							AddSelfAuthorize(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
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
			if (rpc == 253307592 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ClearList "));
				}
				TimeWarning val2 = TimeWarning.New("ClearList", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(253307592u, "ClearList", this, player, 3f))
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
							ClearList(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ClearList");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3617985969u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RemoveSelfAuthorize "));
				}
				TimeWarning val2 = TimeWarning.New("RemoveSelfAuthorize", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3617985969u, "RemoveSelfAuthorize", this, player, 3f))
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
							RemoveSelfAuthorize(rpc4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RemoveSelfAuthorize");
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

	public bool IsDriver(BasePlayer player)
	{
		BaseEntity baseEntity = GetParentEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return false;
		}
		BaseVehicle baseVehicle = baseEntity as BaseVehicle;
		if ((Object)(object)baseVehicle == (Object)null)
		{
			return false;
		}
		return baseVehicle.IsDriver(player);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void AddSelfAuthorize(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && IsDriver(rpc.player))
		{
			AddPlayer(rpc.player);
			SendNetworkUpdate();
		}
	}

	public void AddPlayer(BasePlayer player)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		if (!AtMaxAuthCapacity())
		{
			authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == player.userID);
			PlayerNameID val = new PlayerNameID();
			val.userid = player.userID;
			val.username = player.displayName;
			authorizedPlayers.Add(val);
			Analytics.Azure.OnEntityAuthChanged(this, player, authorizedPlayers.Select((PlayerNameID x) => x.userid), "added", player.userID);
			UpdateMaxAuthCapacity();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RemoveSelfAuthorize(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && IsDriver(rpc.player))
		{
			authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
			Analytics.Azure.OnEntityAuthChanged(this, rpc.player, authorizedPlayers.Select((PlayerNameID x) => x.userid), "removed", rpc.player.userID);
			UpdateMaxAuthCapacity();
			SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void ClearList(RPCMessage rpc)
	{
		if (rpc.player.CanInteract() && IsDriver(rpc.player))
		{
			authorizedPlayers.Clear();
			UpdateMaxAuthCapacity();
			SendNetworkUpdate();
		}
	}
}
