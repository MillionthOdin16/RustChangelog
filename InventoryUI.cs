using UnityEngine;

public class InventoryUI : MonoBehaviour
{
	public GameObject ContactsButton;

	private void Update()
	{
		if ((Object)(object)ContactsButton != (Object)null && RelationshipManager.contacts != ContactsButton.activeSelf)
		{
			ContactsButton.SetActive(RelationshipManager.contacts);
		}
	}
}
