using System;
using UnityEngine;

[ExecuteInEditMode]
public class WaterBody : MonoBehaviour
{
	[Flags]
	public enum FishingTag
	{
		MoonPool = 1,
		River = 2,
		Ocean = 4,
		Swamp = 8
	}

	public WaterBodyType Type = WaterBodyType.Lake;

	public Renderer Renderer;

	public Collider[] Triggers;

	public bool IsOcean = false;

	public FishingTag FishingType = (FishingTag)0;

	public Transform Transform { get; private set; }

	private void Awake()
	{
		Transform = ((Component)this).transform;
	}

	private void OnEnable()
	{
		WaterSystem.RegisterBody(this);
	}

	private void OnDisable()
	{
		WaterSystem.UnregisterBody(this);
	}

	public void OnOceanLevelChanged(float newLevel)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOcean || Triggers == null)
		{
			return;
		}
		Collider[] triggers = Triggers;
		foreach (Collider val in triggers)
		{
			if (!((Object)(object)val == (Object)null))
			{
				Vector3 position = ((Component)val).transform.position;
				position.y = newLevel;
				((Component)val).transform.position = position;
			}
		}
	}
}
