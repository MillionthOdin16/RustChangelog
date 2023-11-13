using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class TerrainMap : TerrainExtension
{
	internal int res;

	public void ApplyFilter(float normX, float normZ, float radius, float fade, Action<int, int, float> action)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		float num = TerrainMeta.OneOverSize.x * radius;
		float num2 = TerrainMeta.OneOverSize.x * fade;
		float num3 = (float)res * (num - num2);
		float num4 = (float)res * num;
		float num5 = normX * (float)res;
		float num6 = normZ * (float)res;
		int num7 = Index(normX - num);
		int num8 = Index(normX + num);
		int num9 = Index(normZ - num);
		int num10 = Index(normZ + num);
		Vector2 val;
		if (num3 != num4)
		{
			for (int i = num9; i <= num10; i++)
			{
				for (int j = num7; j <= num8; j++)
				{
					val = new Vector2((float)j + 0.5f - num5, (float)i + 0.5f - num6);
					float magnitude = ((Vector2)(ref val)).magnitude;
					float arg = Mathf.InverseLerp(num4, num3, magnitude);
					action(j, i, arg);
				}
			}
			return;
		}
		for (int k = num9; k <= num10; k++)
		{
			for (int l = num7; l <= num8; l++)
			{
				val = new Vector2((float)l + 0.5f - num5, (float)k + 0.5f - num6);
				float magnitude2 = ((Vector2)(ref val)).magnitude;
				float arg2 = ((magnitude2 < num4) ? 1 : 0);
				action(l, k, arg2);
			}
		}
	}

	public void ForEach(Vector3 worldPos, float radius, Action<int, int> action)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		int num = Index(TerrainMeta.NormalizeX(worldPos.x - radius));
		int num2 = Index(TerrainMeta.NormalizeX(worldPos.x + radius));
		int num3 = Index(TerrainMeta.NormalizeZ(worldPos.z - radius));
		int num4 = Index(TerrainMeta.NormalizeZ(worldPos.z + radius));
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				action(j, i);
			}
		}
	}

	public void ForEach(float normX, float normZ, float normRadius, Action<int, int> action)
	{
		int num = Index(normX - normRadius);
		int num2 = Index(normX + normRadius);
		int num3 = Index(normZ - normRadius);
		int num4 = Index(normZ + normRadius);
		for (int i = num3; i <= num4; i++)
		{
			for (int j = num; j <= num2; j++)
			{
				action(j, i);
			}
		}
	}

	public void ForEachParallel(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Vector2i v3 = default(Vector2i);
		((Vector2i)(ref v3))._002Ector(Index(TerrainMeta.NormalizeX(v0.x)), Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v4 = default(Vector2i);
		((Vector2i)(ref v4))._002Ector(Index(TerrainMeta.NormalizeX(v1.x)), Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v5 = default(Vector2i);
		((Vector2i)(ref v5))._002Ector(Index(TerrainMeta.NormalizeX(v2.x)), Index(TerrainMeta.NormalizeZ(v2.z)));
		ForEachParallel(v3, v4, v5, action);
	}

	public void ForEachParallel(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathx.Min(v0.x, v1.x, v2.x);
		int num2 = Mathx.Max(v0.x, v1.x, v2.x);
		int num3 = Mathx.Min(v0.y, v1.y, v2.y);
		int num4 = Mathx.Max(v0.y, v1.y, v2.y);
		Vector2i base_min = new Vector2i(num, num3);
		Vector2i val = default(Vector2i);
		((Vector2i)(ref val))._002Ector(num2, num4);
		Vector2i base_count = val - base_min + Vector2i.one;
		Parallel.Call((Action<int, int>)delegate(int thread_id, int thread_count)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			Vector2i min = base_min + base_count * thread_id / thread_count;
			Vector2i max = base_min + base_count * (thread_id + 1) / thread_count - Vector2i.one;
			ForEachInternal(v0, v1, v2, action, min, max);
		});
	}

	public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Action<int, int> action)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Vector2i v3 = default(Vector2i);
		((Vector2i)(ref v3))._002Ector(Index(TerrainMeta.NormalizeX(v0.x)), Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v4 = default(Vector2i);
		((Vector2i)(ref v4))._002Ector(Index(TerrainMeta.NormalizeX(v1.x)), Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v5 = default(Vector2i);
		((Vector2i)(ref v5))._002Ector(Index(TerrainMeta.NormalizeX(v2.x)), Index(TerrainMeta.NormalizeZ(v2.z)));
		ForEach(v3, v4, v5, action);
	}

	public void ForEach(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Vector2i min = default(Vector2i);
		((Vector2i)(ref min))._002Ector(int.MinValue, int.MinValue);
		Vector2i max = default(Vector2i);
		((Vector2i)(ref max))._002Ector(int.MaxValue, int.MaxValue);
		ForEachInternal(v0, v1, v2, action, min, max);
	}

	private void ForEachInternal(Vector2i v0, Vector2i v1, Vector2i v2, Action<int, int> action, Vector2i min, Vector2i max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.Max(min.x, Mathx.Min(v0.x, v1.x, v2.x));
		int num2 = Mathf.Min(max.x, Mathx.Max(v0.x, v1.x, v2.x));
		int num3 = Mathf.Max(min.y, Mathx.Min(v0.y, v1.y, v2.y));
		int num4 = Mathf.Min(max.y, Mathx.Max(v0.y, v1.y, v2.y));
		int num5 = v0.y - v1.y;
		int num6 = v1.x - v0.x;
		int num7 = v1.y - v2.y;
		int num8 = v2.x - v1.x;
		int num9 = v2.y - v0.y;
		int num10 = v0.x - v2.x;
		Vector2i val = default(Vector2i);
		((Vector2i)(ref val))._002Ector(num, num3);
		int num11 = (v2.x - v1.x) * (val.y - v1.y) - (v2.y - v1.y) * (val.x - v1.x);
		int num12 = (v0.x - v2.x) * (val.y - v2.y) - (v0.y - v2.y) * (val.x - v2.x);
		int num13 = (v1.x - v0.x) * (val.y - v0.y) - (v1.y - v0.y) * (val.x - v0.x);
		val.y = num3;
		while (val.y <= num4)
		{
			int num14 = num11;
			int num15 = num12;
			int num16 = num13;
			val.x = num;
			while (val.x <= num2)
			{
				if ((num14 | num15 | num16) >= 0)
				{
					action(val.x, val.y);
				}
				num14 += num7;
				num15 += num9;
				num16 += num5;
				val.x++;
			}
			num11 += num8;
			num12 += num10;
			num13 += num6;
			val.y++;
		}
	}

	public void ForEachParallel(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Action<int, int> action)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		Vector2i v4 = default(Vector2i);
		((Vector2i)(ref v4))._002Ector(Index(TerrainMeta.NormalizeX(v0.x)), Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v5 = default(Vector2i);
		((Vector2i)(ref v5))._002Ector(Index(TerrainMeta.NormalizeX(v1.x)), Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v6 = default(Vector2i);
		((Vector2i)(ref v6))._002Ector(Index(TerrainMeta.NormalizeX(v2.x)), Index(TerrainMeta.NormalizeZ(v2.z)));
		Vector2i v7 = default(Vector2i);
		((Vector2i)(ref v7))._002Ector(Index(TerrainMeta.NormalizeX(v3.x)), Index(TerrainMeta.NormalizeZ(v3.z)));
		ForEachParallel(v4, v5, v6, v7, action);
	}

	public void ForEachParallel(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathx.Min(v0.x, v1.x, v2.x, v3.x);
		int num2 = Mathx.Max(v0.x, v1.x, v2.x, v3.x);
		int num3 = Mathx.Min(v0.y, v1.y, v2.y, v3.y);
		int num4 = Mathx.Max(v0.y, v1.y, v2.y, v3.y);
		Vector2i base_min = new Vector2i(num, num3);
		Vector2i val = default(Vector2i);
		((Vector2i)(ref val))._002Ector(num2, num4);
		Vector2i val2 = val - base_min + Vector2i.one;
		Vector2i size_x = new Vector2i(val2.x, 0);
		Vector2i size_y = new Vector2i(0, val2.y);
		Parallel.Call((Action<int, int>)delegate(int thread_id, int thread_count)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			Vector2i min = base_min + size_y * thread_id / thread_count;
			Vector2i max = base_min + size_y * (thread_id + 1) / thread_count + size_x - Vector2i.one;
			ForEachInternal(v0, v1, v2, v3, action, min, max);
		});
	}

	public void ForEach(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Action<int, int> action)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		Vector2i v4 = default(Vector2i);
		((Vector2i)(ref v4))._002Ector(Index(TerrainMeta.NormalizeX(v0.x)), Index(TerrainMeta.NormalizeZ(v0.z)));
		Vector2i v5 = default(Vector2i);
		((Vector2i)(ref v5))._002Ector(Index(TerrainMeta.NormalizeX(v1.x)), Index(TerrainMeta.NormalizeZ(v1.z)));
		Vector2i v6 = default(Vector2i);
		((Vector2i)(ref v6))._002Ector(Index(TerrainMeta.NormalizeX(v2.x)), Index(TerrainMeta.NormalizeZ(v2.z)));
		Vector2i v7 = default(Vector2i);
		((Vector2i)(ref v7))._002Ector(Index(TerrainMeta.NormalizeX(v3.x)), Index(TerrainMeta.NormalizeZ(v3.z)));
		ForEach(v4, v5, v6, v7, action);
	}

	public void ForEach(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Vector2i min = default(Vector2i);
		((Vector2i)(ref min))._002Ector(int.MinValue, int.MinValue);
		Vector2i max = default(Vector2i);
		((Vector2i)(ref max))._002Ector(int.MaxValue, int.MaxValue);
		ForEachInternal(v0, v1, v2, v3, action, min, max);
	}

	private void ForEachInternal(Vector2i v0, Vector2i v1, Vector2i v2, Vector2i v3, Action<int, int> action, Vector2i min, Vector2i max)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.Max(min.x, Mathx.Min(v0.x, v1.x, v2.x, v3.x));
		int num2 = Mathf.Min(max.x, Mathx.Max(v0.x, v1.x, v2.x, v3.x));
		int num3 = Mathf.Max(min.y, Mathx.Min(v0.y, v1.y, v2.y, v3.y));
		int num4 = Mathf.Min(max.y, Mathx.Max(v0.y, v1.y, v2.y, v3.y));
		int num5 = v0.y - v1.y;
		int num6 = v1.x - v0.x;
		int num7 = v1.y - v2.y;
		int num8 = v2.x - v1.x;
		int num9 = v2.y - v0.y;
		int num10 = v0.x - v2.x;
		int num11 = v3.y - v2.y;
		int num12 = v2.x - v3.x;
		int num13 = v2.y - v1.y;
		int num14 = v1.x - v2.x;
		int num15 = v1.y - v3.y;
		int num16 = v3.x - v1.x;
		Vector2i val = default(Vector2i);
		((Vector2i)(ref val))._002Ector(num, num3);
		int num17 = (v2.x - v1.x) * (val.y - v1.y) - (v2.y - v1.y) * (val.x - v1.x);
		int num18 = (v0.x - v2.x) * (val.y - v2.y) - (v0.y - v2.y) * (val.x - v2.x);
		int num19 = (v1.x - v0.x) * (val.y - v0.y) - (v1.y - v0.y) * (val.x - v0.x);
		int num20 = (v1.x - v2.x) * (val.y - v2.y) - (v1.y - v2.y) * (val.x - v2.x);
		int num21 = (v3.x - v1.x) * (val.y - v1.y) - (v3.y - v1.y) * (val.x - v1.x);
		int num22 = (v2.x - v3.x) * (val.y - v3.y) - (v2.y - v3.y) * (val.x - v3.x);
		val.y = num3;
		while (val.y <= num4)
		{
			int num23 = num17;
			int num24 = num18;
			int num25 = num19;
			int num26 = num20;
			int num27 = num21;
			int num28 = num22;
			val.x = num;
			while (val.x <= num2)
			{
				if ((num23 | num24 | num25) >= 0 || (num26 | num27 | num28) >= 0)
				{
					action(val.x, val.y);
				}
				num23 += num7;
				num24 += num9;
				num25 += num5;
				num26 += num13;
				num27 += num15;
				num28 += num11;
				val.x++;
			}
			num17 += num8;
			num18 += num10;
			num19 += num6;
			num20 += num14;
			num21 += num16;
			num22 += num12;
			val.y++;
		}
	}

	public void ForEach(int x_min, int x_max, int z_min, int z_max, Action<int, int> action)
	{
		for (int i = z_min; i <= z_max; i++)
		{
			for (int j = x_min; j <= x_max; j++)
			{
				action(j, i);
			}
		}
	}

	public void ForEach(Action<int, int> action)
	{
		for (int i = 0; i < res; i++)
		{
			for (int j = 0; j < res; j++)
			{
				action(j, i);
			}
		}
	}

	public int Index(float normalized)
	{
		int num = (int)(normalized * (float)res);
		return (num >= 0) ? ((num > res - 1) ? (res - 1) : num) : 0;
	}

	public float Coordinate(int index)
	{
		return ((float)index + 0.5f) / (float)res;
	}
}
public abstract class TerrainMap<T> : TerrainMap where T : struct
{
	internal T[] src;

	internal T[] dst;

	public void Push()
	{
		if (src == dst)
		{
			dst = (T[])src.Clone();
		}
	}

	public void Pop()
	{
		if (src != dst)
		{
			Array.Copy(dst, src, src.Length);
			dst = src;
		}
	}

	public IEnumerable<T> ToEnumerable()
	{
		return src.Cast<T>();
	}

	public int BytesPerElement()
	{
		return Marshal.SizeOf(typeof(T));
	}

	public long GetMemoryUsage()
	{
		return (long)BytesPerElement() * (long)src.Length;
	}

	public byte[] ToByteArray()
	{
		byte[] array = new byte[BytesPerElement() * src.Length];
		Buffer.BlockCopy(src, 0, array, 0, array.Length);
		return array;
	}

	public void FromByteArray(byte[] dat)
	{
		Buffer.BlockCopy(dat, 0, dst, 0, dat.Length);
	}
}
