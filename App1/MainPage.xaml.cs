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
using App1.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
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

        private void Version_Click(object sender, RoutedEventArgs e)
        {
            string firmwareVersion = _Pozyx.GetFirmwareVersion();
            Output.Text = firmwareVersion;
        }

        private void Request_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the user inputted values
                string inputByte = InputBytes.Text;
                string[] byteStrings = inputByte.Split(new[] { ";" }, StringSplitOptions.None);
                byte[] request = new byte[byteStrings.Length];
                for (int i = 0; i < byteStrings.Length; i++)
                {
                    request[i] = Convert.ToByte(byteStrings[i], 16);
                }
                int returnBytes = Int32.Parse(NumberOfReturnBytes.Text);

                byte[] result = _Pozyx.Request(request, returnBytes);

                Output.Text = "";
                foreach (byte returnByte in result)
                {
                    Output.Text += returnByte + "\n";
                }

            }
            catch (Exception ex)
            {
                Output.Text = ex.Message;
            }
        }
    }
}
