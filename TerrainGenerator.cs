using UnityEngine;

public class TerrainGenerator : SingletonComponent<TerrainGenerator>
{
	public TerrainConfig config = null;

	private const float HeightMapRes = 1f;

	private const float SplatMapRes = 0.5f;

	private const float BaseMapRes = 0.01f;

	public static int GetHeightMapRes()
	{
		return Mathf.Min(4096, Mathf.ClosestPowerOfTwo((int)((float)World.Size * 1f))) + 1;
	}

	public static int GetSplatMapRes()
	{
		return Mathf.Min(2048, Mathf.NextPowerOfTwo((int)((float)World.Size * 0.5f)));
	}

	public static int GetBaseMapRes()
	{
		return Mathf.Min(2048, Mathf.NextPowerOfTwo((int)((float)World.Size * 0.01f)));
	}

	public GameObject CreateTerrain()
	{
		return CreateTerrain(GetHeightMapRes(), GetSplatMapRes());
	}

	public GameObject CreateTerrain(int heightmapResolution, int alphamapResolution)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		TerrainData val = new TerrainData();
		val.baseMapResolution = GetBaseMapRes();
		val.heightmapResolution = heightmapResolution;
		val.alphamapResolution = alphamapResolution;
		val.size = new Vector3((float)World.Size, 1000f, (float)World.Size);
		Terrain component = Terrain.CreateTerrainGameObject(val).GetComponent<Terrain>();
		((Component)component).transform.position = ((Component)this).transform.position + new Vector3((float)(0L - (long)World.Size) * 0.5f, 0f, (float)(0L - (long)World.Size) * 0.5f);
		component.drawInstanced = false;
		component.castShadows = config.CastShadows;
		component.materialType = (MaterialType)3;
		component.materialTemplate = config.Material;
		((Component)component).gameObject.tag = ((Component)this).gameObject.tag;
		((Component)component).gameObject.layer = ((Component)this).gameObject.layer;
		TerrainCollider component2 = ((Component)component).gameObject.GetComponent<TerrainCollider>();
		((Collider)component2).sharedMaterial = config.GenericMaterial;
		TerrainMeta terrainMeta = ((Component)component).gameObject.AddComponent<TerrainMeta>();
		((Component)component).gameObject.AddComponent<TerrainPhysics>();
		((Component)component).gameObject.AddComponent<TerrainColors>();
		((Component)component).gameObject.AddComponent<TerrainCollision>();
		((Component)component).gameObject.AddComponent<TerrainBiomeMap>();
		((Component)component).gameObject.AddComponent<TerrainAlphaMap>();
		((Component)component).gameObject.AddComponent<TerrainHeightMap>();
		((Component)component).gameObject.AddComponent<TerrainSplatMap>();
		((Component)component).gameObject.AddComponent<TerrainTopologyMap>();
		((Component)component).gameObject.AddComponent<TerrainWaterMap>();
		((Component)component).gameObject.AddComponent<TerrainPlacementMap>();
		((Component)component).gameObject.AddComponent<TerrainPath>();
		((Component)component).gameObject.AddComponent<TerrainTexturing>();
		terrainMeta.terrain = component;
		terrainMeta.config = config;
		Object.DestroyImmediate((Object)(object)((Component)this).gameObject);
		return ((Component)component).gameObject;
	}
}
