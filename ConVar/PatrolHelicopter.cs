using UnityEngine;

namespace ConVar;

[Factory("heli")]
public class PatrolHelicopter : ConsoleSystem
{
	private const string path = "assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab";

	[ServerVar]
	public static float lifetimeMinutes = 15f;

	[ServerVar]
	public static int guns = 1;

	[ServerVar]
	public static float bulletDamageScale = 1f;

	[ServerVar]
	public static float bulletAccuracy = 2f;

	[ServerVar]
	public static void drop(Arg arg)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			Debug.Log((object)("heli called to : " + ((Component)basePlayer).transform.position));
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab");
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				PatrolHelicopterAI component = ((Component)baseEntity).GetComponent<PatrolHelicopterAI>();
				component.SetInitialDestination(((Component)basePlayer).transform.position + new Vector3(0f, 10f, 0f), 0f);
				baseEntity.Spawn();
			}
		}
	}

	[ServerVar]
	public static void calltome(Arg arg)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			Debug.Log((object)("heli called to : " + ((Component)basePlayer).transform.position));
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab");
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				PatrolHelicopterAI component = ((Component)baseEntity).GetComponent<PatrolHelicopterAI>();
				component.SetInitialDestination(((Component)basePlayer).transform.position + new Vector3(0f, 10f, 0f));
				baseEntity.Spawn();
			}
		}
	}

	[ServerVar]
	public static void call(Arg arg)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			Debug.Log((object)"Helicopter inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab");
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.Spawn();
			}
		}
	}

	[ServerVar]
	public static void strafe(Arg arg)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			PatrolHelicopterAI heliInstance = PatrolHelicopterAI.heliInstance;
			RaycastHit val = default(RaycastHit);
			if ((Object)(object)heliInstance == (Object)null)
			{
				Debug.Log((object)"no heli instance");
			}
			else if (Physics.Raycast(basePlayer.eyes.HeadRay(), ref val, 1000f, 1218652417))
			{
				Debug.Log((object)("strafing :" + ((RaycastHit)(ref val)).point));
				heliInstance.interestZoneOrigin = ((RaycastHit)(ref val)).point;
				heliInstance.ExitCurrentState();
				heliInstance.State_Strafe_Enter(((RaycastHit)(ref val)).point);
			}
			else
			{
				Debug.Log((object)"strafe ray missed");
			}
		}
	}

	[ServerVar]
	public static void testpuzzle(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsDeveloper)
		{
		}
	}
}
