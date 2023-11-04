using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

public class PlayerMetabolism : BaseMetabolism<BasePlayer>
{
	public const float HotThreshold = 40f;

	public const float ColdThreshold = 5f;

	public const float OxygenHurtThreshold = 0.5f;

	public const float OxygenDepleteTime = 10f;

	public const float OxygenRefillTime = 1f;

	public MetabolismAttribute temperature = new MetabolismAttribute();

	public MetabolismAttribute poison = new MetabolismAttribute();

	public MetabolismAttribute radiation_level = new MetabolismAttribute();

	public MetabolismAttribute radiation_poison = new MetabolismAttribute();

	public MetabolismAttribute wetness = new MetabolismAttribute();

	public MetabolismAttribute dirtyness = new MetabolismAttribute();

	public MetabolismAttribute oxygen = new MetabolismAttribute();

	public MetabolismAttribute bleeding = new MetabolismAttribute();

	public MetabolismAttribute comfort = new MetabolismAttribute();

	public MetabolismAttribute pending_health = new MetabolismAttribute();

	public bool isDirty = false;

	private float lastConsumeTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PlayerMetabolism.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void Reset()
	{
		base.Reset();
		poison.Reset();
		radiation_level.Reset();
		radiation_poison.Reset();
		temperature.Reset();
		oxygen.Reset();
		bleeding.Reset();
		wetness.Reset();
		dirtyness.Reset();
		comfort.Reset();
		pending_health.Reset();
		lastConsumeTime = float.NegativeInfinity;
		isDirty = true;
	}

	public override void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
	{
		base.ServerUpdate(ownerEntity, delta);
		Profiler.BeginSample("SendChangesToClient");
		SendChangesToClient();
		Profiler.EndSample();
	}

	internal bool HasChanged()
	{
		bool flag = isDirty;
		flag = calories.HasChanged() || flag;
		flag = hydration.HasChanged() || flag;
		flag = heartrate.HasChanged() || flag;
		flag = poison.HasChanged() || flag;
		flag = radiation_level.HasChanged() || flag;
		flag = radiation_poison.HasChanged() || flag;
		flag = temperature.HasChanged() || flag;
		flag = wetness.HasChanged() || flag;
		flag = dirtyness.HasChanged() || flag;
		flag = comfort.HasChanged() || flag;
		return pending_health.HasChanged() || flag;
	}

	protected override void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
	{
		base.DoMetabolismDamage(ownerEntity, delta);
		if (temperature.value < -20f)
		{
			owner.Hurt(Mathf.InverseLerp(1f, -50f, temperature.value) * delta * 1f, DamageType.Cold);
		}
		else if (temperature.value < -10f)
		{
			owner.Hurt(Mathf.InverseLerp(1f, -50f, temperature.value) * delta * 0.3f, DamageType.Cold);
		}
		else if (temperature.value < 1f)
		{
			owner.Hurt(Mathf.InverseLerp(1f, -50f, temperature.value) * delta * 0.1f, DamageType.Cold);
		}
		if (temperature.value > 60f)
		{
			owner.Hurt(Mathf.InverseLerp(60f, 200f, temperature.value) * delta * 5f, DamageType.Heat);
		}
		if (oxygen.value < 0.5f)
		{
			owner.Hurt(Mathf.InverseLerp(0.5f, 0f, oxygen.value) * delta * 20f, DamageType.Drowned, null, useProtection: false);
		}
		if (bleeding.value > 0f)
		{
			float num = delta * (1f / 3f);
			owner.Hurt(num, DamageType.Bleeding);
			bleeding.Subtract(num);
		}
		if (poison.value > 0f)
		{
			owner.Hurt(poison.value * delta * 0.1f, DamageType.Poison);
		}
		if (ConVar.Server.radiation && radiation_poison.value > 0f)
		{
			float num2 = (1f + Mathf.Clamp01(radiation_poison.value / 25f) * 5f) * (delta / 5f);
			owner.Hurt(num2, DamageType.Radiation);
			radiation_poison.Subtract(num2);
		}
	}

	public bool SignificantBleeding()
	{
		return bleeding.value > 0f;
	}

