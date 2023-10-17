using System;
using ConVar;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

public class GameManager
{
	public static GameManager server = new GameManager(clientside: false, serverside: true);

	internal PrefabPreProcess preProcessed;

	internal PrefabPoolCollection pool;

	private bool Clientside;

	private bool Serverside;

	public void Reset()
	{
		pool.Clear();
	}

	public GameManager(bool clientside, bool serverside)
	{
		Clientside = clientside;
		Serverside = serverside;
		preProcessed = new PrefabPreProcess(clientside, serverside);
		pool = new PrefabPoolCollection();
	}

	public GameObject FindPrefab(uint prefabID)
	{
		string text = StringPool.Get(prefabID);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return FindPrefab(text);
	}

	public GameObject FindPrefab(BaseEntity ent)
	{
		if ((Object)(object)ent == (Object)null)
		{
			return null;
		}
		return FindPrefab(ent.PrefabName);
	}

	public GameObject FindPrefab(string strPrefab)
	{
		Profiler.BeginSample("FindPrefab");
		Profiler.BeginSample("FindProcessed");
		GameObject val = preProcessed.Find(strPrefab);
		if ((Object)(object)val != (Object)null)
		{
			Profiler.EndSample();
			Profiler.EndSample();
			return val;
		}
		Profiler.EndSample();
		Profiler.BeginSample("LoadFromResources");
		val = FileSystem.LoadPrefab(strPrefab);
		if ((Object)(object)val == (Object)null)
		{
			Profiler.EndSample();
			Profiler.EndSample();
			return null;
		}
		Profiler.EndSample();
		Profiler.BeginSample("PrefabPreProcess.Process");
		preProcessed.Process(strPrefab, val);
		Profiler.EndSample();
		Profiler.EndSample();
		GameObject val2 = preProcessed.Find(strPrefab);
		return ((Object)(object)val2 != (Object)null) ? val2 : val;
	}

	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, Vector3 scale, bool active = true)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("GameManager.CreatePrefab");
		GameObject val = Instantiate(strPrefab, pos, rot);
		if (Object.op_Implicit((Object)(object)val))
		{
			val.transform.localScale = scale;
			if (active)
			{
				val.AwakeFromInstantiate();
			}
		}
		Profiler.EndSample();
		return val;
	}

	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, bool active = true)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("GameManager.CreatePrefab");
		GameObject val = Instantiate(strPrefab, pos, rot);
		if (Object.op_Implicit((Object)(object)val) && active)
		{
			val.AwakeFromInstantiate();
		}
		Profiler.EndSample();
		return val;
	}

	public GameObject CreatePrefab(string strPrefab, bool active = true)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("GameManager.CreatePrefab");
		GameObject val = Instantiate(strPrefab, Vector3.zero, Quaternion.identity);
		if (Object.op_Implicit((Object)(object)val) && active)
		{
			val.AwakeFromInstantiate();
		}
		Profiler.EndSample();
		return val;
	}

	public GameObject CreatePrefab(string strPrefab, Transform parent, bool active = true)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("GameManager.CreatePrefab");
		GameObject val = Instantiate(strPrefab, parent.position, parent.rotation);
		if (Object.op_Implicit((Object)(object)val))
		{
			val.transform.SetParent(parent, false);
			val.Identity();
			if (active)
			{
				val.AwakeFromInstantiate();
			}
		}
		Profiler.EndSample();
		return val;
	}

	public BaseEntity CreateEntity(string strPrefab, Vector3 pos = default(Vector3), Quaternion rot = default(Quaternion), bool startActive = true)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(strPrefab))
		{
			return null;
		}
		GameObject val = CreatePrefab(strPrefab, pos, rot, startActive);
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		Profiler.BeginSample("GetComponent");
		BaseEntity component = val.GetComponent<BaseEntity>();
		Profiler.EndSample();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogError((object)("CreateEntity called on a prefab that isn't an entity! " + strPrefab));
			Object.Destroy((Object)(object)val);
			return null;
		}
		if (((Component)component).CompareTag("CannotBeCreated"))
		{
			Debug.LogWarning((object)("CreateEntity called on a prefab that has the CannotBeCreated tag set. " + strPrefab));
			Object.Destroy((Object)(object)val);
			return null;
		}
		return component;
	}

	private GameObject Instantiate(string strPrefab, Vector3 pos, Quaternion rot)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample(strPrefab);
		Profiler.BeginSample("String.ToLower");
		if (!StringEx.IsLower(strPrefab))
		{
			Debug.LogWarning((object)("Converting prefab name to lowercase: " + strPrefab));
			strPrefab = strPrefab.ToLower();
		}
		Profiler.EndSample();
		GameObject val = FindPrefab(strPrefab);
		if (!Object.op_Implicit((Object)(object)val))
		{
			Debug.LogError((object)("Couldn't find prefab \"" + strPrefab + "\""));
			Profiler.EndSample();
			return null;
		}
		Profiler.BeginSample("PrefabPool.Pop");
		GameObject val2 = pool.Pop(StringPool.Get(strPrefab), pos, rot);
		Profiler.EndSample();
		if ((Object)(object)val2 == (Object)null)
		{
			Profiler.BeginSample("GameObject.Instantiate");
			val2 = Instantiate.GameObject(val, pos, rot);
			((Object)val2).name = strPrefab;
			Profiler.EndSample();
		}
		else
		{
			val2.transform.localScale = val.transform.localScale;
		}
		if (!Clientside && Serverside && (Object)(object)val2.transform.parent == (Object)null)
		{
			SceneManager.MoveGameObjectToScene(val2, Rust.Server.EntityScene);
		}
		Profiler.EndSample();
		return val2;
	}

	public static void Destroy(Component component, float delay = 0f)
	{
		BaseEntity ent = component as BaseEntity;
		if (ent.IsValid())
		{
			Debug.LogError((object)("Trying to destroy an entity without killing it first: " + ((Object)component).name));
		}
		Profiler.BeginSample("Component.Destroy");
		Object.Destroy((Object)(object)component, delay);
		Profiler.EndSample();
	}

	public static void Destroy(GameObject instance, float delay = 0f)
	{
		if (Object.op_Implicit((Object)(object)instance))
		{
			Profiler.BeginSample("GetComponent");
			BaseEntity component = instance.GetComponent<BaseEntity>();
			Profiler.EndSample();
			if (component.IsValid())
			{
				Debug.LogError((object)("Trying to destroy an entity without killing it first: " + ((Object)instance).name));
			}
			Profiler.BeginSample("GameObject.Destroy");
			Object.Destroy((Object)(object)instance, delay);
			Profiler.EndSample();
		}
	}

	public static void DestroyImmediate(Component component, bool allowDestroyingAssets = false)
	{
		BaseEntity ent = component as BaseEntity;
		if (ent.IsValid())
		{
			Debug.LogError((object)("Trying to destroy an entity without killing it first: " + ((Object)component).name));
		}
		Profiler.BeginSample("Component.DestroyImmediate");
		Object.DestroyImmediate((Object)(object)component, allowDestroyingAssets);
		Profiler.EndSample();
	}

	public static void DestroyImmediate(GameObject instance, bool allowDestroyingAssets = false)
	{
		Profiler.BeginSample("GetComponent");
		BaseEntity component = instance.GetComponent<BaseEntity>();
		Profiler.EndSample();
		if (component.IsValid())
		{
			Debug.LogError((object)("Trying to destroy an entity without killing it first: " + ((Object)instance).name));
		}
		Profiler.BeginSample("GameObject.DestroyImmediate");
		Object.DestroyImmediate((Object)(object)instance, allowDestroyingAssets);
		Profiler.EndSample();
	}

	public void Retire(GameObject instance)
	{
		if (!Object.op_Implicit((Object)(object)instance))
		{
			return;
		}
		TimeWarning val = TimeWarning.New("GameManager.Retire", 0);
		try
		{
			Profiler.BeginSample("GetComponent");
			BaseEntity component = instance.GetComponent<BaseEntity>();
			Profiler.EndSample();
			if (component.IsValid())
			{
				Debug.LogError((object)("Trying to retire an entity without killing it first: " + ((Object)instance).name));
			}
			if (!Application.isQuitting && Pool.enabled && instance.SupportsPooling())
			{
				Profiler.BeginSample("PrefabPool.Push");
				pool.Push(instance);
				Profiler.EndSample();
			}
			else
			{
				Profiler.BeginSample("GameObject.Destroy");
				Object.Destroy((Object)(object)instance);
				Profiler.EndSample();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
