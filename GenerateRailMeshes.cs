using UnityEngine;

public class GenerateRailMeshes : ProceduralComponent
{
	public const float NormalSmoothing = 0f;

	public const bool SnapToTerrain = false;

	public Mesh RailMesh;

	public Mesh[] RailMeshes;

	public Material RailMaterial;

	public PhysicMaterial RailPhysicMaterial;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		if (RailMeshes == null || RailMeshes.Length == 0)
		{
			RailMeshes = (Mesh[])(object)new Mesh[1] { RailMesh };
		}
		foreach (PathList rail in TerrainMeta.Path.Rails)
		{
			foreach (PathList.MeshObject item in rail.CreateMesh(RailMeshes, 0f, snapToTerrain: false, !rail.Path.Circular && !rail.Start, !rail.Path.Circular && !rail.End))
			{
				GameObject val = new GameObject("Rail Mesh");
				val.transform.position = item.Position;
				val.tag = "Railway";
				val.layer = 16;
				GameObjectEx.SetHierarchyGroup(val, rail.Name);
				val.SetActive(false);
				MeshCollider obj = val.AddComponent<MeshCollider>();
				((Collider)obj).sharedMaterial = RailPhysicMaterial;
				obj.sharedMesh = item.Meshes[0];
				val.AddComponent<AddToHeightMap>();
				val.SetActive(true);
			}
			AddTrackSpline(rail);
		}
	}

	private void AddTrackSpline(PathList rail)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		TrainTrackSpline trainTrackSpline = HierarchyUtil.GetRoot(rail.Name).AddComponent<TrainTrackSpline>();
		trainTrackSpline.aboveGroundSpawn = rail.Hierarchy == 2;
		trainTrackSpline.hierarchy = rail.Hierarchy;
		if (trainTrackSpline.aboveGroundSpawn)
		{
			TrainTrackSpline.SidingSplines.Add(trainTrackSpline);
		}
		Vector3[] array = (Vector3[])(object)new Vector3[rail.Path.Points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = rail.Path.Points[i];
			array[i].y += 0.41f;
		}
		Vector3[] array2 = (Vector3[])(object)new Vector3[rail.Path.Tangents.Length];
		for (int j = 0; j < array.Length; j++)
		{
			array2[j] = rail.Path.Tangents[j];
		}
		trainTrackSpline.SetAll(array, array2, 0.25f);
	}
}
