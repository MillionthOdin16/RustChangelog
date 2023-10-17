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
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			Vector3 pos = ((Component)basePlayer).transform.position;
			Debug.Log((object)("heli called to : " + ((object)(Vector3)(ref pos)).ToString()));
			GameManager server = GameManager.server;
			pos = default(Vector3);
			BaseEntity baseEntity = server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", pos);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				((Component)baseEntity).GetComponent<PatrolHelicopterAI>().SetInitialDestination(((Component)basePlayer).transform.position + new Vector3(0f, 10f, 0f), 0f);
				baseEntity.Spawn();
			}
		}
	}

	[ServerVar]
	public static void calltome(Arg arg)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			Vector3 pos = ((Component)basePlayer).transform.position;
			Debug.Log((object)("heli called to : " + ((object)(Vector3)(ref pos)).ToString()));
			GameManager server = GameManager.server;
			pos = default(Vector3);
			BaseEntity baseEntity = server.CreateEntity("assets/prefabs/npc/patrol helicopter/patrolhelicopter.prefab", pos);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				((Component)baseEntity).GetComponent<PatrolHelicopterAI>().SetInitialDestination(((Component)basePlayer).transform.position + new Vector3(0f, 10f, 0f));
				baseEntity.Spawn();
			}
		}
	}

	[ServerVar]
	public static void call(Arg arg)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)arg.Player()))
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
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
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
				Vector3 point = ((RaycastHit)(ref val)).point;
				Debug.Log((object)("strafing :" + ((object)(Vector3)(ref point)).ToString()));
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
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			_ = basePlayer.IsDeveloper;
		}
	}
}
