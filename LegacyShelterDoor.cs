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

	protected override void OnPlayerOpenedDoor(BasePlayer p)
	{
		base.OnPlayerOpenedDoor(p);
		if ((Object)(object)shelter != (Object)null)
		{
			shelter.HasInteracted();
		}
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
		if (HasParent() && (Object)(object)shelter != (Object)null)
		{
			shelter.ProtectedHurt(info);
		}
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
		if (HasParent() && (Object)(object)shelter != (Object)null)
		{
			shelter.SetHealth(base.health);
		}
	}
}
