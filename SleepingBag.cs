using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class SleepingBag : DecayEntity
{
	public enum BagAssignMode
	{
		Allowed = 0,
		TeamAndFriendlyContacts = 1,
		None = 2,
		LAST = 2
	}

	public enum BagResultType
	{
		Ok,
		TooManyBags,
		BagBlocked
	}

	public struct CanAssignBedResult
	{
		public BagResultType Result;

		public int Count;

		public int Max;
	}

	public struct CanBuildResult
	{
		public bool Result;

		public Phrase Phrase;

		public string[] Arguments;
	}

	public enum SleepingBagResetReason
	{
		Respawned,
		Placed,
		Death
	}

	[NonSerialized]
	public ulong deployerUserID;

	public GameObject renameDialog;

	public GameObject assignDialog;

	public float secondsBetweenReuses = 300f;

	public string niceName = "Unnamed Bag";

	public Vector3 spawnOffset = Vector3.zero;

	public RespawnType RespawnType = (RespawnType)1;

	public bool isStatic;

	public bool canBePublic;

	public const Flags IsPublicFlag = Flags.Reserved3;

	public static Phrase bagLimitPhrase = new Phrase("bag_limit_update", "You are now at {0}/{1} bags");

	public static Phrase bagLimitReachedPhrase = new Phrase("bag_limit_reached", "You have reached your bag limit!");

	public Phrase assignOtherBagPhrase = new Phrase("assigned_other_bag_limit", "You have assigned {0} a bag, they are now at {0}/{1} bags");

	public Phrase assignedBagPhrase = new Phrase("assigned_bag_limit", "You have been assigned a bag, you are now at {0}/{1} bags");

	public Phrase cannotAssignBedPhrase = new Phrase("cannot_assign_bag_limit", "You cannot assign {0} a bag, they have reached their bag limit!");

	public Phrase cannotMakeBedPhrase = new Phrase("cannot_make_bed_limit", "You cannot take ownership of the bed, you are at your bag limit");

	public Phrase bedAssigningBlocked = new Phrase("bag_assign_blocked", "That player has blocked bag assignment");

	internal float unlockTime;

	public static List<SleepingBag> sleepingBags = new List<SleepingBag>();

	private bool notifyPlayerOnServerInit;

	private static Dictionary<ulong, List<SleepingBag>> bagsPerPlayer = new Dictionary<ulong, List<SleepingBag>>();

	public virtual float unlockSeconds
	{
		get
		{
			if (unlockTime < Time.realtimeSinceStartup)
			{
				return 0f;
			}
			return unlockTime - Time.realtimeSinceStartup;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SleepingBag.OnRpcMessage", 0);
		try
		{
			if (rpc == 3057055788u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AssignToFriend "));
				}
				TimeWarning val2 = TimeWarning.New("AssignToFriend", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3057055788u, "AssignToFriend", this, player, 3f))
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
							AssignToFriend(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AssignToFriend");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1335950295 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Rename "));
				}
				TimeWarning val2 = TimeWarning.New("Rename", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1335950295u, "Rename", this, player, 3f))
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
							Rename(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Rename");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 42669546 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_MakeBed "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_MakeBed", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(42669546u, "RPC_MakeBed", this, player, 3f))
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
							RPCMessage msg4 = rPCMessage;
							RPC_MakeBed(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_MakeBed");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 393812086 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_MakePublic "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_MakePublic", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(393812086u, "RPC_MakePublic", this, player, 3f))
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
							RPCMessage msg5 = rPCMessage;
							RPC_MakePublic(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_MakePublic");
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

	public bool IsPublic()
	{
		return HasFlag(Flags.Reserved3);
	}

	public virtual float GetUnlockSeconds(ulong playerID)
	{
		return unlockSeconds;
	}

	public virtual bool ValidForPlayer(ulong playerID, bool ignoreTimers)
	{
		if (deployerUserID == playerID)
		{
			if (!ignoreTimers)
			{
				return unlockTime < Time.realtimeSinceStartup;
			}
			return true;
		}
		return false;
	}

	public static CanAssignBedResult? CanAssignBed(BasePlayer player, SleepingBag newBag, ulong targetPlayer, int countOffset = 1, int maxOffset = 0, SleepingBag ignore = null)
	{
		int num = ConVar.Server.max_sleeping_bags + maxOffset;
		if (num < 0)
		{
			return null;
		}
		int num2 = countOffset;
		BasePlayer basePlayer = BasePlayer.FindByID(targetPlayer);
		CanAssignBedResult value;
		if ((Object)(object)player != (Object)(object)basePlayer && (Object)(object)basePlayer != (Object)null)
		{
			switch ((BagAssignMode)Mathf.Clamp(basePlayer.GetInfoInt("client.bagassignmode", 0), 0, 2))
			{
			case BagAssignMode.None:
				value = default(CanAssignBedResult);
				value.Result = BagResultType.BagBlocked;
				return value;
			case BagAssignMode.TeamAndFriendlyContacts:
			{
				bool flag = false;
				if (basePlayer.Team != null && basePlayer.Team.members.Contains(player.userID))
				{
					flag = true;
				}
				else
				{
					RelationshipManager.PlayerRelationshipInfo relations = RelationshipManager.ServerInstance.GetRelationships(targetPlayer).GetRelations(player.userID);
					if (relations != null && relations.type == RelationshipManager.RelationshipType.Friend)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					value = default(CanAssignBedResult);
					value.Result = BagResultType.BagBlocked;
					return value;
				}
				break;
			}
			}
		}
		foreach (SleepingBag sleepingBag in sleepingBags)
		{
			if ((Object)(object)sleepingBag != (Object)(object)ignore && sleepingBag.deployerUserID == targetPlayer)
			{
				num2++;
				if (num2 > num)
				{
					value = default(CanAssignBedResult);
					value.Count = num2;
					value.Max = num;
					value.Result = BagResultType.TooManyBags;
					return value;
				}
			}
		}
		value = default(CanAssignBedResult);
		value.Count = num2;
		value.Max = num;
		value.Result = BagResultType.Ok;
		return value;
	}

	public static CanBuildResult? CanBuildBed(BasePlayer player, Construction construction)
	{
		GameObject obj = GameManager.server.FindPrefab(construction.prefabID);
		if (((obj != null) ? obj.GetComponent<BaseEntity>() : null) is SleepingBag)
		{
			CanAssignBedResult? canAssignBedResult = CanAssignBed(player, null, player.userID);
			if (canAssignBedResult.HasValue)
			{
				CanBuildResult value;
				if (canAssignBedResult.Value.Result == BagResultType.Ok)
				{
					value = default(CanBuildResult);
					value.Result = true;
					value.Phrase = bagLimitPhrase;
					value.Arguments = new string[2]
					{
						canAssignBedResult.Value.Count.ToString(),
						canAssignBedResult.Value.Max.ToString()
					};
					return value;
				}
				value = default(CanBuildResult);
				value.Result = false;
				value.Phrase = bagLimitReachedPhrase;
				return value;
			}
		}
		return null;
	}

	public static SleepingBag[] FindForPlayer(ulong playerID, bool ignoreTimers)
	{
		return sleepingBags.Where((SleepingBag x) => x.ValidForPlayer(playerID, ignoreTimers)).ToArray();
	}

	public static bool SpawnPlayer(BasePlayer player, NetworkableId sleepingBag)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		SleepingBag[] array = FindForPlayer(player.userID, ignoreTimers: true);
		SleepingBag sleepingBag2 = array.FirstOrDefault((SleepingBag x) => x.ValidForPlayer(player.userID, ignoreTimers: false) && x.net.ID == sleepingBag && x.unlockTime < Time.realtimeSinceStartup);
		if ((Object)(object)sleepingBag2 == (Object)null)
		{
			return false;
		}
		if (sleepingBag2.IsOccupied(player.userID))
		{
			return false;
		}
		sleepingBag2.GetSpawnPos(out var pos, out var rot);
		player.RespawnAt(pos, rot, sleepingBag2);
		sleepingBag2.PostPlayerSpawn(player);
		SleepingBag[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			SetBagTimer(array2[i], pos, SleepingBagResetReason.Respawned);
		}
		return true;
	}

	public static void AddBagForPlayer(SleepingBag bag, ulong user, bool networkUpdate = true)
	{
		if (user == 0L)
		{
			return;
		}
		if (!bagsPerPlayer.TryGetValue(user, out var value))
		{
			value = new List<SleepingBag>();
			bagsPerPlayer[user] = value;
		}
		if (!value.Contains(bag))
		{
			value.Add(bag);
			if (networkUpdate)
			{
				RelationshipManager.FindByID(user)?.SendNetworkUpdate();
			}
		}
	}

	public static void RemoveBagForPlayer(SleepingBag bag, ulong user)
	{
		if (user != 0L && bagsPerPlayer.TryGetValue(user, out var value))
		{
			if (value.Remove(bag))
			{
				RelationshipManager.FindByID(user)?.SendNetworkUpdate();
			}
			if (value.Count == 0)
			{
				bagsPerPlayer.Remove(user);
			}
		}
	}

	public static void OnBagChangedOwnership(SleepingBag bag, ulong oldUser)
	{
		if (bag.deployerUserID != oldUser)
		{
			RemoveBagForPlayer(bag, oldUser);
			AddBagForPlayer(bag, bag.deployerUserID);
		}
	}

	public static int GetSleepingBagCount(ulong userId)
	{
		if (userId == 0L)
		{
			return 0;
		}
		if (!bagsPerPlayer.TryGetValue(userId, out var value))
		{
			return 0;
		}
		return value.Count;
	}

	public static bool TrySpawnPlayer(BasePlayer player, NetworkableId sleepingBag, out string errorMessage)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!player.IsDead())
		{
			errorMessage = "Couldn't spawn - player is not dead!";
			return false;
		}
		if (player.CanRespawn())
		{
			if (SpawnPlayer(player, sleepingBag))
			{
				player.MarkRespawn();
				errorMessage = null;
				return true;
			}
			errorMessage = "Couldn't spawn in sleeping bag!";
			return false;
		}
		errorMessage = "You can't respawn again so quickly, wait a while";
		return false;
	}

	public virtual void SetUnlockTime(float newTime)
	{
		unlockTime = newTime;
	}

	public static bool DestroyBag(ulong userID, NetworkableId sleepingBag)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SleepingBag sleepingBag2 = FindForPlayer(userID, ignoreTimers: true).FirstOrDefault((SleepingBag x) => x.net.ID == sleepingBag);
		if ((Object)(object)sleepingBag2 == (Object)null)
		{
			return false;
		}
		RemoveBagForPlayer(sleepingBag2, sleepingBag2.deployerUserID);
		sleepingBag2.deployerUserID = 0uL;
		sleepingBag2.SendNetworkUpdate();
		BasePlayer basePlayer = BasePlayer.FindByID(userID);
		if ((Object)(object)basePlayer != (Object)null)
		{
			basePlayer.SendRespawnOptions();
			Analytics.Azure.OnBagUnclaimed(basePlayer, sleepingBag2);
		}
		return true;
	}

	public static void ResetTimersForPlayer(BasePlayer player)
	{
		SleepingBag[] array = FindForPlayer(player.userID, ignoreTimers: true);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].unlockTime = 0f;
		}
	}

	public virtual void GetSpawnPos(out Vector3 pos, out Quaternion rot)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		pos = ((Component)this).transform.position + spawnOffset;
		Quaternion rotation = ((Component)this).transform.rotation;
		rot = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).eulerAngles.y, 0f);
	}

	public void SetPublic(bool isPublic)
	{
		SetFlag(Flags.Reserved3, isPublic);
	}

	private void SetDeployedBy(BasePlayer player)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)player == (Object)null))
		{
			deployerUserID = player.userID;
			SetBagTimer(this, ((Component)this).transform.position, SleepingBagResetReason.Placed);
			SendNetworkUpdate();
			notifyPlayerOnServerInit = true;
		}
	}

	public static void OnPlayerDeath(BasePlayer player)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		SleepingBag[] array = FindForPlayer(player.userID, ignoreTimers: true);
		for (int i = 0; i < array.Length; i++)
		{
			SetBagTimer(array[i], ((Component)player).transform.position, SleepingBagResetReason.Death);
		}
	}

	public static void SetBagTimer(SleepingBag bag, Vector3 position, SleepingBagResetReason reason)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		float? num = null;
		if ((Object)(object)activeGameMode != (Object)null)
		{
			num = activeGameMode.EvaluateSleepingBagReset(bag, position, reason);
		}
		if (num.HasValue)
		{
			bag.SetUnlockTime(Time.realtimeSinceStartup + num.Value);
			return;
		}
		if (reason == SleepingBagResetReason.Respawned && Vector3.Distance(position, ((Component)bag).transform.position) <= ConVar.Server.respawnresetrange)
		{
			bag.SetUnlockTime(Time.realtimeSinceStartup + bag.secondsBetweenReuses);
			bag.SendNetworkUpdate();
		}
		if (reason != SleepingBagResetReason.Placed)
		{
			return;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		SleepingBag[] array = sleepingBags.Where((SleepingBag x) => x.deployerUserID != 0L && x.deployerUserID == bag.deployerUserID && x.unlockTime > Time.realtimeSinceStartup).ToArray();
		foreach (SleepingBag sleepingBag in array)
		{
			if (bag.unlockTime > realtimeSinceStartup && Vector3.Distance(((Component)sleepingBag).transform.position, position) <= ConVar.Server.respawnresetrange)
			{
				realtimeSinceStartup = bag.unlockTime;
			}
		}
		bag.SetUnlockTime(Mathf.Max(realtimeSinceStartup, Time.realtimeSinceStartup + bag.secondsBetweenReuses));
		bag.SendNetworkUpdate();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!sleepingBags.Contains(this))
		{
			sleepingBags.Add(this);
			if (deployerUserID != 0L)
			{
				AddBagForPlayer(this, deployerUserID, !Application.isLoadingSave);
			}
		}
		if (notifyPlayerOnServerInit)
		{
			notifyPlayerOnServerInit = false;
			NotifyPlayer(deployerUserID);
		}
	}

	public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		((FacepunchBehaviour)this).Invoke((Action)DelayedPlayerNotify, 0.1f);
	}

	private void DelayedPlayerNotify()
	{
		NotifyPlayer(deployerUserID);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		AddBagForPlayer(this, deployerUserID, !Application.isLoadingSave);
	}

	private void NotifyPlayer(ulong id)
	{
		BasePlayer basePlayer = BasePlayer.FindByID(id);
		if ((Object)(object)basePlayer != (Object)null && basePlayer.IsConnected)
		{
			basePlayer.SendRespawnOptions();
		}
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		sleepingBags.RemoveAll((SleepingBag x) => (Object)(object)x == (Object)(object)this);
		RemoveBagForPlayer(this, deployerUserID);
		NotifyPlayer(deployerUserID);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.sleepingBag = Pool.Get<SleepingBag>();
		info.msg.sleepingBag.name = niceName;
		info.msg.sleepingBag.deployerID = deployerUserID;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void Rename(RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			string str = msg.read.String(256);
			str = WordFilter.Filter(str);
			if (string.IsNullOrEmpty(str))
			{
				str = "Unnamed Sleeping Bag";
			}
			if (str.Length > 24)
			{
				str = str.Substring(0, 22) + "..";
			}
			niceName = str;
			SendNetworkUpdate();
			NotifyPlayer(deployerUserID);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void AssignToFriend(RPCMessage msg)
	{
		if (!msg.player.CanInteract() || deployerUserID != msg.player.userID)
		{
			return;
		}
		ulong num = msg.read.UInt64();
		if (num == 0L)
		{
			return;
		}
		if (ConVar.Server.max_sleeping_bags > 0)
		{
			CanAssignBedResult? canAssignBedResult = CanAssignBed(msg.player, this, num);
			if (canAssignBedResult.HasValue)
			{
				BasePlayer basePlayer = RelationshipManager.FindByID(num);
				if (canAssignBedResult.Value.Result == BagResultType.TooManyBags)
				{
					msg.player.ShowToast(GameTip.Styles.Red_Normal, cannotAssignBedPhrase, basePlayer?.displayName ?? "other player");
				}
				else if (canAssignBedResult.Value.Result == BagResultType.BagBlocked)
				{
					msg.player.ShowToast(GameTip.Styles.Red_Normal, bedAssigningBlocked);
				}
				else
				{
					basePlayer?.ShowToast(GameTip.Styles.Blue_Long, assignedBagPhrase, canAssignBedResult.Value.Count.ToString(), canAssignBedResult.Value.Max.ToString());
					msg.player.ShowToast(GameTip.Styles.Blue_Long, bagLimitPhrase, (GetSleepingBagCount(msg.player.userID) - 1).ToString(), canAssignBedResult.Value.Max.ToString());
				}
				if (canAssignBedResult.Value.Result != 0)
				{
					return;
				}
			}
		}
		ulong num2 = deployerUserID;
		deployerUserID = num;
		NotifyPlayer(num2);
		NotifyPlayer(deployerUserID);
		OnBagChangedOwnership(this, num2);
		Analytics.Azure.OnSleepingBagAssigned(msg.player, this, num);
		SendNetworkUpdate();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public virtual void RPC_MakePublic(RPCMessage msg)
	{
		if (!canBePublic || !msg.player.CanInteract() || (deployerUserID != msg.player.userID && !msg.player.CanBuild()))
		{
			return;
		}
		bool flag = msg.read.Bit();
		if (flag == IsPublic())
		{
			return;
		}
		SetPublic(flag);
		if (!IsPublic())
		{
			if (ConVar.Server.max_sleeping_bags > 0)
			{
				CanAssignBedResult? canAssignBedResult = CanAssignBed(msg.player, this, msg.player.userID, 1, 0, this);
				if (canAssignBedResult.HasValue)
				{
					if (canAssignBedResult.Value.Result == BagResultType.Ok)
					{
						msg.player.ShowToast(GameTip.Styles.Blue_Long, bagLimitPhrase, canAssignBedResult.Value.Count.ToString(), canAssignBedResult.Value.Max.ToString());
					}
					else
					{
						msg.player.ShowToast(GameTip.Styles.Blue_Long, cannotMakeBedPhrase, canAssignBedResult.Value.Count.ToString(), canAssignBedResult.Value.Max.ToString());
					}
					if (canAssignBedResult.Value.Result != 0)
					{
						return;
					}
				}
			}
			ulong num = deployerUserID;
			deployerUserID = msg.player.userID;
			NotifyPlayer(num);
			NotifyPlayer(deployerUserID);
			OnBagChangedOwnership(this, num);
			Analytics.Azure.OnSleepingBagAssigned(msg.player, this, deployerUserID = msg.player.userID);
		}
		else
		{
			Analytics.Azure.OnSleepingBagAssigned(msg.player, this, 0uL);
		}
		SendNetworkUpdate();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_MakeBed(RPCMessage msg)
	{
		if (!canBePublic || !IsPublic() || !msg.player.CanInteract())
		{
			return;
		}
		if (ConVar.Server.max_sleeping_bags > 0)
		{
			CanAssignBedResult? canAssignBedResult = CanAssignBed(msg.player, this, msg.player.userID, 1, 0, this);
			if (canAssignBedResult.HasValue)
			{
				if (canAssignBedResult.Value.Result != 0)
				{
					msg.player.ShowToast(GameTip.Styles.Red_Normal, cannotMakeBedPhrase);
				}
				else
				{
					msg.player.ShowToast(GameTip.Styles.Blue_Long, bagLimitPhrase, canAssignBedResult.Value.Count.ToString(), canAssignBedResult.Value.Max.ToString());
				}
				if (canAssignBedResult.Value.Result != 0)
				{
					return;
				}
			}
		}
		ulong num = deployerUserID;
		deployerUserID = msg.player.userID;
		NotifyPlayer(num);
		NotifyPlayer(deployerUserID);
		OnBagChangedOwnership(this, num);
		SendNetworkUpdate();
	}

	protected virtual void PostPlayerSpawn(BasePlayer p)
	{
		p.SendRespawnOptions();
	}

	public virtual bool IsOccupied(ulong userID)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return WaterLevel.Test(((Component)this).transform.position, waves: true, volumes: false);
	}

	public virtual bool IsMobile()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		BaseEntity baseEntity = GetParentEntity();
		if ((Object)(object)baseEntity != (Object)null && baseEntity is BaseVehicle)
		{
			return true;
		}
		return (int)RespawnType == 4;
	}

	public override string Admin_Who()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(base.Admin_Who());
		stringBuilder.AppendLine($"Assigned bag ID: {deployerUserID}");
		stringBuilder.AppendLine("Assigned player name: " + Admin.GetPlayerName(deployerUserID));
		stringBuilder.AppendLine("Bag Name:" + niceName);
		return stringBuilder.ToString();
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sleepingBag != null)
		{
			niceName = info.msg.sleepingBag.name;
			deployerUserID = info.msg.sleepingBag.deployerID;
		}
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (base.CanPickup(player))
		{
			return player.userID == deployerUserID;
		}
		return false;
	}
}
