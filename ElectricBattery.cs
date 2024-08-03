using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class ElectricBattery : IOEntity, IInstanceDataReceiver
{
	public int maxOutput;

	public float maxCapactiySeconds;

	public float rustWattSeconds;

	private int activeDrain;

	public bool rechargable;

	[Tooltip("How much energy we can request from power sources for charging is this value * our maxOutput")]
	public float maximumInboundEnergyRatio = 4f;

	public float chargeRatio = 0.25f;

	private const float tickRateSeconds = 1f;

	public const Flags Flag_HalfFull = Flags.Reserved5;

	public const Flags Flag_VeryFull = Flags.Reserved6;

	private bool wasLoaded;

	private HashSet<(IOEntity entity, int inputIndex)> connectedList = new HashSet<(IOEntity, int)>();

	private Queue<int> inputHistory = new Queue<int>();

	private const int inputHistorySize = 5;

	public override bool IsRootEntity()
	{
		return true;
	}

	public override int ConsumptionAmount()
	{
		return 0;
	}

	public override int MaximalPowerOutput()
	{
		return maxOutput;
	}

	public int GetActiveDrain()
	{
		if (!IsOn())
		{
			return 0;
		}
		return activeDrain;
	}

	public void ReceiveInstanceData(InstanceData data)
	{
		rustWattSeconds = data.dataInt;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		wasLoaded = true;
	}

	public override void OnPickedUp(Item createdItem, BasePlayer player)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		base.OnPickedUp(createdItem, player);
		if (createdItem.instanceData == null)
		{
			createdItem.instanceData = new InstanceData();
		}
		createdItem.instanceData.ShouldPool = false;
		createdItem.instanceData.dataInt = Mathf.FloorToInt(rustWattSeconds);
	}

	public override int GetCurrentEnergy()
	{
		return currentEnergy;
	}

	public override int DesiredPower(int inputIndex = 0)
	{
		if (rustWattSeconds >= maxCapactiySeconds)
		{
			return 0;
		}
		if (!IsFlickering())
		{
			return Mathf.Min(currentEnergy, Mathf.FloorToInt((float)maxOutput * maximumInboundEnergyRatio));
		}
		return GetHighestInputFromHistory();
	}

	public override void SendAdditionalData(BasePlayer player, int slot, bool input)
	{
		int passthroughAmountForAnySlot = GetPassthroughAmountForAnySlot(slot, input);
		ClientRPC(RpcTarget.Player("Client_ReceiveAdditionalData", player), currentEnergy, passthroughAmountForAnySlot, rustWattSeconds, (float)activeDrain);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)CheckDischarge, Random.Range(0f, 1f), 1f, 0.1f);
	}

	public int GetDrainFor(IOEntity ent)
	{
		return 0;
	}

	public void AddConnectedRecursive(IOEntity root, int inputIndex, ref HashSet<(IOEntity, int)> listToUse)
	{
		listToUse.Add((root, inputIndex));
		if (!root.WantsPassthroughPower())
		{
			return;
		}
		for (int i = 0; i < root.outputs.Length; i++)
		{
			if (!root.AllowDrainFrom(i))
			{
				continue;
			}
			IOSlot iOSlot = root.outputs[i];
			if (iOSlot.type == IOType.Electric)
			{
				IOEntity iOEntity = iOSlot.connectedTo.Get();
				if ((Object)(object)iOEntity != (Object)null && !listToUse.Contains((iOEntity, iOSlot.connectedToSlot)) && iOEntity.WantsPower(iOSlot.connectedToSlot))
				{
					AddConnectedRecursive(iOEntity, iOSlot.connectedToSlot, ref listToUse);
				}
			}
		}
	}

	public int GetDrain()
	{
		connectedList.Clear();
		IOEntity iOEntity = outputs[0].connectedTo.Get();
		if ((Object)(object)iOEntity != (Object)null)
		{
			int connectedToSlot = outputs[0].connectedToSlot;
			if (iOEntity.WantsPower(connectedToSlot))
			{
				AddConnectedRecursive(iOEntity, connectedToSlot, ref connectedList);
			}
			else
			{
				connectedList.Add((iOEntity, connectedToSlot));
			}
		}
		int num = 0;
		foreach (var connected in connectedList)
		{
			if (connected.entity.ShouldDrainBattery(this))
			{
				num += connected.entity.DesiredPower(connected.inputIndex);
				if (num >= maxOutput)
				{
					num = maxOutput;
					break;
				}
			}
		}
		return num;
	}

	public override void OnCircuitChanged(bool forceUpdate)
	{
		base.OnCircuitChanged(forceUpdate);
		int drain = GetDrain();
		activeDrain = drain;
	}

	public void CheckDischarge()
	{
		if (rustWattSeconds < 5f)
		{
			SetDischarging(wantsOn: false);
			return;
		}
		IOEntity iOEntity = outputs[0].connectedTo.Get();
		int drain = GetDrain();
		activeDrain = drain;
		SetDischarging((Object)(object)iOEntity != (Object)null);
	}

	public void SetDischarging(bool wantsOn)
	{
		SetPassthroughOn(wantsOn);
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!IsOn())
		{
			return 0;
		}
		return Mathf.FloorToInt((float)maxOutput * ((rustWattSeconds >= 1f) ? 1f : 0f));
	}

	public override bool WantsPower(int inputIndex)
	{
		return rustWattSeconds < maxCapactiySeconds;
	}

	private int GetHighestInputFromHistory()
	{
		int num = 0;
		foreach (int item in inputHistory)
		{
			if (item > num)
			{
				num = item;
			}
		}
		return num;
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (IsFlickering())
		{
			if (inputHistory.Count >= 5)
			{
				inputHistory.Dequeue();
			}
			inputHistory.Enqueue(inputAmount);
		}
		if (inputSlot == 0 && rechargable)
		{
			if (!IsPowered() && !IsFlickering())
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)AddCharge);
			}
			else if (!((FacepunchBehaviour)this).IsInvoking((Action)AddCharge))
			{
				((FacepunchBehaviour)this).InvokeRandomized((Action)AddCharge, 1f, 1f, 0.1f);
			}
		}
	}

	public void TickUsage()
	{
		float oldCharge = rustWattSeconds;
		bool num = rustWattSeconds > 0f;
		if (rustWattSeconds >= 1f)
		{
			float num2 = 1f * (float)activeDrain;
			rustWattSeconds -= num2;
		}
		if (rustWattSeconds <= 0f)
		{
			rustWattSeconds = 0f;
		}
		bool flag = rustWattSeconds > 0f;
		ChargeChanged(oldCharge);
		if (num != flag)
		{
			MarkDirty();
			SendNetworkUpdate();
		}
	}

	public virtual void ChargeChanged(float oldCharge)
	{
		_ = rustWattSeconds;
		bool flag = rustWattSeconds > maxCapactiySeconds * 0.25f;
		bool flag2 = rustWattSeconds > maxCapactiySeconds * 0.75f;
		if (HasFlag(Flags.Reserved5) != flag || HasFlag(Flags.Reserved6) != flag2)
		{
			SetFlag(Flags.Reserved5, flag, recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved6, flag2, recursive: false, networkupdate: false);
			SendNetworkUpdate();
		}
	}

	public void AddCharge()
	{
		float oldCharge = rustWattSeconds;
		float num = (float)Mathf.Min(IsFlickering() ? GetHighestInputFromHistory() : currentEnergy, DesiredPower()) * 1f * chargeRatio;
		if (num > 0f)
		{
			rustWattSeconds += num;
			rustWattSeconds = Mathf.Clamp(rustWattSeconds, 0f, maxCapactiySeconds);
			ChargeChanged(oldCharge);
		}
	}

	public void SetPassthroughOn(bool wantsOn)
	{
		if (wantsOn == IsOn() && !wasLoaded)
		{
			return;
		}
		wasLoaded = false;
		SetFlag(Flags.On, wantsOn);
		if (IsOn())
		{
			if (!((FacepunchBehaviour)this).IsInvoking((Action)TickUsage))
			{
				((FacepunchBehaviour)this).InvokeRandomized((Action)TickUsage, 1f, 1f, 0.1f);
			}
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)TickUsage);
		}
		MarkDirty();
	}

	public void Unbusy()
	{
		SetFlag(Flags.Busy, b: false);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<IOEntity>();
		}
		info.msg.ioEntity.genericFloat1 = rustWattSeconds;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			rustWattSeconds = info.msg.ioEntity.genericFloat1;
		}
	}

	[ServerVar]
	public static void batteryid(Arg arg)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		ElectricBattery electricBattery = BaseNetworkable.serverEntities.Find(arg.GetEntityID(1)) as ElectricBattery;
		if ((Object)(object)electricBattery == (Object)null)
		{
			arg.ReplyWith("Not a battery");
			return;
		}
		string @string = arg.GetString(0, "");
		if (!(@string == "charge"))
		{
			if (@string == "deplete")
			{
				electricBattery.rustWattSeconds = 0f;
				arg.ReplyWith("Depleted " + electricBattery.GetDisplayName());
			}
			else
			{
				arg.ReplyWith("Unknown command");
			}
		}
		else
		{
			float num = arg.GetInt(2, (int)electricBattery.maxCapactiySeconds / 60);
			electricBattery.rustWattSeconds = Mathf.Clamp(electricBattery.rustWattSeconds + num * 60f, 0f, electricBattery.maxCapactiySeconds);
			arg.ReplyWith("Charged " + electricBattery.GetDisplayName());
		}
	}
}
