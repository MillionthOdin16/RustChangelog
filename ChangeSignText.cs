using System;
using Rust.UI;
using UnityEngine;

public class ChangeSignText : UIDialog
{
	public Action<int, Texture2D> onUpdateTexture;

	public Action onClose;

	public GameObject objectContainer;

	public GameObject currentFrameSection;

	public GameObject[] frameOptions;

	public Canvas canvas;

	public RectTransform rightPanelRect;

	public Camera cameraPreview;

	public Camera camera3D;

	public Light previewLight;

	public Vector3 homeRotation;

	public RectTransform toolsContainer;

	public RectTransform brushesContainer;

	public RustSlider brushSizeSlider;

	public RustSlider brushSpacingSlider;

	public RustSlider brushOpacitySlider;
}
