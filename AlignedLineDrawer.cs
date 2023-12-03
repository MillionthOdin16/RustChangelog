using System;
using UnityEngine;

public class AlignedLineDrawer : MonoBehaviour, IClientComponent
{
	[Serializable]
	public struct LinePoint
	{
		public Vector3 LocalPosition;

		public Vector3 WorldNormal;
	}

	public MeshFilter Filter = null;

	public MeshRenderer Renderer = null;

	public float LineWidth = 1f;

	public float SurfaceOffset = 0.001f;

	public float SprayThickness = 0.4f;

	public float uvTilingFactor = 1f;

	public bool DrawEndCaps = false;

	public bool DrawSideMesh = false;

	public bool DrawBackMesh = false;

	public SprayCanSpray_Freehand Spray = null;
}
