using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Nexus.Models;
using UnityEngine;

public static class NexusUtil
{
	private static readonly char[] ScheduleSeparators = new char[1] { ',' };

	public static bool TryParseFerrySchedule(string zone, string scheduleString, out string[] entries)
	{
		if (string.IsNullOrWhiteSpace(zone) || string.IsNullOrWhiteSpace(scheduleString))
		{
			entries = null;
			return false;
		}
		string[] array = scheduleString.Split(ScheduleSeparators, StringSplitOptions.RemoveEmptyEntries);
		if (!array.Contains(zone, StringComparer.InvariantCultureIgnoreCase))
		{
			Array.Resize(ref array, array.Length + 1);
			Array.Copy(array, 0, array, 1, array.Length - 1);
			array[0] = zone;
		}
		if (array.Length <= 1)
		{
			Debug.LogWarning((object)("Ferry schedule for '" + zone + "' needs at least two zones in it: " + scheduleString));
			entries = null;
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = array[i].Trim();
			string text = array[i];
			if (string.IsNullOrWhiteSpace(text))
			{
				Debug.LogWarning((object)("Ferry schedule for '" + zone + "' has empty entries: " + scheduleString));
				entries = null;
				return false;
			}
			string b = ((i == 0) ? array[array.Length - 1] : array[i - 1]);
			if (string.Equals(text, b, StringComparison.InvariantCultureIgnoreCase))
			{
				Debug.LogWarning((object)("Ferry schedule for '" + zone + "' has the same zone twice in a row: " + scheduleString));
				entries = null;
				return false;
			}
		}
		entries = array;
		return true;
	}

	public static string ConnectionProtocol(this NexusZoneDetails zone)
	{
		if (zone == null || !zone.Variables.TryGetString("protocol", out var value))
		{
			return "";
		}
		return value ?? "";
	}

	public static bool IsStarterZone(this ZoneDetails zone)
	{
		return ((zone != null) ? zone.Variables : null).IsStarterZone();
	}

	public static bool IsStarterZone(this NexusZoneDetails zone)
	{
		return ((zone != null) ? zone.Variables : null).IsStarterZone();
	}

	private static bool IsStarterZone(this VariableDictionary variables)
	{
		string value;
		bool result = default(bool);
		return variables.TryGetString("starterZone", out value) && bool.TryParse(value, out result) && result;
	}

	public static bool TryGetString(this VariableDictionary variables, string key, out string value)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		if (variables != null && ((Dictionary<string, VariableData>)(object)variables).TryGetValue(key, out VariableData value2) && (int)((VariableData)(ref value2)).Type == 1 && !string.IsNullOrWhiteSpace(((VariableData)(ref value2)).Value))
		{
			value = ((VariableData)(ref value2)).Value;
			return true;
		}
		value = null;
		return false;
	}
}
