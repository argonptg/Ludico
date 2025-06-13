using System;
using System.IO;
using Gtk;

namespace LudicoGTK;

public static class AppGlobals
{
    public static Application appInstance { get; set; }
    public const string AppIdentifier = "uk.argonptg.Ludico";

    public static string GetLocalDataPath()
    {
        string basePath;

        if (OperatingSystem.IsWindows())
        {
            basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            
        } 
        else if (OperatingSystem.IsLinux())
        {
            string home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            basePath = Path.Combine(home, ".local", "share");
        }
        else
        {
            throw new Exception("Operating system not supported");
        }
        
        return Path.Combine(basePath, AppIdentifier);
    }
}

