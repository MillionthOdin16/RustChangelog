using System;
using Rust.Interpolation;
using UnityEngine;

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
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
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
		interpolator.Cull(lerpTime - num);
	}

	public void Snapshot(Vector3 position, Quaternion rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Snapshot(position, rotation, LerpTime - TimeOffset);
	}

	public void SnapTo(Vector3 position, Quaternion rotation, float serverTime)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		interpolator.Clear();
		Snapshot(position, rotation, serverTime);
		target.SetNetworkPosition(position);
		target.SetNetworkRotation(rotation);
	}

	public void SnapTo(Vector3 position, Quaternion rotation)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		interpolator.last = new TransformSnapshot(LerpTime, position, rotation);
		Wipe();
	}

	public void SnapToEnd()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		float interpolationDelay = target.GetInterpolationDelay();
		Interpolator<TransformSnapshot>.Segment segment = interpolator.Query(LerpTime, interpolationDelay, 0f, 0f, ref snapshotPrototype);
		target.SetNetworkPosition(segment.tick.pos);
		target.SetNetworkRotation(segment.tick.rot);
		Wipe();
	}

	public void Wipe()
	{
		interpolator.Clear();
		timeOffsetCount = 0;
		timeOffset0 = float.MaxValue;
		timeOffset1 = float.MaxValue;
		timeOffset2 = float.MaxValue;
		timeOffset3 = float.MaxValue;
	}

	public static void WipeAll()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<PositionLerp> enumerator = InstanceList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.Wipe();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	protected void DoCycle()
	{
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		if (target == null)
		{
			return;
		}
		float interpolationInertia = target.GetInterpolationInertia();
		float num = ((interpolationInertia > 0f) ? Mathf.InverseLerp(0f, interpolationInertia, LerpTime - enabledTime) : 1f);
		float extrapolationTime = target.GetExtrapolationTime();
		float interpolation = target.GetInterpolationDelay() * num;
		float num2 = target.GetInterpolationSmoothing() * num;
		Interpolator<TransformSnapshot>.Segment segment = interpolator.Query(LerpTime, interpolation, extrapolationTime, num2, ref snapshotPrototype);
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
		target.SetNetworkPosition(segment.tick.pos);
		target.SetNetworkRotation(segment.tick.rot);
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
	}

	public void TransformEntries(Matrix4x4 matrix)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
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
