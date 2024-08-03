using UnityEngine;
using UnityEngine.Events;

public class ItemStoreBuyButton : ListComponent<ItemStoreBuyButton>
{
	public RectTransform Buy;

	public RectTransform InCart;

	public RectTransform AlreadyPurchased;

	public UnityEvent OnAddedToCart;
}
