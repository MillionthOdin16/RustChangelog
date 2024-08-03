using System;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionGrade : PrefabAttribute
{
	[NonSerialized]
	public Construction construction;

	public BuildingGrade gradeBase;

	public GameObjectRef skinObject;

	internal List<ItemAmount> _costToBuild = null;

	public float maxHealth => (Object.op_Implicit((Object)(object)gradeBase) && (bool)construction) ? (gradeBase.baseHealth * construction.healthMultiplier) : 0f;

	public List<ItemAmount> CostToBuild(BuildingGrade.Enum fromGrade = BuildingGrade.Enum.None)
	{
		if (_costToBuild == null)
		{
			_costToBuild = new List<ItemAmount>();
		}
		else
		{
			_costToBuild.Clear();
		}
		float num = ((fromGrade == gradeBase.type) ? 0.2f : 1f);
		foreach (ItemAmount item in gradeBase.baseCost)
		{
			_costToBuild.Add(new ItemAmount(item.itemDef, Mathf.Ceil(item.amount * construction.costMultiplier * num)));
		}
		return _costToBuild;
	}

	protected override Type GetIndexedType()
	{
		return typeof(ConstructionGrade);
	}
}
