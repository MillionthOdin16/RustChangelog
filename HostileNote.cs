using UnityEngine;
using UnityEngine.UI;

public class HostileNote : MonoBehaviour, IClientComponent
{
	public CanvasGroup warnGroup;

	public CanvasGroup group;

	public CanvasGroup timerGroup;

	public CanvasGroup smallWarning;

	public Text timerText;

	public Text smallWarningText;

	public static float unhostileTime = 0f;

	public static float weaponDrawnDuration = 0f;

	public Color warnColor;

	public Color hostileColor;

	public float requireDistanceToSafeZone = 200f;
}
