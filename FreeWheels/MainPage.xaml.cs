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

        public MainPage()
        {
            this.InitializeComponent();
            _Pozyx = new Pozyx();

            this.Tag = new Position();
            this.Anchors = new Position[4];
            this.Anchors[0] = new Position(0, 0, 1880);
            this.Anchors[1] = new Position(0, 2554, 1600);
            this.Anchors[2] = new Position(8123, 0, 1900);
            this.Anchors[3] = new Position(8176, 3105, 2050);

            GridCanvas.Invalidate();
        }

        async void Start()
        {
            _Pozyx.LetsGo();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);

            dispatcherTimer.Start();
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

        private async void TestGrid_Click(object sender, RoutedEventArgs e)
        {

            // Get Screen Size/Bounds
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

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

            //Set Canvas Size
            GridCanvas.Width = width;
            GridCanvas.Height = height;

            //Calculate Seperator
            double space = height / 12;
            double pixelSize = height / 12000;

            //Set Text Properties
            CanvasTextFormat textFormat = new CanvasTextFormat();
            textFormat.FontSize = 14;

            CanvasTextFormat textFormatFat = new CanvasTextFormat();
            textFormatFat.FontSize = 16;
            textFormatFat.FontWeight = Windows.UI.Text.FontWeights.Medium;

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

            //Anchor Test
            foreach(Position anchor in this.Anchors)
            {
                args.DrawingSession.DrawEllipse((float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space), 5, 5, Colors.Blue);
                args.DrawingSession.FillCircle((float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space), 5, Colors.Blue);
                args.DrawingSession.DrawText("ID:", (float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space), Colors.SlateGray, textFormatFat);
                args.DrawingSession.DrawText(anchor.X + "," + anchor.Y + "," + anchor.Z, (float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space), Colors.DarkGray, textFormat);
            }

            // Example
            //args.DrawingSession.DrawEllipse(100, 300, 5, 5, Colors.Blue);
            //args.DrawingSession.FillCircle(100, 300, 5, Colors.Blue);
            //args.DrawingSession.DrawText("0x605B", 100, 302, Colors.DarkGray, textFormatFat);
            //args.DrawingSession.DrawText("0,2000", 100, 315, Colors.DarkGray, textFormat);

            //Pozyx Test
            args.DrawingSession.DrawEllipse((float)(this.Tag.X * pixelSize + space), (float)(this.Tag.Y * pixelSize + space), 5, 5, Colors.Green);
            args.DrawingSession.FillCircle((float)(this.Tag.X * pixelSize + space), (float)(this.Tag.Y * pixelSize + space), 5, Colors.Green);

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterFunctions.ResetSys();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Start();
            this.StartButton.Visibility = Visibility.Collapsed;
            GridCanvas.Visibility = Visibility.Visible;
        }

    }
}
