using System;
using System.Collections.Generic;
using Gtk;
using LudicoGTK.Plugin.Modules;
using NLua;
using System.IO;
using UI = Gtk.Builder.ObjectAttribute;
using IOPath = System.IO.Path;

namespace LudicoGTK
{
    class MainWindow : Window
    {
        [UI] private SearchEntry _searchBar = null!;
        [UI] private Box _resultsBox = null!;
        [UI] private ListStore pluginList = null!;
        [UI] private ComboBox _pluginCombo = null!;
        
        private static readonly Lua Lua = new Lua();
        
        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            _searchBar.Activated += SearchBar_Handler;

            GLib.Idle.Add(() =>
            {
                InitializeLua();
                InitializePlugins();
                
                return false;
            });
            
        }

        private void InitializeLua()
        {
            // global "class" for json
            Lua.DoString("JsonWrapper = {}"); 
            Lua.RegisterFunction(
                "json_parse_internal", 
                null, 
                typeof(JsonWrapper).GetMethod("ParseJson"));
            Lua.DoString("JsonWrapper.parse = json_parse_internal");
            
            // notifications
            Lua.DoString("Notifications = {}");
            Lua.RegisterFunction(
                "internal_push",
                null,
                typeof(Notifications).GetMethod("Push"));
            Lua.RegisterFunction(
                "internal_success",
                null,
                typeof(Notifications).GetMethod("PushSuccess"));
            Lua.RegisterFunction(
                "internal_error",
                null,
                typeof(Notifications).GetMethod("PushError"));
            Lua.RegisterFunction(
                "internal_warning",
                null,
                typeof(Notifications).GetMethod("PushWarning"));
            Lua.DoString("Notifications.push = internal_push");
            Lua.DoString("Notifications.push_success = internal_success");
            Lua.DoString("Notifications.push_error = internal_error");
            Lua.DoString("Notifications.push_warning = internal_warning");
        }

        private void InitializePlugins()
        {
            var localDataPath = AppGlobals.GetLocalDataPath();
            var pluginPath = IOPath.Combine(localDataPath, "plugins");
            
            // Ensures the dir actually exists

            Directory.CreateDirectory(pluginPath);
            var plugins = Directory.GetFiles(pluginPath);

            // Adds the plugin to the listStore
            foreach (var plugin in plugins)
            {
                var pluginName = IOPath.GetFileName(plugin);
                pluginList.AppendValues(pluginName, plugin);
            }
        }
        
        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        private async void SearchBar_Handler(object sender, EventArgs a)
        { 
            Console.WriteLine($"Haiiiiiii :3c!!!!\n Input: {_searchBar.Text}"); 
            Console.WriteLine("Searching...");
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
