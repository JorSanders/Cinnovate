using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public byte[] Request(byte[] request, int length)
        {
            try
            {
                var data = new byte[length];
                _PozyxShield.WriteRead(request, data);
                return data;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                return new byte[0];
            }
        }

        /*
         * Returns the firmware version
         */
        public string GetFirmwareVersion()
        {
            byte[] request = { 0x1 };
            byte[] data = Request(request, 1);

            if (data.Length > 0)
            {
                UInt16 minorVersion = (byte)(data[0] & 0x1f);
                UInt16 majorVersion = (byte)(data[0] >> 4);

                return majorVersion + "." + minorVersion;
            }
            else
            {
                return "ERR: Failed to get firmware";
            }
        }

        /*
         * Discover all the devices
         * 
         * @param Devicetype
         *          0 achors only
         *          1 tags only
         *          2 all devices
         * @param number of devices
         * @param waitTime time in seconds
         * 
         * @return bool
         */
        public bool DiscoverDevices(int deviceType = 0, int devices = 10, int waitTime = 10)
        {
            byte[] request = { 0xC1, (byte)deviceType, (byte)devices, (byte)waitTime };
            byte[] data = Request(request, 1);

            if (data.Length > 0 && data[0] == 1)
            {
                return true;
            }

            return false;
        }

        /*
         * Returns the number of devices stored internally
         */
        public int GetDeviceListSize()
        {
            byte[] request = { 0x81 };
            byte[] data = Request(request, 1);

            if (data.Length > 0)
            {
                return data[0];
            }

            return 0;
        }

        /*
         * Starts the positioning proces
         */
        public bool StartPositioning()
        {
            byte[] request = { 0xB6 };
            byte[] data = Request(request, 1);

            if (data.Length > 0 && data[0] == 1)
            {
                return true;
            }

            return false;
        }

        /*
         * returns up to 20 anchor ids
         */
        public byte[][] GetAnchorIds()
        {
            byte[] request = { 0xB8 };
            byte[] data = Request(request, 33);


            if (data.Length <= 0 || data[0] != 1)
            {
                byte[][] empty = { };
                return empty;
            }

            byte[][] result = new byte[data.Length / 2][];

            for (int i = 1; i + 1 < data.Length; i += 2)
            {
                /*
                if (data[i] == 0 && data[i + 1] == 0)
                {
                    return result;
                }
                */
                result[(i + 1) / 2 - 1] = new byte[2];
                result[(i + 1) / 2 - 1][0] = data[i];
                result[(i + 1) / 2 - 1][1] = data[i + 1];
            }

            return result;
        }

        public void GetAnchorPosition(byte[] anchorId)
        {
            byte[] request = { 0xC6, anchorId[0], anchorId[1] };
            byte[] data = Request(request, 13);

            if (data.Length <= 0 || data[0] != 1)
            {

            }


            byte[] xBytes = { data[1], data[2], data[3], data[4] };
            byte[] yBytes = { data[5], data[6], data[7], data[8] };
            byte[] zBytes = { data[9], data[10], data[11], data[12] };


            int x = BitConverter.ToInt32(xBytes, 0);
            int y = BitConverter.ToInt32(yBytes, 0);
            int z = BitConverter.ToInt32(zBytes, 0);



        }
    }
}
