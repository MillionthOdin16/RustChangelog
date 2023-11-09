using System;
using Network;
using ProtoBuf;
using UnityEngine;

public class MapMarkerGenericRadius : MapMarker
{
	public float radius;

	public Color color1;

	public Color color2;

	public float alpha;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MapMarkerGenericRadius.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public void SendUpdate(bool fullUpdate = true)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		float a = color1.a;
		Vector3 arg = default(Vector3);
		((Vector3)(ref arg))._002Ector(color1.r, color1.g, color1.b);
		Vector3 arg2 = default(Vector3);
		((Vector3)(ref arg2))._002Ector(color2.r, color2.g, color2.b);
		ClientRPC<Vector3, float, Vector3, float, float>(null, "MarkerUpdate", arg, a, arg2, alpha, radius);
	}

	public override AppMarker GetAppMarkerData()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.radius = radius;
		appMarkerData.color1 = Color.op_Implicit(color1);
		appMarkerData.color2 = Color.op_Implicit(color2);
		appMarkerData.alpha = alpha;
		return appMarkerData;
	}
}
