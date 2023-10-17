using ProtoBuf;
using UnityEngine;

public class ThreatDetectedAIEvent : BaseAIEvent
{
	public float Range { get; set; }

	public ThreatDetectedAIEvent()
		: base(AIEventType.ThreatDetected)
	{
		base.Rate = ExecuteRate.Slow;
	}

	public override void Init(AIEventData data, BaseEntity owner)
	{
		base.Init(data, owner);
		ThreatDetectedAIEventData threatDetectedData = data.threatDetectedData;
		Range = threatDetectedData.range;
	}

	public override AIEventData ToProto()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		AIEventData val = base.ToProto();
		val.threatDetectedData = new ThreatDetectedAIEventData();
		val.threatDetectedData.range = Range;
		return val;
	}

	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		BaseEntity nearestThreat = senses.GetNearestThreat(Range);
		if (base.Inverted)
		{
			if ((Object)(object)nearestThreat == (Object)null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Remove(base.OutputEntityMemorySlot);
			}
			base.Result = (Object)(object)nearestThreat == (Object)null;
		}
		else
		{
			if ((Object)(object)nearestThreat != (Object)null && base.ShouldSetOutputEntityMemory)
			{
				memory.Entity.Set(nearestThreat, base.OutputEntityMemorySlot);
			}
			base.Result = (Object)(object)nearestThreat != (Object)null;
		}
	}
}
