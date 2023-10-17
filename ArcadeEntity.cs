using System;
using UnityEngine;

public class ArcadeEntity : BaseMonoBehaviour
{
	public uint id = 0u;

	public uint spriteID = 0u;

	public uint soundID = 0u;

	public bool visible = false;

	public Vector3 heading = new Vector3(0f, 1f, 0f);

	public bool isEnabled = false;

	public bool dirty = false;

	public float alpha = 1f;

	public BoxCollider boxCollider;

	public bool host = false;

	public bool localAuthorativeOverride = false;

	public ArcadeEntity arcadeEntityParent;

	public uint prefabID = 0u;

	[Header("Health")]
	public bool takesDamage = false;

	public float health = 1f;

	public float maxHealth = 1f;

	[NonSerialized]
	public bool mapLoadedEntiy = false;
}
