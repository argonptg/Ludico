using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Dapper;
using Newtonsoft.Json;
using LudicoGTK.Utils;
using System.ComponentModel.DataAnnotations.Schema;

namespace LudicoGTK.Search;

public class HydraManager
{
    public static readonly HttpClient Client = new();
    private static string conStr = "";

    public HydraManager()
    {
        conStr = $"Data Source={Path.Combine(AppGlobals.GetDocumentsPath(), "hydra.db")}";
    }

    public static void CreateDatabase()
    {
        var dbFile = Path.Combine(AppGlobals.GetDocumentsPath(), "hydra.db");

        if (!File.Exists(dbFile))
        {
            Log.Info("Creating database file");
            File.WriteAllText(dbFile, "");

            Log.Info("Creating database structure");

            using var connection = new SqliteConnection(conStr);
            connection.Open();

            // Create FTS5 games table
            const string ftsSql = @"
                CREATE VIRTUAL TABLE games USING fts5 (
                    game_title,
                    source_file,
                    download_url,
                    file_size,
                    upload_date
                );
            ";

            using (var cmd = new SqliteCommand(ftsSql, connection))
                cmd.ExecuteNonQuery();

            // Create metadata table with unique constraint
            const string metaSql = @"
                CREATE TABLE IF NOT EXISTS games_meta (
                    game_title TEXT NOT NULL,
                    source_file TEXT NOT NULL,
                    UNIQUE(game_title, source_file)
                );
            ";

            using (var cmd = new SqliteCommand(metaSql, connection))
                cmd.ExecuteNonQuery();
        }
    }

    // TODO: Fix this shit
    public static IEnumerable<GameResult> SearchHydraSources(string query)
    {
        using var connection = new SqliteConnection(conStr);
        connection.Open();

        string searchSql = $@"
            SELECT game_title AS GameTitle, source_file AS SourceFile, download_url AS DownloadUrl, file_size AS FileSize, upload_date AS UploadDate
            FROM games 
            WHERE game_title MATCH @query;
        ";

        // pesky quote/apostrophe
        var escapedQuery = $"\"{query.Replace("\"", "\"\"")}\"";

        var search = connection.Query<GameResult>(searchSql, new { query = escapedQuery });

        return search;
    }

    public static async Task ProcessHydraSource(string url)
    {
        if (url != null)
        {
            var hydraJson = await Client.GetAsync(url);
            var hydraJsonString = await hydraJson.Content.ReadAsStringAsync();

            HydraSourceData data = JsonConvert.DeserializeObject<HydraSourceData>(hydraJsonString);

            foreach (var game in data.Downloads)
            {
                Log.Info($"Processing game: {game.Title} from source {data.Name}");
                AddGameToDatabase(game, data.Name);
            }

            Log.Info($"Cached all games from source {data.Name}!");
            return;
        }

        Log.Error("No source provided");
    }

    private static void AddGameToDatabase(DownloadInfo data, string source)
    {
        using var connection = new SqliteConnection(conStr);
        connection.Open();

        // Check if this game + source is already in games_meta
        const string checkSql = @"
            SELECT 1 FROM games_meta
            WHERE game_title = @Title AND source_file = @Source
            LIMIT 1;
        ";

        var exists = connection.ExecuteScalar<int?>(checkSql, new
        {
            Title = data.Title,
            Source = source
        });

        if (exists == 1)
        {
            Log.Info($"Skipping duplicate: {data.Title} ({source})");
            return;
        }

        // Insert into FTS5 search table
        const string insertFtsSql = @"
            INSERT INTO games (game_title, source_file, download_url, file_size, upload_date)
            VALUES (@Title, @Source, @DownloadUrl, @FileSize, @UploadDate);
        ";

        connection.Execute(insertFtsSql, new
        {
            Title = data.Title,
            Source = source,
            DownloadUrl = data.Uris[0],
            FileSize = data.FileSize,
            UploadDate = data.UploadDate
        });

        // Insert into metadata table
        const string insertMetaSql = @"
            INSERT INTO games_meta (game_title, source_file)
            VALUES (@Title, @Source);
        ";

        connection.Execute(insertMetaSql, new
        {
            Title = data.Title,
            Source = source
        });

        Log.Info($"Inserted: {data.Title}");
    }

}

public class GameResult
{
    [Column("game_title")]
    public string GameTitle { get; set; }

    [Column("source_file")]
    public string SourceFile { get; set; }

    [Column("download_url")]
    public string DownloadUrl { get; set; }

    [Column("file_size")]
    public string FileSize { get; set; }

    [Column("upload_date")]
    public string UploadDate { get; set; }
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

