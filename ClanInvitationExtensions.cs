using System.Collections.Generic;
using Facepunch;
using ProtoBuf;

public static class ClanInvitationExtensions
{
	public static ClanInvitations ToProto(this List<ClanInvitation> invitations)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		List<Invitation> list = Pool.GetList<Invitation>();
		foreach (ClanInvitation invitation in invitations)
		{
			list.Add(invitation.ToProto());
		}
		ClanInvitations obj = Pool.Get<ClanInvitations>();
		obj.invitations = list;
		return obj;
	}

	public static Invitation ToProto(this ClanInvitation invitation)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Invitation obj = Pool.Get<Invitation>();
		obj.clanId = invitation.ClanId;
		obj.recruiter = invitation.Recruiter;
		obj.timestamp = invitation.Timestamp;
		return obj;
	}
}
