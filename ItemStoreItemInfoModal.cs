using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoreItemInfoModal : FacepunchBehaviour
{
	public HttpImage Icon;

	public TextMeshProUGUI Name;

	public TextMeshProUGUI Price;

	public TextMeshProUGUI Description;

	public RustText itemCategory;

	public RawImage skinViewerImage;

	public GameObjectRef skinViewerPrefab;

	public RectTransform sectionReplaces;

	public Image replacesIcon;

	public RustText replacesName;

	public RectTransform sectionBreakdown;

	public RectTransform breakdownCloth;

	public RectTransform breakdownMetal;

	public RectTransform breakdownWood;

	[SerializeField]
	private GameObject icon2D;

	[SerializeField]
	private GameObject icon3D;

	[SerializeField]
	private Image loadingSpinner;
}
