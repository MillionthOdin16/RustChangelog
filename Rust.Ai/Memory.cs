using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai;

public class Memory
{
	public struct SeenInfo
	{
		public BaseEntity Entity;

		public Vector3 Position;

		public float Timestamp;

		public float Danger;
	}

	public struct ExtendedInfo
	{
		public BaseEntity Entity;

		public Vector3 Direction;

		public float Dot;

		public float DistanceSqr;

		public byte LineOfSight;

		public float LastHurtUsTime;
	}

	public List<BaseEntity> Visible = new List<BaseEntity>();

	public List<SeenInfo> All = new List<SeenInfo>();

	public List<ExtendedInfo> AllExtended = new List<ExtendedInfo>();

	public SeenInfo Update(BaseEntity entity, float score, Vector3 direction, float dot, float distanceSqr, byte lineOfSight, bool updateLastHurtUsTime, float lastHurtUsTime, out ExtendedInfo extendedInfo)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return Update(entity, entity.ServerPosition, score, direction, dot, distanceSqr, lineOfSight, updateLastHurtUsTime, lastHurtUsTime, out extendedInfo);
	}

	public SeenInfo Update(BaseEntity entity, Vector3 position, float score, Vector3 direction, float dot, float distanceSqr, byte lineOfSight, bool updateLastHurtUsTime, float lastHurtUsTime, out ExtendedInfo extendedInfo)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		extendedInfo = default(ExtendedInfo);
		bool flag = false;
		for (int i = 0; i < AllExtended.Count; i++)
		{
			if ((Object)(object)AllExtended[i].Entity == (Object)(object)entity)
			{
				ExtendedInfo extendedInfo2 = AllExtended[i];
				extendedInfo2.Direction = direction;
				extendedInfo2.Dot = dot;
				extendedInfo2.DistanceSqr = distanceSqr;
				extendedInfo2.LineOfSight = lineOfSight;
				if (updateLastHurtUsTime)
				{
					extendedInfo2.LastHurtUsTime = lastHurtUsTime;
				}
				AllExtended[i] = extendedInfo2;
				extendedInfo = extendedInfo2;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			if (updateLastHurtUsTime)
			{
				ExtendedInfo extendedInfo3 = default(ExtendedInfo);
				extendedInfo3.Entity = entity;
				extendedInfo3.Direction = direction;
				extendedInfo3.Dot = dot;
				extendedInfo3.DistanceSqr = distanceSqr;
				extendedInfo3.LineOfSight = lineOfSight;
				extendedInfo3.LastHurtUsTime = lastHurtUsTime;
				ExtendedInfo extendedInfo4 = extendedInfo3;
				AllExtended.Add(extendedInfo4);
				extendedInfo = extendedInfo4;
			}
			else
			{
				ExtendedInfo extendedInfo3 = default(ExtendedInfo);
				extendedInfo3.Entity = entity;
				extendedInfo3.Direction = direction;
				extendedInfo3.Dot = dot;
				extendedInfo3.DistanceSqr = distanceSqr;
				extendedInfo3.LineOfSight = lineOfSight;
				ExtendedInfo extendedInfo5 = extendedInfo3;
				AllExtended.Add(extendedInfo5);
				extendedInfo = extendedInfo5;
			}
		}
		return Update(entity, position, score);
	}

	public SeenInfo Update(BaseEntity ent, float danger = 0f)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return Update(ent, ent.ServerPosition, danger);
	}

	public SeenInfo Update(BaseEntity ent, Vector3 position, float danger = 0f)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < All.Count; i++)
		{
			if ((Object)(object)All[i].Entity == (Object)(object)ent)
			{
				SeenInfo seenInfo = All[i];
				seenInfo.Position = position;
				seenInfo.Timestamp = Time.realtimeSinceStartup;
				seenInfo.Danger += danger;
				All[i] = seenInfo;
				return seenInfo;
			}
		}
		SeenInfo seenInfo2 = default(SeenInfo);
		seenInfo2.Entity = ent;
		seenInfo2.Position = position;
		seenInfo2.Timestamp = Time.realtimeSinceStartup;
		seenInfo2.Danger = danger;
		SeenInfo seenInfo3 = seenInfo2;
		All.Add(seenInfo3);
		Visible.Add(ent);
		return seenInfo3;
	}

	public void AddDanger(Vector3 position, float amount)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < All.Count; i++)
		{
			if (Mathf.Approximately(All[i].Position.x, position.x) && Mathf.Approximately(All[i].Position.y, position.y) && Mathf.Approximately(All[i].Position.z, position.z))
			{
				SeenInfo value = All[i];
				value.Danger = amount;
				All[i] = value;
				return;
			}
		}
		All.Add(new SeenInfo
		{
			Position = position,
			Timestamp = Time.realtimeSinceStartup,
			Danger = amount
		});
	}

	public SeenInfo GetInfo(BaseEntity entity)
	{
		foreach (SeenInfo item in All)
		{
			if ((Object)(object)item.Entity == (Object)(object)entity)
			{
				return item;
			}
		}
		return default(SeenInfo);
	}

	public SeenInfo GetInfo(Vector3 position)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		foreach (SeenInfo item in All)
		{
			Vector3 val = item.Position - position;
			if (((Vector3)(ref val)).sqrMagnitude < 1f)
			{
				return item;
			}
		}
		return default(SeenInfo);
	}

	public ExtendedInfo GetExtendedInfo(BaseEntity entity)
	{
		foreach (ExtendedInfo item in AllExtended)
		{
			if ((Object)(object)item.Entity == (Object)(object)entity)
			{
				return item;
			}
		}
		return default(ExtendedInfo);
	}

	internal void Forget(float maxSecondsOld)
	{
		for (int i = 0; i < All.Count; i++)
		{
			float num = Time.realtimeSinceStartup - All[i].Timestamp;
			if (num > maxSecondsOld)
			{
				if ((Object)(object)All[i].Entity != (Object)null)
				{
					Visible.Remove(All[i].Entity);
					for (int j = 0; j < AllExtended.Count; j++)
					{
						if ((Object)(object)AllExtended[j].Entity == (Object)(object)All[i].Entity)
						{
							AllExtended.RemoveAt(j);
							break;
						}
					}
				}
				All.RemoveAt(i);
				i--;
			}
			else
			{
				if (!(num > 0f))
				{
					continue;
				}
				float num2 = num / maxSecondsOld;
				if (All[i].Danger > 0f)
				{
					SeenInfo value = All[i];
					value.Danger -= num2;
					All[i] = value;
				}
				if (!(num >= 1f))
				{
					continue;
				}
				for (int k = 0; k < AllExtended.Count; k++)
				{
					if ((Object)(object)AllExtended[k].Entity == (Object)(object)All[i].Entity)
					{
						ExtendedInfo value2 = AllExtended[k];
						value2.LineOfSight = 0;
						AllExtended[k] = value2;
						break;
					}
				}
			}
		}
		for (int l = 0; l < Visible.Count; l++)
		{
			if ((Object)(object)Visible[l] == (Object)null)
			{
				Visible.RemoveAt(l);
				l--;
			}
		}
		for (int m = 0; m < AllExtended.Count; m++)
		{
			if ((Object)(object)AllExtended[m].Entity == (Object)null)
			{
				AllExtended.RemoveAt(m);
				m--;
			}
		}
	}
}
