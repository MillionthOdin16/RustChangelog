using UnityEngine;

public class MonumentNode : MonoBehaviour
{
	public string ResourceFolder = string.Empty;

	protected void Awake()
	{
		if (!((Object)(object)SingletonComponent<WorldSetup>.Instance == (Object)null))
		{
			if (SingletonComponent<WorldSetup>.Instance.MonumentNodes == null)
			{
				Debug.LogError((object)"WorldSetup.Instance.MonumentNodes is null.", (Object)(object)this);
			}
			else
			{
				SingletonComponent<WorldSetup>.Instance.MonumentNodes.Add(this);
			}
		}
	}

	public void Process(ref uint seed)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		if (World.Networked)
		{
			World.Spawn("Monument", "assets/bundled/prefabs/autospawn/" + ResourceFolder + "/");
			return;
		}
		Prefab<MonumentInfo>[] array = Prefab.Load<MonumentInfo>("assets/bundled/prefabs/autospawn/" + ResourceFolder, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true, useWorldConfig: true);
		if (array != null && array.Length != 0)
		{
			Prefab<MonumentInfo> random = array.GetRandom(ref seed);
			float height = TerrainMeta.HeightMap.GetHeight(((Component)this).transform.position);
			Vector3 pos = default(Vector3);
			((Vector3)(ref pos))._002Ector(((Component)this).transform.position.x, height, ((Component)this).transform.position.z);
			Quaternion rot = random.Object.transform.localRotation;
			Vector3 scale = random.Object.transform.localScale;
			random.ApplyDecorComponents(ref pos, ref rot, ref scale);
			World.AddPrefab("Monument", random, pos, rot, scale);
		}
	}
}
