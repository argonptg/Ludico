using NLua;
using System.Collections.Generic;
using System.Net.Http;

namespace LudicoGTK.Plugin;

public class Http
{
    private static HttpClient _client = new HttpClient();
    
    public static string Get(string link, LuaTable headers)
    {
        _client.DefaultRequestHeaders.Clear();

        foreach (KeyValuePair<object, object> header in headers)
        {
            _client.DefaultRequestHeaders.Add(header.Key.ToString(), header.Value.ToString());
        }
        
        var response = _client.GetAsync(link).GetAwaiter().GetResult();
        var body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        
        return body;
    }
}