using System;
using System.Collections.Generic;
using System.Globalization;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class AppIOEntity : IOEntity
{
	private float _cacheTime;

	private BuildingPrivlidge _cache;

	public abstract AppEntityType Type { get; }

	public virtual bool Value
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("AppIOEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 3018927126u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - PairWithApp "));
				}
				TimeWarning val2 = TimeWarning.New("PairWithApp", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3018927126u, "PairWithApp", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3018927126u, "PairWithApp", this, player, 3f))
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
							PairWithApp(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in PairWithApp");
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

	protected void BroadcastValueChange()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (this.IsValid())
		{
			EntityTarget target = GetTarget();
			AppBroadcast val = Pool.Get<AppBroadcast>();
			val.entityChanged = Pool.Get<AppEntityChanged>();
			val.entityChanged.entityId = net.ID;
			val.entityChanged.payload = Pool.Get<AppEntityPayload>();
			FillEntityPayload(val.entityChanged.payload);
			CompanionServer.Server.Broadcast(target, val);
		}
	}

	internal virtual void FillEntityPayload(AppEntityPayload payload)
	{
		payload.value = Value;
	}

	public override BuildingPrivlidge GetBuildingPrivilege()
	{
		if (Time.realtimeSinceStartup - _cacheTime > 5f)
		{
			_cache = base.GetBuildingPrivilege();
			_cacheTime = Time.realtimeSinceStartup;
		}
		return _cache;
	}

	public EntityTarget GetTarget()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return new EntityTarget(net.ID);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public async void PairWithApp(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		Dictionary<string, string> data = CompanionServer.Util.GetPlayerPairingData(player);
		data.Add("entityId", net.ID.Value.ToString("G", CultureInfo.InvariantCulture));
		data.Add("entityType", ((int)Type).ToString("G", CultureInfo.InvariantCulture));
		data.Add("entityName", GetDisplayName());
		NotificationSendResult result = await CompanionServer.Util.SendPairNotification("entity", player, GetDisplayName(), "Tap to pair with this device.", data);
		if (result == NotificationSendResult.Sent)
		{
			OnPairedWithPlayer(msg.player);
		}
		else
		{
			player.ClientRPCPlayer(null, player, "HandleCompanionPairingResult", (int)result);
		}
	}

	protected virtual void OnPairedWithPlayer(BasePlayer player)
	{
	}
}
