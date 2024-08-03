using System.Collections.Generic;
using CompanionServer;
using CompanionServer.Cameras;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Network;
using Facepunch.Rust;
using Facepunch.Rust.Profiling;
using Facepunch.UI;
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
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Expected O, but got Unknown
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Expected O, but got Unknown
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Expected O, but got Unknown
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Expected O, but got Unknown
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Expected O, but got Unknown
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Expected O, but got Unknown
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Expected O, but got Unknown
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Expected O, but got Unknown
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Expected O, but got Unknown
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Expected O, but got Unknown
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Expected O, but got Unknown
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Expected O, but got Unknown
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Expected O, but got Unknown
		//IL_07db: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e1: Expected O, but got Unknown
		//IL_0841: Unknown result type (might be due to invalid IL or missing references)
		//IL_0847: Expected O, but got Unknown
		//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ad: Expected O, but got Unknown
		//IL_090d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0913: Expected O, but got Unknown
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0984: Expected O, but got Unknown
		//IL_09ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f5: Expected O, but got Unknown
		//IL_0a55: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5b: Expected O, but got Unknown
		//IL_0abb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ac1: Expected O, but got Unknown
		//IL_0b2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b32: Expected O, but got Unknown
		//IL_0b92: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b98: Expected O, but got Unknown
		//IL_0bf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfe: Expected O, but got Unknown
		//IL_0c5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c64: Expected O, but got Unknown
		//IL_0cc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cca: Expected O, but got Unknown
		//IL_0d35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d3b: Expected O, but got Unknown
		//IL_0d9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da1: Expected O, but got Unknown
		//IL_0e01: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e07: Expected O, but got Unknown
		//IL_0e72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e78: Expected O, but got Unknown
		//IL_0ee3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ee9: Expected O, but got Unknown
		//IL_0f54: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5a: Expected O, but got Unknown
		//IL_0fcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fd2: Expected O, but got Unknown
		//IL_1032: Unknown result type (might be due to invalid IL or missing references)
		//IL_1038: Expected O, but got Unknown
		//IL_10a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a9: Expected O, but got Unknown
		//IL_111b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1121: Expected O, but got Unknown
		//IL_118c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1192: Expected O, but got Unknown
		//IL_1204: Unknown result type (might be due to invalid IL or missing references)
		//IL_120a: Expected O, but got Unknown
		//IL_127c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1282: Expected O, but got Unknown
		//IL_12ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f3: Expected O, but got Unknown
		//IL_1365: Unknown result type (might be due to invalid IL or missing references)
		//IL_136b: Expected O, but got Unknown
		//IL_13d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d8: Expected O, but got Unknown
		//IL_1422: Unknown result type (might be due to invalid IL or missing references)
		//IL_1428: Expected O, but got Unknown
		//IL_14c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c7: Expected O, but got Unknown
		//IL_1527: Unknown result type (might be due to invalid IL or missing references)
		//IL_152d: Expected O, but got Unknown
		//IL_158d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1593: Expected O, but got Unknown
		//IL_15f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f9: Expected O, but got Unknown
		//IL_1659: Unknown result type (might be due to invalid IL or missing references)
		//IL_165f: Expected O, but got Unknown
		//IL_16bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c5: Expected O, but got Unknown
		//IL_1725: Unknown result type (might be due to invalid IL or missing references)
		//IL_172b: Expected O, but got Unknown
		//IL_178b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1791: Expected O, but got Unknown
		//IL_17f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f7: Expected O, but got Unknown
		//IL_1857: Unknown result type (might be due to invalid IL or missing references)
		//IL_185d: Expected O, but got Unknown
		//IL_18bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c3: Expected O, but got Unknown
		//IL_190d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1913: Expected O, but got Unknown
		//IL_1973: Unknown result type (might be due to invalid IL or missing references)
		//IL_1979: Expected O, but got Unknown
		//IL_19c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19c9: Expected O, but got Unknown
		//IL_1a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a19: Expected O, but got Unknown
		//IL_1a79: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a7f: Expected O, but got Unknown
		//IL_1adf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae5: Expected O, but got Unknown
		//IL_1b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4b: Expected O, but got Unknown
		//IL_1bab: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb1: Expected O, but got Unknown
		//IL_1c11: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c17: Expected O, but got Unknown
		//IL_1c77: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c7d: Expected O, but got Unknown
		//IL_1cdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ce3: Expected O, but got Unknown
		//IL_1d43: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d49: Expected O, but got Unknown
		//IL_1da9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1daf: Expected O, but got Unknown
		//IL_1df9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dff: Expected O, but got Unknown
		//IL_1e49: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e4f: Expected O, but got Unknown
		//IL_1ecf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ed5: Expected O, but got Unknown
		//IL_1f47: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f4d: Expected O, but got Unknown
		//IL_1fa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa8: Expected O, but got Unknown
		//IL_2013: Unknown result type (might be due to invalid IL or missing references)
		//IL_2019: Expected O, but got Unknown
		//IL_2079: Unknown result type (might be due to invalid IL or missing references)
		//IL_207f: Expected O, but got Unknown
		//IL_20df: Unknown result type (might be due to invalid IL or missing references)
		//IL_20e5: Expected O, but got Unknown
		//IL_212f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2135: Expected O, but got Unknown
		//IL_217f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2185: Expected O, but got Unknown
		//IL_21cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_21d5: Expected O, but got Unknown
		//IL_2235: Unknown result type (might be due to invalid IL or missing references)
		//IL_223b: Expected O, but got Unknown
		//IL_229b: Unknown result type (might be due to invalid IL or missing references)
		//IL_22a1: Expected O, but got Unknown
		//IL_2301: Unknown result type (might be due to invalid IL or missing references)
		//IL_2307: Expected O, but got Unknown
		//IL_2367: Unknown result type (might be due to invalid IL or missing references)
		//IL_236d: Expected O, but got Unknown
		//IL_23cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_23d3: Expected O, but got Unknown
		//IL_2433: Unknown result type (might be due to invalid IL or missing references)
		//IL_2439: Expected O, but got Unknown
		//IL_2499: Unknown result type (might be due to invalid IL or missing references)
		//IL_249f: Expected O, but got Unknown
		//IL_24ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_2505: Expected O, but got Unknown
		//IL_2565: Unknown result type (might be due to invalid IL or missing references)
		//IL_256b: Expected O, but got Unknown
		//IL_25cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_25d1: Expected O, but got Unknown
		//IL_2631: Unknown result type (might be due to invalid IL or missing references)
		//IL_2637: Expected O, but got Unknown
		//IL_2697: Unknown result type (might be due to invalid IL or missing references)
		//IL_269d: Expected O, but got Unknown
		//IL_26fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_2703: Expected O, but got Unknown
		//IL_2763: Unknown result type (might be due to invalid IL or missing references)
		//IL_2769: Expected O, but got Unknown
		//IL_27c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_27cf: Expected O, but got Unknown
		//IL_282f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2835: Expected O, but got Unknown
		//IL_287f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2885: Expected O, but got Unknown
		//IL_28cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_28d5: Expected O, but got Unknown
		//IL_291f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2925: Expected O, but got Unknown
		//IL_296f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2975: Expected O, but got Unknown
		//IL_29bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_29c5: Expected O, but got Unknown
		//IL_2a0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a15: Expected O, but got Unknown
		//IL_2a5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a65: Expected O, but got Unknown
		//IL_2aaf: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ab5: Expected O, but got Unknown
		//IL_2b40: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b46: Expected O, but got Unknown
		//IL_2b9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ba1: Expected O, but got Unknown
		//IL_2beb: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bf1: Expected O, but got Unknown
		//IL_2c46: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c4c: Expected O, but got Unknown
		//IL_2ca1: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ca7: Expected O, but got Unknown
		//IL_2cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d02: Expected O, but got Unknown
		//IL_2d57: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d5d: Expected O, but got Unknown
		//IL_2db2: Unknown result type (might be due to invalid IL or missing references)
		//IL_2db8: Expected O, but got Unknown
		//IL_2e0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e13: Expected O, but got Unknown
		//IL_2e68: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e6e: Expected O, but got Unknown
		//IL_2eb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ebe: Expected O, but got Unknown
		//IL_2f08: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f0e: Expected O, but got Unknown
		//IL_2f58: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f5e: Expected O, but got Unknown
		//IL_2fa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fae: Expected O, but got Unknown
		//IL_2ff8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ffe: Expected O, but got Unknown
		//IL_3053: Unknown result type (might be due to invalid IL or missing references)
		//IL_3059: Expected O, but got Unknown
		//IL_30a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_30a9: Expected O, but got Unknown
		//IL_30f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_30f9: Expected O, but got Unknown
		//IL_3143: Unknown result type (might be due to invalid IL or missing references)
		//IL_3149: Expected O, but got Unknown
		//IL_3196: Unknown result type (might be due to invalid IL or missing references)
		//IL_319c: Expected O, but got Unknown
		//IL_31e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_31ef: Expected O, but got Unknown
		//IL_323c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3242: Expected O, but got Unknown
		//IL_329a: Unknown result type (might be due to invalid IL or missing references)
		//IL_32a0: Expected O, but got Unknown
		//IL_32ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_32f3: Expected O, but got Unknown
		//IL_3340: Unknown result type (might be due to invalid IL or missing references)
		//IL_3346: Expected O, but got Unknown
		//IL_339e: Unknown result type (might be due to invalid IL or missing references)
		//IL_33a4: Expected O, but got Unknown
		//IL_33f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_33f7: Expected O, but got Unknown
		//IL_344f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3455: Expected O, but got Unknown
		//IL_34ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_34b3: Expected O, but got Unknown
		//IL_3500: Unknown result type (might be due to invalid IL or missing references)
		//IL_3506: Expected O, but got Unknown
		//IL_3553: Unknown result type (might be due to invalid IL or missing references)
		//IL_3559: Expected O, but got Unknown
		//IL_35a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_35ac: Expected O, but got Unknown
		//IL_3604: Unknown result type (might be due to invalid IL or missing references)
		//IL_360a: Expected O, but got Unknown
		//IL_3662: Unknown result type (might be due to invalid IL or missing references)
		//IL_3668: Expected O, but got Unknown
		//IL_36c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_36c6: Expected O, but got Unknown
		//IL_371e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3724: Expected O, but got Unknown
		//IL_3771: Unknown result type (might be due to invalid IL or missing references)
		//IL_3777: Expected O, but got Unknown
		//IL_37cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_37d5: Expected O, but got Unknown
		//IL_382d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3833: Expected O, but got Unknown
		//IL_388b: Unknown result type (might be due to invalid IL or missing references)
		//IL_3891: Expected O, but got Unknown
		//IL_38e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_38ef: Expected O, but got Unknown
		//IL_3947: Unknown result type (might be due to invalid IL or missing references)
		//IL_394d: Expected O, but got Unknown
		//IL_399a: Unknown result type (might be due to invalid IL or missing references)
		//IL_39a0: Expected O, but got Unknown
		//IL_39ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_39f3: Expected O, but got Unknown
		//IL_3a40: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a46: Expected O, but got Unknown
		//IL_3a9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3aa4: Expected O, but got Unknown
		//IL_3afc: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b02: Expected O, but got Unknown
		//IL_3b5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b60: Expected O, but got Unknown
		//IL_3bb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bbe: Expected O, but got Unknown
		//IL_3c21: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c27: Expected O, but got Unknown
		//IL_3c7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c85: Expected O, but got Unknown
		//IL_3cd2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3cd8: Expected O, but got Unknown
		//IL_3d62: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d68: Expected O, but got Unknown
		//IL_3dd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ddc: Expected O, but got Unknown
		//IL_3e29: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e2f: Expected O, but got Unknown
		//IL_3e87: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e8d: Expected O, but got Unknown
		//IL_3ef0: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ef6: Expected O, but got Unknown
		//IL_3f59: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f5f: Expected O, but got Unknown
		//IL_3fc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_3fc8: Expected O, but got Unknown
		//IL_4015: Unknown result type (might be due to invalid IL or missing references)
		//IL_401b: Expected O, but got Unknown
		//IL_4068: Unknown result type (might be due to invalid IL or missing references)
		//IL_406e: Expected O, but got Unknown
		//IL_40d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_40d7: Expected O, but got Unknown
		//IL_4145: Unknown result type (might be due to invalid IL or missing references)
		//IL_414b: Expected O, but got Unknown
		//IL_41b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_41bf: Expected O, but got Unknown
		//IL_422d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4233: Expected O, but got Unknown
		//IL_42a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_42a7: Expected O, but got Unknown
		//IL_4315: Unknown result type (might be due to invalid IL or missing references)
		//IL_431b: Expected O, but got Unknown
		//IL_437e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4384: Expected O, but got Unknown
		//IL_43f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_43f8: Expected O, but got Unknown
		//IL_4466: Unknown result type (might be due to invalid IL or missing references)
		//IL_446c: Expected O, but got Unknown
		//IL_44da: Unknown result type (might be due to invalid IL or missing references)
		//IL_44e0: Expected O, but got Unknown
		//IL_454e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4554: Expected O, but got Unknown
		//IL_45c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_45c8: Expected O, but got Unknown
		//IL_4636: Unknown result type (might be due to invalid IL or missing references)
		//IL_463c: Expected O, but got Unknown
		//IL_46aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_46b0: Expected O, but got Unknown
		//IL_471e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4724: Expected O, but got Unknown
		//IL_4792: Unknown result type (might be due to invalid IL or missing references)
		//IL_4798: Expected O, but got Unknown
		//IL_4806: Unknown result type (might be due to invalid IL or missing references)
		//IL_480c: Expected O, but got Unknown
		//IL_487a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4880: Expected O, but got Unknown
		//IL_48ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_48f4: Expected O, but got Unknown
		//IL_4962: Unknown result type (might be due to invalid IL or missing references)
		//IL_4968: Expected O, but got Unknown
		//IL_49d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_49dc: Expected O, but got Unknown
		//IL_4a4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a50: Expected O, but got Unknown
		//IL_4abe: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ac4: Expected O, but got Unknown
		//IL_4b32: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b38: Expected O, but got Unknown
		//IL_4ba6: Unknown result type (might be due to invalid IL or missing references)
		//IL_4bac: Expected O, but got Unknown
		//IL_4c1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c20: Expected O, but got Unknown
		//IL_4c8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c94: Expected O, but got Unknown
		//IL_4d02: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d08: Expected O, but got Unknown
		//IL_4d76: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d7c: Expected O, but got Unknown
		//IL_4dea: Unknown result type (might be due to invalid IL or missing references)
		//IL_4df0: Expected O, but got Unknown
		//IL_4e5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e64: Expected O, but got Unknown
		//IL_4ed2: Unknown result type (might be due to invalid IL or missing references)
		//IL_4ed8: Expected O, but got Unknown
		//IL_4f46: Unknown result type (might be due to invalid IL or missing references)
		//IL_4f4c: Expected O, but got Unknown
		//IL_4fba: Unknown result type (might be due to invalid IL or missing references)
		//IL_4fc0: Expected O, but got Unknown
		//IL_502e: Unknown result type (might be due to invalid IL or missing references)
		//IL_5034: Expected O, but got Unknown
		//IL_50a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_50a8: Expected O, but got Unknown
		//IL_5116: Unknown result type (might be due to invalid IL or missing references)
		//IL_511c: Expected O, but got Unknown
		//IL_518a: Unknown result type (might be due to invalid IL or missing references)
		//IL_5190: Expected O, but got Unknown
		//IL_51fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_5204: Expected O, but got Unknown
		//IL_5272: Unknown result type (might be due to invalid IL or missing references)
		//IL_5278: Expected O, but got Unknown
		//IL_52e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_52ec: Expected O, but got Unknown
		//IL_535a: Unknown result type (might be due to invalid IL or missing references)
		//IL_5360: Expected O, but got Unknown
		//IL_53ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_53d4: Expected O, but got Unknown
		//IL_5442: Unknown result type (might be due to invalid IL or missing references)
		//IL_5448: Expected O, but got Unknown
		//IL_54b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_54bc: Expected O, but got Unknown
		//IL_552a: Unknown result type (might be due to invalid IL or missing references)
		//IL_5530: Expected O, but got Unknown
		//IL_559e: Unknown result type (might be due to invalid IL or missing references)
		//IL_55a4: Expected O, but got Unknown
		//IL_5612: Unknown result type (might be due to invalid IL or missing references)
		//IL_5618: Expected O, but got Unknown
		//IL_5686: Unknown result type (might be due to invalid IL or missing references)
		//IL_568c: Expected O, but got Unknown
		//IL_56ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_56f5: Expected O, but got Unknown
		//IL_5758: Unknown result type (might be due to invalid IL or missing references)
		//IL_575e: Expected O, but got Unknown
		//IL_57b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_57bc: Expected O, but got Unknown
		//IL_5814: Unknown result type (might be due to invalid IL or missing references)
		//IL_581a: Expected O, but got Unknown
		//IL_5867: Unknown result type (might be due to invalid IL or missing references)
		//IL_586d: Expected O, but got Unknown
		//IL_58d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_58d6: Expected O, but got Unknown
		//IL_5939: Unknown result type (might be due to invalid IL or missing references)
		//IL_593f: Expected O, but got Unknown
		//IL_59a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_59a8: Expected O, but got Unknown
		//IL_59f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_59fb: Expected O, but got Unknown
		//IL_5a5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_5a64: Expected O, but got Unknown
		//IL_5ac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_5acd: Expected O, but got Unknown
		//IL_5b30: Unknown result type (might be due to invalid IL or missing references)
		//IL_5b36: Expected O, but got Unknown
		//IL_5b99: Unknown result type (might be due to invalid IL or missing references)
		//IL_5b9f: Expected O, but got Unknown
		//IL_5c02: Unknown result type (might be due to invalid IL or missing references)
		//IL_5c08: Expected O, but got Unknown
		//IL_5c6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_5c71: Expected O, but got Unknown
		//IL_5cbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_5cc4: Expected O, but got Unknown
		//IL_5d27: Unknown result type (might be due to invalid IL or missing references)
		//IL_5d2d: Expected O, but got Unknown
		//IL_5d90: Unknown result type (might be due to invalid IL or missing references)
		//IL_5d96: Expected O, but got Unknown
		//IL_5df9: Unknown result type (might be due to invalid IL or missing references)
		//IL_5dff: Expected O, but got Unknown
		//IL_5e62: Unknown result type (might be due to invalid IL or missing references)
		//IL_5e68: Expected O, but got Unknown
		//IL_5ecb: Unknown result type (might be due to invalid IL or missing references)
		//IL_5ed1: Expected O, but got Unknown
		//IL_5f34: Unknown result type (might be due to invalid IL or missing references)
		//IL_5f3a: Expected O, but got Unknown
		//IL_5f9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_5fa3: Expected O, but got Unknown
		//IL_6006: Unknown result type (might be due to invalid IL or missing references)
		//IL_600c: Expected O, but got Unknown
		//IL_606f: Unknown result type (might be due to invalid IL or missing references)
		//IL_6075: Expected O, but got Unknown
		//IL_60d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_60de: Expected O, but got Unknown
		//IL_6141: Unknown result type (might be due to invalid IL or missing references)
		//IL_6147: Expected O, but got Unknown
		//IL_61aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_61b0: Expected O, but got Unknown
		//IL_6213: Unknown result type (might be due to invalid IL or missing references)
		//IL_6219: Expected O, but got Unknown
		//IL_627c: Unknown result type (might be due to invalid IL or missing references)
		//IL_6282: Expected O, but got Unknown
		//IL_62e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_62eb: Expected O, but got Unknown
		//IL_634e: Unknown result type (might be due to invalid IL or missing references)
		//IL_6354: Expected O, but got Unknown
		//IL_63b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_63bd: Expected O, but got Unknown
		//IL_6420: Unknown result type (might be due to invalid IL or missing references)
		//IL_6426: Expected O, but got Unknown
		//IL_6489: Unknown result type (might be due to invalid IL or missing references)
		//IL_648f: Expected O, but got Unknown
		//IL_64f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_64f8: Expected O, but got Unknown
		//IL_655b: Unknown result type (might be due to invalid IL or missing references)
		//IL_6561: Expected O, but got Unknown
		//IL_65c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_65ca: Expected O, but got Unknown
		//IL_662d: Unknown result type (might be due to invalid IL or missing references)
		//IL_6633: Expected O, but got Unknown
		//IL_6696: Unknown result type (might be due to invalid IL or missing references)
		//IL_669c: Expected O, but got Unknown
		//IL_66ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_6705: Expected O, but got Unknown
		//IL_6768: Unknown result type (might be due to invalid IL or missing references)
		//IL_676e: Expected O, but got Unknown
		//IL_67d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_67d7: Expected O, but got Unknown
		//IL_683a: Unknown result type (might be due to invalid IL or missing references)
		//IL_6840: Expected O, but got Unknown
		//IL_68a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_68a9: Expected O, but got Unknown
		//IL_690c: Unknown result type (might be due to invalid IL or missing references)
		//IL_6912: Expected O, but got Unknown
		//IL_6975: Unknown result type (might be due to invalid IL or missing references)
		//IL_697b: Expected O, but got Unknown
		//IL_69de: Unknown result type (might be due to invalid IL or missing references)
		//IL_69e4: Expected O, but got Unknown
		//IL_6a47: Unknown result type (might be due to invalid IL or missing references)
		//IL_6a4d: Expected O, but got Unknown
		//IL_6ab0: Unknown result type (might be due to invalid IL or missing references)
		//IL_6ab6: Expected O, but got Unknown
		//IL_6b19: Unknown result type (might be due to invalid IL or missing references)
		//IL_6b1f: Expected O, but got Unknown
		//IL_6b82: Unknown result type (might be due to invalid IL or missing references)
		//IL_6b88: Expected O, but got Unknown
		//IL_6beb: Unknown result type (might be due to invalid IL or missing references)
		//IL_6bf1: Expected O, but got Unknown
		//IL_6c54: Unknown result type (might be due to invalid IL or missing references)
		//IL_6c5a: Expected O, but got Unknown
		//IL_6cbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_6cc3: Expected O, but got Unknown
		//IL_6d26: Unknown result type (might be due to invalid IL or missing references)
		//IL_6d2c: Expected O, but got Unknown
		//IL_6d8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_6d95: Expected O, but got Unknown
		//IL_6df8: Unknown result type (might be due to invalid IL or missing references)
		//IL_6dfe: Expected O, but got Unknown
		//IL_6e61: Unknown result type (might be due to invalid IL or missing references)
		//IL_6e67: Expected O, but got Unknown
		//IL_6eca: Unknown result type (might be due to invalid IL or missing references)
		//IL_6ed0: Expected O, but got Unknown
		//IL_6f33: Unknown result type (might be due to invalid IL or missing references)
		//IL_6f39: Expected O, but got Unknown
		//IL_6f9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_6fa2: Expected O, but got Unknown
		//IL_7005: Unknown result type (might be due to invalid IL or missing references)
		//IL_700b: Expected O, but got Unknown
		//IL_706e: Unknown result type (might be due to invalid IL or missing references)
		//IL_7074: Expected O, but got Unknown
		//IL_70f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_70fd: Expected O, but got Unknown
		//IL_7160: Unknown result type (might be due to invalid IL or missing references)
		//IL_7166: Expected O, but got Unknown
		//IL_71e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_71ef: Expected O, but got Unknown
		//IL_7252: Unknown result type (might be due to invalid IL or missing references)
		//IL_7258: Expected O, but got Unknown
		//IL_72bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_72c1: Expected O, but got Unknown
		//IL_7324: Unknown result type (might be due to invalid IL or missing references)
		//IL_732a: Expected O, but got Unknown
		//IL_738d: Unknown result type (might be due to invalid IL or missing references)
		//IL_7393: Expected O, but got Unknown
		//IL_73f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_73fc: Expected O, but got Unknown
		//IL_745f: Unknown result type (might be due to invalid IL or missing references)
		//IL_7465: Expected O, but got Unknown
		//IL_74c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_74ce: Expected O, but got Unknown
		//IL_7531: Unknown result type (might be due to invalid IL or missing references)
		//IL_7537: Expected O, but got Unknown
		//IL_759a: Unknown result type (might be due to invalid IL or missing references)
		//IL_75a0: Expected O, but got Unknown
		//IL_7603: Unknown result type (might be due to invalid IL or missing references)
		//IL_7609: Expected O, but got Unknown
		//IL_766c: Unknown result type (might be due to invalid IL or missing references)
		//IL_7672: Expected O, but got Unknown
		//IL_76d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_76db: Expected O, but got Unknown
		//IL_773e: Unknown result type (might be due to invalid IL or missing references)
		//IL_7744: Expected O, but got Unknown
		//IL_77a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_77ad: Expected O, but got Unknown
		//IL_7810: Unknown result type (might be due to invalid IL or missing references)
		//IL_7816: Expected O, but got Unknown
		//IL_7879: Unknown result type (might be due to invalid IL or missing references)
		//IL_787f: Expected O, but got Unknown
		//IL_78e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_78e8: Expected O, but got Unknown
		//IL_794b: Unknown result type (might be due to invalid IL or missing references)
		//IL_7951: Expected O, but got Unknown
		//IL_79b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_79ba: Expected O, but got Unknown
		//IL_7a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_7a23: Expected O, but got Unknown
		//IL_7a86: Unknown result type (might be due to invalid IL or missing references)
		//IL_7a8c: Expected O, but got Unknown
		//IL_7aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_7af5: Expected O, but got Unknown
		//IL_7b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_7b5e: Expected O, but got Unknown
		//IL_7bc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_7bc7: Expected O, but got Unknown
		//IL_7c2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_7c30: Expected O, but got Unknown
		//IL_7c93: Unknown result type (might be due to invalid IL or missing references)
		//IL_7c99: Expected O, but got Unknown
		//IL_7cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_7d02: Expected O, but got Unknown
		//IL_7d65: Unknown result type (might be due to invalid IL or missing references)
		//IL_7d6b: Expected O, but got Unknown
		//IL_7dce: Unknown result type (might be due to invalid IL or missing references)
		//IL_7dd4: Expected O, but got Unknown
		//IL_7e37: Unknown result type (might be due to invalid IL or missing references)
		//IL_7e3d: Expected O, but got Unknown
		//IL_7ea0: Unknown result type (might be due to invalid IL or missing references)
		//IL_7ea6: Expected O, but got Unknown
		//IL_7f09: Unknown result type (might be due to invalid IL or missing references)
		//IL_7f0f: Expected O, but got Unknown
		//IL_7f72: Unknown result type (might be due to invalid IL or missing references)
		//IL_7f78: Expected O, but got Unknown
		//IL_7fdb: Unknown result type (might be due to invalid IL or missing references)
		//IL_7fe1: Expected O, but got Unknown
		//IL_8044: Unknown result type (might be due to invalid IL or missing references)
		//IL_804a: Expected O, but got Unknown
		//IL_80ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_80b3: Expected O, but got Unknown
		//IL_8116: Unknown result type (might be due to invalid IL or missing references)
		//IL_811c: Expected O, but got Unknown
		//IL_817f: Unknown result type (might be due to invalid IL or missing references)
		//IL_8185: Expected O, but got Unknown
		//IL_81e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_81ee: Expected O, but got Unknown
		//IL_8251: Unknown result type (might be due to invalid IL or missing references)
		//IL_8257: Expected O, but got Unknown
		//IL_82ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_82c0: Expected O, but got Unknown
		//IL_8323: Unknown result type (might be due to invalid IL or missing references)
		//IL_8329: Expected O, but got Unknown
		//IL_838c: Unknown result type (might be due to invalid IL or missing references)
		//IL_8392: Expected O, but got Unknown
		//IL_83f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_83fb: Expected O, but got Unknown
		//IL_845e: Unknown result type (might be due to invalid IL or missing references)
		//IL_8464: Expected O, but got Unknown
		//IL_84c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_84cd: Expected O, but got Unknown
		//IL_853b: Unknown result type (might be due to invalid IL or missing references)
		//IL_8541: Expected O, but got Unknown
		//IL_858e: Unknown result type (might be due to invalid IL or missing references)
		//IL_8594: Expected O, but got Unknown
		//IL_85e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_85e7: Expected O, but got Unknown
		//IL_8634: Unknown result type (might be due to invalid IL or missing references)
		//IL_863a: Expected O, but got Unknown
		//IL_8687: Unknown result type (might be due to invalid IL or missing references)
		//IL_868d: Expected O, but got Unknown
		//IL_86f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_86f6: Expected O, but got Unknown
		//IL_8759: Unknown result type (might be due to invalid IL or missing references)
		//IL_875f: Expected O, but got Unknown
		//IL_87c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_87c8: Expected O, but got Unknown
		//IL_882b: Unknown result type (might be due to invalid IL or missing references)
		//IL_8831: Expected O, but got Unknown
		//IL_889f: Unknown result type (might be due to invalid IL or missing references)
		//IL_88a5: Expected O, but got Unknown
		//IL_88f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_88f8: Expected O, but got Unknown
		//IL_895b: Unknown result type (might be due to invalid IL or missing references)
		//IL_8961: Expected O, but got Unknown
		//IL_89c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_89ca: Expected O, but got Unknown
		//IL_8a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_8a3e: Expected O, but got Unknown
		//IL_8a8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_8a91: Expected O, but got Unknown
		//IL_8ade: Unknown result type (might be due to invalid IL or missing references)
		//IL_8ae4: Expected O, but got Unknown
		//IL_8b3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_8b42: Expected O, but got Unknown
		//IL_8bc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_8bcb: Expected O, but got Unknown
		//IL_8c39: Unknown result type (might be due to invalid IL or missing references)
		//IL_8c3f: Expected O, but got Unknown
		//IL_8ca2: Unknown result type (might be due to invalid IL or missing references)
		//IL_8ca8: Expected O, but got Unknown
		//IL_8d0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_8d11: Expected O, but got Unknown
		//IL_8d5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_8d64: Expected O, but got Unknown
		//IL_8dc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_8dcd: Expected O, but got Unknown
		//IL_8e30: Unknown result type (might be due to invalid IL or missing references)
		//IL_8e36: Expected O, but got Unknown
		//IL_8e83: Unknown result type (might be due to invalid IL or missing references)
		//IL_8e89: Expected O, but got Unknown
		//IL_8ed6: Unknown result type (might be due to invalid IL or missing references)
		//IL_8edc: Expected O, but got Unknown
		//IL_8f3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_8f45: Expected O, but got Unknown
		//IL_8fc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_8fce: Expected O, but got Unknown
		//IL_9051: Unknown result type (might be due to invalid IL or missing references)
		//IL_9057: Expected O, but got Unknown
		//IL_90c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_90cb: Expected O, but got Unknown
		//IL_914e: Unknown result type (might be due to invalid IL or missing references)
		//IL_9154: Expected O, but got Unknown
		//IL_91b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_91bd: Expected O, but got Unknown
		//IL_920a: Unknown result type (might be due to invalid IL or missing references)
		//IL_9210: Expected O, but got Unknown
		//IL_925d: Unknown result type (might be due to invalid IL or missing references)
		//IL_9263: Expected O, but got Unknown
		//IL_92b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_92b6: Expected O, but got Unknown
		//IL_9319: Unknown result type (might be due to invalid IL or missing references)
		//IL_931f: Expected O, but got Unknown
		//IL_936c: Unknown result type (might be due to invalid IL or missing references)
		//IL_9372: Expected O, but got Unknown
		//IL_93bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_93c5: Expected O, but got Unknown
		//IL_9453: Unknown result type (might be due to invalid IL or missing references)
		//IL_9459: Expected O, but got Unknown
		//IL_94c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_94cd: Expected O, but got Unknown
		//IL_9525: Unknown result type (might be due to invalid IL or missing references)
		//IL_952b: Expected O, but got Unknown
		//IL_9599: Unknown result type (might be due to invalid IL or missing references)
		//IL_959f: Expected O, but got Unknown
		//IL_960d: Unknown result type (might be due to invalid IL or missing references)
		//IL_9613: Expected O, but got Unknown
		//IL_9681: Unknown result type (might be due to invalid IL or missing references)
		//IL_9687: Expected O, but got Unknown
		//IL_96f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_96fb: Expected O, but got Unknown
		//IL_9769: Unknown result type (might be due to invalid IL or missing references)
		//IL_976f: Expected O, but got Unknown
		//IL_97dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_97e3: Expected O, but got Unknown
		//IL_9851: Unknown result type (might be due to invalid IL or missing references)
		//IL_9857: Expected O, but got Unknown
		//IL_98c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_98cb: Expected O, but got Unknown
		//IL_9939: Unknown result type (might be due to invalid IL or missing references)
		//IL_993f: Expected O, but got Unknown
		//IL_99ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_99b3: Expected O, but got Unknown
		//IL_9a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_9a27: Expected O, but got Unknown
		//IL_9a74: Unknown result type (might be due to invalid IL or missing references)
		//IL_9a7a: Expected O, but got Unknown
		//IL_9ac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_9acd: Expected O, but got Unknown
		//IL_9b30: Unknown result type (might be due to invalid IL or missing references)
		//IL_9b36: Expected O, but got Unknown
		//IL_9b83: Unknown result type (might be due to invalid IL or missing references)
		//IL_9b89: Expected O, but got Unknown
		//IL_9bd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_9bdc: Expected O, but got Unknown
		//IL_9c29: Unknown result type (might be due to invalid IL or missing references)
		//IL_9c2f: Expected O, but got Unknown
		//IL_9c7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_9c82: Expected O, but got Unknown
		//IL_9ce5: Unknown result type (might be due to invalid IL or missing references)
		//IL_9ceb: Expected O, but got Unknown
		//IL_9d80: Unknown result type (might be due to invalid IL or missing references)
		//IL_9d86: Expected O, but got Unknown
		//IL_9e1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_9e21: Expected O, but got Unknown
		//IL_9eb6: Unknown result type (might be due to invalid IL or missing references)
		//IL_9ebc: Expected O, but got Unknown
		//IL_9f51: Unknown result type (might be due to invalid IL or missing references)
		//IL_9f57: Expected O, but got Unknown
		//IL_9fa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_9faa: Expected O, but got Unknown
		//IL_a03f: Unknown result type (might be due to invalid IL or missing references)
		//IL_a045: Expected O, but got Unknown
		//IL_a092: Unknown result type (might be due to invalid IL or missing references)
		//IL_a098: Expected O, but got Unknown
		//IL_a0f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_a0f6: Expected O, but got Unknown
		//IL_a14e: Unknown result type (might be due to invalid IL or missing references)
		//IL_a154: Expected O, but got Unknown
		//IL_a1ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_a1b2: Expected O, but got Unknown
		//IL_a20a: Unknown result type (might be due to invalid IL or missing references)
		//IL_a210: Expected O, but got Unknown
		//IL_a273: Unknown result type (might be due to invalid IL or missing references)
		//IL_a279: Expected O, but got Unknown
		//IL_a2dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_a2e2: Expected O, but got Unknown
		//IL_a345: Unknown result type (might be due to invalid IL or missing references)
		//IL_a34b: Expected O, but got Unknown
		//IL_a3a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_a3a9: Expected O, but got Unknown
		//IL_a3f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_a3fc: Expected O, but got Unknown
		//IL_a449: Unknown result type (might be due to invalid IL or missing references)
		//IL_a44f: Expected O, but got Unknown
		//IL_a49c: Unknown result type (might be due to invalid IL or missing references)
		//IL_a4a2: Expected O, but got Unknown
		//IL_a505: Unknown result type (might be due to invalid IL or missing references)
		//IL_a50b: Expected O, but got Unknown
		//IL_a558: Unknown result type (might be due to invalid IL or missing references)
		//IL_a55e: Expected O, but got Unknown
		//IL_a5cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_a5d2: Expected O, but got Unknown
		//IL_a61f: Unknown result type (might be due to invalid IL or missing references)
		//IL_a625: Expected O, but got Unknown
		//IL_a672: Unknown result type (might be due to invalid IL or missing references)
		//IL_a678: Expected O, but got Unknown
		//IL_a6c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_a6cb: Expected O, but got Unknown
		//IL_a723: Unknown result type (might be due to invalid IL or missing references)
		//IL_a729: Expected O, but got Unknown
		//IL_a776: Unknown result type (might be due to invalid IL or missing references)
		//IL_a77c: Expected O, but got Unknown
		//IL_a7c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_a7cf: Expected O, but got Unknown
		//IL_a832: Unknown result type (might be due to invalid IL or missing references)
		//IL_a838: Expected O, but got Unknown
		//IL_a885: Unknown result type (might be due to invalid IL or missing references)
		//IL_a88b: Expected O, but got Unknown
		//IL_a8ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_a8f4: Expected O, but got Unknown
		//IL_a94c: Unknown result type (might be due to invalid IL or missing references)
		//IL_a952: Expected O, but got Unknown
		//IL_a99f: Unknown result type (might be due to invalid IL or missing references)
		//IL_a9a5: Expected O, but got Unknown
		//IL_a9f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_a9f8: Expected O, but got Unknown
		//IL_aa45: Unknown result type (might be due to invalid IL or missing references)
		//IL_aa4b: Expected O, but got Unknown
		//IL_aaa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_aaa9: Expected O, but got Unknown
		//IL_aaf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_aafc: Expected O, but got Unknown
		//IL_ab49: Unknown result type (might be due to invalid IL or missing references)
		//IL_ab4f: Expected O, but got Unknown
		//IL_ab9c: Unknown result type (might be due to invalid IL or missing references)
		//IL_aba2: Expected O, but got Unknown
		//IL_abef: Unknown result type (might be due to invalid IL or missing references)
		//IL_abf5: Expected O, but got Unknown
		//IL_ac42: Unknown result type (might be due to invalid IL or missing references)
		//IL_ac48: Expected O, but got Unknown
		//IL_ac95: Unknown result type (might be due to invalid IL or missing references)
		//IL_ac9b: Expected O, but got Unknown
		//IL_ace8: Unknown result type (might be due to invalid IL or missing references)
		//IL_acee: Expected O, but got Unknown
		//IL_ad3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_ad41: Expected O, but got Unknown
		//IL_ad8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_ad94: Expected O, but got Unknown
		//IL_ade1: Unknown result type (might be due to invalid IL or missing references)
		//IL_ade7: Expected O, but got Unknown
		//IL_ae4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_ae50: Expected O, but got Unknown
		//IL_ae9d: Unknown result type (might be due to invalid IL or missing references)
		//IL_aea3: Expected O, but got Unknown
		//IL_af11: Unknown result type (might be due to invalid IL or missing references)
		//IL_af17: Expected O, but got Unknown
		//IL_af85: Unknown result type (might be due to invalid IL or missing references)
		//IL_af8b: Expected O, but got Unknown
		//IL_aff9: Unknown result type (might be due to invalid IL or missing references)
		//IL_afff: Expected O, but got Unknown
		//IL_b06d: Unknown result type (might be due to invalid IL or missing references)
		//IL_b073: Expected O, but got Unknown
		//IL_b0e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_b0e7: Expected O, but got Unknown
		//IL_b155: Unknown result type (might be due to invalid IL or missing references)
		//IL_b15b: Expected O, but got Unknown
		//IL_b1c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_b1cf: Expected O, but got Unknown
		//IL_b23d: Unknown result type (might be due to invalid IL or missing references)
		//IL_b243: Expected O, but got Unknown
		//IL_b2a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_b2ac: Expected O, but got Unknown
		//IL_b31a: Unknown result type (might be due to invalid IL or missing references)
		//IL_b320: Expected O, but got Unknown
		//IL_b38e: Unknown result type (might be due to invalid IL or missing references)
		//IL_b394: Expected O, but got Unknown
		//IL_b402: Unknown result type (might be due to invalid IL or missing references)
		//IL_b408: Expected O, but got Unknown
		//IL_b476: Unknown result type (might be due to invalid IL or missing references)
		//IL_b47c: Expected O, but got Unknown
		//IL_b4ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_b4f0: Expected O, but got Unknown
		//IL_b55e: Unknown result type (might be due to invalid IL or missing references)
		//IL_b564: Expected O, but got Unknown
		//IL_b5d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_b5d8: Expected O, but got Unknown
		//IL_b646: Unknown result type (might be due to invalid IL or missing references)
		//IL_b64c: Expected O, but got Unknown
		//IL_b6ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_b6c0: Expected O, but got Unknown
		//IL_b72e: Unknown result type (might be due to invalid IL or missing references)
		//IL_b734: Expected O, but got Unknown
		//IL_b7a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_b7a8: Expected O, but got Unknown
		//IL_b816: Unknown result type (might be due to invalid IL or missing references)
		//IL_b81c: Expected O, but got Unknown
		//IL_b88a: Unknown result type (might be due to invalid IL or missing references)
		//IL_b890: Expected O, but got Unknown
		//IL_b8f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_b8f9: Expected O, but got Unknown
		//IL_b95c: Unknown result type (might be due to invalid IL or missing references)
		//IL_b962: Expected O, but got Unknown
		//IL_b9d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_b9d6: Expected O, but got Unknown
		//IL_ba44: Unknown result type (might be due to invalid IL or missing references)
		//IL_ba4a: Expected O, but got Unknown
		//IL_bab8: Unknown result type (might be due to invalid IL or missing references)
		//IL_babe: Expected O, but got Unknown
		//IL_bb2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_bb32: Expected O, but got Unknown
		//IL_bba0: Unknown result type (might be due to invalid IL or missing references)
		//IL_bba6: Expected O, but got Unknown
		//IL_bbf3: Unknown result type (might be due to invalid IL or missing references)
		//IL_bbf9: Expected O, but got Unknown
		//IL_bc6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_bc74: Expected O, but got Unknown
		//IL_bce9: Unknown result type (might be due to invalid IL or missing references)
		//IL_bcef: Expected O, but got Unknown
		//IL_bd5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_bd63: Expected O, but got Unknown
		//IL_bdd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_bdde: Expected O, but got Unknown
		//IL_be2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_be31: Expected O, but got Unknown
		//IL_be9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_bea1: Expected O, but got Unknown
		//IL_bf16: Unknown result type (might be due to invalid IL or missing references)
		//IL_bf1c: Expected O, but got Unknown
		//IL_bf8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_bf90: Expected O, but got Unknown
		//IL_bffa: Unknown result type (might be due to invalid IL or missing references)
		//IL_c000: Expected O, but got Unknown
		//IL_c06a: Unknown result type (might be due to invalid IL or missing references)
		//IL_c070: Expected O, but got Unknown
		//IL_c0d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_c0d9: Expected O, but got Unknown
		//IL_c13c: Unknown result type (might be due to invalid IL or missing references)
		//IL_c142: Expected O, but got Unknown
		//IL_c18f: Unknown result type (might be due to invalid IL or missing references)
		//IL_c195: Expected O, but got Unknown
		//IL_c1f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_c1fe: Expected O, but got Unknown
		//IL_c268: Unknown result type (might be due to invalid IL or missing references)
		//IL_c26e: Expected O, but got Unknown
		//IL_c2d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_c2de: Expected O, but got Unknown
		//IL_c353: Unknown result type (might be due to invalid IL or missing references)
		//IL_c359: Expected O, but got Unknown
		//IL_c3a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_c3ac: Expected O, but got Unknown
		//IL_c404: Unknown result type (might be due to invalid IL or missing references)
		//IL_c40a: Expected O, but got Unknown
		//IL_c462: Unknown result type (might be due to invalid IL or missing references)
		//IL_c468: Expected O, but got Unknown
		//IL_c4b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_c4bb: Expected O, but got Unknown
		//IL_c508: Unknown result type (might be due to invalid IL or missing references)
		//IL_c50e: Expected O, but got Unknown
		//IL_c55b: Unknown result type (might be due to invalid IL or missing references)
		//IL_c561: Expected O, but got Unknown
		//IL_c5ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_c5b4: Expected O, but got Unknown
		//IL_c601: Unknown result type (might be due to invalid IL or missing references)
		//IL_c607: Expected O, but got Unknown
		//IL_c654: Unknown result type (might be due to invalid IL or missing references)
		//IL_c65a: Expected O, but got Unknown
		//IL_c6a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_c6ad: Expected O, but got Unknown
		//IL_c6fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_c700: Expected O, but got Unknown
		//IL_c74d: Unknown result type (might be due to invalid IL or missing references)
		//IL_c753: Expected O, but got Unknown
		//IL_c7a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_c7a6: Expected O, but got Unknown
		//IL_c7f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_c7f9: Expected O, but got Unknown
		//IL_c846: Unknown result type (might be due to invalid IL or missing references)
		//IL_c84c: Expected O, but got Unknown
		//IL_c899: Unknown result type (might be due to invalid IL or missing references)
		//IL_c89f: Expected O, but got Unknown
		//IL_c902: Unknown result type (might be due to invalid IL or missing references)
		//IL_c908: Expected O, but got Unknown
		//IL_c96b: Unknown result type (might be due to invalid IL or missing references)
		//IL_c971: Expected O, but got Unknown
		//IL_c9f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_c9fa: Expected O, but got Unknown
		//IL_ca7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_ca83: Expected O, but got Unknown
		//IL_cb06: Unknown result type (might be due to invalid IL or missing references)
		//IL_cb0c: Expected O, but got Unknown
		//IL_cb8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_cb95: Expected O, but got Unknown
		//IL_cbf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_cbfe: Expected O, but got Unknown
		//IL_cc68: Unknown result type (might be due to invalid IL or missing references)
		//IL_cc6e: Expected O, but got Unknown
		//IL_ccd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_ccd7: Expected O, but got Unknown
		//IL_cd41: Unknown result type (might be due to invalid IL or missing references)
		//IL_cd47: Expected O, but got Unknown
		//IL_cd94: Unknown result type (might be due to invalid IL or missing references)
		//IL_cd9a: Expected O, but got Unknown
		//IL_cde7: Unknown result type (might be due to invalid IL or missing references)
		//IL_cded: Expected O, but got Unknown
		//IL_ce3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_ce40: Expected O, but got Unknown
		//IL_ce8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_ce93: Expected O, but got Unknown
		//IL_cef6: Unknown result type (might be due to invalid IL or missing references)
		//IL_cefc: Expected O, but got Unknown
		//IL_cf5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_cf65: Expected O, but got Unknown
		//IL_cfc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_cfce: Expected O, but got Unknown
		//IL_d01b: Unknown result type (might be due to invalid IL or missing references)
		//IL_d021: Expected O, but got Unknown
		//IL_d084: Unknown result type (might be due to invalid IL or missing references)
		//IL_d08a: Expected O, but got Unknown
		//IL_d0d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_d0dd: Expected O, but got Unknown
		//IL_d12a: Unknown result type (might be due to invalid IL or missing references)
		//IL_d130: Expected O, but got Unknown
		//IL_d1b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_d1b6: Expected O, but got Unknown
		//IL_d203: Unknown result type (might be due to invalid IL or missing references)
		//IL_d209: Expected O, but got Unknown
		//IL_d256: Unknown result type (might be due to invalid IL or missing references)
		//IL_d25c: Expected O, but got Unknown
		//IL_d2a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_d2af: Expected O, but got Unknown
		//IL_d2fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_d302: Expected O, but got Unknown
		//IL_d34f: Unknown result type (might be due to invalid IL or missing references)
		//IL_d355: Expected O, but got Unknown
		//IL_d3a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_d3a8: Expected O, but got Unknown
		//IL_d40b: Unknown result type (might be due to invalid IL or missing references)
		//IL_d411: Expected O, but got Unknown
		//IL_d47f: Unknown result type (might be due to invalid IL or missing references)
		//IL_d485: Expected O, but got Unknown
		//IL_d4d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_d4d8: Expected O, but got Unknown
		//IL_d53b: Unknown result type (might be due to invalid IL or missing references)
		//IL_d541: Expected O, but got Unknown
		//IL_d58e: Unknown result type (might be due to invalid IL or missing references)
		//IL_d594: Expected O, but got Unknown
		//IL_d5e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_d5e7: Expected O, but got Unknown
		//IL_d64a: Unknown result type (might be due to invalid IL or missing references)
		//IL_d650: Expected O, but got Unknown
		//IL_d69d: Unknown result type (might be due to invalid IL or missing references)
		//IL_d6a3: Expected O, but got Unknown
		//IL_d71f: Unknown result type (might be due to invalid IL or missing references)
		//IL_d725: Expected O, but got Unknown
		//IL_d788: Unknown result type (might be due to invalid IL or missing references)
		//IL_d78e: Expected O, but got Unknown
		//IL_d7db: Unknown result type (might be due to invalid IL or missing references)
		//IL_d7e1: Expected O, but got Unknown
		//IL_d84b: Unknown result type (might be due to invalid IL or missing references)
		//IL_d851: Expected O, but got Unknown
		//IL_d8b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_d8ba: Expected O, but got Unknown
		//IL_d907: Unknown result type (might be due to invalid IL or missing references)
		//IL_d90d: Expected O, but got Unknown
		//IL_d95a: Unknown result type (might be due to invalid IL or missing references)
		//IL_d960: Expected O, but got Unknown
		//IL_d9ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_d9b3: Expected O, but got Unknown
		//IL_da00: Unknown result type (might be due to invalid IL or missing references)
		//IL_da06: Expected O, but got Unknown
		//IL_da53: Unknown result type (might be due to invalid IL or missing references)
		//IL_da59: Expected O, but got Unknown
		//IL_daa6: Unknown result type (might be due to invalid IL or missing references)
		//IL_daac: Expected O, but got Unknown
		//IL_daf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_daff: Expected O, but got Unknown
		//IL_db4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_db52: Expected O, but got Unknown
		//IL_db9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_dba5: Expected O, but got Unknown
		//IL_dbf2: Unknown result type (might be due to invalid IL or missing references)
		//IL_dbf8: Expected O, but got Unknown
		//IL_dc5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_dc61: Expected O, but got Unknown
		//IL_dcae: Unknown result type (might be due to invalid IL or missing references)
		//IL_dcb4: Expected O, but got Unknown
		//IL_dd01: Unknown result type (might be due to invalid IL or missing references)
		//IL_dd07: Expected O, but got Unknown
		//IL_dd54: Unknown result type (might be due to invalid IL or missing references)
		//IL_dd5a: Expected O, but got Unknown
		//IL_dda7: Unknown result type (might be due to invalid IL or missing references)
		//IL_ddad: Expected O, but got Unknown
		//IL_de29: Unknown result type (might be due to invalid IL or missing references)
		//IL_de2f: Expected O, but got Unknown
		//IL_deab: Unknown result type (might be due to invalid IL or missing references)
		//IL_deb1: Expected O, but got Unknown
		//IL_defe: Unknown result type (might be due to invalid IL or missing references)
		//IL_df04: Expected O, but got Unknown
		//IL_df51: Unknown result type (might be due to invalid IL or missing references)
		//IL_df57: Expected O, but got Unknown
		//IL_dfa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_dfaa: Expected O, but got Unknown
		//IL_dff7: Unknown result type (might be due to invalid IL or missing references)
		//IL_dffd: Expected O, but got Unknown
		//IL_e04a: Unknown result type (might be due to invalid IL or missing references)
		//IL_e050: Expected O, but got Unknown
		//IL_e09d: Unknown result type (might be due to invalid IL or missing references)
		//IL_e0a3: Expected O, but got Unknown
		//IL_e0f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_e0f6: Expected O, but got Unknown
		//IL_e143: Unknown result type (might be due to invalid IL or missing references)
		//IL_e149: Expected O, but got Unknown
		//IL_e196: Unknown result type (might be due to invalid IL or missing references)
		//IL_e19c: Expected O, but got Unknown
		//IL_e1e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_e1ef: Expected O, but got Unknown
		//IL_e23c: Unknown result type (might be due to invalid IL or missing references)
		//IL_e242: Expected O, but got Unknown
		//IL_e28f: Unknown result type (might be due to invalid IL or missing references)
		//IL_e295: Expected O, but got Unknown
		//IL_e2e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_e2e8: Expected O, but got Unknown
		//IL_e335: Unknown result type (might be due to invalid IL or missing references)
		//IL_e33b: Expected O, but got Unknown
		//IL_e388: Unknown result type (might be due to invalid IL or missing references)
		//IL_e38e: Expected O, but got Unknown
		//IL_e3db: Unknown result type (might be due to invalid IL or missing references)
		//IL_e3e1: Expected O, but got Unknown
		//IL_e42e: Unknown result type (might be due to invalid IL or missing references)
		//IL_e434: Expected O, but got Unknown
		//IL_e481: Unknown result type (might be due to invalid IL or missing references)
		//IL_e487: Expected O, but got Unknown
		//IL_e4e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_e4ec: Expected O, but got Unknown
		//IL_e54f: Unknown result type (might be due to invalid IL or missing references)
		//IL_e555: Expected O, but got Unknown
		//IL_e5a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_e5af: Expected O, but got Unknown
		//IL_e612: Unknown result type (might be due to invalid IL or missing references)
		//IL_e618: Expected O, but got Unknown
		//IL_e67b: Unknown result type (might be due to invalid IL or missing references)
		//IL_e681: Expected O, but got Unknown
		//IL_e6ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_e6f5: Expected O, but got Unknown
		//IL_e763: Unknown result type (might be due to invalid IL or missing references)
		//IL_e769: Expected O, but got Unknown
		//IL_e7d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_e7dd: Expected O, but got Unknown
		//IL_e84b: Unknown result type (might be due to invalid IL or missing references)
		//IL_e851: Expected O, but got Unknown
		//IL_e8bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_e8c5: Expected O, but got Unknown
		//IL_e933: Unknown result type (might be due to invalid IL or missing references)
		//IL_e939: Expected O, but got Unknown
		//IL_e9a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_e9ad: Expected O, but got Unknown
		//IL_e9fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_ea00: Expected O, but got Unknown
		//IL_ea4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_ea53: Expected O, but got Unknown
		//IL_eaa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_eaa6: Expected O, but got Unknown
		//IL_eaf3: Unknown result type (might be due to invalid IL or missing references)
		//IL_eaf9: Expected O, but got Unknown
		//IL_eb46: Unknown result type (might be due to invalid IL or missing references)
		//IL_eb4c: Expected O, but got Unknown
		//IL_eba4: Unknown result type (might be due to invalid IL or missing references)
		//IL_ebaa: Expected O, but got Unknown
		//IL_ec02: Unknown result type (might be due to invalid IL or missing references)
		//IL_ec08: Expected O, but got Unknown
		//IL_ec55: Unknown result type (might be due to invalid IL or missing references)
		//IL_ec5b: Expected O, but got Unknown
		//IL_ecb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_ecb9: Expected O, but got Unknown
		//IL_ed11: Unknown result type (might be due to invalid IL or missing references)
		//IL_ed17: Expected O, but got Unknown
		//IL_eda5: Unknown result type (might be due to invalid IL or missing references)
		//IL_edab: Expected O, but got Unknown
		//IL_edf8: Unknown result type (might be due to invalid IL or missing references)
		//IL_edfe: Expected O, but got Unknown
		//IL_ee4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_ee51: Expected O, but got Unknown
		//IL_ee9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_eea4: Expected O, but got Unknown
		//IL_eefc: Unknown result type (might be due to invalid IL or missing references)
		//IL_ef02: Expected O, but got Unknown
		//IL_ef4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_ef55: Expected O, but got Unknown
		//IL_efad: Unknown result type (might be due to invalid IL or missing references)
		//IL_efb3: Expected O, but got Unknown
		//IL_f000: Unknown result type (might be due to invalid IL or missing references)
		//IL_f006: Expected O, but got Unknown
		//IL_f05e: Unknown result type (might be due to invalid IL or missing references)
		//IL_f064: Expected O, but got Unknown
		//IL_f0bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_f0c2: Expected O, but got Unknown
		//IL_f10f: Unknown result type (might be due to invalid IL or missing references)
		//IL_f115: Expected O, but got Unknown
		//IL_f16d: Unknown result type (might be due to invalid IL or missing references)
		//IL_f173: Expected O, but got Unknown
		//IL_f1c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_f1c6: Expected O, but got Unknown
		//IL_f213: Unknown result type (might be due to invalid IL or missing references)
		//IL_f219: Expected O, but got Unknown
		//IL_f266: Unknown result type (might be due to invalid IL or missing references)
		//IL_f26c: Expected O, but got Unknown
		//IL_f2c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_f2ca: Expected O, but got Unknown
		//IL_f317: Unknown result type (might be due to invalid IL or missing references)
		//IL_f31d: Expected O, but got Unknown
		//IL_f36a: Unknown result type (might be due to invalid IL or missing references)
		//IL_f370: Expected O, but got Unknown
		//IL_f3bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_f3c3: Expected O, but got Unknown
		//IL_f410: Unknown result type (might be due to invalid IL or missing references)
		//IL_f416: Expected O, but got Unknown
		//IL_f463: Unknown result type (might be due to invalid IL or missing references)
		//IL_f469: Expected O, but got Unknown
		//IL_f4b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_f4bc: Expected O, but got Unknown
		//IL_f52a: Unknown result type (might be due to invalid IL or missing references)
		//IL_f530: Expected O, but got Unknown
		//IL_f593: Unknown result type (might be due to invalid IL or missing references)
		//IL_f599: Expected O, but got Unknown
		//IL_f607: Unknown result type (might be due to invalid IL or missing references)
		//IL_f60d: Expected O, but got Unknown
		//IL_f67b: Unknown result type (might be due to invalid IL or missing references)
		//IL_f681: Expected O, but got Unknown
		//IL_f6e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_f6ea: Expected O, but got Unknown
		//IL_f74d: Unknown result type (might be due to invalid IL or missing references)
		//IL_f753: Expected O, but got Unknown
		//IL_f7b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_f7bc: Expected O, but got Unknown
		//IL_f809: Unknown result type (might be due to invalid IL or missing references)
		//IL_f80f: Expected O, but got Unknown
		//IL_f87d: Unknown result type (might be due to invalid IL or missing references)
		//IL_f883: Expected O, but got Unknown
		//IL_f8f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_f8f7: Expected O, but got Unknown
		//IL_f985: Unknown result type (might be due to invalid IL or missing references)
		//IL_f98b: Expected O, but got Unknown
		//IL_f9f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_f9ff: Expected O, but got Unknown
		//IL_fa6d: Unknown result type (might be due to invalid IL or missing references)
		//IL_fa73: Expected O, but got Unknown
		//IL_fadd: Unknown result type (might be due to invalid IL or missing references)
		//IL_fae3: Expected O, but got Unknown
		//IL_fb51: Unknown result type (might be due to invalid IL or missing references)
		//IL_fb57: Expected O, but got Unknown
		//IL_fbc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_fbcb: Expected O, but got Unknown
		//IL_fc18: Unknown result type (might be due to invalid IL or missing references)
		//IL_fc1e: Expected O, but got Unknown
		//IL_fc8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_fc92: Expected O, but got Unknown
		//IL_fd00: Unknown result type (might be due to invalid IL or missing references)
		//IL_fd06: Expected O, but got Unknown
		//IL_fd53: Unknown result type (might be due to invalid IL or missing references)
		//IL_fd59: Expected O, but got Unknown
		//IL_fdc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_fdcd: Expected O, but got Unknown
		//IL_fe1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_fe20: Expected O, but got Unknown
		//IL_fe8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_fe94: Expected O, but got Unknown
		//IL_fef7: Unknown result type (might be due to invalid IL or missing references)
		//IL_fefd: Expected O, but got Unknown
		//IL_ff6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_ff71: Expected O, but got Unknown
		//IL_ffbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_ffc4: Expected O, but got Unknown
		//IL_10032: Unknown result type (might be due to invalid IL or missing references)
		//IL_10038: Expected O, but got Unknown
		//IL_10090: Unknown result type (might be due to invalid IL or missing references)
		//IL_10096: Expected O, but got Unknown
		//IL_100f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_100ff: Expected O, but got Unknown
		//IL_10162: Unknown result type (might be due to invalid IL or missing references)
		//IL_10168: Expected O, but got Unknown
		//IL_101cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_101d1: Expected O, but got Unknown
		//IL_1021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_10224: Expected O, but got Unknown
		//IL_10271: Unknown result type (might be due to invalid IL or missing references)
		//IL_10277: Expected O, but got Unknown
		//IL_102c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_102ca: Expected O, but got Unknown
		//IL_10317: Unknown result type (might be due to invalid IL or missing references)
		//IL_1031d: Expected O, but got Unknown
		//IL_1036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_10370: Expected O, but got Unknown
		//IL_103d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_103d9: Expected O, but got Unknown
		//IL_1043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_10442: Expected O, but got Unknown
		//IL_1048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_10495: Expected O, but got Unknown
		//IL_104e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_104e8: Expected O, but got Unknown
		//IL_10535: Unknown result type (might be due to invalid IL or missing references)
		//IL_1053b: Expected O, but got Unknown
		//IL_10588: Unknown result type (might be due to invalid IL or missing references)
		//IL_1058e: Expected O, but got Unknown
		//IL_105db: Unknown result type (might be due to invalid IL or missing references)
		//IL_105e1: Expected O, but got Unknown
		//IL_1062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_10634: Expected O, but got Unknown
		//IL_10697: Unknown result type (might be due to invalid IL or missing references)
		//IL_1069d: Expected O, but got Unknown
		//IL_10700: Unknown result type (might be due to invalid IL or missing references)
		//IL_10706: Expected O, but got Unknown
		//IL_10769: Unknown result type (might be due to invalid IL or missing references)
		//IL_1076f: Expected O, but got Unknown
		//IL_107dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_107e3: Expected O, but got Unknown
		//IL_10871: Unknown result type (might be due to invalid IL or missing references)
		//IL_10877: Expected O, but got Unknown
		//IL_108da: Unknown result type (might be due to invalid IL or missing references)
		//IL_108e0: Expected O, but got Unknown
		//IL_10943: Unknown result type (might be due to invalid IL or missing references)
		//IL_10949: Expected O, but got Unknown
		//IL_109ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_109b2: Expected O, but got Unknown
		//IL_10a20: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a26: Expected O, but got Unknown
		//IL_10a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a9a: Expected O, but got Unknown
		//IL_10b36: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b3c: Expected O, but got Unknown
		//IL_10b9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ba5: Expected O, but got Unknown
		//IL_10c13: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c19: Expected O, but got Unknown
		//IL_10cb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cbb: Expected O, but got Unknown
		//IL_10d1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d24: Expected O, but got Unknown
		//IL_10d87: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d8d: Expected O, but got Unknown
		//IL_10df0: Unknown result type (might be due to invalid IL or missing references)
		//IL_10df6: Expected O, but got Unknown
		//IL_10e59: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e5f: Expected O, but got Unknown
		//IL_10ec2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ec8: Expected O, but got Unknown
		//IL_10f2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f31: Expected O, but got Unknown
		//IL_10f7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f84: Expected O, but got Unknown
		//IL_10fd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fd7: Expected O, but got Unknown
		//IL_11024: Unknown result type (might be due to invalid IL or missing references)
		//IL_1102a: Expected O, but got Unknown
		//IL_11077: Unknown result type (might be due to invalid IL or missing references)
		//IL_1107d: Expected O, but got Unknown
		//IL_110ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_110d0: Expected O, but got Unknown
		//IL_1111d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11123: Expected O, but got Unknown
		//IL_11170: Unknown result type (might be due to invalid IL or missing references)
		//IL_11176: Expected O, but got Unknown
		//IL_111c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_111c9: Expected O, but got Unknown
		//IL_11216: Unknown result type (might be due to invalid IL or missing references)
		//IL_1121c: Expected O, but got Unknown
		//IL_11269: Unknown result type (might be due to invalid IL or missing references)
		//IL_1126f: Expected O, but got Unknown
		//IL_112bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_112c2: Expected O, but got Unknown
		//IL_1130f: Unknown result type (might be due to invalid IL or missing references)
		//IL_11315: Expected O, but got Unknown
		//IL_11362: Unknown result type (might be due to invalid IL or missing references)
		//IL_11368: Expected O, but got Unknown
		//IL_113b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_113bb: Expected O, but got Unknown
		//IL_11408: Unknown result type (might be due to invalid IL or missing references)
		//IL_1140e: Expected O, but got Unknown
		//IL_1145b: Unknown result type (might be due to invalid IL or missing references)
		//IL_11461: Expected O, but got Unknown
		//IL_114ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_114b4: Expected O, but got Unknown
		//IL_1150c: Unknown result type (might be due to invalid IL or missing references)
		//IL_11512: Expected O, but got Unknown
		//IL_1155f: Unknown result type (might be due to invalid IL or missing references)
		//IL_11565: Expected O, but got Unknown
		//IL_115b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_115b8: Expected O, but got Unknown
		//IL_1163b: Unknown result type (might be due to invalid IL or missing references)
		//IL_11641: Expected O, but got Unknown
		//IL_116c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_116ca: Expected O, but got Unknown
		//IL_11717: Unknown result type (might be due to invalid IL or missing references)
		//IL_1171d: Expected O, but got Unknown
		//IL_1176a: Unknown result type (might be due to invalid IL or missing references)
		//IL_11770: Expected O, but got Unknown
		//IL_117bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_117c3: Expected O, but got Unknown
		//IL_1183f: Unknown result type (might be due to invalid IL or missing references)
		//IL_11845: Expected O, but got Unknown
		//IL_11892: Unknown result type (might be due to invalid IL or missing references)
		//IL_11898: Expected O, but got Unknown
		//IL_118e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_118eb: Expected O, but got Unknown
		//IL_11938: Unknown result type (might be due to invalid IL or missing references)
		//IL_1193e: Expected O, but got Unknown
		//IL_119a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_119a7: Expected O, but got Unknown
		//IL_11a0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a10: Expected O, but got Unknown
		//IL_11a5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a63: Expected O, but got Unknown
		//IL_11ab0: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ab6: Expected O, but got Unknown
		//IL_11b19: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b1f: Expected O, but got Unknown
		//IL_11b82: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b88: Expected O, but got Unknown
		//IL_11bd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_11bdb: Expected O, but got Unknown
		//IL_11c28: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c2e: Expected O, but got Unknown
		//IL_11c7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c81: Expected O, but got Unknown
		//IL_11cce: Unknown result type (might be due to invalid IL or missing references)
		//IL_11cd4: Expected O, but got Unknown
		//IL_11d21: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d27: Expected O, but got Unknown
		//IL_11d74: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d7a: Expected O, but got Unknown
		//IL_11dc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_11dcd: Expected O, but got Unknown
		//IL_11e25: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e2b: Expected O, but got Unknown
		//IL_11eae: Unknown result type (might be due to invalid IL or missing references)
		//IL_11eb4: Expected O, but got Unknown
		//IL_11f01: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f07: Expected O, but got Unknown
		//IL_11f75: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f7b: Expected O, but got Unknown
		//IL_11fe9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11fef: Expected O, but got Unknown
		//IL_1207d: Unknown result type (might be due to invalid IL or missing references)
		//IL_12083: Expected O, but got Unknown
		//IL_12111: Unknown result type (might be due to invalid IL or missing references)
		//IL_12117: Expected O, but got Unknown
		//IL_12185: Unknown result type (might be due to invalid IL or missing references)
		//IL_1218b: Expected O, but got Unknown
		//IL_121ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_121f4: Expected O, but got Unknown
		//IL_12257: Unknown result type (might be due to invalid IL or missing references)
		//IL_1225d: Expected O, but got Unknown
		//IL_122c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_122c6: Expected O, but got Unknown
		//IL_12329: Unknown result type (might be due to invalid IL or missing references)
		//IL_1232f: Expected O, but got Unknown
		//IL_12399: Unknown result type (might be due to invalid IL or missing references)
		//IL_1239f: Expected O, but got Unknown
		//IL_12409: Unknown result type (might be due to invalid IL or missing references)
		//IL_1240f: Expected O, but got Unknown
		//IL_12479: Unknown result type (might be due to invalid IL or missing references)
		//IL_1247f: Expected O, but got Unknown
		//IL_124e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_124e8: Expected O, but got Unknown
		//IL_12564: Unknown result type (might be due to invalid IL or missing references)
		//IL_1256a: Expected O, but got Unknown
		//IL_125c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_125c8: Expected O, but got Unknown
		//IL_1264b: Unknown result type (might be due to invalid IL or missing references)
		//IL_12651: Expected O, but got Unknown
		//IL_126bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_126c5: Expected O, but got Unknown
		//IL_12733: Unknown result type (might be due to invalid IL or missing references)
		//IL_12739: Expected O, but got Unknown
		//IL_127a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_127ad: Expected O, but got Unknown
		//IL_12817: Unknown result type (might be due to invalid IL or missing references)
		//IL_1281d: Expected O, but got Unknown
		//IL_12887: Unknown result type (might be due to invalid IL or missing references)
		//IL_1288d: Expected O, but got Unknown
		//IL_128f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_128f6: Expected O, but got Unknown
		//IL_12943: Unknown result type (might be due to invalid IL or missing references)
		//IL_12949: Expected O, but got Unknown
		//IL_129b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_129b9: Expected O, but got Unknown
		//IL_12a23: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a29: Expected O, but got Unknown
		//IL_12aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12aab: Expected O, but got Unknown
		//IL_12b15: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b1b: Expected O, but got Unknown
		//IL_12b85: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b8b: Expected O, but got Unknown
		//IL_12bf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12bff: Expected O, but got Unknown
		//IL_12c4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c52: Expected O, but got Unknown
		//IL_12cb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cbb: Expected O, but got Unknown
		//IL_12d1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d20: Expected O, but got Unknown
		//IL_12d7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d85: Expected O, but got Unknown
		//IL_12de8: Unknown result type (might be due to invalid IL or missing references)
		//IL_12dee: Expected O, but got Unknown
		//IL_12e51: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e57: Expected O, but got Unknown
		//IL_12eba: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ec0: Expected O, but got Unknown
		//IL_12f23: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f29: Expected O, but got Unknown
		//IL_12fa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_12fab: Expected O, but got Unknown
		//IL_1300e: Unknown result type (might be due to invalid IL or missing references)
		//IL_13014: Expected O, but got Unknown
		//IL_1306c: Unknown result type (might be due to invalid IL or missing references)
		//IL_13072: Expected O, but got Unknown
		//IL_130d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_130db: Expected O, but got Unknown
		//IL_13150: Unknown result type (might be due to invalid IL or missing references)
		//IL_13156: Expected O, but got Unknown
		//IL_131c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_131ca: Expected O, but got Unknown
		//IL_13238: Unknown result type (might be due to invalid IL or missing references)
		//IL_1323e: Expected O, but got Unknown
		//IL_132a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_132a7: Expected O, but got Unknown
		//IL_1330a: Unknown result type (might be due to invalid IL or missing references)
		//IL_13310: Expected O, but got Unknown
		//IL_133ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_133b2: Expected O, but got Unknown
		//IL_1341c: Unknown result type (might be due to invalid IL or missing references)
		//IL_13422: Expected O, but got Unknown
		//IL_13485: Unknown result type (might be due to invalid IL or missing references)
		//IL_1348b: Expected O, but got Unknown
		//IL_134ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_134f4: Expected O, but got Unknown
		//IL_13570: Unknown result type (might be due to invalid IL or missing references)
		//IL_13576: Expected O, but got Unknown
		//IL_135d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_135df: Expected O, but got Unknown
		//IL_13642: Unknown result type (might be due to invalid IL or missing references)
		//IL_13648: Expected O, but got Unknown
		//IL_136ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_136b1: Expected O, but got Unknown
		//IL_13714: Unknown result type (might be due to invalid IL or missing references)
		//IL_1371a: Expected O, but got Unknown
		//IL_13796: Unknown result type (might be due to invalid IL or missing references)
		//IL_1379c: Expected O, but got Unknown
		//IL_137e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_137ef: Expected O, but got Unknown
		//IL_13879: Unknown result type (might be due to invalid IL or missing references)
		//IL_1387f: Expected O, but got Unknown
		//IL_13909: Unknown result type (might be due to invalid IL or missing references)
		//IL_1390f: Expected O, but got Unknown
		//IL_13972: Unknown result type (might be due to invalid IL or missing references)
		//IL_13978: Expected O, but got Unknown
		//IL_139e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_139ef: Expected O, but got Unknown
		//IL_13a59: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a5f: Expected O, but got Unknown
		//IL_13ac2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ac8: Expected O, but got Unknown
		//IL_13b32: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b38: Expected O, but got Unknown
		//IL_13b9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ba1: Expected O, but got Unknown
		//IL_13c04: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c0a: Expected O, but got Unknown
		//IL_13c7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c85: Expected O, but got Unknown
		//IL_13cf3: Unknown result type (might be due to invalid IL or missing references)
		//IL_13cf9: Expected O, but got Unknown
		//IL_13d75: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d7b: Expected O, but got Unknown
		//IL_13df7: Unknown result type (might be due to invalid IL or missing references)
		//IL_13dfd: Expected O, but got Unknown
		//IL_13e72: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e78: Expected O, but got Unknown
		//IL_13edb: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ee1: Expected O, but got Unknown
		//IL_13f44: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f4a: Expected O, but got Unknown
		//IL_13fad: Unknown result type (might be due to invalid IL or missing references)
		//IL_13fb3: Expected O, but got Unknown
		//IL_14016: Unknown result type (might be due to invalid IL or missing references)
		//IL_1401c: Expected O, but got Unknown
		//IL_1407f: Unknown result type (might be due to invalid IL or missing references)
		//IL_14085: Expected O, but got Unknown
		//IL_140e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_140ee: Expected O, but got Unknown
		//IL_14151: Unknown result type (might be due to invalid IL or missing references)
		//IL_14157: Expected O, but got Unknown
		//IL_141ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_141c0: Expected O, but got Unknown
		//IL_14223: Unknown result type (might be due to invalid IL or missing references)
		//IL_14229: Expected O, but got Unknown
		//IL_14281: Unknown result type (might be due to invalid IL or missing references)
		//IL_14287: Expected O, but got Unknown
		//IL_142df: Unknown result type (might be due to invalid IL or missing references)
		//IL_142e5: Expected O, but got Unknown
		//IL_14356: Unknown result type (might be due to invalid IL or missing references)
		//IL_1435c: Expected O, but got Unknown
		//IL_143df: Unknown result type (might be due to invalid IL or missing references)
		//IL_143e5: Expected O, but got Unknown
		//IL_14468: Unknown result type (might be due to invalid IL or missing references)
		//IL_1446e: Expected O, but got Unknown
		//IL_144d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_144d7: Expected O, but got Unknown
		//IL_1453a: Unknown result type (might be due to invalid IL or missing references)
		//IL_14540: Expected O, but got Unknown
		//IL_145a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_145a9: Expected O, but got Unknown
		//IL_1460c: Unknown result type (might be due to invalid IL or missing references)
		//IL_14612: Expected O, but got Unknown
		//IL_14675: Unknown result type (might be due to invalid IL or missing references)
		//IL_1467b: Expected O, but got Unknown
		//IL_14717: Unknown result type (might be due to invalid IL or missing references)
		//IL_1471d: Expected O, but got Unknown
		//IL_14799: Unknown result type (might be due to invalid IL or missing references)
		//IL_1479f: Expected O, but got Unknown
		//IL_1481b: Unknown result type (might be due to invalid IL or missing references)
		//IL_14821: Expected O, but got Unknown
		//IL_14884: Unknown result type (might be due to invalid IL or missing references)
		//IL_1488a: Expected O, but got Unknown
		//IL_148ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_148f3: Expected O, but got Unknown
		//IL_14956: Unknown result type (might be due to invalid IL or missing references)
		//IL_1495c: Expected O, but got Unknown
		//IL_149bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_149c5: Expected O, but got Unknown
		//IL_14a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a2e: Expected O, but got Unknown
		//IL_14a91: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a97: Expected O, but got Unknown
		//IL_14afa: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b00: Expected O, but got Unknown
		//IL_14b63: Unknown result type (might be due to invalid IL or missing references)
		//IL_14b69: Expected O, but got Unknown
		//IL_14bcc: Unknown result type (might be due to invalid IL or missing references)
		//IL_14bd2: Expected O, but got Unknown
		//IL_14c35: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c3b: Expected O, but got Unknown
		//IL_14c9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ca4: Expected O, but got Unknown
		//IL_14d0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d14: Expected O, but got Unknown
		//IL_14d77: Unknown result type (might be due to invalid IL or missing references)
		//IL_14d7d: Expected O, but got Unknown
		//IL_14de0: Unknown result type (might be due to invalid IL or missing references)
		//IL_14de6: Expected O, but got Unknown
		//IL_14e49: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e4f: Expected O, but got Unknown
		//IL_14eb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_14eb8: Expected O, but got Unknown
		//IL_14f1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f21: Expected O, but got Unknown
		//IL_14f84: Unknown result type (might be due to invalid IL or missing references)
		//IL_14f8a: Expected O, but got Unknown
		//IL_14fed: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ff3: Expected O, but got Unknown
		//IL_15056: Unknown result type (might be due to invalid IL or missing references)
		//IL_1505c: Expected O, but got Unknown
		//IL_150c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_150cc: Expected O, but got Unknown
		//IL_15136: Unknown result type (might be due to invalid IL or missing references)
		//IL_1513c: Expected O, but got Unknown
		//IL_1519f: Unknown result type (might be due to invalid IL or missing references)
		//IL_151a5: Expected O, but got Unknown
		//IL_15208: Unknown result type (might be due to invalid IL or missing references)
		//IL_1520e: Expected O, but got Unknown
		//IL_1529f: Unknown result type (might be due to invalid IL or missing references)
		//IL_152a5: Expected O, but got Unknown
		//IL_15308: Unknown result type (might be due to invalid IL or missing references)
		//IL_1530e: Expected O, but got Unknown
		//IL_15371: Unknown result type (might be due to invalid IL or missing references)
		//IL_15377: Expected O, but got Unknown
		//IL_153da: Unknown result type (might be due to invalid IL or missing references)
		//IL_153e0: Expected O, but got Unknown
		//IL_1542d: Unknown result type (might be due to invalid IL or missing references)
		//IL_15433: Expected O, but got Unknown
		//IL_1549d: Unknown result type (might be due to invalid IL or missing references)
		//IL_154a3: Expected O, but got Unknown
		//IL_15506: Unknown result type (might be due to invalid IL or missing references)
		//IL_1550c: Expected O, but got Unknown
		//IL_1556f: Unknown result type (might be due to invalid IL or missing references)
		//IL_15575: Expected O, but got Unknown
		//IL_155df: Unknown result type (might be due to invalid IL or missing references)
		//IL_155e5: Expected O, but got Unknown
		//IL_1564f: Unknown result type (might be due to invalid IL or missing references)
		//IL_15655: Expected O, but got Unknown
		//IL_156bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_156c5: Expected O, but got Unknown
		//IL_15712: Unknown result type (might be due to invalid IL or missing references)
		//IL_15718: Expected O, but got Unknown
		//IL_1577b: Unknown result type (might be due to invalid IL or missing references)
		//IL_15781: Expected O, but got Unknown
		//IL_1581d: Unknown result type (might be due to invalid IL or missing references)
		//IL_15823: Expected O, but got Unknown
		//IL_15886: Unknown result type (might be due to invalid IL or missing references)
		//IL_1588c: Expected O, but got Unknown
		//IL_15908: Unknown result type (might be due to invalid IL or missing references)
		//IL_1590e: Expected O, but got Unknown
		//IL_15971: Unknown result type (might be due to invalid IL or missing references)
		//IL_15977: Expected O, but got Unknown
		//IL_159fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a00: Expected O, but got Unknown
		//IL_15a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_15a69: Expected O, but got Unknown
		//IL_15ab6: Unknown result type (might be due to invalid IL or missing references)
		//IL_15abc: Expected O, but got Unknown
		//IL_15b09: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b0f: Expected O, but got Unknown
		//IL_15b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_15b78: Expected O, but got Unknown
		//IL_15bd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_15bd6: Expected O, but got Unknown
		//IL_15c40: Unknown result type (might be due to invalid IL or missing references)
		//IL_15c46: Expected O, but got Unknown
		//IL_15ca9: Unknown result type (might be due to invalid IL or missing references)
		//IL_15caf: Expected O, but got Unknown
		//IL_15d12: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d18: Expected O, but got Unknown
		//IL_15d65: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d6b: Expected O, but got Unknown
		//IL_15dc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_15dc9: Expected O, but got Unknown
		//IL_15e21: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e27: Expected O, but got Unknown
		//IL_15e74: Unknown result type (might be due to invalid IL or missing references)
		//IL_15e7a: Expected O, but got Unknown
		//IL_15eef: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ef5: Expected O, but got Unknown
		//IL_15f4d: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f53: Expected O, but got Unknown
		//IL_15fa0: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fa6: Expected O, but got Unknown
		//IL_16009: Unknown result type (might be due to invalid IL or missing references)
		//IL_1600f: Expected O, but got Unknown
		//IL_16072: Unknown result type (might be due to invalid IL or missing references)
		//IL_16078: Expected O, but got Unknown
		//IL_160db: Unknown result type (might be due to invalid IL or missing references)
		//IL_160e1: Expected O, but got Unknown
		//IL_1614b: Unknown result type (might be due to invalid IL or missing references)
		//IL_16151: Expected O, but got Unknown
		//IL_1619e: Unknown result type (might be due to invalid IL or missing references)
		//IL_161a4: Expected O, but got Unknown
		//IL_16207: Unknown result type (might be due to invalid IL or missing references)
		//IL_1620d: Expected O, but got Unknown
		//IL_16282: Unknown result type (might be due to invalid IL or missing references)
		//IL_16288: Expected O, but got Unknown
		//IL_162fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_16303: Expected O, but got Unknown
		//IL_1635b: Unknown result type (might be due to invalid IL or missing references)
		//IL_16361: Expected O, but got Unknown
		//IL_163cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_163d5: Expected O, but got Unknown
		//IL_16438: Unknown result type (might be due to invalid IL or missing references)
		//IL_1643e: Expected O, but got Unknown
		//IL_164ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_164b2: Expected O, but got Unknown
		//IL_1651c: Unknown result type (might be due to invalid IL or missing references)
		//IL_16522: Expected O, but got Unknown
		//IL_1656f: Unknown result type (might be due to invalid IL or missing references)
		//IL_16575: Expected O, but got Unknown
		//IL_165d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_165de: Expected O, but got Unknown
		//IL_16641: Unknown result type (might be due to invalid IL or missing references)
		//IL_16647: Expected O, but got Unknown
		//IL_1669f: Unknown result type (might be due to invalid IL or missing references)
		//IL_166a5: Expected O, but got Unknown
		//IL_16716: Unknown result type (might be due to invalid IL or missing references)
		//IL_1671c: Expected O, but got Unknown
		//IL_1677f: Unknown result type (might be due to invalid IL or missing references)
		//IL_16785: Expected O, but got Unknown
		//IL_167e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_167ee: Expected O, but got Unknown
		//IL_16851: Unknown result type (might be due to invalid IL or missing references)
		//IL_16857: Expected O, but got Unknown
		//IL_168ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_168c0: Expected O, but got Unknown
		//IL_16923: Unknown result type (might be due to invalid IL or missing references)
		//IL_16929: Expected O, but got Unknown
		//IL_16981: Unknown result type (might be due to invalid IL or missing references)
		//IL_16987: Expected O, but got Unknown
		//IL_169f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_169f7: Expected O, but got Unknown
		//IL_16a4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a55: Expected O, but got Unknown
		//IL_16abf: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ac5: Expected O, but got Unknown
		//IL_16b53: Unknown result type (might be due to invalid IL or missing references)
		//IL_16b59: Expected O, but got Unknown
		//IL_16bb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_16bb7: Expected O, but got Unknown
		//IL_16c21: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c27: Expected O, but got Unknown
		//IL_16c91: Unknown result type (might be due to invalid IL or missing references)
		//IL_16c97: Expected O, but got Unknown
		//IL_16cfa: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d00: Expected O, but got Unknown
		//IL_16d58: Unknown result type (might be due to invalid IL or missing references)
		//IL_16d5e: Expected O, but got Unknown
		//IL_16dc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_16dc7: Expected O, but got Unknown
		//IL_16e2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e30: Expected O, but got Unknown
		//IL_16e88: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e8e: Expected O, but got Unknown
		//IL_16ef1: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ef7: Expected O, but got Unknown
		//IL_16f5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_16f60: Expected O, but got Unknown
		//IL_16fdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_16fe2: Expected O, but got Unknown
		//IL_17045: Unknown result type (might be due to invalid IL or missing references)
		//IL_1704b: Expected O, but got Unknown
		//IL_170e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_170ed: Expected O, but got Unknown
		//IL_17150: Unknown result type (might be due to invalid IL or missing references)
		//IL_17156: Expected O, but got Unknown
		//IL_171b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_171bf: Expected O, but got Unknown
		//IL_17229: Unknown result type (might be due to invalid IL or missing references)
		//IL_1722f: Expected O, but got Unknown
		//IL_17292: Unknown result type (might be due to invalid IL or missing references)
		//IL_17298: Expected O, but got Unknown
		//IL_1730d: Unknown result type (might be due to invalid IL or missing references)
		//IL_17313: Expected O, but got Unknown
		//IL_173a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_173ae: Expected O, but got Unknown
		//IL_17443: Unknown result type (might be due to invalid IL or missing references)
		//IL_17449: Expected O, but got Unknown
		//IL_174de: Unknown result type (might be due to invalid IL or missing references)
		//IL_174e4: Expected O, but got Unknown
		//IL_17547: Unknown result type (might be due to invalid IL or missing references)
		//IL_1754d: Expected O, but got Unknown
		//IL_175c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_175c8: Expected O, but got Unknown
		//IL_1763d: Unknown result type (might be due to invalid IL or missing references)
		//IL_17643: Expected O, but got Unknown
		//IL_176b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_176be: Expected O, but got Unknown
		//IL_17716: Unknown result type (might be due to invalid IL or missing references)
		//IL_1771c: Expected O, but got Unknown
		//IL_17769: Unknown result type (might be due to invalid IL or missing references)
		//IL_1776f: Expected O, but got Unknown
		//IL_177bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_177c2: Expected O, but got Unknown
		//IL_1780f: Unknown result type (might be due to invalid IL or missing references)
		//IL_17815: Expected O, but got Unknown
		//IL_17862: Unknown result type (might be due to invalid IL or missing references)
		//IL_17868: Expected O, but got Unknown
		//IL_178b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_178bb: Expected O, but got Unknown
		//IL_17908: Unknown result type (might be due to invalid IL or missing references)
		//IL_1790e: Expected O, but got Unknown
		//IL_17971: Unknown result type (might be due to invalid IL or missing references)
		//IL_17977: Expected O, but got Unknown
		//IL_179da: Unknown result type (might be due to invalid IL or missing references)
		//IL_179e0: Expected O, but got Unknown
		//IL_17a43: Unknown result type (might be due to invalid IL or missing references)
		//IL_17a49: Expected O, but got Unknown
		//IL_17aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ab2: Expected O, but got Unknown
		//IL_17b15: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b1b: Expected O, but got Unknown
		//IL_17b7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b84: Expected O, but got Unknown
		//IL_17bd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17bd7: Expected O, but got Unknown
		//IL_17c3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c40: Expected O, but got Unknown
		//IL_17ca3: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ca9: Expected O, but got Unknown
		//IL_17d0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d12: Expected O, but got Unknown
		//IL_17d5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_17d65: Expected O, but got Unknown
		//IL_17dc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17dce: Expected O, but got Unknown
		//IL_17e31: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e37: Expected O, but got Unknown
		//IL_17e9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ea0: Expected O, but got Unknown
		//IL_17f03: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f09: Expected O, but got Unknown
		//IL_17f56: Unknown result type (might be due to invalid IL or missing references)
		//IL_17f5c: Expected O, but got Unknown
		//IL_17fbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_17fc5: Expected O, but got Unknown
		//IL_18028: Unknown result type (might be due to invalid IL or missing references)
		//IL_1802e: Expected O, but got Unknown
		//IL_18091: Unknown result type (might be due to invalid IL or missing references)
		//IL_18097: Expected O, but got Unknown
		//IL_180fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_18100: Expected O, but got Unknown
		//IL_18191: Unknown result type (might be due to invalid IL or missing references)
		//IL_18197: Expected O, but got Unknown
		//IL_181e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_181ea: Expected O, but got Unknown
		//IL_18237: Unknown result type (might be due to invalid IL or missing references)
		//IL_1823d: Expected O, but got Unknown
		//IL_1828a: Unknown result type (might be due to invalid IL or missing references)
		//IL_18290: Expected O, but got Unknown
		//IL_182dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_182e3: Expected O, but got Unknown
		//IL_18371: Unknown result type (might be due to invalid IL or missing references)
		//IL_18377: Expected O, but got Unknown
		//IL_18405: Unknown result type (might be due to invalid IL or missing references)
		//IL_1840b: Expected O, but got Unknown
		//IL_1846e: Unknown result type (might be due to invalid IL or missing references)
		//IL_18474: Expected O, but got Unknown
		//IL_184e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_184e8: Expected O, but got Unknown
		//IL_18556: Unknown result type (might be due to invalid IL or missing references)
		//IL_1855c: Expected O, but got Unknown
		//IL_185bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_185c5: Expected O, but got Unknown
		//IL_18628: Unknown result type (might be due to invalid IL or missing references)
		//IL_1862e: Expected O, but got Unknown
		//IL_18691: Unknown result type (might be due to invalid IL or missing references)
		//IL_18697: Expected O, but got Unknown
		//IL_186e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_186ea: Expected O, but got Unknown
		//IL_1874d: Unknown result type (might be due to invalid IL or missing references)
		//IL_18753: Expected O, but got Unknown
		//IL_187ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_187b1: Expected O, but got Unknown
		//IL_1881f: Unknown result type (might be due to invalid IL or missing references)
		//IL_18825: Expected O, but got Unknown
		//IL_18893: Unknown result type (might be due to invalid IL or missing references)
		//IL_18899: Expected O, but got Unknown
		//IL_188e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_188ec: Expected O, but got Unknown
		//IL_18939: Unknown result type (might be due to invalid IL or missing references)
		//IL_1893f: Expected O, but got Unknown
		//IL_1898c: Unknown result type (might be due to invalid IL or missing references)
		//IL_18992: Expected O, but got Unknown
		//IL_189df: Unknown result type (might be due to invalid IL or missing references)
		//IL_189e5: Expected O, but got Unknown
		//IL_18a32: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a38: Expected O, but got Unknown
		//IL_18a85: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a8b: Expected O, but got Unknown
		//IL_18ad8: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ade: Expected O, but got Unknown
		//IL_18b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b31: Expected O, but got Unknown
		//IL_18b7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_18b84: Expected O, but got Unknown
		//IL_18bd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_18bd7: Expected O, but got Unknown
		//IL_18c24: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c2a: Expected O, but got Unknown
		//IL_18c98: Unknown result type (might be due to invalid IL or missing references)
		//IL_18c9e: Expected O, but got Unknown
		//IL_18d0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d12: Expected O, but got Unknown
		//IL_18d75: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d7b: Expected O, but got Unknown
		//IL_18dde: Unknown result type (might be due to invalid IL or missing references)
		//IL_18de4: Expected O, but got Unknown
		//IL_18e47: Unknown result type (might be due to invalid IL or missing references)
		//IL_18e4d: Expected O, but got Unknown
		//IL_18eb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_18eb6: Expected O, but got Unknown
		//IL_18f19: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f1f: Expected O, but got Unknown
		//IL_18f82: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f88: Expected O, but got Unknown
		//IL_18feb: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ff1: Expected O, but got Unknown
		//IL_19054: Unknown result type (might be due to invalid IL or missing references)
		//IL_1905a: Expected O, but got Unknown
		//IL_190b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_190b8: Expected O, but got Unknown
		//IL_1913b: Unknown result type (might be due to invalid IL or missing references)
		//IL_19141: Expected O, but got Unknown
		//IL_191c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_191ca: Expected O, but got Unknown
		//IL_1924d: Unknown result type (might be due to invalid IL or missing references)
		//IL_19253: Expected O, but got Unknown
		//IL_192d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_192dc: Expected O, but got Unknown
		//IL_1935f: Unknown result type (might be due to invalid IL or missing references)
		//IL_19365: Expected O, but got Unknown
		//IL_193e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_193ee: Expected O, but got Unknown
		//IL_19471: Unknown result type (might be due to invalid IL or missing references)
		//IL_19477: Expected O, but got Unknown
		//IL_194fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_19500: Expected O, but got Unknown
		//IL_19583: Unknown result type (might be due to invalid IL or missing references)
		//IL_19589: Expected O, but got Unknown
		//IL_1960c: Unknown result type (might be due to invalid IL or missing references)
		//IL_19612: Expected O, but got Unknown
		//IL_19695: Unknown result type (might be due to invalid IL or missing references)
		//IL_1969b: Expected O, but got Unknown
		//IL_1971e: Unknown result type (might be due to invalid IL or missing references)
		//IL_19724: Expected O, but got Unknown
		//IL_197a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_197ad: Expected O, but got Unknown
		//IL_19830: Unknown result type (might be due to invalid IL or missing references)
		//IL_19836: Expected O, but got Unknown
		//IL_198b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_198bf: Expected O, but got Unknown
		//IL_19942: Unknown result type (might be due to invalid IL or missing references)
		//IL_19948: Expected O, but got Unknown
		//IL_199cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_199d1: Expected O, but got Unknown
		//IL_19a54: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a5a: Expected O, but got Unknown
		//IL_19add: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ae3: Expected O, but got Unknown
		//IL_19b30: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b36: Expected O, but got Unknown
		//IL_19bb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_19bbf: Expected O, but got Unknown
		//IL_19c42: Unknown result type (might be due to invalid IL or missing references)
		//IL_19c48: Expected O, but got Unknown
		//IL_19ccb: Unknown result type (might be due to invalid IL or missing references)
		//IL_19cd1: Expected O, but got Unknown
		//IL_19d54: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d5a: Expected O, but got Unknown
		//IL_19ddd: Unknown result type (might be due to invalid IL or missing references)
		//IL_19de3: Expected O, but got Unknown
		//IL_19e66: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e6c: Expected O, but got Unknown
		//IL_19eb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ebf: Expected O, but got Unknown
		//IL_19f0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f12: Expected O, but got Unknown
		//IL_19f95: Unknown result type (might be due to invalid IL or missing references)
		//IL_19f9b: Expected O, but got Unknown
		//IL_1a01e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a024: Expected O, but got Unknown
		//IL_1a087: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a08d: Expected O, but got Unknown
		//IL_1a0f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a0f6: Expected O, but got Unknown
		//IL_1a179: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a17f: Expected O, but got Unknown
		//IL_1a1cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a1d2: Expected O, but got Unknown
		//IL_1a235: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a23b: Expected O, but got Unknown
		//IL_1a29e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2a4: Expected O, but got Unknown
		//IL_1a307: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a30d: Expected O, but got Unknown
		//IL_1a35a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a360: Expected O, but got Unknown
		//IL_1a3bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3c5: Expected O, but got Unknown
		//IL_1a424: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a42a: Expected O, but got Unknown
		//IL_1a489: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a48f: Expected O, but got Unknown
		//IL_1a4f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a4f8: Expected O, but got Unknown
		//IL_1a55b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a561: Expected O, but got Unknown
		//IL_1a5ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5b4: Expected O, but got Unknown
		//IL_1a617: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a61d: Expected O, but got Unknown
		//IL_1a680: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a686: Expected O, but got Unknown
		//IL_1a6d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6d9: Expected O, but got Unknown
		//IL_1a726: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a72c: Expected O, but got Unknown
		//IL_1a779: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a77f: Expected O, but got Unknown
		//IL_1a7cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a7d2: Expected O, but got Unknown
		//IL_1a840: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a846: Expected O, but got Unknown
		//IL_1a8db: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8e1: Expected O, but got Unknown
		//IL_1a94f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a955: Expected O, but got Unknown
		//IL_1a9b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9be: Expected O, but got Unknown
		//IL_1aa21: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa27: Expected O, but got Unknown
		//IL_1aa74: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa7a: Expected O, but got Unknown
		//IL_1aac7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aacd: Expected O, but got Unknown
		//IL_1ab1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab20: Expected O, but got Unknown
		//IL_1ab83: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab89: Expected O, but got Unknown
		//IL_1abec: Unknown result type (might be due to invalid IL or missing references)
		//IL_1abf2: Expected O, but got Unknown
		//IL_1ac55: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ac5b: Expected O, but got Unknown
		//IL_1acbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1acc4: Expected O, but got Unknown
		//IL_1ad32: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ad38: Expected O, but got Unknown
		//IL_1ada6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1adac: Expected O, but got Unknown
		//IL_1ae0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae15: Expected O, but got Unknown
		//IL_1ae78: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae7e: Expected O, but got Unknown
		//IL_1aee1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aee7: Expected O, but got Unknown
		//IL_1af4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af50: Expected O, but got Unknown
		//IL_1afb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afb9: Expected O, but got Unknown
		//IL_1b01c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b022: Expected O, but got Unknown
		//IL_1b06f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b075: Expected O, but got Unknown
		//IL_1b0e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0e9: Expected O, but got Unknown
		//IL_1b157: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b15d: Expected O, but got Unknown
		//IL_1b1cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1d1: Expected O, but got Unknown
		//IL_1b21e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b224: Expected O, but got Unknown
		//IL_1b287: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b28d: Expected O, but got Unknown
		//IL_1b2f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b2f6: Expected O, but got Unknown
		//IL_1b364: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b36a: Expected O, but got Unknown
		//IL_1b3d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b3de: Expected O, but got Unknown
		//IL_1b448: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b44e: Expected O, but got Unknown
		//IL_1b4b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b4be: Expected O, but got Unknown
		//IL_1b528: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b52e: Expected O, but got Unknown
		//IL_1b57b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b581: Expected O, but got Unknown
		//IL_1b5eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b5f1: Expected O, but got Unknown
		//IL_1b654: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b65a: Expected O, but got Unknown
		//IL_1b6c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b6ca: Expected O, but got Unknown
		//IL_1b734: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b73a: Expected O, but got Unknown
		//IL_1b79d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b7a3: Expected O, but got Unknown
		//IL_1b7f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b7f6: Expected O, but got Unknown
		//IL_1b864: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b86a: Expected O, but got Unknown
		//IL_1b8d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b8de: Expected O, but got Unknown
		//IL_1b94c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b952: Expected O, but got Unknown
		//IL_1b9c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b9c6: Expected O, but got Unknown
		//IL_1ba34: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ba3a: Expected O, but got Unknown
		//IL_1baa8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1baae: Expected O, but got Unknown
		//IL_1bb1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb22: Expected O, but got Unknown
		//IL_1bb85: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb8b: Expected O, but got Unknown
		//IL_1bbf9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bbff: Expected O, but got Unknown
		//IL_1bc62: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bc68: Expected O, but got Unknown
		//IL_1bcf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bcfc: Expected O, but got Unknown
		//IL_1bd5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bd65: Expected O, but got Unknown
		//IL_1bdd3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bdd9: Expected O, but got Unknown
		//IL_1be3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1be42: Expected O, but got Unknown
		//IL_1bea5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1beab: Expected O, but got Unknown
		//IL_1bf0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf14: Expected O, but got Unknown
		//IL_1bf77: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bf7d: Expected O, but got Unknown
		//IL_1bfca: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bfd0: Expected O, but got Unknown
		//IL_1c03e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c044: Expected O, but got Unknown
		//IL_1c0b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c0b8: Expected O, but got Unknown
		//IL_1c126: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c12c: Expected O, but got Unknown
		//IL_1c1a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c1a7: Expected O, but got Unknown
		//IL_1c21c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c222: Expected O, but got Unknown
		//IL_1c285: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c28b: Expected O, but got Unknown
		//IL_1c2ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c2f4: Expected O, but got Unknown
		//IL_1c341: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c347: Expected O, but got Unknown
		//IL_1c3bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c3c2: Expected O, but got Unknown
		//IL_1c430: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c436: Expected O, but got Unknown
		//IL_1c4ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c4b1: Expected O, but got Unknown
		//IL_1c514: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c51a: Expected O, but got Unknown
		//IL_1c57d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c583: Expected O, but got Unknown
		//IL_1c5f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c5f7: Expected O, but got Unknown
		//IL_1c665: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c66b: Expected O, but got Unknown
		//IL_1c6b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c6be: Expected O, but got Unknown
		//IL_1c721: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c727: Expected O, but got Unknown
		//IL_1c78a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c790: Expected O, but got Unknown
		//IL_1c7f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c7f9: Expected O, but got Unknown
		//IL_1c85c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c862: Expected O, but got Unknown
		//IL_1c8c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c8cb: Expected O, but got Unknown
		//IL_1c92e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c934: Expected O, but got Unknown
		//IL_1c997: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c99d: Expected O, but got Unknown
		//IL_1ca00: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ca06: Expected O, but got Unknown
		//IL_1ca89: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ca8f: Expected O, but got Unknown
		//IL_1caee: Unknown result type (might be due to invalid IL or missing references)
		//IL_1caf4: Expected O, but got Unknown
		//IL_1cb57: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cb5d: Expected O, but got Unknown
		//IL_1cbc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cbc6: Expected O, but got Unknown
		//IL_1cc29: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cc2f: Expected O, but got Unknown
		//IL_1cca4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ccaa: Expected O, but got Unknown
		//IL_1ccf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ccfd: Expected O, but got Unknown
		//IL_1cd60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cd66: Expected O, but got Unknown
		//IL_1cddb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cde1: Expected O, but got Unknown
		//IL_1ce4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ce55: Expected O, but got Unknown
		//IL_1cec3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cec9: Expected O, but got Unknown
		//IL_1cf3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cf44: Expected O, but got Unknown
		//IL_1cfa7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cfad: Expected O, but got Unknown
		//IL_1d01b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d021: Expected O, but got Unknown
		//IL_1d08f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d095: Expected O, but got Unknown
		//IL_1d103: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d109: Expected O, but got Unknown
		//IL_1d17e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d184: Expected O, but got Unknown
		//IL_1d1d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d1d7: Expected O, but got Unknown
		//IL_1d265: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d26b: Expected O, but got Unknown
		//IL_1d2e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d2e6: Expected O, but got Unknown
		//IL_1d362: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d368: Expected O, but got Unknown
		//IL_1d3dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d3e3: Expected O, but got Unknown
		//IL_1d458: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d45e: Expected O, but got Unknown
		//IL_1d4d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d4d9: Expected O, but got Unknown
		//IL_1d54e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d554: Expected O, but got Unknown
		//IL_1d5ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d5b2: Expected O, but got Unknown
		//IL_1d60a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d610: Expected O, but got Unknown
		//IL_1d685: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d68b: Expected O, but got Unknown
		//IL_1d6f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d6fb: Expected O, but got Unknown
		//IL_1d765: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d76b: Expected O, but got Unknown
		//IL_1d7ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d7d4: Expected O, but got Unknown
		//IL_1d837: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d83d: Expected O, but got Unknown
		//IL_1d8a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d8a6: Expected O, but got Unknown
		//IL_1d929: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d92f: Expected O, but got Unknown
		//IL_1d992: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d998: Expected O, but got Unknown
		//IL_1d9fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da01: Expected O, but got Unknown
		//IL_1da64: Unknown result type (might be due to invalid IL or missing references)
		//IL_1da6a: Expected O, but got Unknown
		//IL_1dacd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dad3: Expected O, but got Unknown
		//IL_1db41: Unknown result type (might be due to invalid IL or missing references)
		//IL_1db47: Expected O, but got Unknown
		//IL_1dbb5: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dbbb: Expected O, but got Unknown
		//IL_1dc1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc24: Expected O, but got Unknown
		//IL_1dc99: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dc9f: Expected O, but got Unknown
		//IL_1dd02: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dd08: Expected O, but got Unknown
		//IL_1dd55: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dd5b: Expected O, but got Unknown
		//IL_1dda8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ddae: Expected O, but got Unknown
		//IL_1de31: Unknown result type (might be due to invalid IL or missing references)
		//IL_1de37: Expected O, but got Unknown
		//IL_1de84: Unknown result type (might be due to invalid IL or missing references)
		//IL_1de8a: Expected O, but got Unknown
		//IL_1ded7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dedd: Expected O, but got Unknown
		//IL_1df40: Unknown result type (might be due to invalid IL or missing references)
		//IL_1df46: Expected O, but got Unknown
		//IL_1df93: Unknown result type (might be due to invalid IL or missing references)
		//IL_1df99: Expected O, but got Unknown
		//IL_1dfe6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1dfec: Expected O, but got Unknown
		//IL_1e04f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e055: Expected O, but got Unknown
		//IL_1e0b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e0be: Expected O, but got Unknown
		//IL_1e121: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e127: Expected O, but got Unknown
		//IL_1e174: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e17a: Expected O, but got Unknown
		//IL_1e1c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e1cd: Expected O, but got Unknown
		//IL_1e21a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e220: Expected O, but got Unknown
		//IL_1e283: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e289: Expected O, but got Unknown
		//IL_1e2d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e2dc: Expected O, but got Unknown
		//IL_1e329: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e32f: Expected O, but got Unknown
		//IL_1e37c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e382: Expected O, but got Unknown
		//IL_1e3cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e3d5: Expected O, but got Unknown
		//IL_1e422: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e428: Expected O, but got Unknown
		//IL_1e49d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e4a3: Expected O, but got Unknown
		//IL_1e518: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e51e: Expected O, but got Unknown
		//IL_1e56b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e571: Expected O, but got Unknown
		//IL_1e5df: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e5e5: Expected O, but got Unknown
		//IL_1e653: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e659: Expected O, but got Unknown
		//IL_1e6c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e6cd: Expected O, but got Unknown
		//IL_1e73b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e741: Expected O, but got Unknown
		//IL_1e7af: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e7b5: Expected O, but got Unknown
		//IL_1e823: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e829: Expected O, but got Unknown
		//IL_1e897: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e89d: Expected O, but got Unknown
		//IL_1e90b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e911: Expected O, but got Unknown
		//IL_1e97f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e985: Expected O, but got Unknown
		//IL_1e9f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e9f9: Expected O, but got Unknown
		//IL_1ea67: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ea6d: Expected O, but got Unknown
		//IL_1eadb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eae1: Expected O, but got Unknown
		//IL_1eb4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eb55: Expected O, but got Unknown
		//IL_1ebc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ebc9: Expected O, but got Unknown
		//IL_1ec37: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ec3d: Expected O, but got Unknown
		//IL_1eca0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eca6: Expected O, but got Unknown
		//IL_1ed09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ed0f: Expected O, but got Unknown
		//IL_1ed5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ed62: Expected O, but got Unknown
		//IL_1edd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eddd: Expected O, but got Unknown
		//IL_1ee40: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ee46: Expected O, but got Unknown
		//IL_1eea9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1eeaf: Expected O, but got Unknown
		//IL_1ef12: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ef18: Expected O, but got Unknown
		//IL_1ef86: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ef8c: Expected O, but got Unknown
		//IL_1effa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f000: Expected O, but got Unknown
		//IL_1f06e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f074: Expected O, but got Unknown
		//IL_1f0d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f0dd: Expected O, but got Unknown
		//IL_1f152: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f158: Expected O, but got Unknown
		//IL_1f1bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f1c1: Expected O, but got Unknown
		//IL_1f224: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f22a: Expected O, but got Unknown
		//IL_1f28d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f293: Expected O, but got Unknown
		//IL_1f2e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f2e6: Expected O, but got Unknown
		//IL_1f354: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f35a: Expected O, but got Unknown
		//IL_1f3cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f3d5: Expected O, but got Unknown
		//IL_1f443: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f449: Expected O, but got Unknown
		//IL_1f4b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f4bd: Expected O, but got Unknown
		//IL_1f52b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f531: Expected O, but got Unknown
		//IL_1f594: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f59a: Expected O, but got Unknown
		//IL_1f5fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f603: Expected O, but got Unknown
		//IL_1f666: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f66c: Expected O, but got Unknown
		//IL_1f6cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f6d5: Expected O, but got Unknown
		//IL_1f722: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f728: Expected O, but got Unknown
		//IL_1f775: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f77b: Expected O, but got Unknown
		//IL_1f7de: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f7e4: Expected O, but got Unknown
		//IL_1f852: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f858: Expected O, but got Unknown
		//IL_1f8c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f8cc: Expected O, but got Unknown
		//IL_1f936: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f93c: Expected O, but got Unknown
		//IL_1f9aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f9b0: Expected O, but got Unknown
		//IL_1fa1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa20: Expected O, but got Unknown
		//IL_1fa95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa9b: Expected O, but got Unknown
		//IL_1fae8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1faee: Expected O, but got Unknown
		//IL_1fb51: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fb57: Expected O, but got Unknown
		//IL_1fbba: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fbc0: Expected O, but got Unknown
		//IL_1fc0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fc13: Expected O, but got Unknown
		//IL_1fc60: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fc66: Expected O, but got Unknown
		//IL_1fcd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fcda: Expected O, but got Unknown
		//IL_1fd48: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fd4e: Expected O, but got Unknown
		//IL_1fdbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fdc2: Expected O, but got Unknown
		//IL_1fe30: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fe36: Expected O, but got Unknown
		//IL_1fea4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1feaa: Expected O, but got Unknown
		//IL_1ff1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ff25: Expected O, but got Unknown
		//IL_1ff9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ffa0: Expected O, but got Unknown
		//IL_20015: Unknown result type (might be due to invalid IL or missing references)
		//IL_2001b: Expected O, but got Unknown
		//IL_20090: Unknown result type (might be due to invalid IL or missing references)
		//IL_20096: Expected O, but got Unknown
		//IL_200e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_200e9: Expected O, but got Unknown
		Command[] array = new Command[1262];
		Command val = new Command();
		val.Name = "overrideadventcalendarday";
		val.Parent = "adventcalendar";
		val.FullName = "adventcalendar.overrideadventcalendarday";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AdventCalendar.overrideAdventCalendarDay.ToString();
		val.SetOveride = delegate(string str)
		{
			AdventCalendar.overrideAdventCalendarDay = StringExtensions.ToInt(str, 0);
		};
		array[0] = val;
		val = new Command();
		val.Name = "overrideadventcalendarmonth";
		val.Parent = "adventcalendar";
		val.FullName = "adventcalendar.overrideadventcalendarmonth";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AdventCalendar.overrideAdventCalendarMonth.ToString();
		val.SetOveride = delegate(string str)
		{
			AdventCalendar.overrideAdventCalendarMonth = StringExtensions.ToInt(str, 0);
		};
		array[1] = val;
		val = new Command();
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
		array[2] = val;
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
		array[3] = val;
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
		array[4] = val;
		val = new Command();
		val.Name = "usesimpleloscheck";
		val.Parent = "aiinformationzone";
		val.FullName = "aiinformationzone.usesimpleloscheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIInformationZone.UseSimpleLOSCheck.ToString();
		val.SetOveride = delegate(string str)
		{
			AIInformationZone.UseSimpleLOSCheck = StringExtensions.ToBool(str);
		};
		array[5] = val;
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
		array[6] = val;
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
		array[7] = val;
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
		array[8] = val;
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
		array[9] = val;
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
		array[10] = val;
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
		array[11] = val;
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
		array[12] = val;
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
		array[13] = val;
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
		array[14] = val;
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
		array[15] = val;
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
		array[16] = val;
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
		array[17] = val;
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
		array[18] = val;
		val = new Command();
		val.Name = "detectlongrangetick";
		val.Parent = "basemetaldetector";
		val.FullName = "basemetaldetector.detectlongrangetick";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseMetalDetector.DetectLongRangeTick.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseMetalDetector.DetectLongRangeTick = StringExtensions.ToFloat(str, 0f);
		};
		array[19] = val;
		val = new Command();
		val.Name = "detectminmovementdistance";
		val.Parent = "basemetaldetector";
		val.FullName = "basemetaldetector.detectminmovementdistance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseMetalDetector.DetectMinMovementDistance.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseMetalDetector.DetectMinMovementDistance = StringExtensions.ToFloat(str, 0f);
		};
		array[20] = val;
		val = new Command();
		val.Name = "nearestdistancetick";
		val.Parent = "basemetaldetector";
		val.FullName = "basemetaldetector.nearestdistancetick";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseMetalDetector.NearestDistanceTick.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseMetalDetector.NearestDistanceTick = StringExtensions.ToFloat(str, 0f);
		};
		array[21] = val;
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
		array[22] = val;
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
		array[23] = val;
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
		array[24] = val;
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
		array[25] = val;
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
		array[26] = val;
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
		array[27] = val;
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
		array[28] = val;
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
		array[29] = val;
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
		array[30] = val;
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
		array[31] = val;
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
		array[32] = val;
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
		array[33] = val;
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
		array[34] = val;
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
		array[35] = val;
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
		array[36] = val;
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
		array[37] = val;
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
		array[38] = val;
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
		array[39] = val;
		val = new Command();
		val.Name = "doplayerdamage";
		val.Parent = "bike";
		val.FullName = "bike.doplayerdamage";
		val.ServerAdmin = true;
		val.Description = "Can bike crashes cause damage or death to the rider?";
		val.Variable = true;
		val.GetOveride = () => Bike.doPlayerDamage.ToString();
		val.SetOveride = delegate(string str)
		{
			Bike.doPlayerDamage = StringExtensions.ToBool(str);
		};
		array[40] = val;
		val = new Command();
		val.Name = "motorbikemonumentpopulation";
		val.Parent = "bike";
		val.FullName = "bike.motorbikemonumentpopulation";
		val.ServerAdmin = true;
		val.Description = "Motorbike population in monuments";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Bike.motorbikeMonumentPopulation.ToString();
		val.SetOveride = delegate(string str)
		{
			Bike.motorbikeMonumentPopulation = StringExtensions.ToFloat(str, 0f);
		};
		array[41] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "bike";
		val.FullName = "bike.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a bike loses all its health while outside";
		val.Variable = true;
		val.GetOveride = () => Bike.outsideDecayMinutes.ToString();
		val.SetOveride = delegate(string str)
		{
			Bike.outsideDecayMinutes = StringExtensions.ToFloat(str, 0f);
		};
		array[42] = val;
		val = new Command();
		val.Name = "pedalmonumentpopulation";
		val.Parent = "bike";
		val.FullName = "bike.pedalmonumentpopulation";
		val.ServerAdmin = true;
		val.Description = "Pedal bike population in monuments";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Bike.pedalMonumentPopulation.ToString();
		val.SetOveride = delegate(string str)
		{
			Bike.pedalMonumentPopulation = StringExtensions.ToFloat(str, 0f);
		};
		array[43] = val;
		val = new Command();
		val.Name = "pedalroadsidepopulation";
		val.Parent = "bike";
		val.FullName = "bike.pedalroadsidepopulation";
		val.ServerAdmin = true;
		val.Description = "Pedal bike population active on the server (roadside spawns)";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Bike.pedalRoadsidePopulation.ToString();
		val.SetOveride = delegate(string str)
		{
			Bike.pedalRoadsidePopulation = StringExtensions.ToFloat(str, 0f);
		};
		array[44] = val;
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
		array[45] = val;
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
		array[46] = val;
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
		array[47] = val;
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
		array[48] = val;
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
		val.GetOveride = () => BoomBox.ServerUrlList ?? "";
		val.SetOveride = delegate(string str)
		{
			BoomBox.ServerUrlList = str;
		};
		val.Default = "";
		array[49] = val;
		val = new Command();
		val.Name = "deployattackdistancemax";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.deployattackdistancemax";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BradleyAPC.DeployAttackDistanceMax.ToString();
		val.SetOveride = delegate(string str)
		{
			BradleyAPC.DeployAttackDistanceMax = StringExtensions.ToFloat(str, 0f);
		};
		array[50] = val;
		val = new Command();
		val.Name = "deployhealthrangemax";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.deployhealthrangemax";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BradleyAPC.DeployHealthRangeMax.ToString();
		val.SetOveride = delegate(string str)
		{
			BradleyAPC.DeployHealthRangeMax = StringExtensions.ToFloat(str, 0f);
		};
		array[51] = val;
		val = new Command();
		val.Name = "deployhealthrangemin";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.deployhealthrangemin";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BradleyAPC.DeployHealthRangeMin.ToString();
		val.SetOveride = delegate(string str)
		{
			BradleyAPC.DeployHealthRangeMin = StringExtensions.ToFloat(str, 0f);
		};
		array[52] = val;
		val = new Command();
		val.Name = "deployinterval";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.deployinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BradleyAPC.DeployInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			BradleyAPC.DeployInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[53] = val;
		val = new Command();
		val.Name = "deployondamagecheckinterval";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.deployondamagecheckinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BradleyAPC.DeployOnDamageCheckInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			BradleyAPC.DeployOnDamageCheckInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[54] = val;
		val = new Command();
		val.Name = "killscientistsonbradleydeath";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.killscientistsonbradleydeath";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BradleyAPC.KillScientistsOnBradleyDeath.ToString();
		val.SetOveride = delegate(string str)
		{
			BradleyAPC.KillScientistsOnBradleyDeath = StringExtensions.ToBool(str);
		};
		array[55] = val;
		val = new Command();
		val.Name = "mountafternotattackedduration";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.mountafternotattackedduration";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BradleyAPC.MountAfterNotAttackedDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			BradleyAPC.MountAfterNotAttackedDuration = StringExtensions.ToFloat(str, 0f);
		};
		array[56] = val;
		val = new Command();
		val.Name = "mountafternotfiredduration";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.mountafternotfiredduration";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BradleyAPC.MountAfterNotFiredDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			BradleyAPC.MountAfterNotFiredDuration = StringExtensions.ToFloat(str, 0f);
		};
		array[57] = val;
		val = new Command();
		val.Name = "mountafternottargetsduration";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.mountafternottargetsduration";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BradleyAPC.MountAfterNotTargetsDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			BradleyAPC.MountAfterNotTargetsDuration = StringExtensions.ToFloat(str, 0f);
		};
		array[58] = val;
		val = new Command();
		val.Name = "scientistredeploymentmininterval";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.scientistredeploymentmininterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BradleyAPC.ScientistRedeploymentMinInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			BradleyAPC.ScientistRedeploymentMinInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[59] = val;
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
			string text26 = BradleyAPC.svspawnroadbradley(arg.GetVector3(0, Vector3.zero), arg.GetVector3(1, Vector3.zero));
			arg.ReplyWithObject((object)text26);
		};
		array[60] = val;
		val = new Command();
		val.Name = "usesmokegrenades";
		val.Parent = "bradleyapc";
		val.FullName = "bradleyapc.usesmokegrenades";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BradleyAPC.UseSmokeGrenades.ToString();
		val.SetOveride = delegate(string str)
		{
			BradleyAPC.UseSmokeGrenades = StringExtensions.ToBool(str);
		};
		array[61] = val;
		val = new Command();
		val.Name = "debug_cargo_status";
		val.Parent = "cargoship";
		val.FullName = "cargoship.debug_cargo_status";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			CargoShip.debug_cargo_status(arg);
		};
		array[62] = val;
		val = new Command();
		val.Name = "debug_info";
		val.Parent = "cargoship";
		val.FullName = "cargoship.debug_info";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			CargoShip.debug_info(arg);
		};
		array[63] = val;
		val = new Command();
		val.Name = "dock_time";
		val.Parent = "cargoship";
		val.FullName = "cargoship.dock_time";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.dock_time.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.dock_time = StringExtensions.ToFloat(str, 0f);
		};
		array[64] = val;
		val = new Command();
		val.Name = "docking_debug";
		val.Parent = "cargoship";
		val.FullName = "cargoship.docking_debug";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.docking_debug.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.docking_debug = StringExtensions.ToBool(str);
		};
		array[65] = val;
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
		array[66] = val;
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
		array[67] = val;
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
		array[68] = val;
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
		array[69] = val;
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
		array[70] = val;
		val = new Command();
		val.Name = "refresh_loot_on_dock";
		val.Parent = "cargoship";
		val.FullName = "cargoship.refresh_loot_on_dock";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.refresh_loot_on_dock.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.refresh_loot_on_dock = StringExtensions.ToBool(str);
		};
		array[71] = val;
		val = new Command();
		val.Name = "should_dock";
		val.Parent = "cargoship";
		val.FullName = "cargoship.should_dock";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.should_dock.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.should_dock = StringExtensions.ToBool(str);
		};
		array[72] = val;
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
		array[73] = val;
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
		array[74] = val;
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
		array[75] = val;
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
		array[76] = val;
		val = new Command();
		val.Name = "hideobjects";
		val.Parent = "cinematicentity";
		val.FullName = "cinematicentity.hideobjects";
		val.ServerAdmin = true;
		val.Description = "Hides cinematic entities by group (0= none, 1= lights, 2= BGs, 3= props, 4= misc)";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			CinematicEntity.HideObjects(arg);
		};
		array[77] = val;
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
		array[78] = val;
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
		array[79] = val;
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
		array[80] = val;
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
		array[81] = val;
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
		array[82] = val;
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
		array[83] = val;
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
		array[84] = val;
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
		array[85] = val;
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
		array[86] = val;
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
		array[87] = val;
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
		array[88] = val;
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
		array[89] = val;
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
		array[90] = val;
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
		array[91] = val;
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
		array[92] = val;
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
		array[93] = val;
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
		array[94] = val;
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
		array[95] = val;
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
		array[96] = val;
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
		array[97] = val;
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
		array[98] = val;
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
		array[99] = val;
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
		array[100] = val;
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
		array[101] = val;
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
		array[102] = val;
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
		array[103] = val;
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
		array[104] = val;
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
		array[105] = val;
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
		array[106] = val;
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
		array[107] = val;
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
		array[108] = val;
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
		array[109] = val;
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
		array[110] = val;
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
		array[111] = val;
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
		array[112] = val;
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
		array[113] = val;
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
		array[114] = val;
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
		array[115] = val;
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
		array[116] = val;
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
		array[117] = val;
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
		array[118] = val;
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
		array[119] = val;
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
		array[120] = val;
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
		array[121] = val;
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
		array[122] = val;
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
		array[123] = val;
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
		array[124] = val;
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
		array[125] = val;
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
		array[126] = val;
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
		array[127] = val;
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
		array[128] = val;
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
		array[129] = val;
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
		array[130] = val;
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
		array[131] = val;
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
		array[132] = val;
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
		array[133] = val;
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
		array[134] = val;
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
		array[135] = val;
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
		array[136] = val;
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
		array[137] = val;
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
		array[138] = val;
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
		array[139] = val;
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
		array[140] = val;
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
		array[141] = val;
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
		array[142] = val;
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
		array[143] = val;
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
		array[144] = val;
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
		array[145] = val;
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
		array[146] = val;
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
		array[147] = val;
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
		array[148] = val;
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
		array[149] = val;
		val = new Command();
		val.Name = "teaminfo";
		val.Parent = "global";
		val.FullName = "global.teaminfo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text25 = Admin.teaminfo(arg);
			arg.ReplyWithObject((object)text25);
		};
		array[150] = val;
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
		array[151] = val;
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
		array[152] = val;
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
		array[153] = val;
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
		array[154] = val;
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
		array[155] = val;
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
		array[156] = val;
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
		array[157] = val;
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
		array[158] = val;
		val = new Command();
		val.Name = "aizonestats";
		val.Parent = "ai";
		val.FullName = "ai.aizonestats";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.aizonestats(arg);
		};
		array[159] = val;
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
		array[160] = val;
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
		array[161] = val;
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
		array[162] = val;
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
		array[163] = val;
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
		array[164] = val;
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
		array[165] = val;
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
		array[166] = val;
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
		array[167] = val;
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
		array[168] = val;
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
		array[169] = val;
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
		array[170] = val;
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
		array[171] = val;
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
		array[172] = val;
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
		array[173] = val;
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
		array[174] = val;
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
		array[175] = val;
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
		array[176] = val;
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
		array[177] = val;
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
		array[178] = val;
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
		array[179] = val;
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
		array[180] = val;
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
		array[181] = val;
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
		array[182] = val;
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
		array[183] = val;
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
		array[184] = val;
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
		array[185] = val;
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
		array[186] = val;
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
		array[187] = val;
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
		array[188] = val;
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
		array[189] = val;
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
		array[190] = val;
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
		array[191] = val;
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
		array[192] = val;
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
		array[193] = val;
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
		array[194] = val;
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
		array[195] = val;
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
		array[196] = val;
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
		array[197] = val;
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
		array[198] = val;
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
		array[199] = val;
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
		array[200] = val;
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
		array[201] = val;
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
		array[202] = val;
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
		array[203] = val;
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
		array[204] = val;
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
		array[205] = val;
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
		array[206] = val;
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
		array[207] = val;
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
		array[208] = val;
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
		array[209] = val;
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
		array[210] = val;
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
		array[211] = val;
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
		array[212] = val;
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
		array[213] = val;
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
		array[214] = val;
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
		array[215] = val;
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
		array[216] = val;
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
		array[217] = val;
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
		array[218] = val;
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
		array[219] = val;
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
		array[220] = val;
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
		array[221] = val;
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
		array[222] = val;
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
		array[223] = val;
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
		array[224] = val;
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
		array[225] = val;
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
		array[226] = val;
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
		array[227] = val;
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
		array[228] = val;
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
		array[229] = val;
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
		array[230] = val;
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
		array[231] = val;
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
		array[232] = val;
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
		array[233] = val;
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
		array[234] = val;
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
		array[235] = val;
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
		array[236] = val;
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
		array[237] = val;
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
		array[238] = val;
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
		array[239] = val;
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
		array[240] = val;
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
		array[241] = val;
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
		array[242] = val;
		val = new Command();
		val.Name = "eye_distance_parented_mounted_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_distance_parented_mounted_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_distance_parented_mounted_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_distance_parented_mounted_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[243] = val;
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
		array[244] = val;
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
		array[245] = val;
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
		array[246] = val;
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
		array[247] = val;
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
		array[248] = val;
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
		array[249] = val;
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
		array[250] = val;
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
		array[251] = val;
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
		array[252] = val;
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
		array[253] = val;
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
		array[254] = val;
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
		array[255] = val;
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
		array[256] = val;
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
		array[257] = val;
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
		array[258] = val;
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
		array[259] = val;
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
		array[260] = val;
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
		array[261] = val;
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
		array[262] = val;
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
		array[263] = val;
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
		array[264] = val;
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
		array[265] = val;
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
		array[266] = val;
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
		array[267] = val;
		val = new Command();
		val.Name = "impact_effect_distance_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.impact_effect_distance_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.impact_effect_distance_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.impact_effect_distance_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[268] = val;
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
		array[269] = val;
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
		array[270] = val;
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
		array[271] = val;
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
		array[272] = val;
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
		array[273] = val;
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
		array[274] = val;
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
		array[275] = val;
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
		array[276] = val;
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
		array[277] = val;
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
		array[278] = val;
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
		array[279] = val;
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
		array[280] = val;
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
		array[281] = val;
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
		array[282] = val;
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
		array[283] = val;
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
		array[284] = val;
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
		array[285] = val;
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
		array[286] = val;
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
		array[287] = val;
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
		array[288] = val;
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
		array[289] = val;
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
		array[290] = val;
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
		array[291] = val;
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
		array[292] = val;
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
		array[293] = val;
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
		array[294] = val;
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
		array[295] = val;
		val = new Command();
		val.Name = "projectile_distance_forgiveness_minimum";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_distance_forgiveness_minimum";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_distance_forgiveness_minimum.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_distance_forgiveness_minimum = StringExtensions.ToFloat(str, 0f);
		};
		array[296] = val;
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
		array[297] = val;
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
		array[298] = val;
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
		array[299] = val;
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
		array[300] = val;
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
		array[301] = val;
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
		array[302] = val;
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
		array[303] = val;
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
		array[304] = val;
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
		array[305] = val;
		val = new Command();
		val.Name = "projectile_update_limit";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_update_limit";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_update_limit.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_update_limit = StringExtensions.ToInt(str, 0);
		};
		array[306] = val;
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
		array[307] = val;
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
		array[308] = val;
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
		array[309] = val;
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
		array[310] = val;
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
		array[311] = val;
		val = new Command();
		val.Name = "rpcstallfade";
		val.Parent = "antihack";
		val.FullName = "antihack.rpcstallfade";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.rpcstallfade.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.rpcstallfade = StringExtensions.ToFloat(str, 0f);
		};
		array[312] = val;
		val = new Command();
		val.Name = "rpcstallmode";
		val.Parent = "antihack";
		val.FullName = "antihack.rpcstallmode";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.rpcstallmode.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.rpcstallmode = StringExtensions.ToInt(str, 0);
		};
		array[313] = val;
		val = new Command();
		val.Name = "rpcstallthreshold";
		val.Parent = "antihack";
		val.FullName = "antihack.rpcstallthreshold";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.rpcstallthreshold.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.rpcstallthreshold = StringExtensions.ToFloat(str, 0f);
		};
		array[314] = val;
		val = new Command();
		val.Name = "serverside_fall_damage";
		val.Parent = "antihack";
		val.FullName = "antihack.serverside_fall_damage";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.serverside_fall_damage.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.serverside_fall_damage = StringExtensions.ToBool(str);
		};
		array[315] = val;
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
		array[316] = val;
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
		array[317] = val;
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
		array[318] = val;
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
		array[319] = val;
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
		array[320] = val;
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
		array[321] = val;
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
		array[322] = val;
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
		array[323] = val;
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
		array[324] = val;
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
		array[325] = val;
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
		array[326] = val;
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
		array[327] = val;
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
		array[328] = val;
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
		array[329] = val;
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
		array[330] = val;
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
		array[331] = val;
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
		array[332] = val;
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
		array[333] = val;
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
		array[334] = val;
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
		array[335] = val;
		val = new Command();
		val.Name = "listenip";
		val.Parent = "app";
		val.FullName = "app.listenip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.listenip ?? "";
		val.SetOveride = delegate(string str)
		{
			App.listenip = str;
		};
		array[336] = val;
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
		array[337] = val;
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
		array[338] = val;
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
		array[339] = val;
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
		array[340] = val;
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
		array[341] = val;
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
		array[342] = val;
		val = new Command();
		val.Name = "publicip";
		val.Parent = "app";
		val.FullName = "app.publicip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.publicip ?? "";
		val.SetOveride = delegate(string str)
		{
			App.publicip = str;
		};
		array[343] = val;
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
		array[344] = val;
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
		array[345] = val;
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
		array[346] = val;
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
		array[347] = val;
		val = new Command();
		val.Name = "serverid";
		val.Parent = "app";
		val.FullName = "app.serverid";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => App.serverid ?? "";
		val.SetOveride = delegate(string str)
		{
			App.serverid = str;
		};
		val.Default = "";
		array[348] = val;
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
		array[349] = val;
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
		array[350] = val;
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
		array[351] = val;
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
		array[352] = val;
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
		array[353] = val;
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
		array[354] = val;
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
		array[355] = val;
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
		array[356] = val;
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
		array[357] = val;
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
		array[358] = val;
		val = new Command();
		val.Name = "hidechatintutorial";
		val.Parent = "chat";
		val.FullName = "chat.hidechatintutorial";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Chat.hideChatInTutorial.ToString();
		val.SetOveride = delegate(string str)
		{
			Chat.hideChatInTutorial = StringExtensions.ToBool(str);
		};
		val.Default = "True";
		array[359] = val;
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
		array[360] = val;
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
		array[361] = val;
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
		array[362] = val;
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
		array[363] = val;
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
		array[364] = val;
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
		array[365] = val;
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
		array[366] = val;
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
		array[367] = val;
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
		array[368] = val;
		val = new Command();
		val.Name = "editsrequireclantable";
		val.Parent = "clan";
		val.FullName = "clan.editsrequireclantable";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "If enabled then players will need to be near a Clan Table to make changes to clans";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Clan.editsRequireClanTable.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.editsRequireClanTable = StringExtensions.ToBool(str);
		};
		val.Default = "true";
		array[369] = val;
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
		array[370] = val;
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
		array[371] = val;
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
		array[372] = val;
		val = new Command();
		val.Name = "scorefordestroyingbradley";
		val.Parent = "clan";
		val.FullName = "clan.scorefordestroyingbradley";
		val.ServerAdmin = true;
		val.Description = "How much score players earn for destroying bradley";
		val.Variable = true;
		val.GetOveride = () => Clan.scoreForDestroyingBradley.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.scoreForDestroyingBradley = StringExtensions.ToInt(str, 0);
		};
		array[373] = val;
		val = new Command();
		val.Name = "scorefordestroyingtoolcupboards";
		val.Parent = "clan";
		val.FullName = "clan.scorefordestroyingtoolcupboards";
		val.ServerAdmin = true;
		val.Description = "How much score players earn for destroying other player's tool cupboards";
		val.Variable = true;
		val.GetOveride = () => Clan.scoreForDestroyingToolCupboards.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.scoreForDestroyingToolCupboards = StringExtensions.ToInt(str, 0);
		};
		array[374] = val;
		val = new Command();
		val.Name = "scoreforhackingcrates";
		val.Parent = "clan";
		val.FullName = "clan.scoreforhackingcrates";
		val.ServerAdmin = true;
		val.Description = "How much score players earn for hacking crates";
		val.Variable = true;
		val.GetOveride = () => Clan.scoreForHackingCrates.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.scoreForHackingCrates = StringExtensions.ToInt(str, 0);
		};
		array[375] = val;
		val = new Command();
		val.Name = "scoreforkilledbyplayerinotherclan";
		val.Parent = "clan";
		val.FullName = "clan.scoreforkilledbyplayerinotherclan";
		val.ServerAdmin = true;
		val.Description = "How much score players earn for being killed by a player in another clan (this value should be negative)";
		val.Variable = true;
		val.GetOveride = () => Clan.scoreForKilledByPlayerInOtherClan.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.scoreForKilledByPlayerInOtherClan = StringExtensions.ToInt(str, 0);
		};
		array[376] = val;
		val = new Command();
		val.Name = "scoreforkillingplayerinotherclan";
		val.Parent = "clan";
		val.FullName = "clan.scoreforkillingplayerinotherclan";
		val.ServerAdmin = true;
		val.Description = "How much score players earn for killing a player in another clan";
		val.Variable = true;
		val.GetOveride = () => Clan.scoreForKillingPlayerInOtherClan.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.scoreForKillingPlayerInOtherClan = StringExtensions.ToInt(str, 0);
		};
		array[377] = val;
		val = new Command();
		val.Name = "scoreforkillingunarmedplayer";
		val.Parent = "clan";
		val.FullName = "clan.scoreforkillingunarmedplayer";
		val.ServerAdmin = true;
		val.Description = "How much score players earn for killing unarmed players (this value should be negative)";
		val.Variable = true;
		val.GetOveride = () => Clan.scoreForKillingUnarmedPlayer.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.scoreForKillingUnarmedPlayer = StringExtensions.ToInt(str, 0);
		};
		array[378] = val;
		val = new Command();
		val.Name = "scoreforlootingelitecrate";
		val.Parent = "clan";
		val.FullName = "clan.scoreforlootingelitecrate";
		val.ServerAdmin = true;
		val.Description = "How much score players earn for looting an elite crate";
		val.Variable = true;
		val.GetOveride = () => Clan.scoreForLootingEliteCrate.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.scoreForLootingEliteCrate = StringExtensions.ToInt(str, 0);
		};
		array[379] = val;
		val = new Command();
		val.Name = "scoreforopeninghackedcrates";
		val.Parent = "clan";
		val.FullName = "clan.scoreforopeninghackedcrates";
		val.ServerAdmin = true;
		val.Description = "How much score players earn for opening hacked crates";
		val.Variable = true;
		val.GetOveride = () => Clan.scoreForOpeningHackedCrates.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.scoreForOpeningHackedCrates = StringExtensions.ToInt(str, 0);
		};
		array[380] = val;
		val = new Command();
		val.Name = "scoreforreachingcargoship";
		val.Parent = "clan";
		val.FullName = "clan.scoreforreachingcargoship";
		val.ServerAdmin = true;
		val.Description = "How much score players earn for reaching cargo ship";
		val.Variable = true;
		val.GetOveride = () => Clan.scoreForReachingCargoShip.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.scoreForReachingCargoShip = StringExtensions.ToInt(str, 0);
		};
		array[381] = val;
		val = new Command();
		val.Name = "scoreforrunningexcavator";
		val.Parent = "clan";
		val.FullName = "clan.scoreforrunningexcavator";
		val.ServerAdmin = true;
		val.Description = "How much score players earn for running the excavator";
		val.Variable = true;
		val.GetOveride = () => Clan.scoreForRunningExcavator.ToString();
		val.SetOveride = delegate(string str)
		{
			Clan.scoreForRunningExcavator = StringExtensions.ToInt(str, 0);
		};
		array[382] = val;
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
		array[383] = val;
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
		array[384] = val;
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
		array[385] = val;
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
		array[386] = val;
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
		array[387] = val;
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
		array[388] = val;
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
		array[389] = val;
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
		array[390] = val;
		val = new Command();
		val.Name = "allusers";
		val.Parent = "creative";
		val.FullName = "creative.allusers";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "Apply creative mode to the entire server";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Creative.allUsers.ToString();
		val.SetOveride = delegate(string str)
		{
			Creative.allUsers = StringExtensions.ToBool(str);
		};
		val.Default = "False";
		array[391] = val;
		val = new Command();
		val.Name = "freebuild";
		val.Parent = "creative";
		val.FullName = "creative.freebuild";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "Build blocks for free";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Creative.freeBuild.ToString();
		val.SetOveride = delegate(string str)
		{
			Creative.freeBuild = StringExtensions.ToBool(str);
		};
		val.Default = "False";
		array[392] = val;
		val = new Command();
		val.Name = "freeplacement";
		val.Parent = "creative";
		val.FullName = "creative.freeplacement";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "Bypasses all placement checks";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Creative.freePlacement.ToString();
		val.SetOveride = delegate(string str)
		{
			Creative.freePlacement = StringExtensions.ToBool(str);
		};
		val.Default = "False";
		array[393] = val;
		val = new Command();
		val.Name = "freerepair";
		val.Parent = "creative";
		val.FullName = "creative.freerepair";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "Bypass the 30s repair cooldown when repairing objects";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Creative.freeRepair.ToString();
		val.SetOveride = delegate(string str)
		{
			Creative.freeRepair = StringExtensions.ToBool(str);
		};
		val.Default = "False";
		array[394] = val;
		val = new Command();
		val.Name = "togglecreativemodeuser";
		val.Parent = "creative";
		val.FullName = "creative.togglecreativemodeuser";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Creative.toggleCreativeModeUser(arg);
		};
		array[395] = val;
		val = new Command();
		val.Name = "unlimitedio";
		val.Parent = "creative";
		val.FullName = "creative.unlimitedio";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "Bypasses limits on IO length and points";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Creative.unlimitedIo.ToString();
		val.SetOveride = delegate(string str)
		{
			Creative.unlimitedIo = StringExtensions.ToBool(str);
		};
		val.Default = "False";
		array[396] = val;
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
		array[397] = val;
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
		array[398] = val;
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
		array[399] = val;
		val = new Command();
		val.Name = "breakheld_almost";
		val.Parent = "debug";
		val.FullName = "debug.breakheld_almost";
		val.ServerAdmin = true;
		val.Description = "Almost break the current held object";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.breakheld_almost(arg);
		};
		array[400] = val;
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
		array[401] = val;
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
		array[402] = val;
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
		array[403] = val;
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
		array[404] = val;
		val = new Command();
		val.Name = "cleartutorialforplayer";
		val.Parent = "debug";
		val.FullName = "debug.cleartutorialforplayer";
		val.ServerAdmin = true;
		val.Description = "If a player ends up stuck on a tutorial for any reason this will clear the island and reset the player (will also kill player)";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.clearTutorialForPlayer(arg);
		};
		array[405] = val;
		val = new Command();
		val.Name = "completemission";
		val.Parent = "debug";
		val.FullName = "debug.completemission";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.completeMission(arg);
		};
		array[406] = val;
		val = new Command();
		val.Name = "completemissionstage";
		val.Parent = "debug";
		val.FullName = "debug.completemissionstage";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.completeMissionStage(arg);
		};
		array[407] = val;
		val = new Command();
		val.Name = "completetutorial";
		val.Parent = "debug";
		val.FullName = "debug.completetutorial";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.completeTutorial(arg);
		};
		array[408] = val;
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
		array[409] = val;
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
		array[410] = val;
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
		array[411] = val;
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
		array[412] = val;
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
		array[413] = val;
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
		array[414] = val;
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
		array[415] = val;
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
		array[416] = val;
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
		array[417] = val;
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
		array[418] = val;
		val = new Command();
		val.Name = "printgroups";
		val.Parent = "debug";
		val.FullName = "debug.printgroups";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.printgroups(arg);
		};
		array[419] = val;
		val = new Command();
		val.Name = "printmissionspeakinfo";
		val.Parent = "debug";
		val.FullName = "debug.printmissionspeakinfo";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Debugging.printMissionSpeakInfo.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.printMissionSpeakInfo = StringExtensions.ToBool(str);
		};
		array[420] = val;
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
		array[421] = val;
		val = new Command();
		val.Name = "quittutorial";
		val.Parent = "debug";
		val.FullName = "debug.quittutorial";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.quitTutorial(arg);
		};
		array[422] = val;
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
		array[423] = val;
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
		array[424] = val;
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
		array[425] = val;
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
		array[426] = val;
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
		array[427] = val;
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
		array[428] = val;
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
		array[429] = val;
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
		array[430] = val;
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
		array[431] = val;
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
		array[432] = val;
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
		array[433] = val;
		val = new Command();
		val.Name = "starttutorial";
		val.Parent = "debug";
		val.FullName = "debug.starttutorial";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.startTutorial(arg);
		};
		array[434] = val;
		val = new Command();
		val.Name = "testtutorialcinematic";
		val.Parent = "debug";
		val.FullName = "debug.testtutorialcinematic";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text24 = Debugging.testTutorialCinematic(arg);
			arg.ReplyWithObject((object)text24);
		};
		array[435] = val;
		val = new Command();
		val.Name = "tutorial_start_cooldown";
		val.Parent = "debug";
		val.FullName = "debug.tutorial_start_cooldown";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Debugging.tutorial_start_cooldown.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.tutorial_start_cooldown = StringExtensions.ToInt(str, 0);
		};
		array[436] = val;
		val = new Command();
		val.Name = "tutorialstatus";
		val.Parent = "debug";
		val.FullName = "debug.tutorialstatus";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.tutorialStatus(arg);
		};
		array[437] = val;
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
		array[438] = val;
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
		array[439] = val;
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
		array[440] = val;
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
		array[441] = val;
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
		array[442] = val;
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
		array[443] = val;
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
		array[444] = val;
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
		array[445] = val;
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
		array[446] = val;
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
		array[447] = val;
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
		array[448] = val;
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
		array[449] = val;
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
		array[450] = val;
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
		array[451] = val;
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
		array[452] = val;
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
		array[453] = val;
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
		array[454] = val;
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
		array[455] = val;
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
		array[456] = val;
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
		array[457] = val;
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
		array[458] = val;
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
		array[459] = val;
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
		array[460] = val;
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
		array[461] = val;
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
		array[462] = val;
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
		array[463] = val;
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
		array[464] = val;
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
		array[465] = val;
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
		array[466] = val;
		val = new Command();
		val.Name = "benchmark_demo_upload";
		val.Parent = "demo";
		val.FullName = "demo.benchmark_demo_upload";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Demo.BenchmarkDemoUpload(arg);
		};
		array[467] = val;
		val = new Command();
		val.Name = "delete_after_upload";
		val.Parent = "demo";
		val.FullName = "demo.delete_after_upload";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Should the full server demos be deleted after they are uploaded";
		val.Variable = true;
		val.GetOveride = () => Demo.DeleteDemoAfterUpload.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.DeleteDemoAfterUpload = StringExtensions.ToBool(str);
		};
		array[468] = val;
		val = new Command();
		val.Name = "server_demo_cleanup_interval";
		val.Parent = "demo";
		val.FullName = "demo.server_demo_cleanup_interval";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How many minutes between cleaning up demos from the disk";
		val.Variable = true;
		val.GetOveride = () => Demo.DemoDiskCleanupIntervalMinutes.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.DemoDiskCleanupIntervalMinutes = StringExtensions.ToInt(str, 0);
		};
		array[469] = val;
		val = new Command();
		val.Name = "max_upload_concurrency";
		val.Parent = "demo";
		val.FullName = "demo.max_upload_concurrency";
		val.ServerAdmin = true;
		val.Description = "Max parallel requests when uploading demos";
		val.Variable = true;
		val.GetOveride = () => Demo.DemoUploadConcurrency.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.DemoUploadConcurrency = StringExtensions.ToInt(str, 0);
		};
		array[470] = val;
		val = new Command();
		val.Name = "server_demo_disk_space_gb";
		val.Parent = "demo";
		val.FullName = "demo.server_demo_disk_space_gb";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How much disk space full server demos can take before we start to delete them";
		val.Variable = true;
		val.GetOveride = () => Demo.MaxDemoDiskSpaceGB.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.MaxDemoDiskSpaceGB = StringExtensions.ToInt(str, 0);
		};
		array[471] = val;
		val = new Command();
		val.Name = "record";
		val.Parent = "demo";
		val.FullName = "demo.record";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text23 = Demo.record(arg);
			arg.ReplyWithObject((object)text23);
		};
		array[472] = val;
		val = new Command();
		val.Name = "recordlist";
		val.Parent = "demo";
		val.FullName = "demo.recordlist";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Demo.recordlist ?? "";
		val.SetOveride = delegate(string str)
		{
			Demo.recordlist = str;
		};
		array[473] = val;
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
		array[474] = val;
		val = new Command();
		val.Name = "server_demo_directory";
		val.Parent = "demo";
		val.FullName = "demo.server_demo_directory";
		val.ServerAdmin = true;
		val.Description = "Directory to save full server demos";
		val.Variable = true;
		val.GetOveride = () => Demo.ServerDemoDirectory ?? "";
		val.SetOveride = delegate(string str)
		{
			Demo.ServerDemoDirectory = str;
		};
		array[475] = val;
		val = new Command();
		val.Name = "server_flush_seconds";
		val.Parent = "demo";
		val.FullName = "demo.server_flush_seconds";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Demo.ServerDemoFlushInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.ServerDemoFlushInterval = StringExtensions.ToInt(str, 0);
		};
		array[476] = val;
		val = new Command();
		val.Name = "full_server_demo";
		val.Parent = "demo";
		val.FullName = "demo.full_server_demo";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Demo.ServerDemosEnabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.ServerDemosEnabled = StringExtensions.ToBool(str);
		};
		array[477] = val;
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
		array[478] = val;
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
		array[479] = val;
		val = new Command();
		val.Name = "stop";
		val.Parent = "demo";
		val.FullName = "demo.stop";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text22 = Demo.stop(arg);
			arg.ReplyWithObject((object)text22);
		};
		array[480] = val;
		val = new Command();
		val.Name = "upload_bandwidth_limit_ratio";
		val.Parent = "demo";
		val.FullName = "demo.upload_bandwidth_limit_ratio";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Demo.UploadBandwidthLimitRatio.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.UploadBandwidthLimitRatio = StringExtensions.ToFloat(str, 0f);
		};
		array[481] = val;
		val = new Command();
		val.Name = "upload_demos";
		val.Parent = "demo";
		val.FullName = "demo.upload_demos";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Demo.UploadDemos.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.UploadDemos = StringExtensions.ToBool(str);
		};
		array[482] = val;
		val = new Command();
		val.Name = "upload_url";
		val.Parent = "demo";
		val.FullName = "demo.upload_url";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Demo.UploadUrl ?? "";
		val.SetOveride = delegate(string str)
		{
			Demo.UploadUrl = str;
		};
		array[483] = val;
		val = new Command();
		val.Name = "zip_demos";
		val.Parent = "demo";
		val.FullName = "demo.zip_demos";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Should we be zipping the demos before we upload them";
		val.Variable = true;
		val.GetOveride = () => Demo.ZipServerDemos.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.ZipServerDemos = StringExtensions.ToBool(str);
		};
		array[484] = val;
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
		array[485] = val;
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
		array[486] = val;
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
		array[487] = val;
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
		array[488] = val;
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
		array[489] = val;
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
		array[490] = val;
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
		array[491] = val;
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
		array[492] = val;
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
		array[493] = val;
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
		array[494] = val;
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
		array[495] = val;
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
		array[496] = val;
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
			string text21 = Entity.svspawn(arg.GetString(0, ""), arg.GetVector3(1, Vector3.zero), arg.GetVector3(2, Vector3.zero), arg.GetInt(3, 1));
			arg.ReplyWithObject((object)text21);
		};
		array[497] = val;
		val = new Command();
		val.Name = "spawngrid";
		val.Parent = "entity";
		val.FullName = "entity.spawngrid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text20 = Entity.svspawngrid(arg.GetString(0, ""), arg.GetInt(1, 5), arg.GetInt(2, 5), arg.GetInt(3, 5));
			arg.ReplyWithObject((object)text20);
		};
		array[498] = val;
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
			string text19 = Entity.svspawnitem(arg.GetString(0, ""), arg.GetVector3(1, Vector3.zero));
			arg.ReplyWithObject((object)text19);
		};
		array[499] = val;
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
		array[500] = val;
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
		array[501] = val;
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
		array[502] = val;
		val = new Command();
		val.Name = "nightlight_brightness";
		val.Parent = "env";
		val.FullName = "env.nightlight_brightness";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Env.nightlight_brightness.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.nightlight_brightness = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0.0175";
		array[503] = val;
		val = new Command();
		val.Name = "nightlight_distance";
		val.Parent = "env";
		val.FullName = "env.nightlight_distance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Env.nightlight_distance.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.nightlight_distance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "7";
		array[504] = val;
		val = new Command();
		val.Name = "nightlight_fadefraction";
		val.Parent = "env";
		val.FullName = "env.nightlight_fadefraction";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Env.nightlight_fadefraction.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.nightlight_fadefraction = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0.65";
		array[505] = val;
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
		array[506] = val;
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
		array[507] = val;
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
		array[508] = val;
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
		array[509] = val;
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
		array[510] = val;
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
		array[511] = val;
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
		array[512] = val;
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
		array[513] = val;
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
		array[514] = val;
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
		array[515] = val;
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
		array[516] = val;
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
		array[517] = val;
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
		array[518] = val;
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
		array[519] = val;
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
		array[520] = val;
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
		array[521] = val;
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
		array[522] = val;
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
		array[523] = val;
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
		array[524] = val;
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
		array[525] = val;
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
		array[526] = val;
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
		array[527] = val;
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
		array[528] = val;
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
		array[529] = val;
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
		array[530] = val;
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
		array[531] = val;
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
		array[532] = val;
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
		array[533] = val;
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
		array[534] = val;
		val = new Command();
		val.Name = "job_system_threads";
		val.Parent = "global";
		val.FullName = "global.job_system_threads";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Global.job_system_threads.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.job_system_threads = StringExtensions.ToInt(str, 0);
		};
		array[535] = val;
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
		array[536] = val;
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
		array[537] = val;
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
		array[538] = val;
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
		array[539] = val;
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
		array[540] = val;
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
		array[541] = val;
		val = new Command();
		val.Name = "printallscenesinbuild";
		val.Parent = "global";
		val.FullName = "global.printallscenesinbuild";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text18 = Global.printAllScenesInBuild(arg);
			arg.ReplyWithObject((object)text18);
		};
		array[542] = val;
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
		array[543] = val;
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
		array[544] = val;
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
		array[545] = val;
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
		array[546] = val;
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
		array[547] = val;
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
		array[548] = val;
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
		array[549] = val;
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
		array[550] = val;
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
		array[551] = val;
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
		array[552] = val;
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
		array[553] = val;
		val = new Command();
		val.Name = "sleeptarget";
		val.Parent = "global";
		val.FullName = "global.sleeptarget";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.sleeptarget(arg);
		};
		array[554] = val;
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
		array[555] = val;
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
		array[556] = val;
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
		array[557] = val;
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
		array[558] = val;
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
		array[559] = val;
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
		array[560] = val;
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
		array[561] = val;
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
		array[562] = val;
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
		array[563] = val;
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
		array[564] = val;
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
		array[565] = val;
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
		array[566] = val;
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
		array[567] = val;
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
		array[568] = val;
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
		array[569] = val;
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
		array[570] = val;
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
		array[571] = val;
		val = new Command();
		val.Name = "teleportnonsleepers2me";
		val.Parent = "global";
		val.FullName = "global.teleportnonsleepers2me";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleportnonsleepers2me(arg);
		};
		array[572] = val;
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
		array[573] = val;
		val = new Command();
		val.Name = "teleportsleepers2me";
		val.Parent = "global";
		val.FullName = "global.teleportsleepers2me";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleportsleepers2me(arg);
		};
		array[574] = val;
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
		array[575] = val;
		val = new Command();
		val.Name = "togglespectateteaminfo";
		val.Parent = "global";
		val.FullName = "global.togglespectateteaminfo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.toggleSpectateTeamInfo(arg);
		};
		array[576] = val;
		val = new Command();
		val.Name = "updatemanifest";
		val.Parent = "global";
		val.FullName = "global.updatemanifest";
		val.ServerAdmin = true;
		val.Client = true;
		val.Description = "Immediately update the manifest";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.UpdateManifest(arg);
		};
		array[577] = val;
		val = new Command();
		val.Name = "updatenetworkpositionwithdebugcamerawhilespectating";
		val.Parent = "global";
		val.FullName = "global.updatenetworkpositionwithdebugcamerawhilespectating";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Global.updateNetworkPositionWithDebugCameraWhileSpectating.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.updateNetworkPositionWithDebugCameraWhileSpectating = StringExtensions.ToBool(str);
		};
		array[578] = val;
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
		array[579] = val;
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
		array[580] = val;
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
		array[581] = val;
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
		array[582] = val;
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
		array[583] = val;
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
		array[584] = val;
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
		array[585] = val;
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
		array[586] = val;
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
		array[587] = val;
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
		array[588] = val;
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
		array[589] = val;
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
		array[590] = val;
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
		array[591] = val;
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
		array[592] = val;
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
		array[593] = val;
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
		array[594] = val;
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
		array[595] = val;
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
		array[596] = val;
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
		array[597] = val;
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
		array[598] = val;
		val = new Command();
		val.Name = "disableattirelimitations";
		val.Parent = "inventory";
		val.FullName = "inventory.disableattirelimitations";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "Disables all attire limitations, so NPC clothing and invalid overlaps can be equipped";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Inventory.disableAttireLimitations.ToString();
		val.SetOveride = delegate(string str)
		{
			Inventory.disableAttireLimitations = StringExtensions.ToBool(str);
		};
		val.Default = "False";
		array[599] = val;
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
		array[600] = val;
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
		array[601] = val;
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
		array[602] = val;
		val = new Command();
		val.Name = "give";
		val.Parent = "inventory";
		val.FullName = "inventory.give";
		val.ServerAdmin = true;
		val.Description = "{item} {amount} {condition} {skin} {container} {slot}";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.give(arg);
		};
		array[603] = val;
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
		array[604] = val;
		val = new Command();
		val.Name = "givearm";
		val.Parent = "inventory";
		val.FullName = "inventory.givearm";
		val.ServerAdmin = true;
		val.Description = "{itemid} {amount}";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.givearm(arg);
		};
		array[605] = val;
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
		array[606] = val;
		val = new Command();
		val.Name = "giveid";
		val.Parent = "inventory";
		val.FullName = "inventory.giveid";
		val.ServerAdmin = true;
		val.Description = "{itemid} {amount}";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.giveid(arg);
		};
		array[607] = val;
		val = new Command();
		val.Name = "giveto";
		val.Parent = "inventory";
		val.FullName = "inventory.giveto";
		val.ServerAdmin = true;
		val.Description = "{item} {player} {amount} {skin}";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.giveto(arg);
		};
		array[608] = val;
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
		array[609] = val;
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
		array[610] = val;
		val = new Command();
		val.Name = "pipetteid";
		val.Parent = "inventory";
		val.FullName = "inventory.pipetteid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.pipetteid(arg);
		};
		array[611] = val;
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
		array[612] = val;
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
		array[613] = val;
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
		array[614] = val;
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
		array[615] = val;
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
		array[616] = val;
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
		array[617] = val;
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
		array[618] = val;
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
		array[619] = val;
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
		array[620] = val;
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
		array[621] = val;
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
		array[622] = val;
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
		array[623] = val;
		val = new Command();
		val.Name = "network_group_debug";
		val.Parent = "net";
		val.FullName = "net.network_group_debug";
		val.ServerAdmin = true;
		val.Description = "Toggle checking network group bounds whenever an entity changes its network group";
		val.Variable = true;
		val.GetOveride = () => Net.network_group_debug.ToString();
		val.SetOveride = delegate(string str)
		{
			Net.network_group_debug = StringExtensions.ToBool(str);
		};
		array[624] = val;
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
		array[625] = val;
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
		array[626] = val;
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
		array[627] = val;
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
		array[628] = val;
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
		array[629] = val;
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
		array[630] = val;
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
		val.GetOveride = () => Nexus.endpoint ?? "";
		val.SetOveride = delegate(string str)
		{
			Nexus.endpoint = str;
		};
		val.Default = "https://api.facepunch.com/api/nexus/";
		array[631] = val;
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
		array[632] = val;
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
		array[633] = val;
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
		array[634] = val;
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
		array[635] = val;
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
		array[636] = val;
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
		array[637] = val;
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
		array[638] = val;
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
		array[639] = val;
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
		array[640] = val;
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
		array[641] = val;
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
		array[642] = val;
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
		array[643] = val;
		val = new Command();
		val.Name = "secretkey";
		val.Parent = "nexus";
		val.FullName = "nexus.secretkey";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Nexus.secretKey ?? "";
		val.SetOveride = delegate(string str)
		{
			Nexus.secretKey = str;
		};
		array[644] = val;
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
		array[645] = val;
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
		array[646] = val;
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
		array[647] = val;
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
		array[648] = val;
		val = new Command();
		val.Name = "zonecontroller";
		val.Parent = "nexus";
		val.FullName = "nexus.zonecontroller";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Nexus.zoneController ?? "";
		val.SetOveride = delegate(string str)
		{
			Nexus.zoneController = str;
		};
		array[649] = val;
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
		array[650] = val;
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
		array[651] = val;
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
		array[652] = val;
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
		array[653] = val;
		val = new Command();
		val.Name = "death";
		val.Parent = "heli";
		val.FullName = "heli.death";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.PatrolHelicopter.death(arg);
		};
		array[654] = val;
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
		array[655] = val;
		val = new Command();
		val.Name = "flee";
		val.Parent = "heli";
		val.FullName = "heli.flee";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.PatrolHelicopter.flee(arg);
		};
		array[656] = val;
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
		array[657] = val;
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
		array[658] = val;
		val = new Command();
		val.Name = "move";
		val.Parent = "heli";
		val.FullName = "heli.move";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.PatrolHelicopter.move(arg);
		};
		array[659] = val;
		val = new Command();
		val.Name = "orbit";
		val.Parent = "heli";
		val.FullName = "heli.orbit";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.PatrolHelicopter.orbit(arg);
		};
		array[660] = val;
		val = new Command();
		val.Name = "orbitstrafe";
		val.Parent = "heli";
		val.FullName = "heli.orbitstrafe";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.PatrolHelicopter.orbitstrafe(arg);
		};
		array[661] = val;
		val = new Command();
		val.Name = "patrol";
		val.Parent = "heli";
		val.FullName = "heli.patrol";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.PatrolHelicopter.patrol(arg);
		};
		array[662] = val;
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
		array[663] = val;
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
		array[664] = val;
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
		array[665] = val;
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
		array[666] = val;
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
		array[667] = val;
		val = new Command();
		val.Name = "droppedmode";
		val.Parent = "physics";
		val.FullName = "physics.droppedmode";
		val.ServerAdmin = true;
		val.Description = "The collision detection mode that dropped items and corpses should use";
		val.Variable = true;
		val.GetOveride = () => Physics.droppedmode.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.droppedmode = StringExtensions.ToInt(str, 0);
		};
		array[668] = val;
		val = new Command();
		val.Name = "gravity";
		val.Parent = "physics";
		val.FullName = "physics.gravity";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "Gravity multiplier";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Physics.gravity.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.gravity = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "1.0";
		array[669] = val;
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
		array[670] = val;
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
		array[671] = val;
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
		array[672] = val;
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
		array[673] = val;
		val = new Command();
		val.Name = "serverragdollmode";
		val.Parent = "physics";
		val.FullName = "physics.serverragdollmode";
		val.ServerAdmin = true;
		val.Description = "The collision detection mode that server-side ragdolls should use";
		val.Variable = true;
		val.GetOveride = () => Physics.serverragdollmode.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.serverragdollmode = StringExtensions.ToInt(str, 0);
		};
		array[674] = val;
		val = new Command();
		val.Name = "serversideragdolls";
		val.Parent = "physics";
		val.FullName = "physics.serversideragdolls";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "Do ragdoll physics calculations on the server, or use the old client-side system";
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Physics.serversideragdolls.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.serversideragdolls = StringExtensions.ToBool(str);
		};
		val.Default = "True";
		array[675] = val;
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
		array[676] = val;
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
		array[677] = val;
		val = new Command();
		val.Name = "treecollision";
		val.Parent = "physics";
		val.FullName = "physics.treecollision";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "Do players and vehicles collide with trees?";
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Physics.treecollision.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.treecollision = StringExtensions.ToBool(str);
		};
		val.Default = "True";
		array[678] = val;
		val = new Command();
		val.Name = "auto_refresh_region";
		val.Parent = "ping";
		val.FullName = "ping.auto_refresh_region";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Ping.auto_refresh_region.ToString();
		val.SetOveride = delegate(string str)
		{
			Ping.auto_refresh_region = StringExtensions.ToBool(str);
		};
		array[679] = val;
		val = new Command();
		val.Name = "ping_estimate_logging";
		val.Parent = "ping";
		val.FullName = "ping.ping_estimate_logging";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Ping.ping_estimate_logging.ToString();
		val.SetOveride = delegate(string str)
		{
			Ping.ping_estimate_logging = StringExtensions.ToBool(str);
		};
		array[680] = val;
		val = new Command();
		val.Name = "ping_estimation";
		val.Parent = "ping";
		val.FullName = "ping.ping_estimation";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Ping.ping_estimation.ToString();
		val.SetOveride = delegate(string str)
		{
			Ping.ping_estimation = StringExtensions.ToBool(str);
		};
		array[681] = val;
		val = new Command();
		val.Name = "ping_parallel";
		val.Parent = "ping";
		val.FullName = "ping.ping_parallel";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Ping.ping_parallel.ToString();
		val.SetOveride = delegate(string str)
		{
			Ping.ping_parallel = StringExtensions.ToBool(str);
		};
		array[682] = val;
		val = new Command();
		val.Name = "ping_refresh_interval";
		val.Parent = "ping";
		val.FullName = "ping.ping_refresh_interval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Ping.ping_refresh_interval.ToString();
		val.SetOveride = delegate(string str)
		{
			Ping.ping_refresh_interval = StringExtensions.ToInt(str, 0);
		};
		array[683] = val;
		val = new Command();
		val.Name = "ping_samples";
		val.Parent = "ping";
		val.FullName = "ping.ping_samples";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Ping.ping_samples.ToString();
		val.SetOveride = delegate(string str)
		{
			Ping.ping_samples = StringExtensions.ToInt(str, 0);
		};
		array[684] = val;
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
		array[685] = val;
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
		array[686] = val;
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
		array[687] = val;
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
		array[688] = val;
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
		array[689] = val;
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
		array[690] = val;
		val = new Command();
		val.Name = "createtrophy";
		val.Parent = "player";
		val.FullName = "player.createtrophy";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text17 = Player.createTrophy(arg);
			arg.ReplyWithObject((object)text17);
		};
		array[691] = val;
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
		array[692] = val;
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
		array[693] = val;
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
		array[694] = val;
		val = new Command();
		val.Name = "gesture_radius_notme";
		val.Parent = "player";
		val.FullName = "player.gesture_radius_notme";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.gesture_radius_notme(arg);
		};
		array[695] = val;
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
		array[696] = val;
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
		array[697] = val;
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
		array[698] = val;
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
		array[699] = val;
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
		array[700] = val;
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
		array[701] = val;
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
		array[702] = val;
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
		array[703] = val;
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
		array[704] = val;
		val = new Command();
		val.Name = "tickrate_cl";
		val.Parent = "player";
		val.FullName = "player.tickrate_cl";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Player.tickrate_cl.ToString();
		val.SetOveride = delegate(string str)
		{
			Player.tickrate_cl = StringExtensions.ToInt(str, 0);
		};
		val.Default = "32";
		array[705] = val;
		val = new Command();
		val.Name = "tickrate_sv";
		val.Parent = "player";
		val.FullName = "player.tickrate_sv";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Player.tickrate_sv.ToString();
		val.SetOveride = delegate(string str)
		{
			Player.tickrate_sv = StringExtensions.ToInt(str, 0);
		};
		val.Default = "16";
		array[706] = val;
		val = new Command();
		val.Name = "trigger_wildlife_trap";
		val.Parent = "player";
		val.FullName = "player.trigger_wildlife_trap";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.trigger_wildlife_trap(arg);
		};
		array[707] = val;
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
		array[708] = val;
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
		array[709] = val;
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
		array[710] = val;
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
		array[711] = val;
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
		array[712] = val;
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
		array[713] = val;
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
		array[714] = val;
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
		array[715] = val;
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
		array[716] = val;
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
		array[717] = val;
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
		array[718] = val;
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
		array[719] = val;
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
		array[720] = val;
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
		array[721] = val;
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
		array[722] = val;
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
		array[723] = val;
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
		array[724] = val;
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
		array[725] = val;
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
		array[726] = val;
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
		array[727] = val;
		val = new Command();
		val.Name = "show_building_blocked_server";
		val.Parent = "render";
		val.FullName = "render.show_building_blocked_server";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Render.show_building_blocked_server.ToString();
		val.SetOveride = delegate(string str)
		{
			Render.show_building_blocked_server = StringExtensions.ToBool(str);
		};
		val.Default = "True";
		array[728] = val;
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
		array[729] = val;
		val = new Command();
		val.Name = "debugpreventduplicates";
		val.Parent = "sentry";
		val.FullName = "sentry.debugpreventduplicates";
		val.ServerAdmin = true;
		val.Description = "Prevents auto turrets getting added more than once to the IO queue";
		val.Variable = true;
		val.GetOveride = () => Sentry.debugPreventDuplicates.ToString();
		val.SetOveride = delegate(string str)
		{
			Sentry.debugPreventDuplicates = StringExtensions.ToBool(str);
		};
		array[730] = val;
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
		array[731] = val;
		val = new Command();
		val.Name = "interferenceradius";
		val.Parent = "sentry";
		val.FullName = "sentry.interferenceradius";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "radius to check for other turrets";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Sentry.interferenceradius.ToString();
		val.SetOveride = delegate(string str)
		{
			Sentry.interferenceradius = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "40";
		array[732] = val;
		val = new Command();
		val.Name = "maxinterference";
		val.Parent = "sentry";
		val.FullName = "sentry.maxinterference";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "max interference from other turrets";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Sentry.maxinterference.ToString();
		val.SetOveride = delegate(string str)
		{
			Sentry.maxinterference = StringExtensions.ToInt(str, 0);
		};
		val.Default = "12";
		array[733] = val;
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
		array[734] = val;
		val = new Command();
		val.Name = "anticheatid";
		val.Parent = "server";
		val.FullName = "server.anticheatid";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.anticheatid ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.anticheatid = str;
		};
		array[735] = val;
		val = new Command();
		val.Name = "anticheatkey";
		val.Parent = "server";
		val.FullName = "server.anticheatkey";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.anticheatkey ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.anticheatkey = str;
		};
		array[736] = val;
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
		array[737] = val;
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
		array[738] = val;
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
		array[739] = val;
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
		array[740] = val;
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
		array[741] = val;
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
		array[742] = val;
		val = new Command();
		val.Name = "autouploadmap";
		val.Parent = "server";
		val.FullName = "server.autouploadmap";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Automatically upload procedurally generated maps so that players download them (faster) instead of re-generating them";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.autoUploadMap.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.autoUploadMap = StringExtensions.ToBool(str);
		};
		array[743] = val;
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
		array[744] = val;
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
		array[745] = val;
		val = new Command();
		val.Name = "bansserverendpoint";
		val.Parent = "server";
		val.FullName = "server.bansserverendpoint";
		val.ServerAdmin = true;
		val.Description = "HTTP API endpoint for centralized banning (see wiki)";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.bansServerEndpoint ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.bansServerEndpoint = str;
		};
		array[746] = val;
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
		array[747] = val;
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
		array[748] = val;
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
		array[749] = val;
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
		array[750] = val;
		val = new Command();
		val.Name = "branch";
		val.Parent = "server";
		val.FullName = "server.branch";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.branch ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.branch = str;
		};
		array[751] = val;
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
		array[752] = val;
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
		array[753] = val;
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
		array[754] = val;
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
		array[755] = val;
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
		array[756] = val;
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
		array[757] = val;
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
		array[758] = val;
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
		array[759] = val;
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
		array[760] = val;
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
			string text16 = ConVar.Server.combatlog(arg);
			arg.ReplyWithObject((object)text16);
		};
		array[761] = val;
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
			string text15 = ConVar.Server.combatlog_outgoing(arg);
			arg.ReplyWithObject((object)text15);
		};
		array[762] = val;
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
		array[763] = val;
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
		array[764] = val;
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
		array[765] = val;
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
		array[766] = val;
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
		array[767] = val;
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
		array[768] = val;
		val = new Command();
		val.Name = "corpseinfo";
		val.Parent = "server";
		val.FullName = "server.corpseinfo";
		val.ServerAdmin = true;
		val.Description = "Get info on player corpses on the server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.corpseinfo(arg);
		};
		array[769] = val;
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
		array[770] = val;
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
		array[771] = val;
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
		array[772] = val;
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
		array[773] = val;
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
		array[774] = val;
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
		array[775] = val;
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
		array[776] = val;
		val = new Command();
		val.Name = "description";
		val.Parent = "server";
		val.FullName = "server.description";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.description ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.description = str;
		};
		array[777] = val;
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
		array[778] = val;
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
		array[779] = val;
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
		array[780] = val;
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
		array[781] = val;
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
		array[782] = val;
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
		array[783] = val;
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
		array[784] = val;
		val = new Command();
		val.Name = "favoritesendpoint";
		val.Parent = "server";
		val.FullName = "server.favoritesendpoint";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Domain name to save when players favorite your server. The port can be omitted if using the default port or a SRV DNS record is created.";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.favoritesEndpoint ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.favoritesEndpoint = str;
		};
		array[785] = val;
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
		array[786] = val;
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
		array[787] = val;
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
		array[788] = val;
		val = new Command();
		val.Name = "gamemode";
		val.Parent = "server";
		val.FullName = "server.gamemode";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.gamemode ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.gamemode = str;
		};
		array[789] = val;
		val = new Command();
		val.Name = "headerimage";
		val.Parent = "server";
		val.FullName = "server.headerimage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.headerimage ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.headerimage = str;
		};
		array[790] = val;
		val = new Command();
		val.Name = "hostname";
		val.Parent = "server";
		val.FullName = "server.hostname";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.hostname ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.hostname = str;
		};
		array[791] = val;
		val = new Command();
		val.Name = "identity";
		val.Parent = "server";
		val.FullName = "server.identity";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.identity ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.identity = str;
		};
		array[792] = val;
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
		array[793] = val;
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
		array[794] = val;
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
		array[795] = val;
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
		array[796] = val;
		val = new Command();
		val.Name = "industrialallowquickmove";
		val.Parent = "server";
		val.FullName = "server.industrialallowquickmove";
		val.ServerAdmin = true;
		val.Description = "Enables a faster way to move items around during conveyor transfers. Should be on unless there's a issue";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.industrialAllowQuickMove.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.industrialAllowQuickMove = StringExtensions.ToBool(str);
		};
		array[797] = val;
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
		array[798] = val;
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
		array[799] = val;
		val = new Command();
		val.Name = "industrialtransferstricttimelimits";
		val.Parent = "server";
		val.FullName = "server.industrialtransferstricttimelimits";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "When enabled industrial transfers will abort if they start to take too long. Will lead to inconsistent splitting but should retain performance";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.industrialTransferStrictTimeLimits.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.industrialTransferStrictTimeLimits = StringExtensions.ToBool(str);
		};
		array[800] = val;
		val = new Command();
		val.Name = "ip";
		val.Parent = "server";
		val.FullName = "server.ip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.ip ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.ip = str;
		};
		array[801] = val;
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
		array[802] = val;
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
		array[803] = val;
		val = new Command();
		val.Name = "itemdespawn_container_max_multiplier";
		val.Parent = "server";
		val.FullName = "server.itemdespawn_container_max_multiplier";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.itemdespawn_container_max_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.itemdespawn_container_max_multiplier = StringExtensions.ToInt(str, 0);
		};
		array[804] = val;
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
		array[805] = val;
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
		array[806] = val;
		val = new Command();
		val.Name = "level";
		val.Parent = "server";
		val.FullName = "server.level";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.level ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.level = str;
		};
		array[807] = val;
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
		array[808] = val;
		val = new Command();
		val.Name = "levelurl";
		val.Parent = "server";
		val.FullName = "server.levelurl";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.levelurl ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.levelurl = str;
		};
		array[809] = val;
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
		array[810] = val;
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
		array[811] = val;
		val = new Command();
		val.Name = "logoimage";
		val.Parent = "server";
		val.FullName = "server.logoimage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.logoimage ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.logoimage = str;
		};
		array[812] = val;
		val = new Command();
		val.Name = "max_shelters";
		val.Parent = "server";
		val.FullName = "server.max_shelters";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.max_shelters.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.max_shelters = StringExtensions.ToInt(str, 0);
		};
		val.Default = "1";
		array[813] = val;
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
		array[814] = val;
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
		array[815] = val;
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
		array[816] = val;
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
		array[817] = val;
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
		array[818] = val;
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
		array[819] = val;
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
		array[820] = val;
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
		array[821] = val;
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
		array[822] = val;
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
		array[823] = val;
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
		array[824] = val;
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
		array[825] = val;
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
		array[826] = val;
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
		array[827] = val;
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
		array[828] = val;
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
		array[829] = val;
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
		array[830] = val;
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
		array[831] = val;
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
		array[832] = val;
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
		array[833] = val;
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
		array[834] = val;
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
		array[835] = val;
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
		array[836] = val;
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
		array[837] = val;
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
		array[838] = val;
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
		array[839] = val;
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
		array[840] = val;
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
		array[841] = val;
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
		array[842] = val;
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
		array[843] = val;
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
		array[844] = val;
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
		array[845] = val;
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
		array[846] = val;
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
		val.GetOveride = () => ConVar.Server.motd ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.motd = str;
		};
		val.Default = "";
		array[847] = val;
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
		array[848] = val;
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
		array[849] = val;
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
		array[850] = val;
		val = new Command();
		val.Name = "netprotocol";
		val.Parent = "server";
		val.FullName = "server.netprotocol";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text14 = ConVar.Server.netprotocol(arg);
			arg.ReplyWithObject((object)text14);
		};
		array[851] = val;
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
		array[852] = val;
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
		array[853] = val;
		val = new Command();
		val.Name = "oilrig_radiation_alarm_threshold";
		val.Parent = "server";
		val.FullName = "server.oilrig_radiation_alarm_threshold";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.oilrig_radiation_alarm_threshold.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.oilrig_radiation_alarm_threshold = StringExtensions.ToFloat(str, 0f);
		};
		array[854] = val;
		val = new Command();
		val.Name = "oilrig_radiation_amount_scale";
		val.Parent = "server";
		val.FullName = "server.oilrig_radiation_amount_scale";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.oilrig_radiation_amount_scale.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.oilrig_radiation_amount_scale = StringExtensions.ToFloat(str, 0f);
		};
		array[855] = val;
		val = new Command();
		val.Name = "oilrig_radiation_time_scale";
		val.Parent = "server";
		val.FullName = "server.oilrig_radiation_time_scale";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.oilrig_radiation_time_scale.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.oilrig_radiation_time_scale = StringExtensions.ToFloat(str, 0f);
		};
		array[856] = val;
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
		array[857] = val;
		val = new Command();
		val.Name = "packetlog";
		val.Parent = "server";
		val.FullName = "server.packetlog";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text13 = ConVar.Server.packetlog(arg);
			arg.ReplyWithObject((object)text13);
		};
		array[858] = val;
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
		array[859] = val;
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
		array[860] = val;
		val = new Command();
		val.Name = "ping_region_code_override";
		val.Parent = "server";
		val.FullName = "server.ping_region_code_override";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.ping_region_code_override ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.ping_region_code_override = str;
		};
		array[861] = val;
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
		array[862] = val;
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
		array[863] = val;
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
		array[864] = val;
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
		array[865] = val;
		val = new Command();
		val.Name = "player_state_cache_count";
		val.Parent = "server";
		val.FullName = "server.player_state_cache_count";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.player_state_cache_count(arg);
		};
		array[866] = val;
		val = new Command();
		val.Name = "player_state_cache_evictions";
		val.Parent = "server";
		val.FullName = "server.player_state_cache_evictions";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Server.player_state_cache_evictions(arg);
		};
		array[867] = val;
		val = new Command();
		val.Name = "player_state_cache_size";
		val.Parent = "server";
		val.FullName = "server.player_state_cache_size";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.player_state_cache_size.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.player_state_cache_size = StringExtensions.ToInt(str, 0);
		};
		array[868] = val;
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
		array[869] = val;
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
		array[870] = val;
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
		array[871] = val;
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
		array[872] = val;
		val = new Command();
		val.Name = "printdecryptqueue";
		val.Parent = "server";
		val.FullName = "server.printdecryptqueue";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text12 = ConVar.Server.printdecryptqueue(arg);
			arg.ReplyWithObject((object)text12);
		};
		array[873] = val;
		val = new Command();
		val.Name = "printeyes";
		val.Parent = "server";
		val.FullName = "server.printeyes";
		val.ServerAdmin = true;
		val.Description = "Print the current player eyes.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text11 = ConVar.Server.printeyes(arg);
			arg.ReplyWithObject((object)text11);
		};
		array[874] = val;
		val = new Command();
		val.Name = "printpos";
		val.Parent = "server";
		val.FullName = "server.printpos";
		val.ServerAdmin = true;
		val.Description = "Print the current player position.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text10 = ConVar.Server.printpos(arg);
			arg.ReplyWithObject((object)text10);
		};
		array[875] = val;
		val = new Command();
		val.Name = "printreadqueue";
		val.Parent = "server";
		val.FullName = "server.printreadqueue";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text9 = ConVar.Server.printreadqueue(arg);
			arg.ReplyWithObject((object)text9);
		};
		array[876] = val;
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
		array[877] = val;
		val = new Command();
		val.Name = "printrot";
		val.Parent = "server";
		val.FullName = "server.printrot";
		val.ServerAdmin = true;
		val.Description = "Print the current player rotation.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text8 = ConVar.Server.printrot(arg);
			arg.ReplyWithObject((object)text8);
		};
		array[878] = val;
		val = new Command();
		val.Name = "printwritequeue";
		val.Parent = "server";
		val.FullName = "server.printwritequeue";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text7 = ConVar.Server.printwritequeue(arg);
			arg.ReplyWithObject((object)text7);
		};
		array[879] = val;
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
		array[880] = val;
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
		array[881] = val;
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
		array[882] = val;
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
		array[883] = val;
		val = new Command();
		val.Name = "readcfg";
		val.Parent = "server";
		val.FullName = "server.readcfg";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text6 = ConVar.Server.readcfg(arg);
			arg.ReplyWithObject((object)text6);
		};
		array[884] = val;
		val = new Command();
		val.Name = "rejoin_delay";
		val.Parent = "server";
		val.FullName = "server.rejoin_delay";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.rejoin_delay.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.rejoin_delay = StringExtensions.ToInt(str, 0);
		};
		array[885] = val;
		val = new Command();
		val.Name = "reportsserverendpoint";
		val.Parent = "server";
		val.FullName = "server.reportsserverendpoint";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "HTTP API endpoint for receiving F7 reports";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.reportsServerEndpoint ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.reportsServerEndpoint = str;
		};
		array[886] = val;
		val = new Command();
		val.Name = "reportsserverendpointkey";
		val.Parent = "server";
		val.FullName = "server.reportsserverendpointkey";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "If set, this key will be included with any reports sent via reportsServerEndpoint (for validation)";
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.reportsServerEndpointKey ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.reportsServerEndpointKey = str;
		};
		array[887] = val;
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
		array[888] = val;
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
		array[889] = val;
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
		array[890] = val;
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
		array[891] = val;
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
		array[892] = val;
		val = new Command();
		val.Name = "rpclog";
		val.Parent = "server";
		val.FullName = "server.rpclog";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text5 = ConVar.Server.rpclog(arg);
			arg.ReplyWithObject((object)text5);
		};
		array[893] = val;
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
		array[894] = val;
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
		array[895] = val;
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
		array[896] = val;
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
		array[897] = val;
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
		array[898] = val;
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
		array[899] = val;
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
		array[900] = val;
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
		array[901] = val;
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
		array[902] = val;
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
		array[903] = val;
		val = new Command();
		val.Name = "server_id";
		val.Parent = "server";
		val.FullName = "server.server_id";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.server_id ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.server_id = str;
		};
		array[904] = val;
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
		array[905] = val;
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
		array[906] = val;
		val = new Command();
		val.Name = "skipdeathscreenfade";
		val.Parent = "server";
		val.FullName = "server.skipdeathscreenfade";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "Skip death screen fade";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.skipDeathScreenFade.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.skipDeathScreenFade = StringExtensions.ToBool(str);
		};
		val.Default = "False";
		array[907] = val;
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
		array[908] = val;
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
		array[909] = val;
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
		array[910] = val;
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
		array[911] = val;
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
		array[912] = val;
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
		array[913] = val;
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
		array[914] = val;
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
		array[915] = val;
		val = new Command();
		val.Name = "strictauth_eac";
		val.Parent = "server";
		val.FullName = "server.strictauth_eac";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.strictauth_eac.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.strictauth_eac = StringExtensions.ToBool(str);
		};
		array[916] = val;
		val = new Command();
		val.Name = "strictauth_steam";
		val.Parent = "server";
		val.FullName = "server.strictauth_steam";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.strictauth_steam.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.strictauth_steam = StringExtensions.ToBool(str);
		};
		array[917] = val;
		val = new Command();
		val.Name = "tags";
		val.Parent = "server";
		val.FullName = "server.tags";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Comma-separated server browser tag values (see wiki)";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.tags ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.tags = str;
		};
		array[918] = val;
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
		array[919] = val;
		val = new Command();
		val.Name = "tutorialenabled";
		val.Parent = "server";
		val.FullName = "server.tutorialenabled";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "Controls whether the tutorial is enabled on this server";
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.tutorialEnabled.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.tutorialEnabled = StringExtensions.ToBool(str);
		};
		val.Default = "false";
		array[920] = val;
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
		array[921] = val;
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
		array[922] = val;
		val = new Command();
		val.Name = "url";
		val.Parent = "server";
		val.FullName = "server.url";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.url ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.url = str;
		};
		array[923] = val;
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
		array[924] = val;
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
		array[925] = val;
		val = new Command();
		val.Name = "workbench1taxrate";
		val.Parent = "server";
		val.FullName = "server.workbench1taxrate";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "How much of a tax to apply to workbench T1 tech unlocks. 10 = additional 10% scrap cost";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.workbench1TaxRate.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.workbench1TaxRate = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[926] = val;
		val = new Command();
		val.Name = "workbench2taxrate";
		val.Parent = "server";
		val.FullName = "server.workbench2taxrate";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "How much of a tax to apply to workbench T2 tech unlocks. 10 = additional 10% scrap cost";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.workbench2TaxRate.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.workbench2TaxRate = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "10";
		array[927] = val;
		val = new Command();
		val.Name = "workbench3taxrate";
		val.Parent = "server";
		val.FullName = "server.workbench3taxrate";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "How much of a tax to apply to workbench  T3tech unlocks. 10 = additional 10% scrap cost";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Server.workbench3TaxRate.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Server.workbench3TaxRate = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "20";
		array[928] = val;
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
		array[929] = val;
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
		array[930] = val;
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
		array[931] = val;
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
		array[932] = val;
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
		array[933] = val;
		val = new Command();
		val.Name = "cargoshipdockingtest";
		val.Parent = "spawn";
		val.FullName = "spawn.cargoshipdockingtest";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.cargoshipdockingtest(arg);
		};
		array[934] = val;
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
		array[935] = val;
		val = new Command();
		val.Name = "ch47event";
		val.Parent = "spawn";
		val.FullName = "spawn.ch47event";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.ch47event(arg);
		};
		array[936] = val;
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
		array[937] = val;
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
		array[938] = val;
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
		array[939] = val;
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
		array[940] = val;
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
		array[941] = val;
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
		array[942] = val;
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
		array[943] = val;
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
		array[944] = val;
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
		array[945] = val;
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
		array[946] = val;
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
		array[947] = val;
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
		array[948] = val;
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
		array[949] = val;
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
		array[950] = val;
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
		array[951] = val;
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
		array[952] = val;
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
		array[953] = val;
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
		array[954] = val;
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
		array[955] = val;
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
		array[956] = val;
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
		array[957] = val;
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
		array[958] = val;
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
		array[959] = val;
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
		array[960] = val;
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
		array[961] = val;
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
		array[962] = val;
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
		array[963] = val;
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
		array[964] = val;
		val = new Command();
		val.Name = "cl_maxstepsperframe";
		val.Parent = "time";
		val.FullName = "time.cl_maxstepsperframe";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "The maximum amount physics ticks per frame on clients. If things are taking too long, time slows down";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Time.cl_maxstepsperframe.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.cl_maxstepsperframe = StringExtensions.ToInt(str, 0);
		};
		val.Default = "2";
		array[965] = val;
		val = new Command();
		val.Name = "cl_steps";
		val.Parent = "time";
		val.FullName = "time.cl_steps";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "Desired physics ticks per second on clients";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Time.cl_steps.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.cl_steps = StringExtensions.ToInt(str, 0);
		};
		val.Default = "32";
		array[966] = val;
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
		array[967] = val;
		val = new Command();
		val.Name = "sv_maxstepsperframe";
		val.Parent = "time";
		val.FullName = "time.sv_maxstepsperframe";
		val.ServerAdmin = true;
		val.Description = "The maximum amount physics ticks per frame on the server. If things are taking too long, time slows down";
		val.Variable = true;
		val.GetOveride = () => Time.sv_maxstepsperframe.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.sv_maxstepsperframe = StringExtensions.ToInt(str, 0);
		};
		array[968] = val;
		val = new Command();
		val.Name = "sv_steps";
		val.Parent = "time";
		val.FullName = "time.sv_steps";
		val.ServerAdmin = true;
		val.Description = "Desired physics ticks per second on the server";
		val.Variable = true;
		val.GetOveride = () => Time.sv_steps.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.sv_steps = StringExtensions.ToInt(str, 0);
		};
		array[969] = val;
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
		array[970] = val;
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
		array[971] = val;
		val = new Command();
		val.Name = "simplified_collider";
		val.Parent = "tree";
		val.FullName = "tree.simplified_collider";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Tree.simplified_collider.ToString();
		val.SetOveride = delegate(string str)
		{
			Tree.simplified_collider = StringExtensions.ToBool(str);
		};
		array[972] = val;
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
		array[973] = val;
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
		array[974] = val;
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
		array[975] = val;
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
		array[976] = val;
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
		array[977] = val;
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
		array[978] = val;
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
		array[979] = val;
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
		array[980] = val;
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
		array[981] = val;
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
		array[982] = val;
		val = new Command();
		val.Name = "killmotorbikes";
		val.Parent = "vehicle";
		val.FullName = "vehicle.killmotorbikes";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.killmotorbikes(arg);
		};
		array[983] = val;
		val = new Command();
		val.Name = "killpushbikes";
		val.Parent = "vehicle";
		val.FullName = "vehicle.killpushbikes";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.killpushbikes(arg);
		};
		array[984] = val;
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
		array[985] = val;
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
		array[986] = val;
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
		array[987] = val;
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
		array[988] = val;
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
		array[989] = val;
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
		array[990] = val;
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
		array[991] = val;
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
		array[992] = val;
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
		array[993] = val;
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
		array[994] = val;
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
		array[995] = val;
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
		array[996] = val;
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
		array[997] = val;
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
		array[998] = val;
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
		array[999] = val;
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
		array[1000] = val;
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
		array[1001] = val;
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
		array[1002] = val;
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
		array[1003] = val;
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
		array[1004] = val;
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
		array[1005] = val;
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
		array[1006] = val;
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
		array[1007] = val;
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
		array[1008] = val;
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
		array[1009] = val;
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
		array[1010] = val;
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
		array[1011] = val;
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
		array[1012] = val;
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
		array[1013] = val;
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
		array[1014] = val;
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
		array[1015] = val;
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
		array[1016] = val;
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
		array[1017] = val;
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
		array[1018] = val;
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
		array[1019] = val;
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
		array[1020] = val;
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
		array[1021] = val;
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
		array[1022] = val;
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
		array[1023] = val;
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
		array[1024] = val;
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
		array[1025] = val;
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
		array[1026] = val;
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
		array[1027] = val;
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
		array[1028] = val;
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
		array[1029] = val;
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
		array[1030] = val;
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
		array[1031] = val;
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
		array[1032] = val;
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
		array[1033] = val;
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
		array[1034] = val;
		val = new Command();
		val.Name = "configfile";
		val.Parent = "world";
		val.FullName = "world.configfile";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.World.configFile ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.World.configFile = str;
		};
		array[1035] = val;
		val = new Command();
		val.Name = "configstring";
		val.Parent = "world";
		val.FullName = "world.configstring";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.World.configString ?? "";
		val.SetOveride = delegate(string str)
		{
			ConVar.World.configString = str;
		};
		array[1036] = val;
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
		array[1037] = val;
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
		array[1038] = val;
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
		array[1039] = val;
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
		array[1040] = val;
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
		array[1041] = val;
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
		array[1042] = val;
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
		array[1043] = val;
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
		array[1044] = val;
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
		array[1045] = val;
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
		array[1046] = val;
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
		array[1047] = val;
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
		array[1048] = val;
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
		array[1049] = val;
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
		array[1050] = val;
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
		array[1051] = val;
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
		array[1052] = val;
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
		array[1053] = val;
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
		array[1054] = val;
		val = new Command();
		val.Name = "batteryid";
		val.Parent = "electricbattery";
		val.FullName = "electricbattery.batteryid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ElectricBattery.batteryid(arg);
		};
		array[1055] = val;
		val = new Command();
		val.Name = "killallevents";
		val.Parent = "eventschedule";
		val.FullName = "eventschedule.killallevents";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate
		{
			EventSchedule.KillAllEvents();
		};
		array[1056] = val;
		val = new Command();
		val.Name = "triggerevent";
		val.Parent = "eventschedule";
		val.FullName = "eventschedule.triggerevent";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			EventSchedule.TriggerEvent(arg);
		};
		array[1057] = val;
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
		array[1058] = val;
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
		array[1059] = val;
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
		array[1060] = val;
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
		array[1061] = val;
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
		array[1062] = val;
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
		array[1063] = val;
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
		array[1064] = val;
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
		array[1065] = val;
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
		array[1066] = val;
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
		array[1067] = val;
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
		array[1068] = val;
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
		array[1069] = val;
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
		array[1070] = val;
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
		array[1071] = val;
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
		array[1072] = val;
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
		array[1073] = val;
		val = new Command();
		val.Name = "steamstatus";
		val.Parent = "global";
		val.FullName = "global.steamstatus";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text4 = SteamNetworking.steamstatus();
			arg.ReplyWithObject((object)text4);
		};
		array[1074] = val;
		val = new Command();
		val.Name = "ip";
		val.Parent = "rcon";
		val.FullName = "rcon.ip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RCon.Ip ?? "";
		val.SetOveride = delegate(string str)
		{
			RCon.Ip = str;
		};
		array[1075] = val;
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
		array[1076] = val;
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
		array[1077] = val;
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
		array[1078] = val;
		val = new Command();
		val.Name = "analytics_header";
		val.Parent = "analytics";
		val.FullName = "analytics.analytics_header";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.AnalyticsHeader ?? "";
		val.SetOveride = delegate(string str)
		{
			Analytics.AnalyticsHeader = str;
		};
		array[1079] = val;
		val = new Command();
		val.Name = "analytics_secret";
		val.Parent = "analytics";
		val.FullName = "analytics.analytics_secret";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.AnalyticsSecret ?? "";
		val.SetOveride = delegate(string str)
		{
			Analytics.AnalyticsSecret = str;
		};
		array[1080] = val;
		val = new Command();
		val.Name = "analytics_bulk_upload_url";
		val.Parent = "analytics";
		val.FullName = "analytics.analytics_bulk_upload_url";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.BulkUploadConnectionString ?? "";
		val.SetOveride = delegate(string str)
		{
			Analytics.BulkUploadConnectionString = str;
		};
		array[1081] = val;
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
		array[1082] = val;
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
		array[1083] = val;
		val = new Command();
		val.Name = "server_analytics_url";
		val.Parent = "analytics";
		val.FullName = "analytics.server_analytics_url";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.ServerAnalyticsUrl ?? "";
		val.SetOveride = delegate(string str)
		{
			Analytics.ServerAnalyticsUrl = str;
		};
		array[1084] = val;
		val = new Command();
		val.Name = "stats_blacklist";
		val.Parent = "analytics";
		val.FullName = "analytics.stats_blacklist";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.stats_blacklist ?? "";
		val.SetOveride = delegate(string str)
		{
			Analytics.stats_blacklist = str;
		};
		array[1085] = val;
		val = new Command();
		val.Name = "analytics_enabled";
		val.Parent = "analytics";
		val.FullName = "analytics.analytics_enabled";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Analytics.UploadAnalytics.ToString();
		val.SetOveride = delegate(string str)
		{
			Analytics.UploadAnalytics = StringExtensions.ToBool(str);
		};
		array[1086] = val;
		val = new Command();
		val.Name = "command_lagspike_threshold";
		val.Parent = "profile";
		val.FullName = "profile.command_lagspike_threshold";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RuntimeProfiler.command_lagspike_threshold.ToString();
		val.SetOveride = delegate(string str)
		{
			RuntimeProfiler.command_lagspike_threshold = StringExtensions.ToInt(str, 0);
		};
		array[1087] = val;
		val = new Command();
		val.Name = "dump_profile_recorders";
		val.Parent = "profile";
		val.FullName = "profile.dump_profile_recorders";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RuntimeProfiler.dump_profile_recorders(arg);
		};
		array[1088] = val;
		val = new Command();
		val.Name = "profiling_entities";
		val.Parent = "profile";
		val.FullName = "profile.profiling_entities";
		val.ServerAdmin = true;
		val.Description = "0 = off, 1 = spawn/kill, 2 = spawn/kill per entity, 3 = count every '5 min'";
		val.Variable = true;
		val.GetOveride = () => RuntimeProfiler.profiling_entities.ToString();
		val.SetOveride = delegate(string str)
		{
			RuntimeProfiler.profiling_entities = StringExtensions.ToInt(str, 0);
		};
		array[1089] = val;
		val = new Command();
		val.Name = "profiling_entity_count_interval";
		val.Parent = "profile";
		val.FullName = "profile.profiling_entity_count_interval";
		val.ServerAdmin = true;
		val.Description = "How frequently to count all entities across the server";
		val.Variable = true;
		val.GetOveride = () => RuntimeProfiler.profiling_entity_count_interval.ToString();
		val.SetOveride = delegate(string str)
		{
			RuntimeProfiler.profiling_entity_count_interval = StringExtensions.ToInt(str, 0);
		};
		array[1090] = val;
		val = new Command();
		val.Name = "profiling_fixed_invokes";
		val.Parent = "profile";
		val.FullName = "profile.profiling_fixed_invokes";
		val.ServerAdmin = true;
		val.Description = "0 = off, 1 = stats per frame, 2 = stats per method";
		val.Variable = true;
		val.GetOveride = () => RuntimeProfiler.profiling_fixed_invokes.ToString();
		val.SetOveride = delegate(string str)
		{
			RuntimeProfiler.profiling_fixed_invokes = StringExtensions.ToInt(str, 0);
		};
		array[1091] = val;
		val = new Command();
		val.Name = "profiling_invokes";
		val.Parent = "profile";
		val.FullName = "profile.profiling_invokes";
		val.ServerAdmin = true;
		val.Description = "0 = off, 1 = stats per frame, 2 = stats per method";
		val.Variable = true;
		val.GetOveride = () => RuntimeProfiler.profiling_invokes.ToString();
		val.SetOveride = delegate(string str)
		{
			RuntimeProfiler.profiling_invokes = StringExtensions.ToInt(str, 0);
		};
		array[1092] = val;
		val = new Command();
		val.Name = "profiling_lagspikes";
		val.Parent = "profile";
		val.FullName = "profile.profiling_lagspikes";
		val.ServerAdmin = true;
		val.Description = "Record inbound RPC & ConsoleCommands that cause lag spikes";
		val.Variable = true;
		val.GetOveride = () => RuntimeProfiler.profiling_lagspikes.ToString();
		val.SetOveride = delegate(string str)
		{
			RuntimeProfiler.profiling_lagspikes = StringExtensions.ToBool(str);
		};
		array[1093] = val;
		val = new Command();
		val.Name = "profiling_packets";
		val.Parent = "profile";
		val.FullName = "profile.profiling_packets";
		val.ServerAdmin = true;
		val.Description = "Record type of packets inbound/outbound per frame";
		val.Variable = true;
		val.GetOveride = () => RuntimeProfiler.profiling_packets.ToString();
		val.SetOveride = delegate(string str)
		{
			RuntimeProfiler.profiling_packets = StringExtensions.ToBool(str);
		};
		array[1094] = val;
		val = new Command();
		val.Name = "profiling_work_queue";
		val.Parent = "profile";
		val.FullName = "profile.profiling_work_queue";
		val.ServerAdmin = true;
		val.Description = "Record execution time of ObjectWorkQueues per frame";
		val.Variable = true;
		val.GetOveride = () => RuntimeProfiler.profiling_work_queue.ToString();
		val.SetOveride = delegate(string str)
		{
			RuntimeProfiler.profiling_work_queue = StringExtensions.ToBool(str);
		};
		array[1095] = val;
		val = new Command();
		val.Name = "rpc_lagspike_threshold";
		val.Parent = "profile";
		val.FullName = "profile.rpc_lagspike_threshold";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RuntimeProfiler.rpc_lagspike_threshold.ToString();
		val.SetOveride = delegate(string str)
		{
			RuntimeProfiler.rpc_lagspike_threshold = StringExtensions.ToInt(str, 0);
		};
		array[1096] = val;
		val = new Command();
		val.Name = "runtime_profiling";
		val.Parent = "profile";
		val.FullName = "profile.runtime_profiling";
		val.ServerAdmin = true;
		val.Description = "0 = off, 1 = basic, 2 = everything. This will reset all profiling convars, however they can be modified afterwards";
		val.Variable = true;
		val.GetOveride = () => RuntimeProfiler.runtime_profiling.ToString();
		val.SetOveride = delegate(string str)
		{
			RuntimeProfiler.runtime_profiling = StringExtensions.ToInt(str, 0);
		};
		array[1097] = val;
		val = new Command();
		val.Name = "runtime_profiling_interval";
		val.Parent = "profile";
		val.FullName = "profile.runtime_profiling_interval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RuntimeProfiler.runtime_profiling_interval.ToString();
		val.SetOveride = delegate(string str)
		{
			RuntimeProfiler.runtime_profiling_interval = StringExtensions.ToInt(str, 0);
		};
		array[1098] = val;
		val = new Command();
		val.Name = "displaydistancemultiplier";
		val.Parent = "espcanvas";
		val.FullName = "espcanvas.displaydistancemultiplier";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "Multiply the maximum distance for displaying players nameplates";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => ESPCanvas.DisplayDistanceMultiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			ESPCanvas.DisplayDistanceMultiplier = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "1";
		array[1099] = val;
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
		array[1100] = val;
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
		array[1101] = val;
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
		array[1102] = val;
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
		array[1103] = val;
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
		array[1104] = val;
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
		array[1105] = val;
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
		array[1106] = val;
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
		array[1107] = val;
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
		array[1108] = val;
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
		array[1109] = val;
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
		array[1110] = val;
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
		array[1111] = val;
		val = new Command();
		val.Name = "maxconditionrepairlossonpush";
		val.Parent = "handcuffs";
		val.FullName = "handcuffs.maxconditionrepairlossonpush";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Handcuffs.maxConditionRepairLossOnPush.ToString();
		val.SetOveride = delegate(string str)
		{
			Handcuffs.maxConditionRepairLossOnPush = StringExtensions.ToFloat(str, 0f);
		};
		array[1112] = val;
		val = new Command();
		val.Name = "restrainedpushdamage";
		val.Parent = "handcuffs";
		val.FullName = "handcuffs.restrainedpushdamage";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Handcuffs.restrainedPushDamage.ToString();
		val.SetOveride = delegate(string str)
		{
			Handcuffs.restrainedPushDamage = StringExtensions.ToFloat(str, 0f);
		};
		array[1113] = val;
		val = new Command();
		val.Name = "togglecuffslocked";
		val.Parent = "handcuffs";
		val.FullName = "handcuffs.togglecuffslocked";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Handcuffs.togglecuffslocked(arg);
		};
		array[1114] = val;
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
		array[1115] = val;
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
		array[1116] = val;
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
		array[1117] = val;
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
		array[1118] = val;
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
		array[1119] = val;
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
		array[1120] = val;
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
		array[1121] = val;
		val = new Command();
		val.Name = "debugqueue";
		val.Parent = "ioentity";
		val.FullName = "ioentity.debugqueue";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate
		{
			IOEntity.DebugQueue();
		};
		array[1122] = val;
		val = new Command();
		val.Name = "framebudgetelectrichighpriorityms";
		val.Parent = "ioentity";
		val.FullName = "ioentity.framebudgetelectrichighpriorityms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.frameBudgetElectricHighPriorityMs.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.frameBudgetElectricHighPriorityMs = StringExtensions.ToFloat(str, 0f);
		};
		array[1123] = val;
		val = new Command();
		val.Name = "framebudgetelectriclowpriorityms";
		val.Parent = "ioentity";
		val.FullName = "ioentity.framebudgetelectriclowpriorityms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.frameBudgetElectricLowPriorityMs.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.frameBudgetElectricLowPriorityMs = StringExtensions.ToFloat(str, 0f);
		};
		array[1124] = val;
		val = new Command();
		val.Name = "framebudgetfluidms";
		val.Parent = "ioentity";
		val.FullName = "ioentity.framebudgetfluidms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.frameBudgetFluidMs.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.frameBudgetFluidMs = StringExtensions.ToFloat(str, 0f);
		};
		array[1125] = val;
		val = new Command();
		val.Name = "framebudgetgenericms";
		val.Parent = "ioentity";
		val.FullName = "ioentity.framebudgetgenericms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.frameBudgetGenericMs.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.frameBudgetGenericMs = StringExtensions.ToFloat(str, 0f);
		};
		array[1126] = val;
		val = new Command();
		val.Name = "framebudgetindustrialms";
		val.Parent = "ioentity";
		val.FullName = "ioentity.framebudgetindustrialms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.frameBudgetIndustrialMs.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.frameBudgetIndustrialMs = StringExtensions.ToFloat(str, 0f);
		};
		array[1127] = val;
		val = new Command();
		val.Name = "framebudgetkineticms";
		val.Parent = "ioentity";
		val.FullName = "ioentity.framebudgetkineticms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.frameBudgetKineticMs.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.frameBudgetKineticMs = StringExtensions.ToFloat(str, 0f);
		};
		array[1128] = val;
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
		array[1129] = val;
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
		array[1130] = val;
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
		array[1131] = val;
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
		array[1132] = val;
		val = new Command();
		val.Name = "timeoutduration";
		val.Parent = "metaldetectorflag";
		val.FullName = "metaldetectorflag.timeoutduration";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => MetalDetectorFlag.TimeoutDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			MetalDetectorFlag.TimeoutDuration = StringExtensions.ToFloat(str, 0f);
		};
		array[1133] = val;
		val = new Command();
		val.Name = "attemptspersubsourcespawn";
		val.Parent = "metaldetectorsource";
		val.FullName = "metaldetectorsource.attemptspersubsourcespawn";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => MetalDetectorSource.AttemptsPerSubSourceSpawn.ToString();
		val.SetOveride = delegate(string str)
		{
			MetalDetectorSource.AttemptsPerSubSourceSpawn = StringExtensions.ToInt(str, 0);
		};
		array[1134] = val;
		val = new Command();
		val.Name = "mindistancebetweensubsources";
		val.Parent = "metaldetectorsource";
		val.FullName = "metaldetectorsource.mindistancebetweensubsources";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => MetalDetectorSource.MinDistanceBetweenSubSources.ToString();
		val.SetOveride = delegate(string str)
		{
			MetalDetectorSource.MinDistanceBetweenSubSources = StringExtensions.ToFloat(str, 0f);
		};
		array[1135] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "metaldetectorsource";
		val.FullName = "metaldetectorsource.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => MetalDetectorSource.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			MetalDetectorSource.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[1136] = val;
		val = new Command();
		val.Name = "servercountsources";
		val.Parent = "metaldetectorsource";
		val.FullName = "metaldetectorsource.servercountsources";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate
		{
			MetalDetectorSource.ServerCountSources();
		};
		array[1137] = val;
		val = new Command();
		val.Name = "timeoutduration";
		val.Parent = "metaldetectorsource";
		val.FullName = "metaldetectorsource.timeoutduration";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => MetalDetectorSource.TimeoutDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			MetalDetectorSource.TimeoutDuration = StringExtensions.ToFloat(str, 0f);
		};
		array[1138] = val;
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
		array[1139] = val;
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
		array[1140] = val;
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
		array[1141] = val;
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
		array[1142] = val;
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
		array[1143] = val;
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
		array[1144] = val;
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
		array[1145] = val;
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
		array[1146] = val;
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
		array[1147] = val;
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
		array[1148] = val;
		val = new Command();
		val.Name = "sleeperhostiledelay";
		val.Parent = "npcautoturret";
		val.FullName = "npcautoturret.sleeperhostiledelay";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "How many seconds until a sleeping player is considered hostile";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => NPCAutoTurret.sleeperhostiledelay.ToString();
		val.SetOveride = delegate(string str)
		{
			NPCAutoTurret.sleeperhostiledelay = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "1200";
		array[1149] = val;
		val = new Command();
		val.Name = "dynamicpricingenabled";
		val.Parent = "npcvendingmachine";
		val.FullName = "npcvendingmachine.dynamicpricingenabled";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Whether to run the the dynamic pricing system";
		val.Variable = true;
		val.GetOveride = () => NPCVendingMachine.DynamicPricingEnabled.ToString();
		val.SetOveride = delegate(string str)
		{
			NPCVendingMachine.DynamicPricingEnabled = StringExtensions.ToBool(str);
		};
		array[1150] = val;
		val = new Command();
		val.Name = "intervalhours";
		val.Parent = "npcvendingmachine";
		val.FullName = "npcvendingmachine.intervalhours";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How many in game hours are checked when looking for price increases. Max 72 (3 days)";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => NPCVendingMachine.IntervalHours.ToString();
		val.SetOveride = delegate(string str)
		{
			NPCVendingMachine.IntervalHours = StringExtensions.ToInt(str, 0);
		};
		array[1151] = val;
		val = new Command();
		val.Name = "maximumpricemultiplier";
		val.Parent = "npcvendingmachine";
		val.FullName = "npcvendingmachine.maximumpricemultiplier";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "The maximum point that a price can increase to (2 = 200%)";
		val.Variable = true;
		val.GetOveride = () => NPCVendingMachine.MaximumPriceMultiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			NPCVendingMachine.MaximumPriceMultiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[1152] = val;
		val = new Command();
		val.Name = "minimumpricemultiplier";
		val.Parent = "npcvendingmachine";
		val.FullName = "npcvendingmachine.minimumpricemultiplier";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "The Minimum point that the price can drop to (0.5 = 50% off)";
		val.Variable = true;
		val.GetOveride = () => NPCVendingMachine.MinimumPriceMultiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			NPCVendingMachine.MinimumPriceMultiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[1153] = val;
		val = new Command();
		val.Name = "pricedecreaseamount";
		val.Parent = "npcvendingmachine";
		val.FullName = "npcvendingmachine.pricedecreaseamount";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How much to decrease the price for if it is underselling (0.05 = 5%)";
		val.Variable = true;
		val.GetOveride = () => NPCVendingMachine.PriceDecreaseAmount.ToString();
		val.SetOveride = delegate(string str)
		{
			NPCVendingMachine.PriceDecreaseAmount = StringExtensions.ToFloat(str, 0f);
		};
		array[1154] = val;
		val = new Command();
		val.Name = "priceincreaseamount";
		val.Parent = "npcvendingmachine";
		val.FullName = "npcvendingmachine.priceincreaseamount";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How much to increase the price by if it is selling a lot (0.05 = 5%)";
		val.Variable = true;
		val.GetOveride = () => NPCVendingMachine.PriceIncreaseAmount.ToString();
		val.SetOveride = delegate(string str)
		{
			NPCVendingMachine.PriceIncreaseAmount = StringExtensions.ToFloat(str, 0f);
		};
		array[1155] = val;
		val = new Command();
		val.Name = "printallpricechanges";
		val.Parent = "npcvendingmachine";
		val.FullName = "npcvendingmachine.printallpricechanges";
		val.ServerAdmin = true;
		val.Description = "Print out all current price changes on the server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			NPCVendingMachine.printAllPriceChanges(arg);
		};
		array[1156] = val;
		val = new Command();
		val.Name = "resetdynamicpricing";
		val.Parent = "npcvendingmachine";
		val.FullName = "npcvendingmachine.resetdynamicpricing";
		val.ServerAdmin = true;
		val.Description = "Resets the state of all discounts and surcharges from NPC vending machines";
		val.Variable = false;
		val.Call = delegate
		{
			NPCVendingMachine.resetDynamicPricing();
		};
		array[1157] = val;
		val = new Command();
		val.Name = "startingpricemultiplier";
		val.Parent = "npcvendingmachine";
		val.FullName = "npcvendingmachine.startingpricemultiplier";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "What discount surcharge should be applied to items when the server starts";
		val.Variable = true;
		val.GetOveride = () => NPCVendingMachine.StartingPriceMultiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			NPCVendingMachine.StartingPriceMultiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[1158] = val;
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
		array[1159] = val;
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
		array[1160] = val;
		val = new Command();
		val.Name = "flee_damage_percentage";
		val.Parent = "patrolhelicopterai";
		val.FullName = "patrolhelicopterai.flee_damage_percentage";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PatrolHelicopterAI.flee_damage_percentage.ToString();
		val.SetOveride = delegate(string str)
		{
			PatrolHelicopterAI.flee_damage_percentage = StringExtensions.ToFloat(str, 0f);
		};
		array[1161] = val;
		val = new Command();
		val.Name = "monument_crash";
		val.Parent = "patrolhelicopterai";
		val.FullName = "patrolhelicopterai.monument_crash";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PatrolHelicopterAI.monument_crash.ToString();
		val.SetOveride = delegate(string str)
		{
			PatrolHelicopterAI.monument_crash = StringExtensions.ToBool(str);
		};
		array[1162] = val;
		val = new Command();
		val.Name = "use_danger_zones";
		val.Parent = "patrolhelicopterai";
		val.FullName = "patrolhelicopterai.use_danger_zones";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PatrolHelicopterAI.use_danger_zones.ToString();
		val.SetOveride = delegate(string str)
		{
			PatrolHelicopterAI.use_danger_zones = StringExtensions.ToBool(str);
		};
		array[1163] = val;
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
		array[1164] = val;
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
		array[1165] = val;
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
		array[1166] = val;
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
		array[1167] = val;
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
		array[1168] = val;
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
		array[1169] = val;
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
		array[1170] = val;
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
		array[1171] = val;
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
		array[1172] = val;
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
		array[1173] = val;
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
		array[1174] = val;
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
		array[1175] = val;
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
		array[1176] = val;
		val = new Command();
		val.Name = "createandaddtoteam";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.createandaddtoteam";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text3 = RelationshipManager.createAndAddToTeam(arg);
			arg.ReplyWithObject((object)text3);
		};
		array[1177] = val;
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
		array[1178] = val;
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
		array[1179] = val;
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
		array[1180] = val;
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
		array[1181] = val;
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
		array[1182] = val;
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
		array[1183] = val;
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
		array[1184] = val;
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
		array[1185] = val;
		val = new Command();
		val.Name = "promote_id";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.promote_id";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.promote_id(arg);
		};
		array[1186] = val;
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
		array[1187] = val;
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
		array[1188] = val;
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
		array[1189] = val;
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
		array[1190] = val;
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
		array[1191] = val;
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
		array[1192] = val;
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
		array[1193] = val;
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
		array[1194] = val;
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
		array[1195] = val;
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
		array[1196] = val;
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
		array[1197] = val;
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
		array[1198] = val;
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
		array[1199] = val;
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
		array[1200] = val;
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
		array[1201] = val;
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
		array[1202] = val;
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
		array[1203] = val;
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
		array[1204] = val;
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
		array[1205] = val;
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
		array[1206] = val;
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
		array[1207] = val;
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
		array[1208] = val;
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
		array[1209] = val;
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
		array[1210] = val;
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
		array[1211] = val;
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
		array[1212] = val;
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
		array[1213] = val;
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
		array[1214] = val;
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
		array[1215] = val;
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
		array[1216] = val;
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
		array[1217] = val;
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
		array[1218] = val;
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
		array[1219] = val;
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
		array[1220] = val;
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
		array[1221] = val;
		val = new Command();
		val.Name = "demolish_seconds";
		val.Parent = "stabilityentity";
		val.FullName = "stabilityentity.demolish_seconds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => StabilityEntity.demolish_seconds.ToString();
		val.SetOveride = delegate(string str)
		{
			StabilityEntity.demolish_seconds = StringExtensions.ToInt(str, 0);
		};
		array[1222] = val;
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
		array[1223] = val;
		val = new Command();
		val.Name = "reveal_tick_rate";
		val.Parent = "stash";
		val.FullName = "stash.reveal_tick_rate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => StashContainer.PlayerDetectionTickRate.ToString();
		val.SetOveride = delegate(string str)
		{
			StashContainer.PlayerDetectionTickRate = StringExtensions.ToFloat(str, 0f);
		};
		array[1224] = val;
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
		array[1225] = val;
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
		array[1226] = val;
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
		array[1227] = val;
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
		array[1228] = val;
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
		array[1229] = val;
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
		array[1230] = val;
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
		array[1231] = val;
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
		array[1232] = val;
		val = new Command();
		val.Name = "alive_time_seconds";
		val.Parent = "travellingvendor";
		val.FullName = "travellingvendor.alive_time_seconds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => TravellingVendor.alive_time_seconds.ToString();
		val.SetOveride = delegate(string str)
		{
			TravellingVendor.alive_time_seconds = StringExtensions.ToFloat(str, 0f);
		};
		array[1233] = val;
		val = new Command();
		val.Name = "attempt_pullovers";
		val.Parent = "travellingvendor";
		val.FullName = "travellingvendor.attempt_pullovers";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => TravellingVendor.attempt_pullovers.ToString();
		val.SetOveride = delegate(string str)
		{
			TravellingVendor.attempt_pullovers = StringExtensions.ToBool(str);
		};
		array[1234] = val;
		val = new Command();
		val.Name = "should_destroy_buildings";
		val.Parent = "travellingvendor";
		val.FullName = "travellingvendor.should_destroy_buildings";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => TravellingVendor.should_destroy_buildings.ToString();
		val.SetOveride = delegate(string str)
		{
			TravellingVendor.should_destroy_buildings = StringExtensions.ToBool(str);
		};
		array[1235] = val;
		val = new Command();
		val.Name = "should_spawn";
		val.Parent = "travellingvendor";
		val.FullName = "travellingvendor.should_spawn";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => TravellingVendor.should_spawn.ToString();
		val.SetOveride = delegate(string str)
		{
			TravellingVendor.should_spawn = StringExtensions.ToBool(str);
		};
		array[1236] = val;
		val = new Command();
		val.Name = "spawn";
		val.Parent = "travellingvendor";
		val.FullName = "travellingvendor.spawn";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text2 = TravellingVendor.svspawntravellingvendor(arg);
			arg.ReplyWithObject((object)text2);
		};
		array[1237] = val;
		val = new Command();
		val.Name = "startevent";
		val.Parent = "travellingvendor";
		val.FullName = "travellingvendor.startevent";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text = TravellingVendor.svspawntravellingvendorevent(arg);
			arg.ReplyWithObject((object)text);
		};
		array[1238] = val;
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
		array[1239] = val;
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
		array[1240] = val;
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
		array[1241] = val;
		val = new Command();
		val.Name = "enforcetrespasschecks";
		val.Parent = "tutorialisland";
		val.FullName = "tutorialisland.enforcetrespasschecks";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => TutorialIsland.EnforceTrespassChecks.ToString();
		val.SetOveride = delegate(string str)
		{
			TutorialIsland.EnforceTrespassChecks = StringExtensions.ToBool(str);
		};
		array[1242] = val;
		val = new Command();
		val.Name = "overridetutoriallocation";
		val.Parent = "tutorialisland";
		val.FullName = "tutorialisland.overridetutoriallocation";
		val.ServerAdmin = true;
		val.Description = "Will place the tutorial as close as possible to this pos, only for debugging";
		val.Variable = true;
		val.GetOveride = () => ((object)(Vector3)(ref TutorialIsland.OverrideTutorialLocation)).ToString();
		val.SetOveride = delegate(string str)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			TutorialIsland.OverrideTutorialLocation = StringExtensions.ToVector3(str);
		};
		array[1243] = val;
		val = new Command();
		val.Name = "spawntutorialislandfornewplayer";
		val.Parent = "tutorialisland";
		val.FullName = "tutorialisland.spawntutorialislandfornewplayer";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => TutorialIsland.SpawnTutorialIslandForNewPlayer.ToString();
		val.SetOveride = delegate(string str)
		{
			TutorialIsland.SpawnTutorialIslandForNewPlayer = StringExtensions.ToBool(str);
		};
		array[1244] = val;
		val = new Command();
		val.Name = "racetimeout";
		val.Parent = "waypointrace";
		val.FullName = "waypointrace.racetimeout";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "How long a race can go until it times out (in seconds)";
		val.Variable = true;
		val.GetOveride = () => WaypointRace.raceTimeout.ToString();
		val.SetOveride = delegate(string str)
		{
			WaypointRace.raceTimeout = StringExtensions.ToFloat(str, 0f);
		};
		array[1245] = val;
		val = new Command();
		val.Name = "startrace";
		val.Parent = "waypointrace";
		val.FullName = "waypointrace.startrace";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			WaypointRace.startRace(arg);
		};
		array[1246] = val;
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
		array[1247] = val;
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
		array[1248] = val;
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
		array[1249] = val;
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
		array[1250] = val;
		val = new Command();
		val.Name = "wipecronoverride";
		val.Parent = "wipetimer";
		val.FullName = "wipetimer.wipecronoverride";
		val.ServerAdmin = true;
		val.Description = "Custom cron expression for the wipe schedule. Overrides all other convars (except wipeUnixTimestampOverride) if set. Uses Cronos as a parser: https://github.com/HangfireIO/Cronos/";
		val.Variable = true;
		val.GetOveride = () => WipeTimer.wipeCronOverride ?? "";
		val.SetOveride = delegate(string str)
		{
			WipeTimer.wipeCronOverride = str;
		};
		array[1251] = val;
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
		array[1252] = val;
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
		array[1253] = val;
		val = new Command();
		val.Name = "wipetimezone";
		val.Parent = "wipetimer";
		val.FullName = "wipetimer.wipetimezone";
		val.ServerAdmin = true;
		val.Description = "The timezone to use for wipes. Defaults to the server's time zone if not set or invalid. Value should be a TZ identifier as seen here: https://en.wikipedia.org/wiki/List_of_tz_database_time_zones";
		val.Variable = true;
		val.GetOveride = () => WipeTimer.wipeTimezone ?? "";
		val.SetOveride = delegate(string str)
		{
			WipeTimer.wipeTimezone = str;
		};
		array[1254] = val;
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
		array[1255] = val;
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
		array[1256] = val;
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
		array[1257] = val;
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
		array[1258] = val;
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
		array[1259] = val;
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
		array[1260] = val;
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
		array[1261] = val;
		All = (Command[])(object)array;
	}
}
