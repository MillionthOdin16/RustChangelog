using Facepunch;
using ProtoBuf;
using UnityEngine;

public static class ClanInfoExtensions
{
	public static ClanInfo ToProto(this IClan clan)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (clan == null)
		{
			return null;
		}
		ClanInfo val = Pool.Get<ClanInfo>();
		val.clanId = clan.ClanId;
		val.name = clan.Name;
		val.created = clan.Created;
		val.creator = clan.Creator;
		val.motd = clan.Motd;
		val.motdTimestamp = clan.MotdTimestamp;
		val.motdAuthor = clan.MotdAuthor;
		val.logo = clan.Logo;
		val.color = ColorEx.ToInt32(clan.Color);
		val.maxMemberCount = clan.MaxMemberCount;
		val.roles = Pool.GetList<Role>();
		foreach (ClanRole role in clan.Roles)
		{
			val.roles.Add(role.ToProto());
		}
		val.members = Pool.GetList<Member>();
		foreach (ClanMember member in clan.Members)
		{
			val.members.Add(member.ToProto());
		}
		val.invites = Pool.GetList<Invite>();
		foreach (ClanInvite invite in clan.Invites)
		{
			val.invites.Add(invite.ToProto());
		}
		return val;
	}

	private static Role ToProto(this ClanRole role)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		bool flag = role.Rank == 1;
		Role obj = Pool.Get<Role>();
		obj.roleId = role.RoleId;
		obj.rank = role.Rank;
		obj.name = role.Name;
		obj.canSetMotd = flag || role.CanSetMotd;
		obj.canSetLogo = flag || role.CanSetLogo;
		obj.canInvite = flag || role.CanInvite;
		obj.canKick = flag || role.CanKick;
		obj.canPromote = flag || role.CanPromote;
		obj.canDemote = flag || role.CanDemote;
		obj.canSetPlayerNotes = flag || role.CanSetPlayerNotes;
		obj.canAccessLogs = flag || role.CanAccessLogs;
		return obj;
	}

	public static ClanRole FromProto(this Role proto)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		ClanRole result = default(ClanRole);
		result.RoleId = proto.roleId;
		result.Rank = proto.rank;
		result.Name = proto.name;
		result.CanSetMotd = proto.canSetMotd;
		result.CanSetLogo = proto.canSetLogo;
		result.CanInvite = proto.canInvite;
		result.CanKick = proto.canKick;
		result.CanPromote = proto.canPromote;
		result.CanDemote = proto.canDemote;
		result.CanSetPlayerNotes = proto.canSetPlayerNotes;
		result.CanAccessLogs = proto.canAccessLogs;
		return result;
	}

	private static Member ToProto(this ClanMember member)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Member obj = Pool.Get<Member>();
		obj.steamId = member.SteamId;
		obj.roleId = member.RoleId;
		obj.joined = member.Joined;
		obj.lastSeen = member.LastSeen;
		obj.notes = member.Notes;
		obj.online = (NexusServer.Started ? NexusServer.IsOnline(member.SteamId) : ServerPlayers.IsOnline(member.SteamId));
		return obj;
	}

	private static Invite ToProto(this ClanInvite invite)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Invite obj = Pool.Get<Invite>();
		obj.steamId = invite.SteamId;
		obj.recruiter = invite.Recruiter;
		obj.timestamp = invite.Timestamp;
		return obj;
	}
}
