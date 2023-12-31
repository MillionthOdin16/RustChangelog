using System;
using Rust.UI;
using TMPro;
using UnityEngine;

public class ItemStoreItemInfoModal : MonoBehaviour
{
	public HttpImage Icon;

	public TextMeshProUGUI Name;

	public TextMeshProUGUI Price;

	public TextMeshProUGUI Description;

	private IPlayerItemDefinition item;

	public void Show(IPlayerItemDefinition item)
	{
		this.item = item;
		Icon.Load(item.IconUrl);
		((TMP_Text)Name).text = item.Name;
		((TMP_Text)Description).text = StringExtensions.BBCodeToUnity(item.Description);
		((TMP_Text)Price).text = item.LocalPriceFormatted;
		((Component)this).gameObject.SetActive(true);
		((Component)this).GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(((Component)this).GetComponent<CanvasGroup>(), 1f, 0.1f);
	}

	public void Hide()
	{
		LeanTween.alphaCanvas(((Component)this).GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete((Action)delegate
		{
			((Component)this).gameObject.SetActive(false);
		});
	}
}
