using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class MeshPaintableSource : MonoBehaviour, IClientComponent
{
	public Vector4 uvRange = new Vector4(0f, 0f, 1f, 1f);

	public int texWidth = 256;

	public int texHeight = 128;

	public string replacementTextureName = "_DecalTexture";

	public float cameraFOV = 60f;

	public float cameraDistance = 2f;

	[NonSerialized]
	public Texture2D texture;

	public GameObject sourceObject;

	public Mesh collisionMesh;

	public Vector3 localPosition;

	public Vector3 localRotation;

	public bool applyToAllRenderers = true;

	public Renderer[] extraRenderers;

	public bool paint3D;

	public bool applyToSkinRenderers = true;

	public bool applyToFirstPersonLegs = true;

	[NonSerialized]
	public bool isSelected;

	[NonSerialized]
	public Renderer legRenderer;

	private static MaterialPropertyBlock block;

	public void Init()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		if ((Object)(object)texture == (Object)null)
		{
			texture = new Texture2D(texWidth, texHeight, (TextureFormat)5, false);
			((Object)texture).name = "MeshPaintableSource_" + ((Object)((Component)this).gameObject).name;
			((Texture)texture).wrapMode = (TextureWrapMode)1;
			TextureEx.Clear(texture, Color32.op_Implicit(Color.clear));
		}
		if (block == null)
		{
			block = new MaterialPropertyBlock();
		}
		else
		{
			block.Clear();
		}
		UpdateMaterials(block, null, forEditing: false, isSelected);
		List<Renderer> list = Pool.GetList<Renderer>();
		Transform val = (applyToAllRenderers ? ((Component)this).transform.root : ((Component)this).transform);
		if (applyToSkinRenderers)
		{
			BaseEntity componentInParent = ((Component)this).GetComponentInParent<BaseEntity>();
			if ((Object)(object)componentInParent != (Object)null)
			{
				val = ((Component)componentInParent).transform;
			}
		}
		((Component)val).GetComponentsInChildren<Renderer>(true, list);
		PlayerModelSkin playerModelSkin = default(PlayerModelSkin);
		foreach (Renderer item in list)
		{
			if (applyToSkinRenderers || !((Component)item).TryGetComponent<PlayerModelSkin>(ref playerModelSkin))
			{
				item.SetPropertyBlock(block);
			}
		}
		if (extraRenderers != null)
		{
			Renderer[] array = extraRenderers;
			foreach (Renderer val2 in array)
			{
				if ((Object)(object)val2 != (Object)null)
				{
					val2.SetPropertyBlock(block);
				}
			}
		}
		if (applyToFirstPersonLegs && (Object)(object)legRenderer != (Object)null)
		{
			legRenderer.SetPropertyBlock(block);
		}
		Pool.FreeList<Renderer>(ref list);
	}

	public void Free()
	{
		if (Object.op_Implicit((Object)(object)texture))
		{
			Object.Destroy((Object)(object)texture);
			texture = null;
		}
	}

	public void OnDestroy()
	{
		Free();
	}

	public virtual void UpdateMaterials(MaterialPropertyBlock block, Texture2D textureOverride = null, bool forEditing = false, bool isSelected = false)
	{
		block.SetTexture(replacementTextureName, (Texture)(object)(textureOverride ?? texture));
	}

	public virtual Color32[] UpdateFrom(Texture2D input)
	{
		Init();
		Color32[] pixels = input.GetPixels32();
		texture.SetPixels32(pixels);
		texture.Apply(true, false);
		return pixels;
	}

	public void Load(byte[] data)
	{
		Init();
		if (data != null)
		{
			ImageConversion.LoadImage(texture, data);
			texture.Apply(true, false);
		}
	}

	public void Clear()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)texture == (Object)null))
		{
			TextureEx.Clear(texture, Color32.op_Implicit(new Color(0f, 0f, 0f, 0f)));
			texture.Apply(true, false);
		}
	}
}
