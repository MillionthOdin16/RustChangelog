using UnityEngine;
using UnityEngine.Profiling;

public class FollowCamera : MonoBehaviour, IClientComponent
{
	private void LateUpdate()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)MainCamera.mainCamera == (Object)null))
		{
			Profiler.BeginSample("FollowCamera");
			((Component)this).transform.position = MainCamera.position;
			Profiler.EndSample();
		}
	}
}
