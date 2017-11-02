using FreeWheels.Classes;
using FreeWheels.Enums;
using FreeWheels.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.UI.Xaml;

namespace FreeWheels.Classes
{
    public static class PozyxApi //: IPozyx
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
        public static byte[] Request(byte[] request, int length)
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

        public static void Write(byte[] data)
        {
            try
            {
                _PozyxShield.Write(data);
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
        }


        public static byte[] Read(byte[] buffer)
        {
            try
            {
                _PozyxShield.Read(buffer);
                return buffer;
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
        public static string GetFirmwareVersion()
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

        public static List<string> IntStatus()
        {
            List<string> status = new List<string>();

            byte[] request = { 0x5 };
            byte[] data = Request(request, 1);

            byte onlyLast = 0x1;

            string[] statuscodes = new string[5];
            statuscodes[0] = "ERR: An has error occured";
            statuscodes[1] = "POS: A new position estimate is available";
            statuscodes[2] = "IMU: A new IMU measurement is available";
            statuscodes[3] = "RX_DATA: The pozyx device has received some data over its wireless uwb link";
            statuscodes[4] = "FUNC: A register function call has finished (excluding positioning)";

            for (int i = 0; i < statuscodes.Length; i++)
            {
                byte shifted = (byte)(data[0] >> (byte)i);
                if ((int)(shifted & onlyLast) == 1)

                    status.Add(statuscodes[i]);
            }

            return status;
        }

        public static List<string> CalibStatus()
        {
            List<string> status = new List<string>();

            byte[] request = { 0x6 };
            byte[] data = Request(request, 1);

            string[] statuscodes = new string[4] { "SYS", "GYR", "ACC", "MAG" };

            for (int i = 0; i < 2 * statuscodes.Length; i = i + 2)
            {
                if ((byte)((data[0] >> i) & 0x03) == 0x03)
                {
                    status.Add(statuscodes[i / 2]);
                }
            }

            return status;
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
        public static bool DiscoverDevices(int deviceType = 0, int devices = 10, int waitTime = 10)
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
        public static int GetDeviceListSize()
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
        public static bool StartPositioning()
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
        public static List<byte[]> GetAnchorIds()
        {
            byte[] request = { 0xB8 };
            byte[] data = Request(request, 33);

            List<byte[]> result = new List<byte[]>();

            if (data.Length <= 0 || data[0] != 1)
            {
                return result;
            }

            //byte[][] result = new byte[data.Length / 2][];

            for (int i = 1; i + 1 < data.Length; i += 2)
            {

                if (data[i] == 0 && data[i + 1] == 0)
                {
                    return result;
                }

                byte[] id = new byte[] { data[i], data[i + 1] };
                result.Add(id);
            }

            return result;
        }

        public static Position GetAnchorPosition(byte[] anchorId)
        {
            byte[] request = { 0xC6, anchorId[0], anchorId[1] };
            byte[] data = Request(request, 13);

            if (data.Length <= 0 || data[0] != 1)
            {
                return new Position();
            }

            byte[] xBytes = { data[1], data[2], data[3], data[4] };
            byte[] yBytes = { data[5], data[6], data[7], data[8] };
            byte[] zBytes = { data[9], data[10], data[11], data[12] };

            Position position = new Position
            {
                X = BitConverter.ToInt32(xBytes, 0),
                Y = BitConverter.ToInt32(yBytes, 0),
                Z = BitConverter.ToInt32(zBytes, 0)
            };

            return position;
        }

        public static List<string> SelfTest()
        {
            byte[] request = { 0x3 };
            byte[] data = Request(request, 1);

            List<string> errors = new List<string>();

            if (data.Length <= 0)
            {
                errors.Add("Nothing Returned");
                return errors;
            }

            byte result = data[0];

            byte onlyLast = 0x1;

            string[] errorcodes = new string[6];
            errorcodes[0] = "ACC";
            errorcodes[1] = "MAGN";
            errorcodes[2] = "GYRO";
            errorcodes[3] = "IMU";
            errorcodes[4] = "PRESS";
            errorcodes[5] = "UWB";


            for (int i = 0; i < errorcodes.Length; i++)
            {
                byte shifted = (byte)(result >> (byte)i);
                if ((int)(shifted & onlyLast) != 1)

                    errors.Add(errorcodes[i]);
            }

            return errors;
        }


        public static bool CalibrateDevices()
        {
            //byte[] request = { 0xC2, 0x02, 0x38, 0x60, 0x5B, 0x60, 0x29, 0x60, 0x47, 0x60};
            byte[] request = { 0xC2 };
            byte[] data = Request(request, 1);

            if (data.Length > 0 && data[0] == 1)
            {
                return true;
            }

            return false;
        }

        /*
         * Do the ranging for a given Device
         * 
         * @param deviceId
         * 
         * @return bool
         */
        public static bool StartRanging(byte[] deviceId)
        {
            byte[] request = new byte[3];
            request[0] = 0xB5;
            request[1] = deviceId[0];
            request[2] = deviceId[1];

            byte[] data = Request(request, 1);

            if (data[0] == 1)
            {
                return true;
            }
            return false;
        }

        /*
         * Do the ranging for a given Device
         * 
         * @param deviceId
         * 
         * @return bool
         */
        public static bool DoRanging(byte[] deviceId)
        {
            byte[] request = { 0xB5, deviceId[0], deviceId[1] };
            byte[] data = Request(request, 1);

            return (data[0] == 1);
        }

        public static RangeInfo GetRangeInfo(byte[] deviceId)
        {
            byte[] request = { 0xC7, deviceId[0], deviceId[1] };
            byte[] data = Request(request, 11);

            if (data[0] == 1)
            {
                int timestamp = BitConverter.ToInt32(new byte[] { data[1], data[2], data[3], data[4] }, 0);
                int lastmeasurement = BitConverter.ToInt32(new byte[] { data[5], data[6], data[7], data[8] }, 0);
                int signalstrength = BitConverter.ToInt32(new byte[] { data[9], data[10] }, 0);

                return new RangeInfo(timestamp, lastmeasurement, signalstrength);
            }

            return new RangeInfo();

        }
        public static string GetErrorCode()
        {
            byte[] request = { 0x4 };
            byte[] data = Request(request, 1);


            if (data.Length <= 0)
            {
                string errors = "Empty result";
                return errors;
            }
            if (data.Length > 1)
            {
                string errors = "More than one error was found";
                return errors;
            }

            //Stores the result in a variable
            int result = data[0];

            //convert int result to hex
            string hexResult = result.ToString("X2");

            //Gets the variable name that is assigned to the result of the error
            string stringValue = Enum.GetName(typeof(PozyxErrorCode), result);

            if (result == 0)
            {
                return stringValue;
            }
            else
            {
                return "Error: " + stringValue + "\nHex code: 0x" + hexResult + "\n";
            }

        }

        public static int PosX()
        {
            byte[] request = { 0x30 };
            byte[] data = Request(request, 4);

            return BitConverter.ToInt32(data, 0);
        }

        public static int PosY()
        {
            byte[] request = { 0x34 };
            byte[] data = Request(request, 4);

            return BitConverter.ToInt32(data, 0);
        }

        public static int PosZ()
        {
            byte[] request = { 0x38 };
            byte[] data = Request(request, 4);

            return BitConverter.ToInt32(data, 0);
        }

        public static bool Reset()
        {
            byte[] request = { 0xB0 };
            byte[] data = Request(request, 1);

            return (data.Length > 0 && data[0] == 1);
        }

        /// <summary>
        ///     This register configures the external interrupt pin of the Pozyx device. It should be configured in combination with the POZYX_INT_MASK register.
        /// </summary>
        /// <param name="pinNum">
        ///     Selects the pin used by the interrupt. Possible values:
        ///     0 - No pin(default)
        ///     1 - GPIO pin 1 (pin 9 on the pozyx tag)
        ///     2 - GPIO pin 2 (pin 10 on the pozyx tag)
        ///     3 - GPIO pin 3 (pin 11 on the pozyx tag)
        ///     4 - GPIO pin 4 (pin 12 on the pozyx tag)
        ///     5 - GPIO pin 5 (pin 2 on the pozyx tag)
        ///     6 - GPIO pin 6 (pin 3 on the pozyx tag)
        /// </param>
        /// <param name="mode">
        ///     Select the interrupt pin mode. Possible values:
        ///     0 - Push-pull(default): the pin will actively set the interrupt line high or low.The line cannot be shared with multiple devices.
        ///     1 - open drain: this allows the user to share the interrupt line with multiple devices.This mode requires an external pull-up or pull-down resistor.
        /// </param>
        /// <param name="act">
        ///     The voltage level when an interrupt happens. Possible values:
        ///     0 - active low(default): 0V
        ///     1 - active high: 3.3V
        /// </param>
        /// <param name="latch">
        ///     Select if the interrupt pin should latch after an interrupt. Possible values:
        ///     0 - No latch(default): the interrupt is a short pulse of about 6µs
        ///     1 - Latch: after an interrupt, the interrupt pin will stay at the active level until the POZYX_INT_STATE register is read from
        /// </param>
        public static void IntConfig(int pinNum, int mode, int act, int latch)
        {
            byte parameters = (byte)pinNum;

            int[] options = { mode, act, latch};

            for (int i = 0; i < options.Count(); i++)
            {
                if (options[i] == 1)
                {
                    parameters = (byte)(0x1 << (byte)i+3 | parameters);
                }
            }

            byte[] request = { 0x11, parameters };
            Write(request);
        }

        /// <summary>
        ///     See IntConfig(int pinNum, int mode, int act, int latch)
        /// </summary>
        /// <returns>
        /// int[4]
        /// 0: pinnum
        /// 1: mode
        /// 2: act
        /// 3: latch
        /// </returns>
        public static int[] IntConfig()
        {
            byte[] request = { 0x11 };
            byte[] data = Request(request, 1);

            return new int[] { data[0] & 0x7, data[0] & 0x8, data[0] & 0x20, data[0] & 0x40 };
        }

        /// <summary>
        ///     This register selects and configures any additional positioning filters used by the pozyx device.
        /// </summary>
        /// <param name="Strength">
        ///     Indicates the strength of the filter. In general, the position will be delayed by the number of samples equal to the strength.
        ///     Possible values: 0-15 
        /// </param>
        /// <param name="filter">
        /// 	This is the filter type. Possible values:
        /// 	1 : NONE(Default value). No additional filtering is applied.
        /// 	2 : FIR.A low-pass filter is applied which filters out high-frequency jitters.
        /// 	3 : MOVING_AVERAGE.A moving average filter is applied, which smoothens the trajectory.
        /// 	4 : MOVING_MEDIAN.A moving median filter is applied, which filters out outliers.
        /// </param>
        public static void PosFilter(int strength, int filter)
        {
            byte parameters = (byte)filter;

            byte strengthByte = (byte)(strength << 4);

            parameters &= strengthByte;

            byte[] request = { 0x14, parameters };
            Write(request);
        }

        /// <summary>
        ///     See PosFilter(int strength, int filter)
        /// </summary>
        /// <returns></returns>
        public static int[] PosFilter()
        {
            byte[] request = { 0x14 };
            byte[] data = Request(request, 1);

            return new int[] { data[1] & 0xF0, data[1] & 0x0F };
        }


        /// <summary>
        ///     This register configures the functionality of the 6 LEDs on the pozyx device. At all times, the user can control LEDs 1 through 4 using POZYX_LED_CTRL.
        /// </summary>
        /// <param name="led1">
        ///     Possible values:
        ///     0 : The LED is not controlled by the Pozyx system.
        ///     1 : The LED is controlled by the Pozyx system.The LED will blink (roughly) every 2s to indicate that the device is working properly.
        /// </param>
        /// <param name="led2">
        /// 	Possible values:
        /// 	0 : The LED is not controlled by the Pozyx system.
        /// 	1 : The LED is controlled by the Pozyx system.The LED will be turned on when the device is performing a register write operation or a register function (i.e., calibrating, positioning, ..). 
        /// </param>
        /// <param name="led3">
        ///     0 : The LED is not controlled by the Pozyx system.
        ///     1 : The LED is controlled by the Pozyx system.The LED will be turned on when the device is performing a register write operation or a register function(i.e., calibrating, positioning, ..). 
        /// </param>
        ///  <param name="led4">
        ///     Possible values:
        /// 	0 : The LED is not controlled by the Pozyx system.
        /// 	1 : The LED is controlled by the Pozyx system.The LED is turned on whenever an error occurs. The error can be read from the POZYX_ERRORCODE register.
        /// </param>
        /// <param name="ledRx">
        /// 	Possible values:
        /// 	0 : The LED will not blink upon reception of an UWB message.
        /// 	1 : The LED will blink upon reception of an UWB message.
        /// </param>
        /// <param name="ledTx">
        ///     Possible values:
        ///     0 : The LED will not blink upon transmission of an UWB message.
        ///     1 : The LED will blink upon transmission of an UWB message.
        /// </param>
        public static void ConfigLeds(bool led1, bool led2, bool led3, bool led4, bool ledRx, bool ledTx)
        {
            byte parameters = 0x0;

            bool[] leds = { led1, led2, led3, led4, ledRx, ledTx };

            for (int i = 0; i < leds.Count(); i++)
            {
                if (leds[i])
                {
                    parameters = (byte)(0x1 << (byte)i | parameters);
                }
            }

            byte[] request = { 0x15, parameters };
            Write(request);
        }

        /// <summary>
        ///     See ConfigLeds(bool led1, bool led2, bool led3, bool led4, bool ledRx, bool ledTx)
        /// </summary>
        /// <returns>
        ///     int[6] value 1 means led is used. 
        /// </returns>
        public static bool[] ConfigLeds()
        {
            byte[] request = { 0x15 };
            byte[] data = Request(request, 1);

            bool[] leds = new bool[6];

            for (int i = 0; i < leds.Count(); i++)
            {
                leds[i] = (data[0] >> i & 0x1) == 1;
            }

            return leds;
        }


        /// <summary>
        ///     This register selects and configures the positioning algorithm used by the pozyx device.
        /// </summary>
        /// <param name="algorith">
        ///     Indicates which algorithm to use for positioning
        ///     0: UWB-only (Default value).
        ///     4: Tracking
        /// </param>
        /// <param name="dim">
        ///     This indicates the spatial dimension. Possible values:
        ///     2 : 2D (Default value).
        ///     1 : 2,5D
        ///     3 : 3D
        /// </param>
        public static void PosAlg(int algorithm, int dim)
        {
            byte parameters = 0x0;
            parameters &= (byte)algorithm;
            byte dimByte = (byte)(dim << 4);
            parameters &= dimByte;

            byte[] request = { 0x16, parameters };
            Write(request);
        }

        /// <summary>
        ///     See PosAlg(int algorithm, int dim)
        /// </summary>
        /// <returns></returns>
        public static int[] PosAlg()
        {
            byte[] request = { 0x16 };
            byte[] data = Request(request, 1);

            int[] result = new int[2];

            result[0] = data[0] & 0xF;
            result[1] = data[1] >> 4;

            return result;
        }



        /// <summary>
        ///     Configure the number of anchors and selection procedure
        /// </summary>
        /// <param name="num">
        /// 	Indicate the maximum number of anchors to be used for positioning. Value between 3 and 15.
        /// </param>
        /// <param name="mode">
        ///     a single bit to indicate wether to choose from a fixed set of anchors or perform automatic anchor selection. Possible values:
        ///     0 : indicates fixed anchor set. 
        ///     1 : indicates automatic anchor selection.
        /// </param>
        public static void PosNumAnchors(int num, int mode)
        {
            byte parameters = 0x0;
            parameters &= (byte)num;
            byte modeByte = (byte)(mode << 7);
            parameters &= modeByte;
            byte[] request = { 0x17, parameters };

            Write(request);
        }

        /// <summary>
        ///     see PosNumAnchors(int num, int mode)
        /// </summary>
        /// <returns>
        ///     int array
        ///     0: fixed anchor set (number of anchors)
        ///     1: mode
        /// </returns>
        public static int[] PosNumAnchors()
        {
            byte[] request = { 0x17};
            byte[] data = Request(request,1);
            int[] result = new int[2];

            result[0] = data[0] & 0xF;
            result[1] = data[1] >> 7;

            return result;
        }

        /// <summary>
        /// Pozyx can be run in continuous mode to provide continuous positioning. The interval in milliseconds between successive updates can be configured with this register. 
        /// </summary>
        /// <param name="interval">The value is capped between 10ms and 60000ms (1 minute). Writing the value 0 to this registers disables the continuous mode.</param>
        public static void PosInterval(int interval)
        {
            byte[] intervalBytes = BitConverter.GetBytes(interval);
            byte[] request = { 0x18, intervalBytes[0], intervalBytes[1] };
            Write(request);
        }

        /// <summary>
        ///     See void PosInterval(int interval)
        /// </summary>
        /// <returns></returns>
        public static int PosInterval()
        {
            byte[] request = { 0x18 };
            byte[] data = Request(request, 2);

            return BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        ///     The network id.
        /// </summary>
        /// <returns></returns>
        public static int NetworkId()
        {
            byte[] request = { 0x1A };
            return BitConverter.ToInt32( Request(request, 2), 0);
        }
        
        /// <summary>
        /// set the network id
        /// </summary>
        /// <param name="networkId">bytearray with a length of 2</param>
        public static void NetworkId(byte[] networkId)
        {
            byte[] request = { 0x1A, networkId[0], networkId[1]};
            Write(request);
        }

        /// <summary>
        ///     Select the ultra-wideband transmission and reception channel. In general the transmission range increases at lower frequencies, i.e., lower channels. Allow up to 20ms to let the device switch channel. 
        ///     Warning: to enable wireless communication between two devices they must operate on the same channel.
        /// </summary>
        /// <param name="uwbChannel">
        ///     Default value: 5 
        ///     Indicate the UWB channel. Possible values:
        ///     1 : Centre frequency 3494.4MHz, using the band(MHz): 3244.8 – 3744, bandwidth 499.2 MHz 
        ///     2 : Centre frequency 3993.6MHz, using the band(MHz): 3774 – 4243.2, bandwidth 499.2 MHz
        ///     3 : Centre frequency 4492.8MHz, using the band(MHz): 4243.2 – 4742.4 bandwidth 499.2 MHz
        ///     4 : Centre frequency 3993.6MHz, using the band(MHz): 3328 – 4659.2 bandwidth 1331.2 MHz(capped to 900MHz)
        ///     5 : Centre frequency 6489.6MHz, using the band(MHz): 6240 – 6739.2 bandwidth 499.2 MHz
        ///     7 : Centre frequency 6489.6MHz, using the band(MHz): 5980.3 – 6998.9 bandwidth 1081.6 MHz(capped to 900MHz)
        /// </param>
        public static void UwbChannel(int uwbChannel)
        {
            byte[] request = { 0x1C, (byte)uwbChannel };
            Write(request);
        }

        /// <summary>
        ///     Retrieves the UwbChanel
        /// </summary>
        /// <returns>See  void UwbChannel(int uwbChannel)</returns>
        public static int UwbChannel()
        {
            byte[] request = { 0x1C };
            byte[] data = Request(request, 1);

            return data[0];
        }

    }
}
