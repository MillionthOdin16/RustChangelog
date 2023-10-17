using UnityEngine;

public class LaserDetector : BaseDetector
{
	public override void OnObjects()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		foreach (BaseEntity entityContent in myTrigger.entityContents)
		{
			if (entityContent.IsVisible(((Component)this).transform.position + ((Component)this).transform.forward * 0.1f, 4f))
			{
				base.OnObjects();
				break;
			}
		}
	}
}
