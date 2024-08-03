using UnityEngine;

public class AICoverPointTool : MonoBehaviour
{
	private struct TestResult
	{
		public Vector3 Position;

		public bool Valid;

		public bool Forward;

		public bool Right;

		public bool Backward;

		public bool Left;
	}

	[ContextMenu("Place Cover Points")]
	public void PlaceCoverPoints()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		foreach (Transform item in ((Component)this).transform)
		{
			Transform val = item;
			Object.DestroyImmediate((Object)(object)((Component)val).gameObject);
		}
		Vector3 pos = default(Vector3);
		((Vector3)(ref pos))._002Ector(((Component)this).transform.position.x - 50f, ((Component)this).transform.position.y, ((Component)this).transform.position.z - 50f);
		for (int i = 0; i < 50; i++)
		{
			for (int j = 0; j < 50; j++)
			{
				TestResult result = TestPoint(pos);
				if (result.Valid)
				{
					PlacePoint(result);
				}
				pos.x += 2f;
			}
			pos.x -= 100f;
			pos.z += 2f;
		}
	}

	private TestResult TestPoint(Vector3 pos)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		pos.y += 0.5f;
		TestResult result = default(TestResult);
		result.Position = pos;
		if (HitsCover(new Ray(pos, Vector3.forward), 1218519041, 1f))
		{
			result.Forward = true;
			result.Valid = true;
		}
		if (HitsCover(new Ray(pos, Vector3.right), 1218519041, 1f))
		{
			result.Right = true;
			result.Valid = true;
		}
		if (HitsCover(new Ray(pos, Vector3.back), 1218519041, 1f))
		{
			result.Backward = true;
			result.Valid = true;
		}
		if (HitsCover(new Ray(pos, Vector3.left), 1218519041, 1f))
		{
			result.Left = true;
			result.Valid = true;
		}
		return result;
	}

	private void PlacePoint(TestResult result)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (result.Forward)
		{
			PlacePoint(result.Position, Vector3.forward);
		}
		if (result.Right)
		{
			PlacePoint(result.Position, Vector3.right);
		}
		if (result.Backward)
		{
			PlacePoint(result.Position, Vector3.back);
		}
		if (result.Left)
		{
			PlacePoint(result.Position, Vector3.left);
		}
	}

	private void PlacePoint(Vector3 pos, Vector3 dir)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		AICoverPoint aICoverPoint = new GameObject("CP").AddComponent<AICoverPoint>();
		((Component)aICoverPoint).transform.position = pos;
		((Component)aICoverPoint).transform.forward = dir;
		((Component)aICoverPoint).transform.SetParent(((Component)this).transform);
	}

	public bool HitsCover(Ray ray, int layerMask, float maxDistance)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3Ex.IsNaNOrInfinity(((Ray)(ref ray)).origin))
		{
			return false;
		}
		if (Vector3Ex.IsNaNOrInfinity(((Ray)(ref ray)).direction))
		{
			return false;
		}
		if (((Ray)(ref ray)).direction == Vector3.zero)
		{
			return false;
		}
		if (GamePhysics.Trace(ray, 0f, out var _, maxDistance, layerMask, (QueryTriggerInteraction)0))
		{
			return true;
		}
		return false;
	}
}
