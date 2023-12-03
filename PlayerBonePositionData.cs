using Facepunch;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Player/Player Bone Position Data", fileName = "Player Bone Position Data")]
public class PlayerBonePositionData : ScriptableObject
{
	public SkeletonDefinition skeletonDef;

	public GameObjectRef playerModel;

	public AnimationClip sourceAnim;

	public float animTime;

	public Vector3 rootRotationOffset;

	public string[] boneNames;

	public Vector3[] bonePositions;

	public Quaternion[] boneRotations;
}
