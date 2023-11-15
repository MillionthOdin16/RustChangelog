namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Extensions/Primitives/Cut Corners")]
public class UICornerCut : UIPrimitiveBase
{
	public Vector2 cornerSize = new Vector2(16f, 16f);

	[Header("Corners to cut")]
	[SerializeField]
	private bool m_cutUL = true;

	[SerializeField]
	private bool m_cutUR;

	[SerializeField]
	private bool m_cutLL;

	[SerializeField]
	private bool m_cutLR;

	[Tooltip("Up-Down colors become Left-Right colors")]
	[SerializeField]
	private bool m_makeColumns;

	[Header("Color the cut bars differently")]
	[SerializeField]
	private bool m_useColorUp;

	[SerializeField]
	private Color32 m_colorUp;

	[SerializeField]
	private bool m_useColorDown;

	[SerializeField]
	private Color32 m_colorDown;

	public bool CutUL
	{
		get
		{
			return m_cutUL;
		}
		set
		{
			m_cutUL = value;
			((Graphic)this).SetAllDirty();
		}
	}

	public bool CutUR
	{
		get
		{
			return m_cutUR;
		}
		set
		{
			m_cutUR = value;
			((Graphic)this).SetAllDirty();
		}
	}

	public bool CutLL
	{
		get
		{
			return m_cutLL;
		}
		set
		{
			m_cutLL = value;
			((Graphic)this).SetAllDirty();
		}
	}

	public bool CutLR
	{
		get
		{
			return m_cutLR;
		}
		set
		{
			m_cutLR = value;
			((Graphic)this).SetAllDirty();
		}
	}

	public bool MakeColumns
	{
		get
		{
			return m_makeColumns;
		}
		set
		{
			m_makeColumns = value;
			((Graphic)this).SetAllDirty();
		}
	}

	public bool UseColorUp
	{
		get
		{
			return m_useColorUp;
		}
		set
		{
			m_useColorUp = value;
		}
	}

