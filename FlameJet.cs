using UnityEngine;

public class FlameJet : MonoBehaviour
{
	public LineRenderer line;

	public float tesselation = 0.025f;

	private float length;

	public float maxLength = 2f;

	public float drag;

	private int numSegments;

	private float spacing;

	public bool on;

	private Vector3[] lastWorldSegments;

	private Vector3[] currentSegments = (Vector3[])(object)new Vector3[0];

	public Color startColor;

	public Color endColor;

	public Color currentColor;

	private void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		currentColor = startColor;
		tesselation = 0.1f;
		numSegments = Mathf.CeilToInt(maxLength / tesselation);
		spacing = maxLength / (float)numSegments;
		if (currentSegments.Length != numSegments)
		{
			currentSegments = (Vector3[])(object)new Vector3[numSegments];
		}
	}

	private void Awake()
	{
		Initialize();
	}

	public void LateUpdate()
	{
		if (on || currentColor.a > 0f)
		{
			UpdateLine();
		}
	}

	public void SetOn(bool isOn)
	{
		on = isOn;
	}

	private float curve(float x)
	{
		return x * x;
	}

	private void UpdateLine()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		currentColor.a = Mathf.Lerp(currentColor.a, on ? 1f : 0f, Time.deltaTime * 40f);
		line.SetColors(currentColor, endColor);
		if (lastWorldSegments == null)
		{
			lastWorldSegments = (Vector3[])(object)new Vector3[numSegments];
		}
		int num = currentSegments.Length;
		Vector3 val3 = default(Vector3);
		for (int i = 0; i < num; i++)
		{
			float num2 = 0f;
			float num3 = 0f;
			if (lastWorldSegments != null && lastWorldSegments[i] != Vector3.zero && i > 0)
			{
				Vector3 val = ((Component)this).transform.InverseTransformPoint(lastWorldSegments[i]);
				float num4 = (float)i / (float)currentSegments.Length;
				Vector3 val2 = Vector3.Lerp(val, Vector3.zero, Time.deltaTime * drag);
				val2 = Vector3.Lerp(Vector3.zero, val2, Mathf.Sqrt(num4));
				num2 = val2.x;
				num3 = val2.y;
			}
			if (i == 0)
			{
				num2 = (num3 = 0f);
			}
			((Vector3)(ref val3))._002Ector(num2, num3, (float)i * spacing);
			currentSegments[i] = val3;
			lastWorldSegments[i] = ((Component)this).transform.TransformPoint(val3);
		}
		line.positionCount = numSegments;
		line.SetPositions(currentSegments);
	}
}
