using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : SingletonComponent<MusicManager>, IClientComponent
{
	[Serializable]
	public class ClipPlaybackData
	{
		public AudioSource source;

		public MusicTheme.PositionedClip positionedClip;

		public bool isActive = false;

		public bool fadingIn = false;

		public bool fadingOut = false;

		public double fadeStarted = 0.0;

		public bool needsSync = false;
	}

	public AudioMixerGroup mixerGroup;

	public List<MusicTheme> themes;

	public MusicTheme currentTheme;

	public List<AudioSource> sources = new List<AudioSource>();

	public double nextMusic = 0.0;

	public double nextMusicFromIntensityRaise = 0.0;

	[Range(0f, 1f)]
	public float intensity = 0f;

	public Dictionary<MusicTheme.PositionedClip, ClipPlaybackData> clipPlaybackData = new Dictionary<MusicTheme.PositionedClip, ClipPlaybackData>();

	public int holdIntensityUntilBar = 0;

	public bool musicPlaying = false;

	public bool loadingFirstClips = false;

	public MusicTheme nextTheme = null;

	public double lastClipUpdate = 0.0;

	public float clipUpdateInterval = 0.1f;

	public double themeStartTime = 0.0;

	public int lastActiveClipRefresh = -10;

	public int activeClipRefreshInterval = 1;

	public bool forceThemeChange = false;

	public float randomIntensityJumpChance = 0f;

	public int clipScheduleBarsEarly = 1;

	public List<MusicTheme.PositionedClip> activeClips = new List<MusicTheme.PositionedClip>();

	public List<MusicTheme.PositionedClip> activeMusicClips = new List<MusicTheme.PositionedClip>();

	public List<MusicTheme.PositionedClip> activeControlClips = new List<MusicTheme.PositionedClip>();

	public List<MusicZone> currentMusicZones = new List<MusicZone>();

	public int currentBar = 0;

	public int barOffset = 0;

	public double currentThemeTime => AudioSettings.dspTime - themeStartTime;

	public int themeBar => currentBar + barOffset;

	public static void RaiseIntensityTo(float amount, int holdLengthBars = 0)
	{
	}

	public void StopMusic()
	{
	}
}
