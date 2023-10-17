using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Missions/WorldPositionGenerator")]
public class WorldPositionGenerator : ScriptableObject
{
	public SpawnFilter Filter = new SpawnFilter();

	public float FilterCutoff;

	public bool aboveWater;

	public float MaxSlopeRadius;

	public float MaxSlopeDegrees = 90f;

	public float CheckSphereRadius;

	public LayerMask CheckSphereMask;

	private Vector3 _origin;

	private Vector3 _area;

	private ByteQuadtree _quadtree;

	public bool TrySample(Vector3 origin, float minDist, float maxDist, out Vector3 position, List<Vector3> blocked = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		if (_quadtree == null)
		{
			PrecalculatePositions();
		}
		Rect inclusion = new Rect(origin.x - maxDist, origin.z - maxDist, maxDist * 2f, maxDist * 2f);
		Rect exclusion = new Rect(origin.x - minDist, origin.z - minDist, minDist * 2f, minDist * 2f);
		List<Rect> blockedRects = Pool.GetList<Rect>();
		if (blocked != null)
		{
			float num = 10f;
			Rect item = default(Rect);
			foreach (Vector3 item2 in blocked)
			{
				((Rect)(ref item))._002Ector(item2.x - num, item2.z - num, num * 2f, num * 2f);
				blockedRects.Add(item);
			}
		}
		List<ByteQuadtree.Element> candidates = Pool.GetList<ByteQuadtree.Element>();
		candidates.Add(_quadtree.Root);
		for (int i = 0; i < candidates.Count; i++)
		{
			ByteQuadtree.Element element2 = candidates[i];
			if (!element2.IsLeaf)
			{
				ListEx.RemoveUnordered<ByteQuadtree.Element>(candidates, i--);
				EvaluateCandidate(element2.Child1);
				EvaluateCandidate(element2.Child2);
				EvaluateCandidate(element2.Child3);
				EvaluateCandidate(element2.Child4);
			}
		}
		if (candidates.Count == 0)
		{
			position = origin;
			Pool.FreeList<ByteQuadtree.Element>(ref candidates);
			Pool.FreeList<Rect>(ref blockedRects);
			return false;
		}
		Vector3 val2;
		if (CheckSphereRadius <= float.Epsilon)
		{
			ByteQuadtree.Element random = ListEx.GetRandom<ByteQuadtree.Element>(candidates);
			Rect val = GetElementRect(random);
			val2 = Vector3Ex.XZ3D(((Rect)(ref val)).min + ((Rect)(ref val)).size * new Vector2(Random.value, Random.value));
		}
		else
		{
			Vector3 val4;
			while (true)
			{
				if (candidates.Count == 0)
				{
					position = Vector3.zero;
					return false;
				}
				int index = Random.Range(0, candidates.Count);
				ByteQuadtree.Element element3 = candidates[index];
				Rect val3 = GetElementRect(element3);
				val4 = Vector3Ex.XZ3D(((Rect)(ref val3)).center);
				val4.y = TerrainMeta.HeightMap.GetHeight(val4);
				if (!Physics.CheckSphere(val4, CheckSphereRadius, ((LayerMask)(ref CheckSphereMask)).value))
				{
					break;
				}
				candidates.RemoveAt(index);
			}
			val2 = val4;
		}
		position = Vector3Ex.WithY(val2, aboveWater ? TerrainMeta.WaterMap.GetHeight(val2) : TerrainMeta.HeightMap.GetHeight(val2));
		Pool.FreeList<ByteQuadtree.Element>(ref candidates);
		Pool.FreeList<Rect>(ref blockedRects);
		return true;
		void EvaluateCandidate(ByteQuadtree.Element child)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			if (child.Value != 0)
			{
				Rect elementRect = GetElementRect(child);
				if (((Rect)(ref elementRect)).Overlaps(inclusion) && (!((Rect)(ref exclusion)).Contains(((Rect)(ref elementRect)).min) || !((Rect)(ref exclusion)).Contains(((Rect)(ref elementRect)).max)))
				{
					if (blockedRects.Count > 0)
					{
						foreach (Rect item3 in blockedRects)
						{
							Rect current2 = item3;
							if (((Rect)(ref current2)).Contains(((Rect)(ref elementRect)).min) && ((Rect)(ref current2)).Contains(((Rect)(ref elementRect)).max))
							{
								return;
							}
						}
					}
					candidates.Add(child);
				}
			}
		}
		Rect GetElementRect(ByteQuadtree.Element element)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			int num2 = 1 << element.Depth;
			float num3 = 1f / (float)num2;
			Vector2 val5 = element.Coords * num3;
			return new Rect(_origin.x + val5.x * _area.x, _origin.z + val5.y * _area.z, _area.x * num3, _area.z * num3);
		}
	}

	public void PrecalculatePositions()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		int res = Mathf.NextPowerOfTwo((int)((float)World.Size * 0.25f));
		byte[] map = new byte[res * res];
		Parallel.For(0, res, (Action<int>)delegate(int z)
		{
			for (int i = 0; i < res; i++)
			{
				float normX = ((float)i + 0.5f) / (float)res;
				float normZ = ((float)z + 0.5f) / (float)res;
				float factor = Filter.GetFactor(normX, normZ);
				if (factor > 0f && MaxSlopeRadius > 0f)
				{
					TerrainMeta.HeightMap.ForEach(normX, normZ, MaxSlopeRadius / (float)res, delegate(int slopeX, int slopeZ)
					{
						if (TerrainMeta.HeightMap.GetSlope(slopeX, slopeZ) > MaxSlopeDegrees)
						{
							factor = 0f;
						}
					});
				}
				map[z * res + i] = (byte)((factor >= FilterCutoff) ? (255f * factor) : 0f);
			}
		});
		_origin = TerrainMeta.Position;
		_area = TerrainMeta.Size;
		_quadtree = new ByteQuadtree();
		_quadtree.UpdateValues(map);
	}
}
