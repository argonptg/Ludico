using NLua;
using LudicoGTK.Ui;
using LudicoGTK.Plugin;
using System.IO;
using LudicoGTK.Search;
using LudicoGTK.Utils;
using System.Threading.Tasks;

namespace LudicoGTK.Init;

public class AppInitializer
{
    private readonly Lua _lua = AppGlobals.Lua;
    public static string DocumentsPath = AppGlobals.GetDocumentsPath();
    private static string PluginPath = Path.Combine(DocumentsPath, "plugins");
    private static string CachePath = Path.Combine(DocumentsPath, "cache");

    public async Task Initialize()
    {
        HydraManager.CreateDatabase();
        InitDirs();
        await InitHydraDB();
        InitLua();
    }
    
    private void InitDirs()
    {
        // Ensures the dirs/files actually exists        
        Directory.CreateDirectory(PluginPath);
        Directory.CreateDirectory(CachePath);

        Settings.CreateSettingsFile();

        // List for plugins
        var plugins = Directory.GetFiles(PluginPath);

        // Adds the plugin to the listStore
        foreach (var plugin in plugins)
        {
            var pluginName = Path.GetFileName(plugin);
            Wrappers.AddPlugin(pluginName, plugin);
        }
    }

    private async Task InitHydraDB()
    {
        var settings = Settings.ReadSettings();

        foreach (var source in settings.HydraSources)
        {
            Log.Info($"Caching source with URL: {source}");
            await HydraManager.ProcessHydraSource(source);
        }
    }

    private void InitLua()
    {
        _lua.DoString("JsonWrapper = {}");
        _lua.DoString("Notifications = {}");
        _lua.DoString("client = {}");
        _lua.DoString("http = {}");
        _lua.DoString("game = {}");

        // json
        _lua.RegisterFunction(
            "JsonWrapper.parse",
            null,
            typeof(JsonWrapper).GetMethod("ParseJson"));

        // notifications
        _lua.RegisterFunction(
            "Notifications.push",
            null,
            typeof(Notifications).GetMethod("Push"));
        _lua.RegisterFunction(
            "Notifications.push_success",
            null,
            typeof(Notifications).GetMethod("PushSuccess"));
        _lua.RegisterFunction(
            "Notifications.push_error",
            null,
            typeof(Notifications).GetMethod("PushError"));
        _lua.RegisterFunction(
            "Notifications.push_warning",
            null,
            typeof(Notifications).GetMethod("PushWarning"));

        // client
        _lua.RegisterFunction(
            "client.add_callback",
            null,
            typeof(Client).GetMethod("AddCallback"));
        _lua.RegisterFunction(
            "client.log",
            null,
            typeof(Client).GetMethod("Log"));
        _lua.RegisterFunction(
            "client.GetScriptsPath",
            null,
            typeof(Client).GetMethod("GetScriptsPath"));
        _lua.RegisterFunction(
            "client.GetVersion",
            null,
            typeof(Client).GetMethod("GetVersion"));
        _lua.RegisterFunction(
            "client.GetVersionFloat",
            null,
            typeof(Client).GetMethod("GetVersionFloat"));
        _lua.RegisterFunction(
            "client.GetVersionDouble",
            null,
            typeof(Client).GetMethod("GetVersionDouble"));

        // http
        _lua.RegisterFunction(
            "http.get",
            null,
            typeof(Http).GetMethod("Get"));

        // game
        _lua.RegisterFunction(
            "game.getgamename",
            null,
            typeof(Game).GetMethod("GetGameName"));
    }
}