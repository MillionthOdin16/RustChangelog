using System;
using Rust;
using UnityEngine;

public class SkyLantern : StorageContainer, IIgniteable
{
	public float gravityScale = -0.1f;

	public float travelSpeed = 2f;

	public float collisionRadius = 0.5f;

	public float rotationSpeed = 5f;

	public float randOffset = 1f;

	public float lifeTime = 120f;

	public float hoverHeight = 14f;

	public Transform collisionCheckPoint;

	private float idealAltitude;

	private Vector3 travelVec = Vector3.forward;

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public override void ServerInit()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		randOffset = ((Random.Range(0.5f, 1f) * (float)Random.Range(0, 2) == 1f) ? (-1f) : 1f);
		Vector3 val = Vector3.forward + Vector3.right * randOffset;
		travelVec = ((Vector3)(ref val)).normalized;
		((FacepunchBehaviour)this).Invoke((Action)StartSinking, lifeTime - 15f);
		((FacepunchBehaviour)this).Invoke((Action)SelfDestroy, lifeTime);
		travelSpeed = Random.Range(1.75f, 2.25f);
		gravityScale *= Random.Range(1f, 1.25f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateIdealAltitude, 0f, 1f);
	}

	public void Ignite(Vector3 fromPos)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).gameObject.transform.RemoveComponent<GroundWatch>();
		((Component)this).gameObject.transform.RemoveComponent<DestroyOnGroundMissing>();
		((Component)this).gameObject.layer = 14;
		travelVec = Vector3Ex.Direction2D(((Component)this).transform.position, fromPos);
		SetFlag(Flags.On, b: true);
		UpdateIdealAltitude();
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base.OnAttacked(info);
		if (base.isServer)
		{
			if (info.damageTypes.Has(DamageType.Heat) && CanIgnite())
			{
				Ignite(info.PointStart);
			}
			else if (IsOn() && !IsBroken())
			{
				StartSinking();
			}
		}
	}

	public void SelfDestroy()
	{
		Kill();
	}

	public bool CanIgnite()
	{
		if (!IsOn())
		{
			return !IsBroken();
		}
		return false;
	}

	public void UpdateIdealAltitude()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (IsOn())
		{
			float num = TerrainMeta.HeightMap?.GetHeight(((Component)this).transform.position) ?? 0f;
			float num2 = TerrainMeta.WaterMap?.GetHeight(((Component)this).transform.position) ?? 0f;
			idealAltitude = Mathf.Max(num, num2) + hoverHeight;
			if (hoverHeight != 0f)
			{
				idealAltitude -= 2f * Mathf.Abs(randOffset);
			}
		}
	}

	public void StartSinking()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!IsBroken())
		{
			hoverHeight = 0f;
			travelVec = Vector3.zero;
			UpdateIdealAltitude();
			SetFlag(Flags.Broken, b: true);
		}
	}

	public void FixedUpdate()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isClient && IsOn())
		{
			float num = Mathf.Abs(((Component)this).transform.position.y - idealAltitude);
			float num2 = ((((Component)this).transform.position.y < idealAltitude) ? (-1f) : 1f);
			float num3 = Mathf.InverseLerp(0f, 10f, num) * num2;
			if (IsBroken())
			{
				travelVec = Vector3.Lerp(travelVec, Vector3.zero, Time.fixedDeltaTime * 0.5f);
				num3 = 0.7f;
			}
			Vector3 zero = Vector3.zero;
			zero = Vector3.up * gravityScale * Physics.gravity.y * num3;
			zero += travelVec * travelSpeed;
			Vector3 val = ((Component)this).transform.position + zero * Time.fixedDeltaTime;
			Vector3 val2 = Vector3Ex.Direction(val, ((Component)this).transform.position);
			float num4 = Vector3.Distance(val, ((Component)this).transform.position);
			RaycastHit val3 = default(RaycastHit);
			if (!Physics.SphereCast(collisionCheckPoint.position, collisionRadius, val2, ref val3, num4, 1218519297))
			{
				((Component)this).transform.position = val;
				((Component)this).transform.Rotate(Vector3.up, rotationSpeed * randOffset * Time.deltaTime, (Space)1);
			}
			else
			{
				StartSinking();
			}
		}
	}
}
