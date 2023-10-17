using UnityEngine;

public class ItemModConversation : ItemMod
{
	public ConversationData conversationData;

	public GameObjectRef conversationEntity;

	public GameObjectRef squakEffect;

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (command == "squak")
		{
			if (squakEffect.isValid)
			{
				Effect.server.Run(squakEffect.resourcePath, player.eyes.position);
			}
			Debug.Log((object)"Starting conversation");
			BaseEntity baseEntity = GameManager.server.CreateEntity(conversationEntity.resourcePath, ((Component)player).transform.position + Vector3.up * -2f);
			((Component)baseEntity).GetComponent<NPCMissionProvider>().conversations[0] = conversationData;
			baseEntity.Spawn();
			((MonoBehaviour)baseEntity).Invoke("Kill", 600f);
		}
	}
}
