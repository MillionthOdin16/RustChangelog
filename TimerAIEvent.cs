using ProtoBuf;
using UnityEngine;

public class TimerAIEvent : BaseAIEvent
{
	protected float currentDuration;

	protected float elapsedDuration = 0f;

	public float DurationMin { get; set; }

	public float DurationMax { get; set; }

	public TimerAIEvent()
		: base(AIEventType.Timer)
	{
		base.Rate = ExecuteRate.Fast;
	}

	public override void Init(AIEventData data, BaseEntity owner)
	{
		base.Init(data, owner);
		TimerAIEventData timerData = data.timerData;
		DurationMin = timerData.duration;
		DurationMax = timerData.durationMax;
	}

	public override AIEventData ToProto()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		AIEventData val = base.ToProto();
		val.timerData = new TimerAIEventData();
		val.timerData.duration = DurationMin;
		val.timerData.durationMax = DurationMax;
		return val;
	}

	public override void Reset()
	{
		base.Reset();
		currentDuration = Random.Range(DurationMin, DurationMax);
		elapsedDuration = 0f;
	}

	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		elapsedDuration += deltaTime;
		if (elapsedDuration >= currentDuration)
		{
			base.Result = !base.Inverted;
		}
	}
}
