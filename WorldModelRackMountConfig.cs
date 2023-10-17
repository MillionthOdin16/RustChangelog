using System.Collections.Generic;
using UnityEngine;

public class WorldModelRackMountConfig : MonoBehaviour
{
	public List<WeaponRack.RackType> ExcludedRackTypes = new List<WeaponRack.RackType>();

	public Vector3 CenterOffsfet;

	public Vector3 LeftOffset;

	public Vector3 VerticalMountLocalRotation;

	public Vector3 VerticalMountLocalOffset;

	public int XSize = 3;

	public int YSize = 2;

	public int ZSize = 1;

	public List<PegConfig> Pegs = new List<PegConfig>();

	public List<PegConfig> VerticalPegs = new List<PegConfig>();

	public bool OverrideScale;

	public Vector3 Scale = Vector3.one;

	public bool UseManualRenderBounds;

	public Bounds ManualRenderBounds;

	public static WorldModelRackMountConfig GetForItemDef(ItemDefinition itemDef)
	{
		GameObjectRef worldModelPrefab = itemDef.worldModelPrefab;
		if (!worldModelPrefab.isValid)
		{
			return null;
		}
		return worldModelPrefab.Get().GetComponent<WorldModelRackMountConfig>();
	}
}
