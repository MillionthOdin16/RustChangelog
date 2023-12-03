using UnityEngine;

namespace Rust.Interpolation;

public struct TransformSnapshot : ISnapshot<TransformSnapshot>
{
	public Vector3 pos;

	public Quaternion rot;

	public float Time { get; set; }

	public TransformSnapshot(float time, Vector3 pos, Quaternion rot)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Time = time;
		this.pos = pos;
		this.rot = rot;
	}

	public void MatchValuesTo(TransformSnapshot entry)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		pos = entry.pos;
		rot = entry.rot;
	}

	public void Lerp(TransformSnapshot prev, TransformSnapshot next, float delta)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		pos = Vector3.LerpUnclamped(prev.pos, next.pos, delta);
		rot = Quaternion.SlerpUnclamped(prev.rot, next.rot, delta);
	}

	public TransformSnapshot GetNew()
	{
		return default(TransformSnapshot);
	}
}
