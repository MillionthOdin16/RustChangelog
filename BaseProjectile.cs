using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

public class BaseProjectile : AttackEntity
{
	[Serializable]
	public class Magazine
	{
		[Serializable]
		public struct Definition
		{
			[Tooltip("Set to 0 to not use inbuilt mag")]
			public int builtInSize;

			[Tooltip("If using inbuilt mag, will accept these types of ammo")]
			[InspectorFlags]
			public AmmoTypes ammoTypes;
		}

		public Definition definition;

		public int capacity;

		public int contents;

		[ItemSelector(ItemCategory.All)]
		public ItemDefinition ammoType;

		public void ServerInit()
		{
			if (definition.builtInSize > 0)
			{
				capacity = definition.builtInSize;
			}
		}

		public Magazine Save()
		{
			Magazine val = Pool.Get<Magazine>();
			if ((Object)(object)ammoType == (Object)null)
			{
				val.capacity = capacity;
				val.contents = 0;
				val.ammoType = 0;
			}
			else
			{
				val.capacity = capacity;
				val.contents = contents;
				val.ammoType = ammoType.itemid;
			}
			return val;
		}

		public void Load(Magazine mag)
		{
			contents = mag.contents;
			capacity = mag.capacity;
			ammoType = ItemManager.FindItemDefinition(mag.ammoType);
		}

		public bool CanReload(BasePlayer owner)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (contents >= capacity)
			{
				return false;
			}
			return owner.inventory.HasAmmo(definition.ammoTypes);
		}

		public bool CanAiReload(BasePlayer owner)
		{
			if (contents >= capacity)
			{
				return false;
			}
			return true;
		}

