namespace GameTips;

public abstract class BaseTip
{
	public abstract bool ShouldShow { get; }

	public string Type => GetType().Name;

	public virtual bool CanShowInTutorial => false;

	protected bool PlayerIsInTutorial => false;

	public abstract Phrase GetPhrase();
}
