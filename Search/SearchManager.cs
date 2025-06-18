using IGDB;
using IGDB.Models;
using System.Threading.Tasks;
using System;
using System.IO;
using LudicoGTK.Utils;

namespace LudicoGTK.Search;

public class SearchManager
{
    // i mean fuck it eh? these tokens are easy to get
    private static readonly string IGDBClientId = "ri2blwejzic370dkj5sbktwn1o66ju";
    private static readonly string IGDBClientSecret = "k93bz8da1gf6baqaipem1nvyhx4td3";

    public static async Task<Game[]> Search(string query)
    {
        var igdb = IGDBClient.CreateWithDefaults(
            IGDBClientId,
            IGDBClientSecret
        );

        var ids = "0,3,6,14,4,5,7,8,11,12,15,16,18,19,21,22,23,25,26,27,29,30,32,33,35,37,38,41,42,44,50,48,49,130,167,169";
        // no idea why 0 is required, as there's no docs for it

        var result = await igdb.QueryAsync<Game>(
            IGDBClient.Endpoints.Games,
            query: $@"
            fields age_ratings, aggregated_rating, cover.*, game_engines, genres.name, name, platforms.abbreviation, tags, url;
            where (category = ({ids})) 
                & name ~ ""{query}""*
                & version_parent = null
                & parent_game = null;
            sort aggregated_rating desc;
            limit 20;"
        );

        foreach (var game in result)
        {
            var downloadPath = Path.Combine(
                AppGlobals.GetDocumentsPath(),
                "cache",
                $"{game.Id}.jpg");

            if (game?.Cover?.Value?.Url != null)
            {
                var url = game.Cover.Value.Url
                        .Replace("//", "https://")
                        .Replace("t_thumb", "t_cover_big_2x");

                if (!File.Exists(downloadPath))
                {
                    Log.Info($"Downloading image for game: {game.Name}; on Path: {downloadPath}");

                    await Downloader.DownloadFileAsync(url, downloadPath);
                }
            }
            else
            {
                Log.Warn($"No cover found for game: {game.Name}. Downloading sample image");
                await Downloader.DownloadFileAsync($"https://dummyimage.com/190x253/000000/fff.jpg&text={game.Name}", downloadPath);
            }
        }


        return result;
    }
}
