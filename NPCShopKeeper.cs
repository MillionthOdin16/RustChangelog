using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class NPCShopKeeper : NPCPlayer
{
	public EntityRef invisibleVendingMachineRef;

	public InvisibleVendingMachine machine;

	private float greetDir;

	private Vector3 initialFacingDir;

	private BasePlayer lastWavedAtPlayer;

	public InvisibleVendingMachine GetVendingMachine()
	{
		return invisibleVendingMachineRef.IsValid(base.isServer) ? ((Component)invisibleVendingMachineRef.Get(base.isServer)).GetComponent<InvisibleVendingMachine>() : null;
	}

	public void OnDrawGizmos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.green;
		Gizmos.DrawCube(((Component)this).transform.position + Vector3.up * 1f, new Vector3(0.5f, 1f, 0.5f));
	}

	public override void UpdateProtectionFromClothing()
	{
	}

	public override void Hurt(HitInfo info)
	{
	}

	public override void ServerInit()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		initialFacingDir = ((Component)this).transform.rotation * Vector3.forward;
		((FacepunchBehaviour)this).Invoke((Action)DelayedSleepEnd, 3f);
		SetAimDirection(((Component)this).transform.rotation * Vector3.forward);
		((FacepunchBehaviour)this).InvokeRandomized((Action)Greeting, Random.Range(5f, 10f), 5f, Random.Range(0f, 2f));
		if (invisibleVendingMachineRef.IsValid(serverside: true) && (Object)(object)machine == (Object)null)
		{
			machine = GetVendingMachine();
		}
		else if ((Object)(object)machine != (Object)null && !invisibleVendingMachineRef.IsValid(serverside: true))
		{
			invisibleVendingMachineRef.Set(machine);
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.shopKeeper = Pool.Get<ShopKeeper>();
		info.msg.shopKeeper.vendingRef = invisibleVendingMachineRef.uid;
	}

	public override void Load(LoadInfo info)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.shopKeeper != null)
		{
			invisibleVendingMachineRef.uid = info.msg.shopKeeper.vendingRef;
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
	}

	public void DelayedSleepEnd()
	{
		EndSleeping();
	}

	public void GreetPlayer(BasePlayer player)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player != (Object)null)
		{
			SignalBroadcast(Signal.Gesture, "wave");
			SetAimDirection(Vector3Ex.Direction2D(player.eyes.position, eyes.position));
			lastWavedAtPlayer = player;
		}
		else
		{
			SetAimDirection(initialFacingDir);
		}
	}

	public void Greeting()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities(((Component)this).transform.position, 10f, list, 131072, (QueryTriggerInteraction)2);
		Vector3 position = ((Component)this).transform.position;
		BasePlayer basePlayer = null;
		foreach (BasePlayer item in list)
		{
			if (item.isClient || item.IsNpc || (Object)(object)item == (Object)(object)this || !item.IsVisible(eyes.position) || (Object)(object)item == (Object)(object)lastWavedAtPlayer || Vector3.Dot(Vector3Ex.Direction2D(item.eyes.position, eyes.position), initialFacingDir) < 0.2f)
			{
				continue;
			}
			basePlayer = item;
			break;
		}
		if ((Object)(object)basePlayer == (Object)null && !list.Contains(lastWavedAtPlayer))
		{
			lastWavedAtPlayer = null;
		}
		if ((Object)(object)basePlayer != (Object)null)
		{
			SignalBroadcast(Signal.Gesture, "wave");
			SetAimDirection(Vector3Ex.Direction2D(basePlayer.eyes.position, eyes.position));
			lastWavedAtPlayer = basePlayer;
		}
		else
		{
			SetAimDirection(initialFacingDir);
		}
		Pool.FreeList<BasePlayer>(ref list);
	}
}
