using System;
using System.IO;

namespace SpotifySongTracker {
  public static class LoadedConfig {
    public static Config config;
    private static ConfigLoader loader = new ConfigLoader();
    private static string configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "SpotifySongTracker", "config.json");

    public static void RefreshConfig() {
      config = loader.LoadConfigFromFile(configFilePath);
    }
  }
}
