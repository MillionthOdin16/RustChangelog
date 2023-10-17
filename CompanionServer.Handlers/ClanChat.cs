using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers;

public class ClanChat : BaseClanHandler<AppEmpty>
{
	public override async void Execute()
	{
		IClan val = await GetClan();
		if (val == null)
		{
			((BaseHandler<AppEmpty>)this).SendError("no_clan");
			return;
		}
		ClanValueResult<ClanChatScrollback> val2 = await val.GetChatScrollback();
		if (!val2.IsSuccess)
		{
			SendError(val2.Result);
			return;
		}
		AppResponse val3 = Pool.Get<AppResponse>();
		val3.clanChat = Pool.Get<AppClanChat>();
		val3.clanChat.messages = Pool.GetList<AppClanMessage>();
		foreach (ClanChatEntry entry in val2.Value.Entries)
		{
			AppClanMessage val4 = Pool.Get<AppClanMessage>();
			val4.steamId = entry.SteamId;
			val4.name = entry.Name;
			val4.message = entry.Message;
			val4.time = entry.Time;
			val3.clanChat.messages.Add(val4);
		}
		Send(val3);
	}
}
