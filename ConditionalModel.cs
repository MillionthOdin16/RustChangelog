using System;
using UnityEngine;

public class ConditionalModel : PrefabAttribute
{
	public GameObjectRef prefab;

	public bool onClient = true;

	public bool onServer = true;

	[NonSerialized]
	public ModelConditionTest[] conditions;

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		conditions = ((Component)this).GetComponentsInChildren<ModelConditionTest>(true);
	}

	public bool RunTests(BaseEntity parent)
	{
		for (int i = 0; i < conditions.Length; i++)
		{
			if (!conditions[i].DoTest(parent))
			{
				return false;
			}
		}
		return true;
	}

	public GameObject InstantiateSkin(BaseEntity parent)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!onServer && isServer)
		{
			return null;
		}
		GameObject val = gameManager.CreatePrefab(prefab.resourcePath, ((Component)parent).transform, active: false);
		if (Object.op_Implicit((Object)(object)val))
		{
			val.transform.localPosition = worldPosition;
			val.transform.localRotation = worldRotation;
			val.AwakeFromInstantiate();
		}
		return val;
	}

	protected override Type GetIndexedType()
	{
		return typeof(ConditionalModel);
	}
}
