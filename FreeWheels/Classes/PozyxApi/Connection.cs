using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace FreeWheels.Classes.PozyxApi
{
    public static class Connection
    {
        private const int POZYX_I2C_ADDRESS = 0x4B;
        private static I2cDevice _PozyxShield;

        /*
         * initiate the i2c connection to the pozyx device
         */
        public static async Task Connect()
        {
            string i2cDeviceSelector = I2cDevice.GetDeviceSelector();

            IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);

            var Pozyx_settings = new I2cConnectionSettings(POZYX_I2C_ADDRESS);

            _PozyxShield = await I2cDevice.FromIdAsync(devices[0].Id, Pozyx_settings);
        }

        /*
         * Send a byte i2c device
         */
        public static byte[] ReadWrite(byte[] request, int length)
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

        public static void Write(byte[] data)
        {
            try
            {
                _PozyxShield.Write(data);
            }
            catch (Exception ex)
            {
            }
        }

        public static byte[] RemoteReadWrite(byte[] networkID, byte[] request, int length)
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

        public static void RemoteWrite(byte[] networkID, byte[] data)
        {
            try
            {
                _PozyxShield.Write(data);
            }
            catch (Exception ex)
            {
            }
        }

    }
}
