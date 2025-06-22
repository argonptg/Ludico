using System;
using System.IO;
using Gtk;
using NLua;
using LudicoGTK.Ui;

namespace LudicoGTK;

public static class AppGlobals
{
    public static Application appInstance { get; set; }
    public static MainWindow window { get; set; }
    public static readonly Lua Lua = new Lua();
    public static readonly string Version = "0.1";

    // some elemnentwnts
    public static ListStore pluginList { get; set; }
    public static SearchEntry searchBar { get; set; }
    public static FlowBox resultsBox { get; set; }
    public static Stack mainStack { get; set; }
    
    public const string AppIdentifier = "uk.argonptg.Ludico";

    public static string GetDocumentsPath()
    {
        string basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            basePath = Path.Combine(basePath, "Documents");
        else
            throw new Exception("Operating system not supported");
        
        return Path.Combine(basePath, "Ludico");
    }
}

