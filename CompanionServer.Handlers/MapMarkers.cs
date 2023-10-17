using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer.Handlers;

public class MapMarkers : BaseHandler<AppEmpty>
{
	public override void Execute()
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Invalid comparison between Unknown and I4
		AppMapMarkers val = Pool.Get<AppMapMarkers>();
		val.markers = Pool.GetList<AppMarker>();
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(base.UserId);
		if (playerTeam != null)
		{
			foreach (ulong member in playerTeam.members)
			{
				BasePlayer basePlayer = RelationshipManager.FindByID(member);
				if (!((Object)(object)basePlayer == (Object)null))
				{
					val.markers.Add(GetPlayerMarker(basePlayer));
				}
			}
		}
		else if ((Object)(object)base.Player != (Object)null)
		{
			val.markers.Add(GetPlayerMarker(base.Player));
		}
		foreach (MapMarker serverMapMarker in MapMarker.serverMapMarkers)
		{
			if ((int)serverMapMarker.appType != 0)
			{
				val.markers.Add(serverMapMarker.GetAppMarkerData());
			}
		}
		AppResponse val2 = Pool.Get<AppResponse>();
		val2.mapMarkers = val;
		Send(val2);
	}

	private static AppMarker GetPlayerMarker(BasePlayer player)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		AppMarker val = Pool.Get<AppMarker>();
		Vector2 val2 = Util.WorldToMap(((Component)player).transform.position);
		val.id = player.net.ID;
		val.type = (AppMarkerType)1;
		val.x = val2.x;
		val.y = val2.y;
		val.steamId = player.userID;
		return val;
	}
}
