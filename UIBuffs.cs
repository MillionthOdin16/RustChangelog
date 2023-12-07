using UnityEngine;

public class UIBuffs : SingletonComponent<UIBuffs>
{
	public bool Enabled = true;

	public Transform PrefabBuffIcon;

	public void Refresh(PlayerModifiers modifiers)
	{
		if (!Enabled)
		{
			return;
		}
		RemoveAll();
		if ((Object)(object)modifiers == (Object)null)
		{
			return;
		}
		foreach (Modifier item in modifiers.All)
		{
			if (item != null)
			{
				Transform val = Object.Instantiate<Transform>(PrefabBuffIcon);
				val.SetParent(((Component)this).transform);
			}
		}
	}

	private void RemoveAll()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		foreach (Transform item in ((Component)this).transform)
		{
			Transform val = item;
			Object.Destroy((Object)(object)((Component)val).gameObject);
		}
	}
}
