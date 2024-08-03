using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class SpinUpWeapon : BaseProjectile
{
	public float timeBetweenSpinToggle = 1f;

	public float spinUpTime = 1f;

	public GameObjectRef bulletEffect;

	public float projectileThicknessOverride = 0.5f;

	public bool showSpinProgress = true;

	public float spinningMoveSpeedScale = 0.7f;

	public float conditionLossPerSecondSpinning = 1f;

	public ItemModWearable BackpackWearable;

	public const Flags FullySpunFlag = Flags.Reserved10;

	public const Flags SpinningFlag = Flags.Reserved11;

	public const Flags ShootingFlag = Flags.Reserved12;

	private float lastSpinToggleTime = float.NegativeInfinity;

	public override ItemModWearable WearableWhileEquipped
	{
		get
		{
			BasePlayer ownerPlayer = GetOwnerPlayer();
			if ((Object)(object)ownerPlayer != (Object)null && ownerPlayer.inventory.HasBackpackItem())
			{
				return null;
			}
			return BackpackWearable;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SpinUpWeapon.OnRpcMessage", 0);
		try
		{
			if (rpc == 2014484270 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_SetSpinButton "));
				}
				TimeWarning val2 = TimeWarning.New("Server_SetSpinButton", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2014484270u, "Server_SetSpinButton", this, player, 8uL))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(2014484270u, "Server_SetSpinButton", this, player))
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
							Server_SetSpinButton(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_SetSpinButton");
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

	public override float GetOverrideProjectileThickness(Projectile projectile)
	{
		return projectileThicknessOverride;
	}

	public bool IsSpinning()
	{
		return HasFlag(Flags.Reserved11);
	}

	public bool IsFullySpun()
	{
		return HasFlag(Flags.Reserved10);
	}

	public override void ServerUse()
	{
		base.ServerUse();
	}

	public override void ServerReload()
	{
		SetFlag(Flags.Reserved12, b: false);
		base.ServerReload();
	}

	public override void ServerUse(float damageModifier, Transform originOverride = null, bool useBulletThickness = true)
	{
		if (!ServerIsReloading())
		{
			SetFlag(Flags.Reserved12, b: true);
			((FacepunchBehaviour)this).Invoke((Action)StopMainTrigger, repeatDelay * 1.1f);
		}
		base.ServerUse(damageModifier, originOverride, useBulletThickness);
	}

	public virtual void ServerUseBase(float damageModifier, Transform originOverride = null)
	{
		base.ServerUse(damageModifier, originOverride, useBulletThickness: true);
	}

	public override void SetGenericVisible(bool visible)
	{
		base.SetGenericVisible(visible);
		SetFlag(Flags.Reserved11, visible);
	}

	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)ownerPlayer != (Object)null && ownerPlayer.IsNpc)
		{
			SetFlag(Flags.Reserved11, !IsDisabled());
		}
		else
		{
			SetFlag(Flags.Reserved11, b: false);
			SetFlag(Flags.Reserved10, b: false);
			lastSpinToggleTime = float.NegativeInfinity;
		}
		if (IsDisabled())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)UpdateConditionLoss);
			((FacepunchBehaviour)this).CancelInvoke((Action)SetFullySpun);
		}
		else
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateConditionLoss, 0f, 1f);
		}
	}

	public void UpdateConditionLoss()
	{
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!((Object)(object)ownerPlayer == (Object)null) && !ownerPlayer.IsNpc && IsSpinning())
		{
			GetOwnerItem()?.LoseCondition(conditionLossPerSecondSpinning);
		}
	}

	protected override void OnReceivedSignalServer(Signal signal, string arg)
	{
		base.OnReceivedSignalServer(signal, arg);
		if (signal == Signal.Attack)
		{
			SetFlag(Flags.Reserved12, b: true);
			((FacepunchBehaviour)this).Invoke((Action)StopMainTrigger, repeatDelay * 1.1f);
		}
	}

	public void StopMainTrigger()
	{
		SetFlag(Flags.Reserved12, b: false);
	}

	public override void DidAttackServerside()
	{
		base.DidAttackServerside();
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(8uL)]
	[RPC_Server.IsActiveItem]
	private void Server_SetSpinButton(RPCMessage msg)
	{
		bool flag = msg.read.Bit();
		if (!(Time.realtimeSinceStartup < lastSpinToggleTime + 1f))
		{
			SetFlag(Flags.Reserved11, flag);
			((FacepunchBehaviour)this).CancelInvoke((Action)SetFullySpun);
			if (flag)
			{
				((FacepunchBehaviour)this).Invoke((Action)SetFullySpun, spinUpTime);
			}
			else
			{
				SetFlag(Flags.Reserved10, b: false);
			}
			lastSpinToggleTime = Time.realtimeSinceStartup;
		}
	}

	public void SetFullySpun()
	{
		SetFlag(Flags.Reserved10, b: true);
	}
}
