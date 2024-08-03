using UnityEngine;
using UnityEngine.Profiling;

public class AIInformationGrid : MonoBehaviour
{
	public int CellSize = 10;

	public Bounds BoundingBox;

	public AIInformationCell[] Cells;

	private Vector3 origin;

	private int xCellCount;

	private int zCellCount;

	private const int maxPointResults = 2048;

	private AIMovePoint[] movePointResults = new AIMovePoint[2048];

	private AICoverPoint[] coverPointResults = new AICoverPoint[2048];

	private const int maxCellResults = 512;

	private AIInformationCell[] resultCells = new AIInformationCell[512];

	[ContextMenu("Init")]
	public void Init()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		AIInformationZone component = ((Component)this).GetComponent<AIInformationZone>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogWarning((object)"Unable to Init AIInformationGrid, no AIInformationZone found!");
			return;
		}
		BoundingBox = component.bounds;
		((Bounds)(ref BoundingBox)).center = ((Component)this).transform.position + ((Bounds)(ref component.bounds)).center + new Vector3(0f, ((Bounds)(ref BoundingBox)).extents.y, 0f);
		float num = ((Bounds)(ref BoundingBox)).extents.x * 2f;
		float num2 = ((Bounds)(ref BoundingBox)).extents.z * 2f;
		xCellCount = (int)Mathf.Ceil(num / (float)CellSize);
		zCellCount = (int)Mathf.Ceil(num2 / (float)CellSize);
		Cells = new AIInformationCell[xCellCount * zCellCount];
		Vector3 val = (origin = ((Bounds)(ref BoundingBox)).min);
		val.x = ((Bounds)(ref BoundingBox)).min.x + (float)CellSize / 2f;
		val.z = ((Bounds)(ref BoundingBox)).min.z + (float)CellSize / 2f;
		Bounds bounds = default(Bounds);
		for (int i = 0; i < zCellCount; i++)
		{
			for (int j = 0; j < xCellCount; j++)
			{
				Vector3 val2 = val;
				((Bounds)(ref bounds))._002Ector(val2, new Vector3((float)CellSize, ((Bounds)(ref BoundingBox)).extents.y * 2f, (float)CellSize));
				Cells[GetIndex(j, i)] = new AIInformationCell(bounds, ((Component)this).gameObject, j, i);
				val.x += CellSize;
			}
			val.x = ((Bounds)(ref BoundingBox)).min.x + (float)CellSize / 2f;
			val.z += CellSize;
		}
	}

	private int GetIndex(int x, int z)
	{
		return z * xCellCount + x;
	}

	public AIInformationCell CellAt(int x, int z)
	{
		return Cells[GetIndex(x, z)];
	}

	public AIMovePoint[] GetMovePointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("AIInformationGrid.GetMovePointsInRange");
		pointCount = 0;
		int cellCount;
		AIInformationCell[] cellsInRange = GetCellsInRange(position, maxRange, out cellCount);
		if (cellCount > 0)
		{
			for (int i = 0; i < cellCount; i++)
			{
				AIInformationCell aIInformationCell = cellsInRange[i];
				if (aIInformationCell == null)
				{
					continue;
				}
				foreach (AIMovePoint item in cellsInRange[i].MovePoints.Items)
				{
					movePointResults[pointCount] = item;
					pointCount++;
				}
			}
		}
		Profiler.EndSample();
		return movePointResults;
	}

	public AICoverPoint[] GetCoverPointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("AIInformationGrid.GetCoverPointsInRange");
		pointCount = 0;
		int cellCount;
		AIInformationCell[] cellsInRange = GetCellsInRange(position, maxRange, out cellCount);
		if (cellCount > 0)
		{
			for (int i = 0; i < cellCount; i++)
			{
				AIInformationCell aIInformationCell = cellsInRange[i];
				if (aIInformationCell == null)
				{
					continue;
				}
				foreach (AICoverPoint item in cellsInRange[i].CoverPoints.Items)
				{
					coverPointResults[pointCount] = item;
					pointCount++;
				}
			}
		}
		Profiler.EndSample();
		return coverPointResults;
	}

	public AIInformationCell[] GetCellsInRange(Vector3 position, float maxRange, out int cellCount)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("AIInformationGrid.GetCellsInRange");
		cellCount = 0;
		int num = (int)(maxRange / (float)CellSize);
		AIInformationCell cell = GetCell(position);
		if (cell == null)
		{
			Profiler.EndSample();
			return resultCells;
		}
		int num2 = Mathf.Max(cell.X - num, 0);
		int num3 = Mathf.Min(cell.X + num, xCellCount - 1);
		int num4 = Mathf.Max(cell.Z - num, 0);
		int num5 = Mathf.Min(cell.Z + num, zCellCount - 1);
		for (int i = num4; i <= num5; i++)
		{
			for (int j = num2; j <= num3; j++)
			{
				resultCells[cellCount] = CellAt(j, i);
				cellCount++;
				if (cellCount >= 512)
				{
					Profiler.EndSample();
					return resultCells;
				}
			}
		}
		Profiler.EndSample();
		return resultCells;
	}

	public AIInformationCell GetCell(Vector3 position)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (Cells == null)
		{
			return null;
		}
		Vector3 val = position - origin;
		if (val.x < 0f || val.z < 0f)
		{
			return null;
		}
		int num = (int)(val.x / (float)CellSize);
		int num2 = (int)(val.z / (float)CellSize);
		if (num < 0 || num >= xCellCount)
		{
			return null;
		}
		if (num2 < 0 || num2 >= zCellCount)
		{
			return null;
		}
		return CellAt(num, num2);
	}

	public void OnDrawGizmos()
	{
		DebugDraw();
	}

	public void DebugDraw()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (Cells != null)
		{
			AIInformationCell[] cells = Cells;
			for (int i = 0; i < cells.Length; i++)
			{
				cells[i]?.DebugDraw(Color.white, points: false);
			}
		}
	}
}
