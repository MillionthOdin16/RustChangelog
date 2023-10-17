using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class RFTimedExplosive : TimedExplosive, IRFObject
{
	public SoundPlayer beepLoop;

	private ulong creatorPlayerID;

	public ItemDefinition pickupDefinition;

	public float minutesUntilDecayed = 1440f;

	private int RFFrequency = -1;

	private float decayTickDuration = 3600f;

	private float minutesDecayed;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("RFTimedExplosive.OnRpcMessage", 0);
		try
		{
			if (rpc == 2778075470u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Pickup "));
				}
				TimeWarning val2 = TimeWarning.New("Pickup", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2778075470u, "Pickup", this, player, 3f))
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
							Pickup(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Pickup");
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

	public Vector3 GetPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position;
	}

	public float GetMaxRange()
	{
		return float.PositiveInfinity;
	}

	public void RFSignalUpdate(bool on)
	{
		if (IsArmed() && on && !((FacepunchBehaviour)this).IsInvoking((Action)Explode))
		{
			((FacepunchBehaviour)this).Invoke((Action)Explode, Random.Range(0f, 0.2f));
		}
	}

	public void SetFrequency(int newFreq)
	{
		RFManager.RemoveListener(RFFrequency, this);
		RFFrequency = newFreq;
		if (RFFrequency > 0)
		{
			RFManager.AddListener(RFFrequency, this);
		}
	}

	public int GetFrequency()
	{
		return RFFrequency;
	}

	public override void SetFuse(float fuseLength)
	{
		if (!base.isServer)
		{
			return;
		}
		if (GetFrequency() > 0)
		{
			if (((FacepunchBehaviour)this).IsInvoking((Action)Explode))
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)Explode);
			}
			((FacepunchBehaviour)this).Invoke((Action)ArmRF, fuseLength);
			SetFlag(Flags.Reserved1, b: true, recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved2, b: true);
		}
		else
		{
			base.SetFuse(fuseLength);
		}
	}

	public void ArmRF()
	{
		SetFlag(Flags.On, b: true, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved2, b: false);
		SendNetworkUpdate();
	}

	public void DisarmRF()
	{
		SetFlag(Flags.On, b: false);
		SendNetworkUpdate();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.explosive == null)
		{
			info.msg.explosive = Pool.Get<TimedExplosive>();
		}
		if (info.forDisk)
		{
			info.msg.explosive.freq = GetFrequency();
		}
		info.msg.explosive.creatorID = creatorPlayerID;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		SetFrequency(RFFrequency);
		((FacepunchBehaviour)this).InvokeRandomized((Action)DecayCheck, decayTickDuration, decayTickDuration, 10f);
	}

	public void DecayCheck()
	{
		BuildingPrivlidge buildingPrivilege = GetBuildingPrivilege();
		BasePlayer basePlayer = BasePlayer.FindByID(creatorPlayerID);
		if ((Object)(object)basePlayer != (Object)null && ((Object)(object)buildingPrivilege == (Object)null || !buildingPrivilege.IsAuthed(basePlayer)))
		{
			minutesDecayed += decayTickDuration / 60f;
		}
		if (minutesDecayed >= minutesUntilDecayed)
		{
			Kill();
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (RFFrequency > 0)
		{
			if (((FacepunchBehaviour)this).IsInvoking((Action)Explode))
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)Explode);
			}
			SetFrequency(RFFrequency);
			ArmRF();
		}
	}

	internal override void DoServerDestroy()
	{
		if (RFFrequency > 0)
		{
			RFManager.RemoveListener(RFFrequency, this);
		}
		base.DoServerDestroy();
	}

	public void ChangeFrequency(int newFreq)
	{
		RFManager.ChangeFrequency(RFFrequency, newFreq, this, isListener: true);
		RFFrequency = newFreq;
	}

	public override void SetCreatorEntity(BaseEntity newCreatorEntity)
	{
		base.SetCreatorEntity(newCreatorEntity);
		BasePlayer component = ((Component)newCreatorEntity).GetComponent<BasePlayer>();
		if (Object.op_Implicit((Object)(object)component))
		{
			creatorPlayerID = component.userID;
			if (GetFrequency() > 0)
			{
				component.ConsoleMessage("Frequency is:" + GetFrequency());
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void Pickup(RPCMessage msg)
	{
		if (msg.player.CanInteract() && IsArmed())
		{
			Item item = ItemManager.Create(pickupDefinition, 1, 0uL);
			if (item != null)
			{
				item.instanceData.dataInt = GetFrequency();
				item.SetFlag(Item.Flag.IsOn, IsArmed());
				msg.player.GiveItem(item, GiveItemReason.PickedUp);
				Kill();
			}
		}
	}

	public bool IsArmed()
	{
		return HasFlag(Flags.On);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.explosive == null)
		{
			return;
		}
		creatorPlayerID = info.msg.explosive.creatorID;
		if (base.isServer)
		{
			if (info.fromDisk)
			{
				RFFrequency = info.msg.explosive.freq;
			}
			creatorEntity = BasePlayer.FindByID(creatorPlayerID);
		}
	}

	public bool CanPickup(BasePlayer player)
	{
		return IsArmed();
	}
}
