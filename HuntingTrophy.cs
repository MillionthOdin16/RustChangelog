using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust.UI;
using UnityEngine;
using UnityEngine.Assertions;

public class HuntingTrophy : StorageContainer
{
	[Serializable]
	public struct TrophyRoot
	{
		public GameObjectRef SourceEntity;

		public GameObject Root;

		public uint GetSourcePrefabId()
		{
			BaseEntity entity = SourceEntity.GetEntity();
			if ((Object)(object)entity != (Object)null)
			{
				return entity.prefabID;
			}
			return 0u;
		}

		public bool Matches(HeadEntity headEnt)
		{
			BaseEntity entity = SourceEntity.GetEntity();
			bool flag = (Object)(object)entity != (Object)null && headEnt.CurrentTrophyData != null && entity.prefabID == headEnt.CurrentTrophyData.entitySource;
			if (!flag)
			{
				GameObject headSource = headEnt.GetHeadSource();
				BasePlayer basePlayer = default(BasePlayer);
				if ((Object)(object)headSource != (Object)null && headSource.TryGetComponent<BasePlayer>(ref basePlayer) && ((Component)entity).TryGetComponent<BasePlayer>(ref basePlayer))
				{
					flag = true;
				}
			}
			return flag;
		}

		public bool Matches(HeadData data)
		{
			if (data == null)
			{
				return false;
			}
			BaseEntity entity = SourceEntity.GetEntity();
			bool flag = (Object)(object)entity != (Object)null && entity.prefabID == data.entitySource;
			if (!flag)
			{
				GameObject val = null;
				val = GameManager.server.FindPrefab(data.entitySource);
				BasePlayer basePlayer = default(BasePlayer);
				if ((Object)(object)val != (Object)null && val.TryGetComponent<BasePlayer>(ref basePlayer) && ((Component)entity).TryGetComponent<BasePlayer>(ref basePlayer))
				{
					flag = true;
				}
			}
			return flag;
		}
	}

	private HeadData CurrentTrophyData;

	public PlayerModel Player;

	public GameObject MaleRope;

	public GameObject FemaleRope;

	public Renderer[] HorseRenderers;

	public Renderer[] HorseHairRenderers;

	public const uint HORSE_PREFAB_ID = 2421623959u;

	public GameObject NameRoot;

	public RustText NameText;

	public TrophyRoot[] Trophies;

	public HeadData TrophyData => CurrentTrophyData;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("HuntingTrophy.OnRpcMessage", 0);
		try
		{
			if (rpc == 1170506026 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ServerRequestClear "));
				}
				TimeWarning val2 = TimeWarning.New("ServerRequestClear", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1170506026u, "ServerRequestClear", this, player, 3f))
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
							ServerRequestClear();
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ServerRequestClear");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3878554182u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ServerRequestSubmit "));
				}
				TimeWarning val2 = TimeWarning.New("ServerRequestSubmit", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3878554182u, "ServerRequestSubmit", this, player, 3f))
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
							ServerRequestSubmit();
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ServerRequestSubmit");
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

	public override bool ItemFilter(Item item, int targetSlot)
	{
		if ((Object)(object)ItemModAssociatedEntity<HeadEntity>.GetAssociatedEntity(item) == (Object)null)
		{
			return false;
		}
		return base.ItemFilter(item, targetSlot);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void ServerRequestSubmit()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null)
		{
			return;
		}
		HeadEntity associatedEntity = ItemModAssociatedEntity<HeadEntity>.GetAssociatedEntity(slot);
		if ((Object)(object)associatedEntity != (Object)null && !CanSubmitHead(associatedEntity))
		{
			return;
		}
		if ((Object)(object)associatedEntity != (Object)null)
		{
			if (CurrentTrophyData == null)
			{
				CurrentTrophyData = Pool.Get<HeadData>();
				associatedEntity.CurrentTrophyData.CopyTo(CurrentTrophyData);
				CurrentTrophyData.count = 1u;
			}
			else
			{
				HeadData currentTrophyData = CurrentTrophyData;
				currentTrophyData.count++;
			}
		}
		slot.Remove();
		SendNetworkUpdate();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void ServerRequestClear()
	{
		if (CurrentTrophyData != null)
		{
			Pool.Free<HeadData>(ref CurrentTrophyData);
			SendNetworkUpdate();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (CurrentTrophyData != null)
		{
			info.msg.headData = Pool.Get<HeadData>();
			CurrentTrophyData.CopyTo(info.msg.headData);
		}
	}

	public bool CanSubmitHead(HeadEntity headEnt)
	{
		bool result = false;
		bool num = CurrentTrophyData != null;
		if (num && headEnt.CurrentTrophyData.entitySource == CurrentTrophyData.entitySource && headEnt.CurrentTrophyData.playerId == CurrentTrophyData.playerId && headEnt.CurrentTrophyData.horseBreed == CurrentTrophyData.horseBreed)
		{
			result = true;
		}
		if (!num)
		{
			TrophyRoot[] trophies = Trophies;
			foreach (TrophyRoot trophyRoot in trophies)
			{
				if (trophyRoot.Matches(headEnt))
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.headData != null)
		{
			if (CurrentTrophyData == null)
			{
				CurrentTrophyData = Pool.Get<HeadData>();
			}
			info.msg.headData.CopyTo(CurrentTrophyData);
		}
		else if (CurrentTrophyData != null)
		{
			Pool.Free<HeadData>(ref CurrentTrophyData);
		}
	}

	public override void ResetState()
	{
		base.ResetState();
		if (CurrentTrophyData != null)
		{
			Pool.Free<HeadData>(ref CurrentTrophyData);
		}
		TrophyRoot[] trophies = Trophies;
		for (int i = 0; i < trophies.Length; i++)
		{
			TrophyRoot trophyRoot = trophies[i];
			if ((Object)(object)trophyRoot.Root != (Object)null)
			{
				trophyRoot.Root.SetActive(false);
			}
		}
		if ((Object)(object)NameRoot != (Object)null)
		{
			NameRoot.SetActive(false);
		}
		if ((Object)(object)MaleRope != (Object)null)
		{
			MaleRope.SetActive(false);
		}
		if ((Object)(object)FemaleRope != (Object)null)
		{
			FemaleRope.SetActive(false);
		}
	}
}
