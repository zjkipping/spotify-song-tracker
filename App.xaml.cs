using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace SpotifySongTracker {
  public partial class App : Application {
    public App() {

      void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
				MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
        ErrorLogger.LogToFile(e.Exception);
        e.Handled = true;
			}
		}
  }
}
