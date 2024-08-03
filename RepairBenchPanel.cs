using UnityEngine;
using UnityEngine.UI;

public class RepairBenchPanel : LootPanel
{
	public Text infoText;

	public GameObject repairButton;

	public GameObject skinLinkButton;

	public Color gotColor;

	public Color notGotColor;

	public Color skinColour;

	public Phrase phraseEmpty;

	public Phrase phraseNotRepairable;

	public Phrase phraseRepairNotNeeded;

	public Phrase phraseNoBlueprint;

	public GameObject skinsPanel;

	public GameObject changeSkinDialog;

	public IconSkinPicker picker;

	public GameObject attachmentSkinBlocker;
}
