using System;
using System.Windows;

namespace SpotifySongTracker {
  public partial class MainWindow : Window {
    TrayIcon trayIcon;
    SpotifyTitleWatcher spotifyTitleWatcher = new SpotifyTitleWatcher();

    public MainWindow() {
      InitializeComponent();
      LoadedConfig.RefreshConfig();
      this.SetupTrayIcon();
      OutputFileManager.SetupOutputFolder();

      this.spotifyTitleWatcher.SpotifyWindowTitleChange += (string title, WindowTitleChangeType type) => {
        this.Dispatcher.Invoke(() => {
          CurrentlyPlaying.Content = title;
        });

        if (type == WindowTitleChangeType.PlayingTrack) {
          OutputFileManager.UpdateOutputFromNewTrackTitle(title);
          this.trayIcon.ResetToDefaultText();
        } else {
          this.trayIcon.SetText(title);
        }
      };

      spotifyTitleWatcher.Start();
    }

    protected override void OnStateChanged(EventArgs e) {
      if (this.WindowState == WindowState.Minimized) {
        this.Hide();
      }
      base.OnStateChanged(e);
    }

    protected void SetupTrayIcon() {
      this.Hide();
      this.trayIcon = new TrayIcon();
      this.trayIcon.RefreshConfigMenuItemClicked += () => LoadedConfig.RefreshConfig();
      this.trayIcon.ExitMenuItemClicked += () => this.Close();
      this.trayIcon.ShowMenuItemClicked += () => {
        this.Show();
        this.WindowState = WindowState.Normal;
      };
      this.trayIcon.TrayIconClicked += () => {
        this.Show();
        this.WindowState = WindowState.Normal;
      };
    }
  }
}
