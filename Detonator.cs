using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class Detonator : HeldEntity, IRFObject
{
	public int frequency = 55;

	private float timeSinceDeploy;

	public GameObjectRef frequencyPanelPrefab;

	public GameObjectRef attackEffect;

	public GameObjectRef unAttackEffect;

	private float nextChangeTime = 0f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Detonator.OnRpcMessage", 0);
		try
		{
			if (rpc == 2778616053u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerSetFrequency "));
				}
				TimeWarning val2 = TimeWarning.New("ServerSetFrequency", 0);
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
						ServerSetFrequency(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in ServerSetFrequency");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1106698135 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetPressed "));
				}
				TimeWarning val4 = TimeWarning.New("SetPressed", 0);
				try
				{
					TimeWarning val5 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage pressed = rPCMessage;
						SetPressed(pressed);
					}
					finally
					{
						((IDisposable)val5)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in SetPressed");
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
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
	public void SetPressed(RPCMessage msg)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)msg.player == (Object)null) && !((Object)(object)msg.player != (Object)(object)GetOwnerPlayer()))
		{
			bool flag = HasFlag(Flags.On);
			bool flag2 = msg.read.Bit();
			InternalSetPressed(flag2);
			if (flag != flag2)
			{
				Effect.server.Run(flag2 ? attackEffect.resourcePath : unAttackEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
			}
		}
	}

	internal void InternalSetPressed(bool pressed)
	{
		SetFlag(Flags.On, pressed);
		if (pressed)
		{
			RFManager.AddBroadcaster(frequency, this);
		}
		else
		{
			RFManager.RemoveBroadcaster(frequency, this);
		}
	}

	public Vector3 GetPosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position;
	}

	public float GetMaxRange()
	{
		return 100000f;
	}

	public void RFSignalUpdate(bool on)
	{
	}

	public override void SetHeld(bool bHeld)
	{
		if (!bHeld)
		{
			InternalSetPressed(pressed: false);
		}
		base.SetHeld(bHeld);
	}

	[RPC_Server]
	public void ServerSetFrequency(RPCMessage msg)
	{
		ServerSetFrequency(msg.player, msg.read.Int32());
	}

	public void ServerSetFrequency(BasePlayer player, int freq)
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		if ((Object)(object)player == (Object)null || (Object)(object)GetOwnerPlayer() != (Object)(object)player || Time.time < nextChangeTime)
		{
			return;
		}
		nextChangeTime = Time.time + 2f;
		if (RFManager.IsReserved(freq))
		{
			RFManager.ReserveErrorPrint(player);
			return;
		}
		Item ownerItem = GetOwnerItem();
		RFManager.ChangeFrequency(frequency, freq, this, isListener: false, IsOn());
		frequency = freq;
		SendNetworkUpdate();
		Item item = GetItem();
		if (item != null)
		{
			if (item.instanceData == null)
			{
				item.instanceData = new InstanceData();
				item.instanceData.ShouldPool = false;
			}
			item.instanceData.dataInt = frequency;
			item.MarkDirty();
		}
		ownerItem?.LoseCondition(ownerItem.maxCondition * 0.01f);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<IOEntity>();
		}
		info.msg.ioEntity.genericInt1 = frequency;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			frequency = info.msg.ioEntity.genericInt1;
		}
	}

	public int GetFrequency()
	{
		return frequency;
	}
}
