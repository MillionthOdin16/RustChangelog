using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class RemoteControlEntity : BaseCombatEntity, IRemoteControllable
{
	public static List<IRemoteControllable> allControllables = new List<IRemoteControllable>();

	[Header("RC Entity")]
	public string rcIdentifier = "";

	public Transform viewEyes;

	public GameObjectRef IDPanelPrefab;

	public RemoteControllableControls rcControls = RemoteControllableControls.None;

	public bool CanPing => true;

	public virtual bool CanAcceptInput => false;

	public int ViewerCount { get; private set; }

	public CameraViewerId? ControllingViewerId { get; private set; }

	public bool IsBeingControlled => ViewerCount > 0 && ControllingViewerId.HasValue;

	public virtual bool RequiresMouse => false;

	public virtual float MaxRange => 10000f;

	public RemoteControllableControls RequiredControls => rcControls;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("RemoteControlEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 1053317251 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_SetID "));
				}
				TimeWarning val2 = TimeWarning.New("Server_SetID", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1053317251u, "Server_SetID", this, player, 3f))
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
							Server_SetID(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_SetID");
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

	public Transform GetEyes()
	{
		return viewEyes;
	}

	public float GetFovScale()
	{
		return 1f;
	}

	public BaseEntity GetEnt()
	{
		return this;
	}

	public string GetIdentifier()
	{
		return rcIdentifier;
	}

	public virtual bool InitializeControl(CameraViewerId viewerID)
	{
		ViewerCount++;
		if (CanAcceptInput && !ControllingViewerId.HasValue)
		{
			ControllingViewerId = viewerID;
			return true;
		}
		return !CanAcceptInput;
	}

	public virtual void StopControl(CameraViewerId viewerID)
	{
		ViewerCount--;
		if (ControllingViewerId == viewerID)
		{
			ControllingViewerId = null;
		}
	}

	public virtual void UserInput(InputState inputState, CameraViewerId viewerID)
	{
	}

	public void UpdateIdentifier(string newID, bool clientSend = false)
	{
		string text = rcIdentifier;
		if (base.isServer)
		{
			if (!IDInUse(newID))
			{
				rcIdentifier = newID;
			}
			SendNetworkUpdate();
		}
	}

	public virtual void RCSetup()
	{
		if (base.isServer)
		{
			InstallControllable(this);
		}
	}

	public virtual void RCShutdown()
	{
		if (base.isServer)
		{
			RemoveControllable(this);
		}
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

	public virtual bool CanControl(ulong playerID)
	{
		return true;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void Server_SetID(RPCMessage msg)
	{
		if ((Object)(object)msg.player == (Object)null || !CanControl(msg.player.userID) || !CanChangeID(msg.player))
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

	public override bool CanUseNetworkCache(Connection connection)
	{
		return false;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk || CanChangeID(info.forConnection?.player as BasePlayer))
		{
			info.msg.rcEntity = Pool.Get<RCEntity>();
			info.msg.rcEntity.identifier = GetIdentifier();
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null && ComputerStation.IsValidIdentifier(info.msg.rcEntity.identifier))
		{
			UpdateIdentifier(info.msg.rcEntity.identifier);
		}
	}

	protected virtual bool CanChangeID(BasePlayer player)
	{
		return (Object)(object)player != (Object)null && player.CanBuild() && player.IsBuildingAuthed() && player.IsHoldingEntity<Hammer>();
	}

	public static bool IDInUse(string id)
	{
		return FindByID(id) != null;
	}

	public static IRemoteControllable FindByID(string id)
	{
		foreach (IRemoteControllable allControllable in allControllables)
		{
			if (allControllable != null && allControllable.GetIdentifier() == id)
			{
				return allControllable;
			}
		}
		return null;
	}

	public static bool InstallControllable(IRemoteControllable newControllable)
	{
		if (allControllables.Contains(newControllable))
		{
			return false;
		}
		allControllables.Add(newControllable);
		return true;
	}

	public static bool RemoveControllable(IRemoteControllable newControllable)
	{
		if (!allControllables.Contains(newControllable))
		{
			return false;
		}
		allControllables.Remove(newControllable);
		return true;
	}
}
