using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions;

public class UIPrimitiveBase : MaskableGraphic, ILayoutElement, ICanvasRaycastFilter
{
	protected static Material s_ETC1DefaultUI = null;

	private List<Vector2> outputList = new List<Vector2>();

	[SerializeField]
	private Sprite m_Sprite;

	[NonSerialized]
	private Sprite m_OverrideSprite;

	internal float m_EventAlphaThreshold = 1f;

	[SerializeField]
	private ResolutionMode m_improveResolution;

	[SerializeField]
	protected float m_Resolution;

	[SerializeField]
	private bool m_useNativeSize;

	public Sprite sprite
	{
		get
		{
			return m_Sprite;
		}
		set
		{
			if (SetPropertyUtility.SetClass(ref m_Sprite, value))
			{
				GeneratedUVs();
			}
			((Graphic)this).SetAllDirty();
		}
	}

	public Sprite overrideSprite
	{
		get
		{
			return activeSprite;
		}
		set
		{
			if (SetPropertyUtility.SetClass(ref m_OverrideSprite, value))
			{
				GeneratedUVs();
			}
			((Graphic)this).SetAllDirty();
		}
	}

	protected Sprite activeSprite => ((Object)(object)m_OverrideSprite != (Object)null) ? m_OverrideSprite : sprite;

	public float eventAlphaThreshold
	{
		get
		{
			return m_EventAlphaThreshold;
		}
		set
		{
			m_EventAlphaThreshold = value;
		}
	}

	public ResolutionMode ImproveResolution
	{
		get
		{
			return m_improveResolution;
		}
		set
		{
			m_improveResolution = value;
			((Graphic)this).SetAllDirty();
		}
	}

	public float Resoloution
	{
		get
		{
			return m_Resolution;
		}
		set
		{
			m_Resolution = value;
			((Graphic)this).SetAllDirty();
		}
	}

	public bool UseNativeSize
	{
		get
		{
			return m_useNativeSize;
		}
		set
		{
			m_useNativeSize = value;
			((Graphic)this).SetAllDirty();
		}
	}

	public static Material defaultETC1GraphicMaterial
	{
		get
		{
			if ((Object)(object)s_ETC1DefaultUI == (Object)null)
			{
				s_ETC1DefaultUI = Canvas.GetETC1SupportedCanvasMaterial();
			}
			return s_ETC1DefaultUI;
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if ((Object)(object)activeSprite == (Object)null)
			{
				if ((Object)(object)((Graphic)this).material != (Object)null && (Object)(object)((Graphic)this).material.mainTexture != (Object)null)
				{
					return ((Graphic)this).material.mainTexture;
				}
				return (Texture)(object)Graphic.s_WhiteTexture;
			}
			return (Texture)(object)activeSprite.texture;
		}
	}

