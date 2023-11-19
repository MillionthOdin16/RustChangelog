public class DistanceFlareLOD : FacepunchBehaviour, ILOD, IClientComponent
{
	public bool isDynamic = false;

	public float minEnabledDistance = 100f;

	public float maxEnabledDistance = 600f;

	public bool toggleFade = false;

	public float toggleFadeDuration = 0.5f;
}
