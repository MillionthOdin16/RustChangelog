using Rust.UI;
using UnityEngine;

public class MissionsHUD : SingletonComponent<MissionsHUD>
{
	public SoundDefinition listComplete;

	public SoundDefinition itemComplete;

	public SoundDefinition popup;

	public Canvas Canvas;

	public RustText titleText;

	public GameObject timerObject;

	public RustText timerText;

	public GameObject tutorialObject;

	public RustText tutorialText;

	public TokenisedPhrase tutorialTextPhrase;
}
