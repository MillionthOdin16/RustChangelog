using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class RFBroadcaster : IOEntity, IRFObject
{
	public int frequency;

	public GameObjectRef frequencyPanelPrefab;

	public const Flags Flag_Broadcasting = Flags.Reserved3;

	public bool playerUsable = true;

	private float nextChangeTime;

	private float nextStopTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("RFBroadcaster.OnRpcMessage", 0);
		try
		{
			if (rpc == 2778616053u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ServerSetFrequency "));
				}
				TimeWarning val2 = TimeWarning.New("ServerSetFrequency", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2778616053u, "ServerSetFrequency", this, player, 3f))
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

	public int GetFrequency()
	{
		return frequency;
	}

	public override bool WantsPower()
	{
		return true;
	}

	public Vector3 GetPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position;
	}

	public float GetMaxRange()
	{
		return 100000f;
	}

	public void RFSignalUpdate(bool on)
	{
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void ServerSetFrequency(RPCMessage msg)
	{
		if (CanChangeFrequency(msg.player) && !(Time.time < nextChangeTime))
		{
			nextChangeTime = Time.time + 2f;
			int newFrequency = msg.read.Int32();
			if (RFManager.IsReserved(newFrequency))
			{
				RFManager.ReserveErrorPrint(msg.player);
				return;
			}
			RFManager.ChangeFrequency(frequency, newFrequency, this, isListener: false, IsPowered());
			frequency = newFrequency;
			MarkDirty();
			SendNetworkUpdate();
			Hurt(MaxHealth() * 0.01f, DamageType.Decay, this);
		}
	}

	public override bool CanUseNetworkCache(Connection connection)
	{
		if (!playerUsable)
		{
			return base.CanUseNetworkCache(connection);
		}
		return false;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk || CanChangeFrequency(info.forConnection?.player as BasePlayer))
		{
			info.msg.ioEntity.genericInt1 = frequency;
		}
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputAmount > 0)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)StopBroadcasting);
			RFManager.AddBroadcaster(frequency, this);
			SetFlag(Flags.Reserved3, b: true);
			nextStopTime = Time.time + 1f;
		}
		else
		{
			((FacepunchBehaviour)this).Invoke((Action)StopBroadcasting, Mathf.Clamp01(nextStopTime - Time.time));
		}
	}

	public void StopBroadcasting()
	{
		SetFlag(Flags.Reserved3, b: false);
		RFManager.RemoveBroadcaster(frequency, this);
	}

	internal override void DoServerDestroy()
	{
		RFManager.RemoveBroadcaster(frequency, this);
		base.DoServerDestroy();
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			frequency = info.msg.ioEntity.genericInt1;
		}
	}

	private bool CanChangeFrequency(BasePlayer player)
	{
		if (playerUsable && (Object)(object)player != (Object)null)
		{
			return player.CanBuild();
		}
		return false;
	}
}
