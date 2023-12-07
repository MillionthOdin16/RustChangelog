using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class ZiplineArrivalPoint : BaseEntity
{
	public LineRenderer Line = null;

	private Vector3[] linePositions = null;

	public override void Save(SaveInfo info)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (info.msg.ZiplineArrival == null)
		{
			info.msg.ZiplineArrival = Pool.Get<ZiplineArrivalPoint>();
		}
		info.msg.ZiplineArrival.linePoints = Pool.GetList<VectorData>();
		Vector3[] array = linePositions;
		foreach (Vector3 val in array)
		{
			info.msg.ZiplineArrival.linePoints.Add(VectorData.op_Implicit(val));
		}
	}

	public void SetPositions(List<Vector3> points)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		linePositions = (Vector3[])(object)new Vector3[points.Count];
		for (int i = 0; i < points.Count; i++)
		{
			linePositions[i] = points[i];
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.ZiplineArrival != null && linePositions == null)
		{
			linePositions = (Vector3[])(object)new Vector3[info.msg.ZiplineArrival.linePoints.Count];
			for (int i = 0; i < info.msg.ZiplineArrival.linePoints.Count; i++)
			{
				linePositions[i] = VectorData.op_Implicit(info.msg.ZiplineArrival.linePoints[i]);
			}
		}
	}

	public override void ResetState()
	{
		base.ResetState();
		linePositions = null;
	}
}
