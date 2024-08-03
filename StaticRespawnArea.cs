using UnityEngine;

public class StaticRespawnArea : SleepingBag
{
	public Transform[] spawnAreas;

	public bool allowHostileSpawns = false;

	public override bool ValidForPlayer(ulong playerID, bool ignoreTimers)
	{
		if (ignoreTimers || allowHostileSpawns)
		{
			return true;
		}
		BasePlayer basePlayer = BasePlayer.FindByID(playerID);
		return basePlayer.GetHostileDuration() <= 0f;
	}

	public override void GetSpawnPos(out Vector3 pos, out Quaternion rot)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Transform val = spawnAreas[Random.Range(0, spawnAreas.Length)];
		pos = ((Component)val).transform.position + spawnOffset;
		Quaternion rotation = ((Component)val).transform.rotation;
		rot = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).eulerAngles.y, 0f);
	}

	public override void SetUnlockTime(float newTime)
	{
		unlockTime = 0f;
	}

	public override float GetUnlockSeconds(ulong playerID)
	{
		BasePlayer basePlayer = BasePlayer.FindByID(playerID);
		if ((Object)(object)basePlayer == (Object)null || allowHostileSpawns)
		{
			return base.unlockSeconds;
		}
		return Mathf.Max(basePlayer.GetHostileDuration(), base.unlockSeconds);
	}
}
