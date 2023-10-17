using System;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Rust/Tutorials/Full Screen Help Info")]
public class TutorialFullScreenHelpInfo : ScriptableObject
{
	[Serializable]
	public struct Info
	{
		public TokenisedPhrase TextToDisplay;

		public Sprite StaticImage;

		public VideoClip VideoClip;
	}

	public Info[] InfoToDisplay = new Info[3];
}
