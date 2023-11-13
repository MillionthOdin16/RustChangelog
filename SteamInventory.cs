using System;
using System.Linq;
using System.Threading.Tasks;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class SteamInventory : EntityComponent<BasePlayer>
{
	private IPlayerItem[] Items;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SteamInventory.OnRpcMessage", 0);
		try
		{
			if (rpc == 643458331 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - UpdateSteamInventory "));
				}
				TimeWarning val2 = TimeWarning.New("UpdateSteamInventory", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(643458331u, "UpdateSteamInventory", GetBaseEntity(), player))
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
							BaseEntity.RPCMessage rPCMessage = default(BaseEntity.RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							BaseEntity.RPCMessage msg2 = rPCMessage;
							UpdateSteamInventory(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in UpdateSteamInventory");
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

	public bool HasItem(int itemid)
	{
		if (base.baseEntity.UnlockAllSkins)
		{
			return true;
		}
		if (Items == null)
		{
			return false;
		}
		IPlayerItem[] items = Items;
		foreach (IPlayerItem val in items)
		{
			if (val.DefinitionId == itemid)
			{
				return true;
			}
		}
		return false;
	}

	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private async Task UpdateSteamInventory(BaseEntity.RPCMessage msg)
	{
		byte[] data = msg.read.BytesWithSize(10485760u);
		if (data == null)
		{
			Debug.LogWarning((object)"UpdateSteamInventory: Data is null");
			return;
		}
		IPlayerInventory result = await PlatformService.Instance.DeserializeInventory(data);
		if (result == null)
		{
			Debug.LogWarning((object)"UpdateSteamInventory: result is null");
		}
		else if ((Object)(object)base.baseEntity == (Object)null)
		{
			Debug.LogWarning((object)"UpdateSteamInventory: player is null");
		}
		else if (!result.BelongsTo(base.baseEntity.userID))
		{
			Debug.LogWarning((object)$"UpdateSteamPlayer: inventory belongs to someone else (userID={base.baseEntity.userID})");
		}
		else if (Object.op_Implicit((Object)(object)((Component)this).gameObject))
		{
			Items = result.Items.ToArray();
			((IDisposable)result).Dispose();
		}
	}
}
