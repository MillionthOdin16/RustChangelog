using UnityEngine;

public class PathSpeedZone : MonoBehaviour, IAIPathSpeedZone
{
	public Bounds bounds;

	public OBB obbBounds;

	public float maxVelocityPerSec = 5f;

	public OBB WorldSpaceBounds()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new OBB(((Component)this).transform.position, ((Component)this).transform.lossyScale, ((Component)this).transform.rotation, bounds);
	}

	public float GetMaxSpeed()
	{
		return maxVelocityPerSec;
	}

	public virtual void OnDrawGizmosSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0.5f, 0f, 0.5f);
		Gizmos.DrawCube(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).size);
		Gizmos.color = new Color(1f, 0.7f, 0f, 1f);
		Gizmos.DrawWireCube(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).size);
	}
}
