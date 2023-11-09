using UnityEngine;

public class PlatformEntity : BaseEntity
{
	private const float movementSpeed = 1f;

	private const float rotationSpeed = 10f;

	private const float radius = 10f;

	private Vector3 targetPosition = Vector3.zero;

	private Quaternion targetRotation = Quaternion.identity;

	protected void FixedUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		if (targetPosition == Vector3.zero || Vector3.Distance(((Component)this).transform.position, targetPosition) < 0.01f)
		{
			Vector2 val = Random.insideUnitCircle * 10f;
			targetPosition = ((Component)this).transform.position + new Vector3(val.x, 0f, val.y);
			if ((Object)(object)TerrainMeta.HeightMap != (Object)null && (Object)(object)TerrainMeta.WaterMap != (Object)null)
			{
				float height = TerrainMeta.HeightMap.GetHeight(targetPosition);
				float height2 = TerrainMeta.WaterMap.GetHeight(targetPosition);
				targetPosition.y = Mathf.Max(height, height2) + 1f;
			}
			targetRotation = Quaternion.LookRotation(targetPosition - ((Component)this).transform.position);
		}
		((Component)this).transform.SetPositionAndRotation(Vector3.MoveTowards(((Component)this).transform.position, targetPosition, Time.fixedDeltaTime * 1f), Quaternion.RotateTowards(((Component)this).transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));
	}

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}
}
