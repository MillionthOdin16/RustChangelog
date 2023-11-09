using Facepunch;
using ProtoBuf;
using Rust.UI;
using UnityEngine;
using ntw.CurvedTextMeshPro;

public class SkullTrophy : StorageContainer
{
	public RustText NameText = null;

	public TextProOnACircle CircleModifier;

	public int AngleModifierMinCharCount = 3;

	public int AngleModifierMaxCharCount = 20;

	public int AngleModifierMinArcAngle = 20;

	public int AngleModifierMaxArcAngle = 45;

	public float SunsetTime = 18f;

	public float SunriseTime = 5f;

	public MeshRenderer[] SkullRenderers = null;

	public Material[] DaySkull;

	public Material[] NightSkull;

	public Material[] NoSkull;

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		SendNetworkUpdate();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			if (base.inventory != null && base.inventory.itemList.Count == 1)
			{
				info.msg.skullTrophy = Pool.Get<SkullTrophy>();
				info.msg.skullTrophy.playerName = base.inventory.itemList[0].GetName(false);
				info.msg.skullTrophy.streamerName = base.inventory.itemList[0].GetName(true);
			}
			else if (info.msg.skullTrophy != null)
			{
				info.msg.skullTrophy.playerName = string.Empty;
				info.msg.skullTrophy.streamerName = string.Empty;
			}
		}
	}
}
