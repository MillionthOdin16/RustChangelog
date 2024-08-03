using UnityEngine;

public class CH47ReinforcementListener : BaseEntity
{
	public string listenString;

	public GameObjectRef heliPrefab;

	public float startDist = 300f;

	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == listenString)
		{
			Call();
		}
	}

	public void Call()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		CH47HelicopterAIController component = ((Component)GameManager.server.CreateEntity(heliPrefab.resourcePath)).GetComponent<CH47HelicopterAIController>();
		if (Object.op_Implicit((Object)(object)component))
		{
			float x = TerrainMeta.Size.x;
			CH47LandingZone closest = CH47LandingZone.GetClosest(((Component)this).transform.position);
			Vector3 zero = Vector3.zero;
			zero.y = ((Component)closest).transform.position.y;
			Vector3 val = Vector3Ex.Direction2D(((Component)closest).transform.position, zero);
			Vector3 position = ((Component)closest).transform.position + val * startDist;
			position.y = ((Component)closest).transform.position.y;
			((Component)component).transform.position = position;
			component.SetLandingTarget(((Component)closest).transform.position);
			component.Spawn();
		}
	}
}
