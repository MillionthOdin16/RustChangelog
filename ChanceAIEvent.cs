using ProtoBuf;
using UnityEngine;

public class ChanceAIEvent : BaseAIEvent
{
	public float Chance { get; set; }

	public ChanceAIEvent()
		: base(AIEventType.Chance)
	{
		base.Rate = ExecuteRate.Fast;
	}

	public override void Init(AIEventData data, BaseEntity owner)
	{
		base.Init(data, owner);
		Chance = data.chanceData.value;
	}

	public override AIEventData ToProto()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		AIEventData val = base.ToProto();
		val.chanceData = new ChanceAIEventData();
		val.chanceData.value = Chance;
		return val;
	}

	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		bool flag = Random.Range(0f, 1f) <= Chance;
		if (base.Inverted)
		{
			base.Result = !flag;
		}
		else
		{
			base.Result = flag;
		}
	}
}
