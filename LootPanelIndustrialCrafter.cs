using Rust.UI;
using UnityEngine;

public class LootPanelIndustrialCrafter : LootPanel
{
	public GameObject CraftingRoot = null;

	public RustSlider ProgressSlider;

	public Transform Spinner;

	public float SpinSpeed = 90f;

	public GameObject WorkbenchLevelRoot = null;
}
