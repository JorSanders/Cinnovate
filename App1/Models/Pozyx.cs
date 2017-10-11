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
         * Send a byte i2c device
         */ 
        private byte[] Request(byte[] request, int length)
        {
            try
            {
                var data = new byte[length];
                _PozyxShield.WriteRead(request, data);
                return data;
            }
            catch (Exception ex)
            {
                return new byte[0];
            }
        }

        /*
         * Returns the firmware version
         */
        public string GetFirmwareVersion()
        {
            byte[] request = { 0x1 };
            byte[] answerBytes = Request(request, 1);

            if (answerBytes.Length > 0)
            {
                UInt16 minorVersion = (byte)(answerBytes[0] & 0x1f);
                UInt16 majorVersion = (byte)(answerBytes[0] >> 4);

                return majorVersion + "." + minorVersion;
            }
            else
            {
                return "ERR: Failed to get firmware";
            }
        }

    }
}
