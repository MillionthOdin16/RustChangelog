using UnityEngine;

public class DiscoFloorMesh : MonoBehaviour, IClientComponent
{
	public int GridRows = 5;

	public int GridColumns = 5;

	public float GridSize = 1f;

	[Range(0f, 10f)]
	public float TestOffset = 0f;

	public Color OffColor = Color.grey;

	public MeshRenderer Renderer = null;

	public bool DrawInEditor = false;

	public MeshFilter Filter = null;

	public AnimationCurve customCurveX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AnimationCurve customCurveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
