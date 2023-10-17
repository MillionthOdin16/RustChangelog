using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PieShape : Graphic
{
	[Range(0f, 1f)]
	public float outerSize = 1f;

	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	public float startRadius = -45f;

	public float endRadius = 45f;

	public float border = 0f;

	public bool debugDrawing = false;

	protected override void OnPopulateMesh(VertexHelper vbo)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		vbo.Clear();
		UIVertex simpleVert = UIVertex.simpleVert;
		float num = startRadius;
		float num2 = endRadius;
		if (startRadius > endRadius)
		{
			num2 = endRadius + 360f;
		}
		float num3 = Mathf.Floor((num2 - num) / 6f);
		if (num3 <= 1f)
		{
			return;
		}
		float num4 = (num2 - num) / num3;
		float num5 = num + (num2 - num) * 0.5f;
		Color val = ((Graphic)this).color;
		Rect rect = ((Graphic)this).rectTransform.rect;
		float num6 = ((Rect)(ref rect)).height * 0.5f;
		Vector2 val2 = new Vector2(Mathf.Sin(num5 * ((float)Math.PI / 180f)), Mathf.Cos(num5 * ((float)Math.PI / 180f))) * border;
		int num7 = 0;
		for (float num8 = num; num8 < num2; num8 += num4)
		{
			if (debugDrawing)
			{
				val = ((!(val == Color.red)) ? Color.red : Color.white);
			}
			simpleVert.color = Color32.op_Implicit(val);
			float num9 = Mathf.Sin(num8 * ((float)Math.PI / 180f));
			float num10 = Mathf.Cos(num8 * ((float)Math.PI / 180f));
			float num11 = num8 + num4;
			if (num11 > num2)
			{
				num11 = num2;
			}
			float num12 = Mathf.Sin(num11 * ((float)Math.PI / 180f));
			float num13 = Mathf.Cos(num11 * ((float)Math.PI / 180f));
			simpleVert.position = Vector2.op_Implicit(new Vector2(num9 * outerSize * num6, num10 * outerSize * num6) + val2);
			vbo.AddVert(simpleVert);
			simpleVert.position = Vector2.op_Implicit(new Vector2(num12 * outerSize * num6, num13 * outerSize * num6) + val2);
			vbo.AddVert(simpleVert);
			simpleVert.position = Vector2.op_Implicit(new Vector2(num12 * innerSize * num6, num13 * innerSize * num6) + val2);
			vbo.AddVert(simpleVert);
			simpleVert.position = Vector2.op_Implicit(new Vector2(num9 * innerSize * num6, num10 * innerSize * num6) + val2);
			vbo.AddVert(simpleVert);
			vbo.AddTriangle(num7, num7 + 1, num7 + 2);
			vbo.AddTriangle(num7 + 2, num7 + 3, num7);
			num7 += 4;
		}
	}
}
