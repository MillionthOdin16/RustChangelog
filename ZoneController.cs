using System;
using Facepunch.Nexus;
using UnityEngine;

public abstract class ZoneController
{
	protected readonly NexusZoneClient ZoneClient;

	public static ZoneController Instance { get; set; }

	protected ZoneController(NexusZoneClient zoneClient)
	{
		ZoneClient = zoneClient ?? throw new ArgumentNullException("zoneClient");
	}

	public abstract string ChooseSpawnZone(ulong steamId, bool isAlreadyAssignedToThisZone);

	public virtual (Vector3 Position, Quaternion Rotation, bool PreserveY) ChooseTransferDestination(string sourceZone, string method, string from, string to, Vector3 position, Quaternion rotation)
	{
		switch (method)
		{
		case "console":
			return ChooseConsoleTransferDestination(sourceZone);
		case "ferry":
			return ChooseFerryTransferDestination(sourceZone);
		case "ocean":
			return ChooseOceanTransferDestination(sourceZone);
		default:
			Debug.LogError((object)("Unhandled transfer method '" + method + "', using default destination"));
			return ChooseTransferFallbackDestination(sourceZone);
		}
	}

	protected virtual (Vector3, Quaternion, bool) ChooseConsoleTransferDestination(string sourceZone)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer.SpawnPoint spawnPoint = ServerMgr.FindSpawnPoint();
		return (spawnPoint.pos, spawnPoint.rot, false);
	}

	protected virtual (Vector3, Quaternion, bool) ChooseFerryTransferDestination(string sourceZone)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (!NexusServer.TryGetIsland(sourceZone, out var island))
		{
			return ChooseTransferFallbackDestination(sourceZone);
		}
		if (!island.TryFindPosition(out var position))
		{
			Debug.LogWarning((object)("Couldn't find a destination position for source zone '" + sourceZone + "'"));
			return ChooseTransferFallbackDestination(sourceZone);
		}
		return (position, ((Component)island).transform.rotation, true);
	}

	protected virtual (Vector3, Quaternion, bool) ChooseOceanTransferDestination(string sourceZone)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!NexusServer.TryGetIsland(sourceZone, out var island))
		{
			Debug.LogWarning((object)("Couldn't find nexus island for source zone '" + sourceZone + "'"));
			return ChooseTransferFallbackDestination(sourceZone);
		}
		if (!island.TryFindPosition(out var position))
		{
			Debug.LogWarning((object)("Couldn't find a destination position for source zone '" + sourceZone + "'"));
			return ChooseTransferFallbackDestination(sourceZone);
		}
		return (position, ((Component)island).transform.rotation, true);
	}

	protected virtual (Vector3, Quaternion, bool) ChooseTransferFallbackDestination(string sourceZone)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Bounds worldBounds = NexusServer.GetWorldBounds();
		float num = Mathf.Max(((Bounds)(ref worldBounds)).extents.x, ((Bounds)(ref worldBounds)).extents.z);
		Vector3 position;
		Vector3 val = (NexusServer.TryGetIslandPosition(sourceZone, out position) ? (position + new Vector3((float)Random.Range(-1, 1), 0f, (float)Random.Range(-1, 1)) * 100f) : (Vector3Ex.XZ3D(Random.insideUnitCircle) * num * 0.75f));
		Vector3 val2 = Vector3Ex.WithY(val, WaterSystem.GetHeight(val));
		Vector3 val3 = Vector3Ex.WithY(TerrainMeta.Center, val2.y) - val2;
		Quaternion item = Quaternion.LookRotation(((Vector3)(ref val3)).normalized, Vector3.up);
		return (val2, item, true);
	}

	public virtual bool CanRespawnAcrossZones(BasePlayer player)
	{
		return true;
	}
}
