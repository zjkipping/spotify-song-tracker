using System;
using System.Runtime.InteropServices;

namespace SpotifySongTracker {
  public static class Interop {
    public enum ShellEvents : int {
      HSHELL_WINDOWCREATED = 1,
      HSHELL_WINDOWDESTROYED = 2,
      HSHELL_ACTIVATESHELLWINDOW = 3,
      HSHELL_WINDOWACTIVATED = 4,
      HSHELL_GETMINRECT = 5,
      HSHELL_REDRAW = 6,
      HSHELL_TASKMAN = 7,
      HSHELL_LANGUAGE = 8,
      HSHELL_ACCESSIBILITYSTATE = 11,
      HSHELL_APPCOMMAND = 12
    }

    [DllImport("user32.dll", EntryPoint = "RegisterWindowMessageA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern int RegisterWindowMessage(string lpString);

    [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern int DeregisterShellHookWindow(IntPtr hWnd);

    [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern int RegisterShellHookWindow(IntPtr hWnd);

    [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    [DllImport("user32.dll")]
    public static extern IntPtr SetWinEventHook(
                uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc,
                uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    // Constants from winuser.h
    public static uint EVENT_OBJECT_DESTROY = 0x8001;
    public static uint EVENT_OBJECT_NAMECHANGE = 0x800C;
    public static uint WINEVENT_OUTOFCONTEXT = 0;
  }
}
