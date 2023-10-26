using System;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class TorchWeapon : BaseMelee
{
	[NonSerialized]
	public const float FuelTickAmount = 1f / 12f;

	[Header("TorchWeapon")]
	public AnimatorOverrideController LitHoldAnimationOverride;

	public bool ExtinguishUnderwater = true;

	public bool UseTurnOnOffAnimations;

	public GameObjectRef litStrikeFX;

	public const Flags IsInHolder = Flags.Reserved1;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("TorchWeapon.OnRpcMessage", 0);
		try
		{
			if (rpc == 2235491565u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Extinguish "));
				}
				TimeWarning val2 = TimeWarning.New("Extinguish", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(2235491565u, "Extinguish", this, player))
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
							Extinguish(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Extinguish");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3010584743u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Ignite "));
				}
				TimeWarning val2 = TimeWarning.New("Ignite", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(3010584743u, "Ignite", this, player))
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
							RPCMessage msg3 = rPCMessage;
							Ignite(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Ignite");
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

	public override void GetAttackStats(HitInfo info)
	{
		base.GetAttackStats(info);
		if (HasFlag(Flags.On))
		{
			info.damageTypes.Add(DamageType.Heat, 1f);
		}
	}

	public override float GetConditionLoss()
	{
		return base.GetConditionLoss() + (HasFlag(Flags.On) ? 6f : 0f);
	}

	public void SetIsOn(bool isOn)
	{
		if (isOn)
		{
			SetFlag(Flags.On, b: true);
			((FacepunchBehaviour)this).InvokeRepeating((Action)UseFuel, 1f, 1f);
		}
		else
		{
			SetFlag(Flags.On, b: false);
			((FacepunchBehaviour)this).CancelInvoke((Action)UseFuel);
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void Ignite(RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			SetIsOn(isOn: true);
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void Extinguish(RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			SetIsOn(isOn: false);
		}
	}

	public void UseFuel()
	{
		GetOwnerItem()?.LoseCondition(1f / 12f);
	}

	public override void OnHeldChanged()
	{
		if (IsDisabled())
		{
			SetFlag(Flags.On, b: false);
			((FacepunchBehaviour)this).CancelInvoke((Action)UseFuel);
		}
	}

	public override string GetStrikeEffectPath(string materialName)
	{
		for (int i = 0; i < materialStrikeFX.Count; i++)
		{
			if (materialStrikeFX[i].materialName == materialName && materialStrikeFX[i].fx.isValid)
			{
				return materialStrikeFX[i].fx.resourcePath;
			}
		}
		if (HasFlag(Flags.On) && litStrikeFX.isValid)
		{
			return litStrikeFX.resourcePath;
		}
		return strikeFX.resourcePath;
	}
}
