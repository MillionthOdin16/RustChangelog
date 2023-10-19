using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class ComputerStation : BaseMountable
{
	public const Flags Flag_HasFullControl = Flags.Reserved2;

	[Header("Computer")]
	public GameObjectRef menuPrefab;

	public ComputerMenu computerMenu;

	public EntityRef currentlyControllingEnt;

	public List<string> controlBookmarks = new List<string>();

	public Transform leftHandIKPosition;

	public Transform rightHandIKPosition;

	public SoundDefinition turnOnSoundDef;

	public SoundDefinition turnOffSoundDef;

	public SoundDefinition onLoopSoundDef;

	public bool isStatic;

	public float autoGatherRadius;

	private ulong currentPlayerID;

	private float nextAddTime;

	private static readonly char[] BookmarkSplit = new char[1] { ';' };

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ComputerStation.OnRpcMessage", 0);
		try
		{
			if (rpc == 481778085 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - AddBookmark "));
				}
				TimeWarning val2 = TimeWarning.New("AddBookmark", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg2 = rPCMessage;
						AddBookmark(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in AddBookmark");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 552248427 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - BeginControllingBookmark "));
				}
				TimeWarning val2 = TimeWarning.New("BeginControllingBookmark", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg3 = rPCMessage;
						BeginControllingBookmark(msg3);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in BeginControllingBookmark");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2498687923u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - DeleteBookmark "));
				}
				TimeWarning val2 = TimeWarning.New("DeleteBookmark", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg4 = rPCMessage;
						DeleteBookmark(msg4);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex3)
				{
					Debug.LogException(ex3);
					player.Kick("RPC Error in DeleteBookmark");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2139261430 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_DisconnectControl "));
				}
				TimeWarning val2 = TimeWarning.New("Server_DisconnectControl", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg5 = rPCMessage;
						Server_DisconnectControl(msg5);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex4)
				{
					Debug.LogException(ex4);
					player.Kick("RPC Error in Server_DisconnectControl");
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

	public bool AllowPings()
	{
		BaseEntity baseEntity = currentlyControllingEnt.Get(base.isServer);
		if ((Object)(object)baseEntity != (Object)null && baseEntity is IRemoteControllable remoteControllable && remoteControllable.CanPing)
		{
			return true;
		}
		return false;
	}

	public static bool IsValidIdentifier(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return false;
		}
		if (str.Length > 32)
		{
			return false;
		}
		return StringEx.IsAlphaNumeric(str);
	}

	public override void DestroyShared()
	{
		if (base.isServer && Object.op_Implicit((Object)(object)GetMounted()))
		{
			StopControl(GetMounted());
		}
		base.DestroyShared();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)GatherStaticCameras, 5f);
	}

	public void GatherStaticCameras()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isLoadingSave)
		{
			((FacepunchBehaviour)this).Invoke((Action)GatherStaticCameras, 1f);
		}
		else
		{
			if (!isStatic || !(autoGatherRadius > 0f))
			{
				return;
			}
			List<BaseEntity> list = Pool.GetList<BaseEntity>();
			Vis.Entities(((Component)this).transform.position, autoGatherRadius, list, 256, (QueryTriggerInteraction)1);
			foreach (BaseEntity item in list)
			{
				IRemoteControllable component = ((Component)item).GetComponent<IRemoteControllable>();
				if (component != null)
				{
					CCTV_RC component2 = ((Component)item).GetComponent<CCTV_RC>();
					if (!((Object)(object)component2 == (Object)null) && component2.IsStatic() && !controlBookmarks.Contains(component.GetIdentifier()))
					{
						ForceAddBookmark(component.GetIdentifier());
					}
				}
			}
			Pool.FreeList<BaseEntity>(ref list);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		GatherStaticCameras();
	}

	public void StopControl(BasePlayer ply)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = currentlyControllingEnt.Get(serverside: true);
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			((Component)baseEntity).GetComponent<IRemoteControllable>().StopControl(new CameraViewerId(currentPlayerID, 0L));
		}
		if (Object.op_Implicit((Object)(object)ply))
		{
			ply.net.SwitchSecondaryGroup((Group)null);
		}
		currentlyControllingEnt.uid = default(NetworkableId);
		currentPlayerID = 0uL;
		SetFlag(Flags.Reserved2, b: false, recursive: false, networkupdate: false);
		SendNetworkUpdate();
		SendControlBookmarks(ply);
		((FacepunchBehaviour)this).CancelInvoke((Action)ControlCheck);
		((FacepunchBehaviour)this).CancelInvoke((Action)CheckCCTVAchievement);
	}

	public bool IsPlayerAdmin(BasePlayer player)
	{
		return (Object)(object)player == (Object)(object)_mounted;
	}

	[RPC_Server]
	public void DeleteBookmark(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!IsPlayerAdmin(player) || isStatic)
		{
			return;
		}
		string text = msg.read.String(256, false);
		if (IsValidIdentifier(text) && controlBookmarks.Contains(text))
		{
			controlBookmarks.Remove(text);
			SendControlBookmarks(player);
			BaseEntity baseEntity = currentlyControllingEnt.Get(serverside: true);
			IRemoteControllable remoteControllable = default(IRemoteControllable);
			if ((Object)(object)baseEntity != (Object)null && ((Component)baseEntity).TryGetComponent<IRemoteControllable>(ref remoteControllable) && remoteControllable.GetIdentifier() == text)
			{
				StopControl(player);
			}
		}
	}

	[RPC_Server]
	public void Server_DisconnectControl(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (IsPlayerAdmin(player))
		{
			StopControl(player);
		}
	}

	[RPC_Server]
	public void BeginControllingBookmark(RPCMessage msg)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!IsPlayerAdmin(player))
		{
			return;
		}
		string text = msg.read.String(256, false);
		if (!IsValidIdentifier(text) || !controlBookmarks.Contains(text))
		{
			return;
		}
		IRemoteControllable remoteControllable = RemoteControlEntity.FindByID(text);
		if (remoteControllable == null)
		{
			return;
		}
		BaseEntity ent = remoteControllable.GetEnt();
		if ((Object)(object)ent == (Object)null)
		{
			Debug.LogWarning((object)("RC identifier " + text + " was found but has a null or destroyed entity, this should never happen"));
		}
		else if (remoteControllable.CanControl(player.userID) && !(Vector3.Distance(((Component)this).transform.position, ((Component)ent).transform.position) >= remoteControllable.MaxRange))
		{
			BaseEntity baseEntity = currentlyControllingEnt.Get(serverside: true);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				((Component)baseEntity).GetComponent<IRemoteControllable>()?.StopControl(new CameraViewerId(currentPlayerID, 0L));
			}
			player.net.SwitchSecondaryGroup(ent.net.group);
			currentlyControllingEnt.uid = ent.net.ID;
			currentPlayerID = player.userID;
			bool b = remoteControllable.InitializeControl(new CameraViewerId(currentPlayerID, 0L));
			SetFlag(Flags.Reserved2, b, recursive: false, networkupdate: false);
			SendNetworkUpdateImmediate();
			SendControlBookmarks(player);
			if (GameInfo.HasAchievements && remoteControllable.GetEnt() is CCTV_RC)
			{
				((FacepunchBehaviour)this).InvokeRepeating((Action)CheckCCTVAchievement, 1f, 3f);
			}
			((FacepunchBehaviour)this).InvokeRepeating((Action)ControlCheck, 0f, 0f);
		}
	}

	private void CheckCCTVAchievement()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)_mounted != (Object)null))
		{
			return;
		}
		BaseEntity baseEntity = currentlyControllingEnt.Get(serverside: true);
		if (!((Object)(object)baseEntity != (Object)null) || !(baseEntity is CCTV_RC cCTV_RC))
		{
			return;
		}
		foreach (Connection subscriber in _mounted.net.secondaryGroup.subscribers)
		{
			if (!subscriber.active)
			{
				continue;
			}
			BasePlayer basePlayer = subscriber.player as BasePlayer;
			if (!((Object)(object)basePlayer == (Object)null))
			{
				Vector3 val = basePlayer.CenterPoint();
				Vector3 val2 = val - cCTV_RC.pitch.position;
				float num = Vector3.Dot(((Vector3)(ref val2)).normalized, cCTV_RC.pitch.forward);
				Vector3 val3 = cCTV_RC.pitch.InverseTransformPoint(val);
				if (num > 0.6f && ((Vector3)(ref val3)).magnitude < 10f)
				{
					_mounted.GiveAchievement("BIG_BROTHER");
					((FacepunchBehaviour)this).CancelInvoke((Action)CheckCCTVAchievement);
					break;
				}
			}
		}
	}

	public bool CanAddBookmark(BasePlayer player)
	{
		if (!IsPlayerAdmin(player))
		{
			return false;
		}
		if (isStatic)
		{
			return false;
		}
		if (Time.realtimeSinceStartup < nextAddTime)
		{
			return false;
		}
		if (controlBookmarks.Count > 3)
		{
			player.ChatMessage("Too many bookmarks, delete some");
			return false;
		}
		return true;
	}

	public void ForceAddBookmark(string identifier)
	{
		if (controlBookmarks.Count >= 128 || !IsValidIdentifier(identifier) || controlBookmarks.Contains(identifier))
		{
			return;
		}
		IRemoteControllable remoteControllable = RemoteControlEntity.FindByID(identifier);
		if (remoteControllable != null)
		{
			if ((Object)(object)remoteControllable.GetEnt() == (Object)null)
			{
				Debug.LogWarning((object)("RC identifier " + identifier + " was found but has a null or destroyed entity, this should never happen"));
			}
			else
			{
				controlBookmarks.Add(identifier);
			}
		}
	}

	[RPC_Server]
	public void AddBookmark(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (IsPlayerAdmin(player) && !isStatic)
		{
			if (Time.realtimeSinceStartup < nextAddTime)
			{
				player.ChatMessage("Slow down...");
				return;
			}
			if (controlBookmarks.Count >= 128)
			{
				player.ChatMessage("Too many bookmarks, delete some");
				return;
			}
			nextAddTime = Time.realtimeSinceStartup + 1f;
			string identifier = msg.read.String(256, false);
			ForceAddBookmark(identifier);
			SendControlBookmarks(player);
		}
	}

	public void ControlCheck()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		BaseEntity baseEntity = currentlyControllingEnt.Get(base.isServer);
		if (Object.op_Implicit((Object)(object)baseEntity) && Object.op_Implicit((Object)(object)_mounted))
		{
			IRemoteControllable component = ((Component)baseEntity).GetComponent<IRemoteControllable>();
			if (component != null && component.CanControl(_mounted.userID) && Vector3.Distance(((Component)this).transform.position, ((Component)baseEntity).transform.position) < component.MaxRange)
			{
				flag = true;
				_mounted.net.SwitchSecondaryGroup(baseEntity.net.group);
			}
		}
		if (!flag)
		{
			StopControl(_mounted);
		}
	}

	public string GenerateControlBookmarkString()
	{
		return string.Join(";", controlBookmarks);
	}

	public void SendControlBookmarks(BasePlayer player)
	{
		if (!((Object)(object)player == (Object)null))
		{
			string arg = GenerateControlBookmarkString();
			ClientRPCPlayer(null, player, "ReceiveBookmarks", arg);
		}
	}

	public override void OnPlayerMounted()
	{
		base.OnPlayerMounted();
		BasePlayer mounted = _mounted;
		if (Object.op_Implicit((Object)(object)mounted))
		{
			SendControlBookmarks(mounted);
		}
		SetFlag(Flags.On, b: true);
	}

	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		StopControl(player);
		SetFlag(Flags.On, b: false);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (HasFlag(Flags.Reserved2) && currentlyControllingEnt.IsValid(serverside: true))
		{
			((Component)currentlyControllingEnt.Get(serverside: true)).GetComponent<IRemoteControllable>().UserInput(inputState, new CameraViewerId(player.userID, 0L));
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.ioEntity = Pool.Get<IOEntity>();
			info.msg.ioEntity.genericEntRef1 = currentlyControllingEnt.uid;
		}
		else
		{
			info.msg.computerStation = Pool.Get<ComputerStation>();
			info.msg.computerStation.bookmarks = GenerateControlBookmarkString();
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (!info.fromDisk)
		{
			if (info.msg.ioEntity != null)
			{
				currentlyControllingEnt.uid = info.msg.ioEntity.genericEntRef1;
			}
		}
		else
		{
			if (info.msg.computerStation == null)
			{
				return;
			}
			string[] array = info.msg.computerStation.bookmarks.Split(BookmarkSplit, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				if (IsValidIdentifier(text))
				{
					controlBookmarks.Add(text);
				}
			}
		}
	}
}
