using ProtoBuf;
using UnityEngine;

public class ItemModSetFrequency : ItemMod
{
	public GameObjectRef frequencyPanelPrefab;

	public bool allowArmDisarm;

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		base.ServerCommand(item, command, player);
		if (command.Contains("SetFrequency"))
		{
			int result = 0;
			if (int.TryParse(command.Substring(command.IndexOf(":") + 1), out result))
			{
				item.instanceData.dataInt = result;
				item.MarkDirty();
			}
			else
			{
				Debug.Log((object)"Parse fuckup");
			}
		}
		if (command == "rf_on")
		{
			item.SetFlag(Item.Flag.IsOn, b: true);
			item.MarkDirty();
		}
		else if (command == "rf_off")
		{
			item.SetFlag(Item.Flag.IsOn, b: false);
			item.MarkDirty();
		}
	}

	public override void OnItemCreated(Item item)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		if (item.instanceData == null)
		{
			item.instanceData = new InstanceData();
			item.instanceData.ShouldPool = false;
			item.instanceData.dataInt = -1;
		}
	}
}
