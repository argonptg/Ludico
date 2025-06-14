using System;
using Gtk;
using LudicoGTK.Ui;

namespace LudicoGTK
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("uk.argonptg.Ludico", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            AppGlobals.appInstance = app;
            AppGlobals.window = win;
            
            win.Show();
            Application.Run();
        }
    }
}
