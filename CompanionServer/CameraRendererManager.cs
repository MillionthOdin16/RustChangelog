using System;
using System.Collections.Generic;
using System.Diagnostics;
using CompanionServer.Cameras;
using Facepunch;
using Facepunch.Extend;
using UnityEngine;

namespace CompanionServer;

public class CameraRendererManager : SingletonComponent<CameraRendererManager>
{
	private readonly Stack<CameraRenderTask> _taskPool = new Stack<CameraRenderTask>();

	private int _tasksTaken;

	private int _tasksReturned;

	private int _tasksCreated;

	private readonly Stopwatch _stopwatch = new Stopwatch();

	private readonly List<CameraRenderer> _renderers = new List<CameraRenderer>();

	private int _renderIndex;

	private int _completeIndex;

	protected override void OnDestroy()
	{
		((SingletonComponent)this).OnDestroy();
		foreach (CameraRenderer renderer in _renderers)
		{
			renderer.Reset();
		}
		_renderers.Clear();
		CameraRenderTask.FreeCachedSamplePositions();
		while (_taskPool.Count > 0)
		{
			_taskPool.Pop().Dispose();
		}
	}

	public void StartRendering(IRemoteControllable rc)
	{
		if (rc == null || rc.IsUnityNull())
		{
			throw new ArgumentNullException("rc");
		}
		if (List.FindWith<CameraRenderer, IRemoteControllable>((IReadOnlyCollection<CameraRenderer>)_renderers, (Func<CameraRenderer, IRemoteControllable>)((CameraRenderer r) => r.rc), rc, (IEqualityComparer<IRemoteControllable>)null) == null)
		{
			CameraRenderer cameraRenderer = Pool.Get<CameraRenderer>();
			_renderers.Add(cameraRenderer);
			cameraRenderer.Init(rc);
		}
	}

	public void Tick()
	{
		if (CameraRenderer.enabled)
		{
			DispatchRenderers();
			CompleteRenderers();
			CleanupRenderers();
		}
	}

	public CameraRenderTask BorrowTask()
	{
		if (_taskPool.Count > 0)
		{
			_tasksTaken++;
			return _taskPool.Pop();
		}
		_tasksCreated++;
		return new CameraRenderTask();
	}

	public void ReturnTask(ref CameraRenderTask task)
	{
		if (task != null)
		{
			task.Reset();
			_tasksReturned++;
			_taskPool.Push(task);
			task = null;
		}
	}

	[ServerVar]
	public static void pool_stats(Arg arg)
	{
		CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
		if ((Object)(object)instance == (Object)null)
		{
			arg.ReplyWith("Camera renderer manager is null!");
			return;
		}
		arg.ReplyWith($"Active renderers: {instance._renderers.Count}\nTasks in pool: {instance._taskPool.Count}\nTasks taken: {instance._tasksTaken}\nTasks returned: {instance._tasksReturned}\nTasks created: {instance._tasksCreated}");
	}

	private void DispatchRenderers()
	{
		List<CameraRenderer> list = Pool.GetList<CameraRenderer>();
		int count = _renderers.Count;
		for (int i = 0; i < count; i++)
		{
			if (_renderIndex >= count)
			{
				_renderIndex = 0;
			}
			CameraRenderer cameraRenderer = _renderers[_renderIndex++];
			if (cameraRenderer.CanRender())
			{
				list.Add(cameraRenderer);
				if (list.Count >= CameraRenderer.maxRendersPerFrame)
				{
					break;
				}
			}
		}
		if (list.Count > 0)
		{
			int maxSampleCount = CameraRenderer.maxRaysPerFrame / list.Count;
			foreach (CameraRenderer item in list)
			{
				item.Render(maxSampleCount);
			}
		}
		Pool.FreeList<CameraRenderer>(ref list);
	}

	private void CompleteRenderers()
	{
		_stopwatch.Restart();
		int count = _renderers.Count;
		for (int i = 0; i < count; i++)
		{
			if (_completeIndex >= count)
			{
				_completeIndex = 0;
			}
			CameraRenderer cameraRenderer = _renderers[_completeIndex++];
			if (cameraRenderer.state == CameraRendererState.Rendering)
			{
				cameraRenderer.CompleteRender();
				if (_stopwatch.Elapsed.TotalMilliseconds >= (double)CameraRenderer.completionFrameBudgetMs)
				{
					break;
				}
			}
		}
	}

	private void CleanupRenderers()
	{
		List<CameraRenderer> list = Pool.GetList<CameraRenderer>();
		foreach (CameraRenderer renderer in _renderers)
		{
			if (renderer.state == CameraRendererState.Invalid)
			{
				list.Add(renderer);
			}
		}
		_renderers.RemoveAll((CameraRenderer r) => r.state == CameraRendererState.Invalid);
		foreach (CameraRenderer item in list)
		{
			CameraRenderer current2 = item;
			Pool.Free<CameraRenderer>(ref current2);
		}
		Pool.FreeList<CameraRenderer>(ref list);
	}
}
