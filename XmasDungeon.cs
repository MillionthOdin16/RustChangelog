using System;
using UnityEngine;

public class XmasDungeon : HalloweenDungeon
{
	public const Flags HasPlayerOutside = Flags.Reserved7;

	public const Flags HasPlayerInside = Flags.Reserved8;

	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float xmaspopulation = 0f;

	[ServerVar(Help = "How long each active dungeon should last before dying", ShowInAdminUI = true)]
	public static float xmaslifetime = 1200f;

	[ServerVar(Help = "How far we detect players from our inside/outside", ShowInAdminUI = true)]
	public static float playerdetectrange = 30f;

	public override float GetLifetime()
	{
		return xmaslifetime;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRepeating((Action)PlayerChecks, 1f, 1f);
	}

	public void PlayerChecks()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		ProceduralDynamicDungeon proceduralDynamicDungeon = dungeonInstance.Get(serverside: true);
		if ((Object)(object)proceduralDynamicDungeon == (Object)null)
		{
			return;
		}
		bool b = false;
		bool b2 = false;
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				float num = Vector3.Distance(((Component)current).transform.position, ((Component)this).transform.position);
				float num2 = Vector3.Distance(((Component)current).transform.position, ((Component)proceduralDynamicDungeon.GetExitPortal(serverSide: true)).transform.position);
				if (num < playerdetectrange)
				{
					b = true;
				}
				if (num2 < playerdetectrange * 2f)
				{
					b2 = true;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		SetFlag(Flags.Reserved8, b2);
		SetFlag(Flags.Reserved7, b);
		proceduralDynamicDungeon.SetFlag(Flags.Reserved7, b);
		proceduralDynamicDungeon.SetFlag(Flags.Reserved8, b2);
	}
}
