using UnityEngine;
using UnityEngine.UI;

public class WorkbenchPanel : LootPanel, IInventoryChanged
{
	public GameObject tier1Button;

	public GameObject tier2Button;

	public GameObject tier3Button;

	public Text timerText;

	public Text costText;

	public GameObject expermentCostParent;

	public GameObject controlsParent;

	public GameObject allUnlockedNotification;

	public GameObject informationParent;

	public GameObject cycleIcon;

	public TechTreeDialog techTreeDialog;
}
