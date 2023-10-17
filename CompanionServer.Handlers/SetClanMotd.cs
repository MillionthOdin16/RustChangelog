using ProtoBuf;

namespace CompanionServer.Handlers;

public class SetClanMotd : BaseClanHandler<AppSendMessage>
{
	public override async void Execute()
	{
		string motd = default(string);
		if (!ClanValidator.ValidateMotd(base.Proto.message, ref motd))
		{
			((BaseHandler<AppSendMessage>)this).SendError("invalid_motd");
			return;
		}
		IClan clan = await GetClan();
		if (clan == null)
		{
			((BaseHandler<AppSendMessage>)this).SendError("no_clan");
			return;
		}
		ClanResult val = await clan.SetMotd(motd, base.UserId);
		if ((int)val == 1)
		{
			SendSuccess();
			ClanPushNotifications.SendClanAnnouncement(clan, base.UserId);
		}
		else
		{
			SendError(val);
		}
	}
}
