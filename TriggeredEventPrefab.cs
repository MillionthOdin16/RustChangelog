using System;
using UnityEngine;

public class TriggeredEventPrefab : TriggeredEvent
{
	public GameObjectRef targetPrefab;

	public bool shouldBroadcastSpawn;

	public Phrase spawnPhrase;

	public BaseEntity spawnedEntity;

	public override void RunEvent()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)("[event] " + targetPrefab.resourcePath));
		BaseEntity baseEntity = GameManager.server.CreateEntity(targetPrefab.resourcePath);
		if (!Object.op_Implicit((Object)(object)baseEntity))
		{
			return;
		}
		((Component)baseEntity).SendMessage("TriggeredEventSpawn", (SendMessageOptions)1);
		baseEntity.Spawn();
		spawnedEntity = baseEntity;
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
				if (Object.op_Implicit((Object)(object)current) && current.IsConnected && !current.IsInTutorial)
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

	public override void Kill()
	{
		if (!((Object)(object)spawnedEntity == (Object)null))
		{
			base.Kill();
			spawnedEntity.Kill();
			spawnedEntity = null;
			Debug.Log((object)("Killed " + ((Object)this).name));
		}
	}
}
