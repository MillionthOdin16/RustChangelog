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
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite val = ((BaseNetwork)Net.sv).StartWrite();
			val.PacketID((Type)11);
			val.String(strCommand);
			val.Send(new SendInfo(cn));
		}
	}

	public static void SendClientCommand(Connection cn, string strCommand, params object[] args)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite val = ((BaseNetwork)Net.sv).StartWrite();
			val.PacketID((Type)12);
			string text = ConsoleSystem.BuildCommand(strCommand, args);
			val.String(text);
			val.Send(new SendInfo(cn));
		}
	}

	public static void SendClientCommand(List<Connection> cn, string strCommand, params object[] args)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite val = ((BaseNetwork)Net.sv).StartWrite();
			val.PacketID((Type)12);
			val.String(ConsoleSystem.BuildCommand(strCommand, args));
			val.Send(new SendInfo(cn));
		}
	}

	public static void BroadcastToAllClients(string strCommand, params object[] args)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite val = ((BaseNetwork)Net.sv).StartWrite();
			val.PacketID((Type)12);
			val.String(ConsoleSystem.BuildCommand(strCommand, args));
			val.Send(new SendInfo(Net.sv.connections));
		}
	}
}
