using ConVar;
using UnityEngine;

public class AttackEntity : HeldEntity
{
	[Header("Attack Entity")]
	public float deployDelay = 1f;

	public float repeatDelay = 0.5f;

	public float animationDelay;

	[Header("NPCUsage")]
	public float effectiveRange = 1f;

	public float npcDamageScale = 1f;

	public float attackLengthMin = -1f;

	public float attackLengthMax = -1f;

	public float attackSpacing;

	public float aiAimSwayOffset;

	public float aiAimCone;

	public bool aiOnlyInRange;

	public float CloseRangeAddition;

	public float MediumRangeAddition;

	public float LongRangeAddition;

	public bool CanUseAtMediumRange = true;

	public bool CanUseAtLongRange = true;

	public SoundDefinition[] reloadSounds;

	public SoundDefinition thirdPersonMeleeSound;

	[Header("Recoil Compensation")]
	public float recoilCompDelayOverride;

	public bool wantsRecoilComp;

	private float nextAttackTime = float.NegativeInfinity;

	protected bool UsingInfiniteAmmoCheat
	{
		get
		{
			BasePlayer ownerPlayer = GetOwnerPlayer();
			if ((Object)(object)ownerPlayer == (Object)null || (!ownerPlayer.IsAdmin && !ownerPlayer.IsDeveloper))
			{
				return false;
			}
			return ownerPlayer.GetInfoBool("player.infiniteammo", defaultVal: false);
		}
	}

	public float NextAttackTime => nextAttackTime;

	public virtual Vector3 GetInheritedVelocity(BasePlayer player, Vector3 direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.zero;
	}

	public virtual float AmmoFraction()
	{
		return 0f;
	}

	public virtual bool CanReload()
	{
		return false;
	}

	public virtual bool ServerIsReloading()
	{
		return false;
	}

	public virtual void ServerReload()
	{
	}

	public virtual bool ServerTryReload(IAmmoContainer ammoSource)
	{
		return true;
	}

	public virtual void TopUpAmmo()
	{
	}

	public virtual Vector3 ModifyAIAim(Vector3 eulerInput, float swayModifier = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return eulerInput;
	}

	public virtual void GetAttackStats(HitInfo info)
	{
	}

	protected void StartAttackCooldownRaw(float cooldown)
	{
		nextAttackTime = Time.time + cooldown;
	}

	protected void StartAttackCooldown(float cooldown)
	{
		nextAttackTime = CalculateCooldownTime(nextAttackTime, cooldown, catchup: true);
	}

	public void ResetAttackCooldown()
	{
		nextAttackTime = float.NegativeInfinity;
	}

	public bool HasAttackCooldown()
	{
		return Time.time < nextAttackTime;
	}

	protected float GetAttackCooldown()
	{
		return Mathf.Max(nextAttackTime - Time.time, 0f);
	}

	protected float GetAttackIdle()
	{
		return Mathf.Max(Time.time - nextAttackTime, 0f);
	}

	protected float CalculateCooldownTime(float nextTime, float cooldown, bool catchup)
	{
		float time = Time.time;
		float num = 0f;
		if (base.isServer)
		{
			BasePlayer ownerPlayer = GetOwnerPlayer();
			num += 0.1f;
			num += cooldown * 0.1f;
			num += (Object.op_Implicit((Object)(object)ownerPlayer) ? ownerPlayer.desyncTimeClamped : 0.1f);
			num += Mathf.Max(Time.deltaTime, Time.smoothDeltaTime);
		}
		nextTime = ((nextTime < 0f) ? Mathf.Max(0f, time + cooldown - num) : ((!(time - nextTime <= num)) ? Mathf.Max(nextTime + cooldown, time + cooldown - num) : Mathf.Min(nextTime + cooldown, time + cooldown)));
		return nextTime;
	}

