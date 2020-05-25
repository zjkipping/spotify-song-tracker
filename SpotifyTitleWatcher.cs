using System;
using System.Diagnostics;
using System.Linq;

namespace SpotifySongTracker {
  public enum WindowTitleChangeType {
    PlayingTrack,
    NoTrack,
    NoSpotify
  }

  public delegate void SpotifyWindowTitleChangeEventHandler(string title, WindowTitleChangeType type);

  public class SpotifyTitleWatcher {
    public event SpotifyWindowTitleChangeEventHandler SpotifyWindowTitleChange;

    private Interop.WinEventDelegate spotifyWindowNameChangeDelegate;
    private IntPtr spotifyWindowNameChangeHook;

    private Interop.WinEventDelegate spotifyWindowDestroyDelegate;
    private IntPtr spotifyWindowDestroyHook;

    private Process spotifyProc {
      get {
        var procs = Process.GetProcessesByName("Spotify");
        return procs.FirstOrDefault(proc => proc.MainWindowTitle != "");
      }
    }

    private string lastTitle = "";

    public void Start() {
      spotifyWindowNameChangeDelegate = new Interop.WinEventDelegate(this.SpotifyEventNameChange);
      spotifyWindowDestroyDelegate = new Interop.WinEventDelegate(this.SpotifyWindowDestroy);

      this.StartupSpotifyWatch();

      var processForm = new SystemProcessHookForm();
      processForm.WindowCreated += (sender, process) => {
        if (process.ProcessName == "Spotify") {
          this.StartupSpotifyWatch();
        }
      };
    }

    private void TitleChange(string title, WindowTitleChangeType type) {
      if (lastTitle != title) {
        lastTitle = title;
        this.SpotifyWindowTitleChange?.Invoke(title, type);
      }
    }

    private void StartupSpotifyWatch() {
      if (this.spotifyProc != null) {
        this.EmitMainWindowTitle();
        this.SetupSpotifyNameChangeHook();
        this.SetupWindowDestroyHook();
      } else {
        TitleChange(LoadedConfig.config.spotifyNotOpenMessage, WindowTitleChangeType.NoSpotify);
      }
    }

    private void EmitMainWindowTitle() {
      var title = this.spotifyProc.MainWindowTitle;
      if (!title.Contains("-") && title.Contains("Spotify")) {
        TitleChange(LoadedConfig.config.noSongPlayingMessage, WindowTitleChangeType.NoTrack);
      } else {
        TitleChange(title, WindowTitleChangeType.PlayingTrack);
      }
    }

    private void SetupWindowDestroyHook() {
      this.spotifyWindowDestroyHook = Interop.SetWinEventHook(
        Interop.EVENT_OBJECT_DESTROY,
        Interop.EVENT_OBJECT_DESTROY,
        IntPtr.Zero,
        spotifyWindowDestroyDelegate,
        (uint)spotifyProc.Id,
        0,
        Interop.WINEVENT_OUTOFCONTEXT
      );
    }

    private void SpotifyWindowDestroy(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
      TitleChange(LoadedConfig.config.spotifyNotOpenMessage, WindowTitleChangeType.NoSpotify);
      Interop.UnhookWinEvent(spotifyWindowNameChangeHook);
      Interop.UnhookWinEvent(spotifyWindowDestroyHook);
    }

    private void SpotifyEventNameChange(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime) {
      this.EmitMainWindowTitle();
    }

    private void SetupSpotifyNameChangeHook() {
      this.spotifyWindowNameChangeHook = Interop.SetWinEventHook(
        Interop.EVENT_OBJECT_NAMECHANGE,
        Interop.EVENT_OBJECT_NAMECHANGE,
        IntPtr.Zero,
        spotifyWindowNameChangeDelegate,
        (uint)spotifyProc.Id,
        0,
        Interop.WINEVENT_OUTOFCONTEXT
      );
    }
  }
}
