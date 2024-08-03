using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Profiling;

namespace CompanionServer.Cameras;

public class CameraRenderer : IPooled
{
	[ServerVar]
	public static bool enabled = true;

	[ServerVar]
	public static float completionFrameBudgetMs = 5f;

	[ServerVar]
	public static int maxRendersPerFrame = 25;

	[ServerVar]
	public static int maxRaysPerFrame = 100000;

	[ServerVar]
	public static int width = 160;

	[ServerVar]
	public static int height = 90;

	[ServerVar]
	public static float verticalFov = 65f;

	[ServerVar]
	public static float nearPlane = 0f;

	[ServerVar]
	public static float farPlane = 250f;

	[ServerVar]
	public static int layerMask = 1218656529;

	[ServerVar]
	public static float renderInterval = 0.05f;

	[ServerVar]
	public static int samplesPerRender = 3000;

	[ServerVar]
	public static int entityMaxAge = 5;

	[ServerVar]
	public static int entityMaxDistance = 100;

	[ServerVar]
	public static int playerMaxDistance = 30;

	[ServerVar]
	public static int playerNameMaxDistance = 10;

	private static readonly Dictionary<NetworkableId, NetworkableId> _entityIdMap = new Dictionary<NetworkableId, NetworkableId>();

	private readonly Dictionary<int, (byte MaterialIndex, int Age)> _knownColliders = new Dictionary<int, (byte, int)>();

	private readonly Dictionary<int, BaseEntity> _colliderToEntity = new Dictionary<int, BaseEntity>();

	private double _lastRenderTimestamp;

	private float _fieldOfView;

	private int _sampleOffset;

	private int _nextSampleOffset;

	private int _sampleCount;

	private CameraRenderTask _task;

	private ulong? _cachedViewerSteamId;

	private BasePlayer _cachedViewer;

	public CameraRendererState state;

	public IRemoteControllable rc;

	public BaseEntity entity;

	public CameraRenderer()
	{
		Reset();
	}

	public void EnterPool()
	{
		Reset();
	}

	public void LeavePool()
	{
	}

	public void Reset()
	{
		_knownColliders.Clear();
		_colliderToEntity.Clear();
		_lastRenderTimestamp = 0.0;
		_fieldOfView = 0f;
		_sampleOffset = 0;
		_nextSampleOffset = 0;
		_sampleCount = 0;
		if (_task != null)
		{
			CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
			if ((Object)(object)instance != (Object)null)
			{
				instance.ReturnTask(ref _task);
			}
		}
		_cachedViewerSteamId = null;
		_cachedViewer = null;
		state = CameraRendererState.Invalid;
		rc = null;
		entity = null;
	}

	public void Init(IRemoteControllable remoteControllable)
	{
		if (remoteControllable == null)
		{
			throw new ArgumentNullException("remoteControllable");
		}
		rc = remoteControllable;
		entity = remoteControllable.GetEnt();
		if ((Object)(object)entity == (Object)null || !entity.IsValid())
		{
			throw new ArgumentException("RemoteControllable's entity is null or invalid", "rc");
		}
		state = CameraRendererState.WaitingToRender;
	}

	public bool CanRender()
	{
		if (state != CameraRendererState.WaitingToRender)
		{
			return false;
		}
		if (TimeEx.realtimeSinceStartup - _lastRenderTimestamp < (double)renderInterval)
		{
			return false;
		}
		return true;
	}

	public void Render(int maxSampleCount)
	{
		Profiler.BeginSample("CameraRenderer.Render");
		CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
		if ((Object)(object)instance == (Object)null)
		{
			state = CameraRendererState.Invalid;
			Profiler.EndSample();
			return;
		}
		if (state != CameraRendererState.WaitingToRender)
		{
			throw new InvalidOperationException($"CameraRenderer cannot render in state {state}");
		}
		if (rc.IsUnityNull() || !entity.IsValid())
		{
			state = CameraRendererState.Invalid;
			Profiler.EndSample();
			return;
		}
		Transform eyes = rc.GetEyes();
		if ((Object)(object)eyes == (Object)null)
		{
			state = CameraRendererState.Invalid;
			Profiler.EndSample();
			return;
		}
		if (_task != null)
		{
			Debug.LogError((object)"CameraRenderer: Trying to render but a task is already allocated?", (Object)(object)entity);
			instance.ReturnTask(ref _task);
		}
		_fieldOfView = verticalFov / Mathf.Clamp(rc.GetFovScale(), 1f, 8f);
		_sampleCount = Mathf.Clamp(samplesPerRender, 1, Mathf.Min(width * height, maxSampleCount));
		_task = instance.BorrowTask();
		_nextSampleOffset = _task.Start(width, height, _fieldOfView, nearPlane, farPlane, layerMask, eyes, _sampleCount, _sampleOffset, _knownColliders);
		state = CameraRendererState.Rendering;
		Profiler.EndSample();
	}

