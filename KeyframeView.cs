using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

public class KeyframeView : MonoBehaviour
{
	public ScrollRect Scroller;

	public GameObjectRef KeyframePrefab;

	public RectTransform KeyframeRoot = null;

	public Transform CurrentPositionIndicator = null;

	public bool LockScrollToCurrentPosition = false;

	public RustText TrackName;
}
