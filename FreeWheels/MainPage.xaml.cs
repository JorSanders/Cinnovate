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
using FreeWheels.Tests;
using Windows.Graphics.Imaging;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI;
using Microsoft.Graphics.Canvas.Text;
using FreeWheels.PozyxLibrary;
using FreeWheels.PozyxLibrary.Classes;
using System.ComponentModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FreeWheels
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Pozyx _Pozyx;

        private Position _MyPosition;

        private DispatcherTimer dispatcherTimer;
        private bool Init;
        private Testcase testcase;

        public MainPage()
        {
            _Pozyx = new Pozyx();
            this.Init = true;
            this.InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            _MyPosition = new Position();
            testcase = new Testcase(_Pozyx);

            StartUp();
        }

        private async Task StartUp()
        {
            Button1.IsEnabled = false;
            Button2.IsEnabled = false;
            Button3.IsEnabled = false;
            Button4.IsEnabled = false;
            Button5.IsEnabled = false;
            ResetButton.IsEnabled = false;
            await _Pozyx.ConnectI2c();

            _Pozyx.RegisterFunctions.ResetSys();
            await (Task.Delay(1000));
            _Pozyx.RegisterFunctions.FlashReset();
            await (Task.Delay(1000));

            Button1.IsEnabled = true;
            Button2.IsEnabled = true;
            Button3.IsEnabled = true;
            Button4.IsEnabled = true;
            Button5.IsEnabled = true;
            ResetButton.IsEnabled = true;
        }


        void dispatcherTimer_Tick(object sender, object e)
        {
            _Pozyx.RegisterFunctions.DoPositioning();

            _MyPosition = _Pozyx.PositioningData.Pos();

            this.Output.Text = "x: " + _MyPosition.X + "\t y: " + _MyPosition.Y + "\t z: " + _MyPosition.Z;

            String timeStamp = DateTime.Now.ToString();
            this.Timestamp.Text = "Timestamp: " + timeStamp;

            this.GridCanvas.Invalidate();

            string err = _Pozyx.StatusRegisters.ErrorCode();
            if (err != "0x00 - Success")
            {
                Debug.WriteLine("ERROR: " + err);
            }

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
            double scale = 7;
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

            foreach (Anchor anchor in _Pozyx.Anchors)
            {
                args.DrawingSession.DrawEllipse((float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space), 5, 5, Colors.Blue);
                args.DrawingSession.FillCircle((float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space), 5, Colors.Blue);
                args.DrawingSession.DrawText("ID: 0X" + anchor.Id.ToString("X4"), (float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space + 2), Colors.SlateGray, textFormatFat);
                args.DrawingSession.DrawText(anchor.X + "," + anchor.Y + "," + anchor.Z, (float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space + 20), Colors.DarkGray, textFormat);
            }

            /*foreach (Anchor anchor in _Pozyx.Anchors)
            {
                args.DrawingSession.DrawEllipse((float)(anchor.Y * pixelSize + space), (float)(anchor.X * pixelSize + space), 5, 5, Colors.Blue);
                args.DrawingSession.FillCircle((float)(anchor.Y * pixelSize + space), (float)(anchor.X * pixelSize + space), 5, Colors.Blue);
                args.DrawingSession.DrawText("ID: " + anchor.Id.ToString("x4"), (float)(anchor.Y * pixelSize + space), (float)(anchor.X * pixelSize + space + 2), Colors.SlateGray, textFormatFat);
                args.DrawingSession.DrawText(anchor.X + "," + anchor.Y + "," + anchor.Z, (float)(anchor.Y * pixelSize + space), (float)(anchor.X * pixelSize + space + 20), Colors.DarkGray, textFormat);
            }*/

            args.DrawingSession.DrawRectangle((float)(1980 * pixelSize + space), (float)(2000 * pixelSize + space), (float)(3600 * pixelSize), (float)(1200 * pixelSize), Colors.Cyan, 5);

            //Draw Tag

            args.DrawingSession.DrawEllipse((float)(this._MyPosition.X * pixelSize + space), (float)(this._MyPosition.Y * pixelSize + space), 5, 5, Colors.Green);
            args.DrawingSession.FillCircle((float)(this._MyPosition.X * pixelSize + space), (float)(this._MyPosition.Y * pixelSize + space), 5, Colors.Green);

            /*
            args.DrawingSession.DrawEllipse((float)(_MyPosition.Y * pixelSize + space), (float)(_MyPosition.X * pixelSize + space), 5, 5, Colors.Green);
            args.DrawingSession.FillCircle((float)(_MyPosition.Y * pixelSize + space), (float)(_MyPosition.X * pixelSize + space), 5, Colors.Green);
            */

        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            Button1.IsEnabled = false;
            Button2.IsEnabled = false;
            Button3.IsEnabled = false;
            Button4.IsEnabled = false;
            Button5.IsEnabled = false;

            _Pozyx.RegisterFunctions.ResetSys();
            await (Task.Delay(1000));
            _Pozyx.RegisterFunctions.FlashReset();
            await (Task.Delay(1000));

            Button1.IsEnabled = true;
            Button2.IsEnabled = true;
            Button3.IsEnabled = true;
            Button4.IsEnabled = true;
            Button5.IsEnabled = true;
        }

        private async void StartStopOld_Click(object sender, RoutedEventArgs e)
        {
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
                Button1.IsEnabled = true;
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
                    await _Pozyx.ManualAnchorsSetup();

                    dispatcherTimer.Tick += dispatcherTimer_Tick;
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);

                    GridCanvas.Visibility = Visibility.Visible;
                }

                dispatcherTimer.Start();
                Button1.IsEnabled = false;
                Button2.IsEnabled = false;
                Button3.IsEnabled = false;
                Button4.IsEnabled = false;
                Button5.IsEnabled = false;
            }

        }

        private async void Test_Click(object sender, RoutedEventArgs e)
        {
            Button1.IsEnabled = false;
            Button2.IsEnabled = false;
            Button3.IsEnabled = false;
            Button4.IsEnabled = false;
            Button5.IsEnabled = false;

            DispatcherTimer progress = new DispatcherTimer();
            progress.Tick += progress_Tick;
            progress.Interval = new TimeSpan(0, 0, 0, 0, 50);
            progress.Start();

            int timespan = 10 * 60 * 1000;
            //timespan = 5000;
            int interval = 50;
            string testCase = "Tracking 3D";
            string catagory = "Static test";
            string[] description = {
                "Posinterval: 50",
                "Algorithm: Tracking",
                "Dimension: 3D",
                "Filter: None",
                "Range protocol: Fast",
                "UWB plen: 1024",
                "UWB bitrate: 110kbs",
                "UWB prf: 64MHz",
            };

            await testcase.DoTest(timespan, interval, testCase, catagory, description);

            progress.Stop();

            Button1.IsEnabled = true;
            Button2.IsEnabled = true;
            Button3.IsEnabled = true;
            Button4.IsEnabled = true;
            Button5.IsEnabled = true;
        }

        private async void StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
                Button1.IsEnabled = true;
                Button2.IsEnabled = true;
                Button3.IsEnabled = true;
                Button4.IsEnabled = true;
                Button5.IsEnabled = true;
                GridCanvas.Visibility = Visibility.Collapsed;
                Button4.Content = "Start";
            }
            else if (_Pozyx.Anchors.Count >= 4)
            {
                await _Pozyx.SetConfiguration();
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                dispatcherTimer.Start();
                Button1.IsEnabled = false;
                Button2.IsEnabled = false;
                Button3.IsEnabled = false;
                Button5.IsEnabled = false;
                GridCanvas.Visibility = Visibility.Visible;
                Button4.Content = "Stop";
            }
        }

        private async void DiscoverAnchors_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            Button1.IsEnabled = false;
            Button2.IsEnabled = false;
            Button3.IsEnabled = false;
            Button4.IsEnabled = false;
            Button5.IsEnabled = false;
            await _Pozyx.DoAnchorDiscovery();
            Button1.IsEnabled = true;
            Button2.IsEnabled = true;
            Button3.IsEnabled = true;
            Button4.IsEnabled = true;
            Button5.IsEnabled = true;
        }

        private async void SmallRoom_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            Button1.IsEnabled = false;
            Button2.IsEnabled = false;
            Button3.IsEnabled = false;
            Button4.IsEnabled = false;
            Button5.IsEnabled = false;

            _Pozyx.ClearDevices();
            await Task.Delay(500);

            // Onze kamer
            _Pozyx.AddAnchor(0x697D, 1, 0, 45, 2000);
            await Task.Delay(200);
            _Pozyx.AddAnchor(0x6956, 1, 45, 3580, 500);
            await Task.Delay(200);
            _Pozyx.AddAnchor(0x6957, 1, 3590, 3535, 2000);
            await Task.Delay(200);
            _Pozyx.AddAnchor(0x697C, 1, 3545, 0, 500);
            await Task.Delay(200);

            Button1.IsEnabled = true;
            Button2.IsEnabled = true;
            Button3.IsEnabled = true;
            Button4.IsEnabled = true;
            Button5.IsEnabled = true;
        }

        private async void BigRoom_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            Button1.IsEnabled = false;
            Button2.IsEnabled = false;
            Button3.IsEnabled = false;
            Button4.IsEnabled = false;
            Button5.IsEnabled = false;

            _Pozyx.ClearDevices();
            await Task.Delay(500);

            // Grote vergader ruimte
            _Pozyx.AddAnchor(0x605B, 1, 0, 0, 500);
            await Task.Delay(200);
            _Pozyx.AddAnchor(0x6038, 1, 7000, 0, 2000);
            await Task.Delay(200);
            _Pozyx.AddAnchor(0x6029, 1, 0, 5100, 500);
            await Task.Delay(200);
            _Pozyx.AddAnchor(0x6047, 1, 6750, 5100, 10);
            await Task.Delay(200);

            Button1.IsEnabled = true;
            Button2.IsEnabled = true;
            Button3.IsEnabled = true;
            Button4.IsEnabled = true;
            Button5.IsEnabled = true;
        }

        void progress_Tick(object sender, object e)
        {
            this.Output.Text = testcase.Status;
        }
    }
}
