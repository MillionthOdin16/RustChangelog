using System.Linq;
using UnityEngine;

public class RealmedRemove : MonoBehaviour, IPrefabPreProcess
{
	public GameObject[] removedFromClient;

	public Component[] removedComponentFromClient;

	public GameObject[] removedFromServer;

	public Component[] removedComponentFromServer;

	public Component[] doNotRemoveFromServer;

	public Component[] doNotRemoveFromClient;

	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside)
		{
			GameObject[] array = removedFromClient;
			foreach (GameObject val in array)
			{
				Object.DestroyImmediate((Object)(object)val, true);
			}
			Component[] array2 = removedComponentFromClient;
			foreach (Component val2 in array2)
			{
				Object.DestroyImmediate((Object)(object)val2, true);
			}
		}
		if (serverside)
		{
			GameObject[] array3 = removedFromServer;
			foreach (GameObject val3 in array3)
			{
				Object.DestroyImmediate((Object)(object)val3, true);
			}
			Component[] array4 = removedComponentFromServer;
			foreach (Component val4 in array4)
			{
				Object.DestroyImmediate((Object)(object)val4, true);
			}
		}
		if (!bundling)
		{
			process.RemoveComponent((Component)(object)this);
		}
	}

	public bool ShouldDelete(Component comp, bool client, bool server)
	{
		if (client && doNotRemoveFromClient != null && doNotRemoveFromClient.Contains(comp))
		{
			return false;
		}
		if (server && doNotRemoveFromServer != null && doNotRemoveFromServer.Contains(comp))
		{
			return false;
		}
		return true;
	}
}
