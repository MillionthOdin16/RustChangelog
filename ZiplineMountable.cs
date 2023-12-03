using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using UnityEngine;

public class ZiplineMountable : BaseMountable
{
	public float MoveSpeed = 4f;

	public float ForwardAdditive = 5f;

	public CapsuleCollider ZipCollider;

	public Transform ZiplineGrabRoot = null;

	public Transform LeftHandIkPoint;

	public Transform RightHandIkPoint;

	public float SpeedUpTime = 0.6f;

	public bool EditorHoldInPlace = false;

	private List<Vector3> linePoints = null;

	private const Flags PushForward = Flags.Reserved1;

	public AnimationCurve MountPositionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public AnimationCurve MountRotationCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float MountEaseInTime = 0.5f;

	private const Flags ShowHandle = Flags.Reserved2;

	private float additiveValue = 0f;

	private float currentTravelDistance = 0f;

	private TimeSince mountTime;

	private bool hasEnded = false;

	private List<Collider> ignoreColliders = new List<Collider>();

	private Vector3 lastSafePosition;

	private Vector3 startPosition = Vector3.zero;

	private Vector3 endPosition = Vector3.zero;

	private Quaternion startRotation = Quaternion.identity;

	private Quaternion endRotation = Quaternion.identity;

	private float elapsedMoveTime = 0f;

	private bool isAnimatingIn = false;

