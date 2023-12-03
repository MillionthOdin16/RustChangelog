using UnityEngine.UI;

namespace UnityEngine;

public static class UIEx
{
	public static Vector2 Unpivot(this RectTransform rect, Vector2 localPos)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Invalid comparison between Unknown and I4
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
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
		if ((int)scrollrect.movementType > 0)
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
