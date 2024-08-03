using UnityEngine;

public class AverageVelocity
{
	private Vector3 pos;

	private float time;

	private float lastEntry;

	private float averageSpeed;

	private Vector3 averageVelocity;

	public float Speed => averageSpeed;

	public Vector3 Average => averageVelocity;

	public void Record(Vector3 newPos)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.time - time;
		if (!(num < 0.1f))
		{
			if (((Vector3)(ref pos)).sqrMagnitude > 0f)
			{
				Vector3 val = newPos - pos;
				averageVelocity = val * (1f / num);
				averageSpeed = ((Vector3)(ref averageVelocity)).magnitude;
			}
			time = Time.time;
			pos = newPos;
		}
	}
}
