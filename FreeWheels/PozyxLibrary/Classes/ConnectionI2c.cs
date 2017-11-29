using FreeWheels.PozyxLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace FreeWheels.PozyxLibrary.Classes
{
    public class ConnectionI2c : IConnection
    {
        private I2cDevice _PozyxShield;
        private int _I2cAddress = 0x4b;
        public bool connected { get; private set; }

        public ConnectionI2c(int i2cAddress = 0x4b)
        {
            connected = false;
            _I2cAddress = i2cAddress;
        }

        public async Task Connect()
        {
            string i2cDeviceSelector = I2cDevice.GetDeviceSelector();

            IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(i2cDeviceSelector);

            var pozyxI2cDevice = new I2cConnectionSettings(_I2cAddress);

            try
            {
                _PozyxShield = await I2cDevice.FromIdAsync(devices[0].Id, pozyxI2cDevice);
                connected = true;
            }
            catch (Exception e)
            {
                connected = false;
            }
        }
        /// <summary>
        ///     I2C readwrite a byte array to the Pozyx
        /// </summary>
        /// <param name="request">Bytearray with containing the request</param>
        /// <param name="length">Length of the return array</param>
        /// <returns>Bytearray with the length provided. If an error occured the bytearray will be empty but of the proper length</returns>
        public byte[] ReadWrite(byte[] request, int length)
        {
            try
            {
                var data = new byte[length];
                _PozyxShield.WriteRead(request, data);
                return data;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error during readwriting to Poyxyz for register: 0x" + request[0].ToString("X2"));
                Debug.WriteLine("Exception: " + ex.Message);
                return new byte[length];
            }
        }

        /// <summary>
        ///     I2C write a byte array
        /// </summary>
        /// <param name="request">Bytearray to write</param>
        public void Write(byte[] request)
        {
            try
            {
                _PozyxShield.Write(request);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error during writing to Poyxyz for register: 0x" + request[0].ToString("X2"));
                Debug.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}
