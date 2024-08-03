using System;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ConstructionPlaceholder : PrefabAttribute, IPrefabPreProcess
{
	public Mesh mesh = null;

	public Material material = null;

	public bool renderer = false;

	public bool collider = false;

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (!clientside)
		{
			return;
		}
		if (renderer)
		{
			MeshFilter component = rootObj.GetComponent<MeshFilter>();
			MeshRenderer component2 = rootObj.GetComponent<MeshRenderer>();
			if (!Object.op_Implicit((Object)(object)component))
			{
				component = rootObj.AddComponent<MeshFilter>();
				component.sharedMesh = mesh;
			}
			if (!Object.op_Implicit((Object)(object)component2))
			{
				component2 = rootObj.AddComponent<MeshRenderer>();
				((Renderer)component2).sharedMaterial = material;
				((Renderer)component2).shadowCastingMode = (ShadowCastingMode)0;
			}
		}
		if (collider)
		{
			MeshCollider component3 = rootObj.GetComponent<MeshCollider>();
			if (!Object.op_Implicit((Object)(object)component3))
			{
				component3 = rootObj.AddComponent<MeshCollider>();
				component3.sharedMesh = mesh;
			}
		}
	}

	protected override Type GetIndexedType()
	{
		return typeof(ConstructionPlaceholder);
	}
}
