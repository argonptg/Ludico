using System;
using craftersmine.SteamGridDBNet;
using Gdk;
using Gtk;

namespace LudicoGTK.Ui.GamePage
{
    public class GameResultPage : Box
    {
        // Declare private fields for all the widgets you need to access
        private Label _gameDesc;
        private Label _gameTitle;
        private ModelButton _hydraDown;
        private ModelButton _gldDown;
        private DrawingArea _drawArea;
        private Pixbuf _pixbufImg;
        
        public GameResultPage(SteamGridDbGame game, Pixbuf pixbufImg) : base(Orientation.Vertical, 0)
        {
            var builder = new Builder("GamePage.glade");

            _gameTitle = builder.GetObject("_gameTitle") as Label;
            _gameDesc = builder.GetObject("_gameDesc") as Label;
            _gldDown = builder.GetObject("_gldDown") as ModelButton;
            _hydraDown = builder.GetObject("_hydraDown") as ModelButton;
            _drawArea = builder.GetObject("_drawArea") as DrawingArea;

            _pixbufImg = pixbufImg;


            // text shit
            _gameTitle.Text = game.Name;

            // Attach event handlers after assigning the widgets
            if (_gldDown != null)
            {
                _gldDown.Clicked += (sender, e) => Console.WriteLine("Clicked on gld down");
            }
            else
            {
                Console.WriteLine("ERROR: _gldDown button not found in Glade file.");
            }

            if (_hydraDown != null)
            {
                _hydraDown.Clicked += (sender, e) => Console.WriteLine("Clicked on hydra down");
            }
            else
            {
                Console.WriteLine("ERROR: _hydraDown button not found in Glade file.");
            }

            _drawArea.Drawn += (widget, a) =>
            {
                if (_pixbufImg == null)
                {
                    Utils.Log.Error("Pixbuf is empty?");
                    return;
                }

                var drawingArea = (Gtk.DrawingArea)widget;
                var cr = a.Cr;

                int areaWidth = drawingArea.AllocatedWidth;
                int areaHeight = drawingArea.AllocatedHeight;

                int imgWidth = _pixbufImg.Width;
                int imgHeight = _pixbufImg.Height;

                double scaleX = (double)areaWidth / imgWidth;
                double scaleY = (double)areaHeight / imgHeight;

                // Use the LARGER scale factor to ensure the image fills the entire area
                double scale = Math.Max(scaleX, scaleY);

                // Calculate the new size and the offset to center it.
                // One of the offsets will be negative, which is what "cuts" the image.
                double scaledWidth = imgWidth * scale;
                double scaledHeight = imgHeight * scale;
                double dx = (areaWidth - scaledWidth) / 2;
                double dy = (areaHeight - scaledHeight) / 2;

                // save dis shit
                cr.Save();

                cr.Translate(dx, dy);
                cr.Scale(scale, scale);
                Gdk.CairoHelper.SetSourcePixbuf(cr, _pixbufImg, 0, 0);
                cr.Paint();

                cr.Restore();
            };


            _drawArea.QueueDraw();

            var rootWidget = (Widget)builder.GetObject("game_page_root"); // <- this is on GamePage.glade
            Add(rootWidget);

            Destroyed += (sender, e) =>
            {
                pixbufImg?.Dispose();
                _pixbufImg?.Dispose();
            };
        }

        public void Update(SteamGridDbGame game, Pixbuf newPixbuf)
        {
            _gameTitle.Text = game.Name;

            // this just works, apparently
            // memory usage go brrrrr
            _pixbufImg = newPixbuf;
            _drawArea.QueueDraw();
        }
    }
}