using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class FishMount : StorageContainer
{
	public Animator[] FishRoots = (Animator[])(object)new Animator[0];

	public GameObjectRef[] FishInteractSounds = new GameObjectRef[0];

	public float UseCooldown = 3f;

	public const Flags HasFish = Flags.Reserved1;

	private int GetCurrentFishItemIndex
	{
		get
		{
			ItemModFishable itemModFishable = default(ItemModFishable);
			if (base.inventory.GetSlot(0) == null || !((Component)base.inventory.GetSlot(0).info).TryGetComponent<ItemModFishable>(ref itemModFishable))
			{
				return -1;
			}
			return itemModFishable.FishMountIndex;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("FishMount.OnRpcMessage", 0);
		try
		{
			if (rpc == 3280542489u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - UseFish "));
				}
				TimeWarning val2 = TimeWarning.New("UseFish", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3280542489u, "UseFish", this, player, 3f))
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
							UseFish(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in UseFish");
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

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.simpleInt == null)
		{
			info.msg.simpleInt = Pool.Get<SimpleInt>();
		}
		info.msg.simpleInt.value = GetCurrentFishItemIndex;
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		ItemModFishable itemModFishable = default(ItemModFishable);
		if (((Component)item.info).TryGetComponent<ItemModFishable>(ref itemModFishable) && itemModFishable.CanBeMounted)
		{
			return true;
		}
		return false;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.Busy, b: false);
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		SetFlag(Flags.Reserved1, GetCurrentFishItemIndex >= 0);
		SendNetworkUpdate();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void UseFish(RPCMessage msg)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (HasFlag(Flags.Reserved1) && !IsBusy())
		{
			Effect.client.Run(FishInteractSounds[GetCurrentFishItemIndex].resourcePath, ((Component)this).transform.position);
			SetFlag(Flags.Busy, b: true);
			((FacepunchBehaviour)this).Invoke((Action)ClearBusy, UseCooldown);
			ClientRPC(null, "PlayAnimation");
		}
	}

	private void ClearBusy()
	{
		SetFlag(Flags.Busy, b: false);
	}
}
