using UnityEngine;

public class ConstructionSocket : Socket_Base
{
	public enum Type
	{
		None,
		Foundation,
		Floor,
		Misc,
		Doorway,
		Wall,
		Block,
		Ramp,
		StairsTriangle,
		Stairs,
		FloorFrameTriangle,
		Window,
		Shutters,
		WallFrame,
		FloorFrame,
		WindowDressing,
		DoorDressing,
		Elevator,
		DoubleDoorDressing
	}

	public Type socketType = Type.None;

	public int rotationDegrees = 0;

	public int rotationOffset = 0;

	public bool restrictPlacementRotation = false;

	public bool restrictPlacementAngle = false;

	public float faceAngle = 0f;

	public float angleAllowed = 150f;

	[Range(0f, 1f)]
	public float support = 1f;

	private void OnDrawGizmos()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.6f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(Vector3.zero, Vector3.right * 0.1f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(Vector3.zero, Vector3.up * 0.1f);
		Gizmos.DrawIcon(((Component)this).transform.position, "light_circle_green.png", false);
	}

	private void OnDrawGizmosSelected()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (female)
		{
			Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
			Gizmos.DrawWireCube(selectCenter, selectSize);
		}
	}

	public override bool TestTarget(Construction.Target target)
	{
		if (!base.TestTarget(target))
		{
			return false;
		}
		return IsCompatible(target.socket);
	}

	public override bool IsCompatible(Socket_Base socket)
	{
		if (!base.IsCompatible(socket))
		{
			return false;
		}
		ConstructionSocket constructionSocket = socket as ConstructionSocket;
		if (constructionSocket == null)
		{
			return false;
		}
		if (constructionSocket.socketType == Type.None || socketType == Type.None)
		{
			return false;
		}
		if (constructionSocket.socketType != socketType)
		{
			return false;
		}
		return true;
	}

	public override bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
		{
			return false;
		}
		Matrix4x4 val = Matrix4x4.TRS(position, rotation, Vector3.one);
		Matrix4x4 val2 = Matrix4x4.TRS(socketPosition, socketRotation, Vector3.one);
		Vector3 val3 = ((Matrix4x4)(ref val)).MultiplyPoint3x4(worldPosition);
		Vector3 val4 = ((Matrix4x4)(ref val2)).MultiplyPoint3x4(socket.worldPosition);
		float num = Vector3.Distance(val3, val4);
		if (num > 0.01f)
		{
			return false;
		}
		Vector3 val5 = ((Matrix4x4)(ref val)).MultiplyVector(worldRotation * Vector3.forward);
		Vector3 val6 = ((Matrix4x4)(ref val2)).MultiplyVector(socket.worldRotation * Vector3.forward);
		float num2 = Vector3.Angle(val5, val6);
		if (male && female)
		{
			num2 = Mathf.Min(num2, Vector3.Angle(-val5, val6));
		}
		if (socket.male && socket.female)
		{
			num2 = Mathf.Min(num2, Vector3.Angle(val5, -val6));
		}
		if (num2 > 1f)
		{
			return false;
		}
		return true;
	}

	public bool TestRestrictedAngles(Vector3 suggestedPos, Quaternion suggestedAng, Construction.Target target)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (restrictPlacementAngle)
		{
			Quaternion val = Quaternion.Euler(0f, faceAngle, 0f);
			Quaternion val2 = val * suggestedAng;
			Vector3 val3 = Vector3Ex.XZ3D(((Ray)(ref target.ray)).direction);
			float num = Vector3Ex.DotDegrees(val3, val2 * Vector3.forward);
			if (num > angleAllowed * 0.5f)
			{
				return false;
			}
			if (num < angleAllowed * -0.5f)
			{
				return false;
			}
		}
		return true;
	}

	public override Construction.Placement DoPlacement(Construction.Target target)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)target.entity) || !Object.op_Implicit((Object)(object)((Component)target.entity).transform))
		{
			return null;
		}
		if (!CanConnectToEntity(target))
		{
			return null;
		}
		ConstructionSocket constructionSocket = target.socket as ConstructionSocket;
		Vector3 val = target.GetWorldPosition();
		Quaternion val2 = target.GetWorldRotation(female: true);
		if (constructionSocket != null && !IsCompatible(constructionSocket))
		{
			return null;
		}
		if (rotationDegrees > 0 && (constructionSocket == null || !constructionSocket.restrictPlacementRotation))
		{
			Construction.Placement placement = new Construction.Placement();
			float num = float.MaxValue;
			float num2 = 0f;
			for (int i = 0; i < 360; i += rotationDegrees)
			{
				Quaternion val3 = Quaternion.Euler(0f, (float)(rotationOffset + i), 0f);
				Vector3 direction = ((Ray)(ref target.ray)).direction;
				Vector3 val4 = val3 * val2 * Vector3.up;
				float num3 = Vector3.Angle(direction, val4);
				if (num3 < num)
				{
					num = num3;
					num2 = i;
				}
			}
			for (int j = 0; j < 360; j += rotationDegrees)
			{
				Quaternion val5 = val2 * Quaternion.Inverse(rotation);
				Quaternion val6 = Quaternion.Euler(target.rotation);
				Quaternion val7 = Quaternion.Euler(0f, (float)(rotationOffset + j) + num2, 0f);
				Quaternion val8 = val6 * val7 * val5;
				Vector3 val9 = val8 * position;
				placement.position = val - val9;
				placement.rotation = val8;
				if (CheckSocketMods(placement))
				{
					return placement;
				}
			}
		}
		Construction.Placement placement2 = new Construction.Placement();
		Quaternion val10 = val2 * Quaternion.Inverse(rotation);
		Vector3 val11 = val10 * position;
		placement2.position = val - val11;
		placement2.rotation = val10;
		if (!TestRestrictedAngles(val, val2, target))
		{
			return null;
		}
		return placement2;
	}

	protected virtual bool CanConnectToEntity(Construction.Target target)
	{
		return true;
	}
}
