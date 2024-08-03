using UnityEngine;

public class HolosightReticlePositioning : MonoBehaviour
{
	public IronsightAimPoint aimPoint;

	public RectTransform rectTransform
	{
		get
		{
			Transform transform = ((Component)this).transform;
			return (RectTransform)(object)((transform is RectTransform) ? transform : null);
		}
	}

	private void Update()
	{
		if (MainCamera.isValid)
		{
			UpdatePosition(MainCamera.mainCamera);
		}
	}

	private void UpdatePosition(Camera cam)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)aimPoint.targetPoint).transform.position;
		Vector2 val = RectTransformUtility.WorldToScreenPoint(cam, position);
		Transform parent = ((Transform)rectTransform).parent;
		RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)(object)((parent is RectTransform) ? parent : null), val, cam, ref val);
		ref float x = ref val.x;
		float num = x;
		Transform parent2 = ((Transform)rectTransform).parent;
		Rect rect = ((RectTransform)((parent2 is RectTransform) ? parent2 : null)).rect;
		x = num / (((Rect)(ref rect)).width * 0.5f);
		ref float y = ref val.y;
		float num2 = y;
		Transform parent3 = ((Transform)rectTransform).parent;
		rect = ((RectTransform)((parent3 is RectTransform) ? parent3 : null)).rect;
		y = num2 / (((Rect)(ref rect)).height * 0.5f);
		rectTransform.anchoredPosition = val;
	}
}
