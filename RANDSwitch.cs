using UnityEngine;

public class RANDSwitch : ElectricalBlocker
{
	private bool rand = false;

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int passthroughAmount = base.GetPassthroughAmount(outputSlot);
		return passthroughAmount * ((!IsOn()) ? 1 : 0);
	}

	public override void UpdateBlocked()
	{
		bool flag = IsOn();
		SetFlag(Flags.On, rand, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved8, rand, recursive: false, networkupdate: false);
		UpdateHasPower(input1Amount + input2Amount, 1);
		if (flag != IsOn())
		{
			MarkDirty();
		}
	}

	public bool RandomRoll()
	{
		return Random.Range(0, 2) == 1;
	}

	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && inputAmount > 0)
		{
			input1Amount = inputAmount;
			rand = RandomRoll();
			UpdateBlocked();
		}
		if (inputSlot == 2)
		{
			if (inputAmount > 0)
			{
				rand = false;
				UpdateBlocked();
			}
		}
		else
		{
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}
}
