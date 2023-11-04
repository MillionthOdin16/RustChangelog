using UnityEngine;
using UnityEngine.UI;

public class SquareBorder : MonoBehaviour
{
	public float Size;

	public Color Color;

	public RectTransform Top;

	public RectTransform Bottom;

	public RectTransform Left;

	public RectTransform Right;

	public Image TopImage;

	public Image BottomImage;

	public Image LeftImage;

	public Image RightImage;

	private float _lastSize;

	private Color _lastColor;

	private void Update()
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		if (_lastSize != Size)
		{
			Top.offsetMin = new Vector2(0f, 0f - Size);
			Bottom.offsetMax = new Vector2(0f, Size);
			Left.offsetMin = new Vector2(0f, Size);
			Left.offsetMax = new Vector2(Size, 0f - Size);
			Right.offsetMin = new Vector2(0f - Size, Size);
			Right.offsetMax = new Vector2(0f, 0f - Size);
			_lastSize = Size;
		}
		if (_lastColor != Color)
		{
			((Graphic)TopImage).color = Color;
			((Graphic)BottomImage).color = Color;
			((Graphic)LeftImage).color = Color;
			((Graphic)RightImage).color = Color;
			_lastColor = Color;
		}
	}
}
