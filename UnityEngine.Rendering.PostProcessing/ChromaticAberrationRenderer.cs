using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing;

[Preserve]
internal sealed class ChromaticAberrationRenderer : PostProcessEffectRenderer<ChromaticAberration>
{
	private Texture2D m_InternalSpectralLut;

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Invalid comparison between Unknown and I4
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		Texture val = base.settings.spectralLut.value;
		if ((Object)(object)val == (Object)null)
		{
			if ((Object)(object)m_InternalSpectralLut == (Object)null)
			{
				m_InternalSpectralLut = new Texture2D(3, 1, (TextureFormat)3, false)
				{
					name = "Chromatic Aberration Spectrum Lookup",
					filterMode = (FilterMode)1,
					wrapMode = (TextureWrapMode)1,
					anisoLevel = 0,
					hideFlags = (HideFlags)52
				};
				m_InternalSpectralLut.SetPixels((Color[])(object)new Color[3]
				{
					new Color(1f, 0f, 0f),
					new Color(0f, 1f, 0f),
					new Color(0f, 0f, 1f)
				});
				m_InternalSpectralLut.Apply();
			}
			val = (Texture)(object)m_InternalSpectralLut;
		}
		PropertySheet uberSheet = context.uberSheet;
		bool flag = (bool)base.settings.fastMode || (int)SystemInfo.graphicsDeviceType == 8;
		uberSheet.EnableKeyword(flag ? "CHROMATIC_ABERRATION_LOW" : "CHROMATIC_ABERRATION");
		uberSheet.properties.SetFloat(ShaderIDs.ChromaticAberration_Amount, (float)base.settings.intensity * 0.05f);
		uberSheet.properties.SetTexture(ShaderIDs.ChromaticAberration_SpectralLut, val);
	}

	public override void Release()
	{
		RuntimeUtilities.Destroy((Object)(object)m_InternalSpectralLut);
		m_InternalSpectralLut = null;
	}
}
