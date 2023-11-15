using System;

public class NPCPlayerCorpse : PlayerCorpse
{
	private bool lootEnabled = false;

	public override float GetRemovalTime()
	{
		return 600f;
	}

	public override bool CanLoot()
	{
		return lootEnabled;
	}

	public void SetLootableIn(float when)
	{
		((FacepunchBehaviour)this).Invoke((Action)EnableLooting, when);
	}

	public void EnableLooting()
	{
		lootEnabled = true;
	}

	protected override bool CanLootContainer(ItemContainer c, int index)
	{
		if (index == 1 || index == 2)
		{
			return false;
		}
		return base.CanLootContainer(c, index);
	}

	protected override void PreDropItems()
	{
		base.PreDropItems();
		if (containers != null && containers.Length >= 2)
		{
			containers[1].Clear();
			ItemManager.DoRemoves();
		}
	}
}
