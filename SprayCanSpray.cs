using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class SprayCanSpray : DecayEntity, ISplashable
{
	public DateTime sprayTimestamp;

	public ulong sprayedByPlayer;

	public static ListHashSet<SprayCanSpray> AllSprays = new ListHashSet<SprayCanSpray>(8);

	public override bool BypassInsideDecayMultiplier => true;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SprayCanSpray.OnRpcMessage", 0);
		try
		{
			if (rpc == 2774110739u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RequestWaterClear "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RequestWaterClear", 0);
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
						Server_RequestWaterClear(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in Server_RequestWaterClear");
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

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.spray == null)
		{
			info.msg.spray = Pool.Get<Spray>();
		}
		info.msg.spray.sprayedBy = sprayedByPlayer;
		info.msg.spray.timestamp = sprayTimestamp.ToBinary();
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.spray != null)
		{
			sprayedByPlayer = info.msg.spray.sprayedBy;
			sprayTimestamp = DateTime.FromBinary(info.msg.spray.timestamp);
		}
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		sprayTimestamp = DateTime.Now;
		sprayedByPlayer = deployedBy.userID;
		if (Global.MaxSpraysPerPlayer > 0 && sprayedByPlayer != 0)
		{
			int num = -1;
			DateTime now = DateTime.Now;
			int num2 = 0;
			for (int i = 0; i < AllSprays.Count; i++)
			{
				if (AllSprays[i].sprayedByPlayer == sprayedByPlayer)
				{
					num2++;
					if (num == -1 || AllSprays[i].sprayTimestamp < now)
					{
						num = i;
						now = AllSprays[i].sprayTimestamp;
					}
				}
			}
			if (num2 >= Global.MaxSpraysPerPlayer && num != -1)
			{
				AllSprays[num].Kill();
			}
		}
		if ((Object)(object)deployedBy == (Object)null || !deployedBy.IsBuildingAuthed())
		{
			((FacepunchBehaviour)this).Invoke((Action)ApplyOutOfAuthConditionPenalty, 1f);
		}
	}

	private void ApplyOutOfAuthConditionPenalty()
	{
		if (!IsFullySpawned())
		{
			((FacepunchBehaviour)this).Invoke((Action)ApplyOutOfAuthConditionPenalty, 1f);
			return;
		}
		float amount = MaxHealth() * (1f - Global.SprayOutOfAuthMultiplier);
		Hurt(amount, DamageType.Decay);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)RainCheck, 60f, 180f, 30f);
		if (!AllSprays.Contains(this))
		{
			AllSprays.Add(this);
		}
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (AllSprays.Contains(this))
		{
			AllSprays.Remove(this);
		}
	}

	private void RainCheck()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (Climate.GetRain(((Component)this).transform.position) > 0f && IsOutside())
		{
			Kill();
		}
	}

	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return amount > 0;
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		if (!base.IsDestroyed)
		{
			Kill();
		}
		return 1;
	}

	[RPC_Server]
	private void Server_RequestWaterClear(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && Menu_WaterClear_ShowIf(player))
		{
			Kill();
		}
	}

	private bool Menu_WaterClear_ShowIf(BasePlayer player)
	{
		return (Object)(object)player.GetHeldEntity() != (Object)null && player.GetHeldEntity() is BaseLiquidVessel baseLiquidVessel && baseLiquidVessel.AmountHeld() > 0;
	}
}
