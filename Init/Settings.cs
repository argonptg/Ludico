using System;
using System.Collections.Generic;
using System.IO;
using LudicoGTK.Utils;
using Newtonsoft.Json;

namespace LudicoGTK.Init;

public class Settings
{
    private static string settingsPath = Path.Combine(AppGlobals.GetDocumentsPath(), "config.json");

    public static void CreateSettingsFile()
    {

        if (File.Exists(settingsPath))
        {
            Log.Info("Settings file already exists");
            return; // early return
        }
        else
        {
            var baseFile = new SettingsModel()
            {
                Version = AppGlobals.Version,
                SGDBKey = "",
                HydraSources = new List<string>
                    { "https://hydralinks.pages.dev/sources/steamrip.json" }
            };

            string serializedSettings = JsonConvert.SerializeObject(baseFile, Formatting.Indented);

            try
            {
                File.WriteAllText(settingsPath, serializedSettings);
            }
            catch (Exception e)
            {
                Log.Error($"Failed to write settings file. Error: {e}");
            }
        }
    }

    public static SettingsModel ReadSettings()
    {
        try
        {
            if (File.Exists(settingsPath))
            {
                var settingsFile = File.ReadAllText(settingsPath);

                return JsonConvert.DeserializeObject<SettingsModel>(settingsFile);
            }

            Log.Warn("Settings file doesn't exist, creating one");
            CreateSettingsFile();

            return ReadSettings(); // wow recursion actually useful for once
        }
        catch (Exception e)
        {
            Log.Error($"Failed to read settings file! Error: {e}");
            return null;
        }
    }
}

public class SettingsModel
{
    public string Version { get; set; }
    public string SGDBKey { get; set; }
    public List<string> HydraSources { get; set; }
}
