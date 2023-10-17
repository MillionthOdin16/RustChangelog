using UnityEngine;

public class EntityFlag_Animator : EntityFlag_Toggle
{
	public enum AnimatorMode
	{
		Bool,
		Float,
		Trigger,
		Integer
	}

	public Animator TargetAnimator = null;

	public string ParamName = string.Empty;

	public AnimatorMode AnimationMode = AnimatorMode.Bool;

	public float FloatOnState;

	public float FloatOffState;

	public int IntegerOnState;

	public int IntegerOffState;
}
