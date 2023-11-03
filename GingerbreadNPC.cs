using System;
using ConVar;
using ProtoBuf;
using UnityEngine;

public class GingerbreadNPC : HumanNPC, IClientBrainStateListener
{
	public GameObjectRef OverrideCorpseMale;

	public GameObjectRef OverrideCorpseFemale;

	public PhysicMaterial HitMaterial;

	public bool RoamAroundHomePoint;

	protected string CorpseResourcePath
	{
		get
		{
			bool flag = GetFloatBasedOnUserID(userID, 4332uL) > 0.5f;
			if (OverrideCorpseMale.isValid && !flag)
			{
				return OverrideCorpseMale.resourcePath;
			}
			if (OverrideCorpseFemale.isValid && flag)
			{
				return OverrideCorpseFemale.resourcePath;
			}
			return "assets/prefabs/npc/murderer/murderer_corpse.prefab";
			static float GetFloatBasedOnUserID(ulong steamid, ulong seed)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				//IL_0005: Unknown result type (might be due to invalid IL or missing references)
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				State state = Random.state;
				Random.InitState((int)(seed + steamid));
				float result = Random.Range(0f, 1f);
				Random.state = state;
				return result;
			}
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		info.HitMaterial = Global.GingerbreadMaterialID();
	}

	public override string Categorize()
	{
		return "Gingerbread";
	}

	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	public override BaseCorpse CreateCorpse()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Create corpse", 0);
		try
		{
			string corpseResourcePath = CorpseResourcePath;
			NPCPlayerCorpse nPCPlayerCorpse = DropCorpse(corpseResourcePath) as NPCPlayerCorpse;
			if (Object.op_Implicit((Object)(object)nPCPlayerCorpse))
			{
				((Component)nPCPlayerCorpse).transform.position = ((Component)nPCPlayerCorpse).transform.position + Vector3.down * NavAgent.baseOffset;
				nPCPlayerCorpse.SetLootableIn(2f);
				nPCPlayerCorpse.SetFlag(Flags.Reserved5, HasPlayerFlag(PlayerFlags.DisplaySash));
				nPCPlayerCorpse.SetFlag(Flags.Reserved2, b: true);
				nPCPlayerCorpse.TakeFrom(this, inventory.containerMain);
				nPCPlayerCorpse.playerName = "Gingerbread";
				nPCPlayerCorpse.playerSteamID = userID;
				nPCPlayerCorpse.Spawn();
				ItemContainer[] containers = nPCPlayerCorpse.containers;
				for (int i = 0; i < containers.Length; i++)
				{
					containers[i].Clear();
				}
				if (LootSpawnSlots.Length != 0)
				{
					LootContainer.LootSpawnSlot[] lootSpawnSlots = LootSpawnSlots;
					for (int i = 0; i < lootSpawnSlots.Length; i++)
					{
						LootContainer.LootSpawnSlot lootSpawnSlot = lootSpawnSlots[i];
						for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
						{
							if (Random.Range(0f, 1f) <= lootSpawnSlot.probability)
							{
								lootSpawnSlot.definition.SpawnIntoContainer(nPCPlayerCorpse.containers[0]);
							}
						}
					}
				}
			}
			return nPCPlayerCorpse;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void AttackerInfo(DeathInfo info)
	{
		base.AttackerInfo(info);
		info.inflictorName = inventory.containerBelt.GetSlot(0).info.shortname;
		info.attackerName = base.ShortPrefabName;
	}

	public void OnClientStateChanged(AIState state)
	{
	}
}
