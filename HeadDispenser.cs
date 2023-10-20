using UnityEngine;

public class HeadDispenser : EntityComponent<BaseEntity>
{
	public ItemDefinition HeadDef;

	public GameObjectRef SourceEntity;

	private bool hasDispensed;

	public BaseEntity overrideEntity { get; set; }

	public void DispenseHead(HitInfo info, BaseCorpse corpse)
	{
		if (hasDispensed || !(info.Weapon is BaseMelee baseMelee) || !baseMelee.gathering.ProduceHeadItem)
		{
			return;
		}
		if ((Object)(object)info.InitiatorPlayer != (Object)null)
		{
			Item item = ItemManager.CreateByItemID(HeadDef.itemid, 1, 0uL);
			HeadEntity associatedEntity = ItemModAssociatedEntity<HeadEntity>.GetAssociatedEntity(item);
			BaseEntity baseEntity = (((Object)(object)overrideEntity != (Object)null) ? overrideEntity : SourceEntity.GetEntity());
			overrideEntity = null;
			if ((Object)(object)associatedEntity != (Object)null && (Object)(object)baseEntity != (Object)null)
			{
				associatedEntity.SetupSourceId(baseEntity.prefabID);
				if (corpse is PlayerCorpse playerCorpse)
				{
					associatedEntity.SetupPlayerId(playerCorpse.playerName, playerCorpse.playerSteamID);
					associatedEntity.AssignClothing(playerCorpse.containers[1]);
				}
				else if (corpse is HorseCorpse horseCorpse)
				{
					associatedEntity.AssignHorseBreed(horseCorpse.breedIndex);
				}
			}
			info.InitiatorPlayer.inventory.GiveItem(item);
			info.InitiatorPlayer.Command("note.inv", HeadDef.itemid, 1);
		}
		hasDispensed = true;
	}
}
