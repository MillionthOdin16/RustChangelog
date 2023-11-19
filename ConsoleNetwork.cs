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
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (packet.read.Unread > Server.maxpacketsize_command)
		{
			Debug.LogWarning((object)"Dropping client command due to size");
			return;
		}
		string text = packet.read.StringRaw(8388608u);
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
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)11);
			obj.String(strCommand);
			obj.Send(new SendInfo(cn));
		}
	}

	public static void SendClientCommand(Connection cn, string strCommand, params object[] args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)12);
			string text = ConsoleSystem.BuildCommand(strCommand, args);
			obj.String(text);
			obj.Send(new SendInfo(cn));
		}
	}

	public static void SendClientCommandImmediate(Connection cn, string strCommand, params object[] args)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)12);
			string text = ConsoleSystem.BuildCommand(strCommand, args);
			obj.String(text);
			SendInfo val = default(SendInfo);
			((SendInfo)(ref val))._002Ector(cn);
			val.priority = (Priority)0;
			obj.SendImmediate(val);
		}
	}

	public static void SendClientCommand(List<Connection> cn, string strCommand, params object[] args)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)12);
			obj.String(ConsoleSystem.BuildCommand(strCommand, args));
			obj.Send(new SendInfo(cn));
		}
	}

	public static void BroadcastToAllClients(string strCommand, params object[] args)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)12);
			obj.String(ConsoleSystem.BuildCommand(strCommand, args));
			obj.Send(new SendInfo(Net.sv.connections));
		}
	}
}
