using System;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

public class PlayerCorpse : LootableCorpse
{
	public Buoyancy buoyancy;

	public const Flags Flag_Buoyant = Flags.Reserved6;

	public uint underwearSkin;

	public PlayerBonePosData bonePosData;

	private Ragdoll corpseRagdollScript;

	private Action cachedSleepCheck;

	private Vector3 prevLocalPos;

	private const float SLEEP_CHECK_FREQUENCY = 10f;

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
		if (Application.isLoadingSave)
		{
			corpseRagdollScript = ((Component)this).GetComponent<Ragdoll>();
		}
		if (CorpseIsRagdoll)
		{
			corpseRagdollScript.simOnServer = true;
			corpseRagdollScript.ServerInit();
			if (HasParent())
			{
				OnParented();
			}
		}
	}

	public override void ServerInitCorpse(BaseEntity pr, Vector3 posOnDeah, Quaternion rotOnDeath, BasePlayer.PlayerFlags playerFlagsOnDeath, ModelState modelState)
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
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
			PlayerBonePosData.BonePosData bonePositionData = GetBonePositionData(playerFlagsOnDeath, modelState);
			if (bonePositionData != null)
			{
				component2.CopyFrom(bonePositionData.bonePositions, bonePositionData.boneRotations, true);
				Transform transform = component2.Bones[0].transform;
				transform.localEulerAngles += bonePositionData.rootRotationOffset;
			}
		}
		if (CorpseIsRagdoll)
		{
			Quaternion val = (((playerFlagsOnDeath & BasePlayer.PlayerFlags.Sleeping) != 0) ? Quaternion.identity : rotOnDeath);
			((Component)this).transform.SetPositionAndRotation(posOnDeah, val);
		}
		else
		{
			((Component)this).transform.SetPositionAndRotation(parentEnt.CenterPoint(), basePlayer.eyes.bodyRotation);
		}
	}

	private PlayerBonePosData.BonePosData GetBonePositionData(BasePlayer.PlayerFlags flagsOnDeath, ModelState modelState)
	{
		if (flagsOnDeath.HasFlag(BasePlayer.PlayerFlags.Sleeping))
		{
			return bonePosData.sleeping;
		}
		if (flagsOnDeath.HasFlag(BasePlayer.PlayerFlags.Incapacitated))
		{
			return bonePosData.incapacitated;
		}
		if (flagsOnDeath.HasFlag(BasePlayer.PlayerFlags.Wounded))
		{
			return bonePosData.crawling;
		}
		if (modelState.onLadder)
		{
			return bonePosData.onladder;
		}
		if (modelState.ducked)
		{
			return bonePosData.ducking;
		}
		if (modelState.waterLevel >= 0.75f)
		{
			return bonePosData.swimming;
		}
		if (modelState.mounted)
		{
			if (modelState.poseType < bonePosData.mountedPoses.Length)
			{
				return bonePosData.mountedPoses[modelState.poseType];
			}
			if (modelState.poseType == 128)
			{
				return bonePosData.standing;
			}
			Debug.LogWarning((object)$"PlayerCorpse GetBonePositionData: No saved bone position data for mount pose {modelState.poseType}. Falling back to SitGeneric. Please update the 'Server Side Ragdoll Bone Pos Data' file with the new mount pose.");
			return bonePosData.mountedPoses[7];
		}
		return bonePosData.standing;
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
			((FacepunchBehaviour)this).InvokeRandomized(cachedSleepCheck, 5f, 10f, Random.Range(-1f, 1f));
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
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		if (!CorpseIsRagdoll || !HasParent())
		{
			return;
		}
		if (corpseRagdollScript.IsInactive)
		{
			if (!GamePhysics.Trace(new Ray(CenterPoint(), Vector3.down), 0f, out var _, 0.25f, -928830719, (QueryTriggerInteraction)1, this))
			{
				BecomeActive();
			}
		}
		else if (Vector3.SqrMagnitude(((Component)this).transform.localPosition - prevLocalPos) < 0.1f)
		{
			BecomeInactive();
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
