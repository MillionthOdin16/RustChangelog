using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class PaintedItemStorageEntity : BaseEntity, IServerFileReceiver, IUGCBrowserEntity
{
	private uint _currentImageCrc;

	private ulong lastEditedBy = 0uL;

	public uint[] GetContentCRCs => (_currentImageCrc == 0) ? Array.Empty<uint>() : new uint[1] { _currentImageCrc };

	public UGCType ContentType => UGCType.ImageJpg;

	public List<ulong> EditingHistory => (lastEditedBy != 0) ? new List<ulong> { lastEditedBy } : new List<ulong>();

	public BaseNetworkable UgcEntity => this;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PaintedItemStorageEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 2439017595u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_UpdateImage "));
				}
				TimeWarning val2 = TimeWarning.New("Server_UpdateImage", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2439017595u, "Server_UpdateImage", this, player, 3uL))
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
							Server_UpdateImage(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_UpdateImage");
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

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.paintedItem != null)
		{
			_currentImageCrc = info.msg.paintedItem.imageCrc;
			if (base.isServer)
			{
				lastEditedBy = info.msg.paintedItem.editedBy;
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.paintedItem = Pool.Get<PaintedItem>();
		info.msg.paintedItem.imageCrc = _currentImageCrc;
		info.msg.paintedItem.editedBy = lastEditedBy;
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	private void Server_UpdateImage(RPCMessage msg)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)msg.player == (Object)null || msg.player.userID != base.OwnerID)
		{
			return;
		}
		foreach (Item item2 in msg.player.inventory.containerWear.itemList)
		{
			if (item2.instanceData != null && item2.instanceData.subEntity == net.ID)
			{
				return;
			}
		}
		Item item = msg.player.inventory.FindBySubEntityID(net.ID);
		if (item == null || item.isBroken)
		{
			return;
		}
		byte[] array = msg.read.BytesWithSize(10485760u);
		if (array == null)
		{
			if (_currentImageCrc != 0)
			{
				FileStorage.server.RemoveExact(_currentImageCrc, FileStorage.Type.png, net.ID, 0u);
			}
			_currentImageCrc = 0u;
		}
		else
		{
			if (!ImageProcessing.IsValidPNG(array, 512, 512))
			{
				return;
			}
			uint currentImageCrc = _currentImageCrc;
			if (_currentImageCrc != 0)
			{
				FileStorage.server.RemoveExact(_currentImageCrc, FileStorage.Type.png, net.ID, 0u);
			}
			_currentImageCrc = FileStorage.server.Store(array, FileStorage.Type.png, net.ID);
			if (_currentImageCrc != currentImageCrc)
			{
				item.LoseCondition(0.25f);
			}
			lastEditedBy = msg.player.userID;
		}
		SendNetworkUpdate();
	}

	internal override void DoServerDestroy()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		base.DoServerDestroy();
		if (!Application.isQuitting && net != null)
		{
			FileStorage.server.RemoveAllByEntity(net.ID);
		}
	}

	public void ClearContent()
	{
		_currentImageCrc = 0u;
		SendNetworkUpdate();
	}

	[Conditional("PAINTED_ITEM_DEBUG")]
	private void DebugOnlyLog(string str)
	{
		Debug.Log((object)str, (Object)(object)this);
	}
}
