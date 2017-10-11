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
using App1.Pozyx;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private const int POZYX_I2C_ADDRESS = 0x4B;
        private I2cDevice _PozyxShield;
        private DispatcherTimer _Timer;

        public MainPage()
        {
            this.InitializeComponent();
            this.Start();
        }

        private async Task Start()
        {
            //PozyxDevice device = new PozyxDevice();

            string i2cDeviceSelector = I2cDevice.GetDeviceSelector();
            IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);

            var Pozyx_settings = new I2cConnectionSettings(POZYX_I2C_ADDRESS);

            _PozyxShield = await I2cDevice.FromIdAsync(devices[0].Id, Pozyx_settings);

        }

        private void GetData()
        //private void Timer_Tick(object sender, object e)
        {
            try
            {
                TextBlock1.Text = "Check your debugger!";

                byte code = Convert.ToByte(TextBox1.Text, 16);
                int bits = Int32.Parse(TextBox2.Text);

                // Read data from I2C.
                var command = new byte[1];
                var data = new byte[bits];

                command[0] = code;

                _PozyxShield.WriteRead(command, data);

                string str = String.Empty;
                for (int i = 0; i < data.Length; i++)
                {
                    str = str + "/ " + data[i].ToString() + " ";
                }

                TextBlock1.Text = "Result: " + str;
                //TextBlock2.Text = "String: " + System.Text.Encoding.UTF8.GetString(data);
            }
            catch (Exception ex) { };

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }

        private void TextBlock_SelectionChanged(System.Object sender, RoutedEventArgs e)
        {

        }

        private void ButtonByte_Click(object sender, RoutedEventArgs e)
        {

            DecodeByte(ByteInput.Text);
        }

        private void DecodeByte(string ByteInputted)
        {
            byte result = 0x23;
            byte versionPart = 0x1f;
            byte typePart = 0xE0;
            UInt16 version = (byte)(result & versionPart);
            byte typeShifted = (byte)(result & typePart);
            UInt16 type = (byte)(typeShifted >> 5);
        }

        private void OutputByte_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

    }
}
