using System;
using UnityEngine;
using UnityEngine.UI;

public class IconSkinPicker : MonoBehaviour
{
	public GameObjectRef pickerIcon;

	public GameObject container;

	public Action skinChangedEvent;

	public ScrollRect scroller = null;

	public SearchFilterInput searchFilter = null;
}
