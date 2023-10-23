using System;
using System.Net;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using Rust;
using Rust.Platform.Common;

public class RustPlatformHooks : IPlatformHooks
{
	public static readonly RustPlatformHooks Instance = new RustPlatformHooks();

	public uint SteamAppId => Defines.appID;

	public ServerParameters? ServerParameters
	{
		get
		{
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			if (Net.sv == null)
			{
				return null;
			}
			IPAddress iPAddress = null;
			if (!string.IsNullOrEmpty(ConVar.Server.ip))
			{
				iPAddress = IPAddress.Parse(ConVar.Server.ip);
			}
			if (ConVar.Server.queryport <= 0 || ConVar.Server.queryport == ConVar.Server.port)
			{
				ConVar.Server.queryport = Math.Max(ConVar.Server.port, RCon.Port) + 1;
			}
			return new ServerParameters("rust", "Rust", 2509.ToString(), ConVar.Server.secure, CommandLine.HasSwitch("-sdrnet"), iPAddress, (ushort)Net.sv.port, (ushort)ConVar.Server.queryport);
		}
	}

	public void Abort()
	{
		Application.Quit();
	}

	public void OnItemDefinitionsChanged()
	{
		ItemManager.InvalidateWorkshopSkinCache();
	}

	public void AuthSessionValidated(ulong userId, ulong ownerUserId, AuthResponse response, string rawResponse)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Analytics.Azure.OnSteamAuth(userId, ownerUserId, rawResponse);
		SingletonComponent<ServerMgr>.Instance.OnValidateAuthTicketResponse(userId, ownerUserId, response);
	}
}
