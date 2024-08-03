using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing;

public static class TextureFormatUtilities
{
	private static Dictionary<int, RenderTextureFormat> s_FormatAliasMap;

	private static Dictionary<int, bool> s_SupportedRenderTextureFormats;

	private static Dictionary<int, bool> s_SupportedTextureFormats;

	static TextureFormatUtilities()
	{
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		s_FormatAliasMap = new Dictionary<int, RenderTextureFormat>
		{
			{
				1,
				(RenderTextureFormat)0
			},
			{
				2,
				(RenderTextureFormat)5
			},
			{
				3,
				(RenderTextureFormat)0
			},
			{
				4,
				(RenderTextureFormat)0
			},
			{
				5,
				(RenderTextureFormat)0
			},
			{
				7,
				(RenderTextureFormat)4
			},
			{
				9,
				(RenderTextureFormat)15
			},
			{
				10,
				(RenderTextureFormat)0
			},
			{
				12,
				(RenderTextureFormat)0
			},
			{
				13,
				(RenderTextureFormat)5
			},
			{
				14,
				(RenderTextureFormat)0
			},
			{
				15,
				(RenderTextureFormat)15
			},
			{
				16,
				(RenderTextureFormat)13
			},
			{
				17,
				(RenderTextureFormat)2
			},
			{
				18,
				(RenderTextureFormat)14
			},
			{
				19,
				(RenderTextureFormat)12
			},
			{
				20,
				(RenderTextureFormat)11
			},
			{
				22,
				(RenderTextureFormat)2
			},
			{
				26,
				(RenderTextureFormat)16
			},
			{
				27,
				(RenderTextureFormat)13
			},
			{
				24,
				(RenderTextureFormat)2
			},
			{
				25,
				(RenderTextureFormat)0
			},
			{
				28,
				(RenderTextureFormat)0
			},
			{
				29,
				(RenderTextureFormat)0
			},
			{
				30,
				(RenderTextureFormat)0
			},
			{
				31,
				(RenderTextureFormat)0
			},
			{
				32,
				(RenderTextureFormat)0
			},
			{
				33,
				(RenderTextureFormat)0
			},
			{
				34,
				(RenderTextureFormat)0
			},
			{
				45,
				(RenderTextureFormat)0
			},
			{
				46,
				(RenderTextureFormat)0
			},
			{
				47,
				(RenderTextureFormat)0
			},
			{
				48,
				(RenderTextureFormat)0
			},
			{
				49,
				(RenderTextureFormat)0
			},
			{
				50,
				(RenderTextureFormat)0
			},
			{
				51,
				(RenderTextureFormat)0
			},
			{
				52,
				(RenderTextureFormat)0
			},
			{
				53,
				(RenderTextureFormat)0
			}
		};
		s_SupportedRenderTextureFormats = new Dictionary<int, bool>();
		Array values = Enum.GetValues(typeof(RenderTextureFormat));
		foreach (object item in values)
		{
			if ((int)item >= 0 && !IsObsolete(item))
			{
				bool value = SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)item);
				s_SupportedRenderTextureFormats[(int)item] = value;
			}
		}
		s_SupportedTextureFormats = new Dictionary<int, bool>();
		Array values2 = Enum.GetValues(typeof(TextureFormat));
		foreach (object item2 in values2)
		{
			if ((int)item2 >= 0 && !IsObsolete(item2))
			{
				bool value2 = SystemInfo.SupportsTextureFormat((TextureFormat)item2);
				s_SupportedTextureFormats[(int)item2] = value2;
			}
		}
	}

	private static bool IsObsolete(object value)
	{
		FieldInfo field = value.GetType().GetField(value.ToString());
		ObsoleteAttribute[] array = (ObsoleteAttribute[])field.GetCustomAttributes(typeof(ObsoleteAttribute), inherit: false);
		return array != null && array.Length != 0;
	}

	public static RenderTextureFormat GetUncompressedRenderTextureFormat(Texture texture)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected I4, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsNotNull<Texture>(texture);
		if (texture is RenderTexture)
		{
			return ((RenderTexture)((texture is RenderTexture) ? texture : null)).format;
		}
		if (texture is Texture2D)
		{
			TextureFormat format = ((Texture2D)texture).format;
			if (!s_FormatAliasMap.TryGetValue((int)format, out var value))
			{
				throw new NotSupportedException("Texture format not supported");
			}
			return value;
		}
		return (RenderTextureFormat)7;
	}

	internal static bool IsSupported(this RenderTextureFormat format)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected I4, but got Unknown
		s_SupportedRenderTextureFormats.TryGetValue((int)format, out var value);
		return value;
	}

	internal static bool IsSupported(this TextureFormat format)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected I4, but got Unknown
		s_SupportedTextureFormats.TryGetValue((int)format, out var value);
		return value;
	}
}
