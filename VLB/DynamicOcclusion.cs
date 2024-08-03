using System;
using UnityEngine;

namespace VLB;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(VolumetricLightBeam))]
[HelpURL("http://saladgamer.com/vlb-doc/comp-dynocclusion/")]
public class DynamicOcclusion : MonoBehaviour
{
	private enum Direction
	{
		Up,
		Right,
		Down,
		Left
	}

	public LayerMask layerMask = LayerMask.op_Implicit(-1);

	public float minOccluderArea = 0f;

	public int waitFrameCount = 3;

	public float minSurfaceRatio = 0.5f;

	public float maxSurfaceDot = 0.25f;

	public PlaneAlignment planeAlignment = PlaneAlignment.Surface;

	public float planeOffset = 0.1f;

	private VolumetricLightBeam m_Master = null;

	private int m_FrameCountToWait = 0;

	private float m_RangeMultiplier = 1f;

	private uint m_PrevNonSubHitDirectionId = 0u;

	private void OnValidate()
	{
		minOccluderArea = Mathf.Max(minOccluderArea, 0f);
		waitFrameCount = Mathf.Clamp(waitFrameCount, 1, 60);
	}

	private void OnEnable()
	{
		m_Master = ((Component)this).GetComponent<VolumetricLightBeam>();
		Debug.Assert(Object.op_Implicit((Object)(object)m_Master));
	}

	private void OnDisable()
	{
		SetHitNull();
	}

	private void Start()
	{
		if (Application.isPlaying)
		{
			TriggerZone component = ((Component)this).GetComponent<TriggerZone>();
			if (Object.op_Implicit((Object)(object)component))
			{
				m_RangeMultiplier = Mathf.Max(1f, component.rangeMultiplier);
			}
		}
	}

	private void LateUpdate()
	{
		if (m_FrameCountToWait <= 0)
		{
			ProcessRaycasts();
			m_FrameCountToWait = waitFrameCount;
		}
		m_FrameCountToWait--;
	}

	private Vector3 GetRandomVectorAround(Vector3 direction, float angleDiff)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float num = angleDiff * 0.5f;
		return Quaternion.Euler(Random.Range(0f - num, num), Random.Range(0f - num, num), Random.Range(0f - num, num)) * direction;
	}

	private RaycastHit GetBestHit(Vector3 rayPos, Vector3 rayDir)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit[] array = Physics.RaycastAll(rayPos, rayDir, m_Master.fadeEnd * m_RangeMultiplier, ((LayerMask)(ref layerMask)).value);
		int num = -1;
		float num2 = float.MaxValue;
		for (int i = 0; i < array.Length; i++)
		{
			if (!((RaycastHit)(ref array[i])).collider.isTrigger && ((RaycastHit)(ref array[i])).collider.bounds.GetMaxArea2D() >= minOccluderArea && ((RaycastHit)(ref array[i])).distance < num2)
			{
				num2 = ((RaycastHit)(ref array[i])).distance;
				num = i;
			}
		}
		if (num != -1)
		{
			return array[num];
		}
		return default(RaycastHit);
	}

	private Vector3 GetDirection(uint dirInt)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		dirInt %= (uint)Enum.GetValues(typeof(Direction)).Length;
		return (Vector3)(dirInt switch
		{
			0u => ((Component)this).transform.up, 
			1u => ((Component)this).transform.right, 
			2u => -((Component)this).transform.up, 
			3u => -((Component)this).transform.right, 
			_ => Vector3.zero, 
		});
	}

	private bool IsHitValid(RaycastHit hit)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)((RaycastHit)(ref hit)).collider))
		{
			float num = Vector3.Dot(((RaycastHit)(ref hit)).normal, -((Component)this).transform.forward);
			return num >= maxSurfaceDot;
		}
		return false;
	}

	private void ProcessRaycasts()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit hit = GetBestHit(((Component)this).transform.position, ((Component)this).transform.forward);
		if (IsHitValid(hit))
		{
			if (minSurfaceRatio > 0.5f)
			{
				for (uint num = 0u; num < (uint)Enum.GetValues(typeof(Direction)).Length; num++)
				{
					Vector3 direction = GetDirection(num + m_PrevNonSubHitDirectionId);
					Vector3 val = ((Component)this).transform.position + direction * m_Master.coneRadiusStart * (minSurfaceRatio * 2f - 1f);
					Vector3 val2 = ((Component)this).transform.position + ((Component)this).transform.forward * m_Master.fadeEnd + direction * m_Master.coneRadiusEnd * (minSurfaceRatio * 2f - 1f);
					RaycastHit bestHit = GetBestHit(val, val2 - val);
					if (IsHitValid(bestHit))
					{
						if (((RaycastHit)(ref bestHit)).distance > ((RaycastHit)(ref hit)).distance)
						{
							hit = bestHit;
						}
						continue;
					}
					m_PrevNonSubHitDirectionId = num;
					SetHitNull();
					return;
				}
			}
			SetHit(hit);
		}
		else
		{
			SetHitNull();
		}
	}

	private void SetHit(RaycastHit hit)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		PlaneAlignment planeAlignment = this.planeAlignment;
		if (planeAlignment != 0 && planeAlignment == PlaneAlignment.Beam)
		{
			SetClippingPlane(new Plane(-((Component)this).transform.forward, ((RaycastHit)(ref hit)).point));
		}
		else
		{
			SetClippingPlane(new Plane(((RaycastHit)(ref hit)).normal, ((RaycastHit)(ref hit)).point));
		}
	}

	private void SetHitNull()
	{
		SetClippingPlaneOff();
	}

	private void SetClippingPlane(Plane planeWS)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		planeWS = planeWS.TranslateCustom(((Plane)(ref planeWS)).normal * planeOffset);
		m_Master.SetClippingPlane(planeWS);
	}

	private void SetClippingPlaneOff()
	{
		m_Master.SetClippingPlaneOff();
	}
}
