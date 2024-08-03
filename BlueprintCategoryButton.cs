using TMPro;
using UnityEngine;

public class BlueprintCategoryButton : MonoBehaviour, IInventoryChanged
{
	public TextMeshProUGUI amountLabel;

	public ItemCategory Category;

	public bool AlwaysShow = false;

	public bool ShowItemCount = true;

	public GameObject BackgroundHighlight;

	public SoundDefinition clickSound;

	public SoundDefinition hoverSound;
}
