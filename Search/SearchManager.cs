using IGDB;
using IGDB.Models;
using craftersmine.SteamGridDBNet;
using System.Threading.Tasks;
using System.IO;
using LudicoGTK.Utils;
using Gtk;
using System;

namespace LudicoGTK.Search;

public class SearchManager
{
    // i mean fuck it eh? these tokens are easy to get
    private static readonly string SGDBApiKey = "495049f2e68fdf828d8015658b7c41e7";

    private static readonly SteamGridDb sgdb = new(SGDBApiKey);

    public static async Task<SteamGridDbGame[]> Search(string query)
    {
        if (query.Length <= 0)
        {
            SteamGridDbGame[] fakeGame = new SteamGridDbGame[10];

            Log.Warn("Query is empty, not searching anything");
            return fakeGame;
        }

        var gameSearch = await sgdb.SearchForGamesAsync(query);

        if (gameSearch.Length < 0)
        {
            Log.Error($"No games found for query {query}");
            return gameSearch;
        }

        // now the long game, fuck
        foreach (var game in gameSearch)
        {
            var downloadPath = Path.Combine(
                AppGlobals.GetDocumentsPath(),
                "cache",
                $"{game.Id}.cache");

            var grids = await sgdb.GetGridsByGameIdAsync(
                game.Id,
                types: SteamGridDbTypes.Static,
                dimensions: SteamGridDbDimensions.W600H900
            );

            if (grids.Length == 0)
            {
                Log.Warn($"No cover found for game: {game.Name}. Downloading sample image");
                await Downloader.DownloadFileAsync($"https://dummyimage.com/600x900/000000/fff.jpg&text={game.Name}", downloadPath);
            }

            if (!File.Exists(downloadPath))
            {
                try
                {
                    // download only the first grid
                    Log.Info($"Downloading image for game: {game.Name}; on Path: {downloadPath}");
                    await Downloader.DownloadFileAsync(grids[0].FullImageUrl, downloadPath);
                }
                catch (Exception)
                {
                    Log.Error($"Failed to download grid for game {game.Name}");
                }
            }
            else
            {
                Log.Info($"Image already downloaded for {game.Name}");
            }
        }

        return gameSearch;
    }
}
