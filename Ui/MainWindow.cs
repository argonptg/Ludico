using Gtk;
using System;
using LudicoGTK.Init;
using LudicoGTK.Utils;
using LudicoGTK.Search;
using UI = Gtk.Builder.ObjectAttribute;
using IOPath = System.IO.Path;
using Pixbuf = Gdk.Pixbuf;
using InterpType = Gdk.InterpType;
using IGDB.Models;

namespace LudicoGTK.Ui
{
    public class MainWindow : Window
    {
        [UI] private SearchEntry _searchBar = null!;
        [UI] private Grid _resultsBox = null!;
        [UI] private ListStore pluginList = null!;
        [UI] private Stack _mainStack = new();

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

        private void ShowGameInfoDialog(Game game)
        {
            using (var dialog = new Dialog())
            {
                dialog.Title = "Game Information";
                dialog.TransientFor = this;
                dialog.Modal = true;
                // dialog.AddButton("OK", ResponseType.Ok);
                dialog.SetDefaultSize(300, 150);

                var contentBox = dialog.ContentArea;
                contentBox.BorderWidth = 10;
                contentBox.Spacing = 6;

                // Create the label with the game's info
                var infoLabel = new Label($"You have selected: {game.Name}\n\nThis is a modal dialog window attached to the main application.");
                infoLabel.Wrap = true;

                var comboBox = new ComboBox(pluginList);
                var cell = new CellRendererText();
                comboBox.PackStart(cell, false);
                comboBox.AddAttribute(cell, "text", 0);

                contentBox.PackStart(infoLabel, true, true, 0);
                contentBox.PackStart(comboBox, true, true, 0);

                dialog.ShowAll();

                dialog.Run();
            }
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
                    // _resultsBox.Attach(label, leftPos, topPos, 1, 1);
                    CreateGameBtn(game, _resultsBox, leftPos, topPos);

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

        private void CreateGameBtn(Game game, Grid container, int leftPos, int topPos)
        {
            var gameBtn = new Button();
            var buttonBox = new Box(Orientation.Vertical, 5);

            const int maxImageWidth = 190;

            // Just put everything in a try/catch! what could go wrong?
            try
            {
                // Get the image path, i.e /home/$USER/Documents/Ludico/cache
                var imagePath =
                    IOPath.Combine(AppGlobals.GetDocumentsPath(), "cache", $"{game.Id ?? -1}.jpg");

                // I still don't know what a pixbuf is
                var originalPixbuf = new Pixbuf(imagePath);

                // Resize the image
                int targetImageHeight = (int)((float)maxImageWidth / originalPixbuf.Width * originalPixbuf.Height);
                var scaledPixbuf = originalPixbuf.ScaleSimple(maxImageWidth, targetImageHeight, InterpType.Bilinear);

                var img = new Image(scaledPixbuf);
                var label = new Label($"<b>{game.Name}</b>");

                label.UseMarkup = true;
                label.Justify = Justification.Center;

                buttonBox.PackStart(img, true, true, 0);
                buttonBox.PackStart(label, true, true, 0);

                gameBtn.Add(buttonBox);
                gameBtn.Clicked += (sender, args) =>
                {
                    Log.Info($"You selected {game.Name}");
                    ShowGameInfoDialog(game);
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