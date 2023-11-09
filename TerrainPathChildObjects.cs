using System.Collections.Generic;
using UnityEngine;

public class TerrainPathChildObjects : MonoBehaviour
{
	public bool Spline = true;

	public float Width = 0f;

	public float Offset = 0f;

	public float Fade = 0f;

	[InspectorFlags]
	public Enum Splat = (Enum)1;

	[InspectorFlags]
	public Enum Topology = (Enum)2048;

	public InfrastructureType Type = InfrastructureType.Road;

	protected void Awake()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Expected I4, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected I4, but got Unknown
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Expected I4, but got Unknown
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Expected I4, but got Unknown
		if (!World.Cached && !World.Networked)
		{
			List<Vector3> list = new List<Vector3>();
			foreach (Transform item in ((Component)this).transform)
			{
				Transform val = item;
				list.Add(val.position);
			}
			if (list.Count >= 2)
			{
				switch (Type)
				{
				case InfrastructureType.Road:
				{
					PathList pathList2 = new PathList("Road " + TerrainMeta.Path.Roads.Count, list.ToArray());
					pathList2.Width = Width;
					pathList2.InnerFade = Fade * 0.5f;
					pathList2.OuterFade = Fade * 0.5f;
					pathList2.MeshOffset = Offset * 0.3f;
					pathList2.TerrainOffset = Offset;
					pathList2.Topology = (int)Topology;
					pathList2.Splat = (int)Splat;
					pathList2.Spline = Spline;
					pathList2.Path.RecalculateTangents();
					TerrainMeta.Path.Roads.Add(pathList2);
					break;
				}
				case InfrastructureType.Power:
				{
					PathList pathList = new PathList("Powerline " + TerrainMeta.Path.Powerlines.Count, list.ToArray());
					pathList.Width = Width;
					pathList.InnerFade = Fade * 0.5f;
					pathList.OuterFade = Fade * 0.5f;
					pathList.MeshOffset = Offset * 0.3f;
					pathList.TerrainOffset = Offset;
					pathList.Topology = (int)Topology;
					pathList.Splat = (int)Splat;
					pathList.Spline = Spline;
					pathList.Path.RecalculateTangents();
					TerrainMeta.Path.Powerlines.Add(pathList);
					break;
				}
				}
			}
		}
		GameManager.Destroy(((Component)this).gameObject);
	}

	protected void OnDrawGizmos()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Vector3 a = Vector3.zero;
		foreach (Transform item in ((Component)this).transform)
		{
			Transform val = item;
			Vector3 position = val.position;
			if (flag)
			{
				Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
				GizmosUtil.DrawWirePath(a, position, 0.5f * Width);
			}
			a = position;
			flag = true;
		}
	}
}
