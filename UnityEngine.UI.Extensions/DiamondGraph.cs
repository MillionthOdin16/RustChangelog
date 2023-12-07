using System;

namespace UnityEngine.UI.Extensions;

[AddComponentMenu("UI/Extensions/Primitives/Diamond Graph")]
public class DiamondGraph : UIPrimitiveBase
{
	[SerializeField]
	private float m_a = 1f;

	[SerializeField]
	private float m_b = 1f;

	[SerializeField]
	private float m_c = 1f;

	[SerializeField]
	private float m_d = 1f;

	public float A
	{
		get
		{
			return m_a;
		}
		set
		{
			m_a = value;
		}
	}

	public float B
	{
		get
		{
			return m_b;
		}
		set
		{
			m_b = value;
		}
	}

	public float C
	{
		get
		{
			return m_c;
		}
		set
		{
			m_c = value;
		}
	}

	public float D
	{
		get
		{
			return m_d;
		}
		set
		{
			m_d = value;
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		vh.Clear();
		Rect rect = ((Graphic)this).rectTransform.rect;
		float num = ((Rect)(ref rect)).width / 2f;
		m_a = Math.Min(1f, Math.Max(0f, m_a));
		m_b = Math.Min(1f, Math.Max(0f, m_b));
		m_c = Math.Min(1f, Math.Max(0f, m_c));
		m_d = Math.Min(1f, Math.Max(0f, m_d));
		Color32 val = Color32.op_Implicit(((Graphic)this).color);
		vh.AddVert(new Vector3((0f - num) * m_a, 0f), val, new Vector2(0f, 0f));
		vh.AddVert(new Vector3(0f, num * m_b), val, new Vector2(0f, 1f));
		vh.AddVert(new Vector3(num * m_c, 0f), val, new Vector2(1f, 1f));
		vh.AddVert(new Vector3(0f, (0f - num) * m_d), val, new Vector2(1f, 0f));
		vh.AddTriangle(0, 1, 2);
		vh.AddTriangle(2, 3, 0);
	}
}
