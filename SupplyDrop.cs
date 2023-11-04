using System;
using ConVar;
using Rust;
using UnityEngine;

public class SupplyDrop : LootContainer
{
	private const Flags FlagNightLight = Flags.Reserved1;

	private const Flags ShowParachute = Flags.Reserved2;

	public GameObject ParachuteRoot;

	public override void ServerInit()
	{
		base.ServerInit();
		if (!Application.isLoadingSave)
		{
			SetFlag(Flags.Reserved2, b: true);
		}
		isLootable = false;
		((FacepunchBehaviour)this).Invoke((Action)MakeLootable, 300f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)CheckNightLight, 0f, 30f);
	}

	private void RemoveParachute()
	{
		SetFlag(Flags.Reserved2, b: false);
	}

	public void MakeLootable()
	{
		isLootable = true;
	}

	private void OnCollisionEnter(Collision collision)
	{
		bool flag = ((1 << ((Component)collision.collider).gameObject.layer) & 0x40A10111) > 0;
		if (((1 << ((Component)collision.collider).gameObject.layer) & 0x8000000) > 0 && collision.GetEntity() is Tugboat)
		{
			flag = true;
		}
		if (flag)
		{
			RemoveParachute();
			MakeLootable();
		}
	}

	private void CheckNightLight()
	{
		SetFlag(Flags.Reserved1, Env.time > 20f || Env.time < 7f);
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if ((Object)(object)ParachuteRoot != (Object)null)
		{
			ParachuteRoot.SetActive(next.HasFlag(Flags.Reserved2));
		}
	}
}
