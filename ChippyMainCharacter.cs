using UnityEngine;

public class ChippyMainCharacter : SpriteArcadeEntity
{
	public float speed = 0f;

	public float maxSpeed = 0.25f;

	public ChippyBulletEntity bulletPrefab;

	public float fireRate = 0.1f;

	public Vector3 aimDir = Vector3.up;
}
