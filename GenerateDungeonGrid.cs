using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateDungeonGrid : ProceduralComponent
{
	private class PathNode
	{
		public MonumentInfo monument;

		public PathFinder.Node node;
	}

	private class PathSegment
	{
		public PathFinder.Node start;

		public PathFinder.Node end;
	}

	private class PathLink
	{
		public PathLinkSide downwards;

		public PathLinkSide upwards;
	}

	private class PathLinkSide
	{
		public PathLinkSegment origin;

		public List<PathLinkSegment> segments;

		public PathLinkSegment prevSegment
		{
			get
			{
				if (segments.Count <= 0)
				{
					return origin;
				}
				return segments[segments.Count - 1];
			}
		}
	}

	private class PathLinkSegment
	{
		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;

		public Prefab<DungeonGridLink> prefab;

		public DungeonGridLink link;

		public Transform downSocket => link.DownSocket;

		public Transform upSocket => link.UpSocket;

		public DungeonGridLinkType downType => link.DownType;

		public DungeonGridLinkType upType => link.UpType;
	}

	private struct PrefabReplacement
	{
		public Vector2i gridPosition;

		public Vector3 worldPosition;

		public float distance;

		public Prefab<DungeonGridCell> prefab;
	}

	public string TunnelFolder = string.Empty;

	public string StationFolder = string.Empty;

	public string UpwardsFolder = string.Empty;

	public string TransitionFolder = string.Empty;

	public string LinkFolder = string.Empty;

	public InfrastructureType ConnectionType = InfrastructureType.Tunnel;

	public int CellSize = 216;

	public float LinkHeight = 1.5f;

	public float LinkRadius = 3f;

	public float LinkTransition = 9f;

	private const int MaxDepth = 100000;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0861: Unknown result type (might be due to invalid IL or missing references)
		//IL_0869: Unknown result type (might be due to invalid IL or missing references)
		//IL_0870: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_1025: Unknown result type (might be due to invalid IL or missing references)
		//IL_102a: Unknown result type (might be due to invalid IL or missing references)
		//IL_102c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1031: Unknown result type (might be due to invalid IL or missing references)
		//IL_1033: Unknown result type (might be due to invalid IL or missing references)
		//IL_103d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1042: Unknown result type (might be due to invalid IL or missing references)
		//IL_1044: Unknown result type (might be due to invalid IL or missing references)
		//IL_1055: Unknown result type (might be due to invalid IL or missing references)
		//IL_105a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_105c: Unknown result type (might be due to invalid IL or missing references)
		//IL_105e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_113e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1140: Unknown result type (might be due to invalid IL or missing references)
		//IL_1091: Unknown result type (might be due to invalid IL or missing references)
		//IL_1093: Unknown result type (might be due to invalid IL or missing references)
		//IL_1098: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_0515: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10be: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1169: Unknown result type (might be due to invalid IL or missing references)
		//IL_116e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1170: Unknown result type (might be due to invalid IL or missing references)
		//IL_1175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10db: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1104: Unknown result type (might be due to invalid IL or missing references)
		//IL_1109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_05da: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0491: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0646: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_0693: Unknown result type (might be due to invalid IL or missing references)
		//IL_0698: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_11dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1202: Unknown result type (might be due to invalid IL or missing references)
		//IL_1210: Unknown result type (might be due to invalid IL or missing references)
		//IL_1221: Unknown result type (might be due to invalid IL or missing references)
		//IL_1237: Unknown result type (might be due to invalid IL or missing references)
		//IL_1248: Unknown result type (might be due to invalid IL or missing references)
		//IL_124d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1252: Unknown result type (might be due to invalid IL or missing references)
		//IL_1257: Unknown result type (might be due to invalid IL or missing references)
		//IL_125c: Unknown result type (might be due to invalid IL or missing references)
		//IL_125e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1263: Unknown result type (might be due to invalid IL or missing references)
		//IL_127c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1281: Unknown result type (might be due to invalid IL or missing references)
		//IL_1297: Unknown result type (might be due to invalid IL or missing references)
		//IL_129c: Unknown result type (might be due to invalid IL or missing references)
		//IL_12af: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f46: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f54: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_130e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1315: Unknown result type (might be due to invalid IL or missing references)
		//IL_131c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1328: Unknown result type (might be due to invalid IL or missing references)
		//IL_132d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1332: Unknown result type (might be due to invalid IL or missing references)
		//IL_1337: Unknown result type (might be due to invalid IL or missing references)
		//IL_133c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1340: Unknown result type (might be due to invalid IL or missing references)
		//IL_134c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1351: Unknown result type (might be due to invalid IL or missing references)
		//IL_1356: Unknown result type (might be due to invalid IL or missing references)
		//IL_1848: Unknown result type (might be due to invalid IL or missing references)
		//IL_184a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1faa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_22fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_22fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_2302: Unknown result type (might be due to invalid IL or missing references)
		//IL_230b: Unknown result type (might be due to invalid IL or missing references)
		//IL_230d: Unknown result type (might be due to invalid IL or missing references)
		//IL_230f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2314: Unknown result type (might be due to invalid IL or missing references)
		//IL_2319: Unknown result type (might be due to invalid IL or missing references)
		//IL_185f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1861: Unknown result type (might be due to invalid IL or missing references)
		//IL_1868: Unknown result type (might be due to invalid IL or missing references)
		//IL_186a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1871: Unknown result type (might be due to invalid IL or missing references)
		//IL_1876: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_204e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2050: Unknown result type (might be due to invalid IL or missing references)
		//IL_2055: Unknown result type (might be due to invalid IL or missing references)
		//IL_2057: Unknown result type (might be due to invalid IL or missing references)
		//IL_205c: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_18da: Unknown result type (might be due to invalid IL or missing references)
		//IL_18df: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1906: Unknown result type (might be due to invalid IL or missing references)
		//IL_190d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1914: Unknown result type (might be due to invalid IL or missing references)
		//IL_1920: Unknown result type (might be due to invalid IL or missing references)
		//IL_1925: Unknown result type (might be due to invalid IL or missing references)
		//IL_192a: Unknown result type (might be due to invalid IL or missing references)
		//IL_192f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1934: Unknown result type (might be due to invalid IL or missing references)
		//IL_1938: Unknown result type (might be due to invalid IL or missing references)
		//IL_1944: Unknown result type (might be due to invalid IL or missing references)
		//IL_1949: Unknown result type (might be due to invalid IL or missing references)
		//IL_194e: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1414: Unknown result type (might be due to invalid IL or missing references)
		//IL_141d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1422: Unknown result type (might be due to invalid IL or missing references)
		//IL_1427: Unknown result type (might be due to invalid IL or missing references)
		//IL_142c: Unknown result type (might be due to invalid IL or missing references)
		//IL_142e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1437: Unknown result type (might be due to invalid IL or missing references)
		//IL_143c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1441: Unknown result type (might be due to invalid IL or missing references)
		//IL_1453: Unknown result type (might be due to invalid IL or missing references)
		//IL_145f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1464: Unknown result type (might be due to invalid IL or missing references)
		//IL_1469: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e40: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e42: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_14bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_14cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_14cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_14fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_14fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1500: Unknown result type (might be due to invalid IL or missing references)
		//IL_1502: Unknown result type (might be due to invalid IL or missing references)
		//IL_1507: Unknown result type (might be due to invalid IL or missing references)
		//IL_151b: Unknown result type (might be due to invalid IL or missing references)
		//IL_151d: Unknown result type (might be due to invalid IL or missing references)
		//IL_151f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1524: Unknown result type (might be due to invalid IL or missing references)
		//IL_1526: Unknown result type (might be due to invalid IL or missing references)
		//IL_1528: Unknown result type (might be due to invalid IL or missing references)
		//IL_152a: Unknown result type (might be due to invalid IL or missing references)
		//IL_152f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1543: Unknown result type (might be due to invalid IL or missing references)
		//IL_1545: Unknown result type (might be due to invalid IL or missing references)
		//IL_1475: Unknown result type (might be due to invalid IL or missing references)
		//IL_1477: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e57: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e59: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e62: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e69: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ea1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ea6: Unknown result type (might be due to invalid IL or missing references)
		//IL_148b: Unknown result type (might be due to invalid IL or missing references)
		//IL_148d: Unknown result type (might be due to invalid IL or missing references)
		//IL_148f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1494: Unknown result type (might be due to invalid IL or missing references)
		//IL_1499: Unknown result type (might be due to invalid IL or missing references)
		//IL_149b: Unknown result type (might be due to invalid IL or missing references)
		//IL_149d: Unknown result type (might be due to invalid IL or missing references)
		//IL_149f: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_14aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_14af: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_20e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_20e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_20ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_20ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_158c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1598: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_210b: Unknown result type (might be due to invalid IL or missing references)
		//IL_210d: Unknown result type (might be due to invalid IL or missing references)
		//IL_210f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2114: Unknown result type (might be due to invalid IL or missing references)
		//IL_2119: Unknown result type (might be due to invalid IL or missing references)
		//IL_15af: Unknown result type (might be due to invalid IL or missing references)
		//IL_19be: Unknown result type (might be due to invalid IL or missing references)
		//IL_212b: Unknown result type (might be due to invalid IL or missing references)
		//IL_212d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2132: Unknown result type (might be due to invalid IL or missing references)
		//IL_15db: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a15: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a34: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a57: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a61: Unknown result type (might be due to invalid IL or missing references)
		//IL_2442: Unknown result type (might be due to invalid IL or missing references)
		//IL_2444: Unknown result type (might be due to invalid IL or missing references)
		//IL_245a: Unknown result type (might be due to invalid IL or missing references)
		//IL_245f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2464: Unknown result type (might be due to invalid IL or missing references)
		//IL_246d: Unknown result type (might be due to invalid IL or missing references)
		//IL_246f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2471: Unknown result type (might be due to invalid IL or missing references)
		//IL_2476: Unknown result type (might be due to invalid IL or missing references)
		//IL_247b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2144: Unknown result type (might be due to invalid IL or missing references)
		//IL_2146: Unknown result type (might be due to invalid IL or missing references)
		//IL_214b: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aab: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1abe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ada: Unknown result type (might be due to invalid IL or missing references)
		//IL_1adf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aeb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aed: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b13: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b15: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b17: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b20: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b22: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b27: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_255a: Unknown result type (might be due to invalid IL or missing references)
		//IL_255c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2572: Unknown result type (might be due to invalid IL or missing references)
		//IL_2577: Unknown result type (might be due to invalid IL or missing references)
		//IL_257c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2585: Unknown result type (might be due to invalid IL or missing references)
		//IL_2587: Unknown result type (might be due to invalid IL or missing references)
		//IL_2589: Unknown result type (might be due to invalid IL or missing references)
		//IL_258e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2593: Unknown result type (might be due to invalid IL or missing references)
		//IL_215d: Unknown result type (might be due to invalid IL or missing references)
		//IL_215f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2164: Unknown result type (might be due to invalid IL or missing references)
		//IL_162a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1636: Unknown result type (might be due to invalid IL or missing references)
		//IL_1611: Unknown result type (might be due to invalid IL or missing references)
		//IL_1618: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a83: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a87: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a91: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a93: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a97: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2182: Unknown result type (might be due to invalid IL or missing references)
		//IL_2184: Unknown result type (might be due to invalid IL or missing references)
		//IL_164d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b84: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b90: Unknown result type (might be due to invalid IL or missing references)
		//IL_21a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_218f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2198: Unknown result type (might be due to invalid IL or missing references)
		//IL_219d: Unknown result type (might be due to invalid IL or missing references)
		//IL_21a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1660: Unknown result type (might be due to invalid IL or missing references)
		//IL_1667: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba7: Unknown result type (might be due to invalid IL or missing references)
		//IL_21b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1718: Unknown result type (might be due to invalid IL or missing references)
		//IL_1680: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bdf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_21cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_21d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_21d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_21da: Unknown result type (might be due to invalid IL or missing references)
		//IL_1726: Unknown result type (might be due to invalid IL or missing references)
		//IL_1691: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c22: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c10: Unknown result type (might be due to invalid IL or missing references)
		//IL_176a: Unknown result type (might be due to invalid IL or missing references)
		//IL_173b: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c45: Unknown result type (might be due to invalid IL or missing references)
		//IL_1827: Unknown result type (might be due to invalid IL or missing references)
		//IL_1829: Unknown result type (might be due to invalid IL or missing references)
		//IL_182f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1831: Unknown result type (might be due to invalid IL or missing references)
		//IL_1833: Unknown result type (might be due to invalid IL or missing references)
		//IL_1835: Unknown result type (might be due to invalid IL or missing references)
		//IL_17bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_178d: Unknown result type (might be due to invalid IL or missing references)
		//IL_16de: Unknown result type (might be due to invalid IL or missing references)
		//IL_16bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c58: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_17dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d10: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c78: Unknown result type (might be due to invalid IL or missing references)
		//IL_1813: Unknown result type (might be due to invalid IL or missing references)
		//IL_1815: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c89: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d62: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ca0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e21: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e27: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e29: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d85: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ce9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e0d: Unknown result type (might be due to invalid IL or missing references)
		if (World.Cached)
		{
			TerrainMeta.Path.DungeonGridRoot = HierarchyUtil.GetRoot("Dungeon");
		}
		else if (World.Networked)
		{
			World.Spawn("Dungeon");
			TerrainMeta.Path.DungeonGridRoot = HierarchyUtil.GetRoot("Dungeon");
		}
		else
		{
			if (ConnectionType == InfrastructureType.Tunnel && !World.Config.BelowGroundRails)
			{
				return;
			}
			Prefab<DungeonGridCell>[] array = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + TunnelFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true, useWorldConfig: false);
			if (array == null || array.Length == 0)
			{
				return;
			}
			Prefab<DungeonGridCell>[] array2 = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + StationFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true, useWorldConfig: false);
			if (array2 == null || array2.Length == 0)
			{
				return;
			}
			Prefab<DungeonGridCell>[] array3 = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + UpwardsFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true, useWorldConfig: false);
			if (array3 == null)
			{
				return;
			}
			Prefab<DungeonGridCell>[] array4 = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + TransitionFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true, useWorldConfig: false);
			if (array4 == null)
			{
				return;
			}
			Prefab<DungeonGridLink>[] array5 = Prefab.Load<DungeonGridLink>("assets/bundled/prefabs/autospawn/" + LinkFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true, useWorldConfig: false);
			if (array5 == null)
			{
				return;
			}
			array5 = array5.OrderByDescending((Prefab<DungeonGridLink> x) => x.Component.Priority).ToArray();
			List<DungeonGridInfo> list = (Object.op_Implicit((Object)(object)TerrainMeta.Path) ? TerrainMeta.Path.DungeonGridEntrances : null);
			WorldSpaceGrid<Prefab<DungeonGridCell>> val = new WorldSpaceGrid<Prefab<DungeonGridCell>>(TerrainMeta.Size.x * 2f, (float)CellSize);
			int[,] array6 = new int[val.CellCount, val.CellCount];
			DungeonGridConnectionHash[,] hashmap = new DungeonGridConnectionHash[val.CellCount, val.CellCount];
			PathFinder pathFinder = new PathFinder(array6, diagonals: false);
			int cellCount = val.CellCount;
			int num = 0;
			int num2 = val.CellCount - 1;
			for (int i = 0; i < cellCount; i++)
			{
				for (int j = 0; j < cellCount; j++)
				{
					array6[j, i] = 1;
				}
			}
			List<PathSegment> list2 = new List<PathSegment>();
			List<PathLink> list3 = new List<PathLink>();
			List<PathNode> list4 = new List<PathNode>();
			List<PathNode> unconnectedNodeList = new List<PathNode>();
			List<PathNode> secondaryNodeList = new List<PathNode>();
			List<PathFinder.Point> list5 = new List<PathFinder.Point>();
			List<PathFinder.Point> list6 = new List<PathFinder.Point>();
			List<PathFinder.Point> list7 = new List<PathFinder.Point>();
			foreach (DungeonGridInfo item3 in list)
			{
				DungeonGridInfo entrance = item3;
				TerrainPathConnect[] componentsInChildren = ((Component)entrance).GetComponentsInChildren<TerrainPathConnect>(true);
				foreach (TerrainPathConnect terrainPathConnect in componentsInChildren)
				{
					if (terrainPathConnect.Type != ConnectionType)
					{
						continue;
					}
					Vector2i val2 = val.WorldToGridCoords(((Component)terrainPathConnect).transform.position);
					if (array6[val2.x, val2.y] == int.MaxValue)
					{
						continue;
					}
					PathFinder.Node stationNode = pathFinder.FindClosestWalkable(new PathFinder.Point(val2.x, val2.y), 1);
					if (stationNode == null)
					{
						continue;
					}
					Prefab<DungeonGridCell> prefab = ((val2.x > num) ? val[val2.x - 1, val2.y] : null);
					Prefab<DungeonGridCell> prefab2 = ((val2.x < num2) ? val[val2.x + 1, val2.y] : null);
					Prefab<DungeonGridCell> prefab3 = ((val2.y > num) ? val[val2.x, val2.y - 1] : null);
					Prefab<DungeonGridCell> prefab4 = ((val2.y < num2) ? val[val2.x, val2.y + 1] : null);
					Prefab<DungeonGridCell> prefab5 = null;
					float num3 = float.MaxValue;
					array2.Shuffle(ref seed);
					Prefab<DungeonGridCell>[] array7 = array2;
					foreach (Prefab<DungeonGridCell> prefab6 in array7)
					{
						if ((prefab != null && prefab6.Component.West != prefab.Component.East) || (prefab2 != null && prefab6.Component.East != prefab2.Component.West) || (prefab3 != null && prefab6.Component.South != prefab3.Component.North) || (prefab4 != null && prefab6.Component.North != prefab4.Component.South))
						{
							continue;
						}
						DungeonVolume componentInChildren = prefab6.Object.GetComponentInChildren<DungeonVolume>();
						DungeonVolume componentInChildren2 = ((Component)entrance).GetComponentInChildren<DungeonVolume>();
						OBB bounds = componentInChildren.GetBounds(val.GridToWorldCoords(val2), Quaternion.identity);
						OBB bounds2 = componentInChildren2.GetBounds(((Component)entrance).transform.position, Quaternion.identity);
						if (!((OBB)(ref bounds)).Intersects2D(bounds2))
						{
							DungeonGridLink componentInChildren3 = prefab6.Object.GetComponentInChildren<DungeonGridLink>();
							Vector3 val3 = val.GridToWorldCoords(new Vector2i(val2.x, val2.y)) + componentInChildren3.UpSocket.localPosition;
							float num4 = Vector3Ex.Magnitude2D(((Component)terrainPathConnect).transform.position - val3);
							if (!(num3 < num4))
							{
								prefab5 = prefab6;
								num3 = num4;
							}
						}
					}
					bool isStartPoint;
					if (prefab5 != null)
					{
						val[val2.x, val2.y] = prefab5;
						array6[val2.x, val2.y] = int.MaxValue;
						isStartPoint = secondaryNodeList.Count == 0;
						secondaryNodeList.RemoveAll((PathNode x) => x.node.point == stationNode.point);
						unconnectedNodeList.RemoveAll((PathNode x) => x.node.point == stationNode.point);
						if (prefab5.Component.West != 0)
						{
							AddNode(val2.x - 1, val2.y);
						}
						if (prefab5.Component.East != 0)
						{
							AddNode(val2.x + 1, val2.y);
						}
						if (prefab5.Component.South != 0)
						{
							AddNode(val2.x, val2.y - 1);
						}
						if (prefab5.Component.North != 0)
						{
							AddNode(val2.x, val2.y + 1);
						}
						PathLink pathLink = new PathLink();
						DungeonGridLink componentInChildren4 = ((Component)entrance).gameObject.GetComponentInChildren<DungeonGridLink>();
						Vector3 position = ((Component)entrance).transform.position;
						Quaternion rotation = ((Component)entrance).transform.rotation;
						Vector3 eulerAngles = ((Quaternion)(ref rotation)).eulerAngles;
						DungeonGridLink componentInChildren5 = prefab5.Object.GetComponentInChildren<DungeonGridLink>();
						Vector3 position2 = val.GridToWorldCoords(new Vector2i(val2.x, val2.y));
						Vector3 zero = Vector3.zero;
						pathLink.downwards = new PathLinkSide();
						pathLink.downwards.origin = new PathLinkSegment();
						pathLink.downwards.origin.position = position;
						pathLink.downwards.origin.rotation = Quaternion.Euler(eulerAngles);
						pathLink.downwards.origin.scale = Vector3.one;
						pathLink.downwards.origin.link = componentInChildren4;
						pathLink.downwards.segments = new List<PathLinkSegment>();
						pathLink.upwards = new PathLinkSide();
						pathLink.upwards.origin = new PathLinkSegment();
						pathLink.upwards.origin.position = position2;
						pathLink.upwards.origin.rotation = Quaternion.Euler(zero);
						pathLink.upwards.origin.scale = Vector3.one;
						pathLink.upwards.origin.link = componentInChildren5;
						pathLink.upwards.segments = new List<PathLinkSegment>();
						list3.Add(pathLink);
					}
					void AddNode(int x, int y)
					{
						//IL_0059: Unknown result type (might be due to invalid IL or missing references)
						PathFinder.Node node8 = pathFinder.FindClosestWalkable(new PathFinder.Point(x, y), 1);
						if (node8 != null)
						{
							PathNode item2 = new PathNode
							{
								monument = (Object.op_Implicit((Object)(object)TerrainMeta.Path) ? TerrainMeta.Path.FindClosest(TerrainMeta.Path.Monuments, ((Component)entrance).transform.position) : ((Component)((Component)entrance).transform).GetComponentInParent<MonumentInfo>()),
								node = node8
							};
							if (isStartPoint)
							{
								secondaryNodeList.Add(item2);
							}
							else
							{
								unconnectedNodeList.Add(item2);
							}
							DungeonGridConnectionHash dungeonGridConnectionHash4 = hashmap[node8.point.x, node8.point.y];
							DungeonGridConnectionHash dungeonGridConnectionHash5 = hashmap[stationNode.point.x, stationNode.point.y];
							if (node8.point.x > stationNode.point.x)
							{
								dungeonGridConnectionHash4.West = true;
								dungeonGridConnectionHash5.East = true;
							}
							if (node8.point.x < stationNode.point.x)
							{
								dungeonGridConnectionHash4.East = true;
								dungeonGridConnectionHash5.West = true;
							}
							if (node8.point.y > stationNode.point.y)
							{
								dungeonGridConnectionHash4.South = true;
								dungeonGridConnectionHash5.North = true;
							}
							if (node8.point.y < stationNode.point.y)
							{
								dungeonGridConnectionHash4.North = true;
								dungeonGridConnectionHash5.South = true;
							}
							hashmap[node8.point.x, node8.point.y] = dungeonGridConnectionHash4;
							hashmap[stationNode.point.x, stationNode.point.y] = dungeonGridConnectionHash5;
						}
					}
				}
			}
			while (unconnectedNodeList.Count != 0 || secondaryNodeList.Count != 0)
			{
				if (unconnectedNodeList.Count == 0)
				{
					PathNode node3 = secondaryNodeList[0];
					unconnectedNodeList.AddRange(secondaryNodeList.Where((PathNode x) => (Object)(object)x.monument == (Object)(object)node3.monument));
					secondaryNodeList.RemoveAll((PathNode x) => (Object)(object)x.monument == (Object)(object)node3.monument);
					Vector2i val4 = val.WorldToGridCoords(((Component)node3.monument).transform.position);
					pathFinder.PushPoint = new PathFinder.Point(val4.x, val4.y);
					pathFinder.PushRadius = (pathFinder.PushDistance = 2);
					pathFinder.PushMultiplier = 16;
				}
				list7.Clear();
				list7.AddRange(unconnectedNodeList.Select((PathNode x) => x.node.point));
				list6.Clear();
				list6.AddRange(list4.Select((PathNode x) => x.node.point));
				list6.AddRange(secondaryNodeList.Select((PathNode x) => x.node.point));
				list6.AddRange(list5);
				PathFinder.Node node4 = pathFinder.FindPathUndirected(list6, list7, 100000);
				if (node4 == null)
				{
					PathNode node2 = unconnectedNodeList[0];
					secondaryNodeList.AddRange(unconnectedNodeList.Where((PathNode x) => (Object)(object)x.monument == (Object)(object)node2.monument));
					unconnectedNodeList.RemoveAll((PathNode x) => (Object)(object)x.monument == (Object)(object)node2.monument);
					secondaryNodeList.Remove(node2);
					list4.Add(node2);
					continue;
				}
				PathSegment segment = new PathSegment();
				for (PathFinder.Node node5 = node4; node5 != null; node5 = node5.next)
				{
					if (node5 == node4)
					{
						segment.start = node5;
					}
					if (node5.next == null)
					{
						segment.end = node5;
					}
				}
				list2.Add(segment);
				PathNode node = unconnectedNodeList.Find((PathNode x) => x.node.point == segment.start.point || x.node.point == segment.end.point);
				secondaryNodeList.AddRange(unconnectedNodeList.Where((PathNode x) => (Object)(object)x.monument == (Object)(object)node.monument));
				unconnectedNodeList.RemoveAll((PathNode x) => (Object)(object)x.monument == (Object)(object)node.monument);
				secondaryNodeList.Remove(node);
				list4.Add(node);
				PathNode pathNode = secondaryNodeList.Find((PathNode x) => x.node.point == segment.start.point || x.node.point == segment.end.point);
				if (pathNode != null)
				{
					secondaryNodeList.Remove(pathNode);
					list4.Add(pathNode);
				}
				for (PathFinder.Node node6 = node4; node6 != null; node6 = node6.next)
				{
					if (node6 != node4 && node6.next != null)
					{
						list5.Add(node6.point);
					}
				}
			}
			foreach (PathSegment item4 in list2)
			{
				PathFinder.Node node7 = item4.start;
				while (node7 != null && node7.next != null)
				{
					DungeonGridConnectionHash dungeonGridConnectionHash = hashmap[node7.point.x, node7.point.y];
					DungeonGridConnectionHash dungeonGridConnectionHash2 = hashmap[node7.next.point.x, node7.next.point.y];
					if (node7.point.x > node7.next.point.x)
					{
						dungeonGridConnectionHash.West = true;
						dungeonGridConnectionHash2.East = true;
					}
					if (node7.point.x < node7.next.point.x)
					{
						dungeonGridConnectionHash.East = true;
						dungeonGridConnectionHash2.West = true;
					}
					if (node7.point.y > node7.next.point.y)
					{
						dungeonGridConnectionHash.South = true;
						dungeonGridConnectionHash2.North = true;
					}
					if (node7.point.y < node7.next.point.y)
					{
						dungeonGridConnectionHash.North = true;
						dungeonGridConnectionHash2.South = true;
					}
					hashmap[node7.point.x, node7.point.y] = dungeonGridConnectionHash;
					hashmap[node7.next.point.x, node7.next.point.y] = dungeonGridConnectionHash2;
					node7 = node7.next;
				}
			}
			for (int m = 0; m < val.CellCount; m++)
			{
				for (int n = 0; n < val.CellCount; n++)
				{
					if (array6[m, n] == int.MaxValue)
					{
						continue;
					}
					DungeonGridConnectionHash dungeonGridConnectionHash3 = hashmap[m, n];
					if (dungeonGridConnectionHash3.Value == 0)
					{
						continue;
					}
					array.Shuffle(ref seed);
					Prefab<DungeonGridCell>[] array7 = array;
					foreach (Prefab<DungeonGridCell> prefab7 in array7)
					{
						Prefab<DungeonGridCell> prefab8 = ((m > num) ? val[m - 1, n] : null);
						if (((prefab8 != null) ? ((prefab7.Component.West == prefab8.Component.East) ? 1 : 0) : (dungeonGridConnectionHash3.West ? ((int)prefab7.Component.West) : ((prefab7.Component.West == DungeonGridConnectionType.None) ? 1 : 0))) == 0)
						{
							continue;
						}
						Prefab<DungeonGridCell> prefab9 = ((m < num2) ? val[m + 1, n] : null);
						if (((prefab9 != null) ? ((prefab7.Component.East == prefab9.Component.West) ? 1 : 0) : (dungeonGridConnectionHash3.East ? ((int)prefab7.Component.East) : ((prefab7.Component.East == DungeonGridConnectionType.None) ? 1 : 0))) == 0)
						{
							continue;
						}
						Prefab<DungeonGridCell> prefab10 = ((n > num) ? val[m, n - 1] : null);
						if (((prefab10 != null) ? ((prefab7.Component.South == prefab10.Component.North) ? 1 : 0) : (dungeonGridConnectionHash3.South ? ((int)prefab7.Component.South) : ((prefab7.Component.South == DungeonGridConnectionType.None) ? 1 : 0))) == 0)
						{
							continue;
						}
						Prefab<DungeonGridCell> prefab11 = ((n < num2) ? val[m, n + 1] : null);
						if (((prefab11 != null) ? (prefab7.Component.North == prefab11.Component.South) : (dungeonGridConnectionHash3.North ? ((byte)prefab7.Component.North != 0) : (prefab7.Component.North == DungeonGridConnectionType.None))) && (prefab7.Component.West == DungeonGridConnectionType.None || prefab8 == null || !prefab7.Component.ShouldAvoid(prefab8.ID)) && (prefab7.Component.East == DungeonGridConnectionType.None || prefab9 == null || !prefab7.Component.ShouldAvoid(prefab9.ID)) && (prefab7.Component.South == DungeonGridConnectionType.None || prefab10 == null || !prefab7.Component.ShouldAvoid(prefab10.ID)) && (prefab7.Component.North == DungeonGridConnectionType.None || prefab11 == null || !prefab7.Component.ShouldAvoid(prefab11.ID)))
						{
							val[m, n] = prefab7;
							bool num5 = prefab8 == null || prefab7.Component.WestVariant == prefab8.Component.EastVariant;
							bool flag = prefab10 == null || prefab7.Component.SouthVariant == prefab10.Component.NorthVariant;
							if (num5 && flag)
							{
								break;
							}
						}
					}
				}
			}
			Vector3 zero2 = Vector3.zero;
			Vector3 zero3 = Vector3.zero;
			Vector3 val5 = Vector3.up * 10f;
			Vector3 val6 = Vector3.up * (LinkTransition + 1f);
			Vector2i val7 = default(Vector2i);
			do
			{
				zero3 = zero2;
				for (int num6 = 0; num6 < val.CellCount; num6++)
				{
					for (int num7 = 0; num7 < val.CellCount; num7++)
					{
						Prefab<DungeonGridCell> prefab12 = val[num6, num7];
						if (prefab12 != null)
						{
							((Vector2i)(ref val7))._002Ector(num6, num7);
							Vector3 val8 = val.GridToWorldCoords(val7);
							while (!prefab12.CheckEnvironmentVolumesInsideTerrain(zero2 + val8 + val5, Quaternion.identity, Vector3.one, EnvironmentType.Underground) || prefab12.CheckEnvironmentVolumes(zero2 + val8 + val6, Quaternion.identity, Vector3.one, EnvironmentType.Underground | EnvironmentType.Building) || prefab12.CheckEnvironmentVolumes(zero2 + val8, Quaternion.identity, Vector3.one, EnvironmentType.Underground | EnvironmentType.Building))
							{
								zero2.y -= 9f;
							}
						}
					}
				}
			}
			while (zero2 != zero3);
			foreach (PathLink item5 in list3)
			{
				PathLinkSegment origin = item5.upwards.origin;
				origin.position += zero2;
			}
			foreach (PathLink item6 in list3)
			{
				Vector3 val9 = item6.upwards.origin.position + item6.upwards.origin.rotation * Vector3.Scale(item6.upwards.origin.upSocket.localPosition, item6.upwards.origin.scale);
				Vector3 val10 = item6.downwards.origin.position + item6.downwards.origin.rotation * Vector3.Scale(item6.downwards.origin.downSocket.localPosition, item6.downwards.origin.scale) - val9;
				Vector3[] array8 = (Vector3[])(object)new Vector3[2]
				{
					new Vector3(0f, 1f, 0f),
					new Vector3(1f, 1f, 1f)
				};
				foreach (Vector3 val11 in array8)
				{
					int num8 = 0;
					int num9 = 0;
					while (((Vector3)(ref val10)).magnitude > 1f && (num8 < 8 || num9 < 8))
					{
						bool flag2 = num8 > 2 && num9 > 2;
						bool flag3 = num8 > 4 && num9 > 4;
						Prefab<DungeonGridLink> prefab13 = null;
						Vector3 val12 = Vector3.zero;
						int num10 = int.MinValue;
						Vector3 position3 = Vector3.zero;
						Quaternion rotation2 = Quaternion.identity;
						PathLinkSegment prevSegment = item6.downwards.prevSegment;
						Vector3 val13 = prevSegment.position + prevSegment.rotation * Vector3.Scale(prevSegment.scale, prevSegment.downSocket.localPosition);
						Quaternion val14 = prevSegment.rotation * prevSegment.downSocket.localRotation;
						Prefab<DungeonGridLink>[] array9 = array5;
						foreach (Prefab<DungeonGridLink> prefab14 in array9)
						{
							float num11 = SeedRandom.Value(ref seed);
							DungeonGridLink component = prefab14.Component;
							if (prevSegment.downType != component.UpType)
							{
								continue;
							}
							switch (component.DownType)
							{
							case DungeonGridLinkType.Elevator:
								if (flag2 || val11.x != 0f || val11.z != 0f)
								{
									continue;
								}
								break;
							case DungeonGridLinkType.Transition:
								if (val11.x != 0f || val11.z != 0f)
								{
									continue;
								}
								break;
							}
							int num12 = ((!flag2) ? component.Priority : 0);
							if (num10 > num12)
							{
								continue;
							}
							Quaternion val15 = val14 * Quaternion.Inverse(component.UpSocket.localRotation);
							Quaternion val16 = val15 * component.DownSocket.localRotation;
							PathLinkSegment prevSegment2 = item6.upwards.prevSegment;
							Quaternion val17 = prevSegment2.rotation * prevSegment2.upSocket.localRotation;
							if (component.Rotation > 0)
							{
								if (Quaternion.Angle(val17, val16) > (float)component.Rotation)
								{
									continue;
								}
								Quaternion val18 = val17 * Quaternion.Inverse(val16);
								val15 *= val18;
								val16 *= val18;
							}
							Vector3 val19 = val13 - val15 * component.UpSocket.localPosition;
							Vector3 val20 = val15 * (component.DownSocket.localPosition - component.UpSocket.localPosition);
							Vector3 val21 = val10 + val12;
							Vector3 val22 = val10 + val20;
							float magnitude = ((Vector3)(ref val21)).magnitude;
							float magnitude2 = ((Vector3)(ref val22)).magnitude;
							Vector3 val23 = Vector3.Scale(val21, val11);
							Vector3 val24 = Vector3.Scale(val22, val11);
							float magnitude3 = ((Vector3)(ref val23)).magnitude;
							float magnitude4 = ((Vector3)(ref val24)).magnitude;
							if (val12 != Vector3.zero)
							{
								if (magnitude3 < magnitude4 || (magnitude3 == magnitude4 && magnitude < magnitude2) || (magnitude3 == magnitude4 && magnitude == magnitude2 && num11 < 0.5f))
								{
									continue;
								}
							}
							else if (magnitude3 <= magnitude4)
							{
								continue;
							}
							if (Mathf.Abs(val24.x) - Mathf.Abs(val23.x) > 0.01f || (Mathf.Abs(val24.x) > 0.01f && val21.x * val22.x < 0f) || Mathf.Abs(val24.y) - Mathf.Abs(val23.y) > 0.01f || (Mathf.Abs(val24.y) > 0.01f && val21.y * val22.y < 0f) || Mathf.Abs(val24.z) - Mathf.Abs(val23.z) > 0.01f || (Mathf.Abs(val24.z) > 0.01f && val21.z * val22.z < 0f) || (flag2 && val11.x == 0f && val11.z == 0f && component.DownType == DungeonGridLinkType.Default && ((Mathf.Abs(val22.x) > 0.01f && Mathf.Abs(val22.x) < LinkRadius * 2f - 0.1f) || (Mathf.Abs(val22.z) > 0.01f && Mathf.Abs(val22.z) < LinkRadius * 2f - 0.1f))))
							{
								continue;
							}
							num10 = num12;
							if (val11.x == 0f && val11.z == 0f)
							{
								if (!flag2 && Mathf.Abs(val22.y) < LinkTransition - 0.1f)
								{
									continue;
								}
							}
							else if ((!flag2 && magnitude4 > 0.01f && (Mathf.Abs(val22.x) < LinkRadius * 2f - 0.1f || Mathf.Abs(val22.z) < LinkRadius * 2f - 0.1f)) || (!flag3 && magnitude4 > 0.01f && (Mathf.Abs(val22.x) < LinkRadius * 1f - 0.1f || Mathf.Abs(val22.z) < LinkRadius * 1f - 0.1f)))
							{
								continue;
							}
							if (!flag2 || !(magnitude4 < 0.01f) || !(magnitude2 < 0.01f) || !(Quaternion.Angle(val17, val16) > 10f))
							{
								prefab13 = prefab14;
								val12 = val20;
								num10 = num12;
								position3 = val19;
								rotation2 = val15;
							}
						}
						if (val12 != Vector3.zero)
						{
							PathLinkSegment pathLinkSegment = new PathLinkSegment();
							pathLinkSegment.position = position3;
							pathLinkSegment.rotation = rotation2;
							pathLinkSegment.scale = Vector3.one;
							pathLinkSegment.prefab = prefab13;
							pathLinkSegment.link = prefab13.Component;
							item6.downwards.segments.Add(pathLinkSegment);
							val10 += val12;
						}
						else
						{
							num9++;
						}
						if (val11.x > 0f || val11.z > 0f)
						{
							Prefab<DungeonGridLink> prefab15 = null;
							Vector3 val25 = Vector3.zero;
							int num13 = int.MinValue;
							Vector3 position4 = Vector3.zero;
							Quaternion rotation3 = Quaternion.identity;
							PathLinkSegment prevSegment3 = item6.upwards.prevSegment;
							Vector3 val26 = prevSegment3.position + prevSegment3.rotation * Vector3.Scale(prevSegment3.scale, prevSegment3.upSocket.localPosition);
							Quaternion val27 = prevSegment3.rotation * prevSegment3.upSocket.localRotation;
							array9 = array5;
							foreach (Prefab<DungeonGridLink> prefab16 in array9)
							{
								float num14 = SeedRandom.Value(ref seed);
								DungeonGridLink component2 = prefab16.Component;
								if (prevSegment3.upType != component2.DownType)
								{
									continue;
								}
								switch (component2.DownType)
								{
								case DungeonGridLinkType.Elevator:
									if (flag2 || val11.x != 0f || val11.z != 0f)
									{
										continue;
									}
									break;
								case DungeonGridLinkType.Transition:
									if (val11.x != 0f || val11.z != 0f)
									{
										continue;
									}
									break;
								}
								int num15 = ((!flag2) ? component2.Priority : 0);
								if (num13 > num15)
								{
									continue;
								}
								Quaternion val28 = val27 * Quaternion.Inverse(component2.DownSocket.localRotation);
								Quaternion val29 = val28 * component2.UpSocket.localRotation;
								PathLinkSegment prevSegment4 = item6.downwards.prevSegment;
								Quaternion val30 = prevSegment4.rotation * prevSegment4.downSocket.localRotation;
								if (component2.Rotation > 0)
								{
									if (Quaternion.Angle(val30, val29) > (float)component2.Rotation)
									{
										continue;
									}
									Quaternion val31 = val30 * Quaternion.Inverse(val29);
									val28 *= val31;
									val29 *= val31;
								}
								Vector3 val32 = val26 - val28 * component2.DownSocket.localPosition;
								Vector3 val33 = val28 * (component2.UpSocket.localPosition - component2.DownSocket.localPosition);
								Vector3 val34 = val10 - val25;
								Vector3 val35 = val10 - val33;
								float magnitude5 = ((Vector3)(ref val34)).magnitude;
								float magnitude6 = ((Vector3)(ref val35)).magnitude;
								Vector3 val36 = Vector3.Scale(val34, val11);
								Vector3 val37 = Vector3.Scale(val35, val11);
								float magnitude7 = ((Vector3)(ref val36)).magnitude;
								float magnitude8 = ((Vector3)(ref val37)).magnitude;
								if (val25 != Vector3.zero)
								{
									if (magnitude7 < magnitude8 || (magnitude7 == magnitude8 && magnitude5 < magnitude6) || (magnitude7 == magnitude8 && magnitude5 == magnitude6 && num14 < 0.5f))
									{
										continue;
									}
								}
								else if (magnitude7 <= magnitude8)
								{
									continue;
								}
								if (Mathf.Abs(val37.x) - Mathf.Abs(val36.x) > 0.01f || (Mathf.Abs(val37.x) > 0.01f && val34.x * val35.x < 0f) || Mathf.Abs(val37.y) - Mathf.Abs(val36.y) > 0.01f || (Mathf.Abs(val37.y) > 0.01f && val34.y * val35.y < 0f) || Mathf.Abs(val37.z) - Mathf.Abs(val36.z) > 0.01f || (Mathf.Abs(val37.z) > 0.01f && val34.z * val35.z < 0f) || (flag2 && val11.x == 0f && val11.z == 0f && component2.UpType == DungeonGridLinkType.Default && ((Mathf.Abs(val35.x) > 0.01f && Mathf.Abs(val35.x) < LinkRadius * 2f - 0.1f) || (Mathf.Abs(val35.z) > 0.01f && Mathf.Abs(val35.z) < LinkRadius * 2f - 0.1f))))
								{
									continue;
								}
								num13 = num15;
								if (val11.x == 0f && val11.z == 0f)
								{
									if (!flag2 && Mathf.Abs(val35.y) < LinkTransition - 0.1f)
									{
										continue;
									}
								}
								else if ((!flag2 && magnitude8 > 0.01f && (Mathf.Abs(val35.x) < LinkRadius * 2f - 0.1f || Mathf.Abs(val35.z) < LinkRadius * 2f - 0.1f)) || (!flag3 && magnitude8 > 0.01f && (Mathf.Abs(val35.x) < LinkRadius * 1f - 0.1f || Mathf.Abs(val35.z) < LinkRadius * 1f - 0.1f)))
								{
									continue;
								}
								if (!flag2 || !(magnitude8 < 0.01f) || !(magnitude6 < 0.01f) || !(Quaternion.Angle(val30, val29) > 10f))
								{
									prefab15 = prefab16;
									val25 = val33;
									num13 = num15;
									position4 = val32;
									rotation3 = val28;
								}
							}
							if (val25 != Vector3.zero)
							{
								PathLinkSegment pathLinkSegment2 = new PathLinkSegment();
								pathLinkSegment2.position = position4;
								pathLinkSegment2.rotation = rotation3;
								pathLinkSegment2.scale = Vector3.one;
								pathLinkSegment2.prefab = prefab15;
								pathLinkSegment2.link = prefab15.Component;
								item6.upwards.segments.Add(pathLinkSegment2);
								val10 -= val25;
							}
							else
							{
								num8++;
							}
						}
						else
						{
							num8++;
						}
					}
				}
			}
			foreach (PathLink item7 in list3)
			{
				foreach (PathLinkSegment segment2 in item7.downwards.segments)
				{
					World.AddPrefab("Dungeon", segment2.prefab, segment2.position, segment2.rotation, segment2.scale);
				}
				foreach (PathLinkSegment segment3 in item7.upwards.segments)
				{
					World.AddPrefab("Dungeon", segment3.prefab, segment3.position, segment3.rotation, segment3.scale);
				}
			}
			if (TerrainMeta.Path.Rails.Count > 0)
			{
				List<PrefabReplacement> list8 = new List<PrefabReplacement>();
				Vector2i val38 = default(Vector2i);
				for (int num16 = 0; num16 < val.CellCount; num16++)
				{
					for (int num17 = 0; num17 < val.CellCount; num17++)
					{
						Prefab<DungeonGridCell> prefab17 = val[num16, num17];
						if (prefab17 == null || !prefab17.Component.Replaceable)
						{
							continue;
						}
						((Vector2i)(ref val38))._002Ector(num16, num17);
						Vector3 val39 = val.GridToWorldCoords(val38) + zero2;
						Prefab<DungeonGridCell>[] array7 = array3;
						foreach (Prefab<DungeonGridCell> prefab18 in array7)
						{
							if (prefab17.Component.North != prefab18.Component.North || prefab17.Component.South != prefab18.Component.South || prefab17.Component.West != prefab18.Component.West || prefab17.Component.East != prefab18.Component.East || !prefab18.CheckEnvironmentVolumesInsideTerrain(val39 + val5, Quaternion.identity, Vector3.one, EnvironmentType.Underground) || prefab18.CheckEnvironmentVolumes(val39 + val6, Quaternion.identity, Vector3.one, EnvironmentType.Underground | EnvironmentType.Building) || prefab18.CheckEnvironmentVolumes(val39, Quaternion.identity, Vector3.one, EnvironmentType.Underground | EnvironmentType.Building) || !prefab18.ApplyTerrainChecks(val39, Quaternion.identity, Vector3.one) || !prefab18.ApplyTerrainFilters(val39, Quaternion.identity, Vector3.one))
							{
								continue;
							}
							MonumentInfo componentInChildren6 = prefab18.Object.GetComponentInChildren<MonumentInfo>();
							Vector3 val40 = val39;
							if (Object.op_Implicit((Object)(object)componentInChildren6))
							{
								val40 += ((Component)componentInChildren6).transform.position;
							}
							if (!(val40.y < 1f))
							{
								float distanceToAboveGroundRail = GetDistanceToAboveGroundRail(val40);
								if (!(distanceToAboveGroundRail < 200f))
								{
									PrefabReplacement item = default(PrefabReplacement);
									item.gridPosition = val38;
									item.worldPosition = val40;
									item.distance = distanceToAboveGroundRail;
									item.prefab = prefab18;
									list8.Add(item);
								}
							}
						}
					}
				}
				list8.Sort((PrefabReplacement a, PrefabReplacement b) => a.distance.CompareTo(b.distance));
				int num18 = 2;
				while (num18 > 0 && list8.Count > 0)
				{
					num18--;
					PrefabReplacement replacement = list8[0];
					val[replacement.gridPosition.x, replacement.gridPosition.y] = replacement.prefab;
					list8.RemoveAll(delegate(PrefabReplacement a)
					{
						//IL_0001: Unknown result type (might be due to invalid IL or missing references)
						//IL_000c: Unknown result type (might be due to invalid IL or missing references)
						//IL_0011: Unknown result type (might be due to invalid IL or missing references)
						//IL_0016: Unknown result type (might be due to invalid IL or missing references)
						Vector3 val47 = a.worldPosition - replacement.worldPosition;
						return ((Vector3)(ref val47)).magnitude < 1500f;
					});
				}
			}
			Vector2i val41 = default(Vector2i);
			for (int num19 = 0; num19 < val.CellCount; num19++)
			{
				for (int num20 = 0; num20 < val.CellCount; num20++)
				{
					Prefab<DungeonGridCell> prefab19 = val[num19, num20];
					if (prefab19 != null)
					{
						((Vector2i)(ref val41))._002Ector(num19, num20);
						Vector3 val42 = val.GridToWorldCoords(val41);
						World.AddPrefab("Dungeon", prefab19, zero2 + val42, Quaternion.identity, Vector3.one);
					}
				}
			}
			Vector2i val43 = default(Vector2i);
			Vector2i val45 = default(Vector2i);
			for (int num21 = 0; num21 < val.CellCount - 1; num21++)
			{
				for (int num22 = 0; num22 < val.CellCount - 1; num22++)
				{
					Prefab<DungeonGridCell> prefab20 = val[num21, num22];
					Prefab<DungeonGridCell> prefab21 = val[num21 + 1, num22];
					Prefab<DungeonGridCell> prefab22 = val[num21, num22 + 1];
					Prefab<DungeonGridCell>[] array7;
					if (prefab20 != null && prefab21 != null && prefab20.Component.EastVariant != prefab21.Component.WestVariant)
					{
						array4.Shuffle(ref seed);
						array7 = array4;
						foreach (Prefab<DungeonGridCell> prefab23 in array7)
						{
							if (prefab23.Component.West == prefab20.Component.East && prefab23.Component.East == prefab21.Component.West && prefab23.Component.WestVariant == prefab20.Component.EastVariant && prefab23.Component.EastVariant == prefab21.Component.WestVariant)
							{
								((Vector2i)(ref val43))._002Ector(num21, num22);
								Vector3 val44 = val.GridToWorldCoords(val43) + new Vector3(val.CellSizeHalf, 0f, 0f);
								World.AddPrefab("Dungeon", prefab23, zero2 + val44, Quaternion.identity, Vector3.one);
								break;
							}
						}
					}
					if (prefab20 == null || prefab22 == null || prefab20.Component.NorthVariant == prefab22.Component.SouthVariant)
					{
						continue;
					}
					array4.Shuffle(ref seed);
					array7 = array4;
					foreach (Prefab<DungeonGridCell> prefab24 in array7)
					{
						if (prefab24.Component.South == prefab20.Component.North && prefab24.Component.North == prefab22.Component.South && prefab24.Component.SouthVariant == prefab20.Component.NorthVariant && prefab24.Component.NorthVariant == prefab22.Component.SouthVariant)
						{
							((Vector2i)(ref val45))._002Ector(num21, num22);
							Vector3 val46 = val.GridToWorldCoords(val45) + new Vector3(0f, 0f, val.CellSizeHalf);
							World.AddPrefab("Dungeon", prefab24, zero2 + val46, Quaternion.identity, Vector3.one);
							break;
						}
					}
				}
			}
			if (Object.op_Implicit((Object)(object)TerrainMeta.Path))
			{
				TerrainMeta.Path.DungeonGridRoot = HierarchyUtil.GetRoot("Dungeon");
			}
		}
	}

	private float GetDistanceToAboveGroundRail(Vector3 pos)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		float num = float.MaxValue;
		if (Object.op_Implicit((Object)(object)TerrainMeta.Path))
		{
			foreach (PathList rail in TerrainMeta.Path.Rails)
			{
				Vector3[] points = rail.Path.Points;
				foreach (Vector3 val in points)
				{
					num = Mathf.Min(num, Vector3Ex.Distance2D(val, pos));
				}
			}
		}
		return num;
	}
}
