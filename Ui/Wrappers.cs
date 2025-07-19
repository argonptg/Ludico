using System;
using craftersmine.SteamGridDBNet;
using Gdk;
using Gtk;
using LudicoGTK.Search;
using LudicoGTK.Utils;
using IOPath = System.IO.Path;
using LudicoGTK.Ui.GamePage;

namespace LudicoGTK.Ui;

public class Wrappers
{
    public static void SwitchPage(SteamGridDbGame game)
    {
        var pageName = "game_page";

        Log.Info($"Creating page for {game.Name}");
        Widget page = AppGlobals.mainStack.GetChildByName(pageName);

        var heroPath = IOPath.Combine(
            AppGlobals.GetDocumentsPath(),
            "cache",
            $"{game.Id}-hero.cache"
        );

        var pixbufImg = new Pixbuf(heroPath);

        if (page is GameResultPage gameDetailPage)
        {
            gameDetailPage.Update(game, pixbufImg);
        }

        if (page == null)
        {
            Log.Info("Game page not found, creating it for the first time.");

            var newGamePage = new GameResultPage(game, pixbufImg);

            AppGlobals.mainStack.AddNamed(newGamePage, pageName);
            newGamePage.Update(game, pixbufImg);
            newGamePage.ShowAll();
            page = newGamePage;
        }

        AppGlobals.mainStack.VisibleChildName = pageName;

        // hydra testing stuff
        var testSearch = HydraManager.SearchHydraSources(game.Name);

        foreach (var searchedGame in testSearch)
        {
            // does not use Log class, i know, this is for
            // testing anyways so fuck you
            Console.WriteLine($"Name: {searchedGame.GameTitle}, Source: {searchedGame.SourceFile}");
        }
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
