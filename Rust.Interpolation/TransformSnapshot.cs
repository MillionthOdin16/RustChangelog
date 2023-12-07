using UnityEngine;

namespace Rust.Interpolation;

public struct TransformSnapshot : ISnapshot<TransformSnapshot>
{
	public Vector3 pos;

	public Quaternion rot;

	public float Time { get; set; }

	public TransformSnapshot(float time, Vector3 pos, Quaternion rot)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Time = time;
		this.pos = pos;
		this.rot = rot;
	}

	public void MatchValuesTo(TransformSnapshot entry)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		pos = entry.pos;
		rot = entry.rot;
	}

	public void Lerp(TransformSnapshot prev, TransformSnapshot next, float delta)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		pos = Vector3.LerpUnclamped(prev.pos, next.pos, delta);
		rot = Quaternion.SlerpUnclamped(prev.rot, next.rot, delta);
	}

	public TransformSnapshot GetNew()
	{
		return default(TransformSnapshot);
	}
}
