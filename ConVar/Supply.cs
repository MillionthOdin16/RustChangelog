using UnityEngine;

namespace ConVar;

[Factory("supply")]
public class Supply : ConsoleSystem
{
	private const string path = "assets/prefabs/npc/cargo plane/cargo_plane.prefab";

	[ServerVar]
	public static void drop(Arg arg)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			Debug.Log((object)"Supply Drop Inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab");
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				CargoPlane component = ((Component)baseEntity).GetComponent<CargoPlane>();
				component.InitDropPosition(((Component)basePlayer).transform.position + new Vector3(0f, 10f, 0f));
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
			Debug.Log((object)"Supply Drop Inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/cargo plane/cargo_plane.prefab");
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.Spawn();
			}
		}
	}
}
