using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Devices.I2c.Provider;

namespace App1.Pozyx
{

    public class PozyxShield
    {
        private const int POZYX_I2C_ADDRESS = 0x4B;
        private I2cDevice _PozyxShield;

        /// <summary>
        /// Start I2C bus
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            // get I2C selector string
            string selector = I2cDevice.GetDeviceSelector();

            // get all devices
            IReadOnlyList<DeviceInformation> devices = await DeviceInformation.FindAllAsync(selector);

            // create I2C connection string
            var Pozyx_settings = new I2cConnectionSettings(POZYX_I2C_ADDRESS);

            // If this next line crashes with an ArgumentOutOfRangeException, 
            // then the problem is that no I2C devices were found. 
            // 
            // If the next line crashes with Access Denied, then the problem is 
            // that access to the I2C device (HTU21D) is denied. 
            // 
            // The call to FromIdAsync will also crash if the settings are invalid. 
            // 
            // FromIdAsync produces null if there is a sharing violation on the device. 
            // This will result in a NullReferenceException in Timer_Tick below. 
            _PozyxShield = await I2cDevice.FromIdAsync(devices[0].Id, Pozyx_settings);
        }

        /// <summary>
        /// Stop I2C bus
        /// </summary>
        public void Stop()
        {
            // Release the I2C sensor. 
            if (_PozyxShield != null)
            {
                _PozyxShield.Dispose();
                _PozyxShield = null;
            }
        }
    }
}
