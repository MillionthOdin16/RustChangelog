using System;
using UnityEngine;

[RequireComponent(typeof(IOEntity))]
public class IOEntityMovementChecker : FacepunchBehaviour
{
	private IOEntity ioEntity;

	private Vector3 prevPos;

	private const float MAX_MOVE = 0.05f;

	private const float MAX_MOVE_SQR = 0.0025000002f;

	protected void Awake()
	{
		ioEntity = ((Component)this).GetComponent<IOEntity>();
	}

	protected void OnEnable()
	{
		((FacepunchBehaviour)this).InvokeRepeating((Action)CheckPosition, Random.Range(0f, 0.25f), 0.25f);
	}

	protected void OnDisable()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)CheckPosition);
	}

	private void CheckPosition()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (!ioEntity.isClient && Vector3.SqrMagnitude(((Component)this).transform.position - prevPos) > 0.0025000002f)
		{
			prevPos = ((Component)this).transform.position;
			if (ioEntity.HasConnections())
			{
				ioEntity.SendChangedToRoot(forceUpdate: true);
				ioEntity.ClearConnections();
			}
		}
	}
}
