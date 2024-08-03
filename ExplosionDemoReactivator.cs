using UnityEngine;

public class ExplosionDemoReactivator : MonoBehaviour
{
	public float TimeDelayToReactivate = 3f;

	private void Start()
	{
		((MonoBehaviour)this).InvokeRepeating("Reactivate", 0f, TimeDelayToReactivate);
	}

	private void Reactivate()
	{
		Transform[] componentsInChildren = ((Component)this).GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform val in array)
		{
			((Component)val).gameObject.SetActive(false);
			((Component)val).gameObject.SetActive(true);
		}
	}
}
