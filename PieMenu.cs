using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PieMenu : UIBehaviour
{
	[Serializable]
	public class MenuOption
	{
		public struct ColorMode
		{
			public enum PieMenuSpriteColorOption
			{
				CustomColor,
				SpriteColor
			}

			public PieMenuSpriteColorOption Mode;

			public Color CustomColor;
		}

		public string name;

		public string desc;

		public string requirements;

		public Sprite sprite;

		public bool disabled;

		public int order;

		public ColorMode? overrideColorMode;

		public bool showOverlay;

		public float time;

		[NonSerialized]
		public Action<BasePlayer> action;

		[NonSerialized]
		public Action<BasePlayer> actionPrev;

		[NonSerialized]
		public Action<BasePlayer> actionNext;

		[NonSerialized]
		public PieOption option;

		[NonSerialized]
		public bool selected;

		[NonSerialized]
		public bool allowMerge;

		[NonSerialized]
		public bool wantsMerge;
	}

	public static PieMenu Instance;

	public Image middleBox;

	public PieShape pieBackgroundBlur;

	public PieShape pieBackground;

	public PieShape pieSelection;

	public GameObject pieOptionPrefab;

	public GameObject optionsCanvas;

	public MenuOption[] options;

	public GameObject scaleTarget;

	public GameObject arrowLeft;

	public GameObject arrowRight;

	public float sliceGaps = 10f;

	[Range(0f, 1f)]
	public float outerSize = 1f;

	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	[Range(0f, 1f)]
	public float iconSize = 0.8f;

	[Range(0f, 360f)]
	public float startRadius;

	[Range(0f, 360f)]
	public float radiusSize = 360f;

	public Image middleImage;

	public TextMeshProUGUI middleTitle;

	public TextMeshProUGUI middleDesc;

	public TextMeshProUGUI middleRequired;

	public Color colorIconActive;

	public Color colorIconHovered;

	public Color colorIconDisabled;

	public Color colorBackgroundDisabled;

	public SoundDefinition clipOpen;

	public SoundDefinition clipCancel;

	public SoundDefinition clipChanged;

	public SoundDefinition clipSelected;

	public MenuOption defaultOption;

	private bool isClosing;

	private CanvasGroup canvasGroup;

	public Material IconMaterial;

	internal MenuOption selectedOption;

	private static Color pieSelectionColor = new Color(0.804f, 0.255f, 0.169f, 1f);

	private static Color middleImageColor = new Color(0.804f, 0.255f, 0.169f, 0.784f);

	private MenuOption longHoldOption;

	private static AnimationCurve easePunch = new AnimationCurve((Keyframe[])(object)new Keyframe[9]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.112586f, 0.9976035f),
		new Keyframe(0.3120486f, 0.01720615f),
		new Keyframe(0.4316337f, 0.17030682f),
		new Keyframe(0.5524869f, 0.03141804f),
		new Keyframe(0.6549395f, 0.002909959f),
		new Keyframe(0.770987f, 0.009817753f),
		new Keyframe(0.8838775f, 0.001939224f),
		new Keyframe(1f, 0f)
	});

	public bool IsOpen { get; private set; }

	protected override void Start()
	{
		((UIBehaviour)this).Start();
		Instance = this;
		canvasGroup = ((Component)this).GetComponentInChildren<CanvasGroup>();
		canvasGroup.alpha = 0f;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
		IsOpen = false;
		isClosing = true;
		((Component)this).gameObject.SetChildComponentsEnabled<TMP_Text>(enabled: false);
	}

	public void Clear()
	{
		options = new MenuOption[0];
	}

	public void AddOption(MenuOption option)
	{
		List<MenuOption> list = options.ToList();
		list.Add(option);
		options = list.ToArray();
	}

	public void FinishAndOpen()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		longHoldOption = null;
		IsOpen = true;
		isClosing = false;
		SetDefaultOption();
		Rebuild();
		UpdateInteraction(allowLerp: false);
		PlayOpenSound();
		LeanTween.cancel(((Component)this).gameObject);
		LeanTween.cancel(scaleTarget);
		((Component)this).GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(((Component)this).GetComponent<CanvasGroup>(), 1f, 0.1f).setEase((LeanTweenType)21);
		scaleTarget.transform.localScale = Vector3.one * 1.5f;
		LeanTween.scale(scaleTarget, Vector3.one, 0.1f).setEase((LeanTweenType)24);
		((Component)Instance).gameObject.SetChildComponentsEnabled<TMP_Text>(enabled: true);
	}

	protected override void OnEnable()
	{
		((UIBehaviour)this).OnEnable();
		Rebuild();
	}

	public void SetDefaultOption()
	{
		defaultOption = null;
		for (int i = 0; i < options.Length; i++)
		{
			if (!options[i].disabled)
			{
				if (defaultOption == null)
				{
					defaultOption = options[i];
				}
				if (options[i].selected)
				{
					defaultOption = options[i];
					break;
				}
			}
		}
	}

	public void PlayOpenSound()
	{
	}

	public void PlayCancelSound()
	{
	}

	public void Close(bool success = false)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (!isClosing)
		{
			if (!success)
			{
				longHoldOption = null;
			}
			isClosing = true;
			NeedsCursor component = ((Component)this).GetComponent<NeedsCursor>();
			if ((Object)(object)component != (Object)null)
			{
				((Behaviour)component).enabled = false;
			}
			LeanTween.cancel(((Component)this).gameObject);
			LeanTween.cancel(scaleTarget);
			LeanTween.alphaCanvas(((Component)this).GetComponent<CanvasGroup>(), 0f, 0.2f).setEase((LeanTweenType)21);
			LeanTween.scale(scaleTarget, Vector3.one * (success ? 1.5f : 0.5f), 0.2f).setEase((LeanTweenType)21);
			((Component)Instance).gameObject.SetChildComponentsEnabled<TMP_Text>(enabled: false);
			IsOpen = false;
			selectedOption = null;
		}
	}

	private void Update()
	{
		if (pieBackground.innerSize != innerSize || pieBackground.outerSize != outerSize || pieBackground.startRadius != startRadius || pieBackground.endRadius != startRadius + radiusSize)
		{
			pieBackground.startRadius = startRadius;
			pieBackground.endRadius = startRadius + radiusSize;
			pieBackground.innerSize = innerSize;
			pieBackground.outerSize = outerSize;
			((Graphic)pieBackground).SetVerticesDirty();
		}
		UpdateInteraction();
		if (IsOpen)
		{
			CursorManager.HoldOpen();
			IngameMenuBackground.Enabled = true;
		}
	}

	public void Rebuild()
	{
		options = options.OrderBy((MenuOption x) => x.order).ToArray();
		while (optionsCanvas.transform.childCount > 0)
		{
			if (Application.isPlaying)
			{
				GameManager.DestroyImmediate(((Component)optionsCanvas.transform.GetChild(0)).gameObject, allowDestroyingAssets: true);
			}
			else
			{
				Object.DestroyImmediate((Object)(object)((Component)optionsCanvas.transform.GetChild(0)).gameObject);
			}
		}
		if (options.Length != 0)
		{
			for (int i = 0; i < options.Length; i++)
			{
				bool flag = false;
				if (options[i].allowMerge)
				{
					if (i > 0)
					{
						flag |= options[i].order == options[i - 1].order;
					}
					if (i < options.Length - 1)
					{
						flag |= options[i].order == options[i + 1].order;
					}
				}
				options[i].wantsMerge = flag;
			}
			int num = options.Length;
			int num2 = options.Where((MenuOption x) => x.wantsMerge).Count();
			int num3 = num - num2;
			int num4 = num3 + num2 / 2;
			float num5 = radiusSize / (float)num * 0.75f;
			float num6 = (radiusSize - num5 * (float)num2) / (float)num3;
			float num7 = startRadius - radiusSize / (float)num4 * 0.25f;
			for (int j = 0; j < options.Length; j++)
			{
				float num8 = (options[j].wantsMerge ? 0.8f : 1f);
				float num9 = (options[j].wantsMerge ? num5 : num6);
				GameObject val = Instantiate.GameObject(pieOptionPrefab, (Transform)null);
				val.transform.SetParent(optionsCanvas.transform, false);
				options[j].option = val.GetComponent<PieOption>();
				options[j].option.UpdateOption(num7, num9, sliceGaps, options[j].name, outerSize, innerSize, num8 * iconSize, options[j].sprite, options[j].showOverlay);
				((Graphic)options[j].option.imageIcon).material = ((options[j].overrideColorMode.HasValue && options[j].overrideColorMode.Value.Mode == MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor) ? null : IconMaterial);
				num7 += num9;
			}
		}
		selectedOption = null;
	}

	public void UpdateInteraction(bool allowLerp = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_061b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		if (isClosing)
		{
			return;
		}
		Vector3 val = Input.mousePosition - new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
		float num = Mathf.Atan2(val.x, val.y) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		for (int i = 0; i < options.Length; i++)
		{
			float midRadius = options[i].option.midRadius;
			float sliceSize = options[i].option.sliceSize;
			if ((((Vector3)(ref val)).magnitude < 32f && options[i] == defaultOption) || (((Vector3)(ref val)).magnitude >= 32f && Mathf.Abs(Mathf.DeltaAngle(num, midRadius)) < sliceSize * 0.5f))
			{
				if (allowLerp)
				{
					pieSelection.startRadius = Mathf.MoveTowardsAngle(pieSelection.startRadius, options[i].option.background.startRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(pieSelection.startRadius, options[i].option.background.startRadius) * 30f + 10f));
					pieSelection.endRadius = Mathf.MoveTowardsAngle(pieSelection.endRadius, options[i].option.background.endRadius, Time.deltaTime * Mathf.Abs(Mathf.DeltaAngle(pieSelection.endRadius, options[i].option.background.endRadius) * 30f + 10f));
				}
				else
				{
					pieSelection.startRadius = options[i].option.background.startRadius;
					pieSelection.endRadius = options[i].option.background.endRadius;
				}
				((Graphic)middleImage).material = IconMaterial;
				if (options[i].overrideColorMode.HasValue)
				{
					if (options[i].overrideColorMode.Value.Mode == MenuOption.ColorMode.PieMenuSpriteColorOption.CustomColor)
					{
						Color customColor = options[i].overrideColorMode.Value.CustomColor;
						((Graphic)pieSelection).color = customColor;
						customColor.a = middleImageColor.a;
						((Graphic)middleImage).color = customColor;
					}
					else if (options[i].overrideColorMode.Value.Mode == MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor)
					{
						((Graphic)pieSelection).color = pieSelectionColor;
						((Graphic)middleImage).color = Color.white;
						((Graphic)middleImage).material = null;
					}
				}
				else
				{
					((Graphic)pieSelection).color = pieSelectionColor;
					((Graphic)middleImage).color = middleImageColor;
				}
				((Graphic)pieSelection).SetVerticesDirty();
				middleImage.sprite = options[i].sprite;
				((TMP_Text)middleTitle).text = options[i].name;
				((TMP_Text)middleDesc).text = options[i].desc;
				((TMP_Text)middleRequired).text = "";
				Button buttonObjectWithBind = Input.GetButtonObjectWithBind("+prevskin");
				if (options[i].actionPrev != null && buttonObjectWithBind != null && (int)buttonObjectWithBind.Code != 0)
				{
					arrowLeft.SetActive(true);
					((TMP_Text)arrowLeft.GetComponentInChildren<TextMeshProUGUI>()).text = KeyCodeEx.ToShortname(buttonObjectWithBind.Code, true);
				}
				else
				{
					arrowLeft.SetActive(false);
				}
				Button buttonObjectWithBind2 = Input.GetButtonObjectWithBind("+nextskin");
				if (options[i].actionNext != null && buttonObjectWithBind2 != null && (int)buttonObjectWithBind2.Code != 0)
				{
					arrowRight.SetActive(true);
					((TMP_Text)arrowRight.GetComponentInChildren<TextMeshProUGUI>()).text = KeyCodeEx.ToShortname(buttonObjectWithBind2.Code, true);
				}
				else
				{
					arrowRight.SetActive(false);
				}
				string requirements = options[i].requirements;
				if (requirements != null)
				{
					requirements = requirements.Replace("[e]", "<color=#CD412B>");
					requirements = requirements.Replace("[/e]", "</color>");
					((TMP_Text)middleRequired).text = requirements;
				}
				if (!options[i].showOverlay)
				{
					((Graphic)options[i].option.imageIcon).color = colorIconHovered;
				}
				if (selectedOption != options[i])
				{
					if (selectedOption != null && !options[i].disabled)
					{
						scaleTarget.transform.localScale = Vector3.one;
						LeanTween.scale(scaleTarget, Vector3.one * 1.03f, 0.2f).setEase(easePunch);
					}
					if (selectedOption != null)
					{
						selectedOption.option.imageIcon.RebuildHackUnity2019();
					}
					selectedOption = options[i];
					if (selectedOption != null)
					{
						selectedOption.option.imageIcon.RebuildHackUnity2019();
					}
				}
			}
			else
			{
				((Graphic)options[i].option.imageIcon).material = IconMaterial;
				if (options[i].overrideColorMode.HasValue)
				{
					if (options[i].overrideColorMode.Value.Mode == MenuOption.ColorMode.PieMenuSpriteColorOption.CustomColor)
					{
						((Graphic)options[i].option.imageIcon).color = options[i].overrideColorMode.Value.CustomColor;
					}
					else if (options[i].overrideColorMode.Value.Mode == MenuOption.ColorMode.PieMenuSpriteColorOption.SpriteColor)
					{
						((Graphic)options[i].option.imageIcon).color = Color.white;
						((Graphic)options[i].option.imageIcon).material = null;
					}
				}
				else
				{
					((Graphic)options[i].option.imageIcon).color = colorIconActive;
				}
			}
			if (options[i].disabled)
			{
				((Graphic)options[i].option.imageIcon).color = colorIconDisabled;
				((Graphic)options[i].option.background).color = colorBackgroundDisabled;
			}
		}
	}

	public bool DoSelect()
	{
		return true;
	}

	public void RunLongHoldAction(BasePlayer player)
	{
		if (longHoldOption != null)
		{
			longHoldOption.action(player);
			longHoldOption = null;
		}
	}

	public void DoPrev()
	{
	}

	public void DoNext()
	{
	}
}
