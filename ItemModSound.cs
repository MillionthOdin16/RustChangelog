using Rust;
using UnityEngine;

public class ItemModSound : ItemMod
{
	public enum Type
	{
		OnAttachToWeapon
	}

	public GameObjectRef effect = new GameObjectRef();

	public Type actionType;

	public override void OnParentChanged(Item item)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isLoadingSave && actionType == Type.OnAttachToWeapon && item.parentItem != null && item.parentItem.info.category == ItemCategory.Weapon)
		{
			BasePlayer ownerPlayer = item.parentItem.GetOwnerPlayer();
			if (!((Object)(object)ownerPlayer == (Object)null) && !ownerPlayer.IsNpc)
			{
				Effect.server.Run(effect.resourcePath, ownerPlayer, 0u, Vector3.zero, Vector3.zero);
			}
		}
	}
}
