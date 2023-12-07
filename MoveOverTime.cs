using UnityEngine;

public class MoveOverTime : MonoBehaviour
{
	[Range(-10f, 10f)]
	public float speed = 1f;

	public Vector3 position;

	public Vector3 rotation;

	public Vector3 scale;

	private void Update()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)this).transform;
		Quaternion val = ((Component)this).transform.rotation;
		transform.rotation = Quaternion.Euler(((Quaternion)(ref val)).eulerAngles + rotation * speed * Time.deltaTime);
		Transform transform2 = ((Component)this).transform;
		transform2.localScale += scale * speed * Time.deltaTime;
		Transform transform3 = ((Component)this).transform;
		transform3.localPosition += position * speed * Time.deltaTime;
	}
}
