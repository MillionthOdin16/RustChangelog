using UnityEngine;

public class RotateObject : MonoBehaviour
{
	public float rotateSpeed_X = 1f;

	public float rotateSpeed_Y = 1f;

	public float rotateSpeed_Z = 1f;

	public bool localSpace;

	private Vector3 rotateVector;

	private void Awake()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		rotateVector = new Vector3(rotateSpeed_X, rotateSpeed_Y, rotateSpeed_Z);
	}

	private void Update()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (localSpace)
		{
			((Component)this).transform.Rotate(rotateVector * Time.deltaTime, (Space)1);
			return;
		}
		if (rotateSpeed_X != 0f)
		{
			((Component)this).transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed_X);
		}
		if (rotateSpeed_Y != 0f)
		{
			((Component)this).transform.Rotate(((Component)this).transform.forward, Time.deltaTime * rotateSpeed_Y);
		}
		if (rotateSpeed_Z != 0f)
		{
			((Component)this).transform.Rotate(((Component)this).transform.right, Time.deltaTime * rotateSpeed_Z);
		}
	}
}
