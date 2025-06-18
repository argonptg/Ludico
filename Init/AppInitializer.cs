using System;
using NLua;
using LudicoGTK;
using LudicoGTK.Ui;
using LudicoGTK.Plugin;
using System.IO;
using Gtk;

namespace LudicoGTK.Init;

public class AppInitializer
{
    private readonly MainWindow _mainWindow = AppGlobals.window;
    private readonly Lua _lua = AppGlobals.Lua;
    private static string DocumentsPath = AppGlobals.GetDocumentsPath();
    private static string PluginPath = Path.Combine(DocumentsPath, "plugins");
    private static string CachePath = Path.Combine(DocumentsPath, "cache");
    
    public void Initialize()
    {
        InitDirs();
        InitLua();
    }

    private void InitDirs()
    {
        // Ensures the dirs/files actually exists        
        Directory.CreateDirectory(PluginPath);
        Directory.CreateDirectory(CachePath);

        File.WriteAllText(
            Path.Combine(DocumentsPath, "config.json"),
            $@"{{ version: ""{AppGlobals.Version}"" }}"
        ); // ^- Creates the config file, more on that eventually
        
        // List for plugins
        var plugins = Directory.GetFiles(PluginPath);

        // Adds the plugin to the listStore
        foreach (var plugin in plugins)
        {
            var pluginName = Path.GetFileName(plugin);
            _mainWindow.AddPlugin(pluginName, plugin);
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