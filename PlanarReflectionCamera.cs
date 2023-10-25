using UnityEngine;

public class PlanarReflectionCamera : MonoBehaviour
{
	public static PlanarReflectionCamera instance;

	public float updateRate = 1f;

	public Mesh waterPlaneMesh;

	public Material waterPlaneMaterial;
}
