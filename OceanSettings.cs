using System.IO;
using Rust.Water5;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ocean Settings", menuName = "Water5/Ocean Settings")]
public class OceanSettings : ScriptableObject
{
	[Header("Compute Shaders")]
	public ComputeShader waveSpectrumCompute;

	public ComputeShader fftCompute;

	public ComputeShader waveMergeCompute;

	public ComputeShader waveInitialSpectrum;

	[Header("Global Ocean Params")]
	public float[] octaveScales;

	public float lamda;

	public float windDirection;

	public float distanceAttenuationFactor;

	public float depthAttenuationFactor;

	[Header("Ocean Spectra")]
	public OceanSpectrumSettings[] spectrumSettings;

	[HideInInspector]
	public float[] spectrumRanges;

	public unsafe OceanDisplacementShort3[,,] LoadSimData()
	{
		OceanDisplacementShort3[,,] array = new OceanDisplacementShort3[spectrumSettings.Length, 72, 65536];
		string path = Application.streamingAssetsPath + "/" + ((Object)this).name + ".physicsdata.dat";
		if (!File.Exists(path))
		{
			Debug.Log((object)"Simulation Data not found");
			return array;
		}
		byte[] array2 = File.ReadAllBytes(path);
		fixed (byte* ptr2 = array2)
		{
			fixed (OceanDisplacementShort3* ptr = array)
			{
				UnsafeUtility.MemCpy((void*)ptr, (void*)ptr2, (long)array2.Length);
			}
		}
		return array;
	}
}
