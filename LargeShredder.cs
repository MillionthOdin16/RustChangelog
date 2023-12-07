using System;
using Rust;
using UnityEngine;

public class LargeShredder : BaseEntity
{
	public Transform shredRail;

	public Transform shredRailStartPos;

	public Transform shredRailEndPos;

	public Vector3 shredRailStartRotation;

	public Vector3 shredRailEndRotation;

	public LargeShredderTrigger trigger;

	public float shredDurationRotation = 2f;

	public float shredDurationPosition = 5f;

	public float shredSwayAmount = 1f;

	public float shredSwaySpeed = 3f;

	public BaseEntity currentlyShredding;

	public GameObject[] shreddingWheels;

	public float shredRotorSpeed = 1f;

	public GameObjectRef shredSoundEffect;

	public Transform resourceSpawnPoint;

	private Quaternion entryRotation;

	public const string SHRED_STAT = "cars_shredded";

	private bool isShredding = false;

	private float shredStartTime = 0f;

	public virtual void OnEntityEnteredTrigger(BaseEntity ent)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.IsDestroyed)
		{
			Rigidbody component = ((Component)ent).GetComponent<Rigidbody>();
			if (isShredding || (Object)(object)currentlyShredding != (Object)null)
			{
				component.velocity = -component.velocity * 3f;
				return;
			}
			((Component)shredRail).transform.position = shredRailStartPos.position;
			((Component)shredRail).transform.rotation = Quaternion.LookRotation(shredRailStartRotation);
			entryRotation = ((Component)ent).transform.rotation;
			Quaternion rotation = ((Component)ent).transform.rotation;
			component.isKinematic = true;
			currentlyShredding = ent;
			((Component)ent).transform.rotation = rotation;
			isShredding = true;
			SetShredding(isShredding: true);
			shredStartTime = Time.realtimeSinceStartup;
		}
	}

	public void CreateShredResources()
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)currentlyShredding == (Object)null)
		{
			return;
		}
		MagnetLiftable component = ((Component)currentlyShredding).GetComponent<MagnetLiftable>();
		if ((Object)(object)component == (Object)null)
		{
			return;
		}
		if ((Object)(object)component.associatedPlayer != (Object)null && GameInfo.HasAchievements)
		{
			component.associatedPlayer.stats.Add("cars_shredded", 1);
			component.associatedPlayer.stats.Save(forceSteamSave: true);
		}
		ItemAmount[] shredResources = component.shredResources;
		foreach (ItemAmount itemAmount in shredResources)
		{
			Item item = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0uL);
			float num = 0.5f;
			BaseEntity baseEntity = item.CreateWorldObject(((Component)resourceSpawnPoint).transform.position + new Vector3(Random.Range(0f - num, num), 1f, Random.Range(0f - num, num)));
			if ((Object)(object)baseEntity == (Object)null)
			{
				item.Remove();
			}
		}
		BaseModularVehicle component2 = ((Component)currentlyShredding).GetComponent<BaseModularVehicle>();
		if (!Object.op_Implicit((Object)(object)component2))
		{
			return;
		}
		foreach (BaseVehicleModule attachedModuleEntity in component2.AttachedModuleEntities)
		{
			if (!Object.op_Implicit((Object)(object)attachedModuleEntity.AssociatedItemDef) || !Object.op_Implicit((Object)(object)attachedModuleEntity.AssociatedItemDef.Blueprint))
			{
				continue;
			}
			foreach (ItemAmount ingredient in attachedModuleEntity.AssociatedItemDef.Blueprint.ingredients)
			{
				int num2 = Mathf.FloorToInt(ingredient.amount * 0.5f);
				if (num2 != 0)
				{
					Item item2 = ItemManager.Create(ingredient.itemDef, num2, 0uL);
					float num3 = 0.5f;
					BaseEntity baseEntity2 = item2.CreateWorldObject(((Component)resourceSpawnPoint).transform.position + new Vector3(Random.Range(0f - num3, num3), 1f, Random.Range(0f - num3, num3)));
					if ((Object)(object)baseEntity2 == (Object)null)
					{
						item2.Remove();
					}
				}
			}
		}
	}

	public void UpdateBonePosition(float delta)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		float num = delta / shredDurationPosition;
		float num2 = delta / shredDurationRotation;
		((Component)shredRail).transform.localPosition = Vector3.Lerp(shredRailStartPos.localPosition, shredRailEndPos.localPosition, num);
		((Component)shredRail).transform.rotation = Quaternion.LookRotation(Vector3.Lerp(shredRailStartRotation, shredRailEndRotation, num2));
	}

	public void SetShredding(bool isShredding)
	{
		if (isShredding)
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)FireShredEffect, 0.25f, 0.75f, 0.25f);
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)FireShredEffect);
		}
	}

	public void FireShredEffect()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(shredSoundEffect.resourcePath, ((Component)this).transform.position + Vector3.up * 3f, Vector3.up);
	}

	public void ServerUpdate()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		SetFlag(Flags.Reserved10, isShredding);
		if (isShredding)
		{
			float num = Time.realtimeSinceStartup - shredStartTime;
			float num2 = num / shredDurationPosition;
			float num3 = num / shredDurationRotation;
			((Component)shredRail).transform.localPosition = Vector3.Lerp(shredRailStartPos.localPosition, shredRailEndPos.localPosition, num2);
			((Component)shredRail).transform.rotation = Quaternion.LookRotation(Vector3.Lerp(shredRailStartRotation, shredRailEndRotation, num3));
			MagnetLiftable component = ((Component)currentlyShredding).GetComponent<MagnetLiftable>();
			((Component)currentlyShredding).transform.position = ((Component)shredRail).transform.position;
			Vector3 val = ((Component)this).transform.TransformDirection(component.shredDirection);
			if (Vector3.Dot(-val, ((Component)currentlyShredding).transform.forward) > Vector3.Dot(val, ((Component)currentlyShredding).transform.forward))
			{
				val = ((Component)this).transform.TransformDirection(-component.shredDirection);
			}
			bool flag = Vector3.Dot(((Component)component).transform.up, Vector3.up) >= -0.95f;
			Quaternion val2 = QuaternionEx.LookRotationForcedUp(val, flag ? (-((Component)this).transform.right) : ((Component)this).transform.right);
			float num4 = Time.time * shredSwaySpeed;
			float num5 = Mathf.PerlinNoise(num4, 0f);
			float num6 = Mathf.PerlinNoise(0f, num4 + 150f);
			val2 *= Quaternion.Euler(num5 * shredSwayAmount, 0f, num6 * shredSwayAmount);
			((Component)currentlyShredding).transform.rotation = Quaternion.Lerp(entryRotation, val2, num3);
			if (num > 5f)
			{
				CreateShredResources();
				currentlyShredding.Kill();
				currentlyShredding = null;
				isShredding = false;
				SetShredding(isShredding: false);
			}
		}
	}

	private void Update()
	{
		ServerUpdate();
	}
}
