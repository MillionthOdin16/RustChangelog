using Rust;
using UnityEngine;

public class TunnelDweller : HumanNPC
{
	private const string DWELLER_KILL_STAT = "dweller_kills_while_moving";

	protected override string OverrideCorpseName()
	{
		return "Tunnel Dweller";
	}

	protected override void OnKilledByPlayer(BasePlayer p)
	{
		base.OnKilledByPlayer(p);
		if (GameInfo.HasAchievements && (Object)(object)p.GetParentEntity() != (Object)null && p.GetParentEntity() is TrainEngine trainEngine && trainEngine.CurThrottleSetting != TrainEngine.EngineSpeeds.Zero && trainEngine.IsMovingOrOn)
		{
			p.stats.Add("dweller_kills_while_moving", 1, Stats.All);
			p.stats.Save(forceSteamSave: true);
		}
	}
}
