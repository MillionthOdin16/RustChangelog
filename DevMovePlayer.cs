using System;
using UnityEngine;

public class DevMovePlayer : BaseMonoBehaviour
{
	public BasePlayer player;

	public Transform[] Waypoints;

	public bool moveRandomly = false;

	public Vector3 destination = Vector3.zero;

	public Vector3 lookPoint = Vector3.zero;

	private int waypointIndex = 0;

	private float randRun = 0f;

	public void Awake()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		randRun = Random.Range(5f, 10f);
		player = ((Component)this).GetComponent<BasePlayer>();
		if (Waypoints.Length != 0)
		{
			destination = Waypoints[0].position;
		}
		else
		{
			destination = ((Component)this).transform.position;
		}
		if (!player.isClient)
		{
			if ((Object)(object)player.eyes == (Object)null)
			{
				player.eyes = ((Component)player).GetComponent<PlayerEyes>();
			}
			((FacepunchBehaviour)this).Invoke((Action)LateSpawn, 1f);
		}
	}

	public void LateSpawn()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Item item = ItemManager.CreateByName("rifle.semiauto", 1, 0uL);
		player.inventory.GiveItem(item, player.inventory.containerBelt);
		player.UpdateActiveItem(item.uid);
		player.health = 100f;
	}

	public void SetWaypoints(Transform[] wps)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Waypoints = wps;
		destination = wps[0].position;
	}

	public void Update()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		if (player.isClient || !player.IsAlive() || player.IsWounded())
		{
			return;
		}
		if (Vector3.Distance(destination, ((Component)this).transform.position) < 0.25f)
		{
			if (moveRandomly)
			{
				waypointIndex = Random.Range(0, Waypoints.Length);
			}
			else
			{
				waypointIndex++;
			}
			if (waypointIndex >= Waypoints.Length)
			{
				waypointIndex = 0;
			}
		}
		if (Waypoints.Length > waypointIndex)
		{
			destination = Waypoints[waypointIndex].position;
			Vector3 val = destination - ((Component)this).transform.position;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			float running = Mathf.Sin(Time.time + randRun);
			float speed = player.GetSpeed(running, 0f, 0f);
			Vector3 position = ((Component)this).transform.position;
			float range = 1f;
			LayerMask mask = LayerMask.op_Implicit(1537286401);
			if (TransformUtil.GetGroundInfo(((Component)this).transform.position + normalized * speed * Time.deltaTime, out var hitOut, range, mask, ((Component)player).transform))
			{
				position = ((RaycastHit)(ref hitOut)).point;
			}
			((Component)this).transform.position = position;
			val = new Vector3(destination.x, 0f, destination.z) - new Vector3(((Component)player).transform.position.x, 0f, ((Component)player).transform.position.z);
			Vector3 normalized2 = ((Vector3)(ref val)).normalized;
			player.SendNetworkUpdate();
		}
	}
}
