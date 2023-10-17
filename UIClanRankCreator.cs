using Rust.UI;

public class UIClanRankCreator : BaseMonoBehaviour
{
	public static readonly Phrase CreateRankFailure = (Phrase)(object)new TokenisedPhrase("clan.create_rank.fail", "Failed to create the new rank.");

	public static readonly Phrase CreateRankDuplicate = (Phrase)(object)new TokenisedPhrase("clan.create_rank.duplicate", "There is already a rank in your clan with that name.");

	public UIClans UiClans;

	public RustInput RankName;

	public RustButton Submit;
}
