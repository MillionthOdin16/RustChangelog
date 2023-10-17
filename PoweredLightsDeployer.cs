using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class PoweredLightsDeployer : HeldEntity
{
	public GameObjectRef poweredLightsPrefab;

	public EntityRef activeLights;

	public MaterialReplacement guide;

	public GameObject guideObject;

	public float maxPlaceDistance = 5f;

	public float lengthPerAmount = 0.5f;

	private const int placementLayerMask = 10551297;

	public AdvancedChristmasLights active
	{
		get
		{
			BaseEntity baseEntity = activeLights.Get(base.isServer);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				return ((Component)baseEntity).GetComponent<AdvancedChristmasLights>();
			}
			return null;
		}
		set
		{
			activeLights.Set(value);
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PoweredLightsDeployer.OnRpcMessage", 0);
		try
		{
			if (rpc == 447739874 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - AddPoint "));
				}
				TimeWarning val2 = TimeWarning.New("AddPoint", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(447739874u, "AddPoint", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						TimeWarning val4 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							AddPoint(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in AddPoint");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1975273522 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Finish "));
				}
				TimeWarning val5 = TimeWarning.New("Finish", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1975273522u, "Finish", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
					try
					{
						TimeWarning val7 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							Finish(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Finish");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	private bool CheckValidPlacement(Vector3 position, float radius, int layerMask)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		bool result = true;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(position, radius, list, 10551297, (QueryTriggerInteraction)2);
		foreach (BaseEntity item in list)
		{
			if (item is AnimatedBuildingBlock)
			{
				result = false;
				break;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return result;
	}

	public static bool CanPlayerUse(BasePlayer player)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return player.CanBuild() && !GamePhysics.CheckSphere(player.eyes.position, 0.1f, 536870912, (QueryTriggerInteraction)2);
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void AddPoint(RPCMessage msg)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = msg.read.Vector3();
		Vector3 val2 = msg.read.Vector3();
		BasePlayer player = msg.player;
		if (GetItem() == null || GetItem().amount < 1 || !IsVisible(val) || !CanPlayerUse(player) || Vector3.Distance(val, player.eyes.position) > maxPlaceDistance || !CheckValidPlacement(val, 0.1f, 10551297))
		{
			return;
		}
		int num = 1;
		if ((Object)(object)active == (Object)null)
		{
			AdvancedChristmasLights component = ((Component)GameManager.server.CreateEntity(poweredLightsPrefab.resourcePath, val, Quaternion.LookRotation(val2, player.eyes.HeadUp()))).GetComponent<AdvancedChristmasLights>();
			component.Spawn();
			active = component;
			num = 1;
		}
		else
		{
			if (active.IsFinalized())
			{
				return;
			}
			float num2 = 0f;
			Vector3 val3 = ((Component)active).transform.position;
			if (active.points.Count > 0)
			{
				val3 = active.points[active.points.Count - 1].point;
				num2 = Vector3.Distance(val, val3);
			}
			num2 = Mathf.Max(num2, lengthPerAmount);
			float num3 = (float)GetItem().amount * lengthPerAmount;
			if (num2 > num3)
			{
				num2 = num3;
				val = val3 + Vector3Ex.Direction(val, val3) * num2;
			}
			num2 = Mathf.Min(num3, num2);
			num = Mathf.CeilToInt(num2 / lengthPerAmount);
		}
		active.AddPoint(val, val2);
		SetFlag(Flags.Reserved8, (Object)(object)active != (Object)null);
		int iAmount = num;
		UseItemAmount(iAmount);
		active.AddLengthUsed(num);
		SendNetworkUpdate();
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void Finish(RPCMessage msg)
	{
		DoFinish();
	}

	public void DoFinish()
	{
		if (Object.op_Implicit((Object)(object)active))
		{
			active.FinishEditing();
		}
		active = null;
		SendNetworkUpdate();
	}

	public override void OnHeldChanged()
	{
		DoFinish();
		active = null;
		base.OnHeldChanged();
	}

	public override void Save(SaveInfo info)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.lightDeployer = Pool.Get<LightDeployer>();
			info.msg.lightDeployer.active = activeLights.uid;
		}
	}
}
