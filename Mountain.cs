using UnityEngine;

public class Mountain : TerrainPlacement
{
	public float Fade = 10f;

	protected void OnDrawGizmosSelected()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.up * (0.5f * Fade);
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawCube(((Component)this).transform.position + val, new Vector3(size.x, Fade, size.z));
		Gizmos.DrawWireCube(((Component)this).transform.position + val, new Vector3(size.x, Fade, size.z));
	}

	protected override void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(heightmap.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, 0f - extents.z));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, 0f - extents.z));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, extents.z));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, extents.z));
		TerrainMeta.HeightMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			float normZ = TerrainMeta.HeightMap.Coordinate(z);
			float normX = TerrainMeta.HeightMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			float num = position.y + offset.y + heightdata.GetInterpolatedHalf((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z) * size.y;
			float num2 = Mathf.InverseLerp(position.y, position.y + Fade, num);
			if (num2 != 0f)
			{
				float num3 = TerrainMeta.NormalizeY(num);
				float height = TerrainMeta.HeightMap.GetHeight01(x, z);
				num3 = Mathx.SmoothMax(height, num3, 0.1f);
				TerrainMeta.HeightMap.SetHeight(x, z, num3, num2);
			}
		});
	}

	protected override void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		bool should0 = ShouldSplat(1);
		bool should1 = ShouldSplat(2);
		bool should2 = ShouldSplat(4);
		bool should3 = ShouldSplat(8);
		bool should4 = ShouldSplat(16);
		bool should5 = ShouldSplat(32);
		bool should6 = ShouldSplat(64);
		bool should7 = ShouldSplat(128);
		if (!should0 && !should1 && !should2 && !should3 && !should4 && !should5 && !should6 && !should7)
		{
			return;
		}
		Vector3 position = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(heightmap.Get());
		TextureData splat0data = new TextureData(splatmap0.Get());
		TextureData splat1data = new TextureData(splatmap1.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, 0f - extents.z));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, 0f - extents.z));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, extents.z));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, extents.z));
		TerrainMeta.SplatMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
			float normZ = TerrainMeta.SplatMap.Coordinate(z);
			float normX = TerrainMeta.SplatMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			float num = position.y + offset.y + heightdata.GetInterpolatedHalf((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z) * size.y;
			float num2 = Mathf.InverseLerp(position.y, position.y + Fade, num);
			if (num2 != 0f)
			{
				Vector4 interpolatedVector = splat0data.GetInterpolatedVector((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z);
				Vector4 interpolatedVector2 = splat1data.GetInterpolatedVector((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z);
				if (!should0)
				{
					interpolatedVector.x = 0f;
				}
				if (!should1)
				{
					interpolatedVector.y = 0f;
				}
				if (!should2)
				{
					interpolatedVector.z = 0f;
				}
				if (!should3)
				{
					interpolatedVector.w = 0f;
				}
				if (!should4)
				{
					interpolatedVector2.x = 0f;
				}
				if (!should5)
				{
					interpolatedVector2.y = 0f;
				}
				if (!should6)
				{
					interpolatedVector2.z = 0f;
				}
				if (!should7)
				{
					interpolatedVector2.w = 0f;
				}
				TerrainMeta.SplatMap.SetSplatRaw(x, z, interpolatedVector, interpolatedVector2, num2);
			}
		});
	}

	protected override void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}

	protected override void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		bool should0 = ShouldBiome(1);
		bool should1 = ShouldBiome(2);
		bool should2 = ShouldBiome(4);
		bool should3 = ShouldBiome(8);
		if (!should0 && !should1 && !should2 && !should3)
		{
			return;
		}
		Vector3 position = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(heightmap.Get());
		TextureData biomedata = new TextureData(biomemap.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, 0f - extents.z));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, 0f - extents.z));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, extents.z));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, extents.z));
		TerrainMeta.BiomeMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0146: Unknown result type (might be due to invalid IL or missing references)
			//IL_016e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0173: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			float normZ = TerrainMeta.BiomeMap.Coordinate(z);
			float normX = TerrainMeta.BiomeMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			float num = position.y + offset.y + heightdata.GetInterpolatedHalf((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z) * size.y;
			float num2 = Mathf.InverseLerp(position.y, position.y + Fade, num);
			if (num2 != 0f)
			{
				Vector4 interpolatedVector = biomedata.GetInterpolatedVector((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z);
				if (!should0)
				{
					interpolatedVector.x = 0f;
				}
				if (!should1)
				{
					interpolatedVector.y = 0f;
				}
				if (!should2)
				{
					interpolatedVector.z = 0f;
				}
				if (!should3)
				{
					interpolatedVector.w = 0f;
				}
				TerrainMeta.BiomeMap.SetBiomeRaw(x, z, interpolatedVector, num2);
			}
		});
	}

	protected override void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		TextureData topologydata = new TextureData(topologymap.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, 0f - extents.z));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, 0f - extents.z));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - extents.x, 0f, extents.z));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(extents.x, 0f, extents.z));
		TerrainMeta.TopologyMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Expected I4, but got Unknown
			GenerateCliffTopology.Process(x, z);
			float normZ = TerrainMeta.TopologyMap.Coordinate(z);
			float normX = TerrainMeta.TopologyMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			int interpolatedInt = topologydata.GetInterpolatedInt((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z);
			if (ShouldTopology(interpolatedInt))
			{
				TerrainMeta.TopologyMap.AddTopology(x, z, interpolatedInt & TopologyMask);
			}
		});
	}

	protected override void ApplyWater(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
	}
}
