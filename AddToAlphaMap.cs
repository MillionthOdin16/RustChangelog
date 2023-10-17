using UnityEngine;

public class AddToAlphaMap : ProceduralObject
{
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	public override void Process()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(((Component)this).transform, bounds);
		Vector3 point = ((OBB)(ref val)).GetPoint(-1f, 0f, -1f);
		Vector3 point2 = ((OBB)(ref val)).GetPoint(1f, 0f, -1f);
		Vector3 point3 = ((OBB)(ref val)).GetPoint(-1f, 0f, 1f);
		Vector3 point4 = ((OBB)(ref val)).GetPoint(1f, 0f, 1f);
		TerrainMeta.AlphaMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
		{
			TerrainMeta.AlphaMap.SetAlpha(x, z, 0f);
		});
		GameManager.Destroy((Component)(object)this);
	}
}
