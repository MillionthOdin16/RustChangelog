using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Gestures/Gesture Config")]
public class GestureConfig : ScriptableObject
{
	public enum GestureType
	{
		Player,
		NPC,
		Cinematic
	}

	public enum PlayerModelLayer
	{
		UpperBody = 3,
		FullBody
	}

	public enum MovementCapabilities
	{
		FullMovement,
		NoMovement
	}

	public enum AnimationType
	{
		OneShot,
		Loop
	}

	public enum ViewMode
	{
		FirstPerson,
		ThirdPerson
	}

	public enum GestureActionType
	{
		None,
		ShowNameTag,
		DanceAchievement
	}

	[ReadOnly]
	public uint gestureId = 0u;

	public string gestureCommand;

	public string convarName;

	public Phrase gestureName;

	public Sprite icon;

	public int order = 1;

	public float duration = 1.5f;

	public bool canCancel = true;

	[Header("Player model setup")]
	public PlayerModelLayer playerModelLayer = PlayerModelLayer.UpperBody;

	public GestureType gestureType = GestureType.Player;

	public bool hideHeldEntity = true;

	public bool canDuckDuringGesture = false;

	public MovementCapabilities movementMode = MovementCapabilities.FullMovement;

	public AnimationType animationType = AnimationType.OneShot;

	public BasePlayer.CameraMode viewMode = BasePlayer.CameraMode.FirstPerson;

	public bool useRootMotion = false;

	[Header("Ownership")]
	public GestureActionType actionType = GestureActionType.None;

	public bool forceUnlock = false;

	public SteamDLCItem dlcItem = null;

	public SteamInventoryItem inventoryItem = null;

	public bool IsOwnedBy(BasePlayer player)
	{
		if (forceUnlock)
		{
			return true;
		}
		if (gestureType == GestureType.NPC)
		{
			return player.IsNpc;
		}
		if (gestureType == GestureType.Cinematic)
		{
			return player.IsAdmin;
		}
		if ((Object)(object)dlcItem != (Object)null && dlcItem.CanUse(player))
		{
			return true;
		}
		if ((Object)(object)inventoryItem != (Object)null && player.blueprints.steamInventory.HasItem(inventoryItem.id))
		{
			return true;
		}
		return false;
	}

	public bool CanBeUsedBy(BasePlayer player)
	{
		if (player.isMounted)
		{
			if (playerModelLayer == PlayerModelLayer.FullBody)
			{
				return false;
			}
			if (player.GetMounted().allowedGestures == BaseMountable.MountGestureType.None)
			{
				return false;
			}
		}
		if (player.IsSwimming() && playerModelLayer == PlayerModelLayer.FullBody)
		{
			return false;
		}
		if (playerModelLayer == PlayerModelLayer.FullBody && player.modelState.ducked)
		{
			return false;
		}
		return true;
	}
}
