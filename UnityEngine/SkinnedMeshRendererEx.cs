namespace UnityEngine;

public static class SkinnedMeshRendererEx
{
	public static Transform FindRig(this SkinnedMeshRenderer renderer)
	{
		Transform parent = ((Component)renderer).transform.parent;
		Transform val = renderer.rootBone;
		while ((Object)(object)val != (Object)null && (Object)(object)val.parent != (Object)null && (Object)(object)val.parent != (Object)(object)parent)
		{
			val = val.parent;
		}
		return val;
	}
}
