using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

public class ConnectedSpeaker : IOEntity
{
	public AudioSource SoundSource = null;

	private EntityRef<IOEntity> connectedTo;

	public VoiceProcessor VoiceProcessor;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ConnectedSpeaker.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		base.OnFlagsChanged(old, next);
		if (!base.isServer || old.HasFlag(Flags.Reserved8) == next.HasFlag(Flags.Reserved8))
		{
			return;
		}
		if (next.HasFlag(Flags.Reserved8))
		{
			IAudioConnectionSource connectionSource = GetConnectionSource(this, BoomBox.BacktrackLength);
			if (connectionSource != null)
			{
				ClientRPC<NetworkableId>(null, "Client_PlayAudioFrom", connectionSource.ToEntity().net.ID);
				connectedTo.Set(connectionSource.ToEntity());
			}
		}
		else if (connectedTo.IsSet)
		{
			ClientRPC<NetworkableId>(null, "Client_StopPlayingAudio", connectedTo.uid);
			connectedTo.Set(null);
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.connectedSpeaker != null)
		{
			connectedTo.uid = info.msg.connectedSpeaker.connectedTo;
		}
	}

	private IAudioConnectionSource GetConnectionSource(IOEntity entity, int depth)
	{
		if (depth <= 0)
		{
			return null;
		}
		IOSlot[] array = entity.inputs;
		foreach (IOSlot iOSlot in array)
		{
			IOEntity iOEntity = iOSlot.connectedTo.Get(base.isServer);
			if ((Object)(object)iOEntity == (Object)(object)this)
			{
				return null;
			}
			if ((Object)(object)iOEntity != (Object)null && iOEntity is IAudioConnectionSource result)
			{
				return result;
			}
			if ((Object)(object)iOEntity != (Object)null)
			{
				IAudioConnectionSource connectionSource = GetConnectionSource(iOEntity, depth - 1);
				if (connectionSource != null)
				{
					return connectionSource;
				}
			}
		}
		return null;
	}

	public override void Save(SaveInfo info)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (info.msg.connectedSpeaker == null)
		{
			info.msg.connectedSpeaker = Pool.Get<ConnectedSpeaker>();
		}
		info.msg.connectedSpeaker.connectedTo = connectedTo.uid;
	}
}