	private Vector3 ProcessBezierMovement(float distanceToTravel)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (linePoints == null)
		{
			return Vector3.zero;
		}
		float num = 0f;
		for (int i = 0; i < linePoints.Count - 1; i++)
		{
			float num2 = Vector3.Distance(linePoints[i], linePoints[i + 1]);
			if (num + num2 > distanceToTravel)
			{
				float num3 = Mathf.Clamp((distanceToTravel - num) / num2, 0f, 1f);
				return Vector3.Lerp(linePoints[i], linePoints[i + 1], num3);
			}
			num += num2;
		}
		return linePoints[linePoints.Count - 1];
	}

	private Vector3 GetLineEndPoint(bool applyDismountOffset = false)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (applyDismountOffset && linePoints != null)
		{
			Vector3 val = linePoints[linePoints.Count - 2] - linePoints[linePoints.Count - 1];
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			Vector3 val2 = linePoints[linePoints.Count - 1];
			return val2 + normalized * 1.5f;
		}
		return linePoints?[linePoints.Count - 1] ?? Vector3.zero;
	}

	private Vector3 GetNextLinePoint(Transform forTransform)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = forTransform.position;
		Vector3 forward = forTransform.forward;
		for (int i = 1; i < linePoints.Count - 1; i++)
		{
			Vector3 val = linePoints[i + 1] - position;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			val = linePoints[i - 1] - position;
			Vector3 normalized2 = ((Vector3)(ref val)).normalized;
			float num = Vector3.Dot(forward, normalized);
			float num2 = Vector3.Dot(forward, normalized2);
			if (num > 0f && num2 < 0f)
			{
				return linePoints[i + 1];
			}
		}
		return GetLineEndPoint();
	}

	public override void ResetState()
	{
		base.ResetState();
		additiveValue = 0f;
		currentTravelDistance = 0f;
		hasEnded = false;
		linePoints = null;
	}

	public override float MaxVelocity()
	{
		return MoveSpeed + ForwardAdditive;
	}

	public void SetDestination(List<Vector3> targetLinePoints, Vector3 lineStartPos, Quaternion lineStartRot)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		linePoints = targetLinePoints;
		currentTravelDistance = 0f;
		mountTime = TimeSince.op_Implicit(0f);
		Vector3 position = ((Component)this).transform.position;
		GamePhysics.OverlapSphere(position, 6f, ignoreColliders, 1218511105, (QueryTriggerInteraction)1);
		startPosition = ((Component)this).transform.position;
		startRotation = ((Component)this).transform.rotation;
		lastSafePosition = startPosition;
		endPosition = lineStartPos;
		endRotation = lineStartRot;
		elapsedMoveTime = 0f;
		isAnimatingIn = true;
		((FacepunchBehaviour)this).InvokeRepeating((Action)MovePlayerToPosition, 0f, 0f);
		Analytics.Server.UsedZipline();
	}

	private void Update()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		if (linePoints == null || base.isClient || isAnimatingIn || hasEnded)
		{
			return;
		}
		float num = (MoveSpeed + additiveValue * ForwardAdditive) * Mathf.Clamp(TimeSince.op_Implicit(mountTime) / SpeedUpTime, 0f, 1f);
		float num2 = num * Time.smoothDeltaTime;
		currentTravelDistance += num2;
		Vector3 val = ProcessBezierMovement(currentTravelDistance);
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		Vector3 position = Vector3Ex.WithY(val, val.y - ZipCollider.height * 0.6f);
		Vector3 position2 = val;
		GamePhysics.CapsuleSweep(position, position2, ZipCollider.radius, ((Component)this).transform.forward, num2, list, 1218511105, (QueryTriggerInteraction)1);
		foreach (RaycastHit item in list)
		{
			RaycastHit current = item;
			if ((Object)(object)((RaycastHit)(ref current)).collider == (Object)(object)ZipCollider || ignoreColliders.Contains(((RaycastHit)(ref current)).collider) || (Object)(object)((Component)((RaycastHit)(ref current)).collider).GetComponentInParent<PowerlineNode>() != (Object)null)
			{
				continue;
			}
			ZiplineMountable componentInParent = ((Component)((RaycastHit)(ref current)).collider).GetComponentInParent<ZiplineMountable>();
			if ((Object)(object)componentInParent != (Object)null)
			{
				componentInParent.EndZipline();
			}
			if (!GetDismountPosition(_mounted, out var _))
			{
				((Component)this).transform.position = lastSafePosition;
			}
			EndZipline();
			Pool.FreeList<RaycastHit>(ref list);
			return;
		}
		Pool.FreeList<RaycastHit>(ref list);
		if (Vector3.Distance(val, GetLineEndPoint()) < 0.1f)
		{
			((Component)this).transform.position = GetLineEndPoint(applyDismountOffset: true);
			hasEnded = true;
			return;
		}
		if (Vector3.Distance(lastSafePosition, ((Component)this).transform.position) > 0.75f)
		{
			lastSafePosition = ((Component)this).transform.position;
		}
		Vector3 val2 = val - Vector3Ex.WithY(((Component)this).transform.position, val.y);
		Vector3 normalized = ((Vector3)(ref val2)).normalized;
		((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, val, Time.deltaTime * 12f);
		((Component)this).transform.forward = normalized;
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		base.PlayerServerInput(inputState, player);
		if (linePoints != null)
		{
			if (hasEnded)
			{
				EndZipline();
				return;
			}
			Vector3 position = ((Component)this).transform.position;
			Vector3 nextLinePoint = GetNextLinePoint(((Component)this).transform);
			float num = ((nextLinePoint.y < position.y + 0.1f && inputState.IsDown(BUTTON.FORWARD)) ? 1f : 0f);
			additiveValue = Mathf.MoveTowards(additiveValue, num, (float)Server.tickrate * ((num > 0f) ? 4f : 2f));
			SetFlag(Flags.Reserved1, additiveValue > 0.5f);
		}
	}

	private void EndZipline()
	{
		DismountAllPlayers();
	}

	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		if (!base.IsDestroyed)
		{
			Kill();
		}
	}

	public override bool ValidDismountPosition(BasePlayer player, Vector3 disPos)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		((Collider)ZipCollider).enabled = false;
		bool result = base.ValidDismountPosition(player, disPos);
		((Collider)ZipCollider).enabled = true;
		return result;
	}

	public override void Save(SaveInfo info)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (linePoints == null)
		{
			return;
		}
		if (info.msg.ziplineMountable == null)
		{
			info.msg.ziplineMountable = Pool.Get<ZiplineMountable>();
		}
		info.msg.ziplineMountable.linePoints = Pool.GetList<VectorData>();
		foreach (Vector3 linePoint in linePoints)
		{
			info.msg.ziplineMountable.linePoints.Add(VectorData.op_Implicit(linePoint));
		}
	}

	private void MovePlayerToPosition()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		elapsedMoveTime += Time.deltaTime;
		float num = Mathf.Clamp(elapsedMoveTime / MountEaseInTime, 0f, 1f);
		Vector3 localPosition = Vector3.Lerp(startPosition, endPosition, MountPositionCurve.Evaluate(num));
		Quaternion localRotation = Quaternion.Lerp(startRotation, endRotation, MountRotationCurve.Evaluate(num));
		((Component)this).transform.localPosition = localPosition;
		((Component)this).transform.localRotation = localRotation;
		if (num >= 1f)
		{
			isAnimatingIn = false;
			SetFlag(Flags.Reserved2, b: true);
			mountTime = TimeSince.op_Implicit(0f);
			((FacepunchBehaviour)this).CancelInvoke((Action)MovePlayerToPosition);
		}
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && old.HasFlag(Flags.Busy) && !next.HasFlag(Flags.Busy) && !base.IsDestroyed)
		{
			Kill();
		}
	}
}
