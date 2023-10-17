using UnityEngine;

public class MoveForward : MonoBehaviour
{
	public float Speed = 2f;

	protected void Update()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).GetComponent<Rigidbody>().velocity = Speed * ((Component)this).transform.forward;
	}
}
