using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

public class ElectricOven : BaseOven
{
	public GameObjectRef IoEntity;

	public Transform IoEntityAnchor;

	private EntityRef<IOEntity> spawnedIo;

	private bool resumeCookingWhenPowerResumes = false;

	protected override bool CanRunWithNoFuel
	{
		get
		{
			if (spawnedIo.IsValid(serverside: true))
			{
				return spawnedIo.Get(serverside: true).IsPowered();
			}
			return false;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!Application.isLoadingSave)
		{
			SpawnIOEnt();
		}
	}

	private void SpawnIOEnt()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (IoEntity.isValid && (Object)(object)IoEntityAnchor != (Object)null)
		{
			IOEntity iOEntity = GameManager.server.CreateEntity(IoEntity.resourcePath, IoEntityAnchor.position, IoEntityAnchor.rotation) as IOEntity;
			iOEntity.SetParent(this, worldPositionStays: true);
			iOEntity.Spawn();
			spawnedIo.Set(iOEntity);
		}
	}

	public void OnIOEntityFlagsChanged(Flags old, Flags next)
	{
		if (!next.HasFlag(Flags.Reserved8) && IsOn())
		{
			StopCooking();
			resumeCookingWhenPowerResumes = true;
		}
		else if (next.HasFlag(Flags.Reserved8) && !IsOn() && resumeCookingWhenPowerResumes)
		{
			StartCooking();
			resumeCookingWhenPowerResumes = false;
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (info.msg.simpleUID == null)
		{
			info.msg.simpleUID = Pool.Get<SimpleUID>();
		}
		info.msg.simpleUID.uid = spawnedIo.uid;
	}

	public override void Load(LoadInfo info)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.simpleUID != null)
		{
			spawnedIo.uid = info.msg.simpleUID.uid;
		}
	}

	protected override bool CanPickupOven()
	{
		return children.Count == 1;
	}
}
