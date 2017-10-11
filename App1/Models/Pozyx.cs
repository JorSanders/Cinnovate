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

        private int[] DecodeByte(byte ByteInputted)
        {
            byte versionPart = 0x1f;
            byte typePart = 0xE0;
            UInt16 version = (byte)(ByteInputted & versionPart);
            byte typeShifted = (byte)(ByteInputted & typePart);
            UInt16 type = (byte)(typeShifted >> 5);

            int[] result = { version, type };

            return result;
        }

        private byte[] request(byte sendByte, int lenght)
        {
            // Read data from I2C.
            byte[] command = new byte[1];
            byte[] result = new byte[lenght];

            byte[] sendByteArray = { sendByte };

            _PozyxShield.WriteRead(sendByteArray, result);

            return result;
        }

    }
}
