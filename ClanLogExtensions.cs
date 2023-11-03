using System.Collections.Generic;
using Facepunch;
using ProtoBuf;

public static class ClanLogExtensions
{
	public static ClanLog ToProto(this ClanLogs clanLogs)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		List<Entry> list = Pool.GetList<Entry>();
		foreach (ClanLogEntry entry in clanLogs.Entries)
		{
			Entry val = Pool.Get<Entry>();
			val.timestamp = entry.Timestamp;
			val.eventKey = entry.EventKey;
			val.arg1 = entry.Arg1;
			val.arg2 = entry.Arg2;
			val.arg3 = entry.Arg3;
			val.arg4 = entry.Arg4;
			list.Add(val);
		}
		ClanLog obj = Pool.Get<ClanLog>();
		obj.clanId = clanLogs.ClanId;
		obj.logEntries = list;
		return obj;
	}
}
