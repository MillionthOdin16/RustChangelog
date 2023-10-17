using System;
using ConVar;
using Facepunch.Rust;
using Network;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.Assertions;

public class ThrownWeapon : AttackEntity
{
	[Header("Throw Weapon")]
	public GameObjectRef prefabToThrow;

	public float maxThrowVelocity = 10f;

	public float tumbleVelocity = 0f;

	public Vector3 overrideAngle = Vector3.zero;

	public bool canStick = true;

	public bool canThrowUnderwater = true;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ThrownWeapon.OnRpcMessage", 0);
		try
		{
			if (rpc == 1513023343 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoDrop "));
				}
				TimeWarning val2 = TimeWarning.New("DoDrop", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1513023343u, "DoDrop", this, player))
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
							DoDrop(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoDrop");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1974840882 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoThrow "));
				}
				TimeWarning val5 = TimeWarning.New("DoThrow", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1974840882u, "DoThrow", this, player))
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
							DoThrow(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in DoThrow");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
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
		return player.GetInheritedThrowVelocity(direction);
	}

	public void ServerThrow(Vector3 targetPosition)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient || !HasItemAmount() || HasAttackCooldown())
		{
			return;
		}
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)ownerPlayer == (Object)null || (!canThrowUnderwater && ownerPlayer.IsHeadUnderwater()))
		{
			return;
		}
		Vector3 position = ownerPlayer.eyes.position;
		Vector3 val = ownerPlayer.eyes.BodyForward();
		float num = 1f;
		SignalBroadcast(Signal.Throw, string.Empty);
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToThrow.resourcePath, position, Quaternion.LookRotation((overrideAngle == Vector3.zero) ? (-val) : overrideAngle));
		if ((Object)(object)baseEntity == (Object)null)
		{
			return;
		}
		baseEntity.SetCreatorEntity(ownerPlayer);
		Vector3 val2 = val + Quaternion.AngleAxis(10f, Vector3.right) * Vector3.up;
		float num2 = GetThrowVelocity(position, targetPosition, val2);
		if (float.IsNaN(num2))
		{
			val2 = val + Quaternion.AngleAxis(20f, Vector3.right) * Vector3.up;
			num2 = GetThrowVelocity(position, targetPosition, val2);
			if (float.IsNaN(num2))
			{
				num2 = 5f;
			}
		}
		baseEntity.SetVelocity(val2 * num2 * num);
		if (tumbleVelocity > 0f)
		{
			baseEntity.SetAngularVelocity(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * tumbleVelocity);
		}
		baseEntity.Spawn();
		StartAttackCooldown(repeatDelay);
		UseItemAmount(1);
		TimedExplosive timedExplosive = baseEntity as TimedExplosive;
		if ((Object)(object)timedExplosive != (Object)null)
		{
			Analytics.Azure.OnExplosiveLaunched(ownerPlayer, baseEntity);
			float num3 = 0f;
			foreach (DamageTypeEntry damageType in timedExplosive.damageTypes)
			{
				num3 += damageType.amount;
			}
			Sensation sensation = default(Sensation);
			sensation.Type = SensationType.ThrownWeapon;
			sensation.Position = ((Component)ownerPlayer).transform.position;
			sensation.Radius = 50f;
			sensation.DamagePotential = num3;
			sensation.InitiatorPlayer = ownerPlayer;
			sensation.Initiator = ownerPlayer;
			sensation.UsedEntity = timedExplosive;
			Sense.Stimulate(sensation);
		}
		else
		{
			Sensation sensation = default(Sensation);
			sensation.Type = SensationType.ThrownWeapon;
			sensation.Position = ((Component)ownerPlayer).transform.position;
			sensation.Radius = 50f;
			sensation.DamagePotential = 0f;
			sensation.InitiatorPlayer = ownerPlayer;
			sensation.Initiator = ownerPlayer;
			sensation.UsedEntity = this;
			Sense.Stimulate(sensation);
		}
	}

	private float GetThrowVelocity(Vector3 throwPos, Vector3 targetPos, Vector3 aimDir)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = targetPos - throwPos;
		Vector2 val2 = new Vector2(val.x, val.z);
		float magnitude = ((Vector2)(ref val2)).magnitude;
		float y = val.y;
		val2 = new Vector2(aimDir.x, aimDir.z);
		float magnitude2 = ((Vector2)(ref val2)).magnitude;
		float y2 = aimDir.y;
		float y3 = Physics.gravity.y;
		return Mathf.Sqrt(0.5f * y3 * magnitude * magnitude / (magnitude2 * (magnitude2 * y - y2 * magnitude)));
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void DoThrow(RPCMessage msg)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		if (!HasItemAmount() || HasAttackCooldown())
		{
			return;
		}
		Vector3 val = msg.read.Vector3();
		Vector3 val2 = msg.read.Vector3();
		Vector3 normalized = ((Vector3)(ref val2)).normalized;
		float num = Mathf.Clamp01(msg.read.Float());
		if (msg.player.isMounted || msg.player.HasParent())
		{
			val = msg.player.eyes.position;
		}
		else if (!ValidateEyePos(msg.player, val))
		{
			return;
		}
		if (!canThrowUnderwater && msg.player.IsHeadUnderwater())
		{
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToThrow.resourcePath, val, Quaternion.LookRotation((overrideAngle == Vector3.zero) ? (-normalized) : overrideAngle));
		if ((Object)(object)baseEntity == (Object)null)
		{
			return;
		}
		Item ownerItem = GetOwnerItem();
		if (ownerItem != null && ownerItem.instanceData != null && ownerItem.HasFlag(Item.Flag.IsOn))
		{
			((Component)baseEntity).gameObject.SendMessage("SetFrequency", (object)GetOwnerItem().instanceData.dataInt, (SendMessageOptions)1);
		}
		baseEntity.SetCreatorEntity(msg.player);
		baseEntity.skinID = skinID;
		baseEntity.SetVelocity(GetInheritedVelocity(msg.player, normalized) + normalized * maxThrowVelocity * num + msg.player.estimatedVelocity * 0.5f);
		if (tumbleVelocity > 0f)
		{
			baseEntity.SetAngularVelocity(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * tumbleVelocity);
		}
		baseEntity.Spawn();
		SetUpThrownWeapon(baseEntity);
		StartAttackCooldown(repeatDelay);
		UseItemAmount(1);
		BasePlayer player = msg.player;
		if (!((Object)(object)player != (Object)null))
		{
			return;
		}
		TimedExplosive timedExplosive = baseEntity as TimedExplosive;
		if ((Object)(object)timedExplosive != (Object)null)
		{
			float num2 = 0f;
			foreach (DamageTypeEntry damageType in timedExplosive.damageTypes)
			{
				num2 += damageType.amount;
			}
			Sensation sensation = default(Sensation);
			sensation.Type = SensationType.ThrownWeapon;
			sensation.Position = ((Component)player).transform.position;
			sensation.Radius = 50f;
			sensation.DamagePotential = num2;
			sensation.InitiatorPlayer = player;
			sensation.Initiator = player;
			sensation.UsedEntity = timedExplosive;
			Sense.Stimulate(sensation);
		}
		else
		{
			Sensation sensation = default(Sensation);
			sensation.Type = SensationType.ThrownWeapon;
			sensation.Position = ((Component)player).transform.position;
			sensation.Radius = 50f;
			sensation.DamagePotential = 0f;
			sensation.InitiatorPlayer = player;
			sensation.Initiator = player;
			sensation.UsedEntity = this;
			Sense.Stimulate(sensation);
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void DoDrop(RPCMessage msg)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		if (!HasItemAmount() || HasAttackCooldown() || (!canThrowUnderwater && msg.player.IsHeadUnderwater()))
		{
			return;
		}
		Vector3 val = msg.read.Vector3();
		Vector3 val2 = msg.read.Vector3();
		Vector3 normalized = ((Vector3)(ref val2)).normalized;
		if (msg.player.isMounted || msg.player.HasParent())
		{
			val = msg.player.eyes.position;
		}
		else if (!ValidateEyePos(msg.player, val))
		{
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToThrow.resourcePath, val, Quaternion.LookRotation(Vector3.up));
		if ((Object)(object)baseEntity == (Object)null)
		{
			return;
		}
		RaycastHit hit = default(RaycastHit);
		if (canStick && Physics.SphereCast(new Ray(val, normalized), 0.05f, ref hit, 1.5f, 1237003025))
		{
			Vector3 point = ((RaycastHit)(ref hit)).point;
			Vector3 normal = ((RaycastHit)(ref hit)).normal;
			BaseEntity entity = hit.GetEntity();
			Collider collider = ((RaycastHit)(ref hit)).collider;
			if (Object.op_Implicit((Object)(object)entity) && entity is StabilityEntity && baseEntity is TimedExplosive)
			{
				entity = entity.ToServer<BaseEntity>();
				TimedExplosive timedExplosive = baseEntity as TimedExplosive;
				timedExplosive.onlyDamageParent = true;
				timedExplosive.DoStick(point, normal, entity, collider);
				Analytics.Azure.OnExplosiveLaunched(msg.player, timedExplosive);
			}
			else
			{
				baseEntity.SetVelocity(normalized);
			}
		}
		else
		{
			baseEntity.SetVelocity(normalized);
		}
		baseEntity.creatorEntity = msg.player;
		baseEntity.skinID = skinID;
		baseEntity.Spawn();
		SetUpThrownWeapon(baseEntity);
		StartAttackCooldown(repeatDelay);
		UseItemAmount(1);
	}

	protected virtual void SetUpThrownWeapon(BaseEntity ent)
	{
	}
}
