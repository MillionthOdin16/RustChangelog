using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers;

public class ClanInfo : BaseClanHandler<AppEmpty>
{
	public override async void Execute()
	{
		IClan val = await GetClan();
		if (val == null)
		{
			((BaseHandler<AppEmpty>)this).SendError("no_clan");
			return;
		}
		AppClanInfo val2 = Pool.Get<AppClanInfo>();
		val2.clanInfo = val.ToProto();
		AppResponse val3 = Pool.Get<AppResponse>();
		val3.clanInfo = val2;
		Send(val3);
	}
}
