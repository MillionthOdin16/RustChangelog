using UnityEngine;

public class ExplosionsBillboard : MonoBehaviour
{
	public Camera Camera;

	public bool Active = true;

	public bool AutoInitCamera = true;

	private GameObject myContainer;

	private Transform t;

	private Transform camT;

	private Transform contT;

	private void Awake()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if (AutoInitCamera)
		{
			Camera = Camera.main;
			Active = true;
		}
		t = ((Component)this).transform;
		Vector3 localScale = ((Component)t.parent).transform.localScale;
		localScale.z = localScale.x;
		((Component)t.parent).transform.localScale = localScale;
		camT = ((Component)Camera).transform;
		Transform parent = t.parent;
		myContainer = new GameObject
		{
			name = "Billboard_" + ((Object)((Component)t).gameObject).name
		};
		contT = myContainer.transform;
		contT.position = t.position;
		t.parent = myContainer.transform;
		contT.parent = parent;
	}

	private void Update()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (Active)
		{
			contT.LookAt(contT.position + camT.rotation * Vector3.back, camT.rotation * Vector3.up);
		}
	}
}
