using System;
using System.Collections.Generic;
using ConVar;
using Network;
using UnityEngine;

public static class ConsoleNetwork
{
	internal static void Init()
	{
	}

	internal static void OnClientCommand(Message packet)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (packet.read.Unread > Server.maxpacketsize_command)
		{
			Debug.LogWarning((object)"Dropping client command due to size");
			return;
		}
		string text = packet.read.StringRaw(8388608, false);
		if (packet.connection == null || !packet.connection.connected)
		{
			Debug.LogWarning((object)("Client without connection tried to run command: " + text));
			return;
		}
		Option val = Option.Server;
		val = ((Option)(ref val)).FromConnection(packet.connection);
		string text2 = ConsoleSystem.Run(((Option)(ref val)).Quiet(), text, Array.Empty<object>());
		if (!string.IsNullOrEmpty(text2))
		{
			SendClientReply(packet.connection, text2);
		}
	}

	internal static void SendClientReply(Connection cn, string strCommand)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)11);
			obj.String(strCommand, false);
			obj.Send(new SendInfo(cn));
		}
	}

	public static void SendClientCommand(Connection cn, string strCommand, params object[] args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)12);
			string text = ConsoleSystem.BuildCommand(strCommand, args);
			obj.String(text, false);
			obj.Send(new SendInfo(cn));
		}
	}

	public static void SendClientCommandImmediate(Connection cn, string strCommand, params object[] args)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)12);
			string text = ConsoleSystem.BuildCommand(strCommand, args);
			obj.String(text, false);
			SendInfo val = default(SendInfo);
			((SendInfo)(ref val))._002Ector(cn);
			val.priority = (Priority)0;
			obj.SendImmediate(val);
		}
	}

	public static void SendClientCommand(List<Connection> cn, string strCommand, params object[] args)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)12);
			obj.String(ConsoleSystem.BuildCommand(strCommand, args), false);
			obj.Send(new SendInfo(cn));
		}
	}

	public static void BroadcastToAllClients(string strCommand, params object[] args)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)12);
			obj.String(ConsoleSystem.BuildCommand(strCommand, args), false);
			obj.Send(new SendInfo(Net.sv.connections));
		}
	}
}
