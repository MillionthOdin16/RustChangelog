using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomImage : MonoBehaviour, IScrollHandler, IEventSystemHandler
{
	[SerializeField]
	private float _minimumScale = 0.5f;

	[SerializeField]
	private float _initialScale = 1f;

	[SerializeField]
	private float _maximumScale = 3f;

	[SerializeField]
	private float _scaleIncrement = 0.5f;

	[HideInInspector]
	private Vector3 _scale;

	private RectTransform _thisTransform;

	private void Awake()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		ref RectTransform thisTransform = ref _thisTransform;
		Transform transform = ((Component)this).transform;
		thisTransform = (RectTransform)(object)((transform is RectTransform) ? transform : null);
		((Vector3)(ref _scale)).Set(_initialScale, _initialScale, 1f);
		((Transform)_thisTransform).localScale = _scale;
	}

	public void OnScroll(PointerEventData eventData)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(_thisTransform, Vector2.op_Implicit(Input.mousePosition), (Camera)null, ref val);
		float y = eventData.scrollDelta.y;
		if (y > 0f && _scale.x < _maximumScale)
		{
			((Vector3)(ref _scale)).Set(_scale.x + _scaleIncrement, _scale.y + _scaleIncrement, 1f);
			((Transform)_thisTransform).localScale = _scale;
			RectTransform thisTransform = _thisTransform;
			thisTransform.anchoredPosition -= val * _scaleIncrement;
		}
		else if (y < 0f && _scale.x > _minimumScale)
		{
			((Vector3)(ref _scale)).Set(_scale.x - _scaleIncrement, _scale.y - _scaleIncrement, 1f);
			((Transform)_thisTransform).localScale = _scale;
			RectTransform thisTransform2 = _thisTransform;
			thisTransform2.anchoredPosition += val * _scaleIncrement;
		}
	}
}
