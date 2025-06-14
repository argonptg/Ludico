using NLua;
using Newtonsoft.Json.Linq;

namespace LudicoGTK.Plugin;

public class JsonWrapper
{
    public static LuaTable ParseJson(Lua lua, string json)
    {
        var token = JToken.Parse(json);
        return ToLuaTable(lua, token);
    }

    private static LuaTable ToLuaTable(Lua lua, JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
                lua.DoString("jsonObj = {}");
                var tableObj = lua.GetTable("jsonObj");

                foreach (var prop in ((JObject)token).Properties())
                {
                    tableObj[prop.Name] = ConvertToken(lua, prop.Value);
                }

                return tableObj;

            case JTokenType.Array:
                lua.DoString("jsonArr = {}");
                var tableArr = lua.GetTable("jsonArr");

                int i = 1; // Lua arrays are 1-based
                foreach (var item in (JArray)token)
                {
                    tableArr[i++] = ConvertToken(lua, item);
                }

                return tableArr;

            default:
                // Shouldn't hit this in ToLuaTable
                // But just in case
                return null;
        }
    }
    
    private static object ConvertToken(Lua lua, JToken token)
    {
        switch (token.Type)
        {
            case JTokenType.Object:
            case JTokenType.Array:
                return ToLuaTable(lua, token);

            case JTokenType.Integer:
                return token.Value<int>();

            case JTokenType.Float:
                return token.Value<double>();

            case JTokenType.String:
                return token.Value<string>();

            case JTokenType.Boolean:
                return token.Value<bool>();

            case JTokenType.Null:
            case JTokenType.Undefined:
                return null;

            default:
                return token.ToString();
        }
    }
}