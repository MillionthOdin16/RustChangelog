using UnityEngine;

public class StatusLightRenderer : MonoBehaviour, IClientComponent
{
	public Material offMaterial;

	public Material onMaterial;

	private MaterialPropertyBlock propertyBlock;

	private Renderer targetRenderer;

	private Color lightColor;

	private Light targetLight;

	private int colorID;

	private int emissionID;

	protected void Awake()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		propertyBlock = new MaterialPropertyBlock();
		targetRenderer = ((Component)this).GetComponent<Renderer>();
		targetLight = ((Component)this).GetComponent<Light>();
		colorID = Shader.PropertyToID("_Color");
		emissionID = Shader.PropertyToID("_EmissionColor");
	}

	public void SetOff()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)targetRenderer))
		{
			targetRenderer.sharedMaterial = offMaterial;
			targetRenderer.SetPropertyBlock((MaterialPropertyBlock)null);
		}
		if (Object.op_Implicit((Object)(object)targetLight))
		{
			targetLight.color = Color.clear;
		}
	}

	public void SetOn()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)targetRenderer))
		{
			targetRenderer.sharedMaterial = onMaterial;
			targetRenderer.SetPropertyBlock(propertyBlock);
		}
		if (Object.op_Implicit((Object)(object)targetLight))
		{
			targetLight.color = lightColor;
		}
	}

	public void SetRed()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		propertyBlock.Clear();
		propertyBlock.SetColor(colorID, GetColor(197, 46, 0, byte.MaxValue));
		propertyBlock.SetColor(emissionID, GetColor(191, 0, 2, byte.MaxValue, 2.916925f));
		lightColor = GetColor(byte.MaxValue, 111, 102, byte.MaxValue);
		SetOn();
	}

	public void SetGreen()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		propertyBlock.Clear();
		propertyBlock.SetColor(colorID, GetColor(19, 191, 13, byte.MaxValue));
		propertyBlock.SetColor(emissionID, GetColor(19, 191, 13, byte.MaxValue, 2.5f));
		lightColor = GetColor(156, byte.MaxValue, 102, byte.MaxValue);
		SetOn();
	}

	private Color GetColor(byte r, byte g, byte b, byte a)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return Color32.op_Implicit(new Color32(r, g, b, a));
	}

	private Color GetColor(byte r, byte g, byte b, byte a, float intensity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return Color32.op_Implicit(new Color32(r, g, b, a)) * intensity;
	}
}
