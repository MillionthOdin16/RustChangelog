using UnityEngine;

public class AITraversalWaitPoint : MonoBehaviour
{
	public float nextFreeTime = 0f;

	public bool Occupied()
	{
		return Time.time > nextFreeTime;
	}

	public void Occupy(float dur = 1f)
	{
		nextFreeTime = Time.time + dur;
	}
}
