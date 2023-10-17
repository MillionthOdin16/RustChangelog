using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class SlotMachine : BaseMountable
{
	public enum SlotFaces
	{
		Scrap,
		Rope,
		Apple,
		LowGrade,
		Wood,
		Bandage,
		Charcoal,
		Gunpowder,
		Rust,
		Meat,
		Hammer,
		Sulfur,
		TechScrap,
		Frags,
		Cloth,
		LuckySeven
	}

	[ServerVar]
	public static int ForcePayoutIndex = -1;

	[Header("Slot Machine")]
	public Transform Reel1;

	public Transform Reel2;

	public Transform Reel3;

	public Transform Arm;

	public AnimationCurve Curve;

	public int Reel1Spins = 16;

	public int Reel2Spins = 48;

	public int Reel3Spins = 80;

	public int MaxReelSpins = 96;

	public float SpinDuration = 2f;

	private int SpinResult1;

	private int SpinResult2;

	private int SpinResult3;

	private int SpinResultPrevious1;

	private int SpinResultPrevious2;

	private int SpinResultPrevious3;

	private float SpinTime;

	public GameObjectRef StoragePrefab;

	public EntityRef StorageInstance;

	public SoundDefinition SpinSound;

	public SlotMachinePayoutDisplay PayoutDisplay;

	public SlotMachinePayoutSettings PayoutSettings;

	public Transform HandIkTarget;

	private const Flags HasScrapForSpin = Flags.Reserved1;

	private const Flags IsSpinningFlag = Flags.Reserved2;

	public Material PayoutIconMaterial;

	public bool UseTimeOfDayAdjustedSprite = true;

	public MeshRenderer[] PulseRenderers;

	public float PulseSpeed = 5f;

	[ColorUsage(true, true)]
	public Color PulseFrom;

	[ColorUsage(true, true)]
	public Color PulseTo;

	private BasePlayer CurrentSpinPlayer;

	private bool IsSpinning => HasFlag(Flags.Reserved2);

	public int CurrentMultiplier { get; private set; } = 1;


	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SlotMachine.OnRpcMessage", 0);
		try
		{
			if (rpc == 1251063754 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Deposit "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Deposit", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1251063754u, "RPC_Deposit", this, player, 3f))
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
							RPCMessage rpc2 = rPCMessage;
							RPC_Deposit(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Deposit");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1455840454 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Spin "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Spin", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1455840454u, "RPC_Spin", this, player, 3f))
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
							RPCMessage rpc3 = rPCMessage;
							RPC_Spin(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Spin");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3942337446u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_RequestMultiplierChange "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RequestMultiplierChange", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3942337446u, "Server_RequestMultiplierChange", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3942337446u, "Server_RequestMultiplierChange", this, player, 3f))
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
							Server_RequestMultiplierChange(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in Server_RequestMultiplierChange");
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

	public override void Save(SaveInfo info)
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.slotMachine = Pool.Get<SlotMachine>();
		info.msg.slotMachine.oldResult1 = SpinResultPrevious1;
		info.msg.slotMachine.oldResult2 = SpinResultPrevious2;
		info.msg.slotMachine.oldResult3 = SpinResultPrevious3;
		info.msg.slotMachine.newResult1 = SpinResult1;
		info.msg.slotMachine.newResult2 = SpinResult2;
		info.msg.slotMachine.newResult3 = SpinResult3;
		info.msg.slotMachine.isSpinning = IsSpinning;
		info.msg.slotMachine.spinTime = SpinTime;
		info.msg.slotMachine.storageID = StorageInstance.uid;
		info.msg.slotMachine.multiplier = CurrentMultiplier;
	}

	public override void Load(LoadInfo info)
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.slotMachine != null)
		{
			SpinResultPrevious1 = info.msg.slotMachine.oldResult1;
			SpinResultPrevious2 = info.msg.slotMachine.oldResult2;
			SpinResultPrevious3 = info.msg.slotMachine.oldResult3;
			SpinResult1 = info.msg.slotMachine.newResult1;
			SpinResult2 = info.msg.slotMachine.newResult2;
			SpinResult3 = info.msg.slotMachine.newResult3;
			CurrentMultiplier = info.msg.slotMachine.multiplier;
			if (base.isServer)
			{
				SpinTime = info.msg.slotMachine.spinTime;
			}
			StorageInstance.uid = info.msg.slotMachine.storageID;
			if (info.fromDisk && base.isServer)
			{
				SetFlag(Flags.Reserved2, b: false);
			}
		}
	}

	public override float GetComfort()
	{
		return 1f;
	}

	public override void Spawn()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		base.Spawn();
		if (!Application.isLoadingSave)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(StoragePrefab.resourcePath);
			baseEntity.Spawn();
			baseEntity.SetParent(this);
			StorageInstance.Set(baseEntity);
		}
	}

	internal override void DoServerDestroy()
	{
		SlotMachineStorage slotMachineStorage = StorageInstance.Get(base.isServer) as SlotMachineStorage;
		if (slotMachineStorage.IsValid())
		{
			slotMachineStorage.DropItems();
		}
		base.DoServerDestroy();
	}

	private int GetBettingAmount()
	{
		SlotMachineStorage component = ((Component)StorageInstance.Get(base.isServer)).GetComponent<SlotMachineStorage>();
		if ((Object)(object)component == (Object)null)
		{
			return 0;
		}
		return component.inventory.GetSlot(0)?.amount ?? 0;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_Spin(RPCMessage rpc)
	{
		if (IsSpinning || (Object)(object)rpc.player != (Object)(object)GetMounted())
		{
			return;
		}
		SlotMachineStorage component = ((Component)StorageInstance.Get(base.isServer)).GetComponent<SlotMachineStorage>();
		int num = (int)PayoutSettings.SpinCost.amount * CurrentMultiplier;
		if (GetBettingAmount() < num || (Object)(object)rpc.player == (Object)null)
		{
			return;
		}
		(CurrentSpinPlayer = rpc.player).inventory.loot.Clear();
		Item slot = component.inventory.GetSlot(0);
		int amount = 0;
		if (slot != null)
		{
			if (slot.amount > num)
			{
				slot.MarkDirty();
				slot.amount -= num;
				amount = slot.amount;
			}
			else
			{
				slot.amount -= num;
				slot.RemoveFromContainer();
			}
		}
		component.UpdateAmount(amount);
		SetFlag(Flags.Reserved2, b: true);
		SpinResultPrevious1 = SpinResult1;
		SpinResultPrevious2 = SpinResult2;
		SpinResultPrevious3 = SpinResult3;
		CalculateSpinResults();
		SpinTime = Time.time;
		ClientRPC(null, "RPC_OnSpin", (sbyte)SpinResult1, (sbyte)SpinResult2, (sbyte)SpinResult3);
		((FacepunchBehaviour)this).Invoke((Action)CheckPayout, SpinDuration);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_Deposit(RPCMessage rpc)
	{
		BasePlayer player = rpc.player;
		if (!((Object)(object)player == (Object)null) && !HasFlag(Flags.Reserved2) && StorageInstance.IsValid(base.isServer))
		{
			((Component)StorageInstance.Get(base.isServer)).GetComponent<StorageContainer>().PlayerOpenLoot(player, "", doPositionChecks: false);
		}
	}

	private void CheckPayout()
	{
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if ((Object)(object)PayoutSettings != (Object)null)
		{
			if (CalculatePayout(out var info, out var bonus))
			{
				int num = ((int)info.Item.amount + bonus) * CurrentMultiplier;
				BaseEntity baseEntity = StorageInstance.Get(serverside: true);
				if ((Object)(object)baseEntity != (Object)null && baseEntity is SlotMachineStorage slotMachineStorage)
				{
					Item slot = slotMachineStorage.inventory.GetSlot(1);
					if (slot != null)
					{
						slot.amount += num;
						slot.MarkDirty();
					}
					else
					{
						ItemManager.Create(info.Item.itemDef, num, 0uL).MoveToContainer(slotMachineStorage.inventory, 1);
					}
				}
				if (CurrentSpinPlayer.IsValid() && (Object)(object)CurrentSpinPlayer == (Object)(object)_mounted)
				{
					CurrentSpinPlayer.ChatMessage($"You received {num}x {info.Item.itemDef.displayName.english} for slots payout!");
				}
				Analytics.Server.SlotMachineTransaction((int)PayoutSettings.SpinCost.amount * CurrentMultiplier, num);
				Analytics.Azure.OnGamblingResult(CurrentSpinPlayer, this, (int)PayoutSettings.SpinCost.amount, num);
				if (info.OverrideWinEffect != null && info.OverrideWinEffect.isValid)
				{
					Effect.server.Run(info.OverrideWinEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
				}
				else if (PayoutSettings.DefaultWinEffect != null && PayoutSettings.DefaultWinEffect.isValid)
				{
					Effect.server.Run(PayoutSettings.DefaultWinEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
				}
				if (info.OverrideWinEffect != null && info.OverrideWinEffect.isValid)
				{
					flag = true;
				}
			}
			else
			{
				Analytics.Server.SlotMachineTransaction((int)PayoutSettings.SpinCost.amount * CurrentMultiplier, 0);
				Analytics.Azure.OnGamblingResult(CurrentSpinPlayer, this, (int)PayoutSettings.SpinCost.amount * CurrentMultiplier, 0);
			}
		}
		else
		{
			Debug.LogError((object)$"Failed to process spin results: PayoutSettings != null {(Object)(object)PayoutSettings != (Object)null} CurrentSpinPlayer.IsValid {CurrentSpinPlayer.IsValid()} CurrentSpinPlayer == mounted {(Object)(object)CurrentSpinPlayer == (Object)(object)_mounted}");
		}
		if (!flag)
		{
			SetFlag(Flags.Reserved2, b: false);
		}
		else
		{
			((FacepunchBehaviour)this).Invoke((Action)DelayedSpinningReset, 4f);
		}
		CurrentSpinPlayer = null;
	}

	private void DelayedSpinningReset()
	{
		SetFlag(Flags.Reserved2, b: false);
	}

	private void CalculateSpinResults()
	{
		if (ForcePayoutIndex != -1)
		{
			SpinResult1 = PayoutSettings.Payouts[ForcePayoutIndex].Result1;
			SpinResult2 = PayoutSettings.Payouts[ForcePayoutIndex].Result2;
			SpinResult3 = PayoutSettings.Payouts[ForcePayoutIndex].Result3;
		}
		else
		{
			SpinResult1 = RandomSpinResult();
			SpinResult2 = RandomSpinResult();
			SpinResult3 = RandomSpinResult();
		}
	}

	private int RandomSpinResult()
	{
		int num = new Random(Random.Range(0, 1000)).Next(0, PayoutSettings.TotalStops);
		int num2 = 0;
		int num3 = 0;
		int[] virtualFaces = PayoutSettings.VirtualFaces;
		foreach (int num4 in virtualFaces)
		{
			if (num < num4 + num2)
			{
				return num3;
			}
			num2 += num4;
			num3++;
		}
		return 15;
	}

	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		BaseEntity baseEntity = StorageInstance.Get(serverside: true);
		if ((Object)(object)baseEntity != (Object)null && baseEntity is SlotMachineStorage slotMachineStorage)
		{
			slotMachineStorage.inventory.GetSlot(1)?.MoveToContainer(player.inventory.containerMain);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.CallsPerSecond(5uL)]
	private void Server_RequestMultiplierChange(RPCMessage msg)
	{
		if (!((Object)(object)msg.player != (Object)(object)_mounted) && !HasFlag(Flags.Reserved2))
		{
			CurrentMultiplier = Mathf.Clamp(msg.read.Int32(), 1, 5);
			OnBettingScrapUpdated(GetBettingAmount());
			SendNetworkUpdate();
		}
	}

	public void OnBettingScrapUpdated(int amount)
	{
		SetFlag(Flags.Reserved1, (float)amount >= PayoutSettings.SpinCost.amount * (float)CurrentMultiplier);
	}

	private bool CalculatePayout(out SlotMachinePayoutSettings.PayoutInfo info, out int bonus)
	{
		info = default(SlotMachinePayoutSettings.PayoutInfo);
		bonus = 0;
		SlotMachinePayoutSettings.IndividualPayouts[] facePayouts = PayoutSettings.FacePayouts;
		for (int i = 0; i < facePayouts.Length; i++)
		{
			SlotMachinePayoutSettings.IndividualPayouts individualPayouts = facePayouts[i];
			if (individualPayouts.Result == SpinResult1)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (individualPayouts.Result == SpinResult2)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (individualPayouts.Result == SpinResult3)
			{
				bonus += (int)individualPayouts.Item.amount;
			}
			if (bonus > 0)
			{
				info.Item = new ItemAmount(individualPayouts.Item.itemDef);
			}
		}
		SlotMachinePayoutSettings.PayoutInfo[] payouts = PayoutSettings.Payouts;
		for (int i = 0; i < payouts.Length; i++)
		{
			SlotMachinePayoutSettings.PayoutInfo payoutInfo = payouts[i];
			if (payoutInfo.Result1 == SpinResult1 && payoutInfo.Result2 == SpinResult2 && payoutInfo.Result3 == SpinResult3)
			{
				info = payoutInfo;
				return true;
			}
		}
		return bonus > 0;
	}
}