	public void CompleteRender()
	{
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("CameraRenderer.CompleteRender");
		CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
		if ((Object)(object)instance == (Object)null)
		{
			state = CameraRendererState.Invalid;
			Profiler.EndSample();
			return;
		}
		if (state != CameraRendererState.Rendering)
		{
			throw new InvalidOperationException($"CameraRenderer cannot complete render in state {state}");
		}
		if (_task == null)
		{
			Debug.LogError((object)"CameraRenderer: Trying to complete render but no task is allocated?", (Object)(object)entity);
			state = CameraRendererState.Invalid;
			Profiler.EndSample();
			return;
		}
		if (((CustomYieldInstruction)_task).keepWaiting)
		{
			Profiler.EndSample();
			return;
		}
		if (rc.IsUnityNull() || !entity.IsValid())
		{
			instance.ReturnTask(ref _task);
			state = CameraRendererState.Invalid;
			Profiler.EndSample();
			return;
		}
		Transform eyes = rc.GetEyes();
		if ((Object)(object)eyes == (Object)null)
		{
			instance.ReturnTask(ref _task);
			state = CameraRendererState.Invalid;
			Profiler.EndSample();
			return;
		}
		int num = _sampleCount * 4;
		byte[] array = ArrayPool<byte>.Shared.Rent(num);
		List<int> list = Pool.GetList<int>();
		List<int> list2 = Pool.GetList<int>();
		int count = _task.ExtractRayData(array, list, list2);
		instance.ReturnTask(ref _task);
		UpdateCollidersMap(list2);
		Pool.FreeList<int>(ref list);
		Pool.FreeList<int>(ref list2);
		ulong num2 = rc.ControllingViewerId?.SteamId ?? 0;
		if (num2 == 0)
		{
			_cachedViewerSteamId = null;
			_cachedViewer = null;
		}
		else if (num2 != _cachedViewerSteamId)
		{
			_cachedViewerSteamId = num2;
			_cachedViewer = BasePlayer.FindByID(num2) ?? BasePlayer.FindSleeping(num2);
		}
		float distance = (_cachedViewer.IsValid() ? Mathf.Clamp01(Vector3.Distance(((Component)_cachedViewer).transform.position, ((Component)entity).transform.position) / rc.MaxRange) : 0f);
		Vector3 position = eyes.position;
		Quaternion rotation = eyes.rotation;
		Matrix4x4 worldToLocalMatrix = eyes.worldToLocalMatrix;
		NetworkableId iD = entity.net.ID;
		Profiler.BeginSample("CameraRenderer.BroadcastRays");
		_entityIdMap.Clear();
		AppBroadcast val = Pool.Get<AppBroadcast>();
		val.cameraRays = Pool.Get<AppCameraRays>();
		val.cameraRays.verticalFov = _fieldOfView;
		val.cameraRays.sampleOffset = _sampleOffset;
		val.cameraRays.rayData = new ArraySegment<byte>(array, 0, count);
		val.cameraRays.distance = distance;
		val.cameraRays.entities = Pool.GetList<Entity>();
		val.cameraRays.timeOfDay = (((Object)(object)TOD_Sky.Instance != (Object)null) ? TOD_Sky.Instance.LerpValue : 1f);
		foreach (BaseEntity value in _colliderToEntity.Values)
		{
			if (!value.IsValid())
			{
				continue;
			}
			Vector3 position2 = ((Component)value).transform.position;
			float num3 = Vector3.Distance(position2, position);
			if (num3 > (float)entityMaxDistance)
			{
				continue;
			}
			string name = null;
			if (value is BasePlayer basePlayer)
			{
				if (num3 > (float)playerMaxDistance)
				{
					continue;
				}
				if (num3 <= (float)playerNameMaxDistance)
				{
					name = basePlayer.displayName;
				}
			}
			Entity val2 = Pool.Get<Entity>();
			val2.entityId = RandomizeEntityId(value.net.ID);
			val2.type = (EntityType)((value is TreeEntity) ? 1 : 2);
			val2.position = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint3x4(position2);
			Quaternion val3 = Quaternion.Inverse(((Component)value).transform.rotation) * rotation;
			val2.rotation = ((Quaternion)(ref val3)).eulerAngles * ((float)Math.PI / 180f);
			val2.size = Vector3.Scale(((Bounds)(ref value.bounds)).size, ((Component)value).transform.localScale);
			val2.name = name;
			val.cameraRays.entities.Add(val2);
		}
		val.cameraRays.entities.Sort((Entity x, Entity y) => x.entityId.Value.CompareTo(y.entityId.Value));
		Server.Broadcast(new CameraTarget(iD), val);
		Profiler.EndSample();
		_sampleOffset = _nextSampleOffset;
		if (!Server.HasAnySubscribers(new CameraTarget(iD)))
		{
			state = CameraRendererState.Invalid;
		}
		else
		{
			_lastRenderTimestamp = TimeEx.realtimeSinceStartup;
			state = CameraRendererState.WaitingToRender;
		}
		Profiler.EndSample();
	}

