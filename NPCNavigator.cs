using ConVar;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavigator : BaseNavigator
{
	public int DestroyOnFailedSampleCount = 5;

	private int sampleFailCount = 0;

	public BaseNpc NPC { get; private set; }

	public override void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		base.Init(entity, agent);
		NPC = entity as BaseNpc;
		sampleFailCount = 0;
	}

	public override void OnFailedToPlaceOnNavmesh()
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		base.OnFailedToPlaceOnNavmesh();
		if ((Object)(object)SingletonComponent<DynamicNavMesh>.Instance == (Object)null || SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
		{
			return;
		}
		sampleFailCount++;
		if (DestroyOnFailedSampleCount > 0 && sampleFailCount >= DestroyOnFailedSampleCount)
		{
			Debug.LogWarning((object)string.Concat("Failed to sample navmesh ", sampleFailCount, " times in a row at: ", ((Component)this).transform.position, ". Destroying: ", ((Object)((Component)this).gameObject).name));
			if ((Object)(object)NPC != (Object)null && !NPC.IsDestroyed)
			{
				NPC.Kill();
			}
		}
	}

	public override void OnPlacedOnNavmesh()
	{
		base.OnPlacedOnNavmesh();
		sampleFailCount = 0;
	}

	protected override bool CanEnableNavMeshNavigation()
	{
		if (!base.CanEnableNavMeshNavigation())
		{
			return false;
		}
		return true;
	}

	protected override bool CanUpdateMovement()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!base.CanUpdateMovement())
		{
			return false;
		}
		if ((Object)(object)NPC != (Object)null && (NPC.IsDormant || !NPC.syncPosition) && ((Behaviour)base.Agent).enabled)
		{
			SetDestination(NPC.ServerPosition);
			return false;
		}
		return true;
	}

	protected override void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		base.UpdatePositionAndRotation(moveToPosition, delta);
		UpdateRotation(moveToPosition, delta);
	}

	private void UpdateRotation(Vector3 moveToPosition, float delta)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (overrideFacingDirectionMode != 0)
		{
			return;
		}
		if (traversingNavMeshLink)
		{
			Vector3 val = base.Agent.destination - base.BaseEntity.ServerPosition;
			float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
			if (sqrMagnitude > 1f)
			{
				val = currentNavMeshLinkEndPos - base.BaseEntity.ServerPosition;
			}
			sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
			if (!(sqrMagnitude > 0.001f))
			{
			}
			return;
		}
		Vector3 val2 = base.Agent.destination - base.BaseEntity.ServerPosition;
		if (((Vector3)(ref val2)).sqrMagnitude > 1f)
		{
			val2 = base.Agent.desiredVelocity;
			Vector3 normalized = ((Vector3)(ref val2)).normalized;
			float sqrMagnitude2 = ((Vector3)(ref normalized)).sqrMagnitude;
			if (sqrMagnitude2 > 0.001f)
			{
				base.BaseEntity.ServerRotation = Quaternion.LookRotation(normalized);
			}
		}
	}

	public override void ApplyFacingDirectionOverride()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.ApplyFacingDirectionOverride();
		base.BaseEntity.ServerRotation = Quaternion.LookRotation(base.FacingDirectionOverride);
	}

	public override bool IsSwimming()
	{
		if (!AI.npcswimming)
		{
			return false;
		}
		if ((Object)(object)NPC != (Object)null)
		{
			return NPC.swimming;
		}
		return false;
	}
}
