using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TextureColorPicker : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
{
	[Serializable]
	public class onColorSelectedEvent : UnityEvent<Color>
	{
	}

	public Texture2D texture;

	public onColorSelectedEvent onColorSelected = new onColorSelectedEvent();

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		OnDrag(eventData);
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		Transform transform = ((Component)this).transform;
		RectTransform val = (RectTransform)(object)((transform is RectTransform) ? transform : null);
		Vector2 val2 = default(Vector2);
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(val, eventData.position, eventData.pressEventCamera, ref val2))
		{
			ref float x = ref val2.x;
			float num = x;
			Rect rect = val.rect;
			x = num + ((Rect)(ref rect)).width * val.pivot.x;
			ref float y = ref val2.y;
			float num2 = y;
			rect = val.rect;
			y = num2 + ((Rect)(ref rect)).height * val.pivot.y;
			ref float x2 = ref val2.x;
			float num3 = x2;
			rect = val.rect;
			x2 = num3 / ((Rect)(ref rect)).width;
			ref float y2 = ref val2.y;
			float num4 = y2;
			rect = val.rect;
			y2 = num4 / ((Rect)(ref rect)).height;
			Color pixel = texture.GetPixel((int)(val2.x * (float)((Texture)texture).width), (int)(val2.y * (float)((Texture)texture).height));
			((UnityEvent<Color>)onColorSelected).Invoke(pixel);
		}
	}
}
