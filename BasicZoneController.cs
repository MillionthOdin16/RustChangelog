using System.Collections.Generic;
using Facepunch;
using Facepunch.Nexus;
using Facepunch.Nexus.Models;
using UnityEngine;

public class BasicZoneController : ZoneController
{
	public BasicZoneController(NexusZoneClient zoneClient)
		: base(zoneClient)
	{
	}

	public override string ChooseSpawnZone(ulong steamId, bool isAlreadyAssignedToThisZone)
	{
		if (ZoneClient.Zone.IsStarterZone())
		{
			return ZoneClient.Zone.Key;
		}
		string key = ZoneClient.Zone.Key;
		List<NexusZoneDetails> list = Pool.GetList<NexusZoneDetails>();
		GetStarterZones(list);
		if (list.Count > 0)
		{
			int index = Random.Range(0, list.Count);
			key = list[index].Key;
		}
		Pool.FreeList<NexusZoneDetails>(ref list);
		return key;
	}

	private void GetStarterZones(List<NexusZoneDetails> zones)
	{
		NexusZoneClient zoneClient = ZoneClient;
		object obj;
		if (zoneClient == null)
		{
			obj = null;
		}
		else
		{
			NexusDetails nexus = zoneClient.Nexus;
			obj = ((nexus != null) ? nexus.Zones : null);
		}
		if (obj == null)
		{
			return;
		}
		foreach (NexusZoneDetails zone in ZoneClient.Nexus.Zones)
		{
			if (zone.IsStarterZone())
			{
				zones.Add(zone);
			}
		}
	}
}
