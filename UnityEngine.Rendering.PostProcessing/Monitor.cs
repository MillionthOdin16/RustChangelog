namespace UnityEngine.Rendering.PostProcessing;

public abstract class Monitor
{
	internal bool requested = false;

	public RenderTexture output { get; protected set; }

	public bool IsRequestedAndSupported(PostProcessRenderContext context)
	{
		return requested && SystemInfo.supportsComputeShaders && !RuntimeUtilities.isAndroidOpenGL && ShaderResourcesAvailable(context);
	}

	internal abstract bool ShaderResourcesAvailable(PostProcessRenderContext context);

	internal virtual bool NeedsHalfRes()
	{
		return false;
	}

	protected void CheckOutput(int width, int height)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		if ((Object)(object)output == (Object)null || !output.IsCreated() || ((Texture)output).width != width || ((Texture)output).height != height)
		{
			RuntimeUtilities.Destroy((Object)(object)output);
			output = new RenderTexture(width, height, 0, (RenderTextureFormat)0)
			{
				anisoLevel = 0,
				filterMode = (FilterMode)1,
				wrapMode = (TextureWrapMode)1,
				useMipMap = false
			};
		}
	}

	internal virtual void OnEnable()
	{
	}

	internal virtual void OnDisable()
	{
		RuntimeUtilities.Destroy((Object)(object)output);
	}

	internal abstract void Render(PostProcessRenderContext context);
}
