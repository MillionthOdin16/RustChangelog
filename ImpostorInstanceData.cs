using UnityEngine;

public class ImpostorInstanceData
{
	public ImpostorBatch Batch = null;

	public int BatchIndex = 0;

	private int hash = 0;

	private Vector4 positionAndScale = Vector4.zero;

	public Renderer Renderer { get; private set; } = null;


	public Mesh Mesh { get; private set; } = null;


	public Material Material { get; private set; } = null;


	public ImpostorInstanceData(Renderer renderer, Mesh mesh, Material material)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Renderer = renderer;
		Mesh = mesh;
		Material = material;
		hash = GenerateHashCode();
		Update();
	}

	public ImpostorInstanceData(Vector3 position, Vector3 scale, Mesh mesh, Material material)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		positionAndScale = new Vector4(position.x, position.y, position.z, scale.x);
		Mesh = mesh;
		Material = material;
		hash = GenerateHashCode();
		Update();
	}

	private int GenerateHashCode()
	{
		int num = 17;
		num = num * 31 + ((object)Material).GetHashCode();
		return num * 31 + ((object)Mesh).GetHashCode();
	}

	public override bool Equals(object obj)
	{
		ImpostorInstanceData impostorInstanceData = obj as ImpostorInstanceData;
		return (Object)(object)impostorInstanceData.Material == (Object)(object)Material && (Object)(object)impostorInstanceData.Mesh == (Object)(object)Mesh;
	}

	public override int GetHashCode()
	{
		return hash;
	}

	public Vector4 PositionAndScale()
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Renderer != (Object)null)
		{
			Transform transform = ((Component)Renderer).transform;
			Vector3 position = transform.position;
			Vector3 lossyScale = transform.lossyScale;
			float num = (Renderer.enabled ? lossyScale.x : (0f - lossyScale.x));
			positionAndScale = new Vector4(position.x, position.y, position.z, num);
		}
		return positionAndScale;
	}

	public void Update()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (Batch != null)
		{
			Batch.Positions[BatchIndex] = PositionAndScale();
			Batch.IsDirty = true;
		}
	}
}
