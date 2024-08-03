using Rust.UI;
using UnityEngine;

public class DemoShotRecordWidget : MonoBehaviour
{
	public RustInput NameInput;

	public GameObject RecordingRoot = null;

	public GameObject PreRecordingRoot = null;

	public RustButton CountdownToggle = null;

	public RustButton PauseOnSaveToggle = null;

	public RustButton ReturnToStartToggle = null;

	public RustButton RecordDofToggle = null;

	public RustOption FolderDropdown = null;

	public GameObject RecordingUnderlay = null;

	public AudioSource CountdownAudio = null;

	public GameObject ShotRecordTime = null;

	public RustText ShotRecordTimeText = null;

	public RustText ShotNameText = null;

	public GameObject RecordingInProcessRoot = null;

	public GameObject CountdownActiveRoot = null;

	public GameObject CountdownActiveSliderRoot = null;

	public RustSlider CountdownActiveSlider = null;

	public RustText CountdownActiveText = null;
}
