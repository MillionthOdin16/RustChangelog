using System.Collections;
using UnityEngine;
using UnityEngine.Profiling;

namespace Rust.Ai;

public class CoverPoint
{
	public enum CoverType
	{
		Full,
		Partial,
		None
	}

	public CoverType NormalCoverType;

	public bool IsDynamic = false;

	public Transform SourceTransform;

	private Vector3 _staticPosition;

	private Vector3 _staticNormal;

	public CoverPointVolume Volume { get; private set; }

	public Vector3 Position
	{
		get
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (IsDynamic && (Object)(object)SourceTransform != (Object)null)
			{
				return SourceTransform.position;
			}
			return _staticPosition;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			_staticPosition = value;
		}
	}

	public Vector3 Normal
	{
		get
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if (IsDynamic && (Object)(object)SourceTransform != (Object)null)
			{
				return SourceTransform.forward;
			}
			return _staticNormal;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			_staticNormal = value;
		}
	}

	public BaseEntity ReservedFor { get; set; }

	public bool IsReserved => (Object)(object)ReservedFor != (Object)null;

	public bool IsCompromised { get; set; }

	public float Score { get; set; }

	public bool IsValidFor(BaseEntity entity)
	{
		return !IsCompromised && ((Object)(object)ReservedFor == (Object)null || (Object)(object)ReservedFor == (Object)(object)entity);
	}

	public CoverPoint(CoverPointVolume volume, float score)
	{
		Volume = volume;
		Score = score;
	}

	public void CoverIsCompromised(float cooldown)
	{
		if (!IsCompromised && (Object)(object)Volume != (Object)null)
		{
			((MonoBehaviour)Volume).StartCoroutine(StartCooldown(cooldown));
		}
	}

	private IEnumerator StartCooldown(float cooldown)
	{
		IsCompromised = true;
		yield return CoroutineEx.waitForSeconds(cooldown);
		IsCompromised = false;
	}

	public bool ProvidesCoverFromPoint(Vector3 point, float arcThreshold)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("ProvidesCoverFromPoint");
		Vector3 val = Position - point;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		float num = Vector3.Dot(Normal, normalized);
		Profiler.EndSample();
		return num < arcThreshold;
	}
}
