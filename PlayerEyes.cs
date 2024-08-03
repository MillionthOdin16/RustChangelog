using UnityEngine;

public class PlayerEyes : EntityComponent<BasePlayer>
{
	public static readonly Vector3 EyeOffset = new Vector3(0f, 1.5f, 0f);

	public static readonly Vector3 DuckOffset = new Vector3(0f, -0.6f, 0f);

	public static readonly Vector3 CrawlOffset = new Vector3(0f, -1.15f, 0.175f);

	public Vector3 thirdPersonSleepingOffset = new Vector3(0.43f, 1.25f, 0.7f);

	public LazyAimProperties defaultLazyAim;

	private Vector3 viewOffset = Vector3.zero;

	public Vector3 worldMountedPosition
	{
		get
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)base.baseEntity) && base.baseEntity.isMounted)
			{
				Vector3 val = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity, GetLookRotation());
				if (val != Vector3.zero)
				{
					return val;
				}
			}
			return worldStandingPosition;
		}
	}

	public Vector3 worldStandingPosition => ((Component)this).transform.position + EyeOffset;

	public Vector3 worldCrouchedPosition => worldStandingPosition + DuckOffset;

	public Vector3 worldCrawlingPosition => worldStandingPosition + CrawlOffset;

	public Vector3 position
	{
		get
		{
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)base.baseEntity) && base.baseEntity.isMounted)
			{
				Vector3 val = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity, GetLookRotation());
				if (val != Vector3.zero)
				{
					return val;
				}
				return ((Component)this).transform.position + ((Component)this).transform.up * (EyeOffset.y + viewOffset.y) + BodyLeanOffset;
			}
			return ((Component)this).transform.position + ((Component)this).transform.rotation * (EyeOffset + viewOffset) + BodyLeanOffset;
		}
	}

	private Vector3 BodyLeanOffset => Vector3.zero;

	public Vector3 center
	{
		get
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			if (Object.op_Implicit((Object)(object)base.baseEntity) && base.baseEntity.isMounted)
			{
				Vector3 val = base.baseEntity.GetMounted().EyeCenterForPlayer(base.baseEntity, GetLookRotation());
				if (val != Vector3.zero)
				{
					return val;
				}
			}
			return ((Component)this).transform.position + ((Component)this).transform.up * (EyeOffset.y + DuckOffset.y);
		}
	}

	public Vector3 offset => ((Component)this).transform.up * (EyeOffset.y + viewOffset.y);

	public Quaternion rotation
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return parentRotation * bodyRotation;
		}
		set
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			bodyRotation = Quaternion.Inverse(parentRotation) * value;
		}
	}

	public Quaternion bodyRotation { get; set; }

	public Quaternion parentRotation
	{
		get
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			Quaternion result;
			if (base.baseEntity.isMounted || !((Object)(object)((Component)this).transform.parent != (Object)null))
			{
				result = Quaternion.identity;
			}
			else
			{
				Quaternion val = ((Component)this).transform.parent.rotation;
				result = Quaternion.Euler(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f);
			}
			return result;
		}
	}

	public void NetworkUpdate(Quaternion rot)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (base.baseEntity.IsCrawling())
		{
			viewOffset = CrawlOffset;
		}
		else if (base.baseEntity.IsDucked())
		{
			viewOffset = DuckOffset;
		}
		else
		{
			viewOffset = Vector3.zero;
		}
		bodyRotation = rot;
	}

	public Vector3 MovementForward()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = rotation;
		return Quaternion.Euler(new Vector3(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f)) * Vector3.forward;
	}

	public Vector3 MovementRight()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = rotation;
		return Quaternion.Euler(new Vector3(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f)) * Vector3.right;
	}

	public Ray BodyRay()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return new Ray(position, BodyForward());
	}

	public Vector3 BodyForward()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return rotation * Vector3.forward;
	}

	public Vector3 BodyRight()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return rotation * Vector3.right;
	}

	public Vector3 BodyUp()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return rotation * Vector3.up;
	}

	public Ray HeadRay()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return new Ray(position, HeadForward());
	}

	public Vector3 HeadForward()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return GetLookRotation() * Vector3.forward;
	}

	public Vector3 HeadRight()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return GetLookRotation() * Vector3.right;
	}

	public Vector3 HeadUp()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return GetLookRotation() * Vector3.up;
	}

	public Quaternion GetLookRotation()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return rotation;
	}

	public Quaternion GetAimRotation()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return rotation;
	}
}
