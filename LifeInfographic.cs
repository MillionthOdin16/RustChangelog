using System;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.UI;

public class LifeInfographic : MonoBehaviour
{
	[Serializable]
	public struct DamageSetting
	{
		public DamageType ForType;

		public string Display;

		public Sprite DamageSprite;
	}

	[NonSerialized]
	public PlayerLifeStory life = null;

	public GameObject container;

	public RawImage AttackerAvatarImage = null;

	public Image DamageSourceImage = null;

	public LifeInfographicStat[] Stats;

	public Animator[] AllAnimators;

	public GameObject WeaponRoot = null;

	public GameObject DistanceRoot = null;

	public GameObject DistanceDivider = null;

	public Image WeaponImage = null;

	public DamageSetting[] DamageDisplays;

	public Texture2D defaultAvatarTexture = null;

	public bool ShowDebugData = false;
}
