using Network;
using Rust;
using UnityEngine;

public class Effect : EffectData
{
	public enum Type : uint
	{
		Generic,
		Projectile,
		GenericGlobal
	}

	public static class client
	{
		public static void Run(Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
		}

		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal)
		{
			if (!string.IsNullOrEmpty(strName))
			{
			}
		}

		public static void Run(Type fxtype, Vector3 posWorld, Vector3 normWorld, Vector3 up = default(Vector3))
		{
		}

		public static void Run(string strName, Vector3 posWorld = default(Vector3), Vector3 normWorld = default(Vector3), Vector3 up = default(Vector3), Type overrideType = Type.Generic)
		{
			if (!string.IsNullOrEmpty(strName))
			{
			}
		}

		public static void Run(string strName, GameObject obj)
		{
			if (!string.IsNullOrEmpty(strName))
			{
			}
		}

		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			if (info.HitEntity.IsValid())
			{
				Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal + info.HitNormalLocal * 0.1f, info.HitNormalLocal);
			}
			else
			{
				Run(effectName, info.HitPositionWorld + info.HitNormalWorld * 0.1f, info.HitNormalWorld);
			}
		}

		public static void ImpactEffect(HitInfo info)
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0164: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0200: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01db: Unknown result type (might be due to invalid IL or missing references)
			if (!info.DoHitEffects)
			{
				return;
			}
			string materialName = StringPool.Get(info.HitMaterial);
			string strName = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), materialName);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), materialName);
			if ((Object)(object)TerrainMeta.WaterMap != (Object)null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < TerrainMeta.WaterMap.GetHeight(info.HitPositionWorld) && WaterLevel.Test(info.HitPositionWorld, waves: false, volumes: false))
			{
				return;
			}
			if (info.HitEntity.IsValid())
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					strName = impactEffect.resourcePath;
				}
				Run(strName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				if (info.DoDecals)
				{
					Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
				}
			}
			else
			{
				Type overrideType = Type.Generic;
				Run(strName, info.HitPositionWorld, info.HitNormalWorld, default(Vector3), overrideType);
				Run(decal, info.HitPositionWorld, info.HitNormalWorld, default(Vector3), overrideType);
			}
			if (Object.op_Implicit((Object)(object)info.WeaponPrefab))
			{
				BaseMelee baseMelee = info.WeaponPrefab as BaseMelee;
				if ((Object)(object)baseMelee != (Object)null)
				{
					string strikeEffectPath = baseMelee.GetStrikeEffectPath(materialName);
					if (info.HitEntity.IsValid())
					{
						Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal);
					}
					else
					{
						Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld);
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}
	}

	public static class server
	{
		public static void Run(Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			reusableInstace.Init(fxtype, ent, boneID, posLocal, normLocal, sourceConnection);
			reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(reusableInstace);
		}

		public static void Run(string strName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null, bool broadcast = false)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(strName))
			{
				reusableInstace.Init(Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
				reusableInstace.pooledString = strName;
				reusableInstace.broadcast = broadcast;
				EffectNetwork.Send(reusableInstace);
			}
		}

		public static void Run(Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null, bool broadcast = false)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			reusableInstace.Init(fxtype, posWorld, normWorld, sourceConnection);
			reusableInstace.broadcast = broadcast;
			EffectNetwork.Send(reusableInstace);
		}

		public static void Run(string strName, Vector3 posWorld = default(Vector3), Vector3 normWorld = default(Vector3), Connection sourceConnection = null, bool broadcast = false)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(strName))
			{
				reusableInstace.Init(Type.Generic, posWorld, normWorld, sourceConnection);
				reusableInstace.pooledString = strName;
				reusableInstace.broadcast = broadcast;
				EffectNetwork.Send(reusableInstace);
			}
		}

		public static void DoAdditiveImpactEffect(HitInfo info, string effectName)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (info.HitEntity.IsValid())
			{
				Run(effectName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted);
			}
			else
			{
				Run(effectName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted);
			}
		}

		public static void ImpactEffect(HitInfo info)
		{
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0207: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0221: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_02af: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0296: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			if (!info.DoHitEffects)
			{
				return;
			}
			string materialName = StringPool.Get(info.HitMaterial);
			if ((Object)(object)TerrainMeta.WaterMap != (Object)null && info.HitMaterial != Projectile.WaterMaterialID() && info.HitMaterial != Projectile.FleshMaterialID() && info.HitPositionWorld.y < TerrainMeta.WaterMap.GetHeight(info.HitPositionWorld) && WaterLevel.Test(info.HitPositionWorld, waves: false, volumes: false))
			{
				return;
			}
			string strName = EffectDictionary.GetParticle(info.damageTypes.GetMajorityDamageType(), materialName);
			string decal = EffectDictionary.GetDecal(info.damageTypes.GetMajorityDamageType(), materialName);
			if (info.HitEntity.IsValid())
			{
				GameObjectRef impactEffect = info.HitEntity.GetImpactEffect(info);
				if (impactEffect.isValid)
				{
					strName = impactEffect.resourcePath;
				}
				Bounds bounds = info.HitEntity.bounds;
				float num = info.HitEntity.BoundsPadding();
				((Bounds)(ref bounds)).extents = ((Bounds)(ref bounds)).extents + new Vector3(num, num, num);
				if (!((Bounds)(ref bounds)).Contains(info.HitPositionLocal))
				{
					BasePlayer initiatorPlayer = info.InitiatorPlayer;
					if ((Object)(object)initiatorPlayer != (Object)null && ((object)initiatorPlayer).GetType() == typeof(BasePlayer))
					{
						float num2 = Mathf.Sqrt(((Bounds)(ref bounds)).SqrDistance(info.HitPositionLocal));
						AntiHack.Log(initiatorPlayer, AntiHackType.EffectHack, $"Tried to run an impact effect outside of entity '{info.HitEntity.ShortPrefabName}' bounds by {num2}m");
					}
					return;
				}
				Run(strName, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted);
				Run(decal, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted);
			}
			else
			{
				Run(strName, info.HitPositionWorld, info.HitNormalWorld, info.Predicted);
				Run(decal, info.HitPositionWorld, info.HitNormalWorld, info.Predicted);
			}
			if (Object.op_Implicit((Object)(object)info.WeaponPrefab))
			{
				BaseMelee baseMelee = info.WeaponPrefab as BaseMelee;
				if ((Object)(object)baseMelee != (Object)null)
				{
					string strikeEffectPath = baseMelee.GetStrikeEffectPath(materialName);
					if (info.HitEntity.IsValid())
					{
						Run(strikeEffectPath, info.HitEntity, info.HitBone, info.HitPositionLocal, info.HitNormalLocal, info.Predicted);
					}
					else
					{
						Run(strikeEffectPath, info.HitPositionWorld, info.HitNormalWorld, info.Predicted);
					}
				}
			}
			if (info.damageTypes.Has(DamageType.Explosion))
			{
				DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/explosion.prefab");
			}
			if (info.damageTypes.Has(DamageType.Heat))
			{
				DoAdditiveImpactEffect(info, "assets/bundled/prefabs/fx/impacts/additive/fire.prefab");
			}
		}
	}

	public Vector3 Up;

	public Vector3 worldPos;

	public Vector3 worldNrm;

	public bool attached;

	public Transform transform;

	public GameObject gameObject;

	public string pooledString;

	public bool broadcast;

	private static Effect reusableInstace = new Effect();

	public Effect()
	{
	}

	public Effect(string effectName, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Init(Type.Generic, posWorld, normWorld, sourceConnection);
		pooledString = effectName;
	}

	public Effect(string effectName, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Init(Type.Generic, ent, boneID, posLocal, normLocal, sourceConnection);
		pooledString = effectName;
	}

	public void Init(Type fxtype, BaseEntity ent, uint boneID, Vector3 posLocal, Vector3 normLocal, Connection sourceConnection = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Clear();
		base.type = (uint)fxtype;
		attached = true;
		base.origin = posLocal;
		base.normal = normLocal;
		gameObject = null;
		Up = Vector3.zero;
		if ((Object)(object)ent != (Object)null && !ent.IsValid())
		{
			Debug.LogWarning((object)"Effect.Init - invalid entity");
		}
		base.entity = (NetworkableId)(ent.IsValid() ? ent.net.ID : default(NetworkableId));
		base.source = sourceConnection?.userid ?? 0;
		base.bone = boneID;
	}

	public void Init(Type fxtype, Vector3 posWorld, Vector3 normWorld, Connection sourceConnection = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Clear();
		base.type = (uint)fxtype;
		attached = false;
		worldPos = posWorld;
		worldNrm = normWorld;
		gameObject = null;
		Up = Vector3.zero;
		base.entity = default(NetworkableId);
		base.origin = worldPos;
		base.normal = worldNrm;
		base.bone = 0u;
		base.source = sourceConnection?.userid ?? 0;
	}

	public void Clear()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		worldPos = Vector3.zero;
		worldNrm = Vector3.zero;
		attached = false;
		transform = null;
		gameObject = null;
		pooledString = null;
		broadcast = false;
	}
}
