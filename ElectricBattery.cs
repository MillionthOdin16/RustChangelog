using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Profiling;

public class ElectricBattery : IOEntity, IInstanceDataReceiver
{
	public int maxOutput;

	public float maxCapactiySeconds;

	public float rustWattSeconds;

	private int activeDrain = 0;

	public bool rechargable;

	[Tooltip("How much energy we can request from power sources for charging is this value * our maxOutput")]
	public float maximumInboundEnergyRatio = 4f;

	public float chargeRatio = 0.25f;

	private const float tickRateSeconds = 1f;

	public const Flags Flag_HalfFull = Flags.Reserved5;

	public const Flags Flag_VeryFull = Flags.Reserved6;

	private bool wasLoaded = false;

	private HashSet<IOEntity> connectedList = new HashSet<IOEntity>();

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
		return IsOn() ? activeDrain : 0;
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
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
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

	public override int DesiredPower()
	{
		if (rustWattSeconds >= maxCapactiySeconds)
		{
			return 0;
		}
		return Mathf.FloorToInt((float)maxOutput * maximumInboundEnergyRatio);
	}

	public override void SendAdditionalData(BasePlayer player, int slot, bool input)
	{
		int passthroughAmountForAnySlot = GetPassthroughAmountForAnySlot(slot, input);
		ClientRPCPlayer((Connection)null, player, "Client_ReceiveAdditionalData", currentEnergy, passthroughAmountForAnySlot, rustWattSeconds, (float)activeDrain);
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

	public void AddConnectedRecursive(IOEntity root, ref HashSet<IOEntity> listToUse)
	{
		Profiler.BeginSample("AddConnectedRecursive");
		listToUse.Add(root);
		if (root.WantsPassthroughPower())
		{
			for (int i = 0; i < root.outputs.Length; i++)
			{
				if (!root.AllowDrainFrom(i))
				{
					continue;
				}
				IOSlot iOSlot = root.outputs[i];
				if (iOSlot.type != 0)
				{
					continue;
				}
				IOEntity iOEntity = iOSlot.connectedTo.Get();
				if (!((Object)(object)iOEntity != (Object)null))
				{
					continue;
				}
				bool flag = iOEntity.WantsPower();
				if (!listToUse.Contains(iOEntity))
				{
					if (flag)
					{
						AddConnectedRecursive(iOEntity, ref listToUse);
					}
					else
					{
						listToUse.Add(iOEntity);
					}
				}
			}
		}
		Profiler.EndSample();
	}

	public int GetDrain()
	{
		Profiler.BeginSample("GetDrain");
		connectedList.Clear();
		IOEntity iOEntity = outputs[0].connectedTo.Get();
		if (Object.op_Implicit((Object)(object)iOEntity))
		{
			AddConnectedRecursive(iOEntity, ref connectedList);
		}
		int num = 0;
		foreach (IOEntity connected in connectedList)
		{
			if (connected.ShouldDrainBattery(this))
			{
				num += connected.DesiredPower();
				if (num >= maxOutput)
				{
					num = maxOutput;
					break;
				}
			}
		}
		Profiler.EndSample();
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
		Profiler.BeginSample("ElectricBattery.CheckDischarge");
		IOEntity iOEntity = outputs[0].connectedTo.Get();
		int drain = GetDrain();
		activeDrain = drain;
		if (Object.op_Implicit((Object)(object)iOEntity))
		{
			SetDischarging(iOEntity.WantsPower());
		}
		else
		{
			SetDischarging(wantsOn: false);
		}
		Profiler.EndSample();
	}

	public void SetDischarging(bool wantsOn)
	{
		SetPassthroughOn(wantsOn);
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (IsOn())
		{
			return Mathf.FloorToInt((float)maxOutput * ((rustWattSeconds >= 1f) ? 1f : 0f));
		}
		return 0;
	}

	public override bool WantsPower()
	{
		return rustWattSeconds < maxCapactiySeconds;
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (inputSlot != 0)
		{
			return;
		}
		if (!IsPowered())
		{
			if (rechargable)
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)AddCharge);
			}
		}
		else if (rechargable && !((FacepunchBehaviour)this).IsInvoking((Action)AddCharge))
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)AddCharge, 1f, 1f, 0.1f);
		}
	}

	public void TickUsage()
	{
		float oldCharge = rustWattSeconds;
		Profiler.BeginSample("ElectricBattery.TickUsage");
		bool flag = rustWattSeconds > 0f;
		if (rustWattSeconds >= 1f)
		{
			float num = 1f * (float)activeDrain;
			rustWattSeconds -= num;
		}
		if (rustWattSeconds <= 0f)
		{
			rustWattSeconds = 0f;
		}
		bool flag2 = rustWattSeconds > 0f;
		ChargeChanged(oldCharge);
		if (flag != flag2)
		{
			MarkDirty();
			SendNetworkUpdate();
		}
		Profiler.EndSample();
	}

	public virtual void ChargeChanged(float oldCharge)
	{
		float num = rustWattSeconds;
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
		Profiler.BeginSample("ElectricBattery.TickUsage");
		float oldCharge = rustWattSeconds;
		int num = Mathf.Min(currentEnergy, DesiredPower());
		float num2 = (float)num * 1f * chargeRatio;
		rustWattSeconds += num2;
		rustWattSeconds = Mathf.Clamp(rustWattSeconds, 0f, maxCapactiySeconds);
		ChargeChanged(oldCharge);
		Profiler.EndSample();
	}

	public void SetPassthroughOn(bool wantsOn)
	{
		if (wantsOn == IsOn() && !wasLoaded)
		{
			return;
		}
		wasLoaded = false;
		Profiler.BeginSample("ElectricBattery.SetPassthroughOn");
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
		Profiler.EndSample();
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
}
