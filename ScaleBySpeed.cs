using UnityEngine;

public class ScaleBySpeed : MonoBehaviour
{
	public float minScale = 0.001f;

	public float maxScale = 1f;

	public float minSpeed = 0f;

	public float maxSpeed = 1f;

	public MonoBehaviour component;

	public bool toggleComponent = true;

	public bool onlyWhenSubmerged = false;

	public float submergedThickness = 0.33f;

	private Vector3 prevPosition = Vector3.zero;

	private void Start()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		prevPosition = ((Component)this).transform.position;
	}

	private void Update()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		Vector3 val = position - prevPosition;
		float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
		float num = minScale;
		float height = WaterSystem.GetHeight(position);
		bool enabled = height > position.y - submergedThickness;
		if (sqrMagnitude > 0.0001f)
		{
			sqrMagnitude = Mathf.Sqrt(sqrMagnitude) / Time.deltaTime;
			float num2 = Mathf.Clamp(sqrMagnitude, minSpeed, maxSpeed) / (maxSpeed - minSpeed);
			num = Mathf.Lerp(minScale, maxScale, Mathf.Clamp01(num2));
			if ((Object)(object)component != (Object)null && toggleComponent)
			{
				((Behaviour)component).enabled = enabled;
			}
		}
		else if ((Object)(object)component != (Object)null && toggleComponent)
		{
			((Behaviour)component).enabled = false;
		}
		((Component)this).transform.localScale = new Vector3(num, num, num);
		prevPosition = position;
	}
}
