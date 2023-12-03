using System;
using Network;
using UnityEngine;

public class JunkPile : BaseEntity
{
	public GameObjectRef sinkEffect;

	public SpawnGroup[] spawngroups;

	public NPCSpawner NPCSpawn;

	private const float lifetimeMinutes = 30f;

	protected bool isSinking;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("JunkPile.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)TimeOut, 1800f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)CheckEmpty, 10f, 30f);
		((FacepunchBehaviour)this).Invoke((Action)SpawnInitial, 1f);
		isSinking = false;
	}

	internal override void DoServerDestroy()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.DoServerDestroy();
		StabilityEntity.UpdateSurroundingsQueue updateSurroundingsQueue = StabilityEntity.updateSurroundingsQueue;
		OBB val = WorldSpaceBounds();
		((ObjectWorkQueue<Bounds>)updateSurroundingsQueue).Add(((OBB)(ref val)).ToBounds());
	}

	private void SpawnInitial()
	{
		SpawnGroup[] array = spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SpawnInitial();
		}
	}

	public bool SpawnGroupsEmpty()
	{
		SpawnGroup[] array = spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].currentPopulation > 0)
			{
				return false;
			}
		}
		if ((Object)(object)NPCSpawn != (Object)null && NPCSpawn.currentPopulation > 0)
		{
			return false;
		}
		return true;
	}

	public void CheckEmpty()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (SpawnGroupsEmpty() && !BaseNetworkable.HasCloseConnections(((Component)this).transform.position, TimeoutPlayerCheckRadius()))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)CheckEmpty);
			SinkAndDestroy();
		}
	}

	public virtual float TimeoutPlayerCheckRadius()
	{
		return 15f;
	}

	public void TimeOut()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (BaseNetworkable.HasCloseConnections(((Component)this).transform.position, TimeoutPlayerCheckRadius()))
		{
			((FacepunchBehaviour)this).Invoke((Action)TimeOut, 30f);
			return;
		}
		SpawnGroupsEmpty();
		SinkAndDestroy();
	}

	public void SinkAndDestroy()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		((FacepunchBehaviour)this).CancelInvoke((Action)SinkAndDestroy);
		SpawnGroup[] array = spawngroups;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
		SetFlag(Flags.Reserved8, b: true, recursive: true);
		if ((Object)(object)NPCSpawn != (Object)null)
		{
			NPCSpawn.Clear();
		}
		ClientRPC(null, "CLIENT_StartSink");
		Transform transform = ((Component)this).transform;
		transform.position -= new Vector3(0f, 5f, 0f);
		isSinking = true;
		((FacepunchBehaviour)this).Invoke((Action)KillMe, 22f);
	}

	public void KillMe()
	{
		Kill();
	}
}
