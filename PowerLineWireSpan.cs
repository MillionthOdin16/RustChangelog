using System.Collections.Generic;
using UnityEngine;

public class PowerLineWireSpan : MonoBehaviour
{
	public GameObjectRef wirePrefab;

	public Transform start;

	public Transform end;

	public float WireLength;

	public List<PowerLineWireConnection> connections = new List<PowerLineWireConnection>();

	public void Init(PowerLineWire wire)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)start) && Object.op_Implicit((Object)(object)end))
		{
			WireLength = Vector3.Distance(start.position, end.position);
			for (int i = 0; i < connections.Count; i++)
			{
				Vector3 val = start.TransformPoint(connections[i].outOffset);
				Vector3 val2 = end.TransformPoint(connections[i].inOffset);
				Vector3 val3 = val - val2;
				WireLength = ((Vector3)(ref val3)).magnitude;
				GameObject val4 = wirePrefab.Instantiate(((Component)this).transform);
				((Object)val4).name = "WIRE";
				val4.transform.position = Vector3.Lerp(val, val2, 0.5f);
				val4.transform.LookAt(val2);
				val4.transform.localScale = new Vector3(1f, 1f, Vector3.Distance(val, val2));
				val4.SetActive(true);
			}
		}
	}
}
