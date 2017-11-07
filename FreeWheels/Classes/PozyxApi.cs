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

        public static List<string> CalibStatus ()
        {
            List<string> status = new List<string>();

            byte[] request = { 0x6 };
            byte[] data = Request(request, 1);

            string[] statuscodes = new string[4] { "SYS", "GYR", "ACC", "MAG" };

            for(int i = 0; i < 2 * statuscodes.Length; i = i + 2)
            {
                if((byte)((data[0] >> i) & 0x03) == 0x03){
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

        public static bool SetPosInterval(int interval)
        {
            byte[] request = { 0x18, 0x01, 0xf4 };
            byte[] data = Request(request, 2);

            if (data.Length > 0)
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool Reset()
        {
            byte[] request = { 0xB0 };
            byte[] data = Request(request, 1);

            return (data.Length > 0 && data[0] == 1);
        }

        /*
         * Configures which interrupts are enabled
         * 
         * @param err, Enables interrupts whenever an error occurs
         * @param pos, Enables interrupts whenever a new positiong update is availabe
         * @param imu, Enables interrupts whenever a new IMU update is availabe
         * @param rxData, Enables interrupts whenever data is received through the ultra-wideband network
         * @param funt,	Enables interrupts whenever a register function call has completed
         * @param pin, Configures the interup pin valid options are 0 and 1
         *
         * @return Only returns false if pin is invalid otherwise true
         */
        public static bool IntMask(bool err, bool pos, bool imu, bool rxData, bool funt, int pin)
        {
            byte parameters = 0x0;

            if (err)
            {
                parameters &= 0x1;
            }
            if (pos)
            {
                parameters &= 0x2;
            }
            if (imu)
            {
                parameters &= 0x4;
            }
            if (rxData)
            {
                parameters &= 0x8;
            }
            if (funt)
            {
                parameters &= 0x10;
            }
            if(pin > 1)
            {
                return false;
            }
            else if (pin == 1){
                parameters &= 0x80;
            }

            byte[] request = { 0x10, parameters };
            byte[] data = Request(request, 1);

            return true;
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
    }
}
