using System;
using System.Collections.Generic;
using System.IO;
using ConVar;
using Facepunch;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using UnityEngine;

public class BoomBox : EntityComponent<BaseEntity>, INotifyLOD
{
	public static Dictionary<string, string> ValidStations;

	public static Dictionary<string, string> ServerValidStations;

	[ReplicatedVar(Saved = true, Help = "A list of radio stations that are valid on this server. Format: NAME,URL,NAME,URL,etc", ShowInAdminUI = true)]
	public static string ServerUrlList = string.Empty;

	private static string lastParsedServerList;

	public ShoutcastStreamer ShoutcastStreamer = null;

	public GameObjectRef RadioIpDialog;

	public ulong AssignedRadioBy = 0uL;

	public AudioSource SoundSource = null;

	public float ConditionLossRate = 0.25f;

	public ItemDefinition[] ValidCassettes;

	public SoundDefinition PlaySfx = null;

	public SoundDefinition StopSfx = null;

	public const BaseEntity.Flags HasCassette = BaseEntity.Flags.Reserved1;

	[ServerVar(Saved = true)]
	public static int BacktrackLength = 30;

	public Action<float> HurtCallback = null;

	public string CurrentRadioIp { get; private set; } = "rustradio.facepunch.com";


	public BaseEntity BaseEntity => base.baseEntity;

	private bool isClient => (Object)(object)base.baseEntity != (Object)null && base.baseEntity.isClient;

	[ServerVar]
	public static void ClearRadioByUser(Arg arg)
	{
		ulong uInt = arg.GetUInt64(0, 0uL);
		int num = 0;
		foreach (BaseNetworkable serverEntity in BaseNetworkable.serverEntities)
		{
			if (serverEntity is DeployableBoomBox deployableBoomBox)
			{
				if (deployableBoomBox.ClearRadioByUserId(uInt))
				{
					num++;
				}
			}
			else if (serverEntity is HeldBoomBox heldBoomBox && heldBoomBox.ClearRadioByUserId(uInt))
			{
				num++;
			}
		}
		arg.ReplyWith($"Stopped and cleared saved URL of {num} boom boxes");
	}

	public static void LoadStations()
	{
		if (ValidStations == null)
		{
			ValidStations = GetStationData() ?? new Dictionary<string, string>();
			ParseServerUrlList();
		}
	}

