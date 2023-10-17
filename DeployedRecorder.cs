using System;
using System.IO;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class DeployedRecorder : StorageContainer, ICassettePlayer
{
	public AudioSource SoundSource;

	public ItemDefinition[] ValidCassettes;

	public SoundDefinition PlaySfx;

	public SoundDefinition StopSfx;

	public SwapKeycard TapeSwapper;

	private CollisionDetectionMode? initialCollisionDetectionMode;

	public BaseEntity ToBaseEntity => this;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("DeployedRecorder.OnRpcMessage", 0);
		try
		{
			if (rpc == 1785864031 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ServerTogglePlay "));
				}
				TimeWarning val2 = TimeWarning.New("ServerTogglePlay", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1785864031u, "ServerTogglePlay", this, player, 3f))
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
							ServerTogglePlay(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerTogglePlay");
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

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void ServerTogglePlay(RPCMessage msg)
	{
		bool play = ((Stream)(object)msg.read).ReadByte() == 1;
		ServerTogglePlay(play);
	}

	private void ServerTogglePlay(bool play)
	{
		SetFlag(Flags.On, play);
	}

	public void OnCassetteInserted(Cassette c)
	{
		ClientRPC(null, "Client_OnCassetteInserted", c.net.ID);
		SendNetworkUpdate();
	}

	public void OnCassetteRemoved(Cassette c)
	{
		ClientRPC(null, "Client_OnCassetteRemoved");
		ServerTogglePlay(play: false);
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		ItemDefinition[] validCassettes = ValidCassettes;
		for (int i = 0; i < validCassettes.Length; i++)
		{
			if ((Object)(object)validCassettes[i] == (Object)(object)item.info)
			{
				return true;
			}
		}
		return false;
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (base.isServer)
		{
			DoCollisionStick(collision, hitEntity);
		}
	}

	private void DoCollisionStick(Collision collision, BaseEntity ent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ContactPoint contact = collision.GetContact(0);
		DoStick(((ContactPoint)(ref contact)).point, ((ContactPoint)(ref contact)).normal, ent, collision.collider);
	}

	public virtual void SetMotionEnabled(bool wantsMotion)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)component))
		{
			if (!initialCollisionDetectionMode.HasValue)
			{
				initialCollisionDetectionMode = component.collisionDetectionMode;
			}
			component.useGravity = wantsMotion;
			if (!wantsMotion)
			{
				component.collisionDetectionMode = (CollisionDetectionMode)0;
			}
			component.isKinematic = !wantsMotion;
			if (wantsMotion)
			{
				component.collisionDetectionMode = initialCollisionDetectionMode.Value;
			}
		}
	}

	public void DoStick(Vector3 position, Vector3 normal, BaseEntity ent, Collider hitCollider)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ent != (Object)null && ent is TimedExplosive)
		{
			if (!ent.HasParent())
			{
				return;
			}
			position = ((Component)ent).transform.position;
			ent = ent.parentEntity.Get(serverside: true);
		}
		SetMotionEnabled(wantsMotion: false);
		SetCollisionEnabled(wantsCollision: false);
		if (!((Object)(object)ent != (Object)null) || !HasChild(ent))
		{
			((Component)this).transform.position = position;
			((Component)this).transform.rotation = Quaternion.LookRotation(normal, ((Component)this).transform.up);
			if ((Object)(object)hitCollider != (Object)null && (Object)(object)ent != (Object)null)
			{
				SetParent(ent, ent.FindBoneID(((Component)hitCollider).transform), worldPositionStays: true);
			}
			else
			{
				SetParent(ent, StringPool.closest, worldPositionStays: true);
			}
			ReceiveCollisionMessages(b: false);
		}
	}

	private void UnStick()
	{
		if (Object.op_Implicit((Object)(object)GetParentEntity()))
		{
			SetParent(null, worldPositionStays: true, sendImmediate: true);
			SetMotionEnabled(wantsMotion: true);
			SetCollisionEnabled(wantsCollision: true);
			ReceiveCollisionMessages(b: true);
		}
	}

	internal override void OnParentRemoved()
	{
		UnStick();
	}

	public virtual void SetCollisionEnabled(bool wantsCollision)
	{
		Collider component = ((Component)this).GetComponent<Collider>();
		if (Object.op_Implicit((Object)(object)component) && component.enabled != wantsCollision)
		{
			component.enabled = wantsCollision;
		}
	}

	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			initialCollisionDetectionMode = null;
		}
	}
}
