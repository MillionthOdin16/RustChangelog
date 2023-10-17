using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class PlanterBox : StorageContainer, ISplashable
{
	public int soilSaturation;

	public int soilSaturationMax = 8000;

	public MeshRenderer soilRenderer;

	private static readonly float MinimumSaturationTriggerLevel = ConVar.Server.optimalPlanterQualitySaturation - 0.2f;

	private static readonly float MaximumSaturationTriggerLevel = ConVar.Server.optimalPlanterQualitySaturation + 0.1f;

	private TimeCachedValue<float> sunExposure;

	private TimeCachedValue<float> artificialLightExposure;

	private TimeCachedValue<float> plantTemperature;

	private TimeCachedValue<float> plantArtificalTemperature;

	private TimeSince lastSplashNetworkUpdate;

	private TimeSince lastRainCheck;

	public float soilSaturationFraction => (float)soilSaturation / (float)soilSaturationMax;

	public int availableIdealWaterCapacity => Mathf.Max(availableIdealWaterCapacity, Mathf.Max(idealSaturation - soilSaturation, 0));

	public int availableWaterCapacity => soilSaturationMax - soilSaturation;

	public int idealSaturation => Mathf.FloorToInt((float)soilSaturationMax * ConVar.Server.optimalPlanterQualitySaturation);

	public bool BelowMinimumSaturationTriggerLevel => soilSaturationFraction < MinimumSaturationTriggerLevel;

	public bool AboveMaximumSaturationTriggerLevel => soilSaturationFraction > MaximumSaturationTriggerLevel;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PlanterBox.OnRpcMessage", 0);
		try
		{
			if (rpc == 2965786167u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RequestSaturationUpdate "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_RequestSaturationUpdate", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2965786167u, "RPC_RequestSaturationUpdate", this, player, 3f))
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
							RPC_RequestSaturationUpdate(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_RequestSaturationUpdate");
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

	public override void ServerInit()
	{
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		base.inventory.onItemAddedRemoved = OnItemAddedOrRemoved;
		base.inventory.SetOnlyAllowedItem(allowedItem);
		ItemContainer itemContainer = base.inventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(InventoryItemFilter));
		sunExposure = new TimeCachedValue<float>
		{
			refreshCooldown = 30f,
			refreshRandomRange = 5f,
			updateValue = CalculateSunExposure
		};
		artificialLightExposure = new TimeCachedValue<float>
		{
			refreshCooldown = 60f,
			refreshRandomRange = 5f,
			updateValue = CalculateArtificialLightExposure
		};
		plantTemperature = new TimeCachedValue<float>
		{
			refreshCooldown = 20f,
			refreshRandomRange = 5f,
			updateValue = CalculatePlantTemperature
		};
		plantArtificalTemperature = new TimeCachedValue<float>
		{
			refreshCooldown = 60f,
			refreshRandomRange = 5f,
			updateValue = CalculateArtificialTemperature
		};
		lastRainCheck = TimeSince.op_Implicit(0f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)CalculateRainFactor, 20f, 30f, 15f);
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		if (added && ItemIsFertilizer(item))
		{
			FertilizeGrowables();
		}
	}

	public bool InventoryItemFilter(Item item, int targetSlot)
	{
		if (item == null)
		{
			return false;
		}
		if (ItemIsFertilizer(item))
		{
			return true;
		}
		return false;
	}

	private bool ItemIsFertilizer(Item item)
	{
		return item.info.shortname == "fertilizer";
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.resource = Pool.Get<BaseResource>();
		info.msg.resource.stage = soilSaturation;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource != null)
		{
			soilSaturation = info.msg.resource.stage;
		}
	}

	public void FertilizeGrowables()
	{
		int num = GetFertilizerCount();
		if (num <= 0)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			if ((Object)(object)child == (Object)null)
			{
				continue;
			}
			GrowableEntity growableEntity = child as GrowableEntity;
			if (!((Object)(object)growableEntity == (Object)null) && !growableEntity.Fertilized && ConsumeFertilizer())
			{
				growableEntity.Fertilize();
				num--;
				if (num == 0)
				{
					break;
				}
			}
		}
	}

	public int GetFertilizerCount()
	{
		int num = 0;
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot != null && ItemIsFertilizer(slot))
			{
				num += slot.amount;
			}
		}
		return num;
	}

	public bool ConsumeFertilizer()
	{
		for (int i = 0; i < base.inventory.capacity; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot != null && ItemIsFertilizer(slot))
			{
				int num = Mathf.Min(1, slot.amount);
				if (num > 0)
				{
					slot.UseItem(num);
					return true;
				}
			}
		}
		return false;
	}

	public int ConsumeWater(int amount, GrowableEntity ignoreEntity = null)
	{
		int num = Mathf.Min(amount, soilSaturation);
		soilSaturation -= num;
		RefreshGrowables(ignoreEntity);
		SendNetworkUpdate();
		return num;
	}

	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		if (base.IsDestroyed)
		{
			return false;
		}
		if ((Object)(object)splashType == (Object)null || splashType.shortname == null)
		{
			return false;
		}
		if (!(splashType.shortname == "water.salt"))
		{
			return soilSaturation < soilSaturationMax;
		}
		return true;
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (splashType.shortname == "water.salt")
		{
			soilSaturation = 0;
			RefreshGrowables();
			if (TimeSince.op_Implicit(lastSplashNetworkUpdate) > 60f)
			{
				SendNetworkUpdate();
				lastSplashNetworkUpdate = TimeSince.op_Implicit(0f);
			}
			return amount;
		}
		int num = Mathf.Min(availableWaterCapacity, amount);
		soilSaturation += num;
		RefreshGrowables();
		if (TimeSince.op_Implicit(lastSplashNetworkUpdate) > 60f)
		{
			SendNetworkUpdate();
			lastSplashNetworkUpdate = TimeSince.op_Implicit(0f);
		}
		return num;
	}

	private void RefreshGrowables(GrowableEntity ignoreEntity = null)
	{
		if (children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			if (!((Object)(object)child == (Object)null) && !((Object)(object)child == (Object)(object)ignoreEntity) && child is GrowableEntity growableEntity)
			{
				growableEntity.QueueForQualityUpdate();
			}
		}
	}

	public void ForceLightUpdate()
	{
		sunExposure?.ForceNextRun();
		artificialLightExposure?.ForceNextRun();
	}

	public void ForceTemperatureUpdate()
	{
		plantArtificalTemperature?.ForceNextRun();
	}

	public float GetSunExposure()
	{
		return sunExposure?.Get(force: false) ?? 0f;
	}

	private float CalculateSunExposure()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return GrowableEntity.SunRaycast(((Component)this).transform.position + new Vector3(0f, 1f, 0f));
	}

	public float GetArtificialLightExposure()
	{
		return artificialLightExposure?.Get(force: false) ?? 0f;
	}

	private float CalculateArtificialLightExposure()
	{
		return GrowableEntity.CalculateArtificialLightExposure(((Component)this).transform);
	}

	public float GetPlantTemperature()
	{
		return (plantTemperature?.Get(force: false) ?? 0f) + (plantArtificalTemperature?.Get(force: false) ?? 0f);
	}

	private float CalculatePlantTemperature()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Max(Climate.GetTemperature(((Component)this).transform.position), 15f);
	}

	private void CalculateRainFactor()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (sunExposure.Get(force: false) > 0f)
		{
			float rain = Climate.GetRain(((Component)this).transform.position);
			if (rain > 0f)
			{
				soilSaturation = Mathf.Clamp(soilSaturation + Mathf.RoundToInt(4f * rain * TimeSince.op_Implicit(lastRainCheck)), 0, soilSaturationMax);
				RefreshGrowables();
				SendNetworkUpdate();
			}
		}
		lastRainCheck = TimeSince.op_Implicit(0f);
	}

	private float CalculateArtificialTemperature()
	{
		return GrowableEntity.CalculateArtificialTemperature(((Component)this).transform);
	}

	public void OnPlantInserted(GrowableEntity entity, BasePlayer byPlayer)
	{
		if (!GameInfo.HasAchievements)
		{
			return;
		}
		List<uint> list = Pool.GetList<uint>();
		foreach (BaseEntity child in children)
		{
			if (child is GrowableEntity growableEntity && !list.Contains(growableEntity.prefabID))
			{
				list.Add(growableEntity.prefabID);
			}
		}
		if (list.Count == 9)
		{
			byPlayer.GiveAchievement("HONEST_WORK");
		}
		Pool.FreeList<uint>(ref list);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_RequestSaturationUpdate(RPCMessage msg)
	{
		if ((Object)(object)msg.player != (Object)null)
		{
			ClientRPCPlayer(null, msg.player, "RPC_ReceiveSaturationUpdate", soilSaturation);
		}
	}

	public override bool SupportsChildDeployables()
	{
		return true;
	}
}
