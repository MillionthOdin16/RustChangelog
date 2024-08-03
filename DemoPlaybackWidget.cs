using Rust.UI;
using UnityEngine;

public class DemoPlaybackWidget : MonoBehaviour
{
	public RustSlider DemoProgress;

	public RustText DemoName;

	public RustText DemoDuration;

	public RustText DemoCurrentTime;

	public GameObject PausedRoot = null;

	public GameObject PlayingRoot = null;

	public RectTransform DemoPlaybackHandle = null;

	public RectTransform ShotPlaybackWindow = null;

	public RustButton LoopButton = null;

	public GameObject ShotButtonRoot = null;

	public RustText ShotNameText = null;

	public GameObject ShotNameRoot = null;

	public RectTransform ShotRecordWindow = null;
}