	protected override void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
	{
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		Profiler.BeginSample("Temperature");
		float currentTemperature = owner.currentTemperature;
		float fTarget = owner.currentComfort;
		Profiler.EndSample();
		Profiler.BeginSample("Workbench");
		float currentCraftLevel = owner.currentCraftLevel;
		owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench1, currentCraftLevel == 1f);
		owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench2, currentCraftLevel == 2f);
		owner.SetPlayerFlag(BasePlayer.PlayerFlags.Workbench3, currentCraftLevel == 3f);
		owner.SetPlayerFlag(BasePlayer.PlayerFlags.SafeZone, owner.InSafeZone());
		Profiler.EndSample();
		if ((Object)(object)activeGameMode == (Object)null || activeGameMode.allowTemperature)
		{
			float num = currentTemperature;
			num -= DeltaWet() * 34f;
			float num2 = Mathf.Clamp(owner.baseProtection.amounts[18] * 1.5f, -1f, 1f);
			float num3 = num2;
			float num4 = Mathf.InverseLerp(20f, -50f, currentTemperature);
			float num5 = Mathf.InverseLerp(20f, 30f, currentTemperature);
			num += num4 * 70f * num3;
			num += num5 * 10f * Mathf.Abs(num3);
			num += heartrate.value * 5f;
			temperature.MoveTowards(num, delta * 5f);
		}
		else
		{
			temperature.value = 25f;
		}
		if (temperature.value >= 40f)
		{
			fTarget = 0f;
		}
		comfort.MoveTowards(fTarget, delta / 5f);
		float num6 = 0.6f + 0.4f * comfort.value;
		if (calories.value > 100f && owner.healthFraction < num6 && radiation_poison.Fraction() < 0.25f && owner.SecondsSinceAttacked > 10f && !SignificantBleeding() && temperature.value >= 10f && hydration.value > 40f)
		{
			float num7 = Mathf.InverseLerp(calories.min, calories.max, calories.value);
			float num8 = 5f;
			float num9 = num8 * owner.MaxHealth() * 0.8f / 600f;
			num9 += num9 * num7 * 0.5f;
			float num10 = num9 / num8;
			num10 += num10 * comfort.value * 6f;
			ownerEntity.Heal(num10 * delta);
			calories.Subtract(num9 * delta);
			hydration.Subtract(num9 * delta * 0.2f);
		}
		float num11 = owner.estimatedSpeed2D / owner.GetMaxSpeed() * 0.75f;
		float fTarget2 = Mathf.Clamp(0.05f + num11, 0f, 1f);
		heartrate.MoveTowards(fTarget2, delta * 0.1f);
		if (!owner.IsGod())
		{
			float num12 = heartrate.Fraction() * 0.375f;
			calories.MoveTowards(0f, delta * num12);
			float num13 = 1f / 120f;
			num13 += Mathf.InverseLerp(40f, 60f, temperature.value) * (1f / 12f);
			num13 += heartrate.value * (1f / 15f);
			hydration.MoveTowards(0f, delta * num13);
		}
		bool b = hydration.Fraction() <= 0f || radiation_poison.value >= 100f;
		owner.SetPlayerFlag(BasePlayer.PlayerFlags.NoSprint, b);
		if (temperature.value > 40f)
		{
			hydration.Add(Mathf.InverseLerp(40f, 200f, temperature.value) * delta * -1f);
		}
		if (temperature.value < 10f)
		{
			float num14 = Mathf.InverseLerp(20f, -100f, temperature.value);
			heartrate.MoveTowards(Mathf.Lerp(0.2f, 1f, num14), delta * 2f * num14);
		}
		float num15 = owner.AirFactor();
		float num16 = ((num15 > oxygen.value) ? 1f : 0.1f);
		oxygen.MoveTowards(num15, delta * num16);
		float num17 = 0f;
		float num18 = 0f;
		Profiler.BeginSample("Weather");
		if (owner.IsOutside(owner.eyes.position))
		{
			num17 = Climate.GetRain(owner.eyes.position) * Weather.wetness_rain;
			num18 = Climate.GetSnow(owner.eyes.position) * Weather.wetness_snow;
		}
		Profiler.EndSample();
		bool flag = owner.baseProtection.amounts[4] > 0f;
		float currentEnvironmentalWetness = owner.currentEnvironmentalWetness;
		currentEnvironmentalWetness = Mathf.Clamp(currentEnvironmentalWetness, 0f, 0.8f);
		float num19 = owner.WaterFactor();
		if (!flag && num19 > 0f)
		{
			wetness.value = Mathf.Max(wetness.value, Mathf.Clamp(num19, wetness.min, wetness.max));
		}
		float num20 = Mathx.Max(wetness.value, num17, num18, currentEnvironmentalWetness);
		num20 = Mathf.Min(num20, flag ? 0f : num20);
		wetness.MoveTowards(num20, delta * 0.05f);
		if (num19 < wetness.value && currentEnvironmentalWetness <= 0f)
		{
			wetness.MoveTowards(0f, delta * 0.2f * Mathf.InverseLerp(0f, 100f, currentTemperature));
		}
		poison.MoveTowards(0f, delta * (5f / 9f));
		if (wetness.Fraction() > 0.4f && owner.estimatedSpeed > 0.25f && radiation_level.Fraction() == 0f)
		{
			radiation_poison.Subtract(radiation_poison.value * 0.2f * wetness.Fraction() * delta * 0.2f);
		}
		if (ConVar.Server.radiation && !owner.IsGod())
		{
			radiation_level.value = owner.radiationLevel;
			if (radiation_level.value > 0f)
			{
				radiation_poison.Add(radiation_level.value * delta);
			}
		}
		if (pending_health.value > 0f)
		{
			float num21 = Mathf.Min(1f * delta, pending_health.value);
			ownerEntity.Heal(num21);
			if (ownerEntity.healthFraction == 1f)
			{
				pending_health.value = 0f;
			}
			else
			{
				pending_health.Subtract(num21);
			}
		}
	}

	private float DeltaHot()
	{
		return Mathf.InverseLerp(20f, 100f, temperature.value);
	}

	private float DeltaCold()
	{
		return Mathf.InverseLerp(20f, -50f, temperature.value);
	}

	private float DeltaWet()
	{
		return wetness.value;
	}

	public void UseHeart(float frate)
	{
		if (heartrate.value > frate)
		{
			heartrate.Add(frate);
		}
		else
		{
			heartrate.value = frate;
		}
	}

	public void SendChangesToClient()
	{
		if (!HasChanged())
		{
			return;
		}
		isDirty = false;
		PlayerMetabolism val = Save();
		try
		{
			base.baseEntity.ClientRPCPlayerAndSpectators<PlayerMetabolism>(null, base.baseEntity, "UpdateMetabolism", val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public bool CanConsume()
	{
		if (Object.op_Implicit((Object)(object)owner) && owner.IsHeadUnderwater())
		{
			return false;
		}
		return Time.time - lastConsumeTime > 1f;
	}

	public void MarkConsumption()
	{
		lastConsumeTime = Time.time;
	}

	public PlayerMetabolism Save()
	{
		PlayerMetabolism val = Pool.Get<PlayerMetabolism>();
		val.calories = calories.value;
		val.hydration = hydration.value;
		val.heartrate = heartrate.value;
		val.temperature = temperature.value;
		val.radiation_level = radiation_level.value;
		val.radiation_poisoning = radiation_poison.value;
		val.wetness = wetness.value;
		val.dirtyness = dirtyness.value;
		val.oxygen = oxygen.value;
		val.bleeding = bleeding.value;
		val.comfort = comfort.value;
		val.pending_health = pending_health.value;
		if (Object.op_Implicit((Object)(object)owner))
		{
			val.health = owner.Health();
		}
		return val;
	}

	public void Load(PlayerMetabolism s)
	{
		calories.SetValue(s.calories);
		hydration.SetValue(s.hydration);
		comfort.SetValue(s.comfort);
		heartrate.value = s.heartrate;
		temperature.value = s.temperature;
		radiation_level.value = s.radiation_level;
		radiation_poison.value = s.radiation_poisoning;
		wetness.value = s.wetness;
		dirtyness.value = s.dirtyness;
		oxygen.value = s.oxygen;
		bleeding.value = s.bleeding;
		pending_health.value = s.pending_health;
		if (Object.op_Implicit((Object)(object)owner))
		{
			owner.health = s.health;
		}
	}

	public override MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		return type switch
		{
			MetabolismAttribute.Type.Poison => poison, 
			MetabolismAttribute.Type.Bleeding => bleeding, 
			MetabolismAttribute.Type.Radiation => radiation_poison, 
			MetabolismAttribute.Type.HealthOverTime => pending_health, 
			_ => base.FindAttribute(type), 
		};
	}
}
