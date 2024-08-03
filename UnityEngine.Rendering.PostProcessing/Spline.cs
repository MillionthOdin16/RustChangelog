using System;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing;

[Serializable]
public sealed class Spline
{
	public const int k_Precision = 128;

	public const float k_Step = 1f / 128f;

	public AnimationCurve curve;

	[SerializeField]
	private bool m_Loop;

	[SerializeField]
	private float m_ZeroValue;

	[SerializeField]
	private float m_Range;

	private AnimationCurve m_InternalLoopingCurve;

	private int frameCount = -1;

	public float[] cachedData;

	public Spline(AnimationCurve curve, float zeroValue, bool loop, Vector2 bounds)
	{
		Assert.IsNotNull<AnimationCurve>(curve);
		this.curve = curve;
		m_ZeroValue = zeroValue;
		m_Loop = loop;
		m_Range = ((Vector2)(ref bounds)).magnitude;
		cachedData = new float[128];
	}

	public void Cache(int frame)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		if (frame == frameCount)
		{
			return;
		}
		int length = curve.length;
		if (m_Loop && length > 1)
		{
			if (m_InternalLoopingCurve == null)
			{
				m_InternalLoopingCurve = new AnimationCurve();
			}
			Keyframe val = curve[length - 1];
			((Keyframe)(ref val)).time = ((Keyframe)(ref val)).time - m_Range;
			Keyframe val2 = curve[0];
			((Keyframe)(ref val2)).time = ((Keyframe)(ref val2)).time + m_Range;
			m_InternalLoopingCurve.keys = curve.keys;
			m_InternalLoopingCurve.AddKey(val);
			m_InternalLoopingCurve.AddKey(val2);
		}
		for (int i = 0; i < 128; i++)
		{
			cachedData[i] = Evaluate((float)i * (1f / 128f), length);
		}
		frameCount = Time.renderedFrameCount;
	}

	public float Evaluate(float t, int length)
	{
		if (length == 0)
		{
			return m_ZeroValue;
		}
		if (!m_Loop || length == 1)
		{
			return curve.Evaluate(t);
		}
		return m_InternalLoopingCurve.Evaluate(t);
	}

	public float Evaluate(float t)
	{
		return Evaluate(t, curve.length);
	}

	public override int GetHashCode()
	{
		int num = 17;
		return num * 23 + ((object)curve).GetHashCode();
	}
}
