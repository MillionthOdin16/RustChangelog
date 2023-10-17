using System;
using System.Collections.Generic;
using Facepunch.Nexus.Models;

public class NexusEx : Nexus
{
	public static readonly char[] SplitComma = new char[1] { ',' };

	public string Key { get; }

	public HashSet<string> TagsSet { get; }

	public NexusEx(string endpoint, Nexus nexus)
	{
		((Nexus)this).NexusId = nexus.NexusId;
		((Nexus)this).Name = nexus.Name;
		((Nexus)this).LastReset = nexus.LastReset;
		((Nexus)this).ZoneCount = nexus.ZoneCount;
		((Nexus)this).MaxPlayers = nexus.MaxPlayers;
		((Nexus)this).OnlinePlayers = nexus.OnlinePlayers;
		((Nexus)this).QueuedPlayers = nexus.QueuedPlayers;
		((Nexus)this).Build = nexus.Build;
		((Nexus)this).Protocol = nexus.Protocol;
		((Nexus)this).Tags = nexus.Tags;
		Key = $"{endpoint}#{nexus.NexusId}";
		string[] collection = ((Nexus)this).Tags?.Split(SplitComma, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
		TagsSet = new HashSet<string>(collection, StringComparer.OrdinalIgnoreCase);
	}
}