		public void SwitchAmmoTypesIfNeeded(BasePlayer owner)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			List<Item> list = owner.inventory.FindItemIDs(ammoType.itemid).ToList();
			if (list.Count != 0)
			{
				return;
			}
			List<Item> list2 = new List<Item>();
			owner.inventory.FindAmmo(list2, definition.ammoTypes);
			if (list2.Count == 0)
			{
				return;
			}
			list = owner.inventory.FindItemIDs(list2[0].info.itemid).ToList();
			if (list != null && list.Count != 0)
			{
				if (contents > 0)
				{
					owner.GiveItem(ItemManager.CreateByItemID(ammoType.itemid, contents, 0uL));
					contents = 0;
				}
				ammoType = list[0].info;
			}
		}

		public bool Reload(BasePlayer owner, int desiredAmount = -1, bool canRefundAmmo = true)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			List<Item> list = owner.inventory.FindItemIDs(ammoType.itemid).ToList();
			if (list.Count == 0)
			{
				List<Item> list2 = new List<Item>();
				owner.inventory.FindAmmo(list2, definition.ammoTypes);
				if (list2.Count == 0)
				{
					return false;
				}
				list = owner.inventory.FindItemIDs(list2[0].info.itemid).ToList();
				if (list == null || list.Count == 0)
				{
					return false;
				}
				if (contents > 0)
				{
					if (canRefundAmmo)
					{
						owner.GiveItem(ItemManager.CreateByItemID(ammoType.itemid, contents, 0uL));
					}
					contents = 0;
				}
				ammoType = list[0].info;
			}
			int num = desiredAmount;
			if (num == -1)
			{
				num = capacity - contents;
			}
			foreach (Item item in list)
			{
				int amount = item.amount;
				int num2 = Mathf.Min(num, item.amount);
				item.UseItem(num2);
				contents += num2;
				num -= num2;
				if (num <= 0)
				{
					break;
				}
			}
			return false;
		}
	}

	public static class BaseProjectileFlags
	{
		public const Flags BurstToggle = Flags.Reserved6;
	}

	[Header("NPC Info")]
	public float NoiseRadius = 100f;

	[Header("Projectile")]
	public float damageScale = 1f;

	public float distanceScale = 1f;

	public float projectileVelocityScale = 1f;

	public bool automatic;

	public bool usableByTurret = true;

	[Tooltip("Final damage is scaled by this amount before being applied to a target when this weapon is mounted to a turret")]
	public float turretDamageScale = 0.35f;

	[Header("Effects")]
	public GameObjectRef attackFX;

	public GameObjectRef silencedAttack;

	public GameObjectRef muzzleBrakeAttack;

	public Transform MuzzlePoint;

	[Header("Reloading")]
	public float reloadTime = 1f;

	public bool canUnloadAmmo = true;

	public Magazine primaryMagazine;

	public bool fractionalReload = false;

	public float reloadStartDuration = 0f;

	public float reloadFractionDuration = 0f;

	public float reloadEndDuration = 0f;

	[Header("Recoil")]
	public float aimSway = 3f;

	public float aimSwaySpeed = 1f;

	public RecoilProperties recoil;

	[Header("Aim Cone")]
	public AnimationCurve aimconeCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	public float aimCone;

	public float hipAimCone = 1.8f;

	public float aimconePenaltyPerShot = 0f;

	public float aimConePenaltyMax = 0f;

	public float aimconePenaltyRecoverTime = 0.1f;

	public float aimconePenaltyRecoverDelay = 0.1f;

	public float stancePenaltyScale = 1f;

	[Header("Iconsights")]
	public bool hasADS = true;

	public bool noAimingWhileCycling = false;

	public bool manualCycle = false;

	[NonSerialized]
	protected bool needsCycle = false;

	[NonSerialized]
	protected bool isCycling = false;

	[NonSerialized]
	public bool aiming = false;

	[Header("ViewModel")]
	public bool useEmptyAmmoState = false;

	[Header("Burst Information")]
	public bool isBurstWeapon = false;

	public bool canChangeFireModes = true;

	public bool defaultOn = true;

	public float internalBurstRecoilScale = 0.8f;

	public float internalBurstFireRateScale = 0.8f;

	public float internalBurstAimConeScale = 0.8f;

	public Phrase Toast_BurstDisabled = new Phrase("burst_disabled", "Burst Disabled");

	public Phrase Toast_BurstEnabled = new Phrase("burst enabled", "Burst Enabled");

	public float resetDuration = 0.3f;

	public int numShotsFired = 0;

	[NonSerialized]
	private float nextReloadTime = float.NegativeInfinity;

	[NonSerialized]
	private float startReloadTime = float.NegativeInfinity;

	private float lastReloadTime = -10f;

	private bool modsChangedInitialized = false;

	private float stancePenalty = 0f;

	private float aimconePenalty = 0f;

	private uint cachedModHash = 0u;

	private float sightAimConeScale = 1f;

	private float sightAimConeOffset = 0f;

	private float hipAimConeScale = 1f;

	private float hipAimConeOffset = 0f;

	protected bool reloadStarted = false;

	protected bool reloadFinished = false;

	private int fractionalInsertCounter = 0;

	private static readonly Effect reusableInstance = new Effect();

	public RecoilProperties recoilProperties => ((Object)(object)recoil == (Object)null) ? null : recoil.GetRecoil();

	public bool isSemiAuto => !automatic;

	public override bool IsUsableByTurret => usableByTurret;

	public override Transform MuzzleTransform => MuzzlePoint;

	protected virtual bool CanRefundAmmo => true;

	protected virtual ItemDefinition PrimaryMagazineAmmo => primaryMagazine.ammoType;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseProjectile.OnRpcMessage", 0);
		try
		{
			if (rpc == 3168282921u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - CLProject "));
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
						TimeWarning val4 = TimeWarning.New("Call", 0);
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
							((IDisposable)val4)?.Dispose();
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
			if (rpc == 1720368164 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Reload "));
				}
				TimeWarning val5 = TimeWarning.New("Reload", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1720368164u, "Reload", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
					try
					{
						TimeWarning val7 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							Reload(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Reload");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 240404208 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerFractionalReloadInsert "));
				}
				TimeWarning val8 = TimeWarning.New("ServerFractionalReloadInsert", 0);
				try
				{
					TimeWarning val9 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(240404208u, "ServerFractionalReloadInsert", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val9)?.Dispose();
					}
					try
					{
						TimeWarning val10 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg4 = rPCMessage;
							ServerFractionalReloadInsert(msg4);
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in ServerFractionalReloadInsert");
					}
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
				}
				return true;
			}
			if (rpc == 555589155 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - StartReload "));
				}
				TimeWarning val11 = TimeWarning.New("StartReload", 0);
				try
				{
					TimeWarning val12 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(555589155u, "StartReload", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val12)?.Dispose();
					}
					try
					{
						TimeWarning val13 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg5 = rPCMessage;
							StartReload(msg5);
						}
						finally
						{
							((IDisposable)val13)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in StartReload");
					}
				}
				finally
				{
					((IDisposable)val11)?.Dispose();
				}
				return true;
			}
			if (rpc == 1918419884 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SwitchAmmoTo "));
				}
				TimeWarning val14 = TimeWarning.New("SwitchAmmoTo", 0);
				try
				{
					TimeWarning val15 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1918419884u, "SwitchAmmoTo", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val15)?.Dispose();
					}
					try
					{
						TimeWarning val16 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg6 = rPCMessage;
							SwitchAmmoTo(msg6);
						}
						finally
						{
							((IDisposable)val16)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in SwitchAmmoTo");
					}
				}
				finally
				{
					((IDisposable)val14)?.Dispose();
				}
				return true;
			}
			if (rpc == 3327286961u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ToggleFireMode "));
				}
				TimeWarning val17 = TimeWarning.New("ToggleFireMode", 0);
				try
				{
					TimeWarning val18 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3327286961u, "ToggleFireMode", this, player, 2uL))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(3327286961u, "ToggleFireMode", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val18)?.Dispose();
					}
					try
					{
						TimeWarning val19 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg7 = rPCMessage;
							ToggleFireMode(msg7);
						}
						finally
						{
							((IDisposable)val19)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in ToggleFireMode");
					}
				}
				finally
				{
					((IDisposable)val17)?.Dispose();
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

	public override Vector3 GetInheritedVelocity(BasePlayer player, Vector3 direction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return player.GetInheritedProjectileVelocity(direction);
	}

	public virtual float GetDamageScale(bool getMax = false)
	{
		return damageScale;
	}

	public virtual float GetDistanceScale(bool getMax = false)
	{
		return distanceScale;
	}

	public virtual float GetProjectileVelocityScale(bool getMax = false)
	{
		return projectileVelocityScale;
	}

	protected void StartReloadCooldown(float cooldown)
	{
		nextReloadTime = CalculateCooldownTime(nextReloadTime, cooldown, catchup: false);
		startReloadTime = nextReloadTime - cooldown;
	}

	protected void ResetReloadCooldown()
	{
		nextReloadTime = float.NegativeInfinity;
	}

	protected bool HasReloadCooldown()
	{
		return Time.time < nextReloadTime;
	}

	protected float GetReloadCooldown()
	{
		return Mathf.Max(nextReloadTime - Time.time, 0f);
	}

	protected float GetReloadIdle()
	{
		return Mathf.Max(Time.time - nextReloadTime, 0f);
	}

	private void OnDrawGizmos()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient && (Object)(object)MuzzlePoint != (Object)null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(MuzzlePoint.position, MuzzlePoint.position + MuzzlePoint.forward * 10f);
			BasePlayer ownerPlayer = GetOwnerPlayer();
			if (Object.op_Implicit((Object)(object)ownerPlayer))
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(MuzzlePoint.position, MuzzlePoint.position + ownerPlayer.eyes.rotation * Vector3.forward * 10f);
			}
		}
	}

	public virtual RecoilProperties GetRecoil()
	{
		return recoilProperties;
	}

	public virtual void DidAttackServerside()
	{
	}

	public override bool ServerIsReloading()
	{
		return Time.time < lastReloadTime + reloadTime;
	}

	public override bool CanReload()
	{
		return primaryMagazine.contents < primaryMagazine.capacity;
	}

	public override float AmmoFraction()
	{
		return (float)primaryMagazine.contents / (float)primaryMagazine.capacity;
	}

	public override void TopUpAmmo()
	{
		primaryMagazine.contents = primaryMagazine.capacity;
	}

	public override void ServerReload()
	{
		if (!ServerIsReloading())
		{
			lastReloadTime = Time.time;
			StartAttackCooldown(reloadTime);
			GetOwnerPlayer().SignalBroadcast(Signal.Reload);
			primaryMagazine.contents = primaryMagazine.capacity;
		}
	}

	public override Vector3 ModifyAIAim(Vector3 eulerInput, float swayModifier = 1f)
	{
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		float num = Time.time * (aimSwaySpeed * 1f + aiAimSwayOffset);
		float num2 = Mathf.Sin(Time.time * 2f);
		float num3 = ((num2 < 0f) ? (1f - Mathf.Clamp(Mathf.Abs(num2) / 1f, 0f, 1f)) : 1f);
		float num4 = (flag ? 0.6f : 1f);
		float num5 = (aimSway * 1f + aiAimSwayOffset) * num4 * num3 * swayModifier;
		eulerInput.y += (Mathf.PerlinNoise(num, num) - 0.5f) * num5 * Time.deltaTime;
		eulerInput.x += (Mathf.PerlinNoise(num + 0.1f, num + 0.2f) - 0.5f) * num5 * Time.deltaTime;
		return eulerInput;
	}

	public float GetAIAimcone()
	{
		NPCPlayer nPCPlayer = GetOwnerPlayer() as NPCPlayer;
		if (Object.op_Implicit((Object)(object)nPCPlayer))
		{
			return nPCPlayer.GetAimConeScale() * aiAimCone;
		}
		return aiAimCone;
	}

	public override void ServerUse()
	{
		ServerUse(1f);
	}

	public override void ServerUse(float damageModifier, Transform originOverride = null)
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient || HasAttackCooldown())
		{
			return;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		bool flag = (Object)(object)ownerPlayer != (Object)null;
		if (primaryMagazine.contents <= 0)
		{
			SignalBroadcast(Signal.DryFire);
			StartAttackCooldownRaw(1f);
			return;
		}
		primaryMagazine.contents--;
		if (primaryMagazine.contents < 0)
		{
			primaryMagazine.contents = 0;
		}
		bool flag2 = flag && ownerPlayer.IsNpc;
		if (flag2 && (ownerPlayer.isMounted || (Object)(object)ownerPlayer.GetParentEntity() != (Object)null))
		{
			NPCPlayer nPCPlayer = ownerPlayer as NPCPlayer;
			if ((Object)(object)nPCPlayer != (Object)null)
			{
				nPCPlayer.SetAimDirection(nPCPlayer.GetAimDirection());
			}
		}
		StartAttackCooldownRaw(repeatDelay);
		Vector3 val = (flag ? ownerPlayer.eyes.position : ((Component)MuzzlePoint).transform.position);
		Vector3 inputVec = ((Component)MuzzlePoint).transform.forward;
		if ((Object)(object)originOverride != (Object)null)
		{
			val = originOverride.position;
			inputVec = originOverride.forward;
		}
		ItemModProjectile component = ((Component)primaryMagazine.ammoType).GetComponent<ItemModProjectile>();
		SignalBroadcast(Signal.Attack, string.Empty);
		Projectile component2 = component.projectileObject.Get().GetComponent<Projectile>();
		BaseEntity baseEntity = null;
		if (flag)
		{
			inputVec = ownerPlayer.eyes.BodyForward();
		}
		for (int i = 0; i < component.numProjectiles; i++)
		{
			Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(component.projectileSpread + GetAimCone() + GetAIAimcone() * 1f, inputVec);
			List<RaycastHit> list = Pool.GetList<RaycastHit>();
			GamePhysics.TraceAll(new Ray(val, modifiedAimConeDirection), 0f, list, 300f, 1220225793, (QueryTriggerInteraction)0);
			for (int j = 0; j < list.Count; j++)
			{
				RaycastHit hit = list[j];
				BaseEntity entity = hit.GetEntity();
				if (((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)this || entity.EqualNetID((BaseNetworkable)this))) || ((Object)(object)entity != (Object)null && entity.isClient))
				{
					continue;
				}
				ColliderInfo component3 = ((Component)((RaycastHit)(ref hit)).collider).GetComponent<ColliderInfo>();
				if ((Object)(object)component3 != (Object)null && !component3.HasFlag(ColliderInfo.Flags.Shootable))
				{
					continue;
				}
				BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
				if ((!((Object)(object)entity != (Object)null && entity.IsNpc && flag2) || baseCombatEntity.GetFaction() == BaseCombatEntity.Faction.Horror || entity is BasePet) && (Object)(object)baseCombatEntity != (Object)null && ((Object)(object)baseEntity == (Object)null || (Object)(object)entity == (Object)(object)baseEntity || entity.EqualNetID((BaseNetworkable)baseEntity)))
				{
					HitInfo hitInfo = new HitInfo();
					AssignInitiator(hitInfo);
					hitInfo.Weapon = this;
					hitInfo.WeaponPrefab = base.gameManager.FindPrefab(base.PrefabName).GetComponent<AttackEntity>();
					hitInfo.IsPredicting = false;
					hitInfo.DoHitEffects = component2.doDefaultHitEffects;
					hitInfo.DidHit = true;
					hitInfo.ProjectileVelocity = modifiedAimConeDirection * 300f;
					hitInfo.PointStart = MuzzlePoint.position;
					hitInfo.PointEnd = ((RaycastHit)(ref hit)).point;
					hitInfo.HitPositionWorld = ((RaycastHit)(ref hit)).point;
					hitInfo.HitNormalWorld = ((RaycastHit)(ref hit)).normal;
					hitInfo.HitEntity = entity;
					hitInfo.UseProtection = true;
					component2.CalculateDamage(hitInfo, GetProjectileModifier(), 1f);
					hitInfo.damageTypes.ScaleAll(GetDamageScale() * damageModifier * (flag2 ? npcDamageScale : turretDamageScale));
					baseCombatEntity.OnAttacked(hitInfo);
					component.ServerProjectileHit(hitInfo);
					if (entity is BasePlayer || entity is BaseNpc)
					{
						hitInfo.HitPositionLocal = ((Component)entity).transform.InverseTransformPoint(hitInfo.HitPositionWorld);
						hitInfo.HitNormalLocal = ((Component)entity).transform.InverseTransformDirection(hitInfo.HitNormalWorld);
						hitInfo.HitMaterial = StringPool.Get("Flesh");
						Effect.server.ImpactEffect(hitInfo);
					}
				}
				if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
				{
					break;
				}
			}
			Pool.FreeList<RaycastHit>(ref list);
			Vector3 val2 = ((flag && ownerPlayer.isMounted) ? (modifiedAimConeDirection * 6f) : Vector3.zero);
			CreateProjectileEffectClientside(component.projectileObject.resourcePath, val + val2, modifiedAimConeDirection * component.projectileVelocity, Random.Range(1, 100), null, IsSilenced(), forceClientsideEffects: true);
		}
	}

	private void AssignInitiator(HitInfo info)
	{
		info.Initiator = GetOwnerPlayer();
		if ((Object)(object)info.Initiator == (Object)null)
		{
			info.Initiator = GetParentEntity();
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		primaryMagazine.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)DelayedModSetup, 0.1f);
	}

	public void DelayedModSetup()
	{
		if (!modsChangedInitialized)
		{
			Item item = GetCachedItem();
			if (item != null && item.contents != null)
			{
				ItemContainer contents = item.contents;
				contents.onItemAddedRemoved = (Action<Item, bool>)Delegate.Combine(contents.onItemAddedRemoved, new Action<Item, bool>(ModsChanged));
				modsChangedInitialized = true;
			}
		}
	}

	public override void DestroyShared()
	{
		if (base.isServer)
		{
			Item item = GetCachedItem();
			if (item != null && item.contents != null)
			{
				ItemContainer contents = item.contents;
				contents.onItemAddedRemoved = (Action<Item, bool>)Delegate.Remove(contents.onItemAddedRemoved, new Action<Item, bool>(ModsChanged));
				modsChangedInitialized = false;
			}
		}
		base.DestroyShared();
	}

	public void ModsChanged(Item item, bool added)
	{
		((FacepunchBehaviour)this).Invoke((Action)DelayedModsChanged, 0.1f);
	}

	public void ForceModsChanged()
	{
		((FacepunchBehaviour)this).Invoke((Action)DelayedModSetup, 0f);
		((FacepunchBehaviour)this).Invoke((Action)DelayedModsChanged, 0.2f);
	}

	public void DelayedModsChanged()
	{
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("DelayedModsChanged");
		float num = ProjectileWeaponMod.Mult(this, (ProjectileWeaponMod x) => x.magazineCapacity, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * (float)primaryMagazine.definition.builtInSize;
		int num2 = Mathf.CeilToInt(num);
		if (num2 == primaryMagazine.capacity)
		{
			return;
		}
		if (primaryMagazine.contents > 0 && primaryMagazine.contents > num2)
		{
			ItemDefinition ammoType = primaryMagazine.ammoType;
			int contents = primaryMagazine.contents;
			BasePlayer ownerPlayer = GetOwnerPlayer();
			ItemContainer itemContainer = null;
			if ((Object)(object)ownerPlayer != (Object)null)
			{
				itemContainer = ownerPlayer.inventory.containerMain;
			}
			else if (GetCachedItem() != null)
			{
				itemContainer = GetCachedItem().parent;
			}
			primaryMagazine.contents = 0;
			if (itemContainer != null)
			{
				Item item = ItemManager.Create(primaryMagazine.ammoType, contents, 0uL);
				if (!item.MoveToContainer(itemContainer))
				{
					Vector3 vPos = ((Component)this).transform.position;
					if ((Object)(object)itemContainer.entityOwner != (Object)null)
					{
						vPos = ((Component)itemContainer.entityOwner).transform.position + Vector3.up * 0.25f;
					}
					item.Drop(vPos, Vector3.up * 5f);
				}
			}
		}
		primaryMagazine.capacity = num2;
		SendNetworkUpdate();
		Profiler.EndSample();
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (item != null && command == "unload_ammo" && !HasReloadCooldown())
		{
			UnloadAmmo(item, player);
		}
	}

	public void UnloadAmmo(Item item, BasePlayer player)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity heldEntity = item.GetHeldEntity();
		BaseProjectile component = ((Component)heldEntity).GetComponent<BaseProjectile>();
		if (!component.canUnloadAmmo || !Object.op_Implicit((Object)(object)component))
		{
			return;
		}
		int contents = component.primaryMagazine.contents;
		if (contents > 0)
		{
			component.primaryMagazine.contents = 0;
			SendNetworkUpdateImmediate();
			Item item2 = ItemManager.Create(component.primaryMagazine.ammoType, contents, 0uL);
			if (!item2.MoveToContainer(player.inventory.containerMain))
			{
				item2.Drop(player.GetDropPosition(), player.GetDropVelocity());
			}
		}
	}

	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		if (!((Object)(object)crafter == (Object)null) && item != null)
		{
			UnloadAmmo(item, crafter);
		}
	}

	public override void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
		if (!((Object)(object)crafter == (Object)null) && item != null)
		{
			BaseEntity heldEntity = item.GetHeldEntity();
			BaseProjectile component = ((Component)heldEntity).GetComponent<BaseProjectile>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.primaryMagazine.contents = 0;
			}
		}
	}

	public override void SetLightsOn(bool isOn)
	{
		base.SetLightsOn(isOn);
		UpdateAttachmentsState();
	}

	public void UpdateAttachmentsState()
	{
		Flags flags = base.flags;
		bool b = ShouldLightsBeOn();
		if (children == null)
		{
			return;
		}
		foreach (ProjectileWeaponMod item in from ProjectileWeaponMod x in children
			where (Object)(object)x != (Object)null && x.isLight
			select x)
		{
			item.SetFlag(Flags.On, b);
		}
	}

	private bool ShouldLightsBeOn()
	{
		return LightsOn() && (IsDeployed() || parentEntity.Get(base.isServer) is AutoTurret);
	}

	protected override void OnChildRemoved(BaseEntity child)
	{
		base.OnChildRemoved(child);
		if (child is ProjectileWeaponMod projectileWeaponMod && projectileWeaponMod.isLight)
		{
			child.SetFlag(Flags.On, b: false);
			SetLightsOn(isOn: false);
		}
	}

	public bool CanAiAttack()
	{
		return true;
	}

	public virtual float GetAimCone()
	{
		uint num = 0u;
		foreach (BaseEntity child in children)
		{
			num += (uint)(int)child.net.ID.Value;
			num += (uint)child.flags;
		}
		uint num2 = CRC.Compute32(0u, num);
		if (num2 != cachedModHash)
		{
			sightAimConeScale = ProjectileWeaponMod.Mult(this, (ProjectileWeaponMod x) => x.sightAimCone, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
			sightAimConeOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.sightAimCone, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
			hipAimConeScale = ProjectileWeaponMod.Mult(this, (ProjectileWeaponMod x) => x.hipAimCone, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
			hipAimConeOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.hipAimCone, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
			cachedModHash = num2;
		}
		float num3 = aimCone;
		num3 *= (UsingInternalBurstMode() ? internalBurstAimConeScale : 1f);
		if ((Object)(object)recoilProperties != (Object)null && recoilProperties.overrideAimconeWithCurve && primaryMagazine.capacity > 0)
		{
			num3 += recoilProperties.aimconeCurve.Evaluate((float)numShotsFired / (float)primaryMagazine.capacity % 1f) * recoilProperties.aimconeCurveScale;
			aimconePenalty = 0f;
		}
		if (aiming || base.isServer)
		{
			return (num3 + aimconePenalty + stancePenalty * stancePenaltyScale) * sightAimConeScale + sightAimConeOffset;
		}
		return (num3 + aimconePenalty + stancePenalty * stancePenaltyScale) * sightAimConeScale + sightAimConeOffset + hipAimCone * hipAimConeScale + hipAimConeOffset;
	}

	public float ScaleRepeatDelay(float delay)
	{
		float num = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.repeatDelay, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
		float num2 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.repeatDelay, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		float num3 = (UsingInternalBurstMode() ? internalBurstFireRateScale : 1f);
		return delay * num * num3 + num2;
	}

	public Projectile.Modifier GetProjectileModifier()
	{
		Projectile.Modifier result = default(Projectile.Modifier);
		result.damageOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.projectileDamage, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		result.damageScale = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.projectileDamage, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * GetDamageScale();
		result.distanceOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.projectileDistance, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		result.distanceScale = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.projectileDistance, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * GetDistanceScale();
		return result;
	}

	public bool UsingBurstMode()
	{
		if (IsBurstDisabled())
		{
			return false;
		}
		return isBurstWeapon || (children != null && (Object)(object)(from ProjectileWeaponMod x in children
			where (Object)(object)x != (Object)null && x.burstCount > 0
			select x).FirstOrDefault() != (Object)null);
	}

	public bool UsingInternalBurstMode()
	{
		if (IsBurstDisabled())
		{
			return false;
		}
		return isBurstWeapon;
	}

	public bool IsBurstEligable()
	{
		return isBurstWeapon || (children != null && (Object)(object)(from ProjectileWeaponMod x in children
			where (Object)(object)x != (Object)null && x.burstCount > 0
			select x).FirstOrDefault() != (Object)null);
	}

	public float TimeBetweenBursts()
	{
		return repeatDelay * 3f;
	}

	public float GetReloadDuration()
	{
		if (fractionalReload)
		{
			int num = Mathf.Min(primaryMagazine.capacity - primaryMagazine.contents, GetAvailableAmmo());
			return reloadStartDuration + reloadEndDuration + reloadFractionDuration * (float)num;
		}
		return reloadTime;
	}

	public int GetAvailableAmmo()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)ownerPlayer == (Object)null)
		{
			return primaryMagazine.capacity;
		}
		List<Item> list = Pool.GetList<Item>();
		ownerPlayer.inventory.FindAmmo(list, primaryMagazine.definition.ammoTypes);
		int num = 0;
		if (list.Count != 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				Item item = list[i];
				if ((Object)(object)item.info == (Object)(object)primaryMagazine.ammoType)
				{
					num += item.amount;
				}
			}
		}
		Pool.FreeList<Item>(ref list);
		return num;
	}

	public bool IsBurstDisabled()
	{
		return HasFlag(Flags.Reserved6) == defaultOn;
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.CallsPerSecond(2uL)]
	private void ToggleFireMode(RPCMessage msg)
	{
		if (canChangeFireModes && IsBurstEligable())
		{
			SetFlag(Flags.Reserved6, !HasFlag(Flags.Reserved6));
			SendNetworkUpdate_Flags();
			BasePlayer ownerPlayer = GetOwnerPlayer();
			if (!ownerPlayer.IsNpc && ownerPlayer.IsConnected)
			{
				ownerPlayer.ShowToast(GameTip.Styles.Blue_Short, IsBurstDisabled() ? Toast_BurstDisabled : Toast_BurstEnabled);
			}
		}
	}

	protected virtual void ReloadMagazine(int desiredAmount = -1)
	{
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (Object.op_Implicit((Object)(object)ownerPlayer))
		{
			primaryMagazine.Reload(ownerPlayer, desiredAmount);
			SendNetworkUpdateImmediate();
			ItemManager.DoRemoves();
			ownerPlayer.inventory.ServerUpdate(0f);
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void SwitchAmmoTo(RPCMessage msg)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num == primaryMagazine.ammoType.itemid)
		{
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(num);
		if ((Object)(object)itemDefinition == (Object)null)
		{
			return;
		}
		ItemModProjectile component = ((Component)itemDefinition).GetComponent<ItemModProjectile>();
		if (Object.op_Implicit((Object)(object)component) && component.IsAmmo(primaryMagazine.definition.ammoTypes))
		{
			if (primaryMagazine.contents > 0)
			{
				ownerPlayer.GiveItem(ItemManager.CreateByItemID(primaryMagazine.ammoType.itemid, primaryMagazine.contents, 0uL));
				primaryMagazine.contents = 0;
			}
			primaryMagazine.ammoType = itemDefinition;
			SendNetworkUpdateImmediate();
			ItemManager.DoRemoves();
			ownerPlayer.inventory.ServerUpdate(0f);
		}
	}

	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		reloadStarted = false;
		reloadFinished = false;
		fractionalInsertCounter = 0;
		UpdateAttachmentsState();
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void StartReload(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!VerifyClientRPC(player))
		{
			SendNetworkUpdate();
			reloadStarted = false;
			reloadFinished = false;
			return;
		}
		reloadFinished = false;
		reloadStarted = true;
		fractionalInsertCounter = 0;
		if (CanRefundAmmo)
		{
			primaryMagazine.SwitchAmmoTypesIfNeeded(player);
		}
		StartReloadCooldown(GetReloadDuration());
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void ServerFractionalReloadInsert(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!VerifyClientRPC(player))
		{
			SendNetworkUpdate();
			reloadStarted = false;
			reloadFinished = false;
			return;
		}
		if (!fractionalReload)
		{
			AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload not allowed (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_type");
			return;
		}
		if (!reloadStarted)
		{
			AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload request skipped (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_skip");
			reloadStarted = false;
			reloadFinished = false;
			return;
		}
		if (GetReloadIdle() > 3f)
		{
			AntiHack.Log(player, AntiHackType.ReloadHack, "T+" + GetReloadIdle() + "s (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_time");
			reloadStarted = false;
			reloadFinished = false;
			return;
		}
		if (Time.time < startReloadTime + reloadStartDuration)
		{
			AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload too early (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_fraction_too_early");
			reloadStarted = false;
			reloadFinished = false;
		}
		if (Time.time < startReloadTime + reloadStartDuration + (float)fractionalInsertCounter * reloadFractionDuration)
		{
			AntiHack.Log(player, AntiHackType.ReloadHack, "Fractional reload rate too high (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_fraction_rate");
			reloadStarted = false;
			reloadFinished = false;
		}
		else
		{
			fractionalInsertCounter++;
			if (primaryMagazine.contents < primaryMagazine.capacity)
			{
				ReloadMagazine(1);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void Reload(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!VerifyClientRPC(player))
		{
			SendNetworkUpdate();
			reloadStarted = false;
			reloadFinished = false;
			return;
		}
		if (!reloadStarted)
		{
			AntiHack.Log(player, AntiHackType.ReloadHack, "Request skipped (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_skip");
			reloadStarted = false;
			reloadFinished = false;
			return;
		}
		if (!fractionalReload)
		{
			if (GetReloadCooldown() > 1f)
			{
				AntiHack.Log(player, AntiHackType.ReloadHack, "T-" + GetReloadCooldown() + "s (" + base.ShortPrefabName + ")");
				player.stats.combat.LogInvalid(player, this, "reload_time");
				reloadStarted = false;
				reloadFinished = false;
				return;
			}
			if (GetReloadIdle() > 1.5f)
			{
				AntiHack.Log(player, AntiHackType.ReloadHack, "T+" + GetReloadIdle() + "s (" + base.ShortPrefabName + ")");
				player.stats.combat.LogInvalid(player, this, "reload_time");
				reloadStarted = false;
				reloadFinished = false;
				return;
			}
		}
		if (fractionalReload)
		{
			ResetReloadCooldown();
		}
		reloadStarted = false;
		reloadFinished = true;
		if (!fractionalReload)
		{
			ReloadMagazine();
		}
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.IsActiveItem]
	private void CLProject(RPCMessage msg)
	{
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!VerifyClientAttack(player))
		{
			SendNetworkUpdate();
			return;
		}
		if (reloadFinished && HasReloadCooldown())
		{
			AntiHack.Log(player, AntiHackType.ProjectileHack, "Reloading (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_cooldown");
			return;
		}
		reloadStarted = false;
		reloadFinished = false;
		if (primaryMagazine.contents <= 0 && !base.UsingInfiniteAmmoCheat)
		{
			AntiHack.Log(player, AntiHackType.ProjectileHack, "Magazine empty (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "ammo_missing");
			return;
		}
		ItemDefinition primaryMagazineAmmo = PrimaryMagazineAmmo;
		ProjectileShoot val = ProjectileShoot.Deserialize((Stream)(object)msg.read);
		if (primaryMagazineAmmo.itemid != val.ammoType)
		{
			AntiHack.Log(player, AntiHackType.ProjectileHack, "Ammo mismatch (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "ammo_mismatch");
			return;
		}
		if (!base.UsingInfiniteAmmoCheat)
		{
			primaryMagazine.contents--;
		}
		ItemModProjectile component = ((Component)primaryMagazineAmmo).GetComponent<ItemModProjectile>();
		if ((Object)(object)component == (Object)null)
		{
			AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "mod_missing");
		}
		else if (val.projectiles.Count > component.numProjectiles)
		{
			AntiHack.Log(player, AntiHackType.ProjectileHack, "Count mismatch (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "count_mismatch");
		}
		else
		{
			if (player.InGesture)
			{
				return;
			}
			SignalBroadcast(Signal.Attack, string.Empty, msg.connection);
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
					Vector3 position = player.eyes.position;
					positionOffset = position - projectile.startPos;
					projectile.startPos = position;
				}
				else if (!ValidateEyePos(player, projectile.startPos))
				{
					continue;
				}
				player.NoteFiredProjectile(projectile.projectileID, projectile.startPos, projectile.startVel, this, primaryMagazineAmmo, projectileGroupId, positionOffset);
				CreateProjectileEffectClientside(component.projectileObject.resourcePath, projectile.startPos, projectile.startVel, projectile.seed, msg.connection, IsSilenced());
			}
			player.MakeNoise(((Component)player).transform.position, BaseCombatEntity.ActionVolume.Loud);
			player.stats.Add(component.category + "_fired", val.projectiles.Count(), (Stats)5);
			player.LifeStoryShotFired(this);
			StartAttackCooldown(ScaleRepeatDelay(repeatDelay) + animationDelay);
			player.MarkHostileFor();
			UpdateItemCondition();
			DidAttackServerside();
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
			float num2 = NoiseRadius;
			if (IsSilenced())
			{
				num2 *= AI.npc_gun_noise_silencer_modifier;
			}
			Sensation sensation = default(Sensation);
			sensation.Type = SensationType.Gunshot;
			sensation.Position = ((Component)player).transform.position;
			sensation.Radius = num2;
			sensation.DamagePotential = num;
			sensation.InitiatorPlayer = player;
			sensation.Initiator = player;
			Sense.Stimulate(sensation);
			EACServer.LogPlayerUseWeapon(player, this);
		}
	}

	private void CreateProjectileEffectClientside(string prefabName, Vector3 pos, Vector3 velocity, int seed, Connection sourceConnection, bool silenced = false, bool forceClientsideEffects = false)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Effect effect = reusableInstance;
		effect.Clear();
		effect.Init(Effect.Type.Projectile, pos, velocity, sourceConnection);
		((EffectData)effect).scale = (silenced ? 0f : 1f);
		if (forceClientsideEffects)
		{
			((EffectData)effect).scale = 2f;
		}
		effect.pooledString = prefabName;
		((EffectData)effect).number = seed;
		EffectNetwork.Send(effect);
	}

	public void UpdateItemCondition()
	{
		Item ownerItem = GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ItemModProjectile component = ((Component)primaryMagazine.ammoType).GetComponent<ItemModProjectile>();
		float barrelConditionLoss = component.barrelConditionLoss;
		float num = 0.25f;
		bool usingInfiniteAmmoCheat = base.UsingInfiniteAmmoCheat;
		if (!usingInfiniteAmmoCheat)
		{
			ownerItem.LoseCondition(num + barrelConditionLoss);
		}
		if (ownerItem.contents == null || ownerItem.contents.itemList == null)
		{
			return;
		}
		for (int num2 = ownerItem.contents.itemList.Count - 1; num2 >= 0; num2--)
		{
			Item item = ownerItem.contents.itemList[num2];
			if (item != null && !usingInfiniteAmmoCheat)
			{
				item.LoseCondition(num + barrelConditionLoss);
			}
		}
	}

	public bool IsSilenced()
	{
		Profiler.BeginSample("IsSilenced");
		if (children != null)
		{
			foreach (BaseEntity child in children)
			{
				ProjectileWeaponMod projectileWeaponMod = child as ProjectileWeaponMod;
				if ((Object)(object)projectileWeaponMod != (Object)null && projectileWeaponMod.isSilencer && !projectileWeaponMod.IsBroken())
				{
					Profiler.EndSample();
					return true;
				}
			}
		}
		Profiler.EndSample();
		return false;
	}

	public override bool CanUseNetworkCache(Connection sendingTo)
	{
		Connection ownerConnection = GetOwnerConnection();
		if (sendingTo == null || ownerConnection == null)
		{
			return true;
		}
		return sendingTo != ownerConnection;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Pool.Get<BaseProjectile>();
		if (info.forDisk || info.SendingTo(GetOwnerConnection()) || ForceSendMagazine(info))
		{
			info.msg.baseProjectile.primaryMagazine = primaryMagazine.Save();
		}
	}

	public virtual bool ForceSendMagazine(SaveInfo saveInfo)
	{
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (Object.op_Implicit((Object)(object)ownerPlayer) && ownerPlayer.IsBeingSpectated)
		{
			foreach (BaseEntity child in ownerPlayer.children)
			{
				if (child.net != null && child.net.connection == saveInfo.forConnection)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			primaryMagazine.Load(info.msg.baseProjectile.primaryMagazine);
		}
	}
}
