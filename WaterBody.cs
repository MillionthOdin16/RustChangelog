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

	public bool IsOcean;

	public FishingTag FishingType;

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
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
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
