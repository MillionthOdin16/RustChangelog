using System;
using Network;
using UnityEngine;

public class BigWheelBettingTerminal : StorageContainer
{
	public BigWheelGame bigWheel;

	public Vector3 seatedPlayerOffset = Vector3.forward;

	public float offsetCheckRadius = 0.4f;

	public SoundDefinition winSound;

	public SoundDefinition loseSound;

	[NonSerialized]
	public BasePlayer lastPlayer;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BigWheelBettingTerminal.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public new void OnDrawGizmos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.yellow;
		Vector3 val = ((Component)this).transform.TransformPoint(seatedPlayerOffset);
		Gizmos.DrawSphere(val, offsetCheckRadius);
		base.OnDrawGizmos();
	}

	public bool IsPlayerValid(BasePlayer player)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!player.isMounted || !(player.GetMounted() is BaseChair))
		{
			return false;
		}
		Vector3 val = ((Component)this).transform.TransformPoint(seatedPlayerOffset);
		float num = Vector3Ex.Distance2D(((Component)player).transform.position, val);
		if (num > offsetCheckRadius)
		{
			return false;
		}
		return true;
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (!IsPlayerValid(player))
		{
			return false;
		}
		bool flag = base.PlayerOpenLoot(player, panelToOpen);
		if (flag)
		{
			lastPlayer = player;
		}
		return flag;
	}

	public bool TrySetBigWheel(BigWheelGame newWheel)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return false;
		}
		if ((Object)(object)bigWheel != (Object)null && (Object)(object)bigWheel != (Object)(object)newWheel)
		{
			float num = Vector3.SqrMagnitude(((Component)bigWheel).transform.position - ((Component)this).transform.position);
			float num2 = Vector3.SqrMagnitude(((Component)newWheel).transform.position - ((Component)this).transform.position);
			if (num2 >= num)
			{
				return false;
			}
			bigWheel.RemoveTerminal(this);
		}
		bigWheel = newWheel;
		return true;
	}
}
