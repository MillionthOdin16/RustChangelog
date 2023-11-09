using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
	public Collider collider;

	protected void OnTriggerEnter(Collider other)
	{
		Debug.Log((object)("IgnoreCollision: " + ((Object)((Component)collider).gameObject).name + " + " + ((Object)((Component)other).gameObject).name));
		Physics.IgnoreCollision(other, collider, true);
	}
}
