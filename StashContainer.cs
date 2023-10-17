using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class StashContainer : StorageContainer
{
	public static class StashContainerFlags
	{
		public const Flags Hidden = Flags.Reserved5;
	}

	public Transform visuals;

	public float burriedOffset;

	public float raisedOffset;

	public GameObjectRef buryEffect;

	public float uncoverRange = 3f;

	private float lastToggleTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("StashContainer.OnRpcMessage", 0);
		try
		{
			if (rpc == 4130263076u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_HideStash "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_HideStash", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(4130263076u, "RPC_HideStash", this, player, 3f))
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
							RPCMessage rpc2 = rPCMessage;
							RPC_HideStash(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_HideStash");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 298671803 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_WantsUnhide "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_WantsUnhide", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(298671803u, "RPC_WantsUnhide", this, player, 3f))
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
							RPCMessage rpc3 = rPCMessage;
							RPC_WantsUnhide(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_WantsUnhide");
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

	public bool IsHidden()
	{
		return HasFlag(Flags.Reserved5);
	}

	public bool PlayerInRange(BasePlayer ply)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3.Distance(((Component)this).transform.position, ((Component)ply).transform.position) <= uncoverRange)
		{
			Vector3 val = ((Component)this).transform.position - ply.eyes.position;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			if (Vector3.Dot(ply.eyes.BodyForward(), normalized) > 0.95f)
			{
				return true;
			}
		}
		return false;
	}

	public override void InitShared()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		base.InitShared();
		((Component)visuals).transform.localPosition = Vector3Ex.WithY(((Component)visuals).transform.localPosition, raisedOffset);
	}

	public void DoOccludedCheck()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Physics.SphereCast(new Ray(((Component)this).transform.position + Vector3.up * 5f, Vector3.down), 0.25f, 5f, 2097152))
		{
			DropItems();
			Kill();
		}
	}

	public void OnPhysicsNeighbourChanged()
	{
		if (!((FacepunchBehaviour)this).IsInvoking((Action)DoOccludedCheck))
		{
			((FacepunchBehaviour)this).Invoke((Action)DoOccludedCheck, Random.Range(5f, 10f));
		}
	}

	public void SetHidden(bool isHidden)
	{
		if (!(Time.realtimeSinceStartup - lastToggleTime < 3f) && isHidden != HasFlag(Flags.Reserved5))
		{
			lastToggleTime = Time.realtimeSinceStartup;
			((FacepunchBehaviour)this).Invoke((Action)Decay, 259200f);
			if (base.isServer)
			{
				SetFlag(Flags.Reserved5, isHidden);
			}
		}
	}

	public void DisableNetworking()
	{
		base.limitNetworking = true;
		SetFlag(Flags.Disabled, b: true);
	}

	public void Decay()
	{
		Kill();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		SetHidden(isHidden: false);
	}

	public void ToggleHidden()
	{
		SetHidden(!IsHidden());
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_HideStash(RPCMessage rpc)
	{
		Analytics.Azure.OnStashHidden(rpc.player, this);
		SetHidden(isHidden: true);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_WantsUnhide(RPCMessage rpc)
	{
		if (IsHidden())
		{
			BasePlayer player = rpc.player;
			if (PlayerInRange(player))
			{
				Analytics.Azure.OnStashRevealed(rpc.player, this);
				SetHidden(isHidden: false);
			}
		}
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool num = (old & Flags.Reserved5) == Flags.Reserved5;
		bool flag = (next & Flags.Reserved5) == Flags.Reserved5;
		if (num != flag)
		{
			float num2 = (flag ? burriedOffset : raisedOffset);
			LeanTween.cancel(((Component)visuals).gameObject);
			LeanTween.moveLocalY(((Component)visuals).gameObject, num2, 1f);
		}
	}
}
