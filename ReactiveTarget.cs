using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class ReactiveTarget : IOEntity
{
	public Animator myAnimator;

	public GameObjectRef bullseyeEffect;

	public GameObjectRef knockdownEffect;

	public float activationPowerTime = 0.5f;

	public int activationPowerAmount = 1;

	private float lastToggleTime = float.NegativeInfinity;

	private float knockdownHealth = 100f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ReactiveTarget.OnRpcMessage", 0);
		try
		{
			if (rpc == 1798082523 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Lower "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Lower", 0);
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
						RPC_Lower(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in RPC_Lower");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2169477377u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Reset "));
				}
				TimeWarning val4 = TimeWarning.New("RPC_Reset", 0);
				try
				{
					TimeWarning val5 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg3 = rPCMessage;
						RPC_Reset(msg3);
					}
					finally
					{
						((IDisposable)val5)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in RPC_Reset");
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

	public void OnHitShared(HitInfo info)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		if (IsKnockedDown())
		{
			return;
		}
		bool flag = info.HitBone == StringPool.Get("target_collider");
		bool flag2 = info.HitBone == StringPool.Get("target_collider_bullseye");
		if ((flag || flag2) && base.isServer)
		{
			float num = info.damageTypes.Total();
			if (flag2)
			{
				num *= 2f;
				Effect.server.Run(bullseyeEffect.resourcePath, this, StringPool.Get("target_collider_bullseye"), Vector3.zero, Vector3.zero);
			}
			knockdownHealth -= num;
			if (knockdownHealth <= 0f)
			{
				Effect.server.Run(knockdownEffect.resourcePath, this, StringPool.Get("target_collider_bullseye"), Vector3.zero, Vector3.zero);
				SetFlag(Flags.On, b: false);
				QueueReset();
				SendPowerBurst();
				SendNetworkUpdate();
			}
			else
			{
				ClientRPC<NetworkableId>(null, "HitEffect", info.Initiator.net.ID);
			}
			Hurt(1f, DamageType.Suicide, info.Initiator, useProtection: false);
		}
	}

	public bool IsKnockedDown()
	{
		return !HasFlag(Flags.On);
	}

	public override void OnAttacked(HitInfo info)
	{
		OnHitShared(info);
		base.OnAttacked(info);
	}

	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && CanToggle();
	}

	public bool CanToggle()
	{
		return Time.time > lastToggleTime + 1f;
	}

	public void QueueReset()
	{
		((FacepunchBehaviour)this).Invoke((Action)ResetTarget, 6f);
	}

	public void ResetTarget()
	{
		if (IsKnockedDown() && CanToggle())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)ResetTarget);
			SetFlag(Flags.On, b: true);
			knockdownHealth = 100f;
			SendPowerBurst();
		}
	}

	private void LowerTarget()
	{
		if (!IsKnockedDown() && CanToggle())
		{
			SetFlag(Flags.On, b: false);
			SendPowerBurst();
		}
	}

	private void SendPowerBurst()
	{
		lastToggleTime = Time.time;
		MarkDirtyForceUpdateOutputs();
		((FacepunchBehaviour)this).Invoke((Action)base.MarkDirtyForceUpdateOutputs, activationPowerTime * 1.01f);
	}

	public override int ConsumptionAmount()
	{
		return 1;
	}

	public override bool IsRootEntity()
	{
		return true;
	}

	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
		}
		else if (inputAmount > 0)
		{
			switch (inputSlot)
			{
			case 1:
				ResetTarget();
				break;
			case 2:
				LowerTarget();
				break;
			}
		}
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (IsKnockedDown())
		{
			if (IsPowered())
			{
				return base.GetPassthroughAmount();
			}
			if (Time.time < lastToggleTime + activationPowerTime)
			{
				return activationPowerAmount;
			}
		}
		return 0;
	}

	[RPC_Server]
	public void RPC_Reset(RPCMessage msg)
	{
		ResetTarget();
	}

	[RPC_Server]
	public void RPC_Lower(RPCMessage msg)
	{
		LowerTarget();
	}
}
