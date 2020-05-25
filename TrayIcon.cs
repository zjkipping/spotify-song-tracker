using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpotifySongTracker {
  public delegate void TrayIconClickEventHandler();
  public delegate void TrayMenuItemClickEventHandler();

  public class TrayIcon {
    private NotifyIcon icon = new NotifyIcon();
    private string defaultText = "Spotify Song Tracker";

    public event TrayIconClickEventHandler TrayIconClicked;
    public event TrayMenuItemClickEventHandler ExitMenuItemClicked;
    public event TrayMenuItemClickEventHandler ShowMenuItemClicked;
    public event TrayMenuItemClickEventHandler RefreshConfigMenuItemClicked;

    public TrayIcon() {
      this.icon.Icon = new Icon("Assets/SpotifySongTracker.ico");
      this.icon.Visible = true;
      this.icon.Text = defaultText;
      this.icon.MouseClick += delegate (object sender, MouseEventArgs args) {
        if (args.Button == MouseButtons.Left) {
          TrayIconClicked?.Invoke();
        }
      };

      var contextMenu = new ContextMenu();
      var exitMenuItem = new MenuItem();
      var showMenuItem = new MenuItem();
      var refreshConfigMenuItem = new MenuItem();
      var menuItems = new MenuItem[] { exitMenuItem, refreshConfigMenuItem, showMenuItem };

      contextMenu.MenuItems.AddRange(menuItems);

      exitMenuItem.Index = 2;
      exitMenuItem.Text = "Exit";
      exitMenuItem.Click += (object sender, EventArgs e) => this.ExitMenuItemClicked?.Invoke();

      showMenuItem.Index = 0;
      showMenuItem.Text = "Show";
      showMenuItem.Click += (object sender, EventArgs e) => this.ShowMenuItemClicked?.Invoke();

      refreshConfigMenuItem.Index = 1;
      refreshConfigMenuItem.Text = "Refresh Config";
      refreshConfigMenuItem.Click += (object sender, EventArgs e) => this.RefreshConfigMenuItemClicked?.Invoke();

      icon.ContextMenu = contextMenu;
    }

    public void ResetToDefaultText() {
      this.icon.Text = defaultText;
    }

    public void SetText(string text) {
      this.icon.Text = text;
    }
  }
}
