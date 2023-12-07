using UnityEngine;

public class PlacementTest : MonoBehaviour
{
	public MeshCollider myMeshCollider;

	public Transform testTransform;

	public Transform visualTest;

	public float hemisphere = 45f;

	public float clampTest = 45f;

	public float testDist = 2f;

	private float nextTest;

	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(insideUnitCircle.x * degreesOffset, Random.Range(-1f, 1f) * degreesOffset, insideUnitCircle.y * degreesOffset);
		Vector3 val2 = input + val;
		return ((Vector3)(ref val2)).normalized;
	}

	public Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		Vector3 val = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		return new Vector3(normalized.x * distance, Random.Range(minHeight, maxHeight), normalized.z * distance);
	}

	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 val = hemiInput + Vector3.one * degreesOffset;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		val = hemiInput + Vector3.one * (0f - degreesOffset);
		Vector3 normalized2 = ((Vector3)(ref val)).normalized;
		for (int i = 0; i < 3; i++)
		{
			((Vector3)(ref inputVec))[i] = Mathf.Clamp(((Vector3)(ref inputVec))[i], ((Vector3)(ref normalized2))[i], ((Vector3)(ref normalized))[i]);
		}
		return ((Vector3)(ref inputVec)).normalized;
	}

	private void Update()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (Time.realtimeSinceStartup < nextTest)
		{
			return;
		}
		nextTest = Time.realtimeSinceStartup + 0f;
		Vector3 val = RandomCylinderPointAroundVector(Vector3.up, 0.5f, 0.25f, 0.5f);
		val = ((Component)this).transform.TransformPoint(val);
		((Component)testTransform).transform.position = val;
		if ((Object)(object)testTransform != (Object)null && (Object)(object)visualTest != (Object)null)
		{
			Vector3 position = ((Component)this).transform.position;
			MeshCollider obj = myMeshCollider;
			Vector3 position2 = testTransform.position;
			Vector3 val2 = ((Component)this).transform.position - testTransform.position;
			RaycastHit val3 = default(RaycastHit);
			if (((Collider)obj).Raycast(new Ray(position2, ((Vector3)(ref val2)).normalized), ref val3, 5f))
			{
				position = ((RaycastHit)(ref val3)).point;
			}
			else
			{
				Debug.LogError((object)"Missed");
			}
			((Component)visualTest).transform.position = position;
		}
	}

	public void OnDrawGizmos()
	{
	}
}
