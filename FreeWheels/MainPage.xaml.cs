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
using FreeWheels.Models;

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
            //this.Start();
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
                byte[] result = _Pozyx.Request(request, nReturnBytes);

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
            string firmwareVersion = _Pozyx.GetFirmwareVersion();
            Output.Text = firmwareVersion;
        }

        private void Achors_Click(object sender, RoutedEventArgs e)
        {
            Output.Text = "";

            if (!_Pozyx.DiscoverDevices())
            {
                Output.Text += "Discover failed \n";
                Debug.Write("Discover failed \n");
            }

            if (_Pozyx.GetDeviceListSize() <= 0)
            {
                Output.Text += "Device List empty \n";
                Debug.Write("Device List empty \n");
            }

            if (!_Pozyx.StartPositioning())
            {
                Output.Text += "Positioning failed \n";
                Debug.Write("Positioning failed \n");
            }
            byte[][] anchorIds = _Pozyx.GetAnchorIds();

            foreach (byte[] anchorId in anchorIds)
            {
                Output.Text += anchorId[0] + " - " + anchorId[1] + " \n";
                Debug.Write(anchorId[0] + " - " + anchorId[1] + " \n");
            }
            
        }

        private void AchorsPos_Click(object sender, RoutedEventArgs e)
        {
            //0x47 0x60
            byte[] anchorId = { 0x47, 0x60 };
            _Pozyx.GetAnchorPosition(anchorId);
        }
    }
}
