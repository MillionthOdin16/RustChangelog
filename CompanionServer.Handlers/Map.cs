using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Profiling;

namespace CompanionServer.Handlers;

public class Map : BaseHandler<AppEmpty>
{
	private static int _width;

	private static int _height;

	private static byte[] _imageData;

	private static string _background;

	protected override double TokenCost => 5.0;

	public override void Execute()
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		if (_imageData == null)
		{
			SendError("no_map");
			return;
		}
		AppMap val = Pool.Get<AppMap>();
		val.width = (uint)_width;
		val.height = (uint)_height;
		val.oceanMargin = 500;
		val.jpgImage = _imageData;
		val.background = _background;
		val.monuments = Pool.GetList<Monument>();
		if ((Object)(object)TerrainMeta.Path != (Object)null && TerrainMeta.Path.Landmarks != null)
		{
			foreach (LandmarkInfo landmark in TerrainMeta.Path.Landmarks)
			{
				if (landmark.shouldDisplayOnMap)
				{
					Vector2 val2 = Util.WorldToMap(((Component)landmark).transform.position);
					Monument val3 = Pool.Get<Monument>();
					val3.token = (landmark.displayPhrase.IsValid() ? landmark.displayPhrase.token : ((Object)((Component)landmark).transform.root).name);
					val3.x = val2.x;
					val3.y = val2.y;
					val.monuments.Add(val3);
				}
			}
		}
		AppResponse val4 = Pool.Get<AppResponse>();
		val4.map = val;
		Send(val4);
	}

	public static void PopulateCache()
	{
		RenderToCache();
	}

	private static void RenderToCache()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Map.RenderToCache");
		_imageData = null;
		_width = 0;
		_height = 0;
		try
		{
			_imageData = MapImageRenderer.Render(out _width, out _height, out var background);
			_background = "#" + ColorUtility.ToHtmlStringRGB(background);
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"Exception thrown when rendering map for the app: {arg}");
		}
		if (_imageData == null)
		{
			Debug.LogError((object)"Map image is null! App users will not be able to see the map.");
		}
		Profiler.EndSample();
	}
}
