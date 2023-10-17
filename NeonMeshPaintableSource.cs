using System;
using UnityEngine;

public class NeonMeshPaintableSource : MeshPaintableSource
{
	public NeonSign neonSign;

	public float editorEmissionScale = 2f;

	public AnimationCurve lightingCurve;

	[NonSerialized]
	public Color topLeft;

	[NonSerialized]
	public Color topRight;

	[NonSerialized]
	public Color bottomLeft;

	[NonSerialized]
	public Color bottomRight;

	public override void UpdateMaterials(MaterialPropertyBlock block, Texture2D textureOverride = null, bool forEditing = false, bool isSelected = false)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		base.UpdateMaterials(block, textureOverride, forEditing);
		if (forEditing)
		{
			block.SetFloat("_EmissionScale", editorEmissionScale);
			block.SetFloat("_Power", (float)(isSelected ? 1 : 0));
			if (!isSelected)
			{
				block.SetColor("_TubeInner", Color.clear);
				block.SetColor("_TubeOuter", Color.clear);
			}
		}
		else if ((Object)(object)neonSign != (Object)null)
		{
			block.SetFloat("_Power", (float)((isSelected && neonSign.HasFlag(BaseEntity.Flags.Reserved8)) ? 1 : 0));
		}
	}

	public override Color32[] UpdateFrom(Texture2D input)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		Init();
		Color32[] pixels = input.GetPixels32();
		texture.SetPixels32(pixels);
		texture.Apply(true, false);
		int width = ((Texture)input).width;
		int height = ((Texture)input).height;
		int num = width / 2;
		int num2 = height / 2;
		topLeft = GetColorForRegion(0, num2, num, num2);
		topRight = GetColorForRegion(num, num2, num, num2);
		bottomLeft = GetColorForRegion(0, 0, num, num2);
		bottomRight = GetColorForRegion(num, 0, num, num2);
		return pixels;
		Color GetColorForRegion(int x, int y, int regionWidth, int regionHeight)
		{
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			int num6 = y + regionHeight;
			for (int i = y; i < num6; i++)
			{
				int num7 = i * width + x;
				int num8 = num7 + regionWidth;
				for (int j = num7; j < num8; j++)
				{
					Color32 val = pixels[j];
					float num9 = (float)(int)val.a / 255f;
					num3 += (float)(int)val.r * num9;
					num4 += (float)(int)val.g * num9;
					num5 += (float)(int)val.b * num9;
				}
			}
			int num10 = regionWidth * regionHeight * 255;
			return new Color(lightingCurve.Evaluate(num3 / (float)num10), lightingCurve.Evaluate(num4 / (float)num10), lightingCurve.Evaluate(num5 / (float)num10), 1f);
		}
	}
}
