using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using UnityEngine;

public class SpawnHandler : SingletonComponent<SpawnHandler>
{
	public float TickInterval = 60f;

	public int MinSpawnsPerTick = 100;

	public int MaxSpawnsPerTick = 100;

	public LayerMask PlacementMask;

	public LayerMask PlacementCheckMask;

	public float PlacementCheckHeight = 25f;

	public LayerMask RadiusCheckMask;

	public float RadiusCheckDistance = 5f;

	public LayerMask BoundsCheckMask;

	public SpawnFilter CharacterSpawn;

	public float CharacterSpawnCutoff;

	public SpawnPopulation[] SpawnPopulations;

	internal SpawnDistribution[] SpawnDistributions;

	internal SpawnDistribution CharDistribution;

	internal ListHashSet<ISpawnGroup> SpawnGroups = new ListHashSet<ISpawnGroup>(8);

	internal List<SpawnIndividual> SpawnIndividuals = new List<SpawnIndividual>();

	[ReadOnly]
	public SpawnPopulation[] ConvarSpawnPopulations;

	private Dictionary<SpawnPopulation, SpawnDistribution> population2distribution;

	private bool spawnTick;

	private SpawnPopulation[] AllSpawnPopulations;

	protected void OnEnable()
	{
		AllSpawnPopulations = SpawnPopulations.Concat(ConvarSpawnPopulations).ToArray();
		((MonoBehaviour)this).StartCoroutine(SpawnTick());
		((MonoBehaviour)this).StartCoroutine(SpawnGroupTick());
		((MonoBehaviour)this).StartCoroutine(SpawnIndividualTick());
	}

	public static BasePlayer.SpawnPoint GetSpawnPoint()
	{
		if ((Object)(object)SingletonComponent<SpawnHandler>.Instance == (Object)null || SingletonComponent<SpawnHandler>.Instance.CharDistribution == null)
		{
			return null;
		}
		BasePlayer.SpawnPoint spawnPoint = new BasePlayer.SpawnPoint();
		if (!((WaterSystem.OceanLevel < 0.5f) ? GetSpawnPointStandard(spawnPoint) : FloodedSpawnHandler.GetSpawnPoint(spawnPoint, WaterSystem.OceanLevel + 1f)))
		{
			return null;
		}
		return spawnPoint;
	}

