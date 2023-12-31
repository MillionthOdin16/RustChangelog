using UnityEngine.UI;

namespace UnityEngine;

public static class UIEx
{
	public static Vector2 Unpivot(this RectTransform rect, Vector2 localPos)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		ref float x = ref localPos.x;
		float num = x;
		float x2 = rect.pivot.x;
		Rect rect2 = rect.rect;
		x = num + x2 * ((Rect)(ref rect2)).width;
		ref float y = ref localPos.y;
		float num2 = y;
		float y2 = rect.pivot.y;
		rect2 = rect.rect;
		y = num2 + y2 * ((Rect)(ref rect2)).height;
		return localPos;
	}

	public static void CenterOnPosition(this ScrollRect scrollrect, Vector2 pos)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)scrollrect).transform;
		RectTransform val = (RectTransform)(object)((transform is RectTransform) ? transform : null);
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(((Transform)scrollrect.content).localScale.x, ((Transform)scrollrect.content).localScale.y);
		pos.x *= val2.x;
		pos.y *= val2.y;
		Rect rect = scrollrect.content.rect;
		float num = ((Rect)(ref rect)).width * val2.x;
		rect = val.rect;
		float num2 = num - ((Rect)(ref rect)).width;
		rect = scrollrect.content.rect;
		float num3 = ((Rect)(ref rect)).height * val2.y;
		rect = val.rect;
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(num2, num3 - ((Rect)(ref rect)).height);
		pos.x = pos.x / val3.x + scrollrect.content.pivot.x;
		pos.y = pos.y / val3.y + scrollrect.content.pivot.y;
		if ((int)scrollrect.movementType != 0)
		{
			pos.x = Mathf.Clamp(pos.x, 0f, 1f);
			pos.y = Mathf.Clamp(pos.y, 0f, 1f);
		}
		scrollrect.normalizedPosition = pos;
	}

	public static void RebuildHackUnity2019(this Image image)
	{
		Sprite sprite = image.sprite;
		image.sprite = null;
		image.sprite = sprite;
	}
}