	public Color32 ColorUp
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return m_colorUp;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			m_colorUp = value;
		}
	}

	public bool UseColorDown
	{
		get
		{
			return m_useColorDown;
		}
		set
		{
			m_useColorDown = value;
		}
	}

	public Color32 ColorDown
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			return m_colorDown;
		}
		set
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			m_colorDown = value;
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = ((Graphic)this).rectTransform.rect;
		Rect val = rect;
		Color32 val2 = Color32.op_Implicit(((Graphic)this).color);
		bool flag = m_cutUL | m_cutUR;
		bool flag2 = m_cutLL | m_cutLR;
		bool flag3 = m_cutLL | m_cutUL;
		bool flag4 = m_cutLR | m_cutUR;
		if (!(flag || flag2) || !(((Vector2)(ref cornerSize)).sqrMagnitude > 0f))
		{
			return;
		}
		vh.Clear();
		if (flag3)
		{
			((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + cornerSize.x;
		}
		if (flag2)
		{
			((Rect)(ref val)).yMin = ((Rect)(ref val)).yMin + cornerSize.y;
		}
		if (flag)
		{
			((Rect)(ref val)).yMax = ((Rect)(ref val)).yMax - cornerSize.y;
		}
		if (flag4)
		{
			((Rect)(ref val)).xMax = ((Rect)(ref val)).xMax - cornerSize.x;
		}
		Vector2 val3 = default(Vector2);
		Vector2 val4 = default(Vector2);
		Vector2 val5 = default(Vector2);
		Vector2 val6 = default(Vector2);
		if (m_makeColumns)
		{
			((Vector2)(ref val3))._002Ector(((Rect)(ref rect)).xMin, m_cutUL ? ((Rect)(ref val)).yMax : ((Rect)(ref rect)).yMax);
			((Vector2)(ref val4))._002Ector(((Rect)(ref rect)).xMax, m_cutUR ? ((Rect)(ref val)).yMax : ((Rect)(ref rect)).yMax);
			((Vector2)(ref val5))._002Ector(((Rect)(ref rect)).xMin, m_cutLL ? ((Rect)(ref val)).yMin : ((Rect)(ref rect)).yMin);
			((Vector2)(ref val6))._002Ector(((Rect)(ref rect)).xMax, m_cutLR ? ((Rect)(ref val)).yMin : ((Rect)(ref rect)).yMin);
			if (flag3)
			{
				AddSquare(val5, val3, new Vector2(((Rect)(ref val)).xMin, ((Rect)(ref rect)).yMax), new Vector2(((Rect)(ref val)).xMin, ((Rect)(ref rect)).yMin), rect, m_useColorUp ? m_colorUp : val2, vh);
			}
			if (flag4)
			{
				AddSquare(val4, val6, new Vector2(((Rect)(ref val)).xMax, ((Rect)(ref rect)).yMin), new Vector2(((Rect)(ref val)).xMax, ((Rect)(ref rect)).yMax), rect, m_useColorDown ? m_colorDown : val2, vh);
			}
		}
		else
		{
			((Vector2)(ref val3))._002Ector(m_cutUL ? ((Rect)(ref val)).xMin : ((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMax);
			((Vector2)(ref val4))._002Ector(m_cutUR ? ((Rect)(ref val)).xMax : ((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMax);
			((Vector2)(ref val5))._002Ector(m_cutLL ? ((Rect)(ref val)).xMin : ((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMin);
			((Vector2)(ref val6))._002Ector(m_cutLR ? ((Rect)(ref val)).xMax : ((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMin);
			if (flag2)
			{
				AddSquare(val6, val5, new Vector2(((Rect)(ref rect)).xMin, ((Rect)(ref val)).yMin), new Vector2(((Rect)(ref rect)).xMax, ((Rect)(ref val)).yMin), rect, m_useColorDown ? m_colorDown : val2, vh);
			}
			if (flag)
			{
				AddSquare(val3, val4, new Vector2(((Rect)(ref rect)).xMax, ((Rect)(ref val)).yMax), new Vector2(((Rect)(ref rect)).xMin, ((Rect)(ref val)).yMax), rect, m_useColorUp ? m_colorUp : val2, vh);
			}
		}
		if (m_makeColumns)
		{
			AddSquare(new Rect(((Rect)(ref val)).xMin, ((Rect)(ref rect)).yMin, ((Rect)(ref val)).width, ((Rect)(ref rect)).height), rect, val2, vh);
		}
		else
		{
			AddSquare(new Rect(((Rect)(ref rect)).xMin, ((Rect)(ref val)).yMin, ((Rect)(ref rect)).width, ((Rect)(ref val)).height), rect, val2, vh);
		}
	}

	private static void AddSquare(Rect rect, Rect rectUV, Color32 color32, VertexHelper vh)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		int num = AddVert(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMin, rectUV, color32, vh);
		int num2 = AddVert(((Rect)(ref rect)).xMin, ((Rect)(ref rect)).yMax, rectUV, color32, vh);
		int num3 = AddVert(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMax, rectUV, color32, vh);
		int num4 = AddVert(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).yMin, rectUV, color32, vh);
		vh.AddTriangle(num, num2, num3);
		vh.AddTriangle(num3, num4, num);
	}

	private static void AddSquare(Vector2 a, Vector2 b, Vector2 c, Vector2 d, Rect rectUV, Color32 color32, VertexHelper vh)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		int num = AddVert(a.x, a.y, rectUV, color32, vh);
		int num2 = AddVert(b.x, b.y, rectUV, color32, vh);
		int num3 = AddVert(c.x, c.y, rectUV, color32, vh);
		int num4 = AddVert(d.x, d.y, rectUV, color32, vh);
		vh.AddTriangle(num, num2, num3);
		vh.AddTriangle(num3, num4, num);
	}

	private static int AddVert(float x, float y, Rect area, Color32 color32, VertexHelper vh)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(Mathf.InverseLerp(((Rect)(ref area)).xMin, ((Rect)(ref area)).xMax, x), Mathf.InverseLerp(((Rect)(ref area)).yMin, ((Rect)(ref area)).yMax, y));
		vh.AddVert(new Vector3(x, y), color32, val);
		return vh.currentVertCount - 1;
	}
}
