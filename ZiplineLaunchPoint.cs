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

	public bool RegenLine = false;

	private List<Vector3> ziplineTargets = new List<Vector3>();

	private List<Vector3> linePoints = null;

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
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - MountPlayer "));
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
						TimeWarning val4 = TimeWarning.New("Call", 0);
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
							((IDisposable)val4)?.Dispose();
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
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		base.PostMapEntitySpawn();
		FindZiplineTarget(ref ziplineTargets);
		CalculateZiplinePoints(ziplineTargets, ref linePoints);
		if (ziplineTargets.Count == 0)
		{
			Kill();
			return;
		}
		float num = Vector3.Distance(linePoints[0], linePoints[linePoints.Count - 1]);
		if (num > 100f && ArrivalPointRef != null && ArrivalPointRef.isValid)
		{
			ZiplineArrivalPoint ziplineArrivalPoint = base.gameManager.CreateEntity(ArrivalPointRef.resourcePath, linePoints[linePoints.Count - 1]) as ZiplineArrivalPoint;
			ziplineArrivalPoint.SetPositions(linePoints);
			ziplineArrivalPoint.Spawn();
		}
		UpdateBuildingBlocks();
		SendNetworkUpdate();
	}

	private void FindZiplineTarget(ref List<Vector3> foundPositions)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
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
							bool flag3 = CheckLineOfSight(((Component)item2).transform.position, ((Component)item3).transform.position);
							bool flag4 = CheckLineOfSight(((Component)item3).transform.position, ((Component)item).transform.position);
							if (flag3 && flag4)
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = CalculateLineMidPoint(from, to) - Vector3.up * 0.75f;
		return GamePhysics.LineOfSightRadius(from, to, 1218511105, 0.5f, 2f) && GamePhysics.LineOfSightRadius(from, val, 1218511105, 0.5f, 2f) && GamePhysics.LineOfSightRadius(val, to, 1218511105, 0.5f, 2f);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(2uL)]
	private void MountPlayer(RPCMessage msg)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = Vector3.Lerp(start, endPoint, 0.5f);
		result.y -= LineSlackAmount;
		return result;
	}

	private void UpdateBuildingBlocks()
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		BoxCollider[] buildingBlocks = BuildingBlocks;
		foreach (BoxCollider val in buildingBlocks)
		{
			((Component)val).gameObject.SetActive(false);
		}
		BoxCollider[] pointBuildingBlocks = PointBuildingBlocks;
		foreach (BoxCollider val2 in pointBuildingBlocks)
		{
			((Component)val2).gameObject.SetActive(false);
		}
		SpawnableBoundsBlocker[] spawnableBoundsBlockers = SpawnableBoundsBlockers;
		foreach (SpawnableBoundsBlocker spawnableBoundsBlocker in spawnableBoundsBlockers)
		{
			((Component)spawnableBoundsBlocker).gameObject.SetActive(false);
		}
		int num = 0;
		if (ziplineTargets.Count <= 0)
		{
			return;
		}
		Vector3 val3 = Vector3.zero;
		int startIndex2 = 0;
		for (int l = 0; l < linePoints.Count; l++)
		{
			if (l == 0 || (base.isClient && l == 1))
			{
				continue;
			}
			Vector3 val4 = linePoints[l];
			Vector3 val5 = val4 - Vector3Ex.WithY(linePoints[l - 1], val4.y);
			Vector3 normalized = ((Vector3)(ref val5)).normalized;
			if (val3 != Vector3.zero)
			{
				float num2 = Vector3.Dot(normalized, val3);
				if (num2 < 0.98f)
				{
					if (num < BuildingBlocks.Length)
					{
						SetUpBuildingBlock(BuildingBlocks[num], PointBuildingBlocks[num], SpawnableBoundsBlockers[num++], startIndex2, l - 1);
					}
					startIndex2 = l - 1;
				}
			}
			val3 = normalized;
		}
		if (num < BuildingBlocks.Length)
		{
			SetUpBuildingBlock(BuildingBlocks[num], PointBuildingBlocks[num], SpawnableBoundsBlockers[num], startIndex2, linePoints.Count - 1);
		}
		void SetUpBuildingBlock(BoxCollider longCollider, BoxCollider pointCollider, SpawnableBoundsBlocker spawnBlocker, int startIndex, int endIndex)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Unknown result type (might be due to invalid IL or missing references)
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_014b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val6 = linePoints[startIndex];
			Vector3 val7 = linePoints[endIndex];
			Vector3 val8 = Vector3.zero;
			Vector3 center = val6 - val7;
			Quaternion rotation = Quaternion.LookRotation(((Vector3)(ref center)).normalized, Vector3.up);
			Vector3 position = Vector3.Lerp(val6, val7, 0.5f);
			((Component)longCollider).transform.position = position;
			((Component)longCollider).transform.rotation = rotation;
			for (int m = startIndex; m < endIndex; m++)
			{
				Vector3 val9 = ((Component)longCollider).transform.InverseTransformPoint(linePoints[m]);
				if (val9.y < val8.y)
				{
					val8 = val9;
				}
			}
			float num3 = Mathf.Abs(val8.y) + 2f;
			float num4 = Vector3.Distance(val6, val7);
			center = (longCollider.size = (spawnBlocker.BoxCollider.size = new Vector3(0.5f, num3, num4) + Vector3.one));
			BoxCollider boxCollider = spawnBlocker.BoxCollider;
			((Vector3)(ref center))._002Ector(0f, 0f - num3 * 0.5f, 0f);
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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Dot(ply.eyes.HeadForward(), ((Component)this).transform.forward);
		return num > 0.2f;
	}

	public float GetLineLength()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
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
