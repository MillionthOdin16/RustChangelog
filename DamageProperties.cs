using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Damage Properties")]
public class DamageProperties : ScriptableObject
{
	[Serializable]
	public class HitAreaProperty
	{
		public HitArea area = HitArea.Head;

		public float damage = 1f;
	}

	public DamageProperties fallback = null;

	[Horizontal(1, 0)]
	public HitAreaProperty[] bones;

	public float GetMultiplier(HitArea area)
	{
		for (int i = 0; i < bones.Length; i++)
		{
			HitAreaProperty hitAreaProperty = bones[i];
			if (hitAreaProperty.area == area)
			{
				return hitAreaProperty.damage;
			}
		}
		return Object.op_Implicit((Object)(object)fallback) ? fallback.GetMultiplier(area) : 1f;
	}

	public void ScaleDamage(HitInfo info)
	{
		HitArea boneArea = info.boneArea;
		if (boneArea != (HitArea)(-1) && boneArea != 0)
		{
			info.damageTypes.ScaleAll(GetMultiplier(boneArea));
		}
	}
}
