using System.Collections.Generic;
using UnityEngine;

public class GenerateRoadMeshes : ProceduralComponent
{
	public const float NormalSmoothing = 0f;

	public const bool SnapToTerrain = true;

	public Mesh RoadMesh = null;

	public Mesh[] RoadMeshes = null;

	public Material RoadMaterial = null;

	public Material RoadRingMaterial = null;

	public PhysicMaterial RoadPhysicMaterial = null;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (RoadMeshes == null || RoadMeshes.Length == 0)
		{
			RoadMeshes = (Mesh[])(object)new Mesh[1] { RoadMesh };
		}
		List<PathList> roads = TerrainMeta.Path.Roads;
		foreach (PathList item in roads)
		{
			if (item.Hierarchy >= 2)
			{
				continue;
			}
			foreach (PathList.MeshObject item2 in item.CreateMesh(RoadMeshes, 0f, snapToTerrain: true, !item.Path.Circular, !item.Path.Circular))
			{
				GameObject val = new GameObject("Road Mesh");
				val.transform.position = item2.Position;
				val.layer = 16;
				val.SetHierarchyGroup(item.Name);
				val.SetActive(false);
				MeshCollider val2 = val.AddComponent<MeshCollider>();
				((Collider)val2).sharedMaterial = RoadPhysicMaterial;
				val2.sharedMesh = item2.Meshes[0];
				val.AddComponent<AddToHeightMap>();
				val.SetActive(true);
			}
		}
	}
}
