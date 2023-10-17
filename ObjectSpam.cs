using UnityEngine;

public class ObjectSpam : MonoBehaviour
{
	public GameObject source;

	public int amount = 1000;

	public float radius;

	private void Start()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < amount; i++)
		{
			GameObject val = Object.Instantiate<GameObject>(source);
			val.transform.position = ((Component)this).transform.position + Vector3Ex.Range(0f - radius, radius);
			((Object)val).hideFlags = (HideFlags)3;
		}
	}
}
