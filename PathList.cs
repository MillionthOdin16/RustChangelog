using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class PathList
{
	public enum Side
	{
		Both,
		Left,
		Right,
		Any
	}

	public enum Placement
	{
		Center,
		Side
	}

	public enum Alignment
	{
		None,
		Neighbor,
		Forward,
		Inward
	}

	[Serializable]
	public class BasicObject
	{
		public string Folder;

		public SpawnFilter Filter;

		public Placement Placement;

		public bool AlignToNormal = true;

		public bool HeightToTerrain = true;

		public float Offset;
	}

	[Serializable]
	public class SideObject
	{
		public string Folder;

		public SpawnFilter Filter;

		public Side Side;

		public Alignment Alignment;

		public float Density = 1f;

		public float Distance = 25f;

		public float Offset = 2f;
	}

	[Serializable]
	public class PathObject
	{
		public string Folder;

		public SpawnFilter Filter;

		public Alignment Alignment;

		public float Density = 1f;

		public float Distance = 5f;

		public float Dithering = 5f;
	}

	[Serializable]
	public class BridgeObject
	{
		public string Folder;

		public float Distance = 10f;
	}

	public class MeshObject
	{
		public Vector3 Position;

		public Mesh[] Meshes;

		public MeshObject(Vector3 meshPivot, MeshData[] meshData)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			//IL_0031: Expected O, but got Unknown
			Position = meshPivot;
			Meshes = (Mesh[])(object)new Mesh[meshData.Length];
			for (int i = 0; i < Meshes.Length; i++)
			{
				MeshData obj = meshData[i];
				Mesh[] meshes = Meshes;
				int num = i;
				Mesh val = new Mesh();
				Mesh val2 = val;
				meshes[num] = val;
				Mesh mesh = val2;
				obj.Apply(mesh);
			}
		}
	}

	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	private static Quaternion rot180 = Quaternion.Euler(0f, 180f, 0f);

	private static Quaternion rot270 = Quaternion.Euler(0f, 270f, 0f);

	public string Name;

	public PathInterpolator Path;

	public bool Spline;

	public bool Start;

	public bool End;

	public float Width;

	public float InnerPadding;

	public float OuterPadding;

	public float InnerFade;

	public float OuterFade;

	public float RandomScale;

	public float MeshOffset;

	public float TerrainOffset;

	public int Topology;

	public int Splat;

	public int Hierarchy;

	public PathFinder.Node ProcgenStartNode;

	public PathFinder.Node ProcgenEndNode;

	public const float StepSize = 1f;

	private static float[] placements = new float[3] { 0f, -1f, 1f };

	public PathList(string name, Vector3[] points)
	{
		Name = name;
		Path = new PathInterpolator(points);
	}

	private void SpawnObjectsNeighborAligned(ref uint seed, Prefab[] prefabs, List<Vector3> positions, SpawnFilter filter = null)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (positions.Count < 2)
		{
			return;
		}
		List<Prefab> list = Pool.GetList<Prefab>();
		for (int i = 0; i < positions.Count; i++)
		{
			int index = Mathf.Max(i - 1, 0);
			int index2 = Mathf.Min(i + 1, positions.Count - 1);
			Vector3 position = positions[i];
			Quaternion rotation = Quaternion.LookRotation(Vector3Ex.XZ3D(positions[index2] - positions[index]));
			SpawnObject(ref seed, prefabs, position, rotation, list, out var spawned, positions.Count, i, filter);
			if (spawned != null)
			{
				list.Add(spawned);
			}
		}
		Pool.FreeList<Prefab>(ref list);
	}

	private bool SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Prefab random = prefabs.GetRandom(ref seed);
		Vector3 pos = position;
		Quaternion rot = rotation;
		Vector3 scale = random.Object.transform.localScale;
		random.ApplyDecorComponents(ref pos, ref rot, ref scale);
		if (!random.ApplyTerrainAnchors(ref pos, rot, scale, filter))
		{
			return false;
		}
		World.AddPrefab(Name, random, pos, rot, scale);
		return true;
	}

	private bool SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 position, Quaternion rotation, List<Prefab> previousSpawns, out Prefab spawned, int pathLength, int index, SpawnFilter filter = null)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		spawned = null;
		Prefab replacement = prefabs.GetRandom(ref seed);
		replacement.ApplySequenceReplacement(previousSpawns, ref replacement, prefabs, pathLength, index);
		Vector3 pos = position;
		Quaternion rot = rotation;
		Vector3 scale = replacement.Object.transform.localScale;
		replacement.ApplyDecorComponents(ref pos, ref rot, ref scale);
		if (!replacement.ApplyTerrainAnchors(ref pos, rot, scale, filter))
		{
			return false;
		}
		World.AddPrefab(Name, replacement, pos, rot, scale);
		spawned = replacement;
		return true;
	}

	private bool CheckObjects(Prefab[] prefabs, Vector3 position, Quaternion rotation, SpawnFilter filter = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		foreach (Prefab obj in prefabs)
		{
			Vector3 pos = position;
			Vector3 localScale = obj.Object.transform.localScale;
			if (!obj.ApplyTerrainAnchors(ref pos, rotation, localScale, filter))
			{
				return false;
			}
		}
		return true;
	}

	private void SpawnObject(ref uint seed, Prefab[] prefabs, Vector3 pos, Vector3 dir, BasicObject obj)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (!obj.AlignToNormal)
		{
			Vector3 val = Vector3Ex.XZ3D(dir);
			dir = ((Vector3)(ref val)).normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 val2 = (Width * 0.5f + obj.Offset) * (rot90 * dir);
		for (int i = 0; i < placements.Length; i++)
		{
			if ((obj.Placement == Placement.Center && i != 0) || (obj.Placement == Placement.Side && i == 0))
			{
				continue;
			}
			Vector3 val3 = pos + placements[i] * val2;
			if (obj.HeightToTerrain)
			{
				val3.y = TerrainMeta.HeightMap.GetHeight(val3);
			}
			if (filter.Test(val3))
			{
				Quaternion rotation = ((i == 2) ? Quaternion.LookRotation(rot180 * dir) : Quaternion.LookRotation(dir));
				if (SpawnObject(ref seed, prefabs, val3, rotation, filter))
				{
					break;
				}
			}
		}
	}

	private bool CheckObjects(Prefab[] prefabs, Vector3 pos, Vector3 dir, BasicObject obj)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (!obj.AlignToNormal)
		{
			Vector3 val = Vector3Ex.XZ3D(dir);
			dir = ((Vector3)(ref val)).normalized;
		}
		SpawnFilter filter = obj.Filter;
		Vector3 val2 = (Width * 0.5f + obj.Offset) * (rot90 * dir);
		for (int i = 0; i < placements.Length; i++)
		{
			if ((obj.Placement == Placement.Center && i != 0) || (obj.Placement == Placement.Side && i == 0))
			{
				continue;
			}
			Vector3 val3 = pos + placements[i] * val2;
			if (obj.HeightToTerrain)
			{
				val3.y = TerrainMeta.HeightMap.GetHeight(val3);
			}
			if (filter.Test(val3))
			{
				Quaternion rotation = ((i == 2) ? Quaternion.LookRotation(rot180 * dir) : Quaternion.LookRotation(dir));
				if (CheckObjects(prefabs, val3, rotation, filter))
				{
					return true;
				}
			}
		}
		return false;
	}

	public void SpawnSide(ref uint seed, SideObject obj)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder);
		if (array == null || array.Length == 0)
		{
			Debug.LogError((object)("Empty decor folder: " + obj.Folder));
			return;
		}
		Side side = obj.Side;
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float num = Width * 0.5f + obj.Offset;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		float[] array2 = new float[2]
		{
			0f - num,
			num
		};
		int num2 = 0;
		Vector3 val = Path.GetStartPoint();
		List<Vector3> list = new List<Vector3>();
		float num3 = distance * 0.25f;
		float num4 = distance * 0.5f;
		float num5 = Path.StartOffset + num4;
		float num6 = Path.Length - Path.EndOffset - num4;
		for (float num7 = num5; num7 <= num6; num7 += num3)
		{
			Vector3 val2 = (Spline ? Path.GetPointCubicHermite(num7) : Path.GetPoint(num7));
			Vector3 val3 = val2 - val;
			if (((Vector3)(ref val3)).magnitude < distance)
			{
				continue;
			}
			Vector3 tangent = Path.GetTangent(num7);
			Vector3 val4 = rot90 * tangent;
			for (int i = 0; i < array2.Length; i++)
			{
				int num8 = (num2 + i) % array2.Length;
				if ((side == Side.Left && num8 != 0) || (side == Side.Right && num8 != 1))
				{
					continue;
				}
				float num9 = array2[num8];
				Vector3 val5 = val2;
				val5.x += val4.x * num9;
				val5.z += val4.z * num9;
				float normX = TerrainMeta.NormalizeX(val5.x);
				float normZ = TerrainMeta.NormalizeZ(val5.z);
				if (filter.GetFactor(normX, normZ) < SeedRandom.Value(ref seed))
				{
					continue;
				}
				if (density >= SeedRandom.Value(ref seed))
				{
					val5.y = heightMap.GetHeight(normX, normZ);
					if (obj.Alignment == Alignment.None)
					{
						if (!SpawnObject(ref seed, array, val5, Quaternion.LookRotation(Vector3.zero), filter))
						{
							continue;
						}
					}
					else if (obj.Alignment == Alignment.Forward)
					{
						if (!SpawnObject(ref seed, array, val5, Quaternion.LookRotation(tangent * num9), filter))
						{
							continue;
						}
					}
					else if (obj.Alignment == Alignment.Inward)
					{
						if (!SpawnObject(ref seed, array, val5, Quaternion.LookRotation(tangent * num9) * rot270, filter))
						{
							continue;
						}
					}
					else
					{
						list.Add(val5);
					}
				}
				num2 = num8;
				val = val2;
				if (side == Side.Any)
				{
					break;
				}
			}
		}
		if (list.Count > 0)
		{
			SpawnObjectsNeighborAligned(ref seed, array, list, filter);
		}
	}

	public void SpawnAlong(ref uint seed, PathObject obj)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder);
		if (array == null || array.Length == 0)
		{
			Debug.LogError((object)("Empty decor folder: " + obj.Folder));
			return;
		}
		SpawnFilter filter = obj.Filter;
		float density = obj.Density;
		float distance = obj.Distance;
		float dithering = obj.Dithering;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 val = Path.GetStartPoint();
		List<Vector3> list = new List<Vector3>();
		float num = distance * 0.25f;
		float num2 = distance * 0.5f;
		float num3 = Path.StartOffset + num2;
		float num4 = Path.Length - Path.EndOffset - num2;
		for (float num5 = num3; num5 <= num4; num5 += num)
		{
			Vector3 val2 = (Spline ? Path.GetPointCubicHermite(num5) : Path.GetPoint(num5));
			Vector3 val3 = val2 - val;
			if (((Vector3)(ref val3)).magnitude < distance)
			{
				continue;
			}
			Vector3 tangent = Path.GetTangent(num5);
			Vector3 val4 = rot90 * tangent;
			Vector3 val5 = val2;
			val5.x += SeedRandom.Range(ref seed, 0f - dithering, dithering);
			val5.z += SeedRandom.Range(ref seed, 0f - dithering, dithering);
			float normX = TerrainMeta.NormalizeX(val5.x);
			float normZ = TerrainMeta.NormalizeZ(val5.z);
			if (filter.GetFactor(normX, normZ) < SeedRandom.Value(ref seed))
			{
				continue;
			}
			if (density >= SeedRandom.Value(ref seed))
			{
				val5.y = heightMap.GetHeight(normX, normZ);
				if (obj.Alignment == Alignment.None)
				{
					if (!SpawnObject(ref seed, array, val5, Quaternion.identity, filter))
					{
						continue;
					}
				}
				else if (obj.Alignment == Alignment.Forward)
				{
					if (!SpawnObject(ref seed, array, val5, Quaternion.LookRotation(tangent), filter))
					{
						continue;
					}
				}
				else if (obj.Alignment == Alignment.Inward)
				{
					if (!SpawnObject(ref seed, array, val5, Quaternion.LookRotation(val4), filter))
					{
						continue;
					}
				}
				else
				{
					list.Add(val5);
				}
			}
			val = val2;
		}
		if (list.Count > 0)
		{
			SpawnObjectsNeighborAligned(ref seed, array, list, filter);
		}
	}

	public void SpawnBridge(ref uint seed, BridgeObject obj)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder);
		if (array == null || array.Length == 0)
		{
			Debug.LogError((object)("Empty decor folder: " + obj.Folder));
			return;
		}
		Vector3 startPoint = Path.GetStartPoint();
		Vector3 val = Path.GetEndPoint() - startPoint;
		float magnitude = ((Vector3)(ref val)).magnitude;
		Vector3 val2 = val / magnitude;
		float num = magnitude / obj.Distance;
		int num2 = Mathf.RoundToInt(num);
		float num3 = 0.5f * (num - (float)num2);
		Vector3 val3 = obj.Distance * val2;
		Vector3 val4 = startPoint + (0.5f + num3) * val3;
		Quaternion rotation = Quaternion.LookRotation(val2);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		for (int i = 0; i < num2; i++)
		{
			float num4 = Mathf.Max(heightMap.GetHeight(val4), waterMap.GetHeight(val4)) - 1f;
			if (val4.y > num4)
			{
				SpawnObject(ref seed, array, val4, rotation);
			}
			val4 += val3;
		}
	}

	public void SpawnStart(ref uint seed, BasicObject obj)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (Start && !string.IsNullOrEmpty(obj.Folder))
		{
			Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder);
			if (array == null || array.Length == 0)
			{
				Debug.LogError((object)("Empty decor folder: " + obj.Folder));
				return;
			}
			Vector3 startPoint = Path.GetStartPoint();
			Vector3 startTangent = Path.GetStartTangent();
			SpawnObject(ref seed, array, startPoint, startTangent, obj);
		}
	}

	public void SpawnEnd(ref uint seed, BasicObject obj)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (End && !string.IsNullOrEmpty(obj.Folder))
		{
			Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder);
			if (array == null || array.Length == 0)
			{
				Debug.LogError((object)("Empty decor folder: " + obj.Folder));
				return;
			}
			Vector3 endPoint = Path.GetEndPoint();
			Vector3 dir = -Path.GetEndTangent();
			SpawnObject(ref seed, array, endPoint, dir, obj);
		}
	}

	public void TrimStart(BasicObject obj)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (!Start || string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder);
		if (array == null || array.Length == 0)
		{
			Debug.LogError((object)("Empty decor folder: " + obj.Folder));
			return;
		}
		Vector3[] points = Path.Points;
		Vector3[] tangents = Path.Tangents;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 pos = points[Path.MinIndex + i];
			Vector3 dir = tangents[Path.MinIndex + i];
			if (CheckObjects(array, pos, dir, obj))
			{
				Path.MinIndex += i;
				break;
			}
		}
	}

	public void TrimEnd(BasicObject obj)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!End || string.IsNullOrEmpty(obj.Folder))
		{
			return;
		}
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + obj.Folder);
		if (array == null || array.Length == 0)
		{
			Debug.LogError((object)("Empty decor folder: " + obj.Folder));
			return;
		}
		Vector3[] points = Path.Points;
		Vector3[] tangents = Path.Tangents;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 pos = points[Path.MaxIndex - i];
			Vector3 dir = -tangents[Path.MaxIndex - i];
			if (CheckObjects(array, pos, dir, obj))
			{
				Path.MaxIndex -= i;
				break;
			}
		}
	}

	public void TrimTopology(int topology)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] points = Path.Points;
		int num = points.Length / 4;
		for (int i = 0; i < num; i++)
		{
			Vector3 worldPos = points[Path.MinIndex + i];
			if (!TerrainMeta.TopologyMap.GetTopology(worldPos, topology))
			{
				Path.MinIndex += i;
				break;
			}
		}
		for (int j = 0; j < num; j++)
		{
			Vector3 worldPos2 = points[Path.MaxIndex - j];
			if (!TerrainMeta.TopologyMap.GetTopology(worldPos2, topology))
			{
				Path.MaxIndex -= j;
				break;
			}
		}
	}

	public void ResetTrims()
	{
		Path.MinIndex = Path.DefaultMinIndex;
		Path.MaxIndex = Path.DefaultMaxIndex;
	}

	public void AdjustTerrainHeight(float intensity = 1f, float fade = 1f)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		_ = TerrainMeta.TopologyMap;
		float num = 1f;
		float randomScale = RandomScale;
		float outerPadding = OuterPadding;
		float innerPadding = InnerPadding;
		float outerFade = OuterFade * fade;
		float innerFade = InnerFade;
		float offset = TerrainOffset * TerrainMeta.OneOverSize.y;
		float num2 = Width * 0.5f;
		Vector3 startPoint = Path.GetStartPoint();
		Vector3 endPoint = Path.GetEndPoint();
		Vector3 startTangent = Path.GetStartTangent();
		Vector3 val = Vector3Ex.XZ3D(startTangent);
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 val2 = rot90 * normalized;
		Vector3 val3 = startPoint;
		Line prev_line = new Line(startPoint, startPoint + startTangent * num);
		Vector3 val4 = startPoint - val2 * (num2 + outerPadding + outerFade);
		Vector3 val5 = startPoint + val2 * (num2 + outerPadding + outerFade);
		Vector3 val6 = val3;
		Vector3 val7 = startTangent;
		Line cur_line = prev_line;
		Vector3 val8 = val4;
		Vector3 val9 = val5;
		float num3 = Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 val10 = (Spline ? Path.GetPointCubicHermite(num4 + num) : Path.GetPoint(num4 + num));
			Vector3 tangent = Path.GetTangent(num4 + num);
			Line next_line = new Line(val10, val10 + tangent * num);
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(val6.x, val6.z, 2, 0.005f));
			if (!Path.Circular)
			{
				float num5 = Vector3Ex.Magnitude2D(startPoint - val6);
				float num6 = Vector3Ex.Magnitude2D(endPoint - val6);
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(num5, num6));
			}
			val = Vector3Ex.XZ3D(val7);
			normalized = ((Vector3)(ref val)).normalized;
			val2 = rot90 * normalized;
			val8 = val6 - val2 * (radius + outerPadding + outerFade);
			val9 = val6 + val2 * (radius + outerPadding + outerFade);
			float yn = TerrainMeta.NormalizeY((val6.y + val3.y) * 0.5f);
			heightmap.ForEach(val4, val5, val8, val9, delegate(int x, int z)
			{
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0065: Unknown result type (might be due to invalid IL or missing references)
				//IL_0067: Unknown result type (might be due to invalid IL or missing references)
				//IL_0068: Unknown result type (might be due to invalid IL or missing references)
				//IL_0069: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				//IL_0076: Unknown result type (might be due to invalid IL or missing references)
				//IL_0077: Unknown result type (might be due to invalid IL or missing references)
				//IL_0083: Unknown result type (might be due to invalid IL or missing references)
				//IL_0084: Unknown result type (might be due to invalid IL or missing references)
				//IL_0086: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_0097: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
				//IL_00af: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
				//IL_012e: Unknown result type (might be due to invalid IL or missing references)
				float num7 = heightmap.Coordinate(x);
				float num8 = heightmap.Coordinate(z);
				Vector3 val11 = TerrainMeta.Denormalize(new Vector3(num7, yn, num8));
				Vector3 val12 = ((Line)(ref prev_line)).ClosestPoint2D(val11);
				Vector3 val13 = ((Line)(ref cur_line)).ClosestPoint2D(val11);
				Vector3 val14 = ((Line)(ref next_line)).ClosestPoint2D(val11);
				float num9 = Vector3Ex.Magnitude2D(val11 - val12);
				float num10 = Vector3Ex.Magnitude2D(val11 - val13);
				float num11 = Vector3Ex.Magnitude2D(val11 - val14);
				float num12 = num10;
				Vector3 val15 = val13;
				if (!(num10 <= num9) || !(num10 <= num11))
				{
					if (num9 <= num11)
					{
						num12 = num9;
						val15 = val12;
					}
					else
					{
						num12 = num11;
						val15 = val14;
					}
				}
				float num13 = Mathf.InverseLerp(radius + outerPadding + outerFade, radius + outerPadding, num12);
				float num14 = Mathf.InverseLerp(radius - innerPadding, radius - innerPadding - innerFade, num12);
				float num15 = TerrainMeta.NormalizeY(val15.y);
				heightmap.SetHeight(x, z, num15 + Mathf.SmoothStep(0f, offset, num14), intensity * opacity * num13);
			});
			val3 = val6;
			val4 = val8;
			val5 = val9;
			prev_line = cur_line;
			val6 = val10;
			val7 = tangent;
			cur_line = next_line;
		}
	}

	public void AdjustTerrainTexture()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if (Splat == 0)
		{
			return;
		}
		TerrainSplatMap splatmap = TerrainMeta.SplatMap;
		float num = 1f;
		float randomScale = RandomScale;
		float outerPadding = OuterPadding;
		float innerPadding = InnerPadding;
		float num2 = Width * 0.5f;
		Vector3 startPoint = Path.GetStartPoint();
		Vector3 endPoint = Path.GetEndPoint();
		Vector3 startTangent = Path.GetStartTangent();
		Vector3 val = Vector3Ex.XZ3D(startTangent);
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 val2 = rot90 * normalized;
		Vector3 v = startPoint - val2 * (num2 + outerPadding);
		Vector3 v2 = startPoint + val2 * (num2 + outerPadding);
		float num3 = Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 val3 = (Spline ? Path.GetPointCubicHermite(num4) : Path.GetPoint(num4));
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(val3.x, val3.z, 2, 0.005f));
			if (!Path.Circular)
			{
				float num5 = Vector3Ex.Magnitude2D(startPoint - val3);
				float num6 = Vector3Ex.Magnitude2D(endPoint - val3);
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(num5, num6));
			}
			startTangent = Path.GetTangent(num4);
			val = Vector3Ex.XZ3D(startTangent);
			normalized = ((Vector3)(ref val)).normalized;
			val2 = rot90 * normalized;
			Ray ray = new Ray(val3, startTangent);
			Vector3 val4 = val3 - val2 * (radius + outerPadding);
			Vector3 val5 = val3 + val2 * (radius + outerPadding);
			float yn = TerrainMeta.NormalizeY(val3.y);
			splatmap.ForEach(v, v2, val4, val5, delegate(int x, int z)
			{
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				float num7 = splatmap.Coordinate(x);
				float num8 = splatmap.Coordinate(z);
				Vector3 val6 = TerrainMeta.Denormalize(new Vector3(num7, yn, num8));
				Vector3 val7 = ray.ClosestPoint(val6);
				float num9 = Vector3Ex.Magnitude2D(val6 - val7);
				float num10 = Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, num9);
				splatmap.SetSplat(x, z, Splat, num10 * opacity);
			});
			v = val4;
			v2 = val5;
		}
	}

	public void AdjustTerrainTopology()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if (Topology == 0)
		{
			return;
		}
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		float num = 1f;
		float randomScale = RandomScale;
		float outerPadding = OuterPadding;
		float innerPadding = InnerPadding;
		float num2 = Width * 0.5f;
		Vector3 startPoint = Path.GetStartPoint();
		Vector3 endPoint = Path.GetEndPoint();
		Vector3 startTangent = Path.GetStartTangent();
		Vector3 val = Vector3Ex.XZ3D(startTangent);
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 val2 = rot90 * normalized;
		Vector3 v = startPoint - val2 * (num2 + outerPadding);
		Vector3 v2 = startPoint + val2 * (num2 + outerPadding);
		float num3 = Path.Length + num;
		for (float num4 = 0f; num4 < num3; num4 += num)
		{
			Vector3 val3 = (Spline ? Path.GetPointCubicHermite(num4) : Path.GetPoint(num4));
			float opacity = 1f;
			float radius = Mathf.Lerp(num2, num2 * randomScale, Noise.Billow(val3.x, val3.z, 2, 0.005f));
			if (!Path.Circular)
			{
				float num5 = Vector3Ex.Magnitude2D(startPoint - val3);
				float num6 = Vector3Ex.Magnitude2D(endPoint - val3);
				opacity = Mathf.InverseLerp(0f, num2, Mathf.Min(num5, num6));
			}
			startTangent = Path.GetTangent(num4);
			val = Vector3Ex.XZ3D(startTangent);
			normalized = ((Vector3)(ref val)).normalized;
			val2 = rot90 * normalized;
			Ray ray = new Ray(val3, startTangent);
			Vector3 val4 = val3 - val2 * (radius + outerPadding);
			Vector3 val5 = val3 + val2 * (radius + outerPadding);
			float yn = TerrainMeta.NormalizeY(val3.y);
			topomap.ForEach(v, v2, val4, val5, delegate(int x, int z)
			{
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				float num7 = topomap.Coordinate(x);
				float num8 = topomap.Coordinate(z);
				Vector3 val6 = TerrainMeta.Denormalize(new Vector3(num7, yn, num8));
				Vector3 val7 = ray.ClosestPoint(val6);
				float num9 = Vector3Ex.Magnitude2D(val6 - val7);
				if (Mathf.InverseLerp(radius + outerPadding, radius - innerPadding, num9) * opacity > 0.3f)
				{
					topomap.AddTopology(x, z, Topology);
				}
			});
			v = val4;
			v2 = val5;
		}
	}

	public void AdjustPlacementMap(float width)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		TerrainPlacementMap placementmap = TerrainMeta.PlacementMap;
		float num = 1f;
		float radius = width * 0.5f;
		Vector3 startPoint = Path.GetStartPoint();
		Path.GetEndPoint();
		Vector3 startTangent = Path.GetStartTangent();
		Vector3 val = Vector3Ex.XZ3D(startTangent);
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 val2 = rot90 * normalized;
		Vector3 v = startPoint - val2 * radius;
		Vector3 v2 = startPoint + val2 * radius;
		float num2 = Path.Length + num;
		for (float num3 = 0f; num3 < num2; num3 += num)
		{
			Vector3 val3 = (Spline ? Path.GetPointCubicHermite(num3) : Path.GetPoint(num3));
			startTangent = Path.GetTangent(num3);
			val = Vector3Ex.XZ3D(startTangent);
			normalized = ((Vector3)(ref val)).normalized;
			val2 = rot90 * normalized;
			Ray ray = new Ray(val3, startTangent);
			Vector3 val4 = val3 - val2 * radius;
			Vector3 val5 = val3 + val2 * radius;
			float yn = TerrainMeta.NormalizeY(val3.y);
			placementmap.ForEach(v, v2, val4, val5, delegate(int x, int z)
			{
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0042: Unknown result type (might be due to invalid IL or missing references)
				//IL_0043: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				float num4 = placementmap.Coordinate(x);
				float num5 = placementmap.Coordinate(z);
				Vector3 val6 = TerrainMeta.Denormalize(new Vector3(num4, yn, num5));
				Vector3 val7 = ray.ClosestPoint(val6);
				if (Vector3Ex.Magnitude2D(val6 - val7) <= radius)
				{
					placementmap.SetBlocked(x, z);
				}
			});
			v = val4;
			v2 = val5;
		}
	}

	public List<MeshObject> CreateMesh(Mesh[] meshes, float normalSmoothing, bool snapToTerrain, bool snapStartToTerrain, bool snapEndToTerrain)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		MeshCache.Data[] array = new MeshCache.Data[meshes.Length];
		MeshData[] array2 = new MeshData[meshes.Length];
		for (int i = 0; i < meshes.Length; i++)
		{
			array[i] = MeshCache.Get(meshes[i]);
			array2[i] = new MeshData();
		}
		MeshData[] array3 = array2;
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j].AllocMinimal();
		}
		Bounds bounds = meshes[meshes.Length - 1].bounds;
		Vector3 min = ((Bounds)(ref bounds)).min;
		Vector3 size = ((Bounds)(ref bounds)).size;
		float num = Width / ((Bounds)(ref bounds)).size.x;
		List<MeshObject> list = new List<MeshObject>();
		int num2 = (int)(Path.Length / (num * ((Bounds)(ref bounds)).size.z));
		int num3 = 5;
		float num4 = Path.Length / (float)num2;
		float randomScale = RandomScale;
		float meshOffset = MeshOffset;
		float num5 = Width * 0.5f;
		_ = array[0].vertices.Length;
		_ = array[0].triangles.Length;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		for (int k = 0; k < num2; k += num3)
		{
			float distance = (float)k * num4 + 0.5f * (float)num3 * num4;
			Vector3 val = (Spline ? Path.GetPointCubicHermite(distance) : Path.GetPoint(distance));
			for (int l = 0; l < num3 && k + l < num2; l++)
			{
				float num6 = (float)(k + l) * num4;
				for (int m = 0; m < meshes.Length; m++)
				{
					MeshCache.Data data = array[m];
					MeshData meshData = array2[m];
					int count = meshData.vertices.Count;
					for (int n = 0; n < data.vertices.Length; n++)
					{
						Vector2 item = data.uv[n];
						Vector3 val2 = data.vertices[n];
						Vector3 val3 = data.normals[n];
						Vector4 val4 = data.tangents[n];
						float num7 = (val2.x - min.x) / size.x;
						float num8 = val2.y - min.y;
						float num9 = (val2.z - min.z) / size.z;
						float num10 = num6 + num9 * num4;
						Vector3 val5 = (Spline ? Path.GetPointCubicHermite(num10) : Path.GetPoint(num10));
						Vector3 tangent = Path.GetTangent(num10);
						Vector3 val6 = Vector3Ex.XZ3D(tangent);
						Vector3 normalized = ((Vector3)(ref val6)).normalized;
						Vector3 val7 = rot90 * normalized;
						Vector3 val8 = Vector3.Cross(tangent, val7);
						Quaternion val9 = Quaternion.LookRotation(normalized, val8);
						float num11 = Mathf.Lerp(num5, num5 * randomScale, Noise.Billow(val5.x, val5.z, 2, 0.005f));
						Vector3 val10 = val5 - val7 * num11;
						Vector3 val11 = val5 + val7 * num11;
						if (snapToTerrain)
						{
							val10.y = heightMap.GetHeight(val10);
							val11.y = heightMap.GetHeight(val11);
						}
						val10 += val8 * meshOffset;
						val11 += val8 * meshOffset;
						val2 = Vector3.Lerp(val10, val11, num7);
						if ((snapStartToTerrain && num10 < 0.1f) || (snapEndToTerrain && num10 > Path.Length - 0.1f))
						{
							val2.y = heightMap.GetHeight(val2);
						}
						else
						{
							val2.y += num8;
						}
						val2 -= val;
						val3 = val9 * val3;
						val4 = Vector4.op_Implicit(val9 * Vector4.op_Implicit(val4));
						if (normalSmoothing > 0f)
						{
							val3 = Vector3.Slerp(val3, Vector3.up, normalSmoothing);
						}
						meshData.vertices.Add(val2);
						meshData.normals.Add(val3);
						meshData.tangents.Add(val4);
						meshData.uv.Add(item);
					}
					for (int num12 = 0; num12 < data.triangles.Length; num12++)
					{
						int num13 = data.triangles[num12];
						meshData.triangles.Add(count + num13);
					}
				}
			}
			list.Add(new MeshObject(val, array2));
			array3 = array2;
			for (int j = 0; j < array3.Length; j++)
			{
				array3[j].Clear();
			}
		}
		array3 = array2;
		for (int j = 0; j < array3.Length; j++)
		{
			array3[j].Free();
		}
		return list;
	}
}
