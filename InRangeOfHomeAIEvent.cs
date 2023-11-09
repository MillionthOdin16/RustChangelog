using ProtoBuf;
using UnityEngine;

public class InRangeOfHomeAIEvent : BaseAIEvent
{
	public float Range { get; set; }

	public InRangeOfHomeAIEvent()
		: base(AIEventType.InRangeOfHome)
	{
		base.Rate = ExecuteRate.Fast;
	}

	public override void Init(AIEventData data, BaseEntity owner)
	{
		base.Init(data, owner);
		InRangeOfHomeAIEventData inRangeOfHomeData = data.inRangeOfHomeData;
		Range = inRangeOfHomeData.range;
	}

	public override AIEventData ToProto()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		AIEventData val = base.ToProto();
		val.inRangeOfHomeData = new InRangeOfHomeAIEventData();
		val.inRangeOfHomeData.range = Range;
		return val;
	}

	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = memory.Position.Get(4);
		base.Result = false;
		bool flag = Vector3Ex.Distance2D(((Component)base.Owner).transform.position, val) <= Range;
		base.Result = (base.Inverted ? (!flag) : flag);
	}
}
