using UnityEngine;

public class GenerateRoadMeshes : ProceduralComponent
{
	public const float NormalSmoothing = 0f;

	public const bool SnapToTerrain = true;

	public Mesh RoadMesh;

	public Mesh[] RoadMeshes;

	public Material RoadMaterial;

	public Material RoadRingMaterial;

	public PhysicMaterial RoadPhysicMaterial;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (RoadMeshes == null || RoadMeshes.Length == 0)
		{
			RoadMeshes = (Mesh[])(object)new Mesh[1] { RoadMesh };
		}
		foreach (PathList road in TerrainMeta.Path.Roads)
		{
			if (road.Hierarchy >= 2)
			{
				continue;
			}
			foreach (PathList.MeshObject item in road.CreateMesh(RoadMeshes, 0f, snapToTerrain: true, !road.Path.Circular, !road.Path.Circular))
			{
				GameObject val = new GameObject("Road Mesh");
				val.transform.position = item.Position;
				val.layer = 16;
				GameObjectEx.SetHierarchyGroup(val, road.Name);
				val.SetActive(false);
				MeshCollider obj = val.AddComponent<MeshCollider>();
				((Collider)obj).sharedMaterial = RoadPhysicMaterial;
				obj.sharedMesh = item.Meshes[0];
				val.AddComponent<AddToHeightMap>();
				val.SetActive(true);
			}
		}
	}
}
