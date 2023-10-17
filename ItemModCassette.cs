public class ItemModCassette : ItemModAssociatedEntity<Cassette>
{
	public int noteSpriteIndex = 0;

	public PreloadedCassetteContent PreloadedContent = null;

	protected override bool AllowNullParenting => true;

	protected override bool AllowHeldEntityParenting => true;

	protected override void OnAssociatedItemCreated(Cassette ent)
	{
		base.OnAssociatedItemCreated(ent);
		ent.AssignPreloadContent();
	}
}
