using System;
using UnityEngine;
using UnityEngine.Profiling;

public class WaterCatcher : LiquidContainer
{
	[Header("Water Catcher")]
	public ItemDefinition itemToCreate;

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
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!IsFull())
		{
			Profiler.BeginSample("WaterCalc");
			float num = 0.25f;
			num += Climate.GetFog(((Component)this).transform.position) * 2f;
			if (TestIsOutside())
			{
				num += Climate.GetRain(((Component)this).transform.position);
				num += Climate.GetSnow(((Component)this).transform.position) * 0.5f;
			}
			Profiler.EndSample();
			AddResource(Mathf.CeilToInt(maxItemToCreate * num));
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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		return !Physics.SphereCast(new Ray(((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(rainTestPosition), Vector3.up), rainTestSize, 256f, 161546513);
	}

	private void AddResource(int iAmount)
	{
		Profiler.BeginSample("AddResource.FindTarget");
		if (outputs.Length != 0)
		{
			IOEntity iOEntity = CheckPushLiquid(outputs[0].connectedTo.Get(), iAmount, this, IOEntity.backtracking * 2);
			Profiler.EndSample();
			if ((Object)(object)iOEntity != (Object)null && iOEntity is LiquidContainer liquidContainer)
			{
				Profiler.BeginSample("AddResource.AddResult");
				liquidContainer.inventory.AddItem(itemToCreate, iAmount, 0uL);
				Profiler.EndSample();
				return;
			}
		}
		base.inventory.AddItem(itemToCreate, iAmount, 0uL);
		UpdateOnFlag();
	}

	private IOEntity CheckPushLiquid(IOEntity connected, int amount, IOEntity fromSource, int depth)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (depth <= 0 || (Object)(object)itemToCreate == (Object)null)
		{
			return null;
		}
		if ((Object)(object)connected == (Object)null)
		{
			return null;
		}
		Profiler.BeginSample("WaterCatcher.CheckPushLiquid.FindGravitySource");
		Vector3 worldHandlePosition = Vector3.zero;
		IOEntity iOEntity = connected.FindGravitySource(ref worldHandlePosition, IOEntity.backtracking, ignoreSelf: true);
		Profiler.EndSample();
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
			Profiler.BeginSample("WaterCatcher.CheckPushLiquid.AllowPassthrough");
			IOEntity iOEntity2 = iOSlot.connectedTo.Get();
			Vector3 sourceWorldPosition = ((Component)connected).transform.TransformPoint(iOSlot.handlePosition);
			if ((Object)(object)iOEntity2 != (Object)null && (Object)(object)iOEntity2 != (Object)(object)fromSource && iOEntity2.AllowLiquidPassthrough(connected, sourceWorldPosition))
			{
				IOEntity iOEntity3 = CheckPushLiquid(iOEntity2, amount, fromSource, depth - 1);
				if ((Object)(object)iOEntity3 != (Object)null)
				{
					Profiler.EndSample();
					return iOEntity3;
				}
			}
			Profiler.EndSample();
		}
		if (connected is LiquidContainer liquidContainer)
		{
			int amount2 = liquidContainer.inventory.GetAmount(itemToCreate.itemid, onlyUsableAmounts: false);
			if (amount2 + amount < liquidContainer.maxStackSize)
			{
				return connected;
			}
		}
		return null;
	}
}
