using FreeWheels.PozyxLibrary.Interfaces;
using FreeWheels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.RegisterHeaders
{
    public class StatusRegisters : RegisterHeader
    {
        public StatusRegisters(IConnection connection) : base(connection)
        {
        }

        /// <summary>
        ///     This register identifies the Pozyx device. This can be used to make sure that Pozyx is connected properly.
        /// </summary>
        /// <returns>Returns the constant value 0x43</returns>
        public int WhoAmI(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x0, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     The value of this register describes the firmware version installed on the devices.
        ///     It is recommended to have all devices run on the same firmware version. 
        /// </summary>
        /// <returns>Firmware Version</returns>
        public string FirmwareVer(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x1, 1, null, remoteId);

            UInt16 minor = (byte)(data[0] & 0x0f);
            UInt16 major = (byte)(data[0] >> 4);

            return major + "." + minor;
        }

        /// <summary>
        ///     The value of this register describes the hardware type and version.
        ///     The value is programmed during production and cannot be changed.
        /// </summary>
        /// <returns>Hardware Version</returns>
        public string HarwareVer(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x2, 1, null, remoteId);

            UInt16 version = (byte)(data[0] & 0x1f);
            UInt16 type = (byte)(data[0] >> 5);

            return type + "." + version;
        }

        /// <summary>
        ///     This register shows the results of the internal selftest.
        ///     The self test is automatically initiated at device startup.
        /// </summary>
        /// <returns>Self-test Result</returns>
        public List<string> STResult(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x3, 1, null, remoteId);

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

        /// <summary>
        ///     This register hold the error code whenever an error occurs on the pozyx platform.
        ///     The presence of an error is indicated by the ERR-bit in the IntStatus() register. 
        /// </summary>
        /// <returns>Describes a possible system error</returns>
        public string ErrorCode(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x4, 1, null, remoteId);

            //convert hex to string
            string hexResult = data[0].ToString("X2");

            //Gets the variable name that is assigned to the result of the error
            string stringValue = Enum.GetName(typeof(PozyxErrorCode), data[0]);

            return "0x" + hexResult + " - " + stringValue;

        }

        /// <summary>
        ///     Indicates the source of the interrupt
        /// </summary>
        /// <returns>
        ///     (ERR)       Indicates that an has error occured.
        ///     (POS)       Indicates that a new position estimate is available.
        ///     (IMU)       Indicates that a new IMU measurement is available.
        ///     (RX_DATA)   Indicates that the pozyx device has received some data over its wireless uwb link.
        ///     (FUNC)      Indicates that a register function call has finished(excluding positioning).
        /// </returns>
        public List<string> IntStatus(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x5, 1, null, remoteId);

            List<string> status = new List<string>();

            string[] statuscodes = new string[] {
                "ERR: An has error occured",
                "POS: A new position estimate is available",
                "IMU: A new IMU measurement is available",
                "RX_DATA: The pozyx device has received some data over its wireless uwb link",
                "FUNC: A register function call has finished (excluding positioning)"
            };

            for (int i = 0; i < statuscodes.Length; i++)
            {
                byte shifted = (byte)(data[0] >> (byte)i);
                if ((int)(shifted & 0x1) == 1)

                    status.Add(statuscodes[i]);
            }

            return status;
        }

        /// <summary>
        ///     Part of the calibration of the motion sensors occurs in the background when the system is running.
        ///     For this calibration, each sensor requires its own typical device movements to become fully calibrated.
        ///     This register contains information about the calibration status of the motion sensors.
        /// </summary>
        /// <returns>
        ///     (SYS) Current system calibration status, depends on status of all sensors.
        ///     (GYR) Current gyroscope calibration status, depends on status of the gyroscope.
        ///     (ACC) Current accelerometer calibration status, depends on status of the accelerometer.
        ///     (MAG) Current magnetometer calibration status, depends on status of the magnetometer.
        /// </returns>
        public List<string> CalibStatus(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x6, 1, null, remoteId);

            List<string> status = new List<string>();

            string[] statuscodes = new string[] { "MAG", "ACC", "GYR", "SYS" };

            for (int i = 0; i < 2 * statuscodes.Length; i = i + 2)
            {
                if ((byte)((data[0] >> i) & 0x03) == 0x03)
                {
                    status.Add(statuscodes[i / 2]);
                }
            }

            return status;
        }
    }
}