	protected bool VerifyClientRPC(BasePlayer player)
	{
		if ((Object)(object)player == (Object)null)
		{
			Debug.LogWarning((object)"Received RPC from null player");
			return false;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)ownerPlayer == (Object)null)
		{
			AntiHack.Log(player, AntiHackType.AttackHack, "Owner not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "owner_missing");
			return false;
		}
		if ((Object)(object)ownerPlayer != (Object)(object)player)
		{
			AntiHack.Log(player, AntiHackType.AttackHack, "Player mismatch (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_mismatch");
			return false;
		}
		if (player.IsDead())
		{
			AntiHack.Log(player, AntiHackType.AttackHack, "Player dead (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_dead");
			return false;
		}
		if (player.IsWounded())
		{
			AntiHack.Log(player, AntiHackType.AttackHack, "Player down (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_down");
			return false;
		}
		if (player.IsSleeping())
		{
			AntiHack.Log(player, AntiHackType.AttackHack, "Player sleeping (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "player_sleeping");
			return false;
		}
		if (player.desyncTimeRaw > ConVar.AntiHack.maxdesync)
		{
			AntiHack.Log(player, AntiHackType.AttackHack, "Player stalled (" + base.ShortPrefabName + " with " + player.desyncTimeRaw + "s)");
			player.stats.combat.LogInvalid(player, this, "player_stalled");
			return false;
		}
		Item ownerItem = GetOwnerItem();
		if (ownerItem == null)
		{
			AntiHack.Log(player, AntiHackType.AttackHack, "Item not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "item_missing");
			return false;
		}
		if (ownerItem.isBroken)
		{
			AntiHack.Log(player, AntiHackType.AttackHack, "Item broken (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "item_broken");
			return false;
		}
		return true;
	}

	protected virtual bool VerifyClientAttack(BasePlayer player)
	{
		if (!VerifyClientRPC(player))
		{
			return false;
		}
		if (HasAttackCooldown())
		{
			AntiHack.Log(player, AntiHackType.CooldownHack, "T-" + GetAttackCooldown() + "s (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "attack_cooldown");
			return false;
		}
		return true;
	}

	protected bool ValidateEyePos(BasePlayer player, Vector3 eyePos, bool checkLineOfSight = true)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048a: Unknown result type (might be due to invalid IL or missing references)
		bool flag = true;
		if (Vector3Ex.IsNaNOrInfinity(eyePos))
		{
			string shortPrefabName = base.ShortPrefabName;
			AntiHack.Log(player, AntiHackType.EyeHack, "Contains NaN (" + shortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "eye_nan");
			flag = false;
		}
		if (ConVar.AntiHack.eye_protection > 0)
		{
			float num = 1f + ConVar.AntiHack.eye_forgiveness;
			float eye_clientframes = ConVar.AntiHack.eye_clientframes;
			float eye_serverframes = ConVar.AntiHack.eye_serverframes;
			float num2 = eye_clientframes / 60f;
			float num3 = eye_serverframes * Mathx.Max(Time.deltaTime, Time.smoothDeltaTime, Time.fixedDeltaTime);
			float num4 = (player.desyncTimeClamped + num2 + num3) * num;
			Vector3 val;
			if (ConVar.AntiHack.eye_protection >= 1)
			{
				float num5 = player.MaxVelocity();
				val = player.GetParentVelocity();
				float num6 = num5 + ((Vector3)(ref val)).magnitude;
				float num7 = player.BoundsPadding() + num4 * num6;
				float num8 = Vector3.Distance(player.eyes.position, eyePos);
				if (num8 > num7)
				{
					string shortPrefabName2 = base.ShortPrefabName;
					AntiHack.Log(player, AntiHackType.EyeHack, "Distance (" + shortPrefabName2 + " on attack with " + num8 + "m > " + num7 + "m)");
					player.stats.combat.LogInvalid(player, this, "eye_distance");
					flag = false;
				}
			}
			if (ConVar.AntiHack.eye_protection >= 3)
			{
				float num9 = Mathf.Abs(player.GetMountVelocity().y + player.GetParentVelocity().y);
				float num10 = player.BoundsPadding() + num4 * num9 + player.GetJumpHeight();
				float num11 = Mathf.Abs(player.eyes.position.y - eyePos.y);
				if (num11 > num10)
				{
					string shortPrefabName3 = base.ShortPrefabName;
					AntiHack.Log(player, AntiHackType.EyeHack, "Altitude (" + shortPrefabName3 + " on attack with " + num11 + "m > " + num10 + "m)");
					player.stats.combat.LogInvalid(player, this, "eye_altitude");
					flag = false;
				}
			}
			if (checkLineOfSight)
			{
				int num12 = 2162688;
				if (ConVar.AntiHack.eye_terraincheck)
				{
					num12 |= 0x800000;
				}
				if (ConVar.AntiHack.eye_vehiclecheck)
				{
					num12 |= 0x8000000;
				}
				if (ConVar.AntiHack.eye_protection >= 2)
				{
					Vector3 center = player.eyes.center;
					Vector3 position = player.eyes.position;
					if (!GamePhysics.LineOfSightRadius(center, position, num12, ConVar.AntiHack.eye_losradius) || !GamePhysics.LineOfSightRadius(position, eyePos, num12, ConVar.AntiHack.eye_losradius))
					{
						string shortPrefabName4 = base.ShortPrefabName;
						string[] obj = new string[8] { "Line of sight (", shortPrefabName4, " on attack) ", null, null, null, null, null };
						val = center;
						obj[3] = ((object)(Vector3)(ref val)).ToString();
						obj[4] = " ";
						val = position;
						obj[5] = ((object)(Vector3)(ref val)).ToString();
						obj[6] = " ";
						val = eyePos;
						obj[7] = ((object)(Vector3)(ref val)).ToString();
						AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(obj));
						player.stats.combat.LogInvalid(player, this, "eye_los");
						flag = false;
					}
				}
				if (ConVar.AntiHack.eye_protection >= 4 && !player.HasParent())
				{
					Vector3 position2 = player.eyes.position;
					float num13 = Vector3.Distance(position2, eyePos);
					Collider collider;
					if (num13 > ConVar.AntiHack.eye_noclip_cutoff)
					{
						if (AntiHack.TestNoClipping(position2, eyePos, player.NoClipRadius(ConVar.AntiHack.eye_noclip_margin), ConVar.AntiHack.eye_noclip_backtracking, ConVar.AntiHack.noclip_protection >= 2, out collider))
						{
							string shortPrefabName5 = base.ShortPrefabName;
							string[] obj2 = new string[6] { "NoClip (", shortPrefabName5, " on attack) ", null, null, null };
							val = position2;
							obj2[3] = ((object)(Vector3)(ref val)).ToString();
							obj2[4] = " ";
							val = eyePos;
							obj2[5] = ((object)(Vector3)(ref val)).ToString();
							AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(obj2));
							player.stats.combat.LogInvalid(player, this, "eye_noclip");
							flag = false;
						}
					}
					else if (num13 > 0.01f && AntiHack.TestNoClipping(position2, eyePos, 0.01f, ConVar.AntiHack.eye_noclip_backtracking, ConVar.AntiHack.noclip_protection >= 2, out collider))
					{
						string shortPrefabName6 = base.ShortPrefabName;
						string[] obj3 = new string[6] { "NoClip (", shortPrefabName6, " on attack) ", null, null, null };
						val = position2;
						obj3[3] = ((object)(Vector3)(ref val)).ToString();
						obj3[4] = " ";
						val = eyePos;
						obj3[5] = ((object)(Vector3)(ref val)).ToString();
						AntiHack.Log(player, AntiHackType.EyeHack, string.Concat(obj3));
						player.stats.combat.LogInvalid(player, this, "eye_noclip");
						flag = false;
					}
				}
			}
			if (!flag)
			{
				AntiHack.AddViolation(player, AntiHackType.EyeHack, ConVar.AntiHack.eye_penalty);
			}
			else if (ConVar.AntiHack.eye_protection >= 5 && !player.HasParent() && !player.isMounted)
			{
				player.eyeHistory.PushBack(eyePos);
			}
		}
		return flag;
	}

	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		StartAttackCooldown(deployDelay * 0.9f);
	}
}
