using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIRecordingInfo : SingletonComponent<UIRecordingInfo>
{
	public RustText CountdownText = null;

	public Slider TapeProgressSlider = null;

	public GameObject CountdownRoot;

	public GameObject RecordingRoot;

	public Transform Spinner = null;

	public float SpinSpeed = 180f;

	public Image CassetteImage = null;

	private void Start()
	{
		((Component)this).gameObject.SetActive(false);
	}
}
