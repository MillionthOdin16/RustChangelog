using System;
using Rust.Interpolation;
using UnityEngine;
using UnityEngine.Profiling;

public class PositionLerp : IDisposable
{
	private static ListHashSet<PositionLerp> InstanceList = new ListHashSet<PositionLerp>(8);

	public static bool DebugLog = false;

	public static bool DebugDraw = false;

	public static int TimeOffsetInterval = 16;

	public static float TimeOffset = 0f;

	public const int TimeOffsetIntervalMin = 4;

	public const int TimeOffsetIntervalMax = 64;

	private bool enabled = true;

	private Action idleDisable;

	private Interpolator<TransformSnapshot> interpolator = new Interpolator<TransformSnapshot>(32);

	private IPosLerpTarget target;

	private static TransformSnapshot snapshotPrototype = default(TransformSnapshot);

	private float timeOffset0 = float.MaxValue;

	private float timeOffset1 = float.MaxValue;

	private float timeOffset2 = float.MaxValue;

	private float timeOffset3 = float.MaxValue;

	private int timeOffsetCount;

	private float lastClientTime;

	private float lastServerTime;

	private float extrapolatedTime;

	private float enabledTime;

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
			if (enabled)
			{
				OnEnable();
			}
			else
			{
				OnDisable();
			}
		}
	}

	public static float LerpTime => Time.time;

	private void OnEnable()
	{
		InstanceList.Add(this);
		enabledTime = LerpTime;
	}

	private void OnDisable()
	{
		InstanceList.Remove(this);
	}

	public void Initialize(IPosLerpTarget target)
	{
		this.target = target;
		Enabled = true;
	}

	public void Snapshot(Vector3 position, Quaternion rotation, float serverTime)
	{
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("PositionLerp.Snapshot");
		float interpolationDelay = target.GetInterpolationDelay();
		float interpolationSmoothing = target.GetInterpolationSmoothing();
		float num = interpolationDelay + interpolationSmoothing + 1f;
		float lerpTime = LerpTime;
		timeOffset0 = Mathf.Min(timeOffset0, lerpTime - serverTime);
		timeOffsetCount++;
		if (timeOffsetCount >= TimeOffsetInterval / 4)
		{
			timeOffset3 = timeOffset2;
			timeOffset2 = timeOffset1;
			timeOffset1 = timeOffset0;
			timeOffset0 = float.MaxValue;
			timeOffsetCount = 0;
		}
		TimeOffset = Mathx.Min(timeOffset0, timeOffset1, timeOffset2, timeOffset3);
		lerpTime = serverTime + TimeOffset;
		if (DebugLog && interpolator.list.Count > 0 && serverTime < lastServerTime)
		{
			Debug.LogWarning((object)(target.ToString() + " adding tick from the past: server time " + serverTime + " < " + lastServerTime));
		}
		else if (DebugLog && interpolator.list.Count > 0 && lerpTime < lastClientTime)
		{
			Debug.LogWarning((object)(target.ToString() + " adding tick from the past: client time " + lerpTime + " < " + lastClientTime));
		}
		else
		{
			lastClientTime = lerpTime;
			lastServerTime = serverTime;
			interpolator.Add(new TransformSnapshot(lerpTime, position, rotation));
		}
		Profiler.BeginSample("Cull");
		interpolator.Cull(lerpTime - num);
		Profiler.EndSample();
		Profiler.EndSample();
	}

	public void Snapshot(Vector3 position, Quaternion rotation)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Snapshot(position, rotation, LerpTime - TimeOffset);
	}

	public void SnapTo(Vector3 position, Quaternion rotation, float serverTime)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("PositionLerp.SnapTo");
		Profiler.BeginSample("TransformInterpolator.Clear");
		interpolator.Clear();
		Profiler.EndSample();
		Snapshot(position, rotation, serverTime);
		Profiler.BeginSample("SetNetworkPosition");
		target.SetNetworkPosition(position);
		Profiler.EndSample();
		Profiler.BeginSample("SetNetworkRotation");
		target.SetNetworkRotation(rotation);
		Profiler.EndSample();
		Profiler.EndSample();
	}

	public void SnapTo(Vector3 position, Quaternion rotation)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("PositionLerp.SnapTo");
		interpolator.last = new TransformSnapshot(LerpTime, position, rotation);
		Wipe();
		Profiler.EndSample();
	}

	public void SnapToEnd()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("PositionLerp.SnapToEnd");
		float interpolationDelay = target.GetInterpolationDelay();
		Profiler.BeginSample("TransformInterpolator.Query");
		Interpolator<TransformSnapshot>.Segment segment = interpolator.Query(LerpTime, interpolationDelay, 0f, 0f, ref snapshotPrototype);
		Profiler.EndSample();
		Profiler.BeginSample("SetNetworkPosition");
		target.SetNetworkPosition(segment.tick.pos);
		Profiler.EndSample();
		Profiler.BeginSample("SetNetworkRotation");
		target.SetNetworkRotation(segment.tick.rot);
		Profiler.EndSample();
		Wipe();
		Profiler.EndSample();
	}

	public void Wipe()
	{
		Profiler.BeginSample("TransformInterpolator.Clear");
		interpolator.Clear();
		timeOffsetCount = 0;
		timeOffset0 = float.MaxValue;
		timeOffset1 = float.MaxValue;
		timeOffset2 = float.MaxValue;
		timeOffset3 = float.MaxValue;
		Profiler.EndSample();
	}

	public static void WipeAll()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<PositionLerp> enumerator = InstanceList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				PositionLerp current = enumerator.Current;
				current.Wipe();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	protected void DoCycle()
	{
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		if (target == null)
		{
			return;
		}
		Profiler.BeginSample("PositionLerp.Update");
		float interpolationInertia = target.GetInterpolationInertia();
		float num = ((interpolationInertia > 0f) ? Mathf.InverseLerp(0f, interpolationInertia, LerpTime - enabledTime) : 1f);
		float extrapolationTime = target.GetExtrapolationTime();
		float interpolation = target.GetInterpolationDelay() * num;
		float num2 = target.GetInterpolationSmoothing() * num;
		Profiler.BeginSample("TransformInterpolator.Query");
		Interpolator<TransformSnapshot>.Segment segment = interpolator.Query(LerpTime, interpolation, extrapolationTime, num2, ref snapshotPrototype);
		Profiler.EndSample();
		Profiler.BeginSample("PositionLerp.Smoothing");
		if (segment.next.Time >= interpolator.last.Time)
		{
			extrapolatedTime = Mathf.Min(extrapolatedTime + Time.deltaTime, extrapolationTime);
		}
		else
		{
			extrapolatedTime = Mathf.Max(extrapolatedTime - Time.deltaTime, 0f);
		}
		if (extrapolatedTime > 0f && extrapolationTime > 0f && num2 > 0f)
		{
			float num3 = Time.deltaTime / (extrapolatedTime / extrapolationTime * num2);
			segment.tick.pos = Vector3.Lerp(target.GetNetworkPosition(), segment.tick.pos, num3);
			segment.tick.rot = Quaternion.Slerp(target.GetNetworkRotation(), segment.tick.rot, num3);
		}
		Profiler.EndSample();
		Profiler.BeginSample("SetNetworkPosition");
		target.SetNetworkPosition(segment.tick.pos);
		Profiler.EndSample();
		Profiler.BeginSample("SetNetworkRotation");
		target.SetNetworkRotation(segment.tick.rot);
		Profiler.EndSample();
		if (DebugDraw)
		{
			target.DrawInterpolationState(segment, interpolator.list);
		}
		if (LerpTime - lastClientTime > 10f)
		{
			if (idleDisable == null)
			{
				idleDisable = target.LerpIdleDisable;
			}
			IPosLerpTarget posLerpTarget = target;
			InvokeHandler.Invoke((Behaviour)((posLerpTarget is Behaviour) ? posLerpTarget : null), idleDisable, 0f);
		}
		Profiler.EndSample();
	}

	public void TransformEntries(Matrix4x4 matrix)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Quaternion rotation = ((Matrix4x4)(ref matrix)).rotation;
		for (int i = 0; i < interpolator.list.Count; i++)
		{
			TransformSnapshot value = interpolator.list[i];
			value.pos = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(value.pos);
			value.rot = rotation * value.rot;
			interpolator.list[i] = value;
		}
		interpolator.last.pos = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(interpolator.last.pos);
		interpolator.last.rot = rotation * interpolator.last.rot;
	}

	public Quaternion GetEstimatedAngularVelocity()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (target == null)
		{
			return Quaternion.identity;
		}
		float extrapolationTime = target.GetExtrapolationTime();
		float interpolationDelay = target.GetInterpolationDelay();
		float interpolationSmoothing = target.GetInterpolationSmoothing();
		Interpolator<TransformSnapshot>.Segment segment = interpolator.Query(LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing, ref snapshotPrototype);
		TransformSnapshot next = segment.next;
		TransformSnapshot prev = segment.prev;
		if (next.Time == prev.Time)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler((((Quaternion)(ref prev.rot)).eulerAngles - ((Quaternion)(ref next.rot)).eulerAngles) / (prev.Time - next.Time));
	}

	public Vector3 GetEstimatedVelocity()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (target == null)
		{
			return Vector3.zero;
		}
		float extrapolationTime = target.GetExtrapolationTime();
		float interpolationDelay = target.GetInterpolationDelay();
		float interpolationSmoothing = target.GetInterpolationSmoothing();
		Interpolator<TransformSnapshot>.Segment segment = interpolator.Query(LerpTime, interpolationDelay, extrapolationTime, interpolationSmoothing, ref snapshotPrototype);
		TransformSnapshot next = segment.next;
		TransformSnapshot prev = segment.prev;
		if (next.Time == prev.Time)
		{
			return Vector3.zero;
		}
		return (prev.pos - next.pos) / (prev.Time - next.Time);
	}

	public void Dispose()
	{
		target = null;
		idleDisable = null;
		interpolator.Clear();
		timeOffset0 = float.MaxValue;
		timeOffset1 = float.MaxValue;
		timeOffset2 = float.MaxValue;
		timeOffset3 = float.MaxValue;
		lastClientTime = 0f;
		lastServerTime = 0f;
		extrapolatedTime = 0f;
		timeOffsetCount = 0;
		Enabled = false;
	}

	public static void Clear()
	{
		InstanceList.Clear();
	}

	public static void Cycle()
	{
		PositionLerp[] buffer = InstanceList.Values.Buffer;
		int count = InstanceList.Count;
		for (int i = 0; i < count; i++)
		{
			buffer[i].DoCycle();
		}
	}
}
