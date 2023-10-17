using System;

namespace UnityEngine.Rendering.PostProcessing;

[Serializable]
public sealed class TextureParameter : ParameterOverride<Texture>
{
	public TextureParameterDefault defaultState = TextureParameterDefault.Black;

	public override void Interp(Texture from, Texture to, float t)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)from == (Object)null && (Object)(object)to == (Object)null)
		{
			value = null;
			return;
		}
		if ((Object)(object)from != (Object)null && (Object)(object)to != (Object)null)
		{
			value = TextureLerper.instance.Lerp(from, to, t);
			return;
		}
		if (defaultState == TextureParameterDefault.Lut2D)
		{
			int size = (((Object)(object)from != (Object)null) ? from.height : to.height);
			Texture lutStrip = (Texture)(object)RuntimeUtilities.GetLutStrip(size);
			if ((Object)(object)from == (Object)null)
			{
				from = lutStrip;
			}
			if ((Object)(object)to == (Object)null)
			{
				to = lutStrip;
			}
		}
		Color to2;
		switch (defaultState)
		{
		case TextureParameterDefault.Black:
			to2 = Color.black;
			break;
		case TextureParameterDefault.White:
			to2 = Color.white;
			break;
		case TextureParameterDefault.Transparent:
			to2 = Color.clear;
			break;
		case TextureParameterDefault.Lut2D:
		{
			int size2 = (((Object)(object)from != (Object)null) ? from.height : to.height);
			Texture lutStrip2 = (Texture)(object)RuntimeUtilities.GetLutStrip(size2);
			if ((Object)(object)from == (Object)null)
			{
				from = lutStrip2;
			}
			if ((Object)(object)to == (Object)null)
			{
				to = lutStrip2;
			}
			if (from.width != to.width || from.height != to.height)
			{
				value = null;
			}
			else
			{
				value = TextureLerper.instance.Lerp(from, to, t);
			}
			return;
		}
		default:
			base.Interp(from, to, t);
			return;
		}
		if ((Object)(object)from == (Object)null)
		{
			value = TextureLerper.instance.Lerp(to, to2, 1f - t);
		}
		else
		{
			value = TextureLerper.instance.Lerp(from, to2, t);
		}
	}
}
