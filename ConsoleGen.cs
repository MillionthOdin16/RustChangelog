using System.Collections.Generic;
using CompanionServer;
using CompanionServer.Cameras;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Network;
using Facepunch.Rust;
using Rust.Ai;
using UnityEngine;

public class ConsoleGen
{
	public static Command[] All;

	static ConsoleGen()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Expected O, but got Unknown
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Expected O, but got Unknown
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Expected O, but got Unknown
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Expected O, but got Unknown
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Expected O, but got Unknown
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Expected O, but got Unknown
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Expected O, but got Unknown
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Expected O, but got Unknown
		//IL_0577: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Expected O, but got Unknown
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Expected O, but got Unknown
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Expected O, but got Unknown
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Expected O, but got Unknown
		//IL_071a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Expected O, but got Unknown
		//IL_078b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Expected O, but got Unknown
		//IL_07f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f7: Expected O, but got Unknown
		//IL_0857: Unknown result type (might be due to invalid IL or missing references)
		//IL_085d: Expected O, but got Unknown
		//IL_08c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ce: Expected O, but got Unknown
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0934: Expected O, but got Unknown
		//IL_0994: Unknown result type (might be due to invalid IL or missing references)
		//IL_099a: Expected O, but got Unknown
		//IL_09fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a00: Expected O, but got Unknown
		//IL_0a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a66: Expected O, but got Unknown
		//IL_0ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Expected O, but got Unknown
		//IL_0b37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b3d: Expected O, but got Unknown
		//IL_0b9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ba3: Expected O, but got Unknown
		//IL_0c0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c14: Expected O, but got Unknown
		//IL_0c7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c85: Expected O, but got Unknown
		//IL_0cf0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf6: Expected O, but got Unknown
		//IL_0d68: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d6e: Expected O, but got Unknown
		//IL_0dce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dd4: Expected O, but got Unknown
		//IL_0e3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e45: Expected O, but got Unknown
		//IL_0eb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ebd: Expected O, but got Unknown
		//IL_0f24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f2a: Expected O, but got Unknown
		//IL_0f74: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f7a: Expected O, but got Unknown
		//IL_1013: Unknown result type (might be due to invalid IL or missing references)
		//IL_1019: Expected O, but got Unknown
		//IL_1063: Unknown result type (might be due to invalid IL or missing references)
		//IL_1069: Expected O, but got Unknown
		//IL_10c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cf: Expected O, but got Unknown
		//IL_112f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1135: Expected O, but got Unknown
		//IL_1195: Unknown result type (might be due to invalid IL or missing references)
		//IL_119b: Expected O, but got Unknown
		//IL_11fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1201: Expected O, but got Unknown
		//IL_1261: Unknown result type (might be due to invalid IL or missing references)
		//IL_1267: Expected O, but got Unknown
		//IL_12b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b7: Expected O, but got Unknown
		//IL_1301: Unknown result type (might be due to invalid IL or missing references)
		//IL_1307: Expected O, but got Unknown
		//IL_1387: Unknown result type (might be due to invalid IL or missing references)
		//IL_138d: Expected O, but got Unknown
		//IL_13ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1405: Expected O, but got Unknown
		//IL_1470: Unknown result type (might be due to invalid IL or missing references)
		//IL_1476: Expected O, but got Unknown
		//IL_14e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e7: Expected O, but got Unknown
		//IL_1547: Unknown result type (might be due to invalid IL or missing references)
		//IL_154d: Expected O, but got Unknown
		//IL_15ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b3: Expected O, but got Unknown
		//IL_15fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1603: Expected O, but got Unknown
		//IL_164d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1653: Expected O, but got Unknown
		//IL_169d: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a3: Expected O, but got Unknown
		//IL_1703: Unknown result type (might be due to invalid IL or missing references)
		//IL_1709: Expected O, but got Unknown
		//IL_1769: Unknown result type (might be due to invalid IL or missing references)
		//IL_176f: Expected O, but got Unknown
		//IL_17cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d5: Expected O, but got Unknown
		//IL_1835: Unknown result type (might be due to invalid IL or missing references)
		//IL_183b: Expected O, but got Unknown
		//IL_189b: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a1: Expected O, but got Unknown
		//IL_1901: Unknown result type (might be due to invalid IL or missing references)
		//IL_1907: Expected O, but got Unknown
		//IL_1967: Unknown result type (might be due to invalid IL or missing references)
		//IL_196d: Expected O, but got Unknown
		//IL_19cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d3: Expected O, but got Unknown
		//IL_1a33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a39: Expected O, but got Unknown
		//IL_1a99: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9f: Expected O, but got Unknown
		//IL_1aff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b05: Expected O, but got Unknown
		//IL_1b65: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b6b: Expected O, but got Unknown
		//IL_1bcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd1: Expected O, but got Unknown
		//IL_1c31: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c37: Expected O, but got Unknown
		//IL_1c97: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c9d: Expected O, but got Unknown
		//IL_1cfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d03: Expected O, but got Unknown
		//IL_1d4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d53: Expected O, but got Unknown
		//IL_1d9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da3: Expected O, but got Unknown
		//IL_1ded: Unknown result type (might be due to invalid IL or missing references)
		//IL_1df3: Expected O, but got Unknown
		//IL_1e3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e43: Expected O, but got Unknown
		//IL_1e8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e93: Expected O, but got Unknown
		//IL_1edd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ee3: Expected O, but got Unknown
		//IL_1f2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f33: Expected O, but got Unknown
		//IL_1f7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f83: Expected O, but got Unknown
		//IL_200e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2014: Expected O, but got Unknown
		//IL_2069: Unknown result type (might be due to invalid IL or missing references)
		//IL_206f: Expected O, but got Unknown
		//IL_20b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_20bf: Expected O, but got Unknown
		//IL_2114: Unknown result type (might be due to invalid IL or missing references)
		//IL_211a: Expected O, but got Unknown
		//IL_216f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2175: Expected O, but got Unknown
		//IL_21ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_21d0: Expected O, but got Unknown
		//IL_2225: Unknown result type (might be due to invalid IL or missing references)
		//IL_222b: Expected O, but got Unknown
		//IL_2280: Unknown result type (might be due to invalid IL or missing references)
		//IL_2286: Expected O, but got Unknown
		//IL_22db: Unknown result type (might be due to invalid IL or missing references)
		//IL_22e1: Expected O, but got Unknown
		//IL_2336: Unknown result type (might be due to invalid IL or missing references)
		//IL_233c: Expected O, but got Unknown
		//IL_2386: Unknown result type (might be due to invalid IL or missing references)
		//IL_238c: Expected O, but got Unknown
		//IL_23d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_23dc: Expected O, but got Unknown
		//IL_2426: Unknown result type (might be due to invalid IL or missing references)
		//IL_242c: Expected O, but got Unknown
		//IL_2476: Unknown result type (might be due to invalid IL or missing references)
		//IL_247c: Expected O, but got Unknown
		//IL_24c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_24cc: Expected O, but got Unknown
		//IL_2521: Unknown result type (might be due to invalid IL or missing references)
		//IL_2527: Expected O, but got Unknown
		//IL_2571: Unknown result type (might be due to invalid IL or missing references)
		//IL_2577: Expected O, but got Unknown
		//IL_25c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_25c7: Expected O, but got Unknown
		//IL_2611: Unknown result type (might be due to invalid IL or missing references)
		//IL_2617: Expected O, but got Unknown
		//IL_2661: Unknown result type (might be due to invalid IL or missing references)
		//IL_2667: Expected O, but got Unknown
		//IL_26b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_26b7: Expected O, but got Unknown
		//IL_2701: Unknown result type (might be due to invalid IL or missing references)
		//IL_2707: Expected O, but got Unknown
		//IL_275c: Unknown result type (might be due to invalid IL or missing references)
		//IL_2762: Expected O, but got Unknown
		//IL_27ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_27b2: Expected O, but got Unknown
		//IL_27fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2802: Expected O, but got Unknown
		//IL_2857: Unknown result type (might be due to invalid IL or missing references)
		//IL_285d: Expected O, but got Unknown
		//IL_28a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_28ad: Expected O, but got Unknown
		//IL_2902: Unknown result type (might be due to invalid IL or missing references)
		//IL_2908: Expected O, but got Unknown
		//IL_295d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2963: Expected O, but got Unknown
		//IL_29ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_29b3: Expected O, but got Unknown
		//IL_29fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a03: Expected O, but got Unknown
		//IL_2a4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a53: Expected O, but got Unknown
		//IL_2aa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2aae: Expected O, but got Unknown
		//IL_2b03: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b09: Expected O, but got Unknown
		//IL_2b5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b64: Expected O, but got Unknown
		//IL_2bb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bbf: Expected O, but got Unknown
		//IL_2c09: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c0f: Expected O, but got Unknown
		//IL_2c64: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c6a: Expected O, but got Unknown
		//IL_2cbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_2cc5: Expected O, but got Unknown
		//IL_2d1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d20: Expected O, but got Unknown
		//IL_2d75: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d7b: Expected O, but got Unknown
		//IL_2dd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dd6: Expected O, but got Unknown
		//IL_2e20: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e26: Expected O, but got Unknown
		//IL_2e70: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e76: Expected O, but got Unknown
		//IL_2ec0: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ec6: Expected O, but got Unknown
		//IL_2f1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f21: Expected O, but got Unknown
		//IL_2f76: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f7c: Expected O, but got Unknown
		//IL_2fd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fda: Expected O, but got Unknown
		//IL_3032: Unknown result type (might be due to invalid IL or missing references)
		//IL_3038: Expected O, but got Unknown
		//IL_309b: Unknown result type (might be due to invalid IL or missing references)
		//IL_30a1: Expected O, but got Unknown
		//IL_30f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_30ff: Expected O, but got Unknown
		//IL_3189: Unknown result type (might be due to invalid IL or missing references)
		//IL_318f: Expected O, but got Unknown
		//IL_31fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_3203: Expected O, but got Unknown
		//IL_3250: Unknown result type (might be due to invalid IL or missing references)
		//IL_3256: Expected O, but got Unknown
		//IL_32ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_32b4: Expected O, but got Unknown
		//IL_3317: Unknown result type (might be due to invalid IL or missing references)
		//IL_331d: Expected O, but got Unknown
		//IL_3380: Unknown result type (might be due to invalid IL or missing references)
		//IL_3386: Expected O, but got Unknown
		//IL_33e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_33ef: Expected O, but got Unknown
		//IL_343c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3442: Expected O, but got Unknown
		//IL_348f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3495: Expected O, but got Unknown
		//IL_34f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_34fe: Expected O, but got Unknown
		//IL_356c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3572: Expected O, but got Unknown
		//IL_35e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_35e6: Expected O, but got Unknown
		//IL_3654: Unknown result type (might be due to invalid IL or missing references)
		//IL_365a: Expected O, but got Unknown
		//IL_36c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_36ce: Expected O, but got Unknown
		//IL_373c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3742: Expected O, but got Unknown
		//IL_37a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_37ab: Expected O, but got Unknown
		//IL_3819: Unknown result type (might be due to invalid IL or missing references)
		//IL_381f: Expected O, but got Unknown
		//IL_388d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3893: Expected O, but got Unknown
		//IL_3901: Unknown result type (might be due to invalid IL or missing references)
		//IL_3907: Expected O, but got Unknown
		//IL_3975: Unknown result type (might be due to invalid IL or missing references)
		//IL_397b: Expected O, but got Unknown
		//IL_39e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_39ef: Expected O, but got Unknown
		//IL_3a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a63: Expected O, but got Unknown
		//IL_3ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ad7: Expected O, but got Unknown
		//IL_3b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b4b: Expected O, but got Unknown
		//IL_3bb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bbf: Expected O, but got Unknown
		//IL_3c2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c33: Expected O, but got Unknown
		//IL_3ca1: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ca7: Expected O, but got Unknown
		//IL_3d15: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d1b: Expected O, but got Unknown
		//IL_3d89: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d8f: Expected O, but got Unknown
		//IL_3dfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e03: Expected O, but got Unknown
		//IL_3e71: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e77: Expected O, but got Unknown
		//IL_3ee5: Unknown result type (might be due to invalid IL or missing references)
		//IL_3eeb: Expected O, but got Unknown
		//IL_3f59: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f5f: Expected O, but got Unknown
		//IL_3fcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_3fd3: Expected O, but got Unknown
		//IL_4041: Unknown result type (might be due to invalid IL or missing references)
		//IL_4047: Expected O, but got Unknown
		//IL_40b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_40bb: Expected O, but got Unknown
		//IL_4129: Unknown result type (might be due to invalid IL or missing references)
		//IL_412f: Expected O, but got Unknown
		//IL_419d: Unknown result type (might be due to invalid IL or missing references)
		//IL_41a3: Expected O, but got Unknown
		//IL_4211: Unknown result type (might be due to invalid IL or missing references)
		//IL_4217: Expected O, but got Unknown
		//IL_4285: Unknown result type (might be due to invalid IL or missing references)
		//IL_428b: Expected O, but got Unknown
		//IL_42f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_42ff: Expected O, but got Unknown
		//IL_436d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4373: Expected O, but got Unknown
		//IL_43e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_43e7: Expected O, but got Unknown
		//IL_4455: Unknown result type (might be due to invalid IL or missing references)
		//IL_445b: Expected O, but got Unknown
		//IL_44c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_44cf: Expected O, but got Unknown
		//IL_453d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4543: Expected O, but got Unknown
		//IL_45b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_45b7: Expected O, but got Unknown
		//IL_4625: Unknown result type (might be due to invalid IL or missing references)
		//IL_462b: Expected O, but got Unknown
		//IL_4699: Unknown result type (might be due to invalid IL or missing references)
		//IL_469f: Expected O, but got Unknown
		//IL_470d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4713: Expected O, but got Unknown
		//IL_4781: Unknown result type (might be due to invalid IL or missing references)
		//IL_4787: Expected O, but got Unknown
		//IL_47f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_47fb: Expected O, but got Unknown
		//IL_4869: Unknown result type (might be due to invalid IL or missing references)
		//IL_486f: Expected O, but got Unknown
		//IL_48dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_48e3: Expected O, but got Unknown
		//IL_4951: Unknown result type (might be due to invalid IL or missing references)
		//IL_4957: Expected O, but got Unknown
		//IL_49c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_49cb: Expected O, but got Unknown
		//IL_4a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a3f: Expected O, but got Unknown
		//IL_4aad: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ab3: Expected O, but got Unknown
		//IL_4b16: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b1c: Expected O, but got Unknown
		//IL_4b7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b85: Expected O, but got Unknown
		//IL_4bdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_4be3: Expected O, but got Unknown
		//IL_4c3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c41: Expected O, but got Unknown
		//IL_4c8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c94: Expected O, but got Unknown
		//IL_4cf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_4cfd: Expected O, but got Unknown
		//IL_4d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d66: Expected O, but got Unknown
		//IL_4dc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_4dcf: Expected O, but got Unknown
		//IL_4e1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e22: Expected O, but got Unknown
		//IL_4e85: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e8b: Expected O, but got Unknown
		//IL_4eee: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ef4: Expected O, but got Unknown
		//IL_4f57: Unknown result type (might be due to invalid IL or missing references)
		//IL_4f5d: Expected O, but got Unknown
		//IL_4fc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_4fc6: Expected O, but got Unknown
		//IL_5029: Unknown result type (might be due to invalid IL or missing references)
		//IL_502f: Expected O, but got Unknown
		//IL_5092: Unknown result type (might be due to invalid IL or missing references)
		//IL_5098: Expected O, but got Unknown
		//IL_50e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_50eb: Expected O, but got Unknown
		//IL_514e: Unknown result type (might be due to invalid IL or missing references)
		//IL_5154: Expected O, but got Unknown
		//IL_51b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_51bd: Expected O, but got Unknown
		//IL_5220: Unknown result type (might be due to invalid IL or missing references)
		//IL_5226: Expected O, but got Unknown
		//IL_5289: Unknown result type (might be due to invalid IL or missing references)
		//IL_528f: Expected O, but got Unknown
		//IL_52f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_52f8: Expected O, but got Unknown
		//IL_535b: Unknown result type (might be due to invalid IL or missing references)
		//IL_5361: Expected O, but got Unknown
		//IL_53c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_53ca: Expected O, but got Unknown
		//IL_542d: Unknown result type (might be due to invalid IL or missing references)
		//IL_5433: Expected O, but got Unknown
		//IL_5496: Unknown result type (might be due to invalid IL or missing references)
		//IL_549c: Expected O, but got Unknown
		//IL_54ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_5505: Expected O, but got Unknown
		//IL_5568: Unknown result type (might be due to invalid IL or missing references)
		//IL_556e: Expected O, but got Unknown
		//IL_55d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_55d7: Expected O, but got Unknown
		//IL_563a: Unknown result type (might be due to invalid IL or missing references)
		//IL_5640: Expected O, but got Unknown
		//IL_56a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_56a9: Expected O, but got Unknown
		//IL_570c: Unknown result type (might be due to invalid IL or missing references)
		//IL_5712: Expected O, but got Unknown
		//IL_5775: Unknown result type (might be due to invalid IL or missing references)
		//IL_577b: Expected O, but got Unknown
		//IL_57de: Unknown result type (might be due to invalid IL or missing references)
		//IL_57e4: Expected O, but got Unknown
		//IL_5847: Unknown result type (might be due to invalid IL or missing references)
		//IL_584d: Expected O, but got Unknown
		//IL_58b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_58b6: Expected O, but got Unknown
		//IL_5919: Unknown result type (might be due to invalid IL or missing references)
		//IL_591f: Expected O, but got Unknown
		//IL_5982: Unknown result type (might be due to invalid IL or missing references)
		//IL_5988: Expected O, but got Unknown
		//IL_59eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_59f1: Expected O, but got Unknown
		//IL_5a54: Unknown result type (might be due to invalid IL or missing references)
		//IL_5a5a: Expected O, but got Unknown
		//IL_5abd: Unknown result type (might be due to invalid IL or missing references)
		//IL_5ac3: Expected O, but got Unknown
		//IL_5b26: Unknown result type (might be due to invalid IL or missing references)
		//IL_5b2c: Expected O, but got Unknown
		//IL_5b8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_5b95: Expected O, but got Unknown
		//IL_5bf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_5bfe: Expected O, but got Unknown
		//IL_5c61: Unknown result type (might be due to invalid IL or missing references)
		//IL_5c67: Expected O, but got Unknown
		//IL_5cca: Unknown result type (might be due to invalid IL or missing references)
		//IL_5cd0: Expected O, but got Unknown
		//IL_5d33: Unknown result type (might be due to invalid IL or missing references)
		//IL_5d39: Expected O, but got Unknown
		//IL_5d9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_5da2: Expected O, but got Unknown
		//IL_5e05: Unknown result type (might be due to invalid IL or missing references)
		//IL_5e0b: Expected O, but got Unknown
		//IL_5e6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_5e74: Expected O, but got Unknown
		//IL_5ed7: Unknown result type (might be due to invalid IL or missing references)
		//IL_5edd: Expected O, but got Unknown
		//IL_5f40: Unknown result type (might be due to invalid IL or missing references)
		//IL_5f46: Expected O, but got Unknown
		//IL_5fa9: Unknown result type (might be due to invalid IL or missing references)
		//IL_5faf: Expected O, but got Unknown
		//IL_6012: Unknown result type (might be due to invalid IL or missing references)
		//IL_6018: Expected O, but got Unknown
		//IL_607b: Unknown result type (might be due to invalid IL or missing references)
		//IL_6081: Expected O, but got Unknown
		//IL_60e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_60ea: Expected O, but got Unknown
		//IL_614d: Unknown result type (might be due to invalid IL or missing references)
		//IL_6153: Expected O, but got Unknown
		//IL_61b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_61bc: Expected O, but got Unknown
		//IL_621f: Unknown result type (might be due to invalid IL or missing references)
		//IL_6225: Expected O, but got Unknown
		//IL_6288: Unknown result type (might be due to invalid IL or missing references)
		//IL_628e: Expected O, but got Unknown
		//IL_62f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_62f7: Expected O, but got Unknown
		//IL_635a: Unknown result type (might be due to invalid IL or missing references)
		//IL_6360: Expected O, but got Unknown
		//IL_63c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_63c9: Expected O, but got Unknown
		//IL_644c: Unknown result type (might be due to invalid IL or missing references)
		//IL_6452: Expected O, but got Unknown
		//IL_64b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_64bb: Expected O, but got Unknown
		//IL_653e: Unknown result type (might be due to invalid IL or missing references)
		//IL_6544: Expected O, but got Unknown
		//IL_65a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_65ad: Expected O, but got Unknown
		//IL_6610: Unknown result type (might be due to invalid IL or missing references)
		//IL_6616: Expected O, but got Unknown
		//IL_6679: Unknown result type (might be due to invalid IL or missing references)
		//IL_667f: Expected O, but got Unknown
		//IL_66e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_66e8: Expected O, but got Unknown
		//IL_674b: Unknown result type (might be due to invalid IL or missing references)
		//IL_6751: Expected O, but got Unknown
		//IL_67b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_67ba: Expected O, but got Unknown
		//IL_681d: Unknown result type (might be due to invalid IL or missing references)
		//IL_6823: Expected O, but got Unknown
		//IL_6886: Unknown result type (might be due to invalid IL or missing references)
		//IL_688c: Expected O, but got Unknown
		//IL_68ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_68f5: Expected O, but got Unknown
		//IL_6958: Unknown result type (might be due to invalid IL or missing references)
		//IL_695e: Expected O, but got Unknown
		//IL_69c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_69c7: Expected O, but got Unknown
		//IL_6a2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_6a30: Expected O, but got Unknown
		//IL_6a93: Unknown result type (might be due to invalid IL or missing references)
		//IL_6a99: Expected O, but got Unknown
		//IL_6afc: Unknown result type (might be due to invalid IL or missing references)
		//IL_6b02: Expected O, but got Unknown
		//IL_6b65: Unknown result type (might be due to invalid IL or missing references)
		//IL_6b6b: Expected O, but got Unknown
		//IL_6bce: Unknown result type (might be due to invalid IL or missing references)
		//IL_6bd4: Expected O, but got Unknown
		//IL_6c37: Unknown result type (might be due to invalid IL or missing references)
		//IL_6c3d: Expected O, but got Unknown
		//IL_6ca0: Unknown result type (might be due to invalid IL or missing references)
		//IL_6ca6: Expected O, but got Unknown
		//IL_6d09: Unknown result type (might be due to invalid IL or missing references)
		//IL_6d0f: Expected O, but got Unknown
		//IL_6d72: Unknown result type (might be due to invalid IL or missing references)
		//IL_6d78: Expected O, but got Unknown
		//IL_6ddb: Unknown result type (might be due to invalid IL or missing references)
		//IL_6de1: Expected O, but got Unknown
		//IL_6e44: Unknown result type (might be due to invalid IL or missing references)
		//IL_6e4a: Expected O, but got Unknown
		//IL_6ead: Unknown result type (might be due to invalid IL or missing references)
		//IL_6eb3: Expected O, but got Unknown
		//IL_6f16: Unknown result type (might be due to invalid IL or missing references)
		//IL_6f1c: Expected O, but got Unknown
		//IL_6f7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_6f85: Expected O, but got Unknown
		//IL_6fe8: Unknown result type (might be due to invalid IL or missing references)
		//IL_6fee: Expected O, but got Unknown
		//IL_7051: Unknown result type (might be due to invalid IL or missing references)
		//IL_7057: Expected O, but got Unknown
		//IL_70ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_70c0: Expected O, but got Unknown
		//IL_7123: Unknown result type (might be due to invalid IL or missing references)
		//IL_7129: Expected O, but got Unknown
		//IL_718c: Unknown result type (might be due to invalid IL or missing references)
		//IL_7192: Expected O, but got Unknown
		//IL_71f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_71fb: Expected O, but got Unknown
		//IL_725e: Unknown result type (might be due to invalid IL or missing references)
		//IL_7264: Expected O, but got Unknown
		//IL_72c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_72cd: Expected O, but got Unknown
		//IL_7330: Unknown result type (might be due to invalid IL or missing references)
		//IL_7336: Expected O, but got Unknown
		//IL_7399: Unknown result type (might be due to invalid IL or missing references)
		//IL_739f: Expected O, but got Unknown
		//IL_7402: Unknown result type (might be due to invalid IL or missing references)
		//IL_7408: Expected O, but got Unknown
		//IL_746b: Unknown result type (might be due to invalid IL or missing references)
		//IL_7471: Expected O, but got Unknown
		//IL_74d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_74da: Expected O, but got Unknown
		//IL_753d: Unknown result type (might be due to invalid IL or missing references)
		//IL_7543: Expected O, but got Unknown
		//IL_75a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_75ac: Expected O, but got Unknown
		//IL_761a: Unknown result type (might be due to invalid IL or missing references)
		//IL_7620: Expected O, but got Unknown
		//IL_766d: Unknown result type (might be due to invalid IL or missing references)
		//IL_7673: Expected O, but got Unknown
		//IL_76c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_76c6: Expected O, but got Unknown
		//IL_7713: Unknown result type (might be due to invalid IL or missing references)
		//IL_7719: Expected O, but got Unknown
		//IL_7766: Unknown result type (might be due to invalid IL or missing references)
		//IL_776c: Expected O, but got Unknown
		//IL_77cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_77d5: Expected O, but got Unknown
		//IL_7838: Unknown result type (might be due to invalid IL or missing references)
		//IL_783e: Expected O, but got Unknown
		//IL_78a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_78a7: Expected O, but got Unknown
		//IL_790a: Unknown result type (might be due to invalid IL or missing references)
		//IL_7910: Expected O, but got Unknown
		//IL_797e: Unknown result type (might be due to invalid IL or missing references)
		//IL_7984: Expected O, but got Unknown
		//IL_79d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_79d7: Expected O, but got Unknown
		//IL_7a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_7a40: Expected O, but got Unknown
		//IL_7aa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_7aa9: Expected O, but got Unknown
		//IL_7b17: Unknown result type (might be due to invalid IL or missing references)
		//IL_7b1d: Expected O, but got Unknown
		//IL_7b6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_7b70: Expected O, but got Unknown
		//IL_7bbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_7bc3: Expected O, but got Unknown
		//IL_7c1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_7c21: Expected O, but got Unknown
		//IL_7ca4: Unknown result type (might be due to invalid IL or missing references)
		//IL_7caa: Expected O, but got Unknown
		//IL_7d18: Unknown result type (might be due to invalid IL or missing references)
		//IL_7d1e: Expected O, but got Unknown
		//IL_7d81: Unknown result type (might be due to invalid IL or missing references)
		//IL_7d87: Expected O, but got Unknown
		//IL_7dea: Unknown result type (might be due to invalid IL or missing references)
		//IL_7df0: Expected O, but got Unknown
		//IL_7e3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_7e43: Expected O, but got Unknown
		//IL_7ea6: Unknown result type (might be due to invalid IL or missing references)
		//IL_7eac: Expected O, but got Unknown
		//IL_7f0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_7f15: Expected O, but got Unknown
		//IL_7f62: Unknown result type (might be due to invalid IL or missing references)
		//IL_7f68: Expected O, but got Unknown
		//IL_7fb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_7fbb: Expected O, but got Unknown
		//IL_801e: Unknown result type (might be due to invalid IL or missing references)
		//IL_8024: Expected O, but got Unknown
		//IL_80a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_80ad: Expected O, but got Unknown
		//IL_811b: Unknown result type (might be due to invalid IL or missing references)
		//IL_8121: Expected O, but got Unknown
		//IL_81a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_81aa: Expected O, but got Unknown
		//IL_820d: Unknown result type (might be due to invalid IL or missing references)
		//IL_8213: Expected O, but got Unknown
		//IL_8260: Unknown result type (might be due to invalid IL or missing references)
		//IL_8266: Expected O, but got Unknown
		//IL_82b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_82b9: Expected O, but got Unknown
		//IL_8306: Unknown result type (might be due to invalid IL or missing references)
		//IL_830c: Expected O, but got Unknown
		//IL_836f: Unknown result type (might be due to invalid IL or missing references)
		//IL_8375: Expected O, but got Unknown
		//IL_83c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_83c8: Expected O, but got Unknown
		//IL_8415: Unknown result type (might be due to invalid IL or missing references)
		//IL_841b: Expected O, but got Unknown
		//IL_8489: Unknown result type (might be due to invalid IL or missing references)
		//IL_848f: Expected O, but got Unknown
		//IL_84e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_84ed: Expected O, but got Unknown
		//IL_855b: Unknown result type (might be due to invalid IL or missing references)
		//IL_8561: Expected O, but got Unknown
		//IL_85ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_85b4: Expected O, but got Unknown
		//IL_8601: Unknown result type (might be due to invalid IL or missing references)
		//IL_8607: Expected O, but got Unknown
		//IL_866a: Unknown result type (might be due to invalid IL or missing references)
		//IL_8670: Expected O, but got Unknown
		//IL_86bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_86c3: Expected O, but got Unknown
		//IL_8710: Unknown result type (might be due to invalid IL or missing references)
		//IL_8716: Expected O, but got Unknown
		//IL_8763: Unknown result type (might be due to invalid IL or missing references)
		//IL_8769: Expected O, but got Unknown
		//IL_87b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_87bc: Expected O, but got Unknown
		//IL_881f: Unknown result type (might be due to invalid IL or missing references)
		//IL_8825: Expected O, but got Unknown
		//IL_8872: Unknown result type (might be due to invalid IL or missing references)
		//IL_8878: Expected O, but got Unknown
		//IL_88d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_88d6: Expected O, but got Unknown
		//IL_892e: Unknown result type (might be due to invalid IL or missing references)
		//IL_8934: Expected O, but got Unknown
		//IL_898c: Unknown result type (might be due to invalid IL or missing references)
		//IL_8992: Expected O, but got Unknown
		//IL_89f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_89fb: Expected O, but got Unknown
		//IL_8a5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_8a64: Expected O, but got Unknown
		//IL_8ac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_8acd: Expected O, but got Unknown
		//IL_8b30: Unknown result type (might be due to invalid IL or missing references)
		//IL_8b36: Expected O, but got Unknown
		//IL_8b83: Unknown result type (might be due to invalid IL or missing references)
		//IL_8b89: Expected O, but got Unknown
		//IL_8bf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_8bfd: Expected O, but got Unknown
		//IL_8c4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_8c50: Expected O, but got Unknown
		//IL_8c9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_8ca3: Expected O, but got Unknown
		//IL_8cf0: Unknown result type (might be due to invalid IL or missing references)
		//IL_8cf6: Expected O, but got Unknown
		//IL_8d4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_8d54: Expected O, but got Unknown
		//IL_8da1: Unknown result type (might be due to invalid IL or missing references)
		//IL_8da7: Expected O, but got Unknown
		//IL_8df4: Unknown result type (might be due to invalid IL or missing references)
		//IL_8dfa: Expected O, but got Unknown
		//IL_8e5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_8e63: Expected O, but got Unknown
		//IL_8ebb: Unknown result type (might be due to invalid IL or missing references)
		//IL_8ec1: Expected O, but got Unknown
		//IL_8f0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_8f14: Expected O, but got Unknown
		//IL_8f61: Unknown result type (might be due to invalid IL or missing references)
		//IL_8f67: Expected O, but got Unknown
		//IL_8fbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_8fc5: Expected O, but got Unknown
		//IL_9012: Unknown result type (might be due to invalid IL or missing references)
		//IL_9018: Expected O, but got Unknown
		//IL_9065: Unknown result type (might be due to invalid IL or missing references)
		//IL_906b: Expected O, but got Unknown
		//IL_90b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_90be: Expected O, but got Unknown
		//IL_910b: Unknown result type (might be due to invalid IL or missing references)
		//IL_9111: Expected O, but got Unknown
		//IL_915e: Unknown result type (might be due to invalid IL or missing references)
		//IL_9164: Expected O, but got Unknown
		//IL_91b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_91b7: Expected O, but got Unknown
		//IL_9204: Unknown result type (might be due to invalid IL or missing references)
		//IL_920a: Expected O, but got Unknown
		//IL_9257: Unknown result type (might be due to invalid IL or missing references)
		//IL_925d: Expected O, but got Unknown
		//IL_92cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_92d1: Expected O, but got Unknown
		//IL_933f: Unknown result type (might be due to invalid IL or missing references)
		//IL_9345: Expected O, but got Unknown
		//IL_93b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_93b9: Expected O, but got Unknown
		//IL_9427: Unknown result type (might be due to invalid IL or missing references)
		//IL_942d: Expected O, but got Unknown
		//IL_949b: Unknown result type (might be due to invalid IL or missing references)
		//IL_94a1: Expected O, but got Unknown
		//IL_950f: Unknown result type (might be due to invalid IL or missing references)
		//IL_9515: Expected O, but got Unknown
		//IL_9583: Unknown result type (might be due to invalid IL or missing references)
		//IL_9589: Expected O, but got Unknown
		//IL_95f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_95fd: Expected O, but got Unknown
		//IL_9660: Unknown result type (might be due to invalid IL or missing references)
		//IL_9666: Expected O, but got Unknown
		//IL_96d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_96da: Expected O, but got Unknown
		//IL_9748: Unknown result type (might be due to invalid IL or missing references)
		//IL_974e: Expected O, but got Unknown
		//IL_97bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_97c2: Expected O, but got Unknown
		//IL_9830: Unknown result type (might be due to invalid IL or missing references)
		//IL_9836: Expected O, but got Unknown
		//IL_98a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_98aa: Expected O, but got Unknown
		//IL_9918: Unknown result type (might be due to invalid IL or missing references)
		//IL_991e: Expected O, but got Unknown
		//IL_998c: Unknown result type (might be due to invalid IL or missing references)
		//IL_9992: Expected O, but got Unknown
		//IL_9a00: Unknown result type (might be due to invalid IL or missing references)
		//IL_9a06: Expected O, but got Unknown
		//IL_9a74: Unknown result type (might be due to invalid IL or missing references)
		//IL_9a7a: Expected O, but got Unknown
		//IL_9ae8: Unknown result type (might be due to invalid IL or missing references)
		//IL_9aee: Expected O, but got Unknown
		//IL_9b5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_9b62: Expected O, but got Unknown
		//IL_9bd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_9bd6: Expected O, but got Unknown
		//IL_9c44: Unknown result type (might be due to invalid IL or missing references)
		//IL_9c4a: Expected O, but got Unknown
		//IL_9cad: Unknown result type (might be due to invalid IL or missing references)
		//IL_9cb3: Expected O, but got Unknown
		//IL_9d16: Unknown result type (might be due to invalid IL or missing references)
		//IL_9d1c: Expected O, but got Unknown
		//IL_9d8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_9d90: Expected O, but got Unknown
		//IL_9dfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_9e04: Expected O, but got Unknown
		//IL_9e72: Unknown result type (might be due to invalid IL or missing references)
		//IL_9e78: Expected O, but got Unknown
		//IL_9ee6: Unknown result type (might be due to invalid IL or missing references)
		//IL_9eec: Expected O, but got Unknown
		//IL_9f5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_9f60: Expected O, but got Unknown
		//IL_9fad: Unknown result type (might be due to invalid IL or missing references)
		//IL_9fb3: Expected O, but got Unknown
		//IL_a01d: Unknown result type (might be due to invalid IL or missing references)
		//IL_a023: Expected O, but got Unknown
		//IL_a098: Unknown result type (might be due to invalid IL or missing references)
		//IL_a09e: Expected O, but got Unknown
		//IL_a101: Unknown result type (might be due to invalid IL or missing references)
		//IL_a107: Expected O, but got Unknown
		//IL_a16a: Unknown result type (might be due to invalid IL or missing references)
		//IL_a170: Expected O, but got Unknown
		//IL_a1bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_a1c3: Expected O, but got Unknown
		//IL_a210: Unknown result type (might be due to invalid IL or missing references)
		//IL_a216: Expected O, but got Unknown
		//IL_a26e: Unknown result type (might be due to invalid IL or missing references)
		//IL_a274: Expected O, but got Unknown
		//IL_a2cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_a2d2: Expected O, but got Unknown
		//IL_a31f: Unknown result type (might be due to invalid IL or missing references)
		//IL_a325: Expected O, but got Unknown
		//IL_a372: Unknown result type (might be due to invalid IL or missing references)
		//IL_a378: Expected O, but got Unknown
		//IL_a3c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_a3cb: Expected O, but got Unknown
		//IL_a418: Unknown result type (might be due to invalid IL or missing references)
		//IL_a41e: Expected O, but got Unknown
		//IL_a46b: Unknown result type (might be due to invalid IL or missing references)
		//IL_a471: Expected O, but got Unknown
		//IL_a4be: Unknown result type (might be due to invalid IL or missing references)
		//IL_a4c4: Expected O, but got Unknown
		//IL_a511: Unknown result type (might be due to invalid IL or missing references)
		//IL_a517: Expected O, but got Unknown
		//IL_a564: Unknown result type (might be due to invalid IL or missing references)
		//IL_a56a: Expected O, but got Unknown
		//IL_a5b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_a5bd: Expected O, but got Unknown
		//IL_a60a: Unknown result type (might be due to invalid IL or missing references)
		//IL_a610: Expected O, but got Unknown
		//IL_a65d: Unknown result type (might be due to invalid IL or missing references)
		//IL_a663: Expected O, but got Unknown
		//IL_a6b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_a6b6: Expected O, but got Unknown
		//IL_a703: Unknown result type (might be due to invalid IL or missing references)
		//IL_a709: Expected O, but got Unknown
		//IL_a76c: Unknown result type (might be due to invalid IL or missing references)
		//IL_a772: Expected O, but got Unknown
		//IL_a7d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_a7db: Expected O, but got Unknown
		//IL_a85e: Unknown result type (might be due to invalid IL or missing references)
		//IL_a864: Expected O, but got Unknown
		//IL_a8c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_a8cd: Expected O, but got Unknown
		//IL_a937: Unknown result type (might be due to invalid IL or missing references)
		//IL_a93d: Expected O, but got Unknown
		//IL_a9a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_a9a6: Expected O, but got Unknown
		//IL_aa10: Unknown result type (might be due to invalid IL or missing references)
		//IL_aa16: Expected O, but got Unknown
		//IL_aa63: Unknown result type (might be due to invalid IL or missing references)
		//IL_aa69: Expected O, but got Unknown
		//IL_aab6: Unknown result type (might be due to invalid IL or missing references)
		//IL_aabc: Expected O, but got Unknown
		//IL_ab09: Unknown result type (might be due to invalid IL or missing references)
		//IL_ab0f: Expected O, but got Unknown
		//IL_ab5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_ab62: Expected O, but got Unknown
		//IL_abc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_abcb: Expected O, but got Unknown
		//IL_ac2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_ac34: Expected O, but got Unknown
		//IL_ac97: Unknown result type (might be due to invalid IL or missing references)
		//IL_ac9d: Expected O, but got Unknown
		//IL_acea: Unknown result type (might be due to invalid IL or missing references)
		//IL_acf0: Expected O, but got Unknown
		//IL_ad53: Unknown result type (might be due to invalid IL or missing references)
		//IL_ad59: Expected O, but got Unknown
		//IL_ada6: Unknown result type (might be due to invalid IL or missing references)
		//IL_adac: Expected O, but got Unknown
		//IL_adf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_adff: Expected O, but got Unknown
		//IL_ae7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_ae85: Expected O, but got Unknown
		//IL_aed2: Unknown result type (might be due to invalid IL or missing references)
		//IL_aed8: Expected O, but got Unknown
		//IL_af25: Unknown result type (might be due to invalid IL or missing references)
		//IL_af2b: Expected O, but got Unknown
		//IL_af78: Unknown result type (might be due to invalid IL or missing references)
		//IL_af7e: Expected O, but got Unknown
		//IL_afcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_afd1: Expected O, but got Unknown
		//IL_b01e: Unknown result type (might be due to invalid IL or missing references)
		//IL_b024: Expected O, but got Unknown
		//IL_b071: Unknown result type (might be due to invalid IL or missing references)
		//IL_b077: Expected O, but got Unknown
		//IL_b0da: Unknown result type (might be due to invalid IL or missing references)
		//IL_b0e0: Expected O, but got Unknown
		//IL_b14e: Unknown result type (might be due to invalid IL or missing references)
		//IL_b154: Expected O, but got Unknown
		//IL_b1a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_b1a7: Expected O, but got Unknown
		//IL_b20a: Unknown result type (might be due to invalid IL or missing references)
		//IL_b210: Expected O, but got Unknown
		//IL_b25d: Unknown result type (might be due to invalid IL or missing references)
		//IL_b263: Expected O, but got Unknown
		//IL_b2b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_b2b6: Expected O, but got Unknown
		//IL_b303: Unknown result type (might be due to invalid IL or missing references)
		//IL_b309: Expected O, but got Unknown
		//IL_b385: Unknown result type (might be due to invalid IL or missing references)
		//IL_b38b: Expected O, but got Unknown
		//IL_b3ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_b3f4: Expected O, but got Unknown
		//IL_b441: Unknown result type (might be due to invalid IL or missing references)
		//IL_b447: Expected O, but got Unknown
		//IL_b4b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_b4b7: Expected O, but got Unknown
		//IL_b51a: Unknown result type (might be due to invalid IL or missing references)
		//IL_b520: Expected O, but got Unknown
		//IL_b56d: Unknown result type (might be due to invalid IL or missing references)
		//IL_b573: Expected O, but got Unknown
		//IL_b5c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_b5c6: Expected O, but got Unknown
		//IL_b613: Unknown result type (might be due to invalid IL or missing references)
		//IL_b619: Expected O, but got Unknown
		//IL_b666: Unknown result type (might be due to invalid IL or missing references)
		//IL_b66c: Expected O, but got Unknown
		//IL_b6b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_b6bf: Expected O, but got Unknown
		//IL_b70c: Unknown result type (might be due to invalid IL or missing references)
		//IL_b712: Expected O, but got Unknown
		//IL_b75f: Unknown result type (might be due to invalid IL or missing references)
		//IL_b765: Expected O, but got Unknown
		//IL_b7b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_b7b8: Expected O, but got Unknown
		//IL_b805: Unknown result type (might be due to invalid IL or missing references)
		//IL_b80b: Expected O, but got Unknown
		//IL_b86e: Unknown result type (might be due to invalid IL or missing references)
		//IL_b874: Expected O, but got Unknown
		//IL_b8c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_b8c7: Expected O, but got Unknown
		//IL_b914: Unknown result type (might be due to invalid IL or missing references)
		//IL_b91a: Expected O, but got Unknown
		//IL_b967: Unknown result type (might be due to invalid IL or missing references)
		//IL_b96d: Expected O, but got Unknown
		//IL_b9e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_b9ef: Expected O, but got Unknown
		//IL_ba6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_ba71: Expected O, but got Unknown
		//IL_babe: Unknown result type (might be due to invalid IL or missing references)
		//IL_bac4: Expected O, but got Unknown
		//IL_bb11: Unknown result type (might be due to invalid IL or missing references)
		//IL_bb17: Expected O, but got Unknown
		//IL_bb64: Unknown result type (might be due to invalid IL or missing references)
		//IL_bb6a: Expected O, but got Unknown
		//IL_bbb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_bbbd: Expected O, but got Unknown
		//IL_bc0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_bc10: Expected O, but got Unknown
		//IL_bc5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_bc63: Expected O, but got Unknown
		//IL_bcb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_bcb6: Expected O, but got Unknown
		//IL_bd03: Unknown result type (might be due to invalid IL or missing references)
		//IL_bd09: Expected O, but got Unknown
		//IL_bd56: Unknown result type (might be due to invalid IL or missing references)
		//IL_bd5c: Expected O, but got Unknown
		//IL_bda9: Unknown result type (might be due to invalid IL or missing references)
		//IL_bdaf: Expected O, but got Unknown
		//IL_bdfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_be02: Expected O, but got Unknown
		//IL_be4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_be55: Expected O, but got Unknown
		//IL_bea2: Unknown result type (might be due to invalid IL or missing references)
		//IL_bea8: Expected O, but got Unknown
		//IL_bef5: Unknown result type (might be due to invalid IL or missing references)
		//IL_befb: Expected O, but got Unknown
		//IL_bf48: Unknown result type (might be due to invalid IL or missing references)
		//IL_bf4e: Expected O, but got Unknown
		//IL_bfa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_bfa8: Expected O, but got Unknown
		//IL_c00b: Unknown result type (might be due to invalid IL or missing references)
		//IL_c011: Expected O, but got Unknown
		//IL_c074: Unknown result type (might be due to invalid IL or missing references)
		//IL_c07a: Expected O, but got Unknown
		//IL_c0e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_c0ee: Expected O, but got Unknown
		//IL_c15c: Unknown result type (might be due to invalid IL or missing references)
		//IL_c162: Expected O, but got Unknown
		//IL_c1d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_c1d6: Expected O, but got Unknown
		//IL_c244: Unknown result type (might be due to invalid IL or missing references)
		//IL_c24a: Expected O, but got Unknown
		//IL_c2b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_c2be: Expected O, but got Unknown
		//IL_c32c: Unknown result type (might be due to invalid IL or missing references)
		//IL_c332: Expected O, but got Unknown
		//IL_c3a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_c3a6: Expected O, but got Unknown
		//IL_c3f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_c3f9: Expected O, but got Unknown
		//IL_c446: Unknown result type (might be due to invalid IL or missing references)
		//IL_c44c: Expected O, but got Unknown
		//IL_c499: Unknown result type (might be due to invalid IL or missing references)
		//IL_c49f: Expected O, but got Unknown
		//IL_c4ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_c4f2: Expected O, but got Unknown
		//IL_c53f: Unknown result type (might be due to invalid IL or missing references)
		//IL_c545: Expected O, but got Unknown
		//IL_c59d: Unknown result type (might be due to invalid IL or missing references)
		//IL_c5a3: Expected O, but got Unknown
		//IL_c5fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_c601: Expected O, but got Unknown
		//IL_c64e: Unknown result type (might be due to invalid IL or missing references)
		//IL_c654: Expected O, but got Unknown
		//IL_c6ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_c6b2: Expected O, but got Unknown
		//IL_c70a: Unknown result type (might be due to invalid IL or missing references)
		//IL_c710: Expected O, but got Unknown
		//IL_c77e: Unknown result type (might be due to invalid IL or missing references)
		//IL_c784: Expected O, but got Unknown
		//IL_c7d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_c7d7: Expected O, but got Unknown
		//IL_c824: Unknown result type (might be due to invalid IL or missing references)
		//IL_c82a: Expected O, but got Unknown
		//IL_c877: Unknown result type (might be due to invalid IL or missing references)
		//IL_c87d: Expected O, but got Unknown
		//IL_c8ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_c8d0: Expected O, but got Unknown
		//IL_c91d: Unknown result type (might be due to invalid IL or missing references)
		//IL_c923: Expected O, but got Unknown
		//IL_c970: Unknown result type (might be due to invalid IL or missing references)
		//IL_c976: Expected O, but got Unknown
		//IL_c9c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_c9c9: Expected O, but got Unknown
		//IL_ca16: Unknown result type (might be due to invalid IL or missing references)
		//IL_ca1c: Expected O, but got Unknown
		//IL_ca69: Unknown result type (might be due to invalid IL or missing references)
		//IL_ca6f: Expected O, but got Unknown
		//IL_cabc: Unknown result type (might be due to invalid IL or missing references)
		//IL_cac2: Expected O, but got Unknown
		//IL_cb1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_cb20: Expected O, but got Unknown
		//IL_cb6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_cb73: Expected O, but got Unknown
		//IL_cbc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_cbc6: Expected O, but got Unknown
		//IL_cc1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_cc24: Expected O, but got Unknown
		//IL_cc71: Unknown result type (might be due to invalid IL or missing references)
		//IL_cc77: Expected O, but got Unknown
		//IL_ccc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_ccca: Expected O, but got Unknown
		//IL_cd17: Unknown result type (might be due to invalid IL or missing references)
		//IL_cd1d: Expected O, but got Unknown
		//IL_cd6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_cd70: Expected O, but got Unknown
		//IL_cdbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_cdc3: Expected O, but got Unknown
		//IL_ce10: Unknown result type (might be due to invalid IL or missing references)
		//IL_ce16: Expected O, but got Unknown
		//IL_ce84: Unknown result type (might be due to invalid IL or missing references)
		//IL_ce8a: Expected O, but got Unknown
		//IL_ceed: Unknown result type (might be due to invalid IL or missing references)
		//IL_cef3: Expected O, but got Unknown
		//IL_cf61: Unknown result type (might be due to invalid IL or missing references)
		//IL_cf67: Expected O, but got Unknown
		//IL_cfca: Unknown result type (might be due to invalid IL or missing references)
		//IL_cfd0: Expected O, but got Unknown
		//IL_d033: Unknown result type (might be due to invalid IL or missing references)
		//IL_d039: Expected O, but got Unknown
		//IL_d09c: Unknown result type (might be due to invalid IL or missing references)
		//IL_d0a2: Expected O, but got Unknown
		//IL_d0ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_d0f5: Expected O, but got Unknown
		//IL_d163: Unknown result type (might be due to invalid IL or missing references)
		//IL_d169: Expected O, but got Unknown
		//IL_d1d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_d1dd: Expected O, but got Unknown
		//IL_d26b: Unknown result type (might be due to invalid IL or missing references)
		//IL_d271: Expected O, but got Unknown
		//IL_d2df: Unknown result type (might be due to invalid IL or missing references)
		//IL_d2e5: Expected O, but got Unknown
		//IL_d353: Unknown result type (might be due to invalid IL or missing references)
		//IL_d359: Expected O, but got Unknown
		//IL_d3c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_d3c9: Expected O, but got Unknown
		//IL_d437: Unknown result type (might be due to invalid IL or missing references)
		//IL_d43d: Expected O, but got Unknown
		//IL_d4ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_d4b1: Expected O, but got Unknown
		//IL_d4fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_d504: Expected O, but got Unknown
		//IL_d572: Unknown result type (might be due to invalid IL or missing references)
		//IL_d578: Expected O, but got Unknown
		//IL_d5e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_d5ec: Expected O, but got Unknown
		//IL_d639: Unknown result type (might be due to invalid IL or missing references)
		//IL_d63f: Expected O, but got Unknown
		//IL_d6ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_d6b3: Expected O, but got Unknown
		//IL_d700: Unknown result type (might be due to invalid IL or missing references)
		//IL_d706: Expected O, but got Unknown
		//IL_d774: Unknown result type (might be due to invalid IL or missing references)
		//IL_d77a: Expected O, but got Unknown
		//IL_d7dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_d7e3: Expected O, but got Unknown
		//IL_d851: Unknown result type (might be due to invalid IL or missing references)
		//IL_d857: Expected O, but got Unknown
		//IL_d8a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_d8aa: Expected O, but got Unknown
		//IL_d918: Unknown result type (might be due to invalid IL or missing references)
		//IL_d91e: Expected O, but got Unknown
		//IL_d976: Unknown result type (might be due to invalid IL or missing references)
		//IL_d97c: Expected O, but got Unknown
		//IL_d9df: Unknown result type (might be due to invalid IL or missing references)
		//IL_d9e5: Expected O, but got Unknown
		//IL_da48: Unknown result type (might be due to invalid IL or missing references)
		//IL_da4e: Expected O, but got Unknown
		//IL_dab1: Unknown result type (might be due to invalid IL or missing references)
		//IL_dab7: Expected O, but got Unknown
		//IL_db04: Unknown result type (might be due to invalid IL or missing references)
		//IL_db0a: Expected O, but got Unknown
		//IL_db57: Unknown result type (might be due to invalid IL or missing references)
		//IL_db5d: Expected O, but got Unknown
		//IL_dbaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_dbb0: Expected O, but got Unknown
		//IL_dc13: Unknown result type (might be due to invalid IL or missing references)
		//IL_dc19: Expected O, but got Unknown
		//IL_dc7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_dc82: Expected O, but got Unknown
		//IL_dccf: Unknown result type (might be due to invalid IL or missing references)
		//IL_dcd5: Expected O, but got Unknown
		//IL_dd22: Unknown result type (might be due to invalid IL or missing references)
		//IL_dd28: Expected O, but got Unknown
		//IL_dd8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_dd91: Expected O, but got Unknown
		//IL_ddf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_ddfa: Expected O, but got Unknown
		//IL_de5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_de63: Expected O, but got Unknown
		//IL_ded1: Unknown result type (might be due to invalid IL or missing references)
		//IL_ded7: Expected O, but got Unknown
		//IL_df3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_df40: Expected O, but got Unknown
		//IL_dfa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_dfa9: Expected O, but got Unknown
		//IL_e00c: Unknown result type (might be due to invalid IL or missing references)
		//IL_e012: Expected O, but got Unknown
		//IL_e080: Unknown result type (might be due to invalid IL or missing references)
		//IL_e086: Expected O, but got Unknown
		//IL_e0f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_e0fa: Expected O, but got Unknown
		//IL_e15d: Unknown result type (might be due to invalid IL or missing references)
		//IL_e163: Expected O, but got Unknown
		//IL_e1d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_e1d7: Expected O, but got Unknown
		//IL_e245: Unknown result type (might be due to invalid IL or missing references)
		//IL_e24b: Expected O, but got Unknown
		//IL_e298: Unknown result type (might be due to invalid IL or missing references)
		//IL_e29e: Expected O, but got Unknown
		//IL_e2eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_e2f1: Expected O, but got Unknown
		//IL_e33e: Unknown result type (might be due to invalid IL or missing references)
		//IL_e344: Expected O, but got Unknown
		//IL_e391: Unknown result type (might be due to invalid IL or missing references)
		//IL_e397: Expected O, but got Unknown
		//IL_e3e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_e3ea: Expected O, but got Unknown
		//IL_e437: Unknown result type (might be due to invalid IL or missing references)
		//IL_e43d: Expected O, but got Unknown
		//IL_e48a: Unknown result type (might be due to invalid IL or missing references)
		//IL_e490: Expected O, but got Unknown
		//IL_e4dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_e4e3: Expected O, but got Unknown
		//IL_e530: Unknown result type (might be due to invalid IL or missing references)
		//IL_e536: Expected O, but got Unknown
		//IL_e583: Unknown result type (might be due to invalid IL or missing references)
		//IL_e589: Expected O, but got Unknown
		//IL_e5d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_e5dc: Expected O, but got Unknown
		//IL_e629: Unknown result type (might be due to invalid IL or missing references)
		//IL_e62f: Expected O, but got Unknown
		//IL_e67c: Unknown result type (might be due to invalid IL or missing references)
		//IL_e682: Expected O, but got Unknown
		//IL_e6cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_e6d5: Expected O, but got Unknown
		//IL_e722: Unknown result type (might be due to invalid IL or missing references)
		//IL_e728: Expected O, but got Unknown
		//IL_e780: Unknown result type (might be due to invalid IL or missing references)
		//IL_e786: Expected O, but got Unknown
		//IL_e7d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_e7d9: Expected O, but got Unknown
		//IL_e826: Unknown result type (might be due to invalid IL or missing references)
		//IL_e82c: Expected O, but got Unknown
		//IL_e88f: Unknown result type (might be due to invalid IL or missing references)
		//IL_e895: Expected O, but got Unknown
		//IL_e8f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_e8fe: Expected O, but got Unknown
		//IL_e94b: Unknown result type (might be due to invalid IL or missing references)
		//IL_e951: Expected O, but got Unknown
		//IL_e99e: Unknown result type (might be due to invalid IL or missing references)
		//IL_e9a4: Expected O, but got Unknown
		//IL_ea20: Unknown result type (might be due to invalid IL or missing references)
		//IL_ea26: Expected O, but got Unknown
		//IL_ea73: Unknown result type (might be due to invalid IL or missing references)
		//IL_ea79: Expected O, but got Unknown
		//IL_eac6: Unknown result type (might be due to invalid IL or missing references)
		//IL_eacc: Expected O, but got Unknown
		//IL_eb19: Unknown result type (might be due to invalid IL or missing references)
		//IL_eb1f: Expected O, but got Unknown
		//IL_eb82: Unknown result type (might be due to invalid IL or missing references)
		//IL_eb88: Expected O, but got Unknown
		//IL_ebeb: Unknown result type (might be due to invalid IL or missing references)
		//IL_ebf1: Expected O, but got Unknown
		//IL_ec3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_ec44: Expected O, but got Unknown
		//IL_ec91: Unknown result type (might be due to invalid IL or missing references)
		//IL_ec97: Expected O, but got Unknown
		//IL_ecfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_ed00: Expected O, but got Unknown
		//IL_ed63: Unknown result type (might be due to invalid IL or missing references)
		//IL_ed69: Expected O, but got Unknown
		//IL_edb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_edbc: Expected O, but got Unknown
		//IL_ee09: Unknown result type (might be due to invalid IL or missing references)
		//IL_ee0f: Expected O, but got Unknown
		//IL_ee5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_ee62: Expected O, but got Unknown
		//IL_eeaf: Unknown result type (might be due to invalid IL or missing references)
		//IL_eeb5: Expected O, but got Unknown
		//IL_ef02: Unknown result type (might be due to invalid IL or missing references)
		//IL_ef08: Expected O, but got Unknown
		//IL_ef55: Unknown result type (might be due to invalid IL or missing references)
		//IL_ef5b: Expected O, but got Unknown
		//IL_efa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_efae: Expected O, but got Unknown
		//IL_f006: Unknown result type (might be due to invalid IL or missing references)
		//IL_f00c: Expected O, but got Unknown
		//IL_f059: Unknown result type (might be due to invalid IL or missing references)
		//IL_f05f: Expected O, but got Unknown
		//IL_f0cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_f0d3: Expected O, but got Unknown
		//IL_f141: Unknown result type (might be due to invalid IL or missing references)
		//IL_f147: Expected O, but got Unknown
		//IL_f1b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_f1bb: Expected O, but got Unknown
		//IL_f229: Unknown result type (might be due to invalid IL or missing references)
		//IL_f22f: Expected O, but got Unknown
		//IL_f292: Unknown result type (might be due to invalid IL or missing references)
		//IL_f298: Expected O, but got Unknown
		//IL_f2fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_f301: Expected O, but got Unknown
		//IL_f364: Unknown result type (might be due to invalid IL or missing references)
		//IL_f36a: Expected O, but got Unknown
		//IL_f3cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_f3d3: Expected O, but got Unknown
		//IL_f43d: Unknown result type (might be due to invalid IL or missing references)
		//IL_f443: Expected O, but got Unknown
		//IL_f4ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_f4b3: Expected O, but got Unknown
		//IL_f51d: Unknown result type (might be due to invalid IL or missing references)
		//IL_f523: Expected O, but got Unknown
		//IL_f586: Unknown result type (might be due to invalid IL or missing references)
		//IL_f58c: Expected O, but got Unknown
		//IL_f5e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_f5ea: Expected O, but got Unknown
		//IL_f66d: Unknown result type (might be due to invalid IL or missing references)
		//IL_f673: Expected O, but got Unknown
		//IL_f6e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_f6e7: Expected O, but got Unknown
		//IL_f755: Unknown result type (might be due to invalid IL or missing references)
		//IL_f75b: Expected O, but got Unknown
		//IL_f7c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_f7cf: Expected O, but got Unknown
		//IL_f839: Unknown result type (might be due to invalid IL or missing references)
		//IL_f83f: Expected O, but got Unknown
		//IL_f8a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_f8af: Expected O, but got Unknown
		//IL_f912: Unknown result type (might be due to invalid IL or missing references)
		//IL_f918: Expected O, but got Unknown
		//IL_f965: Unknown result type (might be due to invalid IL or missing references)
		//IL_f96b: Expected O, but got Unknown
		//IL_f9d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_f9db: Expected O, but got Unknown
		//IL_fa45: Unknown result type (might be due to invalid IL or missing references)
		//IL_fa4b: Expected O, but got Unknown
		//IL_fac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_facd: Expected O, but got Unknown
		//IL_fb37: Unknown result type (might be due to invalid IL or missing references)
		//IL_fb3d: Expected O, but got Unknown
		//IL_fba7: Unknown result type (might be due to invalid IL or missing references)
		//IL_fbad: Expected O, but got Unknown
		//IL_fc1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_fc21: Expected O, but got Unknown
		//IL_fc6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_fc74: Expected O, but got Unknown
		//IL_fcd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_fcdd: Expected O, but got Unknown
		//IL_fd3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_fd42: Expected O, but got Unknown
		//IL_fda1: Unknown result type (might be due to invalid IL or missing references)
		//IL_fda7: Expected O, but got Unknown
		//IL_fe0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_fe10: Expected O, but got Unknown
		//IL_fe73: Unknown result type (might be due to invalid IL or missing references)
		//IL_fe79: Expected O, but got Unknown
		//IL_fedc: Unknown result type (might be due to invalid IL or missing references)
		//IL_fee2: Expected O, but got Unknown
		//IL_ff45: Unknown result type (might be due to invalid IL or missing references)
		//IL_ff4b: Expected O, but got Unknown
		//IL_ffc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_ffcd: Expected O, but got Unknown
		//IL_10030: Unknown result type (might be due to invalid IL or missing references)
		//IL_10036: Expected O, but got Unknown
		//IL_10099: Unknown result type (might be due to invalid IL or missing references)
		//IL_1009f: Expected O, but got Unknown
		//IL_10114: Unknown result type (might be due to invalid IL or missing references)
		//IL_1011a: Expected O, but got Unknown
		//IL_10188: Unknown result type (might be due to invalid IL or missing references)
		//IL_1018e: Expected O, but got Unknown
		//IL_101fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_10202: Expected O, but got Unknown
		//IL_10265: Unknown result type (might be due to invalid IL or missing references)
		//IL_1026b: Expected O, but got Unknown
		//IL_102ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_102d4: Expected O, but got Unknown
		//IL_10370: Unknown result type (might be due to invalid IL or missing references)
		//IL_10376: Expected O, but got Unknown
		//IL_103e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_103e6: Expected O, but got Unknown
		//IL_10449: Unknown result type (might be due to invalid IL or missing references)
		//IL_1044f: Expected O, but got Unknown
		//IL_104b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_104b8: Expected O, but got Unknown
		//IL_10534: Unknown result type (might be due to invalid IL or missing references)
		//IL_1053a: Expected O, but got Unknown
		//IL_1059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_105a3: Expected O, but got Unknown
		//IL_10606: Unknown result type (might be due to invalid IL or missing references)
		//IL_1060c: Expected O, but got Unknown
		//IL_1066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_10675: Expected O, but got Unknown
		//IL_106d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_106de: Expected O, but got Unknown
		//IL_1072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_10731: Expected O, but got Unknown
		//IL_107bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_107c1: Expected O, but got Unknown
		//IL_1084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_10851: Expected O, but got Unknown
		//IL_108b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_108ba: Expected O, but got Unknown
		//IL_1092b: Unknown result type (might be due to invalid IL or missing references)
		//IL_10931: Expected O, but got Unknown
		//IL_1099b: Unknown result type (might be due to invalid IL or missing references)
		//IL_109a1: Expected O, but got Unknown
		//IL_10a04: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a0a: Expected O, but got Unknown
		//IL_10a74: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a7a: Expected O, but got Unknown
		//IL_10add: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ae3: Expected O, but got Unknown
		//IL_10b46: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b4c: Expected O, but got Unknown
		//IL_10bc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10bc7: Expected O, but got Unknown
		//IL_10c43: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c49: Expected O, but got Unknown
		//IL_10cc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ccb: Expected O, but got Unknown
		//IL_10d2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d34: Expected O, but got Unknown
		//IL_10d97: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d9d: Expected O, but got Unknown
		//IL_10e00: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e06: Expected O, but got Unknown
		//IL_10e69: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e6f: Expected O, but got Unknown
		//IL_10ed2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ed8: Expected O, but got Unknown
		//IL_10f3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f41: Expected O, but got Unknown
		//IL_10fa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10faa: Expected O, but got Unknown
		//IL_1100d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11013: Expected O, but got Unknown
		//IL_1106b: Unknown result type (might be due to invalid IL or missing references)
		//IL_11071: Expected O, but got Unknown
		//IL_110c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_110cf: Expected O, but got Unknown
		//IL_11140: Unknown result type (might be due to invalid IL or missing references)
		//IL_11146: Expected O, but got Unknown
		//IL_111c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_111cf: Expected O, but got Unknown
		//IL_11232: Unknown result type (might be due to invalid IL or missing references)
		//IL_11238: Expected O, but got Unknown
		//IL_1129b: Unknown result type (might be due to invalid IL or missing references)
		//IL_112a1: Expected O, but got Unknown
		//IL_11304: Unknown result type (might be due to invalid IL or missing references)
		//IL_1130a: Expected O, but got Unknown
		//IL_1136d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11373: Expected O, but got Unknown
		//IL_113d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_113dc: Expected O, but got Unknown
		//IL_11478: Unknown result type (might be due to invalid IL or missing references)
		//IL_1147e: Expected O, but got Unknown
		//IL_114fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_11500: Expected O, but got Unknown
		//IL_1157c: Unknown result type (might be due to invalid IL or missing references)
		//IL_11582: Expected O, but got Unknown
		//IL_115e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_115eb: Expected O, but got Unknown
		//IL_1164e: Unknown result type (might be due to invalid IL or missing references)
		//IL_11654: Expected O, but got Unknown
		//IL_116b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_116bd: Expected O, but got Unknown
		//IL_11720: Unknown result type (might be due to invalid IL or missing references)
		//IL_11726: Expected O, but got Unknown
		//IL_11789: Unknown result type (might be due to invalid IL or missing references)
		//IL_1178f: Expected O, but got Unknown
		//IL_117f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_117f8: Expected O, but got Unknown
		//IL_1185b: Unknown result type (might be due to invalid IL or missing references)
		//IL_11861: Expected O, but got Unknown
		//IL_118c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_118ca: Expected O, but got Unknown
		//IL_1192d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11933: Expected O, but got Unknown
		//IL_11996: Unknown result type (might be due to invalid IL or missing references)
		//IL_1199c: Expected O, but got Unknown
		//IL_119ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a05: Expected O, but got Unknown
		//IL_11a6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a75: Expected O, but got Unknown
		//IL_11ad8: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ade: Expected O, but got Unknown
		//IL_11b41: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b47: Expected O, but got Unknown
		//IL_11baa: Unknown result type (might be due to invalid IL or missing references)
		//IL_11bb0: Expected O, but got Unknown
		//IL_11c13: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c19: Expected O, but got Unknown
		//IL_11c7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c82: Expected O, but got Unknown
		//IL_11ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ceb: Expected O, but got Unknown
		//IL_11d4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d54: Expected O, but got Unknown
		//IL_11db7: Unknown result type (might be due to invalid IL or missing references)
		//IL_11dbd: Expected O, but got Unknown
		//IL_11e27: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e2d: Expected O, but got Unknown
		//IL_11e97: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e9d: Expected O, but got Unknown
		//IL_11f00: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f06: Expected O, but got Unknown
		//IL_11f69: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f6f: Expected O, but got Unknown
		//IL_12000: Unknown result type (might be due to invalid IL or missing references)
		//IL_12006: Expected O, but got Unknown
		//IL_12069: Unknown result type (might be due to invalid IL or missing references)
		//IL_1206f: Expected O, but got Unknown
		//IL_120d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_120d8: Expected O, but got Unknown
		//IL_1213b: Unknown result type (might be due to invalid IL or missing references)
		//IL_12141: Expected O, but got Unknown
		//IL_1218e: Unknown result type (might be due to invalid IL or missing references)
		//IL_12194: Expected O, but got Unknown
		//IL_121fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_12204: Expected O, but got Unknown
		//IL_12267: Unknown result type (might be due to invalid IL or missing references)
		//IL_1226d: Expected O, but got Unknown
		//IL_122d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_122dd: Expected O, but got Unknown
		//IL_1232a: Unknown result type (might be due to invalid IL or missing references)
		//IL_12330: Expected O, but got Unknown
		//IL_12393: Unknown result type (might be due to invalid IL or missing references)
		//IL_12399: Expected O, but got Unknown
		//IL_12435: Unknown result type (might be due to invalid IL or missing references)
		//IL_1243b: Expected O, but got Unknown
		//IL_124b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_124bd: Expected O, but got Unknown
		//IL_12520: Unknown result type (might be due to invalid IL or missing references)
		//IL_12526: Expected O, but got Unknown
		//IL_125a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_125af: Expected O, but got Unknown
		//IL_12612: Unknown result type (might be due to invalid IL or missing references)
		//IL_12618: Expected O, but got Unknown
		//IL_12670: Unknown result type (might be due to invalid IL or missing references)
		//IL_12676: Expected O, but got Unknown
		//IL_126e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_126e6: Expected O, but got Unknown
		//IL_12749: Unknown result type (might be due to invalid IL or missing references)
		//IL_1274f: Expected O, but got Unknown
		//IL_127b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_127b8: Expected O, but got Unknown
		//IL_12805: Unknown result type (might be due to invalid IL or missing references)
		//IL_1280b: Expected O, but got Unknown
		//IL_12863: Unknown result type (might be due to invalid IL or missing references)
		//IL_12869: Expected O, but got Unknown
		//IL_128c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_128c7: Expected O, but got Unknown
		//IL_12914: Unknown result type (might be due to invalid IL or missing references)
		//IL_1291a: Expected O, but got Unknown
		//IL_1298f: Unknown result type (might be due to invalid IL or missing references)
		//IL_12995: Expected O, but got Unknown
		//IL_129ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_129f3: Expected O, but got Unknown
		//IL_12a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a46: Expected O, but got Unknown
		//IL_12aa9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12aaf: Expected O, but got Unknown
		//IL_12b12: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b18: Expected O, but got Unknown
		//IL_12b7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b81: Expected O, but got Unknown
		//IL_12beb: Unknown result type (might be due to invalid IL or missing references)
		//IL_12bf1: Expected O, but got Unknown
		//IL_12c3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c44: Expected O, but got Unknown
		//IL_12cb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cbf: Expected O, but got Unknown
		//IL_12d34: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d3a: Expected O, but got Unknown
		//IL_12d92: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d98: Expected O, but got Unknown
		//IL_12e06: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e0c: Expected O, but got Unknown
		//IL_12e6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e75: Expected O, but got Unknown
		//IL_12ee3: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ee9: Expected O, but got Unknown
		//IL_12f53: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f59: Expected O, but got Unknown
		//IL_12fa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_12fac: Expected O, but got Unknown
		//IL_1300f: Unknown result type (might be due to invalid IL or missing references)
		//IL_13015: Expected O, but got Unknown
		//IL_13078: Unknown result type (might be due to invalid IL or missing references)
		//IL_1307e: Expected O, but got Unknown
		//IL_130d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_130dc: Expected O, but got Unknown
		//IL_1314d: Unknown result type (might be due to invalid IL or missing references)
		//IL_13153: Expected O, but got Unknown
		//IL_131b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_131bc: Expected O, but got Unknown
		//IL_1321f: Unknown result type (might be due to invalid IL or missing references)
		//IL_13225: Expected O, but got Unknown
		//IL_13288: Unknown result type (might be due to invalid IL or missing references)
		//IL_1328e: Expected O, but got Unknown
		//IL_132f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_132f7: Expected O, but got Unknown
		//IL_1335a: Unknown result type (might be due to invalid IL or missing references)
		//IL_13360: Expected O, but got Unknown
		//IL_133b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_133be: Expected O, but got Unknown
		//IL_13416: Unknown result type (might be due to invalid IL or missing references)
		//IL_1341c: Expected O, but got Unknown
		//IL_13486: Unknown result type (might be due to invalid IL or missing references)
		//IL_1348c: Expected O, but got Unknown
		//IL_134e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_134ea: Expected O, but got Unknown
		//IL_13554: Unknown result type (might be due to invalid IL or missing references)
		//IL_1355a: Expected O, but got Unknown
		//IL_135c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_135ca: Expected O, but got Unknown
		//IL_1362d: Unknown result type (might be due to invalid IL or missing references)
		//IL_13633: Expected O, but got Unknown
		//IL_1368b: Unknown result type (might be due to invalid IL or missing references)
		//IL_13691: Expected O, but got Unknown
		//IL_136f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_136fa: Expected O, but got Unknown
		//IL_1375d: Unknown result type (might be due to invalid IL or missing references)
		//IL_13763: Expected O, but got Unknown
		//IL_137bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_137c1: Expected O, but got Unknown
		//IL_1383d: Unknown result type (might be due to invalid IL or missing references)
		//IL_13843: Expected O, but got Unknown
		//IL_138a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_138ac: Expected O, but got Unknown
		//IL_1390f: Unknown result type (might be due to invalid IL or missing references)
		//IL_13915: Expected O, but got Unknown
		//IL_13978: Unknown result type (might be due to invalid IL or missing references)
		//IL_1397e: Expected O, but got Unknown
		//IL_139e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_139ee: Expected O, but got Unknown
		//IL_13a51: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a57: Expected O, but got Unknown
		//IL_13acc: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ad2: Expected O, but got Unknown
		//IL_13b35: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b3b: Expected O, but got Unknown
		//IL_13bb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13bb6: Expected O, but got Unknown
		//IL_13c2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c31: Expected O, but got Unknown
		//IL_13ca6: Unknown result type (might be due to invalid IL or missing references)
		//IL_13cac: Expected O, but got Unknown
		//IL_13d04: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d0a: Expected O, but got Unknown
		//IL_13d57: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d5d: Expected O, but got Unknown
		//IL_13daa: Unknown result type (might be due to invalid IL or missing references)
		//IL_13db0: Expected O, but got Unknown
		//IL_13dfd: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e03: Expected O, but got Unknown
		//IL_13e50: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e56: Expected O, but got Unknown
		//IL_13eb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ebf: Expected O, but got Unknown
		//IL_13f22: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f28: Expected O, but got Unknown
		//IL_13f8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f91: Expected O, but got Unknown
		//IL_13ff4: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ffa: Expected O, but got Unknown
		//IL_1405d: Unknown result type (might be due to invalid IL or missing references)
		//IL_14063: Expected O, but got Unknown
		//IL_140c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_140cc: Expected O, but got Unknown
		//IL_14119: Unknown result type (might be due to invalid IL or missing references)
		//IL_1411f: Expected O, but got Unknown
		//IL_14182: Unknown result type (might be due to invalid IL or missing references)
		//IL_14188: Expected O, but got Unknown
		//IL_141eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_141f1: Expected O, but got Unknown
		//IL_14254: Unknown result type (might be due to invalid IL or missing references)
		//IL_1425a: Expected O, but got Unknown
		//IL_142a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_142ad: Expected O, but got Unknown
		//IL_14310: Unknown result type (might be due to invalid IL or missing references)
		//IL_14316: Expected O, but got Unknown
		//IL_14379: Unknown result type (might be due to invalid IL or missing references)
		//IL_1437f: Expected O, but got Unknown
		//IL_143e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_143e8: Expected O, but got Unknown
		//IL_1444b: Unknown result type (might be due to invalid IL or missing references)
		//IL_14451: Expected O, but got Unknown
		//IL_1449e: Unknown result type (might be due to invalid IL or missing references)
		//IL_144a4: Expected O, but got Unknown
		//IL_14507: Unknown result type (might be due to invalid IL or missing references)
		//IL_1450d: Expected O, but got Unknown
		//IL_14570: Unknown result type (might be due to invalid IL or missing references)
		//IL_14576: Expected O, but got Unknown
		//IL_145d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_145df: Expected O, but got Unknown
		//IL_14642: Unknown result type (might be due to invalid IL or missing references)
		//IL_14648: Expected O, but got Unknown
		//IL_146d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_146df: Expected O, but got Unknown
		//IL_1472c: Unknown result type (might be due to invalid IL or missing references)
		//IL_14732: Expected O, but got Unknown
		//IL_1477f: Unknown result type (might be due to invalid IL or missing references)
		//IL_14785: Expected O, but got Unknown
		//IL_147d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_147d8: Expected O, but got Unknown
		//IL_14825: Unknown result type (might be due to invalid IL or missing references)
		//IL_1482b: Expected O, but got Unknown
		//IL_1488e: Unknown result type (might be due to invalid IL or missing references)
		//IL_14894: Expected O, but got Unknown
		//IL_148f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_148fd: Expected O, but got Unknown
		//IL_14960: Unknown result type (might be due to invalid IL or missing references)
		//IL_14966: Expected O, but got Unknown
		//IL_149c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_149cf: Expected O, but got Unknown
		//IL_14a32: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a38: Expected O, but got Unknown
		//IL_14a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a8b: Expected O, but got Unknown
		//IL_14aee: Unknown result type (might be due to invalid IL or missing references)
		//IL_14af4: Expected O, but got Unknown
		//IL_14b4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b52: Expected O, but got Unknown
		//IL_14bc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14bc6: Expected O, but got Unknown
		//IL_14c34: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c3a: Expected O, but got Unknown
		//IL_14c87: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c8d: Expected O, but got Unknown
		//IL_14cda: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ce0: Expected O, but got Unknown
		//IL_14d2d: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d33: Expected O, but got Unknown
		//IL_14d80: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d86: Expected O, but got Unknown
		//IL_14dd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_14dd9: Expected O, but got Unknown
		//IL_14e26: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e2c: Expected O, but got Unknown
		//IL_14e79: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e7f: Expected O, but got Unknown
		//IL_14ecc: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ed2: Expected O, but got Unknown
		//IL_14f1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f25: Expected O, but got Unknown
		//IL_14f93: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f99: Expected O, but got Unknown
		//IL_15007: Unknown result type (might be due to invalid IL or missing references)
		//IL_1500d: Expected O, but got Unknown
		//IL_15070: Unknown result type (might be due to invalid IL or missing references)
		//IL_15076: Expected O, but got Unknown
		//IL_150d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_150df: Expected O, but got Unknown
		//IL_15142: Unknown result type (might be due to invalid IL or missing references)
		//IL_15148: Expected O, but got Unknown
		//IL_151ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_151b1: Expected O, but got Unknown
		//IL_15214: Unknown result type (might be due to invalid IL or missing references)
		//IL_1521a: Expected O, but got Unknown
		//IL_1527d: Unknown result type (might be due to invalid IL or missing references)
		//IL_15283: Expected O, but got Unknown
		//IL_152e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_152ec: Expected O, but got Unknown
		//IL_1534f: Unknown result type (might be due to invalid IL or missing references)
		//IL_15355: Expected O, but got Unknown
		//IL_153ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_153b3: Expected O, but got Unknown
		//IL_15436: Unknown result type (might be due to invalid IL or missing references)
		//IL_1543c: Expected O, but got Unknown
		//IL_154bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_154c5: Expected O, but got Unknown
		//IL_15548: Unknown result type (might be due to invalid IL or missing references)
		//IL_1554e: Expected O, but got Unknown
		//IL_155d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_155d7: Expected O, but got Unknown
		//IL_1565a: Unknown result type (might be due to invalid IL or missing references)
		//IL_15660: Expected O, but got Unknown
		//IL_156e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_156e9: Expected O, but got Unknown
		//IL_1576c: Unknown result type (might be due to invalid IL or missing references)
		//IL_15772: Expected O, but got Unknown
		//IL_157f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_157fb: Expected O, but got Unknown
		//IL_1587e: Unknown result type (might be due to invalid IL or missing references)
		//IL_15884: Expected O, but got Unknown
		//IL_15907: Unknown result type (might be due to invalid IL or missing references)
		//IL_1590d: Expected O, but got Unknown
		//IL_15990: Unknown result type (might be due to invalid IL or missing references)
		//IL_15996: Expected O, but got Unknown
		//IL_15a19: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a1f: Expected O, but got Unknown
		//IL_15aa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_15aa8: Expected O, but got Unknown
		//IL_15b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b31: Expected O, but got Unknown
		//IL_15bb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_15bba: Expected O, but got Unknown
		//IL_15c3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c43: Expected O, but got Unknown
		//IL_15cc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ccc: Expected O, but got Unknown
		//IL_15d4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d55: Expected O, but got Unknown
		//IL_15dd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_15dde: Expected O, but got Unknown
		//IL_15e2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e31: Expected O, but got Unknown
		//IL_15eb4: Unknown result type (might be due to invalid IL or missing references)
		//IL_15eba: Expected O, but got Unknown
		//IL_15f3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f43: Expected O, but got Unknown
		//IL_15fc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fcc: Expected O, but got Unknown
		//IL_1604f: Unknown result type (might be due to invalid IL or missing references)
		//IL_16055: Expected O, but got Unknown
		//IL_160d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_160de: Expected O, but got Unknown
		//IL_16161: Unknown result type (might be due to invalid IL or missing references)
		//IL_16167: Expected O, but got Unknown
		//IL_161b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_161ba: Expected O, but got Unknown
		//IL_16207: Unknown result type (might be due to invalid IL or missing references)
		//IL_1620d: Expected O, but got Unknown
		//IL_16290: Unknown result type (might be due to invalid IL or missing references)
		//IL_16296: Expected O, but got Unknown
		//IL_16319: Unknown result type (might be due to invalid IL or missing references)
		//IL_1631f: Expected O, but got Unknown
		//IL_16382: Unknown result type (might be due to invalid IL or missing references)
		//IL_16388: Expected O, but got Unknown
		//IL_163eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_163f1: Expected O, but got Unknown
		//IL_16474: Unknown result type (might be due to invalid IL or missing references)
		//IL_1647a: Expected O, but got Unknown
		//IL_164c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_164cd: Expected O, but got Unknown
		//IL_16530: Unknown result type (might be due to invalid IL or missing references)
		//IL_16536: Expected O, but got Unknown
		//IL_16599: Unknown result type (might be due to invalid IL or missing references)
		//IL_1659f: Expected O, but got Unknown
		//IL_16602: Unknown result type (might be due to invalid IL or missing references)
		//IL_16608: Expected O, but got Unknown
		//IL_16655: Unknown result type (might be due to invalid IL or missing references)
		//IL_1665b: Expected O, but got Unknown
		//IL_166ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_166c0: Expected O, but got Unknown
		//IL_1671f: Unknown result type (might be due to invalid IL or missing references)
		//IL_16725: Expected O, but got Unknown
		//IL_16784: Unknown result type (might be due to invalid IL or missing references)
		//IL_1678a: Expected O, but got Unknown
		//IL_167ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_167f3: Expected O, but got Unknown
		//IL_16856: Unknown result type (might be due to invalid IL or missing references)
		//IL_1685c: Expected O, but got Unknown
		//IL_168a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_168af: Expected O, but got Unknown
		//IL_16912: Unknown result type (might be due to invalid IL or missing references)
		//IL_16918: Expected O, but got Unknown
		//IL_1697b: Unknown result type (might be due to invalid IL or missing references)
		//IL_16981: Expected O, but got Unknown
		//IL_169ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_169d4: Expected O, but got Unknown
		//IL_16a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a27: Expected O, but got Unknown
		//IL_16a74: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a7a: Expected O, but got Unknown
		//IL_16ac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_16acd: Expected O, but got Unknown
		//IL_16b3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b41: Expected O, but got Unknown
		//IL_16bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_16bdc: Expected O, but got Unknown
		//IL_16c4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c50: Expected O, but got Unknown
		//IL_16cb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_16cb9: Expected O, but got Unknown
		//IL_16d1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d22: Expected O, but got Unknown
		//IL_16d85: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d8b: Expected O, but got Unknown
		//IL_16dee: Unknown result type (might be due to invalid IL or missing references)
		//IL_16df4: Expected O, but got Unknown
		//IL_16e57: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e5d: Expected O, but got Unknown
		//IL_16ec0: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ec6: Expected O, but got Unknown
		//IL_16f34: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f3a: Expected O, but got Unknown
		//IL_16fa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_16fae: Expected O, but got Unknown
		//IL_17011: Unknown result type (might be due to invalid IL or missing references)
		//IL_17017: Expected O, but got Unknown
		//IL_1707a: Unknown result type (might be due to invalid IL or missing references)
		//IL_17080: Expected O, but got Unknown
		//IL_170e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_170e9: Expected O, but got Unknown
		//IL_1714c: Unknown result type (might be due to invalid IL or missing references)
		//IL_17152: Expected O, but got Unknown
		//IL_171b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_171bb: Expected O, but got Unknown
		//IL_1721e: Unknown result type (might be due to invalid IL or missing references)
		//IL_17224: Expected O, but got Unknown
		//IL_17271: Unknown result type (might be due to invalid IL or missing references)
		//IL_17277: Expected O, but got Unknown
		//IL_172e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_172eb: Expected O, but got Unknown
		//IL_17359: Unknown result type (might be due to invalid IL or missing references)
		//IL_1735f: Expected O, but got Unknown
		//IL_173cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_173d3: Expected O, but got Unknown
		//IL_17420: Unknown result type (might be due to invalid IL or missing references)
		//IL_17426: Expected O, but got Unknown
		//IL_17489: Unknown result type (might be due to invalid IL or missing references)
		//IL_1748f: Expected O, but got Unknown
		//IL_174f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_174f8: Expected O, but got Unknown
		//IL_17566: Unknown result type (might be due to invalid IL or missing references)
		//IL_1756c: Expected O, but got Unknown
		//IL_175da: Unknown result type (might be due to invalid IL or missing references)
		//IL_175e0: Expected O, but got Unknown
		//IL_1764a: Unknown result type (might be due to invalid IL or missing references)
		//IL_17650: Expected O, but got Unknown
		//IL_176ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_176c0: Expected O, but got Unknown
		//IL_1770d: Unknown result type (might be due to invalid IL or missing references)
		//IL_17713: Expected O, but got Unknown
		//IL_1777d: Unknown result type (might be due to invalid IL or missing references)
		//IL_17783: Expected O, but got Unknown
		//IL_177e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_177ec: Expected O, but got Unknown
		//IL_17856: Unknown result type (might be due to invalid IL or missing references)
		//IL_1785c: Expected O, but got Unknown
		//IL_178bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_178c5: Expected O, but got Unknown
		//IL_17928: Unknown result type (might be due to invalid IL or missing references)
		//IL_1792e: Expected O, but got Unknown
		//IL_1799c: Unknown result type (might be due to invalid IL or missing references)
		//IL_179a2: Expected O, but got Unknown
		//IL_17a05: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a0b: Expected O, but got Unknown
		//IL_17a6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a74: Expected O, but got Unknown
		//IL_17ad7: Unknown result type (might be due to invalid IL or missing references)
		//IL_17add: Expected O, but got Unknown
		//IL_17b40: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b46: Expected O, but got Unknown
		//IL_17b93: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b99: Expected O, but got Unknown
		//IL_17c07: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c0d: Expected O, but got Unknown
		//IL_17c7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c81: Expected O, but got Unknown
		//IL_17cef: Unknown result type (might be due to invalid IL or missing references)
		//IL_17cf5: Expected O, but got Unknown
		//IL_17d6a: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d70: Expected O, but got Unknown
		//IL_17de5: Unknown result type (might be due to invalid IL or missing references)
		//IL_17deb: Expected O, but got Unknown
		//IL_17e60: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e66: Expected O, but got Unknown
		//IL_17ed4: Unknown result type (might be due to invalid IL or missing references)
		//IL_17eda: Expected O, but got Unknown
		//IL_17f4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f55: Expected O, but got Unknown
		//IL_17fb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17fbe: Expected O, but got Unknown
		//IL_18021: Unknown result type (might be due to invalid IL or missing references)
		//IL_18027: Expected O, but got Unknown
		//IL_18095: Unknown result type (might be due to invalid IL or missing references)
		//IL_1809b: Expected O, but got Unknown
		//IL_18109: Unknown result type (might be due to invalid IL or missing references)
		//IL_1810f: Expected O, but got Unknown
		//IL_18172: Unknown result type (might be due to invalid IL or missing references)
		//IL_18178: Expected O, but got Unknown
		//IL_181db: Unknown result type (might be due to invalid IL or missing references)
		//IL_181e1: Expected O, but got Unknown
		//IL_18244: Unknown result type (might be due to invalid IL or missing references)
		//IL_1824a: Expected O, but got Unknown
		//IL_182cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_182d3: Expected O, but got Unknown
		//IL_18332: Unknown result type (might be due to invalid IL or missing references)
		//IL_18338: Expected O, but got Unknown
		//IL_183ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_183b3: Expected O, but got Unknown
		//IL_18421: Unknown result type (might be due to invalid IL or missing references)
		//IL_18427: Expected O, but got Unknown
		//IL_18495: Unknown result type (might be due to invalid IL or missing references)
		//IL_1849b: Expected O, but got Unknown
		//IL_18510: Unknown result type (might be due to invalid IL or missing references)
		//IL_18516: Expected O, but got Unknown
		//IL_18579: Unknown result type (might be due to invalid IL or missing references)
		//IL_1857f: Expected O, but got Unknown
		//IL_185ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_185f3: Expected O, but got Unknown
		//IL_18661: Unknown result type (might be due to invalid IL or missing references)
		//IL_18667: Expected O, but got Unknown
		//IL_186d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_186db: Expected O, but got Unknown
		//IL_18750: Unknown result type (might be due to invalid IL or missing references)
		//IL_18756: Expected O, but got Unknown
		//IL_187a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_187a9: Expected O, but got Unknown
		//IL_18817: Unknown result type (might be due to invalid IL or missing references)
		//IL_1881d: Expected O, but got Unknown
		//IL_18887: Unknown result type (might be due to invalid IL or missing references)
		//IL_1888d: Expected O, but got Unknown
		//IL_188f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_188fd: Expected O, but got Unknown
		//IL_18980: Unknown result type (might be due to invalid IL or missing references)
		//IL_18986: Expected O, but got Unknown
		//IL_189e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_189ef: Expected O, but got Unknown
		//IL_18a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a58: Expected O, but got Unknown
		//IL_18abb: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ac1: Expected O, but got Unknown
		//IL_18b24: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b2a: Expected O, but got Unknown
		//IL_18b98: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b9e: Expected O, but got Unknown
		//IL_18c0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c12: Expected O, but got Unknown
		//IL_18c75: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c7b: Expected O, but got Unknown
		//IL_18cf0: Unknown result type (might be due to invalid IL or missing references)
		//IL_18cf6: Expected O, but got Unknown
		//IL_18d59: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d5f: Expected O, but got Unknown
		//IL_18dac: Unknown result type (might be due to invalid IL or missing references)
		//IL_18db2: Expected O, but got Unknown
		//IL_18dff: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e05: Expected O, but got Unknown
		//IL_18e88: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e8e: Expected O, but got Unknown
		//IL_18edb: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ee1: Expected O, but got Unknown
		//IL_18f44: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f4a: Expected O, but got Unknown
		//IL_18f97: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f9d: Expected O, but got Unknown
		//IL_18fea: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ff0: Expected O, but got Unknown
		//IL_19053: Unknown result type (might be due to invalid IL or missing references)
		//IL_19059: Expected O, but got Unknown
		//IL_190bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_190c2: Expected O, but got Unknown
		//IL_19125: Unknown result type (might be due to invalid IL or missing references)
		//IL_1912b: Expected O, but got Unknown
		//IL_19178: Unknown result type (might be due to invalid IL or missing references)
		//IL_1917e: Expected O, but got Unknown
		//IL_191cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_191d1: Expected O, but got Unknown
		//IL_19234: Unknown result type (might be due to invalid IL or missing references)
		//IL_1923a: Expected O, but got Unknown
		//IL_19287: Unknown result type (might be due to invalid IL or missing references)
		//IL_1928d: Expected O, but got Unknown
		//IL_192da: Unknown result type (might be due to invalid IL or missing references)
		//IL_192e0: Expected O, but got Unknown
		//IL_1932d: Unknown result type (might be due to invalid IL or missing references)
		//IL_19333: Expected O, but got Unknown
		//IL_19380: Unknown result type (might be due to invalid IL or missing references)
		//IL_19386: Expected O, but got Unknown
		//IL_193d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_193d9: Expected O, but got Unknown
		//IL_1944e: Unknown result type (might be due to invalid IL or missing references)
		//IL_19454: Expected O, but got Unknown
		//IL_194c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_194cf: Expected O, but got Unknown
		//IL_1951c: Unknown result type (might be due to invalid IL or missing references)
		//IL_19522: Expected O, but got Unknown
		//IL_19590: Unknown result type (might be due to invalid IL or missing references)
		//IL_19596: Expected O, but got Unknown
		//IL_19604: Unknown result type (might be due to invalid IL or missing references)
		//IL_1960a: Expected O, but got Unknown
		//IL_19678: Unknown result type (might be due to invalid IL or missing references)
		//IL_1967e: Expected O, but got Unknown
		//IL_196ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_196f2: Expected O, but got Unknown
		//IL_19760: Unknown result type (might be due to invalid IL or missing references)
		//IL_19766: Expected O, but got Unknown
		//IL_197d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_197da: Expected O, but got Unknown
		//IL_19848: Unknown result type (might be due to invalid IL or missing references)
		//IL_1984e: Expected O, but got Unknown
		//IL_198bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_198c2: Expected O, but got Unknown
		//IL_19930: Unknown result type (might be due to invalid IL or missing references)
		//IL_19936: Expected O, but got Unknown
		//IL_199a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_199aa: Expected O, but got Unknown
		//IL_19a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a1e: Expected O, but got Unknown
		//IL_19a8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a92: Expected O, but got Unknown
		//IL_19b00: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b06: Expected O, but got Unknown
		//IL_19b74: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b7a: Expected O, but got Unknown
		//IL_19be8: Unknown result type (might be due to invalid IL or missing references)
		//IL_19bee: Expected O, but got Unknown
		//IL_19c51: Unknown result type (might be due to invalid IL or missing references)
		//IL_19c57: Expected O, but got Unknown
		//IL_19cba: Unknown result type (might be due to invalid IL or missing references)
		//IL_19cc0: Expected O, but got Unknown
		//IL_19d0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d13: Expected O, but got Unknown
		//IL_19d88: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d8e: Expected O, but got Unknown
		//IL_19df1: Unknown result type (might be due to invalid IL or missing references)
		//IL_19df7: Expected O, but got Unknown
		//IL_19e5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e60: Expected O, but got Unknown
		//IL_19ec3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ec9: Expected O, but got Unknown
		//IL_19f37: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f3d: Expected O, but got Unknown
		//IL_19fab: Unknown result type (might be due to invalid IL or missing references)
		//IL_19fb1: Expected O, but got Unknown
		//IL_1a01f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a025: Expected O, but got Unknown
		//IL_1a09a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a0a0: Expected O, but got Unknown
		//IL_1a103: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a109: Expected O, but got Unknown
		//IL_1a16c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a172: Expected O, but got Unknown
		//IL_1a1bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a1c5: Expected O, but got Unknown
		//IL_1a233: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a239: Expected O, but got Unknown
		//IL_1a2ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2b4: Expected O, but got Unknown
		//IL_1a322: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a328: Expected O, but got Unknown
		//IL_1a396: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a39c: Expected O, but got Unknown
		//IL_1a40a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a410: Expected O, but got Unknown
		//IL_1a473: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a479: Expected O, but got Unknown
		//IL_1a4e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a4ed: Expected O, but got Unknown
		//IL_1a55b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a561: Expected O, but got Unknown
		//IL_1a5c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5ca: Expected O, but got Unknown
		//IL_1a62d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a633: Expected O, but got Unknown
		//IL_1a680: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a686: Expected O, but got Unknown
		//IL_1a6d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6d9: Expected O, but got Unknown
		//IL_1a747: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a74d: Expected O, but got Unknown
		//IL_1a7bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a7c1: Expected O, but got Unknown
		//IL_1a82f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a835: Expected O, but got Unknown
		//IL_1a8a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8a9: Expected O, but got Unknown
		//IL_1a917: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a91d: Expected O, but got Unknown
		//IL_1a992: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a998: Expected O, but got Unknown
		//IL_1aa0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa13: Expected O, but got Unknown
		//IL_1aa88: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa8e: Expected O, but got Unknown
		//IL_1ab03: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab09: Expected O, but got Unknown
		//IL_1ab56: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab5c: Expected O, but got Unknown
		Command[] array = new Command[1062];
		Command val = new Command();
		val.Name = "humanknownplayerslosupdateinterval";
		val.Parent = "aibrainsenses";
		val.FullName = "aibrainsenses.humanknownplayerslosupdateinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIBrainSenses.HumanKnownPlayersLOSUpdateInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			AIBrainSenses.HumanKnownPlayersLOSUpdateInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[0] = val;
		val = new Command();
		val.Name = "knownplayerslosupdateinterval";
		val.Parent = "aibrainsenses";
		val.FullName = "aibrainsenses.knownplayerslosupdateinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIBrainSenses.KnownPlayersLOSUpdateInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			AIBrainSenses.KnownPlayersLOSUpdateInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[1] = val;
		val = new Command();
		val.Name = "updateinterval";
		val.Parent = "aibrainsenses";
		val.FullName = "aibrainsenses.updateinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIBrainSenses.UpdateInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			AIBrainSenses.UpdateInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[2] = val;
		val = new Command();
		val.Name = "animalframebudgetms";
		val.Parent = "aithinkmanager";
		val.FullName = "aithinkmanager.animalframebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIThinkManager.animalframebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			AIThinkManager.animalframebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[3] = val;
		val = new Command();
		val.Name = "framebudgetms";
		val.Parent = "aithinkmanager";
		val.FullName = "aithinkmanager.framebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIThinkManager.framebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			AIThinkManager.framebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[4] = val;
		val = new Command();
		val.Name = "petframebudgetms";
		val.Parent = "aithinkmanager";
		val.FullName = "aithinkmanager.petframebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIThinkManager.petframebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			AIThinkManager.petframebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[5] = val;
		val = new Command();
		val.Name = "auto_turret_budget_ms";
		val.Parent = "autoturret";
		val.FullName = "autoturret.auto_turret_budget_ms";
		val.ServerAdmin = true;
		val.Description = "How many milliseconds to spend on target scanning per frame";
		val.Variable = true;
		val.GetOveride = () => AutoTurret.auto_turret_budget_ms.ToString();
		val.SetOveride = delegate(string str)
		{
			AutoTurret.auto_turret_budget_ms = StringExtensions.ToFloat(str, 0f);
		};
		array[6] = val;
		val = new Command();
		val.Name = "do_shore_drift";
		val.Parent = "baseboat";
		val.FullName = "baseboat.do_shore_drift";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseBoat.do_shore_drift.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseBoat.do_shore_drift = StringExtensions.ToBool(str);
		};
		array[7] = val;
		val = new Command();
		val.Name = "drift_speed";
		val.Parent = "baseboat";
		val.FullName = "baseboat.drift_speed";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseBoat.drift_speed.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseBoat.drift_speed = StringExtensions.ToFloat(str, 0f);
		};
		array[8] = val;
		val = new Command();
		val.Name = "generate_paths";
		val.Parent = "baseboat";
		val.FullName = "baseboat.generate_paths";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseBoat.generate_paths.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseBoat.generate_paths = StringExtensions.ToBool(str);
		};
		array[9] = val;
		val = new Command();
		val.Name = "seconds_between_shore_drift";
		val.Parent = "baseboat";
		val.FullName = "baseboat.seconds_between_shore_drift";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			int num3 = BaseBoat.seconds_between_shore_drift(arg);
			arg.ReplyWithObject((object)num3);
		};
		array[10] = val;
		val = new Command();
		val.Name = "seconds_until_shore_drift";
		val.Parent = "baseboat";
		val.FullName = "baseboat.seconds_until_shore_drift";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			int num2 = BaseBoat.seconds_until_shore_drift(arg);
			arg.ReplyWithObject((object)num2);
		};
		array[11] = val;
		val = new Command();
		val.Name = "maxactivefireworks";
		val.Parent = "basefirework";
		val.FullName = "basefirework.maxactivefireworks";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseFirework.maxActiveFireworks.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseFirework.maxActiveFireworks = StringExtensions.ToInt(str, 0);
		};
		array[12] = val;
		val = new Command();
		val.Name = "forcefail";
		val.Parent = "basefishingrod";
		val.FullName = "basefishingrod.forcefail";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseFishingRod.ForceFail.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseFishingRod.ForceFail = StringExtensions.ToBool(str);
		};
		array[13] = val;
		val = new Command();
		val.Name = "forcesuccess";
		val.Parent = "basefishingrod";
		val.FullName = "basefishingrod.forcesuccess";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseFishingRod.ForceSuccess.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseFishingRod.ForceSuccess = StringExtensions.ToBool(str);
		};
		array[14] = val;
		val = new Command();
		val.Name = "immediatehook";
		val.Parent = "basefishingrod";
		val.FullName = "basefishingrod.immediatehook";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseFishingRod.ImmediateHook.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseFishingRod.ImmediateHook = StringExtensions.ToBool(str);
		};
		array[15] = val;
		val = new Command();
		val.Name = "missionsenabled";
		val.Parent = "basemission";
		val.FullName = "basemission.missionsenabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseMission.missionsenabled.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseMission.missionsenabled = StringExtensions.ToBool(str);
		};
		array[16] = val;
		val = new Command();
		val.Name = "basenavmovementframeinterval";
		val.Parent = "basenavigator";
		val.FullName = "basenavigator.basenavmovementframeinterval";
		val.ServerAdmin = true;
		val.Description = "How many frames between base navigation movement updates";
		val.Variable = true;
		val.GetOveride = () => BaseNavigator.baseNavMovementFrameInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseNavigator.baseNavMovementFrameInterval = StringExtensions.ToInt(str, 0);
		};
		array[17] = val;
		val = new Command();
		val.Name = "maxstepupdistance";
		val.Parent = "basenavigator";
		val.FullName = "basenavigator.maxstepupdistance";
		val.ServerAdmin = true;
		val.Description = "The max step-up height difference for pet base navigation";
		val.Variable = true;
		val.GetOveride = () => BaseNavigator.maxStepUpDistance.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseNavigator.maxStepUpDistance = StringExtensions.ToFloat(str, 0f);
		};
		array[18] = val;
		val = new Command();
		val.Name = "navtypedistance";
		val.Parent = "basenavigator";
		val.FullName = "basenavigator.navtypedistance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseNavigator.navTypeDistance.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseNavigator.navTypeDistance = StringExtensions.ToFloat(str, 0f);
		};
		array[19] = val;
		val = new Command();
		val.Name = "navtypeheightoffset";
		val.Parent = "basenavigator";
		val.FullName = "basenavigator.navtypeheightoffset";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseNavigator.navTypeHeightOffset.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseNavigator.navTypeHeightOffset = StringExtensions.ToFloat(str, 0f);
		};
		array[20] = val;
		val = new Command();
		val.Name = "stucktriggerduration";
		val.Parent = "basenavigator";
		val.FullName = "basenavigator.stucktriggerduration";
		val.ServerAdmin = true;
		val.Description = "How long we are not moving for before trigger the stuck event";
		val.Variable = true;
		val.GetOveride = () => BaseNavigator.stuckTriggerDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseNavigator.stuckTriggerDuration = StringExtensions.ToFloat(str, 0f);
		};
		array[21] = val;
		val = new Command();
		val.Name = "movementupdatebudgetms";
		val.Parent = "basepet";
		val.FullName = "basepet.movementupdatebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BasePet.movementupdatebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			BasePet.movementupdatebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[22] = val;
		val = new Command();
		val.Name = "onlyqueuebasenavmovements";
		val.Parent = "basepet";
		val.FullName = "basepet.onlyqueuebasenavmovements";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BasePet.onlyQueueBaseNavMovements.ToString();
		val.SetOveride = delegate(string str)
		{
			BasePet.onlyQueueBaseNavMovements = StringExtensions.ToBool(str);
		};
		array[23] = val;
		val = new Command();
		val.Name = "queuedmovementsallowed";
		val.Parent = "basepet";
		val.FullName = "basepet.queuedmovementsallowed";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BasePet.queuedMovementsAllowed.ToString();
		val.SetOveride = delegate(string str)
		{
			BasePet.queuedMovementsAllowed = StringExtensions.ToBool(str);
		};
		array[24] = val;
		val = new Command();
		val.Name = "lifestoryframebudgetms";
		val.Parent = "baseplayer";
		val.FullName = "baseplayer.lifestoryframebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BasePlayer.lifeStoryFramebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			BasePlayer.lifeStoryFramebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[25] = val;
		val = new Command();
		val.Name = "decayminutes";
		val.Parent = "baseridableanimal";
		val.FullName = "baseridableanimal.decayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a horse dies unattended";
		val.Variable = true;
		val.GetOveride = () => BaseRidableAnimal.decayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseRidableAnimal.decayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[26] = val;
		val = new Command();
		val.Name = "dungtimescale";
		val.Parent = "baseridableanimal";
		val.FullName = "baseridableanimal.dungtimescale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseRidableAnimal.dungTimeScale.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseRidableAnimal.dungTimeScale = StringExtensions.ToFloat(str, 0f);
		};
		array[27] = val;
		val = new Command();
		val.Name = "framebudgetms";
		val.Parent = "baseridableanimal";
		val.FullName = "baseridableanimal.framebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseRidableAnimal.framebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseRidableAnimal.framebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[28] = val;
		val = new Command();
		val.Name = "deepwaterdecayminutes";
		val.Parent = "basesubmarine";
		val.FullName = "basesubmarine.deepwaterdecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a submarine loses all its health while in deep water";
		val.Variable = true;
		val.GetOveride = () => BaseSubmarine.deepwaterdecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseSubmarine.deepwaterdecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[29] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "basesubmarine";
		val.FullName = "basesubmarine.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a submarine loses all its health while outside. If it's in deep water, deepwaterdecayminutes is used";
		val.Variable = true;
		val.GetOveride = () => BaseSubmarine.outsidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseSubmarine.outsidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[30] = val;
		val = new Command();
		val.Name = "oxygenminutes";
		val.Parent = "basesubmarine";
		val.FullName = "basesubmarine.oxygenminutes";
		val.ServerAdmin = true;
		val.Description = "How long a submarine can stay underwater until players start taking damage from low oxygen";
		val.Variable = true;
		val.GetOveride = () => BaseSubmarine.oxygenminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseSubmarine.oxygenminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[31] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "bear";
		val.FullName = "bear.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Bear.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Bear.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[32] = val;
		val = new Command();
		val.Name = "spinfrequencyseconds";
		val.Parent = "bigwheelgame";
		val.FullName = "bigwheelgame.spinfrequencyseconds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BigWheelGame.spinFrequencySeconds.ToString();
		val.SetOveride = delegate(string str)
		{
			BigWheelGame.spinFrequencySeconds = StringExtensions.ToFloat(str, 0f);
		};
		array[33] = val;
		val = new Command();
		val.Name = "maxbet";
		val.Parent = "blackjackmachine";
		val.FullName = "blackjackmachine.maxbet";
		val.ServerAdmin = true;
		val.Description = "Maximum initial bet per round";
		val.Variable = true;
		val.GetOveride = () => BlackjackMachine.maxbet.ToString();
		val.SetOveride = delegate(string str)
		{
			BlackjackMachine.maxbet = StringExtensions.ToInt(str, 0);
		};
		array[34] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "boar";
		val.FullName = "boar.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Boar.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Boar.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[35] = val;
		val = new Command();
		val.Name = "backtracklength";
		val.Parent = "boombox";
		val.FullName = "boombox.backtracklength";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => BoomBox.BacktrackLength.ToString();
		val.SetOveride = delegate(string str)
		{
			BoomBox.BacktrackLength = StringExtensions.ToInt(str, 0);
		};
		array[36] = val;
		val = new Command();
		val.Name = "clearradiobyuser";
		val.Parent = "boombox";
		val.FullName = "boombox.clearradiobyuser";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			BoomBox.ClearRadioByUser(arg);
		};
		array[37] = val;
		val = new Command();
		val.Name = "serverurllist";
		val.Parent = "boombox";
		val.FullName = "boombox.serverurllist";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "A list of radio stations that are valid on this server. Format: NAME,URL,NAME,URL,etc";
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => BoomBox.ServerUrlList.ToString();
		val.SetOveride = delegate(string str)
		{
			BoomBox.ServerUrlList = str;
		};
		val.Default = "";
		array[38] = val;
		val = new Command();
		val.Name = "spawnroadbradley";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.spawnroadbradley";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			string text20 = BradleyAPC.svspawnroadbradley(arg.GetVector3(0, Vector3.zero), arg.GetVector3(1, Vector3.zero));
			arg.ReplyWithObject((object)text20);
		};
		array[39] = val;
		val = new Command();
		val.Name = "egress_duration_minutes";
		val.Parent = "cargoship";
		val.FullName = "cargoship.egress_duration_minutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.egress_duration_minutes.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.egress_duration_minutes = StringExtensions.ToFloat(str, 0f);
		};
		array[40] = val;
		val = new Command();
		val.Name = "event_duration_minutes";
		val.Parent = "cargoship";
		val.FullName = "cargoship.event_duration_minutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.event_duration_minutes.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.event_duration_minutes = StringExtensions.ToFloat(str, 0f);
		};
		array[41] = val;
		val = new Command();
		val.Name = "event_enabled";
		val.Parent = "cargoship";
		val.FullName = "cargoship.event_enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.event_enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.event_enabled = StringExtensions.ToBool(str);
		};
		array[42] = val;
		val = new Command();
		val.Name = "loot_round_spacing_minutes";
		val.Parent = "cargoship";
		val.FullName = "cargoship.loot_round_spacing_minutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.loot_round_spacing_minutes.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.loot_round_spacing_minutes = StringExtensions.ToFloat(str, 0f);
		};
		array[43] = val;
		val = new Command();
		val.Name = "loot_rounds";
		val.Parent = "cargoship";
		val.FullName = "cargoship.loot_rounds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.loot_rounds.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.loot_rounds = StringExtensions.ToInt(str, 0);
		};
		array[44] = val;
		val = new Command();
		val.Name = "clearcassettes";
		val.Parent = "cassette";
		val.FullName = "cassette.clearcassettes";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Cassette.ClearCassettes(arg);
		};
		array[45] = val;
		val = new Command();
		val.Name = "clearcassettesbyuser";
		val.Parent = "cassette";
		val.FullName = "cassette.clearcassettesbyuser";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Cassette.ClearCassettesByUser(arg);
		};
		array[46] = val;
		val = new Command();
		val.Name = "maxcassettefilesizemb";
		val.Parent = "cassette";
		val.FullName = "cassette.maxcassettefilesizemb";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Cassette.MaxCassetteFileSizeMB.ToString();
		val.SetOveride = delegate(string str)
		{
			Cassette.MaxCassetteFileSizeMB = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "5";
		array[47] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "chicken";
		val.FullName = "chicken.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Chicken.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Chicken.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[48] = val;
		val = new Command();
		val.Name = "hideobjects";
		val.Parent = "cinematicentity";
		val.FullName = "cinematicentity.hideobjects";
		val.ServerAdmin = true;
		val.Description = "Hides cinematic light source meshes (keeps lights visible)";
		val.Variable = true;
		val.GetOveride = () => CinematicEntity.HideObjects.ToString();
		val.SetOveride = delegate(string str)
		{
			CinematicEntity.HideObjects = StringExtensions.ToBool(str);
		};
		array[49] = val;
		val = new Command();
		val.Name = "clothloddist";
		val.Parent = "clothlod";
		val.FullName = "clothlod.clothloddist";
		val.ServerAdmin = true;
		val.Description = "distance cloth will simulate until";
		val.Variable = true;
		val.GetOveride = () => ClothLOD.clothLODDist.ToString();
		val.SetOveride = delegate(string str)
		{
			ClothLOD.clothLODDist = StringExtensions.ToFloat(str, 0f);
		};
		array[50] = val;
		val = new Command();
		val.Name = "lockoutcooldown";
		val.Parent = "codelock";
		val.FullName = "codelock.lockoutcooldown";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CodeLock.lockoutCooldown.ToString();
		val.SetOveride = delegate(string str)
		{
			CodeLock.lockoutCooldown = StringExtensions.ToFloat(str, 0f);
		};
		array[51] = val;
		val = new Command();
		val.Name = "maxfailedattempts";
		val.Parent = "codelock";
		val.FullName = "codelock.maxfailedattempts";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CodeLock.maxFailedAttempts.ToString();
		val.SetOveride = delegate(string str)
		{
			CodeLock.maxFailedAttempts = StringExtensions.ToFloat(str, 0f);
		};
		array[52] = val;
		val = new Command();
		val.Name = "echo";
		val.Parent = "commands";
		val.FullName = "commands.echo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Commands.Echo(arg.FullString);
		};
		array[53] = val;
		val = new Command();
		val.Name = "find";
		val.Parent = "commands";
		val.FullName = "commands.find";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Commands.Find(arg);
		};
		array[54] = val;
		val = new Command();
		val.Name = "pool_stats";
		val.Parent = "camerarenderermanager";
		val.FullName = "camerarenderermanager.pool_stats";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			CameraRendererManager.pool_stats(arg);
		};
		array[55] = val;
		val = new Command();
		val.Name = "completionframebudgetms";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.completionframebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.completionFrameBudgetMs.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.completionFrameBudgetMs = StringExtensions.ToFloat(str, 0f);
		};
		array[56] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.enabled = StringExtensions.ToBool(str);
		};
		array[57] = val;
		val = new Command();
		val.Name = "entitymaxage";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.entitymaxage";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.entityMaxAge.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.entityMaxAge = StringExtensions.ToInt(str, 0);
		};
		array[58] = val;
		val = new Command();
		val.Name = "entitymaxdistance";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.entitymaxdistance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.entityMaxDistance.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.entityMaxDistance = StringExtensions.ToInt(str, 0);
		};
		array[59] = val;
		val = new Command();
		val.Name = "farplane";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.farplane";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.farPlane.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.farPlane = StringExtensions.ToFloat(str, 0f);
		};
		array[60] = val;
		val = new Command();
		val.Name = "height";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.height";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.height.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.height = StringExtensions.ToInt(str, 0);
		};
		array[61] = val;
		val = new Command();
		val.Name = "layermask";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.layermask";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.layerMask.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.layerMask = StringExtensions.ToInt(str, 0);
		};
		array[62] = val;
		val = new Command();
		val.Name = "maxraysperframe";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.maxraysperframe";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.maxRaysPerFrame.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.maxRaysPerFrame = StringExtensions.ToInt(str, 0);
		};
		array[63] = val;
		val = new Command();
		val.Name = "maxrendersperframe";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.maxrendersperframe";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.maxRendersPerFrame.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.maxRendersPerFrame = StringExtensions.ToInt(str, 0);
		};
		array[64] = val;
		val = new Command();
		val.Name = "nearplane";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.nearplane";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.nearPlane.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.nearPlane = StringExtensions.ToFloat(str, 0f);
		};
		array[65] = val;
		val = new Command();
		val.Name = "playermaxdistance";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.playermaxdistance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.playerMaxDistance.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.playerMaxDistance = StringExtensions.ToInt(str, 0);
		};
		array[66] = val;
		val = new Command();
		val.Name = "playernamemaxdistance";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.playernamemaxdistance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.playerNameMaxDistance.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.playerNameMaxDistance = StringExtensions.ToInt(str, 0);
		};
		array[67] = val;
		val = new Command();
		val.Name = "renderinterval";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.renderinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.renderInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.renderInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[68] = val;
		val = new Command();
		val.Name = "samplesperrender";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.samplesperrender";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.samplesPerRender.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.samplesPerRender = StringExtensions.ToInt(str, 0);
		};
		array[69] = val;
		val = new Command();
		val.Name = "verticalfov";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.verticalfov";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.verticalFov.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.verticalFov = StringExtensions.ToFloat(str, 0f);
		};
		array[70] = val;
		val = new Command();
		val.Name = "width";
		val.Parent = "camerarenderer";
		val.FullName = "camerarenderer.width";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CameraRenderer.width.ToString();
		val.SetOveride = delegate(string str)
		{
			CameraRenderer.width = StringExtensions.ToInt(str, 0);
		};
		array[71] = val;
		val = new Command();
		val.Name = "adminui_deleteugccontent";
		val.Parent = "global";
		val.FullName = "global.adminui_deleteugccontent";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.AdminUI_DeleteUGCContent(arg);
		};
		array[72] = val;
		val = new Command();
		val.Name = "adminui_fullrefresh";
		val.Parent = "global";
		val.FullName = "global.adminui_fullrefresh";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.AdminUI_FullRefresh(arg);
		};
		array[73] = val;
		val = new Command();
		val.Name = "adminui_requestfireworkpattern";
		val.Parent = "global";
		val.FullName = "global.adminui_requestfireworkpattern";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.AdminUI_RequestFireworkPattern(arg);
		};
		array[74] = val;
		val = new Command();
		val.Name = "adminui_requestplayerlist";
		val.Parent = "global";
		val.FullName = "global.adminui_requestplayerlist";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.AdminUI_RequestPlayerList(arg);
		};
		array[75] = val;
		val = new Command();
		val.Name = "adminui_requestserverconvars";
		val.Parent = "global";
		val.FullName = "global.adminui_requestserverconvars";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.AdminUI_RequestServerConvars(arg);
		};
		array[76] = val;
		val = new Command();
		val.Name = "adminui_requestserverinfo";
		val.Parent = "global";
		val.FullName = "global.adminui_requestserverinfo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.AdminUI_RequestServerInfo(arg);
		};
		array[77] = val;
		val = new Command();
		val.Name = "adminui_requestugccontent";
		val.Parent = "global";
		val.FullName = "global.adminui_requestugccontent";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.AdminUI_RequestUGCContent(arg);
		};
		array[78] = val;
		val = new Command();
		val.Name = "adminui_requestugclist";
		val.Parent = "global";
		val.FullName = "global.adminui_requestugclist";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.AdminUI_RequestUGCList(arg);
		};
		array[79] = val;
		val = new Command();
		val.Name = "allowadminui";
		val.Parent = "global";
		val.FullName = "global.allowadminui";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "Controls whether the in-game admin UI is displayed to admins";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Admin.allowAdminUI.ToString();
		val.SetOveride = delegate(string str)
		{
			Admin.allowAdminUI = StringExtensions.ToBool(str);
		};
		val.Default = "True";
		array[80] = val;
		val = new Command();
		val.Name = "authcount";
		val.Parent = "global";
		val.FullName = "global.authcount";
		val.ServerAdmin = true;
		val.Description = "Returns all entities that the provided player is authed to (TC's, locks, etc), supports --json";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.authcount(arg);
		};
		array[81] = val;
		val = new Command();
		val.Name = "authradius";
		val.Parent = "global";
		val.FullName = "global.authradius";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.authradius(arg);
		};
		array[82] = val;
		val = new Command();
		val.Name = "ban";
		val.Parent = "global";
		val.FullName = "global.ban";
		val.ServerAdmin = true;
		val.Description = "ban <player> <reason> [optional duration]";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.ban(arg);
		};
		array[83] = val;
		val = new Command();
		val.Name = "banid";
		val.Parent = "global";
		val.FullName = "global.banid";
		val.ServerAdmin = true;
		val.Description = "banid <steamid> <username> <reason> [optional duration]";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.banid(arg);
		};
		array[84] = val;
		val = new Command();
		val.Name = "banlist";
		val.Parent = "global";
		val.FullName = "global.banlist";
		val.ServerAdmin = true;
		val.Description = "List of banned users (sourceds compat)";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.banlist(arg);
		};
		array[85] = val;
		val = new Command();
		val.Name = "banlistex";
		val.Parent = "global";
		val.FullName = "global.banlistex";
		val.ServerAdmin = true;
		val.Description = "List of banned users - shows reasons and usernames";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.banlistex(arg);
		};
		array[86] = val;
		val = new Command();
		val.Name = "bans";
		val.Parent = "global";
		val.FullName = "global.bans";
		val.ServerAdmin = true;
		val.Description = "List of banned users";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ServerUsers.User[] array3 = Admin.Bans();
			arg.ReplyWithObject((object)array3);
		};
		array[87] = val;
		val = new Command();
		val.Name = "buildinfo";
		val.Parent = "global";
		val.FullName = "global.buildinfo";
		val.ServerAdmin = true;
		val.Description = "Get information about this build";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			BuildInfo val2 = Admin.BuildInfo();
			arg.ReplyWithObject((object)val2);
		};
		array[88] = val;
		val = new Command();
		val.Name = "carstats";
		val.Parent = "global";
		val.FullName = "global.carstats";
		val.ServerAdmin = true;
		val.Description = "Get information about all the cars in the world";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.carstats(arg);
		};
		array[89] = val;
		val = new Command();
		val.Name = "clearugcentitiesinrange";
		val.Parent = "global";
		val.FullName = "global.clearugcentitiesinrange";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.clearugcentitiesinrange(arg);
		};
		array[90] = val;
		val = new Command();
		val.Name = "clearugcentity";
		val.Parent = "global";
		val.FullName = "global.clearugcentity";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.clearugcentity(arg);
		};
		array[91] = val;
		val = new Command();
		val.Name = "clientperf";
		val.Parent = "global";
		val.FullName = "global.clientperf";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.clientperf(arg);
		};
		array[92] = val;
		val = new Command();
		val.Name = "clientperf_frametime";
		val.Parent = "global";
		val.FullName = "global.clientperf_frametime";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.clientperf_frametime(arg);
		};
		array[93] = val;
		val = new Command();
		val.Name = "deauthradius";
		val.Parent = "global";
		val.FullName = "global.deauthradius";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.deauthradius(arg);
		};
		array[94] = val;
		val = new Command();
		val.Name = "entcount";
		val.Parent = "global";
		val.FullName = "global.entcount";
		val.ServerAdmin = true;
		val.Description = "Returns all entities that the provided player has placed, supports --json";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.entcount(arg);
		};
		array[95] = val;
		val = new Command();
		val.Name = "entid";
		val.Parent = "global";
		val.FullName = "global.entid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.entid(arg);
		};
		array[96] = val;
		val = new Command();
		val.Name = "getugcinfo";
		val.Parent = "global";
		val.FullName = "global.getugcinfo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.getugcinfo(arg);
		};
		array[97] = val;
		val = new Command();
		val.Name = "injureplayer";
		val.Parent = "global";
		val.FullName = "global.injureplayer";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.injureplayer(arg);
		};
		array[98] = val;
		val = new Command();
		val.Name = "kick";
		val.Parent = "global";
		val.FullName = "global.kick";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.kick(arg);
		};
		array[99] = val;
		val = new Command();
		val.Name = "kickall";
		val.Parent = "global";
		val.FullName = "global.kickall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.kickall(arg);
		};
		array[100] = val;
		val = new Command();
		val.Name = "killplayer";
		val.Parent = "global";
		val.FullName = "global.killplayer";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.killplayer(arg);
		};
		array[101] = val;
		val = new Command();
		val.Name = "listid";
		val.Parent = "global";
		val.FullName = "global.listid";
		val.ServerAdmin = true;
		val.Description = "List of banned users, by ID (sourceds compat)";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.listid(arg);
		};
		array[102] = val;
		val = new Command();
		val.Name = "moderatorid";
		val.Parent = "global";
		val.FullName = "global.moderatorid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.moderatorid(arg);
		};
		array[103] = val;
		val = new Command();
		val.Name = "mute";
		val.Parent = "global";
		val.FullName = "global.mute";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.mute(arg);
		};
		array[104] = val;
		val = new Command();
		val.Name = "mutelist";
		val.Parent = "global";
		val.FullName = "global.mutelist";
		val.ServerAdmin = true;
		val.Description = "Print a list of currently muted players";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.mutelist(arg);
		};
		array[105] = val;
		val = new Command();
		val.Name = "ownerid";
		val.Parent = "global";
		val.FullName = "global.ownerid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.ownerid(arg);
		};
		array[106] = val;
		val = new Command();
		val.Name = "playerlist";
		val.Parent = "global";
		val.FullName = "global.playerlist";
		val.ServerAdmin = true;
		val.Description = "Get a list of players";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.PlayerInfo[] array2 = Admin.playerlist();
			arg.ReplyWithObject((object)array2);
		};
		array[107] = val;
		val = new Command();
		val.Name = "players";
		val.Parent = "global";
		val.FullName = "global.players";
		val.ServerAdmin = true;
		val.Description = "Print out currently connected clients etc";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.players(arg);
		};
		array[108] = val;
		val = new Command();
		val.Name = "recoverplayer";
		val.Parent = "global";
		val.FullName = "global.recoverplayer";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.recoverplayer(arg);
		};
		array[109] = val;
		val = new Command();
		val.Name = "removemoderator";
		val.Parent = "global";
		val.FullName = "global.removemoderator";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.removemoderator(arg);
		};
		array[110] = val;
		val = new Command();
		val.Name = "removeowner";
		val.Parent = "global";
		val.FullName = "global.removeowner";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.removeowner(arg);
		};
		array[111] = val;
		val = new Command();
		val.Name = "removeskipqueue";
		val.Parent = "global";
		val.FullName = "global.removeskipqueue";
		val.ServerAdmin = true;
		val.Description = "Removes skip queue permission from a SteamID";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.removeskipqueue(arg);
		};
		array[112] = val;
		val = new Command();
		val.Name = "say";
		val.Parent = "global";
		val.FullName = "global.say";
		val.ServerAdmin = true;
		val.Description = "Sends a message in chat";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.say(arg);
		};
		array[113] = val;
		val = new Command();
		val.Name = "serverinfo";
		val.Parent = "global";
		val.FullName = "global.serverinfo";
		val.ServerAdmin = true;
		val.Description = "Get a list of information about the server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.ServerInfoOutput serverInfoOutput = Admin.ServerInfo();
			arg.ReplyWithObject((object)serverInfoOutput);
		};
		array[114] = val;
		val = new Command();
		val.Name = "skin_radius";
		val.Parent = "global";
		val.FullName = "global.skin_radius";
		val.ServerAdmin = true;
		val.Description = "skin_radius 'skin' 'radius'";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.skin_radius(arg);
		};
		array[115] = val;
		val = new Command();
		val.Name = "skipqueue";
		val.Parent = "global";
		val.FullName = "global.skipqueue";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.skipqueue(arg);
		};
		array[116] = val;
		val = new Command();
		val.Name = "skipqueueid";
		val.Parent = "global";
		val.FullName = "global.skipqueueid";
		val.ServerAdmin = true;
		val.Description = "Adds skip queue permissions to a SteamID";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.skipqueueid(arg);
		};
		array[117] = val;
		val = new Command();
		val.Name = "sleepingusers";
		val.Parent = "global";
		val.FullName = "global.sleepingusers";
		val.ServerAdmin = true;
		val.Description = "Show user info for players on server.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.sleepingusers(arg);
		};
		array[118] = val;
		val = new Command();
		val.Name = "sleepingusersinrange";
		val.Parent = "global";
		val.FullName = "global.sleepingusersinrange";
		val.ServerAdmin = true;
		val.Description = "Show user info for sleeping players on server in range of the player.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.sleepingusersinrange(arg);
		};
		array[119] = val;
		val = new Command();
		val.Name = "stats";
		val.Parent = "global";
		val.FullName = "global.stats";
		val.ServerAdmin = true;
		val.Description = "Print out stats of currently connected clients";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.stats(arg);
		};
		array[120] = val;
		val = new Command();
		val.Name = "status";
		val.Parent = "global";
		val.FullName = "global.status";
		val.ServerAdmin = true;
		val.Description = "Print out currently connected clients";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.status(arg);
		};
		array[121] = val;
		val = new Command();
		val.Name = "teaminfo";
		val.Parent = "global";
		val.FullName = "global.teaminfo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text19 = Admin.teaminfo(arg);
			arg.ReplyWithObject((object)text19);
		};
		array[122] = val;
		val = new Command();
		val.Name = "unban";
		val.Parent = "global";
		val.FullName = "global.unban";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.unban(arg);
		};
		array[123] = val;
		val = new Command();
		val.Name = "unmute";
		val.Parent = "global";
		val.FullName = "global.unmute";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.unmute(arg);
		};
		array[124] = val;
		val = new Command();
		val.Name = "upgrade_radius";
		val.Parent = "global";
		val.FullName = "global.upgrade_radius";
		val.ServerAdmin = true;
		val.Description = "upgrade_radius 'grade' 'radius'";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.upgrade_radius(arg);
		};
		array[125] = val;
		val = new Command();
		val.Name = "users";
		val.Parent = "global";
		val.FullName = "global.users";
		val.ServerAdmin = true;
		val.Description = "Show user info for players on server.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.users(arg);
		};
		array[126] = val;
		val = new Command();
		val.Name = "usersinrange";
		val.Parent = "global";
		val.FullName = "global.usersinrange";
		val.ServerAdmin = true;
		val.Description = "Show user info for players on server in range of the player.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.usersinrange(arg);
		};
		array[127] = val;
		val = new Command();
		val.Name = "usersinrangeofplayer";
		val.Parent = "global";
		val.FullName = "global.usersinrangeofplayer";
		val.ServerAdmin = true;
		val.Description = "Show user info for players on server in range of the supplied player (eg. Jim 50)";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.usersinrangeofplayer(arg);
		};
		array[128] = val;
		val = new Command();
		val.Name = "accuratevisiondistance";
		val.Parent = "ai";
		val.FullName = "ai.accuratevisiondistance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.accuratevisiondistance.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.accuratevisiondistance = StringExtensions.ToBool(str);
		};
		array[129] = val;
		val = new Command();
		val.Name = "addignoreplayer";
		val.Parent = "ai";
		val.FullName = "ai.addignoreplayer";
		val.ServerAdmin = true;
		val.Description = "Add a player (or command user if no player is specified) to the AIs ignore list.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.addignoreplayer(arg);
		};
		array[130] = val;
		val = new Command();
		val.Name = "allowdesigning";
		val.Parent = "ai";
		val.FullName = "ai.allowdesigning";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => AI.allowdesigning.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.allowdesigning = StringExtensions.ToBool(str);
		};
		val.Default = "True";
		array[131] = val;
		val = new Command();
		val.Name = "animal_ignore_food";
		val.Parent = "ai";
		val.FullName = "ai.animal_ignore_food";
		val.ServerAdmin = true;
		val.Description = "If animal_ignore_food is true, animals will not sense food sources or interact with them (server optimization). (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.animal_ignore_food.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.animal_ignore_food = StringExtensions.ToBool(str);
		};
		array[132] = val;
		val = new Command();
		val.Name = "brainstats";
		val.Parent = "ai";
		val.FullName = "ai.brainstats";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.brainstats(arg);
		};
		array[133] = val;
		val = new Command();
		val.Name = "clearignoredplayers";
		val.Parent = "ai";
		val.FullName = "ai.clearignoredplayers";
		val.ServerAdmin = true;
		val.Description = "Remove all players from the AIs ignore list.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.clearignoredplayers(arg);
		};
		array[134] = val;
		val = new Command();
		val.Name = "frametime";
		val.Parent = "ai";
		val.FullName = "ai.frametime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.frametime.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.frametime = StringExtensions.ToFloat(str, 0f);
		};
		array[135] = val;
		val = new Command();
		val.Name = "groups";
		val.Parent = "ai";
		val.FullName = "ai.groups";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.groups.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.groups = StringExtensions.ToBool(str);
		};
		array[136] = val;
		val = new Command();
		val.Name = "ignoreplayers";
		val.Parent = "ai";
		val.FullName = "ai.ignoreplayers";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.ignoreplayers.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.ignoreplayers = StringExtensions.ToBool(str);
		};
		array[137] = val;
		val = new Command();
		val.Name = "killanimals";
		val.Parent = "ai";
		val.FullName = "ai.killanimals";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.killanimals(arg);
		};
		array[138] = val;
		val = new Command();
		val.Name = "killscientists";
		val.Parent = "ai";
		val.FullName = "ai.killscientists";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.killscientists(arg);
		};
		array[139] = val;
		val = new Command();
		val.Name = "move";
		val.Parent = "ai";
		val.FullName = "ai.move";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.move.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.move = StringExtensions.ToBool(str);
		};
		array[140] = val;
		val = new Command();
		val.Name = "nav_carve_height";
		val.Parent = "ai";
		val.FullName = "ai.nav_carve_height";
		val.ServerAdmin = true;
		val.Description = "The height of the carve volume. (default: 2)";
		val.Variable = true;
		val.GetOveride = () => AI.nav_carve_height.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.nav_carve_height = StringExtensions.ToFloat(str, 0f);
		};
		array[141] = val;
		val = new Command();
		val.Name = "nav_carve_min_base_size";
		val.Parent = "ai";
		val.FullName = "ai.nav_carve_min_base_size";
		val.ServerAdmin = true;
		val.Description = "The minimum size we allow a carving volume to be. (default: 2)";
		val.Variable = true;
		val.GetOveride = () => AI.nav_carve_min_base_size.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.nav_carve_min_base_size = StringExtensions.ToFloat(str, 0f);
		};
		array[142] = val;
		val = new Command();
		val.Name = "nav_carve_min_building_blocks_to_apply_optimization";
		val.Parent = "ai";
		val.FullName = "ai.nav_carve_min_building_blocks_to_apply_optimization";
		val.ServerAdmin = true;
		val.Description = "The minimum number of building blocks a building needs to consist of for this optimization to be applied. (default: 25)";
		val.Variable = true;
		val.GetOveride = () => AI.nav_carve_min_building_blocks_to_apply_optimization.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.nav_carve_min_building_blocks_to_apply_optimization = StringExtensions.ToInt(str, 0);
		};
		array[143] = val;
		val = new Command();
		val.Name = "nav_carve_size_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.nav_carve_size_multiplier";
		val.ServerAdmin = true;
		val.Description = "The size multiplier applied to the size of the carve volume. The smaller the value, the tighter the skirt around foundation edges, but too small and animals can attack through walls. (default: 4)";
		val.Variable = true;
		val.GetOveride = () => AI.nav_carve_size_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.nav_carve_size_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[144] = val;
		val = new Command();
		val.Name = "nav_carve_use_building_optimization";
		val.Parent = "ai";
		val.FullName = "ai.nav_carve_use_building_optimization";
		val.ServerAdmin = true;
		val.Description = "If nav_carve_use_building_optimization is true, we attempt to reduce the amount of navmesh carves for a building. (default: false)";
		val.Variable = true;
		val.GetOveride = () => AI.nav_carve_use_building_optimization.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.nav_carve_use_building_optimization = StringExtensions.ToBool(str);
		};
		array[145] = val;
		val = new Command();
		val.Name = "navthink";
		val.Parent = "ai";
		val.FullName = "ai.navthink";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.navthink.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.navthink = StringExtensions.ToBool(str);
		};
		array[146] = val;
		val = new Command();
		val.Name = "npc_alertness_drain_rate";
		val.Parent = "ai";
		val.FullName = "ai.npc_alertness_drain_rate";
		val.ServerAdmin = true;
		val.Description = "npc_alertness_drain_rate define the rate at which we drain the alertness level of an NPC when there are no enemies in sight. (Default: 0.01)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_alertness_drain_rate.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_alertness_drain_rate = StringExtensions.ToFloat(str, 0f);
		};
		array[147] = val;
		val = new Command();
		val.Name = "npc_alertness_to_aim_modifier";
		val.Parent = "ai";
		val.FullName = "ai.npc_alertness_to_aim_modifier";
		val.ServerAdmin = true;
		val.Description = "This is multiplied with the current alertness (0-10) to decide how long it will take for the NPC to deliberately miss again. (default: 0.33)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_alertness_to_aim_modifier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_alertness_to_aim_modifier = StringExtensions.ToFloat(str, 0f);
		};
		array[148] = val;
		val = new Command();
		val.Name = "npc_alertness_zero_detection_mod";
		val.Parent = "ai";
		val.FullName = "ai.npc_alertness_zero_detection_mod";
		val.ServerAdmin = true;
		val.Description = "npc_alertness_zero_detection_mod define the threshold of visibility required to detect an enemy when alertness is zero. (Default: 0.5)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_alertness_zero_detection_mod.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_alertness_zero_detection_mod = StringExtensions.ToFloat(str, 0f);
		};
		array[149] = val;
		val = new Command();
		val.Name = "npc_cover_compromised_cooldown";
		val.Parent = "ai";
		val.FullName = "ai.npc_cover_compromised_cooldown";
		val.ServerAdmin = true;
		val.Description = "npc_cover_compromised_cooldown defines how long a cover point is marked as compromised before it's cleared again for selection. (default: 10)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_cover_compromised_cooldown.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_cover_compromised_cooldown = StringExtensions.ToFloat(str, 0f);
		};
		array[150] = val;
		val = new Command();
		val.Name = "npc_cover_info_tick_rate_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.npc_cover_info_tick_rate_multiplier";
		val.ServerAdmin = true;
		val.Description = "The rate at which we gather information about available cover points. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 20)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_cover_info_tick_rate_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_cover_info_tick_rate_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[151] = val;
		val = new Command();
		val.Name = "npc_cover_path_vs_straight_dist_max_diff";
		val.Parent = "ai";
		val.FullName = "ai.npc_cover_path_vs_straight_dist_max_diff";
		val.ServerAdmin = true;
		val.Description = "npc_cover_path_vs_straight_dist_max_diff defines what the maximum difference between straight-line distance and path distance can be when evaluating cover points. (default: 2)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_cover_path_vs_straight_dist_max_diff.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_cover_path_vs_straight_dist_max_diff = StringExtensions.ToFloat(str, 0f);
		};
		array[152] = val;
		val = new Command();
		val.Name = "npc_cover_use_path_distance";
		val.Parent = "ai";
		val.FullName = "ai.npc_cover_use_path_distance";
		val.ServerAdmin = true;
		val.Description = "If npc_cover_use_path_distance is set to true then npcs will look at the distance between the cover point and their target using the path between the two, rather than the straight-line distance.";
		val.Variable = true;
		val.GetOveride = () => AI.npc_cover_use_path_distance.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_cover_use_path_distance = StringExtensions.ToBool(str);
		};
		array[153] = val;
		val = new Command();
		val.Name = "npc_deliberate_hit_randomizer";
		val.Parent = "ai";
		val.FullName = "ai.npc_deliberate_hit_randomizer";
		val.ServerAdmin = true;
		val.Description = "The percentage away from a maximum miss the randomizer is allowed to travel when shooting to deliberately hit the target (we don't want perfect hits with every shot). (default: 0.85f)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_deliberate_hit_randomizer.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_deliberate_hit_randomizer = StringExtensions.ToFloat(str, 0f);
		};
		array[154] = val;
		val = new Command();
		val.Name = "npc_deliberate_miss_offset_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.npc_deliberate_miss_offset_multiplier";
		val.ServerAdmin = true;
		val.Description = "The offset with which the NPC will maximum miss the target. (default: 1.25)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_deliberate_miss_offset_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_deliberate_miss_offset_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[155] = val;
		val = new Command();
		val.Name = "npc_deliberate_miss_to_hit_alignment_time";
		val.Parent = "ai";
		val.FullName = "ai.npc_deliberate_miss_to_hit_alignment_time";
		val.ServerAdmin = true;
		val.Description = "The time it takes for the NPC to deliberately miss to the time the NPC tries to hit its target. (default: 1.5)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_deliberate_miss_to_hit_alignment_time.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_deliberate_miss_to_hit_alignment_time = StringExtensions.ToFloat(str, 0f);
		};
		array[156] = val;
		val = new Command();
		val.Name = "npc_door_trigger_size";
		val.Parent = "ai";
		val.FullName = "ai.npc_door_trigger_size";
		val.ServerAdmin = true;
		val.Description = "npc_door_trigger_size defines the size of the trigger box on doors that opens the door as npcs walk close to it (default: 1.5)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_door_trigger_size.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_door_trigger_size = StringExtensions.ToFloat(str, 0f);
		};
		array[157] = val;
		val = new Command();
		val.Name = "npc_enable";
		val.Parent = "ai";
		val.FullName = "ai.npc_enable";
		val.ServerAdmin = true;
		val.Description = "If npc_enable is set to false then npcs won't spawn. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_enable.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_enable = StringExtensions.ToBool(str);
		};
		array[158] = val;
		val = new Command();
		val.Name = "npc_families_no_hurt";
		val.Parent = "ai";
		val.FullName = "ai.npc_families_no_hurt";
		val.ServerAdmin = true;
		val.Description = "If npc_families_no_hurt is true, npcs of the same family won't be able to hurt each other. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_families_no_hurt.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_families_no_hurt = StringExtensions.ToBool(str);
		};
		array[159] = val;
		val = new Command();
		val.Name = "npc_gun_noise_silencer_modifier";
		val.Parent = "ai";
		val.FullName = "ai.npc_gun_noise_silencer_modifier";
		val.ServerAdmin = true;
		val.Description = "The modifier by which a silencer reduce the noise that a gun makes when shot. (Default: 0.15)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_gun_noise_silencer_modifier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_gun_noise_silencer_modifier = StringExtensions.ToFloat(str, 0f);
		};
		array[160] = val;
		val = new Command();
		val.Name = "npc_htn_player_base_damage_modifier";
		val.Parent = "ai";
		val.FullName = "ai.npc_htn_player_base_damage_modifier";
		val.ServerAdmin = true;
		val.Description = "Baseline damage modifier for the new HTN Player NPCs to nerf their damage compared to the old NPCs. (default: 1.15f)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_htn_player_base_damage_modifier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_htn_player_base_damage_modifier = StringExtensions.ToFloat(str, 0f);
		};
		array[161] = val;
		val = new Command();
		val.Name = "npc_htn_player_frustration_threshold";
		val.Parent = "ai";
		val.FullName = "ai.npc_htn_player_frustration_threshold";
		val.ServerAdmin = true;
		val.Description = "npc_htn_player_frustration_threshold defines where the frustration threshold for NPCs go, where they have the opportunity to change to a more aggressive tactic. (default: 3)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_htn_player_frustration_threshold.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_htn_player_frustration_threshold = StringExtensions.ToInt(str, 0);
		};
		array[162] = val;
		val = new Command();
		val.Name = "npc_ignore_chairs";
		val.Parent = "ai";
		val.FullName = "ai.npc_ignore_chairs";
		val.ServerAdmin = true;
		val.Description = "If npc_ignore_chairs is true, npcs won't care about seeking out and sitting in chairs. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_ignore_chairs.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_ignore_chairs = StringExtensions.ToBool(str);
		};
		array[163] = val;
		val = new Command();
		val.Name = "npc_junkpile_a_spawn_chance";
		val.Parent = "ai";
		val.FullName = "ai.npc_junkpile_a_spawn_chance";
		val.ServerAdmin = true;
		val.Description = "npc_junkpile_a_spawn_chance define the chance for scientists to spawn at junkpile a. (Default: 0.1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_junkpile_a_spawn_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_junkpile_a_spawn_chance = StringExtensions.ToFloat(str, 0f);
		};
		array[164] = val;
		val = new Command();
		val.Name = "npc_junkpile_dist_aggro_gate";
		val.Parent = "ai";
		val.FullName = "ai.npc_junkpile_dist_aggro_gate";
		val.ServerAdmin = true;
		val.Description = "npc_junkpile_dist_aggro_gate define at what range (or closer) a junkpile scientist will get aggressive. (Default: 8)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_junkpile_dist_aggro_gate.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_junkpile_dist_aggro_gate = StringExtensions.ToFloat(str, 0f);
		};
		array[165] = val;
		val = new Command();
		val.Name = "npc_junkpile_g_spawn_chance";
		val.Parent = "ai";
		val.FullName = "ai.npc_junkpile_g_spawn_chance";
		val.ServerAdmin = true;
		val.Description = "npc_junkpile_g_spawn_chance define the chance for scientists to spawn at junkpile g. (Default: 0.1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_junkpile_g_spawn_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_junkpile_g_spawn_chance = StringExtensions.ToFloat(str, 0f);
		};
		array[166] = val;
		val = new Command();
		val.Name = "npc_junkpilespawn_chance";
		val.Parent = "ai";
		val.FullName = "ai.npc_junkpilespawn_chance";
		val.ServerAdmin = true;
		val.Description = "defines the chance for scientists to spawn at NPC junkpiles. (Default: 0.1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_junkpilespawn_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_junkpilespawn_chance = StringExtensions.ToFloat(str, 0f);
		};
		array[167] = val;
		val = new Command();
		val.Name = "npc_max_junkpile_count";
		val.Parent = "ai";
		val.FullName = "ai.npc_max_junkpile_count";
		val.ServerAdmin = true;
		val.Description = "npc_max_junkpile_count define how many npcs can spawn into the world at junkpiles at the same time (does not include monuments) (Default: 30)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_max_junkpile_count.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_max_junkpile_count = StringExtensions.ToInt(str, 0);
		};
		array[168] = val;
		val = new Command();
		val.Name = "npc_max_population_military_tunnels";
		val.Parent = "ai";
		val.FullName = "ai.npc_max_population_military_tunnels";
		val.ServerAdmin = true;
		val.Description = "npc_max_population_military_tunnels defines the size of the npc population at military tunnels. (default: 3)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_max_population_military_tunnels.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_max_population_military_tunnels = StringExtensions.ToInt(str, 0);
		};
		array[169] = val;
		val = new Command();
		val.Name = "npc_max_roam_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.npc_max_roam_multiplier";
		val.ServerAdmin = true;
		val.Description = "This is multiplied with the max roam range stat of an NPC to determine how far from its spawn point the NPC is allowed to roam. (default: 3)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_max_roam_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_max_roam_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[170] = val;
		val = new Command();
		val.Name = "npc_only_hurt_active_target_in_safezone";
		val.Parent = "ai";
		val.FullName = "ai.npc_only_hurt_active_target_in_safezone";
		val.ServerAdmin = true;
		val.Description = "If npc_only_hurt_active_target_in_safezone is true, npcs won't any player other than their actively targeted player when in a safe zone. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_only_hurt_active_target_in_safezone.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_only_hurt_active_target_in_safezone = StringExtensions.ToBool(str);
		};
		array[171] = val;
		val = new Command();
		val.Name = "npc_patrol_point_cooldown";
		val.Parent = "ai";
		val.FullName = "ai.npc_patrol_point_cooldown";
		val.ServerAdmin = true;
		val.Description = "npc_patrol_point_cooldown defines the cooldown time on a patrol point until it's available again (default: 5)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_patrol_point_cooldown.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_patrol_point_cooldown = StringExtensions.ToFloat(str, 0f);
		};
		array[172] = val;
		val = new Command();
		val.Name = "npc_reasoning_system_tick_rate_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.npc_reasoning_system_tick_rate_multiplier";
		val.ServerAdmin = true;
		val.Description = "The rate at which we tick the reasoning system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_reasoning_system_tick_rate_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_reasoning_system_tick_rate_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[173] = val;
		val = new Command();
		val.Name = "npc_respawn_delay_max_military_tunnels";
		val.Parent = "ai";
		val.FullName = "ai.npc_respawn_delay_max_military_tunnels";
		val.ServerAdmin = true;
		val.Description = "npc_respawn_delay_max_military_tunnels defines the maximum delay between spawn ticks at military tunnels. (default: 1920)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_respawn_delay_max_military_tunnels.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_respawn_delay_max_military_tunnels = StringExtensions.ToFloat(str, 0f);
		};
		array[174] = val;
		val = new Command();
		val.Name = "npc_respawn_delay_min_military_tunnels";
		val.Parent = "ai";
		val.FullName = "ai.npc_respawn_delay_min_military_tunnels";
		val.ServerAdmin = true;
		val.Description = "npc_respawn_delay_min_military_tunnels defines the minimum delay between spawn ticks at military tunnels. (default: 480)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_respawn_delay_min_military_tunnels.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_respawn_delay_min_military_tunnels = StringExtensions.ToFloat(str, 0f);
		};
		array[175] = val;
		val = new Command();
		val.Name = "npc_sensory_system_tick_rate_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.npc_sensory_system_tick_rate_multiplier";
		val.ServerAdmin = true;
		val.Description = "The rate at which we tick the sensory system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 5)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_sensory_system_tick_rate_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_sensory_system_tick_rate_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[176] = val;
		val = new Command();
		val.Name = "npc_spawn_on_cargo_ship";
		val.Parent = "ai";
		val.FullName = "ai.npc_spawn_on_cargo_ship";
		val.ServerAdmin = true;
		val.Description = "Spawn NPCs on the Cargo Ship. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_spawn_on_cargo_ship.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_spawn_on_cargo_ship = StringExtensions.ToBool(str);
		};
		array[177] = val;
		val = new Command();
		val.Name = "npc_spawn_per_tick_max_military_tunnels";
		val.Parent = "ai";
		val.FullName = "ai.npc_spawn_per_tick_max_military_tunnels";
		val.ServerAdmin = true;
		val.Description = "npc_spawn_per_tick_max_military_tunnels defines how many can maximum spawn at once at military tunnels. (default: 1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_spawn_per_tick_max_military_tunnels.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_spawn_per_tick_max_military_tunnels = StringExtensions.ToInt(str, 0);
		};
		array[178] = val;
		val = new Command();
		val.Name = "npc_spawn_per_tick_min_military_tunnels";
		val.Parent = "ai";
		val.FullName = "ai.npc_spawn_per_tick_min_military_tunnels";
		val.ServerAdmin = true;
		val.Description = "npc_spawn_per_tick_min_military_tunnels defineshow many will minimum spawn at once at military tunnels. (default: 1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_spawn_per_tick_min_military_tunnels.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_spawn_per_tick_min_military_tunnels = StringExtensions.ToInt(str, 0);
		};
		array[179] = val;
		val = new Command();
		val.Name = "npc_speed_crouch_run";
		val.Parent = "ai";
		val.FullName = "ai.npc_speed_crouch_run";
		val.ServerAdmin = true;
		val.Description = "npc_speed_crouch_run define the speed of an npc when in the crouched run state, and should be a number between 0 and 1. (Default: 0.25)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_speed_crouch_run.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_speed_crouch_run = StringExtensions.ToFloat(str, 0f);
		};
		array[180] = val;
		val = new Command();
		val.Name = "npc_speed_crouch_walk";
		val.Parent = "ai";
		val.FullName = "ai.npc_speed_crouch_walk";
		val.ServerAdmin = true;
		val.Description = "npc_speed_walk define the speed of an npc when in the crouched walk state, and should be a number between 0 and 1. (Default: 0.1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_speed_crouch_walk.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_speed_crouch_walk = StringExtensions.ToFloat(str, 0f);
		};
		array[181] = val;
		val = new Command();
		val.Name = "npc_speed_run";
		val.Parent = "ai";
		val.FullName = "ai.npc_speed_run";
		val.ServerAdmin = true;
		val.Description = "npc_speed_walk define the speed of an npc when in the run state, and should be a number between 0 and 1. (Default: 0.4)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_speed_run.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_speed_run = StringExtensions.ToFloat(str, 0f);
		};
		array[182] = val;
		val = new Command();
		val.Name = "npc_speed_sprint";
		val.Parent = "ai";
		val.FullName = "ai.npc_speed_sprint";
		val.ServerAdmin = true;
		val.Description = "npc_speed_walk define the speed of an npc when in the sprint state, and should be a number between 0 and 1. (Default: 1.0)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_speed_sprint.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_speed_sprint = StringExtensions.ToFloat(str, 0f);
		};
		array[183] = val;
		val = new Command();
		val.Name = "npc_speed_walk";
		val.Parent = "ai";
		val.FullName = "ai.npc_speed_walk";
		val.ServerAdmin = true;
		val.Description = "npc_speed_walk define the speed of an npc when in the walk state, and should be a number between 0 and 1. (Default: 0.18)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_speed_walk.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_speed_walk = StringExtensions.ToFloat(str, 0f);
		};
		array[184] = val;
		val = new Command();
		val.Name = "npc_use_new_aim_system";
		val.Parent = "ai";
		val.FullName = "ai.npc_use_new_aim_system";
		val.ServerAdmin = true;
		val.Description = "If npc_use_new_aim_system is true, npcs will miss on purpose on occasion, where the old system would randomize aim cone. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_use_new_aim_system.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_use_new_aim_system = StringExtensions.ToBool(str);
		};
		array[185] = val;
		val = new Command();
		val.Name = "npc_use_thrown_weapons";
		val.Parent = "ai";
		val.FullName = "ai.npc_use_thrown_weapons";
		val.ServerAdmin = true;
		val.Description = "If npc_use_thrown_weapons is true, npcs will throw grenades, etc. This is an experimental feature. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_use_thrown_weapons.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_use_thrown_weapons = StringExtensions.ToBool(str);
		};
		array[186] = val;
		val = new Command();
		val.Name = "npc_valid_aim_cone";
		val.Parent = "ai";
		val.FullName = "ai.npc_valid_aim_cone";
		val.ServerAdmin = true;
		val.Description = "npc_valid_aim_cone defines how close their aim needs to be on target in order to fire. (default: 0.8)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_valid_aim_cone.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_valid_aim_cone = StringExtensions.ToFloat(str, 0f);
		};
		array[187] = val;
		val = new Command();
		val.Name = "npc_valid_mounted_aim_cone";
		val.Parent = "ai";
		val.FullName = "ai.npc_valid_mounted_aim_cone";
		val.ServerAdmin = true;
		val.Description = "npc_valid_mounted_aim_cone defines how close their aim needs to be on target in order to fire while mounted. (default: 0.92)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_valid_mounted_aim_cone.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_valid_mounted_aim_cone = StringExtensions.ToFloat(str, 0f);
		};
		array[188] = val;
		val = new Command();
		val.Name = "npcswimming";
		val.Parent = "ai";
		val.FullName = "ai.npcswimming";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.npcswimming.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npcswimming = StringExtensions.ToBool(str);
		};
		array[189] = val;
		val = new Command();
		val.Name = "ocean_patrol_path_iterations";
		val.Parent = "ai";
		val.FullName = "ai.ocean_patrol_path_iterations";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.ocean_patrol_path_iterations.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.ocean_patrol_path_iterations = StringExtensions.ToInt(str, 0);
		};
		array[190] = val;
		val = new Command();
		val.Name = "printignoredplayers";
		val.Parent = "ai";
		val.FullName = "ai.printignoredplayers";
		val.ServerAdmin = true;
		val.Description = "Print a lost of all the players in the AI ignore list.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.printignoredplayers(arg);
		};
		array[191] = val;
		val = new Command();
		val.Name = "removeignoreplayer";
		val.Parent = "ai";
		val.FullName = "ai.removeignoreplayer";
		val.ServerAdmin = true;
		val.Description = "Remove a player (or command user if no player is specified) from the AIs ignore list.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.removeignoreplayer(arg);
		};
		array[192] = val;
		val = new Command();
		val.Name = "selectnpclookatserver";
		val.Parent = "ai";
		val.FullName = "ai.selectnpclookatserver";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.selectNPCLookatServer(arg);
		};
		array[193] = val;
		val = new Command();
		val.Name = "sensetime";
		val.Parent = "ai";
		val.FullName = "ai.sensetime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.sensetime.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.sensetime = StringExtensions.ToFloat(str, 0f);
		};
		array[194] = val;
		val = new Command();
		val.Name = "setdestinationsamplenavmesh";
		val.Parent = "ai";
		val.FullName = "ai.setdestinationsamplenavmesh";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.setdestinationsamplenavmesh.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.setdestinationsamplenavmesh = StringExtensions.ToBool(str);
		};
		array[195] = val;
		val = new Command();
		val.Name = "sleepwake";
		val.Parent = "ai";
		val.FullName = "ai.sleepwake";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.sleepwake.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.sleepwake = StringExtensions.ToBool(str);
		};
		array[196] = val;
		val = new Command();
		val.Name = "sleepwakestats";
		val.Parent = "ai";
		val.FullName = "ai.sleepwakestats";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.sleepwakestats(arg);
		};
		array[197] = val;
		val = new Command();
		val.Name = "spliceupdates";
		val.Parent = "ai";
		val.FullName = "ai.spliceupdates";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.spliceupdates.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.spliceupdates = StringExtensions.ToBool(str);
		};
		array[198] = val;
		val = new Command();
		val.Name = "think";
		val.Parent = "ai";
		val.FullName = "ai.think";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.think.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.think = StringExtensions.ToBool(str);
		};
		array[199] = val;
		val = new Command();
		val.Name = "tickrate";
		val.Parent = "ai";
		val.FullName = "ai.tickrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.tickrate.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.tickrate = StringExtensions.ToFloat(str, 0f);
		};
		array[200] = val;
		val = new Command();
		val.Name = "usecalculatepath";
		val.Parent = "ai";
		val.FullName = "ai.usecalculatepath";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.usecalculatepath.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.usecalculatepath = StringExtensions.ToBool(str);
		};
		array[201] = val;
		val = new Command();
		val.Name = "usegrid";
		val.Parent = "ai";
		val.FullName = "ai.usegrid";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.usegrid.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.usegrid = StringExtensions.ToBool(str);
		};
		array[202] = val;
		val = new Command();
		val.Name = "usesetdestinationfallback";
		val.Parent = "ai";
		val.FullName = "ai.usesetdestinationfallback";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.usesetdestinationfallback.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.usesetdestinationfallback = StringExtensions.ToBool(str);
		};
		array[203] = val;
		val = new Command();
		val.Name = "wakesleepingai";
		val.Parent = "ai";
		val.FullName = "ai.wakesleepingai";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.wakesleepingai(arg);
		};
		array[204] = val;
		val = new Command();
		val.Name = "admincheat";
		val.Parent = "antihack";
		val.FullName = "antihack.admincheat";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.admincheat.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.admincheat = StringExtensions.ToBool(str);
		};
		array[205] = val;
		val = new Command();
		val.Name = "build_inside_check";
		val.Parent = "antihack";
		val.FullName = "antihack.build_inside_check";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.build_inside_check.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.build_inside_check = StringExtensions.ToInt(str, 0);
		};
		array[206] = val;
		val = new Command();
		val.Name = "build_losradius";
		val.Parent = "antihack";
		val.FullName = "antihack.build_losradius";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.build_losradius.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.build_losradius = StringExtensions.ToFloat(str, 0f);
		};
		array[207] = val;
		val = new Command();
		val.Name = "build_losradius_sleepingbag";
		val.Parent = "antihack";
		val.FullName = "antihack.build_losradius_sleepingbag";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.build_losradius_sleepingbag.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.build_losradius_sleepingbag = StringExtensions.ToFloat(str, 0f);
		};
		array[208] = val;
		val = new Command();
		val.Name = "build_terraincheck";
		val.Parent = "antihack";
		val.FullName = "antihack.build_terraincheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.build_terraincheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.build_terraincheck = StringExtensions.ToBool(str);
		};
		array[209] = val;
		val = new Command();
		val.Name = "build_vehiclecheck";
		val.Parent = "antihack";
		val.FullName = "antihack.build_vehiclecheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.build_vehiclecheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.build_vehiclecheck = StringExtensions.ToBool(str);
		};
		array[210] = val;
		val = new Command();
		val.Name = "debuglevel";
		val.Parent = "antihack";
		val.FullName = "antihack.debuglevel";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.debuglevel.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.debuglevel = StringExtensions.ToInt(str, 0);
		};
		array[211] = val;
		val = new Command();
		val.Name = "enforcementlevel";
		val.Parent = "antihack";
		val.FullName = "antihack.enforcementlevel";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.enforcementlevel.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.enforcementlevel = StringExtensions.ToInt(str, 0);
		};
		array[212] = val;
		val = new Command();
		val.Name = "eye_clientframes";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_clientframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_clientframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_clientframes = StringExtensions.ToFloat(str, 0f);
		};
		array[213] = val;
		val = new Command();
		val.Name = "eye_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[214] = val;
		val = new Command();
		val.Name = "eye_history_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_history_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_history_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_history_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[215] = val;
		val = new Command();
		val.Name = "eye_history_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_history_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_history_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_history_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[216] = val;
		val = new Command();
		val.Name = "eye_losradius";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_losradius";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_losradius.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_losradius = StringExtensions.ToFloat(str, 0f);
		};
		array[217] = val;
		val = new Command();
		val.Name = "eye_noclip_backtracking";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_noclip_backtracking";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_noclip_backtracking.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_noclip_backtracking = StringExtensions.ToFloat(str, 0f);
		};
		array[218] = val;
		val = new Command();
		val.Name = "eye_noclip_cutoff";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_noclip_cutoff";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_noclip_cutoff.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_noclip_cutoff = StringExtensions.ToFloat(str, 0f);
		};
		array[219] = val;
		val = new Command();
		val.Name = "eye_noclip_margin";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_noclip_margin";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_noclip_margin.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_noclip_margin = StringExtensions.ToFloat(str, 0f);
		};
		array[220] = val;
		val = new Command();
		val.Name = "eye_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[221] = val;
		val = new Command();
		val.Name = "eye_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_protection = StringExtensions.ToInt(str, 0);
		};
		array[222] = val;
		val = new Command();
		val.Name = "eye_serverframes";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_serverframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_serverframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_serverframes = StringExtensions.ToFloat(str, 0f);
		};
		array[223] = val;
		val = new Command();
		val.Name = "eye_terraincheck";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_terraincheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_terraincheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_terraincheck = StringExtensions.ToBool(str);
		};
		array[224] = val;
		val = new Command();
		val.Name = "eye_vehiclecheck";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_vehiclecheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_vehiclecheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_vehiclecheck = StringExtensions.ToBool(str);
		};
		array[225] = val;
		val = new Command();
		val.Name = "flyhack_extrusion";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_extrusion";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_extrusion.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_extrusion = StringExtensions.ToFloat(str, 0f);
		};
		array[226] = val;
		val = new Command();
		val.Name = "flyhack_forgiveness_horizontal";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_forgiveness_horizontal";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_forgiveness_horizontal.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_forgiveness_horizontal = StringExtensions.ToFloat(str, 0f);
		};
		array[227] = val;
		val = new Command();
		val.Name = "flyhack_forgiveness_horizontal_inertia";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_forgiveness_horizontal_inertia";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_forgiveness_horizontal_inertia.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_forgiveness_horizontal_inertia = StringExtensions.ToFloat(str, 0f);
		};
		array[228] = val;
		val = new Command();
		val.Name = "flyhack_forgiveness_vertical";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_forgiveness_vertical";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_forgiveness_vertical.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_forgiveness_vertical = StringExtensions.ToFloat(str, 0f);
		};
		array[229] = val;
		val = new Command();
		val.Name = "flyhack_forgiveness_vertical_inertia";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_forgiveness_vertical_inertia";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_forgiveness_vertical_inertia.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_forgiveness_vertical_inertia = StringExtensions.ToFloat(str, 0f);
		};
		array[230] = val;
		val = new Command();
		val.Name = "flyhack_margin";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_margin";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_margin.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_margin = StringExtensions.ToFloat(str, 0f);
		};
		array[231] = val;
		val = new Command();
		val.Name = "flyhack_maxsteps";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_maxsteps";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_maxsteps.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_maxsteps = StringExtensions.ToInt(str, 0);
		};
		array[232] = val;
		val = new Command();
		val.Name = "flyhack_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[233] = val;
		val = new Command();
		val.Name = "flyhack_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_protection = StringExtensions.ToInt(str, 0);
		};
		array[234] = val;
		val = new Command();
		val.Name = "flyhack_reject";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_reject";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_reject.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_reject = StringExtensions.ToBool(str);
		};
		array[235] = val;
		val = new Command();
		val.Name = "flyhack_stepsize";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_stepsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_stepsize.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_stepsize = StringExtensions.ToFloat(str, 0f);
		};
		array[236] = val;
		val = new Command();
		val.Name = "forceposition";
		val.Parent = "antihack";
		val.FullName = "antihack.forceposition";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.forceposition.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.forceposition = StringExtensions.ToBool(str);
		};
		array[237] = val;
		val = new Command();
		val.Name = "maxdeltatime";
		val.Parent = "antihack";
		val.FullName = "antihack.maxdeltatime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.maxdeltatime.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.maxdeltatime = StringExtensions.ToFloat(str, 0f);
		};
		array[238] = val;
		val = new Command();
		val.Name = "maxdesync";
		val.Parent = "antihack";
		val.FullName = "antihack.maxdesync";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.maxdesync.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.maxdesync = StringExtensions.ToFloat(str, 0f);
		};
		array[239] = val;
		val = new Command();
		val.Name = "maxviolation";
		val.Parent = "antihack";
		val.FullName = "antihack.maxviolation";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.maxviolation.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.maxviolation = StringExtensions.ToFloat(str, 0f);
		};
		array[240] = val;
		val = new Command();
		val.Name = "melee_backtracking";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_backtracking";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_backtracking.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_backtracking = StringExtensions.ToFloat(str, 0f);
		};
		array[241] = val;
		val = new Command();
		val.Name = "melee_clientframes";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_clientframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_clientframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_clientframes = StringExtensions.ToFloat(str, 0f);
		};
		array[242] = val;
		val = new Command();
		val.Name = "melee_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[243] = val;
		val = new Command();
		val.Name = "melee_losforgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_losforgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_losforgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_losforgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[244] = val;
		val = new Command();
		val.Name = "melee_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[245] = val;
		val = new Command();
		val.Name = "melee_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_protection = StringExtensions.ToInt(str, 0);
		};
		array[246] = val;
		val = new Command();
		val.Name = "melee_serverframes";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_serverframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_serverframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_serverframes = StringExtensions.ToFloat(str, 0f);
		};
		array[247] = val;
		val = new Command();
		val.Name = "melee_terraincheck";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_terraincheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_terraincheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_terraincheck = StringExtensions.ToBool(str);
		};
		array[248] = val;
		val = new Command();
		val.Name = "melee_vehiclecheck";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_vehiclecheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_vehiclecheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_vehiclecheck = StringExtensions.ToBool(str);
		};
		array[249] = val;
		val = new Command();
		val.Name = "modelstate";
		val.Parent = "antihack";
		val.FullName = "antihack.modelstate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.modelstate.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.modelstate = StringExtensions.ToBool(str);
		};
		array[250] = val;
		val = new Command();
		val.Name = "noclip_backtracking";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_backtracking";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_backtracking.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_backtracking = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0.01";
		array[251] = val;
		val = new Command();
		val.Name = "noclip_margin";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_margin";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_margin.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_margin = StringExtensions.ToFloat(str, 0f);
		};
		array[252] = val;
		val = new Command();
		val.Name = "noclip_margin_dismount";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_margin_dismount";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_margin_dismount.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_margin_dismount = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0.22";
		array[253] = val;
		val = new Command();
		val.Name = "noclip_maxsteps";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_maxsteps";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_maxsteps.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_maxsteps = StringExtensions.ToInt(str, 0);
		};
		array[254] = val;
		val = new Command();
		val.Name = "noclip_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[255] = val;
		val = new Command();
		val.Name = "noclip_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_protection = StringExtensions.ToInt(str, 0);
		};
		array[256] = val;
		val = new Command();
		val.Name = "noclip_reject";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_reject";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_reject.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_reject = StringExtensions.ToBool(str);
		};
		array[257] = val;
		val = new Command();
		val.Name = "noclip_stepsize";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_stepsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_stepsize.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_stepsize = StringExtensions.ToFloat(str, 0f);
		};
		array[258] = val;
		val = new Command();
		val.Name = "objectplacement";
		val.Parent = "antihack";
		val.FullName = "antihack.objectplacement";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.objectplacement.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.objectplacement = StringExtensions.ToBool(str);
		};
		array[259] = val;
		val = new Command();
		val.Name = "projectile_anglechange";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_anglechange";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_anglechange.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_anglechange = StringExtensions.ToFloat(str, 0f);
		};
		array[260] = val;
		val = new Command();
		val.Name = "projectile_backtracking";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_backtracking";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_backtracking.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_backtracking = StringExtensions.ToFloat(str, 0f);
		};
		array[261] = val;
		val = new Command();
		val.Name = "projectile_clientframes";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_clientframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_clientframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_clientframes = StringExtensions.ToFloat(str, 0f);
		};
		array[262] = val;
		val = new Command();
		val.Name = "projectile_damagedepth";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_damagedepth";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_damagedepth.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_damagedepth = StringExtensions.ToInt(str, 0);
		};
		array[263] = val;
		val = new Command();
		val.Name = "projectile_desync";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_desync";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_desync.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_desync = StringExtensions.ToFloat(str, 0f);
		};
		array[264] = val;
		val = new Command();
		val.Name = "projectile_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[265] = val;
		val = new Command();
		val.Name = "projectile_impactspawndepth";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_impactspawndepth";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_impactspawndepth.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_impactspawndepth = StringExtensions.ToInt(str, 0);
		};
		array[266] = val;
		val = new Command();
		val.Name = "projectile_losforgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_losforgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_losforgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_losforgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[267] = val;
		val = new Command();
		val.Name = "projectile_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[268] = val;
		val = new Command();
		val.Name = "projectile_positionoffset";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_positionoffset";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_positionoffset.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_positionoffset = StringExtensions.ToBool(str);
		};
		array[269] = val;
		val = new Command();
		val.Name = "projectile_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_protection = StringExtensions.ToInt(str, 0);
		};
		array[270] = val;
		val = new Command();
		val.Name = "projectile_serverframes";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_serverframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_serverframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_serverframes = StringExtensions.ToFloat(str, 0f);
		};
		array[271] = val;
		val = new Command();
		val.Name = "projectile_terraincheck";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_terraincheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_terraincheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_terraincheck = StringExtensions.ToBool(str);
		};
		array[272] = val;
		val = new Command();
		val.Name = "projectile_trajectory";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_trajectory";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_trajectory.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_trajectory = StringExtensions.ToFloat(str, 0f);
		};
		array[273] = val;
		val = new Command();
		val.Name = "projectile_vehiclecheck";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_vehiclecheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_vehiclecheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_vehiclecheck = StringExtensions.ToBool(str);
		};
		array[274] = val;
		val = new Command();
		val.Name = "projectile_velocitychange";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_velocitychange";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_velocitychange.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_velocitychange = StringExtensions.ToFloat(str, 0f);
		};
		array[275] = val;
		val = new Command();
		val.Name = "relaxationpause";
		val.Parent = "antihack";
		val.FullName = "antihack.relaxationpause";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.relaxationpause.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.relaxationpause = StringExtensions.ToFloat(str, 0f);
		};
		array[276] = val;
		val = new Command();
		val.Name = "relaxationrate";
		val.Parent = "antihack";
		val.FullName = "antihack.relaxationrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.relaxationrate.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.relaxationrate = StringExtensions.ToFloat(str, 0f);
		};
		array[277] = val;
		val = new Command();
		val.Name = "reporting";
		val.Parent = "antihack";
		val.FullName = "antihack.reporting";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.reporting.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.reporting = StringExtensions.ToBool(str);
		};
		array[278] = val;
		val = new Command();
		val.Name = "speedhack_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[279] = val;
		val = new Command();
		val.Name = "speedhack_forgiveness_inertia";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_forgiveness_inertia";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_forgiveness_inertia.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_forgiveness_inertia = StringExtensions.ToFloat(str, 0f);
		};
		array[280] = val;
		val = new Command();
		val.Name = "speedhack_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[281] = val;
		val = new Command();
		val.Name = "speedhack_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_protection = StringExtensions.ToInt(str, 0);
		};
		array[282] = val;
		val = new Command();
		val.Name = "speedhack_reject";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_reject";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_reject.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_reject = StringExtensions.ToBool(str);
		};
		array[283] = val;
		val = new Command();
		val.Name = "speedhack_slopespeed";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_slopespeed";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_slopespeed.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_slopespeed = StringExtensions.ToFloat(str, 0f);
		};
		array[284] = val;
		val = new Command();
		val.Name = "terrain_check_geometry";
		val.Parent = "antihack";
		val.FullName = "antihack.terrain_check_geometry";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.terrain_check_geometry.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.terrain_check_geometry = StringExtensions.ToBool(str);
		};
		array[285] = val;
		val = new Command();
		val.Name = "terrain_kill";
		val.Parent = "antihack";
		val.FullName = "antihack.terrain_kill";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.terrain_kill.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.terrain_kill = StringExtensions.ToBool(str);
		};
		array[286] = val;
		val = new Command();
		val.Name = "terrain_padding";
		val.Parent = "antihack";
		val.FullName = "antihack.terrain_padding";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.terrain_padding.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.terrain_padding = StringExtensions.ToFloat(str, 0f);
		};
		array[287] = val;
		val = new Command();
		val.Name = "terrain_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.terrain_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.terrain_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.terrain_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[288] = val;
		val = new Command();
		val.Name = "terrain_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.terrain_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.terrain_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.terrain_protection = StringExtensions.ToInt(str, 0);
		};
		array[289] = val;
		val = new Command();
		val.Name = "terrain_timeslice";
		val.Parent = "antihack";
		val.FullName = "antihack.terrain_timeslice";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.terrain_timeslice.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.terrain_timeslice = StringExtensions.ToInt(str, 0);
		};
		array[290] = val;
		val = new Command();
		val.Name = "tickhistoryforgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.tickhistoryforgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.tickhistoryforgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.tickhistoryforgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[291] = val;
		val = new Command();
		val.Name = "tickhistorytime";
		val.Parent = "antihack";
		val.FullName = "antihack.tickhistorytime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.tickhistorytime.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.tickhistorytime = StringExtensions.ToFloat(str, 0f);
		};
		array[292] = val;
		val = new Command();
		val.Name = "userlevel";
		val.Parent = "antihack";
		val.FullName = "antihack.userlevel";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.userlevel.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.userlevel = StringExtensions.ToInt(str, 0);
		};
		array[293] = val;
		val = new Command();
		val.Name = "alarmcooldown";
		val.Parent = "app";
		val.FullName = "app.alarmcooldown";
		val.ServerAdmin = true;
		val.Description = "Cooldown time before alarms can send another notification (in seconds)";
		val.Variable = true;
		val.GetOveride = () => App.alarmcooldown.ToString();
		val.SetOveride = delegate(string str)
		{
			App.alarmcooldown = StringExtensions.ToFloat(str, 0f);
		};
		array[294] = val;
		val = new Command();
		val.Name = "appban";
		val.Parent = "app";
		val.FullName = "app.appban";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.appban(arg);
		};
		array[295] = val;
		val = new Command();
		val.Name = "appunban";
		val.Parent = "app";
		val.FullName = "app.appunban";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.appunban(arg);
		};
		array[296] = val;
		val = new Command();
		val.Name = "connections";
		val.Parent = "app";
		val.FullName = "app.connections";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.connections(arg);
		};
		array[297] = val;
		val = new Command();
		val.Name = "info";
		val.Parent = "app";
		val.FullName = "app.info";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.info(arg);
		};
		array[298] = val;
		val = new Command();
		val.Name = "listenip";
		val.Parent = "app";
		val.FullName = "app.listenip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.listenip.ToString();
		val.SetOveride = delegate(string str)
		{
			App.listenip = str;
		};
		array[299] = val;
		val = new Command();
		val.Name = "maxconnections";
		val.Parent = "app";
		val.FullName = "app.maxconnections";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.maxconnections.ToString();
		val.SetOveride = delegate(string str)
		{
			App.maxconnections = StringExtensions.ToInt(str, 0);
		};
		array[300] = val;
		val = new Command();
		val.Name = "maxconnectionsperip";
		val.Parent = "app";
		val.FullName = "app.maxconnectionsperip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.maxconnectionsperip.ToString();
		val.SetOveride = delegate(string str)
		{
			App.maxconnectionsperip = StringExtensions.ToInt(str, 0);
		};
		array[301] = val;
		val = new Command();
		val.Name = "maxmessagesize";
		val.Parent = "app";
		val.FullName = "app.maxmessagesize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.maxmessagesize.ToString();
		val.SetOveride = delegate(string str)
		{
			App.maxmessagesize = StringExtensions.ToInt(str, 0);
		};
		array[302] = val;
		val = new Command();
		val.Name = "notifications";
		val.Parent = "app";
		val.FullName = "app.notifications";
		val.ServerAdmin = true;
		val.Description = "Enables sending push notifications";
		val.Variable = true;
		val.GetOveride = () => App.notifications.ToString();
		val.SetOveride = delegate(string str)
		{
			App.notifications = StringExtensions.ToBool(str);
		};
		array[303] = val;
		val = new Command();
		val.Name = "pair";
		val.Parent = "app";
		val.FullName = "app.pair";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.pair(arg);
		};
		array[304] = val;
		val = new Command();
		val.Name = "port";
		val.Parent = "app";
		val.FullName = "app.port";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.port.ToString();
		val.SetOveride = delegate(string str)
		{
			App.port = StringExtensions.ToInt(str, 0);
		};
		array[305] = val;
		val = new Command();
		val.Name = "publicip";
		val.Parent = "app";
		val.FullName = "app.publicip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.publicip.ToString();
		val.SetOveride = delegate(string str)
		{
			App.publicip = str;
		};
		array[306] = val;
		val = new Command();
		val.Name = "queuelimit";
		val.Parent = "app";
		val.FullName = "app.queuelimit";
		val.ServerAdmin = true;
		val.Description = "Max number of queued messages - set to 0 to disable message processing";
		val.Variable = true;
		val.GetOveride = () => App.queuelimit.ToString();
		val.SetOveride = delegate(string str)
		{
			App.queuelimit = StringExtensions.ToInt(str, 0);
		};
		array[307] = val;
		val = new Command();
		val.Name = "regeneratetoken";
		val.Parent = "app";
		val.FullName = "app.regeneratetoken";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.regeneratetoken(arg);
		};
		array[308] = val;
		val = new Command();
		val.Name = "resetlimiter";
		val.Parent = "app";
		val.FullName = "app.resetlimiter";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.resetlimiter(arg);
		};
		array[309] = val;
		val = new Command();
		val.Name = "retry_initialize";
		val.Parent = "app";
		val.FullName = "app.retry_initialize";
		val.ServerAdmin = true;
		val.Description = "Retry initializing the Rust+ companion server if it previously failed";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.retry_initialize(arg);
		};
		array[310] = val;
		val = new Command();
		val.Name = "serverid";
		val.Parent = "app";
		val.FullName = "app.serverid";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => App.serverid.ToString();
		val.SetOveride = delegate(string str)
		{
			App.serverid = str;
		};
		val.Default = "";
		array[311] = val;
		val = new Command();
		val.Name = "update";
		val.Parent = "app";
		val.FullName = "app.update";
		val.ServerAdmin = true;
		val.Description = "Disables updating entirely - emergency use only";
		val.Variable = true;
		val.GetOveride = () => App.update.ToString();
		val.SetOveride = delegate(string str)
		{
			App.update = StringExtensions.ToBool(str);
		};
		array[312] = val;
		val = new Command();
		val.Name = "verbose";
		val.Parent = "batching";
		val.FullName = "batching.verbose";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Batching.verbose.ToString();
		val.SetOveride = delegate(string str)
		{
			Batching.verbose = StringExtensions.ToInt(str, 0);
		};
		array[313] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "bradley";
		val.FullName = "bradley.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Bradley.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Bradley.enabled = StringExtensions.ToBool(str);
		};
		array[314] = val;
		val = new Command();
		val.Name = "quickrespawn";
		val.Parent = "bradley";
		val.FullName = "bradley.quickrespawn";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Bradley.quickrespawn(arg);
		};
		array[315] = val;
		val = new Command();
		val.Name = "respawndelayminutes";
		val.Parent = "bradley";
		val.FullName = "bradley.respawndelayminutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Bradley.respawnDelayMinutes.ToString();
		val.SetOveride = delegate(string str)
		{
			Bradley.respawnDelayMinutes = StringExtensions.ToFloat(str, 0f);
		};
		array[316] = val;
		val = new Command();
		val.Name = "respawndelayvariance";
		val.Parent = "bradley";
		val.FullName = "bradley.respawndelayvariance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Bradley.respawnDelayVariance.ToString();
		val.SetOveride = delegate(string str)
		{
			Bradley.respawnDelayVariance = StringExtensions.ToFloat(str, 0f);
		};
		array[317] = val;
		val = new Command();
		val.Name = "cardgamesay";
		val.Parent = "chat";
		val.FullName = "chat.cardgamesay";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Chat.cardgamesay(arg);
		};
		array[318] = val;
		val = new Command();
		val.Name = "clansay";
		val.Parent = "chat";
		val.FullName = "chat.clansay";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Chat.clansay(arg);
		};
		array[319] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "chat";
		val.FullName = "chat.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Chat.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Chat.enabled = StringExtensions.ToBool(str);
		};
		array[320] = val;
		val = new Command();
		val.Name = "globalchat";
		val.Parent = "chat";
		val.FullName = "chat.globalchat";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Chat.globalchat.ToString();
		val.SetOveride = delegate(string str)
		{
			Chat.globalchat = StringExtensions.ToBool(str);
		};
		val.Default = "True";
		array[321] = val;
		val = new Command();
		val.Name = "historysize";
		val.Parent = "chat";
		val.FullName = "chat.historysize";
		val.ServerAdmin = true;
		val.Description = "Number of messages to keep in memory for chat history";
		val.Variable = true;
		val.GetOveride = () => Chat.historysize.ToString();
		val.SetOveride = delegate(string str)
		{
			Chat.historysize = StringExtensions.ToInt(str, 0);
		};
		array[322] = val;
		val = new Command();
		val.Name = "localchat";
		val.Parent = "chat";
		val.FullName = "chat.localchat";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Chat.localchat.ToString();
		val.SetOveride = delegate(string str)
		{
			Chat.localchat = StringExtensions.ToBool(str);
		};
		val.Default = "False";
		array[323] = val;
		val = new Command();
		val.Name = "localchatrange";
		val.Parent = "chat";
		val.FullName = "chat.localchatrange";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Chat.localChatRange.ToString();
		val.SetOveride = delegate(string str)
		{
			Chat.localChatRange = StringExtensions.ToFloat(str, 0f);
		};
		array[324] = val;
		val = new Command();
		val.Name = "localsay";
		val.Parent = "chat";
		val.FullName = "chat.localsay";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Chat.localsay(arg);
		};
		array[325] = val;
		val = new Command();
		val.Name = "say";
		val.Parent = "chat";
		val.FullName = "chat.say";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Chat.say(arg);
		};
		array[326] = val;
		val = new Command();
		val.Name = "search";
		val.Parent = "chat";
		val.FullName = "chat.search";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			IEnumerable<Chat.ChatEntry> enumerable4 = Chat.search(arg);
			arg.ReplyWithObject((object)enumerable4);
		};
		array[327] = val;
		val = new Command();
		val.Name = "serverlog";
		val.Parent = "chat";
		val.FullName = "chat.serverlog";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Chat.serverlog.ToString();
		val.SetOveride = delegate(string str)
		{
			Chat.serverlog = StringExtensions.ToBool(str);
		};
		array[328] = val;
		val = new Command();
		val.Name = "tail";
		val.Parent = "chat";
		val.FullName = "chat.tail";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			IEnumerable<Chat.ChatEntry> enumerable3 = Chat.tail(arg);
			arg.ReplyWithObject((object)enumerable3);
		};
		array[329] = val;
		val = new Command();
		val.Name = "teamsay";
		val.Parent = "chat";
		val.FullName = "chat.teamsay";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Chat.teamsay(arg);
		};
		array[330] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "clan";
		val.FullName = "clan.enabled";
		val.ServerAdmin = true;
		val.Description = "Enables the clan system if set to true (must be set at boot, requires restart)";
		val.Variable = true;
		val.GetOveride = () => Clan.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.enabled = StringExtensions.ToBool(str);
		};
		array[331] = val;
		val = new Command();
		val.Name = "info";
		val.Parent = "clan";
		val.FullName = "clan.info";
		val.ServerAdmin = true;
		val.Description = "Prints info about a clan given its ID";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Clan.Info(arg);
		};
		array[332] = val;
		val = new Command();
		val.Name = "maxmembercount";
		val.Parent = "clan";
		val.FullName = "clan.maxmembercount";
		val.ServerAdmin = true;
		val.Description = "Maximum number of members each clan can have (local backend only!)";
		val.Variable = true;
		val.GetOveride = () => Clan.maxMemberCount.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.maxMemberCount = StringExtensions.ToInt(str, 0);
		};
		array[333] = val;
		val = new Command();
		val.Name = "search";
		val.Parent = "console";
		val.FullName = "console.search";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			IEnumerable<Output.Entry> enumerable2 = Console.search(arg);
			arg.ReplyWithObject((object)enumerable2);
		};
		array[334] = val;
		val = new Command();
		val.Name = "tail";
		val.Parent = "console";
		val.FullName = "console.tail";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			IEnumerable<Output.Entry> enumerable = Console.tail(arg);
			arg.ReplyWithObject((object)enumerable);
		};
		array[335] = val;
		val = new Command();
		val.Name = "frameminutes";
		val.Parent = "construct";
		val.FullName = "construct.frameminutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Construct.frameminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			Construct.frameminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[336] = val;
		val = new Command();
		val.Name = "add";
		val.Parent = "craft";
		val.FullName = "craft.add";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Craft.add(arg);
		};
		array[337] = val;
		val = new Command();
		val.Name = "cancel";
		val.Parent = "craft";
		val.FullName = "craft.cancel";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Craft.cancel(arg);
		};
		array[338] = val;
		val = new Command();
		val.Name = "canceltask";
		val.Parent = "craft";
		val.FullName = "craft.canceltask";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Craft.canceltask(arg);
		};
		array[339] = val;
		val = new Command();
		val.Name = "fasttracktask";
		val.Parent = "craft";
		val.FullName = "craft.fasttracktask";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Craft.fasttracktask(arg);
		};
		array[340] = val;
		val = new Command();
		val.Name = "instant";
		val.Parent = "craft";
		val.FullName = "craft.instant";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Craft.instant.ToString();
		val.SetOveride = delegate(string str)
		{
			Craft.instant = StringExtensions.ToBool(str);
		};
		array[341] = val;
		val = new Command();
		val.Name = "export";
		val.Parent = "data";
		val.FullName = "data.export";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Data.export(arg);
		};
		array[342] = val;
		val = new Command();
		val.Name = "bench_io";
		val.Parent = "debug";
		val.FullName = "debug.bench_io";
		val.ServerAdmin = true;
		val.Description = "Spawn lots of IO entities to lag the server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.bench_io(arg);
		};
		array[343] = val;
		val = new Command();
		val.Name = "breakheld";
		val.Parent = "debug";
		val.FullName = "debug.breakheld";
		val.ServerAdmin = true;
		val.Description = "Break the current held object";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.breakheld(arg);
		};
		array[344] = val;
		val = new Command();
		val.Name = "breakitem";
		val.Parent = "debug";
		val.FullName = "debug.breakitem";
		val.ServerAdmin = true;
		val.Description = "Break all the items in your inventory whose name match the passed string";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.breakitem(arg);
		};
		array[345] = val;
		val = new Command();
		val.Name = "callbacks";
		val.Parent = "debug";
		val.FullName = "debug.callbacks";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Debugging.callbacks.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.callbacks = StringExtensions.ToBool(str);
		};
		array[346] = val;
		val = new Command();
		val.Name = "checkparentingtriggers";
		val.Parent = "debug";
		val.FullName = "debug.checkparentingtriggers";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Debugging.checkparentingtriggers.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.checkparentingtriggers = StringExtensions.ToBool(str);
		};
		array[347] = val;
		val = new Command();
		val.Name = "checktriggers";
		val.Parent = "debug";
		val.FullName = "debug.checktriggers";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Debugging.checktriggers.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.checktriggers = StringExtensions.ToBool(str);
		};
		array[348] = val;
		val = new Command();
		val.Name = "debugdismounts";
		val.Parent = "debug";
		val.FullName = "debug.debugdismounts";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Debugging.DebugDismounts.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.DebugDismounts = StringExtensions.ToBool(str);
		};
		array[349] = val;
		val = new Command();
		val.Name = "deleteentitiesbyshortname";
		val.Parent = "debug";
		val.FullName = "debug.deleteentitiesbyshortname";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.deleteEntitiesByShortname(arg);
		};
		array[350] = val;
		val = new Command();
		val.Name = "disablecondition";
		val.Parent = "debug";
		val.FullName = "debug.disablecondition";
		val.ServerAdmin = true;
		val.Description = "Do not damage any items";
		val.Variable = true;
		val.GetOveride = () => Debugging.disablecondition.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.disablecondition = StringExtensions.ToBool(str);
		};
		array[351] = val;
		val = new Command();
		val.Name = "drink";
		val.Parent = "debug";
		val.FullName = "debug.drink";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.drink(arg);
		};
		array[352] = val;
		val = new Command();
		val.Name = "eat";
		val.Parent = "debug";
		val.FullName = "debug.eat";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.eat(arg);
		};
		array[353] = val;
		val = new Command();
		val.Name = "enable_player_movement";
		val.Parent = "debug";
		val.FullName = "debug.enable_player_movement";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.enable_player_movement(arg);
		};
		array[354] = val;
		val = new Command();
		val.Name = "flushgroup";
		val.Parent = "debug";
		val.FullName = "debug.flushgroup";
		val.ServerAdmin = true;
		val.Description = "Takes you in and out of your current network group, causing you to delete and then download all entities in your PVS again";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.flushgroup(arg);
		};
		array[355] = val;
		val = new Command();
		val.Name = "heal";
		val.Parent = "debug";
		val.FullName = "debug.heal";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.heal(arg);
		};
		array[356] = val;
		val = new Command();
		val.Name = "hurt";
		val.Parent = "debug";
		val.FullName = "debug.hurt";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.hurt(arg);
		};
		array[357] = val;
		val = new Command();
		val.Name = "log";
		val.Parent = "debug";
		val.FullName = "debug.log";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Debugging.log.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.log = StringExtensions.ToBool(str);
		};
		array[358] = val;
		val = new Command();
		val.Name = "puzzlereset";
		val.Parent = "debug";
		val.FullName = "debug.puzzlereset";
		val.ServerAdmin = true;
		val.Description = "reset all puzzles";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.puzzlereset(arg);
		};
		array[359] = val;
		val = new Command();
		val.Name = "refillvitals";
		val.Parent = "debug";
		val.FullName = "debug.refillvitals";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.refillvitals(arg);
		};
		array[360] = val;
		val = new Command();
		val.Name = "renderinfo";
		val.Parent = "debug";
		val.FullName = "debug.renderinfo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.renderinfo(arg);
		};
		array[361] = val;
		val = new Command();
		val.Name = "repair_inventory";
		val.Parent = "debug";
		val.FullName = "debug.repair_inventory";
		val.ServerAdmin = true;
		val.Description = "Repair all items in inventory";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.repair_inventory(arg);
		};
		array[362] = val;
		val = new Command();
		val.Name = "resetsleepingbagtimers";
		val.Parent = "debug";
		val.FullName = "debug.resetsleepingbagtimers";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.ResetSleepingBagTimers(arg);
		};
		array[363] = val;
		val = new Command();
		val.Name = "setdamage";
		val.Parent = "debug";
		val.FullName = "debug.setdamage";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.setdamage(arg);
		};
		array[364] = val;
		val = new Command();
		val.Name = "setfood";
		val.Parent = "debug";
		val.FullName = "debug.setfood";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.setfood(arg);
		};
		array[365] = val;
		val = new Command();
		val.Name = "sethealth";
		val.Parent = "debug";
		val.FullName = "debug.sethealth";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.sethealth(arg);
		};
		array[366] = val;
		val = new Command();
		val.Name = "setradiation";
		val.Parent = "debug";
		val.FullName = "debug.setradiation";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.setradiation(arg);
		};
		array[367] = val;
		val = new Command();
		val.Name = "setwater";
		val.Parent = "debug";
		val.FullName = "debug.setwater";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.setwater(arg);
		};
		array[368] = val;
		val = new Command();
		val.Name = "spawnparachutetester";
		val.Parent = "debug";
		val.FullName = "debug.spawnparachutetester";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.spawnParachuteTester(arg);
		};
		array[369] = val;
		val = new Command();
		val.Name = "stall";
		val.Parent = "debug";
		val.FullName = "debug.stall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.stall(arg);
		};
		array[370] = val;
		val = new Command();
		val.Name = "bracket_0_blockcount";
		val.Parent = "decay";
		val.FullName = "decay.bracket_0_blockcount";
		val.ServerAdmin = true;
		val.Description = "Between 0 and this value are considered bracket 0 and will cost bracket_0_costfraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_0_blockcount.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_0_blockcount = StringExtensions.ToInt(str, 0);
		};
		array[371] = val;
		val = new Command();
		val.Name = "bracket_0_costfraction";
		val.Parent = "decay";
		val.FullName = "decay.bracket_0_costfraction";
		val.ServerAdmin = true;
		val.Description = "blocks within bracket 0 will cost this fraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_0_costfraction.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_0_costfraction = StringExtensions.ToFloat(str, 0f);
		};
		array[372] = val;
		val = new Command();
		val.Name = "bracket_1_blockcount";
		val.Parent = "decay";
		val.FullName = "decay.bracket_1_blockcount";
		val.ServerAdmin = true;
		val.Description = "Between bracket_0_blockcount and this value are considered bracket 1 and will cost bracket_1_costfraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_1_blockcount.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_1_blockcount = StringExtensions.ToInt(str, 0);
		};
		array[373] = val;
		val = new Command();
		val.Name = "bracket_1_costfraction";
		val.Parent = "decay";
		val.FullName = "decay.bracket_1_costfraction";
		val.ServerAdmin = true;
		val.Description = "blocks within bracket 1 will cost this fraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_1_costfraction.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_1_costfraction = StringExtensions.ToFloat(str, 0f);
		};
		array[374] = val;
		val = new Command();
		val.Name = "bracket_2_blockcount";
		val.Parent = "decay";
		val.FullName = "decay.bracket_2_blockcount";
		val.ServerAdmin = true;
		val.Description = "Between bracket_1_blockcount and this value are considered bracket 2 and will cost bracket_2_costfraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_2_blockcount.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_2_blockcount = StringExtensions.ToInt(str, 0);
		};
		array[375] = val;
		val = new Command();
		val.Name = "bracket_2_costfraction";
		val.Parent = "decay";
		val.FullName = "decay.bracket_2_costfraction";
		val.ServerAdmin = true;
		val.Description = "blocks within bracket 2 will cost this fraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_2_costfraction.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_2_costfraction = StringExtensions.ToFloat(str, 0f);
		};
		array[376] = val;
		val = new Command();
		val.Name = "bracket_3_blockcount";
		val.Parent = "decay";
		val.FullName = "decay.bracket_3_blockcount";
		val.ServerAdmin = true;
		val.Description = "Between bracket_2_blockcount and this value (and beyond) are considered bracket 3 and will cost bracket_3_costfraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_3_blockcount.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_3_blockcount = StringExtensions.ToInt(str, 0);
		};
		array[377] = val;
		val = new Command();
		val.Name = "bracket_3_costfraction";
		val.Parent = "decay";
		val.FullName = "decay.bracket_3_costfraction";
		val.ServerAdmin = true;
		val.Description = "blocks within bracket 3 will cost this fraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_3_costfraction.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_3_costfraction = StringExtensions.ToFloat(str, 0f);
		};
		array[378] = val;
		val = new Command();
		val.Name = "debug";
		val.Parent = "decay";
		val.FullName = "decay.debug";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.debug.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.debug = StringExtensions.ToBool(str);
		};
		array[379] = val;
		val = new Command();
		val.Name = "delay_metal";
		val.Parent = "decay";
		val.FullName = "decay.delay_metal";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_metal.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_metal = StringExtensions.ToFloat(str, 0f);
		};
		array[380] = val;
		val = new Command();
		val.Name = "delay_override";
		val.Parent = "decay";
		val.FullName = "decay.delay_override";
		val.ServerAdmin = true;
		val.Description = "When set to a value above 0 everything will decay with this delay";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_override.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_override = StringExtensions.ToFloat(str, 0f);
		};
		array[381] = val;
		val = new Command();
		val.Name = "delay_stone";
		val.Parent = "decay";
		val.FullName = "decay.delay_stone";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_stone.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_stone = StringExtensions.ToFloat(str, 0f);
		};
		array[382] = val;
		val = new Command();
		val.Name = "delay_toptier";
		val.Parent = "decay";
		val.FullName = "decay.delay_toptier";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_toptier.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_toptier = StringExtensions.ToFloat(str, 0f);
		};
		array[383] = val;
		val = new Command();
		val.Name = "delay_twig";
		val.Parent = "decay";
		val.FullName = "decay.delay_twig";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_twig.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_twig = StringExtensions.ToFloat(str, 0f);
		};
		array[384] = val;
		val = new Command();
		val.Name = "delay_wood";
		val.Parent = "decay";
		val.FullName = "decay.delay_wood";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_wood.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_wood = StringExtensions.ToFloat(str, 0f);
		};
		array[385] = val;
		val = new Command();
		val.Name = "duration_metal";
		val.Parent = "decay";
		val.FullName = "decay.duration_metal";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade take to decay when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_metal.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_metal = StringExtensions.ToFloat(str, 0f);
		};
		array[386] = val;
		val = new Command();
		val.Name = "duration_override";
		val.Parent = "decay";
		val.FullName = "decay.duration_override";
		val.ServerAdmin = true;
		val.Description = "When set to a value above 0 everything will decay with this duration";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_override.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_override = StringExtensions.ToFloat(str, 0f);
		};
		array[387] = val;
		val = new Command();
		val.Name = "duration_stone";
		val.Parent = "decay";
		val.FullName = "decay.duration_stone";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade take to decay when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_stone.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_stone = StringExtensions.ToFloat(str, 0f);
		};
		array[388] = val;
		val = new Command();
		val.Name = "duration_toptier";
		val.Parent = "decay";
		val.FullName = "decay.duration_toptier";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade take to decay when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_toptier.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_toptier = StringExtensions.ToFloat(str, 0f);
		};
		array[389] = val;
		val = new Command();
		val.Name = "duration_twig";
		val.Parent = "decay";
		val.FullName = "decay.duration_twig";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade take to decay when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_twig.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_twig = StringExtensions.ToFloat(str, 0f);
		};
		array[390] = val;
		val = new Command();
		val.Name = "duration_wood";
		val.Parent = "decay";
		val.FullName = "decay.duration_wood";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade take to decay when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_wood.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_wood = StringExtensions.ToFloat(str, 0f);
		};
		array[391] = val;
		val = new Command();
		val.Name = "outside_test_range";
		val.Parent = "decay";
		val.FullName = "decay.outside_test_range";
		val.ServerAdmin = true;
		val.Description = "Maximum distance to test to see if a structure is outside, higher values are slower but accurate for huge buildings";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.outside_test_range.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.outside_test_range = StringExtensions.ToFloat(str, 0f);
		};
		array[392] = val;
		val = new Command();
		val.Name = "scale";
		val.Parent = "decay";
		val.FullName = "decay.scale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.scale.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.scale = StringExtensions.ToFloat(str, 0f);
		};
		array[393] = val;
		val = new Command();
		val.Name = "tick";
		val.Parent = "decay";
		val.FullName = "decay.tick";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.tick.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.tick = StringExtensions.ToFloat(str, 0f);
		};
		array[394] = val;
		val = new Command();
		val.Name = "upkeep";
		val.Parent = "decay";
		val.FullName = "decay.upkeep";
		val.ServerAdmin = true;
		val.Description = "Is upkeep enabled";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.upkeep.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.upkeep = StringExtensions.ToBool(str);
		};
		array[395] = val;
		val = new Command();
		val.Name = "upkeep_grief_protection";
		val.Parent = "decay";
		val.FullName = "decay.upkeep_grief_protection";
		val.ServerAdmin = true;
		val.Description = "How many minutes can the upkeep cost last after the cupboard was destroyed? default : 1440 (24 hours)";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.upkeep_grief_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.upkeep_grief_protection = StringExtensions.ToFloat(str, 0f);
		};
		array[396] = val;
		val = new Command();
		val.Name = "upkeep_heal_scale";
		val.Parent = "decay";
		val.FullName = "decay.upkeep_heal_scale";
		val.ServerAdmin = true;
		val.Description = "Scale at which objects heal when upkeep conditions are met, default of 1 is same rate at which they decay";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.upkeep_heal_scale.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.upkeep_heal_scale = StringExtensions.ToFloat(str, 0f);
		};
		array[397] = val;
		val = new Command();
		val.Name = "upkeep_inside_decay_scale";
		val.Parent = "decay";
		val.FullName = "decay.upkeep_inside_decay_scale";
		val.ServerAdmin = true;
		val.Description = "Scale at which objects decay when they are inside, default of 0.1";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.upkeep_inside_decay_scale.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.upkeep_inside_decay_scale = StringExtensions.ToFloat(str, 0f);
		};
		array[398] = val;
		val = new Command();
		val.Name = "upkeep_period_minutes";
		val.Parent = "decay";
		val.FullName = "decay.upkeep_period_minutes";
		val.ServerAdmin = true;
		val.Description = "How many minutes does the upkeep cost last? default : 1440 (24 hours)";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.upkeep_period_minutes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.upkeep_period_minutes = StringExtensions.ToFloat(str, 0f);
		};
		array[399] = val;
		val = new Command();
		val.Name = "record";
		val.Parent = "demo";
		val.FullName = "demo.record";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text18 = Demo.record(arg);
			arg.ReplyWithObject((object)text18);
		};
		array[400] = val;
		val = new Command();
		val.Name = "recordlist";
		val.Parent = "demo";
		val.FullName = "demo.recordlist";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Demo.recordlist.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.recordlist = str;
		};
		array[401] = val;
		val = new Command();
		val.Name = "recordlistmode";
		val.Parent = "demo";
		val.FullName = "demo.recordlistmode";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Controls the behavior of recordlist, 0=whitelist, 1=blacklist";
		val.Variable = true;
		val.GetOveride = () => Demo.recordlistmode.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.recordlistmode = StringExtensions.ToInt(str, 0);
		};
		array[402] = val;
		val = new Command();
		val.Name = "splitmegabytes";
		val.Parent = "demo";
		val.FullName = "demo.splitmegabytes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Demo.splitmegabytes.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.splitmegabytes = StringExtensions.ToFloat(str, 0f);
		};
		array[403] = val;
		val = new Command();
		val.Name = "splitseconds";
		val.Parent = "demo";
		val.FullName = "demo.splitseconds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Demo.splitseconds.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.splitseconds = StringExtensions.ToFloat(str, 0f);
		};
		array[404] = val;
		val = new Command();
		val.Name = "stop";
		val.Parent = "demo";
		val.FullName = "demo.stop";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text17 = Demo.stop(arg);
			arg.ReplyWithObject((object)text17);
		};
		array[405] = val;
		val = new Command();
		val.Name = "debug_toggle";
		val.Parent = "entity";
		val.FullName = "entity.debug_toggle";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.debug_toggle(arg);
		};
		array[406] = val;
		val = new Command();
		val.Name = "deleteby";
		val.Parent = "entity";
		val.FullName = "entity.deleteby";
		val.ServerAdmin = true;
		val.Description = "Destroy all entities created by provided users (separate users by space)";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			int num = Entity.DeleteBy(arg);
			arg.ReplyWithObject((object)num);
		};
		array[407] = val;
		val = new Command();
		val.Name = "deletebytextblock";
		val.Parent = "entity";
		val.FullName = "entity.deletebytextblock";
		val.ServerAdmin = true;
		val.Description = "Destroy all entities created by users in the provided text block (can use with copied results from ent auth)";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.DeleteByTextBlock(arg);
		};
		array[408] = val;
		val = new Command();
		val.Name = "find_entity";
		val.Parent = "entity";
		val.FullName = "entity.find_entity";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_entity(arg);
		};
		array[409] = val;
		val = new Command();
		val.Name = "find_group";
		val.Parent = "entity";
		val.FullName = "entity.find_group";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_group(arg);
		};
		array[410] = val;
		val = new Command();
		val.Name = "find_id";
		val.Parent = "entity";
		val.FullName = "entity.find_id";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_id(arg);
		};
		array[411] = val;
		val = new Command();
		val.Name = "find_parent";
		val.Parent = "entity";
		val.FullName = "entity.find_parent";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_parent(arg);
		};
		array[412] = val;
		val = new Command();
		val.Name = "find_radius";
		val.Parent = "entity";
		val.FullName = "entity.find_radius";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_radius(arg);
		};
		array[413] = val;
		val = new Command();
		val.Name = "find_self";
		val.Parent = "entity";
		val.FullName = "entity.find_self";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_self(arg);
		};
		array[414] = val;
		val = new Command();
		val.Name = "find_status";
		val.Parent = "entity";
		val.FullName = "entity.find_status";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_status(arg);
		};
		array[415] = val;
		val = new Command();
		val.Name = "nudge";
		val.Parent = "entity";
		val.FullName = "entity.nudge";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.nudge(arg);
		};
		array[416] = val;
		val = new Command();
		val.Name = "spawnlootfrom";
		val.Parent = "entity";
		val.FullName = "entity.spawnlootfrom";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.spawnlootfrom(arg);
		};
		array[417] = val;
		val = new Command();
		val.Name = "spawn";
		val.Parent = "entity";
		val.FullName = "entity.spawn";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			string text16 = Entity.svspawn(arg.GetString(0, ""), arg.GetVector3(1, Vector3.zero), arg.GetVector3(2, Vector3.zero));
			arg.ReplyWithObject((object)text16);
		};
		array[418] = val;
		val = new Command();
		val.Name = "spawngrid";
		val.Parent = "entity";
		val.FullName = "entity.spawngrid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text15 = Entity.svspawngrid(arg.GetString(0, ""), arg.GetInt(1, 5), arg.GetInt(2, 5), arg.GetInt(3, 5));
			arg.ReplyWithObject((object)text15);
		};
		array[419] = val;
		val = new Command();
		val.Name = "spawnitem";
		val.Parent = "entity";
		val.FullName = "entity.spawnitem";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			string text14 = Entity.svspawnitem(arg.GetString(0, ""), arg.GetVector3(1, Vector3.zero));
			arg.ReplyWithObject((object)text14);
		};
		array[420] = val;
		val = new Command();
		val.Name = "addtime";
		val.Parent = "env";
		val.FullName = "env.addtime";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Env.addtime(arg);
		};
		array[421] = val;
		val = new Command();
		val.Name = "day";
		val.Parent = "env";
		val.FullName = "env.day";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Env.day.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.day = StringExtensions.ToInt(str, 0);
		};
		array[422] = val;
		val = new Command();
		val.Name = "month";
		val.Parent = "env";
		val.FullName = "env.month";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Env.month.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.month = StringExtensions.ToInt(str, 0);
		};
		array[423] = val;
		val = new Command();
		val.Name = "oceanlevel";
		val.Parent = "env";
		val.FullName = "env.oceanlevel";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Env.oceanlevel.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.oceanlevel = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[424] = val;
		val = new Command();
		val.Name = "progresstime";
		val.Parent = "env";
		val.FullName = "env.progresstime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Env.progresstime.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.progresstime = StringExtensions.ToBool(str);
		};
		array[425] = val;
		val = new Command();
		val.Name = "time";
		val.Parent = "env";
		val.FullName = "env.time";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Env.time.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.time = StringExtensions.ToFloat(str, 0f);
		};
		array[426] = val;
		val = new Command();
		val.Name = "year";
		val.Parent = "env";
		val.FullName = "env.year";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Env.year.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.year = StringExtensions.ToInt(str, 0);
		};
		array[427] = val;
		val = new Command();
		val.Name = "limit";
		val.Parent = "fps";
		val.FullName = "fps.limit";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => FPS.limit.ToString();
		val.SetOveride = delegate(string str)
		{
			FPS.limit = StringExtensions.ToInt(str, 0);
		};
		array[428] = val;
		val = new Command();
		val.Name = "set";
		val.Parent = "gamemode";
		val.FullName = "gamemode.set";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			gamemode.set(arg);
		};
		array[429] = val;
		val = new Command();
		val.Name = "setteam";
		val.Parent = "gamemode";
		val.FullName = "gamemode.setteam";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			gamemode.setteam(arg);
		};
		array[430] = val;
		val = new Command();
		val.Name = "alloc";
		val.Parent = "gc";
		val.FullName = "gc.alloc";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			GC.alloc(arg);
		};
		array[431] = val;
		val = new Command();
		val.Name = "collect";
		val.Parent = "gc";
		val.FullName = "gc.collect";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate
		{
			GC.collect();
		};
		array[432] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "gc";
		val.FullName = "gc.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GC.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			GC.enabled = StringExtensions.ToBool(str);
		};
		array[433] = val;
		val = new Command();
		val.Name = "incremental_enabled";
		val.Parent = "gc";
		val.FullName = "gc.incremental_enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GC.incremental_enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			GC.incremental_enabled = StringExtensions.ToBool(str);
		};
		array[434] = val;
		val = new Command();
		val.Name = "incremental_milliseconds";
		val.Parent = "gc";
		val.FullName = "gc.incremental_milliseconds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GC.incremental_milliseconds.ToString();
		val.SetOveride = delegate(string str)
		{
			GC.incremental_milliseconds = StringExtensions.ToInt(str, 0);
		};
		array[435] = val;
		val = new Command();
		val.Name = "unload";
		val.Parent = "gc";
		val.FullName = "gc.unload";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate
		{
			GC.unload();
		};
		array[436] = val;
		val = new Command();
		val.Name = "asyncwarmup";
		val.Parent = "global";
		val.FullName = "global.asyncwarmup";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Global.asyncWarmup.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.asyncWarmup = StringExtensions.ToBool(str);
		};
		array[437] = val;
		val = new Command();
		val.Name = "breakclothing";
		val.Parent = "global";
		val.FullName = "global.breakclothing";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.breakclothing(arg);
		};
		array[438] = val;
		val = new Command();
		val.Name = "breakitem";
		val.Parent = "global";
		val.FullName = "global.breakitem";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.breakitem(arg);
		};
		array[439] = val;
		val = new Command();
		val.Name = "cinematicgingerbreadcorpses";
		val.Parent = "global";
		val.FullName = "global.cinematicgingerbreadcorpses";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Description = "When enabled a player wearing a gingerbread suit will gib like the gingerbread NPC's";
		val.Variable = true;
		val.GetOveride = () => Global.cinematicGingerbreadCorpses.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.cinematicGingerbreadCorpses = StringExtensions.ToBool(str);
		};
		val.Default = "False";
		array[440] = val;
		val = new Command();
		val.Name = "clearallsprays";
		val.Parent = "global";
		val.FullName = "global.clearallsprays";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate
		{
			Global.ClearAllSprays();
		};
		array[441] = val;
		val = new Command();
		val.Name = "clearallspraysbyplayer";
		val.Parent = "global";
		val.FullName = "global.clearallspraysbyplayer";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.ClearAllSpraysByPlayer(arg);
		};
		array[442] = val;
		val = new Command();
		val.Name = "cleardroppeditems";
		val.Parent = "global";
		val.FullName = "global.cleardroppeditems";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate
		{
			Global.ClearDroppedItems();
		};
		array[443] = val;
		val = new Command();
		val.Name = "clearspraysatpositioninradius";
		val.Parent = "global";
		val.FullName = "global.clearspraysatpositioninradius";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.ClearSpraysAtPositionInRadius(arg);
		};
		array[444] = val;
		val = new Command();
		val.Name = "clearspraysinradius";
		val.Parent = "global";
		val.FullName = "global.clearspraysinradius";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.ClearSpraysInRadius(arg);
		};
		array[445] = val;
		val = new Command();
		val.Name = "colliders";
		val.Parent = "global";
		val.FullName = "global.colliders";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.colliders(arg);
		};
		array[446] = val;
		val = new Command();
		val.Name = "developer";
		val.Parent = "global";
		val.FullName = "global.developer";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Global.developer.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.developer = StringExtensions.ToInt(str, 0);
		};
		array[447] = val;
		val = new Command();
		val.Name = "disablebagdropping";
		val.Parent = "global";
		val.FullName = "global.disablebagdropping";
		val.ServerAdmin = true;
		val.Description = "Disables the backpacks that appear after a corpse times out";
		val.Variable = true;
		val.GetOveride = () => Global.disableBagDropping.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.disableBagDropping = StringExtensions.ToBool(str);
		};
		array[448] = val;
		val = new Command();
		val.Name = "error";
		val.Parent = "global";
		val.FullName = "global.error";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.error(arg);
		};
		array[449] = val;
		val = new Command();
		val.Name = "forceunloadbundles";
		val.Parent = "global";
		val.FullName = "global.forceunloadbundles";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Global.forceUnloadBundles.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.forceUnloadBundles = StringExtensions.ToBool(str);
		};
		array[450] = val;
		val = new Command();
		val.Name = "free";
		val.Parent = "global";
		val.FullName = "global.free";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.free(arg);
		};
		array[451] = val;
		val = new Command();
		val.Name = "injure";
		val.Parent = "global";
		val.FullName = "global.injure";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.injure(arg);
		};
		array[452] = val;
		val = new Command();
		val.Name = "kill";
		val.Parent = "global";
		val.FullName = "global.kill";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.kill(arg);
		};
		array[453] = val;
		val = new Command();
		val.Name = "maxspraysperplayer";
		val.Parent = "global";
		val.FullName = "global.maxspraysperplayer";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "If a player sprays more than this, the oldest spray will be destroyed. 0 will disable";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Global.MaxSpraysPerPlayer.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.MaxSpraysPerPlayer = StringExtensions.ToInt(str, 0);
		};
		array[454] = val;
		val = new Command();
		val.Name = "maxthreads";
		val.Parent = "global";
		val.FullName = "global.maxthreads";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Global.maxthreads.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.maxthreads = StringExtensions.ToInt(str, 0);
		};
		array[455] = val;
		val = new Command();
		val.Name = "objects";
		val.Parent = "global";
		val.FullName = "global.objects";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.objects(arg);
		};
		array[456] = val;
		val = new Command();
		val.Name = "perf";
		val.Parent = "global";
		val.FullName = "global.perf";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Global.perf.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.perf = StringExtensions.ToInt(str, 0);
		};
		array[457] = val;
		val = new Command();
		val.Name = "preloadconcurrency";
		val.Parent = "global";
		val.FullName = "global.preloadconcurrency";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Global.preloadConcurrency.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.preloadConcurrency = StringExtensions.ToInt(str, 0);
		};
		array[458] = val;
		val = new Command();
		val.Name = "queue";
		val.Parent = "global";
		val.FullName = "global.queue";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.queue(arg);
		};
		array[459] = val;
		val = new Command();
		val.Name = "quit";
		val.Parent = "global";
		val.FullName = "global.quit";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.quit(arg);
		};
		array[460] = val;
		val = new Command();
		val.Name = "recover";
		val.Parent = "global";
		val.FullName = "global.recover";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.recover(arg);
		};
		array[461] = val;
		val = new Command();
		val.Name = "report";
		val.Parent = "global";
		val.FullName = "global.report";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.report(arg);
		};
		array[462] = val;
		val = new Command();
		val.Name = "respawn";
		val.Parent = "global";
		val.FullName = "global.respawn";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.respawn(arg);
		};
		array[463] = val;
		val = new Command();
		val.Name = "respawn_sleepingbag";
		val.Parent = "global";
		val.FullName = "global.respawn_sleepingbag";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.respawn_sleepingbag(arg);
		};
		array[464] = val;
		val = new Command();
		val.Name = "respawn_sleepingbag_remove";
		val.Parent = "global";
		val.FullName = "global.respawn_sleepingbag_remove";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.respawn_sleepingbag_remove(arg);
		};
		array[465] = val;
		val = new Command();
		val.Name = "restart";
		val.Parent = "global";
		val.FullName = "global.restart";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.restart(arg);
		};
		array[466] = val;
		val = new Command();
		val.Name = "setinfo";
		val.Parent = "global";
		val.FullName = "global.setinfo";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.setinfo(arg);
		};
		array[467] = val;
		val = new Command();
		val.Name = "skipassetwarmup_crashes";
		val.Parent = "global";
		val.FullName = "global.skipassetwarmup_crashes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Global.skipAssetWarmup_crashes.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.skipAssetWarmup_crashes = StringExtensions.ToBool(str);
		};
		array[468] = val;
		val = new Command();
		val.Name = "sleep";
		val.Parent = "global";
		val.FullName = "global.sleep";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.sleep(arg);
		};
		array[469] = val;
		val = new Command();
		val.Name = "spectate";
		val.Parent = "global";
		val.FullName = "global.spectate";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.spectate(arg);
		};
		array[470] = val;
		val = new Command();
		val.Name = "spectateid";
		val.Parent = "global";
		val.FullName = "global.spectateid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.spectateid(arg);
		};
		array[471] = val;
		val = new Command();
		val.Name = "sprayduration";
		val.Parent = "global";
		val.FullName = "global.sprayduration";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Base time (in seconds) that sprays last";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Global.SprayDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.SprayDuration = StringExtensions.ToFloat(str, 0f);
		};
		array[472] = val;
		val = new Command();
		val.Name = "sprayoutofauthmultiplier";
		val.Parent = "global";
		val.FullName = "global.sprayoutofauthmultiplier";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Multiplier applied to SprayDuration if a spray isn't in the sprayers auth (cannot go above 1f)";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Global.SprayOutOfAuthMultiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.SprayOutOfAuthMultiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[473] = val;
		val = new Command();
		val.Name = "status_sv";
		val.Parent = "global";
		val.FullName = "global.status_sv";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.status_sv(arg);
		};
		array[474] = val;
		val = new Command();
		val.Name = "subscriptions";
		val.Parent = "global";
		val.FullName = "global.subscriptions";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.subscriptions(arg);
		};
		array[475] = val;
		val = new Command();
		val.Name = "sysinfo";
		val.Parent = "global";
		val.FullName = "global.sysinfo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.sysinfo(arg);
		};
		array[476] = val;
		val = new Command();
		val.Name = "sysuid";
		val.Parent = "global";
		val.FullName = "global.sysuid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.sysuid(arg);
		};
		array[477] = val;
		val = new Command();
		val.Name = "teleport";
		val.Parent = "global";
		val.FullName = "global.teleport";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleport(arg);
		};
		array[478] = val;
		val = new Command();
		val.Name = "teleport2autheditem";
		val.Parent = "global";
		val.FullName = "global.teleport2autheditem";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleport2autheditem(arg);
		};
		array[479] = val;
		val = new Command();
		val.Name = "teleport2death";
		val.Parent = "global";
		val.FullName = "global.teleport2death";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleport2death(arg);
		};
		array[480] = val;
		val = new Command();
		val.Name = "teleport2marker";
		val.Parent = "global";
		val.FullName = "global.teleport2marker";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleport2marker(arg);
		};
		array[481] = val;
		val = new Command();
		val.Name = "teleport2me";
		val.Parent = "global";
		val.FullName = "global.teleport2me";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleport2me(arg);
		};
		array[482] = val;
		val = new Command();
		val.Name = "teleport2owneditem";
		val.Parent = "global";
		val.FullName = "global.teleport2owneditem";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleport2owneditem(arg);
		};
		array[483] = val;
		val = new Command();
		val.Name = "teleportany";
		val.Parent = "global";
		val.FullName = "global.teleportany";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleportany(arg);
		};
		array[484] = val;
		val = new Command();
		val.Name = "teleporteveryone2me";
		val.Parent = "global";
		val.FullName = "global.teleporteveryone2me";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleporteveryone2me(arg);
		};
		array[485] = val;
		val = new Command();
		val.Name = "teleportlos";
		val.Parent = "global";
		val.FullName = "global.teleportlos";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleportlos(arg);
		};
		array[486] = val;
		val = new Command();
		val.Name = "teleportpos";
		val.Parent = "global";
		val.FullName = "global.teleportpos";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleportpos(arg);
		};
		array[487] = val;
		val = new Command();
		val.Name = "textures";
		val.Parent = "global";
		val.FullName = "global.textures";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.textures(arg);
		};
		array[488] = val;
		val = new Command();
		val.Name = "version";
		val.Parent = "global";
		val.FullName = "global.version";
		val.ServerAdmin = true;
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.version(arg);
		};
		array[489] = val;
		val = new Command();
		val.Name = "warmupconcurrency";
		val.Parent = "global";
		val.FullName = "global.warmupconcurrency";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Global.warmupConcurrency.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.warmupConcurrency = StringExtensions.ToInt(str, 0);
		};
		array[490] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "halloween";
		val.FullName = "halloween.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Halloween.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.enabled = StringExtensions.ToBool(str);
		};
		array[491] = val;
		val = new Command();
		val.Name = "murdererpopulation";
		val.Parent = "halloween";
		val.FullName = "halloween.murdererpopulation";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.Variable = true;
		val.GetOveride = () => Halloween.murdererpopulation.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.murdererpopulation = StringExtensions.ToFloat(str, 0f);
		};
		array[492] = val;
		val = new Command();
		val.Name = "scarecrow_beancan_vs_player_dmg_modifier";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrow_beancan_vs_player_dmg_modifier";
		val.ServerAdmin = true;
		val.Description = "Modified damage from beancan explosion vs players (Default: 0.1).";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrow_beancan_vs_player_dmg_modifier.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrow_beancan_vs_player_dmg_modifier = StringExtensions.ToFloat(str, 0f);
		};
		array[493] = val;
		val = new Command();
		val.Name = "scarecrow_body_dmg_modifier";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrow_body_dmg_modifier";
		val.ServerAdmin = true;
		val.Description = "Modifier to how much damage scarecrows take to the body. (Default: 0.25)";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrow_body_dmg_modifier.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrow_body_dmg_modifier = StringExtensions.ToFloat(str, 0f);
		};
		array[494] = val;
		val = new Command();
		val.Name = "scarecrow_chase_stopping_distance";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrow_chase_stopping_distance";
		val.ServerAdmin = true;
		val.Description = "Stopping distance for destinations set while chasing a target (Default: 0.5)";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrow_chase_stopping_distance.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrow_chase_stopping_distance = StringExtensions.ToFloat(str, 0f);
		};
		array[495] = val;
		val = new Command();
		val.Name = "scarecrow_throw_beancan_global_delay";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrow_throw_beancan_global_delay";
		val.ServerAdmin = true;
		val.Description = "The delay globally on a server between each time a scarecrow throws a beancan (Default: 8 seconds).";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrow_throw_beancan_global_delay.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrow_throw_beancan_global_delay = StringExtensions.ToFloat(str, 0f);
		};
		array[496] = val;
		val = new Command();
		val.Name = "scarecrowpopulation";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrowpopulation";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrowpopulation.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrowpopulation = StringExtensions.ToFloat(str, 0f);
		};
		array[497] = val;
		val = new Command();
		val.Name = "scarecrows_throw_beancans";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrows_throw_beancans";
		val.ServerAdmin = true;
		val.Description = "Scarecrows can throw beancans (Default: true).";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrows_throw_beancans.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrows_throw_beancans = StringExtensions.ToBool(str);
		};
		array[498] = val;
		val = new Command();
		val.Name = "load";
		val.Parent = "harmony";
		val.FullName = "harmony.load";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Harmony.Load(arg);
		};
		array[499] = val;
		val = new Command();
		val.Name = "unload";
		val.Parent = "harmony";
		val.FullName = "harmony.unload";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Harmony.Unload(arg);
		};
		array[500] = val;
		val = new Command();
		val.Name = "cd";
		val.Parent = "hierarchy";
		val.FullName = "hierarchy.cd";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Hierarchy.cd(arg);
		};
		array[501] = val;
		val = new Command();
		val.Name = "del";
		val.Parent = "hierarchy";
		val.FullName = "hierarchy.del";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Hierarchy.del(arg);
		};
		array[502] = val;
		val = new Command();
		val.Name = "ls";
		val.Parent = "hierarchy";
		val.FullName = "hierarchy.ls";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Hierarchy.ls(arg);
		};
		array[503] = val;
		val = new Command();
		val.Name = "clearinventory";
		val.Parent = "inventory";
		val.FullName = "inventory.clearinventory";
		val.ServerAdmin = true;
		val.Description = "Clears the inventory of a target player. eg. inventory.clearInventory jim";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.clearInventory(arg);
		};
		array[504] = val;
		val = new Command();
		val.Name = "copyto";
		val.Parent = "inventory";
		val.FullName = "inventory.copyto";
		val.ServerAdmin = true;
		val.Description = "Copies the players inventory to the player in front of them";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.copyTo(arg);
		};
		array[505] = val;
		val = new Command();
		val.Name = "defs";
		val.Parent = "inventory";
		val.FullName = "inventory.defs";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.defs(arg);
		};
		array[506] = val;
		val = new Command();
		val.Name = "deployloadout";
		val.Parent = "inventory";
		val.FullName = "inventory.deployloadout";
		val.ServerAdmin = true;
		val.Description = "Deploys the given loadout to a target player. eg. inventory.deployLoadout testloadout jim";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.deployLoadout(arg);
		};
		array[507] = val;
		val = new Command();
		val.Name = "deployloadoutinrange";
		val.Parent = "inventory";
		val.FullName = "inventory.deployloadoutinrange";
		val.ServerAdmin = true;
		val.Description = "Deploys a loadout to players in a radius eg. inventory.deployLoadoutInRange testloadout 30";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.deployLoadoutInRange(arg);
		};
		array[508] = val;
		val = new Command();
		val.Name = "disableattirelimitations";
		val.Parent = "inventory";
		val.FullName = "inventory.disableattirelimitations";
		val.ServerAdmin = true;
		val.Description = "Disables all attire limitations, so NPC clothing and invalid overlaps can be equipped";
		val.Variable = true;
		val.GetOveride = () => Inventory.disableAttireLimitations.ToString();
		val.SetOveride = delegate(string str)
		{
			Inventory.disableAttireLimitations = StringExtensions.ToBool(str);
		};
		array[509] = val;
		val = new Command();
		val.Name = "endloot";
		val.Parent = "inventory";
		val.FullName = "inventory.endloot";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.endloot(arg);
		};
		array[510] = val;
		val = new Command();
		val.Name = "equipslot";
		val.Parent = "inventory";
		val.FullName = "inventory.equipslot";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.equipslot(arg);
		};
		array[511] = val;
		val = new Command();
		val.Name = "equipslottarget";
		val.Parent = "inventory";
		val.FullName = "inventory.equipslottarget";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.equipslottarget(arg);
		};
		array[512] = val;
		val = new Command();
		val.Name = "give";
		val.Parent = "inventory";
		val.FullName = "inventory.give";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.give(arg);
		};
		array[513] = val;
		val = new Command();
		val.Name = "giveall";
		val.Parent = "inventory";
		val.FullName = "inventory.giveall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.giveall(arg);
		};
		array[514] = val;
		val = new Command();
		val.Name = "givearm";
		val.Parent = "inventory";
		val.FullName = "inventory.givearm";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.givearm(arg);
		};
		array[515] = val;
		val = new Command();
		val.Name = "givebp";
		val.Parent = "inventory";
		val.FullName = "inventory.givebp";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.giveBp(arg);
		};
		array[516] = val;
		val = new Command();
		val.Name = "giveid";
		val.Parent = "inventory";
		val.FullName = "inventory.giveid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.giveid(arg);
		};
		array[517] = val;
		val = new Command();
		val.Name = "giveto";
		val.Parent = "inventory";
		val.FullName = "inventory.giveto";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.giveto(arg);
		};
		array[518] = val;
		val = new Command();
		val.Name = "lighttoggle";
		val.Parent = "inventory";
		val.FullName = "inventory.lighttoggle";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.lighttoggle(arg);
		};
		array[519] = val;
		val = new Command();
		val.Name = "listloadouts";
		val.Parent = "inventory";
		val.FullName = "inventory.listloadouts";
		val.ServerAdmin = true;
		val.Description = "Prints all saved inventory loadouts";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.listloadouts(arg);
		};
		array[520] = val;
		val = new Command();
		val.Name = "reloaddefs";
		val.Parent = "inventory";
		val.FullName = "inventory.reloaddefs";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.reloaddefs(arg);
		};
		array[521] = val;
		val = new Command();
		val.Name = "resetbp";
		val.Parent = "inventory";
		val.FullName = "inventory.resetbp";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.resetbp(arg);
		};
		array[522] = val;
		val = new Command();
		val.Name = "saveloadout";
		val.Parent = "inventory";
		val.FullName = "inventory.saveloadout";
		val.ServerAdmin = true;
		val.Description = "Saves the current equipped loadout of the calling player. eg. inventory.saveLoadout loaduoutname";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.saveloadout(arg);
		};
		array[523] = val;
		val = new Command();
		val.Name = "unlockall";
		val.Parent = "inventory";
		val.FullName = "inventory.unlockall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.unlockall(arg);
		};
		array[524] = val;
		val = new Command();
		val.Name = "printmanifest";
		val.Parent = "manifest";
		val.FullName = "manifest.printmanifest";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			object obj2 = Manifest.PrintManifest();
			arg.ReplyWithObject(obj2);
		};
		array[525] = val;
		val = new Command();
		val.Name = "printmanifestraw";
		val.Parent = "manifest";
		val.FullName = "manifest.printmanifestraw";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			object obj = Manifest.PrintManifestRaw();
			arg.ReplyWithObject(obj);
		};
		array[526] = val;
		val = new Command();
		val.Name = "full";
		val.Parent = "memsnap";
		val.FullName = "memsnap.full";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			MemSnap.full(arg);
		};
		array[527] = val;
		val = new Command();
		val.Name = "managed";
		val.Parent = "memsnap";
		val.FullName = "memsnap.managed";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			MemSnap.managed(arg);
		};
		array[528] = val;
		val = new Command();
		val.Name = "native";
		val.Parent = "memsnap";
		val.FullName = "memsnap.native";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			MemSnap.native(arg);
		};
		array[529] = val;
		val = new Command();
		val.Name = "global_network_debug";
		val.Parent = "net";
		val.FullName = "net.global_network_debug";
		val.ServerAdmin = true;
		val.Description = "Toggle printing time taken to send all trees & all global entities to client when they connect";
		val.Variable = true;
		val.GetOveride = () => Net.global_network_debug.ToString();
		val.SetOveride = delegate(string str)
		{
			Net.global_network_debug = StringExtensions.ToBool(str);
		};
		array[530] = val;
		val = new Command();
		val.Name = "global_networked_bases";
		val.Parent = "net";
		val.FullName = "net.global_networked_bases";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Net.globalNetworkedBases.ToString();
		val.SetOveride = delegate(string str)
		{
			Net.globalNetworkedBases = StringExtensions.ToBool(str);
		};
		array[531] = val;
		val = new Command();
		val.Name = "limit_global_update_broadcast";
		val.Parent = "net";
		val.FullName = "net.limit_global_update_broadcast";
		val.ServerAdmin = true;
		val.Description = "(default) true = only broadcast to clients with global networking enabled, false = broadcast to every client regardless";
		val.Variable = true;
		val.GetOveride = () => Net.limit_global_update_broadcast.ToString();
		val.SetOveride = delegate(string str)
		{
			Net.limit_global_update_broadcast = StringExtensions.ToBool(str);
		};
		array[532] = val;
		val = new Command();
		val.Name = "visdebug";
		val.Parent = "net";
		val.FullName = "net.visdebug";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Net.visdebug.ToString();
		val.SetOveride = delegate(string str)
		{
			Net.visdebug = StringExtensions.ToBool(str);
		};
		array[533] = val;
		val = new Command();
		val.Name = "visibilityradiusfaroverride";
		val.Parent = "net";
		val.FullName = "net.visibilityradiusfaroverride";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Net.visibilityRadiusFarOverride.ToString();
		val.SetOveride = delegate(string str)
		{
			Net.visibilityRadiusFarOverride = StringExtensions.ToInt(str, 0);
		};
		array[534] = val;
		val = new Command();
		val.Name = "visibilityradiusnearoverride";
		val.Parent = "net";
		val.FullName = "net.visibilityradiusnearoverride";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Net.visibilityRadiusNearOverride.ToString();
		val.SetOveride = delegate(string str)
		{
			Net.visibilityRadiusNearOverride = StringExtensions.ToInt(str, 0);
		};
		array[535] = val;
		val = new Command();
		val.Name = "broadcast_ping";
		val.Parent = "nexus";
		val.FullName = "nexus.broadcast_ping";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Nexus.broadcast_ping(arg);
		};
		array[536] = val;
		val = new Command();
		val.Name = "clanclatbatchduration";
		val.Parent = "nexus";
		val.FullName = "nexus.clanclatbatchduration";
		val.ServerAdmin = true;
		val.Description = "Maximum duration in seconds to batch clan chat messages to send to other servers on the nexus";
		val.Variable = true;
		val.GetOveride = () => Nexus.clanClatBatchDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.clanClatBatchDuration = StringExtensions.ToFloat(str, 0f);
		};
		array[537] = val;
		val = new Command();
		val.Name = "defaultzonecontactradius";
		val.Parent = "nexus";
		val.FullName = "nexus.defaultzonecontactradius";
		val.ServerAdmin = true;
		val.Description = "Default distance between zones to allow boat travel, if map.contactRadius isn't set in the nexus (uses normalized coordinates)";
		val.Variable = true;
		val.GetOveride = () => Nexus.defaultZoneContactRadius.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.defaultZoneContactRadius = StringExtensions.ToFloat(str, 0f);
		};
		array[538] = val;
		val = new Command();
		val.Name = "endpoint";
		val.Parent = "nexus";
		val.FullName = "nexus.endpoint";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "URL endpoint to use for the Nexus API";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Nexus.endpoint.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.endpoint = str;
		};
		val.Default = "https://api.facepunch.com/api/nexus/";
		array[539] = val;
		val = new Command();
		val.Name = "islandspawndistance";
		val.Parent = "nexus";
		val.FullName = "nexus.islandspawndistance";
		val.ServerAdmin = true;
		val.Description = "How far away islands should be spawned, as a factor of the map size";
		val.Variable = true;
		val.GetOveride = () => Nexus.islandSpawnDistance.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.islandSpawnDistance = StringExtensions.ToFloat(str, 0f);
		};
		array[540] = val;
		val = new Command();
		val.Name = "loadingtimeout";
		val.Parent = "nexus";
		val.FullName = "nexus.loadingtimeout";
		val.ServerAdmin = true;
		val.Description = "Time in seconds to keep players in the loading state before going to sleep";
		val.Variable = true;
		val.GetOveride = () => Nexus.loadingTimeout.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.loadingTimeout = StringExtensions.ToFloat(str, 0f);
		};
		array[541] = val;
		val = new Command();
		val.Name = "logging";
		val.Parent = "nexus";
		val.FullName = "nexus.logging";
		val.ServerAdmin = true;
		val.Client = true;
		val.Variable = true;
		val.GetOveride = () => Nexus.logging.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.logging = StringExtensions.ToBool(str);
		};
		array[542] = val;
		val = new Command();
		val.Name = "mapimagescale";
		val.Parent = "nexus";
		val.FullName = "nexus.mapimagescale";
		val.ServerAdmin = true;
		val.Description = "Scale of the map to render and upload to the nexus";
		val.Variable = true;
		val.GetOveride = () => Nexus.mapImageScale.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.mapImageScale = StringExtensions.ToFloat(str, 0f);
		};
		array[543] = val;
		val = new Command();
		val.Name = "messagelockduration";
		val.Parent = "nexus";
		val.FullName = "nexus.messagelockduration";
		val.ServerAdmin = true;
		val.Description = "Time in seconds to allow the server to process nexus messages before re-sending (requires restart)";
		val.Variable = true;
		val.GetOveride = () => Nexus.messageLockDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.messageLockDuration = StringExtensions.ToInt(str, 0);
		};
		array[544] = val;
		val = new Command();
		val.Name = "ping";
		val.Parent = "nexus";
		val.FullName = "nexus.ping";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Nexus.ping(arg);
		};
		array[545] = val;
		val = new Command();
		val.Name = "pinginterval";
		val.Parent = "nexus";
		val.FullName = "nexus.pinginterval";
		val.ServerAdmin = true;
		val.Description = "Time in seconds to wait between server status pings";
		val.Variable = true;
		val.GetOveride = () => Nexus.pingInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.pingInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[546] = val;
		val = new Command();
		val.Name = "playermanifestinterval";
		val.Parent = "nexus";
		val.FullName = "nexus.playermanifestinterval";
		val.ServerAdmin = true;
		val.Description = "Interval in seconds to broadcast the player manifest to other servers on the nexus";
		val.Variable = true;
		val.GetOveride = () => Nexus.playerManifestInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.playerManifestInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[547] = val;
		val = new Command();
		val.Name = "playeronline";
		val.Parent = "nexus";
		val.FullName = "nexus.playeronline";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Nexus.playeronline(arg);
		};
		array[548] = val;
		val = new Command();
		val.Name = "protectionduration";
		val.Parent = "nexus";
		val.FullName = "nexus.protectionduration";
		val.ServerAdmin = true;
		val.Description = "Maximum time in seconds to keep transfer protection enabled on entities";
		val.Variable = true;
		val.GetOveride = () => Nexus.protectionDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.protectionDuration = StringExtensions.ToFloat(str, 0f);
		};
		array[549] = val;
		val = new Command();
		val.Name = "refreshislands";
		val.Parent = "nexus";
		val.FullName = "nexus.refreshislands";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Nexus.refreshislands(arg);
		};
		array[550] = val;
		val = new Command();
		val.Name = "rpctimeoutmultiplier";
		val.Parent = "nexus";
		val.FullName = "nexus.rpctimeoutmultiplier";
		val.ServerAdmin = true;
		val.Description = "Multiplier for nexus RPC timeout durations in case we expect different latencies";
		val.Variable = true;
		val.GetOveride = () => Nexus.rpcTimeoutMultiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.rpcTimeoutMultiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[551] = val;
		val = new Command();
		val.Name = "secretkey";
		val.Parent = "nexus";
		val.FullName = "nexus.secretkey";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Nexus.secretKey.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.secretKey = str;
		};
		array[552] = val;
		val = new Command();
		val.Name = "timeoffset";
		val.Parent = "nexus";
		val.FullName = "nexus.timeoffset";
		val.ServerAdmin = true;
		val.Description = "Time offset in hours from the nexus clock";
		val.Variable = true;
		val.GetOveride = () => Nexus.timeOffset.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.timeOffset = StringExtensions.ToFloat(str, 0f);
		};
		array[553] = val;
		val = new Command();
		val.Name = "transfer";
		val.Parent = "nexus";
		val.FullName = "nexus.transfer";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Nexus.transfer(arg);
		};
		array[554] = val;
		val = new Command();
		val.Name = "transferflushtime";
		val.Parent = "nexus";
		val.FullName = "nexus.transferflushtime";
		val.ServerAdmin = true;
		val.Description = "Maximum amount of time in seconds that transfers should be cached before auto-saving";
		val.Variable = true;
		val.GetOveride = () => Nexus.transferFlushTime.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.transferFlushTime = StringExtensions.ToInt(str, 0);
		};
		array[555] = val;
		val = new Command();
		val.Name = "uploadmap";
		val.Parent = "nexus";
		val.FullName = "nexus.uploadmap";
		val.ServerAdmin = true;
		val.Description = "Reupload the map image to the nexus. Normally happens automatically at server boot. WARNING: This will lag the server!";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Nexus.uploadmap(arg);
		};
		array[556] = val;
		val = new Command();
		val.Name = "zonecontroller";
		val.Parent = "nexus";
		val.FullName = "nexus.zonecontroller";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Nexus.zoneController.ToString();
		val.SetOveride = delegate(string str)
		{
			Nexus.zoneController = str;
		};
		array[557] = val;
		val = new Command();
		val.Name = "bulletaccuracy";
		val.Parent = "heli";
		val.FullName = "heli.bulletaccuracy";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.PatrolHelicopter.bulletAccuracy.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.PatrolHelicopter.bulletAccuracy = StringExtensions.ToFloat(str, 0f);
		};
		array[558] = val;
		val = new Command();
		val.Name = "bulletdamagescale";
		val.Parent = "heli";
		val.FullName = "heli.bulletdamagescale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.PatrolHelicopter.bulletDamageScale.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.PatrolHelicopter.bulletDamageScale = StringExtensions.ToFloat(str, 0f);
		};
		array[559] = val;
		val = new Command();
		val.Name = "call";
		val.Parent = "heli";
		val.FullName = "heli.call";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.PatrolHelicopter.call(arg);
		};
		array[560] = val;
		val = new Command();
		val.Name = "calltome";
		val.Parent = "heli";
		val.FullName = "heli.calltome";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.PatrolHelicopter.calltome(arg);
		};
		array[561] = val;
		val = new Command();
		val.Name = "drop";
		val.Parent = "heli";
		val.FullName = "heli.drop";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.PatrolHelicopter.drop(arg);
		};
		array[562] = val;
		val = new Command();
		val.Name = "guns";
		val.Parent = "heli";
		val.FullName = "heli.guns";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.PatrolHelicopter.guns.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.PatrolHelicopter.guns = StringExtensions.ToInt(str, 0);
		};
		array[563] = val;
		val = new Command();
		val.Name = "lifetimeminutes";
		val.Parent = "heli";
		val.FullName = "heli.lifetimeminutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.PatrolHelicopter.lifetimeMinutes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.PatrolHelicopter.lifetimeMinutes = StringExtensions.ToFloat(str, 0f);
		};
		array[564] = val;
		val = new Command();
		val.Name = "strafe";
		val.Parent = "heli";
		val.FullName = "heli.strafe";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.PatrolHelicopter.strafe(arg);
		};
		array[565] = val;
		val = new Command();
		val.Name = "testpuzzle";
		val.Parent = "heli";
		val.FullName = "heli.testpuzzle";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.PatrolHelicopter.testpuzzle(arg);
		};
		array[566] = val;
		val = new Command();
		val.Name = "autosynctransforms";
		val.Parent = "physics";
		val.FullName = "physics.autosynctransforms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.autosynctransforms.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.autosynctransforms = StringExtensions.ToBool(str);
		};
		array[567] = val;
		val = new Command();
		val.Name = "batchsynctransforms";
		val.Parent = "physics";
		val.FullName = "physics.batchsynctransforms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.batchsynctransforms.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.batchsynctransforms = StringExtensions.ToBool(str);
		};
		array[568] = val;
		val = new Command();
		val.Name = "bouncethreshold";
		val.Parent = "physics";
		val.FullName = "physics.bouncethreshold";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.bouncethreshold.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.bouncethreshold = StringExtensions.ToFloat(str, 0f);
		};
		array[569] = val;
		val = new Command();
		val.Name = "gravity";
		val.Parent = "physics";
		val.FullName = "physics.gravity";
		val.ServerAdmin = true;
		val.Description = "Gravity multiplier";
		val.Variable = true;
		val.GetOveride = () => Physics.gravity.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.gravity = StringExtensions.ToFloat(str, 0f);
		};
		array[570] = val;
		val = new Command();
		val.Name = "groundwatchdebug";
		val.Parent = "physics";
		val.FullName = "physics.groundwatchdebug";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.groundwatchdebug.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.groundwatchdebug = StringExtensions.ToBool(str);
		};
		array[571] = val;
		val = new Command();
		val.Name = "groundwatchdelay";
		val.Parent = "physics";
		val.FullName = "physics.groundwatchdelay";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.groundwatchdelay.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.groundwatchdelay = StringExtensions.ToFloat(str, 0f);
		};
		array[572] = val;
		val = new Command();
		val.Name = "groundwatchfails";
		val.Parent = "physics";
		val.FullName = "physics.groundwatchfails";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.groundwatchfails.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.groundwatchfails = StringExtensions.ToInt(str, 0);
		};
		array[573] = val;
		val = new Command();
		val.Name = "minsteps";
		val.Parent = "physics";
		val.FullName = "physics.minsteps";
		val.ServerAdmin = true;
		val.Description = "The slowest physics steps will operate";
		val.Variable = true;
		val.GetOveride = () => Physics.minsteps.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.minsteps = StringExtensions.ToFloat(str, 0f);
		};
		array[574] = val;
		val = new Command();
		val.Name = "sendeffects";
		val.Parent = "physics";
		val.FullName = "physics.sendeffects";
		val.ServerAdmin = true;
		val.Description = "Send effects to clients when physics objects collide";
		val.Variable = true;
		val.GetOveride = () => Physics.sendeffects.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.sendeffects = StringExtensions.ToBool(str);
		};
		array[575] = val;
		val = new Command();
		val.Name = "sleepthreshold";
		val.Parent = "physics";
		val.FullName = "physics.sleepthreshold";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.sleepthreshold.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.sleepthreshold = StringExtensions.ToFloat(str, 0f);
		};
		array[576] = val;
		val = new Command();
		val.Name = "solveriterationcount";
		val.Parent = "physics";
		val.FullName = "physics.solveriterationcount";
		val.ServerAdmin = true;
		val.Description = "The default solver iteration count permitted for any rigid bodies (default 7). Must be positive";
		val.Variable = true;
		val.GetOveride = () => Physics.solveriterationcount.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.solveriterationcount = StringExtensions.ToInt(str, 0);
		};
		array[577] = val;
		val = new Command();
		val.Name = "steps";
		val.Parent = "physics";
		val.FullName = "physics.steps";
		val.ServerAdmin = true;
		val.Description = "The amount of physics steps per second";
		val.Variable = true;
		val.GetOveride = () => Physics.steps.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.steps = StringExtensions.ToFloat(str, 0f);
		};
		array[578] = val;
		val = new Command();
		val.Name = "abandonmission";
		val.Parent = "player";
		val.FullName = "player.abandonmission";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.abandonmission(arg);
		};
		array[579] = val;
		val = new Command();
		val.Name = "cinematic_gesture";
		val.Parent = "player";
		val.FullName = "player.cinematic_gesture";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.cinematic_gesture(arg);
		};
		array[580] = val;
		val = new Command();
		val.Name = "cinematic_play";
		val.Parent = "player";
		val.FullName = "player.cinematic_play";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.cinematic_play(arg);
		};
		array[581] = val;
		val = new Command();
		val.Name = "cinematic_stop";
		val.Parent = "player";
		val.FullName = "player.cinematic_stop";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.cinematic_stop(arg);
		};
		array[582] = val;
		val = new Command();
		val.Name = "copyrotation";
		val.Parent = "player";
		val.FullName = "player.copyrotation";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.copyrotation(arg);
		};
		array[583] = val;
		val = new Command();
		val.Name = "createskull";
		val.Parent = "player";
		val.FullName = "player.createskull";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.createskull(arg);
		};
		array[584] = val;
		val = new Command();
		val.Name = "dismount";
		val.Parent = "player";
		val.FullName = "player.dismount";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.dismount(arg);
		};
		array[585] = val;
		val = new Command();
		val.Name = "fillwater";
		val.Parent = "player";
		val.FullName = "player.fillwater";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.fillwater(arg);
		};
		array[586] = val;
		val = new Command();
		val.Name = "gesture_radius";
		val.Parent = "player";
		val.FullName = "player.gesture_radius";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.gesture_radius(arg);
		};
		array[587] = val;
		val = new Command();
		val.Name = "gotosleep";
		val.Parent = "player";
		val.FullName = "player.gotosleep";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.gotosleep(arg);
		};
		array[588] = val;
		val = new Command();
		val.Name = "markhostile";
		val.Parent = "player";
		val.FullName = "player.markhostile";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.markhostile(arg);
		};
		array[589] = val;
		val = new Command();
		val.Name = "mount";
		val.Parent = "player";
		val.FullName = "player.mount";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.mount(arg);
		};
		array[590] = val;
		val = new Command();
		val.Name = "printpresence";
		val.Parent = "player";
		val.FullName = "player.printpresence";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.printpresence(arg);
		};
		array[591] = val;
		val = new Command();
		val.Name = "printstats";
		val.Parent = "player";
		val.FullName = "player.printstats";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.printstats(arg);
		};
		array[592] = val;
		val = new Command();
		val.Name = "reloadweapons";
		val.Parent = "player";
		val.FullName = "player.reloadweapons";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.reloadweapons(arg);
		};
		array[593] = val;
		val = new Command();
		val.Name = "resetstate";
		val.Parent = "player";
		val.FullName = "player.resetstate";
		val.ServerAdmin = true;
		val.Description = "Resets the PlayerState of the given player";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.resetstate(arg);
		};
		array[594] = val;
		val = new Command();
		val.Name = "stopgesture_radius";
		val.Parent = "player";
		val.FullName = "player.stopgesture_radius";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.stopgesture_radius(arg);
		};
		array[595] = val;
		val = new Command();
		val.Name = "swapseat";
		val.Parent = "player";
		val.FullName = "player.swapseat";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.swapseat(arg);
		};
		array[596] = val;
		val = new Command();
		val.Name = "tickrate_cl";
		val.Parent = "player";
		val.FullName = "player.tickrate_cl";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Player.tickrate_cl.ToString();
		val.SetOveride = delegate(string str)
		{
			Player.tickrate_cl = StringExtensions.ToInt(str, 0);
		};
		array[597] = val;
		val = new Command();
		val.Name = "tickrate_sv";
		val.Parent = "player";
		val.FullName = "player.tickrate_sv";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Player.tickrate_sv.ToString();
		val.SetOveride = delegate(string str)
		{
			Player.tickrate_sv = StringExtensions.ToInt(str, 0);
		};
		array[598] = val;
		val = new Command();
		val.Name = "wakeup";
		val.Parent = "player";
		val.FullName = "player.wakeup";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.wakeup(arg);
		};
		array[599] = val;
		val = new Command();
		val.Name = "wakeupall";
		val.Parent = "player";
		val.FullName = "player.wakeupall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.wakeupall(arg);
		};
		array[600] = val;
		val = new Command();
		val.Name = "woundforever";
		val.Parent = "player";
		val.FullName = "player.woundforever";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Whether the crawling state expires";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Player.woundforever.ToString();
		val.SetOveride = delegate(string str)
		{
			Player.woundforever = StringExtensions.ToBool(str);
		};
		array[601] = val;
		val = new Command();
		val.Name = "clear_assets";
		val.Parent = "pool";
		val.FullName = "pool.clear_assets";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.clear_assets(arg);
		};
		array[602] = val;
		val = new Command();
		val.Name = "clear_memory";
		val.Parent = "pool";
		val.FullName = "pool.clear_memory";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.clear_memory(arg);
		};
		array[603] = val;
		val = new Command();
		val.Name = "clear_prefabs";
		val.Parent = "pool";
		val.FullName = "pool.clear_prefabs";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.clear_prefabs(arg);
		};
		array[604] = val;
		val = new Command();
		val.Name = "debug";
		val.Parent = "pool";
		val.FullName = "pool.debug";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Pool.debug.ToString();
		val.SetOveride = delegate(string str)
		{
			Pool.debug = StringExtensions.ToBool(str);
		};
		array[605] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "pool";
		val.FullName = "pool.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Pool.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Pool.enabled = StringExtensions.ToBool(str);
		};
		array[606] = val;
		val = new Command();
		val.Name = "export_prefabs";
		val.Parent = "pool";
		val.FullName = "pool.export_prefabs";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.export_prefabs(arg);
		};
		array[607] = val;
		val = new Command();
		val.Name = "fill_prefabs";
		val.Parent = "pool";
		val.FullName = "pool.fill_prefabs";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.fill_prefabs(arg);
		};
		array[608] = val;
		val = new Command();
		val.Name = "mode";
		val.Parent = "pool";
		val.FullName = "pool.mode";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Pool.mode.ToString();
		val.SetOveride = delegate(string str)
		{
			Pool.mode = StringExtensions.ToInt(str, 0);
		};
		array[609] = val;
		val = new Command();
		val.Name = "prewarm";
		val.Parent = "pool";
		val.FullName = "pool.prewarm";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Pool.prewarm.ToString();
		val.SetOveride = delegate(string str)
		{
			Pool.prewarm = StringExtensions.ToBool(str);
		};
		array[610] = val;
		val = new Command();
		val.Name = "print_arraypool";
		val.Parent = "pool";
		val.FullName = "pool.print_arraypool";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.print_arraypool(arg);
		};
		array[611] = val;
		val = new Command();
		val.Name = "print_assets";
		val.Parent = "pool";
		val.FullName = "pool.print_assets";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.print_assets(arg);
		};
		array[612] = val;
		val = new Command();
		val.Name = "print_memory";
		val.Parent = "pool";
		val.FullName = "pool.print_memory";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.print_memory(arg);
		};
		array[613] = val;
		val = new Command();
		val.Name = "print_prefabs";
		val.Parent = "pool";
		val.FullName = "pool.print_prefabs";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.print_prefabs(arg);
		};
		array[614] = val;
		val = new Command();
		val.Name = "flush_analytics";
		val.Parent = "profile";
		val.FullName = "profile.flush_analytics";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Profile.flush_analytics(arg);
		};
		array[615] = val;
		val = new Command();
		val.Name = "start";
		val.Parent = "profile";
		val.FullName = "profile.start";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Profile.start(arg);
		};
		array[616] = val;
		val = new Command();
		val.Name = "stop";
		val.Parent = "profile";
		val.FullName = "profile.stop";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Profile.stop(arg);
		};
		array[617] = val;
		val = new Command();
		val.Name = "print_global_entities";
		val.Parent = "render";
		val.FullName = "render.print_global_entities";
		val.ServerAdmin = true;
		val.Description = "Print off count of global building entities on the server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Render.print_global_entities(arg);
		};
		array[618] = val;
		val = new Command();
		val.Name = "tree_entities";
		val.Parent = "render";
		val.FullName = "render.tree_entities";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Render.tree_entities(arg);
		};
		array[619] = val;
		val = new Command();
		val.Name = "hostileduration";
		val.Parent = "sentry";
		val.FullName = "sentry.hostileduration";
		val.ServerAdmin = true;
		val.Description = "how long until something is considered hostile after it attacked";
		val.Variable = true;
		val.GetOveride = () => Sentry.hostileduration.ToString();
		val.SetOveride = delegate(string str)
		{
			Sentry.hostileduration = StringExtensions.ToFloat(str, 0f);
		};
		array[620] = val;
		val = new Command();
		val.Name = "interferenceradius";
		val.Parent = "sentry";
		val.FullName = "sentry.interferenceradius";
		val.ServerAdmin = true;
		val.Description = "radius to check for other turrets";
		val.Variable = true;
		val.GetOveride = () => Sentry.interferenceradius.ToString();
		val.SetOveride = delegate(string str)
		{
			Sentry.interferenceradius = StringExtensions.ToFloat(str, 0f);
		};
		array[621] = val;
		val = new Command();
		val.Name = "maxinterference";
		val.Parent = "sentry";
		val.FullName = "sentry.maxinterference";
		val.ServerAdmin = true;
		val.Description = "max interference from other turrets";
		val.Variable = true;
		val.GetOveride = () => Sentry.maxinterference.ToString();
		val.SetOveride = delegate(string str)
		{
			Sentry.maxinterference = StringExtensions.ToFloat(str, 0f);
		};
		array[622] = val;
		val = new Command();
		val.Name = "targetall";
		val.Parent = "sentry";
		val.FullName = "sentry.targetall";
		val.ServerAdmin = true;
		val.Description = "target everyone regardless of authorization";
		val.Variable = true;
		val.GetOveride = () => Sentry.targetall.ToString();
		val.SetOveride = delegate(string str)
		{
			Sentry.targetall = StringExtensions.ToBool(str);
		};
		array[623] = val;
		val = new Command();
		val.Name = "anticheatid";
		val.Parent = "server";
		val.FullName = "server.anticheatid";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.anticheatid.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.anticheatid = str;
		};
		array[624] = val;
		val = new Command();
		val.Name = "anticheatkey";
		val.Parent = "server";
		val.FullName = "server.anticheatkey";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.anticheatkey.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.anticheatkey = str;
		};
		array[625] = val;
		val = new Command();
		val.Name = "anticheatlog";
		val.Parent = "server";
		val.FullName = "server.anticheatlog";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.anticheatlog.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.anticheatlog = StringExtensions.ToInt(str, 0);
		};
		array[626] = val;
		val = new Command();
		val.Name = "anticheattoken";
		val.Parent = "server";
		val.FullName = "server.anticheattoken";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.anticheattoken.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.anticheattoken = StringExtensions.ToBool(str);
		};
		array[627] = val;
		val = new Command();
		val.Name = "arrowarmor";
		val.Parent = "server";
		val.FullName = "server.arrowarmor";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.arrowarmor.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.arrowarmor = StringExtensions.ToFloat(str, 0f);
		};
		array[628] = val;
		val = new Command();
		val.Name = "arrowdamage";
		val.Parent = "server";
		val.FullName = "server.arrowdamage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.arrowdamage.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.arrowdamage = StringExtensions.ToFloat(str, 0f);
		};
		array[629] = val;
		val = new Command();
		val.Name = "artificialtemperaturegrowablerange";
		val.Parent = "server";
		val.FullName = "server.artificialtemperaturegrowablerange";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.artificialTemperatureGrowableRange.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.artificialTemperatureGrowableRange = StringExtensions.ToFloat(str, 0f);
		};
		array[630] = val;
		val = new Command();
		val.Name = "authtimeout";
		val.Parent = "server";
		val.FullName = "server.authtimeout";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.authtimeout.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.authtimeout = StringExtensions.ToInt(str, 0);
		};
		array[631] = val;
		val = new Command();
		val.Name = "backup";
		val.Parent = "server";
		val.FullName = "server.backup";
		val.ServerAdmin = true;
		val.Description = "Backup server folder";
		val.Variable = false;
		val.Call = delegate
		{
			ConVar.Server.backup();
		};
		array[632] = val;
		val = new Command();
		val.Name = "bag_quota_item_amount";
		val.Parent = "server";
		val.FullName = "server.bag_quota_item_amount";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.bag_quota_item_amount.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.bag_quota_item_amount = StringExtensions.ToBool(str);
		};
		val.Default = "True";
		array[633] = val;
		val = new Command();
		val.Name = "bansserverendpoint";
		val.Parent = "server";
		val.FullName = "server.bansserverendpoint";
		val.ServerAdmin = true;
		val.Description = "HTTP API endpoint for centralized banning (see wiki)";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.bansServerEndpoint.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.bansServerEndpoint = str;
		};
		array[634] = val;
		val = new Command();
		val.Name = "bansserverfailuremode";
		val.Parent = "server";
		val.FullName = "server.bansserverfailuremode";
		val.ServerAdmin = true;
		val.Description = "Failure mode for centralized banning, set to 1 to reject players from joining if it's down (see wiki)";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.bansServerFailureMode.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.bansServerFailureMode = StringExtensions.ToInt(str, 0);
		};
		array[635] = val;
		val = new Command();
		val.Name = "bansservertimeout";
		val.Parent = "server";
		val.FullName = "server.bansservertimeout";
		val.ServerAdmin = true;
		val.Description = "Timeout (in seconds) for centralized banning web server requests";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.bansServerTimeout.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.bansServerTimeout = StringExtensions.ToInt(str, 0);
		};
		array[636] = val;
		val = new Command();
		val.Name = "bleedingarmor";
		val.Parent = "server";
		val.FullName = "server.bleedingarmor";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.bleedingarmor.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.bleedingarmor = StringExtensions.ToFloat(str, 0f);
		};
		array[637] = val;
		val = new Command();
		val.Name = "bleedingdamage";
		val.Parent = "server";
		val.FullName = "server.bleedingdamage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.bleedingdamage.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.bleedingdamage = StringExtensions.ToFloat(str, 0f);
		};
		array[638] = val;
		val = new Command();
		val.Name = "branch";
		val.Parent = "server";
		val.FullName = "server.branch";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.branch.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.branch = str;
		};
		array[639] = val;
		val = new Command();
		val.Name = "broadcastplayvideo";
		val.Parent = "server";
		val.FullName = "server.broadcastplayvideo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.BroadcastPlayVideo(arg);
		};
		array[640] = val;
		val = new Command();
		val.Name = "bulletarmor";
		val.Parent = "server";
		val.FullName = "server.bulletarmor";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.bulletarmor.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.bulletarmor = StringExtensions.ToFloat(str, 0f);
		};
		array[641] = val;
		val = new Command();
		val.Name = "bulletdamage";
		val.Parent = "server";
		val.FullName = "server.bulletdamage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.bulletdamage.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.bulletdamage = StringExtensions.ToFloat(str, 0f);
		};
		array[642] = val;
		val = new Command();
		val.Name = "canequipbackpacksinair";
		val.Parent = "server";
		val.FullName = "server.canequipbackpacksinair";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Allows backpack equipping while not grounded";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.canEquipBackpacksInAir.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.canEquipBackpacksInAir = StringExtensions.ToBool(str);
		};
		array[643] = val;
		val = new Command();
		val.Name = "ceilinglightgrowablerange";
		val.Parent = "server";
		val.FullName = "server.ceilinglightgrowablerange";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.ceilingLightGrowableRange.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.ceilingLightGrowableRange = StringExtensions.ToFloat(str, 0f);
		};
		array[644] = val;
		val = new Command();
		val.Name = "ceilinglightheightoffset";
		val.Parent = "server";
		val.FullName = "server.ceilinglightheightoffset";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.ceilingLightHeightOffset.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.ceilingLightHeightOffset = StringExtensions.ToFloat(str, 0f);
		};
		array[645] = val;
		val = new Command();
		val.Name = "censorplayerlist";
		val.Parent = "server";
		val.FullName = "server.censorplayerlist";
		val.ServerAdmin = true;
		val.Description = "Censors the Steam player list to make player tracking more difficult";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.censorplayerlist.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.censorplayerlist = StringExtensions.ToBool(str);
		};
		array[646] = val;
		val = new Command();
		val.Name = "cheatreport";
		val.Parent = "server";
		val.FullName = "server.cheatreport";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.cheatreport(arg);
		};
		array[647] = val;
		val = new Command();
		val.Name = "cinematic";
		val.Parent = "server";
		val.FullName = "server.cinematic";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.cinematic.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.cinematic = StringExtensions.ToBool(str);
		};
		array[648] = val;
		val = new Command();
		val.Name = "combatlog";
		val.Parent = "server";
		val.FullName = "server.combatlog";
		val.ServerAdmin = true;
		val.ServerUser = true;
		val.Description = "Get the player combat log";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text13 = ConVar.Server.combatlog(arg);
			arg.ReplyWithObject((object)text13);
		};
		array[649] = val;
		val = new Command();
		val.Name = "combatlog_outgoing";
		val.Parent = "server";
		val.FullName = "server.combatlog_outgoing";
		val.ServerAdmin = true;
		val.ServerUser = true;
		val.Description = "Get the player combat log, only showing outgoing damage";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text12 = ConVar.Server.combatlog_outgoing(arg);
			arg.ReplyWithObject((object)text12);
		};
		array[650] = val;
		val = new Command();
		val.Name = "combatlogdelay";
		val.Parent = "server";
		val.FullName = "server.combatlogdelay";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.combatlogdelay.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.combatlogdelay = StringExtensions.ToInt(str, 0);
		};
		array[651] = val;
		val = new Command();
		val.Name = "combatlogsize";
		val.Parent = "server";
		val.FullName = "server.combatlogsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.combatlogsize.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.combatlogsize = StringExtensions.ToInt(str, 0);
		};
		array[652] = val;
		val = new Command();
		val.Name = "composterupdateinterval";
		val.Parent = "server";
		val.FullName = "server.composterupdateinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.composterUpdateInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.composterUpdateInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[653] = val;
		val = new Command();
		val.Name = "compression";
		val.Parent = "server";
		val.FullName = "server.compression";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.compression.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.compression = StringExtensions.ToBool(str);
		};
		array[654] = val;
		val = new Command();
		val.Name = "conveyormovefrequency";
		val.Parent = "server";
		val.FullName = "server.conveyormovefrequency";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How often industrial conveyors attempt to move items (value is an interval measured in seconds). Setting to 0 will disable all movement";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.conveyorMoveFrequency.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.conveyorMoveFrequency = StringExtensions.ToFloat(str, 0f);
		};
		array[655] = val;
		val = new Command();
		val.Name = "corpsedespawn";
		val.Parent = "server";
		val.FullName = "server.corpsedespawn";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.corpsedespawn.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.corpsedespawn = StringExtensions.ToFloat(str, 0f);
		};
		array[656] = val;
		val = new Command();
		val.Name = "corpses";
		val.Parent = "server";
		val.FullName = "server.corpses";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.corpses.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.corpses = StringExtensions.ToBool(str);
		};
		array[657] = val;
		val = new Command();
		val.Name = "crawlingenabled";
		val.Parent = "server";
		val.FullName = "server.crawlingenabled";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Do players go into the crawling wounded state";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.crawlingenabled.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.crawlingenabled = StringExtensions.ToBool(str);
		};
		array[658] = val;
		val = new Command();
		val.Name = "crawlingmaximumhealth";
		val.Parent = "server";
		val.FullName = "server.crawlingmaximumhealth";
		val.ServerAdmin = true;
		val.Description = "Maximum initial health given when a player dies and moves to crawling wounded state";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.crawlingmaximumhealth.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.crawlingmaximumhealth = StringExtensions.ToInt(str, 0);
		};
		array[659] = val;
		val = new Command();
		val.Name = "crawlingminimumhealth";
		val.Parent = "server";
		val.FullName = "server.crawlingminimumhealth";
		val.ServerAdmin = true;
		val.Description = "Minimum initial health given when a player dies and moves to crawling wounded state";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.crawlingminimumhealth.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.crawlingminimumhealth = StringExtensions.ToInt(str, 0);
		};
		array[660] = val;
		val = new Command();
		val.Name = "cycletime";
		val.Parent = "server";
		val.FullName = "server.cycletime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.cycletime.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.cycletime = StringExtensions.ToFloat(str, 0f);
		};
		array[661] = val;
		val = new Command();
		val.Name = "debrisdespawn";
		val.Parent = "server";
		val.FullName = "server.debrisdespawn";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.debrisdespawn.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.debrisdespawn = StringExtensions.ToFloat(str, 0f);
		};
		array[662] = val;
		val = new Command();
		val.Name = "defaultblueprintresearchcost";
		val.Parent = "server";
		val.FullName = "server.defaultblueprintresearchcost";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "How much scrap is required to research default blueprints";
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.defaultBlueprintResearchCost.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.defaultBlueprintResearchCost = StringExtensions.ToInt(str, 0);
		};
		val.Default = "10";
		array[663] = val;
		val = new Command();
		val.Name = "description";
		val.Parent = "server";
		val.FullName = "server.description";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.description.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.description = str;
		};
		array[664] = val;
		val = new Command();
		val.Name = "dropitems";
		val.Parent = "server";
		val.FullName = "server.dropitems";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.dropitems.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.dropitems = StringExtensions.ToBool(str);
		};
		array[665] = val;
		val = new Command();
		val.Name = "encryption";
		val.Parent = "server";
		val.FullName = "server.encryption";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.encryption.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.encryption = StringExtensions.ToInt(str, 0);
		};
		array[666] = val;
		val = new Command();
		val.Name = "enforcepipechecksonbuildingblockchanges";
		val.Parent = "server";
		val.FullName = "server.enforcepipechecksonbuildingblockchanges";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Whether to check for illegal industrial pipes when changing building block states (roof bunkers)";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.enforcePipeChecksOnBuildingBlockChanges.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.enforcePipeChecksOnBuildingBlockChanges = StringExtensions.ToBool(str);
		};
		array[667] = val;
		val = new Command();
		val.Name = "entitybatchsize";
		val.Parent = "server";
		val.FullName = "server.entitybatchsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.entitybatchsize.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.entitybatchsize = StringExtensions.ToInt(str, 0);
		};
		array[668] = val;
		val = new Command();
		val.Name = "entitybatchtime";
		val.Parent = "server";
		val.FullName = "server.entitybatchtime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.entitybatchtime.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.entitybatchtime = StringExtensions.ToFloat(str, 0f);
		};
		array[669] = val;
		val = new Command();
		val.Name = "entityrate";
		val.Parent = "server";
		val.FullName = "server.entityrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.entityrate.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.entityrate = StringExtensions.ToInt(str, 0);
		};
		array[670] = val;
		val = new Command();
		val.Name = "events";
		val.Parent = "server";
		val.FullName = "server.events";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.events.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.events = StringExtensions.ToBool(str);
		};
		array[671] = val;
		val = new Command();
		val.Name = "fps";
		val.Parent = "server";
		val.FullName = "server.fps";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.fps(arg);
		};
		array[672] = val;
		val = new Command();
		val.Name = "funwaterdamagethreshold";
		val.Parent = "server";
		val.FullName = "server.funwaterdamagethreshold";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.funWaterDamageThreshold.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.funWaterDamageThreshold = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0.8";
		array[673] = val;
		val = new Command();
		val.Name = "funwaterwetnessgain";
		val.Parent = "server";
		val.FullName = "server.funwaterwetnessgain";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.funWaterWetnessGain.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.funWaterWetnessGain = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0.05";
		array[674] = val;
		val = new Command();
		val.Name = "gamemode";
		val.Parent = "server";
		val.FullName = "server.gamemode";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.gamemode.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.gamemode = str;
		};
		array[675] = val;
		val = new Command();
		val.Name = "headerimage";
		val.Parent = "server";
		val.FullName = "server.headerimage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.headerimage.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.headerimage = str;
		};
		array[676] = val;
		val = new Command();
		val.Name = "hostname";
		val.Parent = "server";
		val.FullName = "server.hostname";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.hostname.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.hostname = str;
		};
		array[677] = val;
		val = new Command();
		val.Name = "identity";
		val.Parent = "server";
		val.FullName = "server.identity";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.identity.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.identity = str;
		};
		array[678] = val;
		val = new Command();
		val.Name = "idlekick";
		val.Parent = "server";
		val.FullName = "server.idlekick";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.idlekick.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.idlekick = StringExtensions.ToInt(str, 0);
		};
		array[679] = val;
		val = new Command();
		val.Name = "idlekickadmins";
		val.Parent = "server";
		val.FullName = "server.idlekickadmins";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.idlekickadmins.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.idlekickadmins = StringExtensions.ToInt(str, 0);
		};
		array[680] = val;
		val = new Command();
		val.Name = "idlekickmode";
		val.Parent = "server";
		val.FullName = "server.idlekickmode";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.idlekickmode.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.idlekickmode = StringExtensions.ToInt(str, 0);
		};
		array[681] = val;
		val = new Command();
		val.Name = "incapacitatedrecoverchance";
		val.Parent = "server";
		val.FullName = "server.incapacitatedrecoverchance";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Base chance of recovery after incapacitated wounded state";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.incapacitatedrecoverchance.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.incapacitatedrecoverchance = StringExtensions.ToFloat(str, 0f);
		};
		array[682] = val;
		val = new Command();
		val.Name = "industrialcrafterfrequency";
		val.Parent = "server";
		val.FullName = "server.industrialcrafterfrequency";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How often industrial crafters attempt to craft items (value is an interval measured in seconds). Setting to 0 will disable all crafting";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.industrialCrafterFrequency.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.industrialCrafterFrequency = StringExtensions.ToFloat(str, 0f);
		};
		array[683] = val;
		val = new Command();
		val.Name = "industrialframebudgetms";
		val.Parent = "server";
		val.FullName = "server.industrialframebudgetms";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How long per frame to spend on industrial jobs";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.industrialFrameBudgetMs.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.industrialFrameBudgetMs = StringExtensions.ToFloat(str, 0f);
		};
		array[684] = val;
		val = new Command();
		val.Name = "ip";
		val.Parent = "server";
		val.FullName = "server.ip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.ip.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.ip = str;
		};
		array[685] = val;
		val = new Command();
		val.Name = "ipqueriespermin";
		val.Parent = "server";
		val.FullName = "server.ipqueriespermin";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.ipQueriesPerMin.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.ipQueriesPerMin = StringExtensions.ToInt(str, 0);
		};
		array[686] = val;
		val = new Command();
		val.Name = "itemdespawn";
		val.Parent = "server";
		val.FullName = "server.itemdespawn";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.itemdespawn.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.itemdespawn = StringExtensions.ToFloat(str, 0f);
		};
		array[687] = val;
		val = new Command();
		val.Name = "itemdespawn_container_scale";
		val.Parent = "server";
		val.FullName = "server.itemdespawn_container_scale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.itemdespawn_container_scale.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.itemdespawn_container_scale = StringExtensions.ToFloat(str, 0f);
		};
		array[688] = val;
		val = new Command();
		val.Name = "itemdespawn_quick";
		val.Parent = "server";
		val.FullName = "server.itemdespawn_quick";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.itemdespawn_quick.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.itemdespawn_quick = StringExtensions.ToFloat(str, 0f);
		};
		array[689] = val;
		val = new Command();
		val.Name = "level";
		val.Parent = "server";
		val.FullName = "server.level";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.level.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.level = str;
		};
		array[690] = val;
		val = new Command();
		val.Name = "leveltransfer";
		val.Parent = "server";
		val.FullName = "server.leveltransfer";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.leveltransfer.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.leveltransfer = StringExtensions.ToBool(str);
		};
		array[691] = val;
		val = new Command();
		val.Name = "levelurl";
		val.Parent = "server";
		val.FullName = "server.levelurl";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.levelurl.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.levelurl = str;
		};
		array[692] = val;
		val = new Command();
		val.Name = "listtoolcupboards";
		val.Parent = "server";
		val.FullName = "server.listtoolcupboards";
		val.ServerAdmin = true;
		val.Description = "Prints all the Tool Cupboards on the server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.listtoolcupboards(arg);
		};
		array[693] = val;
		val = new Command();
		val.Name = "listvendingmachines";
		val.Parent = "server";
		val.FullName = "server.listvendingmachines";
		val.ServerAdmin = true;
		val.Description = "Prints all the vending machines on the server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.listvendingmachines(arg);
		};
		array[694] = val;
		val = new Command();
		val.Name = "logoimage";
		val.Parent = "server";
		val.FullName = "server.logoimage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.logoimage.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.logoimage = str;
		};
		array[695] = val;
		val = new Command();
		val.Name = "max_sleeping_bags";
		val.Parent = "server";
		val.FullName = "server.max_sleeping_bags";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.max_sleeping_bags.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.max_sleeping_bags = StringExtensions.ToInt(str, 0);
		};
		val.Default = "15";
		array[696] = val;
		val = new Command();
		val.Name = "maxclientinfosize";
		val.Parent = "server";
		val.FullName = "server.maxclientinfosize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxclientinfosize.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxclientinfosize = StringExtensions.ToInt(str, 0);
		};
		array[697] = val;
		val = new Command();
		val.Name = "maxconnectionsperip";
		val.Parent = "server";
		val.FullName = "server.maxconnectionsperip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxconnectionsperip.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxconnectionsperip = StringExtensions.ToInt(str, 0);
		};
		array[698] = val;
		val = new Command();
		val.Name = "maxdecryptqueuebytes";
		val.Parent = "server";
		val.FullName = "server.maxdecryptqueuebytes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxdecryptqueuebytes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxdecryptqueuebytes = StringExtensions.ToInt(str, 0);
		};
		array[699] = val;
		val = new Command();
		val.Name = "maxdecryptqueuelength";
		val.Parent = "server";
		val.FullName = "server.maxdecryptqueuelength";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxdecryptqueuelength.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxdecryptqueuelength = StringExtensions.ToInt(str, 0);
		};
		array[700] = val;
		val = new Command();
		val.Name = "maxdecryptthreadwait";
		val.Parent = "server";
		val.FullName = "server.maxdecryptthreadwait";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxdecryptthreadwait.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxdecryptthreadwait = StringExtensions.ToInt(str, 0);
		};
		array[701] = val;
		val = new Command();
		val.Name = "maximummapmarkers";
		val.Parent = "server";
		val.FullName = "server.maximummapmarkers";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "How many markers each player can place";
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maximumMapMarkers.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maximumMapMarkers = StringExtensions.ToInt(str, 0);
		};
		val.Default = "5";
		array[702] = val;
		val = new Command();
		val.Name = "maximumpings";
		val.Parent = "server";
		val.FullName = "server.maximumpings";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How many pings can be placed by each player";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maximumPings.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maximumPings = StringExtensions.ToInt(str, 0);
		};
		array[703] = val;
		val = new Command();
		val.Name = "maxitemstacksmovedpertickindustrial";
		val.Parent = "server";
		val.FullName = "server.maxitemstacksmovedpertickindustrial";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How many stacks a single conveyor can move in a single tick";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxItemStacksMovedPerTickIndustrial.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxItemStacksMovedPerTickIndustrial = StringExtensions.ToInt(str, 0);
		};
		array[704] = val;
		val = new Command();
		val.Name = "maxmainthreadwait";
		val.Parent = "server";
		val.FullName = "server.maxmainthreadwait";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxmainthreadwait.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxmainthreadwait = StringExtensions.ToInt(str, 0);
		};
		array[705] = val;
		val = new Command();
		val.Name = "maxpacketsize_command";
		val.Parent = "server";
		val.FullName = "server.maxpacketsize_command";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxpacketsize_command.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxpacketsize_command = StringExtensions.ToInt(str, 0);
		};
		array[706] = val;
		val = new Command();
		val.Name = "maxpacketsize_globalentities";
		val.Parent = "server";
		val.FullName = "server.maxpacketsize_globalentities";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxpacketsize_globalentities.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxpacketsize_globalentities = StringExtensions.ToInt(str, 0);
		};
		array[707] = val;
		val = new Command();
		val.Name = "maxpacketsize_globaltrees";
		val.Parent = "server";
		val.FullName = "server.maxpacketsize_globaltrees";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxpacketsize_globaltrees.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxpacketsize_globaltrees = StringExtensions.ToInt(str, 0);
		};
		array[708] = val;
		val = new Command();
		val.Name = "maxpacketspersecond";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxpacketspersecond.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxpacketspersecond = StringExtensions.ToInt(str, 0);
		};
		array[709] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_command";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_command";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxpacketspersecond_command.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxpacketspersecond_command = StringExtensions.ToInt(str, 0);
		};
		array[710] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_rpc";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_rpc";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxpacketspersecond_rpc.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxpacketspersecond_rpc = StringExtensions.ToInt(str, 0);
		};
		array[711] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_rpc_signal";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_rpc_signal";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxpacketspersecond_rpc_signal.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxpacketspersecond_rpc_signal = StringExtensions.ToInt(str, 0);
		};
		array[712] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_tick";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_tick";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxpacketspersecond_tick.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxpacketspersecond_tick = StringExtensions.ToInt(str, 0);
		};
		array[713] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_voice";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_voice";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxpacketspersecond_voice.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxpacketspersecond_voice = StringExtensions.ToInt(str, 0);
		};
		array[714] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_world";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_world";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxpacketspersecond_world.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxpacketspersecond_world = StringExtensions.ToInt(str, 0);
		};
		array[715] = val;
		val = new Command();
		val.Name = "maxplayers";
		val.Parent = "server";
		val.FullName = "server.maxplayers";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxplayers.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxplayers = StringExtensions.ToInt(str, 0);
		};
		array[716] = val;
		val = new Command();
		val.Name = "maxreadqueuebytes";
		val.Parent = "server";
		val.FullName = "server.maxreadqueuebytes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxreadqueuebytes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxreadqueuebytes = StringExtensions.ToInt(str, 0);
		};
		array[717] = val;
		val = new Command();
		val.Name = "maxreadqueuelength";
		val.Parent = "server";
		val.FullName = "server.maxreadqueuelength";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxreadqueuelength.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxreadqueuelength = StringExtensions.ToInt(str, 0);
		};
		array[718] = val;
		val = new Command();
		val.Name = "maxreadthreadwait";
		val.Parent = "server";
		val.FullName = "server.maxreadthreadwait";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxreadthreadwait.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxreadthreadwait = StringExtensions.ToInt(str, 0);
		};
		array[719] = val;
		val = new Command();
		val.Name = "maxreceivetime";
		val.Parent = "server";
		val.FullName = "server.maxreceivetime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxreceivetime.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxreceivetime = StringExtensions.ToInt(str, 0);
		};
		array[720] = val;
		val = new Command();
		val.Name = "maxunack";
		val.Parent = "server";
		val.FullName = "server.maxunack";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxunack.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxunack = StringExtensions.ToInt(str, 0);
		};
		array[721] = val;
		val = new Command();
		val.Name = "maxwritequeuebytes";
		val.Parent = "server";
		val.FullName = "server.maxwritequeuebytes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxwritequeuebytes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxwritequeuebytes = StringExtensions.ToInt(str, 0);
		};
		array[722] = val;
		val = new Command();
		val.Name = "maxwritequeuelength";
		val.Parent = "server";
		val.FullName = "server.maxwritequeuelength";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxwritequeuelength.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxwritequeuelength = StringExtensions.ToInt(str, 0);
		};
		array[723] = val;
		val = new Command();
		val.Name = "maxwritethreadwait";
		val.Parent = "server";
		val.FullName = "server.maxwritethreadwait";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.maxwritethreadwait.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.maxwritethreadwait = StringExtensions.ToInt(str, 0);
		};
		array[724] = val;
		val = new Command();
		val.Name = "meleearmor";
		val.Parent = "server";
		val.FullName = "server.meleearmor";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.meleearmor.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.meleearmor = StringExtensions.ToFloat(str, 0f);
		};
		array[725] = val;
		val = new Command();
		val.Name = "meleedamage";
		val.Parent = "server";
		val.FullName = "server.meleedamage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.meleedamage.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.meleedamage = StringExtensions.ToFloat(str, 0f);
		};
		array[726] = val;
		val = new Command();
		val.Name = "metabolismtick";
		val.Parent = "server";
		val.FullName = "server.metabolismtick";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.metabolismtick.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.metabolismtick = StringExtensions.ToFloat(str, 0f);
		};
		array[727] = val;
		val = new Command();
		val.Name = "modifiertickrate";
		val.Parent = "server";
		val.FullName = "server.modifiertickrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.modifierTickRate.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.modifierTickRate = StringExtensions.ToFloat(str, 0f);
		};
		array[728] = val;
		val = new Command();
		val.Name = "motd";
		val.Parent = "server";
		val.FullName = "server.motd";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.motd.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.motd = str;
		};
		val.Default = "";
		array[729] = val;
		val = new Command();
		val.Name = "netcache";
		val.Parent = "server";
		val.FullName = "server.netcache";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.netcache.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.netcache = StringExtensions.ToBool(str);
		};
		array[730] = val;
		val = new Command();
		val.Name = "netcachesize";
		val.Parent = "server";
		val.FullName = "server.netcachesize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.netcachesize.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.netcachesize = StringExtensions.ToInt(str, 0);
		};
		array[731] = val;
		val = new Command();
		val.Name = "netlog";
		val.Parent = "server";
		val.FullName = "server.netlog";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.netlog.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.netlog = StringExtensions.ToBool(str);
		};
		array[732] = val;
		val = new Command();
		val.Name = "netprotocol";
		val.Parent = "server";
		val.FullName = "server.netprotocol";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text11 = ConVar.Server.netprotocol(arg);
			arg.ReplyWithObject((object)text11);
		};
		array[733] = val;
		val = new Command();
		val.Name = "nonplanterdeathchancepertick";
		val.Parent = "server";
		val.FullName = "server.nonplanterdeathchancepertick";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.nonPlanterDeathChancePerTick.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.nonPlanterDeathChancePerTick = StringExtensions.ToFloat(str, 0f);
		};
		array[734] = val;
		val = new Command();
		val.Name = "official";
		val.Parent = "server";
		val.FullName = "server.official";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.official.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.official = StringExtensions.ToBool(str);
		};
		array[735] = val;
		val = new Command();
		val.Name = "optimalplanterqualitysaturation";
		val.Parent = "server";
		val.FullName = "server.optimalplanterqualitysaturation";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.optimalPlanterQualitySaturation.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.optimalPlanterQualitySaturation = StringExtensions.ToFloat(str, 0f);
		};
		array[736] = val;
		val = new Command();
		val.Name = "packetlog";
		val.Parent = "server";
		val.FullName = "server.packetlog";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text10 = ConVar.Server.packetlog(arg);
			arg.ReplyWithObject((object)text10);
		};
		array[737] = val;
		val = new Command();
		val.Name = "packetlog_enabled";
		val.Parent = "server";
		val.FullName = "server.packetlog_enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.packetlog_enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.packetlog_enabled = StringExtensions.ToBool(str);
		};
		array[738] = val;
		val = new Command();
		val.Name = "parachuterepacktime";
		val.Parent = "server";
		val.FullName = "server.parachuterepacktime";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "How long it takes to pick up a used parachute in seconds";
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.parachuteRepackTime.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.parachuteRepackTime = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "8";
		array[739] = val;
		val = new Command();
		val.Name = "pingduration";
		val.Parent = "server";
		val.FullName = "server.pingduration";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How long a ping should last";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.pingDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.pingDuration = StringExtensions.ToFloat(str, 0f);
		};
		array[740] = val;
		val = new Command();
		val.Name = "plantlightdetection";
		val.Parent = "server";
		val.FullName = "server.plantlightdetection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.plantlightdetection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.plantlightdetection = StringExtensions.ToBool(str);
		};
		array[741] = val;
		val = new Command();
		val.Name = "planttick";
		val.Parent = "server";
		val.FullName = "server.planttick";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.planttick.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.planttick = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "60";
		array[742] = val;
		val = new Command();
		val.Name = "planttickscale";
		val.Parent = "server";
		val.FullName = "server.planttickscale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.planttickscale.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.planttickscale = StringExtensions.ToFloat(str, 0f);
		};
		array[743] = val;
		val = new Command();
		val.Name = "playerlistpos";
		val.Parent = "server";
		val.FullName = "server.playerlistpos";
		val.ServerAdmin = true;
		val.Description = "Prints the position of all players on the server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.playerlistpos(arg);
		};
		array[744] = val;
		val = new Command();
		val.Name = "playerserverfall";
		val.Parent = "server";
		val.FullName = "server.playerserverfall";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.playerserverfall.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.playerserverfall = StringExtensions.ToBool(str);
		};
		array[745] = val;
		val = new Command();
		val.Name = "playertimeout";
		val.Parent = "server";
		val.FullName = "server.playertimeout";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.playertimeout.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.playertimeout = StringExtensions.ToInt(str, 0);
		};
		array[746] = val;
		val = new Command();
		val.Name = "port";
		val.Parent = "server";
		val.FullName = "server.port";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.port.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.port = StringExtensions.ToInt(str, 0);
		};
		array[747] = val;
		val = new Command();
		val.Name = "printdecryptqueue";
		val.Parent = "server";
		val.FullName = "server.printdecryptqueue";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text9 = ConVar.Server.printdecryptqueue(arg);
			arg.ReplyWithObject((object)text9);
		};
		array[748] = val;
		val = new Command();
		val.Name = "printeyes";
		val.Parent = "server";
		val.FullName = "server.printeyes";
		val.ServerAdmin = true;
		val.Description = "Print the current player eyes.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text8 = ConVar.Server.printeyes(arg);
			arg.ReplyWithObject((object)text8);
		};
		array[749] = val;
		val = new Command();
		val.Name = "printpos";
		val.Parent = "server";
		val.FullName = "server.printpos";
		val.ServerAdmin = true;
		val.Description = "Print the current player position.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text7 = ConVar.Server.printpos(arg);
			arg.ReplyWithObject((object)text7);
		};
		array[750] = val;
		val = new Command();
		val.Name = "printreadqueue";
		val.Parent = "server";
		val.FullName = "server.printreadqueue";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text6 = ConVar.Server.printreadqueue(arg);
			arg.ReplyWithObject((object)text6);
		};
		array[751] = val;
		val = new Command();
		val.Name = "printreportstoconsole";
		val.Parent = "server";
		val.FullName = "server.printreportstoconsole";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Should F7 reports from players be printed to console";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.printReportsToConsole.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.printReportsToConsole = StringExtensions.ToBool(str);
		};
		array[752] = val;
		val = new Command();
		val.Name = "printrot";
		val.Parent = "server";
		val.FullName = "server.printrot";
		val.ServerAdmin = true;
		val.Description = "Print the current player rotation.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text5 = ConVar.Server.printrot(arg);
			arg.ReplyWithObject((object)text5);
		};
		array[753] = val;
		val = new Command();
		val.Name = "printwritequeue";
		val.Parent = "server";
		val.FullName = "server.printwritequeue";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text4 = ConVar.Server.printwritequeue(arg);
			arg.ReplyWithObject((object)text4);
		};
		array[754] = val;
		val = new Command();
		val.Name = "pve";
		val.Parent = "server";
		val.FullName = "server.pve";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.pve.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.pve = StringExtensions.ToBool(str);
		};
		array[755] = val;
		val = new Command();
		val.Name = "queriespersecond";
		val.Parent = "server";
		val.FullName = "server.queriespersecond";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.queriesPerSecond.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.queriesPerSecond = StringExtensions.ToInt(str, 0);
		};
		array[756] = val;
		val = new Command();
		val.Name = "queryport";
		val.Parent = "server";
		val.FullName = "server.queryport";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.queryport.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.queryport = StringExtensions.ToInt(str, 0);
		};
		array[757] = val;
		val = new Command();
		val.Name = "radiation";
		val.Parent = "server";
		val.FullName = "server.radiation";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.radiation.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.radiation = StringExtensions.ToBool(str);
		};
		array[758] = val;
		val = new Command();
		val.Name = "readcfg";
		val.Parent = "server";
		val.FullName = "server.readcfg";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text3 = ConVar.Server.readcfg(arg);
			arg.ReplyWithObject((object)text3);
		};
		array[759] = val;
		val = new Command();
		val.Name = "reportsserverendpoint";
		val.Parent = "server";
		val.FullName = "server.reportsserverendpoint";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "HTTP API endpoint for receiving F7 reports";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.reportsServerEndpoint.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.reportsServerEndpoint = str;
		};
		array[760] = val;
		val = new Command();
		val.Name = "reportsserverendpointkey";
		val.Parent = "server";
		val.FullName = "server.reportsserverendpointkey";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "If set, this key will be included with any reports sent via reportsServerEndpoint (for validation)";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.reportsServerEndpointKey.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.reportsServerEndpointKey = str;
		};
		array[761] = val;
		val = new Command();
		val.Name = "resetserveremoji";
		val.Parent = "server";
		val.FullName = "server.resetserveremoji";
		val.ServerAdmin = true;
		val.Description = "Rescans the serveremoji folder, note that clients will need to reconnect to get the latest emoji";
		val.Variable = false;
		val.Call = delegate
		{
			ConVar.Server.ResetServerEmoji();
		};
		array[762] = val;
		val = new Command();
		val.Name = "respawnatdeathposition";
		val.Parent = "server";
		val.FullName = "server.respawnatdeathposition";
		val.ServerAdmin = true;
		val.Description = "If a player presses the respawn button, respawn at their death location (for trailer filming)";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.respawnAtDeathPosition.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.respawnAtDeathPosition = StringExtensions.ToBool(str);
		};
		array[763] = val;
		val = new Command();
		val.Name = "respawnresetrange";
		val.Parent = "server";
		val.FullName = "server.respawnresetrange";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.respawnresetrange.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.respawnresetrange = StringExtensions.ToFloat(str, 0f);
		};
		array[764] = val;
		val = new Command();
		val.Name = "respawnwithloadout";
		val.Parent = "server";
		val.FullName = "server.respawnwithloadout";
		val.ServerAdmin = true;
		val.Description = "When a player respawns give them the loadout assigned to client.RespawnLoadout (created with inventory.saveloadout)";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.respawnWithLoadout.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.respawnWithLoadout = StringExtensions.ToBool(str);
		};
		array[765] = val;
		val = new Command();
		val.Name = "rewounddelay";
		val.Parent = "server";
		val.FullName = "server.rewounddelay";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.rewounddelay.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.rewounddelay = StringExtensions.ToFloat(str, 0f);
		};
		array[766] = val;
		val = new Command();
		val.Name = "rpclog";
		val.Parent = "server";
		val.FullName = "server.rpclog";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text2 = ConVar.Server.rpclog(arg);
			arg.ReplyWithObject((object)text2);
		};
		array[767] = val;
		val = new Command();
		val.Name = "rpclog_enabled";
		val.Parent = "server";
		val.FullName = "server.rpclog_enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.rpclog_enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.rpclog_enabled = StringExtensions.ToBool(str);
		};
		array[768] = val;
		val = new Command();
		val.Name = "salt";
		val.Parent = "server";
		val.FullName = "server.salt";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.salt.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.salt = StringExtensions.ToInt(str, 0);
		};
		array[769] = val;
		val = new Command();
		val.Name = "save";
		val.Parent = "server";
		val.FullName = "server.save";
		val.ServerAdmin = true;
		val.Description = "Force save the current game";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.save(arg);
		};
		array[770] = val;
		val = new Command();
		val.Name = "savebackupcount";
		val.Parent = "server";
		val.FullName = "server.savebackupcount";
		val.ServerAdmin = true;
		val.Saved = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.saveBackupCount.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.saveBackupCount = StringExtensions.ToInt(str, 0);
		};
		array[771] = val;
		val = new Command();
		val.Name = "savecachesize";
		val.Parent = "server";
		val.FullName = "server.savecachesize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.savecachesize.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.savecachesize = StringExtensions.ToInt(str, 0);
		};
		array[772] = val;
		val = new Command();
		val.Name = "saveinterval";
		val.Parent = "server";
		val.FullName = "server.saveinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.saveinterval.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.saveinterval = StringExtensions.ToInt(str, 0);
		};
		array[773] = val;
		val = new Command();
		val.Name = "schematime";
		val.Parent = "server";
		val.FullName = "server.schematime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.schematime.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.schematime = StringExtensions.ToFloat(str, 0f);
		};
		array[774] = val;
		val = new Command();
		val.Name = "secure";
		val.Parent = "server";
		val.FullName = "server.secure";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.secure.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.secure = StringExtensions.ToBool(str);
		};
		array[775] = val;
		val = new Command();
		val.Name = "seed";
		val.Parent = "server";
		val.FullName = "server.seed";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.seed.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.seed = StringExtensions.ToInt(str, 0);
		};
		array[776] = val;
		val = new Command();
		val.Name = "sendnetworkupdate";
		val.Parent = "server";
		val.FullName = "server.sendnetworkupdate";
		val.ServerAdmin = true;
		val.Description = "Send network update for all players";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.sendnetworkupdate(arg);
		};
		array[777] = val;
		val = new Command();
		val.Name = "setshowholstereditems";
		val.Parent = "server";
		val.FullName = "server.setshowholstereditems";
		val.ServerAdmin = true;
		val.Description = "Show holstered items on player bodies";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.setshowholstereditems(arg);
		};
		array[778] = val;
		val = new Command();
		val.Name = "showholstereditems";
		val.Parent = "server";
		val.FullName = "server.showholstereditems";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.showHolsteredItems.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.showHolsteredItems = StringExtensions.ToBool(str);
		};
		array[779] = val;
		val = new Command();
		val.Name = "snapshot";
		val.Parent = "server";
		val.FullName = "server.snapshot";
		val.ServerAdmin = true;
		val.Description = "This sends a snapshot of all the entities in the client's pvs. This is mostly redundant, but we request this when the client starts recording a demo.. so they get all the information.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.snapshot(arg);
		};
		array[780] = val;
		val = new Command();
		val.Name = "sprinklereyeheightoffset";
		val.Parent = "server";
		val.FullName = "server.sprinklereyeheightoffset";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.sprinklerEyeHeightOffset.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.sprinklerEyeHeightOffset = StringExtensions.ToFloat(str, 0f);
		};
		array[781] = val;
		val = new Command();
		val.Name = "sprinklerradius";
		val.Parent = "server";
		val.FullName = "server.sprinklerradius";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.sprinklerRadius.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.sprinklerRadius = StringExtensions.ToFloat(str, 0f);
		};
		array[782] = val;
		val = new Command();
		val.Name = "stability";
		val.Parent = "server";
		val.FullName = "server.stability";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.stability.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.stability = StringExtensions.ToBool(str);
		};
		array[783] = val;
		val = new Command();
		val.Name = "start";
		val.Parent = "server";
		val.FullName = "server.start";
		val.ServerAdmin = true;
		val.Description = "Starts a server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.start(arg);
		};
		array[784] = val;
		val = new Command();
		val.Name = "statbackup";
		val.Parent = "server";
		val.FullName = "server.statbackup";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.statBackup.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.statBackup = StringExtensions.ToBool(str);
		};
		array[785] = val;
		val = new Command();
		val.Name = "stats";
		val.Parent = "server";
		val.FullName = "server.stats";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.stats.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.stats = StringExtensions.ToBool(str);
		};
		array[786] = val;
		val = new Command();
		val.Name = "stop";
		val.Parent = "server";
		val.FullName = "server.stop";
		val.ServerAdmin = true;
		val.Description = "Stops a server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.stop(arg);
		};
		array[787] = val;
		val = new Command();
		val.Name = "tags";
		val.Parent = "server";
		val.FullName = "server.tags";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Comma-separated server browser tag values (see wiki)";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.tags.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.tags = str;
		};
		array[788] = val;
		val = new Command();
		val.Name = "tickrate";
		val.Parent = "server";
		val.FullName = "server.tickrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.tickrate.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.tickrate = StringExtensions.ToInt(str, 0);
		};
		array[789] = val;
		val = new Command();
		val.Name = "updatebatch";
		val.Parent = "server";
		val.FullName = "server.updatebatch";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.updatebatch.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.updatebatch = StringExtensions.ToInt(str, 0);
		};
		array[790] = val;
		val = new Command();
		val.Name = "updatebatchspawn";
		val.Parent = "server";
		val.FullName = "server.updatebatchspawn";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.updatebatchspawn.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.updatebatchspawn = StringExtensions.ToInt(str, 0);
		};
		array[791] = val;
		val = new Command();
		val.Name = "url";
		val.Parent = "server";
		val.FullName = "server.url";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.url.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.url = str;
		};
		array[792] = val;
		val = new Command();
		val.Name = "useminimumplantcondition";
		val.Parent = "server";
		val.FullName = "server.useminimumplantcondition";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.useMinimumPlantCondition.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.useMinimumPlantCondition = StringExtensions.ToBool(str);
		};
		array[793] = val;
		val = new Command();
		val.Name = "watercontainersleavewaterbehind";
		val.Parent = "server";
		val.FullName = "server.watercontainersleavewaterbehind";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "When transferring water, should containers keep 1 water behind. Enabling this should help performance if water IO is causing performance loss";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.waterContainersLeaveWaterBehind.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.waterContainersLeaveWaterBehind = StringExtensions.ToBool(str);
		};
		array[794] = val;
		val = new Command();
		val.Name = "worldsize";
		val.Parent = "server";
		val.FullName = "server.worldsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.worldsize.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.worldsize = StringExtensions.ToInt(str, 0);
		};
		array[795] = val;
		val = new Command();
		val.Name = "woundedmaxfoodandwaterbonus";
		val.Parent = "server";
		val.FullName = "server.woundedmaxfoodandwaterbonus";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Maximum percent chance added to base wounded/incapacitated recovery chance, based on the player's food and water level";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.woundedmaxfoodandwaterbonus.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.woundedmaxfoodandwaterbonus = StringExtensions.ToFloat(str, 0f);
		};
		array[796] = val;
		val = new Command();
		val.Name = "woundedrecoverchance";
		val.Parent = "server";
		val.FullName = "server.woundedrecoverchance";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Base chance of recovery after crawling wounded state";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.woundedrecoverchance.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.woundedrecoverchance = StringExtensions.ToFloat(str, 0f);
		};
		array[797] = val;
		val = new Command();
		val.Name = "woundingenabled";
		val.Parent = "server";
		val.FullName = "server.woundingenabled";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Can players be wounded after recieving fatal damage";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.woundingenabled.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.woundingenabled = StringExtensions.ToBool(str);
		};
		array[798] = val;
		val = new Command();
		val.Name = "writecfg";
		val.Parent = "server";
		val.FullName = "server.writecfg";
		val.ServerAdmin = true;
		val.Description = "Writes config files";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.writecfg(arg);
		};
		array[799] = val;
		val = new Command();
		val.Name = "cargoshipevent";
		val.Parent = "spawn";
		val.FullName = "spawn.cargoshipevent";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.cargoshipevent(arg);
		};
		array[800] = val;
		val = new Command();
		val.Name = "fill_groups";
		val.Parent = "spawn";
		val.FullName = "spawn.fill_groups";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.fill_groups(arg);
		};
		array[801] = val;
		val = new Command();
		val.Name = "fill_individuals";
		val.Parent = "spawn";
		val.FullName = "spawn.fill_individuals";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.fill_individuals(arg);
		};
		array[802] = val;
		val = new Command();
		val.Name = "fill_populations";
		val.Parent = "spawn";
		val.FullName = "spawn.fill_populations";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.fill_populations(arg);
		};
		array[803] = val;
		val = new Command();
		val.Name = "max_density";
		val.Parent = "spawn";
		val.FullName = "spawn.max_density";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.max_density.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.max_density = StringExtensions.ToFloat(str, 0f);
		};
		array[804] = val;
		val = new Command();
		val.Name = "max_rate";
		val.Parent = "spawn";
		val.FullName = "spawn.max_rate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.max_rate.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.max_rate = StringExtensions.ToFloat(str, 0f);
		};
		array[805] = val;
		val = new Command();
		val.Name = "min_density";
		val.Parent = "spawn";
		val.FullName = "spawn.min_density";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.min_density.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.min_density = StringExtensions.ToFloat(str, 0f);
		};
		array[806] = val;
		val = new Command();
		val.Name = "min_rate";
		val.Parent = "spawn";
		val.FullName = "spawn.min_rate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.min_rate.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.min_rate = StringExtensions.ToFloat(str, 0f);
		};
		array[807] = val;
		val = new Command();
		val.Name = "player_base";
		val.Parent = "spawn";
		val.FullName = "spawn.player_base";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.player_base.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.player_base = StringExtensions.ToFloat(str, 0f);
		};
		array[808] = val;
		val = new Command();
		val.Name = "player_scale";
		val.Parent = "spawn";
		val.FullName = "spawn.player_scale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.player_scale.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.player_scale = StringExtensions.ToFloat(str, 0f);
		};
		array[809] = val;
		val = new Command();
		val.Name = "report";
		val.Parent = "spawn";
		val.FullName = "spawn.report";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.report(arg);
		};
		array[810] = val;
		val = new Command();
		val.Name = "respawn_groups";
		val.Parent = "spawn";
		val.FullName = "spawn.respawn_groups";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.respawn_groups.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.respawn_groups = StringExtensions.ToBool(str);
		};
		array[811] = val;
		val = new Command();
		val.Name = "respawn_individuals";
		val.Parent = "spawn";
		val.FullName = "spawn.respawn_individuals";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.respawn_individuals.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.respawn_individuals = StringExtensions.ToBool(str);
		};
		array[812] = val;
		val = new Command();
		val.Name = "respawn_populations";
		val.Parent = "spawn";
		val.FullName = "spawn.respawn_populations";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.respawn_populations.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.respawn_populations = StringExtensions.ToBool(str);
		};
		array[813] = val;
		val = new Command();
		val.Name = "scalars";
		val.Parent = "spawn";
		val.FullName = "spawn.scalars";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.scalars(arg);
		};
		array[814] = val;
		val = new Command();
		val.Name = "tick_individuals";
		val.Parent = "spawn";
		val.FullName = "spawn.tick_individuals";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.tick_individuals.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.tick_individuals = StringExtensions.ToFloat(str, 0f);
		};
		array[815] = val;
		val = new Command();
		val.Name = "tick_populations";
		val.Parent = "spawn";
		val.FullName = "spawn.tick_populations";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.tick_populations.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.tick_populations = StringExtensions.ToFloat(str, 0f);
		};
		array[816] = val;
		val = new Command();
		val.Name = "accuracy";
		val.Parent = "stability";
		val.FullName = "stability.accuracy";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.accuracy.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.accuracy = StringExtensions.ToFloat(str, 0f);
		};
		array[817] = val;
		val = new Command();
		val.Name = "collapse";
		val.Parent = "stability";
		val.FullName = "stability.collapse";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.collapse.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.collapse = StringExtensions.ToFloat(str, 0f);
		};
		array[818] = val;
		val = new Command();
		val.Name = "refresh_stability";
		val.Parent = "stability";
		val.FullName = "stability.refresh_stability";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Stability.refresh_stability(arg);
		};
		array[819] = val;
		val = new Command();
		val.Name = "stabilityqueue";
		val.Parent = "stability";
		val.FullName = "stability.stabilityqueue";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.stabilityqueue.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.stabilityqueue = StringExtensions.ToFloat(str, 0f);
		};
		array[820] = val;
		val = new Command();
		val.Name = "strikes";
		val.Parent = "stability";
		val.FullName = "stability.strikes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.strikes.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.strikes = StringExtensions.ToInt(str, 0);
		};
		array[821] = val;
		val = new Command();
		val.Name = "surroundingsqueue";
		val.Parent = "stability";
		val.FullName = "stability.surroundingsqueue";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.surroundingsqueue.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.surroundingsqueue = StringExtensions.ToFloat(str, 0f);
		};
		array[822] = val;
		val = new Command();
		val.Name = "verbose";
		val.Parent = "stability";
		val.FullName = "stability.verbose";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.verbose.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.verbose = StringExtensions.ToInt(str, 0);
		};
		array[823] = val;
		val = new Command();
		val.Name = "server_allow_steam_nicknames";
		val.Parent = "steam";
		val.FullName = "steam.server_allow_steam_nicknames";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Steam.server_allow_steam_nicknames.ToString();
		val.SetOveride = delegate(string str)
		{
			Steam.server_allow_steam_nicknames = StringExtensions.ToBool(str);
		};
		val.Default = "True";
		array[824] = val;
		val = new Command();
		val.Name = "call";
		val.Parent = "supply";
		val.FullName = "supply.call";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Supply.call(arg);
		};
		array[825] = val;
		val = new Command();
		val.Name = "drop";
		val.Parent = "supply";
		val.FullName = "supply.drop";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Supply.drop(arg);
		};
		array[826] = val;
		val = new Command();
		val.Name = "cpu_affinity";
		val.Parent = "system";
		val.FullName = "system.cpu_affinity";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			SystemCommands.cpu_affinity(arg);
		};
		array[827] = val;
		val = new Command();
		val.Name = "cpu_priority";
		val.Parent = "system";
		val.FullName = "system.cpu_priority";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			SystemCommands.cpu_priority(arg);
		};
		array[828] = val;
		val = new Command();
		val.Name = "fixeddelta";
		val.Parent = "time";
		val.FullName = "time.fixeddelta";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Time.fixeddelta.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.fixeddelta = StringExtensions.ToFloat(str, 0f);
		};
		array[829] = val;
		val = new Command();
		val.Name = "maxdelta";
		val.Parent = "time";
		val.FullName = "time.maxdelta";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Time.maxdelta.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.maxdelta = StringExtensions.ToFloat(str, 0f);
		};
		array[830] = val;
		val = new Command();
		val.Name = "pausewhileloading";
		val.Parent = "time";
		val.FullName = "time.pausewhileloading";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Time.pausewhileloading.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.pausewhileloading = StringExtensions.ToBool(str);
		};
		array[831] = val;
		val = new Command();
		val.Name = "timescale";
		val.Parent = "time";
		val.FullName = "time.timescale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Time.timescale.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.timescale = StringExtensions.ToFloat(str, 0f);
		};
		array[832] = val;
		val = new Command();
		val.Name = "global_broadcast";
		val.Parent = "tree";
		val.FullName = "tree.global_broadcast";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Tree.global_broadcast.ToString();
		val.SetOveride = delegate(string str)
		{
			Tree.global_broadcast = StringExtensions.ToBool(str);
		};
		array[833] = val;
		val = new Command();
		val.Name = "autohover";
		val.Parent = "vehicle";
		val.FullName = "vehicle.autohover";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.autohover(arg);
		};
		array[834] = val;
		val = new Command();
		val.Name = "boat_corpse_seconds";
		val.Parent = "vehicle";
		val.FullName = "vehicle.boat_corpse_seconds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => vehicle.boat_corpse_seconds.ToString();
		val.SetOveride = delegate(string str)
		{
			vehicle.boat_corpse_seconds = StringExtensions.ToFloat(str, 0f);
		};
		array[835] = val;
		val = new Command();
		val.Name = "boatdriftinfo";
		val.Parent = "vehicle";
		val.FullName = "vehicle.boatdriftinfo";
		val.ServerAdmin = true;
		val.Description = "Print out boat drift status for all boats";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.boatdriftinfo(arg);
		};
		array[836] = val;
		val = new Command();
		val.Name = "carwrecks";
		val.Parent = "vehicle";
		val.FullName = "vehicle.carwrecks";
		val.ServerAdmin = true;
		val.Description = "Determines whether modular cars turn into wrecks when destroyed, or just immediately gib. Default: true";
		val.Variable = true;
		val.GetOveride = () => vehicle.carwrecks.ToString();
		val.SetOveride = delegate(string str)
		{
			vehicle.carwrecks = StringExtensions.ToBool(str);
		};
		array[837] = val;
		val = new Command();
		val.Name = "cinematictrains";
		val.Parent = "vehicle";
		val.FullName = "vehicle.cinematictrains";
		val.ServerAdmin = true;
		val.Description = "If true, trains always explode when destroyed, and hitting a barrier always destroys the train immediately. Default: false";
		val.Variable = true;
		val.GetOveride = () => vehicle.cinematictrains.ToString();
		val.SetOveride = delegate(string str)
		{
			vehicle.cinematictrains = StringExtensions.ToBool(str);
		};
		array[838] = val;
		val = new Command();
		val.Name = "fixcars";
		val.Parent = "vehicle";
		val.FullName = "vehicle.fixcars";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.fixcars(arg);
		};
		array[839] = val;
		val = new Command();
		val.Name = "killboats";
		val.Parent = "vehicle";
		val.FullName = "vehicle.killboats";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.killboats(arg);
		};
		array[840] = val;
		val = new Command();
		val.Name = "killcars";
		val.Parent = "vehicle";
		val.FullName = "vehicle.killcars";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.killcars(arg);
		};
		array[841] = val;
		val = new Command();
		val.Name = "killdrones";
		val.Parent = "vehicle";
		val.FullName = "vehicle.killdrones";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.killdrones(arg);
		};
		array[842] = val;
		val = new Command();
		val.Name = "killminis";
		val.Parent = "vehicle";
		val.FullName = "vehicle.killminis";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.killminis(arg);
		};
		array[843] = val;
		val = new Command();
		val.Name = "killscraphelis";
		val.Parent = "vehicle";
		val.FullName = "vehicle.killscraphelis";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.killscraphelis(arg);
		};
		array[844] = val;
		val = new Command();
		val.Name = "killtrains";
		val.Parent = "vehicle";
		val.FullName = "vehicle.killtrains";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.killtrains(arg);
		};
		array[845] = val;
		val = new Command();
		val.Name = "stop_all_trains";
		val.Parent = "vehicle";
		val.FullName = "vehicle.stop_all_trains";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.stop_all_trains(arg);
		};
		array[846] = val;
		val = new Command();
		val.Name = "swapseats";
		val.Parent = "vehicle";
		val.FullName = "vehicle.swapseats";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.swapseats(arg);
		};
		array[847] = val;
		val = new Command();
		val.Name = "trainskeeprunning";
		val.Parent = "vehicle";
		val.FullName = "vehicle.trainskeeprunning";
		val.ServerAdmin = true;
		val.Description = "Determines whether trains stop automatically when there's no-one on them. Default: false";
		val.Variable = true;
		val.GetOveride = () => vehicle.trainskeeprunning.ToString();
		val.SetOveride = delegate(string str)
		{
			vehicle.trainskeeprunning = StringExtensions.ToBool(str);
		};
		array[848] = val;
		val = new Command();
		val.Name = "vehiclesdroploot";
		val.Parent = "vehicle";
		val.FullName = "vehicle.vehiclesdroploot";
		val.ServerAdmin = true;
		val.Description = "Determines whether vehicles drop storage items when destroyed. Default: true";
		val.Variable = true;
		val.GetOveride = () => vehicle.vehiclesdroploot.ToString();
		val.SetOveride = delegate(string str)
		{
			vehicle.vehiclesdroploot = StringExtensions.ToBool(str);
		};
		array[849] = val;
		val = new Command();
		val.Name = "attack";
		val.Parent = "vis";
		val.FullName = "vis.attack";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.attack.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.attack = StringExtensions.ToBool(str);
		};
		array[850] = val;
		val = new Command();
		val.Name = "damage";
		val.Parent = "vis";
		val.FullName = "vis.damage";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.damage.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.damage = StringExtensions.ToBool(str);
		};
		array[851] = val;
		val = new Command();
		val.Name = "hitboxes";
		val.Parent = "vis";
		val.FullName = "vis.hitboxes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.hitboxes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.hitboxes = StringExtensions.ToBool(str);
		};
		array[852] = val;
		val = new Command();
		val.Name = "lineofsight";
		val.Parent = "vis";
		val.FullName = "vis.lineofsight";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.lineofsight.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.lineofsight = StringExtensions.ToBool(str);
		};
		array[853] = val;
		val = new Command();
		val.Name = "protection";
		val.Parent = "vis";
		val.FullName = "vis.protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.protection = StringExtensions.ToBool(str);
		};
		array[854] = val;
		val = new Command();
		val.Name = "sense";
		val.Parent = "vis";
		val.FullName = "vis.sense";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.sense.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.sense = StringExtensions.ToBool(str);
		};
		array[855] = val;
		val = new Command();
		val.Name = "triggers";
		val.Parent = "vis";
		val.FullName = "vis.triggers";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.triggers.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.triggers = StringExtensions.ToBool(str);
		};
		array[856] = val;
		val = new Command();
		val.Name = "weakspots";
		val.Parent = "vis";
		val.FullName = "vis.weakspots";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.weakspots.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.weakspots = StringExtensions.ToBool(str);
		};
		array[857] = val;
		val = new Command();
		val.Name = "togglevoicerangeboost";
		val.Parent = "voice";
		val.FullName = "voice.togglevoicerangeboost";
		val.ServerAdmin = true;
		val.Description = "Enabled/disables voice range boost for a player eg. ToggleVoiceRangeBoost sam 1";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Voice.ToggleVoiceRangeBoost(arg);
		};
		array[858] = val;
		val = new Command();
		val.Name = "voicerangeboostamount";
		val.Parent = "voice";
		val.FullName = "voice.voicerangeboostamount";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Voice.voiceRangeBoostAmount.ToString();
		val.SetOveride = delegate(string str)
		{
			Voice.voiceRangeBoostAmount = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "50";
		array[859] = val;
		val = new Command();
		val.Name = "atmosphere_brightness";
		val.Parent = "weather";
		val.FullName = "weather.atmosphere_brightness";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.atmosphere_brightness.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.atmosphere_brightness = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[860] = val;
		val = new Command();
		val.Name = "atmosphere_contrast";
		val.Parent = "weather";
		val.FullName = "weather.atmosphere_contrast";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.atmosphere_contrast.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.atmosphere_contrast = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[861] = val;
		val = new Command();
		val.Name = "atmosphere_directionality";
		val.Parent = "weather";
		val.FullName = "weather.atmosphere_directionality";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.atmosphere_directionality.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.atmosphere_directionality = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[862] = val;
		val = new Command();
		val.Name = "atmosphere_mie";
		val.Parent = "weather";
		val.FullName = "weather.atmosphere_mie";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.atmosphere_mie.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.atmosphere_mie = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[863] = val;
		val = new Command();
		val.Name = "atmosphere_rayleigh";
		val.Parent = "weather";
		val.FullName = "weather.atmosphere_rayleigh";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.atmosphere_rayleigh.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.atmosphere_rayleigh = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[864] = val;
		val = new Command();
		val.Name = "clear_chance";
		val.Parent = "weather";
		val.FullName = "weather.clear_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.clear_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.clear_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "1";
		array[865] = val;
		val = new Command();
		val.Name = "cloud_attenuation";
		val.Parent = "weather";
		val.FullName = "weather.cloud_attenuation";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_attenuation.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_attenuation = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[866] = val;
		val = new Command();
		val.Name = "cloud_brightness";
		val.Parent = "weather";
		val.FullName = "weather.cloud_brightness";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_brightness.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_brightness = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[867] = val;
		val = new Command();
		val.Name = "cloud_coloring";
		val.Parent = "weather";
		val.FullName = "weather.cloud_coloring";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_coloring.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_coloring = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[868] = val;
		val = new Command();
		val.Name = "cloud_coverage";
		val.Parent = "weather";
		val.FullName = "weather.cloud_coverage";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_coverage.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_coverage = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[869] = val;
		val = new Command();
		val.Name = "cloud_opacity";
		val.Parent = "weather";
		val.FullName = "weather.cloud_opacity";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_opacity.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_opacity = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[870] = val;
		val = new Command();
		val.Name = "cloud_saturation";
		val.Parent = "weather";
		val.FullName = "weather.cloud_saturation";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_saturation.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_saturation = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[871] = val;
		val = new Command();
		val.Name = "cloud_scattering";
		val.Parent = "weather";
		val.FullName = "weather.cloud_scattering";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_scattering.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_scattering = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[872] = val;
		val = new Command();
		val.Name = "cloud_sharpness";
		val.Parent = "weather";
		val.FullName = "weather.cloud_sharpness";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_sharpness.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_sharpness = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[873] = val;
		val = new Command();
		val.Name = "cloud_size";
		val.Parent = "weather";
		val.FullName = "weather.cloud_size";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_size.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_size = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[874] = val;
		val = new Command();
		val.Name = "dust_chance";
		val.Parent = "weather";
		val.FullName = "weather.dust_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.dust_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.dust_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[875] = val;
		val = new Command();
		val.Name = "fog";
		val.Parent = "weather";
		val.FullName = "weather.fog";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.fog.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.fog = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[876] = val;
		val = new Command();
		val.Name = "fog_chance";
		val.Parent = "weather";
		val.FullName = "weather.fog_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.fog_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.fog_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[877] = val;
		val = new Command();
		val.Name = "load";
		val.Parent = "weather";
		val.FullName = "weather.load";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Weather.load(arg);
		};
		array[878] = val;
		val = new Command();
		val.Name = "ocean_scale";
		val.Parent = "weather";
		val.FullName = "weather.ocean_scale";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.ocean_scale.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.ocean_scale = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[879] = val;
		val = new Command();
		val.Name = "ocean_time";
		val.Parent = "weather";
		val.FullName = "weather.ocean_time";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.ocean_time.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.ocean_time = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[880] = val;
		val = new Command();
		val.Name = "overcast_chance";
		val.Parent = "weather";
		val.FullName = "weather.overcast_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.overcast_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.overcast_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[881] = val;
		val = new Command();
		val.Name = "rain";
		val.Parent = "weather";
		val.FullName = "weather.rain";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.rain.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.rain = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[882] = val;
		val = new Command();
		val.Name = "rain_chance";
		val.Parent = "weather";
		val.FullName = "weather.rain_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.rain_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.rain_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[883] = val;
		val = new Command();
		val.Name = "rainbow";
		val.Parent = "weather";
		val.FullName = "weather.rainbow";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.rainbow.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.rainbow = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[884] = val;
		val = new Command();
		val.Name = "report";
		val.Parent = "weather";
		val.FullName = "weather.report";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Weather.report(arg);
		};
		array[885] = val;
		val = new Command();
		val.Name = "reset";
		val.Parent = "weather";
		val.FullName = "weather.reset";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Weather.reset(arg);
		};
		array[886] = val;
		val = new Command();
		val.Name = "storm_chance";
		val.Parent = "weather";
		val.FullName = "weather.storm_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.storm_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.storm_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[887] = val;
		val = new Command();
		val.Name = "thunder";
		val.Parent = "weather";
		val.FullName = "weather.thunder";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.thunder.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.thunder = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[888] = val;
		val = new Command();
		val.Name = "wetness_rain";
		val.Parent = "weather";
		val.FullName = "weather.wetness_rain";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Weather.wetness_rain.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.wetness_rain = StringExtensions.ToFloat(str, 0f);
		};
		array[889] = val;
		val = new Command();
		val.Name = "wetness_snow";
		val.Parent = "weather";
		val.FullName = "weather.wetness_snow";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Weather.wetness_snow.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.wetness_snow = StringExtensions.ToFloat(str, 0f);
		};
		array[890] = val;
		val = new Command();
		val.Name = "wind";
		val.Parent = "weather";
		val.FullName = "weather.wind";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.wind.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.wind = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[891] = val;
		val = new Command();
		val.Name = "print_approved_skins";
		val.Parent = "workshop";
		val.FullName = "workshop.print_approved_skins";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Workshop.print_approved_skins(arg);
		};
		array[892] = val;
		val = new Command();
		val.Name = "cache";
		val.Parent = "world";
		val.FullName = "world.cache";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.World.cache.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.World.cache = StringExtensions.ToBool(str);
		};
		array[893] = val;
		val = new Command();
		val.Name = "configfile";
		val.Parent = "world";
		val.FullName = "world.configfile";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.World.configFile.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.World.configFile = str;
		};
		array[894] = val;
		val = new Command();
		val.Name = "configstring";
		val.Parent = "world";
		val.FullName = "world.configstring";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.World.configString.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.World.configString = str;
		};
		array[895] = val;
		val = new Command();
		val.Name = "monuments";
		val.Parent = "world";
		val.FullName = "world.monuments";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.World.monuments(arg);
		};
		array[896] = val;
		val = new Command();
		val.Name = "renderlabs";
		val.Parent = "world";
		val.FullName = "world.renderlabs";
		val.ServerAdmin = true;
		val.Client = true;
		val.Description = "Renders a PNG of the current map's underwater labs, for a specific floor";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.World.renderlabs(arg);
		};
		array[897] = val;
		val = new Command();
		val.Name = "rendermap";
		val.Parent = "world";
		val.FullName = "world.rendermap";
		val.ServerAdmin = true;
		val.Client = true;
		val.Description = "Renders a high resolution PNG of the current map";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.World.rendermap(arg);
		};
		array[898] = val;
		val = new Command();
		val.Name = "rendertunnels";
		val.Parent = "world";
		val.FullName = "world.rendertunnels";
		val.ServerAdmin = true;
		val.Client = true;
		val.Description = "Renders a PNG of the current map's tunnel network";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.World.rendertunnels(arg);
		};
		array[899] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "xmas";
		val.FullName = "xmas.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => XMas.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			XMas.enabled = StringExtensions.ToBool(str);
		};
		array[900] = val;
		val = new Command();
		val.Name = "giftsperplayer";
		val.Parent = "xmas";
		val.FullName = "xmas.giftsperplayer";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => XMas.giftsPerPlayer.ToString();
		val.SetOveride = delegate(string str)
		{
			XMas.giftsPerPlayer = StringExtensions.ToInt(str, 0);
		};
		array[901] = val;
		val = new Command();
		val.Name = "refill";
		val.Parent = "xmas";
		val.FullName = "xmas.refill";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			XMas.refill(arg);
		};
		array[902] = val;
		val = new Command();
		val.Name = "spawnattempts";
		val.Parent = "xmas";
		val.FullName = "xmas.spawnattempts";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => XMas.spawnAttempts.ToString();
		val.SetOveride = delegate(string str)
		{
			XMas.spawnAttempts = StringExtensions.ToInt(str, 0);
		};
		array[903] = val;
		val = new Command();
		val.Name = "spawnrange";
		val.Parent = "xmas";
		val.FullName = "xmas.spawnrange";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => XMas.spawnRange.ToString();
		val.SetOveride = delegate(string str)
		{
			XMas.spawnRange = StringExtensions.ToFloat(str, 0f);
		};
		array[904] = val;
		val = new Command();
		val.Name = "cui_test";
		val.Parent = "cui";
		val.FullName = "cui.cui_test";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			cui.cui_test(arg);
		};
		array[905] = val;
		val = new Command();
		val.Name = "cui_test_update";
		val.Parent = "cui";
		val.FullName = "cui.cui_test_update";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			cui.cui_test_update(arg);
		};
		array[906] = val;
		val = new Command();
		val.Name = "endtest";
		val.Parent = "cui";
		val.FullName = "cui.endtest";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			cui.endtest(arg);
		};
		array[907] = val;
		val = new Command();
		val.Name = "dump";
		val.Parent = "global";
		val.FullName = "global.dump";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			DiagnosticsConSys.dump(arg);
		};
		array[908] = val;
		val = new Command();
		val.Name = "altitudespeedoverride";
		val.Parent = "drone";
		val.FullName = "drone.altitudespeedoverride";
		val.ServerAdmin = true;
		val.Description = "If greater than zero, overrides the drone's vertical movement speed";
		val.Variable = true;
		val.GetOveride = () => Drone.altitudeSpeedOverride.ToString();
		val.SetOveride = delegate(string str)
		{
			Drone.altitudeSpeedOverride = StringExtensions.ToFloat(str, 0f);
		};
		array[909] = val;
		val = new Command();
		val.Name = "maxcontrolrange";
		val.Parent = "drone";
		val.FullName = "drone.maxcontrolrange";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "How far drones can be flown away from the controlling computer station";
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Drone.maxControlRange.ToString();
		val.SetOveride = delegate(string str)
		{
			Drone.maxControlRange = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "250";
		array[910] = val;
		val = new Command();
		val.Name = "movementspeedoverride";
		val.Parent = "drone";
		val.FullName = "drone.movementspeedoverride";
		val.ServerAdmin = true;
		val.Description = "If greater than zero, overrides the drone's planar movement speed";
		val.Variable = true;
		val.GetOveride = () => Drone.movementSpeedOverride.ToString();
		val.SetOveride = delegate(string str)
		{
			Drone.movementSpeedOverride = StringExtensions.ToFloat(str, 0f);
		};
		array[911] = val;
		val = new Command();
		val.Name = "use_baked_terrain_mesh";
		val.Parent = "dungeonnavmesh";
		val.FullName = "dungeonnavmesh.use_baked_terrain_mesh";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => DungeonNavmesh.use_baked_terrain_mesh.ToString();
		val.SetOveride = delegate(string str)
		{
			DungeonNavmesh.use_baked_terrain_mesh = StringExtensions.ToBool(str);
		};
		array[912] = val;
		val = new Command();
		val.Name = "use_baked_terrain_mesh";
		val.Parent = "dynamicnavmesh";
		val.FullName = "dynamicnavmesh.use_baked_terrain_mesh";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => DynamicNavMesh.use_baked_terrain_mesh.ToString();
		val.SetOveride = delegate(string str)
		{
			DynamicNavMesh.use_baked_terrain_mesh = StringExtensions.ToBool(str);
		};
		array[913] = val;
		val = new Command();
		val.Name = "event_hours_before_wipe";
		val.Parent = "eventschedulewipeoffset";
		val.FullName = "eventschedulewipeoffset.event_hours_before_wipe";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => EventScheduleWipeOffset.hoursBeforeWipeRealtime.ToString();
		val.SetOveride = delegate(string str)
		{
			EventScheduleWipeOffset.hoursBeforeWipeRealtime = StringExtensions.ToFloat(str, 0f);
		};
		array[914] = val;
		val = new Command();
		val.Name = "chargeneededforsupplies";
		val.Parent = "excavatorsignalcomputer";
		val.FullName = "excavatorsignalcomputer.chargeneededforsupplies";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ExcavatorSignalComputer.chargeNeededForSupplies.ToString();
		val.SetOveride = delegate(string str)
		{
			ExcavatorSignalComputer.chargeNeededForSupplies = StringExtensions.ToFloat(str, 0f);
		};
		array[915] = val;
		val = new Command();
		val.Name = "steamconnectiontimeout";
		val.Parent = "global";
		val.FullName = "global.steamconnectiontimeout";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamconnectiontimeout.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamconnectiontimeout = StringExtensions.ToInt(str, 0);
		};
		array[916] = val;
		val = new Command();
		val.Name = "steamnagleflush";
		val.Parent = "global";
		val.FullName = "global.steamnagleflush";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamnagleflush.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamnagleflush = StringExtensions.ToBool(str);
		};
		array[917] = val;
		val = new Command();
		val.Name = "steamnagletime";
		val.Parent = "global";
		val.FullName = "global.steamnagletime";
		val.ServerAdmin = true;
		val.Description = "Nagle time, in microseconds";
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamnagletime.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamnagletime = StringExtensions.ToInt(str, 0);
		};
		array[918] = val;
		val = new Command();
		val.Name = "steamnetdebug";
		val.Parent = "global";
		val.FullName = "global.steamnetdebug";
		val.ServerAdmin = true;
		val.Description = "Turns on varying levels of debug output for the Steam Networking. This will affect performance. (0 = off, 1 = bug, 2 = error, 3 = important, 4 = warning, 5 = message, 6 = verbose, 7 = debug, 8 = everything)";
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamnetdebug.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamnetdebug = StringExtensions.ToInt(str, 0);
		};
		array[919] = val;
		val = new Command();
		val.Name = "steamnetdebug_ackrtt";
		val.Parent = "global";
		val.FullName = "global.steamnetdebug_ackrtt";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamnetdebug_ackrtt.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamnetdebug_ackrtt = StringExtensions.ToInt(str, 0);
		};
		array[920] = val;
		val = new Command();
		val.Name = "steamnetdebug_message";
		val.Parent = "global";
		val.FullName = "global.steamnetdebug_message";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamnetdebug_message.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamnetdebug_message = StringExtensions.ToInt(str, 0);
		};
		array[921] = val;
		val = new Command();
		val.Name = "steamnetdebug_p2prendezvous";
		val.Parent = "global";
		val.FullName = "global.steamnetdebug_p2prendezvous";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamnetdebug_p2prendezvous.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamnetdebug_p2prendezvous = StringExtensions.ToInt(str, 0);
		};
		array[922] = val;
		val = new Command();
		val.Name = "steamnetdebug_packetdecode";
		val.Parent = "global";
		val.FullName = "global.steamnetdebug_packetdecode";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamnetdebug_packetdecode.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamnetdebug_packetdecode = StringExtensions.ToInt(str, 0);
		};
		array[923] = val;
		val = new Command();
		val.Name = "steamnetdebug_packetgaps";
		val.Parent = "global";
		val.FullName = "global.steamnetdebug_packetgaps";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamnetdebug_packetgaps.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamnetdebug_packetgaps = StringExtensions.ToInt(str, 0);
		};
		array[924] = val;
		val = new Command();
		val.Name = "steamnetdebug_sdrrelaypings";
		val.Parent = "global";
		val.FullName = "global.steamnetdebug_sdrrelaypings";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamnetdebug_sdrrelaypings.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamnetdebug_sdrrelaypings = StringExtensions.ToInt(str, 0);
		};
		array[925] = val;
		val = new Command();
		val.Name = "steamrelayinit";
		val.Parent = "global";
		val.FullName = "global.steamrelayinit";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate
		{
			SteamNetworking.steamrelayinit();
		};
		array[926] = val;
		val = new Command();
		val.Name = "steamsendbuffer";
		val.Parent = "global";
		val.FullName = "global.steamsendbuffer";
		val.ServerAdmin = true;
		val.Description = "Upper limit of buffered pending bytes to be sent";
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamsendbuffer.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamsendbuffer = StringExtensions.ToInt(str, 0);
		};
		array[927] = val;
		val = new Command();
		val.Name = "steamsendratemax";
		val.Parent = "global";
		val.FullName = "global.steamsendratemax";
		val.ServerAdmin = true;
		val.Description = "Maxminum send rate clamp, 0 is no limit";
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamsendratemax.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamsendratemax = StringExtensions.ToInt(str, 0);
		};
		array[928] = val;
		val = new Command();
		val.Name = "steamsendratemin";
		val.Parent = "global";
		val.FullName = "global.steamsendratemin";
		val.ServerAdmin = true;
		val.Description = "Minimum send rate clamp, 0 is no limit";
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamsendratemin.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamsendratemin = StringExtensions.ToInt(str, 0);
		};
		array[929] = val;
		val = new Command();
		val.Name = "steamstatus";
		val.Parent = "global";
		val.FullName = "global.steamstatus";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text = SteamNetworking.steamstatus();
			arg.ReplyWithObject((object)text);
		};
		array[930] = val;
		val = new Command();
		val.Name = "ip";
		val.Parent = "rcon";
		val.FullName = "rcon.ip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RCon.Ip.ToString();
		val.SetOveride = delegate(string str)
		{
			RCon.Ip = str;
		};
		array[931] = val;
		val = new Command();
		val.Name = "port";
		val.Parent = "rcon";
		val.FullName = "rcon.port";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RCon.Port.ToString();
		val.SetOveride = delegate(string str)
		{
			RCon.Port = StringExtensions.ToInt(str, 0);
		};
		array[932] = val;
		val = new Command();
		val.Name = "print";
		val.Parent = "rcon";
		val.FullName = "rcon.print";
		val.ServerAdmin = true;
		val.Description = "If true, rcon commands etc will be printed in the console";
		val.Variable = true;
		val.GetOveride = () => RCon.Print.ToString();
		val.SetOveride = delegate(string str)
		{
			RCon.Print = StringExtensions.ToBool(str);
		};
		array[933] = val;
		val = new Command();
		val.Name = "web";
		val.Parent = "rcon";
		val.FullName = "rcon.web";
		val.ServerAdmin = true;
		val.Description = "If set to true, use websocket rcon. If set to false use legacy, source engine rcon.";
		val.Variable = true;
		val.GetOveride = () => RCon.Web.ToString();
		val.SetOveride = delegate(string str)
		{
			RCon.Web = StringExtensions.ToBool(str);
		};
		array[934] = val;
		val = new Command();
		val.Name = "analytics_header";
		val.Parent = "analytics";
		val.FullName = "analytics.analytics_header";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.AnalyticsHeader.ToString();
		val.SetOveride = delegate(string str)
		{
			Analytics.AnalyticsHeader = str;
		};
		array[935] = val;
		val = new Command();
		val.Name = "analytics_secret";
		val.Parent = "analytics";
		val.FullName = "analytics.analytics_secret";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.AnalyticsSecret.ToString();
		val.SetOveride = delegate(string str)
		{
			Analytics.AnalyticsSecret = str;
		};
		array[936] = val;
		val = new Command();
		val.Name = "pending_analytics";
		val.Parent = "analytics";
		val.FullName = "analytics.pending_analytics";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Analytics.GetPendingAnalytics(arg);
		};
		array[937] = val;
		val = new Command();
		val.Name = "high_freq_stats";
		val.Parent = "analytics";
		val.FullName = "analytics.high_freq_stats";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.HighFrequencyStats.ToString();
		val.SetOveride = delegate(string str)
		{
			Analytics.HighFrequencyStats = StringExtensions.ToBool(str);
		};
		array[938] = val;
		val = new Command();
		val.Name = "server_analytics_url";
		val.Parent = "analytics";
		val.FullName = "analytics.server_analytics_url";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.ServerAnalyticsUrl.ToString();
		val.SetOveride = delegate(string str)
		{
			Analytics.ServerAnalyticsUrl = str;
		};
		array[939] = val;
		val = new Command();
		val.Name = "stats_blacklist";
		val.Parent = "analytics";
		val.FullName = "analytics.stats_blacklist";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.stats_blacklist.ToString();
		val.SetOveride = delegate(string str)
		{
			Analytics.stats_blacklist = str;
		};
		array[940] = val;
		val = new Command();
		val.Name = "analytics_enabled";
		val.Parent = "analytics";
		val.FullName = "analytics.analytics_enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.UploadAnalytics.ToString();
		val.SetOveride = delegate(string str)
		{
			Analytics.UploadAnalytics = StringExtensions.ToBool(str);
		};
		array[941] = val;
		val = new Command();
		val.Name = "movetowardsrate";
		val.Parent = "frankensteinbrain";
		val.FullName = "frankensteinbrain.movetowardsrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => FrankensteinBrain.MoveTowardsRate.ToString();
		val.SetOveride = delegate(string str)
		{
			FrankensteinBrain.MoveTowardsRate = StringExtensions.ToFloat(str, 0f);
		};
		array[942] = val;
		val = new Command();
		val.Name = "decayminutes";
		val.Parent = "frankensteinpet";
		val.FullName = "frankensteinpet.decayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a Frankenstein Pet dies un controlled and not asleep on table";
		val.Variable = true;
		val.GetOveride = () => FrankensteinPet.decayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			FrankensteinPet.decayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[943] = val;
		val = new Command();
		val.Name = "reclaim_fraction_belt";
		val.Parent = "gamemodesoftcore";
		val.FullName = "gamemodesoftcore.reclaim_fraction_belt";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GameModeSoftcore.reclaim_fraction_belt.ToString();
		val.SetOveride = delegate(string str)
		{
			GameModeSoftcore.reclaim_fraction_belt = StringExtensions.ToFloat(str, 0f);
		};
		array[944] = val;
		val = new Command();
		val.Name = "reclaim_fraction_main";
		val.Parent = "gamemodesoftcore";
		val.FullName = "gamemodesoftcore.reclaim_fraction_main";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GameModeSoftcore.reclaim_fraction_main.ToString();
		val.SetOveride = delegate(string str)
		{
			GameModeSoftcore.reclaim_fraction_main = StringExtensions.ToFloat(str, 0f);
		};
		array[945] = val;
		val = new Command();
		val.Name = "reclaim_fraction_wear";
		val.Parent = "gamemodesoftcore";
		val.FullName = "gamemodesoftcore.reclaim_fraction_wear";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GameModeSoftcore.reclaim_fraction_wear.ToString();
		val.SetOveride = delegate(string str)
		{
			GameModeSoftcore.reclaim_fraction_wear = StringExtensions.ToFloat(str, 0f);
		};
		array[946] = val;
		val = new Command();
		val.Name = "framebudgetms";
		val.Parent = "growableentity";
		val.FullName = "growableentity.framebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GrowableEntity.framebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			GrowableEntity.framebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[947] = val;
		val = new Command();
		val.Name = "growall";
		val.Parent = "growableentity";
		val.FullName = "growableentity.growall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			GrowableEntity.GrowAll(arg);
		};
		array[948] = val;
		val = new Command();
		val.Name = "gun_trap_budget_ms";
		val.Parent = "guntrap";
		val.FullName = "guntrap.gun_trap_budget_ms";
		val.ServerAdmin = true;
		val.Description = "How many milliseconds to spend on target scanning per frame";
		val.Variable = true;
		val.GetOveride = () => GunTrap.gun_trap_budget_ms.ToString();
		val.SetOveride = delegate(string str)
		{
			GunTrap.gun_trap_budget_ms = StringExtensions.ToFloat(str, 0f);
		};
		array[949] = val;
		val = new Command();
		val.Name = "decayseconds";
		val.Parent = "hackablelockedcrate";
		val.FullName = "hackablelockedcrate.decayseconds";
		val.ServerAdmin = true;
		val.Description = "How many seconds until the crate is destroyed without any hack attempts";
		val.Variable = true;
		val.GetOveride = () => HackableLockedCrate.decaySeconds.ToString();
		val.SetOveride = delegate(string str)
		{
			HackableLockedCrate.decaySeconds = StringExtensions.ToFloat(str, 0f);
		};
		array[950] = val;
		val = new Command();
		val.Name = "requiredhackseconds";
		val.Parent = "hackablelockedcrate";
		val.FullName = "hackablelockedcrate.requiredhackseconds";
		val.ServerAdmin = true;
		val.Description = "How many seconds for the crate to unlock";
		val.Variable = true;
		val.GetOveride = () => HackableLockedCrate.requiredHackSeconds.ToString();
		val.SetOveride = delegate(string str)
		{
			HackableLockedCrate.requiredHackSeconds = StringExtensions.ToFloat(str, 0f);
		};
		array[951] = val;
		val = new Command();
		val.Name = "lifetime";
		val.Parent = "halloweendungeon";
		val.FullName = "halloweendungeon.lifetime";
		val.ServerAdmin = true;
		val.Description = "How long each active dungeon should last before dying";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => HalloweenDungeon.lifetime.ToString();
		val.SetOveride = delegate(string str)
		{
			HalloweenDungeon.lifetime = StringExtensions.ToFloat(str, 0f);
		};
		array[952] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "halloweendungeon";
		val.FullName = "halloweendungeon.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => HalloweenDungeon.population.ToString();
		val.SetOveride = delegate(string str)
		{
			HalloweenDungeon.population = StringExtensions.ToFloat(str, 0f);
		};
		array[953] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "horse";
		val.FullName = "horse.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Horse.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Horse.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[954] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "hotairballoon";
		val.FullName = "hotairballoon.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a HAB loses all its health while outside";
		val.Variable = true;
		val.GetOveride = () => HotAirBalloon.outsidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			HotAirBalloon.outsidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[955] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "hotairballoon";
		val.FullName = "hotairballoon.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => HotAirBalloon.population.ToString();
		val.SetOveride = delegate(string str)
		{
			HotAirBalloon.population = StringExtensions.ToFloat(str, 0f);
		};
		array[956] = val;
		val = new Command();
		val.Name = "serviceceiling";
		val.Parent = "hotairballoon";
		val.FullName = "hotairballoon.serviceceiling";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => HotAirBalloon.serviceCeiling.ToString();
		val.SetOveride = delegate(string str)
		{
			HotAirBalloon.serviceCeiling = StringExtensions.ToFloat(str, 0f);
		};
		array[957] = val;
		val = new Command();
		val.Name = "backtracking";
		val.Parent = "ioentity";
		val.FullName = "ioentity.backtracking";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.backtracking.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.backtracking = StringExtensions.ToInt(str, 0);
		};
		array[958] = val;
		val = new Command();
		val.Name = "debugbudget";
		val.Parent = "ioentity";
		val.FullName = "ioentity.debugbudget";
		val.ServerAdmin = true;
		val.Description = "Print out what is taking so long in the IO frame budget";
		val.Variable = true;
		val.GetOveride = () => IOEntity.debugBudget.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.debugBudget = StringExtensions.ToBool(str);
		};
		array[959] = val;
		val = new Command();
		val.Name = "debugbudgetthreshold";
		val.Parent = "ioentity";
		val.FullName = "ioentity.debugbudgetthreshold";
		val.ServerAdmin = true;
		val.Description = "Ignore frames with a lower ms than this while debugBudget is active";
		val.Variable = true;
		val.GetOveride = () => IOEntity.debugBudgetThreshold.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.debugBudgetThreshold = StringExtensions.ToFloat(str, 0f);
		};
		array[960] = val;
		val = new Command();
		val.Name = "framebudgetms";
		val.Parent = "ioentity";
		val.FullName = "ioentity.framebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.framebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.framebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[961] = val;
		val = new Command();
		val.Name = "responsetime";
		val.Parent = "ioentity";
		val.FullName = "ioentity.responsetime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.responsetime.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.responsetime = StringExtensions.ToFloat(str, 0f);
		};
		array[962] = val;
		val = new Command();
		val.Name = "framebudgetms";
		val.Parent = "junkpilewater";
		val.FullName = "junkpilewater.framebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => JunkPileWater.framebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			JunkPileWater.framebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[963] = val;
		val = new Command();
		val.Name = "megaphonevoicerange";
		val.Parent = "megaphone";
		val.FullName = "megaphone.megaphonevoicerange";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Megaphone.MegaphoneVoiceRange.ToString();
		val.SetOveride = delegate(string str)
		{
			Megaphone.MegaphoneVoiceRange = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "100";
		array[964] = val;
		val = new Command();
		val.Name = "add";
		val.Parent = "meta";
		val.FullName = "meta.add";
		val.ServerAdmin = true;
		val.Client = true;
		val.Description = "add <convar> <amount> - adds amount to convar";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Meta.add(arg);
		};
		array[965] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "minicopter";
		val.FullName = "minicopter.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Minicopter.population.ToString();
		val.SetOveride = delegate(string str)
		{
			Minicopter.population = StringExtensions.ToFloat(str, 0f);
		};
		array[966] = val;
		val = new Command();
		val.Name = "brokendownminutes";
		val.Parent = "mlrs";
		val.FullName = "mlrs.brokendownminutes";
		val.ServerAdmin = true;
		val.Description = "How many minutes before the MLRS recovers from use and can be used again";
		val.Variable = true;
		val.GetOveride = () => MLRS.brokenDownMinutes.ToString();
		val.SetOveride = delegate(string str)
		{
			MLRS.brokenDownMinutes = StringExtensions.ToFloat(str, 0f);
		};
		array[967] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "modularcar";
		val.FullName = "modularcar.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How many minutes before a ModularCar loses all its health while outside";
		val.Variable = true;
		val.GetOveride = () => ModularCar.outsidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			ModularCar.outsidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[968] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "modularcar";
		val.FullName = "modularcar.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ModularCar.population.ToString();
		val.SetOveride = delegate(string str)
		{
			ModularCar.population = StringExtensions.ToFloat(str, 0f);
		};
		array[969] = val;
		val = new Command();
		val.Name = "use_baked_terrain_mesh";
		val.Parent = "monumentnavmesh";
		val.FullName = "monumentnavmesh.use_baked_terrain_mesh";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => MonumentNavMesh.use_baked_terrain_mesh.ToString();
		val.SetOveride = delegate(string str)
		{
			MonumentNavMesh.use_baked_terrain_mesh = StringExtensions.ToBool(str);
		};
		array[970] = val;
		val = new Command();
		val.Name = "decaystartdelayminutes";
		val.Parent = "motorrowboat";
		val.FullName = "motorrowboat.decaystartdelayminutes";
		val.ServerAdmin = true;
		val.Description = "How long until decay begins after the boat was last used";
		val.Variable = true;
		val.GetOveride = () => MotorRowboat.decaystartdelayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			MotorRowboat.decaystartdelayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[971] = val;
		val = new Command();
		val.Name = "deepwaterdecayminutes";
		val.Parent = "motorrowboat";
		val.FullName = "motorrowboat.deepwaterdecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a boat loses all its health while in deep water";
		val.Variable = true;
		val.GetOveride = () => MotorRowboat.deepwaterdecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			MotorRowboat.deepwaterdecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[972] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "motorrowboat";
		val.FullName = "motorrowboat.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a boat loses all its health while outside. If it's in deep water, deepwaterdecayminutes is used";
		val.Variable = true;
		val.GetOveride = () => MotorRowboat.outsidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			MotorRowboat.outsidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[973] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "motorrowboat";
		val.FullName = "motorrowboat.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => MotorRowboat.population.ToString();
		val.SetOveride = delegate(string str)
		{
			MotorRowboat.population = StringExtensions.ToFloat(str, 0f);
		};
		array[974] = val;
		val = new Command();
		val.Name = "update";
		val.Parent = "note";
		val.FullName = "note.update";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			note.update(arg);
		};
		array[975] = val;
		val = new Command();
		val.Name = "sleeperhostiledelay";
		val.Parent = "npcautoturret";
		val.FullName = "npcautoturret.sleeperhostiledelay";
		val.ServerAdmin = true;
		val.Description = "How many seconds until a sleeping player is considered hostile";
		val.Variable = true;
		val.GetOveride = () => NPCAutoTurret.sleeperhostiledelay.ToString();
		val.SetOveride = delegate(string str)
		{
			NPCAutoTurret.sleeperhostiledelay = StringExtensions.ToFloat(str, 0f);
		};
		array[976] = val;
		val = new Command();
		val.Name = "bypassrepack";
		val.Parent = "parachute";
		val.FullName = "parachute.bypassrepack";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Parachute.BypassRepack.ToString();
		val.SetOveride = delegate(string str)
		{
			Parachute.BypassRepack = StringExtensions.ToBool(str);
		};
		array[977] = val;
		val = new Command();
		val.Name = "landinganimations";
		val.Parent = "parachute";
		val.FullName = "parachute.landinganimations";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Parachute.LandingAnimations.ToString();
		val.SetOveride = delegate(string str)
		{
			Parachute.LandingAnimations = StringExtensions.ToBool(str);
		};
		array[978] = val;
		val = new Command();
		val.Name = "controldistance";
		val.Parent = "petbrain";
		val.FullName = "petbrain.controldistance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => PetBrain.ControlDistance.ToString();
		val.SetOveride = delegate(string str)
		{
			PetBrain.ControlDistance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "100";
		array[979] = val;
		val = new Command();
		val.Name = "drownindeepwater";
		val.Parent = "petbrain";
		val.FullName = "petbrain.drownindeepwater";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PetBrain.DrownInDeepWater.ToString();
		val.SetOveride = delegate(string str)
		{
			PetBrain.DrownInDeepWater = StringExtensions.ToBool(str);
		};
		array[980] = val;
		val = new Command();
		val.Name = "drowntimer";
		val.Parent = "petbrain";
		val.FullName = "petbrain.drowntimer";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PetBrain.DrownTimer.ToString();
		val.SetOveride = delegate(string str)
		{
			PetBrain.DrownTimer = StringExtensions.ToFloat(str, 0f);
		};
		array[981] = val;
		val = new Command();
		val.Name = "idlewhenownermounted";
		val.Parent = "petbrain";
		val.FullName = "petbrain.idlewhenownermounted";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PetBrain.IdleWhenOwnerMounted.ToString();
		val.SetOveride = delegate(string str)
		{
			PetBrain.IdleWhenOwnerMounted = StringExtensions.ToBool(str);
		};
		array[982] = val;
		val = new Command();
		val.Name = "idlewhenownerofflineordead";
		val.Parent = "petbrain";
		val.FullName = "petbrain.idlewhenownerofflineordead";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PetBrain.IdleWhenOwnerOfflineOrDead.ToString();
		val.SetOveride = delegate(string str)
		{
			PetBrain.IdleWhenOwnerOfflineOrDead = StringExtensions.ToBool(str);
		};
		array[983] = val;
		val = new Command();
		val.Name = "insidedecayminutes";
		val.Parent = "playerhelicopter";
		val.FullName = "playerhelicopter.insidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a player helicopter loses all its health while indoors";
		val.Variable = true;
		val.GetOveride = () => PlayerHelicopter.insidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			PlayerHelicopter.insidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[984] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "playerhelicopter";
		val.FullName = "playerhelicopter.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a player helicopter loses all its health while outside";
		val.Variable = true;
		val.GetOveride = () => PlayerHelicopter.outsidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			PlayerHelicopter.outsidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[985] = val;
		val = new Command();
		val.Name = "forcebirthday";
		val.Parent = "playerinventory";
		val.FullName = "playerinventory.forcebirthday";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PlayerInventory.forceBirthday.ToString();
		val.SetOveride = delegate(string str)
		{
			PlayerInventory.forceBirthday = StringExtensions.ToBool(str);
		};
		array[986] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "polarbear";
		val.FullName = "polarbear.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Polarbear.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Polarbear.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[987] = val;
		val = new Command();
		val.Name = "reclaim_expire_minutes";
		val.Parent = "reclaimmanager";
		val.FullName = "reclaimmanager.reclaim_expire_minutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ReclaimManager.reclaim_expire_minutes.ToString();
		val.SetOveride = delegate(string str)
		{
			ReclaimManager.reclaim_expire_minutes = StringExtensions.ToFloat(str, 0f);
		};
		array[988] = val;
		val = new Command();
		val.Name = "acceptinvite";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.acceptinvite";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.acceptinvite(arg);
		};
		array[989] = val;
		val = new Command();
		val.Name = "addtoteam";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.addtoteam";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.addtoteam(arg);
		};
		array[990] = val;
		val = new Command();
		val.Name = "contacts";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.contacts";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.contacts.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.contacts = StringExtensions.ToBool(str);
		};
		val.Default = "true";
		array[991] = val;
		val = new Command();
		val.Name = "fakeinvite";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.fakeinvite";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.fakeinvite(arg);
		};
		array[992] = val;
		val = new Command();
		val.Name = "forgetafterminutes";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.forgetafterminutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.forgetafterminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.forgetafterminutes = StringExtensions.ToInt(str, 0);
		};
		array[993] = val;
		val = new Command();
		val.Name = "kickmember";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.kickmember";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.kickmember(arg);
		};
		array[994] = val;
		val = new Command();
		val.Name = "leaveteam";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.leaveteam";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.leaveteam(arg);
		};
		array[995] = val;
		val = new Command();
		val.Name = "maxplayerrelationships";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.maxplayerrelationships";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.maxplayerrelationships.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.maxplayerrelationships = StringExtensions.ToInt(str, 0);
		};
		array[996] = val;
		val = new Command();
		val.Name = "maxteamsize";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.maxteamsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.maxTeamSize.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.maxTeamSize = StringExtensions.ToInt(str, 0);
		};
		array[997] = val;
		val = new Command();
		val.Name = "mugshotupdateinterval";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.mugshotupdateinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.mugshotUpdateInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.mugshotUpdateInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[998] = val;
		val = new Command();
		val.Name = "promote";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.promote";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.promote(arg);
		};
		array[999] = val;
		val = new Command();
		val.Name = "rejectinvite";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.rejectinvite";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.rejectinvite(arg);
		};
		array[1000] = val;
		val = new Command();
		val.Name = "seendistance";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.seendistance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.seendistance.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.seendistance = StringExtensions.ToFloat(str, 0f);
		};
		array[1001] = val;
		val = new Command();
		val.Name = "sendinvite";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.sendinvite";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.sendinvite(arg);
		};
		array[1002] = val;
		val = new Command();
		val.Name = "sleeptoggle";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.sleeptoggle";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.sleeptoggle(arg);
		};
		array[1003] = val;
		val = new Command();
		val.Name = "trycreateteam";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.trycreateteam";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.trycreateteam(arg);
		};
		array[1004] = val;
		val = new Command();
		val.Name = "wipe_all_contacts";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.wipe_all_contacts";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.wipe_all_contacts(arg);
		};
		array[1005] = val;
		val = new Command();
		val.Name = "wipecontacts";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.wipecontacts";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.wipecontacts(arg);
		};
		array[1006] = val;
		val = new Command();
		val.Name = "rhibpopulation";
		val.Parent = "rhib";
		val.FullName = "rhib.rhibpopulation";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => RHIB.rhibpopulation.ToString();
		val.SetOveride = delegate(string str)
		{
			RHIB.rhibpopulation = StringExtensions.ToFloat(str, 0f);
		};
		array[1007] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "ridablehorse";
		val.FullName = "ridablehorse.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => RidableHorse.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			RidableHorse.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[1008] = val;
		val = new Command();
		val.Name = "sethorsebreed";
		val.Parent = "ridablehorse";
		val.FullName = "ridablehorse.sethorsebreed";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RidableHorse.setHorseBreed(arg);
		};
		array[1009] = val;
		val = new Command();
		val.Name = "ai_dormant";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_dormant";
		val.ServerAdmin = true;
		val.Description = "If ai_dormant is true, any npc outside the range of players will render itself dormant and take up less resources, but wildlife won't simulate as well.";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_dormant.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_dormant = StringExtensions.ToBool(str);
		};
		array[1010] = val;
		val = new Command();
		val.Name = "ai_dormant_max_wakeup_per_tick";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_dormant_max_wakeup_per_tick";
		val.ServerAdmin = true;
		val.Description = "ai_dormant_max_wakeup_per_tick defines the maximum number of dormant agents we will wake up in a single tick. (default: 30)";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_dormant_max_wakeup_per_tick.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_dormant_max_wakeup_per_tick = StringExtensions.ToInt(str, 0);
		};
		array[1011] = val;
		val = new Command();
		val.Name = "ai_htn_animal_tick_budget";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_htn_animal_tick_budget";
		val.ServerAdmin = true;
		val.Description = "ai_htn_animal_tick_budget defines the maximum amount of milliseconds ticking htn animal agents are allowed to consume. (default: 4 ms)";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_htn_animal_tick_budget.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_htn_animal_tick_budget = StringExtensions.ToFloat(str, 0f);
		};
		array[1012] = val;
		val = new Command();
		val.Name = "ai_htn_player_junkpile_tick_budget";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_htn_player_junkpile_tick_budget";
		val.ServerAdmin = true;
		val.Description = "ai_htn_player_junkpile_tick_budget defines the maximum amount of milliseconds ticking htn player junkpile agents are allowed to consume. (default: 4 ms)";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_htn_player_junkpile_tick_budget.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_htn_player_junkpile_tick_budget = StringExtensions.ToFloat(str, 0f);
		};
		array[1013] = val;
		val = new Command();
		val.Name = "ai_htn_player_tick_budget";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_htn_player_tick_budget";
		val.ServerAdmin = true;
		val.Description = "ai_htn_player_tick_budget defines the maximum amount of milliseconds ticking htn player agents are allowed to consume. (default: 4 ms)";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_htn_player_tick_budget.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_htn_player_tick_budget = StringExtensions.ToFloat(str, 0f);
		};
		array[1014] = val;
		val = new Command();
		val.Name = "ai_htn_use_agency_tick";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_htn_use_agency_tick";
		val.ServerAdmin = true;
		val.Description = "If ai_htn_use_agency_tick is true, the ai manager's agency system will tick htn agents at the ms budgets defined in ai_htn_player_tick_budget and ai_htn_animal_tick_budget. If it's false, each agent registers with the invoke system individually, with no frame-budget restrictions. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_htn_use_agency_tick.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_htn_use_agency_tick = StringExtensions.ToBool(str);
		};
		array[1015] = val;
		val = new Command();
		val.Name = "ai_to_player_distance_wakeup_range";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_to_player_distance_wakeup_range";
		val.ServerAdmin = true;
		val.Description = "If an agent is beyond this distance to a player, it's flagged for becoming dormant.";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_to_player_distance_wakeup_range.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_to_player_distance_wakeup_range = StringExtensions.ToFloat(str, 0f);
		};
		array[1016] = val;
		val = new Command();
		val.Name = "nav_disable";
		val.Parent = "aimanager";
		val.FullName = "aimanager.nav_disable";
		val.ServerAdmin = true;
		val.Description = "If set to true the navmesh won't generate.. which means Ai that uses the navmesh won't be able to move";
		val.Variable = true;
		val.GetOveride = () => AiManager.nav_disable.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.nav_disable = StringExtensions.ToBool(str);
		};
		array[1017] = val;
		val = new Command();
		val.Name = "nav_obstacles_carve_state";
		val.Parent = "aimanager";
		val.FullName = "aimanager.nav_obstacles_carve_state";
		val.ServerAdmin = true;
		val.Description = "nav_obstacles_carve_state defines which obstacles can carve the terrain. 0 - No carving, 1 - Only player construction carves, 2 - All obstacles carve.";
		val.Variable = true;
		val.GetOveride = () => AiManager.nav_obstacles_carve_state.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.nav_obstacles_carve_state = StringExtensions.ToInt(str, 0);
		};
		array[1018] = val;
		val = new Command();
		val.Name = "nav_wait";
		val.Parent = "aimanager";
		val.FullName = "aimanager.nav_wait";
		val.ServerAdmin = true;
		val.Description = "If true we'll wait for the navmesh to generate before completely starting the server. This might cause your server to hitch and lag as it generates in the background.";
		val.Variable = true;
		val.GetOveride = () => AiManager.nav_wait.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.nav_wait = StringExtensions.ToBool(str);
		};
		array[1019] = val;
		val = new Command();
		val.Name = "pathfindingiterationsperframe";
		val.Parent = "aimanager";
		val.FullName = "aimanager.pathfindingiterationsperframe";
		val.ServerAdmin = true;
		val.Description = "The maximum amount of nodes processed each frame in the asynchronous pathfinding process. Increasing this value will cause the paths to be processed faster, but can cause some hiccups in frame rate. Default value is 100, a good range for tuning is between 50 and 500.";
		val.Variable = true;
		val.GetOveride = () => AiManager.pathfindingIterationsPerFrame.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.pathfindingIterationsPerFrame = StringExtensions.ToInt(str, 0);
		};
		array[1020] = val;
		val = new Command();
		val.Name = "setdestination_navmesh_failsafe";
		val.Parent = "aimanager";
		val.FullName = "aimanager.setdestination_navmesh_failsafe";
		val.ServerAdmin = true;
		val.Description = "If set to true, npcs will attempt to place themselves on the navmesh if not on a navmesh when set destination is called.";
		val.Variable = true;
		val.GetOveride = () => AiManager.setdestination_navmesh_failsafe.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.setdestination_navmesh_failsafe = StringExtensions.ToBool(str);
		};
		array[1021] = val;
		val = new Command();
		val.Name = "cover_point_sample_step_height";
		val.Parent = "coverpointvolume";
		val.FullName = "coverpointvolume.cover_point_sample_step_height";
		val.ServerAdmin = true;
		val.Description = "cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)";
		val.Variable = true;
		val.GetOveride = () => CoverPointVolume.cover_point_sample_step_height.ToString();
		val.SetOveride = delegate(string str)
		{
			CoverPointVolume.cover_point_sample_step_height = StringExtensions.ToFloat(str, 0f);
		};
		array[1022] = val;
		val = new Command();
		val.Name = "cover_point_sample_step_size";
		val.Parent = "coverpointvolume";
		val.FullName = "coverpointvolume.cover_point_sample_step_size";
		val.ServerAdmin = true;
		val.Description = "cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)";
		val.Variable = true;
		val.GetOveride = () => CoverPointVolume.cover_point_sample_step_size.ToString();
		val.SetOveride = delegate(string str)
		{
			CoverPointVolume.cover_point_sample_step_size = StringExtensions.ToFloat(str, 0f);
		};
		array[1023] = val;
		val = new Command();
		val.Name = "staticrepairseconds";
		val.Parent = "samsite";
		val.FullName = "samsite.staticrepairseconds";
		val.ServerAdmin = true;
		val.Description = "how long until static sam sites auto repair";
		val.Variable = true;
		val.GetOveride = () => SamSite.staticrepairseconds.ToString();
		val.SetOveride = delegate(string str)
		{
			SamSite.staticrepairseconds = StringExtensions.ToFloat(str, 0f);
		};
		array[1024] = val;
		val = new Command();
		val.Name = "altitudeaboveterrain";
		val.Parent = "santasleigh";
		val.FullName = "santasleigh.altitudeaboveterrain";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SantaSleigh.altitudeAboveTerrain.ToString();
		val.SetOveride = delegate(string str)
		{
			SantaSleigh.altitudeAboveTerrain = StringExtensions.ToFloat(str, 0f);
		};
		array[1025] = val;
		val = new Command();
		val.Name = "desiredaltitude";
		val.Parent = "santasleigh";
		val.FullName = "santasleigh.desiredaltitude";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SantaSleigh.desiredAltitude.ToString();
		val.SetOveride = delegate(string str)
		{
			SantaSleigh.desiredAltitude = StringExtensions.ToFloat(str, 0f);
		};
		array[1026] = val;
		val = new Command();
		val.Name = "drop";
		val.Parent = "santasleigh";
		val.FullName = "santasleigh.drop";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			SantaSleigh.drop(arg);
		};
		array[1027] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "scraptransporthelicopter";
		val.FullName = "scraptransporthelicopter.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ScrapTransportHelicopter.population.ToString();
		val.SetOveride = delegate(string str)
		{
			ScrapTransportHelicopter.population = StringExtensions.ToFloat(str, 0f);
		};
		array[1028] = val;
		val = new Command();
		val.Name = "disable";
		val.Parent = "simpleshark";
		val.FullName = "simpleshark.disable";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SimpleShark.disable.ToString();
		val.SetOveride = delegate(string str)
		{
			SimpleShark.disable = StringExtensions.ToBool(str);
		};
		array[1029] = val;
		val = new Command();
		val.Name = "forcesurfaceamount";
		val.Parent = "simpleshark";
		val.FullName = "simpleshark.forcesurfaceamount";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SimpleShark.forceSurfaceAmount.ToString();
		val.SetOveride = delegate(string str)
		{
			SimpleShark.forceSurfaceAmount = StringExtensions.ToFloat(str, 0f);
		};
		array[1030] = val;
		val = new Command();
		val.Name = "forcepayoutindex";
		val.Parent = "slotmachine";
		val.FullName = "slotmachine.forcepayoutindex";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SlotMachine.ForcePayoutIndex.ToString();
		val.SetOveride = delegate(string str)
		{
			SlotMachine.ForcePayoutIndex = StringExtensions.ToInt(str, 0);
		};
		array[1031] = val;
		val = new Command();
		val.Name = "allowpassengeronly";
		val.Parent = "snowmobile";
		val.FullName = "snowmobile.allowpassengeronly";
		val.ServerAdmin = true;
		val.Description = "Allow mounting as a passenger when there's no driver";
		val.Variable = true;
		val.GetOveride = () => Snowmobile.allowPassengerOnly.ToString();
		val.SetOveride = delegate(string str)
		{
			Snowmobile.allowPassengerOnly = StringExtensions.ToBool(str);
		};
		array[1032] = val;
		val = new Command();
		val.Name = "allterrain";
		val.Parent = "snowmobile";
		val.FullName = "snowmobile.allterrain";
		val.ServerAdmin = true;
		val.Description = "If true, snowmobile goes fast on all terrain types";
		val.Variable = true;
		val.GetOveride = () => Snowmobile.allTerrain.ToString();
		val.SetOveride = delegate(string str)
		{
			Snowmobile.allTerrain = StringExtensions.ToBool(str);
		};
		array[1033] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "snowmobile";
		val.FullName = "snowmobile.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a snowmobile loses all its health while outside";
		val.Variable = true;
		val.GetOveride = () => Snowmobile.outsideDecayMinutes.ToString();
		val.SetOveride = delegate(string str)
		{
			Snowmobile.outsideDecayMinutes = StringExtensions.ToFloat(str, 0f);
		};
		array[1034] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "stag";
		val.FullName = "stag.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Stag.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Stag.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[1035] = val;
		val = new Command();
		val.Name = "maxcalllength";
		val.Parent = "telephonemanager";
		val.FullName = "telephonemanager.maxcalllength";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => TelephoneManager.MaxCallLength.ToString();
		val.SetOveride = delegate(string str)
		{
			TelephoneManager.MaxCallLength = StringExtensions.ToInt(str, 0);
		};
		array[1036] = val;
		val = new Command();
		val.Name = "maxconcurrentcalls";
		val.Parent = "telephonemanager";
		val.FullName = "telephonemanager.maxconcurrentcalls";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => TelephoneManager.MaxConcurrentCalls.ToString();
		val.SetOveride = delegate(string str)
		{
			TelephoneManager.MaxConcurrentCalls = StringExtensions.ToInt(str, 0);
		};
		array[1037] = val;
		val = new Command();
		val.Name = "printallphones";
		val.Parent = "telephonemanager";
		val.FullName = "telephonemanager.printallphones";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			TelephoneManager.PrintAllPhones(arg);
		};
		array[1038] = val;
		val = new Command();
		val.Name = "decayminutes";
		val.Parent = "traincar";
		val.FullName = "traincar.decayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a train car despawns";
		val.Variable = true;
		val.GetOveride = () => TrainCar.decayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			TrainCar.decayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[1039] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "traincar";
		val.FullName = "traincar.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => TrainCar.population.ToString();
		val.SetOveride = delegate(string str)
		{
			TrainCar.population = StringExtensions.ToFloat(str, 0f);
		};
		array[1040] = val;
		val = new Command();
		val.Name = "wagons_per_engine";
		val.Parent = "traincar";
		val.FullName = "traincar.wagons_per_engine";
		val.ServerAdmin = true;
		val.Description = "Ratio of wagons to train engines that spawn";
		val.Variable = true;
		val.GetOveride = () => TrainCar.wagons_per_engine.ToString();
		val.SetOveride = delegate(string str)
		{
			TrainCar.wagons_per_engine = StringExtensions.ToInt(str, 0);
		};
		array[1041] = val;
		val = new Command();
		val.Name = "decayminutesafterunload";
		val.Parent = "traincarunloadable";
		val.FullName = "traincarunloadable.decayminutesafterunload";
		val.ServerAdmin = true;
		val.Description = "How long before an unloadable train car despawns afer being unloaded";
		val.Variable = true;
		val.GetOveride = () => TrainCarUnloadable.decayminutesafterunload.ToString();
		val.SetOveride = delegate(string str)
		{
			TrainCarUnloadable.decayminutesafterunload = StringExtensions.ToFloat(str, 0f);
		};
		array[1042] = val;
		val = new Command();
		val.Name = "max_couple_speed";
		val.Parent = "traincouplingcontroller";
		val.FullName = "traincouplingcontroller.max_couple_speed";
		val.ServerAdmin = true;
		val.Description = "Maximum difference in velocity for train cars to couple";
		val.Variable = true;
		val.GetOveride = () => TrainCouplingController.max_couple_speed.ToString();
		val.SetOveride = delegate(string str)
		{
			TrainCouplingController.max_couple_speed = StringExtensions.ToFloat(str, 0f);
		};
		array[1043] = val;
		val = new Command();
		val.Name = "tugcorpseseconds";
		val.Parent = "tugboat";
		val.FullName = "tugboat.tugcorpseseconds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Tugboat.tugcorpseseconds.ToString();
		val.SetOveride = delegate(string str)
		{
			Tugboat.tugcorpseseconds = StringExtensions.ToFloat(str, 0f);
		};
		array[1044] = val;
		val = new Command();
		val.Name = "tugdecayminutes";
		val.Parent = "tugboat";
		val.FullName = "tugboat.tugdecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a tugboat loses all its health while outside";
		val.Variable = true;
		val.GetOveride = () => Tugboat.tugdecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			Tugboat.tugdecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[1045] = val;
		val = new Command();
		val.Name = "tugdecaystartdelayminutes";
		val.Parent = "tugboat";
		val.FullName = "tugboat.tugdecaystartdelayminutes";
		val.ServerAdmin = true;
		val.Description = "How long until decay begins after the tugboat was last used";
		val.Variable = true;
		val.GetOveride = () => Tugboat.tugdecaystartdelayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			Tugboat.tugdecaystartdelayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[1046] = val;
		val = new Command();
		val.Name = "days_to_add_test";
		val.Parent = "wipetimer";
		val.FullName = "wipetimer.days_to_add_test";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => WipeTimer.daysToAddTest.ToString();
		val.SetOveride = delegate(string str)
		{
			WipeTimer.daysToAddTest = StringExtensions.ToInt(str, 0);
		};
		array[1047] = val;
		val = new Command();
		val.Name = "hours_to_add_test";
		val.Parent = "wipetimer";
		val.FullName = "wipetimer.hours_to_add_test";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => WipeTimer.hoursToAddTest.ToString();
		val.SetOveride = delegate(string str)
		{
			WipeTimer.hoursToAddTest = StringExtensions.ToFloat(str, 0f);
		};
		array[1048] = val;
		val = new Command();
		val.Name = "printtimezones";
		val.Parent = "wipetimer";
		val.FullName = "wipetimer.printtimezones";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			WipeTimer.PrintTimeZones(arg);
		};
		array[1049] = val;
		val = new Command();
		val.Name = "printwipe";
		val.Parent = "wipetimer";
		val.FullName = "wipetimer.printwipe";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			WipeTimer.PrintWipe(arg);
		};
		array[1050] = val;
		val = new Command();
		val.Name = "wipecronoverride";
		val.Parent = "wipetimer";
		val.FullName = "wipetimer.wipecronoverride";
		val.ServerAdmin = true;
		val.Description = "Custom cron expression for the wipe schedule. Overrides all other convars (except wipeUnixTimestampOverride) if set. Uses Cronos as a parser: https://github.com/HangfireIO/Cronos/";
		val.Variable = true;
		val.GetOveride = () => WipeTimer.wipeCronOverride.ToString();
		val.SetOveride = delegate(string str)
		{
			WipeTimer.wipeCronOverride = str;
		};
		array[1051] = val;
		val = new Command();
		val.Name = "wipedayofweek";
		val.Parent = "wipetimer";
		val.FullName = "wipetimer.wipedayofweek";
		val.ServerAdmin = true;
		val.Description = "0=sun,1=mon,2=tues,3=wed,4=thur,5=fri,6=sat";
		val.Variable = true;
		val.GetOveride = () => WipeTimer.wipeDayOfWeek.ToString();
		val.SetOveride = delegate(string str)
		{
			WipeTimer.wipeDayOfWeek = StringExtensions.ToInt(str, 0);
		};
		array[1052] = val;
		val = new Command();
		val.Name = "wipehourofday";
		val.Parent = "wipetimer";
		val.FullName = "wipetimer.wipehourofday";
		val.ServerAdmin = true;
		val.Description = "Which hour to wipe? 14.5 = 2:30pm";
		val.Variable = true;
		val.GetOveride = () => WipeTimer.wipeHourOfDay.ToString();
		val.SetOveride = delegate(string str)
		{
			WipeTimer.wipeHourOfDay = StringExtensions.ToFloat(str, 0f);
		};
		array[1053] = val;
		val = new Command();
		val.Name = "wipetimezone";
		val.Parent = "wipetimer";
		val.FullName = "wipetimer.wipetimezone";
		val.ServerAdmin = true;
		val.Description = "The timezone to use for wipes. Defaults to the server's time zone if not set or invalid. Value should be a TZ identifier as seen here: https://en.wikipedia.org/wiki/List_of_tz_database_time_zones";
		val.Variable = true;
		val.GetOveride = () => WipeTimer.wipeTimezone.ToString();
		val.SetOveride = delegate(string str)
		{
			WipeTimer.wipeTimezone = str;
		};
		array[1054] = val;
		val = new Command();
		val.Name = "wipeunixtimestampoverride";
		val.Parent = "wipetimer";
		val.FullName = "wipetimer.wipeunixtimestampoverride";
		val.ServerAdmin = true;
		val.Description = "Unix timestamp (seconds) for the upcoming wipe. Overrides all other convars if set to a time in the future.";
		val.Variable = true;
		val.GetOveride = () => WipeTimer.wipeUnixTimestampOverride.ToString();
		val.SetOveride = delegate(string str)
		{
			WipeTimer.wipeUnixTimestampOverride = StringExtensions.ToLong(str, 0L);
		};
		array[1055] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "wolf";
		val.FullName = "wolf.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Wolf.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Wolf.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[1056] = val;
		val = new Command();
		val.Name = "playerdetectrange";
		val.Parent = "xmasdungeon";
		val.FullName = "xmasdungeon.playerdetectrange";
		val.ServerAdmin = true;
		val.Description = "How far we detect players from our inside/outside";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => XmasDungeon.playerdetectrange.ToString();
		val.SetOveride = delegate(string str)
		{
			XmasDungeon.playerdetectrange = StringExtensions.ToFloat(str, 0f);
		};
		array[1057] = val;
		val = new Command();
		val.Name = "xmaslifetime";
		val.Parent = "xmasdungeon";
		val.FullName = "xmasdungeon.xmaslifetime";
		val.ServerAdmin = true;
		val.Description = "How long each active dungeon should last before dying";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => XmasDungeon.xmaslifetime.ToString();
		val.SetOveride = delegate(string str)
		{
			XmasDungeon.xmaslifetime = StringExtensions.ToFloat(str, 0f);
		};
		array[1058] = val;
		val = new Command();
		val.Name = "xmaspopulation";
		val.Parent = "xmasdungeon";
		val.FullName = "xmasdungeon.xmaspopulation";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => XmasDungeon.xmaspopulation.ToString();
		val.SetOveride = delegate(string str)
		{
			XmasDungeon.xmaspopulation = StringExtensions.ToFloat(str, 0f);
		};
		array[1059] = val;
		val = new Command();
		val.Name = "report";
		val.Parent = "ziplinelaunchpoint";
		val.FullName = "ziplinelaunchpoint.report";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ZiplineLaunchPoint.report(arg);
		};
		array[1060] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "zombie";
		val.FullName = "zombie.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Zombie.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Zombie.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[1061] = val;
		All = (Command[])(object)array;
	}
}
