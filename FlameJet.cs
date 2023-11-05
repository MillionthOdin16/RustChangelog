using UnityEngine;
using UnityEngine.Profiling;

public class FlameJet : MonoBehaviour
{
	public LineRenderer line;

	public float tesselation = 0.025f;

	private float length;

	public float maxLength = 2f;

	public float drag;

	private int numSegments;

	private float spacing = 0f;

	public bool on = false;

	private Vector3[] lastWorldSegments = null;

	private Vector3[] currentSegments = (Vector3[])(object)new Vector3[0];

	public Color startColor;

	public Color endColor;

	public Color currentColor;

	private void Initialize()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		currentColor.a = Mathf.Lerp(currentColor.a, on ? 1f : 0f, Time.deltaTime * 40f);
		line.SetColors(currentColor, endColor);
		if (lastWorldSegments == null)
		{
			lastWorldSegments = (Vector3[])(object)new Vector3[numSegments];
		}
		Profiler.BeginSample("CalculatePoints");
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
		Profiler.EndSample();
		Profiler.BeginSample("SetSegments");
		line.positionCount = numSegments;
		line.SetPositions(currentSegments);
		Profiler.EndSample();
	}
}