	private void UpdateCollidersMap(List<int> foundColliderIds)
	{
		Profiler.BeginSample("CameraRenderer.UpdateCollidersMap");
		Profiler.BeginSample("IncrementAge");
		List<int> list = Pool.GetList<int>();
		foreach (int key in _knownColliders.Keys)
		{
			list.Add(key);
		}
		List<int> list2 = Pool.GetList<int>();
		foreach (int item2 in list)
		{
			if (_knownColliders.TryGetValue(item2, out (byte, int) value))
			{
				if (value.Item2 > entityMaxAge)
				{
					list2.Add(item2);
				}
				else
				{
					_knownColliders[item2] = (value.Item1, value.Item2 + 1);
				}
			}
		}
		Pool.FreeList<int>(ref list);
		foreach (int item3 in list2)
		{
			_knownColliders.Remove(item3);
			_colliderToEntity.Remove(item3);
		}
		Pool.FreeList<int>(ref list2);
		Profiler.EndSample();
		Profiler.BeginSample("RegisterNew");
		foreach (int foundColliderId in foundColliderIds)
		{
			if (_knownColliders.Count >= 512)
			{
				break;
			}
			Collider collider = BurstUtil.GetCollider(foundColliderId);
			if ((Object)(object)collider == (Object)null)
			{
				continue;
			}
			byte item;
			if (collider is TerrainCollider)
			{
				item = 1;
			}
			else
			{
				BaseEntity baseEntity = collider.ToBaseEntity();
				PhysicMaterial sharedMaterial = collider.sharedMaterial;
				item = GetMaterialIndex(sharedMaterial, baseEntity);
				if (baseEntity is TreeEntity || baseEntity is BasePlayer)
				{
					_colliderToEntity[foundColliderId] = baseEntity;
				}
			}
			_knownColliders[foundColliderId] = (item, 0);
		}
		Profiler.EndSample();
		Profiler.EndSample();
	}

	private static NetworkableId RandomizeEntityId(NetworkableId realId)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (_entityIdMap.TryGetValue(realId, out var value))
		{
			return value;
		}
		NetworkableId val = default(NetworkableId);
		do
		{
			((NetworkableId)(ref val))._002Ector((ulong)Random.Range(0, 2500));
		}
		while (_entityIdMap.ContainsKey(val));
		_entityIdMap.Add(realId, val);
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte GetMaterialIndex(PhysicMaterial material, BaseEntity entity)
	{
		switch (material.GetName())
		{
		case "Water":
			return 2;
		case "Rock":
			return 3;
		case "Stones":
			return 4;
		case "Wood":
			return 5;
		case "Metal":
			return 6;
		default:
			if ((Object)(object)entity != (Object)null && entity is BasePlayer)
			{
				return 7;
			}
			return 0;
		}
	}
}
