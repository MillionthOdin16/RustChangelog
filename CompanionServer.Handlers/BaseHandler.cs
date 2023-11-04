using Facepunch;
using ProtoBuf;

namespace CompanionServer.Handlers;

public abstract class BaseHandler<T> : IHandler, IPooled where T : class
{
	protected TokenBucketList<ulong> PlayerBuckets { get; private set; }

	protected virtual double TokenCost => 1.0;

	public IConnection Client { get; private set; }

	public AppRequest Request { get; private set; }

	public T Proto { get; private set; }

	public void Initialize(TokenBucketList<ulong> playerBuckets, IConnection client, AppRequest request, T proto)
	{
		PlayerBuckets = playerBuckets;
		Client = client;
		Request = request;
		Proto = proto;
	}

	public virtual void EnterPool()
	{
		PlayerBuckets = null;
		Client = null;
		if (Request != null)
		{
			Request.Dispose();
			Request = null;
		}
		Proto = null;
	}

	public void LeavePool()
	{
	}

	public virtual ValidationResult Validate()
	{
		return ValidationResult.Success;
	}

	public abstract void Execute();

	protected void SendSuccess()
	{
		AppSuccess success = Pool.Get<AppSuccess>();
		AppResponse val = Pool.Get<AppResponse>();
		val.success = success;
		Send(val);
	}

	public void SendError(string code)
	{
		AppError val = Pool.Get<AppError>();
		val.error = code;
		AppResponse val2 = Pool.Get<AppResponse>();
		val2.error = val;
		Send(val2);
	}

	public void SendFlag(bool value)
	{
		AppFlag val = Pool.Get<AppFlag>();
		val.value = value;
		AppResponse val2 = Pool.Get<AppResponse>();
		val2.flag = val;
		Send(val2);
	}

	protected void Send(AppResponse response)
	{
		response.seq = Request.seq;
		Client.Send(response);
	}
}
