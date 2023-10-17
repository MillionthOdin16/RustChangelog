using System.Collections.Generic;
using UnityEngine;

public class GenerateRailMeshes : ProceduralComponent
{
	public const float NormalSmoothing = 0f;

	public const bool SnapToTerrain = false;

	public Mesh RailMesh = null;

	public Mesh[] RailMeshes = null;

	public Material RailMaterial = null;

	public PhysicMaterial RailPhysicMaterial = null;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if (RailMeshes == null || RailMeshes.Length == 0)
		{
			RailMeshes = (Mesh[])(object)new Mesh[1] { RailMesh };
		}
		List<PathList> rails = TerrainMeta.Path.Rails;
		foreach (PathList item in rails)
		{
			foreach (PathList.MeshObject item2 in item.CreateMesh(RailMeshes, 0f, snapToTerrain: false, !item.Path.Circular && !item.Start, !item.Path.Circular && !item.End))
			{
				GameObject val = new GameObject("Rail Mesh");
				val.transform.position = item2.Position;
				val.tag = "Railway";
				val.layer = 16;
				val.SetHierarchyGroup(item.Name);
				val.SetActive(false);
				MeshCollider val2 = val.AddComponent<MeshCollider>();
				((Collider)val2).sharedMaterial = RailPhysicMaterial;
				val2.sharedMesh = item2.Meshes[0];
				val.AddComponent<AddToHeightMap>();
				val.SetActive(true);
			}
			AddTrackSpline(item);
		}
	}

	private void AddTrackSpline(PathList rail)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		GameObject root = HierarchyUtil.GetRoot(rail.Name);
		TrainTrackSpline trainTrackSpline = root.AddComponent<TrainTrackSpline>();
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
