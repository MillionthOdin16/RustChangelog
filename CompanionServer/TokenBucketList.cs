using System.Collections.Generic;
using Facepunch;

namespace CompanionServer;

public class TokenBucketList<TKey> : ITokenBucketSettings
{
	private readonly Dictionary<TKey, TokenBucket> _buckets;

	public double MaxTokens { get; }

	public double TokensPerSec { get; }

	public TokenBucketList(double maxTokens, double tokensPerSec)
	{
		_buckets = new Dictionary<TKey, TokenBucket>();
		MaxTokens = maxTokens;
		TokensPerSec = tokensPerSec;
	}

	public TokenBucket Get(TKey key)
	{
		if (_buckets.TryGetValue(key, out var value))
		{
			return value;
		}
		TokenBucket tokenBucket = Pool.Get<TokenBucket>();
		tokenBucket.Settings = this;
		tokenBucket.Reset();
		_buckets.Add(key, tokenBucket);
		return tokenBucket;
	}

	public void Cleanup()
	{
		List<TKey> list = Pool.GetList<TKey>();
		foreach (KeyValuePair<TKey, TokenBucket> bucket in _buckets)
		{
			if (bucket.Value.IsFull)
			{
				list.Add(bucket.Key);
			}
		}
		foreach (TKey item in list)
		{
			if (_buckets.TryGetValue(item, out var value))
			{
				Pool.Free<TokenBucket>(ref value);
				_buckets.Remove(item);
			}
		}
		Pool.FreeList<TKey>(ref list);
	}
}
