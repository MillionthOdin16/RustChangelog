using UnityEngine;

public class SubsurfaceProfile : ScriptableObject
{
	private static SubsurfaceProfileTexture profileTexture = new SubsurfaceProfileTexture();

	public SubsurfaceProfileData Data = SubsurfaceProfileData.Default;

	private int id = -1;

	public static Texture2D Texture => (profileTexture != null) ? profileTexture.Texture : null;

	public static Vector4[] TransmissionTints => (profileTexture != null) ? profileTexture.TransmissionTints : null;

	public int Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	private void OnEnable()
	{
		profileTexture.AddProfile(this);
	}
}
