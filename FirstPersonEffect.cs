using UnityEngine;

public class FirstPersonEffect : MonoBehaviour, IEffect
{
	public bool isGunShot = false;

	[HideInInspector]
	public EffectParentToWeaponBone parentToWeaponComponent;
}
