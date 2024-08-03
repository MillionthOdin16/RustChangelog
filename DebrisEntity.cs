using System;
using ConVar;

public class DebrisEntity : BaseCombatEntity
{
	public float DebrisDespawnOverride = 0f;

	public override void ServerInit()
	{
		ResetRemovalTime();
		base.ServerInit();
	}

	public void RemoveCorpse()
	{
		Kill();
	}

	public void ResetRemovalTime(float dur)
	{
		TimeWarning val = TimeWarning.New("ResetRemovalTime", 0);
		try
		{
			if (((FacepunchBehaviour)this).IsInvoking((Action)RemoveCorpse))
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)RemoveCorpse);
			}
			((FacepunchBehaviour)this).Invoke((Action)RemoveCorpse, dur);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public float GetRemovalTime()
	{
		return (DebrisDespawnOverride > 0f) ? DebrisDespawnOverride : Server.debrisdespawn;
	}

	public void ResetRemovalTime()
	{
		ResetRemovalTime(GetRemovalTime());
	}

	public override string Categorize()
	{
		return "debris";
	}
}
