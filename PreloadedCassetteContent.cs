using UnityEngine;

public class PreloadedCassetteContent : ScriptableObject
{
	public enum PreloadType
	{
		Short,
		Medium,
		Long
	}

	public SoundDefinition[] ShortTapeContent;

	public SoundDefinition[] MediumTapeContent;

	public SoundDefinition[] LongTapeContent;

	public SoundDefinition GetSoundContent(int index, PreloadType type)
	{
		return type switch
		{
			PreloadType.Short => GetDefinition(index, ShortTapeContent), 
			PreloadType.Medium => GetDefinition(index, MediumTapeContent), 
			PreloadType.Long => GetDefinition(index, LongTapeContent), 
			_ => null, 
		};
	}

	private SoundDefinition GetDefinition(int index, SoundDefinition[] array)
	{
		index = Mathf.Clamp(index, 0, array.Length);
		return array[index];
	}

	public uint GetSoundContentId(SoundDefinition def)
	{
		uint num = 0u;
		SoundDefinition[] shortTapeContent = ShortTapeContent;
		foreach (SoundDefinition soundDefinition in shortTapeContent)
		{
			if ((Object)(object)soundDefinition == (Object)(object)def)
			{
				return num;
			}
			num++;
		}
		SoundDefinition[] mediumTapeContent = MediumTapeContent;
		foreach (SoundDefinition soundDefinition2 in mediumTapeContent)
		{
			if ((Object)(object)soundDefinition2 == (Object)(object)def)
			{
				return num;
			}
			num++;
		}
		SoundDefinition[] longTapeContent = LongTapeContent;
		foreach (SoundDefinition soundDefinition3 in longTapeContent)
		{
			if ((Object)(object)soundDefinition3 == (Object)(object)def)
			{
				return num;
			}
			num++;
		}
		return num;
	}

	public SoundDefinition GetSoundContent(uint id)
	{
		int num = 0;
		SoundDefinition[] shortTapeContent = ShortTapeContent;
		foreach (SoundDefinition result in shortTapeContent)
		{
			if (num++ == id)
			{
				return result;
			}
		}
		SoundDefinition[] mediumTapeContent = MediumTapeContent;
		foreach (SoundDefinition result2 in mediumTapeContent)
		{
			if (num++ == id)
			{
				return result2;
			}
		}
		SoundDefinition[] longTapeContent = LongTapeContent;
		foreach (SoundDefinition result3 in longTapeContent)
		{
			if (num++ == id)
			{
				return result3;
			}
		}
		return null;
	}
}
