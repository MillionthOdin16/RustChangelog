using System;
using Facepunch;
using UnityEngine;

public class ServerBrowserTagGroup : MonoBehaviour
{
	[Tooltip("If set then queries will filter out servers matching unselected tags in the group")]
	public bool isExclusive;

	[NonSerialized]
	public ServerBrowserTag[] tags;

	private void Initialize()
	{
		if (tags == null)
		{
			tags = ((Component)this).GetComponentsInChildren<ServerBrowserTag>(true);
		}
	}

	public void Awake()
	{
		Initialize();
	}

	public bool AnyActive()
	{
		ServerBrowserTag[] array = tags;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsActive)
			{
				return true;
			}
		}
		return false;
	}

	public void Refresh(in ServerInfo server, ref int tagsEnabled, int maxTags)
	{
		Initialize();
		bool flag = false;
		ServerBrowserTag[] array = tags;
		foreach (ServerBrowserTag serverBrowserTag in array)
		{
			if ((!isExclusive || !flag) && tagsEnabled <= maxTags && ((ServerInfo)(ref server)).Tags.Contains(serverBrowserTag.serverTag))
			{
				ComponentExtensions.SetActive<ServerBrowserTag>(serverBrowserTag, true);
				tagsEnabled++;
				flag = true;
			}
			else
			{
				ComponentExtensions.SetActive<ServerBrowserTag>(serverBrowserTag, false);
			}
		}
		((Component)this).gameObject.SetActive(flag);
	}
}
