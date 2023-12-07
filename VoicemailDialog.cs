using Rust.UI;
using UnityEngine;

public class VoicemailDialog : MonoBehaviour
{
	public GameObject RecordingRoot = null;

	public RustSlider RecordingProgress = null;

	public GameObject BrowsingRoot = null;

	public PhoneDialler ParentDialler = null;

	public GameObjectRef VoicemailEntry = null;

	public Transform VoicemailEntriesRoot = null;

	public GameObject NoVoicemailRoot = null;

	public GameObject NoCassetteRoot = null;
}
