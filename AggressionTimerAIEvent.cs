using ProtoBuf;

public class AggressionTimerAIEvent : BaseAIEvent
{
	public float Value { get; private set; }

	public AggressionTimerAIEvent()
		: base(AIEventType.AggressionTimer)
	{
		base.Rate = ExecuteRate.Fast;
	}

	public override void Init(AIEventData data, BaseEntity owner)
	{
		base.Init(data, owner);
		AggressionTimerAIEventData aggressionTimerData = data.aggressionTimerData;
		Value = aggressionTimerData.value;
	}

	public override AIEventData ToProto()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		AIEventData val = base.ToProto();
		val.aggressionTimerData = new AggressionTimerAIEventData();
		val.aggressionTimerData.value = Value;
		return val;
	}

	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (base.Inverted)
		{
			base.Result = senses.TimeInAgressiveState < Value;
		}
		else
		{
			base.Result = senses.TimeInAgressiveState >= Value;
		}
	}
}
