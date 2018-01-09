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
        private Pozyx _Pozyx; // Pozyxlibrary object

        private int _FriendId; // the other pozyx tag id
        private Position _FriendPosition; // The other pozyx location

        private DispatcherTimer UpdateScreen; // Dispatchtimer that controls the drawing on screen
        private DispatcherTimer UpdatePosition; // Dispatchtimer that controls updating the position

        private List<float[]> _LinePoints = new List<float[]>(); // Position points for on the canvas with previous positions
        private List<Position> _PositionList = new List<Position>(); // Actual previous positions
        private List<DateTime> _TimestampList = new List<DateTime>(); // Timestams form these positions

        private double _PixelSize, _Space; // Information for the canvas

        private bool _Running; // Indicates wether we are getting the position and updating the screen.

        private Testcase _Testcase;

        public MainPage()
        {
            UpdateScreen = new DispatcherTimer();
            UpdatePosition = new DispatcherTimer();
            UpdateScreen.Tick += UpdateScreen_Tick;
            UpdatePosition.Tick += UpdatePosition_Tick;

            this.InitializeComponent();

            StopRunning();

            _Pozyx = new Pozyx();

            _Testcase = new Testcase(_Pozyx);

            _FriendPosition = new Position();
            _FriendId = 0x6E38; // The id of our 2nd pozyx

            StartUp();
        }

        /// <summary>
        ///     Connect the pozyx via I2C and reset it.
        /// </summary>
        /// <returns></returns>
        private async Task StartUp()
        {
            Button1.IsEnabled = false;
            Button2.IsEnabled = false;
            Button3.IsEnabled = false;
            Button4.IsEnabled = false;
            Button5.IsEnabled = false;
            ResetButton.IsEnabled = false;
            await _Pozyx.ConnectI2c();

            //_Pozyx.RegisterFunctions.ResetSys(_FriendId);
            //await Task.Delay(200);
            //_Pozyx.RegisterFunctions.ResetSys();
            //await (Task.Delay(1000));
            //_Pozyx.RegisterFunctions.FlashReset();
            //await (Task.Delay(1000));

            Button1.IsEnabled = true;
            Button2.IsEnabled = true;
            Button3.IsEnabled = true;
            Button4.IsEnabled = true;
            Button5.IsEnabled = true;
            ResetButton.IsEnabled = true;
        }

        /// <summary>
        ///     Draw the canvas and its contents
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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
            double scale = 15;
            _Space = height / scale;
            _PixelSize = height / (scale * 1000);

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
            for (int i = (int)_Space; i < width; i += (int)_Space)
            {
                args.DrawingSession.DrawLine(i, 0, i, (int)height, Colors.LightGray);
                args.DrawingSession.DrawText((Math.Round(i / _Space) - 1).ToString(), i + 3, 0, Colors.LightGray, textFormat);
            }

            for (int i = (int)_Space; i < height; i += (int)_Space)
            {
                args.DrawingSession.DrawLine(0, i, (int)width, i, Colors.LightGray);
                args.DrawingSession.DrawText((Math.Round(i / _Space) - 1).ToString(), 0, i + 3, Colors.LightGray, textFormat);
            }


            //Draw Anchors
            foreach (Anchor anchor in _Pozyx.Anchors)
            {
                args.DrawingSession.DrawEllipse((float)(anchor.X * _PixelSize + _Space), (float)(anchor.Y * _PixelSize + _Space), 5, 5, Colors.Blue);
                args.DrawingSession.FillCircle((float)(anchor.X * _PixelSize + _Space), (float)(anchor.Y * _PixelSize + _Space), 5, Colors.Blue);
                args.DrawingSession.DrawText("ID: 0X" + anchor.Id.ToString("X4"), (float)(anchor.X * _PixelSize + _Space), (float)(anchor.Y * _PixelSize + _Space + 2), Colors.SlateGray, textFormatFat);
                args.DrawingSession.DrawText(anchor.X + "," + anchor.Y + "," + anchor.Z, (float)(anchor.X * _PixelSize + _Space), (float)(anchor.Y * _PixelSize + _Space + 20), Colors.DarkGray, textFormat);
            }

            // Draw table big
            args.DrawingSession.DrawRectangle((float)(1980 * _PixelSize + _Space), (float)(2000 * _PixelSize + _Space), (float)(3600 * _PixelSize), (float)(1200 * _PixelSize), Colors.Cyan, 5);

            // Draw table circle 
            // args.DrawingSession.DrawEllipse((float)(2030 * pixelSize + space), (float)(2310 * pixelSize + space), (float)(600 * pixelSize), (float)(600 * pixelSize), Colors.Cyan, 5);

            // Draw Test Path
            args.DrawingSession.DrawRectangle((float)(1170 * _PixelSize + _Space), (float)(1220 * _PixelSize + _Space), (float)(5000 * _PixelSize), (float)(2500 * _PixelSize), Colors.BlanchedAlmond, 29);

            // Draw line
            for (int i = 0; i < _LinePoints.Count - 1; i++)
            {
                args.DrawingSession.DrawLine(_LinePoints[i][0], _LinePoints[i][1], _LinePoints[i + 1][0], _LinePoints[i + 1][1], Windows.UI.Colors.Red);
            }

            if (_LinePoints.Count >= 1)
            {
                // Draw Tag
                args.DrawingSession.FillCircle(_LinePoints.Last()[0], _LinePoints.Last()[1], 5, Colors.Green);
            }

            // Draw friend
            args.DrawingSession.FillCircle((float)(this._FriendPosition.X * _PixelSize + _Space), (float)(this._FriendPosition.Y * _PixelSize + _Space), 5, Colors.Pink);
        }

        /// <summary>
        ///     Reset the pozyx system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();
            StopRunning();

            await (Task.Delay(1000));
            _Pozyx.RegisterFunctions.ResetSys();
            await (Task.Delay(1000));
            _Pozyx.RegisterFunctions.FlashReset();
            await (Task.Delay(1000));

            _Pozyx.Anchors = new List<Anchor>();

            EnableButtons();
        }

        private async void Button1_Click(object sender, RoutedEventArgs e)
        {
            if (_Running)
            {

            }
            else // set the anchors in the Kleine vergader ruimte
            {
                DisableButtons();

                _Pozyx.ClearDevices();
                await Task.Delay(500);

                // Onze kamer
                _Pozyx.AddAnchor(0x6029, 1, 0, 0, 2000);
                await Task.Delay(200);
                _Pozyx.AddAnchor(0x6038, 1, 2252, 0, 2000);
                await Task.Delay(200);
                _Pozyx.AddAnchor(0x605B, 1, -560, 8056, 20000);
                await Task.Delay(200);
                _Pozyx.AddAnchor(0x6047, 1, 2953, 8197, 2000);
                await Task.Delay(200);

                EnableButtons();
            }
        }

        private async void Button2_Click(object sender, RoutedEventArgs e)
        {
            if (_Running)
            {

            }
            else // set the anchors in the Grote vergader ruimte
            {
                DisableButtons();

                _Pozyx.ClearDevices();
                await Task.Delay(500);

                _Pozyx.AddAnchor(0x6029, 1, 0, 0, 2000);
                _Pozyx.AddAnchor(0x6029, 1, 0, 0, 2000, _FriendId);
                await Task.Delay(200);
                _Pozyx.AddAnchor(0x6047, 1, 7000, 0, 2000);
                _Pozyx.AddAnchor(0x6047, 1, 7000, 0, 2000, _FriendId);
                await Task.Delay(200);
                _Pozyx.AddAnchor(0x605B, 1, 0, 5100, 2000);
                _Pozyx.AddAnchor(0x605B, 1, 0, 5100, 2000, _FriendId);
                await Task.Delay(200);
                _Pozyx.AddAnchor(0x6038, 1, 6750, 5100, 2000);
                _Pozyx.AddAnchor(0x6038, 1, 6750, 5100, 2000, _FriendId);
                await Task.Delay(200);

                //_Pozyx.AddAnchor(0x6957, 1, 0, 2560, 1100);
                //await Task.Delay(200);
                //_Pozyx.AddAnchor(0x697C, 1, 3500, 0, 1500);
                //await Task.Delay(200);
                //_Pozyx.AddAnchor(0x697D, 1, 7000, 2540, 950);
                //await Task.Delay(200);
                //_Pozyx.AddAnchor(0x6956, 1, 3500, 5200, 2000);
                //await Task.Delay(200);

                EnableButtons();
            }
        }

        private async void Button3_Click(object sender, RoutedEventArgs e)
        {
            DisableButtons();

            if (_Running)
            {
                StopRunning();

                List<string> ExportData = new List<string>();

                string testcase = "";
                ExportData.Add("sep=;");

                ExportData.Add("Testcase;" + testcase);
                ExportData.Add("X;Y;Z;Timestamp");
                for (int i = 0; i < _PositionList.Count; i++)
                {
                    ExportData.Add(_PositionList[i].X + ";" + _PositionList[i].Y + ";" + _PositionList[i].Z + ";" + _TimestampList[i].ToLocalTime().ToString("H:mm:s.ff"));
                }

                StorageFolder folder = ApplicationData.Current.LocalFolder;
                string fileName = testcase + "-" + DateTime.Now.ToLocalTime().ToString("dd-MMMM-yy H-mm") + ".csv";
                StorageFile sample = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

                await FileIO.WriteLinesAsync(sample, ExportData);
            }
            else
            {
                await _Pozyx.DoAnchorDiscovery();
            }

            EnableButtons();
        }

        private async void Button4_Click(object sender, RoutedEventArgs e)
        {
            if (_Running) // Stop
            {
                StopRunning();
            }
            else if (_Pozyx.Anchors.Count >= 4) // Start
            {
                DisableButtons();

                await _Pozyx.SetRecommendedConfigurations();

                await Task.Delay(1000);
                _Pozyx.ConfigurationRegisters.PosAlg(4, 3, _FriendId);

                this.StartRunning();

                EnableButtons();
            }
            else
            {
                this.Output.Text = "Set anchors first";
            }
        }

        private async void Button5_Click(object sender, RoutedEventArgs e)
        {
            if (_Running) //wipe the lists
            {
                this._LinePoints = new List<float[]>();
                this._PositionList = new List<Position>();
                this._TimestampList = new List<DateTime>();
            }
            else
            {
                DisableButtons();

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

                await _Testcase.DoTest(timespan, interval, testCase, catagory, description);

                progress.Stop();
                this.Output.Text = "Test finished";

                EnableButtons();
            }
        }

        void UpdateScreen_Tick(object sender, object e)
        {
            this.Output.Text = "x: " + _FriendPosition.X + "\t y: " + _FriendPosition.Y + "\t z: " + _FriendPosition.Z;

            String timeStamp = DateTime.Now.ToString();
            this.Timestamp.Text = "Timestamp: " + timeStamp;

            this.GridCanvas.Invalidate();


            string err = _Pozyx.StatusRegisters.ErrorCode();
            if (err != "0x00 - Success")
            {
                Debug.WriteLine("ERROR: " + err);
            }
        }

        async void UpdatePosition_Tick(object sender, object e)
        {
            // UPdate postion
            _Pozyx.RegisterFunctions.DoPositioning();
            Position position = _Pozyx.PositioningData.Pos();
            _PositionList.Add(position);
            _TimestampList.Add(DateTime.Now);
            _LinePoints.Add(new float[] { (float)(position.X * _PixelSize + _Space), (float)(position.Y * _PixelSize + _Space) });

            await Task.Delay(1000);

            // friend do pos
            _Pozyx.RegisterFunctions.DoPositioning(_FriendId);
            _FriendPosition = _Pozyx.PositioningData.Pos(_FriendId);
        }

        void progress_Tick(object sender, object e)
        {
            this.Output.Text = _Testcase.Status;
        }

        /// <summary>
        ///     Starts the updating the postion and sceen
        /// </summary>
        private void StartRunning()
        {
            _Running = true;
            UpdateScreen.Interval = new TimeSpan(0, 0, 0, 0, 1000 / 60);
            UpdatePosition.Interval = new TimeSpan(0, 0, 0, 0, 300);
            UpdateScreen.Start();
            UpdatePosition.Start();

            GridCanvas.Visibility = Visibility.Visible;

            Button1.IsEnabled = false;

            Button2.IsEnabled = false;

            Button3.Content = "Export";
            Button3.IsEnabled = true;

            Button4.Content = "Stop";
            Button4.IsEnabled = true;

            Button5.Content = "Erase";
            Button5.IsEnabled = true;
        }

        /// <summary>
        ///     Stops the updating the postion and sceen
        /// </summary>
        private void StopRunning()
        {
            _Running = false;

            UpdateScreen.Stop();
            UpdatePosition.Stop();
            _LinePoints = new List<float[]>();
            GridCanvas.Visibility = Visibility.Collapsed;

            Button1.Content = "Small room";
            Button1.IsEnabled = true;

            Button2.Content = "Big room";
            Button2.IsEnabled = true;

            Button3.Content = "Discover";
            Button3.IsEnabled = true;

            Button4.Content = "Start";
            Button4.IsEnabled = true;

            Button5.Content = "Run test";
            Button5.IsEnabled = true;
        }

        /// <summary>
        ///     Enables the buttons
        /// </summary>
        private void EnableButtons()
        {
            if (!_Running)
            {
                Button1.IsEnabled = true;
                Button2.IsEnabled = true;
            }
            Button3.IsEnabled = true;
            Button4.IsEnabled = true;
            Button5.IsEnabled = true;
        }

        /// <summary>
        ///  Disables the buttons
        /// </summary>
        private void DisableButtons()
        {
            Button1.IsEnabled = false;
            Button2.IsEnabled = false;
            Button3.IsEnabled = false;
            Button4.IsEnabled = false;
            Button5.IsEnabled = false;
        }
    }
}
