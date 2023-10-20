using UnityEngine;

public class uiPlayerPreview : SingletonComponent<uiPlayerPreview>
{
	public Camera previewCamera;

	public PlayerModel playermodel;

	public GameObject wantedSnapshotEffectRoot;

	public SegmentMaskPositioning segmentMask;
}
