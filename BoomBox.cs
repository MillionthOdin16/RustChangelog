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
	public AudioSource SoundSource;

	public float ConditionLossRate = 0.25f;

	public ItemDefinition[] ValidCassettes;

	public SoundDefinition PlaySfx;

	public SoundDefinition StopSfx;

	public const BaseEntity.Flags HasCassette = BaseEntity.Flags.Reserved1;

	[ServerVar(Saved = true)]
	public static int BacktrackLength = 30;

	public Action<float> HurtCallback;

	public static Dictionary<string, string> ValidStations;

	public static Dictionary<string, string> ServerValidStations;

	[ReplicatedVar(Saved = true, Help = "A list of radio stations that are valid on this server. Format: NAME,URL,NAME,URL,etc", ShowInAdminUI = true)]
	public static string ServerUrlList = string.Empty;

	private static string lastParsedServerList;

	public ShoutcastStreamer ShoutcastStreamer;

	public GameObjectRef RadioIpDialog;

	public ulong AssignedRadioBy;

	public BaseEntity BaseEntity => base.baseEntity;

	private bool isClient
	{
		get
		{
			if ((Object)(object)base.baseEntity != (Object)null)
			{
				return base.baseEntity.isClient;
			}
			return false;
		}
	}

	public string CurrentRadioIp { get; private set; } = "rustradio.facepunch.com";


	[ServerVar]
	public static void ClearRadioByUser(Arg arg)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		ulong uInt = arg.GetUInt64(0, 0uL);
		int num = 0;
		Enumerator<BaseNetworkable> enumerator = BaseNetworkable.serverEntities.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseNetworkable current = enumerator.Current;
				if (current is DeployableBoomBox deployableBoomBox)
				{
					if (deployableBoomBox.ClearRadioByUserId(uInt))
					{
						num++;
					}
				}
				else if (current is HeldBoomBox heldBoomBox && heldBoomBox.ClearRadioByUserId(uInt))
				{
					num++;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		arg.ReplyWith($"Stopped and cleared saved URL of {num} boom boxes");
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
		if (!((Object)(object)base.baseEntity == (Object)null) && HasFlag(BaseEntity.Flags.On) != play)
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
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
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
		if (!base.baseEntity.HasFlag(BaseEntity.Flags.Reserved8))
		{
			return base.baseEntity is HeldBoomBox;
		}
		return true;
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
		JObject obj = Application.Manifest?.Metadata;
		JToken obj2 = ((obj != null) ? obj["RadioStations"] : null);
		JArray val = (JArray)(object)((obj2 is JArray) ? obj2 : null);
		if (val != null && ((JContainer)val).Count > 0)
		{
			string[] array = new string[2];
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			{
				foreach (string item in ((JContainer)val).Values<string>())
				{
					array = item.Split(',', StringSplitOptions.None);
					if (!dictionary.ContainsKey(array[0]))
					{
						dictionary.Add(array[0], array[1]);
					}
				}
				return dictionary;
			}
		}
		return null;
	}

	private static bool IsStationValid(string url)
	{
		ParseServerUrlList();
		if (ValidStations == null || !ValidStations.ContainsValue(url))
		{
			if (ServerValidStations != null)
			{
				return ServerValidStations.ContainsValue(url);
			}
			return false;
		}
		return true;
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
			string[] array = ServerUrlList.Split(',', StringSplitOptions.None);
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
		string text = msg.read.String(256, false);
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
}
