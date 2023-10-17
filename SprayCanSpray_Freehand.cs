using System;
using System.Collections.Generic;
using System.IO;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class SprayCanSpray_Freehand : SprayCanSpray
{
	public AlignedLineDrawer LineDrawer;

	public List<AlignedLineDrawer.LinePoint> LinePoints = new List<AlignedLineDrawer.LinePoint>();

	private Color colour = Color.white;

	private float width = 0f;

	private EntityRef<BasePlayer> editingPlayer;

	public GroundWatch groundWatch = null;

	public MeshCollider meshCollider = null;

	public const int MaxLinePointLength = 60;

	public const float SimplifyTolerance = 0.008f;

	private bool AcceptingChanges => editingPlayer.IsValid(serverside: true);

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SprayCanSpray_Freehand.OnRpcMessage", 0);
		try
		{
			if (rpc == 2020094435 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_AddPointMidSpray "));
				}
				TimeWarning val2 = TimeWarning.New("Server_AddPointMidSpray", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg2 = rPCMessage;
						Server_AddPointMidSpray(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in Server_AddPointMidSpray");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 117883393 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_FinishEditing "));
				}
				TimeWarning val4 = TimeWarning.New("Server_FinishEditing", 0);
				try
				{
					TimeWarning val5 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg3 = rPCMessage;
						Server_FinishEditing(msg3);
					}
					finally
					{
						((IDisposable)val5)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in Server_FinishEditing");
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
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

	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (LinePoints == null || LinePoints.Count == 0)
		{
			Kill();
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (info.msg.sprayLine == null)
		{
			info.msg.sprayLine = Pool.Get<SprayLine>();
		}
		if (info.msg.sprayLine.linePoints == null)
		{
			info.msg.sprayLine.linePoints = Pool.GetList<LinePoint>();
		}
		bool flag = AcceptingChanges && info.forDisk;
		if (LinePoints != null && !flag)
		{
			CopyPoints(LinePoints, info.msg.sprayLine.linePoints);
		}
		info.msg.sprayLine.width = width;
		info.msg.sprayLine.colour = new Vector3(colour.r, colour.g, colour.b);
		if (!info.forDisk)
		{
			info.msg.sprayLine.editingPlayer = editingPlayer.uid;
		}
	}

	public void SetColour(Color newColour)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		colour = newColour;
	}

	public void SetWidth(float lineWidth)
	{
		width = lineWidth;
	}

	[RPC_Server]
	private void Server_AddPointMidSpray(RPCMessage msg)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		if (AcceptingChanges && !((Object)(object)editingPlayer.Get(serverside: true) != (Object)(object)msg.player) && LinePoints.Count + 1 <= 60)
		{
			Vector3 val = msg.read.Vector3();
			Vector3 worldNormal = msg.read.Vector3();
			if (!(Vector3.Distance(val, LinePoints[0].LocalPosition) >= 10f))
			{
				LinePoints.Add(new AlignedLineDrawer.LinePoint
				{
					LocalPosition = val,
					WorldNormal = worldNormal
				});
				UpdateGroundWatch();
				SendNetworkUpdate();
			}
		}
	}

	public void EnableChanges(BasePlayer byPlayer)
	{
		base.OwnerID = byPlayer.userID;
		editingPlayer.Set(byPlayer);
		((FacepunchBehaviour)this).Invoke((Action)TimeoutEditing, 30f);
	}

	private void TimeoutEditing()
	{
		if (editingPlayer.IsSet)
		{
			editingPlayer.Set(null);
			SendNetworkUpdate();
			Kill();
		}
	}

	[RPC_Server]
	private void Server_FinishEditing(RPCMessage msg)
	{
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = editingPlayer.Get(serverside: true);
		if ((Object)(object)msg.player != (Object)(object)basePlayer)
		{
			return;
		}
		bool allowNewSprayImmediately = msg.read.Int32() == 1;
		if ((Object)(object)basePlayer != (Object)null && (Object)(object)basePlayer.GetHeldEntity() != (Object)null && basePlayer.GetHeldEntity() is SprayCan sprayCan)
		{
			sprayCan.ClearPaintingLine(allowNewSprayImmediately);
		}
		editingPlayer.Set(null);
		SprayList val = SprayList.Deserialize((Stream)(object)msg.read);
		int count = val.linePoints.Count;
		if (count > 70)
		{
			Kill();
			Pool.FreeList<LinePoint>(ref val.linePoints);
			Pool.Free<SprayList>(ref val);
			return;
		}
		if (LinePoints.Count <= 1)
		{
			Kill();
			Pool.FreeList<LinePoint>(ref val.linePoints);
			Pool.Free<SprayList>(ref val);
			return;
		}
		((FacepunchBehaviour)this).CancelInvoke((Action)TimeoutEditing);
		LinePoints.Clear();
		for (int i = 0; i < count; i++)
		{
			if (((Vector3)(ref val.linePoints[i].localPosition)).sqrMagnitude < 100f)
			{
				LinePoints.Add(new AlignedLineDrawer.LinePoint
				{
					LocalPosition = val.linePoints[i].localPosition,
					WorldNormal = val.linePoints[i].worldNormal
				});
			}
		}
		OnDeployed(null, basePlayer, null);
		UpdateGroundWatch();
		Pool.FreeList<LinePoint>(ref val.linePoints);
		Pool.Free<SprayList>(ref val);
		SendNetworkUpdate();
	}

	public void AddInitialPoint(Vector3 atNormal)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		LinePoints = new List<AlignedLineDrawer.LinePoint>
		{
			new AlignedLineDrawer.LinePoint
			{
				LocalPosition = Vector3.zero,
				WorldNormal = atNormal
			}
		};
	}

	private void UpdateGroundWatch()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer && LinePoints.Count > 1)
		{
			Vector3 groundPosition = Vector3.Lerp(LinePoints[0].LocalPosition, LinePoints[LinePoints.Count - 1].LocalPosition, 0.5f);
			if ((Object)(object)groundWatch != (Object)null)
			{
				groundWatch.groundPosition = groundPosition;
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.sprayLine != null)
		{
			if (info.msg.sprayLine.linePoints != null)
			{
				LinePoints.Clear();
				CopyPoints(info.msg.sprayLine.linePoints, LinePoints);
			}
			colour = new Color(info.msg.sprayLine.colour.x, info.msg.sprayLine.colour.y, info.msg.sprayLine.colour.z);
			width = info.msg.sprayLine.width;
			editingPlayer.uid = info.msg.sprayLine.editingPlayer;
			UpdateGroundWatch();
		}
	}

	private void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<LinePoint> to)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint item in from)
		{
			LinePoint val = Pool.Get<LinePoint>();
			val.localPosition = item.LocalPosition;
			val.worldNormal = item.WorldNormal;
			to.Add(val);
		}
	}

	private void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<Vector3> to)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint item in from)
		{
			to.Add(item.LocalPosition);
			to.Add(item.WorldNormal);
		}
	}

	private void CopyPoints(List<LinePoint> from, List<AlignedLineDrawer.LinePoint> to)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		to.Clear();
		foreach (LinePoint item in from)
		{
			to.Add(new AlignedLineDrawer.LinePoint
			{
				LocalPosition = item.localPosition,
				WorldNormal = item.worldNormal
			});
		}
	}

	public static void CopyPoints(List<AlignedLineDrawer.LinePoint> from, List<AlignedLineDrawer.LinePoint> to)
	{
		to.Clear();
		foreach (AlignedLineDrawer.LinePoint item in from)
		{
			to.Add(item);
		}
	}

	public override void ResetState()
	{
		base.ResetState();
		editingPlayer.Set(null);
	}
}
