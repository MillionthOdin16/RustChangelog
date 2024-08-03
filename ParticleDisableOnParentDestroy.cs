using UnityEngine;

public class ParticleDisableOnParentDestroy : MonoBehaviour, IOnParentDestroying
{
	public float destroyAfterSeconds = 0f;

	public void OnParentDestroying()
	{
		((Component)this).transform.parent = null;
		((Component)this).GetComponent<ParticleSystem>().enableEmission = false;
		if (destroyAfterSeconds > 0f)
		{
			GameManager.Destroy(((Component)this).gameObject, destroyAfterSeconds);
		}
	}
}
