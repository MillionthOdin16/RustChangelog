using UnityEngine;

public class MagnetSnap
{
	private Transform snapLocation;

	private Vector3 prevSnapLocation;

	public MagnetSnap(Transform snapLocation)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		this.snapLocation = snapLocation;
		prevSnapLocation = snapLocation.position;
	}

	public void FixedUpdate(Transform target)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		PositionTarget(target);
		if (snapLocation.hasChanged)
		{
			prevSnapLocation = snapLocation.position;
			snapLocation.hasChanged = false;
		}
	}

	public void PositionTarget(Transform target)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)target == (Object)null))
		{
			Transform transform = ((Component)target).transform;
			Quaternion val = snapLocation.rotation;
			float num = Vector3.Angle(transform.forward, snapLocation.forward);
			if (num > 90f)
			{
				val *= Quaternion.Euler(0f, 180f, 0f);
			}
			if (transform.position != snapLocation.position)
			{
				transform.position += snapLocation.position - prevSnapLocation;
				transform.position = Vector3.MoveTowards(transform.position, snapLocation.position, 1f * Time.fixedDeltaTime);
			}
			if (transform.rotation != val)
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation, val, 40f * Time.fixedDeltaTime);
			}
		}
	}
}
