using Network;

public class EntityComponentBase : BaseMonoBehaviour
{
	public virtual BaseEntity GetBaseEntity()
	{
		return null;
	}

	public virtual void SaveComponent(BaseNetworkable.SaveInfo info)
	{
	}

	public virtual void LoadComponent(BaseNetworkable.LoadInfo info)
	{
	}

	public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}
}
