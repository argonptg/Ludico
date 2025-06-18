using System;

using Gtk;

using LudicoGTK.Utils;
using LudicoGTK.Search;

namespace LudicoGTK.Ui;

public class Handlers
{
    private readonly Wrappers wrappers = new();

    public async void SearchBar_Handler(object sender, EventArgs a)
    {
        Log.Info($"Search results for '{AppGlobals.searchBar.Text}'");

        var search = await SearchManager.Search(AppGlobals.searchBar.Text);

        // NEW: Define your max rows and columns here
        int maxCol = 3;
        int maxRow = 5; // For example, stop after 5 rows are filled

        // Clear out everything
        while (AppGlobals.resultsBox.Children.Length > 0)
        {
            AppGlobals.resultsBox.Remove(AppGlobals.resultsBox.Children[0]);
        }

        int topPos = 0;
        int leftPos = 0;

        /*
         * Note to self:
         * NEVER TRY TO CODE SICK AGAIN HOLY SHIT I CAN'T GET
         * ANYTHING DONE SO I GOT GEMINI TO FIGURE OUT THE SIMPLEST
         * STUFF
         */
        foreach (var game in search)
        {
            // Check if you've reached the maximum number of rows.
            // If so, stop adding more widgets.
            if (topPos >= maxRow)
            {
                break; // Exit the foreach loop
            }

            // Tends to error out a lot, so I just said fuck it
            // and put it on a try/catch
            try
            {
                if (game == null)
                {
                    continue;
                }

                var label = new Label(game.Name.Truncate(25)); // eh, 25 is good enough

                // IMPORTANT: Corrected the order back to (left, top)
                // AppGlobals.resultsBox.Attach(label, leftPos, topPos, 1, 1);
                wrappers.CreateGameBtn(game, AppGlobals.resultsBox, leftPos, topPos);

                leftPos++;  // Shoutout to gemini for this banger code

                // Using the maxCol variable for the check.
                // Using >= makes sure it wraps after the 3rd column (0, 1, 2).
                if (leftPos >= maxCol)
                {
                    leftPos = 0;
                    topPos++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while processing the game, Skipping.\nDetails: {e}");
            }
        }

        AppGlobals.resultsBox.ShowAll();
    }
}
