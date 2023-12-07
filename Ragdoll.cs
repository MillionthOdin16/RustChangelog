using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Utility;
using Network;
using ProtoBuf;
using UnityEngine;

public class Ragdoll : EntityComponent<BaseEntity>, IPrefabPreProcess
{
	[Header("Ragdoll")]
	[Tooltip("If true, ragdoll physics are simulated on the server instead of the client")]
	public bool simOnServer;

	public float lerpToServerSimTime = 0.5f;

	public Transform eyeTransform;

	public Rigidbody primaryBody;

	[ReadOnly]
	public SpringJoint corpseJoint;

	[SerializeField]
	private PhysicMaterial physicMaterial;

	[SerializeField]
	private Skeleton skeleton;

	[SerializeField]
	private Model model;

	[ReadOnly]
	public List<Rigidbody> rigidbodies = new List<Rigidbody>();

	[ReadOnly]
	[SerializeField]
	private List<Transform> rbTransforms = new List<Transform>();

	[ReadOnly]
	[SerializeField]
	private List<Joint> joints = new List<Joint>();

	[ReadOnly]
	[SerializeField]
	private List<CharacterJoint> characterJoints = new List<CharacterJoint>();

	[ReadOnly]
	[SerializeField]
	private List<ConfigurableJoint> configurableJoints = new List<ConfigurableJoint>();

	[ReadOnly]
	[SerializeField]
	private List<Collider> colliders = new List<Collider>();

	[ReadOnly]
	[SerializeField]
	private int[] boneIndex;

	[ReadOnly]
	[SerializeField]
	private Vector3[] genericBonePos;

	[ReadOnly]
	[SerializeField]
	private Quaternion[] genericBoneRot;

	[SerializeField]
	private GameObject GibEffect;

	protected bool isSetUp;

	private const float MAX_JOINT_DIST = 2f;

	private bool wasSyncingJoints = true;

	protected bool IsClient => false;

	protected bool isServer => !IsClient;

	public bool IsInactive => rigidbodies[0].isKinematic;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Ragdoll.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	private void SetUpPhysics(bool isServer)
	{
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		if (isSetUp)
		{
			return;
		}
		isSetUp = true;
		if (isServer != simOnServer)
		{
			return;
		}
		foreach (Joint joint in joints)
		{
			joint.enablePreprocessing = false;
		}
		foreach (CharacterJoint characterJoint in characterJoints)
		{
			characterJoint.enableProjection = true;
		}
		foreach (ConfigurableJoint configurableJoint in configurableJoints)
		{
			configurableJoint.projectionMode = (JointProjectionMode)1;
		}
		SetInterpolationMode(((Component)this).transform.parent, isServer);
		foreach (Rigidbody rigidbody in rigidbodies)
		{
			SetCollisionMode(rigidbody);
			rigidbody.angularDrag = 1f;
			rigidbody.drag = 1f;
			rigidbody.detectCollisions = true;
			if (isServer)
			{
				rigidbody.solverIterations = 40;
			}
			else
			{
				rigidbody.solverIterations = 20;
			}
			rigidbody.solverVelocityIterations = 10;
			rigidbody.maxDepenetrationVelocity = 2f;
			rigidbody.sleepThreshold = Mathf.Max(0.05f, Physics.sleepThreshold);
			if (rigidbody.mass < 1f)
			{
				rigidbody.mass = 1f;
			}
			rigidbody.velocity = Random.onUnitSphere * 5f;
			rigidbody.angularVelocity = Random.onUnitSphere * 5f;
		}
	}

	public void ParentChanging(BaseCorpse corpse, Transform newParent)
	{
		SetInterpolationMode(newParent, corpse.isServer);
	}

