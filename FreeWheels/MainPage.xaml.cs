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
using FreeWheels.Api;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FreeWheels
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //private Pozyx _Pozyx;

        public MainPage()
        {
            this.InitializeComponent();
            PozyxApi.Connect();
            //this.Start();
            //_Pozyx = new Pozyx();
        }

        private void Request_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create an array strings with the hex codes, then cast to a byte array
                string[] byteStrings = InputBytes.Text.Split(new[] { ";" }, StringSplitOptions.None);
                byte[] request = new byte[byteStrings.Length];
                for (int i = 0; i < byteStrings.Length; i++)
                {
                    request[i] = Convert.ToByte(byteStrings[i], 16);
                }
                int nReturnBytes = Int32.Parse(NumberOfReturnBytes.Text);

                //make the request
                byte[] result = PozyxApi.Request(request, nReturnBytes);

                //wipe output text
                Output.Text = "";

                Debug.Write(InputBytes.Text + "\n");
                Debug.Write(nReturnBytes + "\n");
                foreach (byte returnByte in result)
                {
                    Output.Text += returnByte + "\n";

                    Debug.Write(returnByte + "\n");
                }

            }
            catch (Exception ex)
            {
                Output.Text = ex.Message;
            }
        }


        private void Version_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = "Firmware:" + _Pozyx.GetFirmwareVersion();
        }

        private void Discover_Click (object sender, RoutedEventArgs e)
        {

            if (!PozyxApi.DiscoverDevices())
            {
                Output.Text = "Discover: FAILED \n";
            }
            else
            {
                Output.Text = "Discover: SUCCES";
            }
            
        }

        private async void DevList_Click(object sender, RoutedEventArgs e)
        {

            Output.Text = "Number of devices: " + PozyxApi.GetDeviceListSize();

        }

        private async void Calibrate_Click (object sender, RoutedEventArgs e)
        {
            if (!PozyxApi.CalibrateDevices())
            {
                Output.Text = "Calibrate Anchors: FAILED \n";
            }
            else
            {
                Output.Text = "Calibrate Anchors: SUCCESS \n";
            }
        }

        private async void StartPos_Click(object sender, RoutedEventArgs e)
        {
            if (!PozyxApi.StartPositioning())
            {
                Output.Text = "Start Positioning: FAILED \n";
            }
            else
            {
                Output.Text = "Start Positioning: SUCCESS \n";
            }
        }

        private async void AnchorsPos_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = "";

            List<byte[]> anchorIds = PozyxApi.GetAnchorIds();
            Device[] anchors = new Device[anchorIds.Count];

            for (int i = 0; i < anchors.Length; i++)
            {
                anchors[i] = new Device(anchorIds[i]);
                anchors[i].Position = PozyxApi.GetAnchorPosition(anchors[i].Id);

                Output.Text += anchors[i].Id[0] + " - " + anchors[i].Id[1] + " \n";
                Debug.Write(anchors[i].Id[0] + " - " + anchors[i].Id[1] + " \n");
                Output.Text += "x: " + anchors[i].Position.X + "\t y: " + anchors[i].Position.Y + "\t z: " + anchors[i].Position.Z + "\n";
                Debug.Write("x: " + anchors[i].Position.X + "\t y: " + anchors[i].Position.Y + "\t z: " + anchors[i].Position.Z + "\n");
            }

        }

        private void SelfTest_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = "";

            List<string> selfTestResult = PozyxApi.SelfTest();

            if (selfTestResult.Count <= 0)
            {
                Output.Text = "selfTest Passed";
                Debug.Write("selfTest Passed");
            }
            foreach (string r in selfTestResult)
            {
                Output.Text += r + " \n";
                Debug.Write(r + " \n");
            }
        }
    }
}
