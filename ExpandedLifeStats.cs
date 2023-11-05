using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpandedLifeStats : MonoBehaviour
{
	[Serializable]
	public struct GenericStatDisplay
	{
		public string statKey;

		public Sprite statSprite;

		public Phrase displayPhrase;
	}

	public GameObject DisplayRoot = null;

	public GameObjectRef GenericStatRow;

	[Header("Resources")]
	public Transform ResourcesStatRoot;

	public List<GenericStatDisplay> ResourceStats;

	[Header("Weapons")]
	public GameObjectRef WeaponStatRow;

	public Transform WeaponsRoot;

	[Header("Misc")]
	public Transform MiscRoot;

	public List<GenericStatDisplay> MiscStats;

	public LifeInfographic Infographic;

	public RectTransform MoveRoot = null;

	public Vector2 OpenPosition;

	public Vector2 ClosedPosition;

	public GameObject OpenButtonRoot = null;

	public GameObject CloseButtonRoot = null;

	public GameObject ScrollGradient = null;

	public ScrollRect Scroller = null;
}
