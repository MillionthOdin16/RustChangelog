using System;
using UnityEngine;

public class m2bradleyAnimator : MonoBehaviour
{
	public Animator m2Animator;

	public Material treadLeftMaterial;

	public Material treadRightMaterial;

	private Rigidbody mainRigidbody;

	[Header("GunBones")]
	public Transform turret;

	public Transform mainCannon;

	public Transform coaxGun;

	public Transform rocketsPitch;

	public Transform spotLightYaw;

	public Transform spotLightPitch;

	public Transform sideMG;

	public Transform[] sideguns;

	[Header("WheelBones")]
	public Transform[] ShocksBones;

	public Transform[] ShockTraceLineBegin;

	public Vector3[] vecShocksOffsetPosition;

	[Header("Targeting")]
	public Transform targetTurret;

	public Transform targetSpotLight;

	public Transform[] targetSideguns;

	private Vector3 vecTurret = new Vector3(0f, 0f, 0f);

	private Vector3 vecMainCannon = new Vector3(0f, 0f, 0f);

	private Vector3 vecCoaxGun = new Vector3(0f, 0f, 0f);

	private Vector3 vecRocketsPitch = new Vector3(0f, 0f, 0f);

	private Vector3 vecSpotLightBase = new Vector3(0f, 0f, 0f);

	private Vector3 vecSpotLight = new Vector3(0f, 0f, 0f);

	private float sideMGPitchValue;

	[Header("MuzzleFlash locations")]
	public GameObject muzzleflashCannon;

	public GameObject muzzleflashCoaxGun;

	public GameObject muzzleflashSideMG;

	public GameObject[] muzzleflashRockets;

	public GameObject spotLightHaloSawnpoint;

	public GameObject[] muzzleflashSideguns;

	[Header("MuzzleFlash Particle Systems")]
	public GameObjectRef machineGunMuzzleFlashFX;

	public GameObjectRef mainCannonFireFX;

	public GameObjectRef rocketLaunchFX;

	[Header("Misc")]
	public bool rocketsOpen;

	public Vector3[] vecSideGunRotation;

	public float treadConstant = 0.14f;

	public float wheelSpinConstant = 80f;

	[Header("Gun Movement speeds")]
	public float sidegunsTurnSpeed = 30f;

	public float turretTurnSpeed = 6f;

	public float cannonPitchSpeed = 10f;

	public float rocketPitchSpeed = 20f;

	public float spotLightTurnSpeed = 60f;

	public float machineGunSpeed = 20f;

	private float wheelAngle;

