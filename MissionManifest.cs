using UnityEngine;
using UnityEngine.Profiling;

[CreateAssetMenu(menuName = "Rust/MissionManifest")]
public class MissionManifest : ScriptableObject
{
	public ScriptableObjectRef[] missionList;

	public WorldPositionGenerator[] positionGenerators;

	public static MissionManifest instance;

	public static MissionManifest Get()
	{
		Profiler.BeginSample("MissionManifestGet");
		if ((Object)(object)instance == (Object)null)
		{
			Profiler.BeginSample("ResourcesLoadManifest");
			instance = Resources.Load<MissionManifest>("MissionManifest");
			Profiler.EndSample();
			Profiler.BeginSample("PositionGeneratorLoop");
			WorldPositionGenerator[] array = instance.positionGenerators;
			foreach (WorldPositionGenerator worldPositionGenerator in array)
			{
				if ((Object)(object)worldPositionGenerator != (Object)null)
				{
					worldPositionGenerator.PrecalculatePositions();
				}
			}
			Profiler.EndSample();
		}
		Profiler.EndSample();
		return instance;
	}

	public static BaseMission GetFromShortName(string shortname)
	{
		MissionManifest missionManifest = Get();
		ScriptableObjectRef[] array = missionManifest.missionList;
		foreach (ScriptableObjectRef scriptableObjectRef in array)
		{
			BaseMission baseMission = scriptableObjectRef.Get() as BaseMission;
			if (baseMission.shortname == shortname)
			{
				return baseMission;
			}
		}
		return null;
	}

	public static BaseMission GetFromID(uint id)
	{
		MissionManifest missionManifest = Get();
		if (missionManifest.missionList == null)
		{
			return null;
		}
		ScriptableObjectRef[] array = missionManifest.missionList;
		foreach (ScriptableObjectRef scriptableObjectRef in array)
		{
			BaseMission baseMission = scriptableObjectRef.Get() as BaseMission;
			if (baseMission.id == id)
			{
				return baseMission;
			}
		}
		return null;
	}
}
