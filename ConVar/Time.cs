using UnityEngine;

namespace ConVar;

[Factory("time")]
public class Time : ConsoleSystem
{
	[ServerVar]
	[Help("Pause time while loading")]
	public static bool pausewhileloading = true;

	[ServerVar]
	[Help("Fixed delta time in seconds")]
	public static float fixeddelta
	{
		get
		{
			return Time.fixedDeltaTime;
		}
		set
		{
			Time.fixedDeltaTime = value;
		}
	}

	[ServerVar]
	[Help("The minimum amount of times to tick per frame")]
	public static float maxdelta
	{
		get
		{
			return Time.maximumDeltaTime;
		}
		set
		{
			Time.maximumDeltaTime = value;
		}
	}

	[ServerVar]
	[Help("The time scale")]
	public static float timescale
	{
		get
		{
			return Time.timeScale;
		}
		set
		{
			Time.timeScale = value;
		}
	}
}
