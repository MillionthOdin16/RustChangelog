using System;
using System.Collections.Generic;
using ConVar;
using Network;
using Network.Visibility;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

public class NetworkVisibilityGrid : MonoBehaviour, Provider
{
	public const int overworldLayer = 0;

	public const int cavesLayer = 1;

	public const int tunnelsLayer = 2;

	public const int dynamicDungeonsFirstLayer = 10;

	public int startID = 1024;

	public int gridSize = 100;

	public int cellCount = 32;

	[FormerlySerializedAs("visibilityRadius")]
	public int visibilityRadiusFar = 2;

	public int visibilityRadiusNear = 1;

	public float switchTolerance = 20f;

	public float cavesThreshold = -5f;

	public float tunnelsThreshold = -50f;

	public float dynamicDungeonsThreshold = 1000f;

	public float dynamicDungeonsInterval = 100f;

	private float halfGridSize;

	private float cellSize;

	private float halfCellSize;

	private int numIDsPerLayer;

	private void Awake()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		Debug.Assert(Net.sv != null, "Network.Net.sv is NULL when creating Visibility Grid");
		Debug.Assert(Net.sv.visibility == null, "Network.Net.sv.visibility is being set multiple times");
		Net.sv.visibility = new Manager((Provider)(object)this);
	}

	private void OnEnable()
	{
		halfGridSize = (float)gridSize / 2f;
		cellSize = (float)gridSize / (float)cellCount;
		halfCellSize = cellSize / 2f;
		numIDsPerLayer = cellCount * cellCount;
	}

	private void OnDisable()
	{
		if (!Application.isQuitting && Net.sv != null && Net.sv.visibility != null)
		{
			Net.sv.visibility.Dispose();
			Net.sv.visibility = null;
		}
	}

	private void OnDrawGizmosSelected()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.blue;
		Vector3 position = ((Component)this).transform.position;
		for (int i = 0; i <= cellCount; i++)
		{
			float num = 0f - halfGridSize + (float)i * cellSize - halfCellSize;
			Gizmos.DrawLine(new Vector3(halfGridSize, position.y, num), new Vector3(0f - halfGridSize, position.y, num));
			Gizmos.DrawLine(new Vector3(num, position.y, halfGridSize), new Vector3(num, position.y, 0f - halfGridSize));
		}
	}

	private int PositionToGrid(float value)
	{
		return Mathf.RoundToInt((value + halfGridSize) / cellSize);
	}

	private float GridToPosition(int value)
	{
		return (float)value * cellSize - halfGridSize;
	}

	private int PositionToLayer(float y)
	{
		if (y < tunnelsThreshold)
		{
			return 2;
		}
		if (y < cavesThreshold)
		{
			return 1;
		}
		if (y >= dynamicDungeonsThreshold)
		{
			return 10 + Mathf.FloorToInt((y - dynamicDungeonsThreshold) / dynamicDungeonsInterval);
		}
		return 0;
	}

	private uint CoordToID(int x, int y, int layer)
	{
		return (uint)(layer * numIDsPerLayer + (x * cellCount + y) + startID);
	}

	private uint GetID(Vector3 vPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		int num = PositionToGrid(vPos.x);
		int num2 = PositionToGrid(vPos.z);
		int num3 = PositionToLayer(vPos.y);
		if (num < 0)
		{
			return 0u;
		}
		if (num >= cellCount)
		{
			return 0u;
		}
		if (num2 < 0)
		{
			return 0u;
		}
		if (num2 >= cellCount)
		{
			return 0u;
		}
		uint num4 = CoordToID(num, num2, num3);
		if (num4 < startID)
		{
			Debug.LogError((object)$"NetworkVisibilityGrid.GetID - group is below range {num} {num2} {num3} {num4} {cellCount}");
		}
		return num4;
	}

	private (int x, int y, int layer) DeconstructGroupId(int groupId)
	{
		groupId -= startID;
		int result;
		int item = Math.DivRem(groupId, numIDsPerLayer, out result);
		int result2;
		return (Math.DivRem(result, cellCount, out result2), result2, item);
	}

	private Bounds GetBounds(uint uid)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		var (value, value2, num) = DeconstructGroupId((int)uid);
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(GridToPosition(value) - halfCellSize, 0f, GridToPosition(value2) - halfCellSize);
		Vector3 max = default(Vector3);
		((Vector3)(ref max))._002Ector(val.x + cellSize, 0f, val.z + cellSize);
		if (num == 0)
		{
			val.y = cavesThreshold;
			max.y = dynamicDungeonsThreshold;
		}
		else if (num == 1)
		{
			val.y = tunnelsThreshold;
			max.y = cavesThreshold - float.Epsilon;
		}
		else if (num == 2)
		{
			val.y = -10000f;
			max.y = tunnelsThreshold - float.Epsilon;
		}
		else if (num >= 10)
		{
			int num2 = num - 10;
			val.y = dynamicDungeonsThreshold + (float)num2 * dynamicDungeonsInterval + float.Epsilon;
			max.y = val.y + dynamicDungeonsInterval;
		}
		else
		{
			Debug.LogError((object)$"Cannot get bounds for unknown layer {num}!", (Object)(object)this);
		}
		Bounds result = default(Bounds);
		((Bounds)(ref result)).min = val;
		((Bounds)(ref result)).max = max;
		return result;
	}

	public void OnGroupAdded(Group group)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		group.bounds = GetBounds(group.ID);
	}

	public bool IsInside(Group group, Vector3 vPos)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (0 == 0 && group.ID != 0 && !((Bounds)(ref group.bounds)).Contains(vPos))
		{
			return ((Bounds)(ref group.bounds)).SqrDistance(vPos) < switchTolerance;
		}
		return true;
	}

	public Group GetGroup(Vector3 vPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		uint iD = GetID(vPos);
		if (iD == 0)
		{
			return null;
		}
		Group val = Net.sv.visibility.Get(iD);
		if (!IsInside(val, vPos))
		{
			float num = ((Bounds)(ref val.bounds)).SqrDistance(vPos);
			Debug.Log((object)("Group is inside is all fucked " + iD + "/" + num + "/" + vPos));
		}
		return val;
	}

	public void GetVisibleFromFar(Group group, List<Group> groups)
	{
		int visibilityRadiusFarOverride = Net.visibilityRadiusFarOverride;
		int radius = ((visibilityRadiusFarOverride > 0) ? visibilityRadiusFarOverride : visibilityRadiusFar);
		GetVisibleFrom(group, groups, radius);
	}

	public void GetVisibleFromNear(Group group, List<Group> groups)
	{
		int visibilityRadiusNearOverride = Net.visibilityRadiusNearOverride;
		int radius = ((visibilityRadiusNearOverride > 0) ? visibilityRadiusNearOverride : visibilityRadiusNear);
		GetVisibleFrom(group, groups, radius);
	}

	private void GetVisibleFrom(Group group, List<Group> groups, int radius)
	{
		groups.Add(Net.sv.visibility.Get(0u));
		int iD = (int)group.ID;
		if (iD < startID)
		{
			return;
		}
		var (num, num2, groupLayer2) = DeconstructGroupId(iD);
		AddLayers(num, num2, groupLayer2);
		for (int i = 1; i <= radius; i++)
		{
			AddLayers(num - i, num2, groupLayer2);
			AddLayers(num + i, num2, groupLayer2);
			AddLayers(num, num2 - i, groupLayer2);
			AddLayers(num, num2 + i, groupLayer2);
			for (int j = 1; j < i; j++)
			{
				AddLayers(num - i, num2 - j, groupLayer2);
				AddLayers(num - i, num2 + j, groupLayer2);
				AddLayers(num + i, num2 - j, groupLayer2);
				AddLayers(num + i, num2 + j, groupLayer2);
				AddLayers(num - j, num2 - i, groupLayer2);
				AddLayers(num + j, num2 - i, groupLayer2);
				AddLayers(num - j, num2 + i, groupLayer2);
				AddLayers(num + j, num2 + i, groupLayer2);
			}
			AddLayers(num - i, num2 - i, groupLayer2);
			AddLayers(num - i, num2 + i, groupLayer2);
			AddLayers(num + i, num2 - i, groupLayer2);
			AddLayers(num + i, num2 + i, groupLayer2);
		}
		void Add(int groupX, int groupY, int groupLayer)
		{
			groups.Add(Net.sv.visibility.Get(CoordToID(groupX, groupY, groupLayer)));
		}
		void AddLayers(int groupX, int groupY, int groupLayer)
		{
			Add(groupX, groupY, groupLayer);
			if (groupLayer == 0)
			{
				Add(groupX, groupY, 1);
			}
			if (groupLayer == 1)
			{
				Add(groupX, groupY, 2);
				Add(groupX, groupY, 0);
			}
		}
	}
}
