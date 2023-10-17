namespace UnityEngine.Rendering.PostProcessing;

public abstract class PostProcessEffectRenderer
{
	protected bool m_ResetHistory = true;

	public virtual void Init()
	{
	}

	public virtual DepthTextureMode GetCameraFlags()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return (DepthTextureMode)0;
	}

	public virtual void ResetHistory()
	{
		m_ResetHistory = true;
	}

	public virtual void Release()
	{
		ResetHistory();
	}

	public abstract void Render(PostProcessRenderContext context);

	internal abstract void SetSettings(PostProcessEffectSettings settings);
}
public abstract class PostProcessEffectRenderer<T> : PostProcessEffectRenderer where T : PostProcessEffectSettings
{
	public T settings { get; internal set; }

	internal override void SetSettings(PostProcessEffectSettings settings)
	{
		this.settings = (T)settings;
	}
}
