using System;

namespace UnityEngine.UI.Extensions;

internal static class SetPropertyUtility
{
	public static bool SetColor(ref Color currentValue, Color newValue)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
		{
			return false;
		}
		currentValue = newValue;
		return true;
	}

	public static bool SetEquatableStruct<T>(ref T currentValue, T newValue) where T : IEquatable<T>
	{
		if (currentValue.Equals(newValue))
		{
			return false;
		}
		currentValue = newValue;
		return true;
	}

	public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
	{
		if (currentValue.Equals(newValue))
		{
			return false;
		}
		currentValue = newValue;
		return true;
	}

	public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
	{
		if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
		{
			return false;
		}
		currentValue = newValue;
		return true;
	}
}
