using UnityEngine;

public class ChippyMoveTest : MonoBehaviour
{
	public Vector3 heading = new Vector3(0f, 1f, 0f);

	public float speed = 0.2f;

	public float maxSpeed = 1f;

	private void FixedUpdate()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Mathf.Abs(((Vector3)(ref heading)).magnitude) > 0f) ? 1f : 0f);
		speed = Mathf.MoveTowards(speed, maxSpeed * num, Time.fixedDeltaTime * ((num == 0f) ? 2f : 2f));
		Vector3 position = ((Component)this).transform.position;
		Vector3 val = new Vector3(heading.x, heading.y, 0f);
		Ray val2 = default(Ray);
		((Ray)(ref val2))._002Ector(position, ((Vector3)(ref val)).normalized);
		if (!Physics.Raycast(val2, speed * Time.fixedDeltaTime, 16777216))
		{
			Transform transform = ((Component)this).transform;
			transform.position += ((Ray)(ref val2)).direction * Time.fixedDeltaTime * speed;
			if (Mathf.Abs(((Vector3)(ref heading)).magnitude) > 0f)
			{
				Transform transform2 = ((Component)this).transform;
				Vector3 forward = ((Component)this).transform.forward;
				val = new Vector3(heading.x, heading.y, 0f);
				transform2.rotation = QuaternionEx.LookRotationForcedUp(forward, ((Vector3)(ref val)).normalized);
			}
		}
	}
}
