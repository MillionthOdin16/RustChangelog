using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

public class Sprinkler : IOEntity
{
	public float SplashFrequency = 1f;

	public Transform Eyes;

	public int WaterPerSplash = 1;

	public float DecayPerSplash = 0.8f;

	private ItemDefinition currentFuelType;

	private IOEntity currentFuelSource;

	private HashSet<ISplashable> cachedSplashables = new HashSet<ISplashable>();

	private TimeSince updateSplashableCache;

	private bool forceUpdateSplashables;

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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("SprinklerSplash", 0);
		try
		{
			int num = WaterPerSplash;
			if (TimeSince.op_Implicit(updateSplashableCache) > SplashFrequency * 4f || forceUpdateSplashables)
			{
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
			}
			if (cachedSplashables.Count > 0)
			{
				int num3 = num / cachedSplashables.Count;
				float num4 = (float)(num % cachedSplashables.Count) / (float)cachedSplashables.Count;
				foreach (ISplashable cachedSplashable in cachedSplashables)
				{
					int amount = num3 + ((Random.value < num4) ? 1 : 0);
					if (!cachedSplashable.IsUnityNull() && cachedSplashable.WantsSplash(currentFuelType, amount))
					{
						int num5 = cachedSplashable.DoSplash(currentFuelType, amount);
						num -= num5;
						if (num <= 0)
						{
							break;
						}
					}
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
