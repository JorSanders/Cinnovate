using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.UI.Xaml;

namespace App1.Models
{
    class Pozyx
    {

        private const int POZYX_I2C_ADDRESS = 0x4B;
        private I2cDevice _PozyxShield;
        private DispatcherTimer _Timer;

        /*
         * initiate the i2c connection to the pozyx device
         */
        private async Task Connect()
        {
            string i2cDeviceSelector = I2cDevice.GetDeviceSelector();

            IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);

            var Pozyx_settings = new I2cConnectionSettings(POZYX_I2C_ADDRESS);

            _PozyxShield = await I2cDevice.FromIdAsync(devices[0].Id, Pozyx_settings);
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

        private void request(byte sendByte)
        {
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

    }
}
