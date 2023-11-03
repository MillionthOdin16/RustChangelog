using System;
using System.Collections.Generic;
using UnityEngine;

namespace Instancing;

public class TextureAtlasScheduler
{
	private class TextureAtlas
	{
		public int Resolution;

		public Texture2DArray TextureArray;

		public List<TextureAtlasItem> Textures = new List<TextureAtlasItem>();
	}

	private class TextureAtlasItem
	{
		public Texture Texture;

		public bool Occupied;
	}

	public static readonly TextureAtlasScheduler Instanced = new TextureAtlasScheduler();

	private Dictionary<int, TextureAtlas> textureAtlases = new Dictionary<int, TextureAtlas>();

	private int AddTexture(TextureAtlas atlas, Texture texture)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		int num = atlas.Textures.FindIndex((TextureAtlasItem x) => !x.Occupied);
		if (num == -1)
		{
			atlas.Textures.Add(new TextureAtlasItem());
			num = atlas.Textures.Count - 1;
		}
		TextureAtlasItem textureAtlasItem = atlas.Textures[num];
		textureAtlasItem.Occupied = false;
		if (atlas.TextureArray.depth < atlas.Textures.Count)
		{
			Texture2DArray val = new Texture2DArray(atlas.Resolution, atlas.Resolution, atlas.TextureArray.depth * 2, atlas.TextureArray.format, false);
			Graphics.CopyTexture((Texture)(object)atlas.TextureArray, (Texture)(object)val);
			Object.Destroy((Object)(object)atlas.TextureArray);
			atlas.TextureArray = val;
		}
		textureAtlasItem.Texture = texture;
		textureAtlasItem.Occupied = true;
		return num;
	}

	private void UpdateTexture(TextureAtlas atlas, Texture texture, int index)
	{
		atlas.Textures[index].Texture = texture;
		Graphics.CopyTexture(texture, (Texture)(object)atlas.TextureArray);
	}

	public int AddTextureToAtlas(Texture texture)
	{
		TextureAtlas orCreateAtlas = GetOrCreateAtlas(texture.width, texture.height);
		return AddTexture(orCreateAtlas, texture);
	}

	public void ReplaceTextureInAtlas(Texture texture, int index)
	{
		GetOrCreateAtlas(texture.width, texture.height);
	}

	private TextureAtlas GetOrCreateAtlas(int width, int height)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		if (width != height)
		{
			throw new NotSupportedException("Textures must be the same width and height");
		}
		if (!textureAtlases.TryGetValue(width, out var value))
		{
			value = new TextureAtlas
			{
				Resolution = width,
				TextureArray = new Texture2DArray(width, height, 8, (TextureFormat)5, false)
			};
			textureAtlases[width] = value;
		}
		return value;
	}

	private int GetResolutionKey(int xSize, int ySize)
	{
		return xSize * 10000 + ySize;
	}
}
