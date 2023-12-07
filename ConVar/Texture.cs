using System.Text;
using UnityEngine;

namespace ConVar;

[Factory("texture")]
public class Texture : ConsoleSystem
{
	[ClientVar]
	public static int streamingBudgetOverride;

	[ClientVar(Saved = true, Help = "Enable/Disable texture streaming")]
	public static bool streaming
	{
		get
		{
			return QualitySettings.streamingMipmapsActive;
		}
		set
		{
			QualitySettings.streamingMipmapsActive = value;
		}
	}

	[ClientVar]
	public static void stats(Arg arg)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"Supports streaming:               {SystemInfo.supportsMipStreaming}");
		stringBuilder.AppendLine($"Streaming enabled:                {QualitySettings.streamingMipmapsActive}");
		stringBuilder.AppendLine($"Immediately discard unused mips:  {Texture.streamingTextureDiscardUnusedMips}");
		stringBuilder.AppendLine($"Max level of reduction:           {QualitySettings.streamingMipmapsMaxLevelReduction}");
		stringBuilder.AppendLine();
		stringBuilder.AppendLine($"currentTextureMemory:             {Texture.currentTextureMemory / 1048576}MB (current estimated usage)");
		stringBuilder.AppendLine($"desiredTextureMemory:             {Texture.desiredTextureMemory / 1048576}MB");
		stringBuilder.AppendLine($"nonStreamingTextureCount:         {Texture.nonStreamingTextureCount}");
		stringBuilder.AppendLine($"nonStreamingTextureMemory:        {Texture.nonStreamingTextureMemory / 1048576}MB");
		stringBuilder.AppendLine($"streamingTextureCount:            {Texture.streamingTextureCount}");
		stringBuilder.AppendLine($"targetTextureMemory:              {Texture.targetTextureMemory / 1048576}MB");
		stringBuilder.AppendLine($"totalTextureMemory:               {Texture.totalTextureMemory / 1048576}MB (if everything was loaded at highest quality)");
		stringBuilder.AppendLine();
		stringBuilder.AppendLine($"streamingMipmapUploadCount:       {Texture.streamingMipmapUploadCount}");
		stringBuilder.AppendLine($"streamingTextureLoadingCount:     {Texture.streamingTextureLoadingCount}");
		stringBuilder.AppendLine($"streamingTexturePendingLoadCount: {Texture.streamingTexturePendingLoadCount}");
		stringBuilder.AppendLine();
		stringBuilder.AppendLine($"TargetBudget:                     {QualitySettings.streamingMipmapsMemoryBudget}MB");
		arg.ReplyWith(stringBuilder.ToString());
	}
}
