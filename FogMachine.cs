using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class FogMachine : ContainerIOEntity
{
	public const Flags FogFieldOn = Flags.Reserved10;

	public const Flags MotionMode = Flags.Reserved9;

	public const Flags Emitting = Flags.Reserved6;

	public const Flags Flag_HasJuice = Flags.Reserved5;

	public float fogLength = 60f;

	public float nozzleBlastDuration = 5f;

	public float fuelPerSec = 1f;

	private float pendingFuel = 0f;

	public bool IsEmitting()
	{
		return HasFlag(Flags.Reserved6);
	}

	public bool HasJuice()
	{
		return HasFlag(Flags.Reserved5);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetFogOn(RPCMessage msg)
	{
		if (!IsEmitting() && !IsOn() && HasFuel())
		{
			BasePlayer player = msg.player;
			if (player.CanBuild())
			{
				SetFlag(Flags.On, b: true);
				((FacepunchBehaviour)this).InvokeRepeating((Action)StartFogging, 0f, fogLength - 1f);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetFogOff(RPCMessage msg)
	{
		if (IsOn())
		{
			BasePlayer player = msg.player;
			if (player.CanBuild())
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)StartFogging);
				SetFlag(Flags.On, b: false);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetMotionDetection(RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		BasePlayer player = msg.player;
		if (player.CanBuild())
		{
			SetFlag(Flags.Reserved9, flag);
			if (flag)
			{
				SetFlag(Flags.On, b: false);
			}
			UpdateMotionMode();
		}
	}

	public void UpdateMotionMode()
	{
		if (HasFlag(Flags.Reserved9))
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)CheckTrigger, Random.Range(0f, 0.5f), 0.5f, 0.1f);
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)CheckTrigger);
		}
	}

	public void CheckTrigger()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!IsEmitting())
		{
			Vector3 pos = ((Component)this).transform.position + ((Component)this).transform.forward * 3f;
			if (BasePlayer.AnyPlayersVisibleToEntity(pos, 3f, this, ((Component)this).transform.position + Vector3.up * 0.1f, ignorePlayersWithPriv: true))
			{
				StartFogging();
			}
		}
	}

	public void StartFogging()
	{
		if (!UseFuel(1f))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)StartFogging);
			SetFlag(Flags.On, b: false);
			return;
		}
		SetFlag(Flags.Reserved6, b: true);
		((FacepunchBehaviour)this).Invoke((Action)EnableFogField, 1f);
		((FacepunchBehaviour)this).Invoke((Action)DisableNozzle, nozzleBlastDuration);
		((FacepunchBehaviour)this).Invoke((Action)FinishFogging, fogLength);
	}

	public virtual void EnableFogField()
	{
		SetFlag(Flags.Reserved10, b: true);
	}

	public void DisableNozzle()
	{
		SetFlag(Flags.Reserved6, b: false);
	}

	public virtual void FinishFogging()
	{
		SetFlag(Flags.Reserved10, b: false);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.Reserved10, b: false);
		SetFlag(Flags.Reserved6, b: false);
		SetFlag(Flags.Reserved5, HasFuel());
		if (IsOn())
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)StartFogging, 0f, fogLength - 1f);
		}
		UpdateMotionMode();
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		SetFlag(Flags.Reserved5, HasFuel());
		base.PlayerStoppedLooting(player);
	}

	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	public bool HasFuel()
	{
		return GetFuelAmount() >= 1;
	}

	public bool UseFuel(float seconds)
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		pendingFuel += seconds * fuelPerSec;
		if (pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(pendingFuel);
			slot.UseItem(num);
			Analytics.Azure.AddPendingItems(this, slot.info.shortname, num, "fog");
			pendingFuel -= num;
		}
		return true;
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		bool flag = false;
		switch (inputSlot)
		{
		case 0:
			flag = inputAmount > 0;
			break;
		case 1:
			if (inputAmount == 0)
			{
				return;
			}
			flag = true;
			break;
		case 2:
			if (inputAmount == 0)
			{
				return;
			}
			flag = false;
			break;
		}
		if (flag)
		{
			if (!IsEmitting() && !IsOn() && HasFuel())
			{
				SetFlag(Flags.On, b: true);
				((FacepunchBehaviour)this).InvokeRepeating((Action)StartFogging, 0f, fogLength - 1f);
			}
		}
		else if (IsOn())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)StartFogging);
			SetFlag(Flags.On, b: false);
		}
	}

	public virtual bool MotionModeEnabled()
	{
		return true;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("FogMachine.OnRpcMessage", 0);
		try
		{
			if (rpc == 2788115565u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetFogOff "));
				}
				TimeWarning val2 = TimeWarning.New("SetFogOff", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2788115565u, "SetFogOff", this, player, 3f))
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
							RPCMessage fogOff = rPCMessage;
							SetFogOff(fogOff);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetFogOff");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3905831928u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetFogOn "));
				}
				TimeWarning val5 = TimeWarning.New("SetFogOn", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3905831928u, "SetFogOn", this, player, 3f))
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
							RPCMessage fogOn = rPCMessage;
							SetFogOn(fogOn);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SetFogOn");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 1773639087 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetMotionDetection "));
				}
				TimeWarning val8 = TimeWarning.New("SetMotionDetection", 0);
				try
				{
					TimeWarning val9 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1773639087u, "SetMotionDetection", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val9)?.Dispose();
					}
					try
					{
						TimeWarning val10 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage motionDetection = rPCMessage;
							SetMotionDetection(motionDetection);
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in SetMotionDetection");
					}
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
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
}
