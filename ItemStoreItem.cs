using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStoreItem : MonoBehaviour
{
	public Button Button;

	public HttpImage Icon;

	public RawImage IconImage;

	public Material IconImageDisabledMaterial;

	public RustText Name;

	public GameObject PriceButton;

	public TextMeshProUGUI Price;

	public RustText ItemName;

	public GameObject NewTag;

	public GameObject InInventoryTag;

	public RustText InInventoryText;

	public GameObject InCartTag;

	public GameObject Footer;
}
