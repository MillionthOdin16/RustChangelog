using System;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class CarvablePumpkin : BaseOven, ILOD, ISignage, IUGCBrowserEntity
{
	private List<ulong> editHistory = new List<ulong>();

	private const float TextureRequestTimeout = 15f;

	public GameObjectRef changeTextDialog;

	public MeshPaintableSource[] paintableSources;

	[NonSerialized]
	public uint[] textureIDs;

	public FileStorage.Type FileType => FileStorage.Type.png;

	public NetworkableId NetworkID => net.ID;

	public UGCType ContentType => UGCType.ImagePng;

	public List<ulong> EditingHistory => editHistory;

	public uint[] GetContentCRCs => textureIDs;

	public BaseNetworkable UgcEntity => this;

	public Vector2i TextureSize
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (paintableSources == null || paintableSources.Length == 0)
			{
				return Vector2i.zero;
			}
			MeshPaintableSource meshPaintableSource = paintableSources[0];
			return new Vector2i(meshPaintableSource.texWidth, meshPaintableSource.texHeight);
		}
	}

	public int TextureCount
	{
		get
		{
			MeshPaintableSource[] array = paintableSources;
			if (array == null)
			{
				return 0;
			}
			return array.Length;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CarvablePumpkin.OnRpcMessage", 0);
		try
		{
			if (rpc == 1455609404 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - LockSign "));
				}
				TimeWarning val2 = TimeWarning.New("LockSign", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1455609404u, "LockSign", this, player, 3f))
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
							LockSign(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in LockSign");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4149904254u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - UnLockSign "));
				}
				TimeWarning val2 = TimeWarning.New("UnLockSign", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4149904254u, "UnLockSign", this, player, 3f))
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
							UnLockSign(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in UnLockSign");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1255380462 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - UpdateSign "));
				}
				TimeWarning val2 = TimeWarning.New("UpdateSign", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1255380462u, "UpdateSign", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(1255380462u, "UpdateSign", this, player, 5f))
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
							UpdateSign(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in UpdateSign");
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

	public uint[] GetTextureCRCs()
	{
		return textureIDs;
	}

	public virtual bool CanUpdateSign(BasePlayer player)
	{
		if (player.IsAdmin || player.IsDeveloper)
		{
			return true;
		}
		if (!player.CanBuild())
		{
			return false;
		}
		if (IsLocked())
		{
			return player.userID == base.OwnerID;
		}
		return true;
	}

	public bool CanUnlockSign(BasePlayer player)
	{
		if (!IsLocked())
		{
			return false;
		}
		return CanUpdateSign(player);
	}

	public bool CanLockSign(BasePlayer player)
	{
		if (IsLocked())
		{
			return false;
		}
		return CanUpdateSign(player);
	}

	public override void Load(LoadInfo info)
	{
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		EnsureInitialized();
		bool flag = false;
		if (info.msg.sign != null)
		{
			uint num = textureIDs[0];
			if (info.msg.sign.imageIds != null && info.msg.sign.imageIds.Count > 0)
			{
				int num2 = Mathf.Min(info.msg.sign.imageIds.Count, textureIDs.Length);
				for (int i = 0; i < num2; i++)
				{
					uint num3 = info.msg.sign.imageIds[i];
					bool flag2 = num3 != textureIDs[i];
					flag = flag || flag2;
					textureIDs[i] = num3;
				}
			}
			else
			{
				flag = num != info.msg.sign.imageid;
				textureIDs[0] = info.msg.sign.imageid;
			}
		}
		if (!base.isServer)
		{
			return;
		}
		bool flag3 = false;
		for (int j = 0; j < paintableSources.Length; j++)
		{
			uint num4 = textureIDs[j];
			if (num4 != 0)
			{
				byte[] array = FileStorage.server.Get(num4, FileStorage.Type.png, net.ID, (uint)j);
				if (array == null)
				{
					Log($"Frame {j} (id={num4}) doesn't exist, clearing");
					textureIDs[j] = 0u;
				}
				flag3 = flag3 || array != null;
			}
		}
		if (!flag3)
		{
			SetFlag(Flags.Locked, b: false);
		}
		if (info.msg.sign == null)
		{
			return;
		}
		if (info.msg.sign.editHistory != null)
		{
			if (editHistory == null)
			{
				editHistory = Pool.GetList<ulong>();
			}
			editHistory.Clear();
			{
				foreach (ulong item in info.msg.sign.editHistory)
				{
					editHistory.Add(item);
				}
				return;
			}
		}
		if (editHistory != null)
		{
			Pool.FreeList<ulong>(ref editHistory);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void LockSign(RPCMessage msg)
	{
		if (msg.player.CanInteract() && CanUpdateSign(msg.player))
		{
			SetFlag(Flags.Locked, b: true);
			SendNetworkUpdate();
			base.OwnerID = msg.player.userID;
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void UnLockSign(RPCMessage msg)
	{
		if (msg.player.CanInteract() && CanUnlockSign(msg.player))
		{
			SetFlag(Flags.Locked, b: false);
			SendNetworkUpdate();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		EnsureInitialized();
		List<uint> list = Pool.GetList<uint>();
		uint[] array = textureIDs;
		foreach (uint item in array)
		{
			list.Add(item);
		}
		info.msg.sign = Pool.Get<Sign>();
		info.msg.sign.imageid = 0u;
		info.msg.sign.imageIds = list;
		if (editHistory.Count <= 0)
		{
			return;
		}
		info.msg.sign.editHistory = Pool.GetList<ulong>();
		foreach (ulong item2 in editHistory)
		{
			info.msg.sign.editHistory.Add(item2);
		}
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (net != null)
		{
			FileStorage.server.RemoveAllByEntity(net.ID);
		}
		if (textureIDs != null)
		{
			Array.Clear(textureIDs, 0, textureIDs.Length);
		}
		base.OnKilled(info);
	}

	public override void OnPickedUpPreItemMove(Item createdItem, BasePlayer player)
	{
		base.OnPickedUpPreItemMove(createdItem, player);
		bool flag = false;
		uint[] array = textureIDs;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != 0)
			{
				flag = true;
				break;
			}
		}
		ItemModSign itemModSign = default(ItemModSign);
		if (flag && ((Component)createdItem.info).TryGetComponent<ItemModSign>(ref itemModSign))
		{
			itemModSign.OnSignPickedUp(this, this, createdItem);
		}
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		ItemModSign itemModSign = default(ItemModSign);
		if (((Component)fromItem.info).TryGetComponent<ItemModSign>(ref itemModSign))
		{
			SignContent associatedEntity = ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(fromItem);
			if ((Object)(object)associatedEntity != (Object)null)
			{
				associatedEntity.CopyInfoToSign(this, this);
			}
		}
	}

	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	public void SetTextureCRCs(uint[] crcs)
	{
		textureIDs = new uint[crcs.Length];
		crcs.CopyTo(textureIDs, 0);
		SendNetworkUpdate();
	}

	private void LogEdit(BasePlayer byPlayer)
	{
		if (!editHistory.Contains(byPlayer.userID))
		{
			editHistory.Insert(0, byPlayer.userID);
			int num = 0;
			while (editHistory.Count > 5 && num < 10)
			{
				editHistory.RemoveAt(5);
				num++;
			}
		}
	}

	public void ClearContent()
	{
		SetTextureCRCs(Array.Empty<uint>());
	}

	public override string Categorize()
	{
		return "sign";
	}

	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (paintableSources != null && paintableSources.Length > 1)
		{
			MeshPaintableSource meshPaintableSource = paintableSources[0];
			for (int i = 1; i < paintableSources.Length; i++)
			{
				MeshPaintableSource obj = paintableSources[i];
				obj.texWidth = meshPaintableSource.texWidth;
				obj.texHeight = meshPaintableSource.texHeight;
			}
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.MaxDistance(5f)]
	public void UpdateSign(RPCMessage msg)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)msg.player == (Object)null || !CanUpdateSign(msg.player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num < 0 || num >= paintableSources.Length)
		{
			return;
		}
		byte[] array = msg.read.BytesWithSize(10485760u, false);
		if (msg.read.Unread > 0 && msg.read.Bit() && !msg.player.IsAdmin)
		{
			Debug.LogWarning((object)$"{msg.player} tried to upload a sign from a file but they aren't admin, ignoring");
			return;
		}
		EnsureInitialized();
		if (array == null)
		{
			if (textureIDs[num] != 0)
			{
				FileStorage.server.RemoveExact(textureIDs[num], FileStorage.Type.png, net.ID, (uint)num);
			}
			textureIDs[num] = 0u;
		}
		else
		{
			if (!ImageProcessing.IsValidPNG(array, 1024, 1024))
			{
				return;
			}
			if (textureIDs[num] != 0)
			{
				FileStorage.server.RemoveExact(textureIDs[num], FileStorage.Type.png, net.ID, (uint)num);
			}
			textureIDs[num] = FileStorage.server.Store(array, FileStorage.Type.png, net.ID, (uint)num);
		}
		LogEdit(msg.player);
		SendNetworkUpdate();
	}

	private void EnsureInitialized()
	{
		int num = Mathf.Max(paintableSources.Length, 1);
		if (textureIDs == null || textureIDs.Length != num)
		{
			Array.Resize(ref textureIDs, num);
		}
	}

	[Conditional("SIGN_DEBUG")]
	private static void SignDebugLog(string str)
	{
		Debug.Log((object)str);
	}
}
