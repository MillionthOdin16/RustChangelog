public class ModelConditionTest_Outside : ModelConditionTest
{
	public override bool DoTest(BaseEntity ent)
	{
		return CheckCondition(ent);
	}

	public static bool CheckCondition(BaseEntity ent)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		OBB val = ent.WorldSpaceBounds();
		return ent.IsOutside(((OBB)(ref val)).GetPoint(0f, 1f, 0f));
	}
}
