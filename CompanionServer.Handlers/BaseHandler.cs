using Facepunch;
using ProtoBuf;
using UnityEngine.Profiling;

namespace CompanionServer.Handlers;

public abstract class BaseHandler<T> : IHandler, IPooled where T : class
{
	private TokenBucketList<ulong> _playerBuckets;

	protected virtual double TokenCost => 1.0;

	public IConnection Client { get; private set; }

	public AppRequest Request { get; private set; }

	public T Proto { get; private set; }

	protected ulong UserId { get; private set; }

	protected BasePlayer Player { get; private set; }

	public void Initialize(TokenBucketList<ulong> playerBuckets, IConnection client, AppRequest request, T proto)
	{
		_playerBuckets = playerBuckets;
		Client = client;
		Request = request;
		Proto = proto;
	}

	public virtual void EnterPool()
	{
		_playerBuckets = null;
		Client = null;
		if (Request != null)
		{
			Request.Dispose();
			Request = null;
		}
		Proto = null;
		UserId = 0uL;
		Player = null;
	}

	public void LeavePool()
	{
	}

	public virtual ValidationResult Validate()
	{
		Profiler.BeginSample("AppHandler.Validate");
		bool locked;
		int orGenerateAppToken = SingletonComponent<ServerMgr>.Instance.persistance.GetOrGenerateAppToken(Request.playerId, out locked);
		if (Request.playerId == 0L || Request.playerToken != orGenerateAppToken)
		{
			Profiler.EndSample();
			return ValidationResult.NotFound;
		}
		if (locked)
		{
			Profiler.EndSample();
			return ValidationResult.Banned;
		}
		ServerUsers.UserGroup userGroup = ServerUsers.Get(Request.playerId)?.group ?? ServerUsers.UserGroup.None;
		if (userGroup == ServerUsers.UserGroup.Banned)
		{
			Profiler.EndSample();
			return ValidationResult.Banned;
		}
		TokenBucket tokenBucket = _playerBuckets?.Get(Request.playerId);
		if (tokenBucket == null || !tokenBucket.TryTake(TokenCost))
		{
			Profiler.EndSample();
			return (tokenBucket != null && tokenBucket.IsNaughty) ? ValidationResult.Rejected : ValidationResult.RateLimit;
		}
		UserId = Request.playerId;
		Player = BasePlayer.FindByID(UserId) ?? BasePlayer.FindSleeping(UserId);
		Client.Subscribe(new PlayerTarget(UserId));
		Profiler.EndSample();
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
