using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class MobilePhone : HeldEntity
{
	public PhoneController Controller = null;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MobilePhone.OnRpcMessage", 0);
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
						if (!RPC_Server.FromOwner.Test(1529322558u, "AnswerPhone", this, player))
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
						if (!RPC_Server.FromOwner.Test(2754362156u, "ClearCurrentUser", this, player))
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
						if (!RPC_Server.FromOwner.Test(1095090232u, "InitiateCall", this, player))
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
						if (!RPC_Server.FromOwner.Test(2606442785u, "Server_AddSavedNumber", this, player))
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
						if (!RPC_Server.FromOwner.Test(1402406333u, "Server_RemoveSavedNumber", this, player))
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
			if (rpc == 2704491961u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RequestCurrentState "));
				}
				TimeWarning val17 = TimeWarning.New("Server_RequestCurrentState", 0);
				try
				{
					TimeWarning val18 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(2704491961u, "Server_RequestCurrentState", this, player))
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
							Server_RequestCurrentState(msg7);
						}
						finally
						{
							((IDisposable)val19)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in Server_RequestCurrentState");
					}
				}
				finally
				{
					((IDisposable)val17)?.Dispose();
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
				TimeWarning val20 = TimeWarning.New("Server_RequestPhoneDirectory", 0);
				try
				{
					TimeWarning val21 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(942544266u, "Server_RequestPhoneDirectory", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(942544266u, "Server_RequestPhoneDirectory", this, player))
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
							Server_RequestPhoneDirectory(msg8);
						}
						finally
						{
							((IDisposable)val22)?.Dispose();
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in Server_RequestPhoneDirectory");
					}
				}
				finally
				{
					((IDisposable)val20)?.Dispose();
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
				TimeWarning val23 = TimeWarning.New("ServerDeleteVoicemail", 0);
				try
				{
					TimeWarning val24 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1240133378u, "ServerDeleteVoicemail", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(1240133378u, "ServerDeleteVoicemail", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val24)?.Dispose();
					}
					try
					{
						TimeWarning val25 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg9 = rPCMessage;
							ServerDeleteVoicemail(msg9);
						}
						finally
						{
							((IDisposable)val25)?.Dispose();
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in ServerDeleteVoicemail");
					}
				}
				finally
				{
					((IDisposable)val23)?.Dispose();
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
				TimeWarning val26 = TimeWarning.New("ServerHangUp", 0);
				try
				{
					TimeWarning val27 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(1221129498u, "ServerHangUp", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val27)?.Dispose();
					}
					try
					{
						TimeWarning val28 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg10 = rPCMessage;
							ServerHangUp(msg10);
						}
						finally
						{
							((IDisposable)val28)?.Dispose();
						}
					}
					catch (Exception ex9)
					{
						Debug.LogException(ex9);
						player.Kick("RPC Error in ServerHangUp");
					}
				}
				finally
				{
					((IDisposable)val26)?.Dispose();
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
				TimeWarning val29 = TimeWarning.New("ServerPlayVoicemail", 0);
				try
				{
					TimeWarning val30 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(239260010u, "ServerPlayVoicemail", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(239260010u, "ServerPlayVoicemail", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val30)?.Dispose();
					}
					try
					{
						TimeWarning val31 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg11 = rPCMessage;
							ServerPlayVoicemail(msg11);
						}
						finally
						{
							((IDisposable)val31)?.Dispose();
						}
					}
					catch (Exception ex10)
					{
						Debug.LogException(ex10);
						player.Kick("RPC Error in ServerPlayVoicemail");
					}
				}
				finally
				{
					((IDisposable)val29)?.Dispose();
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
				TimeWarning val32 = TimeWarning.New("ServerSendVoicemail", 0);
				try
				{
					TimeWarning val33 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(189198880u, "ServerSendVoicemail", this, player, 5uL))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val33)?.Dispose();
					}
					try
					{
						TimeWarning val34 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg12 = rPCMessage;
							ServerSendVoicemail(msg12);
						}
						finally
						{
							((IDisposable)val34)?.Dispose();
						}
					}
					catch (Exception ex11)
					{
						Debug.LogException(ex11);
						player.Kick("RPC Error in ServerSendVoicemail");
					}
				}
				finally
				{
					((IDisposable)val32)?.Dispose();
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
				TimeWarning val35 = TimeWarning.New("ServerStopVoicemail", 0);
				try
				{
					TimeWarning val36 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2760189029u, "ServerStopVoicemail", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(2760189029u, "ServerStopVoicemail", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val36)?.Dispose();
					}
					try
					{
						TimeWarning val37 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg13 = rPCMessage;
							ServerStopVoicemail(msg13);
						}
						finally
						{
							((IDisposable)val37)?.Dispose();
						}
					}
					catch (Exception ex12)
					{
						Debug.LogException(ex12);
						player.Kick("RPC Error in ServerStopVoicemail");
					}
				}
				finally
				{
					((IDisposable)val35)?.Dispose();
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
				TimeWarning val38 = TimeWarning.New("SetCurrentUser", 0);
				try
				{
					TimeWarning val39 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(3900772076u, "SetCurrentUser", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val39)?.Dispose();
					}
					try
					{
						TimeWarning val40 = TimeWarning.New("Call", 0);
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
							((IDisposable)val40)?.Dispose();
						}
					}
					catch (Exception ex13)
					{
						Debug.LogException(ex13);
						player.Kick("RPC Error in SetCurrentUser");
					}
				}
				finally
				{
					((IDisposable)val38)?.Dispose();
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
				TimeWarning val41 = TimeWarning.New("UpdatePhoneName", 0);
				try
				{
					TimeWarning val42 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2760249627u, "UpdatePhoneName", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.FromOwner.Test(2760249627u, "UpdatePhoneName", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val42)?.Dispose();
					}
					try
					{
						TimeWarning val43 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg14 = rPCMessage;
							UpdatePhoneName(msg14);
						}
						finally
						{
							((IDisposable)val43)?.Dispose();
						}
					}
					catch (Exception ex14)
					{
						Debug.LogException(ex14);
						player.Kick("RPC Error in UpdatePhoneName");
					}
				}
				finally
				{
					((IDisposable)val41)?.Dispose();
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
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (info.msg.telephone == null)
		{
			info.msg.telephone = Pool.Get<Telephone>();
		}
		info.msg.telephone.phoneNumber = Controller.PhoneNumber;
		info.msg.telephone.phoneName = Controller.PhoneName;
		info.msg.telephone.lastNumber = Controller.lastDialedNumber;
		info.msg.telephone.savedNumbers = Controller.savedNumbers;
		if (!info.forDisk)
		{
			info.msg.telephone.usingPlayer = Controller.currentPlayerRef.uid;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		Controller.ServerInit();
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

	public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		Controller.OnParentChanged(newParent);
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void ClearCurrentUser(RPCMessage msg)
	{
		Controller.ClearCurrentUser(msg);
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void SetCurrentUser(RPCMessage msg)
	{
		Controller.SetCurrentUser(msg);
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void InitiateCall(RPCMessage msg)
	{
		Controller.InitiateCall(msg);
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void AnswerPhone(RPCMessage msg)
	{
		Controller.AnswerPhone(msg);
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	private void ServerHangUp(RPCMessage msg)
	{
		Controller.ServerHangUp(msg);
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		Controller.DestroyShared();
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(5uL)]
	public void UpdatePhoneName(RPCMessage msg)
	{
		Controller.UpdatePhoneName(msg);
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(5uL)]
	public void Server_RequestPhoneDirectory(RPCMessage msg)
	{
		Controller.Server_RequestPhoneDirectory(msg);
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(5uL)]
	public void Server_AddSavedNumber(RPCMessage msg)
	{
		Controller.Server_AddSavedNumber(msg);
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	[RPC_Server.CallsPerSecond(5uL)]
	public void Server_RemoveSavedNumber(RPCMessage msg)
	{
		Controller.Server_RemoveSavedNumber(msg);
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	public void Server_RequestCurrentState(RPCMessage msg)
	{
		Controller.SetPhoneStateWithPlayer(Controller.serverState);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	public void ServerSendVoicemail(RPCMessage msg)
	{
		Controller.ServerSendVoicemail(msg);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.FromOwner]
	public void ServerPlayVoicemail(RPCMessage msg)
	{
		Controller.ServerPlayVoicemail(msg);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.FromOwner]
	public void ServerStopVoicemail(RPCMessage msg)
	{
		Controller.ServerStopVoicemail(msg);
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.FromOwner]
	public void ServerDeleteVoicemail(RPCMessage msg)
	{
		Controller.ServerDeleteVoicemail(msg);
	}

	public void ToggleRinging(bool state)
	{
		MobileInventoryEntity associatedEntity = ItemModAssociatedEntity<MobileInventoryEntity>.GetAssociatedEntity(GetItem());
		if ((Object)(object)associatedEntity != (Object)null)
		{
			associatedEntity.ToggleRinging(state);
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg?.telephone != null)
		{
			Controller.PhoneNumber = info.msg.telephone.phoneNumber;
			Controller.PhoneName = info.msg.telephone.phoneName;
			Controller.lastDialedNumber = info.msg.telephone.lastNumber;
			Controller.currentPlayerRef.uid = info.msg.telephone.usingPlayer;
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
		}
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(Flags.Busy) != next.HasFlag(Flags.Busy))
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
		Controller.OnFlagsChanged(old, next);
	}
}
