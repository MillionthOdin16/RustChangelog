using UnityEngine;

public class PlayerDetectionTrigger : TriggerBase
{
	public GameObject detector;

	public IDetector _detector;

	private IDetector myDetector
	{
		get
		{
			if (_detector == null)
			{
				_detector = detector.GetComponent<IDetector>();
			}
			return _detector;
		}
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return ((Component)baseEntity).gameObject;
	}

	internal override void OnObjects()
	{
		base.OnObjects();
		myDetector.OnObjects();
	}

	internal override void OnObjectAdded(GameObject obj, Collider col)
	{
		base.OnObjectAdded(obj, col);
		myDetector.OnObjectAdded(obj, col);
	}

	internal override void OnEmpty()
	{
		base.OnEmpty();
		myDetector.OnEmpty();
	}
}
