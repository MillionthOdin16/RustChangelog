using UnityEngine;

namespace Rust.Ai;

public class ManualCoverPoint : FacepunchBehaviour
{
	public bool IsDynamic = false;

	public float Score = 2f;

	public CoverPointVolume Volume;

	public Vector3 Normal;

	public CoverPoint.CoverType NormalCoverType;

	public Vector3 Position => ((Component)this).transform.position;

	public float DirectionMagnitude
	{
		get
		{
			if ((Object)(object)Volume != (Object)null)
			{
				return Volume.CoverPointRayLength;
			}
			return 1f;
		}
	}

	private void Awake()
	{
		if ((Object)(object)((Component)this).transform.parent != (Object)null)
		{
			Volume = ((Component)((Component)this).transform.parent).GetComponent<CoverPointVolume>();
		}
	}

	public CoverPoint ToCoverPoint(CoverPointVolume volume)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Volume = volume;
		if (IsDynamic)
		{
			CoverPoint obj = new CoverPoint(Volume, Score)
			{
				IsDynamic = true,
				SourceTransform = ((Component)this).transform,
				NormalCoverType = NormalCoverType
			};
			Transform transform = ((Component)this).transform;
			obj.Position = ((transform != null) ? transform.position : Vector3.zero);
			return obj;
		}
		Vector3 val = ((Component)this).transform.rotation * Normal;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		return new CoverPoint(Volume, Score)
		{
			IsDynamic = false,
			Position = ((Component)this).transform.position,
			Normal = normalized,
			NormalCoverType = NormalCoverType
		};
	}
}
