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
    
    public void Initialize()
    {
        InitDirs();
        InitLua();
    }

    private void InitDirs()
    {
        // Ensures the dirs/files actually exists
        Console.WriteLine(DocumentsPath);
        
        Directory.CreateDirectory(PluginPath);
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
        // global "class" for json
        _lua.DoString("JsonWrapper = {}"); 
        _lua.RegisterFunction(
            "json_parse_internal", 
            null, 
            typeof(JsonWrapper).GetMethod("ParseJson"));
        _lua.DoString("JsonWrapper.parse = json_parse_internal");
            
        // notifications
        _lua.DoString("Notifications = {}");
        _lua.RegisterFunction(
            "internal_push",
            null,
            typeof(Notifications).GetMethod("Push"));
        _lua.RegisterFunction(
            "internal_success",
            null,
            typeof(Notifications).GetMethod("PushSuccess"));
        _lua.RegisterFunction(
            "internal_error",
            null,
            typeof(Notifications).GetMethod("PushError"));
        _lua.RegisterFunction(
            "internal_warning",
            null,
            typeof(Notifications).GetMethod("PushWarning"));
        _lua.DoString("Notifications.push = internal_push");
        _lua.DoString("Notifications.push_success = internal_success");
        _lua.DoString("Notifications.push_error = internal_error");
        _lua.DoString("Notifications.push_warning = internal_warning");
        
        // client
        _lua.DoString("client = {}");
        _lua.RegisterFunction(
            "internal_addcb",
            null,
            typeof(Client).GetMethod("AddCallback"));
        _lua.RegisterFunction(
            "internal_log",
            null,
            typeof(Client).GetMethod("Log"));
        _lua.RegisterFunction(
            "internal_scriptspath",
            null,
            typeof(Client).GetMethod("GetScriptsPath"));
        _lua.RegisterFunction(
            "internal_version",
            null,
            typeof(Client).GetMethod("GetVersion"));
        _lua.RegisterFunction(
            "internal_verfloat",
            null,
            typeof(Client).GetMethod("GetVersionFloat"));
        _lua.RegisterFunction(
            "internal_verdouble",
            null,
            typeof(Client).GetMethod("GetVersionDouble"));
        
        _lua.DoString("client.add_callback = internal_addcb");
        _lua.DoString("client.log = internal_log");
        _lua.DoString("client.GetVersion = internal_version");
        _lua.DoString("client.GetVersionFloat = internal_verfloat");
        _lua.DoString("client.GetVersionDouble = internal_verdouble");
        
        // http
        _lua.DoString("http = {}");
        _lua.RegisterFunction(
            "internal_get",
            null,
            typeof(Http).GetMethod("Get"));
        
        _lua.DoString("http.get = internal_get");
    }
}