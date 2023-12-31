using UnityEngine;

public class TriggerEnsnare : TriggerBase
{
	public bool blockHands = true;

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
		return ((Component)baseEntity).gameObject;
	}
}
