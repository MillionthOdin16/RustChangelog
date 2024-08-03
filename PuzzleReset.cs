using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

public class PuzzleReset : FacepunchBehaviour
{
	public SpawnGroup[] respawnGroups;

	public IOEntity[] resetEnts;

	public GameObject[] resetObjects;

	public bool playersBlockReset = false;

	public bool CheckSleepingAIZForPlayers = false;

	public float playerDetectionRadius;

	public float playerHeightDetectionMinMax = -1f;

	public Transform playerDetectionOrigin;

	public float timeBetweenResets = 30f;

	public bool scaleWithServerPopulation = false;

	[HideInInspector]
	public Vector3[] resetPositions;

	public bool broadcastResetMessage = false;

	public Phrase resetPhrase;

	private AIInformationZone zone = null;

	private float resetTimeElapsed = 0f;

	private float resetTickTime = 10f;

	public float GetResetSpacing()
	{
		return timeBetweenResets * (scaleWithServerPopulation ? (1f - SpawnHandler.PlayerLerp(Spawn.min_rate, Spawn.max_rate)) : 1f);
	}

	public void Start()
	{
		if (timeBetweenResets != float.PositiveInfinity)
		{
			ResetTimer();
		}
	}

	public void ResetTimer()
	{
		resetTimeElapsed = 0f;
		((FacepunchBehaviour)this).CancelInvoke((Action)ResetTick);
		((FacepunchBehaviour)this).InvokeRandomized((Action)ResetTick, Random.Range(0f, 1f), resetTickTime, 0.5f);
	}

	public bool PassesResetCheck()
	{
		if (playersBlockReset)
		{
			if (CheckSleepingAIZForPlayers)
			{
				return AIZSleeping();
			}
			return !PlayersWithinDistance();
		}
		return true;
	}

	private bool AIZSleeping()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("PuzzleReset.AIZSleeping");
		if ((Object)(object)zone != (Object)null)
		{
			if (!zone.PointInside(((Component)this).transform.position))
			{
				zone = AIInformationZone.GetForPoint(((Component)this).transform.position);
			}
		}
		else
		{
			zone = AIInformationZone.GetForPoint(((Component)this).transform.position);
		}
		Profiler.EndSample();
		if ((Object)(object)zone == (Object)null)
		{
			return false;
		}
		return zone.Sleeping;
	}

	private bool PlayersWithinDistance()
	{
		return AnyPlayersWithinDistance(playerDetectionOrigin, playerDetectionRadius);
	}

	public static bool AnyPlayersWithinDistance(Transform origin, float radius)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("PuzzleReset.PlayersWithinDistance");
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				if (!current.IsSleeping() && current.IsAlive())
				{
					float num = Vector3.Distance(((Component)current).transform.position, origin.position);
					if (num < radius)
					{
						Profiler.EndSample();
						return true;
					}
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		Profiler.EndSample();
		return false;
	}

	public void ResetTick()
	{
		Profiler.BeginSample("PuzzleReset.ResetTick");
		if (PassesResetCheck())
		{
			resetTimeElapsed += resetTickTime;
		}
		if (resetTimeElapsed > GetResetSpacing())
		{
			resetTimeElapsed = 0f;
			DoReset();
		}
		Profiler.EndSample();
	}

	public void CleanupSleepers()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)playerDetectionOrigin == (Object)null || BasePlayer.sleepingPlayerList == null)
		{
			return;
		}
		for (int num = BasePlayer.sleepingPlayerList.Count - 1; num >= 0; num--)
		{
			BasePlayer basePlayer = BasePlayer.sleepingPlayerList[num];
			if (!((Object)(object)basePlayer == (Object)null) && basePlayer.IsSleeping())
			{
				float num2 = Vector3.Distance(((Component)basePlayer).transform.position, playerDetectionOrigin.position);
				if (num2 <= playerDetectionRadius)
				{
					basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, useProtection: false);
				}
			}
		}
	}

	public void DoReset()
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("PuzzleReset.DoReset");
		CleanupSleepers();
		IOEntity component = ((Component)this).GetComponent<IOEntity>();
		if ((Object)(object)component != (Object)null)
		{
			ResetIOEntRecursive(component, Time.frameCount);
			component.MarkDirty();
		}
		else if (resetPositions != null)
		{
			Vector3[] array = resetPositions;
			foreach (Vector3 val in array)
			{
				Vector3 position = ((Component)this).transform.TransformPoint(val);
				List<IOEntity> list = Pool.GetList<IOEntity>();
				Vis.Entities(position, 0.5f, list, 1235288065, (QueryTriggerInteraction)1);
				foreach (IOEntity item in list)
				{
					if (item.IsRootEntity() && item.isServer)
					{
						ResetIOEntRecursive(item, Time.frameCount);
						item.MarkDirty();
					}
				}
				Pool.FreeList<IOEntity>(ref list);
			}
		}
		List<SpawnGroup> list2 = Pool.GetList<SpawnGroup>();
		Vis.Components<SpawnGroup>(((Component)this).transform.position, 1f, list2, 262144, (QueryTriggerInteraction)2);
		foreach (SpawnGroup item2 in list2)
		{
			if (!((Object)(object)item2 == (Object)null))
			{
				item2.Clear();
				item2.DelayedSpawn();
			}
		}
		Pool.FreeList<SpawnGroup>(ref list2);
		GameObject[] array2 = resetObjects;
		foreach (GameObject val2 in array2)
		{
			if ((Object)(object)val2 != (Object)null)
			{
				val2.SendMessage("OnPuzzleReset", (SendMessageOptions)1);
			}
		}
		if (broadcastResetMessage)
		{
			Enumerator<BasePlayer> enumerator3 = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					BasePlayer current3 = enumerator3.Current;
					if (!current3.IsNpc && current3.IsConnected)
					{
						current3.ShowToast(GameTip.Styles.Server_Event, resetPhrase);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator3).Dispose();
			}
		}
		Profiler.EndSample();
	}

	public static void ResetIOEntRecursive(IOEntity target, int resetIndex)
	{
		if (target.lastResetIndex == resetIndex)
		{
			return;
		}
		target.lastResetIndex = resetIndex;
		target.ResetIOState();
		IOEntity.IOSlot[] outputs = target.outputs;
		foreach (IOEntity.IOSlot iOSlot in outputs)
		{
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null && (Object)(object)iOSlot.connectedTo.Get() != (Object)(object)target)
			{
				ResetIOEntRecursive(iOSlot.connectedTo.Get(), resetIndex);
			}
		}
	}
}
