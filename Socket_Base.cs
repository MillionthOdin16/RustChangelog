using System;
using UnityEngine;

public class Socket_Base : PrefabAttribute
{
	[Serializable]
	public class OccupiedSocketCheck
	{
		public Socket_Base Socket;

		public bool FemaleDummy;
	}

	public bool male = true;

	public bool maleDummy = false;

	public bool female = false;

	public bool femaleDummy = false;

	public bool femaleNoStability = false;

	public bool monogamous = false;

	[NonSerialized]
	public Vector3 position;

	[NonSerialized]
	public Quaternion rotation;

	private Type cachedType;

	public Vector3 selectSize = new Vector3(2f, 0.1f, 2f);

	public Vector3 selectCenter = new Vector3(0f, 0f, 1f);

	[ReadOnly]
	public string socketName;

	[NonSerialized]
	public SocketMod[] socketMods;

	public OccupiedSocketCheck[] checkOccupiedSockets;

	public Socket_Base()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		cachedType = ((object)this).GetType();
	}

	public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return position + rotation * worldPosition;
	}

	public OBB GetSelectBounds(Vector3 position, Quaternion rotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		return new OBB(position + rotation * worldPosition, Vector3.one, rotation * worldRotation, new Bounds(selectCenter, selectSize));
	}

	protected override Type GetIndexedType()
	{
		return typeof(Socket_Base);
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		position = ((Component)this).transform.position;
		rotation = ((Component)this).transform.rotation;
		socketMods = ((Component)this).GetComponentsInChildren<SocketMod>(true);
		SocketMod[] array = socketMods;
		foreach (SocketMod socketMod in array)
		{
			socketMod.baseSocket = this;
		}
	}

	public virtual bool TestTarget(Construction.Target target)
	{
		return target.socket != null;
	}

	public virtual bool IsCompatible(Socket_Base socket)
	{
		if (socket == null)
		{
			return false;
		}
		if (!socket.male && !male)
		{
			return false;
		}
		if (!socket.female && !female)
		{
			return false;
		}
		return socket.cachedType == cachedType;
	}

	public virtual bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		return IsCompatible(socket);
	}

	public virtual Construction.Placement DoPlacement(Construction.Target target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.LookRotation(target.normal, Vector3.up) * Quaternion.Euler(target.rotation);
		Vector3 val2 = target.position;
		val2 -= val * position;
		Construction.Placement placement = new Construction.Placement();
		placement.rotation = val;
		placement.position = val2;
		return placement;
	}

	public virtual bool CheckSocketMods(Construction.Placement placement)
	{
		SocketMod[] array = socketMods;
		foreach (SocketMod socketMod in array)
		{
			socketMod.ModifyPlacement(placement);
		}
		SocketMod[] array2 = socketMods;
		foreach (SocketMod socketMod2 in array2)
		{
			if (!socketMod2.DoCheck(placement))
			{
				if (socketMod2.FailedPhrase.IsValid())
				{
					Construction.lastPlacementError = "Failed Check: (" + socketMod2.FailedPhrase.translated + ")";
				}
				return false;
			}
		}
		return true;
	}
}
