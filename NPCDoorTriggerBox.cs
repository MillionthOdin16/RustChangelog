using ConVar;
using UnityEngine;

public class NPCDoorTriggerBox : MonoBehaviour
{
	private Door door;

	private static int playerServerLayer = -1;

	public void Setup(Door d)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		door = d;
		((Component)this).transform.SetParent(((Component)door).transform, false);
		((Component)this).gameObject.layer = 18;
		BoxCollider val = ((Component)this).gameObject.AddComponent<BoxCollider>();
		((Collider)val).isTrigger = true;
		val.center = Vector3.zero;
		val.size = Vector3.one * AI.npc_door_trigger_size;
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((Object)(object)door == (Object)null || door.isClient || door.IsLocked() || (!door.isSecurityDoor && door.IsOpen()) || (door.isSecurityDoor && !door.IsOpen()))
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
			if ((Object)(object)component != (Object)null && component.IsNpc && !door.isSecurityDoor)
			{
				door.SetOpen(open: true);
			}
		}
	}
}
