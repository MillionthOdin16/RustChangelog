using Rust.UI;
using UnityEngine;

public class EmojiGallery : MonoBehaviour
{
	public GameObjectRef EmojiPrefab;

	public Transform Parent;

	public RustEmojiLibrary Library;

	public GameObject HighlightRoot = null;

	public RustText HighlightText = null;

	public EmojiController SkinIndicator = null;

	public EmojiController[] SkinToneGallery;

	public RustEmojiConfig SkinDemoConfig;

	public GameObject SkinPickerRoot = null;

	public TmProEmojiInputField TargetInputField;
}
