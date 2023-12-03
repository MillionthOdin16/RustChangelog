using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Underwear")]
public class Underwear : ScriptableObject
{
	public string shortname = "";

	public Phrase displayName;

	public Sprite icon;

	public Sprite iconFemale;

	public SkinReplacement[] replacementsMale;

	public SkinReplacement[] replacementsFemale;

	[Tooltip("User can craft this item on any server if they have this steam item")]
	public SteamInventoryItem steamItem;

	[Tooltip("User can craft this item if they have this DLC purchased")]
	public SteamDLCItem steamDLC;

	public bool adminOnly = false;

	public uint GetID()
	{
		return StringPool.Get(shortname);
	}

	public bool HasMaleParts()
	{
		return replacementsMale.Length != 0;
	}

	public bool HasFemaleParts()
	{
		return replacementsFemale.Length != 0;
	}

	public bool ValidForPlayer(BasePlayer player)
	{
		if (HasMaleParts() && HasFemaleParts())
		{
			return true;
		}
		bool flag = IsFemale(player);
		if (flag && HasFemaleParts())
		{
			return true;
		}
		if (!flag && HasMaleParts())
		{
			return true;
		}
		return false;
	}

	public static bool IsFemale(BasePlayer player)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		ulong userID = player.userID;
		ulong num = 4332uL;
		State state = Random.state;
		Random.InitState((int)(num + userID));
		float num2 = Random.Range(0f, 1f);
		Random.state = state;
		return num2 > 0.5f;
	}

	public static bool Validate(Underwear underwear, BasePlayer player)
	{
		if ((Object)(object)underwear == (Object)null)
		{
			return true;
		}
		if (!underwear.ValidForPlayer(player))
		{
			return false;
		}
		if (underwear.adminOnly && (!player.IsAdmin || !player.IsDeveloper))
		{
			return false;
		}
		bool flag = (Object)(object)underwear.steamItem == (Object)null || player.blueprints.steamInventory.HasItem(underwear.steamItem.id);
		bool flag2 = false;
		if (player.isServer && ((Object)(object)underwear.steamDLC == (Object)null || underwear.steamDLC.HasLicense(player.userID)))
		{
			flag2 = true;
		}
		return flag && flag2;
	}
}
