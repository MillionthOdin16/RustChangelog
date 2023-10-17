using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseLiquidVessel : AttackEntity
{
	[Header("Liquid Vessel")]
	public GameObjectRef thrownWaterObject;

	public GameObjectRef ThrowEffect3P;

	public SoundDefinition throwSound3P;

	public GameObjectRef fillFromContainer;

	public GameObjectRef fillFromWorld;

	public SoundDefinition fillFromContainerStartSoundDef;

	public SoundDefinition fillFromContainerSoundDef;

	public SoundDefinition fillFromWorldStartSoundDef;

	public SoundDefinition fillFromWorldSoundDef;

	public bool hasLid = false;

	public float throwScale = 10f;

	public bool canDrinkFrom = false;

	public bool updateVMWater = false;

	public float minThrowFrac = 0f;

	public bool useThrowAnim = false;

	public float fillMlPerSec = 500f;

	private float lastFillTime = 0f;

	private float nextFreeTime = 0f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseLiquidVessel.OnRpcMessage", 0);
		try
		{
			if (rpc == 4013436649u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoDrink "));
				}
				TimeWarning val2 = TimeWarning.New("DoDrink", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(4013436649u, "DoDrink", this, player))
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
							RPCMessage msg2 = rPCMessage;
							DoDrink(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoDrink");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2781345828u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SendFilling "));
				}
				TimeWarning val5 = TimeWarning.New("SendFilling", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg3 = rPCMessage;
						SendFilling(msg3);
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in SendFilling");
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 3038767821u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ThrowContents "));
				}
				TimeWarning val7 = TimeWarning.New("ThrowContents", 0);
				try
				{
					TimeWarning val8 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg4 = rPCMessage;
						ThrowContents(msg4);
					}
					finally
					{
						((IDisposable)val8)?.Dispose();
					}
				}
				catch (Exception ex3)
				{
					Debug.LogException(ex3);
					player.Kick("RPC Error in ThrowContents");
				}
				finally
				{
					((IDisposable)val7)?.Dispose();
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

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRepeating((Action)FillCheck, 1f, 1f);
	}

	public override void OnHeldChanged()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		base.OnHeldChanged();
		if (IsDisabled())
		{
			StopFilling();
		}
		if (!hasLid)
		{
			DoThrow(((Component)this).transform.position, Vector3.zero);
			Item item = GetItem();
			if (item != null && item.contents != null)
			{
				item.contents.SetLocked(IsDisabled());
				SendNetworkUpdateImmediate();
			}
		}
	}

	public void SetFilling(bool isFilling)
	{
		SetFlag(Flags.Open, isFilling);
		if (isFilling)
		{
			StartFilling();
		}
		else
		{
			StopFilling();
		}
		OnSetFilling(isFilling);
	}

	public virtual void OnSetFilling(bool flag)
	{
	}

	public void StartFilling()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.realtimeSinceStartup - lastFillTime;
		StopFilling();
		((FacepunchBehaviour)this).InvokeRepeating((Action)FillCheck, 0f, 0.3f);
		if (num > 1f)
		{
			LiquidContainer facingLiquidContainer = GetFacingLiquidContainer();
			if ((Object)(object)facingLiquidContainer != (Object)null && facingLiquidContainer.GetLiquidItem() != null)
			{
				if (fillFromContainer.isValid)
				{
					Effect.server.Run(fillFromContainer.resourcePath, ((Component)facingLiquidContainer).transform.position, Vector3.up);
				}
				ClientRPC(null, "CLIENT_StartFillingSoundsContainer");
			}
			else if (CanFillFromWorld())
			{
				if (fillFromWorld.isValid)
				{
					Effect.server.Run(fillFromWorld.resourcePath, GetOwnerPlayer(), 0u, Vector3.zero, Vector3.up);
				}
				ClientRPC(null, "CLIENT_StartFillingSoundsWorld");
			}
		}
		lastFillTime = Time.realtimeSinceStartup;
	}

	public void StopFilling()
	{
		ClientRPC(null, "CLIENT_StopFillingSounds");
		((FacepunchBehaviour)this).CancelInvoke((Action)FillCheck);
	}

	public void FillCheck()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return;
		}
		float num = Time.realtimeSinceStartup - lastFillTime;
		float num2 = num * fillMlPerSec;
		Vector3 pos = ((Component)ownerPlayer).transform.position - new Vector3(0f, 1f, 0f);
		LiquidContainer facingLiquidContainer = GetFacingLiquidContainer();
		if ((Object)(object)facingLiquidContainer == (Object)null && CanFillFromWorld())
		{
			AddLiquid(WaterResource.GetAtPoint(pos), Mathf.FloorToInt(num2));
		}
		else if ((Object)(object)facingLiquidContainer != (Object)null && facingLiquidContainer.HasLiquidItem())
		{
			int num3 = Mathf.CeilToInt((1f - HeldFraction()) * (float)MaxHoldable());
			if (num3 > 0)
			{
				Item liquidItem = facingLiquidContainer.GetLiquidItem();
				int num4 = Mathf.Min(Mathf.CeilToInt(num2), Mathf.Min(liquidItem.amount, num3));
				AddLiquid(liquidItem.info, num4);
				liquidItem.UseItem(num4);
				facingLiquidContainer.OpenTap(2f);
			}
		}
		lastFillTime = Time.realtimeSinceStartup;
	}

	public void LoseWater(int amount)
	{
		if (!base.UsingInfiniteAmmoCheat)
		{
			Item item = GetItem();
			Item slot = item.contents.GetSlot(0);
			if (slot != null)
			{
				slot.UseItem(amount);
				slot.MarkDirty();
				SendNetworkUpdateImmediate();
			}
		}
	}

	public void AddLiquid(ItemDefinition liquidType, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		Item item = GetItem();
		Item item2 = item.contents.GetSlot(0);
		ItemModContainer component = ((Component)item.info).GetComponent<ItemModContainer>();
		if (item2 == null)
		{
			ItemManager.Create(liquidType, amount, 0uL)?.MoveToContainer(item.contents);
			return;
		}
		int num = Mathf.Clamp(item2.amount + amount, 0, component.maxStackSize);
		ItemDefinition itemDefinition = WaterResource.Merge(item2.info, liquidType);
		if ((Object)(object)itemDefinition != (Object)(object)item2.info)
		{
			item2.Remove();
			item2 = ItemManager.Create(itemDefinition, num, 0uL);
			item2.MoveToContainer(item.contents);
		}
		else
		{
			item2.amount = num;
		}
		item2.MarkDirty();
		SendNetworkUpdateImmediate();
	}

	public int AmountHeld()
	{
		Item item = GetItem();
		if (item == null || item.contents == null)
		{
			return 0;
		}
		return item.contents.GetSlot(0)?.amount ?? 0;
	}

	public float HeldFraction()
	{
		Item item = GetItem();
		if (item == null || item.contents == null)
		{
			return 0f;
		}
		return (float)AmountHeld() / (float)MaxHoldable();
	}

	public int MaxHoldable()
	{
		Item item = GetItem();
		if (item == null || item.contents == null)
		{
			return 1;
		}
		return ((Component)GetItem().info).GetComponent<ItemModContainer>().maxStackSize;
	}

	public bool CanDrink()
	{
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return false;
		}
		if (!ownerPlayer.metabolism.CanConsume())
		{
			return false;
		}
		if (!canDrinkFrom)
		{
			return false;
		}
		Item item = GetItem();
		if (item == null)
		{
			return false;
		}
		if (item.contents == null)
		{
			return false;
		}
		if (item.contents.itemList == null)
		{
			return false;
		}
		if (item.contents.itemList.Count == 0)
		{
			return false;
		}
		return true;
	}

	private bool IsWeaponBusy()
	{
		return Time.realtimeSinceStartup < nextFreeTime;
	}

	private void SetBusyFor(float dur)
	{
		nextFreeTime = Time.realtimeSinceStartup + dur;
	}

	private void ClearBusy()
	{
		nextFreeTime = Time.realtimeSinceStartup - 1f;
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void DoDrink(RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		Item item = GetItem();
		if (item == null || item.contents == null || !msg.player.metabolism.CanConsume())
		{
			return;
		}
		foreach (Item item2 in item.contents.itemList)
		{
			ItemModConsume component = ((Component)item2.info).GetComponent<ItemModConsume>();
			if ((Object)(object)component == (Object)null || !component.CanDoAction(item2, msg.player))
			{
				continue;
			}
			component.DoAction(item2, msg.player);
			break;
		}
	}

	[RPC_Server]
	private void ThrowContents(RPCMessage msg)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!((Object)(object)ownerPlayer == (Object)null))
		{
			DoThrow(ownerPlayer.eyes.position + ownerPlayer.eyes.BodyForward() * 1f, ownerPlayer.estimatedVelocity + ownerPlayer.eyes.BodyForward() * throwScale);
			Effect.server.Run(ThrowEffect3P.resourcePath, ((Component)ownerPlayer).transform.position, ownerPlayer.eyes.BodyForward(), ownerPlayer.net.connection);
		}
	}

	public void DoThrow(Vector3 pos, Vector3 velocity)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)ownerPlayer == (Object)null)
		{
			return;
		}
		Item item = GetItem();
		if (item == null || item.contents == null)
		{
			return;
		}
		Item slot = item.contents.GetSlot(0);
		if (slot != null && slot.amount > 0)
		{
			Vector3 val = ownerPlayer.eyes.position + ownerPlayer.eyes.BodyForward() * 1f;
			WaterBall waterBall = GameManager.server.CreateEntity(thrownWaterObject.resourcePath, val, Quaternion.identity) as WaterBall;
			if (Object.op_Implicit((Object)(object)waterBall))
			{
				waterBall.liquidType = slot.info;
				waterBall.waterAmount = slot.amount;
				((Component)waterBall).transform.position = val;
				waterBall.SetVelocity(velocity);
				waterBall.Spawn();
			}
			slot.UseItem(slot.amount);
			slot.MarkDirty();
			SendNetworkUpdateImmediate();
		}
	}

	[RPC_Server]
	private void SendFilling(RPCMessage msg)
	{
		bool filling = msg.read.Bit();
		SetFilling(filling);
	}

	public bool CanFillFromWorld()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return false;
		}
		if (ownerPlayer.IsInWaterVolume(((Component)this).transform.position))
		{
			return false;
		}
		return ownerPlayer.WaterFactor() >= 0.05f;
	}

	public bool CanThrow()
	{
		return HeldFraction() > minThrowFrac;
	}

	public LiquidContainer GetFacingLiquidContainer()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return null;
		}
		RaycastHit hit = default(RaycastHit);
		if (Physics.Raycast(ownerPlayer.eyes.HeadRay(), ref hit, 2f, 1237003025))
		{
			BaseEntity entity = hit.GetEntity();
			if (Object.op_Implicit((Object)(object)entity) && !((Component)((RaycastHit)(ref hit)).collider).gameObject.CompareTag("Not Player Usable") && !((Component)((RaycastHit)(ref hit)).collider).gameObject.CompareTag("Usable Primary"))
			{
				entity = entity.ToServer<BaseEntity>();
				return ((Component)entity).GetComponent<LiquidContainer>();
			}
		}
		return null;
	}
}
