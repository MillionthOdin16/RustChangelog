using System;
using UnityEngine;

namespace VacuumBreather;

public class PidQuaternionController
{
	private readonly PidController[] _internalController;

	public float Kp
	{
		get
		{
			return _internalController[0].Kp;
		}
		set
		{
			if (value < 0f)
			{
				throw new ArgumentOutOfRangeException("value", "Kp must be a non-negative number.");
			}
			_internalController[0].Kp = value;
			_internalController[1].Kp = value;
			_internalController[2].Kp = value;
			_internalController[3].Kp = value;
		}
	}

	public float Ki
	{
		get
		{
			return _internalController[0].Ki;
		}
		set
		{
			if (value < 0f)
			{
				throw new ArgumentOutOfRangeException("value", "Ki must be a non-negative number.");
			}
			_internalController[0].Ki = value;
			_internalController[1].Ki = value;
			_internalController[2].Ki = value;
			_internalController[3].Ki = value;
		}
	}

	public float Kd
	{
		get
		{
			return _internalController[0].Kd;
		}
		set
		{
			if (value < 0f)
			{
				throw new ArgumentOutOfRangeException("value", "Kd must be a non-negative number.");
			}
			_internalController[0].Kd = value;
			_internalController[1].Kd = value;
			_internalController[2].Kd = value;
			_internalController[3].Kd = value;
		}
	}

	public PidQuaternionController(float kp, float ki, float kd)
	{
		if (kp < 0f)
		{
			throw new ArgumentOutOfRangeException("kp", "kp must be a non-negative number.");
		}
		if (ki < 0f)
		{
			throw new ArgumentOutOfRangeException("ki", "ki must be a non-negative number.");
		}
		if (kd < 0f)
		{
			throw new ArgumentOutOfRangeException("kd", "kd must be a non-negative number.");
		}
		_internalController = new PidController[4]
		{
			new PidController(kp, ki, kd),
			new PidController(kp, ki, kd),
			new PidController(kp, ki, kd),
			new PidController(kp, ki, kd)
		};
	}

	public static Quaternion MultiplyAsVector(Matrix4x4 matrix, Quaternion quaternion)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Vector4 val = default(Vector4);
		((Vector4)(ref val))._002Ector(quaternion.w, quaternion.x, quaternion.y, quaternion.z);
		Vector4 val2 = matrix * val;
		return new Quaternion(val2.y, val2.z, val2.w, val2.x);
	}

	public static Quaternion ToEulerAngleQuaternion(Vector3 eulerAngles)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		return new Quaternion(eulerAngles.x, eulerAngles.y, eulerAngles.z, 0f);
	}

	public Vector3 ComputeRequiredAngularAcceleration(Quaternion currentOrientation, Quaternion desiredOrientation, Vector3 currentAngularVelocity, float deltaTime)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = QuaternionExtensions.RequiredRotation(currentOrientation, desiredOrientation);
		Quaternion error = Quaternion.identity.Subtract(val);
		Quaternion val2 = ToEulerAngleQuaternion(currentAngularVelocity);
		Quaternion delta = val2 * val;
		Matrix4x4 val3 = default(Matrix4x4);
		val3.m00 = (0f - val.x) * (0f - val.x) + (0f - val.y) * (0f - val.y) + (0f - val.z) * (0f - val.z);
		val3.m01 = (0f - val.x) * val.w + (0f - val.y) * (0f - val.z) + (0f - val.z) * val.y;
		val3.m02 = (0f - val.x) * val.z + (0f - val.y) * val.w + (0f - val.z) * (0f - val.x);
		val3.m03 = (0f - val.x) * (0f - val.y) + (0f - val.y) * val.x + (0f - val.z) * val.w;
		val3.m10 = val.w * (0f - val.x) + (0f - val.z) * (0f - val.y) + val.y * (0f - val.z);
		val3.m11 = val.w * val.w + (0f - val.z) * (0f - val.z) + val.y * val.y;
		val3.m12 = val.w * val.z + (0f - val.z) * val.w + val.y * (0f - val.x);
		val3.m13 = val.w * (0f - val.y) + (0f - val.z) * val.x + val.y * val.w;
		val3.m20 = val.z * (0f - val.x) + val.w * (0f - val.y) + (0f - val.x) * (0f - val.z);
		val3.m21 = val.z * val.w + val.w * (0f - val.z) + (0f - val.x) * val.y;
		val3.m22 = val.z * val.z + val.w * val.w + (0f - val.x) * (0f - val.x);
		val3.m23 = val.z * (0f - val.y) + val.w * val.x + (0f - val.x) * val.w;
		val3.m30 = (0f - val.y) * (0f - val.x) + val.x * (0f - val.y) + val.w * (0f - val.z);
		val3.m31 = (0f - val.y) * val.w + val.x * (0f - val.z) + val.w * val.y;
		val3.m32 = (0f - val.y) * val.z + val.x * val.w + val.w * (0f - val.x);
		val3.m33 = (0f - val.y) * (0f - val.y) + val.x * val.x + val.w * val.w;
		Matrix4x4 matrix = val3;
		Quaternion quaternion = ComputeOutput(error, delta, deltaTime);
		quaternion = MultiplyAsVector(matrix, quaternion);
		Quaternion val4 = quaternion.Multiply(-2f);
		Quaternion val5 = val4 * Quaternion.Inverse(val);
		return new Vector3(val5.x, val5.y, val5.z);
	}

	private Quaternion ComputeOutput(Quaternion error, Quaternion delta, float deltaTime)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		Quaternion result = default(Quaternion);
		result.x = _internalController[0].ComputeOutput(error.x, delta.x, deltaTime);
		result.y = _internalController[1].ComputeOutput(error.y, delta.y, deltaTime);
		result.z = _internalController[2].ComputeOutput(error.z, delta.z, deltaTime);
		result.w = _internalController[3].ComputeOutput(error.w, delta.w, deltaTime);
		return result;
	}
}
