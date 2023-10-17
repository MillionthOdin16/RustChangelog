using Rust.UI;
using UnityEngine;

public class ServerBrowserTag : MonoBehaviour
{
	public string serverTag;

	public RustButton button;

	public bool IsActive
	{
		get
		{
			if ((Object)(object)button != (Object)null)
			{
				return ((RustControl)button).IsPressed;
			}
			return false;
		}
	}
}
