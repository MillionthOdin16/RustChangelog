using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

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

		public bool rootConnectionsOnly = false;

		public bool mainPowerSlot = false;

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

	private int cachedOutputsUsed = 0;

	protected int lastPassthroughEnergy = 0;

	private int lastEnergy = 0;

	protected int currentEnergy = 0;

	protected float lastUpdateTime = 0f;

	protected int lastUpdateBlockedFrame = 0;

	protected bool ensureOutputsUpdated = false;

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
						TimeWarning val4 = TimeWarning.New("Call", 0);
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
							((IDisposable)val4)?.Dispose();
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
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (depth <= 0)
		{
			return null;
		}
		if (!ignoreSelf && IsGravitySource)
		{
			Profiler.BeginSample("Transform Point");
			worldHandlePosition = ((Component)this).transform.TransformPoint(outputs[0].handlePosition);
			Profiler.EndSample();
			return this;
		}
		IOSlot[] array = inputs;
		foreach (IOSlot iOSlot in array)
		{
			IOEntity iOEntity = iOSlot.connectedTo.Get(base.isServer);
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
			IOSlot iOSlot = inputs[slot];
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
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		Profiler.BeginSample("IOEntity.ProcessQueue");
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
				Profiler.BeginSample(iOEntity.ShortPrefabName);
				iOEntity.UpdateOutputs();
				Profiler.EndSample();
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
		Profiler.EndSample();
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
		IOSlot[] array3 = outputs;
		foreach (IOSlot iOSlot3 in array3)
		{
			if ((Object)(object)iOSlot3.connectedTo.Get() != (Object)null)
			{
				list.Add(iOSlot3.connectedTo.Get());
				if (iOSlot3.type == IOType.Industrial)
				{
					list2.Add(list[list.Count - 1]);
				}
				IOSlot[] array4 = iOSlot3.connectedTo.Get().inputs;
				foreach (IOSlot iOSlot4 in array4)
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
		for (int m = 0; m < inputs.Length; m++)
		{
			UpdateFromInput(0, m);
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
		Profiler.BeginSample("IOEntity.UpdateUsedOutputs");
		cachedOutputsUsed = 0;
		IOSlot[] array = outputs;
		foreach (IOSlot iOSlot in array)
		{
			IOEntity iOEntity = iOSlot.connectedTo.Get();
			if ((Object)(object)iOEntity != (Object)null && !iOEntity.IsDestroyed)
			{
				cachedOutputsUsed++;
			}
		}
		Profiler.EndSample();
	}

	public virtual void MarkDirty()
	{
		if (!base.isClient)
		{
			Profiler.BeginSample("IOEntity.MarkDirty");
			UpdateUsedOutputs();
			TouchIOState();
			Profiler.EndSample();
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
		bool flag = lastPassthroughEnergy != passthroughAmount;
		lastPassthroughEnergy = passthroughAmount;
		if (flag)
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
		Profiler.BeginSample("IOEntity.UpdateFromInput");
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
		Profiler.EndSample();
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
		Profiler.BeginSample("IOEntity.SendChangedToRoot");
		List<IOEntity> existing = Pool.GetList<IOEntity>();
		SendChangedToRootRecursive(forceUpdate, ref existing);
		Pool.FreeList<IOEntity>(ref existing);
		Profiler.EndSample();
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
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("IOEntity.Save");
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
		IOSlot[] array2 = outputs;
		foreach (IOSlot iOSlot2 in array2)
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
				for (int k = 0; k < iOSlot2.linePoints.Length; k++)
				{
					Vector3 val3 = iOSlot2.linePoints[k];
					LineVec val4 = Pool.Get<LineVec>();
					val4.vec = Vector4.op_Implicit(val3);
					if (iOSlot2.slackLevels.Length > k)
					{
						val4.vec.w = iOSlot2.slackLevels[k];
					}
					val2.linePointList.Add(val4);
				}
			}
			info.msg.ioEntity.outputs.Add(val2);
		}
		Profiler.EndSample();
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
		if (!input)
		{
			num2 = 0;
			IOSlot[] array = outputs;
			foreach (IOSlot iOSlot in array)
			{
				if (iOSlot.type == IOType.Industrial)
				{
					num2++;
				}
			}
		}
		List<int> list = Pool.GetList<int>();
		IOSlot[] array2 = (input ? inputs : outputs);
		foreach (IOSlot iOSlot2 in array2)
		{
			num++;
			if (iOSlot2.type != IOType.Industrial)
			{
				continue;
			}
			IOEntity iOEntity = iOSlot2.connectedTo.Get(base.isServer);
			if (!((Object)(object)iOEntity != (Object)null) || ignoreList.Contains(iOEntity))
			{
				continue;
			}
			int num3 = -1;
			if (iOEntity is IIndustrialStorage storage2)
			{
				num = iOSlot2.connectedToSlot;
				int num4 = GetExistingCount(storage2);
				if (num4 < 2)
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
			int num5 = 0;
			foreach (ContainerInputOutput item2 in found)
			{
				if (item2.Storage == storage)
				{
					num5++;
				}
			}
			return num5;
		}
	}

	public virtual bool AllowLiquidPassthrough(IOEntity fromSource, Vector3 sourceWorldPosition, bool forPlacement = false)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.ioEntity == null)
		{
			return;
		}
		Profiler.BeginSample("IOEntity.Load");
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
		Profiler.EndSample();
	}

	public int GetConnectedInputCount()
	{
		int num = 0;
		IOSlot[] array = inputs;
		foreach (IOSlot iOSlot in array)
		{
			if ((Object)(object)iOSlot.connectedTo.Get(base.isServer) != (Object)null)
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
		foreach (IOSlot iOSlot in array)
		{
			if ((Object)(object)iOSlot.connectedTo.Get(base.isServer) != (Object)null)
			{
				num++;
			}
		}
		return num;
	}

	public bool HasConnections()
	{
		return GetConnectedInputCount() > 0 || GetConnectedOutputCount() > 0;
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		ClearIndustrialPreventBuilding();
	}

	public void RefreshIndustrialPreventBuilding()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("RefreshIndustrialPreventBuilding");
		ClearIndustrialPreventBuilding();
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		BoxCollider val5 = default(BoxCollider);
		ColliderInfo_Pipe colliderInfo_Pipe = default(ColliderInfo_Pipe);
		for (int i = 0; i < outputs.Length; i++)
		{
			IOSlot iOSlot = outputs[i];
			Profiler.BeginSample("CreateOutput");
			if (iOSlot.type == IOType.Industrial && iOSlot.linePoints != null && iOSlot.linePoints.Length > 1)
			{
				Vector3 val = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(iOSlot.linePoints[0]);
				for (int j = 1; j < iOSlot.linePoints.Length; j++)
				{
					Vector3 val2 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(iOSlot.linePoints[j]);
					Vector3 pos = Vector3.Lerp(val2, val, 0.5f);
					float num = Vector3.Distance(val2, val);
					Vector3 val3 = val2 - val;
					Quaternion rot = Quaternion.LookRotation(((Vector3)(ref val3)).normalized);
					GameObject val4 = base.gameManager.CreatePrefab("assets/prefabs/misc/ioentitypreventbuilding.prefab", pos, rot);
					val4.transform.SetParent(((Component)this).transform);
					if (val4.TryGetComponent<BoxCollider>(ref val5))
					{
						val5.size = new Vector3(0.1f, 0.1f, num);
						spawnedColliders.Add(val5);
					}
					if (val4.TryGetComponent<ColliderInfo_Pipe>(ref colliderInfo_Pipe))
					{
						colliderInfo_Pipe.OutputSlotIndex = i;
						colliderInfo_Pipe.ParentEntity = this;
					}
					val = val2;
				}
			}
			Profiler.EndSample();
		}
		Profiler.EndSample();
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
