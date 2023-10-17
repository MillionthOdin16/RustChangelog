using UnityEngine;

public class BaseMagnet : MonoBehaviour
{
	public BaseEntity entityOwner;

	public BaseEntity.Flags magnetFlag = BaseEntity.Flags.Reserved6;

	public TriggerMagnet magnetTrigger;

	public FixedJoint fixedJoint;

	public Rigidbody kinematicAttachmentBody;

	public float magnetForce;

	public Transform attachDepthPoint;

	public GameObjectRef attachEffect;

	public bool isMagnetOn;

	public GameObject colliderSource;

	private BasePlayer associatedPlayer;

	public bool HasConnectedObject()
	{
		if ((Object)(object)((Joint)fixedJoint).connectedBody != (Object)null)
		{
			return isMagnetOn;
		}
		return false;
	}

	public OBB GetConnectedOBB(float scale = 1f)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)((Joint)fixedJoint).connectedBody == (Object)null)
		{
			Debug.LogError((object)"BaseMagnet returning fake OBB because no connected body!");
			return new OBB(Vector3.zero, Vector3.one, Quaternion.identity);
		}
		BaseEntity component = ((Component)((Joint)fixedJoint).connectedBody).gameObject.GetComponent<BaseEntity>();
		Bounds bounds = component.bounds;
		((Bounds)(ref bounds)).extents = ((Bounds)(ref bounds)).extents * scale;
		return new OBB(((Component)component).transform.position, ((Component)component).transform.rotation, bounds);
	}

	public void SetCollisionsEnabled(GameObject other, bool wants)
	{
		Collider[] componentsInChildren = other.GetComponentsInChildren<Collider>();
		Collider[] componentsInChildren2 = colliderSource.GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider val in array)
		{
			Collider[] array2 = componentsInChildren2;
			foreach (Collider val2 in array2)
			{
				Physics.IgnoreCollision(val, val2, !wants);
			}
		}
	}

	public virtual void SetMagnetEnabled(bool wantsOn, BasePlayer forPlayer)
	{
		if (isMagnetOn != wantsOn)
		{
			associatedPlayer = forPlayer;
			isMagnetOn = wantsOn;
			if (isMagnetOn)
			{
				OnMagnetEnabled();
			}
			else
			{
				OnMagnetDisabled();
			}
			if ((Object)(object)entityOwner != (Object)null)
			{
				entityOwner.SetFlag(magnetFlag, isMagnetOn);
			}
		}
	}

	public virtual void OnMagnetEnabled()
	{
	}

	public virtual void OnMagnetDisabled()
	{
		if (Object.op_Implicit((Object)(object)((Joint)fixedJoint).connectedBody))
		{
			SetCollisionsEnabled(((Component)((Joint)fixedJoint).connectedBody).gameObject, wants: true);
			Rigidbody connectedBody = ((Joint)fixedJoint).connectedBody;
			((Joint)fixedJoint).connectedBody = null;
			connectedBody.WakeUp();
		}
	}

	public bool IsMagnetOn()
	{
		return isMagnetOn;
	}

	public void MagnetThink(float delta)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		if (!isMagnetOn)
		{
			return;
		}
		Vector3 position = ((Component)magnetTrigger).transform.position;
		if (magnetTrigger.entityContents == null)
		{
			return;
		}
		OBB val = default(OBB);
		foreach (BaseEntity entityContent in magnetTrigger.entityContents)
		{
			if ((Object)(object)entityContent == (Object)null || !entityContent.syncPosition)
			{
				continue;
			}
			Rigidbody component = ((Component)entityContent).GetComponent<Rigidbody>();
			if ((Object)(object)component == (Object)null || component.isKinematic || entityContent.isClient)
			{
				continue;
			}
			((OBB)(ref val))._002Ector(((Component)entityContent).transform.position, ((Component)entityContent).transform.rotation, entityContent.bounds);
			if (((OBB)(ref val)).Contains(attachDepthPoint.position))
			{
				MagnetLiftable component2 = ((Component)entityContent).GetComponent<MagnetLiftable>();
				if ((Object)(object)component2 != (Object)null)
				{
					component2.SetMagnetized(wantsOn: true, this, associatedPlayer);
					if ((Object)(object)((Joint)fixedJoint).connectedBody == (Object)null)
					{
						Effect.server.Run(attachEffect.resourcePath, attachDepthPoint.position, -attachDepthPoint.up);
						((Joint)fixedJoint).connectedBody = component;
						SetCollisionsEnabled(((Component)component).gameObject, wants: false);
						continue;
					}
				}
			}
			if ((Object)(object)((Joint)fixedJoint).connectedBody == (Object)null)
			{
				Vector3 position2 = ((Component)entityContent).transform.position;
				float num = Vector3.Distance(position2, position);
				Vector3 val2 = Vector3Ex.Direction(position, position2);
				float num2 = 1f / Mathf.Max(1f, num);
				component.AddForce(val2 * magnetForce * num2, (ForceMode)5);
			}
		}
	}
}
