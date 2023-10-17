using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class ZiplineLaunchPoint : BaseEntity
{
	public Transform LineDeparturePoint;

	public LineRenderer ZiplineRenderer;

	public Collider MountCollider;

	public BoxCollider[] BuildingBlocks;

	public BoxCollider[] PointBuildingBlocks;

	public SpawnableBoundsBlocker[] SpawnableBoundsBlockers;

	public GameObjectRef MountableRef;

	public float LineSlackAmount = 2f;

	public bool RegenLine;

	private List<Vector3> ziplineTargets = new List<Vector3>();

	private List<Vector3> linePoints;

	public GameObjectRef ArrivalPointRef;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ZiplineLaunchPoint.OnRpcMessage", 0);
		try
		{
			if (rpc == 2256922575u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - MountPlayer "));
				}
				TimeWarning val2 = TimeWarning.New("MountPlayer", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2256922575u, "MountPlayer", this, player, 2uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(2256922575u, "MountPlayer", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							MountPlayer(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in MountPlayer");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ResetState()
	{
		base.ResetState();
		ziplineTargets.Clear();
		linePoints = null;
	}

	public override void PostMapEntitySpawn()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		base.PostMapEntitySpawn();
		FindZiplineTarget(ref ziplineTargets);
		CalculateZiplinePoints(ziplineTargets, ref linePoints);
		if (ziplineTargets.Count == 0)
		{
			Kill();
			return;
		}
		if (Vector3.Distance(linePoints[0], linePoints[linePoints.Count - 1]) > 100f && ArrivalPointRef != null && ArrivalPointRef.isValid)
		{
			ZiplineArrivalPoint obj = base.gameManager.CreateEntity(ArrivalPointRef.resourcePath, linePoints[linePoints.Count - 1]) as ZiplineArrivalPoint;
			obj.SetPositions(linePoints);
			obj.Spawn();
		}
		UpdateBuildingBlocks();
		SendNetworkUpdate();
	}

	private void FindZiplineTarget(ref List<Vector3> foundPositions)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		foundPositions.Clear();
		Vector3 position = LineDeparturePoint.position;
		List<ZiplineTarget> list = Pool.GetList<ZiplineTarget>();
		GamePhysics.OverlapSphere<ZiplineTarget>(position + ((Component)this).transform.forward * 200f, 200f, list, 1218511105, (QueryTriggerInteraction)1);
		ZiplineTarget ziplineTarget = null;
		float num = float.MaxValue;
		float num2 = 3f;
		foreach (ZiplineTarget item in list)
		{
			if (item.IsChainPoint)
			{
				continue;
			}
			Vector3 position2 = ((Component)item).transform.position;
			Vector3 val = Vector3Ex.WithY(position2, position.y) - position;
			float num3 = Vector3.Dot(((Vector3)(ref val)).normalized, ((Component)this).transform.forward);
			float num4 = Vector3.Distance(position, position2);
			if (!(num3 > 0.2f) || !item.IsValidPosition(position) || !(position.y + num2 > position2.y) || !(num4 > 10f) || !(num4 < num))
			{
				continue;
			}
			if (CheckLineOfSight(position, position2))
			{
				num = num4;
				ziplineTarget = item;
				foundPositions.Clear();
				foundPositions.Add(((Component)ziplineTarget).transform.position);
				continue;
			}
			foreach (ZiplineTarget item2 in list)
			{
				if (!item2.IsChainPoint || !item2.IsValidChainPoint(position, position2))
				{
					continue;
				}
				bool flag = CheckLineOfSight(position, ((Component)item2).transform.position);
				bool flag2 = CheckLineOfSight(((Component)item2).transform.position, position2);
				if (flag && flag2)
				{
					num = num4;
					ziplineTarget = item;
					foundPositions.Clear();
					foundPositions.Add(((Component)item2).transform.position);
					foundPositions.Add(((Component)ziplineTarget).transform.position);
				}
				else
				{
					if (!flag)
					{
						continue;
					}
					foreach (ZiplineTarget item3 in list)
					{
						if (!((Object)(object)item3 == (Object)(object)item2) && item3.IsValidChainPoint(item2.Target.position, item.Target.position))
						{
							bool num5 = CheckLineOfSight(((Component)item2).transform.position, ((Component)item3).transform.position);
							bool flag3 = CheckLineOfSight(((Component)item3).transform.position, ((Component)item).transform.position);
							if (num5 && flag3)
							{
								num = num4;
								ziplineTarget = item;
								foundPositions.Clear();
								foundPositions.Add(((Component)item2).transform.position);
								foundPositions.Add(((Component)item3).transform.position);
								foundPositions.Add(((Component)ziplineTarget).transform.position);
							}
						}
					}
				}
			}
		}
	}

	private bool CheckLineOfSight(Vector3 from, Vector3 to)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = CalculateLineMidPoint(from, to) - Vector3.up * 0.75f;
		if (GamePhysics.LineOfSightRadius(from, to, 1218511105, 0.5f, 2f) && GamePhysics.LineOfSightRadius(from, val, 1218511105, 0.5f, 2f))
		{
			return GamePhysics.LineOfSightRadius(val, to, 1218511105, 0.5f, 2f);
		}
		return false;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(2uL)]
	private void MountPlayer(RPCMessage msg)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		if (IsBusy() || (Object)(object)msg.player == (Object)null || msg.player.Distance(LineDeparturePoint.position) > 3f || !IsPlayerFacingValidDirection(msg.player) || ziplineTargets.Count == 0)
		{
			return;
		}
		Vector3 position = LineDeparturePoint.position;
		Vector3 val = Vector3Ex.WithY(ziplineTargets[0], position.y) - position;
		Quaternion lineStartRot = Quaternion.LookRotation(((Vector3)(ref val)).normalized);
		val = position - Vector3Ex.WithY(((Component)msg.player).transform.position, position.y);
		Quaternion rot = Quaternion.LookRotation(((Vector3)(ref val)).normalized);
		ZiplineMountable ziplineMountable = base.gameManager.CreateEntity(MountableRef.resourcePath, ((Component)msg.player).transform.position + Vector3.up * 2.1f, rot) as ZiplineMountable;
		if ((Object)(object)ziplineMountable != (Object)null)
		{
			CalculateZiplinePoints(ziplineTargets, ref linePoints);
			ziplineMountable.SetDestination(linePoints, position, lineStartRot);
			ziplineMountable.Spawn();
			ziplineMountable.MountPlayer(msg.player);
			if ((Object)(object)msg.player.GetMounted() != (Object)(object)ziplineMountable)
			{
				ziplineMountable.Kill();
			}
			SetFlag(Flags.Busy, b: true);
			((FacepunchBehaviour)this).Invoke((Action)ClearBusy, 2f);
		}
	}

	private void ClearBusy()
	{
		SetFlag(Flags.Busy, b: false);
	}

	public override void Save(SaveInfo info)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (info.msg.zipline == null)
		{
			info.msg.zipline = Pool.Get<Zipline>();
		}
		info.msg.zipline.destinationPoints = Pool.GetList<VectorData>();
		foreach (Vector3 ziplineTarget in ziplineTargets)
		{
			info.msg.zipline.destinationPoints.Add(new VectorData(ziplineTarget.x, ziplineTarget.y, ziplineTarget.z));
		}
	}

	[ServerVar(ServerAdmin = true)]
	public static void report(Arg arg)
	{
		float num = 0f;
		int num2 = 0;
		int num3 = 0;
		foreach (BaseNetworkable serverEntity in BaseNetworkable.serverEntities)
		{
			if (serverEntity is ZiplineLaunchPoint ziplineLaunchPoint)
			{
				float lineLength = ziplineLaunchPoint.GetLineLength();
				num2++;
				num += lineLength;
			}
			else if (serverEntity is ZiplineArrivalPoint)
			{
				num3++;
			}
		}
		arg.ReplyWith($"{num2} ziplines, total distance: {num:F2}, avg length: {num / (float)num2:F2}, arrival points: {num3}");
	}

	public override void Load(LoadInfo info)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.zipline == null)
		{
			return;
		}
		ziplineTargets.Clear();
		foreach (VectorData destinationPoint in info.msg.zipline.destinationPoints)
		{
			ziplineTargets.Add(VectorData.op_Implicit(destinationPoint));
		}
	}

	private void CalculateZiplinePoints(List<Vector3> targets, ref List<Vector3> points)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (points == null && targets.Count != 0)
		{
			Vector3[] array = (Vector3[])(object)new Vector3[targets.Count + 1];
			array[0] = LineDeparturePoint.position;
			for (int i = 0; i < targets.Count; i++)
			{
				array[i + 1] = targets[i];
			}
			float[] array2 = new float[array.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = LineSlackAmount;
			}
			points = Pool.GetList<Vector3>();
			Bezier.ApplyLineSlack(array, array2, ref points, 25);
		}
	}

	private Vector3 CalculateLineMidPoint(Vector3 start, Vector3 endPoint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = Vector3.Lerp(start, endPoint, 0.5f);
		result.y -= LineSlackAmount;
		return result;
	}

	private void UpdateBuildingBlocks()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		BoxCollider[] buildingBlocks = BuildingBlocks;
		for (int i = 0; i < buildingBlocks.Length; i++)
		{
			((Component)buildingBlocks[i]).gameObject.SetActive(false);
		}
		buildingBlocks = PointBuildingBlocks;
		for (int i = 0; i < buildingBlocks.Length; i++)
		{
			((Component)buildingBlocks[i]).gameObject.SetActive(false);
		}
		SpawnableBoundsBlocker[] spawnableBoundsBlockers = SpawnableBoundsBlockers;
		for (int i = 0; i < spawnableBoundsBlockers.Length; i++)
		{
			((Component)spawnableBoundsBlockers[i]).gameObject.SetActive(false);
		}
		int num = 0;
		if (ziplineTargets.Count <= 0)
		{
			return;
		}
		Vector3 val = Vector3.zero;
		int startIndex2 = 0;
		for (int j = 0; j < linePoints.Count; j++)
		{
			if (j == 0 || (base.isClient && j == 1))
			{
				continue;
			}
			Vector3 val2 = linePoints[j];
			Vector3 val3 = val2 - Vector3Ex.WithY(linePoints[j - 1], val2.y);
			Vector3 normalized = ((Vector3)(ref val3)).normalized;
			if (val != Vector3.zero && Vector3.Dot(normalized, val) < 0.98f)
			{
				if (num < BuildingBlocks.Length)
				{
					SetUpBuildingBlock(BuildingBlocks[num], PointBuildingBlocks[num], SpawnableBoundsBlockers[num++], startIndex2, j - 1);
				}
				startIndex2 = j - 1;
			}
			val = normalized;
		}
		if (num < BuildingBlocks.Length)
		{
			SetUpBuildingBlock(BuildingBlocks[num], PointBuildingBlocks[num], SpawnableBoundsBlockers[num], startIndex2, linePoints.Count - 1);
		}
		void SetUpBuildingBlock(BoxCollider longCollider, BoxCollider pointCollider, SpawnableBoundsBlocker spawnBlocker, int startIndex, int endIndex)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val4 = linePoints[startIndex];
			Vector3 val5 = linePoints[endIndex];
			Vector3 val6 = Vector3.zero;
			Vector3 center = val4 - val5;
			Quaternion rotation = Quaternion.LookRotation(((Vector3)(ref center)).normalized, Vector3.up);
			Vector3 position = Vector3.Lerp(val4, val5, 0.5f);
			((Component)longCollider).transform.position = position;
			((Component)longCollider).transform.rotation = rotation;
			for (int k = startIndex; k < endIndex; k++)
			{
				Vector3 val7 = ((Component)longCollider).transform.InverseTransformPoint(linePoints[k]);
				if (val7.y < val6.y)
				{
					val6 = val7;
				}
			}
			float num2 = Mathf.Abs(val6.y) + 2f;
			float num3 = Vector3.Distance(val4, val5);
			center = (longCollider.size = (spawnBlocker.BoxCollider.size = new Vector3(0.5f, num2, num3) + Vector3.one));
			BoxCollider boxCollider = spawnBlocker.BoxCollider;
			((Vector3)(ref center))._002Ector(0f, 0f - num2 * 0.5f, 0f);
			boxCollider.center = center;
			longCollider.center = center;
			((Component)longCollider).gameObject.SetActive(true);
			((Component)pointCollider).transform.position = linePoints[endIndex];
			((Component)pointCollider).gameObject.SetActive(true);
			((Component)spawnBlocker).gameObject.SetActive(true);
			if (base.isServer)
			{
				spawnBlocker.ClearTrees();
			}
		}
	}

	private bool IsPlayerFacingValidDirection(BasePlayer ply)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Dot(ply.eyes.HeadForward(), ((Component)this).transform.forward) > 0.2f;
	}

	public float GetLineLength()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (linePoints == null)
		{
			return 0f;
		}
		float num = 0f;
		for (int i = 0; i < linePoints.Count - 1; i++)
		{
			num += Vector3.Distance(linePoints[i], linePoints[i + 1]);
		}
		return num;
	}
}
