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
         */ public static int GetDeviceListSize()
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
                    break;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
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

        /*******************************************************************************************************
         *      POSITIONING DATA
         * *****************************************************************************************************/

        /// <summary>
        ///     x-coordinate of the device in mm.
        /// </summary>
        /// <returns></returns>
        public static int PosX()
        {
            byte[] request = { 0x30 };
            byte[] data = Request(request, 4);

            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        ///     y-coordinate of the device in mm.
        /// </summary>
        /// <returns></returns>
        public static int PosY()
        {
            byte[] request = { 0x34 };
            byte[] data = Request(request, 4);

            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        ///     z-coordinate of the device in mm.
        /// </summary>
        /// <returns></returns>
        public static int PosZ()
        {
            byte[] request = { 0x38 };
            byte[] data = Request(request, 4);

            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        ///     estimated error covariance of x
        /// </summary>
        /// <returns></returns>
        public static int PosErrX()
        {
            byte[] request = { 0x3C };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     estimated error covariance of y
        /// </summary>
        /// <returns></returns>
        public static int PosErrY()
        {
            byte[] request = { 0x3E };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     estimated error covariance of z
        /// </summary>
        /// <returns></returns>
        public static int PosErrZ()
        {
            byte[] request = { 0x40 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     estimated covariance of xy
        /// </summary>
        /// <returns></returns>
        public static int PosErrXY()
        {
            byte[] request = { 0x42 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        /// 	estimated covariance of xz
        /// </summary>
        /// <returns></returns>
        public static int PosErrXZ()
        {
            byte[] request = { 0x44 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        /// 	estimated covariance of YZ
        /// </summary>
        /// <returns></returns>
        public static int PosErrYZ()
        {
            byte[] request = { 0x46 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Calling this function resets the Pozyx device.
        ///     This also clears the device list and returns the settings to their defualt state (including UWB settings)
        /// </summary>
        /// <returns></returns>
        public static bool Reset()
        {
            byte[] request = { 0xB0 };
            byte[] data = Request(request, 1);

            return (data.Length > 0 && data[0] == 1);
        }

        /***********************************************************************************************
         *      CONFIGURATION REGISTERS
         ***********************************************************************************************/

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
            int[] options = { mode, act, latch };

            for (int i = 0; i < options.Count(); i++)
            {
                if (options[i] == 1)
                {
                    parameters = (byte)(0x1 << (byte)i + 3 | parameters);
                }
            }

            byte[] request = { 0x11, parameters };
            Write(request);
        }

        // See IntConfig(int pinNum, int mode, int act, int latch)
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
            parameters |= strengthByte;

            byte[] request = { 0x14, parameters };
            Write(request);
        }

        // See PosFilter(int strength, int filter)
        public static int[] PosFilter()
        {
            byte[] request = { 0x14 };
            byte[] data = Request(request, 1);

            return new int[] { data[0] & 0xF0, data[0] & 0x0F };
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

        // See ConfigLeds(bool led1, bool led2, bool led3, bool led4, bool ledRx, bool ledTx)
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
        /// <param name="algorithm">
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

        // See PosAlg(int algorithm, int dim)
        public static int[] PosAlg()
        {
            byte[] request = { 0x16 };
            byte[] data = Request(request, 1);

            return new int[] { data[0] & 0xF, data[0] >> 4 };
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

        // PosNumAnchors(int num, int mode)
        public static int[] PosNumAnchors()
        {
            byte[] request = { 0x17 };
            byte[] data = Request(request, 1);

            return new int[] { data[0] & 0xF, data[0] >> 7 };
        }

        /// <summary>
        ///     Pozyx can be run in continuous mode to provide continuous positioning. 
        ///     The interval in milliseconds between successive updates can be configured with this register. 
        /// </summary>
        /// <param name="interval">
        ///     The value is capped between 10ms and 60000ms (1 minute). 
        ///     Writing the value 0 to this registers disables the continuous mode.
        /// </param>
        public static void PosInterval(int interval)
        {
            byte[] intervalBytes = BitConverter.GetBytes(interval);

            byte[] request = { 0x18, intervalBytes[0], intervalBytes[1] };
            Write(request);
        }

        // See void PosInterval(int interval)
        public static int PosInterval()
        {
            byte[] request = { 0x18 };
            byte[] data = Request(request, 2);

            return BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        ///     set the network id
        /// </summary>
        /// <param name="networkID"></param>
        public static void NetworkID(int networkID)
        {
            byte[] request = new byte[3];
            request[0] = 0x1A;
            BitConverter.GetBytes((UInt16)networkID).CopyTo(request, 1);

            Write(request);
        }

        // See NetworkId(NetworkID(int networkID))
        public static int NetworkID()
        {
            byte[] request = { 0x1A };
            return BitConverter.ToInt32(Request(request, 2), 0);
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


        // See UwbChannel(int uwbChannel)
        public static int UwbChannel()
        {
            byte[] request = { 0x1C };
            byte[] data = Request(request, 1);

            return data[0];
        }

        /// <summary>
        ///     	Indicates which interrupts are enabled.    
        /// </summary>
        /// <param name="err">Enables interrupts whenever an error occurs.</param>
        /// <param name="pos">Enables interrupts whenever a new positiong update is availabe.</param>
        /// <param name="imu">Enables interrupts whenever a new IMU update is availabe.</param>
        /// <param name="rxData">Enables interrupts whenever data is received through the ultra-wideband network.</param>
        /// <param name="funt">Enables interrupts whenever a register function call has completed.</param>
        /// <param name="pin">Configures the interrupt pin.</param>
        public static void IntMask(bool err, bool pos, bool imu, bool rxData, bool funt, int pin)
        {
            byte parameters = 0x0;
            bool[] interupts = { err, pos, imu, rxData, funt };

            for (int i = 0; i < interupts.Count(); i++)
            {
                if (interupts[i])
                {
                    parameters = (byte)(0x1 << i | parameters);
                }
            }
            parameters = (byte)(pin << 7 | parameters);

            byte[] request = { 0x10, parameters };
            Write(request);
        }

        // See IntMask(bool err, bool pos, bool imu, bool rxData, bool funt, int pin)
        public static List<string> IntMask()
        {
            byte[] request = { 0x10 };
            byte[] data = Request(request, 1);

            List<string> interupts = new List<string>();
            interupts.Add(((0x80 & data[0]) == 1) ? "PIN1" : "PIN0");

            string[] interuptFlags = { "ERR", "POS", "IMU", "RXDATA", "FUNC" };

            for (int i = 0; i < interuptFlags.Length; i++)
            {
                if ((data[0] >> i & 0x1) == 1)
                {
                    interupts.Add(interuptFlags[i]);
                }
            }

            return interupts;
        }

        /// <summary>
        ///     This register describes the UWB bitrate and nominal pulse repition frequency (PRF).
        /// </summary>
        /// <param name="bitrate">
        ///     Indicate the UWB bitrate. Possible values:
        ///     0 : bitrate 110 kbits/s(Default value)
        ///     1 : bitrate 850 kbits/s
        ///     2 : bitrate 6.8 Mbits/s</param>
        /// <param name="prf">
        ///     Indicates the pulse repetition frequency to be used. Possible values
        ///     1 : 16 MHz 
        ///     2 : 64 MHz (default value)
        /// </param>
        public static void UwbRates(int bitrate, int prf)
        {
            byte parameters = (byte)prf;
            parameters |= (byte)(prf << 6);

            byte[] request = { 0x1D, parameters };
            Write(request);
        }

        // See UwbRates(int bitrate, int prf)
        public static int[] UwbRates()
        {
            byte[] request = { 0x1D };
            byte[] data = Request(request, 1);

            return new int[] { data[0] & 0x3F, data[0] >> 6 };
        }

        /// <summary>
        ///     This register describes the preamble length of the UWB wireless packets.
        /// </summary>
        /// <param name="plen">
        ///     Indicate the UWB preamble length. Possible values:
        ///     12 : 4096 symbols.Standard preamble length 4096 symbols
        ///     40 : 2048 symbols.Non-standard preamble length 2048 symbols
        ///     24 : 1536 symbols.Non-standard preamble length 1536 symbols
        ///     8  : 1024 symbols.Standard preamble length 1024 symbols(default value)
        ///     52 : 512 symbols.Non-standard preamble length 512 symbols
        ///     36 : 256 symbols.Non-standard preamble length 256 symbols
        ///     20 : 128 symbols.Non-standard preamble length 128 symbols
        ///     4  : 64 symbols.Standard preamble length 64 symbols
        /// </param>
        public static void UwbPlen(int plen)
        {
            byte[] request = { 0x1E, (byte)plen };
            Write(request);
        }

        // See UwbPlen(int plen)
        public static int UwbPlen()
        {
            byte[] request = { 0x1E };
            byte[] data = Request(request, 1);

            return data[0];
        }

        /// <summary>
        ///     Configure the power gain for the UWB transmitter
        ///     Warning: when changing channel, bitrate or preamble length, the power is also overwritten to the default value for this UWB configuration.
        ///     Warning: changing this value can make the Pozyx device fall out of regulation.
        /// </summary>
        /// <param name="gain">
        ///     Possible values are between 0 and 67. 1dB = 2 int.
        /// </param>
        public static void UwbGain(int gain)
        {
            byte[] request = { 0x1F, (byte)gain };
            Write(request);
        }

        // See UwbGain(int gain)
        public static int UwbGain()
        {
            byte[] request = { 0x1F };
            byte[] data = Request(request, 1);

            return data[0];
        }

        /// <summary>
        ///     This register contains the trimming value to fine-tune the operating frequency of the crystal oscillator used by the ultra-wideband front-end. 
        ///     By carefully selecting this value, the operating frequency can be tuned with an error of 1ppm. 
        ///     A smaller error on the operating frequency will increase the sensitivity of the UWB receiver. 
        /// </summary>
        /// <param name="xTalTrim"></param>
        public static void XTalTrim(int xTalTrim)
        {
            byte[] request = { 0x20, (byte)xTalTrim };
            Write(request);
        }

        // See XTalTrim(int xTalTrim)
        public static int XTalTrim()
        {
            byte[] request = { 0x20 };
            byte[] data = Request(request, 1);

            return data[0];
        }

        /// <summary>
        ///     This register determines how the ranging measurements are made. 
        /// </summary>
        /// <param name="rangeProtocol">
        ///     0: PRECISION (Default value)
        ///     1: FAST
        /// </param>
        public static void RangeProtocol(int rangeProtocol)
        {
            byte[] request = { 0x21, (byte)rangeProtocol };
            Write(request);
        }

        // See RangeProtocol(int rangeProtocol)
        public static int RangeProtocol()
        {
            byte[] request = { 0x21 };
            byte[] data = Request(request, 1);

            return data[0];
        }

        /// <summary>
        ///     Configure the mode of operation of the pozyx device
        /// </summary>
        /// <param name="operationMode">
        ///     0 : Tag mode. In tag mode, the device can more around. In this mode the device cannot be used by other devices for positioning.
        ///     1 : Anchor mode.In anchor mode the device is assumed to be immobile. The device can be used by other devices for positioning.
        /// </param>
        public static void OperationMode(int operationMode)
        {
            byte[] request = { 0x22, (byte)operationMode };
            Write(request);
        }

        // See OperationMode(int operationMode)
        public static int OperationMode()
        {
            byte[] request = { 0x22 };
            byte[] data = Request(request, 1);

            return data[0];
        }

        /// <summary>
        ///     Configure the mode of operation of the sensors
        /// </summary>
        /// <param name="sensorMode">
        ///     Non-fusion modes:
        ///         0 : MODE_OFF
        ///         1 : ACCONLY 
        ///         2 : MAGONLY 
        ///         3 : GYROONLY 
        ///         4 : ACCMAGx
        ///         5 : ACCGYRO 
        ///         6 : MAGGYRO 
        ///         7 : AMG
        ///     Fusion modes:
        ///         8 : IMU 
        ///         9 : COMPASS 
        ///         10 : M4G 
        ///         11 : NDOF_FMC_OFF
        ///         12 : NDOF
        /// </param>
        public static void SensorsMode(int sensorMode)
        {
            byte[] request = { 0x23, (byte)sensorMode };
            Write(request);
        }

        // See SensorsMode(int sensorMode)
        public static int SensorsMode()
        {
            byte[] request = { 0x23 };
            byte[] data = Request(request, 1);

            return data[0];
        }

        /// <summary>
        ///     Configure GPIO pin 1.
        /// </summary>
        /// <param name="mode">
        ///     Indicates the input or output mode of the pin
        ///     0 : digital input
        ///     1 : digital output(push-pull)
        ///     2 : digital output(open-drain)
        /// </param>
        /// <param name="pull">
        ///     When selecting input or open-drain output, the pin can be internally connected with a pull-up (to 0V) or pull-down (to 3.3V) resistor. 
        ///     0 : no pull-up or pull-down resistor.
        ///     1 : pull-up resistor. 
        ///     2 : pull-down resistor
        /// </param>
        public static void ConfigGpio1(int mode, int pull)
        {
            byte parameters = (byte)mode;
            parameters |= (byte)(pull >> 3);

            byte[] request = { 0x27, parameters };
            Write(request);
        }

        // See ConfigGpio1(int mode, int pull)
        public static int[] ConfigGpio1()
        {
            byte[] request = { 0x27  };
            byte[] data = Request(request, 1);

            return new int[] { data[0] & 0x7, data[0] >> 3};
        }

        public static void ConfigGpio2(int mode, int pull)
        {
            byte parameters = (byte)mode;
            parameters |= (byte)(pull >> 3);

            byte[] request = { 0x28, parameters };
            Write(request);
        }

        public static int[] ConfigGpio2()
        {
            byte[] request = { 0x28 };
            byte[] data = Request(request, 1);

            return new int[] { data[0] & 0x7, data[0] >> 3 };
        }

        public static void ConfigGpio3(int mode, int pull)
        {
            byte parameters = (byte)mode;
            parameters |= (byte)(pull >> 3);

            byte[] request = { 0x29, parameters };
            Write(request);
        }

        public static int[] ConfigGpio3()
        {
            byte[] request = { 0x29 };
            byte[] data = Request(request, 1);

            return new int[] { data[0] & 0x7, data[0] >> 3 };
        }

        public static void ConfigGpio4(int mode, int pull)
        {
            byte parameters = (byte)mode;
            parameters |= (byte)(pull >> 3);

            byte[] request = { 0x2A, parameters };
            Write(request);
        }

        public static int[] ConfigGpio4()
        {
            byte[] request = { 0x2A };
            byte[] data = Request(request, 1);

            return new int[] { data[0] & 0x7, data[0] >> 3 };
        }

        /***************************************************************************************************
         *      DEVICE LIST FUNCTIONS
         * *************************************************************************************************/

        /// <summary>
        ///     Get all the network IDs's of devices in the device list.
        /// </summary>
        /// <param name="offset">Offset (optional). This function will return network ID's starting from this offset in the list of network ID's. The default value is 0.</param>
        /// <param name="size">	size (optional). Number of network ID's to return, starting from the offset. The default value is (20-offset), i.e., returning the complete list. Possible values are between 1 and 20.</param>
        /// <returns>network IDs of all devices</returns>
        public static int[] DevicesGetIds(int offset = 0, int size = 20)
        {
            byte[] request = { 0xC0, (byte)offset, (byte)size };
            byte[] data = Request(request, size * 2 + 1); //2 bytes per device + 1 byte for success/failure

            int[] DeviceIds = new int[size];

            if(data[0] == 1)
            {
                for (int i = 1; i < data.Length; i += 2)
                {
                    DeviceIds[(i - 1) / 2] = BitConverter.ToUInt16(new byte[] { data[i], data[i + 1] }, 0);
                }
            }

            return DeviceIds;
        }

        /// <summary>
        ///     Clear the list of all pozyx devices.
        /// </summary>
        /// <returns>Success</returns>
        public static bool DevicesClear()
        {
            byte[] request = { 0xC3 };
            byte[] data = Request(request, 1);

            return data[0] == 1;
        }

        /// <summary>
        ///     This function adds a device to the internal list of devices.
        ///     When the device is already present in the device list, the values will be overwritten.
        /// </summary>
        /// <param name="networkID">Network address of the device</param>
        /// <param name="flag">Special flag describing the device</param>
        /// <param name="x">x-coordinate of the device</param>
        /// <param name="y">y-coordinate of the device</param>
        /// <param name="z">z-coordinate of the device</param>
        /// <returns>Success</returns>
        public static bool DeviceAdd(int networkID, int flag, int x, int y, int z)
        {
            byte[] request = new byte[15];
            request[0] = 0xC4;

            // Convert parameters to byte[] and join with byte[] request
            BitConverter.GetBytes((UInt16)networkID).CopyTo(request, 1);
            BitConverter.GetBytes((byte)flag).CopyTo(request, 3);
            BitConverter.GetBytes(x).CopyTo(request, 4);
            BitConverter.GetBytes(y).CopyTo(request, 8);
            BitConverter.GetBytes(z).CopyTo(request, 12);

            byte[] data = Request(request, 1);

            return data[0] == 1;
        }

        /// <summary>
        ///     Get the stored device information for a given pozyx device.
        /// </summary>
        /// <param name="networkID">Network address of the device</param>
        /// <returns>
        ///     (0) NetworkID
        ///     (1) Flag
        ///     (2) x-coördianate
        ///     (3) y-coördianate
        ///     (4) z-coördianate
        /// </returns>
        public static int[] DeviceGetInfo(int networkID)
        {
            byte[] request = new byte[3];
            request[0] = 0xC5;
            BitConverter.GetBytes((UInt16)networkID).CopyTo(request, 1);

            byte[] data = Request(request, 16);

            int[] DeviceInfo = new int[5];

            if (data[0] == 1)
            {
                DeviceInfo[0] = BitConverter.ToUInt16(new byte[] { data[1], data[2] }, 0);
                DeviceInfo[1] = (int)data[3];
                DeviceInfo[2] = BitConverter.ToInt32(new byte[] { data[4], data[5], data[6], data[7] }, 0);
                DeviceInfo[3] = BitConverter.ToInt32(new byte[] { data[8], data[9], data[10], data[11] }, 0);
                DeviceInfo[4] = BitConverter.ToInt32(new byte[] { data[12], data[13], data[14], data[15] }, 0);
            }

            return DeviceInfo;
        }

        /// <summary>
        ///     This function returns the coordinates of a given pozyx device as they are stored in the internal device list.
        ///     The coordinates are either inputted by the user using DeviceAdd() or obtained automatically with DevicesCalibrate(). 
        /// </summary>
        /// <param name="networkID">Network address of the device</param>
        /// <returns>
        ///     (0) x-coördinate
        ///     (1) y-coördinate
        ///     (2) z-coördinate
        /// </returns>
        public static int[] DeviceGetCoords(int networkID)
        {
            byte[] request = new byte[3];
            request[0] = 0xC5;
            BitConverter.GetBytes((UInt16)networkID).CopyTo(request, 1);

            byte[] data = Request(request, 12);

            int[] DeviceInfo = new int[3];

            if (data[0] == 1)
            {
                DeviceInfo[0] = BitConverter.ToInt32(new byte[] { data[1], data[2], data[3], data[4] }, 0);
                DeviceInfo[1] = BitConverter.ToInt32(new byte[] { data[5], data[6], data[7], data[8] }, 0);
                DeviceInfo[2] = BitConverter.ToInt32(new byte[] { data[9], data[10], data[11], data[12] }, 0);
            }

            return DeviceInfo;
        }

        /// <summary>
        ///     This function returns the channel impulse response (CIR) of the last received ultra-wideband message.
        ///     The CIR can be used for diagnostic purposes, or to run custom timing algorithms.
        ///     Using the default PRF of 64MHz, a total of 1016 complex coefficients are available.
        ///     For a PRF of 16MHz, 996 complex coefficients are available.
        ///     Every complex coefficient is represented by 4 bytes (2 for the real part and 2 for the imaginary part).
        ///     The coefficients are taken at an interval of 1.0016ns, or more precisely, at half the period of a 499.2MHz sampling frequency.
        /// </summary>
        /// <param name="offset">CIR buffer offset. This value indicates where the offset inside the CIR buffer to start reading the the data bytes. Possible values range between 0 and 1015.</param>
        /// <param name="size">Data length. The number of coefficients to read from the CIR buffer. Possible values range between 1 and 49.</param>
        /// <returns>
        ///     (0) CIR coefficient 0+offset (real value).
        ///     (1) CIR coefficient 0+offset (imaginary value).
        /// </returns>
        public static int[][] CirData(int offset, int size)
        {
            //Offset needs to be between 0 - 1015
            //Size needs to be between 1 - 49
            offset = offset > 1015 ? 1015 : offset;
            size = size > 49 || size < 1  ? 49 : size;

            byte[] request = new byte[4];
            request[0] = 0xC8;

            //Add parameters to request byte[]
            BitConverter.GetBytes((UInt16)offset).CopyTo(request, 1);
            request[3] = (byte)size;

            byte[] data = Request(request, (size * 2 + 1));

            int[][] CirData = new int[size*2][];

            if (data[0] == 1)
            {
                for(int i = 1; i<data.Length; i+=4)
                {
                    // Don't ask!
                    CirData[(i - 1) / 4] = new int[2];
                    CirData[(i - 1) / 4][0] = BitConverter.ToInt32(new byte[] { data[i], data[i+1] }, 0);
                    CirData[(i - 1) / 4][1] = BitConverter.ToInt32(new byte[] { data[i+2], data[i+3] }, 0);
                }
            }

            return CirData;
        }
         /***********************************************************************************************
         *      SENSOR DATA
         * *********************************************************************************************/

        /// <summary>
        ///     This register contains the maximum measured norm of the 3D linear acceleration.
        ///     This value is reset after reading the register.
        ///     The sensor data is represented as an unsigned 16-bit integer.
        ///     1mg = 1 int.
        /// </summary>
        /// <returns>Maximum linear acceleration</returns>
        public static int MaxLinAcc()
        {
            byte[] request = { 0x4E };
            byte[] data = Request(request, 2);

            return BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        ///     This register holds the pressure exerted on the pozyx device.
        ///     At sealevel the pressure is The pressure is stored as an unsigned 32-bit integer.
        ///     1mPa = 1 int.
        /// </summary>
        /// <returns>Pressure data</returns>
        public static UInt32 Pressure()
        {
            byte[] request = { 0x50 };
            byte[] data = Request(request, 4);

            return BitConverter.ToUInt32(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated acceleration in the x-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Accelerometer data (in mg)</returns>
        public static int AccelX()
        {
            byte[] request = { 0x54 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated acceleration in the y-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Accelerometer data (in mg)</returns>
        public static int AccelY()
        {
            byte[] request = { 0x56 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated acceleration in the z-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Accelerometer data (in mg)</returns>
        public static int AccelZ()
        {
            byte[] request = { 0x58 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the magnetic field strength along the x-axis of the Pozyx device.
        ///     The sensor data is stored as a signed 16-bit integer.
        ///     1µT = 16 int.
        /// </summary>
        /// <returns>Magnemtometer data</returns>
        public static int MagnX()
        {
            byte[] request = { 0x5A };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the magnetic field strength along the y-axis of the Pozyx device.
        ///     The sensor data is stored as a signed 16-bit integer.
        ///     1µT = 16 int.
        /// </summary>
        /// <returns>Magnemtometer data</returns>
        public static int MagnY()
        {
            byte[] request = { 0x5C };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the magnetic field strength along the z-axis of the Pozyx device.
        ///     The sensor data is stored as a signed 16-bit integer.
        ///     1µT = 16 int.
        /// </summary>
        /// <returns>Magnemtometer data</returns>
        public static int MagnZ()
        {
            byte[] request = { 0x5E };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated angular velocity for the x-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Gyroscope data</returns>
        public static int GyroX()
        {
            byte[] request = { 0x60 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated angular velocity for the y-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Gyroscope data</returns>
        public static int GyroY()
        {
            byte[] request = { 0x62 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated angular velocity for the z-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Gyroscope data</returns>
        public static int GyroZ()
        {
            byte[] request = { 0x64 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the absolute heading or yaw of the device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Euler angles heading (or yaw)</returns>
        public static int EulHeading()
        {
            byte[] request = { 0x66 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the roll of the device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Euler angles roll</returns>
        public static int EulRoll()
        {
            byte[] request = { 0x68 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the pitch of the device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Euler angles pitch</returns>
        public static int EulPitch()
        {
            byte[] request = { 0x6A };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the weight w and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>Weight of quaternion</returns>
        public static int QuatW()
        {
            byte[] request = { 0x6C };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the x and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>x of quaternion</returns>
        public static int QuatX()
        {
            byte[] request = { 0x6E };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the y and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>y of quaternion</returns>
        public static int QuatY()
        {
            byte[] request = { 0x70 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the z and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>z of quaternion</returns>
        public static int QuatZ()
        {
            byte[] request = { 0x72 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The linear acceleration is the total acceleration minus the gravity.
        ///     The linear acceleration expressed the acceleration due to movement.
        ///     This register holds the linear acceleration along the x-axis of the pozyx device (i.e. body coordinates).
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Linear acceleration in x-direction</returns>
        public static int LiaX()
        {
            byte[] request = { 0x74 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The linear acceleration is the total acceleration minus the gravity.
        ///     The linear acceleration expressed the acceleration due to movement.
        ///     This register holds the linear acceleration along the y-axis of the pozyx device (i.e. body coordinates).
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Linear acceleration in y-direction</returns>
        public static int LiaY()
        {
            byte[] request = { 0x76 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The linear acceleration is the total acceleration minus the gravity.
        ///     The linear acceleration expressed the acceleration due to movement.
        ///     This register holds the linear acceleration along the z-axis of the pozyx device (i.e. body coordinates).
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Linear acceleration in z-direction</returns>
        public static int LiaZ()
        {
            byte[] request = { 0x78 };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Gravity is a force of 1g( = 9.80665 m/s2 ) directed towards the ground.
        ///     This register represents the gravity component in the x-axis and is represented by a singed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>x-component of gravity vector</returns>
        public static int GravX()
        {
            byte[] request = { 0x7A };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Gravity is a force of 1g( = 9.80665 m/s2 ) directed towards the ground.
        ///     This register represents the gravity component in the y-axis and is represented by a singed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>y-component of gravity vector</returns>
        public static int GravY()
        {
            byte[] request = { 0x7C };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Gravity is a force of 1g( = 9.80665 m/s2 ) directed towards the ground.
        ///     This register represents the gravity component in the z-axis and is represented by a singed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>z-component of gravity vector</returns>
        public static int GravZ()
        {
            byte[] request = { 0x7E };
            byte[] data = Request(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Read out the internal chip temperature.
        ///     This is loosely related to the ambient room temperature.
        ///     For more accurate ambient temperature measurements, it is recommended to use a separate sensor.
        /// </summary>
        /// <returns>Temperature</returns>
        public static int Temperature()
        {
            byte[] request = { 0x80 };
            byte[] data = Request(request, 1);

            return data[0];
        }
        
        /// <summary>
        ///     Places the network id of device A in POZYX_RX_NETWORK_ID
        /// </summary>
        /// <returns>Network id of the latest received message</returns>
        public static int RxNetworkId()
        {
            byte[] request = { 0x82 };
            byte[] data = Request(request, 2);

            byte[] rxNetworkId = { data[0], data[1] };
            
            return BitConverter.ToUInt16(rxNetworkId, 0);
        }

        /// <summary>
        ///     Places the length of the received data in the register
        /// </summary>
        /// <returns>The length of the latest received message</returns>
        public static int RxDataLen()
        {
            byte[] request = { 0x84 };
            byte[] data = Request(request, 1);

            return data[0];
        }

        /// <summary>
        ///     This register can be read from to obtain the current state of the GPIO pin if it is configured as an input. 
        ///     When the pin is configured as an output, the value written will determine the new state of the pin.
        ///     Possible values:
        ///     The digital state of the pin is LOW at 0V.
        ///     The digital state of the pin is HIGH at 3.3V.
        ///     Default value: 0
        /// </summary>
        /// 
        /// <returns>Value of the GPIO pin 1</returns>
        public static int Gpio1()
        {
            byte[] request = { 0x85 };
            byte[] data = Request(request, 1);

            return data[0];
        }

        /// <summary>
        ///     This register can be read from to obtain the current state of the GPIO pin if it is configured as an input. 
        ///     When the pin is configured as an output, the value written will determine the new state of the pin.
        ///     Possible values:
        ///     The digital state of the pin is LOW at 0V.
        ///     The digital state of the pin is HIGH at 3.3V.
        ///     Default value: 0
        /// </summary>
        /// <returns>Value of the GPIO pin 2</returns>
        public static int Gpio2()
        {
            byte[] request = { 0x86 };
            byte[] data = Request(request, 1);

            return data[0];
        }

        /// <summary>
        ///     This register can be read from to obtain the current state of the GPIO pin if it is configured as an input. 
        ///     When the pin is configured as an output, the value written will determine the new state of the pin.
        ///     Possible values:
        ///     The digital state of the pin is LOW at 0V.
        ///     The digital state of the pin is HIGH at 3.3V.
        ///     Default value: 0
        /// </summary>
        /// <returns>Value of the GPIO pin 3</returns>
        public static int Gpio3()
        {
            byte[] request = { 0x87 };
            byte[] data = Request(request, 1);

            return data[0];
        }

        /// <summary>
        ///     This register can be read from to obtain the current state of the GPIO pin if it is configured as an input. 
        ///     When the pin is configured as an output, the value written will determine the new state of the pin.
        ///     Possible values:
        ///     The digital state of the pin is LOW at 0V.
        ///     The digital state of the pin is HIGH at 3.3V.
        ///     Default value: 0
        /// </summary>
        /// <returns>Value of the GPIO pin 4</returns>
        public static int Gpio4()
        {
            byte[] request = { 0x88 };
            byte[] data = Request(request, 1);

            return data[0];
        }
    }
}
