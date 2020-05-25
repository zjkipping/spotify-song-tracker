using System;
using System.IO;

namespace SpotifySongTracker {
  public static class ErrorLogger {
    public static void LogToFile(Exception e) {
      StreamWriter file = null;
      try {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "SpotifySongTracker", "errors.log");
        File.AppendAllLines(path, new string[] {
            DateTime.Now.ToString(),
            e.Message,
            e.StackTrace
        });
      } catch {
        // meh
      } finally {
        if (file != null) {
          file.Close();
        }
      }
    }
  }
}
