using UnityEngine;
using UnityEngine.UI;

public class EngineItemInformationPanel : ItemInformationPanel
{
	[SerializeField]
	private Text tier;

	[SerializeField]
	private Phrase low;

	[SerializeField]
	private Phrase medium;

	[SerializeField]
	private Phrase high;

	[SerializeField]
	private GameObject accelerationRoot = null;

	[SerializeField]
	private GameObject topSpeedRoot = null;

	[SerializeField]
	private GameObject fuelEconomyRoot = null;
}
