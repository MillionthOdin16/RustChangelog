using UnityEngine;

public class ModelConditionTest_Wallpaper : ModelConditionTest
{
	public override bool DoTest(BaseEntity ent)
	{
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if ((Object)(object)buildingBlock == (Object)null)
		{
			return false;
		}
		return buildingBlock.HasWallpaper();
	}
}
