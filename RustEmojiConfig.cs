using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Emoji Config")]
public class RustEmojiConfig : ScriptableObject
{
	public bool Hide = false;

	public RustEmojiLibrary.EmojiSource Source;
}
