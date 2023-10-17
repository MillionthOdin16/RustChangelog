using System;
using UnityEngine;

public static class Ballistics
{
	private struct TheoreticalProjectile
	{
		public Vector3 pos;

		public Vector3 forward;

		public float gravity;

		public TheoreticalProjectile(Vector3 pos, Vector3 forward, float gravity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			this.pos = pos;
			this.forward = forward;
			this.gravity = gravity;
		}
	}

	public static Vector3 GetAimToTarget(Vector3 origin, Vector3 target, float speed, float maxAngle, float idealGravity, out float requiredGravity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return GetAimToTarget(origin, target, speed, maxAngle, idealGravity, 0f, out requiredGravity);
	}

	public static Vector3 GetAimToTarget(Vector3 origin, Vector3 target, float speed, float maxAngle, float idealGravity, float minRange, out float requiredGravity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		requiredGravity = idealGravity;
		Vector3 val = target - origin;
		float num = Vector3Ex.Magnitude2D(val);
		float y = val.y;
		float num2 = Mathf.Sqrt(speed * speed * speed * speed - requiredGravity * (requiredGravity * (num * num) + 2f * y * speed * speed));
		float num3 = Mathf.Atan((speed * speed + num2) / (requiredGravity * num)) * 57.29578f;
		float num4 = Mathf.Clamp(num3, 0f, 90f);
		if (float.IsNaN(num3))
		{
			num4 = 45f;
			requiredGravity = ProjectileDistToGravity(num, y, num4, speed);
		}
		else if (num3 > maxAngle)
		{
			num4 = maxAngle;
			requiredGravity = ProjectileDistToGravity(Mathf.Max(num, minRange), y, num4, speed);
		}
		((Vector3)(ref val)).Normalize();
		val.y = 0f;
		Vector3 val2 = Vector3.Cross(val, Vector3.up);
		val = Quaternion.AngleAxis(num4, val2) * val;
		return val;
	}

	public static Vector3 GetPhysicsProjectileHitPos(Vector3 origin, Vector3 direction, float speed, float gravity, float flightTimePerUpwardCheck = 2f, float flightTimePerDownwardCheck = 0.66f, float maxRays = 128f, BaseNetworkable owner = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		TheoreticalProjectile projectile = new TheoreticalProjectile(origin, direction * speed, gravity);
		int num = 0;
		float dt = ((projectile.forward.y > 0f) ? flightTimePerUpwardCheck : flightTimePerDownwardCheck);
		while (!NextRayHitSomething(ref projectile, dt, owner) && (float)num < maxRays)
		{
			num++;
		}
		return projectile.pos;
	}

	public static Vector3 GetBulletHitPoint(Vector3 origin, Vector3 direction)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetBulletHitPoint(new Ray(origin, direction));
	}

	public static Vector3 GetBulletHitPoint(Ray aimRay)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (GamePhysics.Trace(aimRay, 0f, out var hitInfo, 300f, 1220225809, (QueryTriggerInteraction)0))
		{
			return ((RaycastHit)(ref hitInfo)).point;
		}
		return ((Ray)(ref aimRay)).origin + ((Ray)(ref aimRay)).direction * 300f;
	}

	private static bool NextRayHitSomething(ref TheoreticalProjectile projectile, float dt, BaseNetworkable owner)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		float gravity = projectile.gravity;
		Vector3 pos = projectile.pos;
		float num = Vector3Ex.MagnitudeXZ(projectile.forward) * dt;
		float num2 = projectile.forward.y * dt + gravity * dt * dt * 0.5f;
		Vector2 val = Vector3Ex.XZ2D(projectile.forward);
		Vector2 val2 = ((Vector2)(ref val)).normalized * num;
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(val2.x, num2, val2.y);
		ref Vector3 pos2 = ref projectile.pos;
		pos2 += val3;
		float y = projectile.forward.y + gravity * dt;
		projectile.forward.y = y;
		RaycastHit hit = default(RaycastHit);
		if (Physics.Linecast(pos, projectile.pos, ref hit, 1084293393, (QueryTriggerInteraction)1))
		{
			projectile.pos = ((RaycastHit)(ref hit)).point;
			BaseEntity entity = hit.GetEntity();
			int num3;
			if ((Object)(object)entity != (Object)null)
			{
				num3 = (entity.EqualNetID(owner) ? 1 : 0);
				if (num3 != 0)
				{
					ref Vector3 pos3 = ref projectile.pos;
					pos3 += projectile.forward * 1f;
				}
			}
			else
			{
				num3 = 0;
			}
			return num3 == 0;
		}
		return false;
	}

	private static float ProjectileDistToGravity(float x, float y, float θ, float v)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		float num = θ * ((float)Math.PI / 180f);
		float num2 = (v * v * x * Mathf.Sin(2f * num) - 2f * v * v * y * Mathf.Cos(num) * Mathf.Cos(num)) / (x * x);
		if (float.IsNaN(num2) || num2 < 0.01f)
		{
			num2 = 0f - Physics.gravity.y;
		}
		return num2;
	}
}
