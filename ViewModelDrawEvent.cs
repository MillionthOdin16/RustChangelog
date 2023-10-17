using System;
using UnityEngine;

[Serializable]
public struct ViewModelDrawEvent : IEquatable<ViewModelDrawEvent>
{
	public ViewModelRenderer viewModelRenderer;

	public Renderer renderer;

	public bool skipDepthPrePass;

	public Material material;

	public int subMesh;

	public int pass;

	public bool Equals(ViewModelDrawEvent other)
	{
		return object.Equals(viewModelRenderer, other.viewModelRenderer) && object.Equals(renderer, other.renderer) && skipDepthPrePass == other.skipDepthPrePass && object.Equals(material, other.material) && subMesh == other.subMesh && pass == other.pass;
	}

	public override bool Equals(object obj)
	{
		return obj is ViewModelDrawEvent other && Equals(other);
	}

	public override int GetHashCode()
	{
		int num = (((Object)(object)viewModelRenderer != (Object)null) ? ((object)viewModelRenderer).GetHashCode() : 0);
		num = (num * 397) ^ (((Object)(object)renderer != (Object)null) ? ((object)renderer).GetHashCode() : 0);
		num = (num * 397) ^ skipDepthPrePass.GetHashCode();
		num = (num * 397) ^ (((Object)(object)material != (Object)null) ? ((object)material).GetHashCode() : 0);
		num = (num * 397) ^ subMesh;
		return (num * 397) ^ pass;
	}
}
