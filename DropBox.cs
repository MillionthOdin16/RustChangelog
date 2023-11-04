using UnityEngine;

public class DropBox : Mailbox
{
	public Transform EyePoint = null;

	public override bool PlayerIsOwner(BasePlayer player)
	{
		return PlayerBehind(player);
	}

	public bool PlayerBehind(BasePlayer player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 forward = ((Component)this).transform.forward;
		Vector3 val = ((Component)player).transform.position - ((Component)this).transform.position;
		float num = Vector3.Dot(forward, ((Vector3)(ref val)).normalized);
		return num <= -0.3f && GamePhysics.LineOfSight(player.eyes.position, EyePoint.position, 2162688);
	}

	public bool PlayerInfront(BasePlayer player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 forward = ((Component)this).transform.forward;
		Vector3 val = ((Component)player).transform.position - ((Component)this).transform.position;
		return Vector3.Dot(forward, ((Vector3)(ref val)).normalized) >= 0.7f;
	}

	public override bool SupportsChildDeployables()
	{
		return true;
	}
}
