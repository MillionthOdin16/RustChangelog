using System;
using UnityEngine;
using UnityEngine.Profiling;

public class AIThinkManager : BaseMonoBehaviour, IServerComponent
{
	public enum QueueType
	{
		Human,
		Animal,
		Pets
	}

	public static ListHashSet<IThinker> _processQueue = new ListHashSet<IThinker>(8);

	public static ListHashSet<IThinker> _removalQueue = new ListHashSet<IThinker>(8);

	public static ListHashSet<IThinker> _animalProcessQueue = new ListHashSet<IThinker>(8);

	public static ListHashSet<IThinker> _animalremovalQueue = new ListHashSet<IThinker>(8);

	public static ListHashSet<IThinker> _petProcessQueue = new ListHashSet<IThinker>(8);

	public static ListHashSet<IThinker> _petRemovalQueue = new ListHashSet<IThinker>(8);

	[ServerVar]
	[Help("How many miliseconds to budget for processing AI entities per server frame")]
	public static float framebudgetms = 2.5f;

	[ServerVar]
	[Help("How many miliseconds to budget for processing animal AI entities per server frame")]
	public static float animalframebudgetms = 2.5f;

	[ServerVar]
	[Help("How many miliseconds to budget for processing pet AI entities per server frame")]
	public static float petframebudgetms = 1f;

	private static int lastIndex = 0;

	private static int lastAnimalIndex = 0;

	private static int lastPetIndex;

	public static void ProcessQueue(QueueType queueType)
	{
		switch (queueType)
		{
		case QueueType.Human:
			Profiler.BeginSample("AIThinkManager.ProcessHumanQueue");
			break;
		case QueueType.Pets:
			Profiler.BeginSample("AIThinkManager.ProcessPetsQueue");
			break;
		default:
			Profiler.BeginSample("AIThinkManager.ProcessAnimalQueue");
			break;
		}
		switch (queueType)
		{
		case QueueType.Human:
			DoRemoval(_removalQueue, _processQueue);
			Profiler.BeginSample("AIInformationZone.BudgetedTick");
			AIInformationZone.BudgetedTick();
			Profiler.EndSample();
			break;
		case QueueType.Pets:
			DoRemoval(_petRemovalQueue, _petProcessQueue);
			break;
		default:
			DoRemoval(_animalremovalQueue, _animalProcessQueue);
			break;
		}
		switch (queueType)
		{
		case QueueType.Human:
			DoProcessing(_processQueue, framebudgetms / 1000f, ref lastIndex);
			break;
		case QueueType.Pets:
			DoProcessing(_petProcessQueue, petframebudgetms / 1000f, ref lastPetIndex);
			break;
		default:
			DoProcessing(_animalProcessQueue, animalframebudgetms / 1000f, ref lastAnimalIndex);
			break;
		}
		Profiler.EndSample();
	}

	private static void DoRemoval(ListHashSet<IThinker> removal, ListHashSet<IThinker> process)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Removal");
		if (removal.Count > 0)
		{
			Enumerator<IThinker> enumerator = removal.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					IThinker current = enumerator.Current;
					process.Remove(current);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			removal.Clear();
		}
		Profiler.EndSample();
	}

	private static void DoProcessing(ListHashSet<IThinker> process, float budgetSeconds, ref int last)
	{
		Profiler.BeginSample("Processing");
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (last < process.Count && Time.realtimeSinceStartup < realtimeSinceStartup + budgetSeconds)
		{
			IThinker thinker = process[last];
			if (thinker != null)
			{
				try
				{
					thinker.TryThink();
				}
				catch (Exception ex)
				{
					Debug.LogWarning((object)ex);
				}
			}
			last++;
		}
		if (last >= process.Count)
		{
			last = 0;
		}
		Profiler.EndSample();
	}

	public static void Add(IThinker toAdd)
	{
		_processQueue.Add(toAdd);
	}

	public static void Remove(IThinker toRemove)
	{
		_removalQueue.Add(toRemove);
	}

	public static void AddAnimal(IThinker toAdd)
	{
		_animalProcessQueue.Add(toAdd);
	}

	public static void RemoveAnimal(IThinker toRemove)
	{
		_animalremovalQueue.Add(toRemove);
	}

	public static void AddPet(IThinker toAdd)
	{
		_petProcessQueue.Add(toAdd);
	}

	public static void RemovePet(IThinker toRemove)
	{
		_petRemovalQueue.Add(toRemove);
	}
}
