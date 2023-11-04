using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Extensions/Primitives/UI Circle")]
public class UICircle : UIPrimitiveBase
{
	[Tooltip("The Arc Invert property will invert the construction of the Arc.")]
	public bool ArcInvert = true;

	[Tooltip("The Arc property is a percentage of the entire circumference of the circle.")]
	[Range(0f, 1f)]
	public float Arc = 1f;

	[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
	[Range(0f, 1000f)]
	public int ArcSteps = 100;

	[Tooltip("The Arc Rotation property permits adjusting the geometry orientation around the Z axis.")]
	[Range(0f, 360f)]
	public int ArcRotation = 0;

	[Tooltip("The Progress property allows the primitive to be used as a progression indicator.")]
	[Range(0f, 1f)]
	public float Progress = 0f;

	private float _progress = 0f;

	public Color ProgressColor = new Color(255f, 255f, 255f, 255f);

	public bool Fill = true;

	public float Thickness = 5f;

	public int Padding = 0;

	private List<int> indices = new List<int>();

	private List<UIVertex> vertices = new List<UIVertex>();

	private Vector2 uvCenter = new Vector2(0.5f, 0.5f);

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		int num = ((!ArcInvert) ? 1 : (-1));
		Rect rect = ((Graphic)this).rectTransform.rect;
		float width = ((Rect)(ref rect)).width;
		rect = ((Graphic)this).rectTransform.rect;
		float num2;
		if (!(width < ((Rect)(ref rect)).height))
		{
			rect = ((Graphic)this).rectTransform.rect;
			num2 = ((Rect)(ref rect)).height;
		}
		else
		{
			rect = ((Graphic)this).rectTransform.rect;
			num2 = ((Rect)(ref rect)).width;
		}
		float num3 = num2 - (float)Padding;
		float num4 = (0f - ((Graphic)this).rectTransform.pivot.x) * num3;
		float num5 = (0f - ((Graphic)this).rectTransform.pivot.x) * num3 + Thickness;
		vh.Clear();
		indices.Clear();
		vertices.Clear();
		int item = 0;
		int num6 = 1;
		int num7 = 0;
		float num8 = Arc * 360f / (float)ArcSteps;
		_progress = (float)ArcSteps * Progress;
		float num9 = (float)num * ((float)Math.PI / 180f) * (float)ArcRotation;
		float num10 = Mathf.Cos(num9);
		float num11 = Mathf.Sin(num9);
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = Color32.op_Implicit((_progress > 0f) ? ProgressColor : ((Graphic)this).color);
		simpleVert.position = Vector2.op_Implicit(new Vector2(num4 * num10, num4 * num11));
		simpleVert.uv0 = new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f);
		vertices.Add(simpleVert);
		Vector2 zero = default(Vector2);
		((Vector2)(ref zero))._002Ector(num5 * num10, num5 * num11);
		if (Fill)
		{
			zero = Vector2.zero;
		}
		simpleVert.position = Vector2.op_Implicit(zero);
		simpleVert.uv0 = (Vector2)(Fill ? uvCenter : new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f));
		vertices.Add(simpleVert);
		for (int i = 1; i <= ArcSteps; i++)
		{
			num9 = (float)num * ((float)Math.PI / 180f) * ((float)i * num8 + (float)ArcRotation);
			num10 = Mathf.Cos(num9);
			num11 = Mathf.Sin(num9);
			simpleVert.color = Color32.op_Implicit(((float)i > _progress) ? ((Graphic)this).color : ProgressColor);
			simpleVert.position = Vector2.op_Implicit(new Vector2(num4 * num10, num4 * num11));
			simpleVert.uv0 = new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f);
			vertices.Add(simpleVert);
			if (!Fill)
			{
				simpleVert.position = Vector2.op_Implicit(new Vector2(num5 * num10, num5 * num11));
				simpleVert.uv0 = new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f);
				vertices.Add(simpleVert);
				num7 = num6;
				indices.Add(item);
				indices.Add(num6 + 1);
				indices.Add(num6);
				num6++;
				item = num6;
				num6++;
				indices.Add(item);
				indices.Add(num6);
				indices.Add(num7);
			}
			else
			{
				indices.Add(item);
				indices.Add(num6 + 1);
				if ((float)i > _progress)
				{
					indices.Add(ArcSteps + 2);
				}
				else
				{
					indices.Add(1);
				}
				num6++;
				item = num6;
			}
		}
		if (Fill)
		{
			simpleVert.position = Vector2.op_Implicit(zero);
			simpleVert.color = Color32.op_Implicit(((Graphic)this).color);
			simpleVert.uv0 = uvCenter;
			vertices.Add(simpleVert);
		}
		vh.AddUIVertexStream(vertices, indices);
	}

	public void SetProgress(float progress)
	{
		Progress = progress;
		((Graphic)this).SetVerticesDirty();
	}

	public void SetArcSteps(int steps)
	{
		ArcSteps = steps;
		((Graphic)this).SetVerticesDirty();
	}

	public void SetInvertArc(bool invert)
	{
		ArcInvert = invert;
		((Graphic)this).SetVerticesDirty();
	}

	public void SetArcRotation(int rotation)
	{
		ArcRotation = rotation;
		((Graphic)this).SetVerticesDirty();
	}

	public void SetFill(bool fill)
	{
		Fill = fill;
		((Graphic)this).SetVerticesDirty();
	}

	public void SetBaseColor(Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)this).color = color;
		((Graphic)this).SetVerticesDirty();
	}

	public void UpdateBaseAlpha(float value)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Color color = ((Graphic)this).color;
		color.a = value;
		((Graphic)this).color = color;
		((Graphic)this).SetVerticesDirty();
	}

	public void SetProgressColor(Color color)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		ProgressColor = color;
		((Graphic)this).SetVerticesDirty();
	}

	public void UpdateProgressAlpha(float value)
	{
		ProgressColor.a = value;
		((Graphic)this).SetVerticesDirty();
	}

	public void SetPadding(int padding)
	{
		Padding = padding;
		((Graphic)this).SetVerticesDirty();
	}

	public void SetThickness(int thickness)
	{
		Thickness = thickness;
		((Graphic)this).SetVerticesDirty();
	}
}
