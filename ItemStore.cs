using System.Collections.Generic;
using Facepunch;
using Rust.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemStore : SingletonComponent<ItemStore>, VirtualScroll.IDataSource
{
	public static readonly Phrase CartEmptyPhrase = new Phrase("store.cart.empty", "Cart");

	public static readonly Phrase CartSingularPhrase = new Phrase("store.cart.singular", "1 item");

	public static readonly Phrase CartPluralPhrase = new Phrase("store.cart.plural", "{amount} items");

	public GameObject ItemPrefab;

	[FormerlySerializedAs("ItemParent")]
	public RectTransform LimitedItemParent;

	public RectTransform GeneralItemParent;

	public List<IPlayerItemDefinition> Cart = new List<IPlayerItemDefinition>();

	public ItemStoreItemInfoModal ItemStoreInfoModal;

	public GameObject BuyingModal;

	public ItemStoreBuyFailedModal ItemStoreBuyFailedModal;

	public ItemStoreBuySuccessModal ItemStoreBuySuccessModal;

	public SoundDefinition AddToCartSound;

	public RustText CartButtonLabel;

	public RustText QuantityValue;

	public RustText TotalValue;

	public int GetItemCount()
	{
		return Cart.Count;
	}

	public void SetItemData(int i, GameObject obj)
	{
		obj.GetComponent<ItemStoreCartItem>().Init(i, Cart[i]);
	}
}
