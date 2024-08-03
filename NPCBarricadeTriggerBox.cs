using ConVar;
using UnityEngine;

public class NPCBarricadeTriggerBox : MonoBehaviour
{
	private Barricade target;

	private static int playerServerLayer = -1;

	public void Setup(Barricade t)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		target = t;
		((Component)this).transform.SetParent(((Component)target).transform, false);
		((Component)this).gameObject.layer = 18;
		BoxCollider val = ((Component)this).gameObject.AddComponent<BoxCollider>();
		((Collider)val).isTrigger = true;
		val.center = Vector3.zero;
		val.size = Vector3.one * AI.npc_door_trigger_size + Vector3.right * ((Bounds)(ref target.bounds)).size.x;
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((Object)(object)target == (Object)null || target.isClient)
		{
			return;
		}
		if (playerServerLayer < 0)
		{
			playerServerLayer = LayerMask.NameToLayer("Player (Server)");
		}
		if ((((Component)other).gameObject.layer & playerServerLayer) > 0)
		{
			BasePlayer component = ((Component)other).gameObject.GetComponent<BasePlayer>();
			if ((Object)(object)component != (Object)null && component.IsNpc && !(component is BasePet))
			{
				target.Kill(BaseNetworkable.DestroyMode.Gib);
			}
		}
	}
}
