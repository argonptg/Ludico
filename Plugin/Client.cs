using System;
using LudicoGTK.LuaPlugin;
using NLua;

namespace LudicoGTK.Plugin;

public class Client
{
    public static void AddCallback(string callbackName, LuaFunction func)
    {
        LuaCallbackManager.RegisterCallback(callbackName, func);
    }
    
    public static void LoadScript() {}
    
    public static void UnloadScript() {}
    
    public static void GetSavePath() {}

    public static void Log(string title, string text)
    {
        Console.WriteLine($"[{title}] {text}");
    }
    
    public static string GetScriptsPath()
    {
        return AppGlobals.GetDocumentsPath();
    }

    public static string GetVersion()
    {
        return AppGlobals.Version;
    }

    public static float GetVersionFloat()
    {
        return float.Parse(AppGlobals.Version);
    }
    
    public static double GetVersionDouble()
    {
        return double.Parse(AppGlobals.Version);
    }
}