using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

public class PhotoFrame : StorageContainer, ILOD, IImageReceiver, ISignage, IUGCBrowserEntity
{
	public GameObjectRef SignEditorDialog;

	public OverlayMeshPaintableSource PaintableSource;

	private const float TextureRequestDistance = 100f;

	private EntityRef _photoEntity;

	private uint _overlayTextureCrc;

	private List<ulong> editHistory = new List<ulong>();

	public Vector2i TextureSize => new Vector2i(PaintableSource.texWidth, PaintableSource.texHeight);

	public int TextureCount => 1;

	public NetworkableId NetworkID => net.ID;

	public FileStorage.Type FileType => FileStorage.Type.png;

	public UGCType ContentType => UGCType.ImagePng;

	public List<ulong> EditingHistory => editHistory;

	public uint[] GetContentCRCs => new uint[1] { _overlayTextureCrc };

	public BaseNetworkable UgcEntity => this;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PhotoFrame.OnRpcMessage", 0);
		try
		{
			if (rpc == 1455609404 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - LockSign "));
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
						TimeWarning val4 = TimeWarning.New("Call", 0);
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
							((IDisposable)val4)?.Dispose();
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
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - UnLockSign "));
				}
				TimeWarning val5 = TimeWarning.New("UnLockSign", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4149904254u, "UnLockSign", this, player, 3f))
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
							UnLockSign(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
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
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 1255380462 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - UpdateSign "));
				}
				TimeWarning val8 = TimeWarning.New("UpdateSign", 0);
				try
				{
					TimeWarning val9 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1255380462u, "UpdateSign", this, player, 3uL))
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
							UpdateSign(msg4);
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
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
					((IDisposable)val8)?.Dispose();
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

	public bool CanUpdateSign(BasePlayer player)
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

	[RPC_Server]
	[RPC_Server.MaxDistance(5f)]
	[RPC_Server.CallsPerSecond(3uL)]
	public void UpdateSign(RPCMessage msg)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)msg.player == (Object)null)
		{
			return;
		}
		Profiler.BeginSample("CanUpdateSign");
		bool flag = CanUpdateSign(msg.player);
		Profiler.EndSample();
		if (!flag)
		{
			return;
		}
		Profiler.BeginSample("BytesWithSize");
		byte[] array = msg.read.BytesWithSize(10485760u);
		Profiler.EndSample();
		if (array != null)
		{
			Profiler.BeginSample("IsValidPNG");
			bool flag2 = ImageProcessing.IsValidPNG(array, 1024, 1024);
			Profiler.EndSample();
			if (flag2)
			{
				FileStorage.server.RemoveAllByEntity(net.ID);
				_overlayTextureCrc = FileStorage.server.Store(array, FileStorage.Type.png, net.ID);
				LogEdit(msg.player);
				SendNetworkUpdate();
			}
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

	public override void OnKilled(HitInfo info)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (net != null)
		{
			FileStorage.server.RemoveAllByEntity(net.ID);
		}
		_overlayTextureCrc = 0u;
		base.OnKilled(info);
	}

	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	public override string Categorize()
	{
		return "sign";
	}

	public override void Load(LoadInfo info)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.photoFrame != null)
		{
			_photoEntity.uid = info.msg.photoFrame.photoEntityId;
			_overlayTextureCrc = info.msg.photoFrame.overlayImageCrc;
		}
		if (!base.isServer || info.msg.photoFrame == null)
		{
			return;
		}
		if (info.msg.photoFrame.editHistory != null)
		{
			if (editHistory == null)
			{
				editHistory = Pool.GetList<ulong>();
			}
			editHistory.Clear();
			{
				foreach (ulong item in info.msg.photoFrame.editHistory)
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

	public uint[] GetTextureCRCs()
	{
		return new uint[1] { _overlayTextureCrc };
	}

	public override void Save(SaveInfo info)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.photoFrame = Pool.Get<PhotoFrame>();
		info.msg.photoFrame.photoEntityId = _photoEntity.uid;
		info.msg.photoFrame.overlayImageCrc = _overlayTextureCrc;
		if (editHistory.Count <= 0)
		{
			return;
		}
		info.msg.photoFrame.editHistory = Pool.GetList<ulong>();
		foreach (ulong item in editHistory)
		{
			info.msg.photoFrame.editHistory.Add(item);
		}
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		base.OnItemAddedOrRemoved(item, added);
		Item item2 = ((base.inventory.itemList.Count > 0) ? base.inventory.itemList[0] : null);
		NetworkableId val = (NetworkableId)((item2 != null && item2.IsValid()) ? item2.instanceData.subEntity : default(NetworkableId));
		if (val != _photoEntity.uid)
		{
			_photoEntity.uid = val;
			SendNetworkUpdate();
		}
	}

	public override void OnPickedUpPreItemMove(Item createdItem, BasePlayer player)
	{
		base.OnPickedUpPreItemMove(createdItem, player);
		ItemModSign itemModSign = default(ItemModSign);
		if (_overlayTextureCrc != 0 && ((Component)createdItem.info).TryGetComponent<ItemModSign>(ref itemModSign))
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

	public void SetTextureCRCs(uint[] crcs)
	{
		if (crcs.Length != 0)
		{
			_overlayTextureCrc = crcs[0];
			SendNetworkUpdate();
		}
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
		_overlayTextureCrc = 0u;
		SendNetworkUpdate();
	}

	public override bool CanPickup(BasePlayer player)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		int result;
		if (base.CanPickup(player))
		{
			NetworkableId uid = _photoEntity.uid;
			result = ((!((NetworkableId)(ref uid)).IsValid) ? 1 : 0);
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}
}
