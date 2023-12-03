using UnityEngine;

public class RagdollEditor : SingletonComponent<RagdollEditor>
{
	private Vector3 view = default(Vector3);

	private Rigidbody grabbedRigid;

	private Vector3 grabPos;

	private Vector3 grabOffset;

	private void OnGUI()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		GUI.Box(new Rect((float)Screen.width * 0.5f - 2f, (float)Screen.height * 0.5f - 2f, 4f, 4f), "");
	}

	protected override void Awake()
	{
		((SingletonComponent)this).Awake();
	}

	private void Update()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		Camera.main.fieldOfView = 75f;
		if (Input.GetKey((KeyCode)324))
		{
			view.y += Input.GetAxisRaw("Mouse X") * 3f;
			view.x -= Input.GetAxisRaw("Mouse Y") * 3f;
			Cursor.lockState = (CursorLockMode)1;
			Cursor.visible = false;
		}
		else
		{
			Cursor.lockState = (CursorLockMode)0;
			Cursor.visible = true;
		}
		((Component)Camera.main).transform.rotation = Quaternion.Euler(view);
		Vector3 val = Vector3.zero;
		if (Input.GetKey((KeyCode)119))
		{
			val += Vector3.forward;
		}
		if (Input.GetKey((KeyCode)115))
		{
			val += Vector3.back;
		}
		if (Input.GetKey((KeyCode)97))
		{
			val += Vector3.left;
		}
		if (Input.GetKey((KeyCode)100))
		{
			val += Vector3.right;
		}
		Transform transform = ((Component)Camera.main).transform;
		transform.position += ((Component)this).transform.rotation * val * 0.05f;
		if (Input.GetKeyDown((KeyCode)323))
		{
			StartGrab();
		}
		if (Input.GetKeyUp((KeyCode)323))
		{
			StopGrab();
		}
	}

	private void FixedUpdate()
	{
		if (Input.GetKey((KeyCode)323))
		{
			UpdateGrab();
		}
	}

	private void StartGrab()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(((Component)this).transform.position, ((Component)this).transform.forward, ref val, 100f))
		{
			grabbedRigid = ((Component)((RaycastHit)(ref val)).collider).GetComponent<Rigidbody>();
			if (!((Object)(object)grabbedRigid == (Object)null))
			{
				Matrix4x4 worldToLocalMatrix = ((Component)grabbedRigid).transform.worldToLocalMatrix;
				grabPos = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint(((RaycastHit)(ref val)).point);
				worldToLocalMatrix = ((Component)this).transform.worldToLocalMatrix;
				grabOffset = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint(((RaycastHit)(ref val)).point);
			}
		}
	}

	private void UpdateGrab()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)grabbedRigid == (Object)null))
		{
			Vector3 val = ((Component)this).transform.TransformPoint(grabOffset);
			Vector3 val2 = ((Component)grabbedRigid).transform.TransformPoint(grabPos);
			Vector3 val3 = val - val2;
			grabbedRigid.AddForceAtPosition(val3 * 100f, val2, (ForceMode)5);
		}
	}

	private void StopGrab()
	{
		grabbedRigid = null;
	}
}
