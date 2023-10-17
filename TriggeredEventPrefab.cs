using System;
using UnityEngine;

public class TriggeredEventPrefab : TriggeredEvent
{
	public GameObjectRef targetPrefab;

	public bool shouldBroadcastSpawn = false;

	public Phrase spawnPhrase;

	private void RunEvent()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)("[event] " + targetPrefab.resourcePath));
		BaseEntity baseEntity = GameManager.server.CreateEntity(targetPrefab.resourcePath);
		if (!Object.op_Implicit((Object)(object)baseEntity))
		{
			return;
		}
		((Component)baseEntity).SendMessage("TriggeredEventSpawn", (SendMessageOptions)1);
		baseEntity.Spawn();
		if (!shouldBroadcastSpawn)
		{
			return;
		}
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				if (Object.op_Implicit((Object)(object)current) && current.IsConnected)
				{
					current.ShowToast(GameTip.Styles.Server_Event, spawnPhrase);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}
}
