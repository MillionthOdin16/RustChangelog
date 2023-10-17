using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.ServerAdmin;

public class ServerAdminUGCEntryImage : ServerAdminUGCEntry
{
	public RawImage Image;

	public RectTransform Backing;

	public GameObject MultiImageRoot = null;

	public RustText ImageIndex = null;

	public Vector2 OriginalImageSize;
}