	private static Dictionary<string, string> GetStationData()
	{
		JObject val = Application.Manifest?.Metadata;
		JToken val2 = ((val != null) ? val["RadioStations"] : null);
		JArray val3;
		if ((val3 = (JArray)(object)((val2 is JArray) ? val2 : null)) != null && ((JContainer)val3).Count > 0)
		{
			string[] array = new string[2];
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string item in ((JToken)val3).Values<string>())
			{
				array = item.Split(',');
				if (!dictionary.ContainsKey(array[0]))
				{
					dictionary.Add(array[0], array[1]);
				}
			}
			return dictionary;
		}
		return null;
	}

	private static bool IsStationValid(string url)
	{
		ParseServerUrlList();
		return (ValidStations != null && ValidStations.ContainsValue(url)) || (ServerValidStations != null && ServerValidStations.ContainsValue(url));
	}

	public static void ParseServerUrlList()
	{
		if (ServerValidStations == null)
		{
			ServerValidStations = new Dictionary<string, string>();
		}
		if (lastParsedServerList == ServerUrlList)
		{
			return;
		}
		ServerValidStations.Clear();
		if (!string.IsNullOrEmpty(ServerUrlList))
		{
			string[] array = ServerUrlList.Split(',');
			if (array.Length % 2 != 0)
			{
				Debug.Log((object)"Invalid number of stations in BoomBox.ServerUrlList, ensure you always have a name and a url");
				return;
			}
			for (int i = 0; i < array.Length; i += 2)
			{
				if (ServerValidStations.ContainsKey(array[i]))
				{
					Debug.Log((object)("Duplicate station name detected in BoomBox.ServerUrlList, all station names must be unique: " + array[i]));
				}
				else
				{
					ServerValidStations.Add(array[i], array[i + 1]);
				}
			}
		}
		lastParsedServerList = ServerUrlList;
	}

	public void Server_UpdateRadioIP(BaseEntity.RPCMessage msg)
	{
		string text = msg.read.String(256);
		if (IsStationValid(text))
		{
			if ((Object)(object)msg.player != (Object)null)
			{
				ulong userID = msg.player.userID;
				AssignedRadioBy = userID;
			}
			CurrentRadioIp = text;
			base.baseEntity.ClientRPC(null, "OnRadioIPChanged", CurrentRadioIp);
			if (IsOn())
			{
				ServerTogglePlay(play: false);
			}
		}
	}

	public void Save(BaseNetworkable.SaveInfo info)
	{
		if (info.msg.boomBox == null)
		{
			info.msg.boomBox = Pool.Get<BoomBox>();
		}
		info.msg.boomBox.radioIp = CurrentRadioIp;
		info.msg.boomBox.assignedRadioBy = AssignedRadioBy;
	}

	public bool ClearRadioByUserId(ulong id)
	{
		if (AssignedRadioBy == id)
		{
			CurrentRadioIp = string.Empty;
			AssignedRadioBy = 0uL;
			if (HasFlag(BaseEntity.Flags.On))
			{
				ServerTogglePlay(play: false);
			}
			return true;
		}
		return false;
	}

	public void Load(BaseNetworkable.LoadInfo info)
	{
		if (info.msg.boomBox != null)
		{
			CurrentRadioIp = info.msg.boomBox.radioIp;
			AssignedRadioBy = info.msg.boomBox.assignedRadioBy;
		}
	}

	public void ServerTogglePlay(BaseEntity.RPCMessage msg)
	{
		if (IsPowered())
		{
			bool play = ((Stream)(object)msg.read).ReadByte() == 1;
			ServerTogglePlay(play);
		}
	}

	private void DeductCondition()
	{
		HurtCallback?.Invoke(ConditionLossRate * ConVar.Decay.scale);
	}

	public void ServerTogglePlay(bool play)
	{
		if (!((Object)(object)base.baseEntity == (Object)null))
		{
			SetFlag(BaseEntity.Flags.On, play);
			if (base.baseEntity is IOEntity iOEntity)
			{
				iOEntity.SendChangedToRoot(forceUpdate: true);
				iOEntity.MarkDirtyForceUpdateOutputs();
			}
			if (play && !((FacepunchBehaviour)this).IsInvoking((Action)DeductCondition) && ConditionLossRate > 0f)
			{
				((FacepunchBehaviour)this).InvokeRepeating((Action)DeductCondition, 1f, 1f);
			}
			else if (((FacepunchBehaviour)this).IsInvoking((Action)DeductCondition))
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)DeductCondition);
			}
		}
	}

	public void OnCassetteInserted(Cassette c)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)base.baseEntity == (Object)null))
		{
			base.baseEntity.ClientRPC<NetworkableId>(null, "Client_OnCassetteInserted", c.net.ID);
			ServerTogglePlay(play: false);
			SetFlag(BaseEntity.Flags.Reserved1, state: true);
			base.baseEntity.SendNetworkUpdate();
		}
	}

	public void OnCassetteRemoved(Cassette c)
	{
		if (!((Object)(object)base.baseEntity == (Object)null))
		{
			base.baseEntity.ClientRPC(null, "Client_OnCassetteRemoved");
			ServerTogglePlay(play: false);
			SetFlag(BaseEntity.Flags.Reserved1, state: false);
		}
	}

	private bool IsPowered()
	{
		if ((Object)(object)base.baseEntity == (Object)null)
		{
			return false;
		}
		return base.baseEntity.HasFlag(BaseEntity.Flags.Reserved8) || base.baseEntity is HeldBoomBox;
	}

	private bool IsOn()
	{
		if ((Object)(object)base.baseEntity == (Object)null)
		{
			return false;
		}
		return base.baseEntity.IsOn();
	}

	private bool HasFlag(BaseEntity.Flags f)
	{
		if ((Object)(object)base.baseEntity == (Object)null)
		{
			return false;
		}
		return base.baseEntity.HasFlag(f);
	}

	private void SetFlag(BaseEntity.Flags f, bool state)
	{
		if ((Object)(object)base.baseEntity != (Object)null)
		{
			base.baseEntity.SetFlag(f, state);
		}
	}
}
