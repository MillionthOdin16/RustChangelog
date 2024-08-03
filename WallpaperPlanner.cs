using ConVar;
using ProtoBuf;
using UnityEngine;

public class WallpaperPlanner : Planner
{
	public BaseEntity[] wallpaperPrefabs;

	private static ItemDefinition wallpaperItem;

	public static ItemDefinition WallpaperItemDef
	{
		get
		{
			if ((Object)(object)wallpaperItem == (Object)null)
			{
				wallpaperItem = ItemManager.FindItemDefinition("wallpaper");
			}
			return wallpaperItem;
		}
	}

	public override bool isTypeDeployable => true;

	private BaseEntity GetWallpaperPrefab(BaseEntity aimedEntity)
	{
		if ((Object)(object)aimedEntity != (Object)null && aimedEntity.net != null)
		{
			for (int i = 0; i < buildableList.Length; i++)
			{
				if (buildableList[i].prefabID == aimedEntity.prefabID)
				{
					return wallpaperPrefabs[i];
				}
			}
		}
		return null;
	}

	public override Deployable GetDeployable()
	{
		return base.GetDeployable();
	}

	public override void DoBuild(CreateBuilding msg)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return;
		}
		if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack())
		{
			ownerPlayer.ChatMessage("AntiHack!");
			return;
		}
		Construction construction = PrefabAttribute.server.Find<Construction>(msg.blockID);
		if (construction == null)
		{
			ownerPlayer.ChatMessage("Couldn't find Construction " + msg.blockID);
		}
		else if (!CanAffordToPlace(construction))
		{
			ownerPlayer.ChatMessage("Can't afford to place!");
		}
		else if (!ownerPlayer.CanBuild() && !construction.canBypassBuildingPermission)
		{
			ownerPlayer.ChatMessage("Building is blocked!");
		}
		else
		{
			if (!((NetworkableId)(ref msg.entity)).IsValid)
			{
				return;
			}
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(msg.entity) as BaseEntity;
			if ((Object)(object)baseEntity == (Object)null)
			{
				NetworkableId entity = msg.entity;
				ownerPlayer.ChatMessage("Couldn't find entity " + ((object)(NetworkableId)(ref entity)).ToString());
				return;
			}
			Socket_Base socket_Base = null;
			if (msg.socket != 0)
			{
				string text = StringPool.Get(msg.socket);
				if (text != "")
				{
					socket_Base = FindSocket(text, baseEntity.prefabID);
				}
				if (socket_Base == null)
				{
					ownerPlayer.ChatMessage("Couldn't find socket " + msg.socket);
					return;
				}
			}
			if (baseEntity is BuildingBlock buildingBlock && !buildingBlock.HasWallpaper())
			{
				PayForPlacement(ownerPlayer, construction);
				buildingBlock.SetWallpaper(skinID);
				if (construction.deployable.placeEffect.isValid)
				{
					Effect.server.Run(construction.deployable.placeEffect.resourcePath, ((Component)buildingBlock).transform.TransformPoint(socket_Base.worldPosition), ((Component)buildingBlock).transform.up);
				}
			}
		}
	}
}
