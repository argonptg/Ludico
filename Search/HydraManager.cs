using Dapper;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Data.SQLite;
using Newtonsoft.Json;
using LudicoGTK.Utils;

namespace LudicoGTK.Search;

public class HydraManager
{
    public static readonly HttpClient Client = new();

    public static void CreateDatabase()
    {
        var dbFile = Path.Combine(AppGlobals.GetDocumentsPath(), "hydra.db");

        if (!File.Exists(dbFile))
        {
            // create the file if it doesn't exist
            File.WriteAllText(dbFile, "");
        }
    }

    // use the sample url for testing, remove later
    public async Task ProcessHydraSource(string url = "https://hydralinks.pages.dev/sources/steamrip.json")
    {
        var hydraJson = await Client.GetAsync(url);
        var hydraJsonString = await hydraJson.Content.ReadAsStringAsync();

        HydraSourceData data = JsonConvert.DeserializeObject<HydraSourceData>(hydraJsonString);

        foreach (var game in data.Downloads) {
            Console.WriteLine(game.Title);
        }
    }

    private static void AddGameToDatabase()
    {
        var conStr = $"Data Source={Path.Combine(AppGlobals.GetDocumentsPath(), "hydra.db")}";

        using var connection = new SQLiteConnection(conStr);
    }
}

public class DownloadInfo
{
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("uploadDate")]
    public DateTime UploadDate { get; set; }

    [JsonProperty("fileSize")]
    public string FileSize { get; set; }

    [JsonProperty("uris")]
    public List<string> Uris { get; set; }
}

public class HydraSourceData
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("downloads")]
    public List<DownloadInfo> Downloads { get; set; }
}