	public bool hasBorder
	{
		get
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)activeSprite != (Object)null)
			{
				Vector4 border = activeSprite.border;
				return ((Vector4)(ref border)).sqrMagnitude > 0f;
			}
			return false;
		}
	}

	public float pixelsPerUnit
	{
		get
		{
			float num = 100f;
			if (Object.op_Implicit((Object)(object)activeSprite))
			{
				num = activeSprite.pixelsPerUnit;
			}
			float num2 = 100f;
			if (Object.op_Implicit((Object)(object)((Graphic)this).canvas))
			{
				num2 = ((Graphic)this).canvas.referencePixelsPerUnit;
			}
			return num / num2;
		}
	}

	public override Material material
	{
		get
		{
			if ((Object)(object)((Graphic)this).m_Material != (Object)null)
			{
				return ((Graphic)this).m_Material;
			}
			if (Object.op_Implicit((Object)(object)activeSprite) && (Object)(object)activeSprite.associatedAlphaSplitTexture != (Object)null)
			{
				return defaultETC1GraphicMaterial;
			}
			return ((Graphic)this).defaultMaterial;
		}
		set
		{
			((Graphic)this).material = value;
		}
	}

	public virtual float minWidth => 0f;

	public virtual float preferredWidth
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)overrideSprite == (Object)null)
			{
				return 0f;
			}
			Rect rect = overrideSprite.rect;
			return ((Rect)(ref rect)).size.x / pixelsPerUnit;
		}
	}

	public virtual float flexibleWidth => -1f;

	public virtual float minHeight => 0f;

	public virtual float preferredHeight
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)overrideSprite == (Object)null)
			{
				return 0f;
			}
			Rect rect = overrideSprite.rect;
			return ((Rect)(ref rect)).size.y / pixelsPerUnit;
		}
	}

	public virtual float flexibleHeight => -1f;

	public virtual int layoutPriority => 0;

	protected UIPrimitiveBase()
	{
		((Graphic)this).useLegacyMeshGeneration = false;
	}

	protected UIVertex[] SetVbo(Vector2[] vertices, Vector2[] uvs)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		UIVertex[] array = (UIVertex[])(object)new UIVertex[4];
		for (int i = 0; i < vertices.Length; i++)
		{
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.color = Color32.op_Implicit(((Graphic)this).color);
			simpleVert.position = Vector2.op_Implicit(vertices[i]);
			simpleVert.uv0 = uvs[i];
			array[i] = simpleVert;
		}
		return array;
	}

	protected Vector2[] IncreaseResolution(Vector2[] input)
	{
		return IncreaseResolution(new List<Vector2>(input)).ToArray();
	}

	protected List<Vector2> IncreaseResolution(List<Vector2> input)
	{
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		outputList.Clear();
		switch (ImproveResolution)
		{
		case ResolutionMode.PerLine:
		{
			float num3 = 0f;
			float num = 0f;
			for (int j = 0; j < input.Count - 1; j++)
			{
				num3 += Vector2.Distance(input[j], input[j + 1]);
			}
			ResolutionToNativeSize(num3);
			num = num3 / m_Resolution;
			int num4 = 0;
			for (int k = 0; k < input.Count - 1; k++)
			{
				Vector2 val3 = input[k];
				outputList.Add(val3);
				Vector2 val4 = input[k + 1];
				float num5 = Vector2.Distance(val3, val4) / num;
				float num6 = 1f / num5;
				for (int l = 0; (float)l < num5; l++)
				{
					outputList.Add(Vector2.Lerp(val3, val4, (float)l * num6));
					num4++;
				}
				outputList.Add(val4);
			}
			break;
		}
		case ResolutionMode.PerSegment:
		{
			for (int i = 0; i < input.Count - 1; i++)
			{
				Vector2 val = input[i];
				outputList.Add(val);
				Vector2 val2 = input[i + 1];
				ResolutionToNativeSize(Vector2.Distance(val, val2));
				float num = 1f / m_Resolution;
				for (float num2 = 1f; num2 < m_Resolution; num2 += 1f)
				{
					outputList.Add(Vector2.Lerp(val, val2, num * num2));
				}
				outputList.Add(val2);
			}
			break;
		}
		}
		return outputList;
	}

	protected virtual void GeneratedUVs()
	{
	}

	protected virtual void ResolutionToNativeSize(float distance)
	{
	}

	public virtual void CalculateLayoutInputHorizontal()
	{
	}

	public virtual void CalculateLayoutInputVertical()
	{
	}

	public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
	{
		//IL_0143: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		if (m_EventAlphaThreshold >= 1f)
		{
			return true;
		}
		Sprite val = overrideSprite;
		if ((Object)(object)val == (Object)null)
		{
			return true;
		}
		Vector2 local = default(Vector2);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(((Graphic)this).rectTransform, screenPoint, eventCamera, ref local);
		Rect pixelAdjustedRect = ((Graphic)this).GetPixelAdjustedRect();
		local.x += ((Graphic)this).rectTransform.pivot.x * ((Rect)(ref pixelAdjustedRect)).width;
		local.y += ((Graphic)this).rectTransform.pivot.y * ((Rect)(ref pixelAdjustedRect)).height;
		local = MapCoordinate(local, pixelAdjustedRect);
		Rect textureRect = val.textureRect;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(local.x / ((Rect)(ref textureRect)).width, local.y / ((Rect)(ref textureRect)).height);
		float num = Mathf.Lerp(((Rect)(ref textureRect)).x, ((Rect)(ref textureRect)).xMax, val2.x) / (float)((Texture)val.texture).width;
		float num2 = Mathf.Lerp(((Rect)(ref textureRect)).y, ((Rect)(ref textureRect)).yMax, val2.y) / (float)((Texture)val.texture).height;
		try
		{
			return val.texture.GetPixelBilinear(num, num2).a >= m_EventAlphaThreshold;
		}
		catch (UnityException val3)
		{
			UnityException val4 = val3;
			Debug.LogError((object)("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + ((Exception)(object)val4).Message + " Also make sure to disable sprite packing for this sprite."), (Object)(object)this);
			return true;
		}
	}

	private Vector2 MapCoordinate(Vector2 local, Rect rect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Rect rect2 = sprite.rect;
		return new Vector2(local.x * ((Rect)(ref rect)).width, local.y * ((Rect)(ref rect)).height);
	}

	private Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i <= 1; i++)
		{
			float num = ((Vector4)(ref border))[i] + ((Vector4)(ref border))[i + 2];
			Vector2 size = ((Rect)(ref rect)).size;
			if (((Vector2)(ref size))[i] < num && num != 0f)
			{
				size = ((Rect)(ref rect)).size;
				float num2 = ((Vector2)(ref size))[i] / num;
				ref Vector4 reference = ref border;
				int num3 = i;
				((Vector4)(ref reference))[num3] = ((Vector4)(ref reference))[num3] * num2;
				reference = ref border;
				num3 = i + 2;
				((Vector4)(ref reference))[num3] = ((Vector4)(ref reference))[num3] * num2;
			}
		}
		return border;
	}

	protected override void OnEnable()
	{
		((MaskableGraphic)this).OnEnable();
		((Graphic)this).SetAllDirty();
	}
}
