using ProtoBuf;

public class AmmoBelowAIEvent : BaseAIEvent
{
	public float Value { get; private set; }

	public AmmoBelowAIEvent()
		: base(AIEventType.AmmoBelow)
	{
		base.Rate = ExecuteRate.Normal;
	}

	public override void Init(AIEventData data, BaseEntity owner)
	{
		base.Init(data, owner);
		AmmoBelowAIEventData ammoBelowData = data.ammoBelowData;
		Value = ammoBelowData.value;
	}

	public override AIEventData ToProto()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		AIEventData val = base.ToProto();
		val.ammoBelowData = new AmmoBelowAIEventData();
		val.ammoBelowData.value = Value;
		return val;
	}

	public override void Execute(AIMemory memory, AIBrainSenses senses, StateStatus stateStatus)
	{
		base.Result = base.Inverted;
		if (base.Owner is IAIAttack iAIAttack)
		{
			bool flag = iAIAttack.GetAmmoFraction() < Value;
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
}
