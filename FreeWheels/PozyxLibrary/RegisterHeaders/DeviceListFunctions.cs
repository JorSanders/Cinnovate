using FreeWheels.PozyxLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.RegisterHeaders
{
    public class DeviceListFunctions : RegisterHeader
    {
        public DeviceListFunctions(IConnection connection) : base(connection)
        {
        }

        /// <summary>
        ///     Get all the network IDs's of devices in the device list.
        /// </summary>
        /// <param name="offset">Offset (optional). This function will return network ID's starting from this offset in the list of network ID's. The default value is 0.</param>
        /// <param name="size">	size (optional). Number of network ID's to return, starting from the offset. The default value is (20-offset), i.e., returning the complete list. Possible values are between 1 and 20.</param>
        /// <returns>network IDs of all devices</returns>
        public int[] DevicesGetIds(int offset = 0, int size = 20, int remoteId = 0)
        {
            byte[] parameters = {(byte)offset, (byte)size };
            byte[] data = ReadRegister(0xC0, size * 2 + 1, parameters, remoteId); //2 bytes per device + 1 byte for success/failure

            int[] DeviceIds = new int[size];

            if (data[0] == 1)
            {
                for (int i = 1; i < data.Length; i += 2)
                {
                    DeviceIds[(i - 1) / 2] = BitConverter.ToUInt16(new byte[] { data[i], data[i + 1] }, 0);
                }
            }

            return DeviceIds;
        }

        /// <summary>
        ///     This function performs a discovery operation to identify other pozyx devices within radio range.
        ///     Newly discovered devices will be added to the internal device list.
        ///     This process may take several milliseconds.
        /// </summary>
        /// <param name="deviceType">
        ///     Discover options (optional). This determines which type of devices should be discovered.
        ///     The type is given by theoperation mode in POZYX_OPERATION_MODE.
        ///     Possible values:
        ///         (0x0) Anchors only(default value)
        ///         (0x1) Tags only 
        ///         (0x2) All Pozyx devices
        /// </param>
        /// <param name="idleSlots">
        ///     Number of Idle slots (optional). The number of slots to wait for a response of an undiscovered device.
        ///     If no response was received the discovery process is terminated.
        ///     The default value is 3 idle slots.
        /// </param>
        /// <param name="idleSlotDuration">
        ///     Idle slot duration (optional). The time duration in milliseconds of the idle slot.
        ///     Depending on the ultra-wideband settings a shorter or longer slot duration must be chosen.
        ///     The default value is 10ms.
        /// </param>
        /// <returns>Success</returns>
        public bool DevicesDiscover(int deviceType = 0, int idleSlots = 3, int idleSlotDuration = 10, int remoteId = 0)
        {
            byte[] parameters = { (byte)deviceType, (byte)idleSlots, (byte)idleSlotDuration };
            byte[] data = ReadRegister(0xC1, 1, parameters, remoteId); 

            return data[0] == 1;
        }

        /// <summary>
        ///     This function estimates the relative position of up to 6 pozyx devices within range.
        ///     This function can be used for quickly setting up the positioning system.
        ///     This procedure may take several hundres of milliseconds depending on the number of devices in range and the number of range measurements requested.
        ///     During the calibration proces LED 2 will turned on.
        ///     At the end of calibration the corresponding bit in the POZYX_CALIB_STATUS register will be set.
        ///     Note that only the coordinates of pozyx devices within range are determined.
        ///     The resulting coordinates are stored in the internal device list.
        ///     It is advised that during the calibration process, the pozyx tag is not used for other wireless communication.
        /// </summary>
        /// <param name="calibrationOption">
        ///     Calibration options (optional).
        ///     Possible values:
        ///         (0x02) 2D (default). The relative x and y coordinates of the anchors are estimated.It is expected that all anchors are located in the same 2D plane.
        ///         (0x01) 2.5D. The relative x and y coordinates of the anchors are estimated. However it is not expected that all anchors are located in the same 2D plane. For this option to work, the z-coordinates of the anchors must be available in the device list.
        /// </param>
        /// <param name="measurements">
        ///     Number of Measurements (optional).
        ///     This describes the number of range measurements that should be made for the estimation.Note that a larger number of measurements will increase the accuracy of the relative coordinates, but will also make the process take longer.
        ///     The default value is 10 measurments.
        /// </param>
        /// <param name="networkIds">
        ///     (Network id anchor 0)  (optional) The network id of the first anchor is given. This anchor will be used to define the origin, i.e., it's coordinates will be forced to zero.
        ///     (Network id anchor 1)  (optional) The network id of the second anchor is given. This anchor will be used to determine the x-axis, i.e., its y coordinate will be forced to zero.
        ///     (Network id anchor 2)  (optional) The network id of the third anchor is given. This anchor will be used to determine the which way is up for the y-coordinate, i.e., its y coordinate will be forced to be positive.
        ///     (Network id anchor 3+) (optional) The network id of the fourth anchor is given.
        /// </param>
        /// <returns>Success</returns>
        public bool CalibrateDevices(int calibrationOption = 0x02, int measurements = 10, int[] networkIds = null, int remoteId = 0)
        {
            networkIds = networkIds ?? new int[0];

            byte[] parameters = new byte[2 + networkIds.Length * 2];

            parameters[1] = (byte)calibrationOption;
            parameters[2] = (byte)measurements;

            for (int i = 0; i < networkIds.Length * 2; i += 2)
            {
                BitConverter.GetBytes((UInt16)networkIds[i / 2]).CopyTo(parameters, i + 2);
            }

            byte[] data = ReadRegister(0xC2, 1, parameters, remoteId);

            return data[0] == 1;
        }

        /// <summary>
        ///     Clear the list of all pozyx devices.
        /// </summary>
        /// <returns>Success</returns>
        public bool DevicesClear(int remoteId = 0)
        {
            byte[] data = ReadRegister(0xC3, 1, null, remoteId);

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
        public bool DeviceAdd(int networkID, int flag, int x, int y, int z, int remoteId = 0)
        {
            byte[] parameters = new byte[15];

            // Convert parameters to byte[] and join with byte[] request
            BitConverter.GetBytes((UInt16)networkID).CopyTo(parameters, 0);
            BitConverter.GetBytes((byte)flag).CopyTo(parameters, 2);
            BitConverter.GetBytes(x).CopyTo(parameters, 3);
            BitConverter.GetBytes(y).CopyTo(parameters, 7);
            BitConverter.GetBytes(z).CopyTo(parameters, 11);

            byte[] data = ReadRegister(0xC4, 1, parameters, remoteId);

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
        public int[] DeviceGetInfo(int networkID, int remoteId = 0)
        {
            byte[] parameters = new byte[2];
            BitConverter.GetBytes((UInt16)networkID).CopyTo(parameters, 0);

            byte[] data = ReadRegister(0xC5, 16, parameters, remoteId);

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
        public int[] DeviceGetCoords(int networkID, int remoteId = 0)
        {
            byte[] parameters = new byte[2];
            BitConverter.GetBytes((UInt16)networkID).CopyTo(parameters, 1);
            byte[] data = ReadRegister(0xC6, 13, parameters, remoteId);

            int[] DeviceCoords = new int[3];

            if (data[0] == 1)
            {
                DeviceCoords[0] = BitConverter.ToInt32(data, 1);
                DeviceCoords[1] = BitConverter.ToInt32(data, 5);
                DeviceCoords[2] = BitConverter.ToInt32(data, 9);
            }

            return DeviceCoords;
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
        public int[][] CirData(int offset, int size, int remoteId = 0)
        {
            //Offset needs to be between 0 - 1015
            //Size needs to be between 1 - 49
            offset = offset > 1015 ? 1015 : offset;
            size = size > 49 || size < 1 ? 49 : size;

            byte[] parameters = new byte[3];

            //Add parameters to request byte[]
            BitConverter.GetBytes((UInt16)offset).CopyTo(parameters, 0);
            parameters[2] = (byte)size;

            byte[] data = ReadRegister(0xC8, (size * 2 + 1), parameters, remoteId);

            int[][] CirData = new int[size * 2][];

            if (data[0] == 1)
            {
                for (int i = 1; i < data.Length; i += 4)
                {
                    // Don't ask!
                    CirData[(i - 1) / 4] = new int[2];
                    CirData[(i - 1) / 4][0] = BitConverter.ToInt32(new byte[] { data[i], data[i + 1] }, 0);
                    CirData[(i - 1) / 4][1] = BitConverter.ToInt32(new byte[] { data[i + 2], data[i + 3] }, 0);
                }
            }

            return CirData;
        }
    }
}
