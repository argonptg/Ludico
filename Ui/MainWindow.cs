using System;
using System.Collections.Generic;
using LudicoGTK.Init;
using Gtk;
using LudicoGTK.LuaPlugin;
using LudicoGTK.Search;
using NLua;
using UI = Gtk.Builder.ObjectAttribute;
using IOPath = System.IO.Path;

namespace LudicoGTK.Ui
{
    public class MainWindow : Window
    {
        [UI] private SearchEntry _searchBar = null!;
        [UI] private Box _resultsBox = null!;
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
            Console.WriteLine($"Input: {_searchBar.Text}");

            var search = await SearchManager.Search(_searchBar.Text);

            foreach (var game in search)
            {
                Console.WriteLine($@"
{game.Name}
========================
Cover URL: {game.Cover.Value.Url.Replace("//", "https://").Replace("t_thumb", "t_cover_big_2x")}
ID: {game.Id}
Url: {game.Url}
========================
                ");
            }
            
            /*TreeIter iter;
            if (_pluginCombo.GetActiveIter(out iter))
            {
                string pluginPath = (string)_pluginCombo.Model.GetValue(iter, 1);
                AppGlobals.Lua.DoFile(pluginPath);
            }*/
        }

        private void DisplaySearchResults(List<string> results)
        {
            foreach (var result in results)
            {
                var label = new Gtk.Label(result);
                
                _resultsBox.PackStart(label, false, false, 0);
                
                label.Show();
            }
        }
    }
}
