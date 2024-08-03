using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SleepingBagButton : MonoBehaviour
{
	public GameObject TimeLockRoot;

	public GameObject LockRoot;

	public GameObject OccupiedRoot = null;

	public Button ClickButton;

	public TextMeshProUGUI BagName;

	public TextMeshProUGUI LockTime;

	public Image Icon;

	public Sprite SleepingBagSprite;

	public Sprite BedSprite;

	public Sprite BeachTowelSprite;

	public Sprite CamperSprite;

	public Image CircleRim;

	public Image CircleFill;

	public Image Background;

	public RustButton DeleteButton = null;

	public Image ConfirmSlider = null;

	public static Phrase toastHoldToUnclaimBag = new Phrase("hold_unclaim_bag", "Hold down the delete button to unclaim a sleeping bag");
}
