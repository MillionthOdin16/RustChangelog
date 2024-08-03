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
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
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
				val.tag = "IgnoreCollider";
				GameObjectEx.SetHierarchyGroup(val, road.Name);
				val.SetActive(false);
				MeshCollider obj = val.AddComponent<MeshCollider>();
				((Collider)obj).sharedMaterial = RoadPhysicMaterial;
				obj.sharedMesh = item.Meshes[0];
				TagComponentEx.SetCustomTag(val, GameObjectTag.Road, apply: true);
				val.AddComponent<AddToHeightMap>();
				val.SetActive(true);
			}
		}
	}
}
