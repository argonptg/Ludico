using Gtk;
using System;
using LudicoGTK.Init;
using LudicoGTK.Logger;
using LudicoGTK.Search;
using UI = Gtk.Builder.ObjectAttribute;

namespace LudicoGTK.Ui
{
    public class MainWindow : Window
    {
        [UI] private SearchEntry _searchBar = null!;
        [UI] private Grid _resultsBox = null!;
        [UI] private ListStore pluginList = null!;
        [UI] private ComboBox _pluginCombo = null!;
        
        public MainWindow() : this(new Builder("MainWindow.glade")) { }
        
        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            _searchBar.Activated += SearchBar_Handler;

            GLib.Idle.Add(() =>
            {
                var init = new AppInitializer();
                init.Initialize();
                
                return false;
            });
            
        }

        public void AddPlugin(string pluginName, string pluginPath)
        {
            pluginList.AppendValues(pluginName, pluginPath);
        }
        
        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private async void SearchBar_Handler(object sender, EventArgs a)
        { 
            Log.Info($"Search results for '{_searchBar.Text}'");
            
            var search = await SearchManager.Search(_searchBar.Text);

            // NEW: Define your max rows and columns here
            int maxCol = 3;
            int maxRow = 5; // For example, stop after 5 rows are filled

            // Clear out everything
            while (_resultsBox.Children.Length > 0)
            {
                _resultsBox.Remove(_resultsBox.Children[0]);
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
                    _resultsBox.Attach(label, leftPos, topPos, 1, 1);
                    
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
            
            _resultsBox.ShowAll();
        }
    }
}

public static class StringExt
{
    public static string? Truncate(this string? value, int maxLength, string truncationSuffix = "…")
    {
        return value?.Length > maxLength
            ? value.Substring(0, maxLength) + truncationSuffix
            : value;
    }
}