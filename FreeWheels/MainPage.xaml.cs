using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.Text;
using FreeWheels.Classes;
using FreeWheels.Classes.PozyxApi;
using Windows.Graphics.Imaging;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI;
using Microsoft.Graphics.Canvas.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FreeWheels
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Pozyx _Pozyx;

        private Position[] Anchors;
        private Position Tag;

        private DispatcherTimer dispatcherTimer;
        private bool Init;

        public MainPage()
        {
            this.Init = true;
            this.InitializeComponent();
            _Pozyx = new Pozyx();

            dispatcherTimer = new DispatcherTimer();

            this.Tag = new Position();
            this.Anchors = new Position[4];
            this.Anchors[0] = new Position(0, 0, 1880);
            this.Anchors[1] = new Position(0, 2554, 1600);
            this.Anchors[2] = new Position(8123, 0, 1900);
            this.Anchors[3] = new Position(8176, 3105, 2050);

            GridCanvas.Invalidate();
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            this.Tag.X = PositioningData.PosX();
            this.Tag.Y = PositioningData.PosY();
            this.Tag.Z = PositioningData.PosZ();

            this.Output.Text = "x: " + this.Tag.X + "\t y: " + this.Tag.Y + "\t z: " + this.Tag.Z;

            String timeStamp = DateTime.Now.ToString();
            this.Timestamp.Text = "Timestamp: " + timeStamp;

            this.GridCanvas.Invalidate();

        }

        void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            // Get Screen Size/Bounds
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

            //Calculate Canvase Size
            double width = size.Width - 200;
            double height = size.Height - 180;

            //Calculate Seperator
            double scale = 5;
            double space = height / scale;
            double pixelSize = height / (scale * 1000);

            //Set Text Properties
            CanvasTextFormat textFormat = new CanvasTextFormat();
            textFormat.FontSize = 14;

            CanvasTextFormat textFormatFat = new CanvasTextFormat();
            textFormatFat.FontSize = 16;
            textFormatFat.FontWeight = Windows.UI.Text.FontWeights.Medium;

            //Set Canvas Size
            GridCanvas.Width = width;
            GridCanvas.Height = height;

            //Draw Grid
            for (int i = (int)space; i < width; i += (int)space)
            {
                args.DrawingSession.DrawLine(i, 0, i, (int)height, Colors.LightGray);
                args.DrawingSession.DrawText((Math.Round(i / space) - 1).ToString(), i + 3, 0, Colors.LightGray, textFormat);
            }

            for (int i = (int)space; i < height; i += (int)space)
            {
                args.DrawingSession.DrawLine(0, i, (int)width, i, Colors.LightGray);
                args.DrawingSession.DrawText((Math.Round(i / space) - 1).ToString(), 0, i + 3, Colors.LightGray, textFormat);
            }

            //Draw Anchors
            foreach (Position anchor in this.Anchors)
            {
                args.DrawingSession.DrawEllipse((float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space), 5, 5, Colors.Blue);
                args.DrawingSession.FillCircle((float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space), 5, Colors.Blue);
                args.DrawingSession.DrawText("ID: 0x6038", (float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space + 2), Colors.SlateGray, textFormatFat);
                args.DrawingSession.DrawText(anchor.X + "," + anchor.Y + "," + anchor.Z, (float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space + 20), Colors.DarkGray, textFormat);
            }

            //Draw Tag
            args.DrawingSession.DrawEllipse((float)(this.Tag.X * pixelSize + space), (float)(this.Tag.Y * pixelSize + space), 5, 5, Colors.Green);
            args.DrawingSession.FillCircle((float)(this.Tag.X * pixelSize + space), (float)(this.Tag.Y * pixelSize + space), 5, Colors.Green);

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterFunctions.ResetSys();
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
                Button2.IsEnabled = true;
                Button3.IsEnabled = true;
                Button4.IsEnabled = true;
                Button5.IsEnabled = true;
            }
            else
            {
                if (this.Init)
                {
                    this.Init = false;
                    _Pozyx.LetsGo();

                    dispatcherTimer.Tick += dispatcherTimer_Tick;
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);

                    GridCanvas.Visibility = Visibility.Visible;
                }

                dispatcherTimer.Start();
                Button2.IsEnabled = false;
                Button3.IsEnabled = false;
                Button4.IsEnabled = false;
                Button5.IsEnabled = false;
            }

        }

    }
}
