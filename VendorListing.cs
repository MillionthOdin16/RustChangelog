using Facepunch.Flexbox;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

public class VendorListing : FacepunchBehaviour
{
	public Image panelBacking;

	[Header("Text")]
	public RustText AvailableText;

	public RustText CostText;

	public RustText AvailableAmount;

	public RustText CostAmount;

	public RustText InStockAmount;

	public RustText InStockText;

	public VendingPriceMultiplierWidget PriceMultiplier;

	[Header("Icons")]
	public Image AvaliableIcon;

	public Image CostIcon;

	[Header("BP Icons")]
	public Image AvaliableBPIcon;

	public Image CostBPIcon;

	[Header("Backing")]
	public Image AvaliableBPBacking;

	public Image CostBPBacking;

	[Header("Tooltips")]
	public Tooltip avaliableIconTooltip;

	public Tooltip costIconTooltip;

	[Header("Condition")]
	public CanvasGroup availableConditionGroup;

	public Image availableCondition;

	public Image availableMaxCondition;

	public GameObject brokenIcon;

	[Header("Slots")]
	public GameObject slots;

	public Image[] slotImages;

	[SerializeField]
	private FlexTransition transition;

	public static Phrase inStockPhrase = new Phrase("vendor_in_stock", "In Stock");

	public static Phrase outOfStockPhrase = new Phrase("vendor_out_stock", "Sold Out");

	public static Phrase attachmentsPhrase = new Phrase("vendor_attachments", "Attachments");

	public static Phrase ammoPhrase = new Phrase("vendor_ammo", "Ammo");
}
