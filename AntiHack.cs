using System;
using System.Collections.Generic;
using ConVar;
using Epic.OnlineServices.Reports;
using Facepunch;
using Facepunch.Rust;
using UnityEngine;
using UnityEngine.Profiling;

public static class AntiHack
{
	private class GroupedLog : IPooled
	{
		public float firstLogTime;

		public string playerName;

		public AntiHackType antiHackType;

		public string message;

		public Vector3 averagePos;

		public int num;

		public GroupedLog()
		{
		}

		public GroupedLog(string playerName, AntiHackType antiHackType, string message, Vector3 pos)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			SetInitial(playerName, antiHackType, message, pos);
		}

		public void EnterPool()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			firstLogTime = 0f;
			playerName = string.Empty;
			antiHackType = AntiHackType.None;
			averagePos = Vector3.zero;
			num = 0;
		}

		public void LeavePool()
		{
		}

		public void SetInitial(string playerName, AntiHackType antiHackType, string message, Vector3 pos)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			firstLogTime = Time.unscaledTime;
			this.playerName = playerName;
			this.antiHackType = antiHackType;
			this.message = message;
			averagePos = pos;
			num = 1;
		}

		public bool TryGroup(string playerName, AntiHackType antiHackType, string message, Vector3 pos, float maxDistance)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			if (antiHackType != this.antiHackType || playerName != this.playerName || message != this.message)
			{
				return false;
			}
			if (Vector3.SqrMagnitude(averagePos - pos) > maxDistance * maxDistance)
			{
				return false;
			}
			Vector3 val = averagePos * (float)num;
			averagePos = (val + pos) / (float)(num + 1);
			num++;
			return true;
		}
	}

	private const int movement_mask = 429990145;

	private const int vehicle_mask = 8192;

	private const int grounded_mask = 1503731969;

	private const int player_mask = 131072;

	private static Collider[] buffer = (Collider[])(object)new Collider[4];

	private static Dictionary<ulong, int> kicks = new Dictionary<ulong, int>();

	private static Dictionary<ulong, int> bans = new Dictionary<ulong, int>();

	private const float LOG_GROUP_SECONDS = 60f;

	private static Queue<GroupedLog> groupedLogs = new Queue<GroupedLog>();

	public static RaycastHit isInsideRayHit;

	public static bool TestNoClipping(Vector3 oldPos, Vector3 newPos, float radius, float backtracking, bool sphereCast, out Collider collider, bool vehicleLayer = false, BaseEntity ignoreEntity = null)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		int num = 429990145;
		if (!vehicleLayer)
		{
			num &= -8193;
		}
		Vector3 val = newPos - oldPos;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 val2 = oldPos - normalized * backtracking;
		val = newPos - val2;
		float magnitude = ((Vector3)(ref val)).magnitude;
		Ray val3 = default(Ray);
		((Ray)(ref val3))._002Ector(val2, normalized);
		RaycastHit hitInfo = default(RaycastHit);
		bool flag = (((Object)(object)ignoreEntity == (Object)null) ? Physics.Raycast(val3, ref hitInfo, magnitude + radius, num, (QueryTriggerInteraction)1) : GamePhysics.Trace(val3, 0f, out hitInfo, magnitude + radius, num, (QueryTriggerInteraction)1, ignoreEntity));
		if (!flag && sphereCast)
		{
			flag = (((Object)(object)ignoreEntity == (Object)null) ? Physics.SphereCast(val3, radius, ref hitInfo, magnitude, num, (QueryTriggerInteraction)1) : GamePhysics.Trace(val3, radius, out hitInfo, magnitude, num, (QueryTriggerInteraction)1, ignoreEntity));
		}
		collider = ((RaycastHit)(ref hitInfo)).collider;
		return flag && GamePhysics.Verify(hitInfo);
	}

	public static void Cycle()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.unscaledTime - 60f;
		if (groupedLogs.Count <= 0)
		{
			return;
		}
		GroupedLog groupedLog = groupedLogs.Peek();
		while (groupedLog.firstLogTime <= num)
		{
			GroupedLog groupedLog2 = groupedLogs.Dequeue();
			LogToConsole(groupedLog2.playerName, groupedLog2.antiHackType, $"{groupedLog2.message} (x{groupedLog2.num})", groupedLog2.averagePos);
			Pool.Free<GroupedLog>(ref groupedLog2);
			if (groupedLogs.Count == 0)
			{
				break;
			}
			groupedLog = groupedLogs.Peek();
		}
	}

	public static void ResetTimer(BasePlayer ply)
	{
		ply.lastViolationTime = Time.realtimeSinceStartup;
	}

	public static bool ShouldIgnore(BasePlayer ply)
	{
		TimeWarning val = TimeWarning.New("AntiHack.ShouldIgnore", 0);
		try
		{
			if (ply.IsFlying)
			{
				ply.lastAdminCheatTime = Time.realtimeSinceStartup;
			}
			else if ((ply.IsAdmin || ply.IsDeveloper) && ply.lastAdminCheatTime == 0f)
			{
				ply.lastAdminCheatTime = Time.realtimeSinceStartup;
			}
			if (ply.IsAdmin)
			{
				if (ConVar.AntiHack.userlevel < 1)
				{
					return true;
				}
				if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat())
				{
					return true;
				}
			}
			if (ply.IsDeveloper)
			{
				if (ConVar.AntiHack.userlevel < 2)
				{
					return true;
				}
				if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat())
				{
					return true;
				}
			}
			if (ply.IsSpectating())
			{
				return true;
			}
			return false;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool ValidateMove(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("AntiHack.ValidateMove", 0);
		try
		{
			if (ShouldIgnore(ply))
			{
				return true;
			}
			bool flag = deltaTime > ConVar.AntiHack.maxdeltatime;
			if (IsNoClipping(ply, ticks, deltaTime, out var collider))
			{
				if (flag)
				{
					return false;
				}
				Analytics.Azure.OnNoclipViolation(ply, ticks.CurrentPoint, ticks.EndPoint, ticks.Count, collider);
				AddViolation(ply, AntiHackType.NoClip, ConVar.AntiHack.noclip_penalty * ticks.Length);
				if (ConVar.AntiHack.noclip_reject)
				{
					return false;
				}
			}
			if (IsSpeeding(ply, ticks, deltaTime))
			{
				if (flag)
				{
					return false;
				}
				Analytics.Azure.OnSpeedhackViolation(ply, ticks.CurrentPoint, ticks.EndPoint, ticks.Count);
				AddViolation(ply, AntiHackType.SpeedHack, ConVar.AntiHack.speedhack_penalty * ticks.Length);
				if (ConVar.AntiHack.speedhack_reject)
				{
					return false;
				}
			}
			if (IsFlying(ply, ticks, deltaTime))
			{
				if (flag)
				{
					return false;
				}
				Analytics.Azure.OnFlyhackViolation(ply, ticks.CurrentPoint, ticks.EndPoint, ticks.Count);
				AddViolation(ply, AntiHackType.FlyHack, ConVar.AntiHack.flyhack_penalty * ticks.Length);
				if (ConVar.AntiHack.flyhack_reject)
				{
					return false;
				}
			}
			return true;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static void ValidateEyeHistory(BasePlayer ply)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("AntiHack.ValidateEyeHistory", 0);
		try
		{
			for (int i = 0; i < ply.eyeHistory.Count; i++)
			{
				Vector3 val2 = ply.eyeHistory[i];
				if (ply.tickHistory.Distance(ply, val2) > ConVar.AntiHack.eye_history_forgiveness)
				{
					AddViolation(ply, AntiHackType.EyeHack, ConVar.AntiHack.eye_history_penalty);
					Analytics.Azure.OnEyehackViolation(ply, val2);
				}
			}
			ply.eyeHistory.Clear();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool IsInsideTerrain(BasePlayer ply)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return TestInsideTerrain(((Component)ply).transform.position);
	}

	public static bool TestInsideTerrain(Vector3 pos)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("AntiHack.TestInsideTerrain", 0);
		try
		{
			if (!Object.op_Implicit((Object)(object)TerrainMeta.Terrain))
			{
				return false;
			}
			if (!Object.op_Implicit((Object)(object)TerrainMeta.HeightMap))
			{
				return false;
			}
			if (!Object.op_Implicit((Object)(object)TerrainMeta.Collision))
			{
				return false;
			}
			float terrain_padding = ConVar.AntiHack.terrain_padding;
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			if (pos.y > height - terrain_padding)
			{
				return false;
			}
			float num = TerrainMeta.Position.y + TerrainMeta.Terrain.SampleHeight(pos);
			if (pos.y > num - terrain_padding)
			{
				return false;
			}
			if (TerrainMeta.Collision.GetIgnore(pos))
			{
				return false;
			}
			return true;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool IsInsideMesh(Vector3 pos)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		bool queriesHitBackfaces = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		if (Physics.Raycast(pos, Vector3.up, ref isInsideRayHit, 50f, 65537))
		{
			Physics.queriesHitBackfaces = queriesHitBackfaces;
			return Vector3.Dot(Vector3.up, ((RaycastHit)(ref isInsideRayHit)).normal) > 0f;
		}
		Physics.queriesHitBackfaces = queriesHitBackfaces;
		return false;
	}

	public static bool IsNoClipping(BasePlayer ply, TickInterpolator ticks, float deltaTime, out Collider collider)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		collider = null;
		TimeWarning val = TimeWarning.New("AntiHack.IsNoClipping", 0);
		try
		{
			ply.vehiclePauseTime = Mathf.Max(0f, ply.vehiclePauseTime - deltaTime);
			if (ConVar.AntiHack.noclip_protection <= 0)
			{
				return false;
			}
			ticks.Reset();
			if (!ticks.HasNext())
			{
				return false;
			}
			Profiler.BeginSample("GetPosition");
			bool flag = (Object)(object)((Component)ply).transform.parent == (Object)null;
			Matrix4x4 val2 = (flag ? Matrix4x4.identity : ((Component)ply).transform.parent.localToWorldMatrix);
			Vector3 val3 = (flag ? ticks.StartPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.StartPoint));
			Vector3 val4 = (flag ? ticks.EndPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.EndPoint));
			Vector3 val5 = ply.NoClipOffset();
			float radius = ply.NoClipRadius(ConVar.AntiHack.noclip_margin);
			float noclip_backtracking = ConVar.AntiHack.noclip_backtracking;
			bool vehicleLayer = ply.vehiclePauseTime <= 0f;
			Profiler.EndSample();
			if (ConVar.AntiHack.noclip_protection >= 3)
			{
				float num = Mathf.Max(ConVar.AntiHack.noclip_stepsize, 0.1f);
				int num2 = Mathf.Max(ConVar.AntiHack.noclip_maxsteps, 1);
				num = Mathf.Max(ticks.Length / (float)num2, num);
				while (ticks.MoveNext(num))
				{
					val4 = (flag ? ticks.CurrentPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.CurrentPoint));
					if (TestNoClipping(val3 + val5, val4 + val5, radius, noclip_backtracking, sphereCast: true, out collider, vehicleLayer))
					{
						return true;
					}
					val3 = val4;
				}
			}
			else if (ConVar.AntiHack.noclip_protection >= 2)
			{
				if (TestNoClipping(val3 + val5, val4 + val5, radius, noclip_backtracking, sphereCast: true, out collider, vehicleLayer))
				{
					return true;
				}
			}
			else if (TestNoClipping(val3 + val5, val4 + val5, radius, noclip_backtracking, sphereCast: false, out collider, vehicleLayer))
			{
				return true;
			}
			return false;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool IsSpeeding(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("AntiHack.IsSpeeding", 0);
		try
		{
			ply.speedhackPauseTime = Mathf.Max(0f, ply.speedhackPauseTime - deltaTime);
			if (ConVar.AntiHack.speedhack_protection <= 0)
			{
				return false;
			}
			Profiler.BeginSample("GetPosition");
			bool flag = (Object)(object)((Component)ply).transform.parent == (Object)null;
			Matrix4x4 val2 = (flag ? Matrix4x4.identity : ((Component)ply).transform.parent.localToWorldMatrix);
			Vector3 val3 = (flag ? ticks.StartPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.StartPoint));
			Vector3 val4 = (flag ? ticks.EndPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.EndPoint));
			Profiler.EndSample();
			float running = 1f;
			float ducking = 0f;
			float crawling = 0f;
			if (ConVar.AntiHack.speedhack_protection >= 2)
			{
				Profiler.BeginSample("IsRunning");
				bool flag2 = ply.IsRunning();
				Profiler.EndSample();
				Profiler.BeginSample("IsDucked");
				bool flag3 = ply.IsDucked();
				Profiler.EndSample();
				Profiler.BeginSample("IsSwimming");
				bool flag4 = ply.IsSwimming();
				Profiler.EndSample();
				Profiler.BeginSample("IsCrawling");
				bool flag5 = ply.IsCrawling();
				Profiler.EndSample();
				running = (flag2 ? 1f : 0f);
				ducking = ((flag3 || flag4) ? 1f : 0f);
				crawling = (flag5 ? 1f : 0f);
			}
			Profiler.BeginSample("GetSpeed");
			float speed = ply.GetSpeed(running, ducking, crawling);
			Profiler.EndSample();
			Profiler.BeginSample("GetDistance");
			Vector3 val5 = val4 - val3;
			float num = Vector3Ex.Magnitude2D(val5);
			float num2 = deltaTime * speed;
			Profiler.EndSample();
			if (num > num2)
			{
				Profiler.BeginSample("SlopeSpeedAdjustment");
				Vector3 val6 = (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetNormal(val3) : Vector3.up);
				float num3 = Mathf.Max(0f, Vector3.Dot(Vector3Ex.XZ3D(val6), Vector3Ex.XZ3D(val5)));
				float num4 = num3 * ConVar.AntiHack.speedhack_slopespeed * deltaTime;
				num = Mathf.Max(0f, num - num4);
				Profiler.EndSample();
			}
			float num5 = Mathf.Max((ply.speedhackPauseTime > 0f) ? ConVar.AntiHack.speedhack_forgiveness_inertia : ConVar.AntiHack.speedhack_forgiveness, 0.1f);
			float num6 = num5 + Mathf.Max(ConVar.AntiHack.speedhack_forgiveness, 0.1f);
			ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance, 0f - num6, num6);
			ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance - num2, 0f - num6, num6);
			if (ply.speedhackDistance > num5)
			{
				return true;
			}
			ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance + num, 0f - num6, num6);
			if (ply.speedhackDistance > num5)
			{
				return true;
			}
			return false;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool IsFlying(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("AntiHack.IsFlying", 0);
		try
		{
			ply.flyhackPauseTime = Mathf.Max(0f, ply.flyhackPauseTime - deltaTime);
			if (ConVar.AntiHack.flyhack_protection <= 0)
			{
				return false;
			}
			ticks.Reset();
			if (!ticks.HasNext())
			{
				return false;
			}
			Profiler.BeginSample("GetPosition");
			bool flag = (Object)(object)((Component)ply).transform.parent == (Object)null;
			Matrix4x4 val2 = (flag ? Matrix4x4.identity : ((Component)ply).transform.parent.localToWorldMatrix);
			Vector3 oldPos = (flag ? ticks.StartPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.StartPoint));
			Vector3 newPos = (flag ? ticks.EndPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.EndPoint));
			Profiler.EndSample();
			if (ConVar.AntiHack.flyhack_protection >= 3)
			{
				float num = Mathf.Max(ConVar.AntiHack.flyhack_stepsize, 0.1f);
				int num2 = Mathf.Max(ConVar.AntiHack.flyhack_maxsteps, 1);
				num = Mathf.Max(ticks.Length / (float)num2, num);
				while (ticks.MoveNext(num))
				{
					newPos = (flag ? ticks.CurrentPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.CurrentPoint));
					if (TestFlying(ply, oldPos, newPos, verifyGrounded: true))
					{
						return true;
					}
					oldPos = newPos;
				}
			}
			else if (ConVar.AntiHack.flyhack_protection >= 2)
			{
				if (TestFlying(ply, oldPos, newPos, verifyGrounded: true))
				{
					return true;
				}
			}
			else if (TestFlying(ply, oldPos, newPos, verifyGrounded: false))
			{
				return true;
			}
			return false;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool TestFlying(BasePlayer ply, Vector3 oldPos, Vector3 newPos, bool verifyGrounded)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		ply.isInAir = false;
		ply.isOnPlayer = false;
		if (verifyGrounded)
		{
			float flyhack_extrusion = ConVar.AntiHack.flyhack_extrusion;
			Vector3 val = (oldPos + newPos) * 0.5f;
			if (!ply.OnLadder() && !WaterLevel.Test(val - new Vector3(0f, flyhack_extrusion, 0f), waves: true, volumes: true, ply) && (EnvironmentManager.Get(val) & EnvironmentType.Elevator) == 0)
			{
				float flyhack_margin = ConVar.AntiHack.flyhack_margin;
				float radius = ply.GetRadius();
				float height = ply.GetHeight(ducked: false);
				Vector3 val2 = val + new Vector3(0f, radius - flyhack_extrusion, 0f);
				Vector3 val3 = val + new Vector3(0f, height - radius, 0f);
				float num = radius - flyhack_margin;
				ply.isInAir = !Physics.CheckCapsule(val2, val3, num, 1503731969, (QueryTriggerInteraction)1);
				if (ply.isInAir)
				{
					int num2 = Physics.OverlapCapsuleNonAlloc(val2, val3, num, buffer, 131072, (QueryTriggerInteraction)1);
					for (int i = 0; i < num2; i++)
					{
						BasePlayer basePlayer = ((Component)buffer[i]).gameObject.ToBaseEntity() as BasePlayer;
						if (!((Object)(object)basePlayer == (Object)null) && !((Object)(object)basePlayer == (Object)(object)ply) && !basePlayer.isInAir && !basePlayer.isOnPlayer && !basePlayer.TriggeredAntiHack() && !basePlayer.IsSleeping())
						{
							ply.isOnPlayer = true;
							ply.isInAir = false;
							break;
						}
					}
					for (int j = 0; j < buffer.Length; j++)
					{
						buffer[j] = null;
					}
				}
			}
		}
		else
		{
			ply.isInAir = !ply.OnLadder() && !ply.IsSwimming() && !ply.IsOnGround();
		}
		if (ply.isInAir)
		{
			bool flag = false;
			Vector3 val4 = newPos - oldPos;
			float num3 = Mathf.Abs(val4.y);
			float num4 = Vector3Ex.Magnitude2D(val4);
			if (val4.y >= 0f)
			{
				ply.flyhackDistanceVertical += val4.y;
				flag = true;
			}
			if (num3 < num4)
			{
				ply.flyhackDistanceHorizontal += num4;
				flag = true;
			}
			if (flag)
			{
				float num5 = Mathf.Max((ply.flyhackPauseTime > 0f) ? ConVar.AntiHack.flyhack_forgiveness_vertical_inertia : ConVar.AntiHack.flyhack_forgiveness_vertical, 0f);
				float num6 = ply.GetJumpHeight() + num5;
				if (ply.flyhackDistanceVertical > num6)
				{
					return true;
				}
				float num7 = Mathf.Max((ply.flyhackPauseTime > 0f) ? ConVar.AntiHack.flyhack_forgiveness_horizontal_inertia : ConVar.AntiHack.flyhack_forgiveness_horizontal, 0f);
				float num8 = 5f + num7;
				if (ply.flyhackDistanceHorizontal > num8)
				{
					return true;
				}
			}
		}
		else
		{
			ply.flyhackDistanceVertical = 0f;
			ply.flyhackDistanceHorizontal = 0f;
		}
		return false;
	}

	public static bool TestIsBuildingInsideSomething(Construction.Target target, Vector3 deployPos)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (ConVar.AntiHack.build_inside_check <= 0)
		{
			return false;
		}
		foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
		{
			if (monument.IsInBounds(deployPos))
			{
				return false;
			}
		}
		if (IsInsideMesh(deployPos))
		{
			Vector3 origin = ((Ray)(ref target.ray)).origin;
			if (IsInsideMesh(origin))
			{
				LogToConsoleBatched(target.player, AntiHackType.InsideGeometry, "Tried to build while clipped inside " + ((Object)((RaycastHit)(ref isInsideRayHit)).collider).name, 25f);
				if (ConVar.AntiHack.build_inside_check > 1)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void NoteAdminHack(BasePlayer ply)
	{
		Ban(ply, "Cheat Detected!");
	}

	public static void FadeViolations(BasePlayer ply, float deltaTime)
	{
		if (Time.realtimeSinceStartup - ply.lastViolationTime > ConVar.AntiHack.relaxationpause)
		{
			ply.violationLevel = Mathf.Max(0f, ply.violationLevel - ConVar.AntiHack.relaxationrate * deltaTime);
		}
	}

	public static void EnforceViolations(BasePlayer ply)
	{
		if (ConVar.AntiHack.enforcementlevel > 0 && ply.violationLevel > ConVar.AntiHack.maxviolation)
		{
			if (ConVar.AntiHack.debuglevel >= 1)
			{
				LogToConsole(ply, ply.lastViolationType, "Enforcing (violation of " + ply.violationLevel + ")");
			}
			string reason = string.Concat(ply.lastViolationType, " Violation Level ", ply.violationLevel);
			if (ConVar.AntiHack.enforcementlevel > 1)
			{
				Kick(ply, reason);
			}
			else
			{
				Kick(ply, reason);
			}
		}
	}

	public static void Log(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.debuglevel > 1)
		{
			LogToConsole(ply, type, message);
		}
		Analytics.Azure.OnAntihackViolation(ply, (int)type, message);
		LogToEAC(ply, type, message);
	}

	public static void LogToConsoleBatched(BasePlayer ply, AntiHackType type, string message, float maxDistance)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		string playerName = ((object)ply).ToString();
		Vector3 position = ((Component)ply).transform.position;
		foreach (GroupedLog groupedLog2 in groupedLogs)
		{
			if (groupedLog2.TryGroup(playerName, type, message, position, maxDistance))
			{
				return;
			}
		}
		GroupedLog groupedLog = Pool.Get<GroupedLog>();
		groupedLog.SetInitial(playerName, type, message, position);
		groupedLogs.Enqueue(groupedLog);
	}

	private static void LogToConsole(BasePlayer ply, AntiHackType type, string message)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Debug.LogWarning((object)string.Concat(ply, " ", type, ": ", message, " at ", ((Component)ply).transform.position));
	}

	private static void LogToConsole(string plyName, AntiHackType type, string message, Vector3 pos)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Debug.LogWarning((object)string.Concat(plyName, " ", type, ": ", message, " at ", pos));
	}

	private static void LogToEAC(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.reporting)
		{
			EACServer.SendPlayerBehaviorReport((PlayerReportsCategory)2, ply.UserIDString, string.Concat(type, ": ", message));
		}
	}

	public static void AddViolation(BasePlayer ply, AntiHackType type, float amount)
	{
		TimeWarning val = TimeWarning.New("AntiHack.AddViolation", 0);
		try
		{
			ply.lastViolationType = type;
			ply.lastViolationTime = Time.realtimeSinceStartup;
			ply.violationLevel += amount;
			if ((ConVar.AntiHack.debuglevel >= 2 && amount > 0f) || (ConVar.AntiHack.debuglevel >= 3 && type != AntiHackType.NoClip) || ConVar.AntiHack.debuglevel >= 4)
			{
				LogToConsole(ply, type, "Added violation of " + amount + " in frame " + Time.frameCount + " (now has " + ply.violationLevel + ")");
			}
			EnforceViolations(ply);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static void Kick(BasePlayer ply, string reason)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		AddRecord(ply, kicks);
		ConsoleSystem.Run(Option.Server, "kick", new object[2] { ply.userID, reason });
	}

	public static void Ban(BasePlayer ply, string reason)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		AddRecord(ply, bans);
		ConsoleSystem.Run(Option.Server, "ban", new object[2] { ply.userID, reason });
	}

	private static void AddRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		if (records.ContainsKey(ply.userID))
		{
			records[ply.userID]++;
		}
		else
		{
			records.Add(ply.userID, 1);
		}
	}

	public static int GetKickRecord(BasePlayer ply)
	{
		return GetRecord(ply, kicks);
	}

	public static int GetBanRecord(BasePlayer ply)
	{
		return GetRecord(ply, bans);
	}

	private static int GetRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		return records.ContainsKey(ply.userID) ? records[ply.userID] : 0;
	}
}
