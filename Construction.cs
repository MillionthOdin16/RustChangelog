using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;
using UnityEngine.Serialization;

public class Construction : PrefabAttribute
{
	public class Grade
	{
		public BuildingGrade grade;

		public float maxHealth;

		public List<ItemAmount> costToBuild;

		public PhysicMaterial physicMaterial => grade.physicMaterial;

		public ProtectionProperties damageProtecton => grade.damageProtecton;
	}

	public struct Target
	{
		public bool valid;

		public Ray ray;

		public BaseEntity entity;

		public Socket_Base socket;

		public bool onTerrain;

		public Vector3 position;

		public Vector3 normal;

		public Vector3 rotation;

		public BasePlayer player;

		public bool inBuildingPrivilege;

		public Quaternion GetWorldRotation(bool female)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = socket.rotation;
			if (socket.male && socket.female && female)
			{
				val = socket.rotation * Quaternion.Euler(180f, 0f, 180f);
			}
			return ((Component)entity).transform.rotation * val;
		}

		public Vector3 GetWorldPosition()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			Matrix4x4 localToWorldMatrix = ((Component)entity).transform.localToWorldMatrix;
			return ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(socket.position);
		}
	}

	public class Placement
	{
		public Vector3 position;

		public Quaternion rotation;
	}

	public BaseEntity.Menu.Option info;

	public bool canBypassBuildingPermission;

	[FormerlySerializedAs("canRotate")]
	public bool canRotateBeforePlacement;

	[FormerlySerializedAs("canRotate")]
	public bool canRotateAfterPlacement;

	public bool checkVolumeOnRotate;

	public bool checkVolumeOnUpgrade;

	public bool canPlaceAtMaxDistance;

	public bool placeOnWater;

	public Vector3 rotationAmount = new Vector3(0f, 90f, 0f);

	public Vector3 applyStartingRotation = Vector3.zero;

	public Transform deployOffset;

	public bool enforceLineOfSightCheckAgainstParentEntity;

	[Range(0f, 10f)]
	public float healthMultiplier = 1f;

	[Range(0f, 10f)]
	public float costMultiplier = 1f;

	[Range(1f, 50f)]
	public float maxplaceDistance = 4f;

	public Mesh guideMesh;

	[NonSerialized]
	public Socket_Base[] allSockets;

	[NonSerialized]
	public BuildingProximity[] allProximities;

	[NonSerialized]
	public ConstructionGrade defaultGrade;

	[NonSerialized]
	public SocketHandle socketHandle;

	[NonSerialized]
	public Bounds bounds;

	[NonSerialized]
	public bool isBuildingPrivilege;

	[NonSerialized]
	public bool isSleepingBag;

	[NonSerialized]
	public ConstructionGrade[] grades;

	[NonSerialized]
	public Deployable deployable;

	[NonSerialized]
	public ConstructionPlaceholder placeholder;

	public static string lastPlacementError;

	public BaseEntity CreateConstruction(Target target, bool bNeedsValidPlacement = false)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = GameManager.server.CreatePrefab(fullName, Vector3.zero, Quaternion.identity, active: false);
		bool flag = UpdatePlacement(val.transform, this, ref target);
		BaseEntity baseEntity = val.ToBaseEntity();
		if (bNeedsValidPlacement && !flag)
		{
			if (baseEntity.IsValid())
			{
				baseEntity.Kill();
			}
			else
			{
				GameManager.Destroy(val);
			}
			return null;
		}
		DecayEntity decayEntity = baseEntity as DecayEntity;
		if (Object.op_Implicit((Object)(object)decayEntity))
		{
			decayEntity.AttachToBuilding(target.entity as DecayEntity);
		}
		return baseEntity;
	}

	public bool HasMaleSockets(Target target)
	{
		Socket_Base[] array = allSockets;
		foreach (Socket_Base socket_Base in array)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				return true;
			}
		}
		return false;
	}

	public void FindMaleSockets(Target target, List<Socket_Base> sockets)
	{
		Socket_Base[] array = allSockets;
		foreach (Socket_Base socket_Base in array)
		{
			if (socket_Base.male && !socket_Base.maleDummy && socket_Base.TestTarget(target))
			{
				sockets.Add(socket_Base);
			}
		}
	}

	public ConstructionGrade GetGrade(BuildingGrade.Enum iGrade, ulong iSkin)
	{
		ConstructionGrade[] array = grades;
		foreach (ConstructionGrade constructionGrade in array)
		{
			if (constructionGrade.gradeBase.type == iGrade && constructionGrade.gradeBase.skin == iSkin)
			{
				return constructionGrade;
			}
		}
		return defaultGrade;
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		isBuildingPrivilege = Object.op_Implicit((Object)(object)rootObj.GetComponent<BuildingPrivlidge>());
		isSleepingBag = Object.op_Implicit((Object)(object)rootObj.GetComponent<SleepingBag>());
		bounds = rootObj.GetComponent<BaseEntity>().bounds;
		deployable = ((Component)this).GetComponent<Deployable>();
		placeholder = ((Component)this).GetComponentInChildren<ConstructionPlaceholder>();
		allSockets = ((Component)this).GetComponentsInChildren<Socket_Base>(true);
		allProximities = ((Component)this).GetComponentsInChildren<BuildingProximity>(true);
		socketHandle = ((Component)this).GetComponentsInChildren<SocketHandle>(true).FirstOrDefault();
		grades = rootObj.GetComponents<ConstructionGrade>();
		ConstructionGrade[] array = grades;
		foreach (ConstructionGrade constructionGrade in array)
		{
			if (!(constructionGrade == null))
			{
				constructionGrade.construction = this;
				if (!(defaultGrade != null))
				{
					defaultGrade = constructionGrade;
				}
			}
		}
	}

	protected override Type GetIndexedType()
	{
		return typeof(Construction);
	}

	public bool UpdatePlacement(Transform transform, Construction common, ref Target target)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		if (!target.valid)
		{
			return false;
		}
		if (!common.canBypassBuildingPermission && !target.player.CanBuild())
		{
			lastPlacementError = "You don't have permission to build here";
			return false;
		}
		List<Socket_Base> list = Pool.GetList<Socket_Base>();
		common.FindMaleSockets(target, list);
		foreach (Socket_Base item in list)
		{
			Placement placement = null;
			if ((Object)(object)target.entity != (Object)null && target.socket != null && target.entity.IsOccupied(target.socket))
			{
				continue;
			}
			if (placement == null)
			{
				placement = item.DoPlacement(target);
			}
			if ((Object)(object)target.player != (Object)null && target.player.IsInTutorial)
			{
				TutorialIsland currentTutorialIsland = target.player.GetCurrentTutorialIsland();
				if ((Object)(object)currentTutorialIsland != (Object)null && !currentTutorialIsland.CheckPlacement(common, target, placement))
				{
					placement = null;
				}
			}
			if (placement == null)
			{
				continue;
			}
			if (!item.CheckSocketMods(placement))
			{
				transform.position = placement.position;
				transform.rotation = placement.rotation;
				continue;
			}
			if (!TestPlacingThroughRock(ref placement, target))
			{
				transform.position = placement.position;
				transform.rotation = placement.rotation;
				lastPlacementError = "Placing through rock";
				continue;
			}
			if (!TestPlacingThroughWall(ref placement, transform, common, target))
			{
				transform.position = placement.position;
				transform.rotation = placement.rotation;
				lastPlacementError = "Placing through wall";
				continue;
			}
			if (!TestPlacingCloseToRoad(ref placement, target))
			{
				transform.position = placement.position;
				transform.rotation = placement.rotation;
				lastPlacementError = "Placing too close to road";
				continue;
			}
			if (Vector3.Distance(placement.position, target.player.eyes.position) > common.maxplaceDistance + 1f)
			{
				transform.position = placement.position;
				transform.rotation = placement.rotation;
				lastPlacementError = "Too far away";
				continue;
			}
			DeployVolume[] volumes = PrefabAttribute.server.FindAll<DeployVolume>(prefabID);
			if (DeployVolume.Check(placement.position, placement.rotation, volumes))
			{
				transform.position = placement.position;
				transform.rotation = placement.rotation;
				lastPlacementError = "Not enough space";
				continue;
			}
			if (BuildingProximity.Check(target.player, this, placement.position, placement.rotation))
			{
				transform.position = placement.position;
				transform.rotation = placement.rotation;
				continue;
			}
			if (common.isBuildingPrivilege && !target.player.CanPlaceBuildingPrivilege(placement.position, placement.rotation, common.bounds))
			{
				transform.position = placement.position;
				transform.rotation = placement.rotation;
				lastPlacementError = "Cannot stack building privileges";
				continue;
			}
			bool flag = target.player.IsBuildingBlocked(placement.position, placement.rotation, common.bounds);
			if (!common.canBypassBuildingPermission && flag)
			{
				transform.position = placement.position;
				transform.rotation = placement.rotation;
				lastPlacementError = "You don't have permission to build here";
				continue;
			}
			target.inBuildingPrivilege = flag;
			transform.SetPositionAndRotation(placement.position, placement.rotation);
			Pool.FreeList<Socket_Base>(ref list);
			return true;
		}
		Pool.FreeList<Socket_Base>(ref list);
		return false;
	}

	private bool TestPlacingThroughRock(ref Placement placement, Target target)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(placement.position, Vector3.one, placement.rotation, bounds);
		Vector3 center = target.player.GetCenter(ducked: true);
		Vector3 origin = ((Ray)(ref target.ray)).origin;
		if (Physics.Linecast(center, origin, 65536, (QueryTriggerInteraction)1))
		{
			return false;
		}
		RaycastHit val2 = default(RaycastHit);
		Vector3 val3 = (((OBB)(ref val)).Trace(target.ray, ref val2, float.PositiveInfinity) ? ((RaycastHit)(ref val2)).point : ((OBB)(ref val)).ClosestPoint(origin));
		if (Physics.Linecast(origin, val3, 65536, (QueryTriggerInteraction)1))
		{
			return false;
		}
		return true;
	}

	private static bool TestPlacingThroughWall(ref Placement placement, Transform transform, Construction common, Target target)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = placement.position;
		if ((Object)(object)common.deployOffset != (Object)null)
		{
			val += placement.rotation * common.deployOffset.localPosition;
		}
		Vector3 val2 = val - ((Ray)(ref target.ray)).origin;
		RaycastHit hit = default(RaycastHit);
		if (!Physics.Raycast(((Ray)(ref target.ray)).origin, ((Vector3)(ref val2)).normalized, ref hit, ((Vector3)(ref val2)).magnitude, 2097152))
		{
			return true;
		}
		if (!common.enforceLineOfSightCheckAgainstParentEntity)
		{
			StabilityEntity stabilityEntity = hit.GetEntity() as StabilityEntity;
			if ((Object)(object)stabilityEntity != (Object)null && (Object)(object)target.entity == (Object)(object)stabilityEntity)
			{
				return true;
			}
		}
		if (((Vector3)(ref val2)).magnitude - ((RaycastHit)(ref hit)).distance < 0.2f)
		{
			return true;
		}
		lastPlacementError = "object in placement path";
		transform.SetPositionAndRotation(((RaycastHit)(ref hit)).point, placement.rotation);
		return false;
	}

	private bool TestPlacingCloseToRoad(ref Placement placement, Target target)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		if ((Object)(object)heightMap == (Object)null)
		{
			return true;
		}
		if ((Object)(object)topologyMap == (Object)null)
		{
			return true;
		}
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(placement.position, Vector3.one, placement.rotation, bounds);
		float num = Mathf.Abs(heightMap.GetHeight(val.position) - val.position.y);
		if (num > 9f)
		{
			return true;
		}
		float radius = Mathf.Lerp(3f, 0f, num / 9f);
		Vector3 position = val.position;
		Vector3 point = ((OBB)(ref val)).GetPoint(-1f, 0f, -1f);
		Vector3 point2 = ((OBB)(ref val)).GetPoint(-1f, 0f, 1f);
		Vector3 point3 = ((OBB)(ref val)).GetPoint(1f, 0f, -1f);
		Vector3 point4 = ((OBB)(ref val)).GetPoint(1f, 0f, 1f);
		int topology = topologyMap.GetTopology(position, radius);
		int topology2 = topologyMap.GetTopology(point, radius);
		int topology3 = topologyMap.GetTopology(point2, radius);
		int topology4 = topologyMap.GetTopology(point3, radius);
		int topology5 = topologyMap.GetTopology(point4, radius);
		if (((topology | topology2 | topology3 | topology4 | topology5) & 0x80800) == 0)
		{
			return true;
		}
		return false;
	}

	public virtual bool ShowAsNeutral(Target target)
	{
		return target.inBuildingPrivilege;
	}
}
