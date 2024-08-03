using UnityEngine;

public class PingPongRotate : MonoBehaviour
{
	public Vector3 rotationSpeed = Vector3.zero;

	public Vector3 offset = Vector3.zero;

	public Vector3 rotationAmount = Vector3.zero;

	private void Update()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.identity;
		for (int i = 0; i < 3; i++)
		{
			val *= GetRotation(i);
		}
		((Component)this).transform.rotation = val;
	}

	public Quaternion GetRotation(int index)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.zero;
		switch (index)
		{
		case 0:
			val = Vector3.right;
			break;
		case 1:
			val = Vector3.up;
			break;
		case 2:
			val = Vector3.forward;
			break;
		}
		return Quaternion.AngleAxis(Mathf.Sin((((Vector3)(ref offset))[index] + Time.time) * ((Vector3)(ref rotationSpeed))[index]) * ((Vector3)(ref rotationAmount))[index], val);
	}
}
