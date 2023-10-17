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
		GetOwnerItem()?.LoseCondition(amount);
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
		ownerItem.LoseCondition(conditionLoss);
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void PlayerAttack(RPCMessage msg)
	{
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_063b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0670: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_080b: Unknown result type (might be due to invalid IL or missing references)
		//IL_080f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0814: Unknown result type (might be due to invalid IL or missing references)
		//IL_081d: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_073c: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_08de: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0917: Unknown result type (might be due to invalid IL or missing references)
		//IL_0919: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
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
				int layerMask;
				Vector3 val3;
				Vector3 center;
				Vector3 position;
				Vector3 val4;
				Vector3 val5;
				Vector3 val6;
				int num17;
				if (ConVar.AntiHack.melee_protection > 0)
				{
					flag8 = true;
					float num = 1f + ConVar.AntiHack.melee_forgiveness;
					float melee_clientframes = ConVar.AntiHack.melee_clientframes;
					float melee_serverframes = ConVar.AntiHack.melee_serverframes;
					float num2 = melee_clientframes / 60f;
					float num3 = melee_serverframes * Mathx.Max(Time.deltaTime, Time.smoothDeltaTime, Time.fixedDeltaTime);
					float num4 = (player.desyncTimeClamped + num2 + num3) * num;
					layerMask = (ConVar.AntiHack.melee_terraincheck ? 10551296 : 2162688);
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
							float num5 = hitEntity.MaxVelocity();
							val3 = hitEntity.GetParentVelocity();
							float num6 = num5 + ((Vector3)(ref val3)).magnitude;
							float num7 = hitEntity.BoundsPadding() + num4 * num6;
							float num8 = hitEntity.Distance(hitInfo.HitPositionWorld);
							if (num8 > num7)
							{
								string shortPrefabName4 = base.ShortPrefabName;
								string shortPrefabName5 = hitEntity.ShortPrefabName;
								AntiHack.Log(player, AntiHackType.MeleeHack, "Entity too far away (" + shortPrefabName4 + " on " + shortPrefabName5 + " with " + num8 + "m > " + num7 + "m in " + num4 + "s)");
								player.stats.combat.LogInvalid(hitInfo, "melee_target");
								flag8 = false;
							}
						}
						if (ConVar.AntiHack.melee_protection >= 4 && flag8 && flag && !flag7 && !flag2 && !flag3 && !flag4 && !flag5)
						{
							val3 = basePlayer.GetParentVelocity();
							float magnitude = ((Vector3)(ref val3)).magnitude;
							float num9 = basePlayer.BoundsPadding() + num4 * magnitude + ConVar.AntiHack.tickhistoryforgiveness;
							float num10 = basePlayer.tickHistory.Distance(basePlayer, hitInfo.HitPositionWorld);
							if (num10 > num9)
							{
								string shortPrefabName6 = base.ShortPrefabName;
								string shortPrefabName7 = basePlayer.ShortPrefabName;
								AntiHack.Log(player, AntiHackType.ProjectileHack, "Player too far away (" + shortPrefabName6 + " on " + shortPrefabName7 + " with " + num10 + "m > " + num9 + "m in " + num4 + "s)");
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
							float num11 = player.BoundsPadding() + num4 * magnitude2 + num * maxDistance;
							float num12 = player.tickHistory.Distance(player, hitInfo.HitPositionWorld);
							if (num12 > num11)
							{
								string shortPrefabName8 = base.ShortPrefabName;
								string text = (flag6 ? hitEntity.ShortPrefabName : "world");
								AntiHack.Log(player, AntiHackType.MeleeHack, "Initiator too far away (" + shortPrefabName8 + " on " + text + " with " + num12 + "m > " + num11 + "m in " + num4 + "s)");
								player.stats.combat.LogInvalid(hitInfo, "melee_initiator");
								flag8 = false;
							}
						}
						else
						{
							float num13 = player.MaxVelocity();
							val3 = player.GetParentVelocity();
							float num14 = num13 + ((Vector3)(ref val3)).magnitude;
							float num15 = player.BoundsPadding() + num4 * num14 + num * maxDistance;
							float num16 = player.Distance(hitInfo.HitPositionWorld);
							if (num16 > num15)
							{
								string shortPrefabName9 = base.ShortPrefabName;
								string text2 = (flag6 ? hitEntity.ShortPrefabName : "world");
								AntiHack.Log(player, AntiHackType.MeleeHack, "Initiator too far away (" + shortPrefabName9 + " on " + text2 + " with " + num16 + "m > " + num15 + "m in " + num4 + "s)");
								player.stats.combat.LogInvalid(hitInfo, "melee_initiator");
								flag8 = false;
							}
						}
					}
					if (ConVar.AntiHack.melee_protection >= 3)
					{
						if (flag6)
						{
							Vector3 pointStart = hitInfo.PointStart;
							Vector3 hitPositionWorld = hitInfo.HitPositionWorld;
							center = player.eyes.center;
							position = player.eyes.position;
							val4 = pointStart;
							val5 = hitInfo.PositionOnRay(hitPositionWorld) + ((Vector3)(ref hitInfo.HitNormalWorld)).normalized * 0.001f;
							val6 = hitPositionWorld;
							if (GamePhysics.LineOfSight(center, position, layerMask) && GamePhysics.LineOfSight(position, val4, layerMask) && GamePhysics.LineOfSight(val4, val5, layerMask))
							{
								num17 = (GamePhysics.LineOfSight(val5, val6, layerMask, hitEntity) ? 1 : 0);
								if (num17 != 0)
								{
									player.stats.Add("hit_" + hitEntity.Categorize() + "_direct_los", 1, Stats.Server);
									goto IL_06f9;
								}
							}
							else
							{
								num17 = 0;
							}
							player.stats.Add("hit_" + hitEntity.Categorize() + "_indirect_los", 1, Stats.Server);
							goto IL_06f9;
						}
						goto IL_07e6;
					}
					goto IL_094c;
				}
				goto IL_095e;
				IL_06f9:
				if (num17 == 0)
				{
					string shortPrefabName10 = base.ShortPrefabName;
					string shortPrefabName11 = hitEntity.ShortPrefabName;
					string[] obj = new string[14]
					{
						"Line of sight (", shortPrefabName10, " on ", shortPrefabName11, ") ", null, null, null, null, null,
						null, null, null, null
					};
					val3 = center;
					obj[5] = ((object)(Vector3)(ref val3)).ToString();
					obj[6] = " ";
					val3 = position;
					obj[7] = ((object)(Vector3)(ref val3)).ToString();
					obj[8] = " ";
					val3 = val4;
					obj[9] = ((object)(Vector3)(ref val3)).ToString();
					obj[10] = " ";
					val3 = val5;
					obj[11] = ((object)(Vector3)(ref val3)).ToString();
					obj[12] = " ";
					val3 = val6;
					obj[13] = ((object)(Vector3)(ref val3)).ToString();
					AntiHack.Log(player, AntiHackType.MeleeHack, string.Concat(obj));
					player.stats.combat.LogInvalid(hitInfo, "melee_los");
					flag8 = false;
				}
				goto IL_07e6;
				IL_095e:
				player.metabolism.UseHeart(heartStress * 0.2f);
				TimeWarning val7 = TimeWarning.New("DoAttackShared", 50);
				try
				{
					DoAttackShared(hitInfo);
					return;
				}
				finally
				{
					((IDisposable)val7)?.Dispose();
				}
				IL_07e6:
				if (flag8 && flag && !flag7)
				{
					Vector3 hitPositionWorld2 = hitInfo.HitPositionWorld;
					Vector3 position2 = basePlayer.eyes.position;
					Vector3 val8 = basePlayer.CenterPoint();
					float melee_losforgiveness = ConVar.AntiHack.melee_losforgiveness;
					bool flag9 = GamePhysics.LineOfSight(hitPositionWorld2, position2, layerMask, 0f, melee_losforgiveness) && GamePhysics.LineOfSight(position2, hitPositionWorld2, layerMask, melee_losforgiveness, 0f);
					if (!flag9)
					{
						flag9 = GamePhysics.LineOfSight(hitPositionWorld2, val8, layerMask, 0f, melee_losforgiveness) && GamePhysics.LineOfSight(val8, hitPositionWorld2, layerMask, melee_losforgiveness, 0f);
					}
					if (!flag9)
					{
						string shortPrefabName12 = base.ShortPrefabName;
						string shortPrefabName13 = basePlayer.ShortPrefabName;
						string[] obj2 = new string[12]
						{
							"Line of sight (", shortPrefabName12, " on ", shortPrefabName13, ") ", null, null, null, null, null,
							null, null
						};
						val3 = hitPositionWorld2;
						obj2[5] = ((object)(Vector3)(ref val3)).ToString();
						obj2[6] = " ";
						val3 = position2;
						obj2[7] = ((object)(Vector3)(ref val3)).ToString();
						obj2[8] = " or ";
						val3 = hitPositionWorld2;
						obj2[9] = ((object)(Vector3)(ref val3)).ToString();
						obj2[10] = " ";
						val3 = val8;
						obj2[11] = ((object)(Vector3)(ref val3)).ToString();
						AntiHack.Log(player, AntiHackType.MeleeHack, string.Concat(obj2));
						player.stats.combat.LogInvalid(hitInfo, "melee_los");
						flag8 = false;
					}
				}
				goto IL_094c;
				IL_094c:
				if (!flag8)
				{
					AntiHack.AddViolation(player, AntiHackType.MeleeHack, ConVar.AntiHack.melee_penalty);
					return;
				}
				goto IL_095e;
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
			GamePhysics.TraceAll(new Ray(position - val * ((i == 0) ? 0f : 0.2f), val), (i == 0) ? 0f : attackRadius, list, effectiveRange + 0.2f, 1219701521, (QueryTriggerInteraction)0);
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				RaycastHit hit = list[j];
				BaseEntity entity = hit.GetEntity();
				if ((Object)(object)entity == (Object)null || ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)ownerPlayer || entity.EqualNetID(ownerPlayer))) || ((Object)(object)entity != (Object)null && entity.isClient) || entity.Categorize() == ownerPlayer.Categorize())
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
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
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
			foreach (Projectile projectile in val.projectiles)
			{
				if (player.HasFiredProjectile(projectile.projectileID))
				{
					AntiHack.Log(player, AntiHackType.ProjectileHack, "Duplicate ID (" + projectile.projectileID + ")");
					player.stats.combat.LogInvalid(player, this, "duplicate_id");
				}
				else if (ValidateEyePos(player, projectile.startPos))
				{
					player.NoteFiredProjectile(projectile.projectileID, projectile.startPos, projectile.startVel, this, item.info, item);
					Effect effect = new Effect();
					effect.Init(Effect.Type.Projectile, projectile.startPos, projectile.startVel, msg.connection);
					((EffectData)effect).scale = 1f;
					effect.pooledString = component.projectileObject.resourcePath;
					((EffectData)effect).number = projectile.seed;
					EffectNetwork.Send(effect);
				}
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
