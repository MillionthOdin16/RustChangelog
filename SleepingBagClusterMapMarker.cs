using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SleepingBagClusterMapMarker : MonoBehaviour
{
	public TextMeshProUGUI CountText = null;

	public List<SleepingBagButton> SleepingBagButtons;

	public GameObject OpenRoot = null;

	public Tooltip SummaryTooltip = null;

	public Image RimImage = null;
}
