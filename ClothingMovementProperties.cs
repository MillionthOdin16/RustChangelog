using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Clothing Movement Properties")]
public class ClothingMovementProperties : ScriptableObject
{
	public float speedReduction = 0f;

	[Tooltip("If this piece of clothing is worn movement speed will be reduced by atleast this much")]
	public float minSpeedReduction = 0f;

	public float waterSpeedBonus = 0f;
}
