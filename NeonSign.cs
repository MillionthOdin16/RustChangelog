using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class NeonSign : Signage
{
	private const float FastSpeed = 0.5f;

	private const float MediumSpeed = 1f;

	private const float SlowSpeed = 2f;

	private const float MinSpeed = 0.5f;

	private const float MaxSpeed = 5f;

	[Header("Neon Sign")]
	public Light topLeft;

	public Light topRight;

	public Light bottomLeft;

	public Light bottomRight;

	public float lightIntensity = 2f;

	[Range(1f, 100f)]
	public int powerConsumption = 10;

	public Material activeMaterial;

	public Material inactiveMaterial;

	private float animationSpeed = 1f;

	private int currentFrame = 0;

	private List<Lights> frameLighting;

	private bool isAnimating = false;

	private Action animationLoopAction;

	public AmbienceEmitter ambientSoundEmitter;

	public SoundDefinition switchSoundDef;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("NeonSign.OnRpcMessage", 0);
		try
		{
			if (rpc == 2433901419u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetAnimationSpeed "));
				}
				TimeWarning val2 = TimeWarning.New("SetAnimationSpeed", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2433901419u, "SetAnimationSpeed", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(2433901419u, "SetAnimationSpeed", this, player, 3f))
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
							RPCMessage rPCMessage2 = rPCMessage;
							SetAnimationSpeed(rPCMessage2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SetAnimationSpeed");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1919786296 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - UpdateNeonColors "));
				}
				TimeWarning val5 = TimeWarning.New("UpdateNeonColors", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1919786296u, "UpdateNeonColors", this, player, 5uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(1919786296u, "UpdateNeonColors", this, player, 3f))
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
							RPCMessage msg2 = rPCMessage;
							UpdateNeonColors(msg2);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in UpdateNeonColors");
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

	public override int ConsumptionAmount()
	{
		return powerConsumption;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.neonSign == null)
		{
			return;
		}
		if (frameLighting != null)
		{
			foreach (Lights item in frameLighting)
			{
				Lights val = item;
				Pool.Free<Lights>(ref val);
			}
			Pool.FreeList<Lights>(ref frameLighting);
		}
		frameLighting = info.msg.neonSign.frameLighting;
		info.msg.neonSign.frameLighting = null;
		currentFrame = Mathf.Clamp(info.msg.neonSign.currentFrame, 0, paintableSources.Length);
		animationSpeed = Mathf.Clamp(info.msg.neonSign.animationSpeed, 0.5f, 5f);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		animationLoopAction = SwitchToNextFrame;
	}

	public override void ResetState()
	{
		base.ResetState();
		((FacepunchBehaviour)this).CancelInvoke(animationLoopAction);
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (paintableSources.Length <= 1)
		{
			return;
		}
		bool flag = HasFlag(Flags.Reserved8);
		if (flag && !isAnimating)
		{
			if (currentFrame != 0)
			{
				currentFrame = 0;
				ClientRPC(null, "SetFrame", currentFrame);
			}
			((FacepunchBehaviour)this).InvokeRepeating(animationLoopAction, animationSpeed, animationSpeed);
			isAnimating = true;
		}
		else if (!flag && isAnimating)
		{
			((FacepunchBehaviour)this).CancelInvoke(animationLoopAction);
			isAnimating = false;
		}
	}

	private void SwitchToNextFrame()
	{
		int num = currentFrame;
		for (int i = 0; i < paintableSources.Length; i++)
		{
			currentFrame++;
			if (currentFrame >= paintableSources.Length)
			{
				currentFrame = 0;
			}
			if (textureIDs[currentFrame] != 0)
			{
				break;
			}
		}
		if (currentFrame != num)
		{
			ClientRPC(null, "SetFrame", currentFrame);
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		List<Lights> list = Pool.GetList<Lights>();
		if (frameLighting != null)
		{
			foreach (Lights item in frameLighting)
			{
				list.Add(item.Copy());
			}
		}
		info.msg.neonSign = Pool.Get<NeonSign>();
		info.msg.neonSign.frameLighting = list;
		info.msg.neonSign.currentFrame = currentFrame;
		info.msg.neonSign.animationSpeed = animationSpeed;
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.MaxDistance(3f)]
	public void SetAnimationSpeed(RPCMessage msg)
	{
		float num = Mathf.Clamp(msg.read.Float(), 0.5f, 5f);
		animationSpeed = num;
		if (isAnimating)
		{
			((FacepunchBehaviour)this).CancelInvoke(animationLoopAction);
			((FacepunchBehaviour)this).InvokeRepeating(animationLoopAction, animationSpeed, animationSpeed);
		}
		SendNetworkUpdate();
	}

	[RPC_Server]
	[RPC_Server.CallsPerSecond(5uL)]
	[RPC_Server.MaxDistance(3f)]
	public void UpdateNeonColors(RPCMessage msg)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (CanUpdateSign(msg.player))
		{
			int num = msg.read.Int32();
			if (num >= 0 && num < paintableSources.Length)
			{
				EnsureInitialized();
				frameLighting[num].topLeft = ClampColor(msg.read.Color());
				frameLighting[num].topRight = ClampColor(msg.read.Color());
				frameLighting[num].bottomLeft = ClampColor(msg.read.Color());
				frameLighting[num].bottomRight = ClampColor(msg.read.Color());
				SendNetworkUpdate();
			}
		}
	}

	private void EnsureInitialized()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (frameLighting == null)
		{
			frameLighting = Pool.GetList<Lights>();
		}
		while (frameLighting.Count < paintableSources.Length)
		{
			Lights val = Pool.Get<Lights>();
			val.topLeft = Color.clear;
			val.topRight = Color.clear;
			val.bottomLeft = Color.clear;
			val.bottomRight = Color.clear;
			frameLighting.Add(val);
		}
	}

	private static Color ClampColor(Color color)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		return new Color(Mathf.Clamp01(color.r), Mathf.Clamp01(color.g), Mathf.Clamp01(color.b), Mathf.Clamp01(color.a));
	}
}
