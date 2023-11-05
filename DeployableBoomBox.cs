using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class DeployableBoomBox : ContainerIOEntity, ICassettePlayer, IAudioConnectionSource
{
	public BoomBox BoxController = null;

	public int PowerUsageWhilePlaying = 10;

	public const int MaxBacktrackHopsClient = 30;

	public bool IsStatic = false;

	public BaseEntity ToBaseEntity => this;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("DeployableBoomBox.OnRpcMessage", 0);
		try
		{
			if (rpc == 1918716764 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_UpdateRadioIP "));
				}
				TimeWarning val2 = TimeWarning.New("Server_UpdateRadioIP", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1918716764u, "Server_UpdateRadioIP", this, player, 2uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(1918716764u, "Server_UpdateRadioIP", this, player, 3f))
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
							Server_UpdateRadioIP(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_UpdateRadioIP");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1785864031 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerTogglePlay "));
				}
				TimeWarning val5 = TimeWarning.New("ServerTogglePlay", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1785864031u, "ServerTogglePlay", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
					try
					{
						TimeWarning val7 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							ServerTogglePlay(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ServerTogglePlay");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
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

	public IOEntity ToEntity()
	{
		return this;
	}

	public override int ConsumptionAmount()
	{
		return IsOn() ? PowerUsageWhilePlaying : 0;
	}

	public override int DesiredPower()
	{
		return IsOn() ? PowerUsageWhilePlaying : 0;
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
			if (!IsPowered() && IsOn())
			{
				BoxController.ServerTogglePlay(play: false);
			}
		}
		else if (IsPowered() && !IsConnectedToAnySlot(this, inputSlot, IOEntity.backtracking))
		{
			BoxController.ServerTogglePlay(inputAmount > 0);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = ItemFilter;
		BoxController.HurtCallback = HurtCallback;
		if (IsStatic)
		{
			SetFlag(Flags.Reserved8, b: true);
		}
	}

	private bool ItemFilter(Item item, int count)
	{
		ItemDefinition[] validCassettes = BoxController.ValidCassettes;
		foreach (ItemDefinition itemDefinition in validCassettes)
		{
			if ((Object)(object)itemDefinition == (Object)(object)item.info)
			{
				return true;
			}
		}
		return false;
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!IsOn())
		{
			return 0;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (inputSlot != 0)
		{
			return currentEnergy;
		}
		return base.CalculateCurrentEnergy(inputAmount, inputSlot);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void ServerTogglePlay(RPCMessage msg)
	{
		BoxController.ServerTogglePlay(msg);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(2uL)]
	[RPC_Server.IsVisible(3f)]
	private void Server_UpdateRadioIP(RPCMessage msg)
	{
		BoxController.Server_UpdateRadioIP(msg);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		BoxController.Save(info);
	}

	public bool ClearRadioByUserId(ulong id)
	{
		return BoxController.ClearRadioByUserId(id);
	}

	public void OnCassetteInserted(Cassette c)
	{
		BoxController.OnCassetteInserted(c);
	}

	public void OnCassetteRemoved(Cassette c)
	{
		BoxController.OnCassetteRemoved(c);
	}

	public void HurtCallback(float amount)
	{
		Hurt(amount, DamageType.Decay);
	}

	public override void Load(LoadInfo info)
	{
		BoxController.Load(info);
		base.Load(info);
		if (base.isServer && IsStatic)
		{
			SetFlag(Flags.Reserved8, b: true);
		}
	}
}
