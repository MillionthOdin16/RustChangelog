using System;
using System.IO;
using UnityEngine;

namespace ConVar;

[Factory("world")]
public class World : ConsoleSystem
{
	[ServerVar]
	[ClientVar]
	public static bool cache = true;

	[ClientVar]
	public static bool streaming = true;

	[ClientVar]
	[ServerVar]
	public static void monuments(Arg arg)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)TerrainMeta.Path))
		{
			return;
		}
		TextTable val = new TextTable();
		val.AddColumn("type");
		val.AddColumn("name");
		val.AddColumn("prefab");
		val.AddColumn("pos");
		foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
		{
			string[] obj = new string[4]
			{
				monument.Type.ToString(),
				monument.displayPhrase.translated,
				((Object)monument).name,
				null
			};
			Vector3 position = ((Component)monument).transform.position;
			obj[3] = ((object)(Vector3)(ref position)).ToString();
			val.AddRow(obj);
		}
		arg.ReplyWith(((object)val).ToString());
	}

	[ServerVar(Clientside = true, Help = "Renders a high resolution PNG of the current map")]
	public static void rendermap(Arg arg)
	{
		float @float = arg.GetFloat(0, 1f);
		int imageWidth;
		int imageHeight;
		Color background;
		byte[] array = MapImageRenderer.Render(out imageWidth, out imageHeight, out background, @float, lossy: false);
		if (array == null)
		{
			arg.ReplyWith("Failed to render the map (is a map loaded now?)");
			return;
		}
		string fullPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, $"map_{global::World.Size}_{global::World.Seed}.png"));
		File.WriteAllBytes(fullPath, array);
		arg.ReplyWith("Saved map render to: " + fullPath);
	}

	[ServerVar(Clientside = true, Help = "Renders a PNG of the current map's tunnel network")]
	public static void rendertunnels(Arg arg)
	{
		RenderMapLayerToFile(arg, "tunnels", MapLayer.TrainTunnels);
	}

	[ServerVar(Clientside = true, Help = "Renders a PNG of the current map's underwater labs, for a specific floor")]
	public static void renderlabs(Arg arg)
	{
		int underwaterLabFloorCount = MapLayerRenderer.GetOrCreate().GetUnderwaterLabFloorCount();
		int @int = arg.GetInt(0, 0);
		if (@int < 0 || @int >= underwaterLabFloorCount)
		{
			arg.ReplyWith($"Floor number must be between 0 and {underwaterLabFloorCount}");
		}
		else
		{
			RenderMapLayerToFile(arg, $"labs_{@int}", (MapLayer)(1 + @int));
		}
	}

	private static void RenderMapLayerToFile(Arg arg, string name, MapLayer layer)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			MapLayerRenderer orCreate = MapLayerRenderer.GetOrCreate();
			orCreate.Render(layer);
			string fullPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, $"{name}_{global::World.Size}_{global::World.Seed}.png"));
			RenderTexture targetTexture = orCreate.renderCamera.targetTexture;
			Texture2D val = new Texture2D(((Texture)targetTexture).width, ((Texture)targetTexture).height);
			RenderTexture active = RenderTexture.active;
			try
			{
				RenderTexture.active = targetTexture;
				val.ReadPixels(new Rect(0f, 0f, (float)((Texture)targetTexture).width, (float)((Texture)targetTexture).height), 0, 0);
				val.Apply();
				File.WriteAllBytes(fullPath, ImageConversion.EncodeToPNG(val));
			}
			finally
			{
				RenderTexture.active = active;
				Object.DestroyImmediate((Object)(object)val);
			}
			arg.ReplyWith("Saved " + name + " render to: " + fullPath);
		}
		catch (Exception ex)
		{
			Debug.LogWarning((object)ex);
			arg.ReplyWith("Failed to render " + name);
		}
	}
}
