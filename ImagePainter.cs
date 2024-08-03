using System;
using Painting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ImagePainter : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IInitializePotentialDragHandler
{
	[Serializable]
	public class OnDrawingEvent : UnityEvent<Vector2, Brush>
	{
	}

	internal class PointerState
	{
		public Vector2 lastPos;

		public bool isDown;
	}

	public OnDrawingEvent onDrawing = new OnDrawingEvent();

	public MonoBehaviour redirectRightClick;

	[Tooltip("Spacing scale will depend on your texel size, tweak to what's right.")]
	public float spacingScale = 1f;

	internal Brush brush;

	internal PointerState[] pointerState = new PointerState[3]
	{
		new PointerState(),
		new PointerState(),
		new PointerState()
	};

	public RectTransform rectTransform
	{
		get
		{
			Transform transform = ((Component)this).transform;
			return (RectTransform)(object)((transform is RectTransform) ? transform : null);
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)eventData.button != 1)
		{
			Vector2 position = default(Vector2);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, ref position);
			DrawAt(position, eventData.button);
			pointerState[eventData.button].isDown = true;
		}
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		pointerState[eventData.button].isDown = false;
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if ((int)eventData.button == 1)
		{
			if (Object.op_Implicit((Object)(object)redirectRightClick))
			{
				((Component)redirectRightClick).SendMessage("OnDrag", (object)eventData);
			}
		}
		else
		{
			Vector2 position = default(Vector2);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, ref position);
			DrawAt(position, eventData.button);
		}
	}

	public virtual void OnBeginDrag(PointerEventData eventData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)eventData.button == 1 && Object.op_Implicit((Object)(object)redirectRightClick))
		{
			((Component)redirectRightClick).SendMessage("OnBeginDrag", (object)eventData);
		}
	}

	public virtual void OnEndDrag(PointerEventData eventData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)eventData.button == 1 && Object.op_Implicit((Object)(object)redirectRightClick))
		{
			((Component)redirectRightClick).SendMessage("OnEndDrag", (object)eventData);
		}
	}

	public virtual void OnInitializePotentialDrag(PointerEventData eventData)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		if ((int)eventData.button == 1 && Object.op_Implicit((Object)(object)redirectRightClick))
		{
			((Component)redirectRightClick).SendMessage("OnInitializePotentialDrag", (object)eventData);
		}
	}

	private void DrawAt(Vector2 position, InputButton button)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (brush == null)
		{
			return;
		}
		PointerState pointerState = this.pointerState[button];
		Vector2 val = rectTransform.Unpivot(position);
		if (pointerState.isDown)
		{
			Vector2 val2 = pointerState.lastPos - val;
			Vector2 normalized = ((Vector2)(ref val2)).normalized;
			for (float num = 0f; num < ((Vector2)(ref val2)).magnitude; num += Mathf.Max(brush.spacing, 1f) * Mathf.Max(spacingScale, 0.1f))
			{
				((UnityEvent<Vector2, Brush>)onDrawing).Invoke(val + num * normalized, brush);
			}
			pointerState.lastPos = val;
		}
		else
		{
			((UnityEvent<Vector2, Brush>)onDrawing).Invoke(val, brush);
			pointerState.lastPos = val;
		}
	}

	private void Start()
	{
	}

	public void UpdateBrush(Brush brush)
	{
		this.brush = brush;
	}
}
