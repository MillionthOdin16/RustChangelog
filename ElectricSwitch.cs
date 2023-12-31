using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class ElectricSwitch : IOEntity
{
	public bool isToggleSwitch;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ElectricSwitch.OnRpcMessage", 0);
		try
		{
			if (rpc == 4167839872u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - SVSwitch "));
				}
				TimeWarning val2 = TimeWarning.New("SVSwitch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(4167839872u, "SVSwitch", this, player, 3f))
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
							SVSwitch(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVSwitch");
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

	public override bool WantsPower()
	{
		return IsOn();
	}

	public override int ConsumptionAmount()
	{
		if (!IsOn())
		{
			return 0;
		}
		return 1;
	}

	public override void ResetIOState()
	{
		SetFlag(Flags.On, b: false);
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!IsOn())
		{
			return 0;
		}
		return GetCurrentEnergy();
	}

	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (inputSlot != 0)
		{
			return currentEnergy;
		}
		return base.CalculateCurrentEnergy(inputAmount, inputSlot);
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && inputAmount > 0)
		{
			SetSwitch(wantsOn: true);
		}
		if (inputSlot == 2 && inputAmount > 0)
		{
			SetSwitch(wantsOn: false);
		}
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		SetFlag(Flags.Busy, b: false);
	}

	public virtual void SetSwitch(bool wantsOn)
	{
		if (wantsOn != IsOn())
		{
			SetFlag(Flags.On, wantsOn);
			SetFlag(Flags.Busy, b: true);
			((FacepunchBehaviour)this).Invoke((Action)Unbusy, 0.5f);
			SendNetworkUpdateImmediate();
			MarkDirty();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SVSwitch(RPCMessage msg)
	{
		SetSwitch(!IsOn());
	}

	public void Unbusy()
	{
		SetFlag(Flags.Busy, b: false);
	}
}
