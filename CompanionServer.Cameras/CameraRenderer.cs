using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

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
		CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
		if ((Object)(object)instance == (Object)null)
		{
			state = CameraRendererState.Invalid;
			return;
		}
		if (state != CameraRendererState.WaitingToRender)
		{
			throw new InvalidOperationException($"CameraRenderer cannot render in state {state}");
		}
		if (rc.IsUnityNull() || !entity.IsValid())
		{
			state = CameraRendererState.Invalid;
			return;
		}
		Transform eyes = rc.GetEyes();
		if ((Object)(object)eyes == (Object)null)
		{
			state = CameraRendererState.Invalid;
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
	}

	public void CompleteRender()
	{
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
		if ((Object)(object)instance == (Object)null)
		{
			state = CameraRendererState.Invalid;
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
		}
		else
		{
			if (((CustomYieldInstruction)_task).keepWaiting)
			{
				return;
			}
			if (rc.IsUnityNull() || !entity.IsValid())
			{
				instance.ReturnTask(ref _task);
				state = CameraRendererState.Invalid;
				return;
			}
			Transform eyes = rc.GetEyes();
			if ((Object)(object)eyes == (Object)null)
			{
				instance.ReturnTask(ref _task);
				state = CameraRendererState.Invalid;
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
			if (num2 == 0L)
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
			uint iD = entity.net.ID;
			AppBroadcast val = Pool.Get<AppBroadcast>();
			val.cameraRays = Pool.Get<AppCameraRays>();
			val.cameraRays.verticalFov = _fieldOfView;
			val.cameraRays.sampleOffset = _sampleOffset;
			val.cameraRays.rayData = new ArraySegment<byte>(array, 0, count);
			val.cameraRays.distance = distance;
			val.cameraRays.entities = Pool.GetList<Entity>();
			foreach (BaseEntity value in _colliderToEntity.Values)
			{
				if (value.IsValid())
				{
					Vector3 position2 = ((Component)value).transform.position;
					if (!(Vector3.Distance(position2, position) > (float)entityMaxDistance))
					{
						Entity val2 = Pool.Get<Entity>();
						val2.entityId = value.net.ID;
						val2.type = (EntityType)((value is TreeEntity) ? 1 : 2);
						val2.position = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint3x4(position2);
						Quaternion val3 = Quaternion.Inverse(((Component)value).transform.rotation) * rotation;
						val2.rotation = ((Quaternion)(ref val3)).eulerAngles * ((float)Math.PI / 180f);
						val2.size = Vector3.Scale(((Bounds)(ref value.bounds)).size, ((Component)value).transform.localScale);
						val2.name = ((value is BasePlayer basePlayer) ? basePlayer.displayName : null);
						val.cameraRays.entities.Add(val2);
					}
				}
			}
			Server.Broadcast(new CameraTarget(iD), val);
			_sampleOffset = _nextSampleOffset;
			if (!Server.HasAnySubscribers(new CameraTarget(iD)))
			{
				state = CameraRendererState.Invalid;
				return;
			}
			_lastRenderTimestamp = TimeEx.realtimeSinceStartup;
			state = CameraRendererState.WaitingToRender;
		}
	}

	private void UpdateCollidersMap(List<int> foundColliderIds)
	{
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
				item = GetMaterialIndex(collider.sharedMaterial, baseEntity);
				if (baseEntity is TreeEntity || baseEntity is BasePlayer)
				{
					_colliderToEntity[foundColliderId] = baseEntity;
				}
			}
			_knownColliders[foundColliderId] = (item, 0);
		}
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
