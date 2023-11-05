using Rust.UI;
using UnityEngine;

public class ChangelogButton : MonoBehaviour
{
	public RustButton Button;

	public CanvasGroup CanvasGroup;

	private void Update()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: false);
		if ((Object)(object)activeGameMode != (Object)null)
		{
			if (CanvasGroup.alpha != 1f)
			{
				CanvasGroup.alpha = 1f;
				CanvasGroup.blocksRaycasts = true;
				Button.Text.SetPhrase(new Phrase(activeGameMode.shortname, activeGameMode.shortname));
			}
		}
		else if (CanvasGroup.alpha != 0f)
		{
			CanvasGroup.alpha = 0f;
			CanvasGroup.blocksRaycasts = false;
		}
	}
}
