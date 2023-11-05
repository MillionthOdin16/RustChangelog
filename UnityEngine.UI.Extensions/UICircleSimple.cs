using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Extensions/Primitives/UI Circle Simple")]
public class UICircleSimple : UIPrimitiveBase
{
	[Tooltip("The Arc Steps property defines the number of segments that the Arc will be divided into.")]
	[Range(0f, 1000f)]
	public int ArcSteps = 100;

	public bool Fill = true;

	public float Thickness = 5f;

	public bool ThicknessIsOutside;

	private List<int> indices = new List<int>();

	private List<UIVertex> vertices = new List<UIVertex>();

	private Vector2 uvCenter = new Vector2(0.5f, 0.5f);

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = ((Graphic)this).rectTransform.rect;
		float width = ((Rect)(ref rect)).width;
		rect = ((Graphic)this).rectTransform.rect;
		float num;
		if (!(width < ((Rect)(ref rect)).height))
		{
			rect = ((Graphic)this).rectTransform.rect;
			num = ((Rect)(ref rect)).height;
		}
		else
		{
			rect = ((Graphic)this).rectTransform.rect;
			num = ((Rect)(ref rect)).width;
		}
		float num2 = num;
		float num3 = (ThicknessIsOutside ? ((0f - ((Graphic)this).rectTransform.pivot.x) * num2 - Thickness) : ((0f - ((Graphic)this).rectTransform.pivot.x) * num2));
		float num4 = (ThicknessIsOutside ? ((0f - ((Graphic)this).rectTransform.pivot.x) * num2) : ((0f - ((Graphic)this).rectTransform.pivot.x) * num2 + Thickness));
		vh.Clear();
		indices.Clear();
		vertices.Clear();
		int item = 0;
		int num5 = 1;
		int num6 = 0;
		float num7 = 360f / (float)ArcSteps;
		float num8 = Mathf.Cos(0f);
		float num9 = Mathf.Sin(0f);
		UIVertex simpleVert = UIVertex.simpleVert;
		simpleVert.color = Color32.op_Implicit(((Graphic)this).color);
		simpleVert.position = Vector2.op_Implicit(new Vector2(num3 * num8, num3 * num9));
		simpleVert.uv0 = Vector4.op_Implicit(new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f));
		vertices.Add(simpleVert);
		Vector2 zero = default(Vector2);
		((Vector2)(ref zero))._002Ector(num4 * num8, num4 * num9);
		if (Fill)
		{
			zero = Vector2.zero;
		}
		simpleVert.position = Vector2.op_Implicit(zero);
		simpleVert.uv0 = Vector4.op_Implicit((Vector2)(Fill ? uvCenter : new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f)));
		vertices.Add(simpleVert);
		for (int i = 1; i <= ArcSteps; i++)
		{
			float num10 = (float)Math.PI / 180f * ((float)i * num7);
			num8 = Mathf.Cos(num10);
			num9 = Mathf.Sin(num10);
			simpleVert.color = Color32.op_Implicit(((Graphic)this).color);
			simpleVert.position = Vector2.op_Implicit(new Vector2(num3 * num8, num3 * num9));
			simpleVert.uv0 = Vector4.op_Implicit(new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f));
			vertices.Add(simpleVert);
			if (!Fill)
			{
				simpleVert.position = Vector2.op_Implicit(new Vector2(num4 * num8, num4 * num9));
				simpleVert.uv0 = Vector4.op_Implicit(new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f));
				vertices.Add(simpleVert);
				num6 = num5;
				indices.Add(item);
				indices.Add(num5 + 1);
				indices.Add(num5);
				num5++;
				item = num5;
				num5++;
				indices.Add(item);
				indices.Add(num5);
				indices.Add(num6);
			}
			else
			{
				indices.Add(item);
				indices.Add(num5 + 1);
				indices.Add(1);
				num5++;
				item = num5;
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

	public void SetArcSteps(int steps)
	{
		ArcSteps = steps;
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

	public void SetThickness(int thickness)
	{
		Thickness = thickness;
		((Graphic)this).SetVerticesDirty();
	}
}
