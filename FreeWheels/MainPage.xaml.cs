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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FreeWheels
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Pozyx _Pozyx;

        public MainPage()
        {
            this.InitializeComponent();
            //PozyxApi.Connect();
            _Pozyx = new Pozyx();
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
                byte[] result = Connection.ReadWrite(request, nReturnBytes);

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
            Output.Text = "Firmware:" + PozyxApiBase.GetFirmwareVersion();
        }

        private void Discover_Click(object sender, RoutedEventArgs e)
        {

            if (!PozyxApiBase.DiscoverDevices())
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

            Output.Text = "Number of devices: " + GeneralData.GetDeviceListSize();

        }

        private async void Calibrate_Click(object sender, RoutedEventArgs e)
        {
            if (!PozyxApiBase.CalibrateDevices())
            {
                Output.Text = "Calibrate Devices: FAILED \n";
            }
            else
            {
                Output.Text = "Calibrate Devices: SUCCESS \n";
            }
        }

        private async void StartPos_Click(object sender, RoutedEventArgs e)
        {
            if (!PozyxApiBase.StartPositioning())
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

            List<byte[]> anchorIds = PozyxApiBase.GetAnchorIds();
            Anchor[] anchors = new Anchor[anchorIds.Count];

            for (int i = 0; i < anchors.Length; i++)
            {
                anchors[i] = new Anchor(anchorIds[i]);
                anchors[i].Position = PozyxApiBase.GetAnchorPosition(anchors[i].Id);

                Output.Text += anchors[i].Id[0] + " - " + anchors[i].Id[1] + " \n";
                Debug.Write(anchors[i].Id[0] + " - " + anchors[i].Id[1] + " \n");
                Output.Text += "x: " + anchors[i].Position.X + "\t y: " + anchors[i].Position.Y + "\t z: " + anchors[i].Position.Z + "\n";
                Debug.Write("x: " + anchors[i].Position.X + "\t y: " + anchors[i].Position.Y + "\t z: " + anchors[i].Position.Z + "\n");
            }

        }

        private void SelfTest_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = "";

            List<string> selfTestResult = PozyxApiBase.SelfTest();

            if (selfTestResult.Count <= 0)
            {
                Output.Text = "selfTest Passed";
                Debug.Write("selfTest Passed");
            }
            foreach (string str in selfTestResult)
            {
                Output.Text += str + " \n";
                Debug.Write(str + " \n");
            }
        }

        private void Interval_Click(object sender, RoutedEventArgs e)
        {
            //int[] bla= ConfigurationRegisters.IntConfig();
            //int a = 0;
            bool test = _Pozyx.LetsGo();
            Output.Text = "";
        }

        private void Anchors_Click(object sender, RoutedEventArgs e)
        {
            bool test = RegisterFunctions.TXData(0, new byte[] { 0x0 });
            Output.Text = "TXDATA: " + test.ToString();
        }

        private void ErrorCode_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = "";

            string errorCodeResult = PozyxApiBase.GetErrorCode();


            Output.Text += errorCodeResult + " \n";
            Debug.Write(errorCodeResult + " \n");
        }

        private void Status_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = "";

            List<string> status = PozyxApiBase.IntStatus();

            if (status.Count == 0)
            {
                Output.Text = "No Status Update";
            }
            foreach (string str in status)
            {
                Output.Text += str + " \n";
            }
        }

        private void Pos_Click(object sender, RoutedEventArgs e)
        {
            Position myPosition = _Pozyx.MyPozyx.Position;
            Output.Text = "x: " + myPosition.X + "\t y: " + myPosition.Y + "\t z: " + myPosition.Z + "\n\n";
            Debug.Write("x: " + myPosition.X + "\t y: " + myPosition.Y + "\t z: " + myPosition.Z + "\n\n");
        }

        private void CalibStat_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = "";

            List<string> status = PozyxApiBase.CalibStatus();

            foreach (string str in status)
            {
                Output.Text += "Done: " + str + " \n";
            }
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = "Reset failed";
            if (_Pozyx.Reset())
            {
                Output.Text = "Reset succes";
            }
        }
        
        private void General_Click(object sender, RoutedEventArgs e)
        {
        }

    }
}
