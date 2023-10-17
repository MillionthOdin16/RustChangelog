using UnityEngine;

public class MaterialSwap : MonoBehaviour, IClientComponent
{
	public int materialIndex = 0;

	public Renderer myRenderer;

	public Material OverrideMaterial;
}
