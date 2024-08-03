using UnityEngine;

public class SksViewmodel : MonoBehaviour, IClientComponent, IViewmodelComponent, IAnimationEventReceiver
{
	public GameObject ShellRoot;

	public Animator TargetAnimator;
}
