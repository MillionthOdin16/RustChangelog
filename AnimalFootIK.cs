using UnityEngine;

public class AnimalFootIK : MonoBehaviour
{
	public Transform[] Feet;

	public Animator animator;

	public float maxWeightDistance = 0.1f;

	public float minWeightDistance = 0.025f;

	public float actualFootOffset = 0.01f;

	public bool GroundSample(Vector3 origin, out RaycastHit hit)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (Physics.Raycast(origin + Vector3.up * 0.5f, Vector3.down, ref hit, 1f, 455155969))
		{
			return true;
		}
		return false;
	}

	public void Start()
	{
	}

	public AvatarIKGoal GoalFromIndex(int index)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return (AvatarIKGoal)(index switch
		{
			0 => 2, 
			1 => 3, 
			2 => 0, 
			3 => 1, 
			_ => 2, 
		});
	}

	private void OnAnimatorIK(int layerIndex)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"animal ik!");
		for (int i = 0; i < 4; i++)
		{
			Transform val = Feet[i];
			AvatarIKGoal val2 = GoalFromIndex(i);
			Vector3 up = Vector3.up;
			Vector3 position = ((Component)val).transform.position;
			float iKPositionWeight = animator.GetIKPositionWeight(val2);
			if (GroundSample(((Component)val).transform.position - Vector3.down * actualFootOffset, out var hit))
			{
				up = ((RaycastHit)(ref hit)).normal;
				position = ((RaycastHit)(ref hit)).point;
				Vector3 val3 = ((Component)val).transform.position - Vector3.down * actualFootOffset;
				float num = Vector3.Distance(val3, position);
				iKPositionWeight = 1f - Mathf.InverseLerp(minWeightDistance, maxWeightDistance, num);
				animator.SetIKPosition(val2, position + Vector3.up * actualFootOffset);
			}
			else
			{
				iKPositionWeight = 0f;
			}
			animator.SetIKPositionWeight(val2, iKPositionWeight);
		}
	}
}
