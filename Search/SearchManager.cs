using System.Net.Http;
using IGDB;
using IGDB.Models;
using System.Threading.Tasks;

namespace LudicoGTK.Search;

public class SearchManager
{
    private static readonly string IGDBClientId = "ri2blwejzic370dkj5sbktwn1o66ju";
    private static readonly string IGDBAccessToken = "vqzagwkbsdk7ttrgod6fa5d8031lqy";
    private static readonly string IGDBClientSecret = "k93bz8da1gf6baqaipem1nvyhx4td3";
    
    public static async Task<Game[]> Search(string query)
    {
        var igdb = IGDBClient.CreateWithDefaults(
            IGDBClientId,
            IGDBClientSecret
        );

        var result = await igdb.QueryAsync<Game>(
            IGDBClient.Endpoints.Games,
            query: $@"fields age_ratings,aggregated_rating,cover.*,game_type,game_engines,genres,name,tags,url,version_parent; search ""{query}""; limit 30;");
        
        return result;
    }
}
