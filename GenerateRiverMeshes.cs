using UnityEngine;

public class GenerateRiverMeshes : ProceduralComponent
{
	public const float NormalSmoothing = 0.1f;

	public const bool SnapToTerrain = true;

	public Mesh RiverMesh;

	public Mesh[] RiverMeshes;

	public Material RiverMaterial;

	public PhysicMaterial RiverPhysicMaterial;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		RiverMeshes = (Mesh[])(object)new Mesh[1] { RiverMesh };
		foreach (PathList river in TerrainMeta.Path.Rivers)
		{
			foreach (PathList.MeshObject item in river.CreateMesh(RiverMeshes, 0.1f, snapToTerrain: true, !river.Path.Circular, !river.Path.Circular))
			{
				GameObject val = new GameObject("River Mesh");
				val.transform.position = item.Position;
				val.tag = "River";
				val.layer = 4;
				GameObjectEx.SetHierarchyGroup(val, river.Name);
				val.SetActive(false);
				MeshCollider obj = val.AddComponent<MeshCollider>();
				((Collider)obj).sharedMaterial = RiverPhysicMaterial;
				obj.sharedMesh = item.Meshes[0];
				val.AddComponent<RiverInfo>();
				val.AddComponent<WaterBody>().FishingType = WaterBody.FishingTag.River;
				val.AddComponent<AddToWaterMap>();
				val.SetActive(true);
			}
		}
	}
}
