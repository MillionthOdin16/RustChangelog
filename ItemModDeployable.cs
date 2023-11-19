using UnityEngine;

public class ItemModDeployable : MonoBehaviour
{
	public GameObjectRef entityPrefab = new GameObjectRef();

	[Header("Tooltips")]
	public bool showCrosshair = false;

	public string UnlockAchievement;

	public Deployable GetDeployable(BaseEntity entity)
	{
		GameObject val = entity.gameManager.FindPrefab(entityPrefab.resourcePath);
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		return entity.prefabAttribute.Find<Deployable>(entityPrefab.resourceID);
	}

	internal void OnDeployed(BaseEntity ent, BasePlayer player)
	{
		if (player.IsValid() && !string.IsNullOrEmpty(UnlockAchievement))
		{
			player.GiveAchievement(UnlockAchievement);
		}
		if (ent is BuildingPrivlidge buildingPrivlidge)
		{
			buildingPrivlidge.AddPlayer(player);
		}
	}
}
