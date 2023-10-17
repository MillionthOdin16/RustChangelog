using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class ClanManager : BaseEntity
{
	public static readonly TokenisedPhrase InvitationToast = new TokenisedPhrase("clan.invitation.toast", "You were invited to {clanName}! Press [clan.toggleclan] to manage your clan invitations.");

	public const int LogoSize = 512;

	private string _backendType;

	private ClanChangeTracker _changeTracker;

	private const int MaxMetadataRequestsPerSecond = 3;

	private const float MaxMetadataRequestInterval = 0.5f;

	private const float MetadataExpiry = 300f;

	private readonly Dictionary<long, List<Connection>> _clanMemberConnections = new Dictionary<long, List<Connection>>();

	public static ClanManager ServerInstance { get; private set; }

	public IClanBackend Backend { get; private set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ClanManager.OnRpcMessage", 0);
		try
		{
			if (rpc == 3593616087u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_AcceptInvitation "));
				}
				TimeWarning val2 = TimeWarning.New("Server_AcceptInvitation", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3593616087u, "Server_AcceptInvitation", this, player, 3uL))
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
							Server_AcceptInvitation(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_AcceptInvitation");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 73135447 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_CancelInvitation "));
				}
				TimeWarning val2 = TimeWarning.New("Server_CancelInvitation", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(73135447u, "Server_CancelInvitation", this, player, 3uL))
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
							Server_CancelInvitation(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_CancelInvitation");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 785874715 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_CancelInvite "));
				}
				TimeWarning val2 = TimeWarning.New("Server_CancelInvite", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(785874715u, "Server_CancelInvite", this, player, 3uL))
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
							Server_CancelInvite(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in Server_CancelInvite");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4017901233u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_CreateClan "));
				}
				TimeWarning val2 = TimeWarning.New("Server_CreateClan", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(4017901233u, "Server_CreateClan", this, player, 3uL))
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
							Server_CreateClan(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in Server_CreateClan");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 835697933 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_CreateRole "));
				}
				TimeWarning val2 = TimeWarning.New("Server_CreateRole", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(835697933u, "Server_CreateRole", this, player, 3uL))
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
							RPCMessage msg6 = rPCMessage;
							Server_CreateRole(msg6);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in Server_CreateRole");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3966624879u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_DeleteRole "));
				}
				TimeWarning val2 = TimeWarning.New("Server_DeleteRole", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3966624879u, "Server_DeleteRole", this, player, 3uL))
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
							RPCMessage msg7 = rPCMessage;
							Server_DeleteRole(msg7);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in Server_DeleteRole");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4071826018u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_GetClan "));
				}
				TimeWarning val2 = TimeWarning.New("Server_GetClan", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(4071826018u, "Server_GetClan", this, player, 3uL))
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
							RPCMessage msg8 = rPCMessage;
							Server_GetClan(msg8);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in Server_GetClan");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2338234158u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_GetClanMetadata "));
				}
				TimeWarning val2 = TimeWarning.New("Server_GetClanMetadata", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2338234158u, "Server_GetClanMetadata", this, player, 3uL))
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
							RPCMessage msg9 = rPCMessage;
							Server_GetClanMetadata(msg9);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in Server_GetClanMetadata");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 507204008 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_GetInvitations "));
				}
				TimeWarning val2 = TimeWarning.New("Server_GetInvitations", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(507204008u, "Server_GetInvitations", this, player, 3uL))
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
							RPCMessage msg10 = rPCMessage;
							Server_GetInvitations(msg10);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex9)
					{
						Debug.LogException(ex9);
						player.Kick("RPC Error in Server_GetInvitations");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3858074978u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_GetLogs "));
				}
				TimeWarning val2 = TimeWarning.New("Server_GetLogs", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3858074978u, "Server_GetLogs", this, player, 3uL))
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
							RPCMessage msg11 = rPCMessage;
							Server_GetLogs(msg11);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex10)
					{
						Debug.LogException(ex10);
						player.Kick("RPC Error in Server_GetLogs");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1782867876 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_Invite "));
				}
				TimeWarning val2 = TimeWarning.New("Server_Invite", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1782867876u, "Server_Invite", this, player, 3uL))
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
							RPCMessage msg12 = rPCMessage;
							Server_Invite(msg12);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex11)
					{
						Debug.LogException(ex11);
						player.Kick("RPC Error in Server_Invite");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3093528332u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_Kick "));
				}
				TimeWarning val2 = TimeWarning.New("Server_Kick", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3093528332u, "Server_Kick", this, player, 3uL))
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
							RPCMessage msg13 = rPCMessage;
							Server_Kick(msg13);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex12)
					{
						Debug.LogException(ex12);
						player.Kick("RPC Error in Server_Kick");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2235419116u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_SetColor "));
				}
				TimeWarning val2 = TimeWarning.New("Server_SetColor", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2235419116u, "Server_SetColor", this, player, 3uL))
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
							RPCMessage msg14 = rPCMessage;
							Server_SetColor(msg14);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex13)
					{
						Debug.LogException(ex13);
						player.Kick("RPC Error in Server_SetColor");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1189444132 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_SetLogo "));
				}
				TimeWarning val2 = TimeWarning.New("Server_SetLogo", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1189444132u, "Server_SetLogo", this, player, 3uL))
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
							RPCMessage msg15 = rPCMessage;
							Server_SetLogo(msg15);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex14)
					{
						Debug.LogException(ex14);
						player.Kick("RPC Error in Server_SetLogo");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4088477037u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_SetMotd "));
				}
				TimeWarning val2 = TimeWarning.New("Server_SetMotd", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(4088477037u, "Server_SetMotd", this, player, 3uL))
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
							RPCMessage msg16 = rPCMessage;
							Server_SetMotd(msg16);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex15)
					{
						Debug.LogException(ex15);
						player.Kick("RPC Error in Server_SetMotd");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 285489852 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_SetPlayerNotes "));
				}
				TimeWarning val2 = TimeWarning.New("Server_SetPlayerNotes", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(285489852u, "Server_SetPlayerNotes", this, player, 3uL))
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
							RPCMessage msg17 = rPCMessage;
							Server_SetPlayerNotes(msg17);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex16)
					{
						Debug.LogException(ex16);
						player.Kick("RPC Error in Server_SetPlayerNotes");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3232449870u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_SetPlayerRole "));
				}
				TimeWarning val2 = TimeWarning.New("Server_SetPlayerRole", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3232449870u, "Server_SetPlayerRole", this, player, 3uL))
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
							RPCMessage msg18 = rPCMessage;
							Server_SetPlayerRole(msg18);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex17)
					{
						Debug.LogException(ex17);
						player.Kick("RPC Error in Server_SetPlayerRole");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 738181899 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_SwapRoles "));
				}
				TimeWarning val2 = TimeWarning.New("Server_SwapRoles", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(738181899u, "Server_SwapRoles", this, player, 3uL))
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
							RPCMessage msg19 = rPCMessage;
							Server_SwapRoles(msg19);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex18)
					{
						Debug.LogException(ex18);
						player.Kick("RPC Error in Server_SwapRoles");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1548667516 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_UpdateRole "));
				}
				TimeWarning val2 = TimeWarning.New("Server_UpdateRole", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1548667516u, "Server_UpdateRole", this, player, 3uL))
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
							RPCMessage msg20 = rPCMessage;
							Server_UpdateRole(msg20);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex19)
					{
						Debug.LogException(ex19);
						player.Kick("RPC Error in Server_UpdateRole");
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

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_CreateClan(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		string text = default(string);
		if (!ClanValidator.ValidateClanName(msg.read.String(256, false), ref text))
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)6, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Create(msg.player.userID, text);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)1, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_GetClan(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)1, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_GetLogs(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanValueResult<ClanLogs> val = await clan.GetLogs(100, msg.player.userID);
			if (val.IsSuccess)
			{
				ClientRPCPlayer<ClanLog>(null, msg.player, "Client_ReceiveClanLogs", val.Value.ToProto());
			}
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, val.Result, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_GetInvitations(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		ClanValueResult<List<ClanInvitation>> val = await Backend.ListInvitations(msg.player.userID);
		if (val.IsSuccess)
		{
			ClientRPCPlayer<ClanInvitations>(null, msg.player, "Client_ReceiveClanInvitations", val.Value.ToProto());
		}
		ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, val.Result, null, hasClanInfo: false));
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_SetLogo(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		byte[] newLogo = msg.read.BytesWithSize(10485760u, false);
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		if (!ImageProcessing.IsValidPNG(newLogo, 512, 512))
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)7, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.SetLogo(newLogo, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_SetColor(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		Color32 newColor = msg.read.Color32();
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		if (newColor.a != byte.MaxValue)
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)8, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.SetColor(newColor, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_SetMotd(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		string text = msg.read.StringMultiLine(2048, false);
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		string validatedMotd = default(string);
		if (!ClanValidator.ValidateMotd(text, ref validatedMotd))
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)6, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			long previousTimestamp = clan.MotdTimestamp;
			ClanResult val = await clan.SetMotd(validatedMotd, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, val, clan));
			if ((int)val == 1)
			{
				ClanPushNotifications.SendClanAnnouncement(clan, previousTimestamp, msg.player.userID);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_Invite(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		ulong steamId = msg.read.UInt64();
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.Invite(steamId, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_CancelInvite(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		ulong steamId = msg.read.UInt64();
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.CancelInvite(steamId, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_AcceptInvitation(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		long num = msg.read.Int64();
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(num);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.AcceptInvite(msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, null));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_CancelInvitation(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		long num = msg.read.Int64();
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(num);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.CancelInvite(msg.player.userID, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, null));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_Kick(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		ulong steamId = msg.read.UInt64();
		if (msg.player.userID != steamId && !msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.Kick(steamId, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_SetPlayerRole(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		ulong steamId = msg.read.UInt64();
		int newRoleId = msg.read.Int32();
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.SetPlayerRole(steamId, newRoleId, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_SetPlayerNotes(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		ulong steamId = msg.read.UInt64();
		string text = msg.read.StringMultiLine(1024, false);
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		string validatedNotes = default(string);
		if (!ClanValidator.ValidatePlayerNote(text, ref validatedNotes))
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)6, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.SetPlayerNotes(steamId, validatedNotes, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_CreateRole(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		string text = msg.read.String(128, false);
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		string name = default(string);
		if (!ClanValidator.ValidateRoleName(text, ref name))
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)6, null, hasClanInfo: false));
			return;
		}
		ClanRole role = new ClanRole
		{
			Name = name
		};
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.CreateRole(role, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_UpdateRole(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		Role role = Role.Deserialize((Stream)(object)msg.read);
		try
		{
			string name = default(string);
			if (!ClanValidator.ValidateRoleName(role.name, ref name))
			{
				ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)6, null, hasClanInfo: false));
				return;
			}
			role.name = name;
			ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
			if (!CheckClanResult(requestId, msg.player, result, out var clan))
			{
				return;
			}
			ClanResult result2 = await clan.UpdateRole(role.FromProto(), msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, clan));
		}
		finally
		{
			((IDisposable)role)?.Dispose();
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_DeleteRole(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		int roleId = msg.read.Int32();
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.DeleteRole(roleId, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, clan));
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_SwapRoles(RPCMessage msg)
	{
		int requestId = msg.read.Int32();
		int roleIdA = msg.read.Int32();
		int roleIdB = msg.read.Int32();
		if (!msg.player.CanModifyClan())
		{
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, (ClanResult)16, null, hasClanInfo: false));
			return;
		}
		ClanValueResult<IClan> result = await Backend.Get(msg.player.clanId);
		if (CheckClanResult(requestId, msg.player, result, out var clan))
		{
			ClanResult result2 = await clan.SwapRoleRanks(roleIdA, roleIdB, msg.player.userID);
			ClientRPCPlayer<ClanActionResult>(null, msg.player, "Client_ReceiveActionResult", BuildActionResult(requestId, result2, clan));
		}
	}

	private bool CheckClanResult(int requestId, BasePlayer player, ClanValueResult<IClan> result, out IClan clan)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (result.IsSuccess)
		{
			clan = result.Value;
			return true;
		}
		ClientRPCPlayer<ClanActionResult>(null, player, "Client_ReceiveActionResult", BuildActionResult(requestId, result.Result, null));
		clan = null;
		return false;
	}

	private static ClanActionResult BuildActionResult(int requestId, ClanResult result, IClan clan, bool hasClanInfo = true)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected I4, but got Unknown
		ClanActionResult obj = Pool.Get<ClanActionResult>();
		obj.requestId = requestId;
		obj.result = (int)result;
		obj.hasClanInfo = hasClanInfo;
		obj.clanInfo = clan.ToProto();
		return obj;
	}

	public async Task Initialize()
	{
		if (string.IsNullOrWhiteSpace(_backendType))
		{
			throw new InvalidOperationException("Clan backend type has not been assigned!");
		}
		IClanBackend backend = CreateBackendInstance(_backendType);
		if (backend == null)
		{
			throw new InvalidOperationException("Clan backend failed to create (returned null)");
		}
		try
		{
			_changeTracker = new ClanChangeTracker(this);
			await backend.Initialize((IClanChangeSink)(object)_changeTracker);
			Backend = backend;
			((FacepunchBehaviour)this).InvokeRandomized((Action)delegate
			{
				_changeTracker.HandleEvents();
			}, 1f, 0.25f, 0.1f);
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException("Clan backend failed to initialize (threw exception)", innerException);
		}
	}

	public void Shutdown()
	{
		if (Backend == null)
		{
			return;
		}
		try
		{
			((IDisposable)Backend).Dispose();
			Backend = null;
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException("Clan backend failed to shutdown (threw exception)", innerException);
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		if (!base.isServer)
		{
			return;
		}
		if (Application.isLoadingSave)
		{
			if (!Clan.enabled)
			{
				Debug.LogWarning((object)"Clan manager was loaded from a save, but the server has the clan system disabled - destroying clan manager!");
				((FacepunchBehaviour)this).Invoke((Action)delegate
				{
					Kill();
				}, 0.1f);
			}
		}
		else if (!Application.isLoadingSave)
		{
			_backendType = ChooseBackendType();
			if (string.IsNullOrWhiteSpace(_backendType))
			{
				Debug.LogError((object)"Clan manager did not choose a backend type!");
			}
			else
			{
				Debug.Log((object)("Clan manager will use backend type: " + _backendType));
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.clanManager = Pool.Get<ClanManager>();
			info.msg.clanManager.backendType = _backendType;
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.clanManager != null)
		{
			_backendType = info.msg.clanManager.backendType;
		}
	}

	private static string ChooseBackendType()
	{
		if (NexusServer.Started)
		{
			return "nexus";
		}
		return "local";
	}

	private static IClanBackend CreateBackendInstance(string type)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		if (!(type == "local"))
		{
			if (type == "nexus")
			{
				return (IClanBackend)(object)new NexusClanBackend();
			}
			throw new NotSupportedException("Clan backend '" + type + "' is not supported");
		}
		return (IClanBackend)new LocalClanBackend(ConVar.Server.rootFolder, Clan.maxMemberCount);
	}

	public override void InitShared()
	{
		base.InitShared();
		if (base.isServer)
		{
			if ((Object)(object)ServerInstance != (Object)null)
			{
				Debug.LogError((object)"Major fuckup! Server ClanManager spawned twice, contact Developers!");
				Object.Destroy((Object)(object)((Component)this).gameObject);
			}
			else
			{
				ServerInstance = this;
			}
		}
	}

	public void OnDestroy()
	{
		if (base.isServer)
		{
			if ((Object)(object)ServerInstance == (Object)(object)this)
			{
				ServerInstance = null;
			}
			Shutdown();
		}
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(3uL)]
	public async void Server_GetClanMetadata(RPCMessage msg)
	{
		long clanId = msg.read.Int64();
		ClanValueResult<IClan> val = await Backend.Get(clanId);
		if (val.IsSuccess)
		{
			IClan value = val.Value;
			ClientRPCPlayer<long, string, int, Color32>(null, msg.player, "Client_GetClanMetadataResponse", clanId, value.Name ?? "", value.Members?.Count ?? 0, value.Color);
		}
		else
		{
			ClientRPCPlayer<long, string, int, Color32>(null, msg.player, "Client_GetClanMetadataResponse", clanId, "[unknown]", 0, Color32.op_Implicit(Color.white));
		}
	}

	public void SendClanChanged(IClan clan)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		List<Connection> list = Pool.GetList<Connection>();
		foreach (ClanMember member in clan.Members)
		{
			BasePlayer basePlayer = BasePlayer.FindByID(member.SteamId);
			if ((Object)(object)basePlayer != (Object)null && basePlayer.IsConnected)
			{
				list.Add(basePlayer.net.connection);
			}
		}
		ClientRPCEx(new SendInfo(list), null, "Client_CurrentClanChanged");
		Pool.FreeList<Connection>(ref list);
	}

	public void SendClanInvitation(ulong steamId, long clanId)
	{
		BasePlayer basePlayer = BasePlayer.FindByID(steamId);
		if (!((Object)(object)basePlayer == (Object)null) && basePlayer.IsConnected)
		{
			ClientRPCPlayer(null, basePlayer, "Client_ReceiveClanInvitation", clanId);
		}
	}

	public bool TryGetClanMemberConnections(long clanId, out List<Connection> connections)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (_clanMemberConnections.TryGetValue(clanId, out connections))
		{
			return true;
		}
		IClan val = default(IClan);
		if (!Backend.TryGet(clanId, ref val))
		{
			return false;
		}
		connections = Pool.GetList<Connection>();
		foreach (ClanMember member in val.Members)
		{
			BasePlayer basePlayer = BasePlayer.FindByID(member.SteamId);
			if ((Object)(object)basePlayer == (Object)null)
			{
				basePlayer = BasePlayer.FindSleeping(member.SteamId);
			}
			if (!((Object)(object)basePlayer == (Object)null) && basePlayer.IsConnected)
			{
				connections.Add(basePlayer.Connection);
			}
		}
		_clanMemberConnections.Add(clanId, connections);
		return true;
	}

	public void ClanMemberConnectionsChanged(long clanId)
	{
		if (_clanMemberConnections.TryGetValue(clanId, out var value))
		{
			_clanMemberConnections.Remove(clanId);
			Pool.FreeList<Connection>(ref value);
		}
	}

	public async void LoadClanInfoForSleepers()
	{
		Dictionary<ulong, BasePlayer> sleepers = Pool.Get<Dictionary<ulong, BasePlayer>>();
		sleepers.Clear();
		Enumerator<BasePlayer> enumerator = BasePlayer.sleepingPlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				if (current.IsValid() && !current.IsNpc && !current.IsBot)
				{
					sleepers.Add(current.userID, current);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		HashSet<ulong> found = Pool.Get<HashSet<ulong>>();
		found.Clear();
		foreach (BasePlayer player in sleepers.Values)
		{
			if (!player.IsValid() || player.IsConnected || found.Contains(player.userID))
			{
				continue;
			}
			try
			{
				ClanValueResult<IClan> val = await Backend.GetByMember(player.userID);
				if (val.IsSuccess)
				{
					IClan value = val.Value;
					player.clanId = value.ClanId;
					SendNetworkUpdate();
					found.Add(player.userID);
					foreach (ClanMember member in value.Members)
					{
						if (sleepers.TryGetValue(member.SteamId, out var value2) && found.Add(member.SteamId))
						{
							value2.clanId = value.ClanId;
							value2.SendNetworkUpdate();
						}
					}
				}
				else if ((int)val.Result == 3)
				{
					player.clanId = 0L;
					SendNetworkUpdate();
					found.Add(player.userID);
				}
				else
				{
					Debug.LogError((object)$"Failed to find clan for {player.userID}: {val.Result}");
					((FacepunchBehaviour)this).Invoke((Action)delegate
					{
						player.LoadClanInfo();
					}, (float)(45 + Random.Range(0, 30)));
				}
			}
			catch (Exception ex)
			{
				DebugEx.Log((object)$"Exception was thrown while loading clan info for {player.userID}:", (StackTraceLogType)0);
				Debug.LogException(ex);
			}
		}
		found.Clear();
		Pool.Free<HashSet<ulong>>(ref found);
		sleepers.Clear();
		Pool.Free<Dictionary<ulong, BasePlayer>>(ref sleepers);
	}
}
