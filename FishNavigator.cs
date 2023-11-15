using UnityEngine;
using UnityEngine.AI;

public class FishNavigator : BaseNavigator
{
	public BaseNpc NPC { get; private set; }

	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		NPC = entity as BaseNpc;
	}

	protected override bool SetCustomDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!base.SetCustomDestination(pos, speedFraction, updateInterval))
		{
			return false;
		}
		base.Destination = pos;
		return true;
	}

	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, moveToPosition, GetTargetSpeed() * delta);
		base.BaseEntity.ServerPosition = ((Component)this).transform.localPosition;
		if (ReachedPosition(moveToPosition))
		{
			Stop();
		}
		else
		{
			UpdateRotation(moveToPosition, delta);
		}
	}

	private void UpdateRotation(Vector3 moveToPosition, float delta)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		base.BaseEntity.ServerRotation = Quaternion.LookRotation(Vector3Ex.Direction(moveToPosition, ((Component)this).transform.position));
	}
}
