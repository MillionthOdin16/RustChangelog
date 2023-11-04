using UnityEngine;

public static class GameObjectUtil
{
	public static void GlobalBroadcast(string messageName, object param = null)
	{
		Transform[] rootObjects = TransformUtil.GetRootObjects();
		Transform[] array = rootObjects;
		foreach (Transform val in array)
		{
			((Component)val).BroadcastMessage(messageName, param, (SendMessageOptions)1);
		}
	}
}
