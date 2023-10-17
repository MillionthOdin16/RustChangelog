using UnityEngine;

public class RotateCameraAroundObject : MonoBehaviour
{
	public GameObject m_goObjectToRotateAround;

	public float m_flRotateSpeed = 10f;

	private void FixedUpdate()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_goObjectToRotateAround != (Object)null)
		{
			((Component)this).transform.LookAt(m_goObjectToRotateAround.transform.position + Vector3.up * 0.75f);
			((Component)this).transform.Translate(Vector3.right * m_flRotateSpeed * Time.deltaTime);
		}
	}
}
