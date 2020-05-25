using System.Diagnostics;
using System.Windows.Forms;

namespace SpotifySongTracker {
  class SystemProcessHookForm : Form {
    private readonly int msgNotify;
    public delegate void EventHandler(object sender, Process Process);
    public event EventHandler WindowCreated;
    protected virtual void OnWindowCreated(Process process) {
      var handler = WindowCreated;
      if (handler != null) {
        handler(this, process);
      }
    }

    public SystemProcessHookForm() {
      // Hook on to the shell
      msgNotify = Interop.RegisterWindowMessage("SHELLHOOK");
      Interop.RegisterShellHookWindow(this.Handle);
    }

    protected override void WndProc(ref Message m) {
      if (m.Msg == msgNotify) {
        // Receive shell messages
        var hwnd = m.LParam;
        switch ((Interop.ShellEvents)m.WParam.ToInt32()) {
          case Interop.ShellEvents.HSHELL_WINDOWCREATED:
            Interop.GetWindowThreadProcessId(hwnd, out uint processId);
            OnWindowCreated(Process.GetProcessById((int)processId));
            break;
        }
      }
      base.WndProc(ref m);
    }

    protected override void Dispose(bool disposing) {
      try { Interop.DeregisterShellHookWindow(this.Handle); } catch { }
      base.Dispose(disposing);
    }
  }
}
