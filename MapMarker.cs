using System.Collections.Generic;
using CompanionServer;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class MapMarker : BaseEntity
{
	public enum ClusterType
	{
		None,
		Vending
	}

	public AppMarkerType appType;

	public GameObjectRef markerObj;

	public static readonly List<MapMarker> serverMapMarkers = new List<MapMarker>();

	public override void InitShared()
	{
		if (base.isServer && !serverMapMarkers.Contains(this))
		{
			serverMapMarkers.Add(this);
		}
		base.InitShared();
	}

	public override void DestroyShared()
	{
		if (base.isServer)
		{
			serverMapMarkers.Remove(this);
		}
		base.DestroyShared();
	}

	public virtual AppMarker GetAppMarkerData()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		AppMarker val = Pool.Get<AppMarker>();
		Vector2 val2 = CompanionServer.Util.WorldToMap(((Component)this).transform.position);
		val.id = net.ID;
		val.type = appType;
		val.x = val2.x;
		val.y = val2.y;
		return val;
	}
}