	private static bool GetSpawnPointStandard(BasePlayer.SpawnPoint spawnPoint)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 60; i++)
		{
			if (!SingletonComponent<SpawnHandler>.Instance.CharDistribution.Sample(out spawnPoint.pos, out spawnPoint.rot))
			{
				continue;
			}
			bool flag = true;
			if ((Object)(object)TerrainMeta.Path != (Object)null)
			{
				foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
				{
					if (monument.Distance(spawnPoint.pos) < 50f)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateDistributions()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (World.Size == 0)
		{
			return;
		}
		SpawnDistributions = new SpawnDistribution[AllSpawnPopulations.Length];
		population2distribution = new Dictionary<SpawnPopulation, SpawnDistribution>();
		Vector3 size = TerrainMeta.Size;
		Vector3 position = TerrainMeta.Position;
		int pop_res = Mathf.NextPowerOfTwo((int)((float)World.Size * 0.25f));
		for (int i = 0; i < AllSpawnPopulations.Length; i++)
		{
			SpawnPopulation spawnPopulation = AllSpawnPopulations[i];
			if (spawnPopulation == null)
			{
				Debug.LogError((object)"Spawn handler contains null spawn population.");
				continue;
			}
			byte[] map2 = new byte[pop_res * pop_res];
			SpawnFilter filter2 = spawnPopulation.Filter;
			float cutoff2 = spawnPopulation.FilterCutoff;
			Parallel.For(0, pop_res, (Action<int>)delegate(int z)
			{
				for (int k = 0; k < pop_res; k++)
				{
					float normX2 = ((float)k + 0.5f) / (float)pop_res;
					float normZ2 = ((float)z + 0.5f) / (float)pop_res;
					float factor2 = filter2.GetFactor(normX2, normZ2);
					map2[z * pop_res + k] = (byte)((factor2 >= cutoff2) ? (255f * factor2) : 0f);
				}
			});
			SpawnDistribution value = (SpawnDistributions[i] = new SpawnDistribution(this, map2, position, size));
			population2distribution.Add(spawnPopulation, value);
		}
		int char_res = Mathf.NextPowerOfTwo((int)((float)World.Size * 0.5f));
		byte[] map = new byte[char_res * char_res];
		SpawnFilter filter = CharacterSpawn;
		float cutoff = CharacterSpawnCutoff;
		Parallel.For(0, char_res, (Action<int>)delegate(int z)
		{
			for (int j = 0; j < char_res; j++)
			{
				float normX = ((float)j + 0.5f) / (float)char_res;
				float normZ = ((float)z + 0.5f) / (float)char_res;
				float factor = filter.GetFactor(normX, normZ);
				map[z * char_res + j] = (byte)((factor >= cutoff) ? (255f * factor) : 0f);
			}
		});
		CharDistribution = new SpawnDistribution(this, map, position, size);
	}

	public void FillPopulations()
	{
		if (SpawnDistributions == null)
		{
			return;
		}
		for (int i = 0; i < AllSpawnPopulations.Length; i++)
		{
			if (!(AllSpawnPopulations[i] == null))
			{
				SpawnInitial(AllSpawnPopulations[i], SpawnDistributions[i]);
			}
		}
	}

	public void FillGroups()
	{
		for (int i = 0; i < SpawnGroups.Count; i++)
		{
			SpawnGroups[i].Fill();
		}
	}

	public void FillIndividuals()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < SpawnIndividuals.Count; i++)
		{
			SpawnIndividual spawnIndividual = SpawnIndividuals[i];
			Spawn(Prefab.Load<Spawnable>(spawnIndividual.PrefabID, (GameManager)null, (PrefabAttribute.Library)null), spawnIndividual.Position, spawnIndividual.Rotation);
		}
	}

	public void InitialSpawn()
	{
		if (ConVar.Spawn.respawn_populations && SpawnDistributions != null)
		{
			for (int i = 0; i < AllSpawnPopulations.Length; i++)
			{
				if (!(AllSpawnPopulations[i] == null))
				{
					SpawnInitial(AllSpawnPopulations[i], SpawnDistributions[i]);
				}
			}
		}
		if (ConVar.Spawn.respawn_groups)
		{
			for (int j = 0; j < SpawnGroups.Count; j++)
			{
				SpawnGroups[j].SpawnInitial();
			}
		}
	}

	public void StartSpawnTick()
	{
		spawnTick = true;
	}

	private IEnumerator SpawnTick()
	{
		while (true)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (!spawnTick || !ConVar.Spawn.respawn_populations)
			{
				continue;
			}
			yield return CoroutineEx.waitForSeconds(ConVar.Spawn.tick_populations);
			for (int i = 0; i < AllSpawnPopulations.Length; i++)
			{
				SpawnPopulation spawnPopulation = AllSpawnPopulations[i];
				if (spawnPopulation == null)
				{
					continue;
				}
				SpawnDistribution spawnDistribution = SpawnDistributions[i];
				if (spawnDistribution == null)
				{
					continue;
				}
				try
				{
					if (SpawnDistributions != null)
					{
						SpawnRepeating(spawnPopulation, spawnDistribution);
					}
				}
				catch (Exception ex)
				{
					Debug.LogError((object)ex);
				}
				yield return CoroutineEx.waitForEndOfFrame;
			}
		}
	}

	private IEnumerator SpawnGroupTick()
	{
		while (true)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (!spawnTick || !ConVar.Spawn.respawn_groups)
			{
				continue;
			}
			yield return CoroutineEx.waitForSeconds(1f);
			for (int i = 0; i < SpawnGroups.Count; i++)
			{
				ISpawnGroup spawnGroup = SpawnGroups[i];
				if (spawnGroup != null)
				{
					try
					{
						spawnGroup.SpawnRepeating();
					}
					catch (Exception ex)
					{
						Debug.LogError((object)ex);
					}
					yield return CoroutineEx.waitForEndOfFrame;
				}
			}
		}
	}

	private IEnumerator SpawnIndividualTick()
	{
		while (true)
		{
			yield return CoroutineEx.waitForEndOfFrame;
			if (!spawnTick || !ConVar.Spawn.respawn_individuals)
			{
				continue;
			}
			yield return CoroutineEx.waitForSeconds(ConVar.Spawn.tick_individuals);
			for (int i = 0; i < SpawnIndividuals.Count; i++)
			{
				SpawnIndividual spawnIndividual = SpawnIndividuals[i];
				try
				{
					Spawn(Prefab.Load<Spawnable>(spawnIndividual.PrefabID, (GameManager)null, (PrefabAttribute.Library)null), spawnIndividual.Position, spawnIndividual.Rotation);
				}
				catch (Exception ex)
				{
					Debug.LogError((object)ex);
				}
				yield return CoroutineEx.waitForEndOfFrame;
			}
		}
	}

	public void SpawnInitial(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = GetTargetCount(population, distribution);
		int currentCount = GetCurrentCount(population, distribution);
		int num = targetCount - currentCount;
		Fill(population, distribution, targetCount, num, num * population.SpawnAttemptsInitial);
	}

	public void SpawnRepeating(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = GetTargetCount(population, distribution);
		int currentCount = GetCurrentCount(population, distribution);
		int num = targetCount - currentCount;
		num = Mathf.RoundToInt((float)num * population.GetCurrentSpawnRate());
		num = Random.Range(Mathf.Min(num, MinSpawnsPerTick), Mathf.Min(num, MaxSpawnsPerTick));
		Fill(population, distribution, targetCount, num, num * population.SpawnAttemptsRepeating);
	}

	private void Fill(SpawnPopulation population, SpawnDistribution distribution, int targetCount, int numToFill, int numToTry)
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		if (targetCount == 0)
		{
			return;
		}
		if (!population.Initialize())
		{
			Debug.LogError((object)("[Spawn] No prefabs to spawn in " + population.ResourceFolder), (Object)(object)population);
			return;
		}
		if (Global.developer > 1)
		{
			Debug.Log((object)("[Spawn] Population " + population.ResourceFolder + " needs to spawn " + numToFill));
		}
		float num = Mathf.Max((float)population.ClusterSizeMax, distribution.GetGridCellArea() * population.GetMaximumSpawnDensity());
		population.UpdateWeights(distribution, targetCount);
		while (numToFill >= population.ClusterSizeMin && numToTry > 0)
		{
			ByteQuadtree.Element node = distribution.SampleNode();
			int num2 = Random.Range(population.ClusterSizeMin, population.ClusterSizeMax + 1);
			num2 = Mathx.Min(numToTry, numToFill, num2);
			for (int i = 0; i < num2; i++)
			{
				Vector3 spawnPos;
				Quaternion spawnRot;
				bool flag = distribution.Sample(out spawnPos, out spawnRot, node, population.AlignToNormal, population.ClusterDithering) && population.Filter.GetFactor(spawnPos) > 0f;
				if (flag && population.FilterRadius > 0f)
				{
					flag = population.Filter.GetFactor(spawnPos + Vector3.forward * population.FilterRadius) > 0f && population.Filter.GetFactor(spawnPos - Vector3.forward * population.FilterRadius) > 0f && population.Filter.GetFactor(spawnPos + Vector3.right * population.FilterRadius) > 0f && population.Filter.GetFactor(spawnPos - Vector3.right * population.FilterRadius) > 0f;
				}
				if (flag && population.TryTakeRandomPrefab(out var result))
				{
					if (population.GetSpawnPosOverride(result, ref spawnPos, ref spawnRot) && (float)distribution.GetCount(spawnPos) < num)
					{
						Spawn(population, result, spawnPos, spawnRot);
						numToFill--;
					}
					else
					{
						population.ReturnPrefab(result);
					}
				}
				numToTry--;
			}
		}
		population.OnPostFill(this);
	}

	public GameObject Spawn(SpawnPopulation population, Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		if (prefab == null)
		{
			return null;
		}
		if ((Object)(object)prefab.Component == (Object)null)
		{
			Debug.LogError((object)("[Spawn] Missing component 'Spawnable' on " + prefab.Name));
			return null;
		}
		Vector3 scale = Vector3.one;
		DecorComponent[] components = PrefabAttribute.server.FindAll<DecorComponent>(prefab.ID);
		prefab.Object.transform.ApplyDecorComponents(components, ref pos, ref rot, ref scale);
		if (!prefab.ApplyTerrainAnchors(ref pos, rot, scale, TerrainAnchorMode.MinimizeMovement, population.Filter))
		{
			return null;
		}
		if (!prefab.ApplyTerrainChecks(pos, rot, scale, population.Filter))
		{
			return null;
		}
		if (!prefab.ApplyTerrainFilters(pos, rot, scale))
		{
			return null;
		}
		if (!prefab.ApplyWaterChecks(pos, rot, scale))
		{
			return null;
		}
		if (!prefab.ApplyBoundsChecks(pos, rot, scale, BoundsCheckMask))
		{
			return null;
		}
		if (Global.developer > 1)
		{
			Debug.Log((object)("[Spawn] Spawning " + prefab.Name));
		}
		BaseEntity baseEntity = prefab.SpawnEntity(pos, rot, active: false);
		if ((Object)(object)baseEntity == (Object)null)
		{
			Debug.LogWarning((object)("[Spawn] Couldn't create prefab as entity - " + prefab.Name));
			return null;
		}
		Spawnable component = ((Component)baseEntity).GetComponent<Spawnable>();
		if (component.Population != population)
		{
			component.Population = population;
		}
		((Component)baseEntity).gameObject.AwakeFromInstantiate();
		baseEntity.Spawn();
		return ((Component)baseEntity).gameObject;
	}

	private GameObject Spawn(Prefab<Spawnable> prefab, Vector3 pos, Quaternion rot)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!CheckBounds(prefab.Object, pos, rot, Vector3.one))
		{
			return null;
		}
		BaseEntity baseEntity = prefab.SpawnEntity(pos, rot);
		if ((Object)(object)baseEntity == (Object)null)
		{
			Debug.LogWarning((object)("[Spawn] Couldn't create prefab as entity - " + prefab.Name));
			return null;
		}
		baseEntity.Spawn();
		return ((Component)baseEntity).gameObject;
	}

	public bool CheckBounds(GameObject gameObject, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CheckBounds(gameObject, pos, rot, scale, BoundsCheckMask);
	}

	public static bool CheckBounds(GameObject gameObject, Vector3 pos, Quaternion rot, Vector3 scale, LayerMask mask)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)gameObject == (Object)null)
		{
			return true;
		}
		if (LayerMask.op_Implicit(mask) != 0)
		{
			BaseEntity component = gameObject.GetComponent<BaseEntity>();
			if ((Object)(object)component != (Object)null && Physics.CheckBox(pos + rot * Vector3.Scale(((Bounds)(ref component.bounds)).center, scale), Vector3.Scale(((Bounds)(ref component.bounds)).extents, scale), rot, LayerMask.op_Implicit(mask)))
			{
				return false;
			}
		}
		return true;
	}

	public void EnforceLimits(bool forceAll = false)
	{
		if (SpawnDistributions == null)
		{
			return;
		}
		for (int i = 0; i < AllSpawnPopulations.Length; i++)
		{
			if (!(AllSpawnPopulations[i] == null))
			{
				SpawnPopulation spawnPopulation = AllSpawnPopulations[i];
				SpawnDistribution distribution = SpawnDistributions[i];
				if (forceAll || spawnPopulation.EnforcePopulationLimits)
				{
					EnforceLimits(spawnPopulation, distribution);
				}
			}
		}
	}

	private void EnforceLimits(SpawnPopulation population, SpawnDistribution distribution)
	{
		int targetCount = GetTargetCount(population, distribution);
		Spawnable[] array = FindAll(population);
		if (array.Length <= targetCount)
		{
			return;
		}
		Debug.Log((object)string.Concat(population, " has ", array.Length, " objects, but max allowed is ", targetCount));
		int num = array.Length - targetCount;
		Debug.Log((object)(" - deleting " + num + " objects"));
		foreach (Spawnable item in array.Take(num))
		{
			BaseEntity baseEntity = ((Component)item).gameObject.ToBaseEntity();
			if (baseEntity.IsValid())
			{
				baseEntity.Kill();
			}
			else
			{
				GameManager.Destroy(((Component)item).gameObject);
			}
		}
	}

	public Spawnable[] FindAll(SpawnPopulation population)
	{
		return (from x in Object.FindObjectsOfType<Spawnable>()
			where ((Component)x).gameObject.activeInHierarchy && x.Population == population
			select x).ToArray();
	}

	public int GetTargetCount(SpawnPopulation population, SpawnDistribution distribution)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		float num = TerrainMeta.Size.x * TerrainMeta.Size.z;
		float num2 = population.GetCurrentSpawnDensity();
		if (!population.ScaleWithLargeMaps)
		{
			num = Mathf.Min(num, 16000000f);
		}
		if (population.ScaleWithSpawnFilter)
		{
			num2 *= distribution.Density;
		}
		return Mathf.RoundToInt(num * num2);
	}

	public int GetCurrentCount(SpawnPopulation population, SpawnDistribution distribution)
	{
		return distribution.Count;
	}

	public void AddRespawn(SpawnIndividual individual)
	{
		SpawnIndividuals.Add(individual);
	}

	public void AddInstance(Spawnable spawnable)
	{
		if (spawnable.Population != null)
		{
			if (!population2distribution.TryGetValue(spawnable.Population, out var value))
			{
				Debug.LogWarning((object)("[SpawnHandler] trying to add instance to invalid population: " + spawnable.Population));
			}
			else
			{
				value.AddInstance(spawnable);
			}
		}
	}

	public void RemoveInstance(Spawnable spawnable)
	{
		if (spawnable.Population != null)
		{
			if (!population2distribution.TryGetValue(spawnable.Population, out var value))
			{
				Debug.LogWarning((object)("[SpawnHandler] trying to remove instance from invalid population: " + spawnable.Population));
			}
			else
			{
				value.RemoveInstance(spawnable);
			}
		}
	}

	public static float PlayerFraction()
	{
		float num = Mathf.Max(Server.maxplayers, 1);
		return Mathf.Clamp01((float)BasePlayer.activePlayerList.Count / num);
	}

	public static float PlayerLerp(float min, float max)
	{
		return Mathf.Lerp(min, max, PlayerFraction());
	}

	public static float PlayerExcess()
	{
		float num = Mathf.Max(ConVar.Spawn.player_base, 1f);
		float num2 = BasePlayer.activePlayerList.Count;
		if (num2 <= num)
		{
			return 0f;
		}
		return (num2 - num) / num;
	}

	public static float PlayerScale(float scalar)
	{
		return Mathf.Max(1f, PlayerExcess() * scalar);
	}

	public void DumpReport(string filename)
	{
		File.AppendAllText(filename, "\r\n\r\nSpawnHandler Report:\r\n\r\n" + GetReport());
	}

	public string GetReport(bool detailed = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (AllSpawnPopulations == null)
		{
			stringBuilder.AppendLine("Spawn population array is null.");
		}
		if (SpawnDistributions == null)
		{
			stringBuilder.AppendLine("Spawn distribution array is null.");
		}
		if (AllSpawnPopulations != null && SpawnDistributions != null)
		{
			for (int i = 0; i < AllSpawnPopulations.Length; i++)
			{
				if (AllSpawnPopulations[i] == null)
				{
					continue;
				}
				SpawnPopulation spawnPopulation = AllSpawnPopulations[i];
				SpawnDistribution spawnDistribution = SpawnDistributions[i];
				if (spawnPopulation != null)
				{
					if (!string.IsNullOrEmpty(spawnPopulation.ResourceFolder))
					{
						stringBuilder.AppendLine(((Object)spawnPopulation).name + " (autospawn/" + spawnPopulation.ResourceFolder + ")");
					}
					else
					{
						stringBuilder.AppendLine(((Object)spawnPopulation).name);
					}
					if (detailed)
					{
						stringBuilder.AppendLine("\tPrefabs:");
						if (spawnPopulation.Prefabs != null)
						{
							Prefab<Spawnable>[] prefabs = spawnPopulation.Prefabs;
							foreach (Prefab<Spawnable> prefab in prefabs)
							{
								stringBuilder.AppendLine("\t\t" + prefab.Name + " - " + prefab.Object);
							}
						}
						else
						{
							stringBuilder.AppendLine("\t\tN/A");
						}
					}
					if (spawnDistribution != null)
					{
						int currentCount = GetCurrentCount(spawnPopulation, spawnDistribution);
						int targetCount = GetTargetCount(spawnPopulation, spawnDistribution);
						stringBuilder.AppendLine("\tPopulation: " + currentCount + "/" + targetCount);
					}
					else
					{
						stringBuilder.AppendLine("\tDistribution #" + i + " is not set.");
					}
				}
				else
				{
					stringBuilder.AppendLine("Population #" + i + " is not set.");
				}
				stringBuilder.AppendLine();
			}
		}
		return stringBuilder.ToString();
	}
}