	private void Start()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		mainRigidbody = ((Component)this).GetComponent<Rigidbody>();
		for (int i = 0; i < ShocksBones.Length; i++)
		{
			vecShocksOffsetPosition[i] = ShocksBones[i].localPosition;
		}
	}

	private void Update()
	{
		TrackTurret();
		TrackSpotLight();
		TrackSideGuns();
		AnimateWheelsTreads();
		AdjustShocksHeight();
		m2Animator.SetBool("rocketpods", rocketsOpen);
	}

	private void AnimateWheelsTreads()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if ((Object)(object)mainRigidbody != (Object)null)
		{
			num = Vector3.Dot(mainRigidbody.velocity, ((Component)this).transform.forward);
		}
		float num2 = Time.time * -1f * num * treadConstant % 1f;
		treadLeftMaterial.SetTextureOffset("_MainTex", new Vector2(num2, 0f));
		treadLeftMaterial.SetTextureOffset("_BumpMap", new Vector2(num2, 0f));
		treadLeftMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(num2, 0f));
		treadRightMaterial.SetTextureOffset("_MainTex", new Vector2(num2, 0f));
		treadRightMaterial.SetTextureOffset("_BumpMap", new Vector2(num2, 0f));
		treadRightMaterial.SetTextureOffset("_SpecGlossMap", new Vector2(num2, 0f));
		if (num >= 0f)
		{
			wheelAngle = (wheelAngle + Time.deltaTime * num * wheelSpinConstant) % 360f;
		}
		else
		{
			wheelAngle += Time.deltaTime * num * wheelSpinConstant;
			if (wheelAngle <= 0f)
			{
				wheelAngle = 360f;
			}
		}
		m2Animator.SetFloat("wheel_spin", wheelAngle);
		m2Animator.SetFloat("speed", num);
	}

	private void AdjustShocksHeight()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		Ray val = default(Ray);
		int mask = LayerMask.GetMask(new string[3] { "Terrain", "World", "Construction" });
		int num = ShocksBones.Length;
		float num2 = 0.55f;
		float num3 = 0.79f;
		float num4 = 0.26f;
		RaycastHit val2 = default(RaycastHit);
		for (int i = 0; i < num; i++)
		{
			((Ray)(ref val)).origin = ShockTraceLineBegin[i].position;
			((Ray)(ref val)).direction = ((Component)this).transform.up * -1f;
			num4 = ((!Physics.SphereCast(val, 0.15f, ref val2, num3, mask)) ? 0.26f : (((RaycastHit)(ref val2)).distance - num2));
			vecShocksOffsetPosition[i].y = Mathf.Lerp(vecShocksOffsetPosition[i].y, Mathf.Clamp(num4 * -1f, -0.26f, 0f), Time.deltaTime * 5f);
			ShocksBones[i].localPosition = vecShocksOffsetPosition[i];
		}
	}

	private void TrackTurret()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)targetTurret != (Object)null))
		{
			return;
		}
		Vector3 val = targetTurret.position - turret.position;
		_ = ((Vector3)(ref val)).normalized;
		CalculateYawPitchOffset(turret, turret.position, targetTurret.position, out var yaw, out var pitch);
		yaw = NormalizeYaw(yaw);
		float num = Time.deltaTime * turretTurnSpeed;
		if (yaw < -0.5f)
		{
			vecTurret.y = (vecTurret.y - num) % 360f;
		}
		else if (yaw > 0.5f)
		{
			vecTurret.y = (vecTurret.y + num) % 360f;
		}
		turret.localEulerAngles = vecTurret;
		float num2 = Time.deltaTime * cannonPitchSpeed;
		CalculateYawPitchOffset(mainCannon, mainCannon.position, targetTurret.position, out yaw, out pitch);
		if (pitch < -0.5f)
		{
			vecMainCannon.x -= num2;
		}
		else if (pitch > 0.5f)
		{
			vecMainCannon.x += num2;
		}
		vecMainCannon.x = Mathf.Clamp(vecMainCannon.x, -55f, 5f);
		mainCannon.localEulerAngles = vecMainCannon;
		if (pitch < -0.5f)
		{
			vecCoaxGun.x -= num2;
		}
		else if (pitch > 0.5f)
		{
			vecCoaxGun.x += num2;
		}
		vecCoaxGun.x = Mathf.Clamp(vecCoaxGun.x, -65f, 15f);
		coaxGun.localEulerAngles = vecCoaxGun;
		if (rocketsOpen)
		{
			num2 = Time.deltaTime * rocketPitchSpeed;
			CalculateYawPitchOffset(rocketsPitch, rocketsPitch.position, targetTurret.position, out yaw, out pitch);
			if (pitch < -0.5f)
			{
				vecRocketsPitch.x -= num2;
			}
			else if (pitch > 0.5f)
			{
				vecRocketsPitch.x += num2;
			}
			vecRocketsPitch.x = Mathf.Clamp(vecRocketsPitch.x, -45f, 45f);
		}
		else
		{
			vecRocketsPitch.x = Mathf.Lerp(vecRocketsPitch.x, 0f, Time.deltaTime * 1.7f);
		}
		rocketsPitch.localEulerAngles = vecRocketsPitch;
	}

	private void TrackSpotLight()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)targetSpotLight != (Object)null)
		{
			Vector3 val = targetSpotLight.position - spotLightYaw.position;
			_ = ((Vector3)(ref val)).normalized;
			CalculateYawPitchOffset(spotLightYaw, spotLightYaw.position, targetSpotLight.position, out var yaw, out var pitch);
			yaw = NormalizeYaw(yaw);
			float num = Time.deltaTime * spotLightTurnSpeed;
			if (yaw < -0.5f)
			{
				vecSpotLightBase.y = (vecSpotLightBase.y - num) % 360f;
			}
			else if (yaw > 0.5f)
			{
				vecSpotLightBase.y = (vecSpotLightBase.y + num) % 360f;
			}
			spotLightYaw.localEulerAngles = vecSpotLightBase;
			CalculateYawPitchOffset(spotLightPitch, spotLightPitch.position, targetSpotLight.position, out yaw, out pitch);
			if (pitch < -0.5f)
			{
				vecSpotLight.x -= num;
			}
			else if (pitch > 0.5f)
			{
				vecSpotLight.x += num;
			}
			vecSpotLight.x = Mathf.Clamp(vecSpotLight.x, -50f, 50f);
			spotLightPitch.localEulerAngles = vecSpotLight;
			m2Animator.SetFloat("sideMG_pitch", vecSpotLight.x, 0.5f, Time.deltaTime);
		}
	}

	private void TrackSideGuns()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < sideguns.Length; i++)
		{
			if (!((Object)(object)targetSideguns[i] == (Object)null))
			{
				Vector3 val = targetSideguns[i].position - sideguns[i].position;
				_ = ((Vector3)(ref val)).normalized;
				CalculateYawPitchOffset(sideguns[i], sideguns[i].position, targetSideguns[i].position, out var yaw, out var pitch);
				yaw = NormalizeYaw(yaw);
				float num = Time.deltaTime * sidegunsTurnSpeed;
				if (yaw < -0.5f)
				{
					vecSideGunRotation[i].y -= num;
				}
				else if (yaw > 0.5f)
				{
					vecSideGunRotation[i].y += num;
				}
				if (pitch < -0.5f)
				{
					vecSideGunRotation[i].x -= num;
				}
				else if (pitch > 0.5f)
				{
					vecSideGunRotation[i].x += num;
				}
				vecSideGunRotation[i].x = Mathf.Clamp(vecSideGunRotation[i].x, -45f, 45f);
				vecSideGunRotation[i].y = Mathf.Clamp(vecSideGunRotation[i].y, -45f, 45f);
				sideguns[i].localEulerAngles = vecSideGunRotation[i];
			}
		}
	}

	public void CalculateYawPitchOffset(Transform objectTransform, Vector3 vecStart, Vector3 vecEnd, out float yaw, out float pitch)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = objectTransform.InverseTransformDirection(vecEnd - vecStart);
		float num = Mathf.Sqrt(val.x * val.x + val.z * val.z);
		pitch = (0f - Mathf.Atan2(val.y, num)) * (180f / (float)Math.PI);
		Vector3 val2 = vecEnd - vecStart;
		val = ((Vector3)(ref val2)).normalized;
		Vector3 forward = objectTransform.forward;
		forward.y = 0f;
		((Vector3)(ref forward)).Normalize();
		float num2 = Vector3.Dot(val, forward);
		float num3 = Vector3.Dot(val, objectTransform.right);
		float num4 = 360f * num3;
		float num5 = 360f * (0f - num2);
		yaw = (Mathf.Atan2(num4, num5) + (float)Math.PI) * (180f / (float)Math.PI);
	}

	public float NormalizeYaw(float flYaw)
	{
		if (flYaw > 180f)
		{
			return 360f - flYaw;
		}
		return flYaw * -1f;
	}
}
