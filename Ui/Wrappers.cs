using System;
using craftersmine.SteamGridDBNet;
using Gdk;
using Gtk;
using IGDB.Models;
using LudicoGTK.Utils;
using IOPath = System.IO.Path;

namespace LudicoGTK.Ui;

public class Wrappers
{
    public static void SwitchPage(SteamGridDbGame game)
    {
        var pageName = "game_page";

        Log.Info($"Creating page for {game.Name}");
        Widget oldPage = AppGlobals.mainStack.GetChildByName(pageName);

        if (oldPage != null)
        {
            Log.Info("Removing old page");
            AppGlobals.mainStack.Remove(oldPage);
        }

        // Main page design starts here
        var gameContainer = new Box(Orientation.Vertical, 10)
        {
            new Label(game.Name)
        };

        AppGlobals.mainStack.AddNamed(gameContainer, pageName);

        gameContainer.ShowAll();

        AppGlobals.mainStack.VisibleChildName = pageName;
    }

    public static void AddPlugin(string pluginName, string pluginPath)
    {
        AppGlobals.pluginList.AppendValues(pluginName, pluginPath);
    }

    public void CreateGameBtn(SteamGridDbGame game, FlowBox container)
    {
        var gameBtn = new Button();

        // Just put everything in a try/catch! what could go wrong?
        try
        {
            // Get the image path, i.e /home/$USER/Documents/Ludico/cache
            var imagePath =
                IOPath.Combine(AppGlobals.GetDocumentsPath(), "cache", $"{game.Id}.cache");

            // I still don't know what a pixbuf is
            var originalPixbuf = new Pixbuf(imagePath);

            const int targetWidth = 200; // Or whatever width you prefer

            int targetHeight = (int)((float)targetWidth / originalPixbuf.Width * originalPixbuf.Height);
            var scaledPixbuf = originalPixbuf.ScaleSimple(targetWidth, targetHeight, InterpType.Bilinear);
            var img = new Image(scaledPixbuf);

            gameBtn.Add(img);

            gameBtn.Clicked += (sender, args) =>
            {
                Log.Info($"You selected {game.Name}");
                SwitchPage(game);
            };

            // SEE I DIDN'T FORGET THIS TIME!!!!!!
            container.Add(gameBtn);
        }
        catch (Exception e)
        {
            // Well I mean _if_ it goes wrong
            Log.Error($"Failed to add button to the UI. Log: {e}");
        }
    }

}
