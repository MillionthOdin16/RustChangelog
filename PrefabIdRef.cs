using System;
using UnityEngine;

[Serializable]
public struct PrefabIdRef
{
	public uint PrefabId;

	public BaseEntity EditorSideEntity;

	public bool Equals(BaseEntity other)
	{
		if ((Object)(object)other != (Object)null)
		{
			return PrefabId == other.prefabID;
		}
		return false;
	}

	public bool Equals(uint other)
	{
		return PrefabId == other;
	}
}
