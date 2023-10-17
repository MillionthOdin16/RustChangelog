using ConVar;
using Facepunch.Extend;
using ProtoBuf;

namespace CompanionServer.Handlers;

public class SendClanChat : BaseClanHandler<AppSendMessage>
{
	protected override double TokenCost => 2.0;

	public override async void Execute()
	{
		if (await GetClan() == null)
		{
			((BaseHandler<AppSendMessage>)this).SendError("no_clan");
			return;
		}
		string text = base.Proto.message?.Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			SendSuccess();
			return;
		}
		text = StringExtensions.Truncate(text, 256, "…");
		string username = base.Player?.displayName ?? SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(base.UserId) ?? "[unknown]";
		if (await Chat.sayAs(Chat.ChatChannel.Clan, base.UserId, username, text, base.Player))
		{
			SendSuccess();
		}
		else
		{
			((BaseHandler<AppSendMessage>)this).SendError("message_not_sent");
		}
	}
}
