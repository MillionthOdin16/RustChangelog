using UnityEngine;
using UnityEngine.Profiling;

public class PlacementTest : MonoBehaviour
{
	public MeshCollider myMeshCollider;

	public Transform testTransform;

	public Transform visualTest;

	public float hemisphere = 45f;

	public float clampTest = 45f;

	public float testDist = 2f;

	private float nextTest = 0f;

	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("RandomHemisphereDirection");
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(insideUnitCircle.x * degreesOffset, Random.Range(-1f, 1f) * degreesOffset, insideUnitCircle.y * degreesOffset);
		Profiler.EndSample();
		Vector3 val2 = input + val;
		return ((Vector3)(ref val2)).normalized;
	}

	public Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		Vector3 val = new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		return new Vector3(normalized.x * distance, Random.Range(minHeight, maxHeight), normalized.z * distance);
	}

	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("ClampToHemisphere");
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 val = hemiInput + Vector3.one * degreesOffset;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		val = hemiInput + Vector3.one * (0f - degreesOffset);
		Vector3 normalized2 = ((Vector3)(ref val)).normalized;
		for (int i = 0; i < 3; i++)
		{
			((Vector3)(ref inputVec))[i] = Mathf.Clamp(((Vector3)(ref inputVec))[i], ((Vector3)(ref normalized2))[i], ((Vector3)(ref normalized))[i]);
		}
		Profiler.EndSample();
		return ((Vector3)(ref inputVec)).normalized;
	}

	private void Update()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		if (Time.realtimeSinceStartup < nextTest)
		{
			return;
		}
		Profiler.BeginSample("PlacementTest.Update");
		nextTest = Time.realtimeSinceStartup + 0f;
		Vector3 val = RandomCylinderPointAroundVector(Vector3.up, 0.5f, 0.25f, 0.5f);
		val = ((Component)this).transform.TransformPoint(val);
		((Component)testTransform).transform.position = val;
		if ((Object)(object)testTransform != (Object)null && (Object)(object)visualTest != (Object)null)
		{
			Profiler.BeginSample("ClosestPointTest");
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
			Profiler.EndSample();
			((Component)visualTest).transform.position = position;
		}
	}

	public void OnDrawGizmos()
	{
	}
}
