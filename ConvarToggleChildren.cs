using UnityEngine;

public class ConvarToggleChildren : MonoBehaviour
{
	public string ConvarName = null;

	public string ConvarEnabled = "True";

	private bool state = false;

	private Command Command;

	protected void Awake()
	{
		Command = Client.Find(ConvarName);
		if (Command == null)
		{
			Command = Server.Find(ConvarName);
		}
		if (Command != null)
		{
			SetState(Command.String == ConvarEnabled);
		}
	}

	protected void Update()
	{
		if (Command != null)
		{
			bool flag = Command.String == ConvarEnabled;
			if (state != flag)
			{
				SetState(flag);
			}
		}
	}

	private void SetState(bool newState)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		foreach (Transform item in ((Component)this).transform)
		{
			Transform val = item;
			((Component)val).gameObject.SetActive(newState);
		}
		state = newState;
	}
}
