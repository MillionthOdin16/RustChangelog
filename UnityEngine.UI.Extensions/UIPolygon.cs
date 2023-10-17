using System;

namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Extensions/Primitives/UI Polygon")]
public class UIPolygon : UIPrimitiveBase
{
	public bool fill = true;

	public float thickness = 5f;

	[Range(3f, 360f)]
	public int sides = 3;

	[Range(0f, 360f)]
	public float rotation = 0f;

	[Range(0f, 1f)]
	public float[] VerticesDistances = new float[3];

	private float size = 0f;

	public void DrawPolygon(int _sides)
	{
		sides = _sides;
		VerticesDistances = new float[_sides + 1];
		for (int i = 0; i < _sides; i++)
		{
			VerticesDistances[i] = 1f;
		}
		rotation = 0f;
		((Graphic)this).SetAllDirty();
	}

	public void DrawPolygon(int _sides, float[] _VerticesDistances)
	{
		sides = _sides;
		VerticesDistances = _VerticesDistances;
		rotation = 0f;
		((Graphic)this).SetAllDirty();
	}

	public void DrawPolygon(int _sides, float[] _VerticesDistances, float _rotation)
	{
		sides = _sides;
		VerticesDistances = _VerticesDistances;
		rotation = _rotation;
		((Graphic)this).SetAllDirty();
	}

	private void Update()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = ((Graphic)this).rectTransform.rect;
		size = ((Rect)(ref rect)).width;
		rect = ((Graphic)this).rectTransform.rect;
		float width = ((Rect)(ref rect)).width;
		rect = ((Graphic)this).rectTransform.rect;
		if (width > ((Rect)(ref rect)).height)
		{
			rect = ((Graphic)this).rectTransform.rect;
			size = ((Rect)(ref rect)).height;
		}
		else
		{
			rect = ((Graphic)this).rectTransform.rect;
			size = ((Rect)(ref rect)).width;
		}
		thickness = Mathf.Clamp(thickness, 0f, size / 2f);
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		vh.Clear();
		Vector2 val = Vector2.zero;
		Vector2 val2 = Vector2.zero;
		Vector2 val3 = default(Vector2);
		((Vector2)(ref val3))._002Ector(0f, 0f);
		Vector2 val4 = default(Vector2);
		((Vector2)(ref val4))._002Ector(0f, 1f);
		Vector2 val5 = default(Vector2);
		((Vector2)(ref val5))._002Ector(1f, 1f);
		Vector2 val6 = default(Vector2);
		((Vector2)(ref val6))._002Ector(1f, 0f);
		float num = 360f / (float)sides;
		int num2 = sides + 1;
		if (VerticesDistances.Length != num2)
		{
			VerticesDistances = new float[num2];
			for (int i = 0; i < num2 - 1; i++)
			{
				VerticesDistances[i] = 1f;
			}
		}
		VerticesDistances[num2 - 1] = VerticesDistances[0];
		Vector2 val8 = default(Vector2);
		Vector2 zero = default(Vector2);
		for (int j = 0; j < num2; j++)
		{
			float num3 = (0f - ((Graphic)this).rectTransform.pivot.x) * size * VerticesDistances[j];
			float num4 = (0f - ((Graphic)this).rectTransform.pivot.x) * size * VerticesDistances[j] + thickness;
			float num5 = (float)Math.PI / 180f * ((float)j * num + rotation);
			float num6 = Mathf.Cos(num5);
			float num7 = Mathf.Sin(num5);
			((Vector2)(ref val3))._002Ector(0f, 1f);
			((Vector2)(ref val4))._002Ector(1f, 1f);
			((Vector2)(ref val5))._002Ector(1f, 0f);
			((Vector2)(ref val6))._002Ector(0f, 0f);
			Vector2 val7 = val;
			((Vector2)(ref val8))._002Ector(num3 * num6, num3 * num7);
			Vector2 val9;
			if (fill)
			{
				zero = Vector2.zero;
				val9 = Vector2.zero;
			}
			else
			{
				((Vector2)(ref zero))._002Ector(num4 * num6, num4 * num7);
				val9 = val2;
			}
			val = val8;
			val2 = zero;
			vh.AddUIVertexQuad(SetVbo((Vector2[])(object)new Vector2[4] { val7, val8, zero, val9 }, (Vector2[])(object)new Vector2[4] { val3, val4, val5, val6 }));
		}
	}
}
