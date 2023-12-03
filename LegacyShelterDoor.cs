using UnityEngine;

public class LegacyShelterDoor : Door
{
	private LegacyShelter shelter;

	public void SetupDoor(LegacyShelter shelter)
	{
		this.shelter = shelter;
	}

	public override void DecayTick()
	{
	}

	public override void OnRepair()
	{
		base.OnRepair();
		UpdateShelterHp();
	}

	public override void OnRepairFinished()
	{
		base.OnRepairFinished();
		UpdateShelterHp();
	}

	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
		shelter.ProtectedHurt(info);
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if ((Object)(object)shelter != (Object)null && !shelter.IsDead())
		{
			shelter.Die();
		}
	}

	public void ProtectedHurt(HitInfo info)
	{
		info.HitEntity = this;
		base.Hurt(info);
	}

	private void UpdateShelterHp()
	{
		if ((Object)(object)shelter != (Object)null)
		{
			shelter.SetHealth(base.health);
		}
	}
}
