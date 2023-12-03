using System;
using Rust;
using UnityEngine;

public class ConfettiCannon : DecayEntity
{
	public float InitialBlastDelay = 1f;

	public float BlastCooldown = 3f;

	public GameObjectRef ConfettiPrefab;

	public Transform ConfettiPrefabSpawnPoint;

	public const Flags Ignited = Flags.OnFire;

	public float DamagePerBlast = 3f;

	private Action blastAction;

	private Action clearBusy;

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void Blast(RPCMessage msg)
	{
		if (!IsBusy())
		{
			SetFlag(Flags.Busy, b: true);
			SetFlag(Flags.OnFire, b: true);
			if (blastAction == null)
			{
				blastAction = TriggerBlast;
			}
			if (clearBusy == null)
			{
				clearBusy = ClearBusy;
			}
			((FacepunchBehaviour)this).Invoke(blastAction, InitialBlastDelay);
			((FacepunchBehaviour)this).Invoke(clearBusy, InitialBlastDelay + BlastCooldown);
		}
	}

	private void TriggerBlast()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (ConfettiPrefab.isValid && (Object)(object)ConfettiPrefabSpawnPoint != (Object)null)
		{
			Effect.server.Run(ConfettiPrefab.resourcePath, ConfettiPrefabSpawnPoint.position, ConfettiPrefabSpawnPoint.forward);
		}
		SetFlag(Flags.OnFire, b: false);
		Hurt(DamagePerBlast, DamageType.Generic, null, useProtection: false);
	}

	private void ClearBusy()
	{
		SetFlag(Flags.Busy, b: false);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		ClearBusy();
	}
}
