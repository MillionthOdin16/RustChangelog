using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class IOEntity : DecayEntity
{
	public enum IOType
	{
		Electric,
		Fluidic,
		Kinetic,
		Generic,
		Industrial
	}

	[Serializable]
	public class IORef
	{
		public EntityRef entityRef;

		public IOEntity ioEnt;

		public void Init()
		{
			if ((Object)(object)ioEnt != (Object)null && !entityRef.IsValid(serverside: true))
			{
				entityRef.Set(ioEnt);
			}
			if (entityRef.IsValid(serverside: true))
			{
				ioEnt = ((Component)entityRef.Get(serverside: true)).GetComponent<IOEntity>();
			}
		}

		public void InitClient()
		{
			if (entityRef.IsValid(serverside: false) && (Object)(object)ioEnt == (Object)null)
			{
				ioEnt = ((Component)entityRef.Get(serverside: false)).GetComponent<IOEntity>();
			}
		}

		public IOEntity Get(bool isServer = true)
		{
			if ((Object)(object)ioEnt == (Object)null && entityRef.IsValid(isServer))
			{
				ioEnt = entityRef.Get(isServer) as IOEntity;
			}
			return ioEnt;
		}

		public void Clear()
		{
			ioEnt = null;
			entityRef.Set(null);
		}

		public void Set(IOEntity newIOEnt)
		{
			entityRef.Set(newIOEnt);
		}
	}

	[Serializable]
	public class IOSlot
	{
		public string niceName;

		public IOType type;

		public IORef connectedTo;

		public int connectedToSlot;

		public Vector3[] linePoints;

		public float[] slackLevels;

		public Vector3 worldSpaceLineEndRotation;

		public ClientIOLine line;

		public Vector3 handlePosition;

		public Vector3 handleDirection;

		public bool rootConnectionsOnly;

		public bool mainPowerSlot;

		public WireTool.WireColour wireColour;

		public float lineThickness;

		public void Clear()
		{
			if (connectedTo == null)
			{
				connectedTo = new IORef();
			}
			else
			{
				connectedTo.Clear();
			}
			connectedToSlot = 0;
			linePoints = null;
		}
	}

	private struct FrameTiming
	{
		public string PrefabName;

		public float Time;
	}

	public struct ContainerInputOutput
	{
		public IIndustrialStorage Storage;

		public int SlotIndex;

		public int MaxStackSize;

		public int ParentStorage;

		public int IndustrialSiblingCount;
	}

	[Header("IOEntity")]
	public Transform debugOrigin;

	public ItemDefinition sourceItem;

	[NonSerialized]
	public int lastResetIndex;

	[ServerVar]
	[Help("How many miliseconds to budget for processing io entities per server frame")]
	public static float framebudgetms = 1f;

	[ServerVar]
	public static float responsetime = 0.1f;

	[ServerVar]
	public static int backtracking = 8;

	[ServerVar(Help = "Print out what is taking so long in the IO frame budget")]
	public static bool debugBudget = false;

	[ServerVar(Help = "Ignore frames with a lower ms than this while debugBudget is active")]
	public static float debugBudgetThreshold = 2f;

	public const Flags Flag_ShortCircuit = Flags.Reserved7;

	public const Flags Flag_HasPower = Flags.Reserved8;

	public IOSlot[] inputs;

	public IOSlot[] outputs;

	public IOType ioType;

	public static Queue<IOEntity> _processQueue = new Queue<IOEntity>();

	private static List<FrameTiming> timings = new List<FrameTiming>();

	private int cachedOutputsUsed;

	protected int lastPassthroughEnergy;

	private int lastEnergy;

	protected int currentEnergy;

	protected float lastUpdateTime;

	protected int lastUpdateBlockedFrame;

	protected bool ensureOutputsUpdated;

	public const int MaxContainerSourceCount = 32;

	private List<BoxCollider> spawnedColliders = new List<BoxCollider>();

	public virtual bool IsGravitySource => false;

	private bool HasBlockedUpdatedOutputsThisFrame => Time.frameCount == lastUpdateBlockedFrame;

	public virtual bool BlockFluidDraining => false;

	protected virtual float LiquidPassthroughGravityThreshold => 1f;

	protected virtual bool DisregardGravityRestrictionsOnLiquid => false;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("IOEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 4161541566u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RequestData "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RequestData", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(4161541566u, "Server_RequestData", this, player, 10uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(4161541566u, "Server_RequestData", this, player, 6f))
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
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							Server_RequestData(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_RequestData");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
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

	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			lastResetIndex = 0;
			cachedOutputsUsed = 0;
			lastPassthroughEnergy = 0;
			lastEnergy = 0;
			currentEnergy = 0;
			lastUpdateTime = 0f;
			ensureOutputsUpdated = false;
		}
		ClearIndustrialPreventBuilding();
	}

	public string GetDisplayName()
	{
		if ((Object)(object)sourceItem != (Object)null)
		{
			return sourceItem.displayName.translated;
		}
		return base.ShortPrefabName;
	}

	public virtual bool IsRootEntity()
	{
		return false;
	}

	public IOEntity FindGravitySource(ref Vector3 worldHandlePosition, int depth, bool ignoreSelf)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (depth <= 0)
		{
			return null;
		}
		if (!ignoreSelf && IsGravitySource)
		{
			worldHandlePosition = ((Component)this).transform.TransformPoint(outputs[0].handlePosition);
			return this;
		}
		IOSlot[] array = inputs;
		for (int i = 0; i < array.Length; i++)
		{
			IOEntity iOEntity = array[i].connectedTo.Get(base.isServer);
			if ((Object)(object)iOEntity != (Object)null)
			{
				if (iOEntity.IsGravitySource)
				{
					worldHandlePosition = ((Component)iOEntity).transform.TransformPoint(iOEntity.outputs[0].handlePosition);
					return iOEntity;
				}
				iOEntity = iOEntity.FindGravitySource(ref worldHandlePosition, depth - 1, ignoreSelf: false);
				if ((Object)(object)iOEntity != (Object)null)
				{
					worldHandlePosition = ((Component)iOEntity).transform.TransformPoint(iOEntity.outputs[0].handlePosition);
					return iOEntity;
				}
			}
		}
		return null;
	}

	public virtual void SetFuelType(ItemDefinition def, IOEntity source)
	{
	}

	public virtual bool WantsPower()
	{
		return true;
	}

	public virtual bool AllowWireConnections()
	{
		if ((Object)(object)((Component)this).GetComponentInParent<BaseVehicle>() != (Object)null)
		{
			return false;
		}
		return true;
	}

	public virtual bool WantsPassthroughPower()
	{
		return WantsPower();
	}

	public virtual int ConsumptionAmount()
	{
		return 1;
	}

	public virtual bool ShouldDrainBattery(IOEntity battery)
	{
		return ioType == battery.ioType;
	}

	public virtual int MaximalPowerOutput()
	{
		return 0;
	}

	public virtual bool AllowDrainFrom(int outputSlot)
	{
		return true;
	}

	public virtual bool IsPowered()
	{
		return HasFlag(Flags.Reserved8);
	}

	public bool IsConnectedToAnySlot(IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < inputs.Length)
		{
			IOEntity iOEntity = inputs[slot].connectedTo.Get();
			if ((Object)(object)iOEntity != (Object)null)
			{
				if ((Object)(object)iOEntity == (Object)(object)entity)
				{
					return true;
				}
				if (ConsiderConnectedTo(entity))
				{
					return true;
				}
				if (iOEntity.IsConnectedTo(entity, depth - 1, defaultReturn))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsConnectedTo(IOEntity entity, int slot, int depth, bool defaultReturn = false)
	{
		if (depth > 0 && slot < inputs.Length)
		{
			IOSlot iOSlot = inputs[slot];
			if (iOSlot.mainPowerSlot)
			{
				IOEntity iOEntity = iOSlot.connectedTo.Get();
				if ((Object)(object)iOEntity != (Object)null)
				{
					if ((Object)(object)iOEntity == (Object)(object)entity)
					{
						return true;
					}
					if (ConsiderConnectedTo(entity))
					{
						return true;
					}
					if (iOEntity.IsConnectedTo(entity, depth - 1, defaultReturn))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool IsConnectedTo(IOEntity entity, int depth, bool defaultReturn = false)
	{
		if (depth > 0)
		{
			for (int i = 0; i < inputs.Length; i++)
			{
				IOSlot iOSlot = inputs[i];
				if (!iOSlot.mainPowerSlot)
				{
					continue;
				}
				IOEntity iOEntity = iOSlot.connectedTo.Get();
				if ((Object)(object)iOEntity != (Object)null)
				{
					if ((Object)(object)iOEntity == (Object)(object)entity)
					{
						return true;
					}
					if (ConsiderConnectedTo(entity))
					{
						return true;
					}
					if (iOEntity.IsConnectedTo(entity, depth - 1, defaultReturn))
					{
						return true;
					}
				}
			}
			return false;
		}
		return defaultReturn;
	}

	protected virtual bool ConsiderConnectedTo(IOEntity entity)
	{
		return false;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(6f)]
	[RPC_Server.CallsPerSecond(10uL)]
	private void Server_RequestData(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		int slot = msg.read.Int32();
		bool input = msg.read.Int32() == 1;
		SendAdditionalData(player, slot, input);
	}

	public virtual void SendAdditionalData(BasePlayer player, int slot, bool input)
	{
		int passthroughAmountForAnySlot = GetPassthroughAmountForAnySlot(slot, input);
		ClientRPCPlayer(null, player, "Client_ReceiveAdditionalData", currentEnergy, passthroughAmountForAnySlot, 0f, 0f);
	}

	protected int GetPassthroughAmountForAnySlot(int slot, bool isInputSlot)
	{
		int result = 0;
		if (isInputSlot)
		{
			if (slot >= 0 && slot < inputs.Length)
			{
				IOSlot iOSlot = inputs[slot];
				IOEntity iOEntity = iOSlot.connectedTo.Get();
				if ((Object)(object)iOEntity != (Object)null && iOSlot.connectedToSlot >= 0 && iOSlot.connectedToSlot < iOEntity.outputs.Length)
				{
					result = iOEntity.GetPassthroughAmount(inputs[slot].connectedToSlot);
				}
			}
		}
		else if (slot >= 0 && slot < outputs.Length)
		{
			result = GetPassthroughAmount(slot);
		}
		return result;
	}

	public static void ProcessQueue()
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = framebudgetms / 1000f;
		if (debugBudget)
		{
			timings.Clear();
		}
		while (_processQueue.Count > 0 && Time.realtimeSinceStartup < realtimeSinceStartup + num && !_processQueue.Peek().HasBlockedUpdatedOutputsThisFrame)
		{
			float realtimeSinceStartup2 = Time.realtimeSinceStartup;
			IOEntity iOEntity = _processQueue.Dequeue();
			if (iOEntity.IsValid())
			{
				iOEntity.UpdateOutputs();
			}
			if (debugBudget)
			{
				timings.Add(new FrameTiming
				{
					PrefabName = iOEntity.ShortPrefabName,
					Time = (Time.realtimeSinceStartup - realtimeSinceStartup2) * 1000f
				});
			}
		}
		if (!debugBudget)
		{
			return;
		}
		float num2 = Time.realtimeSinceStartup - realtimeSinceStartup;
		float num3 = debugBudgetThreshold / 1000f;
		if (!(num2 > num3))
		{
			return;
		}
		TextTable val = new TextTable();
		val.AddColumns(new string[2] { "Prefab Name", "Time (in ms)" });
		foreach (FrameTiming timing in timings)
		{
			string[] obj = new string[2] { timing.PrefabName, null };
			float time = timing.Time;
			obj[1] = time.ToString();
			val.AddRow(obj);
		}
		val.AddRow(new string[2]
		{
			"Total time",
			(num2 * 1000f).ToString()
		});
		Debug.Log((object)((object)val).ToString());
	}

	public virtual void ResetIOState()
	{
	}

	public virtual void Init()
	{
		for (int i = 0; i < outputs.Length; i++)
		{
			IOSlot iOSlot = outputs[i];
			iOSlot.connectedTo.Init();
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
			{
				int connectedToSlot = iOSlot.connectedToSlot;
				if (connectedToSlot < 0 || connectedToSlot >= iOSlot.connectedTo.Get().inputs.Length)
				{
					Debug.LogError((object)("Slot IOR Error: " + ((Object)this).name + " setting up inputs for " + ((Object)iOSlot.connectedTo.Get()).name + " slot : " + iOSlot.connectedToSlot));
				}
				else
				{
					iOSlot.connectedTo.Get().inputs[iOSlot.connectedToSlot].connectedTo.Set(this);
					iOSlot.connectedTo.Get().inputs[iOSlot.connectedToSlot].connectedToSlot = i;
					iOSlot.connectedTo.Get().inputs[iOSlot.connectedToSlot].connectedTo.Init();
				}
			}
		}
		UpdateUsedOutputs();
		if (IsRootEntity())
		{
			((FacepunchBehaviour)this).Invoke((Action)MarkDirtyForceUpdateOutputs, Random.Range(1f, 1f));
		}
	}

	internal override void DoServerDestroy()
	{
		if (base.isServer)
		{
			Shutdown();
		}
		base.DoServerDestroy();
	}

	public void ClearConnections()
	{
		List<IOEntity> list = Pool.GetList<IOEntity>();
		List<IOEntity> list2 = Pool.GetList<IOEntity>();
		IOSlot[] array = inputs;
		foreach (IOSlot iOSlot in array)
		{
			IOEntity iOEntity = null;
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
			{
				iOEntity = iOSlot.connectedTo.Get();
				if (iOSlot.type == IOType.Industrial)
				{
					list2.Add(iOEntity);
				}
				IOSlot[] array2 = iOSlot.connectedTo.Get().outputs;
				foreach (IOSlot iOSlot2 in array2)
				{
					if ((Object)(object)iOSlot2.connectedTo.Get() != (Object)null && iOSlot2.connectedTo.Get().EqualNetID((BaseNetworkable)this))
					{
						iOSlot2.Clear();
					}
				}
			}
			iOSlot.Clear();
			if (Object.op_Implicit((Object)(object)iOEntity))
			{
				iOEntity.SendNetworkUpdate();
			}
		}
		array = outputs;
		foreach (IOSlot iOSlot3 in array)
		{
			if ((Object)(object)iOSlot3.connectedTo.Get() != (Object)null)
			{
				list.Add(iOSlot3.connectedTo.Get());
				if (iOSlot3.type == IOType.Industrial)
				{
					list2.Add(list[list.Count - 1]);
				}
				IOSlot[] array2 = iOSlot3.connectedTo.Get().inputs;
				foreach (IOSlot iOSlot4 in array2)
				{
					if ((Object)(object)iOSlot4.connectedTo.Get() != (Object)null && iOSlot4.connectedTo.Get().EqualNetID((BaseNetworkable)this))
					{
						iOSlot4.Clear();
					}
				}
			}
			if (Object.op_Implicit((Object)(object)iOSlot3.connectedTo.Get()))
			{
				iOSlot3.connectedTo.Get().UpdateFromInput(0, iOSlot3.connectedToSlot);
			}
			iOSlot3.Clear();
		}
		SendNetworkUpdate();
		foreach (IOEntity item in list)
		{
			if ((Object)(object)item != (Object)null)
			{
				item.MarkDirty();
				item.SendNetworkUpdate();
			}
		}
		for (int k = 0; k < inputs.Length; k++)
		{
			UpdateFromInput(0, k);
		}
		foreach (IOEntity item2 in list2)
		{
			if ((Object)(object)item2 != (Object)null)
			{
				item2.NotifyIndustrialNetworkChanged();
			}
			item2.RefreshIndustrialPreventBuilding();
		}
		Pool.FreeList<IOEntity>(ref list);
		Pool.FreeList<IOEntity>(ref list2);
		RefreshIndustrialPreventBuilding();
	}

	public void Shutdown()
	{
		SendChangedToRoot(forceUpdate: true);
		ClearConnections();
	}

	public void MarkDirtyForceUpdateOutputs()
	{
		ensureOutputsUpdated = true;
		MarkDirty();
	}

	public void UpdateUsedOutputs()
	{
		cachedOutputsUsed = 0;
		IOSlot[] array = outputs;
		for (int i = 0; i < array.Length; i++)
		{
			IOEntity iOEntity = array[i].connectedTo.Get();
			if ((Object)(object)iOEntity != (Object)null && !iOEntity.IsDestroyed)
			{
				cachedOutputsUsed++;
			}
		}
	}

	public virtual void MarkDirty()
	{
		if (!base.isClient)
		{
			UpdateUsedOutputs();
			TouchIOState();
		}
	}

	public virtual int DesiredPower()
	{
		return ConsumptionAmount();
	}

	public virtual int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		return inputAmount;
	}

	public virtual int GetCurrentEnergy()
	{
		return Mathf.Clamp(currentEnergy - ConsumptionAmount(), 0, currentEnergy);
	}

	public virtual int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot < 0 || outputSlot >= outputs.Length)
		{
			return 0;
		}
		IOEntity iOEntity = outputs[outputSlot].connectedTo.Get();
		if ((Object)(object)iOEntity == (Object)null || iOEntity.IsDestroyed)
		{
			return 0;
		}
		int num = ((cachedOutputsUsed == 0) ? 1 : cachedOutputsUsed);
		return GetCurrentEnergy() / num;
	}

	public virtual void UpdateHasPower(int inputAmount, int inputSlot)
	{
		SetFlag(Flags.Reserved8, inputAmount >= ConsumptionAmount() && inputAmount > 0, recursive: false, networkupdate: false);
	}

	public void TouchInternal()
	{
		int passthroughAmount = GetPassthroughAmount();
		bool num = lastPassthroughEnergy != passthroughAmount;
		lastPassthroughEnergy = passthroughAmount;
		if (num)
		{
			IOStateChanged(currentEnergy, 0);
			ensureOutputsUpdated = true;
		}
		_processQueue.Enqueue(this);
	}

	public virtual void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputs[inputSlot].type != ioType || inputs[inputSlot].type == IOType.Industrial)
		{
			IOStateChanged(inputAmount, inputSlot);
			return;
		}
		UpdateHasPower(inputAmount, inputSlot);
		lastEnergy = currentEnergy;
		currentEnergy = CalculateCurrentEnergy(inputAmount, inputSlot);
		int passthroughAmount = GetPassthroughAmount();
		bool flag = lastPassthroughEnergy != passthroughAmount;
		lastPassthroughEnergy = passthroughAmount;
		if (currentEnergy != lastEnergy || flag)
		{
			IOStateChanged(inputAmount, inputSlot);
			ensureOutputsUpdated = true;
		}
		_processQueue.Enqueue(this);
	}

	public virtual void TouchIOState()
	{
		if (!base.isClient)
		{
			TouchInternal();
		}
	}

	public virtual void SendIONetworkUpdate()
	{
		SendNetworkUpdate_Flags();
	}

	public virtual void IOStateChanged(int inputAmount, int inputSlot)
	{
	}

	public virtual void OnCircuitChanged(bool forceUpdate)
	{
		if (forceUpdate)
		{
			MarkDirtyForceUpdateOutputs();
		}
	}

	public virtual void SendChangedToRoot(bool forceUpdate)
	{
		List<IOEntity> existing = Pool.GetList<IOEntity>();
		SendChangedToRootRecursive(forceUpdate, ref existing);
		Pool.FreeList<IOEntity>(ref existing);
	}

	public virtual void SendChangedToRootRecursive(bool forceUpdate, ref List<IOEntity> existing)
	{
		bool flag = IsRootEntity();
		if (existing.Contains(this))
		{
			return;
		}
		existing.Add(this);
		bool flag2 = false;
		for (int i = 0; i < inputs.Length; i++)
		{
			IOSlot iOSlot = inputs[i];
			if (!iOSlot.mainPowerSlot)
			{
				continue;
			}
			IOEntity iOEntity = iOSlot.connectedTo.Get();
			if (!((Object)(object)iOEntity == (Object)null) && !existing.Contains(iOEntity))
			{
				flag2 = true;
				if (forceUpdate)
				{
					iOEntity.ensureOutputsUpdated = true;
				}
				iOEntity.SendChangedToRootRecursive(forceUpdate, ref existing);
			}
		}
		if (flag)
		{
			forceUpdate = forceUpdate && !flag2;
			OnCircuitChanged(forceUpdate);
		}
	}

	public void NotifyIndustrialNetworkChanged()
	{
		List<IOEntity> list = Pool.GetList<IOEntity>();
		OnIndustrialNetworkChanged();
		NotifyIndustrialNetworkChanged(list, input: true, 128);
		list.Clear();
		NotifyIndustrialNetworkChanged(list, input: false, 128);
		Pool.FreeList<IOEntity>(ref list);
	}

	private void NotifyIndustrialNetworkChanged(List<IOEntity> existing, bool input, int maxDepth)
	{
		if (maxDepth <= 0 || existing.Contains(this))
		{
			return;
		}
		if (existing.Count != 0)
		{
			OnIndustrialNetworkChanged();
		}
		existing.Add(this);
		IOSlot[] array = (input ? inputs : outputs);
		foreach (IOSlot iOSlot in array)
		{
			if (iOSlot.type == IOType.Industrial && (Object)(object)iOSlot.connectedTo.Get() != (Object)null)
			{
				iOSlot.connectedTo.Get().NotifyIndustrialNetworkChanged(existing, input, maxDepth - 1);
			}
		}
	}

	protected virtual void OnIndustrialNetworkChanged()
	{
	}

	protected bool ShouldUpdateOutputs()
	{
		if (Time.realtimeSinceStartup - lastUpdateTime < responsetime)
		{
			lastUpdateBlockedFrame = Time.frameCount;
			_processQueue.Enqueue(this);
			return false;
		}
		lastUpdateTime = Time.realtimeSinceStartup;
		SendIONetworkUpdate();
		if (outputs.Length == 0)
		{
			ensureOutputsUpdated = false;
			return false;
		}
		return true;
	}

	public virtual void UpdateOutputs()
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!ShouldUpdateOutputs() || !ensureOutputsUpdated)
		{
			return;
		}
		ensureOutputsUpdated = false;
		TimeWarning val = TimeWarning.New("ProcessIOOutputs", 0);
		try
		{
			for (int i = 0; i < outputs.Length; i++)
			{
				IOSlot iOSlot = outputs[i];
				bool flag = true;
				IOEntity iOEntity = iOSlot.connectedTo.Get();
				if (!((Object)(object)iOEntity != (Object)null))
				{
					continue;
				}
				if (ioType == IOType.Fluidic && !DisregardGravityRestrictionsOnLiquid && !iOEntity.DisregardGravityRestrictionsOnLiquid)
				{
					TimeWarning val2 = TimeWarning.New("FluidOutputProcessing", 0);
					try
					{
						if (!iOEntity.AllowLiquidPassthrough(this, ((Component)this).transform.TransformPoint(iOSlot.handlePosition)))
						{
							flag = false;
						}
					}
					finally
					{
						((IDisposable)val2)?.Dispose();
					}
				}
				int passthroughAmount = GetPassthroughAmount(i);
				iOEntity.UpdateFromInput(flag ? passthroughAmount : 0, iOSlot.connectedToSlot);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		if (!Application.isLoadingSave)
		{
			Init();
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		Init();
	}

	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		Init();
	}

	public override void Save(SaveInfo info)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.ioEntity = Pool.Get<IOEntity>();
		info.msg.ioEntity.inputs = Pool.GetList<IOConnection>();
		info.msg.ioEntity.outputs = Pool.GetList<IOConnection>();
		IOSlot[] array = inputs;
		foreach (IOSlot iOSlot in array)
		{
			IOConnection val = Pool.Get<IOConnection>();
			val.connectedID = iOSlot.connectedTo.entityRef.uid;
			val.connectedToSlot = iOSlot.connectedToSlot;
			val.niceName = iOSlot.niceName;
			val.type = (int)iOSlot.type;
			val.inUse = ((NetworkableId)(ref val.connectedID)).IsValid;
			val.colour = (int)iOSlot.wireColour;
			val.lineThickness = iOSlot.lineThickness;
			info.msg.ioEntity.inputs.Add(val);
		}
		array = outputs;
		foreach (IOSlot iOSlot2 in array)
		{
			IOConnection val2 = Pool.Get<IOConnection>();
			val2.connectedID = iOSlot2.connectedTo.entityRef.uid;
			val2.connectedToSlot = iOSlot2.connectedToSlot;
			val2.niceName = iOSlot2.niceName;
			val2.type = (int)iOSlot2.type;
			val2.inUse = ((NetworkableId)(ref val2.connectedID)).IsValid;
			val2.colour = (int)iOSlot2.wireColour;
			val2.worldSpaceRotation = iOSlot2.worldSpaceLineEndRotation;
			val2.lineThickness = iOSlot2.lineThickness;
			if (iOSlot2.linePoints != null)
			{
				val2.linePointList = Pool.GetList<LineVec>();
				val2.linePointList.Clear();
				for (int j = 0; j < iOSlot2.linePoints.Length; j++)
				{
					Vector3 val3 = iOSlot2.linePoints[j];
					LineVec val4 = Pool.Get<LineVec>();
					val4.vec = Vector4.op_Implicit(val3);
					if (iOSlot2.slackLevels.Length > j)
					{
						val4.vec.w = iOSlot2.slackLevels[j];
					}
					val2.linePointList.Add(val4);
				}
			}
			info.msg.ioEntity.outputs.Add(val2);
		}
	}

	public virtual float IOInput(IOEntity from, IOType inputType, float inputAmount, int slot = 0)
	{
		IOSlot[] array = outputs;
		foreach (IOSlot iOSlot in array)
		{
			if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
			{
				inputAmount = iOSlot.connectedTo.Get().IOInput(this, iOSlot.type, inputAmount, iOSlot.connectedToSlot);
			}
		}
		return inputAmount;
	}

	public void FindContainerSource(List<ContainerInputOutput> found, int depth, bool input, List<IOEntity> ignoreList, int parentId = -1, int stackSize = 0)
	{
		if (depth <= 0 || found.Count >= 32)
		{
			return;
		}
		int num = 0;
		int num2 = 1;
		IOSlot[] array;
		if (!input)
		{
			num2 = 0;
			array = outputs;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].type == IOType.Industrial)
				{
					num2++;
				}
			}
		}
		List<int> list = Pool.GetList<int>();
		array = (input ? inputs : outputs);
		foreach (IOSlot iOSlot in array)
		{
			num++;
			if (iOSlot.type != IOType.Industrial)
			{
				continue;
			}
			IOEntity iOEntity = iOSlot.connectedTo.Get(base.isServer);
			if (!((Object)(object)iOEntity != (Object)null) || ignoreList.Contains(iOEntity))
			{
				continue;
			}
			int num3 = -1;
			if (iOEntity is IIndustrialStorage storage2)
			{
				num = iOSlot.connectedToSlot;
				if (GetExistingCount(storage2) < 2)
				{
					found.Add(new ContainerInputOutput
					{
						SlotIndex = num,
						Storage = storage2,
						ParentStorage = parentId,
						MaxStackSize = stackSize / num2
					});
					num3 = found.Count - 1;
					list.Add(num3);
				}
			}
			else
			{
				ignoreList.Add(iOEntity);
			}
			if ((!(iOEntity is IIndustrialStorage) || iOEntity is IndustrialStorageAdaptor) && !(iOEntity is IndustrialConveyor) && (Object)(object)iOEntity != (Object)null)
			{
				iOEntity.FindContainerSource(found, depth - 1, input, ignoreList, (num3 == -1) ? parentId : num3, stackSize / num2);
			}
		}
		int count = list.Count;
		foreach (int item in list)
		{
			ContainerInputOutput value = found[item];
			value.IndustrialSiblingCount = count;
			found[item] = value;
		}
		Pool.FreeList<int>(ref list);
		int GetExistingCount(IIndustrialStorage storage)
		{
			int num4 = 0;
			foreach (ContainerInputOutput item2 in found)
			{
				if (item2.Storage == storage)
				{
					num4++;
				}
			}
			return num4;
		}
	}

	public virtual bool AllowLiquidPassthrough(IOEntity fromSource, Vector3 sourceWorldPosition, bool forPlacement = false)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (fromSource.DisregardGravityRestrictionsOnLiquid || DisregardGravityRestrictionsOnLiquid)
		{
			return true;
		}
		if (inputs.Length == 0)
		{
			return false;
		}
		Vector3 val = ((Component)this).transform.TransformPoint(inputs[0].handlePosition);
		float num = sourceWorldPosition.y - val.y;
		if (num > 0f)
		{
			return true;
		}
		if (Mathf.Abs(num) < LiquidPassthroughGravityThreshold)
		{
			return true;
		}
		return false;
	}

	public override void Load(LoadInfo info)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.ioEntity == null)
		{
			return;
		}
		if (!info.fromDisk && info.msg.ioEntity.inputs != null)
		{
			int count = info.msg.ioEntity.inputs.Count;
			if (inputs.Length != count)
			{
				inputs = new IOSlot[count];
			}
			for (int i = 0; i < count; i++)
			{
				if (inputs[i] == null)
				{
					inputs[i] = new IOSlot();
				}
				IOConnection val = info.msg.ioEntity.inputs[i];
				inputs[i].connectedTo = new IORef();
				inputs[i].connectedTo.entityRef.uid = val.connectedID;
				if (base.isClient)
				{
					inputs[i].connectedTo.InitClient();
				}
				inputs[i].connectedToSlot = val.connectedToSlot;
				inputs[i].niceName = val.niceName;
				inputs[i].type = (IOType)val.type;
				inputs[i].wireColour = (WireTool.WireColour)val.colour;
				inputs[i].lineThickness = val.lineThickness;
			}
		}
		if (info.msg.ioEntity.outputs != null)
		{
			int count2 = info.msg.ioEntity.outputs.Count;
			IOSlot[] array = null;
			if (outputs.Length != count2 && count2 > 0)
			{
				array = outputs;
				outputs = new IOSlot[count2];
				for (int j = 0; j < array.Length; j++)
				{
					if (j < count2)
					{
						outputs[j] = array[j];
					}
				}
			}
			for (int k = 0; k < count2; k++)
			{
				if (outputs[k] == null)
				{
					outputs[k] = new IOSlot();
				}
				IOConnection val2 = info.msg.ioEntity.outputs[k];
				if (val2.linePointList == null || val2.linePointList.Count == 0 || !((NetworkableId)(ref val2.connectedID)).IsValid)
				{
					outputs[k].Clear();
				}
				outputs[k].connectedTo = new IORef();
				outputs[k].connectedTo.entityRef.uid = val2.connectedID;
				if (base.isClient)
				{
					outputs[k].connectedTo.InitClient();
				}
				outputs[k].connectedToSlot = val2.connectedToSlot;
				outputs[k].niceName = val2.niceName;
				outputs[k].type = (IOType)val2.type;
				outputs[k].wireColour = (WireTool.WireColour)val2.colour;
				outputs[k].worldSpaceLineEndRotation = val2.worldSpaceRotation;
				outputs[k].lineThickness = val2.lineThickness;
				if (info.fromDisk || base.isClient)
				{
					List<LineVec> linePointList = val2.linePointList;
					if (outputs[k].linePoints == null || outputs[k].linePoints.Length != linePointList.Count)
					{
						outputs[k].linePoints = (Vector3[])(object)new Vector3[linePointList.Count];
					}
					if (outputs[k].slackLevels == null || outputs[k].slackLevels.Length != linePointList.Count)
					{
						outputs[k].slackLevels = new float[linePointList.Count];
					}
					for (int l = 0; l < linePointList.Count; l++)
					{
						outputs[k].linePoints[l] = Vector4.op_Implicit(linePointList[l].vec);
						outputs[k].slackLevels[l] = linePointList[l].vec.w;
					}
				}
			}
		}
		RefreshIndustrialPreventBuilding();
	}

	public int GetConnectedInputCount()
	{
		int num = 0;
		IOSlot[] array = inputs;
		for (int i = 0; i < array.Length; i++)
		{
			if ((Object)(object)array[i].connectedTo.Get(base.isServer) != (Object)null)
			{
				num++;
			}
		}
		return num;
	}

	public int GetConnectedOutputCount()
	{
		int num = 0;
		IOSlot[] array = outputs;
		for (int i = 0; i < array.Length; i++)
		{
			if ((Object)(object)array[i].connectedTo.Get(base.isServer) != (Object)null)
			{
				num++;
			}
		}
		return num;
	}

	public bool HasConnections()
	{
		if (GetConnectedInputCount() <= 0)
		{
			return GetConnectedOutputCount() > 0;
		}
		return true;
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		ClearIndustrialPreventBuilding();
	}

	public void RefreshIndustrialPreventBuilding()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		ClearIndustrialPreventBuilding();
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		BoxCollider val4 = default(BoxCollider);
		ColliderInfo_Pipe colliderInfo_Pipe = default(ColliderInfo_Pipe);
		for (int i = 0; i < outputs.Length; i++)
		{
			IOSlot iOSlot = outputs[i];
			if (iOSlot.type != IOType.Industrial || iOSlot.linePoints == null || iOSlot.linePoints.Length <= 1)
			{
				continue;
			}
			Vector3 val = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(iOSlot.linePoints[0]);
			for (int j = 1; j < iOSlot.linePoints.Length; j++)
			{
				Vector3 val2 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(iOSlot.linePoints[j]);
				Vector3 pos = Vector3.Lerp(val2, val, 0.5f);
				float num = Vector3.Distance(val2, val);
				Vector3 val3 = val2 - val;
				Quaternion rot = Quaternion.LookRotation(((Vector3)(ref val3)).normalized);
				GameObject obj = base.gameManager.CreatePrefab("assets/prefabs/misc/ioentitypreventbuilding.prefab", pos, rot);
				obj.transform.SetParent(((Component)this).transform);
				if (obj.TryGetComponent<BoxCollider>(ref val4))
				{
					val4.size = new Vector3(0.1f, 0.1f, num);
					spawnedColliders.Add(val4);
				}
				if (obj.TryGetComponent<ColliderInfo_Pipe>(ref colliderInfo_Pipe))
				{
					colliderInfo_Pipe.OutputSlotIndex = i;
					colliderInfo_Pipe.ParentEntity = this;
				}
				val = val2;
			}
		}
	}

	private void ClearIndustrialPreventBuilding()
	{
		foreach (BoxCollider spawnedCollider in spawnedColliders)
		{
			base.gameManager.Retire(((Component)spawnedCollider).gameObject);
		}
		spawnedColliders.Clear();
	}
}
