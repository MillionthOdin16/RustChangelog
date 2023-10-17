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

	public bool ThicknessIsOutside = false;

	private List<int> indices = new List<int>();

	private List<UIVertex> vertices = new List<UIVertex>();

	private Vector2 uvCenter = new Vector2(0.5f, 0.5f);

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_040a: Unknown result type (might be due to invalid IL or missing references)
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
		simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
		vertices.Add(simpleVert);
		Vector2 zero = default(Vector2);
		((Vector2)(ref zero))._002Ector(num4 * num8, num4 * num9);
		if (Fill)
		{
			zero = Vector2.zero;
		}
		simpleVert.position = Vector2.op_Implicit(zero);
		simpleVert.uv0 = (Vector2)(Fill ? uvCenter : new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f));
		vertices.Add(simpleVert);
		for (int i = 1; i <= ArcSteps; i++)
		{
			float num10 = (float)Math.PI / 180f * ((float)i * num7);
			num8 = Mathf.Cos(num10);
			num9 = Mathf.Sin(num10);
			simpleVert.color = Color32.op_Implicit(((Graphic)this).color);
			simpleVert.position = Vector2.op_Implicit(new Vector2(num3 * num8, num3 * num9));
			simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
			vertices.Add(simpleVert);
			if (!Fill)
			{
				simpleVert.position = Vector2.op_Implicit(new Vector2(num4 * num8, num4 * num9));
				simpleVert.uv0 = new Vector2(simpleVert.position.x / num2 + 0.5f, simpleVert.position.y / num2 + 0.5f);
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
			simpleVert.uv0 = uvCenter;
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

	public void SetThickness(int thickness)
	{
		Thickness = thickness;
		((Graphic)this).SetVerticesDirty();
	}
}
