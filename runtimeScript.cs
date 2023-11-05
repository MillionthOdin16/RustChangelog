using EasyRoads3Dv3;
using UnityEngine;

public class runtimeScript : MonoBehaviour
{
	public ERRoadNetwork roadNetwork;

	public ERRoad road;

	public GameObject go;

	public int currentElement = 0;

	public float distance = 0f;

	public float speed = 5f;

	private void Start()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		Debug.Log((object)"Please read the comments at the top of the runtime script (/Assets/EasyRoads3D/Scripts/runtimeScript) before using the runtime API!");
		roadNetwork = new ERRoadNetwork();
		ERRoadType val = new ERRoadType();
		val.roadWidth = 6f;
		ref Material roadMaterial = ref val.roadMaterial;
		Object obj = Resources.Load("Materials/roads/road material");
		roadMaterial = (Material)(object)((obj is Material) ? obj : null);
		val.layer = 1;
		val.tag = "Untagged";
		Vector3[] array = (Vector3[])(object)new Vector3[4]
		{
			new Vector3(200f, 5f, 200f),
			new Vector3(250f, 5f, 200f),
			new Vector3(250f, 5f, 250f),
			new Vector3(300f, 5f, 250f)
		};
		road = roadNetwork.CreateRoad("road 1", val, array);
		road.AddMarker(new Vector3(300f, 5f, 300f));
		road.InsertMarker(new Vector3(275f, 5f, 235f));
		road.DeleteMarker(2);
		roadNetwork.BuildRoadNetwork();
		go = GameObject.CreatePrimitive((PrimitiveType)3);
	}

	private void Update()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (roadNetwork != null)
		{
			float deltaTime = Time.deltaTime;
			float num = deltaTime * speed;
			distance += num;
			Vector3 position = road.GetPosition(distance, ref currentElement);
			position.y += 1f;
			go.transform.position = position;
			go.transform.forward = road.GetLookatSmooth(distance, currentElement);
		}
	}

	private void OnDestroy()
	{
		if (roadNetwork != null && roadNetwork.isInBuildMode)
		{
			roadNetwork.RestoreRoadNetwork();
			Debug.Log((object)"Restore Road Network");
		}
	}
}
