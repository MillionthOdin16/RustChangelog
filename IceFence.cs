using UnityEngine;

public class IceFence : GraveyardFence
{
	public GameObject[] styles;

	private bool init = false;

	public AdaptMeshToTerrain snowMesh;

	public int GetStyleFromID()
	{
		uint num = (uint)net.ID.Value;
		return SeedRandom.Range(ref num, 0, styles.Length);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		InitStyle();
		UpdatePillars();
	}

	public void InitStyle()
	{
		if (!init)
		{
			SetStyle(GetStyleFromID());
		}
	}

	public void SetStyle(int style)
	{
		GameObject[] array = styles;
		foreach (GameObject val in array)
		{
			val.gameObject.SetActive(false);
		}
		styles[style].gameObject.SetActive(true);
	}

	public override void UpdatePillars()
	{
		base.UpdatePillars();
	}
}
