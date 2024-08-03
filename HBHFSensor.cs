using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class HBHFSensor : BaseDetector
{
	public GameObjectRef detectUp;

	public GameObjectRef detectDown;

	public GameObjectRef panelPrefab;

	public const Flags Flag_IncludeOthers = Flags.Reserved2;

	public const Flags Flag_IncludeAuthed = Flags.Reserved3;

	private int detectedPlayers;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("HBHFSensor.OnRpcMessage", 0);
		try
		{
			if (rpc == 4073303808u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - SetConfig "));
				}
				TimeWarning val2 = TimeWarning.New("SetConfig", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(4073303808u, "SetConfig", this, player, 3f))
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
							RPCMessage config = rPCMessage;
							SetConfig(config);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetConfig");
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

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return Mathf.Min(detectedPlayers, GetCurrentEnergy());
	}

	public override void OnObjects()
	{
		base.OnObjects();
		UpdatePassthroughAmount();
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdatePassthroughAmount, 0f, 1f, 0.1f);
	}

	public override void OnEmpty()
	{
		base.OnEmpty();
		UpdatePassthroughAmount();
		((FacepunchBehaviour)this).CancelInvoke((Action)UpdatePassthroughAmount);
	}

	public void UpdatePassthroughAmount()
	{
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		int num = detectedPlayers;
		detectedPlayers = 0;
		if (myTrigger.entityContents != null)
		{
			foreach (BaseEntity entityContent in myTrigger.entityContents)
			{
				if (!((Object)(object)entityContent == (Object)null) && entityContent.IsVisible(((Component)this).transform.position + ((Component)this).transform.forward * 0.1f, 10f))
				{
					BasePlayer component = ((Component)entityContent).GetComponent<BasePlayer>();
					bool flag = component.CanBuild();
					if ((!flag || ShouldIncludeAuthorized()) && (flag || ShouldIncludeOthers()) && (Object)(object)component != (Object)null && component.IsAlive() && !component.IsSleeping() && component.isServer)
					{
						detectedPlayers++;
					}
				}
			}
		}
		if (num != detectedPlayers && IsPowered())
		{
			MarkDirty();
			if (detectedPlayers > num)
			{
				Effect.server.Run(detectUp.resourcePath, ((Component)this).transform.position, Vector3.up);
			}
			else if (detectedPlayers < num)
			{
				Effect.server.Run(detectDown.resourcePath, ((Component)this).transform.position, Vector3.up);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void SetConfig(RPCMessage msg)
	{
		if (CanUse(msg.player))
		{
			bool b = msg.read.Bit();
			bool b2 = msg.read.Bit();
			SetFlag(Flags.Reserved3, b);
			SetFlag(Flags.Reserved2, b2);
		}
	}

	public bool CanUse(BasePlayer player)
	{
		return player.CanBuild();
	}

	public bool ShouldIncludeAuthorized()
	{
		return HasFlag(Flags.Reserved3);
	}

	public bool ShouldIncludeOthers()
	{
		return HasFlag(Flags.Reserved2);
	}
}
