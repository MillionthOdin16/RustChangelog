using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AtmosphereVolumeRenderer : MonoBehaviour
{
	public FogMode Mode = (FogMode)3;

	public bool DistanceFog = true;

	public bool HeightFog = true;

	public AtmosphereVolume Volume;

	private static bool isSupported => (int)Application.platform != 0 && (int)Application.platform != 1;
}
