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
	public int ArcRotation;

	[Tooltip("The Progress property allows the primitive to be used as a progression indicator.")]
	[Range(0f, 1f)]
	public float Progress;

	private float _progress;

	public Color ProgressColor = new Color(255f, 255f, 255f, 255f);

	public bool Fill = true;

	public float Thickness = 5f;

	public int Padding;

	private List<int> indices = new List<int>();

	private List<UIVertex> vertices = new List<UIVertex>();

	private Vector2 uvCenter = new Vector2(0.5f, 0.5f);

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
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
		simpleVert.uv0 = Vector4.op_Implicit(new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f));
		vertices.Add(simpleVert);
		Vector2 zero = default(Vector2);
		((Vector2)(ref zero))._002Ector(num5 * num10, num5 * num11);
		if (Fill)
		{
			zero = Vector2.zero;
		}
		simpleVert.position = Vector2.op_Implicit(zero);
		simpleVert.uv0 = Vector4.op_Implicit((Vector2)(Fill ? uvCenter : new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f)));
		vertices.Add(simpleVert);
		for (int i = 1; i <= ArcSteps; i++)
		{
			float num12 = (float)num * ((float)Math.PI / 180f) * ((float)i * num8 + (float)ArcRotation);
			num10 = Mathf.Cos(num12);
			num11 = Mathf.Sin(num12);
			simpleVert.color = Color32.op_Implicit(((float)i > _progress) ? ((Graphic)this).color : ProgressColor);
			simpleVert.position = Vector2.op_Implicit(new Vector2(num4 * num10, num4 * num11));
			simpleVert.uv0 = Vector4.op_Implicit(new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f));
			vertices.Add(simpleVert);
			if (!Fill)
			{
				simpleVert.position = Vector2.op_Implicit(new Vector2(num5 * num10, num5 * num11));
				simpleVert.uv0 = Vector4.op_Implicit(new Vector2(simpleVert.position.x / num3 + 0.5f, simpleVert.position.y / num3 + 0.5f));
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
			simpleVert.uv0 = Vector4.op_Implicit(uvCenter);
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((Graphic)this).color = color;
		((Graphic)this).SetVerticesDirty();
	}

	public void UpdateBaseAlpha(float value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Color color = ((Graphic)this).color;
		color.a = value;
		((Graphic)this).color = color;
		((Graphic)this).SetVerticesDirty();
	}

	public void SetProgressColor(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
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
