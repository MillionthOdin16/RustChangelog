using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class FreeableLootContainer : LootContainer
{
	private const Flags tiedDown = Flags.Reserved8;

	public Buoyancy buoyancy;

	public GameObjectRef freedEffect;

	private Rigidbody rb;

	public uint skinOverride;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("FreeableLootContainer.OnRpcMessage", 0);
		try
		{
			if (rpc == 2202685945u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_FreeCrate "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_FreeCrate", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2202685945u, "RPC_FreeCrate", this, player, 3f))
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
							RPC_FreeCrate(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_FreeCrate");
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

	public Rigidbody GetRB()
	{
		if ((Object)(object)rb == (Object)null)
		{
			rb = ((Component)this).GetComponent<Rigidbody>();
		}
		return rb;
	}

	public bool IsTiedDown()
	{
		return HasFlag(Flags.Reserved8);
	}

	public override void ServerInit()
	{
		GetRB().isKinematic = true;
		buoyancy.buoyancyScale = 0f;
		((Behaviour)buoyancy).enabled = false;
		base.ServerInit();
		if (skinOverride != 0)
		{
			skinID = skinOverride;
			SendNetworkUpdate();
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer && (Object)(object)info.Weapon != (Object)null)
		{
			BaseMelee component = ((Component)info.Weapon).GetComponent<BaseMelee>();
			if (Object.op_Implicit((Object)(object)component) && component.canUntieCrates && IsTiedDown())
			{
				base.health -= 1f;
				info.DidGather = true;
				if (base.health <= 0f)
				{
					base.health = MaxHealth();
					Release(info.InitiatorPlayer);
				}
			}
		}
		base.OnAttacked(info);
	}

	public void Release(BasePlayer ply)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		GetRB().isKinematic = false;
		((Behaviour)buoyancy).enabled = true;
		buoyancy.buoyancyScale = 1f;
		SetFlag(Flags.Reserved8, b: false);
		if (freedEffect.isValid)
		{
			Effect.server.Run(freedEffect.resourcePath, ((Component)this).transform.position, Vector3.up);
		}
		if ((Object)(object)ply != (Object)null && !ply.IsNpc && ply.IsConnected)
		{
			ply.ProcessMissionEvent(BaseMission.MissionEventType.FREE_CRATE, "", 1f);
			Analytics.Server.FreeUnderwaterCrate();
			Analytics.Azure.OnFreeUnderwaterCrate(ply, this);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_FreeCrate(RPCMessage msg)
	{
		if (IsTiedDown())
		{
			BasePlayer player = msg.player;
			Release(player);
		}
	}
}
