namespace UnityEngine.Rendering.PostProcessing;

[ExecuteAlways]
[AddComponentMenu("Rendering/Post-process Volume", 1001)]
public sealed class PostProcessVolume : MonoBehaviour
{
	public PostProcessProfile sharedProfile;

	[Tooltip("Check this box to mark this volume as global. This volume's Profile will be applied to the whole Scene.")]
	public bool isGlobal = false;

	public Bounds bounds;

	[Min(0f)]
	[Tooltip("The distance (from the attached Collider) to start blending from. A value of 0 means there will be no blending and the Volume overrides will be applied immediatly upon entry to the attached Collider.")]
	public float blendDistance = 0f;

	[Range(0f, 1f)]
	[Tooltip("The total weight of this Volume in the Scene. A value of 0 signifies that it will have no effect, 1 signifies full effect.")]
	public float weight = 1f;

	[Tooltip("The volume priority in the stack. A higher value means higher priority. Negative values are supported.")]
	public float priority = 0f;

	private int m_PreviousLayer;

	private float m_PreviousPriority;

	private PostProcessProfile m_InternalProfile;

	public PostProcessProfile profile
	{
		get
		{
			if ((Object)(object)m_InternalProfile == (Object)null)
			{
				m_InternalProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
				if ((Object)(object)sharedProfile != (Object)null)
				{
					foreach (PostProcessEffectSettings setting in sharedProfile.settings)
					{
						PostProcessEffectSettings item = Object.Instantiate<PostProcessEffectSettings>(setting);
						m_InternalProfile.settings.Add(item);
					}
				}
			}
			return m_InternalProfile;
		}
		set
		{
			m_InternalProfile = value;
		}
	}

	internal PostProcessProfile profileRef => ((Object)(object)m_InternalProfile == (Object)null) ? sharedProfile : m_InternalProfile;

	public bool HasInstantiatedProfile()
	{
		return (Object)(object)m_InternalProfile != (Object)null;
	}

	private void OnEnable()
	{
		PostProcessManager.instance.Register(this);
		m_PreviousLayer = ((Component)this).gameObject.layer;
	}

	private void OnDisable()
	{
		PostProcessManager.instance.Unregister(this);
	}

	private void Update()
	{
		int layer = ((Component)this).gameObject.layer;
		if (layer != m_PreviousLayer)
		{
			PostProcessManager.instance.UpdateVolumeLayer(this, m_PreviousLayer, layer);
			m_PreviousLayer = layer;
		}
		if (priority != m_PreviousPriority)
		{
			PostProcessManager.instance.SetLayerDirty(layer);
			m_PreviousPriority = priority;
		}
	}

	private void OnDrawGizmos()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (!isGlobal)
		{
			Vector3 lossyScale = ((Component)this).transform.lossyScale;
			Vector3 val = default(Vector3);
			((Vector3)(ref val))._002Ector(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z);
			Gizmos.matrix = Matrix4x4.TRS(((Component)this).transform.position, ((Component)this).transform.rotation, lossyScale);
			Gizmos.DrawCube(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).size);
			Gizmos.DrawWireCube(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).size + val * blendDistance * 4f);
			Gizmos.matrix = Matrix4x4.identity;
		}
	}
}
