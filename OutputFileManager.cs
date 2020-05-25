using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;

namespace SpotifySongTracker {

  public static class OutputFileManager {
    public static string ARTIST_FORMAT_KEYWORD = "{ARTIST}";
    public static string SONG_FORMAT_KEYWORD = "{SONG}";
    public static string ALBUM_FORMAT_KEYWORD = "{ALBUM}";

    private static List<string> songCacheKeys = new List<string>();
    private static Dictionary<string, TrackInfo> songCache = new Dictionary<string, TrackInfo>();
    private static string AlbumArtPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "SpotifySongTracker", "album_art.png");

    public static void SetupOutputFolder() {
      string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "SpotifySongTracker");
      Directory.CreateDirectory(path);
    }

    public static async void UpdateOutputFromNewTrackTitle(string trackTitle) {
      TrackInfo track = null;
      if (!songCache.TryGetValue(trackTitle, out track) || track == null) {
        track = await SpotifyApiManager.GetTrackInfo(trackTitle);
        if (songCacheKeys.Count > 10) {
          string key = songCacheKeys[0];
          songCache.Remove(key);
          songCacheKeys.RemoveAt(0);
        } else {
          songCacheKeys.Remove(trackTitle);
        }
        songCache.Add(trackTitle, track);
        songCacheKeys.Add(trackTitle);
      }
      if (track != null) {
        var formattedTrackTitle = GenerateFormattedTrackTitle(track);
        var output = string.Format("{0}{1}{2}", LoadedConfig.config.prependedText, formattedTrackTitle, LoadedConfig.config.appendedText);
        UpdateOutputFile(output);
        if (LoadedConfig.config.outputAlbumArt) {
          DownloadAlbumArt(track.albumArtUrl);
        }
      } else {
        UpdateOutputFromNonTrackTitle(trackTitle);
      }
    }

    public static void UpdateOutputFromNonTrackTitle(string title) {
      try {
        if (File.Exists(AlbumArtPath)) {
          File.Delete(AlbumArtPath);
        }
      } catch (Exception error) {
        ErrorLogger.LogToFile(error);
      }
      UpdateOutputFile(string.Format(" {0} ", title));
    }

    public static void UpdateOutputFile(string output) {
      StreamWriter file = null;
      try {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDoc‌​uments), "SpotifySongTracker", LoadedConfig.config.outputFileName);
        using (file = File.CreateText(path)) {
          file.WriteLine(output);
        }
      } catch (Exception error) {
        ErrorLogger.LogToFile(error);
      } finally {
        if (file != null) {
          file.Close();
        }
      }
    }

    private static string GenerateFormattedTrackTitle(TrackInfo track) {
      var format = LoadedConfig.config.format;
      var artist_inserted = format.Replace(ARTIST_FORMAT_KEYWORD, track.artist);
      var song_inserted = artist_inserted.Replace(SONG_FORMAT_KEYWORD, track.song);
      return song_inserted.Replace(ALBUM_FORMAT_KEYWORD, track.album);
    }

    private static void DownloadAlbumArt(string url) {
      WebClient webClient = new WebClient();
      webClient.DownloadFile(url, AlbumArtPath);
    }
  }
}
