using System;
using UnityEngine;

public class StartOfFrameHook : MonoBehaviour
{
	public static Action OnStartOfFrame;

	private void OnEnable()
	{
		OnStartOfFrame?.Invoke();
	}

	private void Update()
	{
		((Component)this).gameObject.SetActive(false);
		((Component)this).gameObject.SetActive(true);
	}
}
