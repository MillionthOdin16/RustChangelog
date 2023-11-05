using UnityEngine;

public class JiggleBone : BaseMonoBehaviour
{
	public bool debugMode = true;

	private Vector3 targetPos;

	private Vector3 dynamicPos;

	public Vector3 boneAxis = new Vector3(0f, 0f, 1f);

	public float targetDistance = 2f;

	public float bStiffness = 0.1f;

	public float bMass = 0.9f;

	public float bDamping = 0.75f;

	public float bGravity = 0.75f;

	private Vector3 force = default(Vector3);

	private Vector3 acc = default(Vector3);

	private Vector3 vel = default(Vector3);

	public bool SquashAndStretch = true;

	public float sideStretch = 0.15f;

	public float frontStretch = 0.2f;

	public float disableDistance = 20f;

	private void Awake()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position + ((Component)this).transform.TransformDirection(new Vector3(boneAxis.x * targetDistance, boneAxis.y * targetDistance, boneAxis.z * targetDistance));
		dynamicPos = val;
	}

	private void LateUpdate()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.rotation = default(Quaternion);
		Vector3 val = ((Component)this).transform.TransformDirection(new Vector3(boneAxis.x * targetDistance, boneAxis.y * targetDistance, boneAxis.z * targetDistance));
		Vector3 val2 = ((Component)this).transform.TransformDirection(new Vector3(0f, 1f, 0f));
		Vector3 val3 = ((Component)this).transform.position + ((Component)this).transform.TransformDirection(new Vector3(boneAxis.x * targetDistance, boneAxis.y * targetDistance, boneAxis.z * targetDistance));
		force.x = (val3.x - dynamicPos.x) * bStiffness;
		acc.x = force.x / bMass;
		vel.x += acc.x * (1f - bDamping);
		force.y = (val3.y - dynamicPos.y) * bStiffness;
		force.y -= bGravity / 10f;
		acc.y = force.y / bMass;
		vel.y += acc.y * (1f - bDamping);
		force.z = (val3.z - dynamicPos.z) * bStiffness;
		acc.z = force.z / bMass;
		vel.z += acc.z * (1f - bDamping);
		dynamicPos += vel + force;
		((Component)this).transform.LookAt(dynamicPos, val2);
		if (SquashAndStretch)
		{
			Vector3 val4 = dynamicPos - val3;
			float magnitude = ((Vector3)(ref val4)).magnitude;
			float num = ((boneAxis.x != 0f) ? (1f + magnitude * frontStretch) : (1f + (0f - magnitude) * sideStretch));
			float num2 = ((boneAxis.y != 0f) ? (1f + magnitude * frontStretch) : (1f + (0f - magnitude) * sideStretch));
			float num3 = ((boneAxis.z != 0f) ? (1f + magnitude * frontStretch) : (1f + (0f - magnitude) * sideStretch));
			((Component)this).transform.localScale = new Vector3(num, num2, num3);
		}
		if (debugMode)
		{
			Debug.DrawRay(((Component)this).transform.position, val, Color.blue);
			Debug.DrawRay(((Component)this).transform.position, val2, Color.green);
			Debug.DrawRay(val3, Vector3.up * 0.2f, Color.yellow);
			Debug.DrawRay(dynamicPos, Vector3.up * 0.2f, Color.red);
		}
	}
}
