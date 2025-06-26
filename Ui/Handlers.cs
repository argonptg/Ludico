using System;

using Gtk;

using LudicoGTK.Utils;
using LudicoGTK.Search;
using IGDB.Models;

namespace LudicoGTK.Ui;

public class Handlers
{
    private readonly Wrappers wrappers = new();

    public async void SearchBar_Handler(object sender, EventArgs a)
    {
        Log.Info($"Search results for '{AppGlobals.searchBar.Text.ToLower()}'");

        var search = await SearchManager.Search(AppGlobals.searchBar.Text.ToLower());

        // Clear out everything
        while (AppGlobals.resultsBox.Children.Length > 0)
        {
            AppGlobals.resultsBox.Remove(AppGlobals.resultsBox.Children[0]);
        }

        if (search != null)
        {
            /*
            * Note to self:
            * NEVER TRY TO CODE SICK AGAIN HOLY SHIT I CAN'T GET
            * ANYTHING DONE SO I GOT GEMINI TO FIGURE OUT THE SIMPLEST
            * STUFF
            */
            foreach (var game in search)
            {

                // Tends to error out a lot, so I just said fuck it
                // and put it on a try/catch
                try
                {
                    if (game == null)
                    {
                        continue;
                    }

                    var label = new Label(game.Name.Truncate(25)); // eh, 25 is good enough

                    wrappers.CreateGameBtn(game, AppGlobals.resultsBox);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while processing the game, Skipping.\nDetails: {e}");
                }
            }
        }

        AppGlobals.resultsBox.ShowAll();
    }
}
