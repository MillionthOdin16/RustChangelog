using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(CommandBufferManager))]
public class PostOpaqueDepth : MonoBehaviour
{
	public RenderTexture postOpaqueDepth = null;

	public RenderTexture PostOpaque => postOpaqueDepth;
}
