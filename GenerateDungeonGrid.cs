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

		public PathLinkSegment prevSegment => (segments.Count > 0) ? segments[segments.Count - 1] : origin;
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

	public string TunnelFolder = string.Empty;

	public string StationFolder = string.Empty;

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
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Unknown result type (might be due to invalid IL or missing references)
		//IL_092d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0932: Unknown result type (might be due to invalid IL or missing references)
		//IL_093a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0941: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_120f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1214: Unknown result type (might be due to invalid IL or missing references)
		//IL_1216: Unknown result type (might be due to invalid IL or missing references)
		//IL_121b: Unknown result type (might be due to invalid IL or missing references)
		//IL_121d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1227: Unknown result type (might be due to invalid IL or missing references)
		//IL_122c: Unknown result type (might be due to invalid IL or missing references)
		//IL_122e: Unknown result type (might be due to invalid IL or missing references)
		//IL_123f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1244: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1247: Unknown result type (might be due to invalid IL or missing references)
		//IL_1249: Unknown result type (might be due to invalid IL or missing references)
		//IL_134d: Unknown result type (might be due to invalid IL or missing references)
		//IL_134f: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_0632: Unknown result type (might be due to invalid IL or missing references)
		//IL_1286: Unknown result type (might be due to invalid IL or missing references)
		//IL_1288: Unknown result type (might be due to invalid IL or missing references)
		//IL_128d: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_1382: Unknown result type (might be due to invalid IL or missing references)
		//IL_1387: Unknown result type (might be due to invalid IL or missing references)
		//IL_1389: Unknown result type (might be due to invalid IL or missing references)
		//IL_138e: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0696: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12de: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0705: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0718: Unknown result type (might be due to invalid IL or missing references)
		//IL_071d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0752: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0784: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0804: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_0817: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_12fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1300: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_13fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1771: Unknown result type (might be due to invalid IL or missing references)
		//IL_1782: Unknown result type (might be due to invalid IL or missing references)
		//IL_1798: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_17cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_17dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1803: Unknown result type (might be due to invalid IL or missing references)
		//IL_1808: Unknown result type (might be due to invalid IL or missing references)
		//IL_180d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1812: Unknown result type (might be due to invalid IL or missing references)
		//IL_1817: Unknown result type (might be due to invalid IL or missing references)
		//IL_1819: Unknown result type (might be due to invalid IL or missing references)
		//IL_181b: Unknown result type (might be due to invalid IL or missing references)
		//IL_181d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1822: Unknown result type (might be due to invalid IL or missing references)
		//IL_183b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1840: Unknown result type (might be due to invalid IL or missing references)
		//IL_1856: Unknown result type (might be due to invalid IL or missing references)
		//IL_185b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0530: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0553: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_1873: Unknown result type (might be due to invalid IL or missing references)
		//IL_1878: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b08: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b13: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_156d: Unknown result type (might be due to invalid IL or missing references)
		//IL_156f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1585: Unknown result type (might be due to invalid IL or missing references)
		//IL_158a: Unknown result type (might be due to invalid IL or missing references)
		//IL_158f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1598: Unknown result type (might be due to invalid IL or missing references)
		//IL_159a: Unknown result type (might be due to invalid IL or missing references)
		//IL_159c: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_18bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_18be: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_18dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_18fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1903: Unknown result type (might be due to invalid IL or missing references)
		//IL_1907: Unknown result type (might be due to invalid IL or missing references)
		//IL_1913: Unknown result type (might be due to invalid IL or missing references)
		//IL_1918: Unknown result type (might be due to invalid IL or missing references)
		//IL_191d: Unknown result type (might be due to invalid IL or missing references)
		//IL_16bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_16bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_16dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b92: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ba8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f24: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f26: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f49: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f56: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f58: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f63: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f68: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fae: Unknown result type (might be due to invalid IL or missing references)
		//IL_19cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1992: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fca: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ff4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ff9: Unknown result type (might be due to invalid IL or missing references)
		//IL_200a: Unknown result type (might be due to invalid IL or missing references)
		//IL_200f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2015: Unknown result type (might be due to invalid IL or missing references)
		//IL_201a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2038: Unknown result type (might be due to invalid IL or missing references)
		//IL_2043: Unknown result type (might be due to invalid IL or missing references)
		//IL_204e: Unknown result type (might be due to invalid IL or missing references)
		//IL_205e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2063: Unknown result type (might be due to invalid IL or missing references)
		//IL_2068: Unknown result type (might be due to invalid IL or missing references)
		//IL_206d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2072: Unknown result type (might be due to invalid IL or missing references)
		//IL_207e: Unknown result type (might be due to invalid IL or missing references)
		//IL_208e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2093: Unknown result type (might be due to invalid IL or missing references)
		//IL_2098: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a29: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a32: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a41: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a43: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a51: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a56: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a68: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a74: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ada: Unknown result type (might be due to invalid IL or missing references)
		//IL_1adc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aea: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b10: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b15: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b20: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b25: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b27: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b29: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b30: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b44: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b46: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b48: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b51: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b53: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a91: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a93: Unknown result type (might be due to invalid IL or missing references)
		//IL_2978: Unknown result type (might be due to invalid IL or missing references)
		//IL_297e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1abc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1acc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ace: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_29ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_29b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_29bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_29c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_29ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_29d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_219f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1be6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_2155: Unknown result type (might be due to invalid IL or missing references)
		//IL_21c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c06: Unknown result type (might be due to invalid IL or missing references)
		//IL_2179: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c19: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c20: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c40: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2224: Unknown result type (might be due to invalid IL or missing references)
		//IL_2235: Unknown result type (might be due to invalid IL or missing references)
		//IL_223a: Unknown result type (might be due to invalid IL or missing references)
		//IL_223f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2244: Unknown result type (might be due to invalid IL or missing references)
		//IL_224a: Unknown result type (might be due to invalid IL or missing references)
		//IL_225b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2260: Unknown result type (might be due to invalid IL or missing references)
		//IL_2265: Unknown result type (might be due to invalid IL or missing references)
		//IL_2283: Unknown result type (might be due to invalid IL or missing references)
		//IL_2293: Unknown result type (might be due to invalid IL or missing references)
		//IL_2298: Unknown result type (might be due to invalid IL or missing references)
		//IL_229d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c60: Unknown result type (might be due to invalid IL or missing references)
		//IL_234c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2352: Unknown result type (might be due to invalid IL or missing references)
		//IL_2363: Unknown result type (might be due to invalid IL or missing references)
		//IL_2368: Unknown result type (might be due to invalid IL or missing references)
		//IL_236d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2372: Unknown result type (might be due to invalid IL or missing references)
		//IL_2378: Unknown result type (might be due to invalid IL or missing references)
		//IL_2389: Unknown result type (might be due to invalid IL or missing references)
		//IL_2399: Unknown result type (might be due to invalid IL or missing references)
		//IL_239e: Unknown result type (might be due to invalid IL or missing references)
		//IL_23a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_23a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_23ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_23b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_23b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_23bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_23c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_23c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_23c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_23ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_23f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_23fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_23fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_2403: Unknown result type (might be due to invalid IL or missing references)
		//IL_2409: Unknown result type (might be due to invalid IL or missing references)
		//IL_240f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2411: Unknown result type (might be due to invalid IL or missing references)
		//IL_2416: Unknown result type (might be due to invalid IL or missing references)
		//IL_243e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2444: Unknown result type (might be due to invalid IL or missing references)
		//IL_22c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_22c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c73: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2301: Unknown result type (might be due to invalid IL or missing references)
		//IL_2307: Unknown result type (might be due to invalid IL or missing references)
		//IL_230d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2312: Unknown result type (might be due to invalid IL or missing references)
		//IL_2317: Unknown result type (might be due to invalid IL or missing references)
		//IL_231d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2323: Unknown result type (might be due to invalid IL or missing references)
		//IL_2329: Unknown result type (might be due to invalid IL or missing references)
		//IL_232e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2334: Unknown result type (might be due to invalid IL or missing references)
		//IL_233a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2340: Unknown result type (might be due to invalid IL or missing references)
		//IL_2345: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ca6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cba: Unknown result type (might be due to invalid IL or missing references)
		//IL_251b: Unknown result type (might be due to invalid IL or missing references)
		//IL_252b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ccd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_2543: Unknown result type (might be due to invalid IL or missing references)
		//IL_255a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2565: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2591: Unknown result type (might be due to invalid IL or missing references)
		//IL_25a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d06: Unknown result type (might be due to invalid IL or missing references)
		//IL_25b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_25d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_25db: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2607: Unknown result type (might be due to invalid IL or missing references)
		//IL_2617: Unknown result type (might be due to invalid IL or missing references)
		//IL_262f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2646: Unknown result type (might be due to invalid IL or missing references)
		//IL_2651: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ddc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e18: Unknown result type (might be due to invalid IL or missing references)
		//IL_2681: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e38: Unknown result type (might be due to invalid IL or missing references)
		//IL_268f: Unknown result type (might be due to invalid IL or missing references)
		//IL_276f: Unknown result type (might be due to invalid IL or missing references)
		//IL_26c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e76: Unknown result type (might be due to invalid IL or missing references)
		//IL_277d: Unknown result type (might be due to invalid IL or missing references)
		//IL_26d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e96: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f02: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f04: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f10: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ee5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ee7: Unknown result type (might be due to invalid IL or missing references)
		//IL_2711: Unknown result type (might be due to invalid IL or missing references)
		//IL_27a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_2728: Unknown result type (might be due to invalid IL or missing references)
		//IL_27ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_2811: Unknown result type (might be due to invalid IL or missing references)
		//IL_285f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2883: Unknown result type (might be due to invalid IL or missing references)
		//IL_2926: Unknown result type (might be due to invalid IL or missing references)
		//IL_292c: Unknown result type (might be due to invalid IL or missing references)
		//IL_293e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2944: Unknown result type (might be due to invalid IL or missing references)
		//IL_294a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2950: Unknown result type (might be due to invalid IL or missing references)
		//IL_28f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_28f7: Unknown result type (might be due to invalid IL or missing references)
		if (World.Cached)
		{
			TerrainMeta.Path.DungeonGridRoot = HierarchyUtil.GetRoot("Dungeon");
			return;
		}
		if (World.Networked)
		{
			World.Spawn("Dungeon");
			TerrainMeta.Path.DungeonGridRoot = HierarchyUtil.GetRoot("Dungeon");
			return;
		}
		Prefab<DungeonGridCell>[] array = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + TunnelFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Prefab<DungeonGridCell>[] array2 = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + StationFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
		if (array2 == null || array2.Length == 0)
		{
			return;
		}
		Prefab<DungeonGridCell>[] array3 = Prefab.Load<DungeonGridCell>("assets/bundled/prefabs/autospawn/" + TransitionFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
		if (array3 == null)
		{
			return;
		}
		Prefab<DungeonGridLink>[] array4 = Prefab.Load<DungeonGridLink>("assets/bundled/prefabs/autospawn/" + LinkFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
		if (array4 == null)
		{
			return;
		}
		array4 = array4.OrderByDescending((Prefab<DungeonGridLink> x) => x.Component.Priority).ToArray();
		List<DungeonGridInfo> list = (Object.op_Implicit((Object)(object)TerrainMeta.Path) ? TerrainMeta.Path.DungeonGridEntrances : null);
		WorldSpaceGrid<Prefab<DungeonGridCell>> val = new WorldSpaceGrid<Prefab<DungeonGridCell>>(TerrainMeta.Size.x * 2f, (float)CellSize);
		int[,] array5 = new int[val.CellCount, val.CellCount];
		DungeonGridConnectionHash[,] hashmap = new DungeonGridConnectionHash[val.CellCount, val.CellCount];
		PathFinder pathFinder = new PathFinder(array5, diagonals: false);
		int cellCount = val.CellCount;
		int num = 0;
		int num2 = val.CellCount - 1;
		for (int i = 0; i < cellCount; i++)
		{
			for (int j = 0; j < cellCount; j++)
			{
				array5[j, i] = 1;
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
		foreach (DungeonGridInfo item2 in list)
		{
			DungeonGridInfo entrance = item2;
			TerrainPathConnect[] componentsInChildren = ((Component)entrance).GetComponentsInChildren<TerrainPathConnect>(true);
			foreach (TerrainPathConnect terrainPathConnect in componentsInChildren)
			{
				if (terrainPathConnect.Type != ConnectionType)
				{
					continue;
				}
				Vector2i val2 = val.WorldToGridCoords(((Component)terrainPathConnect).transform.position);
				if (array5[val2.x, val2.y] == int.MaxValue)
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
				Prefab<DungeonGridCell>[] array6 = array2;
				foreach (Prefab<DungeonGridCell> prefab6 in array6)
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
						Vector3 val3 = val.GridToWorldCoords(new Vector2i(val2.x, val2.y));
						Vector3 val4 = val3 + componentInChildren3.UpSocket.localPosition;
						float num4 = Vector3Ex.Magnitude2D(((Component)terrainPathConnect).transform.position - val4);
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
					array5[val2.x, val2.y] = int.MaxValue;
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
					//IL_0064: Unknown result type (might be due to invalid IL or missing references)
					PathFinder.Node node8 = pathFinder.FindClosestWalkable(new PathFinder.Point(x, y), 1);
					if (node8 != null)
					{
						PathNode item = new PathNode
						{
							monument = (Object.op_Implicit((Object)(object)TerrainMeta.Path) ? TerrainMeta.Path.FindClosest(TerrainMeta.Path.Monuments, ((Component)entrance).transform.position) : ((Component)((Component)entrance).transform).GetComponentInParent<MonumentInfo>()),
							node = node8
						};
						if (isStartPoint)
						{
							secondaryNodeList.Add(item);
						}
						else
						{
							unconnectedNodeList.Add(item);
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
				Vector2i val5 = val.WorldToGridCoords(((Component)node3.monument).transform.position);
				pathFinder.PushPoint = new PathFinder.Point(val5.x, val5.y);
				pathFinder.PushRadius = 2;
				pathFinder.PushDistance = 2;
				pathFinder.PushMultiplier = 4;
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
		foreach (PathSegment item3 in list2)
		{
			PathFinder.Node node7 = item3.start;
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
				int num5 = array5[m, n];
				if (num5 == int.MaxValue)
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
						bool flag = prefab8 == null || prefab7.Component.WestVariant == prefab8.Component.EastVariant;
						bool flag2 = prefab10 == null || prefab7.Component.SouthVariant == prefab10.Component.NorthVariant;
						if (flag && flag2)
						{
							break;
						}
					}
				}
			}
		}
		Vector3 zero2 = Vector3.zero;
		Vector3 zero3 = Vector3.zero;
		Vector3 val6 = Vector3.up * 10f;
		Vector3 val7 = Vector3.up * (LinkTransition + 1f);
		Vector2i val8 = default(Vector2i);
		do
		{
			zero3 = zero2;
			for (int num7 = 0; num7 < val.CellCount; num7++)
			{
				for (int num8 = 0; num8 < val.CellCount; num8++)
				{
					Prefab<DungeonGridCell> prefab12 = val[num7, num8];
					if (prefab12 != null)
					{
						((Vector2i)(ref val8))._002Ector(num7, num8);
						Vector3 val9 = val.GridToWorldCoords(val8);
						while (!prefab12.CheckEnvironmentVolumesInsideTerrain(zero2 + val9 + val6, Quaternion.identity, Vector3.one, EnvironmentType.Underground) || prefab12.CheckEnvironmentVolumes(zero2 + val9 + val7, Quaternion.identity, Vector3.one, EnvironmentType.Underground | EnvironmentType.Building) || prefab12.CheckEnvironmentVolumes(zero2 + val9, Quaternion.identity, Vector3.one, EnvironmentType.Underground | EnvironmentType.Building))
						{
							zero2.y -= 9f;
						}
					}
				}
			}
		}
		while (zero2 != zero3);
		foreach (PathLink item4 in list3)
		{
			PathLinkSegment origin = item4.upwards.origin;
			origin.position += zero2;
		}
		Vector2i val10 = default(Vector2i);
		for (int num9 = 0; num9 < val.CellCount; num9++)
		{
			for (int num10 = 0; num10 < val.CellCount; num10++)
			{
				Prefab<DungeonGridCell> prefab13 = val[num9, num10];
				if (prefab13 != null)
				{
					((Vector2i)(ref val10))._002Ector(num9, num10);
					Vector3 val11 = val.GridToWorldCoords(val10);
					World.AddPrefab("Dungeon", prefab13, zero2 + val11, Quaternion.identity, Vector3.one);
				}
			}
		}
		Vector2i val12 = default(Vector2i);
		Vector2i val14 = default(Vector2i);
		for (int num11 = 0; num11 < val.CellCount - 1; num11++)
		{
			for (int num12 = 0; num12 < val.CellCount - 1; num12++)
			{
				Prefab<DungeonGridCell> prefab14 = val[num11, num12];
				Prefab<DungeonGridCell> prefab15 = val[num11 + 1, num12];
				Prefab<DungeonGridCell> prefab16 = val[num11, num12 + 1];
				if (prefab14 != null && prefab15 != null && prefab14.Component.EastVariant != prefab15.Component.WestVariant)
				{
					array3.Shuffle(ref seed);
					Prefab<DungeonGridCell>[] array8 = array3;
					foreach (Prefab<DungeonGridCell> prefab17 in array8)
					{
						if (prefab17.Component.West == prefab14.Component.East && prefab17.Component.East == prefab15.Component.West && prefab17.Component.WestVariant == prefab14.Component.EastVariant && prefab17.Component.EastVariant == prefab15.Component.WestVariant)
						{
							((Vector2i)(ref val12))._002Ector(num11, num12);
							Vector3 val13 = val.GridToWorldCoords(val12) + new Vector3(val.CellSizeHalf, 0f, 0f);
							World.AddPrefab("Dungeon", prefab17, zero2 + val13, Quaternion.identity, Vector3.one);
							break;
						}
					}
				}
				if (prefab14 == null || prefab16 == null || prefab14.Component.NorthVariant == prefab16.Component.SouthVariant)
				{
					continue;
				}
				array3.Shuffle(ref seed);
				Prefab<DungeonGridCell>[] array9 = array3;
				foreach (Prefab<DungeonGridCell> prefab18 in array9)
				{
					if (prefab18.Component.South == prefab14.Component.North && prefab18.Component.North == prefab16.Component.South && prefab18.Component.SouthVariant == prefab14.Component.NorthVariant && prefab18.Component.NorthVariant == prefab16.Component.SouthVariant)
					{
						((Vector2i)(ref val14))._002Ector(num11, num12);
						Vector3 val15 = val.GridToWorldCoords(val14) + new Vector3(0f, 0f, val.CellSizeHalf);
						World.AddPrefab("Dungeon", prefab18, zero2 + val15, Quaternion.identity, Vector3.one);
						break;
					}
				}
			}
		}
		foreach (PathLink item5 in list3)
		{
			Vector3 val16 = item5.upwards.origin.position + item5.upwards.origin.rotation * Vector3.Scale(item5.upwards.origin.upSocket.localPosition, item5.upwards.origin.scale);
			Vector3 val17 = item5.downwards.origin.position + item5.downwards.origin.rotation * Vector3.Scale(item5.downwards.origin.downSocket.localPosition, item5.downwards.origin.scale);
			Vector3 val18 = val17 - val16;
			Vector3[] array10 = (Vector3[])(object)new Vector3[2]
			{
				new Vector3(0f, 1f, 0f),
				new Vector3(1f, 1f, 1f)
			};
			Vector3[] array11 = array10;
			foreach (Vector3 val19 in array11)
			{
				int num16 = 0;
				int num17 = 0;
				while (((Vector3)(ref val18)).magnitude > 1f && (num16 < 8 || num17 < 8))
				{
					bool flag3 = num16 > 2 && num17 > 2;
					bool flag4 = num16 > 4 && num17 > 4;
					Prefab<DungeonGridLink> prefab19 = null;
					Vector3 val20 = Vector3.zero;
					int num18 = int.MinValue;
					Vector3 position3 = Vector3.zero;
					Quaternion rotation2 = Quaternion.identity;
					PathLinkSegment prevSegment = item5.downwards.prevSegment;
					Vector3 val21 = prevSegment.position + prevSegment.rotation * Vector3.Scale(prevSegment.scale, prevSegment.downSocket.localPosition);
					Quaternion val22 = prevSegment.rotation * prevSegment.downSocket.localRotation;
					Prefab<DungeonGridLink>[] array12 = array4;
					foreach (Prefab<DungeonGridLink> prefab20 in array12)
					{
						float num20 = SeedRandom.Value(ref seed);
						DungeonGridLink component = prefab20.Component;
						if (prevSegment.downType != component.UpType)
						{
							continue;
						}
						switch (component.DownType)
						{
						case DungeonGridLinkType.Elevator:
							if (flag3 || val19.x != 0f || val19.z != 0f)
							{
								continue;
							}
							break;
						case DungeonGridLinkType.Transition:
							if (val19.x != 0f || val19.z != 0f)
							{
								continue;
							}
							break;
						}
						int num21 = ((!flag3) ? component.Priority : 0);
						if (num18 > num21)
						{
							continue;
						}
						Quaternion val23 = val22 * Quaternion.Inverse(component.UpSocket.localRotation);
						Quaternion val24 = val23 * component.DownSocket.localRotation;
						PathLinkSegment prevSegment2 = item5.upwards.prevSegment;
						Quaternion val25 = prevSegment2.rotation * prevSegment2.upSocket.localRotation;
						if (component.Rotation > 0)
						{
							float num22 = Quaternion.Angle(val25, val24);
							if (num22 > (float)component.Rotation)
							{
								continue;
							}
							Quaternion val26 = val25 * Quaternion.Inverse(val24);
							val23 *= val26;
							val24 *= val26;
						}
						Vector3 val27 = val21 - val23 * component.UpSocket.localPosition;
						Vector3 val28 = val23 * (component.DownSocket.localPosition - component.UpSocket.localPosition);
						Vector3 val29 = val18 + val20;
						Vector3 val30 = val18 + val28;
						float magnitude = ((Vector3)(ref val29)).magnitude;
						float magnitude2 = ((Vector3)(ref val30)).magnitude;
						Vector3 val31 = Vector3.Scale(val29, val19);
						Vector3 val32 = Vector3.Scale(val30, val19);
						float magnitude3 = ((Vector3)(ref val31)).magnitude;
						float magnitude4 = ((Vector3)(ref val32)).magnitude;
						if (val20 != Vector3.zero)
						{
							if (magnitude3 < magnitude4 || (magnitude3 == magnitude4 && magnitude < magnitude2) || (magnitude3 == magnitude4 && magnitude == magnitude2 && num20 < 0.5f))
							{
								continue;
							}
						}
						else if (magnitude3 <= magnitude4)
						{
							continue;
						}
						if (Mathf.Abs(val32.x) - Mathf.Abs(val31.x) > 0.01f || (Mathf.Abs(val32.x) > 0.01f && val29.x * val30.x < 0f) || Mathf.Abs(val32.y) - Mathf.Abs(val31.y) > 0.01f || (Mathf.Abs(val32.y) > 0.01f && val29.y * val30.y < 0f) || Mathf.Abs(val32.z) - Mathf.Abs(val31.z) > 0.01f || (Mathf.Abs(val32.z) > 0.01f && val29.z * val30.z < 0f) || (flag3 && val19.x == 0f && val19.z == 0f && component.DownType == DungeonGridLinkType.Default && ((Mathf.Abs(val30.x) > 0.01f && Mathf.Abs(val30.x) < LinkRadius * 2f - 0.1f) || (Mathf.Abs(val30.z) > 0.01f && Mathf.Abs(val30.z) < LinkRadius * 2f - 0.1f))))
						{
							continue;
						}
						num18 = num21;
						if (val19.x == 0f && val19.z == 0f)
						{
							if (!flag3 && Mathf.Abs(val30.y) < LinkTransition - 0.1f)
							{
								continue;
							}
						}
						else if ((!flag3 && magnitude4 > 0.01f && (Mathf.Abs(val30.x) < LinkRadius * 2f - 0.1f || Mathf.Abs(val30.z) < LinkRadius * 2f - 0.1f)) || (!flag4 && magnitude4 > 0.01f && (Mathf.Abs(val30.x) < LinkRadius * 1f - 0.1f || Mathf.Abs(val30.z) < LinkRadius * 1f - 0.1f)))
						{
							continue;
						}
						if (!flag3 || !(magnitude4 < 0.01f) || !(magnitude2 < 0.01f) || !(Quaternion.Angle(val25, val24) > 10f))
						{
							prefab19 = prefab20;
							val20 = val28;
							num18 = num21;
							position3 = val27;
							rotation2 = val23;
						}
					}
					if (val20 != Vector3.zero)
					{
						PathLinkSegment pathLinkSegment = new PathLinkSegment();
						pathLinkSegment.position = position3;
						pathLinkSegment.rotation = rotation2;
						pathLinkSegment.scale = Vector3.one;
						pathLinkSegment.prefab = prefab19;
						pathLinkSegment.link = prefab19.Component;
						item5.downwards.segments.Add(pathLinkSegment);
						val18 += val20;
					}
					else
					{
						num17++;
					}
					if (val19.x > 0f || val19.z > 0f)
					{
						Prefab<DungeonGridLink> prefab21 = null;
						Vector3 val33 = Vector3.zero;
						int num23 = int.MinValue;
						Vector3 position4 = Vector3.zero;
						Quaternion rotation3 = Quaternion.identity;
						PathLinkSegment prevSegment3 = item5.upwards.prevSegment;
						Vector3 val34 = prevSegment3.position + prevSegment3.rotation * Vector3.Scale(prevSegment3.scale, prevSegment3.upSocket.localPosition);
						Quaternion val35 = prevSegment3.rotation * prevSegment3.upSocket.localRotation;
						Prefab<DungeonGridLink>[] array13 = array4;
						foreach (Prefab<DungeonGridLink> prefab22 in array13)
						{
							float num25 = SeedRandom.Value(ref seed);
							DungeonGridLink component2 = prefab22.Component;
							if (prevSegment3.upType != component2.DownType)
							{
								continue;
							}
							switch (component2.DownType)
							{
							case DungeonGridLinkType.Elevator:
								if (flag3 || val19.x != 0f || val19.z != 0f)
								{
									continue;
								}
								break;
							case DungeonGridLinkType.Transition:
								if (val19.x != 0f || val19.z != 0f)
								{
									continue;
								}
								break;
							}
							int num26 = ((!flag3) ? component2.Priority : 0);
							if (num23 > num26)
							{
								continue;
							}
							Quaternion val36 = val35 * Quaternion.Inverse(component2.DownSocket.localRotation);
							Quaternion val37 = val36 * component2.UpSocket.localRotation;
							PathLinkSegment prevSegment4 = item5.downwards.prevSegment;
							Quaternion val38 = prevSegment4.rotation * prevSegment4.downSocket.localRotation;
							if (component2.Rotation > 0)
							{
								float num27 = Quaternion.Angle(val38, val37);
								if (num27 > (float)component2.Rotation)
								{
									continue;
								}
								Quaternion val39 = val38 * Quaternion.Inverse(val37);
								val36 *= val39;
								val37 *= val39;
							}
							Vector3 val40 = val34 - val36 * component2.DownSocket.localPosition;
							Vector3 val41 = val36 * (component2.UpSocket.localPosition - component2.DownSocket.localPosition);
							Vector3 val42 = val18 - val33;
							Vector3 val43 = val18 - val41;
							float magnitude5 = ((Vector3)(ref val42)).magnitude;
							float magnitude6 = ((Vector3)(ref val43)).magnitude;
							Vector3 val44 = Vector3.Scale(val42, val19);
							Vector3 val45 = Vector3.Scale(val43, val19);
							float magnitude7 = ((Vector3)(ref val44)).magnitude;
							float magnitude8 = ((Vector3)(ref val45)).magnitude;
							if (val33 != Vector3.zero)
							{
								if (magnitude7 < magnitude8 || (magnitude7 == magnitude8 && magnitude5 < magnitude6) || (magnitude7 == magnitude8 && magnitude5 == magnitude6 && num25 < 0.5f))
								{
									continue;
								}
							}
							else if (magnitude7 <= magnitude8)
							{
								continue;
							}
							if (Mathf.Abs(val45.x) - Mathf.Abs(val44.x) > 0.01f || (Mathf.Abs(val45.x) > 0.01f && val42.x * val43.x < 0f) || Mathf.Abs(val45.y) - Mathf.Abs(val44.y) > 0.01f || (Mathf.Abs(val45.y) > 0.01f && val42.y * val43.y < 0f) || Mathf.Abs(val45.z) - Mathf.Abs(val44.z) > 0.01f || (Mathf.Abs(val45.z) > 0.01f && val42.z * val43.z < 0f) || (flag3 && val19.x == 0f && val19.z == 0f && component2.UpType == DungeonGridLinkType.Default && ((Mathf.Abs(val43.x) > 0.01f && Mathf.Abs(val43.x) < LinkRadius * 2f - 0.1f) || (Mathf.Abs(val43.z) > 0.01f && Mathf.Abs(val43.z) < LinkRadius * 2f - 0.1f))))
							{
								continue;
							}
							num23 = num26;
							if (val19.x == 0f && val19.z == 0f)
							{
								if (!flag3 && Mathf.Abs(val43.y) < LinkTransition - 0.1f)
								{
									continue;
								}
							}
							else if ((!flag3 && magnitude8 > 0.01f && (Mathf.Abs(val43.x) < LinkRadius * 2f - 0.1f || Mathf.Abs(val43.z) < LinkRadius * 2f - 0.1f)) || (!flag4 && magnitude8 > 0.01f && (Mathf.Abs(val43.x) < LinkRadius * 1f - 0.1f || Mathf.Abs(val43.z) < LinkRadius * 1f - 0.1f)))
							{
								continue;
							}
							if (!flag3 || !(magnitude8 < 0.01f) || !(magnitude6 < 0.01f) || !(Quaternion.Angle(val38, val37) > 10f))
							{
								prefab21 = prefab22;
								val33 = val41;
								num23 = num26;
								position4 = val40;
								rotation3 = val36;
							}
						}
						if (val33 != Vector3.zero)
						{
							PathLinkSegment pathLinkSegment2 = new PathLinkSegment();
							pathLinkSegment2.position = position4;
							pathLinkSegment2.rotation = rotation3;
							pathLinkSegment2.scale = Vector3.one;
							pathLinkSegment2.prefab = prefab21;
							pathLinkSegment2.link = prefab21.Component;
							item5.upwards.segments.Add(pathLinkSegment2);
							val18 -= val33;
						}
						else
						{
							num16++;
						}
					}
					else
					{
						num16++;
					}
				}
			}
		}
		foreach (PathLink item6 in list3)
		{
			foreach (PathLinkSegment segment2 in item6.downwards.segments)
			{
				World.AddPrefab("Dungeon", segment2.prefab, segment2.position, segment2.rotation, segment2.scale);
			}
			foreach (PathLinkSegment segment3 in item6.upwards.segments)
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
