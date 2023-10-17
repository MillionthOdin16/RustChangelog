using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public class Sprinkler : IOEntity
{
	public float SplashFrequency = 1f;

	public Transform Eyes = null;

	public int WaterPerSplash = 1;

	public float DecayPerSplash = 0.8f;

	private ItemDefinition currentFuelType;

	private IOEntity currentFuelSource;

	private HashSet<ISplashable> cachedSplashables = new HashSet<ISplashable>();

	private TimeSince updateSplashableCache;

	private bool forceUpdateSplashables = false;

	public override bool BlockFluidDraining => (Object)(object)currentFuelSource != (Object)null;

	public override int ConsumptionAmount()
	{
		return 2;
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		SetSprinklerState(inputAmount > 0);
	}

	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		return inputAmount;
	}

	private void DoSplash()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("SprinklerSplash", 0);
		try
		{
			int num = WaterPerSplash;
			if (TimeSince.op_Implicit(updateSplashableCache) > SplashFrequency * 4f || forceUpdateSplashables)
			{
				Profiler.BeginSample("UpdateCachedSplashables");
				cachedSplashables.Clear();
				forceUpdateSplashables = false;
				updateSplashableCache = TimeSince.op_Implicit(0f);
				Vector3 position = Eyes.position;
				Vector3 up = ((Component)this).transform.up;
				float sprinklerEyeHeightOffset = Server.sprinklerEyeHeightOffset;
				float num2 = Vector3.Angle(up, Vector3.up) / 180f;
				num2 = Mathf.Clamp(num2, 0.2f, 1f);
				sprinklerEyeHeightOffset *= num2;
				Vector3 startPosition = position + up * (Server.sprinklerRadius * 0.5f);
				Vector3 endPosition = position + up * sprinklerEyeHeightOffset;
				List<BaseEntity> list = Pool.GetList<BaseEntity>();
				Vis.Entities(startPosition, endPosition, Server.sprinklerRadius, list, 1237003025, (QueryTriggerInteraction)2);
				if (list.Count > 0)
				{
					foreach (BaseEntity item in list)
					{
						if (!item.isClient && item is ISplashable splashable && !cachedSplashables.Contains(splashable) && splashable.WantsSplash(currentFuelType, num) && item.IsVisible(position) && (!(item is IOEntity entity) || !IsConnectedTo(entity, IOEntity.backtracking)))
						{
							cachedSplashables.Add(splashable);
						}
					}
				}
				Pool.FreeList<BaseEntity>(ref list);
				Profiler.EndSample();
			}
			else
			{
				Profiler.BeginSample("UseCachedSplashables");
				Profiler.EndSample();
			}
			if (cachedSplashables.Count > 0)
			{
				int amount = num / cachedSplashables.Count;
				foreach (ISplashable cachedSplashable in cachedSplashables)
				{
					Profiler.BeginSample("CheckCanSplash");
					if (cachedSplashable.IsUnityNull() || !cachedSplashable.WantsSplash(currentFuelType, amount))
					{
						Profiler.EndSample();
						continue;
					}
					Profiler.EndSample();
					Profiler.BeginSample("ApplySplash");
					int num3 = cachedSplashable.DoSplash(currentFuelType, amount);
					Profiler.EndSample();
					num -= num3;
					if (num > 0)
					{
						continue;
					}
					break;
				}
			}
			if (DecayPerSplash > 0f)
			{
				Hurt(DecayPerSplash);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void SetSprinklerState(bool wantsOn)
	{
		if (wantsOn)
		{
			TurnOn();
		}
		else
		{
			TurnOff();
		}
	}

	public void TurnOn()
	{
		if (!IsOn())
		{
			SetFlag(Flags.On, b: true);
			forceUpdateSplashables = true;
			if (!((FacepunchBehaviour)this).IsInvoking((Action)DoSplash))
			{
				((FacepunchBehaviour)this).InvokeRandomized((Action)DoSplash, SplashFrequency * 0.5f, SplashFrequency, SplashFrequency * 0.2f);
			}
		}
	}

	public void TurnOff()
	{
		if (IsOn())
		{
			SetFlag(Flags.On, b: false);
			if (((FacepunchBehaviour)this).IsInvoking((Action)DoSplash))
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)DoSplash);
			}
			currentFuelSource = null;
			currentFuelType = null;
		}
	}

	public override void SetFuelType(ItemDefinition def, IOEntity source)
	{
		base.SetFuelType(def, source);
		currentFuelType = def;
		currentFuelSource = source;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			SetFlag(Flags.On, b: false, recursive: false, networkupdate: false);
		}
	}
}