	private void SetInterpolationMode(Transform parent, bool isServer)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (isServer != simOnServer)
		{
			return;
		}
		RigidbodyInterpolation interpolation = (simOnServer ? ((RigidbodyInterpolation)0) : (((Object)(object)parent == (Object)null) ? ((RigidbodyInterpolation)1) : ((!AnyParentMoves(parent)) ? ((RigidbodyInterpolation)1) : ((RigidbodyInterpolation)0))));
		foreach (Rigidbody rigidbody in rigidbodies)
		{
			rigidbody.interpolation = interpolation;
		}
	}

	private bool AnyParentMoves(Transform parent)
	{
		while ((Object)(object)parent != (Object)null)
		{
			BaseEntity component = ((Component)parent).GetComponent<BaseEntity>();
			if ((Object)(object)component != (Object)null && component.syncPosition)
			{
				return true;
			}
			parent = parent.parent;
		}
		return false;
	}

	private static void SetCollisionMode(Rigidbody rigidBody)
	{
		int ragdollmode = Physics.ragdollmode;
		if (ragdollmode <= 0)
		{
			rigidBody.collisionDetectionMode = (CollisionDetectionMode)0;
		}
		if (ragdollmode == 1)
		{
			rigidBody.collisionDetectionMode = (CollisionDetectionMode)1;
		}
		if (ragdollmode == 2)
		{
			rigidBody.collisionDetectionMode = (CollisionDetectionMode)2;
		}
		if (ragdollmode >= 3)
		{
			rigidBody.collisionDetectionMode = (CollisionDetectionMode)3;
		}
	}

	public void MoveRigidbodiesToRoot()
	{
		foreach (Transform rbTransform in rbTransforms)
		{
			rbTransform.SetParent(((Component)this).transform, true);
		}
	}

	public override void LoadComponent(BaseNetworkable.LoadInfo info)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (simOnServer && info.msg.ragdoll != null && isServer)
		{
			for (int i = 0; i < rbTransforms.Count; i++)
			{
				rbTransforms[i].localPosition = Compression.UnpackVector3FromInt(info.msg.ragdoll.positions[i], -2f, 2f);
				rbTransforms[i].localEulerAngles = Compression.UnpackVector3FromInt(info.msg.ragdoll.rotations[i], -360f, 360f);
			}
		}
	}

	public void GetCurrentBoneState(GameObject[] bones, ref Vector3[] bonePos, ref Quaternion[] boneRot)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		int num = bones.Length;
		bonePos = (Vector3[])(object)new Vector3[num];
		boneRot = (Quaternion[])(object)new Quaternion[num];
		for (int i = 0; i < num; i++)
		{
			if ((Object)(object)bones[i] != (Object)null)
			{
				Transform transform = bones[i].transform;
				bonePos[i] = transform.localPosition;
				boneRot[i] = transform.localRotation;
			}
		}
	}

	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		joints.Clear();
		rbTransforms.Clear();
		characterJoints.Clear();
		configurableJoints.Clear();
		rigidbodies.Clear();
		colliders.Clear();
		((Component)this).GetComponentsInChildren<Rigidbody>(true, rigidbodies);
		for (int i = 0; i < rigidbodies.Count; i++)
		{
			if (!((Object)(object)((Component)rigidbodies[i]).transform == (Object)(object)((Component)this).transform))
			{
				rbTransforms.Add(((Component)rigidbodies[i]).transform);
			}
		}
		((Component)this).GetComponentsInChildren<Joint>(true, joints);
		((Component)this).GetComponentsInChildren<CharacterJoint>(true, characterJoints);
		((Component)this).GetComponentsInChildren<ConfigurableJoint>(true, configurableJoints);
		((Component)this).GetComponentsInChildren<Collider>(true, colliders);
		rbTransforms.Sort((Transform t1, Transform t2) => TransformEx.GetDepth(t1).CompareTo(TransformEx.GetDepth(t2)));
		if (skeleton.Bones != null && skeleton.Bones.Length != 0)
		{
			GetCurrentBoneState(skeleton.Bones, ref genericBonePos, ref genericBoneRot);
			int num = skeleton.Bones.Length;
			boneIndex = new int[num];
			for (int j = 0; j < num; j++)
			{
				boneIndex[j] = -1;
				GameObject val = skeleton.Bones[j];
				for (int k = 0; k < rbTransforms.Count; k++)
				{
					if ((Object)(object)((Component)rbTransforms[k]).gameObject == (Object)(object)val)
					{
						boneIndex[j] = k;
						break;
					}
				}
			}
		}
		if (!clientside || !simOnServer)
		{
			return;
		}
		foreach (Joint joint in joints)
		{
			Object.DestroyImmediate((Object)(object)joint, true);
		}
		foreach (Rigidbody rigidbody in rigidbodies)
		{
			Object.DestroyImmediate((Object)(object)rigidbody, true);
		}
	}

	private void RemoveRootBoneOffset()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (simOnServer)
		{
			Transform rootBone = model.rootBone;
			if ((Object)(object)rootBone != (Object)null && !((Component)(object)rootBone).HasComponent<Rigidbody>())
			{
				((Component)this).transform.position = rootBone.position;
				((Component)this).transform.rotation = rootBone.rotation;
				rootBone.localPosition = Vector3.zero;
				rootBone.localRotation = Quaternion.identity;
			}
		}
	}

	public virtual void ServerInit()
	{
		if (simOnServer)
		{
			RemoveRootBoneOffset();
			((FacepunchBehaviour)this).InvokeRepeating((Action)SyncJointsToClients, 0f, 0.1f);
		}
		else
		{
			MoveRigidbodiesToRoot();
		}
		SetUpPhysics(isServer: true);
	}

	public override void SaveComponent(BaseNetworkable.SaveInfo info)
	{
		if (simOnServer)
		{
			info.msg.ragdoll = Pool.Get<Ragdoll>();
			SetRagdollMessageVals(info.msg.ragdoll);
		}
	}

	private void SyncJointsToClients()
	{
		if (!ShouldSyncJoints())
		{
			return;
		}
		Ragdoll val = Pool.Get<Ragdoll>();
		try
		{
			SetRagdollMessageVals(val);
			base.baseEntity.ClientRPC<Ragdoll>(null, "RPCSyncJoints", val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private bool ShouldSyncJoints()
	{
		bool result = false;
		if (wasSyncingJoints)
		{
			foreach (Rigidbody rigidbody in rigidbodies)
			{
				if (!rigidbody.IsSleeping())
				{
					result = true;
					break;
				}
			}
		}
		else
		{
			result = !primaryBody.IsSleeping();
		}
		wasSyncingJoints = result;
		return result;
	}

	private void SetRagdollMessageVals(Ragdoll ragdollMsg)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		List<int> list = Pool.GetList<int>();
		List<int> list2 = Pool.GetList<int>();
		foreach (Transform rbTransform in rbTransforms)
		{
			int item = Compression.PackVector3ToInt(rbTransform.localPosition, -2f, 2f);
			int item2 = Compression.PackVector3ToInt(rbTransform.localEulerAngles, -360f, 360f);
			list.Add(item);
			list2.Add(item2);
		}
		ragdollMsg.time = base.baseEntity.GetNetworkTime();
		ragdollMsg.positions = list;
		ragdollMsg.rotations = list2;
	}

	public void BecomeActive()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		foreach (Rigidbody rigidbody in rigidbodies)
		{
			rigidbody.isKinematic = false;
			SetCollisionMode(rigidbody);
			rigidbody.WakeUp();
			if ((Object)(object)base.baseEntity != (Object)null && base.baseEntity.HasParent())
			{
				Rigidbody component = ((Component)base.baseEntity.GetParentEntity()).GetComponent<Rigidbody>();
				if ((Object)(object)component != (Object)null)
				{
					rigidbody.velocity = component.velocity;
					rigidbody.angularVelocity = component.angularVelocity;
				}
			}
			foreach (Collider collider in colliders)
			{
				((Component)collider).gameObject.layer = 9;
			}
		}
	}

	public void BecomeInactive()
	{
		foreach (Rigidbody rigidbody in rigidbodies)
		{
			rigidbody.collisionDetectionMode = (CollisionDetectionMode)0;
			rigidbody.isKinematic = true;
		}
		foreach (Collider collider in colliders)
		{
			((Component)collider).gameObject.layer = 19;
		}
	}
}
