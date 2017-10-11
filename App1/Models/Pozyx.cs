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

        public Pozyx()
        {
            Connect();
        }

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

        /*
         * Send a byte via to an i2c device
         */ 
        private byte[] Request(byte sendByte, int lenght)
        {
            // Read data from I2C.
            byte[] result = new byte[lenght];
            byte[] sendByteArray = { sendByte };

            try
            {
                _PozyxShield.WriteRead(sendByteArray, result);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.Message);
                byte[] empty = { };
                return empty;
            }
            return result;
        }

        /*
         * Returns the firmware version
         */
        public string GetFirmwareVersion()
        {
            byte requestByte = 0x1;
            byte[] answerBytes = Request(requestByte, 1);
            if (answerBytes.Length <= 0)
            {
                return "Failed to get firmware";
            }
            byte answerByte = answerBytes[0];

            // 00001111 Because we need the first 4 binairy characters from the byte
            byte minorPart = 0x1f;
            UInt16 minorVersion = (byte)(answerByte & minorPart);
            // shift the byte 4 times. Because we only need the first 4 binairy characters
            UInt16 majorVersion = (byte)(answerByte >> 4);

            return majorVersion + "." + minorVersion;
        }

    }
}
