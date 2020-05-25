using System;
using System.IO;
using Newtonsoft.Json;

namespace SpotifySongTracker {
  public class ConfigLoader {
    private string formatDefault = " {ARTIST} - {SONG} ";
    private string prependedTextDefault = "";
    private string appendedTextDefault = "";
    private string spotifyNotOpenMessageDefault = "Spotify is not open...";
    private string noSongPlayingMessageDefault = "No song currently playing...";
    private string outputFileNameDefault = "output.txt";
    private bool outputAlbumArtDefault = true;

    private JsonSerializer serializer = new JsonSerializer();

    public Config LoadConfigFromFile(string fileName) {
      StreamReader file = null;
      Config config;
      try {
        using (file = File.OpenText(string.Format(@"{0}", fileName))) {
          config = (Config)serializer.Deserialize(file, typeof(Config));
        }
      } catch (Exception error) {
        ErrorLogger.LogToFile(error);
        config = new Config() {
          format = formatDefault,
          prependedText = prependedTextDefault,
          appendedText = appendedTextDefault,
          spotifyNotOpenMessage = spotifyNotOpenMessageDefault,
          noSongPlayingMessage = noSongPlayingMessageDefault,
          outputFileName = outputFileNameDefault,
          outputAlbumArt = outputAlbumArtDefault
        };
        UpdateConfigFile(config, fileName);
      } finally {
        if (file != null) {
          file.Close();
        }
      }
      return config;
    }

    public void UpdateConfigFile(Config config, string fileName) {
      string json = JsonConvert.SerializeObject(config, Formatting.Indented);
      try {
        File.WriteAllText(fileName, json);
      } catch (Exception error) {
        ErrorLogger.LogToFile(error);
      }
    }
  }
}
