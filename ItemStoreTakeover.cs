using System;
using UnityEngine;

[Serializable]
public struct ItemStoreTakeover
{
	public string Name;

	public SteamInventoryItem Item;

	public GameObjectRef Prefab;

	public Sprite IconOverride;

	public string Subtitle;
}
