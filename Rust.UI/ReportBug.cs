using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI;

public class ReportBug : UIDialog
{
	public GameObject GetInformation;

	public GameObject Finished;

	public RustInput Subject;

	public RustInput Message;

	public RustButton ReportButton;

	public RustButtonGroup Category;

	public RustIcon ProgressIcon;

	public RustText ProgressText;

	public RawImage ScreenshotImage;

	public GameObject ScreenshotRoot;

	public UIBackgroundBlur BlurController;

	public RustButton SubmitButton = null;

	public GameObject SubmitErrorRoot = null;

	public RustText CooldownText = null;

	public RustText ContentMissingText = null;
}
