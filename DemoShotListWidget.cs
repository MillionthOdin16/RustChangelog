using Rust.UI;
using UnityEngine;

public class DemoShotListWidget : SingletonComponent<DemoShotListWidget>
{
	public GameObjectRef ShotListEntry;

	public GameObjectRef FolderEntry;

	public Transform ShotListParent;

	public RustInput FolderNameInput;

	public GameObject ShotsRoot = null;

	public GameObject NoShotsRoot = null;

	public GameObject TopUpArrow = null;

	public GameObject TopDownArrow = null;

	public Canvas DragCanvas = null;
}
