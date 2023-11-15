using System;
using UnityEngine;

public class WaterCatcher : LiquidContainer
{
	[Header("Water Catcher")]
	public ItemDefinition itemToCreate;

	public WaterCatcherCollectRate collectionRates;

	public float maxItemToCreate = 10f;

	[Header("Outside Test")]
	public Vector3 rainTestPosition = new Vector3(0f, 1f, 0f);

	public float rainTestSize = 1f;

	private const float collectInterval = 60f;

	public override void ServerInit()
	{
		base.ServerInit();
		AddResource(1);
		((FacepunchBehaviour)this).InvokeRandomized((Action)CollectWater, 60f, 60f, 6f);
	}

	private void CollectWater()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!IsFull())
		{
			float baseRate = collectionRates.baseRate;
			baseRate += Climate.GetFog(((Component)this).transform.position) * collectionRates.fogRate;
			if (TestIsOutside())
			{
				baseRate += Climate.GetRain(((Component)this).transform.position) * collectionRates.rainRate;
				baseRate += Climate.GetSnow(((Component)this).transform.position) * collectionRates.snowRate;
			}
			AddResource(Mathf.CeilToInt(maxItemToCreate * baseRate));
		}
	}

	private bool IsFull()
	{
		if (base.inventory.itemList.Count == 0)
		{
			return false;
		}
		if (base.inventory.itemList[0].amount < base.inventory.maxStackSize)
		{
			return false;
		}
		return true;
	}

	private bool TestIsOutside()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		return !Physics.SphereCast(new Ray(((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(rainTestPosition), Vector3.up), rainTestSize, 256f, 161546513);
	}

	private void AddResource(int iAmount)
	{
		if (outputs.Length != 0)
		{
			IOEntity iOEntity = CheckPushLiquid(outputs[0].connectedTo.Get(), iAmount, this, IOEntity.backtracking * 2);
			if ((Object)(object)iOEntity != (Object)null && iOEntity is LiquidContainer liquidContainer)
			{
				liquidContainer.inventory.AddItem(itemToCreate, iAmount, 0uL);
				return;
			}
		}
		base.inventory.AddItem(itemToCreate, iAmount, 0uL);
		UpdateOnFlag();
	}

	private IOEntity CheckPushLiquid(IOEntity connected, int amount, IOEntity fromSource, int depth)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (depth <= 0 || (Object)(object)itemToCreate == (Object)null)
		{
			return null;
		}
		if ((Object)(object)connected == (Object)null)
		{
			return null;
		}
		Vector3 worldHandlePosition = Vector3.zero;
		IOEntity iOEntity = connected.FindGravitySource(ref worldHandlePosition, IOEntity.backtracking, ignoreSelf: true);
		if ((Object)(object)iOEntity != (Object)null && !connected.AllowLiquidPassthrough(iOEntity, worldHandlePosition))
		{
			return null;
		}
		if ((Object)(object)connected == (Object)(object)this || ConsiderConnectedTo(connected))
		{
			return null;
		}
		if (connected.prefabID == 2150367216u)
		{
			return null;
		}
		IOSlot[] array = connected.outputs;
		foreach (IOSlot iOSlot in array)
		{
			IOEntity iOEntity2 = iOSlot.connectedTo.Get();
			Vector3 sourceWorldPosition = ((Component)connected).transform.TransformPoint(iOSlot.handlePosition);
			if ((Object)(object)iOEntity2 != (Object)null && (Object)(object)iOEntity2 != (Object)(object)fromSource && iOEntity2.AllowLiquidPassthrough(connected, sourceWorldPosition))
			{
				IOEntity iOEntity3 = CheckPushLiquid(iOEntity2, amount, fromSource, depth - 1);
				if ((Object)(object)iOEntity3 != (Object)null)
				{
					return iOEntity3;
				}
			}
		}
		if (connected is LiquidContainer liquidContainer && liquidContainer.inventory.GetAmount(itemToCreate.itemid, onlyUsableAmounts: false) + amount < liquidContainer.maxStackSize)
		{
			return connected;
		}
		return null;
	}
}
