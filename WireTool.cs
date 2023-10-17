using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class WireTool : HeldEntity
{
	public enum WireColour
	{
		Default,
		Red,
		Green,
		Blue,
		Yellow,
		Pink,
		Purple,
		Orange,
		White,
		LightBlue,
		Count
	}

	public struct PendingPlug_t
	{
		public IOEntity ent;

		public bool input;

		public int index;

		public GameObject tempLine;
	}

	public Sprite InputSprite;

	public Sprite OutputSprite;

	public Sprite ClearSprite;

	public static float maxWireLength = 30f;

	private const int maxLineNodes = 16;

	public GameObjectRef plugEffect;

	public SoundDefinition clearStartSoundDef;

	public SoundDefinition clearSoundDef;

	public GameObjectRef ioLine;

	public IOEntity.IOType wireType = IOEntity.IOType.Electric;

	public float RadialMenuHoldTime = 0.25f;

	private const float IndustrialWallOffset = 0.03f;

	public static Phrase Default = new Phrase("wiretoolcolour.default", "Default");

	public static Phrase DefaultDesc = new Phrase("wiretoolcolour.default.desc", "Default connection color");

	public static Phrase Red = new Phrase("wiretoolcolour.red", "Red");

	public static Phrase RedDesc = new Phrase("wiretoolcolour.red.desc", "Red connection color");

	public static Phrase Green = new Phrase("wiretoolcolour.green", "Green");

	public static Phrase GreenDesc = new Phrase("wiretoolcolour.green.desc", "Green connection color");

	public static Phrase Blue = new Phrase("wiretoolcolour.blue", "Blue");

	public static Phrase BlueDesc = new Phrase("wiretoolcolour.blue.desc", "Blue connection color");

	public static Phrase Yellow = new Phrase("wiretoolcolour.yellow", "Yellow");

	public static Phrase YellowDesc = new Phrase("wiretoolcolour.yellow.desc", "Yellow connection color");

	public static Phrase LightBlue = new Phrase("wiretoolcolour.light_blue", "Light Blue");

	public static Phrase LightBlueDesc = new Phrase("wiretoolcolour.light_blue.desc", "Light Blue connection color");

	public static Phrase Orange = new Phrase("wiretoolcolour.orange", "Orange");

	public static Phrase OrangeDesc = new Phrase("wiretoolcolour.orange.desc", "Orange connection color");

	public static Phrase Purple = new Phrase("wiretoolcolour.purple", "Purple");

	public static Phrase PurpleDesc = new Phrase("wiretoolcolour.purple.desc", "Purple connection color");

	public static Phrase White = new Phrase("wiretoolcolour.white", "White");

	public static Phrase WhiteDesc = new Phrase("wiretoolcolour.white.desc", "White connection color");

	public static Phrase Pink = new Phrase("wiretoolcolour.pink", "Pink");

	public static Phrase PinkDesc = new Phrase("wiretoolcolour.pink.desc", "Pink connection color");

	public PendingPlug_t pending;

	private const float IndustrialThickness = 0.01f;

	public bool CanChangeColours => wireType == IOEntity.IOType.Electric || wireType == IOEntity.IOType.Fluidic || wireType == IOEntity.IOType.Industrial;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("WireTool.OnRpcMessage", 0);
		try
		{
			if (rpc == 40328523 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - MakeConnection "));
				}
				TimeWarning val2 = TimeWarning.New("MakeConnection", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(40328523u, "MakeConnection", this, player))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(40328523u, "MakeConnection", this, player))
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
							MakeConnection(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in MakeConnection");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 121409151 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RequestChangeColor "));
				}
				TimeWarning val5 = TimeWarning.New("RequestChangeColor", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(121409151u, "RequestChangeColor", this, player))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(121409151u, "RequestChangeColor", this, player))
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
							RequestChangeColor(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RequestChangeColor");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 2469840259u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RequestClear "));
				}
				TimeWarning val8 = TimeWarning.New("RequestClear", 0);
				try
				{
					TimeWarning val9 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(2469840259u, "RequestClear", this, player))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(2469840259u, "RequestClear", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val9)?.Dispose();
					}
					try
					{
						TimeWarning val10 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg4 = rPCMessage;
							RequestClear(msg4);
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RequestClear");
					}
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
				}
				return true;
			}
			if (rpc == 2596458392u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetPlugged "));
				}
				TimeWarning val11 = TimeWarning.New("SetPlugged", 0);
				try
				{
					TimeWarning val12 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage plugged = rPCMessage;
						SetPlugged(plugged);
					}
					finally
					{
						((IDisposable)val12)?.Dispose();
					}
				}
				catch (Exception ex4)
				{
					Debug.LogException(ex4);
					player.Kick("RPC Error in SetPlugged");
				}
				finally
				{
					((IDisposable)val11)?.Dispose();
				}
				return true;
			}
			if (rpc == 210386477 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - TryClear "));
				}
				TimeWarning val13 = TimeWarning.New("TryClear", 0);
				try
				{
					TimeWarning val14 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(210386477u, "TryClear", this, player))
						{
							return true;
						}
						if (!RPC_Server.IsActiveItem.Test(210386477u, "TryClear", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val14)?.Dispose();
					}
					try
					{
						TimeWarning val15 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg5 = rPCMessage;
							TryClear(msg5);
						}
						finally
						{
							((IDisposable)val15)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in TryClear");
					}
				}
				finally
				{
					((IDisposable)val13)?.Dispose();
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

	public void ClearPendingPlug()
	{
		pending.ent = null;
		pending.index = -1;
	}

	public bool HasPendingPlug()
	{
		return (Object)(object)pending.ent != (Object)null && pending.index != -1;
	}

	public bool PendingPlugIsInput()
	{
		return (Object)(object)pending.ent != (Object)null && pending.index != -1 && pending.input;
	}

	public bool PendingPlugIsType(IOEntity.IOType type)
	{
		return (Object)(object)pending.ent != (Object)null && pending.index != -1 && ((pending.input && pending.ent.inputs[pending.index].type == type) || (!pending.input && pending.ent.outputs[pending.index].type == type));
	}

	public bool PendingPlugIsOutput()
	{
		return (Object)(object)pending.ent != (Object)null && pending.index != -1 && !pending.input;
	}

	public Vector3 PendingPlugWorldPos()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)pending.ent == (Object)null || pending.index == -1)
		{
			return Vector3.zero;
		}
		if (pending.input)
		{
			return ((Component)pending.ent).transform.TransformPoint(pending.ent.inputs[pending.index].handlePosition);
		}
		return ((Component)pending.ent).transform.TransformPoint(pending.ent.outputs[pending.index].handlePosition);
	}

	public static bool CanPlayerUseWires(BasePlayer player)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (!player.CanBuild())
		{
			return false;
		}
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(player.eyes.position, 0.1f, list, 536870912, (QueryTriggerInteraction)2);
		bool result = list.All((Collider collider) => ((Component)collider).gameObject.CompareTag("IgnoreWireCheck"));
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CanModifyEntity(BasePlayer player, IOEntity ent)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return player.CanBuild(((Component)ent).transform.position, ((Component)ent).transform.rotation, ent.bounds) && ent.AllowWireConnections();
	}

	public bool PendingPlugRoot()
	{
		return (Object)(object)pending.ent != (Object)null && pending.ent.IsRootEntity();
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.FromOwner]
	public void TryClear(RPCMessage msg)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		NetworkableId uid = msg.read.EntityID();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		IOEntity iOEntity = (((Object)(object)baseNetworkable == (Object)null) ? null : ((Component)baseNetworkable).GetComponent<IOEntity>());
		if (!((Object)(object)iOEntity == (Object)null) && CanPlayerUseWires(player) && CanModifyEntity(player, iOEntity))
		{
			iOEntity.ClearConnections();
			iOEntity.SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.FromOwner]
	public void MakeConnection(RPCMessage msg)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!CanPlayerUseWires(player))
		{
			return;
		}
		int num = msg.read.Int32();
		if (num > 18)
		{
			return;
		}
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < num; i++)
		{
			Vector3 item = msg.read.Vector3();
			list.Add(item);
		}
		NetworkableId uid = msg.read.EntityID();
		int num2 = msg.read.Int32();
		NetworkableId uid2 = msg.read.EntityID();
		int num3 = msg.read.Int32();
		WireColour wireColour = IntToColour(msg.read.Int32());
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(uid);
		IOEntity iOEntity = (((Object)(object)baseNetworkable == (Object)null) ? null : ((Component)baseNetworkable).GetComponent<IOEntity>());
		if ((Object)(object)iOEntity == (Object)null)
		{
			return;
		}
		BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(uid2);
		IOEntity iOEntity2 = (((Object)(object)baseNetworkable2 == (Object)null) ? null : ((Component)baseNetworkable2).GetComponent<IOEntity>());
		if (!((Object)(object)iOEntity2 == (Object)null) && ValidateLine(list, iOEntity, iOEntity2, player, num3) && !(Vector3.Distance(((Component)baseNetworkable2).transform.position, ((Component)baseNetworkable).transform.position) > maxWireLength) && num2 < iOEntity.inputs.Length && num3 < iOEntity2.outputs.Length && !((Object)(object)iOEntity.inputs[num2].connectedTo.Get() != (Object)null) && !((Object)(object)iOEntity2.outputs[num3].connectedTo.Get() != (Object)null) && (!iOEntity.inputs[num2].rootConnectionsOnly || iOEntity2.IsRootEntity()) && CanModifyEntity(player, iOEntity) && CanModifyEntity(player, iOEntity2))
		{
			iOEntity.inputs[num2].connectedTo.Set(iOEntity2);
			iOEntity.inputs[num2].connectedToSlot = num3;
			iOEntity.inputs[num2].wireColour = wireColour;
			iOEntity.inputs[num2].connectedTo.Init();
			iOEntity2.outputs[num3].connectedTo.Set(iOEntity);
			iOEntity2.outputs[num3].connectedToSlot = num2;
			iOEntity2.outputs[num3].linePoints = list.ToArray();
			iOEntity2.outputs[num3].wireColour = wireColour;
			iOEntity2.outputs[num3].connectedTo.Init();
			iOEntity2.outputs[num3].worldSpaceLineEndRotation = ((Component)iOEntity).transform.TransformDirection(iOEntity.inputs[num2].handleDirection);
			iOEntity2.MarkDirtyForceUpdateOutputs();
			iOEntity2.SendNetworkUpdate();
			iOEntity.SendNetworkUpdate();
			iOEntity2.SendChangedToRoot(forceUpdate: true);
			iOEntity2.RefreshIndustrialPreventBuilding();
			if (wireType == IOEntity.IOType.Industrial)
			{
				iOEntity.NotifyIndustrialNetworkChanged();
				iOEntity2.NotifyIndustrialNetworkChanged();
			}
		}
	}

	[RPC_Server]
	public void SetPlugged(RPCMessage msg)
	{
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.FromOwner]
	public void RequestClear(RPCMessage msg)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (CanPlayerUseWires(player))
		{
			NetworkableId uid = msg.read.EntityID();
			int clearIndex = msg.read.Int32();
			bool isInput = msg.read.Bit();
			BaseNetworkable clearEnt = BaseNetworkable.serverEntities.Find(uid);
			AttemptClearSlot(clearEnt, player, clearIndex, isInput);
		}
	}

	public static void AttemptClearSlot(BaseNetworkable clearEnt, BasePlayer ply, int clearIndex, bool isInput)
	{
		IOEntity iOEntity = (((Object)(object)clearEnt == (Object)null) ? null : ((Component)clearEnt).GetComponent<IOEntity>());
		if ((Object)(object)iOEntity == (Object)null || ((Object)(object)ply != (Object)null && !CanModifyEntity(ply, iOEntity)) || clearIndex >= (isInput ? iOEntity.inputs.Length : iOEntity.outputs.Length))
		{
			return;
		}
		IOEntity.IOSlot iOSlot = (isInput ? iOEntity.inputs[clearIndex] : iOEntity.outputs[clearIndex]);
		if ((Object)(object)iOSlot.connectedTo.Get() == (Object)null)
		{
			return;
		}
		IOEntity iOEntity2 = iOSlot.connectedTo.Get();
		IOEntity.IOSlot iOSlot2 = (isInput ? iOEntity2.outputs[iOSlot.connectedToSlot] : iOEntity2.inputs[iOSlot.connectedToSlot]);
		if (isInput)
		{
			iOEntity.UpdateFromInput(0, clearIndex);
		}
		else if (Object.op_Implicit((Object)(object)iOEntity2))
		{
			iOEntity2.UpdateFromInput(0, iOSlot.connectedToSlot);
		}
		iOSlot.Clear();
		iOSlot2.Clear();
		iOEntity.MarkDirtyForceUpdateOutputs();
		iOEntity.SendNetworkUpdate();
		iOEntity.RefreshIndustrialPreventBuilding();
		if ((Object)(object)iOEntity2 != (Object)null)
		{
			iOEntity2.RefreshIndustrialPreventBuilding();
		}
		if (isInput && (Object)(object)iOEntity2 != (Object)null)
		{
			iOEntity2.SendChangedToRoot(forceUpdate: true);
		}
		else if (!isInput)
		{
			IOEntity.IOSlot[] inputs = iOEntity.inputs;
			foreach (IOEntity.IOSlot iOSlot3 in inputs)
			{
				if (iOSlot3.mainPowerSlot && Object.op_Implicit((Object)(object)iOSlot3.connectedTo.Get()))
				{
					iOSlot3.connectedTo.Get().SendChangedToRoot(forceUpdate: true);
				}
			}
		}
		iOEntity2.SendNetworkUpdate();
		if ((Object)(object)iOEntity != (Object)null && iOEntity.ioType == IOEntity.IOType.Industrial)
		{
			iOEntity.NotifyIndustrialNetworkChanged();
		}
		if ((Object)(object)iOEntity2 != (Object)null && iOEntity2.ioType == IOEntity.IOType.Industrial)
		{
			iOEntity2.NotifyIndustrialNetworkChanged();
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	[RPC_Server.FromOwner]
	public void RequestChangeColor(RPCMessage msg)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!CanPlayerUseWires(player))
		{
			return;
		}
		NetworkableId uid = msg.read.EntityID();
		int index = msg.read.Int32();
		bool flag = msg.read.Bit();
		WireColour wireColour = IntToColour(msg.read.Int32());
		IOEntity iOEntity = BaseNetworkable.serverEntities.Find(uid) as IOEntity;
		if ((Object)(object)iOEntity == (Object)null)
		{
			return;
		}
		IOEntity.IOSlot iOSlot = (flag ? iOEntity.inputs.ElementAtOrDefault(index) : iOEntity.outputs.ElementAtOrDefault(index));
		if (iOSlot != null)
		{
			IOEntity iOEntity2 = iOSlot.connectedTo.Get();
			if (!((Object)(object)iOEntity2 == (Object)null))
			{
				IOEntity.IOSlot iOSlot2 = (flag ? iOEntity2.outputs : iOEntity2.inputs)[iOSlot.connectedToSlot];
				iOSlot.wireColour = wireColour;
				iOEntity.SendNetworkUpdate();
				iOSlot2.wireColour = wireColour;
				iOEntity2.SendNetworkUpdate();
			}
		}
	}

	private WireColour IntToColour(int i)
	{
		if (i < 0)
		{
			i = 0;
		}
		if (i >= 10)
		{
			i = 9;
		}
		WireColour wireColour = (WireColour)i;
		if (wireType == IOEntity.IOType.Fluidic && wireColour == WireColour.Green)
		{
			wireColour = WireColour.Default;
		}
		return wireColour;
	}

	private bool ValidateLine(List<Vector3> lineList, IOEntity inputEntity, IOEntity outputEntity, BasePlayer byPlayer, int outputIndex)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		if (lineList.Count < 2)
		{
			return false;
		}
		if ((Object)(object)inputEntity == (Object)null || (Object)(object)outputEntity == (Object)null)
		{
			return false;
		}
		Vector3 val = lineList[0];
		float num = 0f;
		int count = lineList.Count;
		for (int i = 1; i < count; i++)
		{
			Vector3 val2 = lineList[i];
			num += Vector3.Distance(val, val2);
			if (num > maxWireLength)
			{
				return false;
			}
			val = val2;
		}
		Vector3 val3 = lineList[count - 1];
		Bounds val4 = outputEntity.bounds;
		((Bounds)(ref val4)).Expand(0.5f);
		if (!((Bounds)(ref val4)).Contains(val3))
		{
			return false;
		}
		Vector3 val5 = ((Component)outputEntity).transform.TransformPoint(lineList[0]);
		val3 = ((Component)inputEntity).transform.InverseTransformPoint(val5);
		Bounds val6 = inputEntity.bounds;
		((Bounds)(ref val6)).Expand(0.5f);
		if (!((Bounds)(ref val6)).Contains(val3))
		{
			return false;
		}
		if ((Object)(object)byPlayer == (Object)null)
		{
			return false;
		}
		Vector3 position = ((Component)outputEntity).transform.TransformPoint(lineList[lineList.Count - 1]);
		if (byPlayer.Distance(position) > 5f && byPlayer.Distance(val5) > 5f)
		{
			return false;
		}
		if (outputIndex >= 0 && outputIndex < outputEntity.outputs.Length && outputEntity.outputs[outputIndex].type == IOEntity.IOType.Industrial && !VerifyLineOfSight(lineList, ((Component)outputEntity).transform.localToWorldMatrix))
		{
			return false;
		}
		return true;
	}

	private bool VerifyLineOfSight(List<Vector3> positions, Matrix4x4 localToWorldSpace)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 worldSpaceA = ((Matrix4x4)(ref localToWorldSpace)).MultiplyPoint3x4(positions[0]);
		for (int i = 1; i < positions.Count; i++)
		{
			Vector3 val = ((Matrix4x4)(ref localToWorldSpace)).MultiplyPoint3x4(positions[i]);
			if (!VerifyLineOfSight(worldSpaceA, val))
			{
				return false;
			}
			worldSpaceA = val;
		}
		return true;
	}

	private bool VerifyLineOfSight(Vector3 worldSpaceA, Vector3 worldSpaceB)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		float maxDistance = Vector3.Distance(worldSpaceA, worldSpaceB);
		Vector3 val = worldSpaceA - worldSpaceB;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(worldSpaceB, normalized), 0.01f, list, maxDistance, 2162944, (QueryTriggerInteraction)0);
		bool result = true;
		foreach (RaycastHit item in list)
		{
			BaseEntity entity = item.GetEntity();
			if ((Object)(object)entity != (Object)null && item.IsOnLayer((Layer)8))
			{
				if (entity is VendingMachine)
				{
					result = false;
					break;
				}
			}
			else if (!((Object)(object)entity != (Object)null) || !(entity is Door))
			{
				result = false;
				break;
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return result;
	}
}
