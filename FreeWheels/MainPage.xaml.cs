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
using Windows.UI.Xaml.Shapes;
using System.ComponentModel;
using Windows.Storage;

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

        private DispatcherTimer UpdateScreen;
        private DispatcherTimer UpdatePosition;
        private bool Init;
        private Testcase testcase;

        private List<float[]> linePoints = new List<float[]>();
        private List<Position> PositionList = new List<Position>();
        private List<DateTime> TimestampList = new List<DateTime>();

        private double pixelSize, space;

        public MainPage()
        {
            _Pozyx = new Pozyx();
            this.Init = true;
            this.InitializeComponent();
            UpdateScreen = new DispatcherTimer();
            UpdatePosition = new DispatcherTimer();
            UpdateScreen.Tick += UpdateScreen_Tick;
            UpdatePosition.Tick += UpdatePosition_Tick;
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
            space = height / scale;
            pixelSize = height / (scale * 1000);

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

            // Draw table big
            args.DrawingSession.DrawRectangle((float)(1980 * pixelSize + space), (float)(2000 * pixelSize + space), (float)(3600 * pixelSize), (float)(1200 * pixelSize), Colors.Cyan, 5);

            // Draw table circle 
            // args.DrawingSession.DrawEllipse((float)(2030 * pixelSize + space), (float)(2310 * pixelSize + space), (float)(600 * pixelSize), (float)(600 * pixelSize), Colors.Cyan, 5);

            // Draw Test Path
            args.DrawingSession.DrawRectangle((float)(1170 * pixelSize + space), (float)(1220 * pixelSize + space), (float)(5000 * pixelSize), (float)(2500 * pixelSize), Colors.BlanchedAlmond, 29);

            // Draw line
            for (int i = 0; i < linePoints.Count - 1; i++)
            {
                args.DrawingSession.DrawLine(linePoints[i][0], linePoints[i][1], linePoints[i + 1][0], linePoints[i + 1][1], Windows.UI.Colors.Red);
            }

            // Draw Tag
            args.DrawingSession.FillCircle((float)(this._MyPosition.X * pixelSize + space), (float)(this._MyPosition.Y * pixelSize + space), 5, Colors.Green);
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateScreen.Stop();
            UpdatePosition.Stop();
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

        private async void Test_Click(object sender, RoutedEventArgs e)
        {
            this.linePoints = new List<float[]>();
            this.PositionList = new List<Position>();

            /*
            Button1.IsEnabled = false;
            Button2.IsEnabled = false;
            Button3.IsEnabled = false;
            Button4.IsEnabled = false;
            Button5.IsEnabled = false;

            DispatcherTimer progress = new DispatcherTimer();
            progress.Tick += progress_Tick;
            progress.Interval = new TimeSpan(0, 0, 0, 0, 1);
            progress.Start();

            int timespan = 10 * 60 * 1000;
            int interval = 50;
            string testCase = "Middle of room";
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
            this.Output.Text = "Test finished";

            Button1.IsEnabled = true;
            Button2.IsEnabled = true;
            Button3.IsEnabled = true;
            Button4.IsEnabled = true;
            Button5.IsEnabled = true;

    */
        }

        private async void StartStop_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateScreen.IsEnabled)
            {
                UpdateScreen.Stop();
                UpdatePosition.Stop();
                linePoints = new List<float[]>();
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
                UpdateScreen.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
                UpdatePosition.Interval = new TimeSpan(0, 0, 0, 0, 50);
                UpdateScreen.Start();
                UpdatePosition.Start();

                Button1.IsEnabled = false;
                Button2.IsEnabled = false;
                //Button3.IsEnabled = false;
                //Button5.IsEnabled = false;
                GridCanvas.Visibility = Visibility.Visible;
                Button4.Content = "Stop";
            }
        }

        private async void DiscoverAnchors_Click(object sender, RoutedEventArgs e)
        {
            UpdatePosition.Stop();
            UpdateScreen.Stop();

            List<string> ExportData = new List<string>();

            string testcase = "";
            ExportData.Add("sep=;");

            ExportData.Add("Testcase;" + testcase);
            ExportData.Add("X;Y;Z;Timestamp");
            for (int i = 0; i < PositionList.Count; i++)
            {
                ExportData.Add(PositionList[i].X + ";" + PositionList[i].Y + ";" + PositionList[i].Z + ";" + TimestampList[i].ToLocalTime().ToString("H:mm:s.ff"));
            }

            StorageFolder folder = ApplicationData.Current.LocalFolder;
            string fileName = testcase + "-" + DateTime.Now.ToLocalTime().ToString("dd-MMMM-yy H-mm") + ".csv";
            StorageFile sample = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteLinesAsync(sample, ExportData);

            //UpdateScreen.Stop();
            //UpdatePosition.Stop();
            //Button1.IsEnabled = false;
            //Button2.IsEnabled = false;
            //Button3.IsEnabled = false;
            //Button4.IsEnabled = false;
            //Button5.IsEnabled = false;
            //await _Pozyx.DoAnchorDiscovery();
            //Button1.IsEnabled = true;
            //Button2.IsEnabled = true;
            //Button3.IsEnabled = true;
            //Button4.IsEnabled = true;
            //Button5.IsEnabled = true;
        }

        private async void SmallRoom_Click(object sender, RoutedEventArgs e)
        {
            UpdateScreen.Stop();
            UpdatePosition.Stop();
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
            UpdateScreen.Stop();
            UpdatePosition.Stop();
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

        void UpdateScreen_Tick(object sender, object e)
        {
            this.Output.Text = "x: " + _MyPosition.X + "\t y: " + _MyPosition.Y + "\t z: " + _MyPosition.Z;

            String timeStamp = DateTime.Now.ToString();
            this.Timestamp.Text = "Timestamp: " + timeStamp;

            this.GridCanvas.Invalidate();
        }

        void UpdatePosition_Tick(object sender, object e)
        {
            _Pozyx.RegisterFunctions.DoPositioning();

            _MyPosition = _Pozyx.PositioningData.Pos();

            // Add to the position list
            PositionList.Add(_MyPosition);
            TimestampList.Add(DateTime.Now);

            //Adds the route walked to the map
            linePoints.Add(new float[] { (float)(_MyPosition.X * pixelSize + space), (float)(_MyPosition.Y * pixelSize + space) });

            string err = _Pozyx.StatusRegisters.ErrorCode();
            if (err != "0x00 - Success")
            {
                Debug.WriteLine("ERROR: " + err);
            }
        }
    }
}
