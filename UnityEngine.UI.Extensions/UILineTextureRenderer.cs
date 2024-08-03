using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Extensions/Primitives/UILineTextureRenderer")]
public class UILineTextureRenderer : UIPrimitiveBase
{
	[SerializeField]
	private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

	[SerializeField]
	private Vector2[] m_points;

	public float LineThickness = 2f;

	public bool UseMargins;

	public Vector2 Margin;

	public bool relativeSize;

	public Rect uvRect
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return m_UVRect;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			if (!(m_UVRect == value))
			{
				m_UVRect = value;
				((Graphic)this).SetVerticesDirty();
			}
		}
	}

	public Vector2[] Points
	{
		get
		{
			return m_points;
		}
		set
		{
			if (m_points != value)
			{
				m_points = value;
				((Graphic)this).SetAllDirty();
			}
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0523: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_0586: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0561: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Unknown result type (might be due to invalid IL or missing references)
		if (m_points == null || m_points.Length < 2)
		{
			m_points = (Vector2[])(object)new Vector2[2]
			{
				new Vector2(0f, 0f),
				new Vector2(1f, 1f)
			};
		}
		int num = 24;
		Rect rect = ((Graphic)this).rectTransform.rect;
		float num2 = ((Rect)(ref rect)).width;
		rect = ((Graphic)this).rectTransform.rect;
		float num3 = ((Rect)(ref rect)).height;
		float num4 = 0f - ((Graphic)this).rectTransform.pivot.x;
		rect = ((Graphic)this).rectTransform.rect;
		float num5 = num4 * ((Rect)(ref rect)).width;
		float num6 = 0f - ((Graphic)this).rectTransform.pivot.y;
		rect = ((Graphic)this).rectTransform.rect;
		float num7 = num6 * ((Rect)(ref rect)).height;
		if (!relativeSize)
		{
			num2 = 1f;
			num3 = 1f;
		}
		List<Vector2> list = new List<Vector2>();
		list.Add(m_points[0]);
		Vector2 val = m_points[0];
		Vector2 val2 = m_points[1] - m_points[0];
		Vector2 item = val + ((Vector2)(ref val2)).normalized * (float)num;
		list.Add(item);
		for (int i = 1; i < m_points.Length - 1; i++)
		{
			list.Add(m_points[i]);
		}
		Vector2 val3 = m_points[m_points.Length - 1];
		val2 = m_points[m_points.Length - 1] - m_points[m_points.Length - 2];
		item = val3 - ((Vector2)(ref val2)).normalized * (float)num;
		list.Add(item);
		list.Add(m_points[m_points.Length - 1]);
		Vector2[] array = list.ToArray();
		if (UseMargins)
		{
			num2 -= Margin.x;
			num3 -= Margin.y;
			num5 += Margin.x / 2f;
			num7 += Margin.y / 2f;
		}
		vh.Clear();
		Vector2 val4 = Vector2.zero;
		Vector2 val5 = Vector2.zero;
		Vector2 val12 = default(Vector2);
		Vector2 val13 = default(Vector2);
		Vector2 val14 = default(Vector2);
		Vector2 val15 = default(Vector2);
		Vector2 val16 = default(Vector2);
		for (int j = 1; j < array.Length; j++)
		{
			Vector2 val6 = array[j - 1];
			Vector2 val7 = array[j];
			((Vector2)(ref val6))._002Ector(val6.x * num2 + num5, val6.y * num3 + num7);
			((Vector2)(ref val7))._002Ector(val7.x * num2 + num5, val7.y * num3 + num7);
			float num8 = Mathf.Atan2(val7.y - val6.y, val7.x - val6.x) * 180f / (float)Math.PI;
			Vector2 val8 = val6 + new Vector2(0f, (0f - LineThickness) / 2f);
			Vector2 val9 = val6 + new Vector2(0f, LineThickness / 2f);
			Vector2 val10 = val7 + new Vector2(0f, LineThickness / 2f);
			Vector2 val11 = val7 + new Vector2(0f, (0f - LineThickness) / 2f);
			val8 = Vector2.op_Implicit(RotatePointAroundPivot(Vector2.op_Implicit(val8), Vector2.op_Implicit(val6), new Vector3(0f, 0f, num8)));
			val9 = Vector2.op_Implicit(RotatePointAroundPivot(Vector2.op_Implicit(val9), Vector2.op_Implicit(val6), new Vector3(0f, 0f, num8)));
			val10 = Vector2.op_Implicit(RotatePointAroundPivot(Vector2.op_Implicit(val10), Vector2.op_Implicit(val7), new Vector3(0f, 0f, num8)));
			val11 = Vector2.op_Implicit(RotatePointAroundPivot(Vector2.op_Implicit(val11), Vector2.op_Implicit(val7), new Vector3(0f, 0f, num8)));
			Vector2 zero = Vector2.zero;
			((Vector2)(ref val12))._002Ector(0f, 1f);
			((Vector2)(ref val13))._002Ector(0.5f, 0f);
			((Vector2)(ref val14))._002Ector(0.5f, 1f);
			((Vector2)(ref val15))._002Ector(1f, 0f);
			((Vector2)(ref val16))._002Ector(1f, 1f);
			Vector2[] uvs = (Vector2[])(object)new Vector2[4] { val13, val14, val14, val13 };
			if (j > 1)
			{
				vh.AddUIVertexQuad(SetVbo((Vector2[])(object)new Vector2[4] { val4, val5, val8, val9 }, uvs));
			}
			if (j == 1)
			{
				uvs = (Vector2[])(object)new Vector2[4] { zero, val12, val14, val13 };
			}
			else if (j == array.Length - 1)
			{
				uvs = (Vector2[])(object)new Vector2[4] { val13, val14, val16, val15 };
			}
			vh.AddUIVertexQuad(SetVbo((Vector2[])(object)new Vector2[4] { val8, val9, val10, val11 }, uvs));
			val4 = val10;
			val5 = val11;
		}
	}

	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = point - pivot;
		val = Quaternion.Euler(angles) * val;
		point = val + pivot;
		return point;
	}
}
