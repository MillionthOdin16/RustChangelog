using UnityEngine;

public class LightGroupAtTime : FacepunchBehaviour
{
	public float IntensityOverride = 1f;

	public AnimationCurve IntensityScaleOverTime;

	public Transform SearchRoot;

	[Header("Power Settings")]
	public bool requiresPower;

	[Tooltip("Can NOT be entity, use new blank gameobject!")]
	public Transform powerOverrideTransform;

	public LayerMask checkLayers;

	public GameObject enableWhenLightsOn;

	public float timeBetweenPowerLookup;

	public LightGroupAtTime()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		AnimationCurve val = new AnimationCurve();
		val.keys = (Keyframe[])(object)new Keyframe[5]
		{
			new Keyframe(0f, 1f),
			new Keyframe(8f, 0f),
			new Keyframe(12f, 0f),
			new Keyframe(19f, 1f),
			new Keyframe(24f, 1f)
		};
		IntensityScaleOverTime = val;
		checkLayers = LayerMask.op_Implicit(1235288065);
		timeBetweenPowerLookup = 10f;
		((FacepunchBehaviour)this)._002Ector();
	}
}
