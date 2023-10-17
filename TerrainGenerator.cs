using UnityEngine;

public class TerrainGenerator : SingletonComponent<TerrainGenerator>
{
	public TerrainConfig config;

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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Terrain component = Terrain.CreateTerrainGameObject(new TerrainData
		{
			baseMapResolution = GetBaseMapRes(),
			heightmapResolution = heightmapResolution,
			alphamapResolution = alphamapResolution,
			size = new Vector3((float)World.Size, 1000f, (float)World.Size)
		}).GetComponent<Terrain>();
		((Component)component).transform.position = ((Component)this).transform.position + new Vector3((float)(0L - (long)World.Size) * 0.5f, 0f, (float)(0L - (long)World.Size) * 0.5f);
		component.drawInstanced = false;
		component.castShadows = config.CastShadows;
		component.materialType = (MaterialType)3;
		component.materialTemplate = config.Material;
		((Component)component).gameObject.tag = ((Component)this).gameObject.tag;
		((Component)component).gameObject.layer = ((Component)this).gameObject.layer;
		((Collider)((Component)component).gameObject.GetComponent<TerrainCollider>()).sharedMaterial = config.GenericMaterial;
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
