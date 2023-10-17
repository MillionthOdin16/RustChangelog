using UnityEngine;

public class ExplosionsShaderColorGradient : MonoBehaviour
{
	public string ShaderProperty = "_TintColor";

	public int MaterialID = 0;

	public Gradient Color = new Gradient();

	public float TimeMultiplier = 1f;

	private bool canUpdate;

	private Material matInstance;

	private int propertyID;

	private float startTime;

	private Color oldColor;

	private void Start()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		Material[] materials = ((Component)this).GetComponent<Renderer>().materials;
		if (MaterialID >= materials.Length)
		{
			Debug.Log((object)"ShaderColorGradient: Material ID more than shader materials count.");
		}
		matInstance = materials[MaterialID];
		if (!matInstance.HasProperty(ShaderProperty))
		{
			Debug.Log((object)("ShaderColorGradient: Shader not have \"" + ShaderProperty + "\" property"));
		}
		propertyID = Shader.PropertyToID(ShaderProperty);
		oldColor = matInstance.GetColor(propertyID);
	}

	private void OnEnable()
	{
		startTime = Time.time;
		canUpdate = true;
	}

	private void Update()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.time - startTime;
		if (canUpdate)
		{
			Color val = Color.Evaluate(num / TimeMultiplier);
			matInstance.SetColor(propertyID, val * oldColor);
		}
		if (num >= TimeMultiplier)
		{
			canUpdate = false;
		}
	}
}
