using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class CCTV_RC : PoweredRemoteControlEntity, IRemoteControllableClientCallbacks, IRemoteControllable
{
	public Transform pivotOrigin;

	public Transform yaw;

	public Transform pitch;

	public Vector2 pitchClamp = new Vector2(-50f, 50f);

	public Vector2 yawClamp = new Vector2(-50f, 50f);

	public float turnSpeed = 25f;

	public float serverLerpSpeed = 15f;

	public float clientLerpSpeed = 10f;

	public float zoomLerpSpeed = 10f;

	public float[] fovScales;

	private float pitchAmount;

	private float yawAmount;

	private int fovScaleIndex;

	private float fovScaleLerped = 1f;

	public bool hasPTZ = true;

	public AnimationCurve dofCurve = AnimationCurve.Constant(0f, 1f, 0f);

	public float dofApertureMax = 10f;

	public const Flags Flag_HasViewer = Flags.Reserved5;

	public SoundDefinition movementLoopSoundDef;

	public AnimationCurve movementLoopGainCurve;

	public float movementLoopSmoothing = 1f;

	public float movementLoopReference = 50f;

	private Sound movementLoop;

	private SoundModulation.Modulator movementLoopGainModulator;

	public SoundDefinition zoomInSoundDef;

	public SoundDefinition zoomOutSoundDef;

	private RealTimeSinceEx timeSinceLastServerTick;

	public override bool RequiresMouse => hasPTZ;

	protected override bool EntityCanPing => true;

	public override bool CanAcceptInput => hasPTZ;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CCTV_RC.OnRpcMessage", 0);
		try
		{
			if (rpc == 3353964129u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - Server_SetDir "));
				}
				TimeWarning val2 = TimeWarning.New("Server_SetDir", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg2 = rPCMessage;
						Server_SetDir(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in Server_SetDir");
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

	public override int ConsumptionAmount()
	{
		return 3;
	}

	public override void ServerInit()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (!base.isClient)
		{
			if (IsStatic())
			{
				pitchAmount = pitch.localEulerAngles.x;
				yawAmount = yaw.localEulerAngles.y;
				UpdateRCAccess(isOnline: true);
			}
			timeSinceLastServerTick = 0.0;
			((FacepunchBehaviour)this).InvokeRandomized((Action)ServerTick, Random.Range(0f, 1f), 0.015f, 0.01f);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		UpdateRotation(10000f);
	}

	public override void UserInput(InputState inputState, CameraViewerId viewerID)
	{
		if (UpdateManualAim(inputState))
		{
			SendNetworkUpdate();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.rcEntity == null)
		{
			info.msg.rcEntity = Pool.Get<RCEntity>();
		}
		info.msg.rcEntity.aim.x = pitchAmount;
		info.msg.rcEntity.aim.y = yawAmount;
		info.msg.rcEntity.aim.z = 0f;
		info.msg.rcEntity.zoom = fovScaleIndex;
	}

	[RPC_Server]
	public void Server_SetDir(RPCMessage msg)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (!IsStatic())
		{
			BasePlayer player = msg.player;
			if (player.CanBuild() && player.IsBuildingAuthed())
			{
				Vector3 val = Vector3Ex.Direction(player.eyes.position, ((Component)yaw).transform.position);
				val = ((Component)this).transform.InverseTransformDirection(val);
				Quaternion val2 = Quaternion.LookRotation(val);
				Vector3 val3 = BaseMountable.ConvertVector(((Quaternion)(ref val2)).eulerAngles);
				pitchAmount = Mathf.Clamp(val3.x, pitchClamp.x, pitchClamp.y);
				yawAmount = Mathf.Clamp(val3.y, yawClamp.x, yawClamp.y);
				SendNetworkUpdate();
			}
		}
	}

	public override bool InitializeControl(CameraViewerId viewerID)
	{
		bool result = base.InitializeControl(viewerID);
		UpdateViewers();
		return result;
	}

	public override void StopControl(CameraViewerId viewerID)
	{
		base.StopControl(viewerID);
		UpdateViewers();
	}

	public void UpdateViewers()
	{
		SetFlag(Flags.Reserved5, base.ViewerCount > 0);
	}

	public void ServerTick()
	{
		if (!base.isClient && !base.IsDestroyed)
		{
			float delta = (float)(double)timeSinceLastServerTick;
			timeSinceLastServerTick = 0.0;
			UpdateRotation(delta);
		}
	}

	private bool UpdateManualAim(InputState inputState)
	{
		if (!hasPTZ)
		{
			return false;
		}
		float num = 0f - inputState.current.mouseDelta.y;
		float x = inputState.current.mouseDelta.x;
		bool flag = inputState.WasJustPressed(BUTTON.FIRE_PRIMARY);
		pitchAmount = Mathf.Clamp(pitchAmount + num * turnSpeed, pitchClamp.x, pitchClamp.y);
		yawAmount = Mathf.Clamp(yawAmount + x * turnSpeed, yawClamp.x, yawClamp.y) % 360f;
		if (flag)
		{
			fovScaleIndex = (fovScaleIndex + 1) % fovScales.Length;
		}
		return num != 0f || x != 0f || flag;
	}

	public void UpdateRotation(float delta)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.Euler(pitchAmount, 0f, 0f);
		Quaternion val2 = Quaternion.Euler(0f, yawAmount, 0f);
		float num = ((base.isServer && !base.IsBeingControlled) ? serverLerpSpeed : clientLerpSpeed);
		((Component)pitch).transform.localRotation = Mathx.Lerp(((Component)pitch).transform.localRotation, val, num, delta);
		((Component)yaw).transform.localRotation = Mathx.Lerp(((Component)yaw).transform.localRotation, val2, num, delta);
		if (fovScales != null && fovScales.Length != 0)
		{
			if (fovScales.Length > 1)
			{
				fovScaleLerped = Mathx.Lerp(fovScaleLerped, fovScales[fovScaleIndex], zoomLerpSpeed, delta);
			}
			else
			{
				fovScaleLerped = fovScales[0];
			}
		}
		else
		{
			fovScaleLerped = 1f;
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.rcEntity != null)
		{
			int num = Mathf.Clamp((int)info.msg.rcEntity.zoom, 0, fovScales.Length - 1);
			if (base.isServer)
			{
				pitchAmount = info.msg.rcEntity.aim.x;
				yawAmount = info.msg.rcEntity.aim.y;
				fovScaleIndex = num;
			}
		}
	}

	public override float GetFovScale()
	{
		return fovScaleLerped;
	}
}
