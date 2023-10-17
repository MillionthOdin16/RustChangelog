using ProtoBuf;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SleepingBagButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public GameObject TimeLockRoot;

	public GameObject LockRoot;

	public GameObject OccupiedRoot;

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

	public GameObject DeleteButton;

	internal SpawnOptions spawnOption;

	internal float releaseTime;

	public float timerSeconds => Mathf.Clamp(releaseTime - Time.realtimeSinceStartup, 0f, 216000f);

	public string friendlyName
	{
		get
		{
			if (spawnOption == null || string.IsNullOrEmpty(spawnOption.name))
			{
				return "Null Sleeping Bag";
			}
			return spawnOption.name;
		}
	}

	private void OnEnable()
	{
		if ((Object)(object)DeleteButton != (Object)null)
		{
			DeleteButton.SetActive(false);
		}
	}

	public void Setup(SpawnOptions option, UIDeathScreen.RespawnColourScheme colourScheme)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected I4, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		spawnOption = option;
		RespawnType type = option.type;
		switch (type - 1)
		{
		case 0:
			Icon.sprite = SleepingBagSprite;
			break;
		case 1:
			Icon.sprite = BedSprite;
			break;
		case 2:
			Icon.sprite = BeachTowelSprite;
			break;
		case 3:
			Icon.sprite = CamperSprite;
			break;
		}
		((Graphic)Background).color = colourScheme.BackgroundColour;
		((Graphic)CircleFill).color = colourScheme.CircleFillColour;
		((Graphic)CircleRim).color = colourScheme.CircleRimColour;
		releaseTime = ((option.unlockSeconds > 0f) ? (Time.realtimeSinceStartup + option.unlockSeconds) : 0f);
		UpdateButtonState(option);
		((TMP_Text)BagName).text = friendlyName;
	}

	private void UpdateButtonState(SpawnOptions option)
	{
		bool flag = releaseTime > 0f && releaseTime > Time.realtimeSinceStartup;
		bool occupied = option.occupied;
		LockRoot.SetActive(flag);
		OccupiedRoot.SetActive(occupied);
		TimeLockRoot.SetActive(flag);
		((Selectable)ClickButton).interactable = !occupied && !flag;
	}

	public void Update()
	{
		if (releaseTime != 0f)
		{
			if (releaseTime < Time.realtimeSinceStartup)
			{
				UpdateButtonState(spawnOption);
			}
			else
			{
				((TMP_Text)LockTime).text = timerSeconds.ToString("N0");
			}
		}
	}

	public void DoSpawn()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (spawnOption != null)
		{
			ConsoleSystem.Run(Option.Client, "respawn_sleepingbag", new object[1] { spawnOption.id });
		}
	}

	public void DeleteBag()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (spawnOption != null)
		{
			ConsoleSystem.Run(Option.Client, "respawn_sleepingbag_remove", new object[1] { spawnOption.id });
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if ((Object)(object)DeleteButton != (Object)null)
		{
			DeleteButton.SetActive(true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if ((Object)(object)DeleteButton != (Object)null)
		{
			DeleteButton.SetActive(false);
		}
	}
}
