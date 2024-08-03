using System;
using UnityEngine;

public class Deployable : PrefabAttribute
{
	public Mesh guideMesh;

	public Vector3 guideMeshScale = Vector3.one;

	public bool guideLights = true;

	public bool wantsInstanceData = false;

	public bool copyInventoryFromItem = false;

	public bool setSocketParent = false;

	public bool toSlot = false;

	public BaseEntity.Slot slot;

	public GameObjectRef placeEffect;

	[NonSerialized]
	public Bounds bounds;

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		bounds = rootObj.GetComponent<BaseEntity>().bounds;
	}

	protected override Type GetIndexedType()
	{
		return typeof(Deployable);
	}
}
