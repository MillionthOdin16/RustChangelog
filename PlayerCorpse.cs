using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class PlayerCorpse : LootableCorpse
{
	public Buoyancy buoyancy;

	public const Flags Flag_Buoyant = Flags.Reserved6;

	public uint underwearSkin;

	public PlayerBonePositionData boneDataStanding;

	public PlayerBonePositionData boneDataCrawling;

	public PlayerBonePositionData boneDataIncapacitated;

	public PlayerBonePositionData boneDataSleeping;

	public PlayerBonePositionData boneDataOnLadder;

	public PlayerBonePositionData boneDataDuck;

	public PlayerBonePositionData boneDataSwimming;

	private Ragdoll corpseRagdollScript;

	private Action cachedSleepCheck;

	private Vector3 prevLocalPos;

	private const float SLEEP_CHECK_FREQUENCY = 15f;

	protected override float PositionTickRate => 0.05f;

	protected override bool PositionTickFixedTime => true;

	private bool CorpseIsRagdoll => (Object)(object)corpseRagdollScript != (Object)null;

	public bool IsBuoyant()
	{
		return HasFlag(Flags.Reserved6);
	}

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if ((baseEntity.InSafeZone() || InSafeZone()) && baseEntity.userID != playerSteamID)
		{
			return false;
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if ((Object)(object)buoyancy == (Object)null)
		{
			Debug.LogWarning((object)("Player corpse has no buoyancy assigned, searching at runtime :" + ((Object)this).name));
			buoyancy = ((Component)this).GetComponent<Buoyancy>();
		}
		if ((Object)(object)buoyancy != (Object)null)
		{
			buoyancy.SubmergedChanged = BuoyancyChanged;
			buoyancy.forEntity = this;
		}
	}

	public override void ServerInitCorpse(BaseEntity pr, BasePlayer.PlayerFlags playerFlagsOnDeath, ModelState modelState)
	{
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		parentEnt = pr;
		BasePlayer basePlayer = (BasePlayer)pr;
		corpseRagdollScript = ((Component)this).GetComponent<Ragdoll>();
		SpawnPointInstance component = ((Component)this).GetComponent<SpawnPointInstance>();
		if ((Object)(object)component != (Object)null)
		{
			spawnGroup = component.parentSpawnPointUser as SpawnGroup;
		}
		Skeleton component2 = ((Component)this).GetComponent<Skeleton>();
		if ((Object)(object)component2 != (Object)null)
		{
			PlayerBonePositionData bonePositionData = GetBonePositionData(playerFlagsOnDeath, modelState);
			if ((Object)(object)bonePositionData != (Object)null)
			{
				component2.CopyFrom(bonePositionData.bonePositions, bonePositionData.boneRotations, true);
				Transform transform = component2.Bones[0].transform;
				transform.localEulerAngles += bonePositionData.rootRotationOffset;
			}
		}
		if (CorpseIsRagdoll)
		{
			Quaternion val = (((playerFlagsOnDeath & BasePlayer.PlayerFlags.Sleeping) != 0) ? Quaternion.identity : basePlayer.eyes.bodyRotation);
			((Component)this).transform.SetPositionAndRotation(((Component)parentEnt).transform.position, val);
			corpseRagdollScript.simOnServer = true;
			corpseRagdollScript.ServerInit();
			if (HasParent())
			{
				OnParented();
			}
		}
		else
		{
			((Component)this).transform.SetPositionAndRotation(parentEnt.CenterPoint(), basePlayer.eyes.bodyRotation);
		}
	}

	private PlayerBonePositionData GetBonePositionData(BasePlayer.PlayerFlags flagsOnDeath, ModelState modelState)
	{
		if (flagsOnDeath.HasFlag(BasePlayer.PlayerFlags.Sleeping))
		{
			return boneDataSleeping;
		}
		if (flagsOnDeath.HasFlag(BasePlayer.PlayerFlags.Incapacitated))
		{
			return boneDataIncapacitated;
		}
		if (flagsOnDeath.HasFlag(BasePlayer.PlayerFlags.Wounded))
		{
			return boneDataCrawling;
		}
		if (modelState.onLadder)
		{
			return boneDataOnLadder;
		}
		if (modelState.ducked)
		{
			return boneDataDuck;
		}
		if (modelState.waterLevel >= 0.75f)
		{
			return boneDataSwimming;
		}
		return boneDataStanding;
	}

	public void BuoyancyChanged(bool isSubmerged)
	{
		if (!IsBuoyant())
		{
			SetFlag(Flags.Reserved6, isSubmerged, recursive: false, networkupdate: false);
			SendNetworkUpdate_Flags();
		}
	}

	public void BecomeActive()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (CorpseIsRagdoll)
		{
			corpseRagdollScript.BecomeActive();
			prevLocalPos = ((Component)this).transform.localPosition;
		}
	}

	public void BecomeInactive()
	{
		if (CorpseIsRagdoll)
		{
			corpseRagdollScript.BecomeInactive();
		}
	}

	public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		if ((Object)(object)newParent != (Object)null && (Object)(object)newParent != (Object)(object)oldParent)
		{
			OnParented();
		}
		else if ((Object)(object)newParent == (Object)null && (Object)(object)oldParent != (Object)null)
		{
			OnUnparented();
		}
	}

	private void OnParented()
	{
		if (CorpseIsRagdoll)
		{
			if (cachedSleepCheck == null)
			{
				cachedSleepCheck = SleepCheck;
			}
			((FacepunchBehaviour)this).InvokeRandomized(cachedSleepCheck, 15f, 15f, Random.Range(-1.5f, 1.5f));
		}
	}

	private void OnUnparented()
	{
		if (CorpseIsRagdoll && cachedSleepCheck != null)
		{
			((FacepunchBehaviour)this).CancelInvoke(cachedSleepCheck);
		}
	}

	private void SleepCheck()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (!CorpseIsRagdoll || !HasParent())
		{
			return;
		}
		if (corpseRagdollScript.IsInactive)
		{
			if (!GamePhysics.Trace(new Ray(CenterPoint(), Vector3.down), 0f, out var _, 0.2f, -928830719, (QueryTriggerInteraction)1, this))
			{
				BecomeActive();
			}
		}
		else
		{
			float num = 0.05f;
			if (Vector3.SqrMagnitude(((Component)this).transform.localPosition - prevLocalPos) < num)
			{
				BecomeInactive();
			}
		}
		prevLocalPos = ((Component)this).transform.localPosition;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.lootableCorpse != null)
		{
			info.msg.lootableCorpse.underwearSkin = underwearSkin;
		}
		if (base.isServer && containers != null && containers.Length > 1 && !info.forDisk)
		{
			info.msg.storageBox = Pool.Get<StorageBox>();
			info.msg.storageBox.contents = containers[1].Save();
		}
	}

	public override string Categorize()
	{
		return "playercorpse";
	}
}
