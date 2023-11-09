using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer;

public static class Server
{
	private class RegisterResponse
	{
		public string ServerId;

		public string ServerToken;
	}

	private class TestConnectionResponse
	{
		public List<string> Messages;
	}

	private const string ApiEndpoint = "https://companion-rust.facepunch.com/api/server";

	private static readonly HttpClient Http = new HttpClient();

	internal static readonly ChatLog TeamChat = new ChatLog();

	internal static string Token;

	public static Listener Listener { get; private set; }

	public static bool IsEnabled => App.port >= 0 && !string.IsNullOrWhiteSpace(App.serverid) && Listener != null;

	public static void Initialize()
	{
		if (App.port < 0)
		{
			return;
		}
		if (IsEnabled)
		{
			Debug.LogWarning((object)"Rust+ is already started up! Skipping second startup");
			return;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if (!((Object)(object)activeGameMode != (Object)null) || activeGameMode.rustPlus)
		{
			Map.PopulateCache();
			if (App.port == 0)
			{
				int num = Math.Max(ConVar.Server.port, RCon.Port);
				App.port = num + 67;
			}
			try
			{
				Listener = new Listener(App.GetListenIP(), App.port);
			}
			catch (Exception arg)
			{
				Debug.LogError((object)$"Companion server failed to start: {arg}");
			}
			PostInitializeServer();
		}
	}

	public static void Shutdown()
	{
		SetServerId(null);
		Listener?.Dispose();
		Listener = null;
	}

	public static void Update()
	{
		Listener?.Update();
	}

	public static void Broadcast(PlayerTarget target, AppBroadcast broadcast)
	{
		Listener?.PlayerSubscribers?.Send(target, broadcast);
	}

	public static void Broadcast(EntityTarget target, AppBroadcast broadcast)
	{
		Listener?.EntitySubscribers?.Send(target, broadcast);
	}

	public static void Broadcast(CameraTarget target, AppBroadcast broadcast)
	{
		Listener?.CameraSubscribers?.Send(target, broadcast);
	}

	public static bool HasAnySubscribers(CameraTarget target)
	{
		return Listener?.CameraSubscribers?.HasAnySubscribers(target) ?? false;
	}

	public static bool CanSendPairingNotification(ulong playerId)
	{
		return Listener?.CanSendPairingNotification(playerId) ?? false;
	}

	private static async void PostInitializeServer()
	{
		await SetupServerRegistration();
		await CheckConnectivity();
	}

	private static async Task SetupServerRegistration()
	{
		try
		{
			if (TryLoadServerRegistration(out var _, out var serverToken))
			{
				StringContent refreshContent = new StringContent(serverToken, Encoding.UTF8, "text/plain");
				HttpResponseMessage refreshResponse = await Http.PostAsync("https://companion-rust.facepunch.com/api/server/refresh", refreshContent);
				if (refreshResponse.IsSuccessStatusCode)
				{
					SetServerRegistration(await refreshResponse.Content.ReadAsStringAsync());
					return;
				}
				Debug.LogWarning((object)"Failed to refresh server ID - registering a new one");
			}
			SetServerRegistration(await Http.GetStringAsync("https://companion-rust.facepunch.com/api/server/register"));
		}
		catch (Exception ex)
		{
			Exception e = ex;
			Debug.LogError((object)$"Failed to setup companion server registration: {e}");
		}
	}

	private static bool TryLoadServerRegistration(out string serverId, out string serverToken)
	{
		serverId = null;
		serverToken = null;
		string serverIdPath = GetServerIdPath();
		if (!File.Exists(serverIdPath))
		{
			return false;
		}
		try
		{
			string text = File.ReadAllText(serverIdPath);
			RegisterResponse registerResponse = JsonConvert.DeserializeObject<RegisterResponse>(text);
			serverId = registerResponse.ServerId;
			serverToken = registerResponse.ServerToken;
			return true;
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"Failed to load companion server registration: {arg}");
			return false;
		}
	}

	private static void SetServerRegistration(string responseJson)
	{
		RegisterResponse registerResponse = null;
		try
		{
			registerResponse = JsonConvert.DeserializeObject<RegisterResponse>(responseJson);
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"Failed to parse registration response JSON: {responseJson}\n\n{arg}");
		}
		SetServerId(registerResponse?.ServerId);
		Token = registerResponse?.ServerToken;
		if (registerResponse == null)
		{
			return;
		}
		try
		{
			string serverIdPath = GetServerIdPath();
			File.WriteAllText(serverIdPath, responseJson);
		}
		catch (Exception arg2)
		{
			Debug.LogError((object)$"Unable to save companion app server registration - server ID may be different after restart: {arg2}");
		}
	}

	private static async Task CheckConnectivity()
	{
		if (!IsEnabled)
		{
			SetServerId(null);
			return;
		}
		try
		{
			string publicIp = await GetPublicIPAsync();
			StringContent testContent = new StringContent("", Encoding.UTF8, "text/plain");
			HttpResponseMessage testResponse = await Http.PostAsync("https://companion-rust.facepunch.com/api/server" + $"/test_connection?address={publicIp}&port={App.port}", testContent);
			string testResponseJson = await testResponse.Content.ReadAsStringAsync();
			TestConnectionResponse response = null;
			try
			{
				response = JsonConvert.DeserializeObject<TestConnectionResponse>(testResponseJson);
			}
			catch (Exception ex)
			{
				Exception e2 = ex;
				Debug.LogError((object)$"Failed to parse connectivity test response JSON: {testResponseJson}\n\n{e2}");
			}
			if (response == null)
			{
				return;
			}
			IEnumerable<string> messages = response.Messages;
			string messagesText = string.Join("\n", messages ?? Enumerable.Empty<string>());
			if (testResponse.StatusCode == (HttpStatusCode)555)
			{
				Debug.LogError((object)("Rust+ companion server connectivity test failed! Disabling Rust+ features.\n\n" + messagesText));
				SetServerId(null);
				return;
			}
			testResponse.EnsureSuccessStatusCode();
			if (!string.IsNullOrWhiteSpace(messagesText))
			{
				Debug.LogWarning((object)("Rust+ companion server connectivity test has warnings:\n" + messagesText));
			}
		}
		catch (Exception e)
		{
			Debug.LogError((object)$"Failed to check connectivity to the companion server: {e}");
		}
	}

	private static async Task<string> GetPublicIPAsync()
	{
		Stopwatch timer = Stopwatch.StartNew();
		string publicIp;
		while (true)
		{
			bool timedOut = timer.Elapsed.TotalMinutes > 2.0;
			publicIp = App.GetPublicIP();
			if (timedOut || (!string.IsNullOrWhiteSpace(publicIp) && publicIp != "0.0.0.0"))
			{
				break;
			}
			await Task.Delay(10000);
		}
		return publicIp;
	}

	private static void SetServerId(string serverId)
	{
		Command val = Server.Find("app.serverid");
		if (val != null)
		{
			val.Set(serverId ?? "");
		}
	}

	private static string GetServerIdPath()
	{
		return Path.Combine(ConVar.Server.rootFolder, "companion.id");
	}
}
