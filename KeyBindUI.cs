using UnityEngine;
using UnityEngine.UI;

public class KeyBindUI : MonoBehaviour
{
	public GameObject blockingCanvas;

	public Text Label;

	public Button btnA;

	public Button btnB;

	public string bindString;

	public static bool IsBinding { get; private set; }
}
