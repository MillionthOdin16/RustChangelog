using System;
using UnityEngine;

public class TerrainTopologyMap : TerrainMap<int>
{
	public Texture2D TopologyTexture = null;

	public override void Setup()
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		res = terrain.terrainData.alphamapResolution;
		src = (dst = new int[res * res]);
		if (!((Object)(object)TopologyTexture != (Object)null))
		{
			return;
		}
		if (((Texture)TopologyTexture).width == ((Texture)TopologyTexture).height && ((Texture)TopologyTexture).width == res)
		{
			Color32[] pixels = TopologyTexture.GetPixels32();
			int i = 0;
			int num = 0;
			for (; i < res; i++)
			{
				int num2 = 0;
				while (num2 < res)
				{
					dst[i * res + num2] = BitUtility.DecodeInt(pixels[num]);
					num2++;
					num++;
				}
			}
		}
		else
		{
			Debug.LogError((object)("Invalid topology texture: " + ((Object)TopologyTexture).name));
		}
	}

	public void GenerateTextures()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		TopologyTexture = new Texture2D(res, res, (TextureFormat)4, false, true);
		((Object)TopologyTexture).name = "TopologyTexture";
		((Texture)TopologyTexture).wrapMode = (TextureWrapMode)1;
		Color32[] col = (Color32[])(object)new Color32[res * res];
		Parallel.For(0, res, (Action<int>)delegate(int z)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < res; i++)
			{
				col[z * res + i] = BitUtility.EncodeInt(src[z * res + i]);
			}
		});
		TopologyTexture.SetPixels32(col);
	}

	public void ApplyTextures()
	{
		TopologyTexture.Apply(false, true);
	}

	public bool GetTopology(Vector3 worldPos, int mask)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return GetTopology(normX, normZ, mask);
	}

	public bool GetTopology(float normX, float normZ, int mask)
	{
		int x = Index(normX);
		int z = Index(normZ);
		return GetTopology(x, z, mask);
	}

	public bool GetTopology(int x, int z, int mask)
	{
		return (src[z * res + x] & mask) != 0;
	}

	public int GetTopology(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return GetTopology(normX, normZ);
	}

	public int GetTopology(float normX, float normZ)
	{
		int x = Index(normX);
		int z = Index(normZ);
		return GetTopology(x, z);
	}

	public int GetTopologyFast(Vector2 uv)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		int num = res - 1;
		int num2 = (int)(uv.x * (float)res);
		int num3 = (int)(uv.y * (float)res);
		num2 = ((num2 >= 0) ? num2 : 0);
		num3 = ((num3 >= 0) ? num3 : 0);
		num2 = ((num2 <= num) ? num2 : num);
		num3 = ((num3 <= num) ? num3 : num);
		return src[num3 * res + num2];
	}

	public int GetTopology(int x, int z)
	{
		return src[z * res + x];
	}

	public void SetTopology(Vector3 worldPos, int mask)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		SetTopology(normX, normZ, mask);
	}

	public void SetTopology(float normX, float normZ, int mask)
	{
		int x = Index(normX);
		int z = Index(normZ);
		SetTopology(x, z, mask);
	}

	public void SetTopology(int x, int z, int mask)
	{
		dst[z * res + x] = mask;
	}

	public void AddTopology(Vector3 worldPos, int mask)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		AddTopology(normX, normZ, mask);
	}

	public void AddTopology(float normX, float normZ, int mask)
	{
		int x = Index(normX);
		int z = Index(normZ);
		AddTopology(x, z, mask);
	}

	public void AddTopology(int x, int z, int mask)
	{
		dst[z * res + x] |= mask;
	}

	public void RemoveTopology(Vector3 worldPos, int mask)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		RemoveTopology(normX, normZ, mask);
	}

	public void RemoveTopology(float normX, float normZ, int mask)
	{
		int x = Index(normX);
		int z = Index(normZ);
		RemoveTopology(x, z, mask);
	}

	public void RemoveTopology(int x, int z, int mask)
	{
		dst[z * res + x] &= ~mask;
	}

	public int GetTopology(Vector3 worldPos, float radius)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return GetTopology(normX, normZ, radius);
	}

	public int GetTopology(float normX, float normZ, float radius)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		float num2 = TerrainMeta.OneOverSize.x * radius;
		int num3 = Index(normX - num2);
		int num4 = Index(normX + num2);
		int num5 = Index(normZ - num2);
		int num6 = Index(normZ + num2);
		for (int i = num5; i <= num6; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				num |= src[i * res + j];
			}
		}
		return num;
	}

	public void SetTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		SetTopology(normX, normZ, mask, radius, fade);
	}

	public void SetTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				dst[z * res + x] = mask;
			}
		};
		ApplyFilter(normX, normZ, radius, fade, action);
	}

	public void AddTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		AddTopology(normX, normZ, mask, radius, fade);
	}

	public void AddTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				dst[z * res + x] |= mask;
			}
		};
		ApplyFilter(normX, normZ, radius, fade, action);
	}
}
