using Rust.UI;
using UnityEngine;

public class RadioDialog : UIDialog
{
	public RustInput IpInput = null;

	public GameObjectRef FavouritePrefab = null;

	public Transform FavouritesContainer = null;

	public GameObject HasCassetteRoot = null;

	public static RadioDialog Instance = null;
}
