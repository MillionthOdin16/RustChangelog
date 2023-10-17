public class ModelConditionTest_False : ModelConditionTest
{
	public ConditionalModel reference = null;

	public override bool DoTest(BaseEntity ent)
	{
		return !reference.RunTests(ent);
	}
}
