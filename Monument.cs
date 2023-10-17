using UnityEngine;

public class Monument : TerrainPlacement
{
	public float Radius = 0f;

	public float Fade = 10f;

	protected void OnDrawGizmosSelected()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (Radius == 0f)
		{
			Radius = extents.x;
		}
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		GizmosUtil.DrawWireCircleY(((Component)this).transform.position, Radius);
		GizmosUtil.DrawWireCircleY(((Component)this).transform.position, Radius - Fade);
	}

	protected override void ApplyHeight(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if (Radius == 0f)
		{
			Radius = extents.x;
		}
		bool useBlendMap = blendmap.isValid;
		Vector3 position = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(Vector3.zero);
		TextureData heightdata = new TextureData(heightmap.Get());
		TextureData blenddata = new TextureData(useBlendMap ? blendmap.Get() : null);
		float num = (useBlendMap ? extents.x : Radius);
		float num2 = (useBlendMap ? extents.z : Radius);
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - num, 0f, 0f - num2));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(num, 0f, 0f - num2));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - num, 0f, num2));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(num, 0f, num2));
		TerrainMeta.HeightMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			float normZ = TerrainMeta.HeightMap.Coordinate(z);
			float normX = TerrainMeta.HeightMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			float num3 = 1f;
			num3 = ((!useBlendMap) ? Mathf.InverseLerp(Radius, Radius - Fade, Vector3Ex.Magnitude2D(val2)) : blenddata.GetInterpolatedVector((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z).w);
			if (num3 != 0f)
			{
				float y = position.y + offset.y + heightdata.GetInterpolatedHalf((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z) * size.y;
				float num4 = TerrainMeta.NormalizeY(y);
				float height = TerrainMeta.HeightMap.GetHeight01(x, z);
				num4 = Mathf.SmoothStep(height, num4, num3);
				TerrainMeta.HeightMap.SetHeight(x, z, num4);
			}
		});
	}

	protected override void ApplySplat(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		if (Radius == 0f)
		{
			Radius = extents.x;
		}
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
		TextureData splat0data = new TextureData(splatmap0.Get());
		TextureData splat1data = new TextureData(splatmap1.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - Radius, 0f, 0f - Radius));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(Radius, 0f, 0f - Radius));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - Radius, 0f, Radius));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(Radius, 0f, Radius));
		TerrainMeta.SplatMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0151: Unknown result type (might be due to invalid IL or missing references)
			//IL_0232: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			GenerateCliffSplat.Process(x, z);
			float normZ = TerrainMeta.SplatMap.Coordinate(z);
			float normX = TerrainMeta.SplatMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			float num = Mathf.InverseLerp(Radius, Radius - Fade, Vector3Ex.Magnitude2D(val2));
			if (num != 0f)
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
				TerrainMeta.SplatMap.SetSplatRaw(x, z, interpolatedVector, interpolatedVector2, num);
			}
		});
	}

	protected override void ApplyAlpha(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if (Radius == 0f)
		{
			Radius = extents.x;
		}
		TextureData alphadata = new TextureData(alphamap.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - Radius, 0f, 0f - Radius));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(Radius, 0f, 0f - Radius));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - Radius, 0f, Radius));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(Radius, 0f, Radius));
		TerrainMeta.AlphaMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			float normZ = TerrainMeta.AlphaMap.Coordinate(z);
			float normX = TerrainMeta.AlphaMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			float num = Mathf.InverseLerp(Radius, Radius - Fade, Vector3Ex.Magnitude2D(val2));
			if (num != 0f)
			{
				float w = alphadata.GetInterpolatedVector((val2.x + extents.x) / size.x, (val2.z + extents.z) / size.z).w;
				TerrainMeta.AlphaMap.SetAlpha(x, z, w, num);
			}
		});
	}

	protected override void ApplyBiome(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		if (Radius == 0f)
		{
			Radius = extents.x;
		}
		bool should0 = ShouldBiome(1);
		bool should1 = ShouldBiome(2);
		bool should2 = ShouldBiome(4);
		bool should3 = ShouldBiome(8);
		if (!should0 && !should1 && !should2 && !should3)
		{
			return;
		}
		TextureData biomedata = new TextureData(biomemap.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - Radius, 0f, 0f - Radius));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(Radius, 0f, 0f - Radius));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - Radius, 0f, Radius));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(Radius, 0f, Radius));
		TerrainMeta.BiomeMap.ForEachParallel(v, v2, v3, v4, delegate(int x, int z)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			float normZ = TerrainMeta.BiomeMap.Coordinate(z);
			float normX = TerrainMeta.BiomeMap.Coordinate(x);
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), 0f, TerrainMeta.DenormalizeZ(normZ));
			Vector3 val2 = ((Matrix4x4)(ref worldToLocal)).MultiplyPoint3x4(val) - offset;
			float num = Mathf.InverseLerp(Radius, Radius - Fade, Vector3Ex.Magnitude2D(val2));
			if (num != 0f)
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
				TerrainMeta.BiomeMap.SetBiomeRaw(x, z, interpolatedVector, num);
			}
		});
	}

	protected override void ApplyTopology(Matrix4x4 localToWorld, Matrix4x4 worldToLocal)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		if (Radius == 0f)
		{
			Radius = extents.x;
		}
		TextureData topologydata = new TextureData(topologymap.Get());
		Vector3 v = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - Radius, 0f, 0f - Radius));
		Vector3 v2 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(Radius, 0f, 0f - Radius));
		Vector3 v3 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(0f - Radius, 0f, Radius));
		Vector3 v4 = ((Matrix4x4)(ref localToWorld)).MultiplyPoint3x4(offset + new Vector3(Radius, 0f, Radius));
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
