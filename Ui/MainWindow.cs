using Gtk;
using LudicoGTK.Init;
using UI = Gtk.Builder.ObjectAttribute;

namespace LudicoGTK.Ui
{
    public class MainWindow : Window
    {
        [UI] private SearchEntry _searchBar = null!;
        [UI] private Grid _resultsBox = null!;
        [UI] private ListStore pluginList = null!;

        [UI] private Stack _mainStack = null!;

        private readonly Handlers handlers;

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);
            handlers = new Handlers();

            DeleteEvent += Window_DeleteEvent;
            _searchBar.Activated += handlers.SearchBar_Handler;

            AppGlobals.pluginList = pluginList;
            AppGlobals.searchBar = _searchBar;
            AppGlobals.resultsBox = _resultsBox;
            AppGlobals.mainStack = _mainStack;

            GLib.Idle.Add(() =>
            {
                var init = new AppInitializer();
                init.Initialize();

                return false;
            });

        }

        private void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }
    }
}

public static class StringExt
{
    #nullable enable // i fucking hate warnings
    public static string? Truncate(this string value, int maxLength, string truncationSuffix = "…")
    # nullable disable
    {
        return value?.Length > maxLength
            ? value.Substring(0, maxLength) + truncationSuffix
            : value;
    }
}