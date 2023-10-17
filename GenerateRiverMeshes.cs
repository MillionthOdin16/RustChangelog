using System.Collections.Generic;
using UnityEngine;

public class GenerateRiverMeshes : ProceduralComponent
{
	public const float NormalSmoothing = 0.1f;

	public const bool SnapToTerrain = true;

	public Mesh RiverMesh = null;

	public Mesh[] RiverMeshes = null;

	public Material RiverMaterial = null;

	public PhysicMaterial RiverPhysicMaterial = null;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		RiverMeshes = (Mesh[])(object)new Mesh[1] { RiverMesh };
		List<PathList> rivers = TerrainMeta.Path.Rivers;
		foreach (PathList item in rivers)
		{
			foreach (PathList.MeshObject item2 in item.CreateMesh(RiverMeshes, 0.1f, snapToTerrain: true, !item.Path.Circular, !item.Path.Circular))
			{
				GameObject val = new GameObject("River Mesh");
				val.transform.position = item2.Position;
				val.tag = "River";
				val.layer = 4;
				val.SetHierarchyGroup(item.Name);
				val.SetActive(false);
				MeshCollider val2 = val.AddComponent<MeshCollider>();
				((Collider)val2).sharedMaterial = RiverPhysicMaterial;
				val2.sharedMesh = item2.Meshes[0];
				val.AddComponent<RiverInfo>();
				WaterBody waterBody = val.AddComponent<WaterBody>();
				waterBody.FishingType = WaterBody.FishingTag.River;
				val.AddComponent<AddToWaterMap>();
				val.SetActive(true);
			}
		}
	}
}
