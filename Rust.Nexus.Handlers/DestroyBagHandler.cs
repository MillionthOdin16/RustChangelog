using ProtoBuf.Nexus;

namespace Rust.Nexus.Handlers;

public class DestroyBagHandler : BaseNexusRequestHandler<SleepingBagDestroyRequest>
{
	protected override void Handle()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		SleepingBag.DestroyBag(base.Request.userId, base.Request.sleepingBagId);
	}
}
