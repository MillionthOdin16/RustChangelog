using UnityEngine;

public class LandmarkInfo : MonoBehaviour
{
	[Header("LandmarkInfo")]
	public bool shouldDisplayOnMap = false;

	public bool isLayerSpecific = false;

	public Phrase displayPhrase;

	public Sprite mapIcon;

	public virtual MapLayer MapLayer => MapLayer.Overworld;

	protected virtual void Awake()
	{
		if (Object.op_Implicit((Object)(object)TerrainMeta.Path))
		{
			TerrainMeta.Path.Landmarks.Add(this);
		}
	}
}
