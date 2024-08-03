using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class Telephone : ContainerIOEntity, ICassettePlayer
{
	public enum CallState
	{
		Idle,
		Dialing,
		Ringing,
		InProcess
	}

	public enum DialFailReason
	{
		TimedOut,
		Engaged,
		WrongNumber,
		CallSelf,
		RemoteHangUp,
		NetworkBusy,
		TimeOutDuringCall,
		SelfHangUp
	}

	public const int MaxPhoneNameLength = 20;

	public const int MaxSavedNumbers = 10;

	public Transform PhoneHotspot;

	public Transform AnsweringMachineHotspot;

	public Transform[] HandsetRoots = null;

	public ItemDefinition[] ValidCassettes;

	public Transform ParentedHandsetTransform = null;

	public LineRenderer CableLineRenderer = null;

	public Transform CableStartPoint = null;

	public Transform CableEndPoint = null;

	public float LineDroopAmount = 0.25f;

	public PhoneController Controller = null;

	public uint AnsweringMessageId => ((Object)(object)cachedCassette != (Object)null) ? cachedCassette.AudioId : 0u;

	public Cassette cachedCassette { get; private set; } = null;


	public BaseEntity ToBaseEntity => this;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Telephone.OnRpcMessage", 0);
		try
		{
			if (rpc == 1529322558 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AnswerPhone "));
				}
				TimeWarning val2 = TimeWarning.New("AnswerPhone", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1529322558u, "AnswerPhone", this, player, 3f))
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
							AnswerPhone(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AnswerPhone");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2754362156u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ClearCurrentUser "));
				}
				TimeWarning val5 = TimeWarning.New("ClearCurrentUser", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2754362156u, "ClearCurrentUser", this, player, 9f))
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
							ClearCurrentUser(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ClearCurrentUser");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 1095090232 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - InitiateCall "));
				}
				TimeWarning val8 = TimeWarning.New("InitiateCall", 0);
				try
				{
					TimeWarning val9 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1095090232u, "InitiateCall", this, player, 3f))
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
							InitiateCall(msg4);
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in InitiateCall");
					}
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
				}
				return true;
			}
			if (rpc == 2606442785u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_AddSavedNumber "));
				}
				TimeWarning val11 = TimeWarning.New("Server_AddSavedNumber", 0);
				try
				{
					TimeWarning val12 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2606442785u, "Server_AddSavedNumber", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(2606442785u, "Server_AddSavedNumber", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val12)?.Dispose();
					}
					try
					{
						TimeWarning val13 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg5 = rPCMessage;
							Server_AddSavedNumber(msg5);
						}
						finally
						{
							((IDisposable)val13)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in Server_AddSavedNumber");
					}
				}
				finally
				{
					((IDisposable)val11)?.Dispose();
				}
				return true;
			}
			if (rpc == 1402406333 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RemoveSavedNumber "));
				}
				TimeWarning val14 = TimeWarning.New("Server_RemoveSavedNumber", 0);
				try
				{
					TimeWarning val15 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1402406333u, "Server_RemoveSavedNumber", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(1402406333u, "Server_RemoveSavedNumber", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val15)?.Dispose();
					}
					try
					{
						TimeWarning val16 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg6 = rPCMessage;
							Server_RemoveSavedNumber(msg6);
						}
						finally
						{
							((IDisposable)val16)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in Server_RemoveSavedNumber");
					}
				}
				finally
				{
					((IDisposable)val14)?.Dispose();
				}
				return true;
			}
			if (rpc == 942544266 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RequestPhoneDirectory "));
				}
				TimeWarning val17 = TimeWarning.New("Server_RequestPhoneDirectory", 0);
				try
				{
					TimeWarning val18 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(942544266u, "Server_RequestPhoneDirectory", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(942544266u, "Server_RequestPhoneDirectory", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val18)?.Dispose();
					}
					try
					{
						TimeWarning val19 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg7 = rPCMessage;
							Server_RequestPhoneDirectory(msg7);
						}
						finally
						{
							((IDisposable)val19)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in Server_RequestPhoneDirectory");
					}
				}
				finally
				{
					((IDisposable)val17)?.Dispose();
				}
				return true;
			}
			if (rpc == 1240133378 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerDeleteVoicemail "));
				}
				TimeWarning val20 = TimeWarning.New("ServerDeleteVoicemail", 0);
				try
				{
					TimeWarning val21 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1240133378u, "ServerDeleteVoicemail", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(1240133378u, "ServerDeleteVoicemail", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val21)?.Dispose();
					}
					try
					{
						TimeWarning val22 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg8 = rPCMessage;
							ServerDeleteVoicemail(msg8);
						}
						finally
						{
							((IDisposable)val22)?.Dispose();
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in ServerDeleteVoicemail");
					}
				}
				finally
				{
					((IDisposable)val20)?.Dispose();
				}
				return true;
			}
			if (rpc == 1221129498 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerHangUp "));
				}
				TimeWarning val23 = TimeWarning.New("ServerHangUp", 0);
				try
				{
					TimeWarning val24 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg9 = rPCMessage;
						ServerHangUp(msg9);
					}
					finally
					{
						((IDisposable)val24)?.Dispose();
					}
				}
				catch (Exception ex8)
				{
					Debug.LogException(ex8);
					player.Kick("RPC Error in ServerHangUp");
				}
				finally
				{
					((IDisposable)val23)?.Dispose();
				}
				return true;
			}
			if (rpc == 239260010 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerPlayVoicemail "));
				}
				TimeWarning val25 = TimeWarning.New("ServerPlayVoicemail", 0);
				try
				{
					TimeWarning val26 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(239260010u, "ServerPlayVoicemail", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(239260010u, "ServerPlayVoicemail", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val26)?.Dispose();
					}
					try
					{
						TimeWarning val27 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg10 = rPCMessage;
							ServerPlayVoicemail(msg10);
						}
						finally
						{
							((IDisposable)val27)?.Dispose();
						}
					}
					catch (Exception ex9)
					{
						Debug.LogException(ex9);
						player.Kick("RPC Error in ServerPlayVoicemail");
					}
				}
				finally
				{
					((IDisposable)val25)?.Dispose();
				}
				return true;
			}
			if (rpc == 189198880 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerSendVoicemail "));
				}
				TimeWarning val28 = TimeWarning.New("ServerSendVoicemail", 0);
				try
				{
					TimeWarning val29 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(189198880u, "ServerSendVoicemail", this, player, 5uL))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val29)?.Dispose();
					}
					try
					{
						TimeWarning val30 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg11 = rPCMessage;
							ServerSendVoicemail(msg11);
						}
						finally
						{
							((IDisposable)val30)?.Dispose();
						}
					}
					catch (Exception ex10)
					{
						Debug.LogException(ex10);
						player.Kick("RPC Error in ServerSendVoicemail");
					}
				}
				finally
				{
					((IDisposable)val28)?.Dispose();
				}
				return true;
			}
			if (rpc == 2760189029u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ServerStopVoicemail "));
				}
				TimeWarning val31 = TimeWarning.New("ServerStopVoicemail", 0);
				try
				{
					TimeWarning val32 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2760189029u, "ServerStopVoicemail", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(2760189029u, "ServerStopVoicemail", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val32)?.Dispose();
					}
					try
					{
						TimeWarning val33 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg12 = rPCMessage;
							ServerStopVoicemail(msg12);
						}
						finally
						{
							((IDisposable)val33)?.Dispose();
						}
					}
					catch (Exception ex11)
					{
						Debug.LogException(ex11);
						player.Kick("RPC Error in ServerStopVoicemail");
					}
				}
				finally
				{
					((IDisposable)val31)?.Dispose();
				}
				return true;
			}
			if (rpc == 3900772076u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetCurrentUser "));
				}
				TimeWarning val34 = TimeWarning.New("SetCurrentUser", 0);
				try
				{
					TimeWarning val35 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3900772076u, "SetCurrentUser", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val35)?.Dispose();
					}
					try
					{
						TimeWarning val36 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage currentUser = rPCMessage;
							SetCurrentUser(currentUser);
						}
						finally
						{
							((IDisposable)val36)?.Dispose();
						}
					}
					catch (Exception ex12)
					{
						Debug.LogException(ex12);
						player.Kick("RPC Error in SetCurrentUser");
					}
				}
				finally
				{
					((IDisposable)val34)?.Dispose();
				}
				return true;
			}
			if (rpc == 2760249627u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - UpdatePhoneName "));
				}
				TimeWarning val37 = TimeWarning.New("UpdatePhoneName", 0);
				try
				{
					TimeWarning val38 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2760249627u, "UpdatePhoneName", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(2760249627u, "UpdatePhoneName", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val38)?.Dispose();
					}
					try
					{
						TimeWarning val39 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg13 = rPCMessage;
							UpdatePhoneName(msg13);
						}
						finally
						{
							((IDisposable)val39)?.Dispose();
						}
					}
					catch (Exception ex13)
					{
						Debug.LogException(ex13);
						player.Kick("RPC Error in UpdatePhoneName");
					}
				}
				finally
				{
					((IDisposable)val37)?.Dispose();
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

	public override void Save(SaveInfo info)
	{
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (info.msg.telephone == null)
		{
			info.msg.telephone = Pool.Get<Telephone>();
		}
		info.msg.telephone.phoneNumber = Controller.PhoneNumber;
		info.msg.telephone.phoneName = Controller.PhoneName;
		info.msg.telephone.lastNumber = Controller.lastDialedNumber;
		info.msg.telephone.savedNumbers = Controller.savedNumbers;
		if (Controller.savedVoicemail != null)
		{
			info.msg.telephone.voicemail = Pool.GetList<VoicemailEntry>();
			foreach (VoicemailEntry item in Controller.savedVoicemail)
			{
				info.msg.telephone.voicemail.Add(item);
			}
		}
		if (!info.forDisk)
		{
			info.msg.telephone.usingPlayer = Controller.currentPlayerRef.uid;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		Controller.ServerInit();
		ItemContainer itemContainer = base.inventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(CanAcceptItem));
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		Controller.PostServerLoad();
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		Controller.DoServerDestroy();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(9f)]
	public void ClearCurrentUser(RPCMessage msg)
	{
		Controller.ClearCurrentUser(msg);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void SetCurrentUser(RPCMessage msg)
	{
		Controller.SetCurrentUser(msg);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void InitiateCall(RPCMessage msg)
	{
		Controller.InitiateCall(msg);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void AnswerPhone(RPCMessage msg)
	{
		Controller.AnswerPhone(msg);
	}

	[RPC_Server]
	private void ServerHangUp(RPCMessage msg)
	{
		Controller.ServerHangUp(msg);
	}

	public void OnCassetteInserted(Cassette c)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		cachedCassette = c;
		ClientRPC<NetworkableId>(null, "ClientOnCassetteChanged", c.net.ID);
	}

	public void OnCassetteRemoved(Cassette c)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		cachedCassette = null;
		Controller.DeleteAllVoicemail();
		ClientRPC<NetworkableId>(null, "ClientOnCassetteChanged", default(NetworkableId));
	}

	private bool CanAcceptItem(Item item, int targetSlot)
	{
		ItemDefinition[] validCassettes = ValidCassettes;
		foreach (ItemDefinition itemDefinition in validCassettes)
		{
			if ((Object)(object)itemDefinition == (Object)(object)item.info)
			{
				return true;
			}
		}
		return false;
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		Controller.DestroyShared();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public void UpdatePhoneName(RPCMessage msg)
	{
		Controller.UpdatePhoneName(msg);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public void Server_RequestPhoneDirectory(RPCMessage msg)
	{
		Controller.Server_RequestPhoneDirectory(msg);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public void Server_AddSavedNumber(RPCMessage msg)
	{
		Controller.Server_AddSavedNumber(msg);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public void Server_RemoveSavedNumber(RPCMessage msg)
	{
		Controller.Server_RemoveSavedNumber(msg);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	public void ServerSendVoicemail(RPCMessage msg)
	{
		Controller.ServerSendVoicemail(msg);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public void ServerPlayVoicemail(RPCMessage msg)
	{
		Controller.ServerPlayVoicemail(msg);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public void ServerStopVoicemail(RPCMessage msg)
	{
		Controller.ServerStopVoicemail(msg);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	public void ServerDeleteVoicemail(RPCMessage msg)
	{
		Controller.ServerDeleteVoicemail(msg);
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (Controller.serverState == CallState.Ringing || Controller.serverState == CallState.InProcess)
		{
			return base.GetPassthroughAmount(outputSlot);
		}
		return 0;
	}

	public override void Load(LoadInfo info)
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg?.telephone == null)
		{
			return;
		}
		Controller.PhoneNumber = info.msg.telephone.phoneNumber;
		Controller.PhoneName = info.msg.telephone.phoneName;
		Controller.lastDialedNumber = info.msg.telephone.lastNumber;
		Controller.savedVoicemail = Pool.GetList<VoicemailEntry>();
		foreach (VoicemailEntry item in info.msg.telephone.voicemail)
		{
			Controller.savedVoicemail.Add(item);
			item.ShouldPool = false;
		}
		if (!info.fromDisk)
		{
			Controller.currentPlayerRef.uid = info.msg.telephone.usingPlayer;
		}
		PhoneDirectory savedNumbers = Controller.savedNumbers;
		if (savedNumbers != null)
		{
			savedNumbers.ResetToPool();
		}
		Controller.savedNumbers = info.msg.telephone.savedNumbers;
		if (Controller.savedNumbers != null)
		{
			Controller.savedNumbers.ShouldPool = false;
		}
		if (info.fromDisk)
		{
			SetFlag(Flags.Busy, b: false);
		}
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!base.CanPickup(player))
		{
			return false;
		}
		return (Object)(object)Controller.currentPlayer == (Object)null;
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			if (Controller.RequirePower && next.HasFlag(Flags.Busy) && !next.HasFlag(Flags.Reserved8))
			{
				Controller.ServerHangUp();
			}
			if (old.HasFlag(Flags.Busy) != next.HasFlag(Flags.Busy))
			{
				if (next.HasFlag(Flags.Busy))
				{
					if (!((FacepunchBehaviour)this).IsInvoking((Action)Controller.WatchForDisconnects))
					{
						((FacepunchBehaviour)this).InvokeRepeating((Action)Controller.WatchForDisconnects, 0f, 0.1f);
					}
				}
				else
				{
					((FacepunchBehaviour)this).CancelInvoke((Action)Controller.WatchForDisconnects);
				}
			}
		}
		Controller.OnFlagsChanged(old, next);
	}
}
