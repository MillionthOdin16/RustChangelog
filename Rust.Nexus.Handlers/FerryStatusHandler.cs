using System;
using Facepunch;
using ProtoBuf.Nexus;
using UnityEngine;

namespace Rust.Nexus.Handlers;

public class FerryStatusHandler : BaseNexusRequestHandler<FerryStatusRequest>
{
	protected override void Handle()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		FerryStatusResponse val = Pool.Get<FerryStatusResponse>();
		val.statuses = Pool.GetList<FerryStatus>();
		Enumerator<NexusFerry> enumerator = NexusFerry.All.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				NexusFerry current = enumerator.Current;
				if (!((Object)(object)current == (Object)null))
				{
					FerryStatus status = current.GetStatus();
					val.statuses.Add(status);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		Response val2 = Pool.Get<Response>();
		val2.ferryStatus = val;
		SendSuccess(val2);
	}
}
