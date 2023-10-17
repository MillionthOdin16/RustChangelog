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
		//IL_149e: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_14bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_17da: Unknown result type (might be due to invalid IL or missing references)
		//IL_17df: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_17fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_180d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1823: Unknown result type (might be due to invalid IL or missing references)
		//IL_1834: Unknown result type (might be due to invalid IL or missing references)
		//IL_1839: Unknown result type (might be due to invalid IL or missing references)
		//IL_183e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1843: Unknown result type (might be due to invalid IL or missing references)
		//IL_1848: Unknown result type (might be due to invalid IL or missing references)
		//IL_184a: Unknown result type (might be due to invalid IL or missing references)
		//IL_184f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1868: Unknown result type (might be due to invalid IL or missing references)
		//IL_186d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1883: Unknown result type (might be due to invalid IL or missing references)
		//IL_1888: Unknown result type (might be due to invalid IL or missing references)
		//IL_189b: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1289: Unknown result type (might be due to invalid IL or missing references)
		//IL_128b: Unknown result type (might be due to invalid IL or missing references)
		//IL_128d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1292: Unknown result type (might be due to invalid IL or missing references)
		//IL_1297: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2532: Unknown result type (might be due to invalid IL or missing references)
		//IL_2539: Unknown result type (might be due to invalid IL or missing references)
		//IL_2540: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1602: Unknown result type (might be due to invalid IL or missing references)
		//IL_1607: Unknown result type (might be due to invalid IL or missing references)
		//IL_1610: Unknown result type (might be due to invalid IL or missing references)
		//IL_1612: Unknown result type (might be due to invalid IL or missing references)
		//IL_1614: Unknown result type (might be due to invalid IL or missing references)
		//IL_1619: Unknown result type (might be due to invalid IL or missing references)
		//IL_161e: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_16fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1715: Unknown result type (might be due to invalid IL or missing references)
		//IL_171a: Unknown result type (might be due to invalid IL or missing references)
		//IL_171f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1728: Unknown result type (might be due to invalid IL or missing references)
		//IL_172a: Unknown result type (might be due to invalid IL or missing references)
		//IL_172c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1731: Unknown result type (might be due to invalid IL or missing references)
		//IL_1736: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_18dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_18fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1901: Unknown result type (might be due to invalid IL or missing references)
		//IL_1908: Unknown result type (might be due to invalid IL or missing references)
		//IL_1914: Unknown result type (might be due to invalid IL or missing references)
		//IL_1919: Unknown result type (might be due to invalid IL or missing references)
		//IL_191e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1923: Unknown result type (might be due to invalid IL or missing references)
		//IL_1928: Unknown result type (might be due to invalid IL or missing references)
		//IL_192c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1938: Unknown result type (might be due to invalid IL or missing references)
		//IL_193d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1942: Unknown result type (might be due to invalid IL or missing references)
		//IL_1300: Unknown result type (might be due to invalid IL or missing references)
		//IL_1302: Unknown result type (might be due to invalid IL or missing references)
		//IL_1307: Unknown result type (might be due to invalid IL or missing references)
		//IL_1325: Unknown result type (might be due to invalid IL or missing references)
		//IL_1327: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e34: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e36: Unknown result type (might be due to invalid IL or missing references)
		//IL_258f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2596: Unknown result type (might be due to invalid IL or missing references)
		//IL_259d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1347: Unknown result type (might be due to invalid IL or missing references)
		//IL_1332: Unknown result type (might be due to invalid IL or missing references)
		//IL_133b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1340: Unknown result type (might be due to invalid IL or missing references)
		//IL_1345: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e54: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e56: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e62: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e91: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e93: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1356: Unknown result type (might be due to invalid IL or missing references)
		//IL_19c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ea4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1372: Unknown result type (might be due to invalid IL or missing references)
		//IL_1374: Unknown result type (might be due to invalid IL or missing references)
		//IL_137b: Unknown result type (might be due to invalid IL or missing references)
		//IL_137d: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ecb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ed4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ed9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1edb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ee0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ef2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ef9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f00: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f11: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f16: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f20: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f24: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f30: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f35: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a50: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_242c: Unknown result type (might be due to invalid IL or missing references)
		//IL_242e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aad: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1add: Unknown result type (might be due to invalid IL or missing references)
		//IL_1adf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aea: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aee: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b07: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b10: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b12: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b14: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b16: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b31: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a61: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_2443: Unknown result type (might be due to invalid IL or missing references)
		//IL_2445: Unknown result type (might be due to invalid IL or missing references)
		//IL_244c: Unknown result type (might be due to invalid IL or missing references)
		//IL_244e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2455: Unknown result type (might be due to invalid IL or missing references)
		//IL_245a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2489: Unknown result type (might be due to invalid IL or missing references)
		//IL_248b: Unknown result type (might be due to invalid IL or missing references)
		//IL_248d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2492: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a77: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a80: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a87: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a89: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a90: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a92: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a96: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b78: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b84: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f99: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fce: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1faa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bae: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ff8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2001: Unknown result type (might be due to invalid IL or missing references)
		//IL_2006: Unknown result type (might be due to invalid IL or missing references)
		//IL_200b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2010: Unknown result type (might be due to invalid IL or missing references)
		//IL_2012: Unknown result type (might be due to invalid IL or missing references)
		//IL_201b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2020: Unknown result type (might be due to invalid IL or missing references)
		//IL_2025: Unknown result type (might be due to invalid IL or missing references)
		//IL_2037: Unknown result type (might be due to invalid IL or missing references)
		//IL_2043: Unknown result type (might be due to invalid IL or missing references)
		//IL_2048: Unknown result type (might be due to invalid IL or missing references)
		//IL_204d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bea: Unknown result type (might be due to invalid IL or missing references)
		//IL_2095: Unknown result type (might be due to invalid IL or missing references)
		//IL_2097: Unknown result type (might be due to invalid IL or missing references)
		//IL_20a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_20a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_20aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_20af: Unknown result type (might be due to invalid IL or missing references)
		//IL_20b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_20ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_20c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_20cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_20d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_20d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_20d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_20d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_20db: Unknown result type (might be due to invalid IL or missing references)
		//IL_20e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_20e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_20e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_20e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_20eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_20ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_2101: Unknown result type (might be due to invalid IL or missing references)
		//IL_2103: Unknown result type (might be due to invalid IL or missing references)
		//IL_2108: Unknown result type (might be due to invalid IL or missing references)
		//IL_210a: Unknown result type (might be due to invalid IL or missing references)
		//IL_210c: Unknown result type (might be due to invalid IL or missing references)
		//IL_210e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2113: Unknown result type (might be due to invalid IL or missing references)
		//IL_2127: Unknown result type (might be due to invalid IL or missing references)
		//IL_2129: Unknown result type (might be due to invalid IL or missing references)
		//IL_2059: Unknown result type (might be due to invalid IL or missing references)
		//IL_205b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c16: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c22: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c04: Unknown result type (might be due to invalid IL or missing references)
		//IL_206f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2071: Unknown result type (might be due to invalid IL or missing references)
		//IL_2073: Unknown result type (might be due to invalid IL or missing references)
		//IL_2078: Unknown result type (might be due to invalid IL or missing references)
		//IL_207d: Unknown result type (might be due to invalid IL or missing references)
		//IL_207f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2081: Unknown result type (might be due to invalid IL or missing references)
		//IL_2083: Unknown result type (might be due to invalid IL or missing references)
		//IL_2088: Unknown result type (might be due to invalid IL or missing references)
		//IL_208a: Unknown result type (might be due to invalid IL or missing references)
		//IL_208c: Unknown result type (might be due to invalid IL or missing references)
		//IL_208e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2093: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c39: Unknown result type (might be due to invalid IL or missing references)
		//IL_2170: Unknown result type (might be due to invalid IL or missing references)
		//IL_217c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c53: Unknown result type (might be due to invalid IL or missing references)
		//IL_2193: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d04: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_21bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_21cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_21a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_21ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d12: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_21e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_220e: Unknown result type (might be due to invalid IL or missing references)
		//IL_221a: Unknown result type (might be due to invalid IL or missing references)
		//IL_21f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_21fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d56: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d27: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c94: Unknown result type (might be due to invalid IL or missing references)
		//IL_2231: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e13: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e15: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e21: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d79: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cca: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ca7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2244: Unknown result type (might be due to invalid IL or missing references)
		//IL_224b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_22fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2264: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e01: Unknown result type (might be due to invalid IL or missing references)
		//IL_230a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2275: Unknown result type (might be due to invalid IL or missing references)
		//IL_234e: Unknown result type (might be due to invalid IL or missing references)
		//IL_231f: Unknown result type (might be due to invalid IL or missing references)
		//IL_228c: Unknown result type (might be due to invalid IL or missing references)
		//IL_240b: Unknown result type (might be due to invalid IL or missing references)
		//IL_240d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2413: Unknown result type (might be due to invalid IL or missing references)
		//IL_2415: Unknown result type (might be due to invalid IL or missing references)
		//IL_2417: Unknown result type (might be due to invalid IL or missing references)
		//IL_2419: Unknown result type (might be due to invalid IL or missing references)
		//IL_23a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_2371: Unknown result type (might be due to invalid IL or missing references)
		//IL_22c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_229f: Unknown result type (might be due to invalid IL or missing references)
		//IL_23c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_22d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_23f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_23f9: Unknown result type (might be due to invalid IL or missing references)
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
			if (TerrainMeta.Path.Rails.Count > 0)
			{
				List<PrefabReplacement> list8 = new List<PrefabReplacement>();
				Vector2i val9 = default(Vector2i);
				for (int num8 = 0; num8 < val.CellCount; num8++)
				{
					for (int num9 = 0; num9 < val.CellCount; num9++)
					{
						Prefab<DungeonGridCell> prefab13 = val[num8, num9];
						if (prefab13 == null || !prefab13.Component.Replaceable)
						{
							continue;
						}
						((Vector2i)(ref val9))._002Ector(num8, num9);
						Vector3 val10 = val.GridToWorldCoords(val9) + zero2;
						Prefab<DungeonGridCell>[] array7 = array3;
						foreach (Prefab<DungeonGridCell> prefab14 in array7)
						{
							if (prefab13.Component.North != prefab14.Component.North || prefab13.Component.South != prefab14.Component.South || prefab13.Component.West != prefab14.Component.West || prefab13.Component.East != prefab14.Component.East || !prefab14.CheckEnvironmentVolumesInsideTerrain(val10 + val5, Quaternion.identity, Vector3.one, EnvironmentType.Underground) || prefab14.CheckEnvironmentVolumes(val10 + val6, Quaternion.identity, Vector3.one, EnvironmentType.Underground | EnvironmentType.Building) || prefab14.CheckEnvironmentVolumes(val10, Quaternion.identity, Vector3.one, EnvironmentType.Underground | EnvironmentType.Building) || !prefab14.ApplyTerrainChecks(val10, Quaternion.identity, Vector3.one) || !prefab14.ApplyTerrainFilters(val10, Quaternion.identity, Vector3.one))
							{
								continue;
							}
							MonumentInfo componentInChildren6 = prefab14.Object.GetComponentInChildren<MonumentInfo>();
							Vector3 val11 = val10;
							if (Object.op_Implicit((Object)(object)componentInChildren6))
							{
								val11 += ((Component)componentInChildren6).transform.position;
							}
							if (!(val11.y < 1f))
							{
								float distanceToAboveGroundRail = GetDistanceToAboveGroundRail(val11);
								if (!(distanceToAboveGroundRail < 200f))
								{
									PrefabReplacement item = default(PrefabReplacement);
									item.gridPosition = val9;
									item.worldPosition = val11;
									item.distance = distanceToAboveGroundRail;
									item.prefab = prefab14;
									list8.Add(item);
								}
							}
						}
					}
				}
				list8.Sort((PrefabReplacement a, PrefabReplacement b) => a.distance.CompareTo(b.distance));
				int num10 = 2;
				while (num10 > 0 && list8.Count > 0)
				{
					num10--;
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
			Vector2i val12 = default(Vector2i);
			for (int num11 = 0; num11 < val.CellCount; num11++)
			{
				for (int num12 = 0; num12 < val.CellCount; num12++)
				{
					Prefab<DungeonGridCell> prefab15 = val[num11, num12];
					if (prefab15 != null)
					{
						((Vector2i)(ref val12))._002Ector(num11, num12);
						Vector3 val13 = val.GridToWorldCoords(val12);
						World.AddPrefab("Dungeon", prefab15, zero2 + val13, Quaternion.identity, Vector3.one);
					}
				}
			}
			Vector2i val14 = default(Vector2i);
			Vector2i val16 = default(Vector2i);
			for (int num13 = 0; num13 < val.CellCount - 1; num13++)
			{
				for (int num14 = 0; num14 < val.CellCount - 1; num14++)
				{
					Prefab<DungeonGridCell> prefab16 = val[num13, num14];
					Prefab<DungeonGridCell> prefab17 = val[num13 + 1, num14];
					Prefab<DungeonGridCell> prefab18 = val[num13, num14 + 1];
					Prefab<DungeonGridCell>[] array7;
					if (prefab16 != null && prefab17 != null && prefab16.Component.EastVariant != prefab17.Component.WestVariant)
					{
						array4.Shuffle(ref seed);
						array7 = array4;
						foreach (Prefab<DungeonGridCell> prefab19 in array7)
						{
							if (prefab19.Component.West == prefab16.Component.East && prefab19.Component.East == prefab17.Component.West && prefab19.Component.WestVariant == prefab16.Component.EastVariant && prefab19.Component.EastVariant == prefab17.Component.WestVariant)
							{
								((Vector2i)(ref val14))._002Ector(num13, num14);
								Vector3 val15 = val.GridToWorldCoords(val14) + new Vector3(val.CellSizeHalf, 0f, 0f);
								World.AddPrefab("Dungeon", prefab19, zero2 + val15, Quaternion.identity, Vector3.one);
								break;
							}
						}
					}
					if (prefab16 == null || prefab18 == null || prefab16.Component.NorthVariant == prefab18.Component.SouthVariant)
					{
						continue;
					}
					array4.Shuffle(ref seed);
					array7 = array4;
					foreach (Prefab<DungeonGridCell> prefab20 in array7)
					{
						if (prefab20.Component.South == prefab16.Component.North && prefab20.Component.North == prefab18.Component.South && prefab20.Component.SouthVariant == prefab16.Component.NorthVariant && prefab20.Component.NorthVariant == prefab18.Component.SouthVariant)
						{
							((Vector2i)(ref val16))._002Ector(num13, num14);
							Vector3 val17 = val.GridToWorldCoords(val16) + new Vector3(0f, 0f, val.CellSizeHalf);
							World.AddPrefab("Dungeon", prefab20, zero2 + val17, Quaternion.identity, Vector3.one);
							break;
						}
					}
				}
			}
			foreach (PathLink item6 in list3)
			{
				Vector3 val18 = item6.upwards.origin.position + item6.upwards.origin.rotation * Vector3.Scale(item6.upwards.origin.upSocket.localPosition, item6.upwards.origin.scale);
				Vector3 val19 = item6.downwards.origin.position + item6.downwards.origin.rotation * Vector3.Scale(item6.downwards.origin.downSocket.localPosition, item6.downwards.origin.scale) - val18;
				Vector3[] array8 = (Vector3[])(object)new Vector3[2]
				{
					new Vector3(0f, 1f, 0f),
					new Vector3(1f, 1f, 1f)
				};
				foreach (Vector3 val20 in array8)
				{
					int num15 = 0;
					int num16 = 0;
					while (((Vector3)(ref val19)).magnitude > 1f && (num15 < 8 || num16 < 8))
					{
						bool flag2 = num15 > 2 && num16 > 2;
						bool flag3 = num15 > 4 && num16 > 4;
						Prefab<DungeonGridLink> prefab21 = null;
						Vector3 val21 = Vector3.zero;
						int num17 = int.MinValue;
						Vector3 position3 = Vector3.zero;
						Quaternion rotation2 = Quaternion.identity;
						PathLinkSegment prevSegment = item6.downwards.prevSegment;
						Vector3 val22 = prevSegment.position + prevSegment.rotation * Vector3.Scale(prevSegment.scale, prevSegment.downSocket.localPosition);
						Quaternion val23 = prevSegment.rotation * prevSegment.downSocket.localRotation;
						Prefab<DungeonGridLink>[] array9 = array5;
						foreach (Prefab<DungeonGridLink> prefab22 in array9)
						{
							float num18 = SeedRandom.Value(ref seed);
							DungeonGridLink component = prefab22.Component;
							if (prevSegment.downType != component.UpType)
							{
								continue;
							}
							switch (component.DownType)
							{
							case DungeonGridLinkType.Elevator:
								if (flag2 || val20.x != 0f || val20.z != 0f)
								{
									continue;
								}
								break;
							case DungeonGridLinkType.Transition:
								if (val20.x != 0f || val20.z != 0f)
								{
									continue;
								}
								break;
							}
							int num19 = ((!flag2) ? component.Priority : 0);
							if (num17 > num19)
							{
								continue;
							}
							Quaternion val24 = val23 * Quaternion.Inverse(component.UpSocket.localRotation);
							Quaternion val25 = val24 * component.DownSocket.localRotation;
							PathLinkSegment prevSegment2 = item6.upwards.prevSegment;
							Quaternion val26 = prevSegment2.rotation * prevSegment2.upSocket.localRotation;
							if (component.Rotation > 0)
							{
								if (Quaternion.Angle(val26, val25) > (float)component.Rotation)
								{
									continue;
								}
								Quaternion val27 = val26 * Quaternion.Inverse(val25);
								val24 *= val27;
								val25 *= val27;
							}
							Vector3 val28 = val22 - val24 * component.UpSocket.localPosition;
							Vector3 val29 = val24 * (component.DownSocket.localPosition - component.UpSocket.localPosition);
							Vector3 val30 = val19 + val21;
							Vector3 val31 = val19 + val29;
							float magnitude = ((Vector3)(ref val30)).magnitude;
							float magnitude2 = ((Vector3)(ref val31)).magnitude;
							Vector3 val32 = Vector3.Scale(val30, val20);
							Vector3 val33 = Vector3.Scale(val31, val20);
							float magnitude3 = ((Vector3)(ref val32)).magnitude;
							float magnitude4 = ((Vector3)(ref val33)).magnitude;
							if (val21 != Vector3.zero)
							{
								if (magnitude3 < magnitude4 || (magnitude3 == magnitude4 && magnitude < magnitude2) || (magnitude3 == magnitude4 && magnitude == magnitude2 && num18 < 0.5f))
								{
									continue;
								}
							}
							else if (magnitude3 <= magnitude4)
							{
								continue;
							}
							if (Mathf.Abs(val33.x) - Mathf.Abs(val32.x) > 0.01f || (Mathf.Abs(val33.x) > 0.01f && val30.x * val31.x < 0f) || Mathf.Abs(val33.y) - Mathf.Abs(val32.y) > 0.01f || (Mathf.Abs(val33.y) > 0.01f && val30.y * val31.y < 0f) || Mathf.Abs(val33.z) - Mathf.Abs(val32.z) > 0.01f || (Mathf.Abs(val33.z) > 0.01f && val30.z * val31.z < 0f) || (flag2 && val20.x == 0f && val20.z == 0f && component.DownType == DungeonGridLinkType.Default && ((Mathf.Abs(val31.x) > 0.01f && Mathf.Abs(val31.x) < LinkRadius * 2f - 0.1f) || (Mathf.Abs(val31.z) > 0.01f && Mathf.Abs(val31.z) < LinkRadius * 2f - 0.1f))))
							{
								continue;
							}
							num17 = num19;
							if (val20.x == 0f && val20.z == 0f)
							{
								if (!flag2 && Mathf.Abs(val31.y) < LinkTransition - 0.1f)
								{
									continue;
								}
							}
							else if ((!flag2 && magnitude4 > 0.01f && (Mathf.Abs(val31.x) < LinkRadius * 2f - 0.1f || Mathf.Abs(val31.z) < LinkRadius * 2f - 0.1f)) || (!flag3 && magnitude4 > 0.01f && (Mathf.Abs(val31.x) < LinkRadius * 1f - 0.1f || Mathf.Abs(val31.z) < LinkRadius * 1f - 0.1f)))
							{
								continue;
							}
							if (!flag2 || !(magnitude4 < 0.01f) || !(magnitude2 < 0.01f) || !(Quaternion.Angle(val26, val25) > 10f))
							{
								prefab21 = prefab22;
								val21 = val29;
								num17 = num19;
								position3 = val28;
								rotation2 = val24;
							}
						}
						if (val21 != Vector3.zero)
						{
							PathLinkSegment pathLinkSegment = new PathLinkSegment();
							pathLinkSegment.position = position3;
							pathLinkSegment.rotation = rotation2;
							pathLinkSegment.scale = Vector3.one;
							pathLinkSegment.prefab = prefab21;
							pathLinkSegment.link = prefab21.Component;
							item6.downwards.segments.Add(pathLinkSegment);
							val19 += val21;
						}
						else
						{
							num16++;
						}
						if (val20.x > 0f || val20.z > 0f)
						{
							Prefab<DungeonGridLink> prefab23 = null;
							Vector3 val34 = Vector3.zero;
							int num20 = int.MinValue;
							Vector3 position4 = Vector3.zero;
							Quaternion rotation3 = Quaternion.identity;
							PathLinkSegment prevSegment3 = item6.upwards.prevSegment;
							Vector3 val35 = prevSegment3.position + prevSegment3.rotation * Vector3.Scale(prevSegment3.scale, prevSegment3.upSocket.localPosition);
							Quaternion val36 = prevSegment3.rotation * prevSegment3.upSocket.localRotation;
							array9 = array5;
							foreach (Prefab<DungeonGridLink> prefab24 in array9)
							{
								float num21 = SeedRandom.Value(ref seed);
								DungeonGridLink component2 = prefab24.Component;
								if (prevSegment3.upType != component2.DownType)
								{
									continue;
								}
								switch (component2.DownType)
								{
								case DungeonGridLinkType.Elevator:
									if (flag2 || val20.x != 0f || val20.z != 0f)
									{
										continue;
									}
									break;
								case DungeonGridLinkType.Transition:
									if (val20.x != 0f || val20.z != 0f)
									{
										continue;
									}
									break;
								}
								int num22 = ((!flag2) ? component2.Priority : 0);
								if (num20 > num22)
								{
									continue;
								}
								Quaternion val37 = val36 * Quaternion.Inverse(component2.DownSocket.localRotation);
								Quaternion val38 = val37 * component2.UpSocket.localRotation;
								PathLinkSegment prevSegment4 = item6.downwards.prevSegment;
								Quaternion val39 = prevSegment4.rotation * prevSegment4.downSocket.localRotation;
								if (component2.Rotation > 0)
								{
									if (Quaternion.Angle(val39, val38) > (float)component2.Rotation)
									{
										continue;
									}
									Quaternion val40 = val39 * Quaternion.Inverse(val38);
									val37 *= val40;
									val38 *= val40;
								}
								Vector3 val41 = val35 - val37 * component2.DownSocket.localPosition;
								Vector3 val42 = val37 * (component2.UpSocket.localPosition - component2.DownSocket.localPosition);
								Vector3 val43 = val19 - val34;
								Vector3 val44 = val19 - val42;
								float magnitude5 = ((Vector3)(ref val43)).magnitude;
								float magnitude6 = ((Vector3)(ref val44)).magnitude;
								Vector3 val45 = Vector3.Scale(val43, val20);
								Vector3 val46 = Vector3.Scale(val44, val20);
								float magnitude7 = ((Vector3)(ref val45)).magnitude;
								float magnitude8 = ((Vector3)(ref val46)).magnitude;
								if (val34 != Vector3.zero)
								{
									if (magnitude7 < magnitude8 || (magnitude7 == magnitude8 && magnitude5 < magnitude6) || (magnitude7 == magnitude8 && magnitude5 == magnitude6 && num21 < 0.5f))
									{
										continue;
									}
								}
								else if (magnitude7 <= magnitude8)
								{
									continue;
								}
								if (Mathf.Abs(val46.x) - Mathf.Abs(val45.x) > 0.01f || (Mathf.Abs(val46.x) > 0.01f && val43.x * val44.x < 0f) || Mathf.Abs(val46.y) - Mathf.Abs(val45.y) > 0.01f || (Mathf.Abs(val46.y) > 0.01f && val43.y * val44.y < 0f) || Mathf.Abs(val46.z) - Mathf.Abs(val45.z) > 0.01f || (Mathf.Abs(val46.z) > 0.01f && val43.z * val44.z < 0f) || (flag2 && val20.x == 0f && val20.z == 0f && component2.UpType == DungeonGridLinkType.Default && ((Mathf.Abs(val44.x) > 0.01f && Mathf.Abs(val44.x) < LinkRadius * 2f - 0.1f) || (Mathf.Abs(val44.z) > 0.01f && Mathf.Abs(val44.z) < LinkRadius * 2f - 0.1f))))
								{
									continue;
								}
								num20 = num22;
								if (val20.x == 0f && val20.z == 0f)
								{
									if (!flag2 && Mathf.Abs(val44.y) < LinkTransition - 0.1f)
									{
										continue;
									}
								}
								else if ((!flag2 && magnitude8 > 0.01f && (Mathf.Abs(val44.x) < LinkRadius * 2f - 0.1f || Mathf.Abs(val44.z) < LinkRadius * 2f - 0.1f)) || (!flag3 && magnitude8 > 0.01f && (Mathf.Abs(val44.x) < LinkRadius * 1f - 0.1f || Mathf.Abs(val44.z) < LinkRadius * 1f - 0.1f)))
								{
									continue;
								}
								if (!flag2 || !(magnitude8 < 0.01f) || !(magnitude6 < 0.01f) || !(Quaternion.Angle(val39, val38) > 10f))
								{
									prefab23 = prefab24;
									val34 = val42;
									num20 = num22;
									position4 = val41;
									rotation3 = val37;
								}
							}
							if (val34 != Vector3.zero)
							{
								PathLinkSegment pathLinkSegment2 = new PathLinkSegment();
								pathLinkSegment2.position = position4;
								pathLinkSegment2.rotation = rotation3;
								pathLinkSegment2.scale = Vector3.one;
								pathLinkSegment2.prefab = prefab23;
								pathLinkSegment2.link = prefab23.Component;
								item6.upwards.segments.Add(pathLinkSegment2);
								val19 -= val34;
							}
							else
							{
								num15++;
							}
						}
						else
						{
							num15++;
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
