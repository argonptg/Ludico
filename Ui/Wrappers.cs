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

        // using var dialog = new Dialog();

        // dialog.Title = "Game Information";
        // dialog.TransientFor = AppGlobals.window;
        // dialog.Modal = true;
        // dialog.SetDefaultSize(500, 250);

        // var contentBox = dialog.ContentArea;
        // contentBox.BorderWidth = 10;
        // contentBox.Spacing = 6;

        // // Create the label with the game's info
        // var infoLabel = new Label($"Please select a plugin to download {game.Name}")
        // {
        //     Wrap = true
        // };

        // var comboBox = new ComboBox(AppGlobals.pluginList);
        // var cell = new CellRendererText();
        // comboBox.PackStart(cell, false);
        // comboBox.AddAttribute(cell, "text", 0);

        // contentBox.PackStart(infoLabel, true, true, 0);
        // contentBox.PackStart(comboBox, true, true, 0);

        // dialog.ShowAll();

        // dialog.Run();
    }

    public static void AddPlugin(string pluginName, string pluginPath)
    {
        AppGlobals.pluginList.AppendValues(pluginName, pluginPath);
    }

    public void CreateGameBtn(SteamGridDbGame game, Grid container, int leftPos, int topPos)
    {
        var gameBtn = new Button();
        var buttonBox = new Box(Orientation.Vertical, 5);

        const int maxImageWidth = 190;

        // Just put everything in a try/catch! what could go wrong?
        try
        {
            // Get the image path, i.e /home/$USER/Documents/Ludico/cache
            var imagePath =
                IOPath.Combine(AppGlobals.GetDocumentsPath(), "cache", $"{game.Id}.jpg");

            // I still don't know what a pixbuf is
            var originalPixbuf = new Pixbuf(imagePath);

            // Resize the image
            int targetImageHeight = (int)((float)maxImageWidth / originalPixbuf.Width * originalPixbuf.Height);
            var scaledPixbuf = originalPixbuf.ScaleSimple(maxImageWidth, targetImageHeight, InterpType.Bilinear);

            var img = new Image(scaledPixbuf);
            var label = new Label($"<b>{game.Name.Truncate(25)}</b>");

            label.UseMarkup = true;
            label.Justify = Justification.Center;

            buttonBox.PackStart(img, true, true, 0);
            buttonBox.PackStart(label, true, true, 0);

            gameBtn.Add(buttonBox);
            gameBtn.Clicked += (sender, args) =>
            {
                Log.Info($"You selected {game.Name}");
                SwitchPage(game);
            };

            // SEE I DIDN'T FORGET THIS TIME!!!!!!
            container.Attach(gameBtn, leftPos, topPos, 1, 1);
        }
        catch (Exception e)
        {
            // Well I mean _if_ it goes wrong
            Log.Error($"Failed to add button to the UI. Log: {e}");
        }
    }
}
