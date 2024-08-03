using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityList", menuName = "Rust/EntityList")]
public class EntityListScriptableObject : ScriptableObject
{
	[SerializeField]
	public BaseEntity[] entities;

	[SerializeField]
	public bool whitelist;

	public bool IsInList(uint prefabId)
	{
		if (entities == null)
		{
			return false;
		}
		BaseEntity[] array = entities;
		foreach (BaseEntity baseEntity in array)
		{
			if (baseEntity.prefabID == prefabId)
			{
				return true;
			}
		}
		return false;
	}
}
