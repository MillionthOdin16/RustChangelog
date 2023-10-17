using UnityEngine;

public class v_chainsaw : MonoBehaviour
{
	public bool bAttacking;

	public bool bHitMetal;

	public bool bHitWood;

	public bool bHitFlesh;

	public bool bEngineOn;

	public ParticleSystem[] hitMetalFX;

	public ParticleSystem[] hitWoodFX;

	public ParticleSystem[] hitFleshFX;

	public SoundDefinition hitMetalSoundDef;

	public SoundDefinition hitWoodSoundDef;

	public SoundDefinition hitFleshSoundDef;

	public Sound hitSound;

	public GameObject hitSoundTarget;

	public float hitSoundFadeTime = 0.1f;

	public ParticleSystem smokeEffect;

	public Animator chainsawAnimator;

	public Renderer chainRenderer;

	public Material chainlink;

	private MaterialPropertyBlock block;

	private Vector2 saveST;

	private float chainSpeed = 0f;

	private float chainAmount = 0f;

	public float temp1;

	public float temp2;

	public void OnEnable()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		if (block == null)
		{
			block = new MaterialPropertyBlock();
		}
		saveST = Vector4.op_Implicit(chainRenderer.sharedMaterial.GetVector("_MainTex_ST"));
	}

	private void Awake()
	{
		chainlink = chainRenderer.sharedMaterial;
	}

	private void Start()
	{
	}

	private void ScrollChainTexture()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		float num = (chainAmount = (chainAmount + Time.deltaTime * chainSpeed) % 1f);
		block.Clear();
		block.SetVector("_MainTex_ST", new Vector4(saveST.x, saveST.y, num, 0f));
		chainRenderer.SetPropertyBlock(block);
	}

	private void Update()
	{
		chainsawAnimator.SetBool("attacking", bAttacking);
		smokeEffect.enableEmission = bEngineOn;
		if (bHitMetal)
		{
			chainsawAnimator.SetBool("attackHit", true);
			ParticleSystem[] array = hitMetalFX;
			foreach (ParticleSystem val in array)
			{
				val.enableEmission = true;
			}
			ParticleSystem[] array2 = hitWoodFX;
			foreach (ParticleSystem val2 in array2)
			{
				val2.enableEmission = false;
			}
			ParticleSystem[] array3 = hitFleshFX;
			foreach (ParticleSystem val3 in array3)
			{
				val3.enableEmission = false;
			}
			DoHitSound(hitMetalSoundDef);
		}
		else if (bHitWood)
		{
			chainsawAnimator.SetBool("attackHit", true);
			ParticleSystem[] array4 = hitMetalFX;
			foreach (ParticleSystem val4 in array4)
			{
				val4.enableEmission = false;
			}
			ParticleSystem[] array5 = hitWoodFX;
			foreach (ParticleSystem val5 in array5)
			{
				val5.enableEmission = true;
			}
			ParticleSystem[] array6 = hitFleshFX;
			foreach (ParticleSystem val6 in array6)
			{
				val6.enableEmission = false;
			}
			DoHitSound(hitWoodSoundDef);
		}
		else if (bHitFlesh)
		{
			chainsawAnimator.SetBool("attackHit", true);
			ParticleSystem[] array7 = hitMetalFX;
			foreach (ParticleSystem val7 in array7)
			{
				val7.enableEmission = false;
			}
			ParticleSystem[] array8 = hitWoodFX;
			foreach (ParticleSystem val8 in array8)
			{
				val8.enableEmission = false;
			}
			ParticleSystem[] array9 = hitFleshFX;
			foreach (ParticleSystem val9 in array9)
			{
				val9.enableEmission = true;
			}
			DoHitSound(hitFleshSoundDef);
		}
		else
		{
			chainsawAnimator.SetBool("attackHit", false);
			ParticleSystem[] array10 = hitMetalFX;
			foreach (ParticleSystem val10 in array10)
			{
				val10.enableEmission = false;
			}
			ParticleSystem[] array11 = hitWoodFX;
			foreach (ParticleSystem val11 in array11)
			{
				val11.enableEmission = false;
			}
			ParticleSystem[] array12 = hitFleshFX;
			foreach (ParticleSystem val12 in array12)
			{
				val12.enableEmission = false;
			}
		}
	}

	private void DoHitSound(SoundDefinition soundDef)
	{
	}
}
