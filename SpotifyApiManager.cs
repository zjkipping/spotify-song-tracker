using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Diagnostics;
using System;

namespace SpotifySongTracker {
  public class TrackInfo {
    public string song;
    public string artist;
    public string album;
    public string albumArtUrl;
  }

  public static class SpotifyApiManager {
    private static readonly HttpClient client = new HttpClient();

    [Conditional("DEBUG")]
    private static void IsDebugCheck(ref bool isDebug) {
      isDebug = true;
    }

    public static async Task<TrackInfo> GetTrackInfo(string query) {
      try {
        bool isDebug = false;
        IsDebugCheck(ref isDebug);
        string apiUrl = isDebug ? "http://localhost:8080/api" : "http://outis-spotify-rest.us-east-1.elasticbeanstalk.com/api";
        var response = await client.GetStringAsync(string.Format("{0}/track?title={1}", apiUrl, query));
        TrackInfo track = JsonConvert.DeserializeObject<TrackInfo>(response);
        return track;
      } catch (Exception error) {
        ErrorLogger.LogToFile(error);
        return null;
      }
    }
  }
}
