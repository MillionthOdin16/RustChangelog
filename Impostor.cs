using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class Impostor : MonoBehaviour, IClientComponent, IPrefabPreProcess
{
	public ImpostorAsset asset;

	[Header("Baking")]
	public GameObject reference = null;

	public float angle = 0f;

	public int resolution = 1024;

	public int padding = 32;

	public bool spriteOutlineAsMesh = false;

	private void OnEnable()
	{
	}

	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
	}
}
