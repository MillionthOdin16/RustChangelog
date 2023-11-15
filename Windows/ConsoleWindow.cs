using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;
using UnityEngine;

namespace Windows;

[SuppressUnmanagedCodeSecurity]
public class ConsoleWindow
{
	private TextWriter oldOutput;

	private const int STD_INPUT_HANDLE = -10;

	private const int STD_OUTPUT_HANDLE = -11;

	public void Initialize()
	{
		FreeConsole();
		if (!AttachConsole(uint.MaxValue))
		{
			AllocConsole();
		}
		oldOutput = Console.Out;
		try
		{
			Console.OutputEncoding = Encoding.UTF8;
			IntPtr stdHandle = GetStdHandle(-11);
			SafeFileHandle handle = new SafeFileHandle(stdHandle, ownsHandle: true);
			FileStream stream = new FileStream(handle, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);
			streamWriter.AutoFlush = true;
			Console.SetOut(streamWriter);
		}
		catch (Exception ex)
		{
			Debug.Log((object)("Couldn't redirect output: " + ex.Message));
		}
	}

	public void Shutdown()
	{
		Console.SetOut(oldOutput);
		FreeConsole();
	}

	public void SetTitle(string strName)
	{
		SetConsoleTitleA(strName);
	}

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool AttachConsole(uint dwProcessId);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool AllocConsole();

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool FreeConsole();

	[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr GetStdHandle(int nStdHandle);

	[DllImport("kernel32.dll")]
	private static extern bool SetConsoleTitleA(string lpConsoleTitle);
}
