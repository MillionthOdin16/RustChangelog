using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class WantedPoster : DecayEntity, ISignage, IUGCBrowserEntity, ILOD, IServerFileReceiver
{
	private uint imageCrc;

	private ulong playerId;

	private string playerName;

	public RawImage PhotoImage;

	public RustText WantedName;

	public GameObjectRef AssignDialog;

	public const Flags HasTarget = Flags.Reserved1;

	public uiPlayerPreview.EffectMode EffectMode = uiPlayerPreview.EffectMode.Polaroid;

	public uint[] GetContentCRCs
	{
		get
		{
			if (imageCrc == 0)
			{
				return null;
			}
			return new uint[1] { imageCrc };
		}
	}

	public UGCType ContentType => UGCType.ImageJpg;

	public List<ulong> EditingHistory { get; } = new List<ulong>();


	public BaseNetworkable UgcEntity => this;

	public Vector2i TextureSize => new Vector2i(1024, 1024);

	public int TextureCount => 1;

	public NetworkableId NetworkID => net.ID;

	public FileStorage.Type FileType => FileStorage.Type.jpg;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("WantedPoster.OnRpcMessage", 0);
		try
		{
			if (rpc == 2419123501u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ClearPlayer "));
				}
				TimeWarning val2 = TimeWarning.New("ClearPlayer", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2419123501u, "ClearPlayer", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(2419123501u, "ClearPlayer", this, player, 3f))
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
							ClearPlayer(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ClearPlayer");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 657465493 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - UpdatePoster "));
				}
				TimeWarning val2 = TimeWarning.New("UpdatePoster", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(657465493u, "UpdatePoster", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(657465493u, "UpdatePoster", this, player, 3f))
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
							UpdatePoster(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in UpdatePoster");
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

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	private void UpdatePoster(RPCMessage msg)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)msg.player == (Object)null || !CanUpdateSign(msg.player))
		{
			return;
		}
		ulong num = msg.read.UInt64();
		string text = msg.read.String(256, false);
		byte[] array = msg.read.BytesWithSize(10485760u, false);
		playerId = num;
		playerName = text;
		SetFlag(Flags.Reserved1, b: true);
		if (array == null)
		{
			if (imageCrc != 0)
			{
				FileStorage.server.RemoveExact(imageCrc, FileType, net.ID, 0u);
			}
			imageCrc = 0u;
		}
		else
		{
			if (!ImageProcessing.IsValidJPG(array, 1024, 1024))
			{
				return;
			}
			if (imageCrc != 0)
			{
				FileStorage.server.RemoveExact(imageCrc, FileType, net.ID, 0u);
			}
			imageCrc = FileStorage.server.Store(array, FileType, net.ID);
		}
		SendNetworkUpdate();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	private void ClearPlayer(RPCMessage msg)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)msg.player == (Object)null) && CanUpdateSign(msg.player))
		{
			playerId = 0uL;
			playerName = string.Empty;
			SetFlag(Flags.Reserved1, b: false);
			if (imageCrc != 0)
			{
				FileStorage.server.RemoveExact(imageCrc, FileType, net.ID, 0u);
				imageCrc = 0u;
			}
			SendNetworkUpdate();
		}
	}

	public void SetTextureCRCs(uint[] crcs)
	{
		imageCrc = crcs[0];
		SendNetworkUpdate();
	}

	public void ClearContent()
	{
		imageCrc = 0u;
		SendNetworkUpdate();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.wantedPoster = Pool.Get<WantedPoster>();
		info.msg.wantedPoster.imageCrc = imageCrc;
		info.msg.wantedPoster.playerId = playerId;
		info.msg.wantedPoster.playerName = playerName;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.wantedPoster != null)
		{
			imageCrc = info.msg.wantedPoster.imageCrc;
			playerName = info.msg.wantedPoster.playerName;
			playerId = info.msg.wantedPoster.playerId;
		}
	}

	public bool CanUpdateSign(BasePlayer player)
	{
		if (player.IsAdmin || player.IsDeveloper)
		{
			return true;
		}
		if (!player.CanBuild())
		{
			return false;
		}
		if (IsLocked())
		{
			return player.userID == base.OwnerID;
		}
		return true;
	}

	public uint[] GetTextureCRCs()
	{
		return new uint[1] { imageCrc };
	}
}
