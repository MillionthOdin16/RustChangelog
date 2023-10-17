using System;
using System.Collections.Generic;
using System.IO;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseMelee : AttackEntity
{
	[Serializable]
	public class MaterialFX
	{
		public string materialName;

		public GameObjectRef fx;
	}

	[Header("Melee")]
	public DamageProperties damageProperties;

	public List<DamageTypeEntry> damageTypes;

	public float maxDistance = 1.5f;

	public float attackRadius = 0.3f;

	public bool isAutomatic = true;

	public bool blockSprintOnAttack = true;

	public bool canUntieCrates;

	[Header("Effects")]
	public GameObjectRef strikeFX;

	public bool useStandardHitEffects = true;

	[Header("NPCUsage")]
	public float aiStrikeDelay = 0.2f;

	public GameObjectRef swingEffect;

	public List<MaterialFX> materialStrikeFX = new List<MaterialFX>();

	[Header("Other")]
	[Range(0f, 1f)]
	public float heartStress = 0.5f;

	public ResourceDispenser.GatherProperties gathering;

	[Header("Throwing")]
	public bool canThrowAsProjectile;

	public bool canAiHearIt;

	public bool onlyThrowAsProjectile;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseMelee.OnRpcMessage", 0);
		try
		{
			if (rpc == 3168282921u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - CLProject "));
				}
				TimeWarning val2 = TimeWarning.New("CLProject", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(3168282921u, "CLProject", this, player))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(3168282921u, "CLProject", this, player))
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
							CLProject(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in CLProject");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4088326849u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - PlayerAttack "));
				}
				TimeWarning val2 = TimeWarning.New("PlayerAttack", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(4088326849u, "PlayerAttack", this, player))
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
							RPCMessage msg3 = rPCMessage;
							PlayerAttack(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in PlayerAttack");
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

	public override void GetAttackStats(HitInfo info)
	{
		info.damageTypes.Add(damageTypes);
		info.CanGather = gathering.Any();
	}

	public virtual void DoAttackShared(HitInfo info)
	{
		GetAttackStats(info);
		if ((Object)(object)info.HitEntity != (Object)null)
		{
			TimeWarning val = TimeWarning.New("OnAttacked", 50);
			try
			{
				info.HitEntity.OnAttacked(info);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (info.DoHitEffects)
		{
			if (base.isServer)
			{
				TimeWarning val = TimeWarning.New("ImpactEffect", 20);
				try
				{
					Effect.server.ImpactEffect(info);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
			else
			{
				TimeWarning val = TimeWarning.New("ImpactEffect", 20);
				try
				{
					Effect.client.ImpactEffect(info);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
			}
		}
		if (base.isServer && !base.IsDestroyed)
		{
			TimeWarning val = TimeWarning.New("UpdateItemCondition", 50);
			try
			{
				UpdateItemCondition(info);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			StartAttackCooldown(repeatDelay);
		}
	}

	public ResourceDispenser.GatherPropertyEntry GetGatherInfoFromIndex(ResourceDispenser.GatherType index)
	{
		return gathering.GetFromIndex(index);
	}

	public virtual bool CanHit(HitTest info)
	{
		return true;
	}

	public float TotalDamage()
	{
		float num = 0f;
		foreach (DamageTypeEntry damageType in damageTypes)
		{
			if (!(damageType.amount <= 0f))
			{
				num += damageType.amount;
			}
		}
		return num;
	}

	public bool IsItemBroken()
	{
		return GetOwnerItem()?.isBroken ?? true;
	}

	public void LoseCondition(float amount)
	{
		Item ownerItem = GetOwnerItem();
		if (ownerItem != null && !base.UsingInfiniteAmmoCheat)
		{
			ownerItem.LoseCondition(amount);
		}
	}

	public virtual float GetConditionLoss()
	{
		return 1f;
	}

	public void UpdateItemCondition(HitInfo info)
	{
		Item ownerItem = GetOwnerItem();
		if (ownerItem == null || !ownerItem.hasCondition || info == null || !info.DidHit || info.DidGather)
		{
			return;
		}
		float conditionLoss = GetConditionLoss();
		float num = 0f;
		foreach (DamageTypeEntry damageType in damageTypes)
		{
			if (!(damageType.amount <= 0f))
			{
				num += Mathf.Clamp(damageType.amount - info.damageTypes.Get(damageType.type), 0f, damageType.amount);
			}
		}
		conditionLoss += num * 0.2f;
		if (!base.UsingInfiniteAmmoCheat)
		{
			ownerItem.LoseCondition(conditionLoss);
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void PlayerAttack(RPCMessage msg)
	{
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_063c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_069b: Unknown result type (might be due to invalid IL or missing references)
		//IL_069d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06db: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0703: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08be: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0728: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_072f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_0759: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0906: Unknown result type (might be due to invalid IL or missing references)
		//IL_0908: Unknown result type (might be due to invalid IL or missing references)
		//IL_0978: Unknown result type (might be due to invalid IL or missing references)
		//IL_097a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0994: Unknown result type (might be due to invalid IL or missing references)
		//IL_0996: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_091b: Unknown result type (might be due to invalid IL or missing references)
		//IL_091d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_0812: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_084d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0869: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!VerifyClientAttack(player))
		{
			SendNetworkUpdate();
			return;
		}
		TimeWarning val = TimeWarning.New("PlayerAttack", 50);
		try
		{
			PlayerAttack val2 = PlayerAttack.Deserialize((Stream)(object)msg.read);
			try
			{
				if (val2 == null)
				{
					return;
				}
				HitInfo hitInfo = Pool.Get<HitInfo>();
				hitInfo.LoadFromAttack(val2.attack, serverSide: true);
				hitInfo.Initiator = player;
				hitInfo.Weapon = this;
				hitInfo.WeaponPrefab = this;
				hitInfo.Predicted = msg.connection;
				hitInfo.damageProperties = damageProperties;
				if (hitInfo.IsNaNOrInfinity())
				{
					string shortPrefabName = base.ShortPrefabName;
					AntiHack.Log(player, AntiHackType.MeleeHack, "Contains NaN (" + shortPrefabName + ")");
					player.stats.combat.LogInvalid(hitInfo, "melee_nan");
					return;
				}
				BaseEntity hitEntity = hitInfo.HitEntity;
				BasePlayer basePlayer = hitInfo.HitEntity as BasePlayer;
				bool flag = (Object)(object)basePlayer != (Object)null;
				bool flag2 = flag && basePlayer.IsSleeping();
				bool flag3 = flag && basePlayer.IsWounded();
				bool flag4 = flag && basePlayer.isMounted;
				bool flag5 = flag && basePlayer.HasParent();
				bool flag6 = (Object)(object)hitEntity != (Object)null;
				bool flag7 = flag6 && hitEntity.IsNpc;
				bool flag8;
				int num5;
				Vector3 val3;
				Vector3 center;
				Vector3 position;
				Vector3 pointStart;
				Vector3 hitPositionWorld;
				Vector3 val5;
				int num18;
				if (ConVar.AntiHack.melee_protection > 0)
				{
					flag8 = true;
					float num = 1f + ConVar.AntiHack.melee_forgiveness;
					float melee_clientframes = ConVar.AntiHack.melee_clientframes;
					float melee_serverframes = ConVar.AntiHack.melee_serverframes;
					float num2 = melee_clientframes / 60f;
					float num3 = melee_serverframes * Mathx.Max(Time.deltaTime, Time.smoothDeltaTime, Time.fixedDeltaTime);
					float num4 = (player.desyncTimeClamped + num2 + num3) * num;
					num5 = 2162688;
					if (ConVar.AntiHack.melee_terraincheck)
					{
						num5 |= 0x800000;
					}
					if (ConVar.AntiHack.melee_vehiclecheck)
					{
						num5 |= 0x8000000;
					}
					if (flag && hitInfo.boneArea == (HitArea)(-1))
					{
						string shortPrefabName2 = base.ShortPrefabName;
						string shortPrefabName3 = basePlayer.ShortPrefabName;
						AntiHack.Log(player, AntiHackType.MeleeHack, "Bone is invalid  (" + shortPrefabName2 + " on " + shortPrefabName3 + " bone " + hitInfo.HitBone + ")");
						player.stats.combat.LogInvalid(hitInfo, "melee_bone");
						flag8 = false;
					}
					if (ConVar.AntiHack.melee_protection >= 2)
					{
						if (flag6)
						{
							float num6 = hitEntity.MaxVelocity();
							val3 = hitEntity.GetParentVelocity();
							float num7 = num6 + ((Vector3)(ref val3)).magnitude;
							float num8 = hitEntity.BoundsPadding() + num4 * num7;
							float num9 = hitEntity.Distance(hitInfo.HitPositionWorld);
							if (num9 > num8)
							{
								string shortPrefabName4 = base.ShortPrefabName;
								string shortPrefabName5 = hitEntity.ShortPrefabName;
								AntiHack.Log(player, AntiHackType.MeleeHack, "Entity too far away (" + shortPrefabName4 + " on " + shortPrefabName5 + " with " + num9 + "m > " + num8 + "m in " + num4 + "s)");
								player.stats.combat.LogInvalid(hitInfo, "melee_target");
								flag8 = false;
							}
						}
						if (ConVar.AntiHack.melee_protection >= 4 && flag8 && flag && !flag7 && !flag2 && !flag3 && !flag4 && !flag5)
						{
							val3 = basePlayer.GetParentVelocity();
							float magnitude = ((Vector3)(ref val3)).magnitude;
							float num10 = basePlayer.BoundsPadding() + num4 * magnitude + ConVar.AntiHack.tickhistoryforgiveness;
							float num11 = basePlayer.tickHistory.Distance(basePlayer, hitInfo.HitPositionWorld);
							if (num11 > num10)
							{
								string shortPrefabName6 = base.ShortPrefabName;
								string shortPrefabName7 = basePlayer.ShortPrefabName;
								AntiHack.Log(player, AntiHackType.ProjectileHack, "Player too far away (" + shortPrefabName6 + " on " + shortPrefabName7 + " with " + num11 + "m > " + num10 + "m in " + num4 + "s)");
								player.stats.combat.LogInvalid(hitInfo, "player_distance");
								flag8 = false;
							}
						}
					}
					if (ConVar.AntiHack.melee_protection >= 1)
					{
						if (ConVar.AntiHack.melee_protection >= 4)
						{
							val3 = player.GetParentVelocity();
							float magnitude2 = ((Vector3)(ref val3)).magnitude;
							float num12 = player.BoundsPadding() + num4 * magnitude2 + num * maxDistance;
							float num13 = player.tickHistory.Distance(player, hitInfo.HitPositionWorld);
							if (num13 > num12)
							{
								string shortPrefabName8 = base.ShortPrefabName;
								string text = (flag6 ? hitEntity.ShortPrefabName : "world");
								AntiHack.Log(player, AntiHackType.MeleeHack, "Initiator too far away (" + shortPrefabName8 + " on " + text + " with " + num13 + "m > " + num12 + "m in " + num4 + "s)");
								player.stats.combat.LogInvalid(hitInfo, "melee_initiator");
								flag8 = false;
							}
						}
						else
						{
							float num14 = player.MaxVelocity();
							val3 = player.GetParentVelocity();
							float num15 = num14 + ((Vector3)(ref val3)).magnitude;
							float num16 = player.BoundsPadding() + num4 * num15 + num * maxDistance;
							float num17 = player.Distance(hitInfo.HitPositionWorld);
							if (num17 > num16)
							{
								string shortPrefabName9 = base.ShortPrefabName;
								string text2 = (flag6 ? hitEntity.ShortPrefabName : "world");
								AntiHack.Log(player, AntiHackType.MeleeHack, "Initiator too far away (" + shortPrefabName9 + " on " + text2 + " with " + num17 + "m > " + num16 + "m in " + num4 + "s)");
								player.stats.combat.LogInvalid(hitInfo, "melee_initiator");
								flag8 = false;
							}
						}
					}
					if (ConVar.AntiHack.melee_protection >= 3)
					{
						if (flag6)
						{
							center = player.eyes.center;
							position = player.eyes.position;
							pointStart = hitInfo.PointStart;
							hitPositionWorld = hitInfo.HitPositionWorld;
							Vector3 val4 = hitPositionWorld;
							val3 = hitPositionWorld - pointStart;
							hitPositionWorld = val4 - ((Vector3)(ref val3)).normalized * 0.001f;
							val5 = hitInfo.PositionOnRay(hitPositionWorld);
							Vector3 val6 = Vector3.zero;
							Vector3 val7 = Vector3.zero;
							Vector3 val8 = Vector3.zero;
							if (ConVar.AntiHack.melee_backtracking > 0f)
							{
								val3 = position - center;
								val6 = ((Vector3)(ref val3)).normalized * ConVar.AntiHack.melee_backtracking;
								val3 = pointStart - position;
								val7 = ((Vector3)(ref val3)).normalized * ConVar.AntiHack.melee_backtracking;
								val3 = val5 - pointStart;
								val8 = ((Vector3)(ref val3)).normalized * ConVar.AntiHack.melee_backtracking;
							}
							if (GamePhysics.LineOfSight(center - val6, position + val6, num5) && GamePhysics.LineOfSight(position - val7, pointStart + val7, num5) && GamePhysics.LineOfSight(pointStart - val8, val5, num5))
							{
								num18 = (GamePhysics.LineOfSight(val5, hitPositionWorld, num5) ? 1 : 0);
								if (num18 != 0)
								{
									player.stats.Add("hit_" + hitEntity.Categorize() + "_direct_los", 1, Stats.Server);
									goto IL_07b1;
								}
							}
							else
							{
								num18 = 0;
							}
							player.stats.Add("hit_" + hitEntity.Categorize() + "_indirect_los", 1, Stats.Server);
							goto IL_07b1;
						}
						goto IL_089e;
					}
					goto IL_0a04;
				}
				goto IL_0a16;
				IL_0a16:
				player.metabolism.UseHeart(heartStress * 0.2f);
				TimeWarning val9 = TimeWarning.New("DoAttackShared", 50);
				try
				{
					DoAttackShared(hitInfo);
					return;
				}
				finally
				{
					((IDisposable)val9)?.Dispose();
				}
				IL_0a04:
				if (!flag8)
				{
					AntiHack.AddViolation(player, AntiHackType.MeleeHack, ConVar.AntiHack.melee_penalty);
					return;
				}
				goto IL_0a16;
				IL_089e:
				if (flag8 && flag && !flag7)
				{
					Vector3 hitPositionWorld2 = hitInfo.HitPositionWorld;
					Vector3 position2 = basePlayer.eyes.position;
					Vector3 val10 = basePlayer.CenterPoint();
					float melee_losforgiveness = ConVar.AntiHack.melee_losforgiveness;
					bool flag9 = GamePhysics.LineOfSight(hitPositionWorld2, position2, num5, 0f, melee_losforgiveness) && GamePhysics.LineOfSight(position2, hitPositionWorld2, num5, melee_losforgiveness, 0f);
					if (!flag9)
					{
						flag9 = GamePhysics.LineOfSight(hitPositionWorld2, val10, num5, 0f, melee_losforgiveness) && GamePhysics.LineOfSight(val10, hitPositionWorld2, num5, melee_losforgiveness, 0f);
					}
					if (!flag9)
					{
						string shortPrefabName10 = base.ShortPrefabName;
						string shortPrefabName11 = basePlayer.ShortPrefabName;
						string[] obj = new string[12]
						{
							"Line of sight (", shortPrefabName10, " on ", shortPrefabName11, ") ", null, null, null, null, null,
							null, null
						};
						val3 = hitPositionWorld2;
						obj[5] = ((object)(Vector3)(ref val3)).ToString();
						obj[6] = " ";
						val3 = position2;
						obj[7] = ((object)(Vector3)(ref val3)).ToString();
						obj[8] = " or ";
						val3 = hitPositionWorld2;
						obj[9] = ((object)(Vector3)(ref val3)).ToString();
						obj[10] = " ";
						val3 = val10;
						obj[11] = ((object)(Vector3)(ref val3)).ToString();
						AntiHack.Log(player, AntiHackType.MeleeHack, string.Concat(obj));
						player.stats.combat.LogInvalid(hitInfo, "melee_los");
						flag8 = false;
					}
				}
				goto IL_0a04;
				IL_07b1:
				if (num18 == 0)
				{
					string shortPrefabName12 = base.ShortPrefabName;
					string shortPrefabName13 = hitEntity.ShortPrefabName;
					string[] obj2 = new string[14]
					{
						"Line of sight (", shortPrefabName12, " on ", shortPrefabName13, ") ", null, null, null, null, null,
						null, null, null, null
					};
					val3 = center;
					obj2[5] = ((object)(Vector3)(ref val3)).ToString();
					obj2[6] = " ";
					val3 = position;
					obj2[7] = ((object)(Vector3)(ref val3)).ToString();
					obj2[8] = " ";
					val3 = pointStart;
					obj2[9] = ((object)(Vector3)(ref val3)).ToString();
					obj2[10] = " ";
					val3 = val5;
					obj2[11] = ((object)(Vector3)(ref val3)).ToString();
					obj2[12] = " ";
					val3 = hitPositionWorld;
					obj2[13] = ((object)(Vector3)(ref val3)).ToString();
					AntiHack.Log(player, AntiHackType.MeleeHack, string.Concat(obj2));
					player.stats.combat.LogInvalid(hitInfo, "melee_los");
					flag8 = false;
				}
				goto IL_089e;
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override bool CanBeUsedInWater()
	{
		return true;
	}

	public virtual string GetStrikeEffectPath(string materialName)
	{
		for (int i = 0; i < materialStrikeFX.Count; i++)
		{
			if (materialStrikeFX[i].materialName == materialName && materialStrikeFX[i].fx.isValid)
			{
				return materialStrikeFX[i].fx.resourcePath;
			}
		}
		return strikeFX.resourcePath;
	}

	public override void ServerUse()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient || HasAttackCooldown())
		{
			return;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!((Object)(object)ownerPlayer == (Object)null))
		{
			StartAttackCooldown(repeatDelay * 2f);
			ownerPlayer.SignalBroadcast(Signal.Attack, string.Empty);
			if (swingEffect.isValid)
			{
				Effect.server.Run(swingEffect.resourcePath, ((Component)this).transform.position, Vector3.forward, ownerPlayer.net.connection);
			}
			if (((FacepunchBehaviour)this).IsInvoking((Action)ServerUse_Strike))
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)ServerUse_Strike);
			}
			((FacepunchBehaviour)this).Invoke((Action)ServerUse_Strike, aiStrikeDelay);
		}
	}

	public virtual void ServerUse_OnHit(HitInfo info)
	{
	}

	public void ServerUse_Strike()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)ownerPlayer == (Object)null)
		{
			return;
		}
		Vector3 position = ownerPlayer.eyes.position;
		Vector3 val = ownerPlayer.eyes.BodyForward();
		for (int i = 0; i < 2; i++)
		{
			List<RaycastHit> list = Pool.GetList<RaycastHit>();
			GamePhysics.TraceAll(new Ray(position - val * ((i == 0) ? 0f : 0.2f), val), (i == 0) ? 0f : attackRadius, list, effectiveRange + 0.2f, 1220225809, (QueryTriggerInteraction)0);
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				RaycastHit hit = list[j];
				BaseEntity entity = hit.GetEntity();
				if ((Object)(object)entity == (Object)null || ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)ownerPlayer || entity.EqualNetID((BaseNetworkable)ownerPlayer))) || ((Object)(object)entity != (Object)null && entity.isClient) || entity.Categorize() == ownerPlayer.Categorize())
				{
					continue;
				}
				float num = 0f;
				foreach (DamageTypeEntry damageType in damageTypes)
				{
					num += damageType.amount;
				}
				entity.OnAttacked(new HitInfo(ownerPlayer, entity, DamageType.Slash, num * npcDamageScale));
				HitInfo hitInfo = Pool.Get<HitInfo>();
				hitInfo.HitEntity = entity;
				hitInfo.HitPositionWorld = ((RaycastHit)(ref hit)).point;
				hitInfo.HitNormalWorld = -val;
				if (entity is BaseNpc || entity is BasePlayer)
				{
					hitInfo.HitMaterial = StringPool.Get("Flesh");
				}
				else
				{
					hitInfo.HitMaterial = StringPool.Get(((Object)(object)hit.GetCollider().sharedMaterial != (Object)null) ? hit.GetCollider().sharedMaterial.GetName() : "generic");
				}
				ServerUse_OnHit(hitInfo);
				Effect.server.ImpactEffect(hitInfo);
				Pool.Free<HitInfo>(ref hitInfo);
				flag = true;
				if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
				{
					break;
				}
			}
			Pool.FreeList<RaycastHit>(ref list);
			if (flag)
			{
				break;
			}
		}
	}

	public override Vector3 GetInheritedVelocity(BasePlayer player, Vector3 direction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return player.GetInheritedThrowVelocity(direction);
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.IsActiveItem]
	private void CLProject(RPCMessage msg)
	{
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!VerifyClientAttack(player))
		{
			SendNetworkUpdate();
		}
		else
		{
			if ((Object)(object)player == (Object)null || player.IsHeadUnderwater())
			{
				return;
			}
			if (!canThrowAsProjectile)
			{
				AntiHack.Log(player, AntiHackType.ProjectileHack, "Not throwable (" + base.ShortPrefabName + ")");
				player.stats.combat.LogInvalid(player, this, "not_throwable");
				return;
			}
			Item item = GetItem();
			if (item == null)
			{
				AntiHack.Log(player, AntiHackType.ProjectileHack, "Item not found (" + base.ShortPrefabName + ")");
				player.stats.combat.LogInvalid(player, this, "item_missing");
				return;
			}
			ItemModProjectile component = ((Component)item.info).GetComponent<ItemModProjectile>();
			if ((Object)(object)component == (Object)null)
			{
				AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + base.ShortPrefabName + ")");
				player.stats.combat.LogInvalid(player, this, "mod_missing");
				return;
			}
			ProjectileShoot val = ProjectileShoot.Deserialize((Stream)(object)msg.read);
			if (val.projectiles.Count != 1)
			{
				AntiHack.Log(player, AntiHackType.ProjectileHack, "Projectile count mismatch (" + base.ShortPrefabName + ")");
				player.stats.combat.LogInvalid(player, this, "count_mismatch");
				return;
			}
			player.CleanupExpiredProjectiles();
			Guid projectileGroupId = Guid.NewGuid();
			foreach (Projectile projectile in val.projectiles)
			{
				if (player.HasFiredProjectile(projectile.projectileID))
				{
					AntiHack.Log(player, AntiHackType.ProjectileHack, "Duplicate ID (" + projectile.projectileID + ")");
					player.stats.combat.LogInvalid(player, this, "duplicate_id");
					continue;
				}
				Vector3 positionOffset = Vector3.zero;
				if (ConVar.AntiHack.projectile_positionoffset && (player.isMounted || player.HasParent()))
				{
					if (!ValidateEyePos(player, projectile.startPos, checkLineOfSight: false))
					{
						continue;
					}
					Vector3 position = player.eyes.position;
					positionOffset = position - projectile.startPos;
					projectile.startPos = position;
				}
				else if (!ValidateEyePos(player, projectile.startPos))
				{
					continue;
				}
				player.NoteFiredProjectile(projectile.projectileID, projectile.startPos, projectile.startVel, this, item.info, projectileGroupId, positionOffset, item);
				Effect effect = new Effect();
				effect.Init(Effect.Type.Projectile, projectile.startPos, projectile.startVel, msg.connection);
				((EffectData)effect).scale = 1f;
				effect.pooledString = component.projectileObject.resourcePath;
				((EffectData)effect).number = projectile.seed;
				EffectNetwork.Send(effect);
			}
			if (val != null)
			{
				val.Dispose();
			}
			item.SetParent(null);
			if (!canAiHearIt)
			{
				return;
			}
			float num = 0f;
			if (component.projectileObject != null)
			{
				GameObject val2 = component.projectileObject.Get();
				if ((Object)(object)val2 != (Object)null)
				{
					Projectile component2 = val2.GetComponent<Projectile>();
					if ((Object)(object)component2 != (Object)null)
					{
						foreach (DamageTypeEntry damageType in component2.damageTypes)
						{
							num += damageType.amount;
						}
					}
				}
			}
			if ((Object)(object)player != (Object)null)
			{
				Sensation sensation = default(Sensation);
				sensation.Type = SensationType.ThrownWeapon;
				sensation.Position = ((Component)player).transform.position;
				sensation.Radius = 50f;
				sensation.DamagePotential = num;
				sensation.InitiatorPlayer = player;
				sensation.Initiator = player;
				Sense.Stimulate(sensation);
			}
		}
	}
}
