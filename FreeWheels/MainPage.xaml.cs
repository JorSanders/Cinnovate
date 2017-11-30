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
using Windows.Graphics.Imaging;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.UI;
using Microsoft.Graphics.Canvas.Text;
using FreeWheels.PozyxLibrary;
using FreeWheels.PozyxLibrary.Classes;

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

        public MainPage()
        {
            _Pozyx = new Pozyx();
            this.Init = true;
            this.InitializeComponent();
            dispatcherTimer = new DispatcherTimer();
            _MyPosition = new Position();
            //GridCanvas.Invalidate();
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
            _MyPosition = new Position(
                _Pozyx.PositioningData.PosX(),
                _Pozyx.PositioningData.PosY(),
                _Pozyx.PositioningData.PosZ()
                );

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
            /*
            foreach (Anchor anchor in _Pozyx.Anchors)
            {
                args.DrawingSession.DrawEllipse((float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space), 5, 5, Colors.Blue);
                args.DrawingSession.FillCircle((float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space), 5, Colors.Blue);
                args.DrawingSession.DrawText("ID: " + anchor.Id.ToString(), (float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space + 2), Colors.SlateGray, textFormatFat);
                args.DrawingSession.DrawText(anchor.X + "," + anchor.Y + "," + anchor.Z, (float)(anchor.X * pixelSize + space), (float)(anchor.Y * pixelSize + space + 20), Colors.DarkGray, textFormat);
            }
            */
            foreach (Anchor anchor in _Pozyx.Anchors)
            {
                args.DrawingSession.DrawEllipse((float)(anchor.Y * pixelSize + space), (float)(anchor.X * pixelSize + space), 5, 5, Colors.Blue);
                args.DrawingSession.FillCircle((float)(anchor.Y * pixelSize + space), (float)(anchor.X * pixelSize + space), 5, Colors.Blue);
                args.DrawingSession.DrawText("ID: " + anchor.Id.ToString("x4"), (float)(anchor.Y * pixelSize + space), (float)(anchor.X * pixelSize + space + 2), Colors.SlateGray, textFormatFat);
                args.DrawingSession.DrawText(anchor.X + "," + anchor.Y + "," + anchor.Z, (float)(anchor.Y * pixelSize + space), (float)(anchor.X * pixelSize + space + 20), Colors.DarkGray, textFormat);
            }

            //Draw Tag
            /*
            args.DrawingSession.DrawEllipse((float)(this.Tag.X * pixelSize + space), (float)(this.Tag.Y * pixelSize + space), 5, 5, Colors.Green);
            args.DrawingSession.FillCircle((float)(this.Tag.X * pixelSize + space), (float)(this.Tag.Y * pixelSize + space), 5, Colors.Green);
            */

            args.DrawingSession.DrawEllipse((float)(_MyPosition.Y * pixelSize + space), (float)(_MyPosition.X * pixelSize + space), 5, 5, Colors.Green);
            args.DrawingSession.FillCircle((float)(_MyPosition.Y * pixelSize + space), (float)(_MyPosition.X * pixelSize + space), 5, Colors.Green);

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _Pozyx.RegisterFunctions.ResetSys();
            _Pozyx.RegisterFunctions.FlashReset();
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

        private async void StartStop2_Click(object sender, RoutedEventArgs e)
        {
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
                Button2.IsEnabled = true;
                Button3.Content = "Start";
                Button3.IsEnabled = true;
                Button4.IsEnabled = true;
                Button5.IsEnabled = true;
            }
            else if (_Pozyx.Anchors.Count >= 4)
            {
                await _Pozyx.SetConfiguration();
                dispatcherTimer.Tick += dispatcherTimer_Tick;
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
                dispatcherTimer.Start();
                GridCanvas.Visibility = Visibility.Visible;
                Button2.IsEnabled = false;
                Button3.Content = "Stop";
                Button4.IsEnabled = false;
                Button5.IsEnabled = false;
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
            await _Pozyx.SetAnchors();
            Button1.IsEnabled = true;
            Button2.IsEnabled = true;
            Button3.IsEnabled = true;
            Button4.IsEnabled = true;
            Button5.IsEnabled = true;
        }

    }
}
